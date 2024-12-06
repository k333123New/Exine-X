using System.Collections.Concurrent;
using System.Net.Sockets;
using Server.ExineDatabase;
using Server.ExineEnvir;
using Server.ExineObjects;


using System.Text.RegularExpressions;
using Server.Utils;

namespace Server.ExineNetwork
{
    /// <summary>
    /// OnRecvPacketHandler
    /// </summary>
    public enum GameStage { None, Login, Select, Game, Observer, Disconnected }

    public class ExineConnection
    {
        protected static Envir Envir
        {
            get { return Envir.Main; }
        }

        protected static MessageQueue MessageQueue
        {
            get { return MessageQueue.Instance; }
        }

        public readonly int SessionID;
        public readonly string IPAddress;

        public GameStage Stage;

        private TcpClient _client;
        private ConcurrentQueue<Packet> _receiveList;
        private ConcurrentQueue<Packet> _sendList; 
        private Queue<Packet> _retryList;

        private bool _disconnecting;
        public bool Connected;
        public bool Disconnecting
        {
            get { return _disconnecting; }
            set
            {
                if (_disconnecting == value) return;
                _disconnecting = value;
                TimeOutTime = Envir.Time + 500;
            }
        }
        public readonly long TimeConnected;
        public long TimeDisconnected, TimeOutTime;

        byte[] _rawData = new byte[0];
        byte[] _rawBytes = new byte[8 * 1024];

        public AccountInfo Account;
        public PlayerObjectSrv Player;

        public List<ExineConnection> Observers = new List<ExineConnection>();
        public ExineConnection Observing;

        public List<ItemInfo> SentItemInfo = new List<ItemInfo>();
        public List<QuestInfo> SentQuestInfo = new List<QuestInfo>();
        public List<RecipeInfo> SentRecipeInfo = new List<RecipeInfo>();
        public List<UserItem> SentChatItem = new List<UserItem>(); //TODO - Add Expiry time
        public List<MapInfo> SentMapInfo = new List<MapInfo>();
        public List<ulong> SentHeroInfo = new List<ulong>();
        public bool WorldMapSetupSent;
        public bool StorageSent;
        public bool HeroStorageSent;
        public Dictionary<long, DateTime> SentRankings = new Dictionary<long, DateTime>();

        private DateTime _dataCounterReset;
        private int _dataCounter;
        private FixedSizedQueue<Packet> _lastPackets;

        public ExineConnection(int sessionID, TcpClient client)
        {
            SessionID = sessionID;
            IPAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];

            Envir.UpdateIPBlock(IPAddress, TimeSpan.FromSeconds(Settings.IPBlockSeconds));

            MessageQueue.SendMsg(IPAddress + ", Connected.");

            _client = client;
            _client.NoDelay = true;

            TimeConnected = Envir.Time;
            TimeOutTime = TimeConnected + Settings.TimeOut;

            _lastPackets = new FixedSizedQueue<Packet>(10);

            _receiveList = new ConcurrentQueue<Packet>();
            _sendList = new ConcurrentQueue<Packet>();
            _sendList.Enqueue(new ServerPacket.Connected());
            _retryList = new Queue<Packet>();

            Connected = true;
            BeginReceive();
        }

        public void AddObserver(ExineConnection exineConnection)
        {
            Observers.Add(exineConnection);

            if (exineConnection.Observing != null)
                exineConnection.Observing.Observers.Remove(exineConnection);
            exineConnection.Observing = this;

            exineConnection.Stage = GameStage.Observer;
        }

        private void BeginReceive()
        {
            if (!Connected) return;

            try
            {
                _client.Client.BeginReceive(_rawBytes, 0, _rawBytes.Length, SocketFlags.None, ReceiveData, _rawBytes);
            }
            catch
            {
                Disconnecting = true;
            }
        }
        private void ReceiveData(IAsyncResult result)
        {
            if (!Connected) return;

            int dataRead;

            try
            {
                dataRead = _client.Client.EndReceive(result);
            }
            catch
            {
                Disconnecting = true;
                return;
            }

            if (dataRead == 0)
            {
                Disconnecting = true;
                return;
            }

            if (_dataCounterReset < Envir.Now)
            {
                _dataCounterReset = Envir.Now.AddSeconds(5);
                _dataCounter = 0;
            }

            _dataCounter++;

            try
            {
                byte[] rawBytes = result.AsyncState as byte[];

                byte[] temp = _rawData;
                _rawData = new byte[dataRead + temp.Length];
                Buffer.BlockCopy(temp, 0, _rawData, 0, temp.Length);
                Buffer.BlockCopy(rawBytes, 0, _rawData, temp.Length, dataRead);

                Packet p;

                while ((p = Packet.ReceivePacket(_rawData, out _rawData)) != null)
                    _receiveList.Enqueue(p);
            }
            catch
            {
                Envir.UpdateIPBlock(IPAddress, TimeSpan.FromHours(24));

                MessageQueue.SendMsg($"{IPAddress} Disconnected, Invalid packet.");

                Disconnecting = true;
                return;
            }

            if (_dataCounter > Settings.MaxPacket)
            {
                Envir.UpdateIPBlock(IPAddress, TimeSpan.FromHours(24));

                List<string> packetList = new List<string>();

                while (_lastPackets.Count > 0)
                {
                    _lastPackets.TryDequeue(out Packet pkt);

                    Enum.TryParse<ClientPacketIds>((pkt?.Index ?? 0).ToString(), out ClientPacketIds cPacket);

                    packetList.Add(cPacket.ToString());
                }

                MessageQueue.SendMsg($"{IPAddress} Disconnected, Large amount of Packets. LastPackets: {String.Join(",", packetList.Distinct())}.");

                Disconnecting = true;
                return;
            }

            BeginReceive();
        }
        
        private void BeginSend(List<byte> data)
        {
            if (!Connected || data.Count == 0) return;

            //Interlocked.Add(ref Network.Sent, data.Count);

            try
            {
                _client.Client.BeginSend(data.ToArray(), 0, data.Count, SocketFlags.None, SendData, Disconnecting);
            }
            catch
            {
                Disconnecting = true;
            }
        }
        private void SendData(IAsyncResult result)
        {
            try
            {
                _client.Client.EndSend(result);
            }
            catch
            { }
        }
        public void SendPacketToClient(Packet p)
        {
            if (p == null) return;
            if (_sendList != null && p != null)
                _sendList.Enqueue(p);

            if (!p.Observable) return;
            foreach (ExineConnection c in Observers)
                c.SendPacketToClient(p);
        }

        public void Process()
        {
            if (_client == null || !_client.Connected)
            {
                OnRecvDisconnectHandler(20);
                return;
            }

            while (!_receiveList.IsEmpty && !Disconnecting)
            {
                Packet p;
                if (!_receiveList.TryDequeue(out p)) continue;

                _lastPackets.Enqueue(p);

                TimeOutTime = Envir.Time + Settings.TimeOut;
                ProcessRecvPacket(p);

                if (_receiveList == null)
                    return;
            }

            while (_retryList.Count > 0)
                _receiveList.Enqueue(_retryList.Dequeue());

            if (Envir.Time > TimeOutTime)
            {
                OnRecvDisconnectHandler(21);
                return;
            }

            if (_sendList == null || _sendList.Count <= 0) return;

            List<byte> data = new List<byte>();

            while (!_sendList.IsEmpty)
            {
                Packet p;
                if (!_sendList.TryDequeue(out p) || p == null) continue;
                data.AddRange(p.GetPacketBytes());
            }

            BeginSend(data);
        }
        private void ProcessRecvPacket(Packet p)
        {
            if (p == null || Disconnecting) return;

            switch (p.Index)
            {
                case (short)ClientPacketIds.ClientVersion:
                    OnRecvClientVersionHandler((ClientPacket.ClientVersion) p);
                    break;
                case (short)ClientPacketIds.Disconnect:
                    OnRecvDisconnectHandler(22);
                    break;
                case (short)ClientPacketIds.KeepAlive: // Keep Alive
                    OnRecvClientKeepAliveHandler((ClientPacket.KeepAlive)p);
                    break;
                case (short)ClientPacketIds.NewAccount:
                    OnRecvNewAccountHandler((ClientPacket.NewAccount) p);
                    break;
                case (short)ClientPacketIds.ChangePassword:
                    OnRecvChangePasswordHandler((ClientPacket.ChangePassword) p);
                    break;
                case (short)ClientPacketIds.Login:
                    OnRecvLoginHandler((ClientPacket.Login) p);
                    break;
                case (short)ClientPacketIds.NewCharacter:
                    OnRecvNewCharacterHandler((ClientPacket.NewCharacter) p);
                    break;
                case (short)ClientPacketIds.DeleteCharacter:
                    OnRecvDeleteCharacterHandler((ClientPacket.DeleteCharacter) p);
                    break;
                case (short)ClientPacketIds.StartGame:
                    OnRecvStartGameHandler((ClientPacket.StartGame) p);
                    break;
                case (short)ClientPacketIds.LogOut:
                    OnRecvLogOutHandler();
                    break;
                case (short)ClientPacketIds.Turn:
                    OnRecvTurnHandler((ClientPacket.Turn) p);
                    break;
                case (short)ClientPacketIds.Walk:
                    OnRecvWalkHandler((ClientPacket.Walk) p);
                    break;
                case (short)ClientPacketIds.Run:
                    OnRecvRunHandler((ClientPacket.Run) p);
                    break;
                case (short)ClientPacketIds.Chat:
                    OnRecvChatHandler((ClientPacket.Chat) p);
                    break;
                case (short)ClientPacketIds.MoveItem:
                    OnRecvMoveItemHandler((ClientPacket.MoveItem) p);
                    break;
                case (short)ClientPacketIds.StoreItem:
                    OnRecvStoreItemHandler((ClientPacket.StoreItem) p);
                    break;
                case (short)ClientPacketIds.DepositRefineItem:
                    OnRecvDepositRefineItemHandler((ClientPacket.DepositRefineItem)p);
                    break;
                case (short)ClientPacketIds.RetrieveRefineItem:
                    OnRecvRetrieveRefineItemHandler((ClientPacket.RetrieveRefineItem)p);
                    break;
                case (short)ClientPacketIds.RefineCancel:
                    OnRefineCancelHandler((ClientPacket.RefineCancel)p);
                    break;
                case (short)ClientPacketIds.RefineItem:
                    OnRecvRefineItemHandler((ClientPacket.RefineItem)p);
                    break;
                case (short)ClientPacketIds.CheckRefine:
                    OnRecvCheckRefineHandler((ClientPacket.CheckRefine)p);
                    break;
                case (short)ClientPacketIds.ReplaceWedRing:
                    OnRecvReplaceWedRingHandler((ClientPacket.ReplaceWedRing)p);
                    break;
                case (short)ClientPacketIds.DepositTradeItem:
                    OnRecvDepositTradeItemHandler((ClientPacket.DepositTradeItem)p);
                    break;
                case (short)ClientPacketIds.RetrieveTradeItem:
                    OnRecvRetrieveTradeItemHandler((ClientPacket.RetrieveTradeItem)p);
                    break;
                case (short)ClientPacketIds.TakeBackItem:
                    OnRecvTakeBackItemHandler((ClientPacket.TakeBackItem) p);
                    break;
                case (short)ClientPacketIds.MergeItem:
                    OnRecvMergeItemHandler((ClientPacket.MergeItem) p);
                    break;
                case (short)ClientPacketIds.EquipItem:
                    OnRecvEquipItemHandler((ClientPacket.EquipItem) p);
                    break;
                case (short)ClientPacketIds.RemoveItem:
                    OnRecvRemoveItemHandler((ClientPacket.RemoveItem) p);
                    break;
                case (short)ClientPacketIds.RemoveSlotItem:
                    OnRecvRemoveSlotItemHandler((ClientPacket.RemoveSlotItem)p);
                    break;
                case (short)ClientPacketIds.SplitItem:
                    OnRecvSplitItemHandler((ClientPacket.SplitItem) p);
                    break;
                case (short)ClientPacketIds.UseItem:
                    OnRecvUseItemHandler((ClientPacket.UseItem) p);
                    break;
                case (short)ClientPacketIds.DropItem:
                    OnRecvDropItemHandler((ClientPacket.DropItem) p);
                    break;
                
                case (short)ClientPacketIds.DropGold:
                    OnRecvDropGoldHandler((ClientPacket.DropGold) p);
                    break;
                case (short)ClientPacketIds.PickUp:
                    OnRecvPickUpHandler();
                    break;
                case (short)ClientPacketIds.RequestMapInfo:
                    OnRecvRequestMapInfoHandler((ClientPacket.RequestMapInfo)p);
                    break;
                case (short)ClientPacketIds.TeleportToNPC:
                    OnRecvTeleportToNPCHandler((ClientPacket.TeleportToNPC)p);
                    break;
                case (short)ClientPacketIds.SearchMap:
                    OnRecvSearchMapHandler((ClientPacket.SearchMap)p);
                    break;
                case (short)ClientPacketIds.Inspect:
                    OnRecvInspectHandler((ClientPacket.Inspect)p);
                    break;
                case (short)ClientPacketIds.Observe:
                    OnRecvObserveHandler((ClientPacket.Observe)p);
                    break;
                case (short)ClientPacketIds.ChangeAMode:
                    OnRecvChangeAModeHandler((ClientPacket.ChangeAMode)p);
                    break;
                case (short)ClientPacketIds.ChangePMode:
                    OnRecvChangePModeHandler((ClientPacket.ChangePMode)p);
                    break;
                case (short)ClientPacketIds.ChangeTrade:
                    OnRecvChangeTradeHandler((ClientPacket.ChangeTrade)p);
                    break;
                case (short)ClientPacketIds.Attack:
                    OnRecvAttackHandler((ClientPacket.Attack)p);
                    break;
                case (short)ClientPacketIds.RangeAttack:
                    OnRecvRangeAttackHandler((ClientPacket.RangeAttack)p);
                    break;
                case (short)ClientPacketIds.Harvest:
                    OnRecvHarvestHandler((ClientPacket.Harvest)p);
                    break;
                case (short)ClientPacketIds.CallNPC:
                    OnRecvCallNPCHandler((ClientPacket.CallNPC)p);
                    break;
                case (short)ClientPacketIds.BuyItem:
                    OnRecvBuyItemHandler((ClientPacket.BuyItem)p);
                    break;
                case (short)ClientPacketIds.CraftItem:
                    OnRecvCraftItemHandler((ClientPacket.CraftItem)p);
                    break;
                case (short)ClientPacketIds.SellItem:
                    OnRecvSellItemHandler((ClientPacket.SellItem)p);
                    break;
                case (short)ClientPacketIds.RepairItem:
                    OnRecvRepairItemHandler((ClientPacket.RepairItem)p);
                    break;
                case (short)ClientPacketIds.BuyItemBack:
                    OnRecvBuyItemBackHandler((ClientPacket.BuyItemBack)p);
                    break;
                case (short)ClientPacketIds.SRepairItem:
                    OnRecvSRepairItemHAndler((ClientPacket.SRepairItem)p);
                    break;
                case (short)ClientPacketIds.MagicKey:
                    OnRecvMagicKeyHandler((ClientPacket.MagicKey)p);
                    break;
                case (short)ClientPacketIds.Magic:
                    OnRecvMagicHAndler((ClientPacket.Magic)p);
                    break;
                case (short)ClientPacketIds.SwitchGroup:
                    OnRecvSwitchGroupHandler((ClientPacket.SwitchGroup)p);
                    return;
                case (short)ClientPacketIds.AddMember:
                    OnRecvAddMemberHandler((ClientPacket.AddMember)p);
                    return;
                case (short)ClientPacketIds.DellMember:
                    OnRecvDelMemberHandler((ClientPacket.DelMember)p);
                    return;
                case (short)ClientPacketIds.GroupInvite:
                    OnRecvGroupInviteHAndler((ClientPacket.GroupInvite)p);
                    return;
                case (short)ClientPacketIds.TownRevive:
                    OnRecvTownReviveHandler();
                    return;
                case (short)ClientPacketIds.SpellToggle:
                    OnRecvSpellToggleHAndler((ClientPacket.SpellToggle)p);
                    return;
                case (short)ClientPacketIds.ConsignItem:
                    OnRecvConsignItemHandler((ClientPacket.ConsignItem)p);
                    return;
                
                case (short)ClientPacketIds.RequestUserName:
                    OnRecvRequestUserNameHandler((ClientPacket.RequestUserName)p);
                    return;
                case (short)ClientPacketIds.RequestChatItem:
                    OnRecvRequestChatItemHandler((ClientPacket.RequestChatItem)p);
                    return;
                case (short)ClientPacketIds.EditGuildMember:
                    OnRecvEditGuildMemberHandler((ClientPacket.EditGuildMember)p);
                    return;
                case (short)ClientPacketIds.EditGuildNotice:
                    OnRecvEditGuildNoticeHandler((ClientPacket.EditGuildNotice)p);
                    return;
                case (short)ClientPacketIds.GuildInvite:
                    OnRecvGuildInviteHandler((ClientPacket.GuildInvite)p);
                    return;
                case (short)ClientPacketIds.RequestGuildInfo:
                    OnRecvRequestGuildInfoHAndler((ClientPacket.RequestGuildInfo)p);
                    return;
                case (short)ClientPacketIds.GuildNameReturn:
                    OnRecvGuildNameReturnHandler((ClientPacket.GuildNameReturn)p);
                    return;
                case (short)ClientPacketIds.GuildStorageGoldChange:
                    OnRecvGuildStorageGoldChangeHandler((ClientPacket.GuildStorageGoldChange)p);
                    return;
                case (short)ClientPacketIds.GuildStorageItemChange:
                    OnRecvGuildStorageItemChangeHandler((ClientPacket.GuildStorageItemChange)p);
                    return;
                case (short)ClientPacketIds.GuildWarReturn:
                    OnRecvGuildWarReturnHandler((ClientPacket.GuildWarReturn)p);
                    return;
                case (short)ClientPacketIds.MarriageRequest:
                    OnRecvMarriageRequestHandler((ClientPacket.MarriageRequest)p);
                    return;
                case (short)ClientPacketIds.MarriageReply:
                    OnRecvMarriageReplyHandler((ClientPacket.MarriageReply)p);
                    return;
                case (short)ClientPacketIds.ChangeMarriage:
                    OnRecvChangeMarriageHandler((ClientPacket.ChangeMarriage)p);
                    return;
                case (short)ClientPacketIds.DivorceRequest:
                    OnRecvDivorceRequestHandler((ClientPacket.DivorceRequest)p);
                    return;
                case (short)ClientPacketIds.DivorceReply:
                    OnRecvDivorceReplyHandler((ClientPacket.DivorceReply)p);
                    return;
                case (short)ClientPacketIds.AddMentor:
                    OnRecvAddMentorHandler((ClientPacket.AddMentor)p);
                    return;
                case (short)ClientPacketIds.MentorReply:
                    OnRecvMentorReplyHandler((ClientPacket.MentorReply)p);
                    return;
                case (short)ClientPacketIds.AllowMentor:
                    OnRecvAllowMentorHandler((ClientPacket.AllowMentor)p);
                    return;
                case (short)ClientPacketIds.CancelMentor:
                    OnRecvCancelMentorHandler((ClientPacket.CancelMentor)p);
                    return;
                case (short)ClientPacketIds.TradeRequest:
                    OnRecvTradeRequestHandler((ClientPacket.TradeRequest)p);
                    return;
                case (short)ClientPacketIds.TradeGold:
                    OnRecvTradeGoldHandler((ClientPacket.TradeGold)p);
                    return;
                case (short)ClientPacketIds.TradeReply:
                    OnRecvTradeReplyHandler((ClientPacket.TradeReply)p);
                    return;
                case (short)ClientPacketIds.TradeConfirm:
                    OnRecvTradeConfirmHandler((ClientPacket.TradeConfirm)p);
                    return;
                case (short)ClientPacketIds.TradeCancel:
                    OnRecvTradeCancelHandler((ClientPacket.TradeCancel)p);
                    return;
                case (short)ClientPacketIds.EquipSlotItem:
                    OnRecvEquipSlotItemHandler((ClientPacket.EquipSlotItem)p);
                    break;
                case (short)ClientPacketIds.FishingCast:
                    OnRecvFishingCastHandler((ClientPacket.FishingCast)p);
                    break;
                case (short)ClientPacketIds.FishingChangeAutocast:
                    OnRecvFishingChangeAutocastHandler((ClientPacket.FishingChangeAutocast)p);
                    break;
                case (short)ClientPacketIds.AcceptQuest:
                    OnRecvAcceptQuestHandler((ClientPacket.AcceptQuest)p);
                    break;
                case (short)ClientPacketIds.FinishQuest:
                    OnRecvFinishQuestHandler((ClientPacket.FinishQuest)p);
                    break;
                case (short)ClientPacketIds.AbandonQuest:
                    OnRecvAbandonQuestHandler((ClientPacket.AbandonQuest)p);
                    break;
                case (short)ClientPacketIds.ShareQuest:
                    OnRecvShareQuestHandler((ClientPacket.ShareQuest)p);
                    break;
                case (short)ClientPacketIds.AcceptReincarnation:
                    OnRecvAcceptReincarnation();
                    break;
                case (short)ClientPacketIds.CancelReincarnation:
                    OnRecvCancelReincarnation();
                    break;
                case (short)ClientPacketIds.CombineItem:
                    OnRecvCombineItemHandler((ClientPacket.CombineItem)p);
                    break;
                
                
                case (short)ClientPacketIds.AddFriend:
                    OnRecvAddFriendHandler((ClientPacket.AddFriend)p);
                    break;
                case (short)ClientPacketIds.RemoveFriend:
                    OnRecvRemoveFriendHandler((ClientPacket.RemoveFriend)p);
                    break;
                case (short)ClientPacketIds.RefreshFriends:
                    {
                        if (Stage != GameStage.Game) return;
                        Player.GetFriends();
                        break;
                    }
                 
                case (short)ClientPacketIds.GuildBuffUpdate:
                    OnRecvGuildBuffUpdateHandler((ClientPacket.GuildBuffUpdate)p);
                    break;
                case (short)ClientPacketIds.GameshopBuy:
                    OnRecvGameshopBuyHandler((ClientPacket.GameshopBuy)p);
                    return;
                case (short)ClientPacketIds.NPCConfirmInput:
                    OnRecvNPCConfirmInputHandler((ClientPacket.NPCConfirmInput)p);
                    break;
                case (short)ClientPacketIds.ReportIssue:
                    OnRecvReportIssueHandler((ClientPacket.ReportIssue)p);
                    break;
                case (short)ClientPacketIds.GetRanking:
                    OnRecvGetRankingHandler((ClientPacket.GetRanking)p);
                    break;
                case (short)ClientPacketIds.Opendoor:
                    OnRecvOpendoorHandler((ClientPacket.Opendoor)p);
                    break;
               
                case (short)ClientPacketIds.Rest:
                    OnRecvRestHandler((ClientPacket.Rest)p);
                    break;

                //Server Recv ClientPacketIds.UpdatePhoto packet
                case (short)ClientPacketIds.UpdatePhoto:
                    Console.WriteLine("@@@333 UpdatePhoto");
                    Console.WriteLine("ExineConnection_ProcessPacket_case (short)ClientPacketIds.UpdatePhoto");
                    OnRecvUpdatePhotoHandler((ClientPacket.UpdatePhoto)p);
                    break;

                default:
                    MessageQueue.SendMsg(string.Format("Invalid packet received. Index : {0}", p.Index));
                    break;
            }
        }
        public void SoftDisconnect(byte reason)
        {
            Stage = GameStage.Disconnected;
            TimeDisconnected = Envir.Time;
            
            lock (Envir.AccountLock)
            {
                if (Player != null)
                    Player.StopGame(reason);

                if (Account != null && Account.Connection == this)
                    Account.Connection = null;
            }

            Account = null;
        }
        public void SendDisconnect(byte reason)
        {
            if (!Connected)
            {
                Disconnecting = true;
                SoftDisconnect(reason);
                return;
            }
            
            Disconnecting = true;

            List<byte> data = new List<byte>();

            data.AddRange(new ServerPacket.Disconnect { Reason = reason }.GetPacketBytes());

            BeginSend(data);
            SoftDisconnect(reason);
        }
        public void CleanObservers()
        {
            foreach (ExineConnection c in Observers)
            {
                c.Stage = GameStage.Login;
                c.SendPacketToClient(new ServerPacket.ReturnToLogin());
            }
        }
         
        public List<byte[]> Image = new List<byte[]>();


        #region OnRecvFromClient Handers
        public void OnRecvDisconnectHandler(byte reason)
        {
            if (!Connected) return;

            Connected = false;
            Stage = GameStage.Disconnected;
            TimeDisconnected = Envir.Time;

            lock (Envir.Connections)
                Envir.Connections.Remove(this);

            lock (Envir.AccountLock)
            {
                if (Player != null)
                    Player.StopGame(reason);

                if (Account != null && Account.Connection == this)
                    Account.Connection = null;
            }

            if (Observing != null)
                Observing.Observers.Remove(this);

            Account = null;

            _sendList = null;
            _receiveList = null;
            _retryList = null;
            _rawData = null;

            if (_client != null) _client.Client.Dispose();
            _client = null;
        }
        private void OnRecvClientVersionHandler(ClientPacket.ClientVersion p)
        {
            if (Stage != GameStage.None) return;

            if (Settings.CheckVersion)
            {
                bool match = false;

                foreach (var hash in Settings.VersionHashes)
                {
                    if (Functions.CompareBytes(hash, p.VersionHash))
                    {
                        match = true;
                        break;
                    }
                }

                if (!match)
                {
                    Disconnecting = true;

                    List<byte> data = new List<byte>();

                    data.AddRange(new ServerPacket.ClientVersion { Result = 0 }.GetPacketBytes());

                    BeginSend(data);
                    SoftDisconnect(10);
                    MessageQueue.SendMsg(SessionID + ", Disconnnected - Wrong Client Version.");
                    return;
                }
            }

            MessageQueue.SendMsg(SessionID + ", " + IPAddress + ", Client version matched.");
            SendPacketToClient(new ServerPacket.ClientVersion { Result = 1 });

            Stage = GameStage.Login;
        }
        private void OnRecvClientKeepAliveHandler(ClientPacket.KeepAlive p)
        {
            SendPacketToClient(new ServerPacket.KeepAlive
            {
                Time = p.Time
            });
        }
        private void OnRecvNewAccountHandler(ClientPacket.NewAccount p)
        {
            if (Stage != GameStage.Login) return;

            MessageQueue.SendMsg(SessionID + ", " + IPAddress + ", New account being created.");
            Envir.NewAccount(p, this);
        }
        private void OnRecvChangePasswordHandler(ClientPacket.ChangePassword p)
        {
            if (Stage != GameStage.Login) return;

            MessageQueue.SendMsg(SessionID + ", " + IPAddress + ", Password being changed.");
            Envir.ChangePassword(p, this);
        }
        private void OnRecvLoginHandler(ClientPacket.Login p)
        {
            if (Stage != GameStage.Login) return;

            MessageQueue.SendMsg(SessionID + ", " + IPAddress + ", User logging in.");
            Envir.Login(p, this);
        }
        private void OnRecvNewCharacterHandler(ClientPacket.NewCharacter p)
        {
            Console.WriteLine("NewCharacter");
            if (Stage != GameStage.Select) return; //231107
            Console.WriteLine("NewCharacter#2");

            Envir.NewCharacter(p, this, Account.AdminAccount);
        }
        private void OnRecvDeleteCharacterHandler(ClientPacket.DeleteCharacter p)
        {
            if (Stage != GameStage.Select) return;
            
            if (!Settings.AllowDeleteCharacter)
            {
                SendPacketToClient(new ServerPacket.DeleteCharacter { Result = 0 });
                return;
            }

            CharacterInfo temp = null;
            

            for (int i = 0; i < Account.Characters.Count; i++)
			{
			    if (Account.Characters[i].Index != p.CharacterIndex) continue;

			    temp = Account.Characters[i];
			    break;
			}

            if (temp == null)
            {
                SendPacketToClient(new ServerPacket.DeleteCharacter { Result = 1 });
                return;
            }

            temp.Deleted = true;
            temp.DeleteDate = Envir.Now;
            Envir.RemoveRank(temp);
            SendPacketToClient(new ServerPacket.DeleteCharacterSuccess { CharacterIndex = temp.Index });
        }
        private void OnRecvStartGameHandler(ClientPacket.StartGame p)
        {
            if (Stage != GameStage.Select) return;

            if (!Settings.AllowStartGame && (Account == null || (Account != null && !Account.AdminAccount)))
            {
                SendPacketToClient(new ServerPacket.StartGame { Result = 0 });
                return;
            }

            if (Account == null)
            {
                SendPacketToClient(new ServerPacket.StartGame { Result = 1 });
                return;
            }


            CharacterInfo info = null;

            for (int i = 0; i < Account.Characters.Count; i++)
            {
                if (Account.Characters[i].Index != p.CharacterIndex) continue;

                info = Account.Characters[i];
                break;
            }
            if (info == null)
            {
                SendPacketToClient(new ServerPacket.StartGame { Result = 2 });
                return;
            }

            if (info.Banned)
            {
                if (info.ExpiryDate > Envir.Now)
                {
                    SendPacketToClient(new ServerPacket.StartGameBanned { Reason = info.BanReason, ExpiryDate = info.ExpiryDate });
                    return;
                }
                info.Banned = false;
            }
            info.BanReason = string.Empty;
            info.ExpiryDate = DateTime.MinValue;

            long delay = (long) (Envir.Now - info.LastLogoutDate).TotalMilliseconds;


            //if (delay < Settings.RelogDelay)
            //{
            //    Enqueue(new ServerPacket.StartGameDelay { Milliseconds = Settings.RelogDelay - delay });
            //    return;
            //}

            Player = new PlayerObjectSrv(info, this);
            Player.StartGame();
        }
        public void OnRecvLogOutHandler()
        {
            if (Stage != GameStage.Game) return;

            if (Envir.Time < Player.LogTime)
            {
                SendPacketToClient(new ServerPacket.LogOutFailed());
                return;
            }

            Player.StopGame(23);

            Stage = GameStage.Select;
            Player = null;

            SendPacketToClient(new ServerPacket.LogOutSuccess { Characters = Account.GetSelectInfo() });
        }
        private void OnRecvTurnHandler(ClientPacket.Turn p)
        {
            if (Stage != GameStage.Game) return;

            if (Player.ActionTime > Envir.Time)
                _retryList.Enqueue(p);
            else
                Player.Turn(p.Direction);
        }
        private void OnRecvWalkHandler(ClientPacket.Walk p)
        {
            if (Stage != GameStage.Game) return;

            if (Player.ActionTime > Envir.Time)
                _retryList.Enqueue(p);
            else
                Player.Walk(p.Direction);
        }
        private void OnRecvRunHandler(ClientPacket.Run p)
        {
            if (Stage != GameStage.Game) return;

            if (Player.ActionTime > Envir.Time)
                _retryList.Enqueue(p);
            else
                Player.Run(p.Direction);
        }
        private void OnRecvChatHandler(ClientPacket.Chat p)
        {
            if (p.Message.Length > Globals.MaxChatLength)
            {
                SendDisconnect(2);
                return;
            }

            if (Stage != GameStage.Game) return;

            Player.Chat(p.Message, p.LinkedItems);
        }
        private void OnRecvMoveItemHandler(ClientPacket.MoveItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.MoveItem(p.Grid, p.From, p.To);
        }
        private void OnRecvStoreItemHandler(ClientPacket.StoreItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.StoreItem(p.From, p.To);
        }
        private void OnRecvDepositRefineItemHandler(ClientPacket.DepositRefineItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.DepositRefineItem(p.From, p.To);
        }
        private void OnRecvRetrieveRefineItemHandler(ClientPacket.RetrieveRefineItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RetrieveRefineItem(p.From, p.To);
        }
        private void OnRefineCancelHandler(ClientPacket.RefineCancel p)
        {
            if (Stage != GameStage.Game) return;

            Player.RefineCancel();
        }
        private void OnRecvRefineItemHandler(ClientPacket.RefineItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RefineItem(p.UniqueID);
        }
        private void OnRecvCheckRefineHandler(ClientPacket.CheckRefine p)
        {
            if (Stage != GameStage.Game) return;

            Player.CheckRefine(p.UniqueID);
        }
        private void OnRecvReplaceWedRingHandler(ClientPacket.ReplaceWedRing p)
        {
            if (Stage != GameStage.Game) return;

            Player.ReplaceWeddingRing(p.UniqueID);
        }
        private void OnRecvDepositTradeItemHandler(ClientPacket.DepositTradeItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.DepositTradeItem(p.From, p.To);
        }
        private void OnRecvRetrieveTradeItemHandler(ClientPacket.RetrieveTradeItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RetrieveTradeItem(p.From, p.To);
        }
        private void OnRecvTakeBackItemHandler(ClientPacket.TakeBackItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.TakeBackItem(p.From, p.To);
        }
        private void OnRecvMergeItemHandler(ClientPacket.MergeItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.MergeItem(p.GridFrom, p.GridTo, p.IDFrom, p.IDTo);
        }
        private void OnRecvEquipItemHandler(ClientPacket.EquipItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.EquipItem(p.Grid, p.UniqueID, p.To);
        }
        private void OnRecvRemoveItemHandler(ClientPacket.RemoveItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RemoveItem(p.Grid, p.UniqueID, p.To);
        }
        private void OnRecvRemoveSlotItemHandler(ClientPacket.RemoveSlotItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RemoveSlotItem(p.Grid, p.UniqueID, p.To, p.GridTo, p.FromUniqueID);
        }
        private void OnRecvSplitItemHandler(ClientPacket.SplitItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.SplitItem(p.Grid, p.UniqueID, p.Count);
        }
        private void OnRecvUseItemHandler(ClientPacket.UseItem p)
        {
            if (Stage != GameStage.Game) return;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    Player.UseItem(p.UniqueID);
                    break;
                    /*
                case MirGridType.HeroInventory:
                    Player.HeroUseItem(p.UniqueID);
                    break;
                    */
            }            
        }
        private void OnRecvDropItemHandler(ClientPacket.DropItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.DropItem(p.UniqueID, p.Count, p.HeroInventory);
        }
         private void OnRecvDropGoldHandler(ClientPacket.DropGold p)
        {
            if (Stage != GameStage.Game) return;

            Player.DropGold(p.Amount);
        }
        private void OnRecvPickUpHandler()
        {
            if (Stage != GameStage.Game) return;

            Player.PickUp();
        }
        private void OnRecvRequestMapInfoHandler(ClientPacket.RequestMapInfo p)
        {
            if (Stage != GameStage.Game) return;

            Player.RequestMapInfo(p.MapIndex);
        }
        private void OnRecvTeleportToNPCHandler(ClientPacket.TeleportToNPC p)
        {
            if (Stage != GameStage.Game) return;

            Player.TeleportToNPC(p.ObjectID);
        }
        private void OnRecvSearchMapHandler(ClientPacket.SearchMap p)
        {
            if (Stage != GameStage.Game) return;

            Player.SearchMap(p.Text);
        }
        private void OnRecvInspectHandler(ClientPacket.Inspect p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            if (p.Ranking)
            {
                Envir.Inspect(this, (int)p.ObjectID);
            }
            else
            {
                Envir.Inspect(this, p.ObjectID);
            } 
        }
        private void OnRecvObserveHandler(ClientPacket.Observe p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            Envir.Observe(this, p.Name);
        }
        private void OnRecvChangeAModeHandler(ClientPacket.ChangeAMode p)
        {
            if (Stage != GameStage.Game) return;

            Player.AMode = p.Mode;

            SendPacketToClient(new ServerPacket.ChangeAMode {Mode = Player.AMode});
        }
        private void OnRecvChangePModeHandler(ClientPacket.ChangePMode p)
        {
            if (Stage != GameStage.Game) return;

            Player.PMode = p.Mode;

            SendPacketToClient(new ServerPacket.ChangePMode { Mode = Player.PMode });
        }
        private void OnRecvChangeTradeHandler(ClientPacket.ChangeTrade p)
        {
            if (Stage != GameStage.Game) return;

            Player.AllowTrade = p.AllowTrade;
        }
        private void OnRecvAttackHandler(ClientPacket.Attack p)
        {
            if (Stage != GameStage.Game) return;

            if (!Player.Dead && (Player.ActionTime > Envir.Time || Player.AttackTime > Envir.Time))
                _retryList.Enqueue(p);
            else
                Player.Attack(p.Direction, p.Spell);
        }
        private void OnRecvRangeAttackHandler(ClientPacket.RangeAttack p)
        {
            if (Stage != GameStage.Game) return;

            if (!Player.Dead && (Player.ActionTime > Envir.Time || Player.AttackTime > Envir.Time))
                _retryList.Enqueue(p);
            else
                Player.RangeAttack(p.Direction, p.TargetLocation, p.TargetID);
        }
        private void OnRecvHarvestHandler(ClientPacket.Harvest p)
        {
            if (Stage != GameStage.Game) return;

            if (!Player.Dead && Player.ActionTime > Envir.Time)
                _retryList.Enqueue(p);
            else
                Player.Harvest(p.Direction);
        }
        private void OnRecvCallNPCHandler(ClientPacket.CallNPC p)
        {
            if (Stage != GameStage.Game) return;

            if (p.Key.Length > 30) //No NPC Key should be that long.
            {
                SendDisconnect(2);
                return;
            }

            if (p.ObjectID == Envir.DefaultNPC.LoadedObjectID && Player.NPCObjectID == Envir.DefaultNPC.LoadedObjectID)
            {
                Player.CallDefaultNPC(p.Key);
                return;
            }

            if (p.ObjectID == uint.MaxValue)
            {
                Player.CallDefaultNPC(DefaultNPCType.Client, null);
                return;
            }

            Player.CallNPC(p.ObjectID, p.Key);
        }
        private void OnRecvBuyItemHandler(ClientPacket.BuyItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.BuyItem(p.ItemIndex, p.Count, p.Type);
        }
        private void OnRecvCraftItemHandler(ClientPacket.CraftItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.CraftItem(p.UniqueID, p.Count, p.Slots);
        }
        private void OnRecvSellItemHandler(ClientPacket.SellItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.SellItem(p.UniqueID, p.Count);
        }
        private void OnRecvRepairItemHandler(ClientPacket.RepairItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RepairItem(p.UniqueID);
        }
        private void OnRecvBuyItemBackHandler(ClientPacket.BuyItemBack p)
        {
            if (Stage != GameStage.Game) return;

           // Player.BuyItemBack(p.UniqueID, p.Count);
        }
        private void OnRecvSRepairItemHAndler(ClientPacket.SRepairItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RepairItem(p.UniqueID, true);
        }
        private void OnRecvMagicKeyHandler(ClientPacket.MagicKey p)
        {
            if (Stage != GameStage.Game) return;

            HumanObjectSrv actor = Player;
            if (p.Key > 16 || p.OldKey > 16)
            {
                return;
            }

            for (int i = 0; i < actor.Info.Magics.Count; i++)
            {
                UserMagic magic = actor.Info.Magics[i];
                if (magic.Spell != p.Spell)
                {
                    if (magic.Key == p.Key)
                        magic.Key = 0;
                    continue;
                }

                magic.Key = p.Key;
            }
        }
        private void OnRecvMagicHAndler(ClientPacket.Magic p)
        {
            if (Stage != GameStage.Game) return;

            HumanObjectSrv actor = Player;
             

            if (actor.Dead) return;

            if (!actor.Dead && (actor.ActionTime > Envir.Time || actor.SpellTime > Envir.Time))
                _retryList.Enqueue(p);
            else
                actor.BeginMagic(p.Spell, p.Direction, p.TargetID, p.Location, p.SpellTargetLock);
        }
        private void OnRecvSwitchGroupHandler(ClientPacket.SwitchGroup p)
        {
            if (Stage != GameStage.Game) return;

            Player.SwitchGroup(p.AllowGroup);
        }
        private void OnRecvAddMemberHandler(ClientPacket.AddMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.AddMember(p.Name);
        }
        private void OnRecvDelMemberHandler(ClientPacket.DelMember p)
        {
            if (Stage != GameStage.Game) return;

            Player.DelMember(p.Name);
        }
        private void OnRecvGroupInviteHAndler(ClientPacket.GroupInvite p)
        {
            if (Stage != GameStage.Game) return;

            Player.GroupInvite(p.AcceptInvite);
        }
        private void OnRecvTownReviveHandler()
        {
            if (Stage != GameStage.Game) return;

            Player.TownRevive();
        }
        private void OnRecvSpellToggleHAndler(ClientPacket.SpellToggle p)
        {
            if (Stage != GameStage.Game) return;

            if (p.canUse > SpellToggleState.None)
            {
                Player.SpellToggle(p.Spell, p.canUse);
                return;
            }
                 
        }
        private void OnRecvConsignItemHandler(ClientPacket.ConsignItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.ConsignItem(p.UniqueID, p.Price, p.Type);
        }        
        private void OnRecvRequestUserNameHandler(ClientPacket.RequestUserName p)
        {
            if (Stage != GameStage.Game) return;

            Player.RequestUserName(p.UserID);
        }
        private void OnRecvRequestChatItemHandler(ClientPacket.RequestChatItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.RequestChatItem(p.ChatItemID);
        }
        private void OnRecvEditGuildMemberHandler(ClientPacket.EditGuildMember p)
        {
            if (Stage != GameStage.Game) return;
            Player.EditGuildMember(p.Name,p.RankName,p.RankIndex,p.ChangeType);
        }
        private void OnRecvEditGuildNoticeHandler(ClientPacket.EditGuildNotice p)
        {
            if (Stage != GameStage.Game) return;
            Player.EditGuildNotice(p.notice);
        }
        private void OnRecvGuildInviteHandler(ClientPacket.GuildInvite p)
        {
            if (Stage != GameStage.Game) return;

            Player.GuildInvite(p.AcceptInvite);
        }
        private void OnRecvRequestGuildInfoHAndler(ClientPacket.RequestGuildInfo p)
        {
            if (Stage != GameStage.Game) return;
            Player.RequestGuildInfo(p.Type);
        }
        private void OnRecvGuildNameReturnHandler(ClientPacket.GuildNameReturn p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildNameReturn(p.Name);
        }
        private void OnRecvGuildStorageGoldChangeHandler(ClientPacket.GuildStorageGoldChange p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildStorageGoldChange(p.Type, p.Amount);
        }
        private void OnRecvGuildStorageItemChangeHandler(ClientPacket.GuildStorageItemChange p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildStorageItemChange(p.Type, p.From, p.To);
        }
        private void OnRecvGuildWarReturnHandler(ClientPacket.GuildWarReturn p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildWarReturn(p.Name);
        }
        private void OnRecvMarriageRequestHandler(ClientPacket.MarriageRequest p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageRequest();
        }
        private void OnRecvMarriageReplyHandler(ClientPacket.MarriageReply p)
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageReply(p.AcceptInvite);
        }
        private void OnRecvChangeMarriageHandler(ClientPacket.ChangeMarriage p)
        {
            if (Stage != GameStage.Game) return;

            if (Player.Info.Married == 0)
            {
                Player.AllowMarriage = !Player.AllowMarriage;
                if (Player.AllowMarriage)
                    Player.ReceiveChat("You're now allowing marriage requests.", ChatType.Hint);
                else
                    Player.ReceiveChat("You're now blocking marriage requests.", ChatType.Hint);
            }
            else
            {
                Player.AllowLoverRecall = !Player.AllowLoverRecall;
                if (Player.AllowLoverRecall)
                    Player.ReceiveChat("You're now allowing recall from lover.", ChatType.Hint);
                else
                    Player.ReceiveChat("You're now blocking recall from lover.", ChatType.Hint);
            }
        }
        private void OnRecvDivorceRequestHandler(ClientPacket.DivorceRequest p)
        {
            if (Stage != GameStage.Game) return;

            Player.DivorceRequest();
        }
        private void OnRecvDivorceReplyHandler(ClientPacket.DivorceReply p)
        {
            if (Stage != GameStage.Game) return;

            Player.DivorceReply(p.AcceptInvite);
        }
        private void OnRecvAddMentorHandler(ClientPacket.AddMentor p)
        {
            if (Stage != GameStage.Game) return;

            Player.AddMentor(p.Name);
        }
        private void OnRecvMentorReplyHandler(ClientPacket.MentorReply p)
        {
            if (Stage != GameStage.Game) return;

            Player.MentorReply(p.AcceptInvite);
        }
        private void OnRecvAllowMentorHandler(ClientPacket.AllowMentor p)
        {
            if (Stage != GameStage.Game) return;

                Player.AllowMentor = !Player.AllowMentor;
                if (Player.AllowMentor)
                    Player.ReceiveChat(GameLanguage.AllowingMentorRequests, ChatType.Hint);
                else
                    Player.ReceiveChat(GameLanguage.BlockingMentorRequests, ChatType.Hint);
        }
        private void OnRecvCancelMentorHandler(ClientPacket.CancelMentor p)
        {
            if (Stage != GameStage.Game) return;

            Player.MentorBreak(true);
        }
        private void OnRecvTradeRequestHandler(ClientPacket.TradeRequest p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeRequest();
        }
        private void OnRecvTradeGoldHandler(ClientPacket.TradeGold p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeGold(p.Amount);
        }
        private void OnRecvTradeReplyHandler(ClientPacket.TradeReply p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeReply(p.AcceptInvite);
        }
        private void OnRecvTradeConfirmHandler(ClientPacket.TradeConfirm p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeConfirm(p.Locked);
        }
        private void OnRecvTradeCancelHandler(ClientPacket.TradeCancel p)
        {
            if (Stage != GameStage.Game) return;

            Player.TradeCancel();
        }
        private void OnRecvEquipSlotItemHandler(ClientPacket.EquipSlotItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.EquipSlotItem(p.Grid, p.UniqueID, p.To, p.GridTo, p.ToUniqueID);
        }
        private void OnRecvFishingCastHandler(ClientPacket.FishingCast p)
        {
            if (Stage != GameStage.Game) return;

            Player.FishingCast(p.Sitdown, true);
        }
        private void OnRecvFishingChangeAutocastHandler(ClientPacket.FishingChangeAutocast p)
        {
            if (Stage != GameStage.Game) return;

            Player.FishingChangeAutocast(p.AutoCast);
        }
        private void OnRecvAcceptQuestHandler(ClientPacket.AcceptQuest p)
        {
            if (Stage != GameStage.Game) return;

            Player.AcceptQuest(p.QuestIndex); //p.NPCIndex,
        }
        private void OnRecvFinishQuestHandler(ClientPacket.FinishQuest p)
        {
            if (Stage != GameStage.Game) return;

            Player.FinishQuest(p.QuestIndex, p.SelectedItemIndex);
        }
        private void OnRecvAbandonQuestHandler(ClientPacket.AbandonQuest p)
        {
            if (Stage != GameStage.Game) return;

            Player.AbandonQuest(p.QuestIndex);
        }
        private void OnRecvShareQuestHandler(ClientPacket.ShareQuest p)
        {
            if (Stage != GameStage.Game) return;

            Player.ShareQuest(p.QuestIndex);
        }
        private void OnRecvAcceptReincarnation()
        {
            if (Stage != GameStage.Game) return;

            if (Player.ReincarnationHost != null && Player.ReincarnationHost.ReincarnationReady)
            {
                Player.Revive(Player.Stats[Stat.HP] / 2, true);
                Player.ReincarnationHost = null;
                return;
            }

            Player.ReceiveChat("Reincarnation failed", ChatType.System);
        }
        private void OnRecvCancelReincarnation()
        {
            if (Stage != GameStage.Game) return;
            Player.ReincarnationExpireTime = Envir.Time;

        }
        private void OnRecvCombineItemHandler(ClientPacket.CombineItem p)
        {
            if (Stage != GameStage.Game) return;

            Player.CombineItem(p.Grid, p.IDFrom, p.IDTo);
        }
        private void OnRecvAddFriendHandler(ClientPacket.AddFriend p)
        {
            if (Stage != GameStage.Game) return;

            Player.AddFriend(p.Name, p.Blocked);
        }
        private void OnRecvRemoveFriendHandler(ClientPacket.RemoveFriend p)
        {
            if (Stage != GameStage.Game) return;

            Player.RemoveFriend(p.CharacterIndex);
        }
        private void OnRecvUpdatePhotoHandler(ClientPacket.UpdatePhoto p)
        {
            Console.WriteLine("@@@222 UpdatePhoto");
            if (Stage != GameStage.Game) return; 
            Player.UpdatePhoto(p.photoDataLen, p.photoData);
        }
        private void OnRecvGuildBuffUpdateHandler(ClientPacket.GuildBuffUpdate p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildBuffUpdate(p.Action,p.Id);
        }
        private void OnRecvGameshopBuyHandler(ClientPacket.GameshopBuy p)
        {
            if (Stage != GameStage.Game) return;
            Player.GameshopBuy(p.GIndex, p.Quantity);
        }
        private void OnRecvNPCConfirmInputHandler(ClientPacket.NPCConfirmInput p)
        {
            if (Stage != GameStage.Game) return;

            Player.NPCData["NPCInputStr"] = p.Value;

            Player.CallNPC(Player.NPCObjectID, p.PageName);
        }
        private void OnRecvReportIssueHandler(ClientPacket.ReportIssue p)
        {
            if (Stage != GameStage.Game) return;

            return;

            // Image.Add(p.Image);

            // if (p.ImageChunk >= p.ImageSize)
            // {
            //     System.Drawing.Image image = Functions.ByteArrayToImage(Functions.CombineArray(Image));
            //     image.Save("Reported-" + Player.Name + "-" + DateTime.Now.ToString("yyMMddHHmmss") + ".jpg");
            //     Image.Clear();
            // }
        }
        private void OnRecvGetRankingHandler(ClientPacket.GetRanking p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;
            Envir.GetRanking(this, p.RankType, p.RankIndex, p.OnlineOnly);
        }
        private void OnRecvOpendoorHandler(ClientPacket.Opendoor p)
        {
            if (Stage != GameStage.Game) return;
            Player.Opendoor(p.DoorIndex);
        }
        private void OnRecvRestHandler(ClientPacket.Rest p) //231107
        {
            if (Stage != GameStage.Game) return;

            //Player.Rest(p.Rest, true);

            if (Player.ActionTime > Envir.Time)
                _retryList.Enqueue(p);
            else
                Player.Rest(p.Direction);
        }
        #endregion OnRecvFromClient Handers

        public void CheckItemInfo(ItemInfo info, bool dontLoop = false)
        {
            if ((dontLoop == false) && (info.ClassBased | info.LevelBased)) //send all potential data so client can display it
            {
                for (int i = 0; i < Envir.ItemInfoList.Count; i++)
                {
                    if ((Envir.ItemInfoList[i] != info) && (Envir.ItemInfoList[i].Name.StartsWith(info.Name)))
                        CheckItemInfo(Envir.ItemInfoList[i], true);
                }
            }

            if (SentItemInfo.Contains(info)) return;
            SendPacketToClient(new ServerPacket.NewItemInfo { Info = info });
            SentItemInfo.Add(info);
        }
        public void CheckItem(UserItem item)
        {
            CheckItemInfo(item.Info);

            for (int i = 0; i < item.Slots.Length; i++)
            {
                if (item.Slots[i] == null) continue;

                CheckItemInfo(item.Slots[i].Info);
            }
             
        }
    }
}

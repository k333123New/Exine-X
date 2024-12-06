

public abstract class Packet
{
    public static bool IsServer;
    public virtual bool Observable => true;
    public abstract short Index { get; }

    public static Packet ReceivePacket(byte[] rawBytes, out byte[] extra)
    {
        extra = rawBytes;

        Packet p;

        if (rawBytes.Length < 4) return null; //| 2Bytes: Packet Size | 2Bytes: Packet ID |

        int length = (rawBytes[1] << 8) + rawBytes[0];

        if (length > rawBytes.Length || length < 2) return null;

        using (MemoryStream stream = new MemoryStream(rawBytes, 2, length - 2))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            try
            { 
                short packetID = reader.ReadInt16(); 
                //brench 
                p = IsServer ? GetClientPacketParser(packetID) : GetServerPacketParser(packetID); 
                if (p == null) return null; 
                p.ToParse(reader); 
            }
            catch
            {
                throw new InvalidDataException();
            }
        }

        extra = new byte[rawBytes.Length - length];
        Buffer.BlockCopy(rawBytes, length, extra, 0, rawBytes.Length - length);

        return p;
    }

    public IEnumerable<byte> GetPacketBytes()
    {
        if (Index < 0) return new byte[0];

        byte[] data;

        using (MemoryStream stream = new MemoryStream())
        {
            stream.SetLength(2);
            stream.Seek(2, SeekOrigin.Begin);
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Index);
                MakePacket(writer);
                stream.Seek(0, SeekOrigin.Begin);
                writer.Write((short)stream.Length);
                stream.Seek(0, SeekOrigin.Begin);

                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }
        }

        return data;
    }

    protected abstract void ToParse(BinaryReader reader);
    protected abstract void MakePacket(BinaryWriter writer);

    private static Packet GetClientPacketParser(short packetID)
    {
        switch (packetID)
        {
            case (short)ClientPacketIds.ClientVersion:
                return new global::ClientPacket.ClientVersion();
            case (short)ClientPacketIds.Disconnect:
                return new global::ClientPacket.Disconnect();
            case (short)ClientPacketIds.KeepAlive:
                return new global::ClientPacket.KeepAlive();
            case (short)ClientPacketIds.NewAccount:
                return new global::ClientPacket.NewAccount();
            case (short)ClientPacketIds.ChangePassword:
                return new global::ClientPacket.ChangePassword();
            case (short)ClientPacketIds.Login:
                return new global::ClientPacket.Login();
            case (short)ClientPacketIds.NewCharacter:
                return new global::ClientPacket.NewCharacter();
            case (short)ClientPacketIds.DeleteCharacter:
                return new global::ClientPacket.DeleteCharacter();
            case (short)ClientPacketIds.StartGame:
                return new global::ClientPacket.StartGame();
            case (short)ClientPacketIds.LogOut:
                return new global::ClientPacket.LogOut();
            case (short)ClientPacketIds.Turn:
                return new global::ClientPacket.Turn();
            case (short)ClientPacketIds.Walk:
                return new global::ClientPacket.Walk();
            case (short)ClientPacketIds.Run:
                return new global::ClientPacket.Run();
            case (short)ClientPacketIds.Chat:
                return new global::ClientPacket.Chat();
            case (short)ClientPacketIds.MoveItem:
                return new global::ClientPacket.MoveItem();
            case (short)ClientPacketIds.StoreItem:
                return new global::ClientPacket.StoreItem();
            case (short)ClientPacketIds.TakeBackItem:
                return new global::ClientPacket.TakeBackItem();
            case (short)ClientPacketIds.MergeItem:
                return new global::ClientPacket.MergeItem();
            case (short)ClientPacketIds.EquipItem:
                return new global::ClientPacket.EquipItem();
            case (short)ClientPacketIds.RemoveItem:
                return new global::ClientPacket.RemoveItem();
            case (short)ClientPacketIds.RemoveSlotItem:
                return new global::ClientPacket.RemoveSlotItem();
            case (short)ClientPacketIds.SplitItem:
                return new global::ClientPacket.SplitItem();
            case (short)ClientPacketIds.UseItem:
                return new global::ClientPacket.UseItem();
            case (short)ClientPacketIds.DropItem:
                return new global::ClientPacket.DropItem();
            case (short)ClientPacketIds.DepositRefineItem:
                return new global::ClientPacket.DepositRefineItem();
            case (short)ClientPacketIds.RetrieveRefineItem:
                return new global::ClientPacket.RetrieveRefineItem();
            case (short)ClientPacketIds.RefineCancel:
                return new global::ClientPacket.RefineCancel();
            case (short)ClientPacketIds.RefineItem:
                return new global::ClientPacket.RefineItem();
            case (short)ClientPacketIds.CheckRefine:
                return new global::ClientPacket.CheckRefine();
            case (short)ClientPacketIds.ReplaceWedRing:
                return new global::ClientPacket.ReplaceWedRing();
            case (short)ClientPacketIds.DepositTradeItem:
                return new global::ClientPacket.DepositTradeItem();
            case (short)ClientPacketIds.RetrieveTradeItem:
                return new global::ClientPacket.RetrieveTradeItem();
            case (short)ClientPacketIds.DropGold:
                return new global::ClientPacket.DropGold();
            case (short)ClientPacketIds.PickUp:
                return new global::ClientPacket.PickUp();
            case (short)ClientPacketIds.RequestMapInfo:
                return new global::ClientPacket.RequestMapInfo();
            case (short)ClientPacketIds.TeleportToNPC:
                return new global::ClientPacket.TeleportToNPC();
            case (short)ClientPacketIds.SearchMap:
                return new global::ClientPacket.SearchMap();
            case (short)ClientPacketIds.Inspect:
                return new global::ClientPacket.Inspect();
            case (short)ClientPacketIds.Observe:
                return new global::ClientPacket.Observe();
            case (short)ClientPacketIds.ChangeAMode:
                return new global::ClientPacket.ChangeAMode();
            case (short)ClientPacketIds.ChangePMode:
                return new global::ClientPacket.ChangePMode();
            case (short)ClientPacketIds.ChangeTrade:
                return new global::ClientPacket.ChangeTrade();
            case (short)ClientPacketIds.Attack:
                return new global::ClientPacket.Attack();
            case (short)ClientPacketIds.RangeAttack:
                return new global::ClientPacket.RangeAttack();
            case (short)ClientPacketIds.Harvest:
                return new global::ClientPacket.Harvest();
            case (short)ClientPacketIds.CallNPC:
                return new global::ClientPacket.CallNPC();
            case (short)ClientPacketIds.BuyItem:
                return new global::ClientPacket.BuyItem();
            case (short)ClientPacketIds.SellItem:
                return new global::ClientPacket.SellItem();
            case (short)ClientPacketIds.CraftItem:
                return new global::ClientPacket.CraftItem();
            case (short)ClientPacketIds.RepairItem:
                return new global::ClientPacket.RepairItem();
            case (short)ClientPacketIds.BuyItemBack:
                return new global::ClientPacket.BuyItemBack();
            case (short)ClientPacketIds.SRepairItem:
                return new global::ClientPacket.SRepairItem();
            case (short)ClientPacketIds.MagicKey:
                return new global::ClientPacket.MagicKey();
            case (short)ClientPacketIds.Magic:
                return new global::ClientPacket.Magic();
            case (short)ClientPacketIds.SwitchGroup:
                return new global::ClientPacket.SwitchGroup();
            case (short)ClientPacketIds.AddMember:
                return new global::ClientPacket.AddMember();
            case (short)ClientPacketIds.DellMember:
                return new global::ClientPacket.DelMember();
            case (short)ClientPacketIds.GroupInvite:
                return new global::ClientPacket.GroupInvite(); 
            case (short)ClientPacketIds.TownRevive:
                return new global::ClientPacket.TownRevive();
            case (short)ClientPacketIds.SpellToggle:
                return new global::ClientPacket.SpellToggle();
            case (short)ClientPacketIds.ConsignItem:
                return new global::ClientPacket.ConsignItem();
            
            case (short)ClientPacketIds.RequestUserName:
                return new global::ClientPacket.RequestUserName();
            case (short)ClientPacketIds.RequestChatItem:
                return new global::ClientPacket.RequestChatItem();
            case (short)ClientPacketIds.EditGuildMember:
                return new global::ClientPacket.EditGuildMember();
            case (short)ClientPacketIds.EditGuildNotice:
                return new global::ClientPacket.EditGuildNotice();
            case (short)ClientPacketIds.GuildInvite:
                return new global::ClientPacket.GuildInvite();
            case (short)ClientPacketIds.GuildNameReturn:
                return new global::ClientPacket.GuildNameReturn();
            case (short)ClientPacketIds.RequestGuildInfo:
                return new global::ClientPacket.RequestGuildInfo();
            case (short)ClientPacketIds.GuildStorageGoldChange:
                return new global::ClientPacket.GuildStorageGoldChange();
            case (short)ClientPacketIds.GuildStorageItemChange:
                return new global::ClientPacket.GuildStorageItemChange();
            case (short)ClientPacketIds.GuildWarReturn:
                return new global::ClientPacket.GuildWarReturn();
            case (short)ClientPacketIds.MarriageRequest:
                return new global::ClientPacket.MarriageRequest();
            case (short)ClientPacketIds.MarriageReply:
                return new global::ClientPacket.MarriageReply();
            case (short)ClientPacketIds.ChangeMarriage:
                return new global::ClientPacket.ChangeMarriage();
            case (short)ClientPacketIds.DivorceRequest:
                return new global::ClientPacket.DivorceRequest();
            case (short)ClientPacketIds.DivorceReply:
                return new global::ClientPacket.DivorceReply();
            case (short)ClientPacketIds.AddMentor:
                return new global::ClientPacket.AddMentor();
            case (short)ClientPacketIds.MentorReply:
                return new global::ClientPacket.MentorReply();
            case (short)ClientPacketIds.AllowMentor:
                return new global::ClientPacket.AllowMentor();
            case (short)ClientPacketIds.CancelMentor:
                return new global::ClientPacket.CancelMentor();
            case (short)ClientPacketIds.TradeRequest:
                return new global::ClientPacket.TradeRequest();
            case (short)ClientPacketIds.TradeReply:
                return new global::ClientPacket.TradeReply();
            case (short)ClientPacketIds.TradeGold:
                return new global::ClientPacket.TradeGold();
            case (short)ClientPacketIds.TradeConfirm:
                return new global::ClientPacket.TradeConfirm();
            case (short)ClientPacketIds.TradeCancel:
                return new global::ClientPacket.TradeCancel();
            case (short)ClientPacketIds.EquipSlotItem:
                return new global::ClientPacket.EquipSlotItem();
            case (short)ClientPacketIds.FishingCast:
                return new global::ClientPacket.FishingCast();
            case (short)ClientPacketIds.FishingChangeAutocast:
                return new global::ClientPacket.FishingChangeAutocast();
            case (short)ClientPacketIds.AcceptQuest:
                return new global::ClientPacket.AcceptQuest();
            case (short)ClientPacketIds.FinishQuest:
                return new global::ClientPacket.FinishQuest();
            case (short)ClientPacketIds.AbandonQuest:
                return new global::ClientPacket.AbandonQuest();
            case (short)ClientPacketIds.ShareQuest:
                return new global::ClientPacket.ShareQuest();
            case (short)ClientPacketIds.AcceptReincarnation:
                return new global::ClientPacket.AcceptReincarnation();
            case (short)ClientPacketIds.CancelReincarnation:
                return new global::ClientPacket.CancelReincarnation();
            case (short)ClientPacketIds.CombineItem:
                return new global::ClientPacket.CombineItem();
             
           
            case (short)ClientPacketIds.AddFriend:
                return new global::ClientPacket.AddFriend();
            case (short)ClientPacketIds.RemoveFriend:
                return new global::ClientPacket.RemoveFriend();
            case (short)ClientPacketIds.RefreshFriends:
                return new global::ClientPacket.RefreshFriends();
            
            case (short)ClientPacketIds.GuildBuffUpdate:
                return new global::ClientPacket.GuildBuffUpdate();
            case (short)ClientPacketIds.GameshopBuy:
                return new global::ClientPacket.GameshopBuy();
            case (short)ClientPacketIds.NPCConfirmInput:
                return new global::ClientPacket.NPCConfirmInput();
            case (short)ClientPacketIds.ReportIssue:
                return new global::ClientPacket.ReportIssue();
            case (short)ClientPacketIds.GetRanking:
                return new global::ClientPacket.GetRanking();
            case (short)ClientPacketIds.Opendoor:
                return new global::ClientPacket.Opendoor();
           
            case (short)ClientPacketIds.Rest://add k333123
                return new global::ClientPacket.Rest();
            case (short)ClientPacketIds.UpdatePhoto://add k333123

                Console.WriteLine("@@@444 UpdatePhoto");
                return new global::ClientPacket.UpdatePhoto();
            default:
                return null;
        }

    }
    public static Packet GetServerPacketParser(short index)
    {
        switch (index)
        {
            case (short)ServerPacketIds.Connected:
                return new global::ServerPacket.Connected();
            case (short)ServerPacketIds.ClientVersion:
                return new global::ServerPacket.ClientVersion();
            case (short)ServerPacketIds.Disconnect:
                return new global::ServerPacket.Disconnect();
            case (short)ServerPacketIds.KeepAlive:
                return new global::ServerPacket.KeepAlive();
            case (short)ServerPacketIds.NewAccount:
                return new global::ServerPacket.NewAccount();
            case (short)ServerPacketIds.ChangePassword:
                return new global::ServerPacket.ChangePassword();
            case (short)ServerPacketIds.ChangePasswordBanned:
                return new global::ServerPacket.ChangePasswordBanned();
            case (short)ServerPacketIds.Login:
                return new global::ServerPacket.Login();
            case (short)ServerPacketIds.LoginBanned:
                return new global::ServerPacket.LoginBanned();
            case (short)ServerPacketIds.LoginSuccess:
                return new global::ServerPacket.LoginSuccess();
            case (short)ServerPacketIds.NewCharacter:
                return new global::ServerPacket.NewCharacter();
            case (short)ServerPacketIds.NewCharacterSuccess:
                return new global::ServerPacket.NewCharacterSuccess();
            case (short)ServerPacketIds.DeleteCharacter:
                return new global::ServerPacket.DeleteCharacter();
            case (short)ServerPacketIds.DeleteCharacterSuccess:
                return new global::ServerPacket.DeleteCharacterSuccess();
            case (short)ServerPacketIds.StartGame:
                return new global::ServerPacket.StartGame();
            case (short)ServerPacketIds.StartGameBanned:
                return new global::ServerPacket.StartGameBanned();
            case (short)ServerPacketIds.StartGameDelay:
                return new global::ServerPacket.StartGameDelay();
            case (short)ServerPacketIds.MapInformation:
                return new global::ServerPacket.MapInformation();
            case (short)ServerPacketIds.NewMapInfo:
                return new global::ServerPacket.NewMapInfo();
            case (short)ServerPacketIds.WorldMapSetup:
                return new global::ServerPacket.WorldMapSetupInfo();
            case (short)ServerPacketIds.SearchMapResult:
                return new global::ServerPacket.SearchMapResult();
            case (short)ServerPacketIds.UserInformation:
                return new global::ServerPacket.UserInformation();
            case (short)ServerPacketIds.UserSlotsRefresh:
                return new global::ServerPacket.UserSlotsRefresh();
            case (short)ServerPacketIds.UserLocation:
                return new global::ServerPacket.UserLocation();
            case (short)ServerPacketIds.ObjectPlayer:
                return new global::ServerPacket.ObjectPlayer();
            case (short)ServerPacketIds.ObjectRemove:
                return new global::ServerPacket.ObjectRemove();
            case (short)ServerPacketIds.ObjectTurn:
                return new global::ServerPacket.ObjectTurn();
            case (short)ServerPacketIds.ObjectWalk:
                return new global::ServerPacket.ObjectWalk();
            case (short)ServerPacketIds.ObjectRun:
                return new global::ServerPacket.ObjectRun();
            case (short)ServerPacketIds.Chat:
                return new global::ServerPacket.Chat();
            case (short)ServerPacketIds.ObjectChat:
                return new global::ServerPacket.ObjectChat();
            case (short)ServerPacketIds.NewItemInfo:
                return new global::ServerPacket.NewItemInfo();
            case (short)ServerPacketIds.NewChatItem:
                return new global::ServerPacket.NewChatItem();
            case (short)ServerPacketIds.MoveItem:
                return new global::ServerPacket.MoveItem();
            case (short)ServerPacketIds.EquipItem:
                return new global::ServerPacket.EquipItem();
            case (short)ServerPacketIds.MergeItem:
                return new global::ServerPacket.MergeItem();
            case (short)ServerPacketIds.RemoveItem:
                return new global::ServerPacket.RemoveItem();
            case (short)ServerPacketIds.RemoveSlotItem:
                return new global::ServerPacket.RemoveSlotItem();
            case (short)ServerPacketIds.TakeBackItem:
                return new global::ServerPacket.TakeBackItem();
            case (short)ServerPacketIds.StoreItem:
                return new global::ServerPacket.StoreItem();
            case (short)ServerPacketIds.DepositRefineItem:
                return new global::ServerPacket.DepositRefineItem();
            case (short)ServerPacketIds.RetrieveRefineItem:
                return new global::ServerPacket.RetrieveRefineItem();
            case (short)ServerPacketIds.RefineItem:
                return new global::ServerPacket.RefineItem();
            case (short)ServerPacketIds.DepositTradeItem:
                return new global::ServerPacket.DepositTradeItem();
            case (short)ServerPacketIds.RetrieveTradeItem:
                return new global::ServerPacket.RetrieveTradeItem();
            case (short)ServerPacketIds.SplitItem:
                return new global::ServerPacket.SplitItem();
            case (short)ServerPacketIds.SplitItem1:
                return new global::ServerPacket.SplitItem1();
            case (short)ServerPacketIds.UseItem:
                return new global::ServerPacket.UseItem();
            case (short)ServerPacketIds.DropItem:
                return new global::ServerPacket.DropItem();
            case (short)ServerPacketIds.PlayerUpdate:
                return new global::ServerPacket.PlayerUpdate();
            case (short)ServerPacketIds.PlayerInspect:
                return new global::ServerPacket.PlayerInspect();
            case (short)ServerPacketIds.LogOutSuccess:
                return new global::ServerPacket.LogOutSuccess();
            case (short)ServerPacketIds.LogOutFailed:
                return new global::ServerPacket.LogOutFailed();
            case (short)ServerPacketIds.ReturnToLogin:
                return new global::ServerPacket.ReturnToLogin();
            case (short)ServerPacketIds.TimeOfDay:
                return new global::ServerPacket.TimeOfDay();
            case (short)ServerPacketIds.ChangeAMode:
                return new global::ServerPacket.ChangeAMode();
            case (short)ServerPacketIds.ChangePMode:
                return new global::ServerPacket.ChangePMode();
            case (short)ServerPacketIds.ObjectItem:
                return new global::ServerPacket.ObjectItem();
            case (short)ServerPacketIds.ObjectGold:
                return new global::ServerPacket.ObjectGold();
            case (short)ServerPacketIds.GainedItem:
                return new global::ServerPacket.GainedItem();
            case (short)ServerPacketIds.GainedGold:
                return new global::ServerPacket.GainedGold();
            case (short)ServerPacketIds.LoseGold:
                return new global::ServerPacket.LoseGold();
            case (short)ServerPacketIds.GainedCredit:
                return new global::ServerPacket.GainedCredit();
            case (short)ServerPacketIds.LoseCredit:
                return new global::ServerPacket.LoseCredit();
            case (short)ServerPacketIds.ObjectMonster:
                return new global::ServerPacket.ObjectMonster();
            case (short)ServerPacketIds.ObjectAttack:
                return new global::ServerPacket.ObjectAttack();
            case (short)ServerPacketIds.Struck:
                return new global::ServerPacket.Struck();
            case (short)ServerPacketIds.DamageIndicator:
                return new global::ServerPacket.DamageIndicator();
            case (short)ServerPacketIds.ObjectStruck:
                return new global::ServerPacket.ObjectStruck();
            case (short)ServerPacketIds.DuraChanged:
                return new global::ServerPacket.DuraChanged();
            case (short)ServerPacketIds.HealthChanged:
                return new global::ServerPacket.HealthChanged();
            case (short)ServerPacketIds.DeleteItem:
                return new global::ServerPacket.DeleteItem();
            case (short)ServerPacketIds.Death:
                return new global::ServerPacket.Death();
            case (short)ServerPacketIds.ObjectDied:
                return new global::ServerPacket.ObjectDied();
            case (short)ServerPacketIds.ColourChanged:
                return new global::ServerPacket.ColourChanged();
            case (short)ServerPacketIds.ObjectColourChanged:
                return new global::ServerPacket.ObjectColourChanged();
            case (short)ServerPacketIds.ObjectGuildNameChanged:
                return new global::ServerPacket.ObjectGuildNameChanged();
            case (short)ServerPacketIds.GainExperience:
                return new global::ServerPacket.GainExperience();
            case (short)ServerPacketIds.LevelChanged:
                return new global::ServerPacket.LevelChanged();;
            case (short)ServerPacketIds.ObjectLeveled:
                return new global::ServerPacket.ObjectLeveled();
            case (short)ServerPacketIds.ObjectHarvest:
                return new global::ServerPacket.ObjectHarvest();
            case (short)ServerPacketIds.ObjectHarvested:
                return new global::ServerPacket.ObjectHarvested();
            case (short)ServerPacketIds.ObjectNpc:
                return new global::ServerPacket.ObjectNPC();
            case (short)ServerPacketIds.NPCResponse:
                return new global::ServerPacket.NPCResponse();
            case (short)ServerPacketIds.ObjectHide:
                return new global::ServerPacket.ObjectHide();
            case (short)ServerPacketIds.ObjectShow:
                return new global::ServerPacket.ObjectShow();
            case (short)ServerPacketIds.Poisoned:
                return new global::ServerPacket.Poisoned();
            case (short)ServerPacketIds.ObjectPoisoned:
                return new global::ServerPacket.ObjectPoisoned();
            case (short)ServerPacketIds.MapChanged:
                return new global::ServerPacket.MapChanged();
            case (short)ServerPacketIds.ObjectTeleportOut:
                return new global::ServerPacket.ObjectTeleportOut();
            case (short)ServerPacketIds.ObjectTeleportIn:
                return new global::ServerPacket.ObjectTeleportIn();
            case (short)ServerPacketIds.TeleportIn:
                return new global::ServerPacket.TeleportIn();
            case (short)ServerPacketIds.NPCGoods:
                return new global::ServerPacket.NPCGoods();
            case (short)ServerPacketIds.NPCSell:
                return new global::ServerPacket.NPCSell();
            case (short)ServerPacketIds.NPCRepair:
                return new global::ServerPacket.NPCRepair();
            case (short)ServerPacketIds.NPCSRepair:
                return new global::ServerPacket.NPCSRepair();
            case (short)ServerPacketIds.NPCRefine:
                return new global::ServerPacket.NPCRefine();
            case (short)ServerPacketIds.NPCCheckRefine:
                return new global::ServerPacket.NPCCheckRefine();
            case (short)ServerPacketIds.NPCCollectRefine:
                return new global::ServerPacket.NPCCollectRefine();
            case (short)ServerPacketIds.NPCReplaceWedRing:
                return new global::ServerPacket.NPCReplaceWedRing();
            case (short)ServerPacketIds.NPCStorage:
                return new global::ServerPacket.NPCStorage();
            case (short)ServerPacketIds.SellItem:
                return new global::ServerPacket.SellItem();
            case (short)ServerPacketIds.CraftItem:
                return new global::ServerPacket.CraftItem();
            case (short)ServerPacketIds.RepairItem:
                return new global::ServerPacket.RepairItem();
            case (short)ServerPacketIds.ItemRepaired:
                return new global::ServerPacket.ItemRepaired();
            case (short)ServerPacketIds.ItemSlotSizeChanged:
                return new global::ServerPacket.ItemSlotSizeChanged();
            case (short)ServerPacketIds.ItemSealChanged:
                return new global::ServerPacket.ItemSealChanged();
            case (short)ServerPacketIds.NewMagic:
                return new global::ServerPacket.NewMagic();
            case (short)ServerPacketIds.MagicLeveled:
                return new global::ServerPacket.MagicLeveled();
            case (short)ServerPacketIds.Magic:
                return new global::ServerPacket.Magic();
            case (short)ServerPacketIds.MagicDelay:
                return new global::ServerPacket.MagicDelay();
            case (short)ServerPacketIds.MagicCast:
                return new global::ServerPacket.MagicCast();
            case (short)ServerPacketIds.ObjectMagic:
                return new global::ServerPacket.ObjectMagic();
            case (short)ServerPacketIds.ObjectProjectile:
                return new global::ServerPacket.ObjectProjectile();
            case (short)ServerPacketIds.ObjectEffect:
                return new global::ServerPacket.ObjectEffect();
            case (short)ServerPacketIds.RangeAttack:
                return new global::ServerPacket.RangeAttack();
            case (short)ServerPacketIds.Pushed:
                return new global::ServerPacket.Pushed();
            case (short)ServerPacketIds.ObjectPushed:
                return new global::ServerPacket.ObjectPushed();
            case (short)ServerPacketIds.ObjectName:
                return new global::ServerPacket.ObjectName();
            case (short)ServerPacketIds.UserStorage:
                return new global::ServerPacket.UserStorage();
            case (short)ServerPacketIds.SwitchGroup:
                return new global::ServerPacket.SwitchGroup();
            case (short)ServerPacketIds.DeleteGroup:
                return new global::ServerPacket.DeleteGroup();
            case (short)ServerPacketIds.DeleteMember:
                return new global::ServerPacket.DeleteMember();
            case (short)ServerPacketIds.GroupInvite:
                return new global::ServerPacket.GroupInvite();
            case (short)ServerPacketIds.AddMember:
                return new global::ServerPacket.AddMember();
            case (short)ServerPacketIds.GroupMembersMap:
                return new global::ServerPacket.GroupMembersMap();
            case (short)ServerPacketIds.SendMemberLocation:
                return new global::ServerPacket.SendMemberLocation();
            case (short)ServerPacketIds.Revived:
                return new global::ServerPacket.Revived();
            case (short)ServerPacketIds.ObjectRevived:
                return new global::ServerPacket.ObjectRevived();
            case (short)ServerPacketIds.SpellToggle:
                return new global::ServerPacket.SpellToggle();
            case (short)ServerPacketIds.ObjectHealth:
                return new global::ServerPacket.ObjectHealth();
            case (short)ServerPacketIds.ObjectMana:
                return new global::ServerPacket.ObjectMana();
            case (short)ServerPacketIds.MapEffect:
                return new global::ServerPacket.MapEffect();
            case (short)ServerPacketIds.AllowObserve:
                return new global::ServerPacket.AllowObserve();
            case (short)ServerPacketIds.ObjectRangeAttack:
                return new global::ServerPacket.ObjectRangeAttack();
            case (short)ServerPacketIds.AddBuff:
                return new global::ServerPacket.AddBuff();
            case (short)ServerPacketIds.RemoveBuff:
                return new global::ServerPacket.RemoveBuff();
            case (short)ServerPacketIds.PauseBuff:
                return new global::ServerPacket.PauseBuff();
            case (short)ServerPacketIds.ObjectHidden:
                return new global::ServerPacket.ObjectHidden();
            case (short)ServerPacketIds.RefreshItem:
                return new global::ServerPacket.RefreshItem();
            case (short)ServerPacketIds.ObjectSpell:
                return new global::ServerPacket.ObjectSpell();
            case (short)ServerPacketIds.UserDash:
                return new global::ServerPacket.UserDash();
            case (short)ServerPacketIds.ObjectDash:
                return new global::ServerPacket.ObjectDash();
            case (short)ServerPacketIds.UserDashFail:
                return new global::ServerPacket.UserDashFail();
            case (short)ServerPacketIds.ObjectDashFail:
                return new global::ServerPacket.ObjectDashFail();
            case (short)ServerPacketIds.NPCConsign:
                return new global::ServerPacket.NPCConsign();
             
            case (short)ServerPacketIds.ConsignItem:
                return new global::ServerPacket.ConsignItem();
            
            case (short)ServerPacketIds.ObjectSitDown:
                return new global::ServerPacket.ObjectSitDown();
            case (short)ServerPacketIds.InTrapRock:
                return new global::ServerPacket.InTrapRock();
            case (short)ServerPacketIds.RemoveMagic:
                return new global::ServerPacket.RemoveMagic();
            case (short)ServerPacketIds.BaseStatsInfo:
                return new global::ServerPacket.BaseStatsInfo();
            case (short)ServerPacketIds.UserName:
                return new global::ServerPacket.UserName();
            case (short)ServerPacketIds.ChatItemStats:
                return new global::ServerPacket.ChatItemStats();
            case (short)ServerPacketIds.GuildMemberChange:
                return new global::ServerPacket.GuildMemberChange();
            case (short)ServerPacketIds.GuildNoticeChange:
                return new global::ServerPacket.GuildNoticeChange();
            case (short)ServerPacketIds.GuildStatus:
                return new global::ServerPacket.GuildStatus();
            case (short)ServerPacketIds.GuildInvite:
                return new global::ServerPacket.GuildInvite();
            case (short)ServerPacketIds.GuildExpGain:
                return new global::ServerPacket.GuildExpGain();
            case (short)ServerPacketIds.GuildNameRequest:
                return new global::ServerPacket.GuildNameRequest();
            case (short)ServerPacketIds.GuildStorageGoldChange:
                return new global::ServerPacket.GuildStorageGoldChange();
            case (short)ServerPacketIds.GuildStorageItemChange:
                return new global::ServerPacket.GuildStorageItemChange();
            case (short)ServerPacketIds.GuildStorageList:
                return new global::ServerPacket.GuildStorageList();
            case (short)ServerPacketIds.GuildRequestWar:
                return new global::ServerPacket.GuildRequestWar(); 
            case (short)ServerPacketIds.DefaultNPC:
                return new global::ServerPacket.DefaultNPC();
            case (short)ServerPacketIds.NPCUpdate:
                return new global::ServerPacket.NPCUpdate();
            case (short)ServerPacketIds.NPCImageUpdate:
                return new global::ServerPacket.NPCImageUpdate();
            case (short)ServerPacketIds.MarriageRequest:
                return new global::ServerPacket.MarriageRequest();
            case (short)ServerPacketIds.DivorceRequest:
                return new global::ServerPacket.DivorceRequest();
            case (short)ServerPacketIds.MentorRequest:
                return new global::ServerPacket.MentorRequest();
            case (short)ServerPacketIds.TradeRequest:
                return new global::ServerPacket.TradeRequest();
            case (short)ServerPacketIds.TradeAccept:
                return new global::ServerPacket.TradeAccept();
            case (short)ServerPacketIds.TradeGold:
                return new global::ServerPacket.TradeGold();
            case (short)ServerPacketIds.TradeItem:
                return new global::ServerPacket.TradeItem();
            case (short)ServerPacketIds.TradeConfirm:
                return new global::ServerPacket.TradeConfirm();
            case (short)ServerPacketIds.TradeCancel:
                return new global::ServerPacket.TradeCancel();
             
            case (short)ServerPacketIds.TransformUpdate:
                return new global::ServerPacket.TransformUpdate();
            case (short)ServerPacketIds.EquipSlotItem:
                return new global::ServerPacket.EquipSlotItem();
            case (short)ServerPacketIds.FishingUpdate:
                return new global::ServerPacket.FishingUpdate();
            case (short)ServerPacketIds.ChangeQuest:
                return new global::ServerPacket.ChangeQuest();
            case (short)ServerPacketIds.CompleteQuest:
                return new global::ServerPacket.CompleteQuest();
            case (short)ServerPacketIds.ShareQuest:
                return new global::ServerPacket.ShareQuest();
            case (short)ServerPacketIds.NewQuestInfo:
                return new global::ServerPacket.NewQuestInfo();
            case (short)ServerPacketIds.GainedQuestItem:
                return new global::ServerPacket.GainedQuestItem();
            case (short)ServerPacketIds.DeleteQuestItem:
                return new global::ServerPacket.DeleteQuestItem();
            case (short)ServerPacketIds.CancelReincarnation:
                return new global::ServerPacket.CancelReincarnation();
            case (short)ServerPacketIds.RequestReincarnation:
                return new global::ServerPacket.RequestReincarnation();
            case (short)ServerPacketIds.UserBackStep:
                return new global::ServerPacket.UserBackStep();
            case (short)ServerPacketIds.ObjectBackStep:
                return new global::ServerPacket.ObjectBackStep();
            case (short)ServerPacketIds.UserDashAttack:
                return new global::ServerPacket.UserDashAttack();
            case (short)ServerPacketIds.ObjectDashAttack:
                return new global::ServerPacket.ObjectDashAttack();
            case (short)ServerPacketIds.UserAttackMove://Warrior Skill - SlashingBurst
                return new global::ServerPacket.UserAttackMove();
            case (short)ServerPacketIds.CombineItem:
                return new global::ServerPacket.CombineItem();
            case (short)ServerPacketIds.ItemUpgraded:
                return new global::ServerPacket.ItemUpgraded();
            case (short)ServerPacketIds.SetConcentration:
                return new global::ServerPacket.SetConcentration();
            case (short)ServerPacketIds.SetElemental:
                return new global::ServerPacket.SetElemental();
            case (short)ServerPacketIds.RemoveDelayedExplosion:
                return new global::ServerPacket.RemoveDelayedExplosion();
            case (short)ServerPacketIds.ObjectDeco:
                return new global::ServerPacket.ObjectDeco();
            case (short)ServerPacketIds.ObjectSneaking:
                return new global::ServerPacket.ObjectSneaking();
            case (short)ServerPacketIds.ObjectLevelEffects:
                return new global::ServerPacket.ObjectLevelEffects();
            case (short)ServerPacketIds.SetBindingShot:
                return new global::ServerPacket.SetBindingShot();
            case (short)ServerPacketIds.SendOutputMessage:
                return new global::ServerPacket.SendOutputMessage();
             
            case (short)ServerPacketIds.ResizeInventory:
                return new global::ServerPacket.ResizeInventory();
            case (short)ServerPacketIds.ResizeStorage:
                return new global::ServerPacket.ResizeStorage();
            case (short)ServerPacketIds.NPCPearlGoods:
                return new global::ServerPacket.NPCPearlGoods();
            case (short)ServerPacketIds.FriendUpdate:
                return new global::ServerPacket.FriendUpdate();
            case (short)ServerPacketIds.LoverUpdate:
                return new global::ServerPacket.LoverUpdate();
            case (short)ServerPacketIds.MentorUpdate:
                return new global::ServerPacket.MentorUpdate();
            case (short)ServerPacketIds.GuildBuffList:
                return new global::ServerPacket.GuildBuffList();
            case (short)ServerPacketIds.GameShopInfo:
                return new global::ServerPacket.GameShopInfo();
            case (short)ServerPacketIds.GameShopStock:
                return new global::ServerPacket.GameShopStock();
            case (short)ServerPacketIds.NPCRequestInput:
                return new global::ServerPacket.NPCRequestInput();
            case (short)ServerPacketIds.Rankings:
                return new global::ServerPacket.Rankings();
            case (short)ServerPacketIds.Opendoor:
                return new global::ServerPacket.Opendoor();
            
            case (short)ServerPacketIds.NewRecipeInfo:
                return new global::ServerPacket.NewRecipeInfo();
            case (short)ServerPacketIds.OpenBrowser:
                return new global::ServerPacket.OpenBrowser();
            case (short)ServerPacketIds.PlaySound:
                return new global::ServerPacket.PlaySound();
            case (short)ServerPacketIds.SetTimer:
                return new global::ServerPacket.SetTimer();
            case (short)ServerPacketIds.ExpireTimer:
                return new global::ServerPacket.ExpireTimer();
            case (short)ServerPacketIds.UpdateNotice:
                return new global::ServerPacket.UpdateNotice();
            case (short)ServerPacketIds.Roll:
                return new global::ServerPacket.Roll();
            case (short)ServerPacketIds.SetCompass:
                return new global::ServerPacket.SetCompass();
            case (short)ServerPacketIds.ObjectRest:
                return new global::ServerPacket.ObjectRest();
            default:
                return null;
        }
    }
}
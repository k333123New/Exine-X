
using Server.ExineDatabase;
using Server.ExineEnvir;
using Server.ExineNetwork;

using System.Text.RegularExpressions;
using Timer = Server.ExineEnvir.Timer;
using Server.ExineObjects.Monsters;
using System.Threading;

namespace Server.ExineObjects
{
    public class PlayerObjectSrv : HumanObjectSrv
    {
        private long NextTradeTime;
        private long NextGroupInviteTime;

        public string GMPassword = Settings.GMPassword;
        public bool GMLogin, EnableGroupRecall, EnableGuildInvite, AllowMarriage, AllowLoverRecall, AllowMentor, HasMapShout, HasServerShout; //TODO - Remove        

        public long LastRecallTime, LastTeleportTime, LastProbeTime;
        public long MenteeEXP;        

        public bool WarZone = false;

         

        protected AccountInfo account;
        public virtual AccountInfo Account
        {
            get { return account; }
            set { account = value; }
        }       
        
        public override bool CanMove
        {
            get
            {
                return base.CanMove && !Fishing;
            }
        }
        public override bool CanWalk
        {
            get
            {
                return base.CanMove && !Fishing;
            }
        }
        public override bool CanAttack
        {
            get
            {
                return base.CanAttack && !Fishing;
            }
        }
        protected override bool CanCast
        {
            get
            {
                return base.CanCast && !Fishing;
            }
        }

        

        public override int PKPoints
        {
            get { return Info.PKPoints; }
            set { Info.PKPoints = value; }
        }        

        public int BindMapIndex
        {
            get { return Info.BindMapIndex; }
            set { Info.BindMapIndex = value; }
        }
        public Point BindLocation
        {
            get { return Info.BindLocation; }
            set { Info.BindLocation = value; }
        }        

        public int FishingChance, FishingChanceCounter, FishingProgressMax, FishingProgress, FishingAutoReelChance = 0, FishingNibbleChance = 0;
        public bool Fishing, FishingAutocast, FishFound, FishFirstFound;

        public const long TurnDelay = 350, HarvestDelay = 350, FishingCastDelay = 750, FishingDelay = 200, MovementDelay = 2000;
        public long ChatTime, ShoutTime, FishingTime, FishingFoundTime, CreatureTimeLeftTicker, RestedTime, MovementTime;

        public byte ChatTick; 

        public bool SendIntelligentCreatureUpdates = false;        

        protected int _fishCounter, _restedCounter;

        public uint NPCObjectID;
        public int NPCScriptID;
        public NPCPage NPCPage;
        public Dictionary<NPCSegment, bool> NPCSuccess = new Dictionary<NPCSegment, bool>();
        public bool NPCDelayed;
        public List<string> NPCSpeech = new List<string>();
        public Dictionary<string, object> NPCData = new Dictionary<string, object>();

        public bool UserMatch;
        public string MatchName;
        public ItemType MatchType;
        public MarketPanelType MarketPanelType;
        public short MinShapes, MaxShapes;

        public int PageSent;    

        public bool CanCreateGuild = false;        
        
        public GuildObjectSrv PendingGuildInvite = null;
        public bool GuildNoticeChanged = true; //set to false first time client requests notice list, set to true each time someone in guild edits notice
        public bool GuildMembersChanged = true;//same as above but for members
        public bool GuildCanRequestItems = true;
        public bool RequestedGuildBuffInfo = false;
           
        public bool AllowGroup
        {
            get { return Info.AllowGroup; }
            set { Info.AllowGroup = value; }
        }

        public bool AllowTrade
        {
            get { return Info.AllowTrade; }
            set { Info.AllowTrade = value; }
        }

        public bool AllowObserve
        {
            get { return Info.AllowObserve; }
            set { Info.AllowObserve = value; }
        }

        public PlayerObjectSrv MarriageProposal;
        public PlayerObjectSrv DivorceProposal;
        public PlayerObjectSrv MentorRequest;

        public PlayerObjectSrv GroupInvitation;
        public PlayerObjectSrv TradeInvitation;

        public PlayerObjectSrv TradePartner = null;
        public bool TradeLocked = false;
        public uint TradeGoldAmount = 0;


        private long LastRankUpdate = Envir.Time;

        public List<QuestProgressInfo> CurrentQuests
        {
            get { return Info.CurrentQuests; }
        }

        public List<int> CompletedQuests
        {
            get { return Info.CompletedQuests; }
        }

        public PlayerObjectSrv() { }

        public PlayerObjectSrv(CharacterInfo info, ExineConnection connection) : base(info, connection) { }
        protected override void Load(CharacterInfo info, ExineConnection connection)
        {
            if (info.Player != null)
            {
                throw new InvalidOperationException("Player.Info not Null.");
            }

            info.Player = this;
             

            Connection = connection;
            Info = info;
            Account = Connection.Account;

            Stats = new Stats();

            Report = new Reporting(this);

            if (Account.AdminAccount)
            {
                IsGM = true;
                MessageQueue.SendMsg(string.Format("{0} is now a GM", Name));
            }

            //Set Starting Character
            if (Level == 0)
            {
                NewCharacter();
                Account.Gold = 10000; //add k333123 241004 starting gold set for test
            }

            if (Info.GuildIndex != -1)
            {
                MyGuild = Envir.GetGuild(Info.GuildIndex);
            }

           

            RefreshStats();

            if (HP == 0)
            {
                SetHP(Stats[Stat.HP]);
                SetMP(Stats[Stat.MP]);

                CurrentLocation = BindLocation;
                CurrentMapIndex = BindMapIndex;

                if (Info.PKPoints >= 200)
                {
                    Map temp = Envir.GetMapByNameAndInstance(Settings.PKTownMapName, 1);
                    Point tempLocation = new Point(Settings.PKTownPositionX, Settings.PKTownPositionY);

                    if (temp != null && temp.ValidPoint(tempLocation))
                    {
                        CurrentMapIndex = temp.Info.Index;
                        CurrentLocation = tempLocation;
                    }
                }
            }

            Info.LastLoginDate = Envir.Now;
        }

        public void StopGame(byte reason)
        {
            if (Node == null) return;

            
            
            for (int i = 0; i < Info.Magics.Count; i++)
            {
                var magic = Info.Magics[i];

                if (Envir.Time < (magic.CastTime + magic.GetDelay()))
                {
                    magic.CastTime -= Envir.Time;
                }
                else
                {
                    magic.CastTime = int.MinValue;
                }
            }

            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                var buff = Buffs[i];
                buff.Caster = null;
                buff.ObjectID = 0;

                if (buff.Properties.HasFlag(BuffProperty.RemoveOnExit))
                {
                    Buffs.RemoveAt(i);
                }
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                var poison = PoisonList[i];
                poison.Owner = null;
            }

            if (MyGuild != null)
            {
                MyGuild.PlayerLogged(this, false);
            }

            Envir.Players.Remove(this);
            CurrentMap.RemoveObject(this);

            Despawn();
            LeaveGroup();
            TradeCancel(); 
            RefineCancel();
            LogoutRelationship();
            LogoutMentor();

            string logReason = LogOutReason(reason);

            MessageQueue.SendMsg(logReason);

            Fishing = false;

            Info.LastIP = Connection.IPAddress;
            Info.LastLogoutDate = Envir.Now;

            Report.Disconnected(logReason);
            Connection.WorldMapSetupSent = false;

            if (!IsGM)
            {
                Envir.OnlineRankingCount[0]--;
                Envir.OnlineRankingCount[(int)Class + 1]--;
            }

            CleanUp();
        }
        private string LogOutReason(byte reason)
        {
            switch (reason)
            {
                //0-10 are 'senddisconnect to client'
                case 0:
                    return string.Format("{0} Has logged out. Reason: Server closed", Name);
                case 1:
                    return string.Format("{0} Has logged out. Reason: Double login", Name);
                case 2:
                    return string.Format("{0} Has logged out. Reason: Chat message too long", Name);
                case 3:
                    return string.Format("{0} Has logged out. Reason: Server crashed", Name);
                case 4:
                    return string.Format("{0} Has logged out. Reason: Kicked by admin", Name);
                case 5:
                    return string.Format("{0} Has logged out. Reason: Maximum connections reached", Name);
                case 10:
                    return string.Format("{0} Has logged out. Reason: Wrong client version", Name);
                case 20:
                    return string.Format("{0} Has logged out. Reason: User gone missing / disconnected", Name);
                case 21:
                    return string.Format("{0} Has logged out. Reason: Connection timed out", Name);
                case 22:
                    return string.Format("{0} Has logged out. Reason: User closed game", Name);
                case 23:
                    return string.Format("{0} Has logged out. Reason: User returned to select char", Name);
                case 24:
                    return string.Format("{0} Has logged out. Reason: Began observing", Name);
                default:
                    return string.Format("{0} Has logged out. Reason: Unknown", Name);
            }
        }
        protected override void NewCharacter()
        {
            if (Envir.StartPoints.Count == 0) return;

            SetBind();

            base.NewCharacter();
        }
        public override void Process()
        {
            if (Connection == null || Node == null || Info == null) return;

            if (GroupInvitation != null && GroupInvitation.Node == null)
                GroupInvitation = null;

            base.Process();

            if (Settings.RestedPeriod > 0 && Envir.Time > RestedTime)
            {
                _restedCounter = InSafeZone ? _restedCounter + 1 : _restedCounter;

                if (_restedCounter > 0)
                {
                    int count = _restedCounter / (Settings.RestedPeriod * 60);

                    GiveRestedBonus(count);
                }

                RestedTime = Envir.Time + Settings.Second;
            }
             
            if (Account.HasExpandedStorage && Envir.Now > Account.ExpandedStorageExpiryDate)
            {
                Account.HasExpandedStorage = false;
                ReceiveChat("Expanded storage has expired.", ChatType.System);
                SendPacketToClient(new ServerPacket.ResizeStorage { Size = Account.Storage.Length, HasExpandedStorage = Account.HasExpandedStorage, ExpiryTime = Account.ExpandedStorageExpiryDate });
            }

            if (Fishing && Envir.Time > FishingTime)
            {
                _fishCounter++;
                UpdateFish();
            }
             
        }
        public override void Process(DelayedAction action)
        {
            if (action.FlaggedToRemove)
                return;

            switch (action.Type)
            {
                case DelayedType.Magic:
                    CompleteMagic(action.Params);
                    break;
                case DelayedType.Damage:
                    CompleteAttack(action.Params);
                    break;
                case DelayedType.MapMovement:
                    CompleteMapMovement(action.Params);
                    break;
                case DelayedType.Mine:
                    CompleteMine(action.Params);
                    break;
                case DelayedType.NPC:
                    CompleteNPC(action.Params);
                    break;
                case DelayedType.Poison:
                    CompletePoison(action.Params);
                    break;
                case DelayedType.DamageIndicator:
                    CompleteDamageIndicator(action.Params);
                    break;
                case DelayedType.Quest:
                    CompleteQuest(action.Params);
                    break;
                case DelayedType.SpellEffect:
                    CompleteSpellEffect(action.Params);
                    break;
            }
        }
        protected override void Moved()
        {
            base.Moved();
            CheckConquest();
            if (TradePartner != null)
                TradeCancel(); 
        }
        public override void Die()
        {
            if (SpecialMode.HasFlag(SpecialItemMode.Revival) && Envir.Time > LastRevivalTime)
            {
                LastRevivalTime = Envir.Time + 300000;

                for (var i = (int)EquipmentSlot.RingL; i <= (int)EquipmentSlot.RingR; i++)
                {
                    var item = Info.Equipment[i];

                    if (item == null) continue;
                    if (!(item.Info.Unique.HasFlag(SpecialItemMode.Revival)) || item.CurrentDura < 1000) continue;
                    SetHP(Stats[Stat.HP]);
                    item.CurrentDura = (ushort)(item.CurrentDura - 1000);
                    SendPacketToClient(new ServerPacket.DuraChanged { UniqueID = item.UniqueID, CurrentDura = item.CurrentDura });
                    RefreshStats();
                    ReceiveChat("You have been given a second chance at life", ChatType.System);
                    return;
                }
            }

            if (LastHitter != null && LastHitter.Race == ObjectType.Player)
            {
                PlayerObjectSrv hitter = (PlayerObjectSrv)LastHitter;

                if (AtWar(hitter) || WarZone)
                {
                    hitter.ReceiveChat(string.Format("You've been protected by the law"), ChatType.System);
                }
                else if (Envir.Time > BrownTime && PKPoints < 200)
                {
                    UserItem weapon = hitter.Info.Equipment[(byte)EquipmentSlot.Weapon];

                    hitter.PKPoints = Math.Min(int.MaxValue, LastHitter.PKPoints + 100);
                    hitter.ReceiveChat(string.Format("You have murdered {0}", Name), ChatType.System);
                    ReceiveChat(string.Format("You have been murdered by {0}", LastHitter.Name), ChatType.System);

                    if (weapon != null && weapon.AddedStats[Stat.Luck] > (Settings.MaxLuck * -1) && Envir.Random.Next(4) == 0)
                    {
                        weapon.AddedStats[Stat.Luck]--;
                        hitter.ReceiveChat("Your weapon has been cursed.", ChatType.System);
                        hitter.SendPacketToClient(new ServerPacket.RefreshItem { Item = weapon });
                    }
                }
            }

            RemoveBuff(BuffType.MagicShield);
            RemoveBuff(BuffType.ElementalBarrier);

            if (PKPoints > 200)
                RedDeathDrop(LastHitter);
            else if (!InSafeZone)
                DeathDrop(LastHitter);

            HP = 0;
            Dead = true;

            LogTime = Envir.Time;
            BrownTime = Envir.Time;

            SendPacketToClient(new ServerPacket.Death { Direction = Direction, Location = CurrentLocation });
            Broadcast(new ServerPacket.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            for (int i = 0; i < Buffs.Count; i++)
            {
                Buff buff = Buffs[i];

                if (!buff.Properties.HasFlag(BuffProperty.RemoveOnDeath)) continue;

                RemoveBuff(buff.Type);
            }

            PoisonList.Clear();
            InTrapRock = false;

            CallDefaultNPC(DefaultNPCType.Die);

            Report.Died(CurrentMap.Info.FileName);
        }
        private void RedDeathDrop(MapObjectSrv killer)
        {
            if (killer == null || killer.Race != ObjectType.Player)
            {
                for (var i = 0; i < Info.Equipment.Length; i++)
                {
                    var item = Info.Equipment[i];

                    if (item == null)
                        continue;

                    if (item.Info.Bind.HasFlag(BindMode.DontDeathdrop))
                        continue;

                    // TODO: Check this.
                    if ((item.WeddingRing != -1) && (Info.Equipment[(int)EquipmentSlot.RingL].UniqueID == item.UniqueID))
                        continue;

                    if (item.SealedInfo != null && item.SealedInfo.ExpiryDate > Envir.Now)
                        continue;

                    if (item.Info.Bind.HasFlag(BindMode.BreakOnDeath))
                    {
                        Info.Equipment[i] = null;
                        SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                        ReceiveChat($"Your {item.FriendlyName} shattered upon death.", ChatType.System2);
                        Report.ItemChanged(item, item.Count, 1, "RedDeathDrop");
                    }

                    if (item.Count > 1)
                    {
                        var percent = Envir.RandomomRange(10, 4);
                        var count = (ushort)Math.Ceiling(item.Count / 10F * percent);

                        if (count > item.Count)
                            throw new ArgumentOutOfRangeException();

                        var temp2 = Envir.CreateFreshItem(item.Info);
                        temp2.Count = count;

                        if (!DropItem(temp2, Settings.DropRange, true))
                            continue;

                        if (count == item.Count)
                            Info.Equipment[i] = null;

                        SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = count });
                        item.Count -= count;

                        Report.ItemChanged(item, count, 1, "RedDeathDrop");
                    }
                    else if (Envir.Random.Next(10) == 0)
                    {
                       

                        if (!DropItem(item, Settings.DropRange, true))
                            continue;

                        if (item.Info.GlobalDropNotify)
                            foreach (var player in Envir.Players)
                            {
                                player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
                            }

                        Info.Equipment[i] = null;
                        SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });

                        Report.ItemChanged(item, item.Count, 1, "RedDeathDrop");
                    }
                }

            }

            for (var i = 0; i < Info.Inventory.Length; i++)
            {
                var item = Info.Inventory[i];

                if (item == null)
                    continue;

                if (item.Info.Bind.HasFlag(BindMode.DontDeathdrop))
                    continue;

                if (item.WeddingRing != -1)
                    continue;

                if (item.SealedInfo != null && item.SealedInfo.ExpiryDate > Envir.Now)
                    continue;

               

                if (!DropItem(item, Settings.DropRange, true))
                    continue;

                if (item.Info.GlobalDropNotify)
                    foreach (var player in Envir.Players)
                    {
                        player.ReceiveChat($"{Name} has dropped {item.FriendlyName}.", ChatType.System2);
                    }

                Info.Inventory[i] = null;
                SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });

                Report.ItemChanged(item, item.Count, 1, "RedDeathDrop");
            }

            RefreshStats();
        }        
        public override void WinExp(uint amount, uint targetLevel = 0)
        {
            int expPoint;
            uint originalAmount = amount;

            expPoint = ReduceExp(amount, targetLevel);
            expPoint = (int)(expPoint * Settings.ExpRate);            

            //party
            float[] partyExpRate = { 1.0F, 1.3F, 1.4F, 1.5F, 1.6F, 1.7F, 1.8F, 1.9F, 2F, 2.1F, 2.2F };

            if (GroupMembers != null)
            {
                int sumLevel = 0;
                int nearCount = 0;
                for (int i = 0; i < GroupMembers.Count; i++)
                {
                    PlayerObjectSrv player = GroupMembers[i];

                    if (Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        sumLevel += player.Level;
                        nearCount++;
                    }
                }

                if (nearCount > partyExpRate.Length) nearCount = partyExpRate.Length;

                for (int i = 0; i < GroupMembers.Count; i++)
                {
                    PlayerObjectSrv player = GroupMembers[i];
                    if (player.CurrentMap == CurrentMap &&
                        Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) && !player.Dead)
                    {
                        player.GainExp((uint)((float)expPoint * partyExpRate[nearCount - 1] * (float)player.Level / (float)sumLevel));
                    }
                }
            }
            else
                GainExp((uint)expPoint);

             
        }

        //240827 exp 1 to 0!
        public override void GainExp(uint amount)
        {
            if (!CanGainExp) return;

            if (amount == 0) return;

            if (Info.Married != 0)
            {
                if (HasBuff(BuffType.Lover, out Buff buff))
                {
                    CharacterInfo lover = Envir.GetCharacterInfo(Info.Married);
                    PlayerObjectSrv loverPlayer = Envir.GetPlayer(lover.Name);
                    if (loverPlayer != null && loverPlayer.CurrentMap == CurrentMap && Functions.InRange(loverPlayer.CurrentLocation, CurrentLocation, Globals.DataRange) && !loverPlayer.Dead)
                    {
                        amount += (uint)Math.Max(0, (amount * Stats[Stat.LoverExpRatePercent]) / 100);
                    }
                }
            }

            if (Info.Mentor != 0 && !Info.IsMentor)
            {
                if (HasBuff(BuffType.Mentee, out _))
                {
                    CharacterInfo mentor = Envir.GetCharacterInfo(Info.Mentor);
                    PlayerObjectSrv mentorPlayer = Envir.GetPlayer(mentor.Name);
                    if (mentorPlayer != null && mentorPlayer.CurrentMap == CurrentMap && Functions.InRange(mentorPlayer.CurrentLocation, CurrentLocation, Globals.DataRange) && !mentorPlayer.Dead)
                    {
                        if (GroupMembers != null && GroupMembers.Contains(mentorPlayer))
                            amount += (uint)Math.Max(0, (amount * Stats[Stat.MentorExpRatePercent]) / 100);
                    }
                }
            }

            if (Stats[Stat.ExpRatePercent] > 0)
            {
                amount += (uint)Math.Max(0, (amount * Stats[Stat.ExpRatePercent]) / 100);
            }

            if (Info.Mentor != 0 && !Info.IsMentor)
            {
                MenteeEXP += (amount * Settings.MenteeExpBank) / 100;
            }

            if (amount == 1) return; //240827 k333123 add

            Experience += amount;

            SendPacketToClient(new ServerPacket.GainExperience { Amount = amount });

           

            if (MyGuild != null && MyGuild.Name != Settings.NewbieGuild)
                MyGuild.GainExp(amount);

            if (Experience < MaxExperience) return;
            if (Level >= ushort.MaxValue) return;

            //Calculate increased levels
            var experience = Experience;

            while (experience >= MaxExperience)
            {
                Level++;
                experience -= MaxExperience;

                RefreshLevelStats();

                if (Level >= ushort.MaxValue) break;
            }

            Experience = experience;

            LevelUp();

            if (IsGM) return;
            if ((LastRankUpdate + 3600 * 1000) > Envir.Time)
            {
                LastRankUpdate = Envir.Time;
                Envir.CheckRankUpdate(Info);
            }
        }
        public override void LevelUp()
        {
            CallDefaultNPC(DefaultNPCType.LevelUp);

            base.LevelUp();

            SendPacketToClient(new ServerPacket.LevelChanged { Level = Level, Experience = Experience, MaxExperience = MaxExperience });

            if (Info.Mentor != 0 && !Info.IsMentor)
            {
                CharacterInfo Mentor = Envir.GetCharacterInfo(Info.Mentor);
                if ((Mentor != null) && ((Info.Level + Settings.MentorLevelGap) > Mentor.Level))
                    MentorBreak();
            }

            for (int i = CurrentMap.NPCs.Count - 1; i >= 0; i--)
            {
                if (Functions.InRange(CurrentMap.NPCs[i].CurrentLocation, CurrentLocation, Globals.DataRange))
                    CurrentMap.NPCs[i].CheckVisible(this);
            }
            Report.Levelled(Level);

            if (IsGM) return;
            Envir.CheckRankUpdate(Info);
        }        
        private void AddQuestItem(UserItem item)
        {
            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < Info.QuestInventory.Length; i++)
                {
                    UserItem temp = Info.QuestInventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= (ushort)(temp.Info.StackSize - temp.Count);
                    temp.Count = temp.Info.StackSize;
                }
            }

            for (int i = 0; i < Info.QuestInventory.Length; i++)
            {
                if (Info.QuestInventory[i] != null) continue;
                Info.QuestInventory[i] = item;

                return;
            }
        }
        public void CheckQuestInfo(QuestInfo info)
        {
            if (Connection.SentQuestInfo.Contains(info)) return;
            SendPacketToClient(new ServerPacket.NewQuestInfo { Info = info.CreateClientQuestInfo() });
            Connection.SentQuestInfo.Add(info);
        }
        public void CheckRecipeInfo(RecipeInfo info)
        {
            if (Connection.SentRecipeInfo.Contains(info)) return;

            CheckItemInfo(info.Item.Info);

            foreach (var tool in info.Tools)
            {
                CheckItemInfo(tool.Info);
            }

            foreach (var ingredient in info.Ingredients)
            {
                CheckItemInfo(ingredient.Info);
            }

            SendPacketToClient(new ServerPacket.NewRecipeInfo { Info = info.CreateClientRecipeInfo() });
            Connection.SentRecipeInfo.Add(info);
        }
        public void CheckMapInfo(MapInfo mapInfo)
        {
            if (!Connection.WorldMapSetupSent)
            {
                SendPacketToClient(new ServerPacket.WorldMapSetupInfo { Setup = Settings.WorldMapSetup, TeleportToNPCCost = Settings.TeleportToNPCCost });
                Connection.WorldMapSetupSent = true;
            }

            if (Connection.SentMapInfo.Contains(mapInfo)) return;

            var map = Envir.GetMap(mapInfo.Index);
            if (map == null) return;

            var info = new ClientMapInfo()
            {
                Width = map.Width,
                Height = map.Height,
                BigMap = mapInfo.BigMap,
                Title = mapInfo.Title
            };

            foreach (MovementInfo mInfo in mapInfo.Movements.Where(x => x.ShowOnBigMap))
            {
                Map destMap = Envir.GetMap(mInfo.MapIndex);
                if (destMap is null)
                    continue;
                var cmInfo = new ClientMovementInfo()
                {
                    Destination = mInfo.MapIndex,
                    Location = mInfo.Source,
                    Icon = mInfo.Icon
                };
                
                cmInfo.Title = destMap.Info.Title;

                info.Movements.Add(cmInfo);
            }

            foreach (NPCObjectSrv npc in Envir.NPCs.Where(x => x.CurrentMap == map && x.Info.ShowOnBigMap).OrderBy(x => x.Info.BigMapIcon))
            {
                info.NPCs.Add(new ClientNPCInfo()
                {
                    ObjectID = npc.ObjectID,
                    Name = npc.Info.Name,
                    Location = npc.Info.Location,
                    Icon = npc.Info.BigMapIcon,
                    CanTeleportTo = npc.Info.CanTeleportTo
                });
            }

            SendPacketToClient(new ServerPacket.NewMapInfo { MapIndex = mapInfo.Index, Info = info });
            Connection.SentMapInfo.Add(mapInfo);
        }
        private void SetBind()
        {
            SafeZoneInfo szi = Envir.StartPoints[Envir.Random.Next(Envir.StartPoints.Count)];

            //BindMapIndex = szi.Info.Index;
            BindMapIndex = szi.InfoIndex;//k333123 240828 for jsonDB
            BindLocation = szi.Location;
        }
        protected override void SetBindSafeZone(SafeZoneInfo szi)
        {
            BindLocation = szi.Location;
            BindMapIndex = CurrentMapIndex;
        }
        public void StartGame()
        {
            Map temp = Envir.GetMap(CurrentMapIndex);

            if (temp != null && temp.Info.NoReconnect)
            {
                Map temp1 = Envir.GetMapByNameAndInstance(temp.Info.NoReconnectMap);
                if (temp1 != null)
                {
                    temp = temp1;
                    CurrentLocation = GetRandomPoint(40, 0, temp);
                }
            }

            if (temp == null || !temp.ValidPoint(CurrentLocation))
            {
                temp = Envir.GetMap(BindMapIndex);

                if (temp == null || !temp.ValidPoint(BindLocation))
                {
                    SetBind();
                    temp = Envir.GetMap(BindMapIndex);

                    if (temp == null || !temp.ValidPoint(BindLocation))
                    {
                        StartGameFailed();
                        return;
                    }
                }
                CurrentMapIndex = BindMapIndex;
                CurrentLocation = BindLocation;
            }
            temp.AddObject(this);
            CurrentMap = temp;
            Envir.Players.Add(this);

            StartGameSuccess();

            //Call Login NPC
            CallDefaultNPC(DefaultNPCType.Login);

            //Call Daily NPC
            if (Info.NewDay)
            {
                CallDefaultNPC(DefaultNPCType.Daily);
            }
        }
        private void StartGameSuccess()
        {
            Connection.Stage = GameStage.Game;

            SendPacketToClient(new ServerPacket.StartGame { Result = 4, Resolution = Settings.AllowedResolution });
            ReceiveChat(string.Format(GameLanguage.Welcome, GameLanguage.GameName), ChatType.Hint);

            if (Settings.TestServer)
            {
                ReceiveChat("Game is currently in test mode.", ChatType.Hint);
                Chat("@GAMEMASTER");
            }

            for (int i = 0; i < Info.Magics.Count; i++)
            {
                var magic = Info.Magics[i];
                magic.CastTime += Envir.Time;

                if (magic.CastTime + magic.GetDelay() < Envir.Time)
                {
                    magic.CastTime = int.MinValue;
                }
            }

            if (Info.GuildIndex != -1)
            {
                if (MyGuild == null)
                {
                    Info.GuildIndex = -1;
                    ReceiveChat("You have been removed from the guild.", ChatType.System);
                }
                else
                {
                    MyGuildRank = MyGuild.FindRank(Info.Name);
                    if (MyGuildRank == null)
                    {
                        MyGuild = null;
                        Info.GuildIndex = -1;
                        ReceiveChat("You have been removed from the guild.", ChatType.System);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(Settings.Notice.Message) && Settings.Notice.LastUpdate > Info.LastLogoutDate)
            {
                SendPacketToClient(new ServerPacket.UpdateNotice { Notice = Settings.Notice });
            }

            Spawned();

            SetLevelEffects();

            GetItemInfo(Connection);
            GetMapInfo(Connection);
            GetUserInfo(Connection);
            GetQuestInfo();
            GetRecipeInfo();

            GetCompletedQuests();
             
            GetFriends();
            GetRelationship();

            if (Info.Mentor != 0 && Info.MentorDate.AddDays(Settings.MentorLength) < Envir.Now)
            {
                MentorBreak();
            }
            else
            {
                GetMentor();
            }

            CheckConquest();

            GetGameShop();

            for (int i = 0; i < CurrentQuests.Count; i++)
            {
                var quest = CurrentQuests[i];
                quest.Init(this);
                SendUpdateQuest(quest, QuestState.Add);
            }

            SendBaseStats();
            GetObjectsPassive();
            SendPacketToClient(new ServerPacket.TimeOfDay { Lights = Envir.Lights });
            SendPacketToClient(new ServerPacket.ChangeAMode { Mode = AMode });
            SendPacketToClient(new ServerPacket.ChangePMode { Mode = PMode });
            SendPacketToClient(new ServerPacket.SwitchGroup { AllowGroup = AllowGroup });

            SendPacketToClient(new ServerPacket.DefaultNPC { ObjectID = Envir.DefaultNPC.LoadedObjectID });

            SendPacketToClient(new ServerPacket.GuildBuffList() { GuildBuffs = Settings.Guild_BuffList });
            RequestedGuildBuffInfo = true;

            if (Info.Thrusting) SendPacketToClient(new ServerPacket.SpellToggle { ObjectID = ObjectID, Spell = Spell.Thrusting, CanUse = true });
            if (Info.HalfMoon) SendPacketToClient(new ServerPacket.SpellToggle { ObjectID = ObjectID, Spell = Spell.HalfMoon, CanUse = true });
            if (Info.CrossHalfMoon) SendPacketToClient(new ServerPacket.SpellToggle { ObjectID = ObjectID, Spell = Spell.CrossHalfMoon, CanUse = true });
            if (Info.DoubleSlash) SendPacketToClient(new ServerPacket.SpellToggle { ObjectID = ObjectID, Spell = Spell.DoubleSlash, CanUse = true });

          

            for (int i = 0; i < Buffs.Count; i++)
            {
                var buff = Buffs[i];
                buff.LastTime = Envir.Time;
                buff.ObjectID = ObjectID;

                AddBuff(buff.Type, null, (int)buff.ExpireTime, buff.Stats, true, true, buff.Values);   
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                var poison = PoisonList[i];
                poison.TickTime = Envir.Time;
            }

            if (MyGuild != null)
            {
                MyGuild.PlayerLogged(this, true);
                if (MyGuild.BuffList.Count > 0)
                {
                    SendPacketToClient(new ServerPacket.GuildBuffList() { ActiveBuffs = MyGuild.BuffList });
                }
            }
             
            if (InSafeZone && Info.LastLogoutDate > DateTime.MinValue)
            {
                double totalMinutes = (Envir.Now - Info.LastLogoutDate).TotalMinutes;

                _restedCounter = (int)(totalMinutes * 60);
            }
             

            Report.Connected(Connection.IPAddress);

            MessageQueue.SendMsg(string.Format("{0} has connected.", Info.Name));

            if (IsGM)
            {
                UpdateGMBuff();
            }
            else
            {
                LastRankUpdate = Envir.Time;
                Envir.CheckRankUpdate(Info);
                Envir.OnlineRankingCount[0]++;
                Envir.OnlineRankingCount[(int)Class + 1]++;
            }
        }
        private void StartGameFailed()
        {
            SendPacketToClient(new ServerPacket.StartGame { Result = 3 });
            CleanUp();
        }        
        public void GiveRestedBonus(int count)
        {
            if (count > 0)
            {
                long existingTime = 0;

                if (HasBuff(BuffType.Rested, out Buff rested))
                {
                    existingTime = rested.ExpireTime;
                }

                int duration = (int)Math.Min(int.MaxValue, ((Settings.RestedBuffLength * Settings.Minute) * count) + existingTime);
                int maxDuration = (Settings.RestedBuffLength * Settings.Minute) * Settings.RestedMaxBonus;

                if (duration > maxDuration) duration = maxDuration;

                AddBuff(BuffType.Rested, this, duration, new Stats { [Stat.ExpRatePercent] = Settings.RestedExpBonus });

                _restedCounter = 0;
            }
        }
        public override void Revive(int hp, bool effect)
        {
            if (!Dead) return;

            base.Revive(hp, effect);

            GetObjects();

            Fishing = false;
            SendPacketToClient(GetFishInfo());
            GroupMemberMapNameChanged();
            GetPlayerLocation();
        }
        public void TownRevive()
        {
            if (!Dead) return;

            Map temp = Envir.GetMap(BindMapIndex);
            Point bindLocation = BindLocation;

            if (Info.PKPoints >= 200)
            {
                temp = Envir.GetMapByNameAndInstance(Settings.PKTownMapName, 1);
                bindLocation = new Point(Settings.PKTownPositionX, Settings.PKTownPositionY);

                if (temp == null)
                {
                    temp = Envir.GetMap(BindMapIndex);
                    bindLocation = BindLocation;
                }
            }

            if (temp == null || !temp.ValidPoint(bindLocation)) return;

            Dead = false;
            SetHP(Stats[Stat.HP]);
            SetMP(Stats[Stat.MP]);
            RefreshStats();

            CurrentMap.RemoveObject(this);
            Broadcast(new ServerPacket.ObjectRemove { ObjectID = ObjectID });

            CurrentMap = temp;
            CurrentLocation = bindLocation;

            CurrentMap.AddObject(this);

            SendPacketToClient(new ServerPacket.MapChanged
            {
                MapIndex = CurrentMap.Info.Index,
                FileName = CurrentMap.Info.FileName,
                Title = CurrentMap.Info.Title,
                MiniMap = CurrentMap.Info.MiniMap,
                BigMap = CurrentMap.Info.BigMap,
                Lights = CurrentMap.Info.Light,
                Location = CurrentLocation,
                Direction = Direction,
                MapDarkLight = CurrentMap.Info.MapDarkLight,
                Music = CurrentMap.Info.Music
            });

            GetObjects();
            SendPacketToClient(new ServerPacket.Revived());
            Broadcast(new ServerPacket.ObjectRevived { ObjectID = ObjectID, Effect = true });

            InSafeZone = true;
            Fishing = false;
            SendPacketToClient(GetFishInfo());
            GroupMemberMapNameChanged();
            GetPlayerLocation();
        }
        public override bool Teleport(Map temp, Point location, bool effects = true, byte effectnumber = 0)
        {
            Map oldMap = CurrentMap;
            Point oldLocation = CurrentLocation;
            bool mapChanged = temp != oldMap;

            if (!base.Teleport(temp, location, effects)) return false;            

            //Cancel actions
            if (TradePartner != null)
                TradeCancel();
             
            GetObjectsPassive();

            CheckConquest();

            Fishing = false;
            SendPacketToClient(GetFishInfo());

            if (mapChanged)
            {
                CallDefaultNPC(DefaultNPCType.MapEnter, CurrentMap.Info.FileName);

                if (Info.Married != 0)
                {
                    CharacterInfo Lover = Envir.GetCharacterInfo(Info.Married);
                    PlayerObjectSrv player = Envir.GetPlayer(Lover.Name);

                    if (player != null) player.GetRelationship(false);
                }
                GroupMemberMapNameChanged();
            }
            GetPlayerLocation();

            Report?.MapChange(oldMap.Info, CurrentMap.Info);

            return true;
        }

        static readonly ServerPacketIds[] BroadcastObservePackets = new ServerPacketIds[]
        {
            ServerPacketIds.ObjectTurn,
            ServerPacketIds.ObjectWalk,
            ServerPacketIds.ObjectRun,
            ServerPacketIds.ObjectAttack,
            ServerPacketIds.ObjectRangeAttack,
            ServerPacketIds.ObjectMagic,
            ServerPacketIds.ObjectHarvest
        };

        public override void Broadcast(Packet p)
        {
            if (p == null || CurrentMap == null) return;

            base.Broadcast(p);

            if (Array.Exists(BroadcastObservePackets, x => x == (ServerPacketIds)p.Index))
            {
                foreach (ExineConnection c in Connection.Observers)
                    c.SendPacketToClient(p);
            }
        }

        public void AddObserver(ExineConnection observer)
        {
            if (observer == Connection) return;

            Connection.AddObserver(observer);
            GetItemInfo(observer);
            GetMapInfo(observer);
            GetUserInfo(observer);
            GetObjectsPassive(observer);
            if (observer.Player != null)
                observer.Player.StopGame(24);            
        }
        protected virtual void GetItemInfo(ExineConnection c)
        {
            UserItem item;
            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                item = Info.Inventory[i];
                if (item == null) continue;

                c.CheckItem(item);
            }

            for (int i = 0; i < Info.Equipment.Length; i++)
            {
                item = Info.Equipment[i];

                if (item == null) continue;

                c.CheckItem(item);
            }

            for (int i = 0; i < Info.QuestInventory.Length; i++)
            {
                item = Info.QuestInventory[i];

                if (item == null) continue;
                c.CheckItem(item);
            }
        }
        private void GetUserInfo(ExineConnection c)
        {
            string guildname = MyGuild != null ? MyGuild.Name : "";
            string guildrank = MyGuild != null ? MyGuildRank.Name : "";
            ServerPacket.UserInformation packet = new ServerPacket.UserInformation
            {
                ObjectID = ObjectID,
                RealId = (uint)Info.Index,
                Name = Name,
                GuildName = guildname,
                GuildRank = guildrank,
                NameColour = GetNameColour(this),
                Class = Class,
                Gender = Gender,
                ExStyle = Info.ExStyle,//add k333123
                ExColor = Info.ExColor,//add k333123
                ExPortraitLen = Info.ExPortraitLen,//add k333123
                ExPortraitBytes = Info.ExPortraitBytes,//add k333123

                Level = Level,
                Location = CurrentLocation,
                Direction = Direction,
                Hair = Hair,
                HP = HP,
                MP = MP,

                Experience = Experience,
                MaxExperience = MaxExperience,

                LevelEffects = LevelEffects,
                 

                Inventory = new UserItem[Info.Inventory.Length],
                Equipment = new UserItem[Info.Equipment.Length],
                QuestInventory = new UserItem[Info.QuestInventory.Length],
                Gold = Account.Gold,
                Credit = Account.Credit,
                HasExpandedStorage = Account.ExpandedStorageExpiryDate > Envir.Now ? true : false,
                ExpandedStorageExpiryTime = Account.ExpandedStorageExpiryDate,
                AllowObserve = AllowObserve,
                Observer = c != Connection
            };

            Console.WriteLine("ExStyle C:" + ExStyle);
            Console.WriteLine("ExPortraitLen:" + ExPortraitLen);

            //Copy this method to prevent modification before sending packet information.
            for (int i = 0; i < Info.Magics.Count; i++)
                packet.Magics.Add(Info.Magics[i].CreateClientMagic());

            Info.Inventory.CopyTo(packet.Inventory, 0);
            Info.Equipment.CopyTo(packet.Equipment, 0);
            Info.QuestInventory.CopyTo(packet.QuestInventory, 0);

          
            SendPacketToClient(packet, c);
        }        
        private void GetMapInfo(ExineConnection c)
        {
            SendPacketToClient(new ServerPacket.MapInformation
            {
                MapIndex = CurrentMap.Info.Index,
                FileName = CurrentMap.Info.FileName,
                Title = CurrentMap.Info.Title,
                MiniMap = CurrentMap.Info.MiniMap,
                Lights = CurrentMap.Info.Light,
                BigMap = CurrentMap.Info.BigMap,
                Lightning = CurrentMap.Info.Lightning,
                Fire = CurrentMap.Info.Fire,
                MapDarkLight = CurrentMap.Info.MapDarkLight,
                Music = CurrentMap.Info.Music,
            }, c);
        }
        private void GetQuestInfo()
        {
            for (int i = 0; i < Envir.QuestInfoList.Count; i++)
            {
                CheckQuestInfo(Envir.QuestInfoList[i]);
            }
        }
        private void GetRecipeInfo()
        {
            for (int i = 0; i < Envir.RecipeInfoList.Count; i++)
            {
                CheckRecipeInfo(Envir.RecipeInfoList[i]);
            }
        }
        private void GetObjects()
        {
            for (int y = CurrentLocation.Y - Globals.DataRange; y <= CurrentLocation.Y + Globals.DataRange; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = CurrentLocation.X - Globals.DataRange; x <= CurrentLocation.X + Globals.DataRange; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;
                    if (x < 0 || x >= CurrentMap.Width) continue;

                    Cell cell = CurrentMap.GetCell(x, y);

                    if (!cell.Valid || cell.Objects == null) continue;

                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObjectSrv ob = cell.Objects[i];

                        //if (ob.Race == ObjectType.Player && ob.Observer) continue;

                        ob.Add(this);
                    }
                }
            }
        }
        private void GetObjectsPassive(ExineConnection c = null)
        {
            for (int y = CurrentLocation.Y - Globals.DataRange; y <= CurrentLocation.Y + Globals.DataRange; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = CurrentLocation.X - Globals.DataRange; x <= CurrentLocation.X + Globals.DataRange; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;
                    if (x < 0 || x >= CurrentMap.Width) continue;

                    Cell cell = CurrentMap.GetCell(x, y);

                    if (!cell.Valid || cell.Objects == null) continue;

                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObjectSrv ob = cell.Objects[i];
                        if (ob == this) continue;

                        if (ob.Race == ObjectType.Player)
                        {
                            PlayerObjectSrv Player = (PlayerObjectSrv)ob;
                            SendPacketToClient(Player.GetInfoEx(this), c);
                        }
                        else if (ob.Race == ObjectType.Spell)
                        {
                            SpellObjectSrv obSpell = (SpellObjectSrv)ob;

                            if ((obSpell.Spell != Spell.ExplosiveTrap) || (obSpell.Caster != null && IsFriendlyTarget(obSpell.Caster)))
                                SendPacketToClient(ob.GetInfo(), c);
                        }
                        else if (ob.Race == ObjectType.Merchant)
                        {
                            NPCObjectSrv NPC = (NPCObjectSrv)ob;

                            NPC.CheckVisible(this);

                            if (NPC.VisibleLog[Info.Index] && NPC.Visible) SendPacketToClient(ob.GetInfo(), c);
                        }
                        else
                        {
                            SendPacketToClient(ob.GetInfo(), c);
                        }

                        if (ob.Race == ObjectType.Player || ob.Race == ObjectType.Monster)
                        {
                            ob.SendHealth(this);
                        }
                    }
                }
            }
        }
        public override void RefreshGuildBuffs()
        {
            if (MyGuild == null) return;
            if (MyGuild.BuffList.Count == 0) return;

            for (int i = 0; i < MyGuild.BuffList.Count; i++)
            {
                GuildBuff buff = MyGuild.BuffList[i];
                if ((buff.Info == null) || (!buff.Active)) continue;

                Stats.Add(buff.Info.Stats);
            }
        }

        public override void RefreshNameColour()
        {
            var prevColor = NameColour;
            NameColour = GetNameColour(this);
            
            if (prevColor == NameColour) return;
            
            SendPacketToClient(new ServerPacket.ColourChanged { NameColour = NameColour });
            BroadcastColourChange();
        }

        public override Color GetNameColour(HumanObjectSrv human)
        {
            if (human == null) return NameColour;

            if (human is PlayerObjectSrv player)
            {
                if (player.PKPoints >= 200)
                    return Color.Red;

                if (Envir.Time < player.BrownTime)
                    return Color.SaddleBrown;

                if (player.WarZone)
                {
                    if (player.MyGuild == null)
                        return Color.Green;

                    if (player.MyGuild == MyGuild)
                        return Color.Green;
                    else
                        return Color.Orange;
                }

                if (MyGuild != null)
                {
                    if (MyGuild.IsAtWar())
                    {
                        if (player.MyGuild != null)
                        {
                            if (player.MyGuild == MyGuild)
                                return Color.Blue;
                            if (MyGuild.IsEnemy(player.MyGuild))
                                return Color.Orange;
                        }
                    }
                }

                if (player.PKPoints >= 100)
                    return Color.Yellow;
            }

            return Color.White;
        }
        public void Chat(string message, List<ChatItem> linkedItems = null)
        {
            if (string.IsNullOrEmpty(message)) return;

            MessageQueue.SendChatMsg(string.Format("{0}: {1}", Name, message));

            if (GMLogin)
            {
                if (message == GMPassword)
                {
                    IsGM = true;
                    UpdateGMBuff();
                    MessageQueue.SendMsg(string.Format("{0} is now a GM", Name));
                    ReceiveChat("You have been made a GM", ChatType.System);
                    Envir.RemoveRank(Info);//remove gm chars from ranking to avoid causing bugs in rank list
                }
                else
                {
                    MessageQueue.SendMsg(string.Format("{0} attempted a GM login", Name));
                    ReceiveChat("Incorrect login password", ChatType.System);
                }
                GMLogin = false;
                return;
            }

            if (Info.ChatBanned)
            {
                if (Info.ChatBanExpiryDate > Envir.Now)
                {
                    ReceiveChat("You are currently banned from chatting.", ChatType.System);
                    return;
                }

                Info.ChatBanned = false;
            }
            else
            {
                if (ChatTime > Envir.Time)
                {
                    if (ChatTick >= 5 & !IsGM)
                    {
                        Info.ChatBanned = true;
                        Info.ChatBanExpiryDate = Envir.Now.AddMinutes(5);
                        ReceiveChat("You have been banned from chatting for 5 minutes.", ChatType.System);
                        return;
                    }

                    ChatTick++;
                }
                else
                    ChatTick = 0;

                ChatTime = Envir.Time + 2000;
            }

            string[] parts;

            message = message.Replace("$pos", Functions.PointToString(CurrentLocation));


            Packet p;
            if (message.StartsWith("/"))
            {
                //Private Message
                message = message.Remove(0, 1);
                parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                PlayerObjectSrv player = Envir.GetPlayer(parts[0]);

                if (player == null)
                {
                     
                    ReceiveChat(string.Format("Could not find {0}.", parts[0]), ChatType.System);
                    return;
                }

                if (player.Info.Friends.Any(e => e.Info == Info && e.Blocked))
                {
                    ReceiveChat("Player is not accepting your messages.", ChatType.System);
                    return;
                }

                if (Info.Friends.Any(e => e.Info == player.Info && e.Blocked))
                {
                    ReceiveChat("Cannot message player whilst they are on your blacklist.", ChatType.System);
                    return;
                }

                message = ProcessChatItems(message, new List<PlayerObjectSrv> { player }, linkedItems);

                ReceiveChat(string.Format("/{0}", message), ChatType.WhisperOut);
                player.ReceiveChat(string.Format("{0}=>{1}", Name, message.Remove(0, parts[0].Length)), ChatType.WhisperIn);
            }
            else if (message.StartsWith("!!"))
            {
                if (GroupMembers == null) return;
                //Group
                message = String.Format("{0}:{1}", Name, message.Remove(0, 2));

                message = ProcessChatItems(message, GroupMembers, linkedItems);

                p = new ServerPacket.ObjectChat { ObjectID = ObjectID, Text = message, Type = ChatType.Group };

                for (int i = 0; i < GroupMembers.Count; i++)
                    GroupMembers[i].SendPacketToClient(p);
            }
            else if (message.StartsWith("!~"))
            {
                if (MyGuild == null) return;

                //Guild
                message = message.Remove(0, 2);

                message = ProcessChatItems(message, MyGuild.GetOnlinePlayers(), linkedItems);

                MyGuild.SendMessage(String.Format("{0}: {1}", Name, message));

            }
            else if (message.StartsWith("!#"))
            {
                //Mentor Message
                message = message.Remove(0, 2);
                parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                if (Info.Mentor == 0) return;

                CharacterInfo Mentor = Envir.GetCharacterInfo(Info.Mentor);
                PlayerObjectSrv player = Envir.GetPlayer(Mentor.Name);

                if (player == null)
                {
                    ReceiveChat(string.Format("{0} isn't online.", Mentor.Name), ChatType.System);
                    return;
                }

                message = ProcessChatItems(message, new List<PlayerObjectSrv> { player }, linkedItems);

                ReceiveChat(string.Format("{0}: {1}", Name, message), ChatType.Mentor);
                player.ReceiveChat(string.Format("{0}: {1}", Name, message), ChatType.Mentor);
            }
            else if (message.StartsWith("!"))
            {
                //Shout
                if (Envir.Time < ShoutTime)
                {
                    ReceiveChat(string.Format("You cannot shout for another {0} seconds.", Math.Ceiling((ShoutTime - Envir.Time) / 1000D)), ChatType.System);
                    return;
                }
                if (Level < 8 && (!HasMapShout && !HasServerShout))
                {
                    ReceiveChat("You need to be level 8 before you can shout.", ChatType.System);
                    return;
                }

                ShoutTime = Envir.Time + 10000;
                message = String.Format("(!){0}:{1}", Name, message.Remove(0, 1));

                if (HasMapShout)
                {
                    message = ProcessChatItems(message, CurrentMap.Players, linkedItems);

                    p = new ServerPacket.Chat { Message = message, Type = ChatType.Shout2 };
                    HasMapShout = false;

                    for (int i = 0; i < CurrentMap.Players.Count; i++)
                    {
                        CurrentMap.Players[i].SendPacketToClient(p);
                    }
                    return;
                }
                else if (HasServerShout)
                {
                    message = ProcessChatItems(message, Envir.Players, linkedItems);

                    p = new ServerPacket.Chat { Message = message, Type = ChatType.Shout3 };
                    HasServerShout = false;

                    for (int i = 0; i < Envir.Players.Count; i++)
                    {
                        Envir.Players[i].SendPacketToClient(p);
                    }
                    return;
                }
                else
                {
                    List<PlayerObjectSrv> playersInRange = new List<PlayerObjectSrv>();

                    for (int i = 0; i < CurrentMap.Players.Count; i++)
                    {
                        if (!Functions.InRange(CurrentLocation, CurrentMap.Players[i].CurrentLocation, Globals.DataRange * 2)) continue;

                        playersInRange.Add(CurrentMap.Players[i]);
                    }

                    message = ProcessChatItems(message, playersInRange, linkedItems);

                    p = new ServerPacket.Chat { Message = message, Type = ChatType.Shout };

                    for (int i = 0; i < playersInRange.Count; i++)
                    {
                        playersInRange[i].SendPacketToClient(p);
                    }

                }

            }
            else if (message.StartsWith(":)"))
            {
                //Relationship Message
                message = message.Remove(0, 2);
                parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                if (Info.Married == 0) return;

                CharacterInfo Lover = Envir.GetCharacterInfo(Info.Married);
                PlayerObjectSrv player = Envir.GetPlayer(Lover.Name);
            
                if (player == null)
                {
                    ReceiveChat(string.Format("{0} isn't online.", Lover.Name), ChatType.System);
                    return;
                }

                message = ProcessChatItems(message, new List<PlayerObjectSrv> { player }, linkedItems);

                ReceiveChat(string.Format("{0}: {1}", Name, message), ChatType.Relationship);
                player.ReceiveChat(string.Format("{0}: {1}", Name, message), ChatType.Relationship);
            }
            else if (message.StartsWith("@!"))
            {
                if (!IsGM) return;

                message = String.Format("(*){0}:{1}", Name, message.Remove(0, 2));

                message = ProcessChatItems(message, Envir.Players, linkedItems);

                p = new ServerPacket.Chat { Message = message, Type = ChatType.Announcement };

                Envir.Broadcast(p);
            }
            else if (message.StartsWith("@"))
            {
                
                //Command
                message = message.Remove(0, 1);
                parts = message.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) return;

                PlayerObjectSrv player;
                CharacterInfo data;
                String hintstring;
                UserItem item;

                List<int> conquestAIs = new()
                {
                    72, // siege gate
                    73, // gate west
                    80, // archer
                    81, // gate 
                    82  // wall
                };

                switch (parts[0].ToUpper())
                {
                    case "LOGIN":
                        GMLogin = true;
                        ReceiveChat("Please type the GM Password", ChatType.Hint);
                        return;

                    case "KILL":
                        if (!IsGM) return;

                        if (parts.Length >= 2)
                        {
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Could not find {0}", parts[0]), ChatType.System);
                                return;
                            }
                            if (!player.GMNeverDie) player.Die();
                        }
                        else
                        {
                            if (!CurrentMap.ValidPoint(Front)) return;

                            Cell cell = CurrentMap.GetCell(Front);

                            if (cell == null || cell.Objects == null) return;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObjectSrv ob = cell.Objects[i];

                                switch (ob.Race)
                                {
                                    case ObjectType.Player:
                                    case ObjectType.Monster:
                                        if (ob.Dead) continue;
                                        ob.EXPOwner = this;
                                        ob.ExpireTime = Envir.Time + MonsterObjectSrv.EXPOwnerDelay;
                                        ob.Die();
                                        break;
                                    default:
                                        continue;
                                }
                            }
                        }
                        return;

                    case "CHANGEGENDER":
                        if (!IsGM && !Settings.TestServer) return;

                        data = parts.Length < 2 ? Info : Envir.GetCharacterInfo(parts[1]);

                        if (data == null) return;

                        switch (data.Gender)
                        {
                            case ExineGender.Male:
                                data.Gender = ExineGender.Female;
                                break;
                            case ExineGender.Female:
                                data.Gender = ExineGender.Male;
                                break;
                        }

                        ReceiveChat(string.Format("Player {0} has been changed to {1}", data.Name, data.Gender), ChatType.System);
                        MessageQueue.SendMsg(string.Format("Player {0} has been changed to {1} by {2}", data.Name, data.Gender, Name));

                        if (data.Player != null)
                            data.Player.Connection.OnRecvLogOutHandler();

                        break;

                    case "LEVEL":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        ushort level;
                        ushort old;
                        if (parts.Length >= 3)
                        {
                            if (!IsGM) return;

                            if (ushort.TryParse(parts[2], out level))
                            {
                                if (level == 0) return;
                                player = Envir.GetPlayer(parts[1]);
                                if (player == null) return;
                                old = player.Level;
                                player.Level = level;
                                player.LevelUp();

                                ReceiveChat(string.Format("Player {0} has been Leveled {1} -> {2}.", player.Name, old, player.Level), ChatType.System);
                                MessageQueue.SendMsg(string.Format("Player {0} has been Leveled {1} -> {2} by {3}", player.Name, old, player.Level, Name));
                                return;
                            }
                        }
                        else
                        {
                            if (parts[1] == "-1")
                            {
                                parts[1] = ushort.MaxValue.ToString();
                            }

                            if (ushort.TryParse(parts[1], out level))
                            {
                                if (level == 0) return;
                                old = Level;
                                Level = level;
                                LevelUp();

                                ReceiveChat(string.Format("{0} {1} -> {2}.", GameLanguage.LevelUp, old, Level), ChatType.System);
                                MessageQueue.SendMsg(string.Format("Player {0} has been Leveled {1} -> {2} by {3}", Name, old, Level, Name));
                                return;
                            }
                        }

                        ReceiveChat("Could not level player", ChatType.System);
                        break;

                    
                    case "MAKE":
                        {
                            if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                            ItemInfo iInfo;
                            int itemIndex = 0;

                            if (Int32.TryParse(parts[1], out itemIndex))
                            {
                                iInfo = Envir.GetItemInfo(itemIndex);
                            }
                            else
                            {
                                iInfo = Envir.GetItemInfo(parts[1]);
                            }

                            if (iInfo == null) return;

                            ushort itemCount = 1;
                            if (parts.Length >= 3 && !ushort.TryParse(parts[2], out itemCount))
                                itemCount = 1;

                            var tempCount = itemCount;

                            while (itemCount > 0)
                            {
                                if (iInfo.StackSize >= itemCount)
                                {
                                    item = Envir.CreateDropItem(iInfo);
                                    item.Count = itemCount;

                                    if (CanGainItem(item)) GainItem(item);

                                    return;
                                }
                                item = Envir.CreateDropItem(iInfo);
                                item.Count = iInfo.StackSize;
                                itemCount -= iInfo.StackSize;

                                if (!CanGainItem(item)) return;
                                GainItem(item);
                            }

                            ReceiveChat(string.Format("{0} x{1} has been created.", iInfo.FriendlyName, tempCount), ChatType.System);
                            MessageQueue.SendMsg(string.Format("Player {0} has attempted to Create {1} x{2}", Name, iInfo.Name, tempCount));
                        }
                        break;
                    case "CLEARBUFFS":
                        foreach (var buff in Buffs)
                        {
                            buff.FlagForRemoval = true;
                            buff.ExpireTime = 0;
                        }
                        break;

                    case "CLEARBAG":
                        if (!IsGM && !Settings.TestServer) return;
                        player = this;

                        if (parts.Length >= 2)
                            player = Envir.GetPlayer(parts[1]);

                        if (player == null) return;
                        for (int i = 0; i < player.Info.Inventory.Length; i++)
                        {
                            item = player.Info.Inventory[i];
                            if (item == null) continue;

                            player.SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                            player.Info.Inventory[i] = null;
                        }
                        player.RefreshStats();
                        break;

                    case "SUPERMAN":
                        if (!IsGM && !Settings.TestServer) return;

                        GMNeverDie = !GMNeverDie;

                        hintstring = GMNeverDie ? "Invincible Mode." : "Normal Mode.";
                        ReceiveChat(hintstring, ChatType.Hint);
                        UpdateGMBuff();
                        break;

                    case "GAMEMASTER":
                        if (!IsGM && !Settings.TestServer) return;

                        GMGameMaster = !GMGameMaster;

                        hintstring = GMGameMaster ? "GameMaster Mode." : "Normal Mode.";
                        ReceiveChat(hintstring, ChatType.Hint);
                        UpdateGMBuff();
                        break;

                    case "OBSERVER":
                        if (!IsGM) return;
                        Observer = !Observer;

                        hintstring = Observer ? "Observer Mode." : "Normal Mode.";
                        ReceiveChat(hintstring, ChatType.Hint);
                        UpdateGMBuff();
                        break;
                    case "ALLOWGUILD":
                        EnableGuildInvite = !EnableGuildInvite;
                        hintstring = EnableGuildInvite ? "Guild invites enabled." : "Guild invites disabled.";
                        ReceiveChat(hintstring, ChatType.Hint);
                        break;
                    case "RECALL":
                        if (!IsGM) return;

                        if (parts.Length < 2) return;
                        player = Envir.GetPlayer(parts[1]);

                        if (player == null) return;

                        player.Teleport(CurrentMap, Front);
                        break;
                    case "OBSERVE":
                        if (parts.Length < 2) return;
                        player = Envir.GetPlayer(parts[1]);

                        if (player == null) return;
                        if ((!player.AllowObserve || !Settings.AllowObserve) && !IsGM) return;
                        
                        player.AddObserver(Connection);
                        break;
                    case "ENABLEGROUPRECALL":
                        EnableGroupRecall = !EnableGroupRecall;
                        hintstring = EnableGroupRecall ? "Group Recall Enabled." : "Group Recall Disabled.";
                        ReceiveChat(hintstring, ChatType.Hint);
                        break;

                    case "GROUPRECALL":
                        if (GroupMembers == null || GroupMembers[0] != this || Dead)
                            return;

                        if (CurrentMap.Info.NoRecall)
                        {
                            ReceiveChat("You cannot recall people on this map", ChatType.System);
                            return;
                        }

                        if (Envir.Time < LastRecallTime)
                        {
                            ReceiveChat(string.Format("You cannot recall for another {0} seconds", (LastRecallTime - Envir.Time) / 1000), ChatType.System);
                            return;
                        }

                        if (ItemSets.Any(set => set.Set == ItemSet.Recall && set.SetComplete))
                        {
                            LastRecallTime = Envir.Time + 180000;
                            for (var i = 1; i < GroupMembers.Count(); i++)
                            {
                                if (GroupMembers[i].EnableGroupRecall)
                                    GroupMembers[i].Teleport(CurrentMap, CurrentLocation);
                                else
                                    GroupMembers[i].ReceiveChat("A recall was attempted without your permission",
                                        ChatType.System);
                            }
                        }
                        break;
                    case "RECALLMEMBER":
                        if (GroupMembers == null || GroupMembers[0] != this)
                        {
                            ReceiveChat("You are not a group leader.", ChatType.System);
                            return;
                        }

                        if (Dead)
                        {
                            ReceiveChat("You cannot recall when you are dead.", ChatType.System);
                            return;
                        }

                        if (CurrentMap.Info.NoRecall)
                        {
                            ReceiveChat("You cannot recall people on this map", ChatType.System);
                            return;
                        }

                        if (Envir.Time < LastRecallTime)
                        {
                            ReceiveChat(string.Format("You cannot recall for another {0} seconds", (LastRecallTime - Envir.Time) / 1000), ChatType.System);
                            return;
                        }
                        if (ItemSets.Any(set => set.Set == ItemSet.Recall && set.SetComplete))
                        {
                            if (parts.Length < 2) return;
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null || !IsMember(player) || this == player)
                            {
                                ReceiveChat((string.Format("Player {0} could not be found", parts[1])), ChatType.System);
                                return;
                            }
                            if (!player.EnableGroupRecall)
                            {
                                player.ReceiveChat("A recall was attempted without your permission",
                                        ChatType.System);
                                ReceiveChat((string.Format("{0} is blocking grouprecall", player.Name)), ChatType.System);
                                return;
                            }
                            LastRecallTime = Envir.Time + 60000;

                            if (!player.Teleport(CurrentMap, Front))
                                player.Teleport(CurrentMap, CurrentLocation);
                        }
                        else
                        {
                            ReceiveChat("You cannot recall without a recallset.", ChatType.System);
                            return;
                        }
                        break;

                    case "RECALLLOVER":
                        if (Info.Married == 0)
                        {
                            ReceiveChat("You're not married.", ChatType.System);
                            return;
                        }

                        if (Dead)
                        {
                            ReceiveChat("You can't recall when you are dead.", ChatType.System);
                            return;
                        }

                        if (CurrentMap.Info.NoRecall)
                        {
                            ReceiveChat("You cannot recall people on this map", ChatType.System);
                            return;
                        }

                        if (Info.Equipment[(int)EquipmentSlot.RingL] == null)
                        {
                            ReceiveChat("You need to be wearing a Wedding Ring for recall.", ChatType.System);
                            return;
                        }


                        if (Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing == Info.Married)
                        {
                            CharacterInfo Lover = Envir.GetCharacterInfo(Info.Married);

                            if (Lover == null) return;

                            player = Envir.GetPlayer(Lover.Name);

                            if (!Settings.WeddingRingRecall)
                            {
                                ReceiveChat($"Teleportation via Wedding Ring is disabled.", ChatType.System);
                                return;
                            }

                            if (player == null)
                            {
                                ReceiveChat((string.Format("{0} is not online.", Lover.Name)), ChatType.System);
                                return;
                            }

                            if (player.Dead)
                            {
                                ReceiveChat("You can't recall a dead player.", ChatType.System);
                                return;
                            }

                            if (player.Info.Equipment[(int)EquipmentSlot.RingL] == null)
                            {
                                player.ReceiveChat((string.Format("You need to wear a Wedding Ring for recall.", Lover.Name)), ChatType.System);
                                ReceiveChat((string.Format("{0} Isn't wearing a Wedding Ring.", Lover.Name)), ChatType.System);
                                return;
                            }

                            if (player.Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing != player.Info.Married)
                            {
                                player.ReceiveChat((string.Format("You need to wear a Wedding Ring on your left finger for recall.", Lover.Name)), ChatType.System);
                                ReceiveChat((string.Format("{0} Isn't wearing a Wedding Ring.", Lover.Name)), ChatType.System);
                                return;
                            }

                            if (!player.AllowLoverRecall)
                            {
                                player.ReceiveChat("A recall was attempted without your permission",
                                        ChatType.System);
                                ReceiveChat((string.Format("{0} is blocking Lover Recall.", player.Name)), ChatType.System);
                                return;
                            }

                            if ((Envir.Time < LastRecallTime) && (Envir.Time < player.LastRecallTime))
                            {
                                ReceiveChat(string.Format("You cannot recall for another {0} seconds", (LastRecallTime - Envir.Time) / 1000), ChatType.System);
                                return;
                            }

                            LastRecallTime = Envir.Time + 60000;
                            player.LastRecallTime = Envir.Time + 60000;

                            if (!player.Teleport(CurrentMap, Front))
                                player.Teleport(CurrentMap, CurrentLocation);
                        }
                        else
                        {
                            ReceiveChat("You cannot recall your lover without wearing a wedding ring", ChatType.System);
                            return;
                        }
                        break;
                    case "TIME":
                        ReceiveChat(string.Format("The time is : {0}", Envir.Now.ToString("hh:mm tt")), ChatType.System);
                        break;

                    case "ROLL":
                        int diceNum = Envir.Random.Next(5) + 1;

                        if (GroupMembers == null) { return; }

                        for (int i = 0; i < GroupMembers.Count; i++)
                        {
                            PlayerObjectSrv playerSend = GroupMembers[i];
                            playerSend.ReceiveChat(string.Format("{0} has rolled a {1}", Name, diceNum), ChatType.Group);
                        }
                        break;

                    case "MAP":
                        var mapName = CurrentMap.Info.FileName;
                        var mapTitle = CurrentMap.Info.Title;
                        ReceiveChat((string.Format("You are currently in {0}. Map ID: {1}", mapTitle, mapName)), ChatType.System);
                        break;

                    case "BACKUPPLAYER":
                        {
                            if (!IsGM || parts.Length < 2) return;

                            var info = Envir.GetCharacterInfo(parts[1]);

                            if (info == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found", parts[1]), ChatType.System);
                                return;
                            }

                            Envir.SaveArchivedCharacter(info);

                            ReceiveChat(string.Format("Player {0} has been backed up", info.Name), ChatType.System);
                            MessageQueue.SendMsg(string.Format("Player {0} has been backed up by {1}", info.Name, Name));
                        }
                        break;

                    case "ARCHIVEPLAYER":
                        {
                            if (!IsGM || parts.Length < 2) return;

                            data = Envir.GetCharacterInfo(parts[1]);

                            if (data == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found", parts[1]), ChatType.System);
                                return;
                            }

                            if (data == Info)
                            {
                                ReceiveChat("Cannot archive the player you are on", ChatType.System);
                                return;
                            }

                            var account = Envir.GetAccountByCharacter(parts[1]);

                            if (account == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found in any account", parts[1]), ChatType.System);
                                return;
                            }

                            Envir.SaveArchivedCharacter(data);

                            Envir.CharacterList.Remove(data);
                            account.Characters.Remove(data);

                            ReceiveChat(string.Format("Player {0} has been archived", data.Name), ChatType.System);
                            MessageQueue.SendMsg(string.Format("Player {0} has been archived by {1}", data.Name, Name));
                        }
                        break;

                    case "LOADPLAYER":
                        {
                            if (!IsGM) return;

                            if (parts.Length < 2) return;

                            var bak = Envir.GetArchivedCharacter(parts[1]);

                            if (bak == null)
                            {
                                ReceiveChat(string.Format("Player {0} could not be loaded. Try specifying the full archive filename", parts[1]), ChatType.System);
                                return;
                            }

                            var info = Envir.GetCharacterInfo(bak.Name);

                            if (info == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found", parts[1]), ChatType.System);
                                return;
                            }

                            if (info.Index != bak.Index)
                            {
                                ReceiveChat("Cannot load this player due to mismatching ID's", ChatType.System);
                                return;
                            }

                            info = bak;

                            ReceiveChat(string.Format("Player {0} has been loaded", info.Name), ChatType.System);
                            MessageQueue.SendMsg(string.Format("Player {0} has been loaded by {1}", info.Name, Name));
                        }
                        break;

                    case "RESTOREPLAYER":
                        {
                            if (!IsGM || parts.Length < 2) return;

                            AccountInfo account = null;

                            if (parts.Length > 2)
                            {
                                if (!Envir.AccountExists(parts[2]))
                                {
                                    ReceiveChat(string.Format("Account {0} was not found", parts[2]), ChatType.System);
                                    return;
                                }

                                account = Envir.GetAccount(parts[2]);

                                if (account.Characters.Count >= Globals.MaxCharacterCount)
                                {
                                    ReceiveChat(string.Format("Account {0} already has {1} characters", parts[2], Globals.MaxCharacterCount), ChatType.System);
                                    return;
                                }
                            }

                            data = Envir.GetCharacterInfo(parts[1]);

                            if (data == null)
                            {
                                if (account != null)
                                {
                                    data = Envir.GetArchivedCharacter(parts[1]);

                                    if (data == null)
                                    {
                                        ReceiveChat(string.Format("Player {0} could not be restored. Try specifying the full archive filename", parts[1]), ChatType.System);
                                        return;
                                    }

                                    data.AccountInfo = account;

                                    account.Characters.Add(data);
                                    Envir.CharacterList.Add(data);

                                    data.Deleted = false;
                                    data.DeleteDate = DateTime.MinValue;

                                    data.LastLoginDate = Envir.Now;
                                }
                                else
                                {
                                    ReceiveChat(string.Format("Player {0} was not found", parts[1]), ChatType.System);
                                    return;
                                }
                            }
                            else
                            {
                                if (!data.Deleted) return;
                                data.Deleted = false;
                                data.DeleteDate = DateTime.MinValue;
                            }

                            ReceiveChat(string.Format("Player {0} has been restored by", data.Name), ChatType.System);
                            MessageQueue.SendMsg(string.Format("Player {0} has been restored by {1}", data.Name, Name));
                        }
                        break;

                    case "MOVE":
                        if (!IsGM && !SpecialMode.HasFlag(SpecialItemMode.Teleport) && !Settings.TestServer) return;
                        if (!IsGM && CurrentMap.Info.NoPosition)
                        {
                            ReceiveChat(("You cannot position move on this map"), ChatType.System);
                            return;
                        }
                        if (Envir.Time < LastTeleportTime)
                        {
                            ReceiveChat(string.Format("You cannot teleport for another {0} seconds", (LastTeleportTime - Envir.Time) / 1000), ChatType.System);
                            return;
                        }

                        int x, y;

                        if (parts.Length <= 2 || !int.TryParse(parts[1], out x) || !int.TryParse(parts[2], out y))
                        {
                            if (!IsGM)
                                LastTeleportTime = Envir.Time + 10000;
                            TeleportRandom(200, 0);
                            return;
                        }
                        if (!IsGM)
                            LastTeleportTime = Envir.Time + 10000;
                        Teleport(CurrentMap, new Point(x, y));
                        break;

                    case "MAPMOVE":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;
                        var instanceID = 1; x = 0; y = 0;

                        if (parts.Length == 3 || parts.Length == 5)
                            int.TryParse(parts[2], out instanceID);

                        if (instanceID < 1) instanceID = 1;

                        var map = Envir.GetMapByNameAndInstance(parts[1], instanceID);
                        if (map == null)
                        {
                            ReceiveChat((string.Format("Map {0}:[{1}] could not be found", parts[1], instanceID)), ChatType.System);
                            return;
                        }

                        if (parts.Length == 4 || parts.Length == 5)
                        {
                            int.TryParse(parts[parts.Length - 2], out x);
                            int.TryParse(parts[parts.Length - 1], out y);
                        }

                        switch (parts.Length)
                        {
                            case 2:
                                ReceiveChat(TeleportRandom(200, 0, map) ? (string.Format("Moved to Map {0}", map.Info.FileName)) :
                                    (string.Format("Failed movement to Map {0}", map.Info.FileName)), ChatType.System);
                                break;
                            case 3:
                                ReceiveChat(TeleportRandom(200, 0, map) ? (string.Format("Moved to Map {0}:[{1}]", map.Info.FileName, instanceID)) :
                                    (string.Format("Failed movement to Map {0}:[{1}]", map.Info.FileName, instanceID)), ChatType.System);
                                break;
                            case 4:
                                ReceiveChat(Teleport(map, new Point(x, y)) ? (string.Format("Moved to Map {0} at {1}:{2}", map.Info.FileName, x, y)) :
                                    (string.Format("Failed movement to Map {0} at {1}:{2}", map.Info.FileName, x, y)), ChatType.System);
                                break;
                            case 5:
                                ReceiveChat(Teleport(map, new Point(x, y)) ? (string.Format("Moved to Map {0}:[{1}] at {2}:{3}", map.Info.FileName, instanceID, x, y)) :
                                    (string.Format("Failed movement to Map {0}:[{1}] at {2}:{3}", map.Info.FileName, instanceID, x, y)), ChatType.System);
                                break;
                        }
                        break;

                    case "GOTO":
                        if (!IsGM) return;

                        if (parts.Length < 2) return;
                        player = Envir.GetPlayer(parts[1]);

                        if (player == null) return;

                        Teleport(player.CurrentMap, player.CurrentLocation);
                        break;

                    case "MOB":
                        if (!IsGM && !Settings.TestServer) return;
                        if (parts.Length < 2)
                        {
                            ReceiveChat("Not enough parameters to spawn monster", ChatType.System);
                            return;
                        }

                        MonsterInfo mInfo = null;
                        int monsterIndex = 0;

                        if (Int32.TryParse(parts[1], out monsterIndex))
                        {
                            mInfo = Envir.GetMonsterInfo(monsterIndex, false);
                        }
                        else
                        {
                            mInfo = Envir.GetMonsterInfo(parts[1]);
                        }

                        if (mInfo == null)
                        {
                            ReceiveChat((string.Format("Monster {0} does not exist", parts[1])), ChatType.System);
                            return;
                        }

                        if (conquestAIs.Contains(mInfo.AI))
                        {
                            ReceiveChat($"Cannot spawn conquest item: {mInfo.Name}", ChatType.System);
                            return;
                        }

                        uint count = 1;
                        if (parts.Length >= 3 && IsGM)
                            if (!uint.TryParse(parts[2], out count)) count = 1;
                        int spread = 0;
                        if (parts.Length >= 4)
                            int.TryParse(parts[3], out spread);

                        for (int i = 0; i < count; i++)
                        {
                            MonsterObjectSrv monster = MonsterObjectSrv.GetMonster(mInfo);
                            if (monster == null)
                            {
                                return;
                            }

                           

                            if (spread == 0)
                                monster.Spawn(CurrentMap, Front);
                            else
                                for (int _ = 0; _ < 20; _++)
                                    if (monster.Spawn(CurrentMap, CurrentLocation.Add(Envir.Random.Next(-spread, spread + 1), Envir.Random.Next(-spread, spread + 1))))
                                        break;
                        }

                        ReceiveChat((string.Format("Monster {0} x{1} has been spawned.", mInfo.Name, count)), ChatType.System);
                        break;

                    case "RECALLMOB":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        MonsterInfo mInfo2 = null;
                        int monsterIndex2 = 0;

                        if (Int32.TryParse(parts[1], out monsterIndex2))
                        {
                            mInfo2 = Envir.GetMonsterInfo(monsterIndex2, false);
                        }
                        else
                        {
                            mInfo2 = Envir.GetMonsterInfo(parts[1]);
                        }

                        if (mInfo2 == null) return;

                        count = 1;
                        byte petlevel = 0;

                        if (parts.Length > 2)
                            if (!uint.TryParse(parts[2], out count) || count > 50) count = 1;

                        if (parts.Length > 3)
                            if (!byte.TryParse(parts[3], out petlevel) || petlevel > 7) petlevel = 0;

                        if (!IsGM ) return;

                        for (int i = 0; i < count; i++)
                        {
                            MonsterObjectSrv monster = MonsterObjectSrv.GetMonster(mInfo2);

                            if (monster == null) return;

                            if (conquestAIs.Contains(monster.Info.AI))
                            {
                                ReceiveChat($"Cannot spawn conquest item: {monster.Name}", ChatType.System);
                                return;
                            }
                            
                        }

                        ReceiveChat((string.Format("Pet {0} x{1} has been recalled.", mInfo2.Name, count)), ChatType.System);
                        break;

                    case "RELOADDROPS":
                        if (!IsGM) return;

                        Envir.ReloadDrops();

                        ReceiveChat("Drops Reloaded.", ChatType.Hint);
                        break;

                    case "RELOADNPCS":
                        if (!IsGM) return;

                        Envir.ReloadNPCs();

                        ReceiveChat("NPC Scripts Reloaded.", ChatType.Hint);
                        break;

                    case "CLEARIPBLOCKS":
                        if (!IsGM) return;

                        Envir.IPBlocks.Clear();
                        break;

                    case "GIVEGOLD":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        player = this;

                        if (parts.Length > 2)
                        {
                            if (!IsGM) return;

                            if (!uint.TryParse(parts[2], out count)) return;
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found.", parts[1]), ChatType.System);
                                return;
                            }
                        }

                        else if (!uint.TryParse(parts[1], out count)) return;

                        if (count + player.Account.Gold >= uint.MaxValue)
                            count = uint.MaxValue - player.Account.Gold;

                        player.GainGold(count);
                        MessageQueue.SendMsg(string.Format("Player {0} has been given {1} gold", player.Name, count));
                        break;

                    case "GIVEPEARLS":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        player = this;

                        if (parts.Length > 2)
                        {
                            if (!IsGM) return;

                            if (!uint.TryParse(parts[2], out count)) return;
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found.", parts[1]), ChatType.System);
                                return;
                            }
                        }

                        else if (!uint.TryParse(parts[1], out count)) return;

                        if (count + player.Info.PearlCount >= int.MaxValue)
                            count = (uint)(int.MaxValue - player.Info.PearlCount);
                         
                        break;
                    case "GIVECREDIT":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        player = this;

                        if (parts.Length > 2)
                        {
                            if (!IsGM) return;

                            if (!uint.TryParse(parts[2], out count)) return;
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found.", parts[1]), ChatType.System);
                                return;
                            }
                        }

                        else if (!uint.TryParse(parts[1], out count)) return;

                        if (count + player.Account.Credit >= uint.MaxValue)
                            count = uint.MaxValue - player.Account.Credit;

                        player.GainCredit(count);
                        MessageQueue.SendMsg(string.Format("Player {0} has been given {1} credit", player.Name, count));
                        break;
                    case "GIVESKILL":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 3) return;

                        byte spellLevel = 0;

                        player = this;
                        Spell skill;

                        if (!Enum.TryParse(parts.Length > 3 ? parts[2] : parts[1], true, out skill)) return;

                        if (skill == Spell.None) return;

                        spellLevel = byte.TryParse(parts.Length > 3 ? parts[3] : parts[2], out spellLevel) ? Math.Min((byte)3, spellLevel) : (byte)0;

                        if (parts.Length > 3)
                        {
                            if (!IsGM) return;

                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found.", parts[1]), ChatType.System);
                                return;
                            }
                        }

                        var magic = new UserMagic(skill) { Level = spellLevel };

                        if (player.Info.Magics.Any(e => e.Spell == skill))
                        {
                            player.Info.Magics.FirstOrDefault(e => e.Spell == skill).Level = spellLevel;
                            player.ReceiveChat(string.Format("Spell {0} changed to level {1}", skill.ToString(), spellLevel), ChatType.Hint);
                            return;
                        }
                        else
                        {
                            player.ReceiveChat(string.Format("You have learned {0} at level {1}", skill.ToString(), spellLevel), ChatType.Hint);

                            if (player != this)
                            {
                                ReceiveChat(string.Format("{0} has learned {1} at level {2}", player.Name, skill.ToString(), spellLevel), ChatType.Hint);
                            }

                            player.Info.Magics.Add(magic);
                        }

                        player.SendMagicInfo(magic);
                        player.RefreshStats();
                        break;

                    case "FIND":
                        if (!IsGM && !SpecialMode.HasFlag(SpecialItemMode.Probe)) return;

                        if (Envir.Time < LastProbeTime)
                        {
                            ReceiveChat(string.Format("You cannot search for another {0} seconds", (LastProbeTime - Envir.Time) / 1000), ChatType.System);
                            return;
                        }

                        if (parts.Length < 2) return;
                        player = Envir.GetPlayer(parts[1]);

                        if (player == null)
                        {
                            ReceiveChat(parts[1] + " is not online", ChatType.System);
                            return;
                        }
                        if (player.CurrentMap == null) return;
                        if (!IsGM)
                            LastProbeTime = Envir.Time + 180000;
                        ReceiveChat((string.Format("{0} is located at {1} ({2},{3})", player.Name, player.CurrentMap.Info.Title, player.CurrentLocation.X, player.CurrentLocation.Y)), ChatType.System);
                        break;

                    case "LEAVEGUILD":
                        if (MyGuild == null) return;
                        if (MyGuildRank == null) return;
                        if(MyGuild.IsAtWar())
                        {
                            ReceiveChat("Cannot leave guild whilst at war.", ChatType.System);
                            return;
                        }

                        MyGuild.DeleteMember(this, Name);
                        break;

                    case "CREATEGUILD":

                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        player = parts.Length < 3 ? this : Envir.GetPlayer(parts[1]);

                        if (player == null)
                        {
                            ReceiveChat(string.Format("Player {0} was not found.", parts[1]), ChatType.System);
                            return;
                        }

                        if (player.MyGuild != null)
                        {
                            ReceiveChat(string.Format("Player {0} is already in a guild.", player.Name), ChatType.System);
                            return;
                        }

                        String gName = parts.Length < 3 ? parts[1] : parts[2];
                        if ((gName.Length < 3) || (gName.Length > 20))
                        {
                            ReceiveChat("Guildname is restricted to 3-20 characters.", ChatType.System);
                            return;
                        }

                        GuildObjectSrv guild = Envir.GetGuild(gName);
                        if (guild != null)
                        {
                            ReceiveChat(string.Format("Guild {0} already exists.", gName), ChatType.System);
                            return;
                        }

                        player.CanCreateGuild = true;
                        if (player.CreateGuild(gName))
                        {
                            ReceiveChat(string.Format("Successfully created guild {0}", gName), ChatType.System);
                        }
                        else
                        {
                            ReceiveChat("Failed to create guild", ChatType.System);
                        }

                        player.CanCreateGuild = false;
                        break;

                    case "ALLOWTRADE":
                        AllowTrade = !AllowTrade;

                        if (AllowTrade)
                            ReceiveChat("You are now allowing trade", ChatType.System);
                        else
                            ReceiveChat("You are no longer allowing trade", ChatType.System);
                        break;

                    case "TRIGGER":
                        if (!IsGM) return;
                        if (parts.Length < 2) return;

                        if (parts.Length >= 3)
                        {
                            player = Envir.GetPlayer(parts[2]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found.", parts[2]), ChatType.System);
                                return;
                            }

                            player.CallDefaultNPC(DefaultNPCType.Trigger, parts[1]);
                            return;
                        }

                        foreach (var pl in Envir.Players)
                        {
                            pl.CallDefaultNPC(DefaultNPCType.Trigger, parts[1]);
                        }

                        break;

                    
                    case "SETFLAG":
                        if (!IsGM && !Settings.TestServer) return;

                        if (parts.Length < 2) return;

                        int tempInt = 0;

                        if (!int.TryParse(parts[1], out tempInt)) return;

                        if (tempInt > Info.Flags.Length - 1) return;

                        Info.Flags[tempInt] = !Info.Flags[tempInt];

                        for (int f = CurrentMap.NPCs.Count - 1; f >= 0; f--)
                        {
                            if (Functions.InRange(CurrentMap.NPCs[f].CurrentLocation, CurrentLocation, Globals.DataRange))
                                CurrentMap.NPCs[f].CheckVisible(this);
                        }

                        break;

                    case "LISTFLAGS":
                        if (!IsGM && !Settings.TestServer) return;

                        for (int i = 0; i < Info.Flags.Length; i++)
                        {
                            if (Info.Flags[i] == false) continue;

                            ReceiveChat("Flag " + i, ChatType.Hint);
                        }
                        break;

                    case "CLEARFLAGS":
                        if (!IsGM && !Settings.TestServer) return;

                        player = parts.Length > 1 && IsGM ? Envir.GetPlayer(parts[1]) : this;

                        if (player == null)
                        {
                            ReceiveChat(parts[1] + " is not online", ChatType.System);
                            return;
                        }

                        for (int i = 0; i < player.Info.Flags.Length; i++)
                        {
                            player.Info.Flags[i] = false;
                        }
                        break;
                    case "CLEARMOB":
                        if (!IsGM) return;

                        if (parts.Length > 1)
                        {
                            map = Envir.GetMapByNameAndInstance(parts[1]);

                            if (map == null) return;

                        }
                        else
                        {
                            map = CurrentMap;
                        }

                        foreach (var cell in map.Cells)
                        {
                            if (cell == null || cell.Objects == null) continue;

                            int obCount = cell.Objects.Count();

                            for (int m = 0; m < obCount; m++)
                            {
                                MapObjectSrv ob = cell.Objects[m];

                                if (ob.Race != ObjectType.Monster) continue;
                                if (ob.Dead) continue;
                                ob.Die();
                            }
                        }

                        break;

                    case "CHANGECLASS": //@changeclass [Player] [Class]
                        if (!IsGM && !Settings.TestServer) return;

                        data = parts.Length <= 2 || !IsGM ? Info : Envir.GetCharacterInfo(parts[1]);

                        if (data == null) return;

                        ExineClass mirClass;

                        if (!Enum.TryParse(parts[parts.Length - 1], true, out mirClass) || data.Class == mirClass) return;

                        data.Class = mirClass;

                        ReceiveChat(string.Format("Player {0} has been changed to {1}", data.Name, data.Class), ChatType.System);
                        MessageQueue.SendMsg(string.Format("Player {0} has been changed to {1} by {2}", data.Name, data.Class, Name));

                        if (data.Player != null)
                            data.Player.Connection.OnRecvLogOutHandler();
                        break;

                    case "DIE":
                        LastHitter = null;
                        Die();
                        break;
                    case "HAIR":
                        if (!IsGM && !Settings.TestServer) return;

                        if (parts.Length < 2)
                        {
                            Info.Hair = (byte)Envir.Random.Next(0, 9);
                        }
                        else
                        {
                            byte tempByte = 0;

                            byte.TryParse(parts[1], out tempByte);

                            Info.Hair = tempByte;
                        }
                        break;

                    case "DECO":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        int.TryParse(parts[1], out tempInt);

                        DecoObjectSrv decoOb = new DecoObjectSrv
                        {
                            Image = tempInt,
                            CurrentMap = CurrentMap,
                            CurrentLocation = CurrentLocation,
                        };

                        CurrentMap.AddObject(decoOb);
                        decoOb.Spawned();

                        SendPacketToClient(decoOb.GetInfo());

                        break;

                    case "ADJUSTPKPOINT":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;

                        if (parts.Length > 2)
                        {
                            if (!IsGM) return;

                            player = Envir.GetPlayer(parts[1]);

                            if (player == null) return;


                            int.TryParse(parts[2], out tempInt);
                        }
                        else
                        {
                            player = this;
                            int.TryParse(parts[1], out tempInt);
                        }

                        player.PKPoints = tempInt;

                        break;

                    
                    case "STARTWAR":
                        if (!IsGM) return;
                        if (parts.Length < 2) return;

                        GuildObjectSrv enemyGuild = Envir.GetGuild(parts[1]);

                        if (MyGuild == null)
                        {
                            ReceiveChat(GameLanguage.NotInGuild, ChatType.System);
                        }

                        if (MyGuild.Ranks[0] != MyGuildRank)
                        {
                            ReceiveChat("You must be a leader to start a war.", ChatType.System);
                            return;
                        }

                        if (enemyGuild == null)
                        {
                            ReceiveChat(string.Format("Could not find guild {0}.", parts[1]), ChatType.System);
                            return;
                        }

                        if (MyGuild == enemyGuild)
                        {
                            ReceiveChat("Cannot go to war with your own guild.", ChatType.System);
                            return;
                        }

                        if (MyGuild.WarringGuilds.Contains(enemyGuild))
                        {
                            ReceiveChat("Already at war with this guild.", ChatType.System);
                            return;
                        }

                        if (MyGuild.GoToWar(enemyGuild))
                        {
                            ReceiveChat(string.Format("You started a war with {0}.", parts[1]), ChatType.System);
                            enemyGuild.SendMessage(string.Format("{0} has started a war", MyGuild.Name), ChatType.System);
                        }
                        break;
                    case "ADDINVENTORY":
                        {
                            int openLevel = (int)((Info.Inventory.Length - 46) / 4);
                            uint openGold = (uint)(1000000 + openLevel * 1000000);
                            if (Account.Gold >= openGold)
                            {
                                Account.Gold -= openGold;
                                SendPacketToClient(new ServerPacket.LoseGold { Gold = openGold });
                                SendPacketToClient(new ServerPacket.ResizeInventory { Size = Info.ResizeInventory() });
                                ReceiveChat(GameLanguage.InventoryIncreased, ChatType.System);
                            }
                            else
                            {
                                ReceiveChat(GameLanguage.LowGold, ChatType.System);
                            }
                            ChatTime = 0;
                        }
                        break;

                    case "ADDSTORAGE":
                        {
                            TimeSpan addedTime = new TimeSpan(10, 0, 0, 0);
                            uint cost = 1000000;

                            if (Account.Gold >= cost)
                            {
                                Account.Gold -= cost;
                                Account.HasExpandedStorage = true;

                                if (Account.ExpandedStorageExpiryDate > Envir.Now)
                                {
                                    Account.ExpandedStorageExpiryDate = Account.ExpandedStorageExpiryDate + addedTime;
                                    ReceiveChat(GameLanguage.ExpandedStorageExpiresOn + Account.ExpandedStorageExpiryDate.ToString(), ChatType.System);
                                }
                                else
                                {
                                    Account.ExpandedStorageExpiryDate = Envir.Now + addedTime;
                                    ReceiveChat(GameLanguage.ExpandedStorageExpiresOn + Account.ExpandedStorageExpiryDate.ToString(), ChatType.System);
                                }

                                SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });
                                SendPacketToClient(new ServerPacket.ResizeStorage { Size = Account.ExpandStorage(), HasExpandedStorage = Account.HasExpandedStorage, ExpiryTime = Account.ExpandedStorageExpiryDate });
                            }
                            else
                            {
                                ReceiveChat(GameLanguage.LowGold, ChatType.System);
                            }
                            ChatTime = 0;
                        }
                        break;

                    

                    case "ALLOWOBSERVE":
                        AllowObserve = !AllowObserve;
                        SendPacketToClient(new ServerPacket.AllowObserve { Allow = AllowObserve });
                        break;

                    case "INFO":
                        {
                            if (!IsGM && !Settings.TestServer) return;

                            MapObjectSrv ob = null;

                            if (parts.Length < 2)
                            {
                                Point target = Functions.PointMove(CurrentLocation, Direction, 1);
                                Cell cell = CurrentMap.GetCell(target);

                                if (cell.Objects == null || cell.Objects.Count < 1) return;

                                ob = cell.Objects[0];
                            }
                            else
                            {
                                ob = Envir.GetPlayer(parts[1]);
                            }

                            if (ob == null) return;

                            switch (ob.Race)
                            {
                                case ObjectType.Player:
                                    PlayerObjectSrv plOb = (PlayerObjectSrv)ob;
                                    ReceiveChat("--Player Info--", ChatType.System2);
                                    ReceiveChat(string.Format("Name : {0}, Level : {1}, X : {2}, Y : {3}", plOb.Name, plOb.Level, plOb.CurrentLocation.X, plOb.CurrentLocation.Y), ChatType.System2);
                                    break;
                                case ObjectType.Monster:
                                    MonsterObjectSrv monOb = (MonsterObjectSrv)ob;
                                    ReceiveChat("--Monster Info--", ChatType.System2);
                                    ReceiveChat(string.Format("ID : {0}, Name : {1}", monOb.Info.Index, monOb.Name), ChatType.System2);
                                    ReceiveChat(string.Format("Level : {0}, X : {1}, Y : {2}, Dir: {3}", monOb.Level, monOb.CurrentLocation.X, monOb.CurrentLocation.Y, monOb.Direction), ChatType.System2);
                                    ReceiveChat(string.Format("HP : {0}, MinDC : {1}, MaxDC : {2}", monOb.Info.Stats[Stat.HP], monOb.Stats[Stat.MinDC], monOb.Stats[Stat.MaxDC]), ChatType.System2);
                                    break;
                                case ObjectType.Merchant:
                                    NPCObjectSrv npcOb = (NPCObjectSrv)ob;
                                    ReceiveChat("--NPC Info--", ChatType.System2);
                                    ReceiveChat(string.Format("ID : {0}, Name : {1}", npcOb.Info.Index, npcOb.Name), ChatType.System2);
                                    ReceiveChat(string.Format("X : {0}, Y : {1}", ob.CurrentLocation.X, ob.CurrentLocation.Y), ChatType.System2);
                                    ReceiveChat(string.Format("File : {0}", npcOb.Info.FileName), ChatType.System2);
                                    break;
                            }
                        }
                        break;

                    case "CLEARQUESTS":
                        if (!IsGM && !Settings.TestServer) return;

                        player = parts.Length > 1 && IsGM ? Envir.GetPlayer(parts[1]) : this;

                        if (player == null)
                        {
                            ReceiveChat(parts[1] + " is not online", ChatType.System);
                            return;
                        }

                        for (int i = player.CurrentQuests.Count - 1; i >= 0; i--)
                        {
                            SendUpdateQuest(player.CurrentQuests[i], QuestState.Remove);
                        }

                        player.CompletedQuests.Clear();
                        player.GetCompletedQuests();

                        break;

                    case "SETQUEST":
                        if ((!IsGM && !Settings.TestServer) || parts.Length < 3) return;

                        player = parts.Length > 3 && IsGM ? Envir.GetPlayer(parts[3]) : this;

                        if (player == null)
                        {
                            ReceiveChat(parts[3] + " is not online", ChatType.System);
                            return;
                        }

                        int.TryParse(parts[1], out int questID);
                        int.TryParse(parts[2], out int questState);

                        if (questID < 1) return;

                        var activeQuest = player.CurrentQuests.FirstOrDefault(e => e.Index == questID);

                        //remove from active list
                        if (activeQuest != null)
                        {
                            player.SendUpdateQuest(activeQuest, QuestState.Remove);
                        }

                        switch (questState)
                        {
                            case 0: //cancel
                                if (player.CompletedQuests.Contains(questID))
                                {
                                    player.CompletedQuests.Remove(questID);
                                }
                                break;
                            case 1: //complete
                                if (!player.CompletedQuests.Contains(questID))
                                {
                                    player.CompletedQuests.Add(questID);
                                }
                                break;
                        }

                        player.GetCompletedQuests();
                        break;

                    case "TOGGLETRANSFORM":
                        if (HasBuff(BuffType.Transform, out Buff transform))
                        {
                            if (transform.Paused)
                            {
                                UnpauseBuff(transform);
                            }
                            else
                            {
                                PauseBuff(transform);
                            }
                            RefreshStats();

                            hintstring = transform.Paused ? "Transform Disabled." : "Transform Enabled.";
                            ReceiveChat(hintstring, ChatType.Hint);
                        }                   
                        break;

                    case "STARTCONQUEST":
                        {
                            if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;
                            int conquestID;

                            if (parts.Length < 1)
                            {
                                ReceiveChat(string.Format("The Syntax is /StartConquest [ConquestID]"), ChatType.System);
                                return;
                            }

                            if (MyGuild == null)
                            {
                                ReceiveChat(string.Format("You need to be in a guild to start a War"), ChatType.System);
                                return;
                            }

                            else if (!int.TryParse(parts[1], out conquestID)) return;

                            ConquestObjectSrv tempConq = Envir.Conquests.FirstOrDefault(t => t.Info.Index == conquestID);

                            if (tempConq != null)
                            {
                                tempConq.StartType = ConquestType.Forced;
                                tempConq.WarIsOn = !tempConq.WarIsOn;
                                tempConq.GuildInfo.AttackerID = MyGuild.Guildindex;
                            }
                            else return;
                            ReceiveChat(string.Format("{0} War Started.", tempConq.Info.Name), ChatType.System);
                            MessageQueue.SendMsg(string.Format("{0} War Started.", tempConq.Info.Name));

                            foreach (var pl in Envir.Players)
                            {
                                if (tempConq.WarIsOn)
                                {
                                    pl.ReceiveChat($"{tempConq.Info.Name} War Started.", ChatType.System);
                                }
                                else
                                {
                                    pl.ReceiveChat($"{tempConq.Info.Name} War Stopped.", ChatType.System);
                                }

                                pl.BroadcastInfo();
                            }
                        }
                        break;
                    case "RESETCONQUEST":
                        {
                            if ((!IsGM && !Settings.TestServer) || parts.Length < 2) return;
                            int conquestID;

                            if (parts.Length < 1)
                            {
                                ReceiveChat(string.Format("The Syntax is /ResetConquest [ConquestID]"), ChatType.System);
                                return;
                            }

                            if (MyGuild == null)
                            {
                                ReceiveChat(string.Format("You need to be in a guild to start a War"), ChatType.System);
                                return;
                            }

                            else if (!int.TryParse(parts[1], out conquestID)) return;

                            ConquestObjectSrv resetConq = Envir.Conquests.FirstOrDefault(t => t.Info.Index == conquestID);

                            if (resetConq != null && !resetConq.WarIsOn)
                            {
                                resetConq.Reset();
                                ReceiveChat(string.Format("{0} has been reset.", resetConq.Info.Name), ChatType.System);
                            }
                            else
                            {
                                ReceiveChat("Conquest not found or War is currently on.", ChatType.System);
                            }
                        }
                        break;
                    case "GATES":
                        if (MyGuild == null || MyGuild.Conquest == null || !MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank) || MyGuild.Conquest.WarIsOn)
                        {
                            ReceiveChat(string.Format("You don't have access to control any gates at the moment."), ChatType.System);
                            return;
                        }

                        bool openClose = false;

                        if (parts.Length > 1)
                        {
                            string openclose = parts[1];

                            if (openclose.ToUpper() == "CLOSE")
                            {
                                openClose = true;
                            }
                            else if (openclose.ToUpper() == "OPEN")
                            {
                                openClose = false;
                            }
                            else
                            {
                                ReceiveChat(string.Format("You must type /Gates Open or /Gates Close."), ChatType.System);
                                return;
                            }

                            for (int i = 0; i < MyGuild.Conquest.GateList.Count; i++)
                            {
                                if (MyGuild.Conquest.GateList[i].Gate != null && !MyGuild.Conquest.GateList[i].Gate.Dead)
                                {
                                    if (openClose)
                                    {
                                        MyGuild.Conquest.GateList[i].Gate.CloseDoor();
                                    }
                                    else
                                    {
                                        MyGuild.Conquest.GateList[i].Gate.OpenDoor();
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < MyGuild.Conquest.GateList.Count; i++)
                            {
                                if (MyGuild.Conquest.GateList[i].Gate != null && !MyGuild.Conquest.GateList[i].Gate.Dead)
                                {
                                    if (!MyGuild.Conquest.GateList[i].Gate.Closed)
                                    {
                                        MyGuild.Conquest.GateList[i].Gate.CloseDoor();
                                        openClose = true;
                                    }
                                    else
                                    {
                                        MyGuild.Conquest.GateList[i].Gate.OpenDoor();
                                        openClose = false;
                                    }
                                }
                            }
                        }

                        if (openClose)
                        {
                            ReceiveChat(string.Format("The gates at {0} have been closed.", MyGuild.Conquest.Info.Name), ChatType.System);
                        }
                        else
                        {
                            ReceiveChat(string.Format("The gates at {0} have been opened.", MyGuild.Conquest.Info.Name), ChatType.System);
                        }
                        break;

                    case "CHANGEFLAG":
                        if (MyGuild == null || MyGuild.Conquest == null || !MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank) || MyGuild.Conquest.WarIsOn)
                        {
                            ReceiveChat(string.Format("You don't have access to change any flags at the moment."), ChatType.System);
                            return;
                        }

                        ushort flag = (ushort)Envir.Random.Next(12);

                        if (parts.Length > 1)
                        {
                            ushort.TryParse(parts[1], out ushort temp);

                            if (temp <= 11) flag = temp;
                        }

                        MyGuild.Info.FlagImage = (ushort)(1000 + flag);

                        for (int i = 0; i < MyGuild.Conquest.FlagList.Count; i++)
                        {
                            MyGuild.Conquest.FlagList[i].UpdateImage();
                        }

                        break;
                    case "CHANGEFLAGCOLOUR":
                        {
                            if (MyGuild == null || MyGuild.Conquest == null || !MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank) || MyGuild.Conquest.WarIsOn)
                            {
                                ReceiveChat(string.Format("You don't have access to change any flags at the moment."), ChatType.System);
                                return;
                            }

                            byte r1 = (byte)Envir.Random.Next(255);
                            byte g1 = (byte)Envir.Random.Next(255);
                            byte b1 = (byte)Envir.Random.Next(255);

                            if (parts.Length > 3)
                            {
                                byte.TryParse(parts[1], out r1);
                                byte.TryParse(parts[2], out g1);
                                byte.TryParse(parts[3], out b1);
                            }

                            MyGuild.Info.FlagColour = Color.FromArgb(255, r1, g1, b1);

                            for (int i = 0; i < MyGuild.Conquest.FlagList.Count; i++)
                            {
                                MyGuild.Conquest.FlagList[i].UpdateColour();
                            }
                        }
                        break;
                    case "REVIVE":
                        if (!IsGM) return;

                        if (parts.Length < 2)
                        {
                            RefreshStats();
                            SetHP(Stats[Stat.HP]);
                            SetMP(Stats[Stat.MP]);
                            Revive(MaxHealth, true);
                        }
                        else
                        {
                            player = Envir.GetPlayer(parts[1]);
                            if (player == null) return;

                            player.Revive(MaxHealth, true);
                        }
                        break;
                    case "DELETESKILL":
                        if ((!IsGM) || parts.Length < 2) return;
                        Spell skill1;

                        if (!Enum.TryParse(parts.Length > 2 ? parts[2] : parts[1], true, out skill1)) return;

                        if (skill1 == Spell.None) return;

                        if (parts.Length > 2)
                        {
                            if (!IsGM) return;
                            player = Envir.GetPlayer(parts[1]);

                            if (player == null)
                            {
                                ReceiveChat(string.Format("Player {0} was not found!", parts[1]), ChatType.System);
                                return;
                            }
                        }
                        else
                        {
                            player = this;
                        }

                        if (player == null) return;

                        var magics = new UserMagic(skill1);
                        bool removed = false;

                        for (var i = player.Info.Magics.Count - 1; i >= 0; i--)
                        {
                            if (player.Info.Magics[i].Spell != skill1) continue;

                            player.Info.Magics.RemoveAt(i);
                            player.SendPacketToClient(new ServerPacket.RemoveMagic { PlaceId = i });
                            removed = true;
                        }

                        if (removed)
                        {
                            ReceiveChat(string.Format("You have deleted skill {0} from player {1}", skill1.ToString(), player.Name), ChatType.Hint);
                            player.ReceiveChat(string.Format("{0} has been removed from you.", skill1), ChatType.Hint);
                        }
                        else
                        {
                            ReceiveChat(string.Format("Unable to delete skill, skill not found"), ChatType.Hint);
                        }

                        break;
                    case "SETTIMER":
                        if (parts.Length < 4) return;

                        string key = parts[1];

                        if (!int.TryParse(parts[2], out int seconds)) return;
                        if (!byte.TryParse(parts[3], out byte timerType)) return;

                        SetTimer(key, seconds, timerType);

                        break;
                    case "SETLIGHT":
                        if ((!IsGM) || parts.Length < 2) return;

                        if (!byte.TryParse(parts[1], out byte light)) return;

                        Light = light;

                        SendPacketToClient(GetUpdateInfo());
                        Broadcast(GetUpdateInfo());
                        break;
                    default:
                        break;
                }

                foreach (string command in Envir.CustomCommands)
                {
                    if (string.Compare(parts[0], command, true) != 0) continue;

                    CallDefaultNPC(DefaultNPCType.CustomCommand, parts[0]);
                }
            }
            else
            {
                message = String.Format("{0}:{1}", CurrentMap.Info.NoNames ? "?????" : Name, message);

                message = ProcessChatItems(message, null, linkedItems);

                p = new ServerPacket.ObjectChat { ObjectID = ObjectID, Text = message, Type = ChatType.Normal };

                SendPacketToClient(p);
                Broadcast(p);
            }
        }
        private string ProcessChatItems(string text, List<PlayerObjectSrv> recipients, List<ChatItem> chatItems)
        {
            if (chatItems == null)
            {
                return text;
            }

            foreach (var chatItem in chatItems)
            {
                Regex r = new Regex(chatItem.RegexInternalName, RegexOptions.IgnoreCase);

                text = r.Replace(text, chatItem.InternalName, 1);

                UserItem[] array;

                switch (chatItem.Grid)
                {
                    case MirGridType.Inventory:
                        array = Info.Inventory;
                        break;
                    case MirGridType.Storage:
                        array = Info.AccountInfo.Storage;
                        break;
                   
                    default:
                        continue;
                }

                UserItem item = null;

                for (int i = 0; i < array.Length; i++)
                {
                    item = array[i];
                    if (item == null || item.UniqueID != chatItem.UniqueID) continue;
                    break;
                }

                if (item != null)
                {
                    if (recipients == null)
                    {
                        for (int i = CurrentMap.Players.Count - 1; i >= 0; i--)
                        {
                            PlayerObjectSrv player = CurrentMap.Players[i];
                            if (player == this) continue;

                            if (player == null || player.Info == null || player.Node == null) continue;

                            if (Functions.InRange(CurrentLocation, player.CurrentLocation, Globals.DataRange))
                            {
                                player.CheckItem(item);

                                if (!player.Connection.SentChatItem.Contains(item))
                                {
                                    player.SendPacketToClient(new ServerPacket.NewChatItem { Item = item });
                                    player.Connection.SentChatItem.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < recipients.Count; i++)
                        {
                            PlayerObjectSrv player = recipients[i];
                            if (player == this) continue;

                            if (player == null || player.Info == null || player.Node == null) continue;

                            player.CheckItem(item);

                            if (!player.Connection.SentChatItem.Contains(item))
                            {
                                player.SendPacketToClient(new ServerPacket.NewChatItem { Item = item });
                                player.Connection.SentChatItem.Add(item);
                            }
                        }
                    }

                    if (!Connection.SentChatItem.Contains(item))
                    {
                        SendPacketToClient(new ServerPacket.NewChatItem { Item = item });
                        Connection.SentChatItem.Add(item);
                    }
                }
            }

            return text;
        }


        public void Turn(ExineDirection dir)
        {
            _stepCounter = 0;

            if (CanMove)
            {
                ActionTime = Envir.Time + GetDelayTime(TurnDelay);

                Direction = dir;
                if (CheckMovement(CurrentLocation)) return;

                SafeZoneInfo szi = CurrentMap.GetSafeZone(CurrentLocation);

                if (szi != null)
                {
                    BindLocation = szi.Location;
                    BindMapIndex = CurrentMapIndex;
                    InSafeZone = true;
                }
                else
                    InSafeZone = false;

                Cell cell = CurrentMap.GetCell(CurrentLocation);

                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;
                    SpellObjectSrv ob = (SpellObjectSrv)cell.Objects[i];

                    ob.ProcessSpell(this);
                    //break;
                }

                if (TradePartner != null)
                    TradeCancel();
                  
                Broadcast(new ServerPacket.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            }

            SendPacketToClient(new ServerPacket.UserLocation { Direction = Direction, Location = CurrentLocation });
        }


        //최종적으로는 클라이언트 요청 > 서버에서 수신 후 패킷 내려줌 > 클라이언트에서 패킷 받은 후 처리 가 맞음.
        //일단은 전체 네트워크에 뿌리고 수신하는 부분까지만 확인할것.
        public void Rest(ExineDirection dir) //add k333123 240926
        { 
            Broadcast(new ServerPacket.ObjectRest { ObjectID = ObjectID, Direction = dir, Location = CurrentLocation });
            SendPacketToClient(new ServerPacket.UserLocation { Direction = Direction, Location = CurrentLocation });
        }

        public void Harvest(ExineDirection dir)
        {
            if (!CanMove)
            {
                SendPacketToClient(new ServerPacket.UserLocation { Direction = Direction, Location = CurrentLocation });
                return;
            }

            ActionTime = Envir.Time + HarvestDelay;

            Direction = dir;

            SendPacketToClient(new ServerPacket.UserLocation { Direction = Direction, Location = CurrentLocation });
            Broadcast(new ServerPacket.ObjectHarvest { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            Point front = Front;
            bool send = false;
            for (int d = 0; d <= 1; d++)
            {
                for (int y = front.Y - d; y <= front.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = front.X - d; x <= front.X + d; x += Math.Abs(y - front.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObjectSrv ob = cell.Objects[i];
                            if (ob.Race != ObjectType.Monster || !ob.Dead || ob.Harvested) continue;

                            if (ob.EXPOwner != null && ob.EXPOwner != this && !IsMember(ob))
                            {
                                send = true;
                                continue;
                            }

                            if (ob.Harvest(this)) return;
                        }
                    }
                }
            }

            if (send)
                ReceiveChat("You do not own any nearby carcasses.", ChatType.System);
        }        


        private void CompleteQuest(IList<object> data)
        {
            QuestProgressInfo quest = (QuestProgressInfo)data[0];
            QuestAction questAction = (QuestAction)data[1];
            bool ignoreIfComplete = (bool)data[2];

            if (quest == null) return;

            switch (questAction)
            {
                case QuestAction.TimeExpired:
                    {
                        if (ignoreIfComplete && quest.Completed)
                        {
                            return;
                        }

                        AbandonQuest(quest.Info.Index);
                    }
                    break;
            }
        }
        private void CompleteNPC(IList<object> data)
        {
            uint npcid = (uint)data[0];
            int scriptid = (int)data[1];
            string page = (string)data[2];

            if (data.Count == 5)
            {
                Map map = (Map)data[3];
                Point coords = (Point)data[4];

                Teleport(map, coords);
            }

            NPCDelayed = true;

            if (page.Length > 0)
            {
                var script = NPCScript.Get(scriptid);
                script.Call(this, npcid, page.ToUpper());
            }
        }
        private UserItem GetBait(int count)
        {
            UserItem item = Info.Equipment[(int)EquipmentSlot.Weapon];
            if (item == null || item.Info.Type != ItemType.Weapon || !item.Info.IsFishingRod) return null;

            UserItem bait = item.Slots[(int)FishingSlot.Bait];

            if (bait == null || bait.Count < count) return null;

            return bait;
        }
        private UserItem GetFishingItem(FishingSlot type)
        {
            UserItem item = Info.Equipment[(int)EquipmentSlot.Weapon];
            if (item == null || item.Info.Type != ItemType.Weapon || !item.Info.IsFishingRod) return null;

            UserItem fishingItem = item.Slots[(int)type];

            if (fishingItem == null) return null;

            return fishingItem;
        }
        private void DeleteFishingItem(FishingSlot type)
        {
            UserItem item = Info.Equipment[(int)EquipmentSlot.Weapon];
            if (item == null || item.Info.Type != ItemType.Weapon || !item.Info.IsFishingRod) return;

            UserItem slotItem = Info.Equipment[(int)EquipmentSlot.Weapon].Slots[(int)type];

            SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = slotItem.UniqueID, Count = 1 });
            Info.Equipment[(int)EquipmentSlot.Weapon].Slots[(int)type] = null;

            Report.ItemChanged(slotItem, 1, 1);
        }
        private void DamagedFishingItem(FishingSlot type, int lossDura)
        {
            UserItem item = GetFishingItem(type);

            if (item != null)
            {
                if (item.CurrentDura <= 0)
                {

                    DeleteFishingItem(type);
                }
                else
                {
                    DamageItem(item, lossDura, true);
                }
            }
        }        
        public override bool CheckMovement(Point location)
        {
            if (Envir.Time < MovementTime) return false;

            //Script triggered coords
            for (int s = 0; s < CurrentMap.Info.ActiveCoords.Count; s++)
            {
                Point activeCoord = CurrentMap.Info.ActiveCoords[s];

                if (activeCoord != location) continue;

                CallDefaultNPC(DefaultNPCType.MapCoord, CurrentMap.Info.FileName, activeCoord.X, activeCoord.Y);
            }

            //Map movements
            for (int i = 0; i < CurrentMap.Info.Movements.Count; i++)
            {
                MovementInfo info = CurrentMap.Info.Movements[i];

                if (info.Source != location) continue;

                if (info.NeedHole)
                {
                    Cell cell = CurrentMap.GetCell(location);

                    if (cell.Objects == null ||
                        cell.Objects.Where(ob => ob.Race == ObjectType.Spell).All(ob => ((SpellObjectSrv)ob).Spell != Spell.DigOutZombie && ((SpellObjectSrv)ob).Spell != Spell.DigOutArmadillo))
                        continue;
                }

                if (info.ConquestIndex > 0)
                {
                    if (MyGuild == null || MyGuild.Conquest == null) continue;
                    if (MyGuild.Conquest.Info.Index != info.ConquestIndex) continue;
                }

                if (info.NeedMove) //use with ENTERMAP npc command
                {
                    NPCData["NPCMoveMap"] = Envir.GetMap(info.MapIndex);
                    NPCData["NPCMoveCoord"] = info.Destination;
                    continue;
                }

                Map temp = Envir.GetMap(info.MapIndex);

                if (temp == null || !temp.ValidPoint(info.Destination)) continue;

                CurrentMap.RemoveObject(this);
                Broadcast(new ServerPacket.ObjectRemove { ObjectID = ObjectID });

                CompleteMapMovement(temp, info.Destination, CurrentMap, CurrentLocation);
                return true;
            }

            return false;
        }
        private void CompleteMapMovement(params object[] data)
        {
            if (this == null) return;
            Map temp = (Map)data[0];
            Point destination = (Point)data[1];
            Map checkmap = (Map)data[2];
            Point checklocation = (Point)data[3];

            if (CurrentMap != checkmap || CurrentLocation != checklocation) return;

            bool mapChanged = temp != CurrentMap;

            CurrentMap = temp;
            CurrentLocation = destination;

            CurrentMap.AddObject(this);

            MovementTime = Envir.Time + MovementDelay;

            SendPacketToClient(new ServerPacket.MapChanged
            {
                MapIndex = CurrentMap.Info.Index,
                FileName = CurrentMap.Info.FileName,
                Title = CurrentMap.Info.Title,
                MiniMap = CurrentMap.Info.MiniMap,
                BigMap = CurrentMap.Info.BigMap,
                Lights = CurrentMap.Info.Light,
                Location = CurrentLocation,
                Direction = Direction,
                MapDarkLight = CurrentMap.Info.MapDarkLight,
                Music = CurrentMap.Info.Music
            });

            

            GetObjects();

            SafeZoneInfo szi = CurrentMap.GetSafeZone(CurrentLocation);

            if (szi != null)
            {
                BindLocation = szi.Location;
                BindMapIndex = CurrentMapIndex;
                InSafeZone = true;
            }
            else
                InSafeZone = false;

            if (mapChanged)
            {
                CallDefaultNPC(DefaultNPCType.MapEnter, CurrentMap.Info.FileName);
                GroupMemberMapNameChanged();
            }
            GetPlayerLocation();

            if (Info.Married != 0)
            {
                CharacterInfo Lover = Envir.GetCharacterInfo(Info.Married);
                PlayerObjectSrv player = Envir.GetPlayer(Lover.Name);

                if (player != null) player.GetRelationship(false);
            }

            CheckConquest(true);
        }
        public bool TeleportEscape(int attempts)
        {
            Map temp = Envir.GetMap(BindMapIndex);

            for (int i = 0; i < attempts; i++)
            {
                Point location = new Point(BindLocation.X + Envir.Random.Next(-100, 100),
                                           BindLocation.Y + Envir.Random.Next(-100, 100));

                if (Teleport(temp, location)) return true;
            }

            return false;
        }
        public override bool MagicTeleport(UserMagic magic)
        {
            Map temp = Envir.GetMap(BindMapIndex);
            int mapSizeX = temp.Width / (magic.Level + 1);
            int mapSizeY = temp.Height / (magic.Level + 1);

            for (int i = 0; i < 200; i++)
            {
                Point location = new Point(BindLocation.X + Envir.Random.Next(-mapSizeX, mapSizeX),
                                     BindLocation.Y + Envir.Random.Next(-mapSizeY, mapSizeY));

                if (Teleport(temp, location)) return true;
            }

            return false;
        }

        public override bool IsAttackTarget(HumanObjectSrv attacker)
        {            
            if (attacker == null || attacker.Node == null) return false; 
            if (Dead || InSafeZone || attacker.InSafeZone || attacker == this || GMGameMaster) return false;
            if (CurrentMap.Info.NoFight) return false;

            switch (attacker.AMode)
            {
                case AttackMode.All:
                    return true;
                case AttackMode.Group:
                    return GroupMembers == null || !GroupMembers.Contains(attacker);
                case AttackMode.Guild:
                    return MyGuild == null || MyGuild != attacker.MyGuild;
                case AttackMode.EnemyGuild:
                    return MyGuild != null && MyGuild.IsEnemy(attacker.MyGuild);
                case AttackMode.Peace:
                    return false;
                case AttackMode.RedBrown:
                    return PKPoints >= 200 || Envir.Time < BrownTime;
            }

            return true;
        }
        public override bool IsAttackTarget(MonsterObjectSrv attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead || attacker.Master == this || GMGameMaster) return false;
            if (attacker.Info.AI == 6 || attacker.Info.AI == 58 || attacker.Info.AI == 113) return PKPoints >= 200;
            if (attacker.Master == null) return true;
            if (InSafeZone || attacker.InSafeZone || attacker.Master.InSafeZone) return false;

            if (LastHitter != attacker.Master && attacker.Master.LastHitter != this)
            {
                bool target = false;
                  
                if (!target)
                    return false;
            }

            switch (attacker.Master.AMode)
            {
                case AttackMode.All:
                    return true;
                case AttackMode.Group:
                    return GroupMembers == null || !GroupMembers.Contains(attacker.Master);
                case AttackMode.Guild:
                    return true;
                case AttackMode.EnemyGuild:
                    return false;
                case AttackMode.Peace:
                    return false;
                case AttackMode.RedBrown:
                    return PKPoints >= 200 || Envir.Time < BrownTime;
            }

            return true;
        }
        public override bool IsFriendlyTarget(HumanObjectSrv ally)
        {
            if (ally == this) return true; 

            switch (ally.AMode)
            {
                case AttackMode.Group:
                    return GroupMembers != null && GroupMembers.Contains(ally);
                case AttackMode.RedBrown:
                    return PKPoints < 200 & Envir.Time > BrownTime;
                case AttackMode.Guild:
                    return MyGuild != null && MyGuild == ally.MyGuild;
                case AttackMode.EnemyGuild:
                    return true;
            }
            return true;
        }
        public override bool IsFriendlyTarget(MonsterObjectSrv ally)
        {
            if (ally.Race != ObjectType.Monster) return false;
            if (ally.Master == null) return false;

            switch (ally.Master.Race)
            {
                case ObjectType.Player:
                    if (!ally.Master.IsFriendlyTarget(this)) return false;
                    break;
                case ObjectType.Monster:
                    return false;
            }

            return true;
        }
        protected override void UpdateLooks(short OldLooks_Weapon)
        {
            base.UpdateLooks(OldLooks_Weapon);

            if (Globals.FishingRodShapes.Contains(OldLooks_Weapon) != Globals.FishingRodShapes.Contains(Looks_Weapon))
            {
                SendPacketToClient(GetFishInfo());
            }
        }
        public override Packet GetInfo()
        {
            //should never use this but i leave it in for safety
            if (Observer) return null;

            string gName = "";
            string conquest = "";
            if (MyGuild != null)
            {
                gName = MyGuild.Name;
                if (MyGuild.Conquest != null)
                {
                    conquest = "[" + MyGuild.Conquest.Info.Name + "]";
                    gName = gName + conquest;
                }
                    
            }

            return new ServerPacket.ObjectPlayer
            {
                ObjectID = ObjectID,
                Name = CurrentMap.Info.NoNames ? "?????" : Name,
                NameColour = NameColour,
                GuildName = CurrentMap.Info.NoNames ? "?????" : gName,
                GuildRankName = CurrentMap.Info.NoNames ? "?????" : MyGuildRank != null ? MyGuildRank.Name : "",
                Class = Class,
                Gender = Gender,
                ExStyle =ExStyle, //add k333123
                ExColor = ExColor, //add k333123
                ExPortraitLen = ExPortraitLen,//add k333123
                ExPortraitBytes = ExPortraitBytes,//add k333123
                //Buffer.BlockCopy(ExPortraitBytes, 0, ExPortraitBytes, 0, ExPortraitBytes),
                Level = Level,
                Location = CurrentLocation,
                Direction = Direction,
                Hair = Hair,
                Weapon = Looks_Weapon,
				WeaponEffect = Looks_WeaponEffect,
                Shield = Looks_Shield, //add k333123
                Armour = Looks_Armour,
                Light = Light,
                Poison = CurrentPoison,
                Dead = Dead,
                Hidden = Hidden,
                Effect = HasBuff(BuffType.MagicShield, out _) ? SpellEffect.MagicShieldUp : HasBuff(BuffType.ElementalBarrier, out _) ? SpellEffect.ElementalBarrierUp : SpellEffect.None,
                WingEffect = Looks_Wings,
                
                Fishing = Fishing,

                TransformType = TransformType,

                ElementOrbEffect = (uint)GetElementalOrbCount(),
                ElementOrbLvl = (uint)ElementsLevel,
                ElementOrbMax = (uint)Settings.OrbsExpList[Settings.OrbsExpList.Count - 1],

                Buffs = Buffs.Where(d => d.Info.Visible).Select(e => e.Type).ToList(),

                LevelEffects = LevelEffects
            };
        }
        public void EquipSlotItem(MirGridType grid, ulong id, int to, MirGridType gridTo, ulong idTo)
        {
            ServerPacket.EquipSlotItem p = new ServerPacket.EquipSlotItem { Grid = grid, UniqueID = id, To = to, GridTo = gridTo, Success = false };

            UserItem item = null;

            switch (gridTo)
            {
               
                case MirGridType.Fishing:
                    item = Info.Equipment[(int)EquipmentSlot.Weapon];
                    break;
                case MirGridType.Socket:
                    UserItem temp2;
                    for (int i = 0; i < Info.Equipment.Length; i++)
                    {
                        temp2 = Info.Equipment[i];
                        if (temp2 == null || temp2.UniqueID != idTo) continue;
                        item = temp2;
                        break;
                    }
                    for (int i = 0; i < Info.Inventory.Length; i++)
                    {
                        temp2 = Info.Inventory[i];
                        if (temp2 == null || temp2.UniqueID != idTo) continue;
                        item = temp2;
                        break;
                    }
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }

            if (item == null || item.Slots == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (gridTo == MirGridType.Fishing && !item.Info.IsFishingRod)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= item.Slots.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (item.Slots[to] != null)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem[] array;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    if (Info.Equipment[to] != null &&
                        Info.Equipment[to].Info.Bind.HasFlag(BindMode.DontStore))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    array = Account.Storage;
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }


            int index = -1;
            UserItem temp = null;

            for (int i = 0; i < array.Length; i++)
            {
                temp = array[i];
                if (temp == null || temp.UniqueID != id) continue;
                index = i;
                break;
            }

            if (temp == null || index == -1)
            {
                SendPacketToClient(p);
                return;
            }

            if ((item.Info.IsFishingRod ) && temp.Info.Type == ItemType.Socket)
            {
                SendPacketToClient(p);
                return;
            }

            if ((temp.SoulBoundId != -1) && (temp.SoulBoundId != Info.Index))
            {
                SendPacketToClient(p);
                return;
            }

            if (CanUseItem(temp))
            {
                if (temp.Info.NeedIdentify && !temp.Identified)
                {
                    temp.Identified = true;
                    SendPacketToClient(new ServerPacket.RefreshItem { Item = temp });
                }

                switch (temp.Info.Shape)
                {
                    case 1:
                        if (item.Info.Type != ItemType.Weapon)
                        {
                            SendPacketToClient(p);
                            return;
                        }
                        break;
                    case 2:
                        if (item.Info.Type != ItemType.Armour)
                        {
                            SendPacketToClient(p);
                            return;
                        }
                        break;
                    case 3:
                        if (item.Info.Type != ItemType.Ring && item.Info.Type != ItemType.Bracelet && item.Info.Type != ItemType.Necklace)
                        {
                            SendPacketToClient(p);
                            return;
                        }
                        break;
                }

                //if ((temp.Info.BindOnEquip) && (temp.SoulBoundId == -1))
                //{
                //    temp.SoulBoundId = Info.Index;
                //    Enqueue(new ServerPacket.RefreshItem { Item = temp });
                //}
                //if (UnlockCurse && Info.Equipment[to].Cursed)
                //    UnlockCurse = false;

                item.Slots[to] = temp;
                array[index] = null;

                p.Success = true;
                SendPacketToClient(p);
                RefreshStats();

                Report.ItemMoved(temp, grid, gridTo, index, to);

                return;
            }

            SendPacketToClient(p);
        }
        public void RemoveItem(MirGridType grid, ulong id, int to)
        {
            ServerPacket.RemoveItem p = new ServerPacket.RemoveItem { Grid = grid, UniqueID = id, To = to, Success = false };
            UserItem[] toArray, fromArray;
            MirGridType fromGrid;
            switch (grid)
            {
                case MirGridType.Inventory:
                    toArray = Info.Inventory;
                    fromArray = Info.Equipment;
                    fromGrid = MirGridType.Equipment;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    toArray = Account.Storage;
                    fromArray = Info.Equipment;
                    fromGrid = MirGridType.Equipment;
                    break;
                
                default:
                    SendPacketToClient(p);
                    return;
            }

            if (to < 0 || to >= toArray.Length) return;

            UserItem temp = null;
            int index = -1;

            for (int i = 0; i < fromArray.Length; i++)
            {
                temp = fromArray[i];
                if (temp == null || temp.UniqueID != id) continue;
                index = i;
                break;
            }

            if (temp == null || index == -1)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Cursed && !UnlockCurse)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.WeddingRing != -1)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Info.Bind.HasFlag(BindMode.DontStore) && grid == MirGridType.Storage)
            {
                SendPacketToClient(p);
                return;
            }

            if (!CanRemoveItem(grid, temp)) return;

            if (temp.Cursed)
                UnlockCurse = false;

            if (toArray[to] == null)
            {
                fromArray[index] = null;

                toArray[to] = temp;
                p.Success = true;
                SendPacketToClient(p);
                
                    RefreshStats();
                    Broadcast(GetUpdateInfo()); 

                Report.ItemMoved(temp, fromGrid, grid, index, to);

                return;
            }

            SendPacketToClient(p);
        }
        public void RemoveSlotItem(MirGridType grid, ulong id, int to, MirGridType gridTo, ulong idFrom)
        {
            ServerPacket.RemoveSlotItem p = new ServerPacket.RemoveSlotItem { Grid = grid, UniqueID = id, To = to, GridTo = gridTo, Success = false };
            UserItem[] array;
            switch (gridTo)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    array = Account.Storage;
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }

            if (to < 0 || to >= array.Length) return;

            UserItem temp = null;
            UserItem slotTemp = null;
            int index = -1;

            switch (grid)
            {
                case MirGridType.Fishing:
                    temp = Info.Equipment[(int)EquipmentSlot.Weapon];
                    break;
                case MirGridType.Socket:
                    UserItem temp2;
                    for (int i = 0; i < Info.Equipment.Length; i++)
                    {
                        temp2 = Info.Equipment[i];
                        if (temp2 == null || temp2.UniqueID != idFrom) continue;
                        temp = temp2;
                        break;
                    }
                    for (int i = 0; i < Info.Inventory.Length; i++)
                    {
                        temp2 = Info.Inventory[i];
                        if (temp2 == null || temp2.UniqueID != idFrom) continue;
                        temp = temp2;
                        break;
                    }
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }

            if (temp == null || temp.Slots == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (grid == MirGridType.Fishing && !temp.Info.IsFishingRod)
            {
                SendPacketToClient(p);
                return;
            }

            for (int i = 0; i < temp.Slots.Length; i++)
            {
                slotTemp = temp.Slots[i];
                if (slotTemp == null || slotTemp.UniqueID != id) continue;
                index = i;
                break;
            }

            if (slotTemp == null || index == -1)
            {
                SendPacketToClient(p);
                return;
            }

            if (slotTemp.Cursed && !UnlockCurse)
            {
                SendPacketToClient(p);
                return;
            }

            if (slotTemp.WeddingRing != -1)
            {
                SendPacketToClient(p);
                return;
            }

            if (!CanRemoveItem(gridTo, slotTemp)) return;

            temp.Slots[index] = null;

            if (slotTemp.Cursed)
                UnlockCurse = false;

            if (array[to] == null)
            {
                array[to] = slotTemp;
                p.Success = true;
                SendPacketToClient(p);
                RefreshStats();
                Broadcast(GetUpdateInfo());

                Report.ItemMoved(temp, grid, gridTo, index, to);

                return;
            }

            SendPacketToClient(p);
        }
        public void MoveItem(MirGridType grid, int from, int to)
        {
            ServerPacket.MoveItem p = new ServerPacket.MoveItem { Grid = grid, From = from, To = to, Success = false };
            UserItem[] array;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    array = Account.Storage;
                    break;
                case MirGridType.Trade:
                    array = Info.Trade;
                    TradeItem();
                    break;
                case MirGridType.Refine:
                    array = Info.Refine;
                    break;
                    
                default:
                    SendPacketToClient(p);
                    return;
            }

            if (from >= 0 && to >= 0 && from < array.Length && to < array.Length)
            {
                if (array[from] == null)
                {
                    Report.ItemError(grid, grid, from, to);
                    ReceiveChat("Item Move Error - Please report the item you tried to move and the time", ChatType.System);
                    SendPacketToClient(p);
                    return;
                }

                UserItem i = array[to];
                array[to] = array[from];

                Report.ItemMoved(array[to], grid, grid, from, to);

                array[from] = i;

                Report.ItemMoved(array[from], grid, grid, to, from);
                
                p.Success = true;
                SendPacketToClient(p);
                return;
            }

            SendPacketToClient(p);
        }
        public void StoreItem(int from, int to)
        {
            ServerPacket.StoreItem p = new ServerPacket.StoreItem { From = from, To = to, Success = false };

            if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
            {
                SendPacketToClient(p);
                return;
            }
            NPCObjectSrv ob = null;
            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                ob = CurrentMap.NPCs[i];             
                break;
            }

            if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
            {
                SendPacketToClient(p);
                return;
            }


            if (from < 0 || from >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Account.Storage.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Info.Inventory[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Info.Bind.HasFlag(BindMode.DontStore))
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.RentalInformation != null && temp.RentalInformation.BindingFlags.HasFlag(BindMode.DontStore))
            {
                SendPacketToClient(p);
                return;
            }

            if (Account.Storage[to] == null)
            {
                Account.Storage[to] = temp;
                Info.Inventory[from] = null;
                RefreshBagWeight();

                Report.ItemMoved(temp, MirGridType.Inventory, MirGridType.Storage, from, to);

                p.Success = true;
                SendPacketToClient(p);
                return;
            }
            SendPacketToClient(p);
        }
        public void TakeBackItem(int from, int to)
        {
            ServerPacket.TakeBackItem p = new ServerPacket.TakeBackItem { From = from, To = to, Success = false };

            if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
            {
                SendPacketToClient(p);
                return;
            }
            NPCObjectSrv ob = null;
            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                ob = CurrentMap.NPCs[i];
                break;
            }

            if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
            {
                SendPacketToClient(p);
                return;
            }


            if (from < 0 || from >= Account.Storage.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Account.Storage[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (Info.Inventory[to] == null)
            {
                Info.Inventory[to] = temp;
                Account.Storage[from] = null;

                Report.ItemMoved(temp, MirGridType.Storage, MirGridType.Inventory, from, to);

                p.Success = true;
                RefreshBagWeight();
                SendPacketToClient(p);

                return;
            }
            SendPacketToClient(p);
        }
        public void EquipItem(MirGridType grid, ulong id, int to)
        {
            ServerPacket.EquipItem p = new ServerPacket.EquipItem { Grid = grid, UniqueID = id, To = to, Success = false };

            if ((grid == MirGridType.Inventory || grid == MirGridType.Storage) && Fishing)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem[] toArray = null;
            MirGridType toGrid = MirGridType.Equipment;
            HumanObjectSrv actor = this;
            switch (grid)
            {
                case MirGridType.Inventory:
                case MirGridType.Storage:
                    toArray = Info.Equipment;
                    break;
                
            }

            if (toArray == null || to < 0 || to >= toArray.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem[] array;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    array = Account.Storage;
                    break;
                    
                default:
                    SendPacketToClient(p);
                    return;
            }

            int index = -1;
            UserItem temp = null;

            for (int i = 0; i < array.Length; i++)
            {
                temp = array[i];
                if (temp == null || temp.UniqueID != id) continue;
                index = i;
                break;
            }

            if (temp == null || index == -1)
            {
                SendPacketToClient(p);
                return;
            }
            if ((toArray[to] != null) && (toArray[to].Cursed) && (!UnlockCurse))
            {
                SendPacketToClient(p);
                return;
            }

            if ((temp.SoulBoundId != -1) && (temp.SoulBoundId != Info.Index))
            {
                SendPacketToClient(p);
                return;
            }

            if (toArray[to] != null)
                if (toArray[to].WeddingRing != -1)
                {
                    SendPacketToClient(p);
                    return;
                }
            if (toArray[to] != null &&
                toArray[to].Info.Bind.HasFlag(BindMode.DontStore))
            {
                SendPacketToClient(p);
                return;
            }

            if (actor.CanEquipItem(temp, to))
            {
                if (temp.Info.NeedIdentify && !temp.Identified)
                {
                    temp.Identified = true;
                    SendPacketToClient(new ServerPacket.RefreshItem { Item = temp });
                }
                if ((temp.Info.Bind.HasFlag(BindMode.BindOnEquip)) && (temp.SoulBoundId == -1))
                {
                    temp.SoulBoundId = Info.Index;
                    SendPacketToClient(new ServerPacket.RefreshItem { Item = temp });
                }

                if ((toArray[to] != null) && (toArray[to].Cursed) && (UnlockCurse))
                    UnlockCurse = false;

                array[index] = toArray[to];

                Report.ItemMoved(temp, toGrid, grid, to, index, "RemoveItem");

                toArray[to] = temp;

                Report.ItemMoved(temp, grid, toGrid, index, to);

                p.Success = true;
                SendPacketToClient(p);
               
                    RefreshStats();

                //Broadcast(GetUpdateInfo());
                return;
            }
            SendPacketToClient(p);
        }
        public override void UseItem(ulong id)
        {
            ServerPacket.UseItem p = new ServerPacket.UseItem { UniqueID = id, Grid = MirGridType.Inventory, Success = false };

            UserItem item = null;
            int index = -1;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                item = Info.Inventory[i];
                if (item == null || item.UniqueID != id) continue;
                index = i;
                break;
            }

            if (item == null || index == -1 || !CanUseItem(item))
            {
                SendPacketToClient(p);
                return;
            }

            if (Dead && !(item.Info.Type == ItemType.Scroll && item.Info.Shape == 6))
            {
                SendPacketToClient(p);
                return;
            }

            switch (item.Info.Type)
            {
                case ItemType.Potion:
                    switch (item.Info.Shape)
                    {
                        case 0: //NormalPotion
                            PotHealthAmount = (ushort)Math.Min(ushort.MaxValue, PotHealthAmount + item.Info.Stats[Stat.HP]);
                            PotManaAmount = (ushort)Math.Min(ushort.MaxValue, PotManaAmount + item.Info.Stats[Stat.MP]);
                            break;
                        case 1: //SunPotion
                            ChangeHP(item.Info.Stats[Stat.HP]);
                            ChangeMP(item.Info.Stats[Stat.MP]);
                            break;
                        case 2: //MysteryWater
                            if (UnlockCurse)
                            {
                                ReceiveChat("You can already unequip a cursed item.", ChatType.Hint);
                                SendPacketToClient(p);
                                return;
                            }
                            ReceiveChat("You can now unequip a cursed item.", ChatType.Hint);
                            UnlockCurse = true;
                            break;
                        case 3: //Buff
                            {
                                int time = item.Info.Durability;

                                if (item.GetTotal(Stat.MaxDC) > 0)
                                    AddBuff(BuffType.Impact, this, time * Settings.Minute, new Stats { [Stat.MaxDC] = item.GetTotal(Stat.MaxDC) });

                                if (item.GetTotal(Stat.MaxMC) > 0)
                                    AddBuff(BuffType.Magic, this, time * Settings.Minute, new Stats { [Stat.MaxMC] = item.GetTotal(Stat.MaxMC) });

                                if (item.GetTotal(Stat.MaxSC) > 0)
                                    AddBuff(BuffType.Taoist, this, time * Settings.Minute, new Stats { [Stat.MaxSC] = item.GetTotal(Stat.MaxSC) });

                                if (item.GetTotal(Stat.AttackSpeed) > 0)
                                    AddBuff(BuffType.Storm, this, time * Settings.Minute, new Stats { [Stat.AttackSpeed] = item.GetTotal(Stat.AttackSpeed) });

                                if (item.GetTotal(Stat.HP) > 0)
                                    AddBuff(BuffType.HealthAid, this, time * Settings.Minute, new Stats { [Stat.HP] = item.GetTotal(Stat.HP) });

                                if (item.GetTotal(Stat.MP) > 0)
                                    AddBuff(BuffType.ManaAid, this, time * Settings.Minute, new Stats { [Stat.MP] = item.GetTotal(Stat.MP) });

                                if (item.GetTotal(Stat.MaxAC) > 0)
                                    AddBuff(BuffType.Defence, this, time * Settings.Minute, new Stats { [Stat.MaxAC] = item.GetTotal(Stat.MaxAC) });

                                if (item.GetTotal(Stat.MaxMAC) > 0)
                                    AddBuff(BuffType.MagicDefence, this, time * Settings.Minute, new Stats { [Stat.MaxMAC] = item.GetTotal(Stat.MaxMAC) });

                                if (item.GetTotal(Stat.BagWeight) > 0)
                                    AddBuff(BuffType.BagWeight, this, time * Settings.Minute, new Stats { [Stat.BagWeight] = item.GetTotal(Stat.BagWeight) });
                            }
                            break;
                        case 4: //Exp
                            {
                                int time = item.Info.Durability;
                                AddBuff(BuffType.Exp, this, Settings.Minute * time, new Stats { [Stat.ExpRatePercent] = item.GetTotal(Stat.Luck) });
                            }
                            break;
                        case 5: //Drop
                            {
                                int time = item.Info.Durability;
                                AddBuff(BuffType.Drop, this, Settings.Minute * time, new Stats { [Stat.ItemDropRatePercent] = item.GetTotal(Stat.Luck) });
                            }
                            break;
                    }
                    break;
                case ItemType.Scroll:
                    UserItem temp;
                    switch (item.Info.Shape)
                    {
                        case 0: //DE
                            if (!TeleportEscape(20))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            foreach (DelayedAction ac in ActionList.Where(u => u.Type == DelayedType.NPC))
                            {
                                ac.FlaggedToRemove = true;
                            }
                            break;
                        case 1: //TT
                            if (!Teleport(Envir.GetMap(BindMapIndex), BindLocation))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            foreach (DelayedAction ac in ActionList.Where(u => u.Type == DelayedType.NPC))
                            {
                                ac.FlaggedToRemove = true;
                            }
                            break;
                        case 2: //RT
                            if (!TeleportRandom(200, item.Info.Durability))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            foreach (DelayedAction ac in ActionList.Where(u => u.Type == DelayedType.NPC))
                            {
                                ac.FlaggedToRemove = true;
                            }
                            break;
                        case 3: //BenedictionOil
                            if (!TryLuckWeapon())
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            break;
                        case 4: //RepairOil
                            temp = Info.Equipment[(int)EquipmentSlot.Weapon];
                            if (temp == null || temp.MaxDura == temp.CurrentDura)
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            if (temp.Info.Bind.HasFlag(BindMode.DontRepair))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            temp.MaxDura = (ushort)Math.Max(0, temp.MaxDura - Math.Min(5000, temp.MaxDura - temp.CurrentDura) / 30);

                            temp.CurrentDura = (ushort)Math.Min(temp.MaxDura, temp.CurrentDura + 5000);
                            temp.DuraChanged = false;

                            ReceiveChat("Your weapon has been partially repaired", ChatType.Hint);
                            SendPacketToClient(new ServerPacket.ItemRepaired { UniqueID = temp.UniqueID, MaxDura = temp.MaxDura, CurrentDura = temp.CurrentDura });
                            break;
                        case 5: //WarGodOil
                            temp = Info.Equipment[(int)EquipmentSlot.Weapon];
                            if (temp == null || temp.MaxDura == temp.CurrentDura)
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            if (temp.Info.Bind.HasFlag(BindMode.DontRepair) || (temp.Info.Bind.HasFlag(BindMode.NoSRepair)))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            temp.CurrentDura = temp.MaxDura;
                            temp.DuraChanged = false;

                            ReceiveChat("Your weapon has been completely repaired", ChatType.Hint);
                            SendPacketToClient(new ServerPacket.ItemRepaired { UniqueID = temp.UniqueID, MaxDura = temp.MaxDura, CurrentDura = temp.CurrentDura });
                            break;
                        case 6: //ResurrectionScroll
                            if (CurrentMap.Info.NoReincarnation)
                            {
                                ReceiveChat(string.Format("Cannot use on this map"), ChatType.System);
                                SendPacketToClient(p);
                                return;
                            }
                            if (Dead)
                            {
                                MP = Stats[Stat.MP];
                                Revive(MaxHealth, true);
                            }
                            break;
                        case 7: //CreditScroll
                            if (item.Info.Price > 0)
                            {
                                GainCredit(item.Info.Price);
                                ReceiveChat(String.Format("{0} Credits have been added to your Account", item.Info.Price), ChatType.Hint);
                            }
                            break;
                        case 8: //MapShoutScroll
                            HasMapShout = true;
                            ReceiveChat("You have been given one free shout across your current map", ChatType.Hint);
                            break;
                        case 9://ServerShoutScroll
                            HasServerShout = true;
                            ReceiveChat("You have been given one free shout across the server", ChatType.Hint);
                            break;
                        case 10://GuildSkillScroll
                            MyGuild.NewBuff(item.Info.Effect, false);
                            break;
                        case 11://HomeTeleport
                            if (MyGuild != null && MyGuild.Conquest != null && !MyGuild.Conquest.WarIsOn && MyGuild.Conquest.PalaceMap != null && !TeleportRandom(200, 0, MyGuild.Conquest.PalaceMap))
                            {
                                SendPacketToClient(p);
                                return;
                            }
                            break;
                        case 12://LotteryTicket                                                                                    
                            if (Envir.Random.Next(item.Info.Effect * 32) == 1) // 1st prize : 1,000,000
                            {
                                ReceiveChat("You won 1st Prize! Received 1,000,000 gold", ChatType.Hint);
                                GainGold(1000000);
                            }
                            else if (Envir.Random.Next(item.Info.Effect * 16) == 1)  // 2nd prize : 200,000
                            {
                                ReceiveChat("You won 2nd Prize! Received 200,000 gold", ChatType.Hint);
                                GainGold(200000);
                            }
                            else if (Envir.Random.Next(item.Info.Effect * 8) == 1)  // 3rd prize : 100,000
                            {
                                ReceiveChat("You won 3rd Prize! Received 100,000 gold", ChatType.Hint);
                                GainGold(100000);
                            }
                            else if (Envir.Random.Next(item.Info.Effect * 4) == 1) // 4th prize : 10,000
                            {
                                ReceiveChat("You won 4th Prize! Received 10,000 gold", ChatType.Hint);
                                GainGold(10000);
                            }
                            else if (Envir.Random.Next(item.Info.Effect * 2) == 1)  // 5th prize : 1,000
                            {
                                ReceiveChat("You won 5th Prize! Received 1,000 gold", ChatType.Hint);
                                GainGold(1000);
                            }
                            else if (Envir.Random.Next(item.Info.Effect) == 1)  // 6th prize 500
                            {
                                ReceiveChat("You won 6th Prize! Received 500 gold", ChatType.Hint);
                                GainGold(500);
                            }
                            else
                            {
                                ReceiveChat("You haven't won anything.", ChatType.Hint);
                            }
                            break;
                         
                    }
                    break;
                case ItemType.Book:
                    UserMagic magic = new UserMagic((Spell)item.Info.Shape);

                    if (magic.Info == null)
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    Info.Magics.Add(magic);
                    SendMagicInfo(magic);
                    RefreshStats();
                    break;
                case ItemType.Script:
                    CallDefaultNPC(DefaultNPCType.UseItem, item.Info.Shape);
                    break;
               
                
                case ItemType.Transform: //Transforms
                    {
                        AddBuff(BuffType.Transform, this, (Settings.Second * item.Info.Durability), new Stats(), values: item.Info.Shape);
                    }
                    break;
                case ItemType.Deco:

                    DecoObjectSrv decoOb = new DecoObjectSrv
                    {
                        Image = item.Info.Shape,
                        CurrentMap = CurrentMap,
                        CurrentLocation = CurrentLocation,
                    };

                    CurrentMap.AddObject(decoOb);
                    decoOb.Spawned();

                    SendPacketToClient(decoOb.GetInfo());

                    break;
                case ItemType.MonsterSpawn:

                    var monsterID = item.Info.Stats[Stat.HP];
                    var spawnAsPet = item.Info.Shape == 1;
                    var conquestOnly = item.Info.Shape == 2;

                    var monsterInfo = Envir.GetMonsterInfo(monsterID);
                    if (monsterInfo == null) break;

                    MonsterObjectSrv monster = MonsterObjectSrv.GetMonster(monsterInfo);
                    if (monster == null) break;

                     

                    if (conquestOnly)
                    {
                        var con = CurrentMap.GetConquest(CurrentLocation);
                        if (con == null)
                        {
                            ReceiveChat(string.Format("{0} can only be spawned during a conquest.", monsterInfo.GameName), ChatType.Hint);
                            SendPacketToClient(p);
                            return;
                        }
                    }

                    monster.Direction = Direction;
                    monster.ActionTime = Envir.Time + 5000;

                    if (!monster.Spawn(CurrentMap, Front))
                        monster.Spawn(CurrentMap, CurrentLocation);
                    break;
                case ItemType.SiegeAmmo:
                    //TODO;
                    break; 
                default:
                    return;
            }

            if (item.Count > 1) item.Count--;
            else Info.Inventory[index] = null;
            RefreshBagWeight();

            Report.ItemChanged(item, 1, 1);

            p.Success = true;
            SendPacketToClient(p);
        }
         
        public void SplitItem(MirGridType grid, ulong id, ushort count)
        {
            ServerPacket.SplitItem1 p = new ServerPacket.SplitItem1 { Grid = grid, UniqueID = id, Count = count, Success = false };
            UserItem[] array;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    array = Account.Storage;
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }

            UserItem temp = null;


            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null || array[i].UniqueID != id) continue;
                temp = array[i];
                break;
            }

            if (temp == null || count >= temp.Count || FreeSpace(array) == 0 || count < 1)
            {
                SendPacketToClient(p);
                return;
            }

            temp.Count -= count;

            var originalItem = temp;

            temp = Envir.CreateFreshItem(temp.Info);
            temp.Count = count;

            Report.ItemSplit(originalItem, temp, grid);

            p.Success = true;
            SendPacketToClient(p);
            SendPacketToClient(new ServerPacket.SplitItem { Item = temp, Grid = grid });

            if (grid == MirGridType.Inventory && (temp.Info.Type == ItemType.Potion || temp.Info.Type == ItemType.Scroll || temp.Info.Type == ItemType.Amulet || (temp.Info.Type == ItemType.Script && temp.Info.Effect == 1)))
            {
                if (temp.Info.Type == ItemType.Potion || temp.Info.Type == ItemType.Scroll || (temp.Info.Type == ItemType.Script && temp.Info.Effect == 1))
                {
                    for (int i = PotionBeltMinimum; i < PotionBeltMaximum; i++)
                    {
                        if (array[i] != null) continue;
                        array[i] = temp;
                        RefreshBagWeight();
                        return;
                    }
                }
                else if (temp.Info.Type == ItemType.Amulet)
                {
                    for (int i = AmuletBeltMinimum; i < AmuletBeltMaximum; i++)
                    {
                        if (array[i] != null) continue;
                        array[i] = temp;
                        RefreshBagWeight();
                        return;
                    }
                }
            }

            for (int i = BeltSize; i < array.Length; i++)
            {
                if (array[i] != null) continue;
                array[i] = temp;
                RefreshBagWeight();
                return;
            }

            for (int i = 0; i < BeltSize; i++)
            {
                if (array[i] != null) continue;
                array[i] = temp;
                RefreshBagWeight();
                return;
            }
        }
        public void MergeItem(MirGridType gridFrom, MirGridType gridTo, ulong fromID, ulong toID)
        {
            ServerPacket.MergeItem p = new ServerPacket.MergeItem { GridFrom = gridFrom, GridTo = gridTo, IDFrom = fromID, IDTo = toID, Success = false };

            UserItem[] arrayFrom;

            switch (gridFrom)
            {
                case MirGridType.Inventory:
                    arrayFrom = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    arrayFrom = Account.Storage;
                    break;
                case MirGridType.Equipment:
                    arrayFrom = Info.Equipment;
                    break;
                case MirGridType.Fishing:
                    if (Info.Equipment[(int)EquipmentSlot.Weapon] == null || !Info.Equipment[(int)EquipmentSlot.Weapon].Info.IsFishingRod)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    arrayFrom = Info.Equipment[(int)EquipmentSlot.Weapon].Slots;
                    break;
                    
                default:
                    SendPacketToClient(p);
                    return;
            }

            UserItem[] arrayTo;
            switch (gridTo)
            {
                case MirGridType.Inventory:
                    arrayTo = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.StorageKey, StringComparison.CurrentCultureIgnoreCase))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    NPCObjectSrv ob = null;
                    for (int i = 0; i < CurrentMap.NPCs.Count; i++)
                    {
                        if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                        ob = CurrentMap.NPCs[i];
                        break;
                    }

                    if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    arrayTo = Account.Storage;
                    break;
                case MirGridType.Equipment:
                    arrayTo = Info.Equipment;
                    break;
                case MirGridType.Fishing:
                    if (Info.Equipment[(int)EquipmentSlot.Weapon] == null || !Info.Equipment[(int)EquipmentSlot.Weapon].Info.IsFishingRod)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    arrayTo = Info.Equipment[(int)EquipmentSlot.Weapon].Slots;
                    break;
                     
                default:
                    SendPacketToClient(p);
                    return;
            }

            UserItem tempFrom = null;
            int index = -1;

            for (int i = 0; i < arrayFrom.Length; i++)
            {
                if (arrayFrom[i] == null || arrayFrom[i].UniqueID != fromID) continue;
                index = i;
                tempFrom = arrayFrom[i];
                break;
            }

            if (tempFrom == null || tempFrom.Info.StackSize == 1 || index == -1)
            {
                SendPacketToClient(p);
                return;
            }


            UserItem tempTo = null;
            int toIndex = -1;

            for (int i = 0; i < arrayTo.Length; i++)
            {
                if (arrayTo[i] == null || arrayTo[i].UniqueID != toID) continue;
                toIndex = i;
                tempTo = arrayTo[i];
                break;
            }

            if (tempTo == null || tempTo.Info != tempFrom.Info || tempTo.Count == tempTo.Info.StackSize)
            {
                SendPacketToClient(p);
                return;
            }

            if (tempTo.Info.Type != ItemType.Amulet && (gridFrom == MirGridType.Equipment || gridTo == MirGridType.Equipment))
            {
                SendPacketToClient(p);
                return;
            }

            if(tempTo.Info.Type != ItemType.Bait && (gridFrom == MirGridType.Fishing || gridTo == MirGridType.Fishing))
            {
                SendPacketToClient(p);
                return;
            }

            if (tempFrom.Count <= tempTo.Info.StackSize - tempTo.Count)
            {
                tempTo.Count += tempFrom.Count;
                arrayFrom[index] = null;
            }
            else
            {
                tempFrom.Count -= (ushort)(tempTo.Info.StackSize - tempTo.Count);
                tempTo.Count = tempTo.Info.StackSize;
            }

            Report.ItemMerged(tempFrom, tempTo, index, toIndex, gridFrom, gridTo);

            TradeUnlock();

            p.Success = true;
            SendPacketToClient(p);
            RefreshStats();
        }
        public void CombineItem(MirGridType grid, ulong fromID, ulong toID)
        {
            ServerPacket.CombineItem p = new ServerPacket.CombineItem { Grid = grid, IDFrom = fromID, IDTo = toID, Success = false };

            UserItem[] array = null;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                    
            }

            if (array == null)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem tempFrom = null;
            UserItem tempTo = null;
            int indexFrom = -1;
            int indexTo = -1;

            if (Dead)
            {
                SendPacketToClient(p);
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null || array[i].UniqueID != fromID) continue;
                indexFrom = i;
                tempFrom = array[i];
                break;
            }

            if (tempFrom == null || indexFrom == -1)
            {
                SendPacketToClient(p);
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null || array[i].UniqueID != toID) continue;
                indexTo = i;
                tempTo = array[i];
                break;
            }

            if (tempTo == null || indexTo == -1)
            {
                SendPacketToClient(p);
                return;
            }

            if ((byte)tempTo.Info.Type < 1 || (byte)tempTo.Info.Type > 11)
            {
                SendPacketToClient(p);
                return;
            }

            bool canRepair = false, canUpgrade = false, canSlotUpgrade = false, canSeal = false;

            if (tempFrom.Info.Type != ItemType.Gem)
            {
                SendPacketToClient(p);
                return;
            }

            switch (tempFrom.Info.Shape)
            {
                case 1: //BoneHammer
                case 2: //SewingSupplies
                case 5: //SpecialHammer
                case 6: //SpecialSewingSupplies

                    if (tempTo.Info.Bind.HasFlag(BindMode.DontRepair))
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    switch (tempTo.Info.Type)
                    {
                        case ItemType.Weapon:
                        case ItemType.Necklace:
                        case ItemType.Ring:
                        case ItemType.Bracelet:
                            if (tempFrom.Info.Shape == 1 || tempFrom.Info.Shape == 5)
                                canRepair = true;
                            break;
                        case ItemType.Armour:
                        case ItemType.Helmet:
                        case ItemType.Boots:
                        case ItemType.Belt:
                            if (tempFrom.Info.Shape == 2 || tempFrom.Info.Shape == 6)
                                canRepair = true;
                            break;
                        default:
                            canRepair = false;
                            break;
                    }

                    if (canRepair != true)
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    if (tempTo.CurrentDura == tempTo.MaxDura)
                    {
                        ReceiveChat("Item does not need to be repaired.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }
                    break;
                case 7: //slots
                    if (tempTo.Info.Bind.HasFlag(BindMode.DontUpgrade) || tempTo.Info.Unique != SpecialItemMode.None)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (tempTo.RentalInformation != null && tempTo.RentalInformation.BindingFlags.HasFlag(BindMode.DontUpgrade))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (!ValidGemForItem(tempFrom, (byte)tempTo.Info.Type))
                    {
                        ReceiveChat("Invalid combination.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }
                    if (tempTo.Info.RandomStats == null)
                    {
                        ReceiveChat("Item already has max sockets.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }
                    if (tempTo.Info.RandomStats.SlotMaxStat <= tempTo.Slots.Length)
                    {
                        ReceiveChat("Item already has max sockets.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }

                    canSlotUpgrade = true;
                    break;
                case 8: //Seal
                    if (tempTo.Info.Bind.HasFlag(BindMode.DontUpgrade) || tempTo.Info.Unique != SpecialItemMode.None)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (tempTo.SealedInfo != null && tempTo.SealedInfo.ExpiryDate > Envir.Now)
                    {
                        ReceiveChat("Item is already sealed.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }
                    if (tempTo.SealedInfo != null && tempTo.SealedInfo.NextSealDate > Envir.Now)
                    {
                        double remainingSeconds = (tempTo.SealedInfo.NextSealDate - Envir.Now).TotalSeconds;

                        ReceiveChat($"Item cannot be resealed for another {Functions.PrintTimeSpanFromSeconds(remainingSeconds, false)}.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }

                    canSeal = true;
                    break;
                case 3: //gems
                case 4: //orbs
                    if (tempTo.Info.Bind.HasFlag(BindMode.DontUpgrade) || tempTo.Info.Unique != SpecialItemMode.None)
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    if (tempTo.RentalInformation != null && tempTo.RentalInformation.BindingFlags.HasFlag(BindMode.DontUpgrade))
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    if ((tempTo.GemCount >= tempFrom.Info.Stats[Stat.CriticalDamage]) || (GetCurrentStatCount(tempFrom, tempTo) >= tempFrom.Info.Stats[Stat.HPDrainRatePercent]))
                    {
                        ReceiveChat("Item has already reached maximum added stats.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }

                    int successchance = tempFrom.Info.Stats[Stat.Reflect];

                    // Gem is only affected by the stat applied.
                    // Drop rate per gem won't work if gems add more than 1 stat, i.e. DC + 2 per gem.
                    if (Settings.GemStatIndependent)
                    {
                        Stat GemType = GetGemType(tempFrom);

                        switch (GemType)
                        {
                            case Stat.MaxAC:
                                successchance *= (int)tempTo.AddedStats[Stat.MaxAC];
                                break;

                            case Stat.MaxMAC:
                                successchance *= (int)tempTo.AddedStats[Stat.MaxMAC];
                                break;

                            case Stat.MaxDC:
                                successchance *= (int)tempTo.AddedStats[Stat.MaxDC];
                                break;

                            case Stat.MaxMC:
                                successchance *= (int)tempTo.AddedStats[Stat.MaxMC];
                                break;

                            case Stat.MaxSC:
                                successchance *= (int)tempTo.AddedStats[Stat.MaxSC];
                                break;

                            case Stat.AttackSpeed:
                                successchance *= (int)tempTo.AddedStats[Stat.AttackSpeed];
                                break;

                            case Stat.Accuracy:
                                successchance *= (int)tempTo.AddedStats[Stat.Accuracy];
                                break;

                            case Stat.Agility:
                                successchance *= (int)tempTo.AddedStats[Stat.Agility];
                                break;

                            case Stat.Freezing:
                                successchance *= (int)tempTo.AddedStats[Stat.Freezing];
                                break;

                            case Stat.PoisonAttack:
                                successchance *= (int)tempTo.AddedStats[Stat.PoisonAttack];
                                break;

                            case Stat.MagicResist:
                                successchance *= (int)tempTo.AddedStats[Stat.MagicResist];
                                break;

                            case Stat.PoisonResist:
                                successchance *= (int)tempTo.AddedStats[Stat.PoisonResist];
                                break;

                            // These attributes may not work as more than 1 stat is
                            // added per gem, i.e + 40 HP.

                            case Stat.HP:
                                successchance *= (int)tempTo.AddedStats[Stat.HP];
                                break;

                            case Stat.MP:
                                successchance *= (int)tempTo.AddedStats[Stat.MP];
                                break;

                            case Stat.HealthRecovery:
                                successchance *= (int)tempTo.AddedStats[Stat.HealthRecovery];
                                break;
                                
                            // I don't know if this conflicts with benes.
                            case Stat.Luck:
                                successchance *= (int)tempTo.AddedStats[Stat.Luck];
                                break;

                            case Stat.Strong:
                                successchance *= (int)tempTo.AddedStats[Stat.Strong];
                                break;

                            case Stat.PoisonRecovery:
                                successchance *= (int)tempTo.AddedStats[Stat.PoisonRecovery];
                                break;


                            /*
                                 Currently not supported.
                                 Missing item definitions.

                                 case StatType.HP_Precent:
                                 case StatType.MP_Precent:
                                 case StatType.MP_Regen:
                                 case StatType.Holy:
                                 case StatType.Durability:


                            */
                            default:
                                successchance *= (int)tempTo.GemCount;
                                break;

                        }
                    }
                    // Gem is affected by the total added stats on the item.
                    else
                    {
                        successchance *= (int)tempTo.GemCount;
                    }

                    successchance = successchance >= tempFrom.Info.Stats[Stat.CriticalRate] ? 0 : (tempFrom.Info.Stats[Stat.CriticalRate] - successchance) + Stats[Stat.GemRatePercent];

                    //check if combine will succeed
                    bool succeeded = Envir.Random.Next(100) < successchance;
                    canUpgrade = true;

                    byte itemType = (byte)tempTo.Info.Type;

                    if (!ValidGemForItem(tempFrom, itemType))
                    {
                        ReceiveChat("Invalid combination", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }

                    if (tempFrom.GetTotal(Stat.MaxDC) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MaxDC] += tempFrom.GetTotal(Stat.MaxDC);
                    }

                    else if (tempFrom.GetTotal(Stat.MaxMC) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MaxMC] += tempFrom.GetTotal(Stat.MaxMC);
                    }

                    else if (tempFrom.GetTotal(Stat.MaxSC) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MaxSC] += tempFrom.GetTotal(Stat.MaxSC);
                    }

                    else if (tempFrom.GetTotal(Stat.MaxAC) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MaxAC] += tempFrom.GetTotal(Stat.MaxAC);
                    }

                    else if (tempFrom.GetTotal(Stat.MaxMAC) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MaxMAC] += tempFrom.GetTotal(Stat.MaxMAC);
                    }

                    else if ((tempFrom.Info.Durability) > 0)
                    {
                        if (succeeded) tempTo.MaxDura = (ushort)Math.Min(ushort.MaxValue, tempTo.MaxDura + tempFrom.MaxDura);
                    }

                    else if (tempFrom.GetTotal(Stat.AttackSpeed) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.AttackSpeed] += tempFrom.GetTotal(Stat.AttackSpeed);
                    }

                    else if (tempFrom.GetTotal(Stat.Agility) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.Agility] += tempFrom.GetTotal(Stat.Agility);
                    }

                    else if (tempFrom.GetTotal(Stat.Accuracy) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.Accuracy] += tempFrom.GetTotal(Stat.Accuracy);
                    }

                    else if (tempFrom.GetTotal(Stat.PoisonAttack) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.PoisonAttack] += tempFrom.GetTotal(Stat.PoisonAttack);
                    }

                    else if (tempFrom.GetTotal(Stat.Freezing) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.Freezing] += tempFrom.GetTotal(Stat.Freezing);
                    }

                    else if (tempFrom.GetTotal(Stat.MagicResist) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.MagicResist] += tempFrom.GetTotal(Stat.MagicResist);
                    }

                    else if (tempFrom.GetTotal(Stat.PoisonResist) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.PoisonResist] += tempFrom.GetTotal(Stat.PoisonResist);
                    }
                    else if (tempFrom.GetTotal(Stat.Luck) > 0)
                    {
                        if (succeeded) tempTo.AddedStats[Stat.Luck] += tempFrom.GetTotal(Stat.Luck);
                    }
                    else
                    {
                        ReceiveChat("Cannot combine these items.", ChatType.Hint);
                        SendPacketToClient(p);
                        return;
                    }

                    if (!succeeded)
                    {
                        if ((tempFrom.Info.Shape == 3) && (Envir.Random.Next(15) < 3))
                        {
                            //item destroyed
                            ReceiveChat("Item has been destroyed.", ChatType.Hint);
                            Report.ItemChanged(array[indexTo], 1, 1, "CombineItem (Item Destroyed)");

                            array[indexTo] = null;
                            p.Destroy = true;
                        }
                        else
                        {
                            //upgrade has no effect
                            ReceiveChat("Upgrade has no effect.", ChatType.Hint);
                        }

                        canUpgrade = false;
                    }
                    break;
                default:
                    SendPacketToClient(p);
                    return;
            }


            switch (grid)
            {
                case MirGridType.Inventory:
                    RefreshBagWeight();
                    break;
                     
            }

            if (canRepair && array[indexTo] != null)
            {
                switch (tempTo.Info.Shape)
                {
                    case 1:
                    case 2:
                        {
                            tempTo.MaxDura = (ushort)Math.Max(0, Math.Min(tempTo.MaxDura, tempTo.MaxDura - 100 * Envir.Random.Next(10)));
                        }
                        break;
                    default:
                        break;
                }
                tempTo.CurrentDura = tempTo.MaxDura;
                tempTo.DuraChanged = false;

                ReceiveChat("Item has been repaired.", ChatType.Hint);
                SendPacketToClient(new ServerPacket.ItemRepaired { UniqueID = tempTo.UniqueID, MaxDura = tempTo.MaxDura, CurrentDura = tempTo.CurrentDura });
            }

            if (canUpgrade && array[indexTo] != null)
            {
                tempTo.GemCount++;
                ReceiveChat("Item has been upgraded.", ChatType.Hint);
                SendPacketToClient(new ServerPacket.ItemUpgraded { Item = tempTo });
            }

            if (canSlotUpgrade && array[indexTo] != null)
            {
                tempTo.SetSlotSize(tempTo.Slots.Length + 1);
                ReceiveChat("Item has increased its sockets.", ChatType.Hint);
                SendPacketToClient(new ServerPacket.ItemSlotSizeChanged { UniqueID = tempTo.UniqueID, SlotSize = tempTo.Slots.Length });
            }

            if (canSeal && array[indexTo] != null)
            {
                var minutes = tempFrom.CurrentDura;
                tempTo.SealedInfo = new SealedInfo 
                { 
                    ExpiryDate = Envir.Now.AddMinutes(minutes), 
                    NextSealDate = Envir.Now.AddMinutes(minutes).AddMinutes(Settings.ItemSealDelay) 
                };

                ReceiveChat($"Item sealed for {Functions.PrintTimeSpanFromSeconds(minutes * 60)}.", ChatType.Hint);

                SendPacketToClient(new ServerPacket.ItemSealChanged { UniqueID = tempTo.UniqueID, ExpiryDate = tempTo.SealedInfo.ExpiryDate });
            }

            if (tempFrom.Count > 1) tempFrom.Count--;
            else array[indexFrom] = null;

            Report.ItemCombined(tempFrom, tempTo, indexFrom, indexTo, grid);

            //item merged ok
            TradeUnlock();

            p.Success = true;
            SendPacketToClient(p);
        }
        private bool ValidGemForItem(UserItem Gem, byte itemtype)
        {
            switch (itemtype)
            {
                case 1: //weapon
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Paralize))
                        return true;
                    break;
                case 2: //Armour
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Teleport))
                        return true;
                    break;
                case 4: //Helmet
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.ClearRing))
                        return true;
                    break;
                case 5: //necklace
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Protection))
                        return true;
                    break;
                case 6: //bracelet
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Revival))
                        return true;
                    break;
                case 7: //ring
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Muscle))
                        return true;
                    break;
                case 8: //amulet
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Flame))
                        return true;
                    break;
                case 9://belt
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Healing))
                        return true;
                    break;
                case 10: //boots
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Probe))
                        return true;
                    break;
                case 11: //stone
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.Skill))
                        return true;
                    break;
                case 12:///torch
                    if (Gem.Info.Unique.HasFlag(SpecialItemMode.NoDuraLoss))
                        return true;
                    break;
            }
            return false;
        }
        //Gems granting multiple stat types are not compatiable with this method.
        private Stat GetGemType(UserItem gem)
        {
            if (gem.GetTotal(Stat.MaxDC) > 0)
                return Stat.MaxDC;

            else if (gem.GetTotal(Stat.MaxMC) > 0)
                return Stat.MaxMC;

            else if (gem.GetTotal(Stat.MaxSC) > 0)
                return Stat.MaxSC;

            else if (gem.GetTotal(Stat.MaxAC) > 0)
                return Stat.MaxAC;

            else if (gem.GetTotal(Stat.MaxMAC) > 0)
                return Stat.MaxMAC;

            else if (gem.GetTotal(Stat.AttackSpeed) > 0)
                return Stat.AttackSpeed;

            else if (gem.GetTotal(Stat.Agility) > 0)
                return Stat.Agility;

            else if (gem.GetTotal(Stat.Accuracy) > 0)
                return Stat.Accuracy;

            else if (gem.GetTotal(Stat.PoisonAttack) > 0)
                return Stat.PoisonAttack;

            else if (gem.GetTotal(Stat.Freezing) > 0)
                return Stat.Freezing;

            else if (gem.GetTotal(Stat.MagicResist) > 0)
                return Stat.MagicResist;

            else if (gem.GetTotal(Stat.PoisonResist) > 0)
                return Stat.PoisonResist;

            else if (gem.GetTotal(Stat.Luck) > 0)
                return Stat.Luck;

            else if (gem.GetTotal(Stat.PoisonRecovery) > 0)
                return Stat.PoisonRecovery;

            else if (gem.GetTotal(Stat.HP) > 0)
                return Stat.HP;

            else if (gem.GetTotal(Stat.MP) > 0)
                return Stat.MP;

            else if (gem.GetTotal(Stat.HealthRecovery) > 0)
                return Stat.HealthRecovery;

            // These may be incomplete. Item definitions may be missing?

            else if (gem.GetTotal(Stat.HPRatePercent) > 0)
                return Stat.HPRatePercent;

            else if (gem.GetTotal(Stat.MPRatePercent) > 0)
                return Stat.MPRatePercent;

            else if (gem.GetTotal(Stat.SpellRecovery) > 0)
                return Stat.SpellRecovery;

            else if (gem.GetTotal(Stat.Holy) > 0)
                return Stat.Holy;

            else if (gem.GetTotal(Stat.Strong) > 0)
                return Stat.Strong;

            else if (gem.GetTotal(Stat.HPDrainRatePercent) > 0)
                return Stat.HPDrainRatePercent;

            return Stat.Unknown;
        }
        //Gems granting multiple stat types are not compatible with this method.        
        public void DropItem(ulong id, ushort count, bool isHeroItem)
        {
            ServerPacket.DropItem p = new ServerPacket.DropItem { UniqueID = id, Count = count, HeroItem = isHeroItem, Success = false };
            if (Dead)
            {
                SendPacketToClient(p);
                return;
            }

            if (CurrentMap.Info.NoThrowItem)
            {
                ReceiveChat(GameLanguage.CanNotDrop, ChatType.System);
                SendPacketToClient(p);
                return;
            }

            UserItem temp = null;
            int index = -1;
             

            if (!isHeroItem)
            {
                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    temp = Info.Inventory[i];
                    if (temp == null || temp.UniqueID != id) continue;
                    index = i;
                    break;
                }
            }
             

            if (temp == null || index == -1 || count > temp.Count || count < 1)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Info.Bind.HasFlag(BindMode.DontDrop))
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.RentalInformation != null && temp.RentalInformation.BindingFlags.HasFlag(BindMode.DontDrop))
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Count == count)
            {
                if (!temp.Info.Bind.HasFlag(BindMode.DestroyOnDrop))
                    if (!DropItem(temp))
                    {
                        SendPacketToClient(p);
                        return;
                    }

                
                    Info.Inventory[index] = null; 
                
            }
            else
            {
                UserItem temp2 = Envir.CreateFreshItem(temp.Info);
                temp2.Count = count;
                if (!temp.Info.Bind.HasFlag(BindMode.DestroyOnDrop))
                    if (!DropItem(temp2))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                temp.Count -= count;
            }
            p.Success = true;
            SendPacketToClient(p);
             
            RefreshBagWeight();
            Report.ItemChanged(temp, count, 1);
        }
        public void DropGold(uint gold)
        {
            if (Account.Gold < gold) return;

            ItemObjectSrv ob = new ItemObjectSrv(this, gold);

            if (!ob.Drop(5)) return;
            Account.Gold -= gold;
            SendPacketToClient(new ServerPacket.LoseGold { Gold = gold });
        }
        public void PickUp()
        {
            if (Dead)
            {
                //Send Fail
                return;
            }

            Cell cell = CurrentMap.GetCell(CurrentLocation);

            bool sendFail = false;

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObjectSrv ob = cell.Objects[i];

                if (ob.Race != ObjectType.Item) continue;

                if (ob.Owner != null && ob.Owner != this && !IsGroupMember(ob.Owner)) //Or Group member.
                {
                    sendFail = true;
                    continue;
                }
                ItemObjectSrv item = (ItemObjectSrv)ob;

                if (item.Item != null)
                {
                    if (!CanGainItem(item.Item)) continue;

                    if (item.Item.Info.ShowGroupPickup && IsGroupMember(this))
                        for (int j = 0; j < GroupMembers.Count; j++)
                            GroupMembers[j].ReceiveChat(Name + " Picked up: {" + item.Item.FriendlyName + "}",
                                ChatType.System);

                    GainItem(item.Item);

                    Report.ItemChanged(item.Item, item.Item.Count, 2);

                    CurrentMap.RemoveObject(ob);
                    ob.Despawn();

                    return;
                }

                if (!CanGainGold(item.Gold)) continue;

                GainGold(item.Gold);
                CurrentMap.RemoveObject(ob);
                ob.Despawn();
                return;
            }

            if (sendFail)
                ReceiveChat("Can not pick up, You do not own this item.", ChatType.System);

        }
        public void RequestMapInfo(int mapIndex)
        {
            var info = Envir.GetMapInfo(mapIndex);
            CheckMapInfo(info);
        }
        public void TeleportToNPC(uint objectID)
        {
            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[i];
                if (ob.ObjectID != objectID) continue;

                if (!ob.Info.CanTeleportTo) return;

                uint cost = (uint)Settings.TeleportToNPCCost;
                if (Account.Gold < cost) return;

                Point p = ob.Front;
                if (!CurrentMap.ValidPoint(p))
                {
                    for (int j = 0; j < 7; j++)
                    {
                        p = Functions.PointMove(CurrentLocation, Functions.ShiftDirection(ob.Direction, j), 1);
                        if (CurrentMap.ValidPoint(p)) break;
                    }
                }

                if (CurrentMap.ValidPoint(p))
                {
                    Account.Gold -= cost;
                    SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });
                    Teleport(CurrentMap, p);
                }

                break;
            }
        }
        public void SearchMap(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 3) return;

            ServerPacket.SearchMapResult p = new ServerPacket.SearchMapResult();

            Map map = Envir.GetWorldMap(text);
            if (map != null)
            {
                CheckMapInfo(map.Info);
                p.MapIndex = map.Info.Index;
                SendPacketToClient(p);
                return;
            }

            NPCObjectSrv npc = Envir.GetWorldMapNPC(text);
            if (npc != null)
            {
                CheckMapInfo(npc.CurrentMap.Info);
                p.MapIndex = npc.CurrentMap.Info.Index;
                p.NPCIndex = npc.ObjectID;
                SendPacketToClient(p);
                return;
            }

            SendPacketToClient(p);
            return;
        }
        private bool IsGroupMember(MapObjectSrv player)
        {
            if (player.Race != ObjectType.Player) return false;
            return GroupMembers != null && GroupMembers.Contains(player);
        }
        public override bool CanGainGold(uint gold)
        {
            return (ulong)gold + Account.Gold <= uint.MaxValue;
        }
        public override void WinGold(uint gold)
        {
            if (GroupMembers == null)
            {
                GainGold(gold);
                return;
            }

            uint count = 0;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                PlayerObjectSrv player = GroupMembers[i];
                if (player.CurrentMap == CurrentMap && Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) && !player.Dead)
                    count++;
            }

            if (count == 0 || count > gold)
            {
                GainGold(gold);
                return;
            }
            gold = gold / count;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                PlayerObjectSrv player = GroupMembers[i];
                if (player.CurrentMap == CurrentMap && Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) && !player.Dead)
                    player.GainGold(gold);
            }
        }
        public void GainGold(uint gold)
        {
            if (gold == 0) return;

            if (((UInt64)Account.Gold + gold) > uint.MaxValue)
                gold = uint.MaxValue - Account.Gold;

            Account.Gold += gold;

            SendPacketToClient(new ServerPacket.GainedGold { Gold = gold });
        }

        public void UpdatePhoto(int len, byte[] datas)
        {
            Console.WriteLine("@@@111 UpdatePhoto");
            //Apply to info.Data
            Info.ExPortraitLen = len;
            Info.ExPortraitBytes = datas;  
        }

        public void GainCredit(uint credit)
        {
            if (credit == 0) return;

            if (((UInt64)Account.Credit + credit) > uint.MaxValue)
                credit = uint.MaxValue - Account.Credit;

            Account.Credit += credit;

            SendPacketToClient(new ServerPacket.GainedCredit { Credit = credit });
        }
                        
        public bool CanRemoveItem(MirGridType grid, UserItem item)
        {
            //Item  Stuck

            UserItem[] array;
            switch (grid)
            {
                case MirGridType.Inventory:
                    array = Info.Inventory;
                    break;
                case MirGridType.Storage:
                    array = Account.Storage;
                    break;
                
                default:
                    return false;
            }


            return FreeSpace(array) > 0;
        }
        public bool CheckQuestItem(UserItem uItem, ushort count)
        {
            foreach (var item in Info.QuestInventory.Where(item => item != null && item.Info == uItem.Info))
            {
                if (count > item.Count)
                {
                    count -= item.Count;
                    continue;
                }

                if (count > item.Count) continue;
                count = 0;
                break;
            }

            return count <= 0;
        }
        public bool CanGainQuestItem(UserItem item)
        {
            if (FreeSpace(Info.QuestInventory) > 0) return true;

            if (item.Info.StackSize > 1)
            {
                ushort count = item.Count;

                for (int i = 0; i < Info.QuestInventory.Length; i++)
                {
                    UserItem bagItem = Info.QuestInventory[i];

                    if (bagItem.Info != item.Info) continue;

                    if (bagItem.Count + count <= bagItem.Info.StackSize) return true;

                    count -= (ushort)(bagItem.Info.StackSize - bagItem.Count);
                }
            }

            ReceiveChat("You cannot carry anymore quest items.", ChatType.System);

            return false;
        }
        public void GainQuestItem(UserItem item)
        {
            CheckItem(item);

            UserItem clonedItem = item.Clone();

            SendPacketToClient(new ServerPacket.GainedQuestItem { Item = clonedItem });

            AddQuestItem(item);
        }
        public void TakeQuestItem(ItemInfo uItem, ushort count)
        {
            for (int o = 0; o < Info.QuestInventory.Length; o++)
            {
                UserItem item = Info.QuestInventory[o];
                if (item == null) continue;
                if (item.Info != uItem) continue;

                if (count > item.Count)
                {
                    SendPacketToClient(new ServerPacket.DeleteQuestItem { UniqueID = item.UniqueID, Count = item.Count });
                    Info.QuestInventory[o] = null;

                    count -= item.Count;
                    continue;
                }

                SendPacketToClient(new ServerPacket.DeleteQuestItem { UniqueID = item.UniqueID, Count = count });

                if (count == item.Count)
                    Info.QuestInventory[o] = null;
                else
                    item.Count -= count;
                break;
            }
        }       
        
        public void RequestChatItem(ulong id)
        {
            //Enqueue(new ServerPacket.ChatItemStats { ChatItemId = id, Stats = whatever });
        }
        
        public override void ReceiveChat(string text, ChatType type)
        {
            SendPacketToClient(new ServerPacket.Chat { Message = text, Type = type });
        }
        public void ReceiveOutputMessage(string text, OutputMessageType type)
        {
            SendPacketToClient(new ServerPacket.SendOutputMessage { Message = text, Type = type });
        }                
        public void Opendoor(byte Doorindex)
        {
            //todo: add check for sw doors
            if (CurrentMap.OpenDoor(Doorindex))
            {
                SendPacketToClient(new ServerPacket.Opendoor() { DoorIndex = Doorindex });
                Broadcast(new ServerPacket.Opendoor() { DoorIndex = Doorindex });
            }
        }

        #region NPC

        public void CallDefaultNPC(DefaultNPCType type, params object[] value)
        {
            string key = string.Empty;

            switch (type)
            {
                case DefaultNPCType.Login:
                    key = "Login";
                    break;
                case DefaultNPCType.UseItem:
                    if (value.Length < 1) return;
                    key = string.Format("UseItem({0})", value[0]);
                    break;
                case DefaultNPCType.Trigger:
                    if (value.Length < 1) return;
                    key = string.Format("Trigger({0})", value[0]);
                    break;
                case DefaultNPCType.MapCoord:
                    if (value.Length < 3) return;
                    key = string.Format("MapCoord({0},{1},{2})", value[0], value[1], value[2]);
                    break;
                case DefaultNPCType.MapEnter:
                    if (value.Length < 1) return;
                    key = string.Format("MapEnter({0})", value[0]);
                    break;
                case DefaultNPCType.Die:
                    key = "Die";
                    break;
                case DefaultNPCType.LevelUp:
                    key = "LevelUp";
                    break;
                case DefaultNPCType.CustomCommand:
                    if (value.Length < 1) return;
                    key = string.Format("CustomCommand({0})", value[0]);
                    break;
                case DefaultNPCType.OnAcceptQuest:
                    if (value.Length < 1) return;
                    key = string.Format("OnAcceptQuest({0})", value[0]);
                    break;
                case DefaultNPCType.OnFinishQuest:
                    if (value.Length < 1) return;
                    key = string.Format("OnFinishQuest({0})", value[0]);
                    break;
                case DefaultNPCType.Daily:
                    key = "Daily";
                    Info.NewDay = false;
                    break;
                case DefaultNPCType.Client:
                    key = "Client";
                    break;
            }

            key = string.Format("[@_{0}]", key);

            DelayedAction action = new DelayedAction(DelayedType.NPC, Envir.Time, Envir.DefaultNPC.LoadedObjectID, Envir.DefaultNPC.ScriptID, key);
            ActionList.Add(action);

            SendPacketToClient(new ServerPacket.NPCUpdate { NPCID = Envir.DefaultNPC.LoadedObjectID });
        }

        public void CallDefaultNPC(string key)
        {
            if (NPCObjectID != Envir.DefaultNPC.LoadedObjectID) return;

            var script = NPCScript.Get(NPCScriptID);
            script.Call(this, NPCObjectID, key.ToUpper());

            CallNPCNextPage();
        }

        public void CallNPC(uint objectID, string key)
        {
            if (Dead) return;

            key = key.ToUpper();

            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[i];
                if (ob.ObjectID != objectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                ob.CheckVisible(this);

                if (!ob.VisibleLog[Info.Index] || !ob.Visible) return;

                var scriptID = NPCScriptID;
                if (objectID != NPCObjectID || key == NPCScript.MainKey)
                {
                    scriptID = ob.ScriptID;
                }

                var script = NPCScript.Get(scriptID);
                script.Call(this, objectID, key);

                break;
            }

            CallNPCNextPage();
        }
        private void CallNPCNextPage()
        {
            //process any new npc calls immediately
            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Type != DelayedType.NPC || ActionList[i].Time != -1) continue;
                var action = ActionList[i];

                ActionList.RemoveAt(i);

                CompleteNPC(action.Params);
            }
        }

        public void BuyItem(ulong index, ushort count, PanelType type)
        {
            if (Dead || count < 1) return;

            if (NPCPage == null ||
                !(String.Equals(NPCPage.Key, NPCScript.BuySellKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.BuyKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.BuyBackKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.BuyUsedKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.PearlBuyKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.BuyNewKey, StringComparison.CurrentCultureIgnoreCase) ||
                String.Equals(NPCPage.Key, NPCScript.BuySellNewKey, StringComparison.CurrentCultureIgnoreCase))) return;

            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[i];
                if (ob.ObjectID != NPCObjectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                if (type == PanelType.Buy)
                {
                    NPCScript script = NPCScript.Get(NPCScriptID);
                    script.Buy(this, index, count);
                }
            }
        }
        public void CraftItem(ulong index, ushort count, int[] slots)
        {
            if (Dead || count < 1) return;

            if (NPCPage == null) return;

            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[i];
                if (ob.ObjectID != NPCObjectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                NPCScript script = NPCScript.Get(NPCScriptID);
                script.Craft(this, index, count, slots);
            }
        }


        public void SellItem(ulong uniqueID, ushort count)
        {
            ServerPacket.SellItem p = new ServerPacket.SellItem { UniqueID = uniqueID, Count = count };

            if (Dead || count == 0)
            {
                SendPacketToClient(p);
                return;
            }

            if (NPCPage == null || !(String.Equals(NPCPage.Key, NPCScript.BuySellKey, StringComparison.CurrentCultureIgnoreCase) || String.Equals(NPCPage.Key, NPCScript.SellKey, StringComparison.CurrentCultureIgnoreCase)))
            {
                SendPacketToClient(p);
                return;
            }

            for (int n = 0; n < CurrentMap.NPCs.Count; n++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[n];
                if (ob.ObjectID != NPCObjectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                UserItem temp = null;
                int index = -1;

                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    temp = Info.Inventory[i];
                    if (temp == null || temp.UniqueID != uniqueID) continue;
                    index = i;
                    break;
                }

                if (temp == null || index == -1 || count > temp.Count)
                {
                    SendPacketToClient(p);
                    return;
                }

                if (temp.Info.Bind.HasFlag(BindMode.DontSell))
                {
                    SendPacketToClient(p);
                    return;
                }

                if (temp.RentalInformation != null && temp.RentalInformation.BindingFlags.HasFlag(BindMode.DontSell))
                {
                    SendPacketToClient(p);
                    return;
                }

                NPCScript script = NPCScript.Get(NPCScriptID);

                if (script.Types.Count != 0 && !script.Types.Contains(temp.Info.Type))
                {
                    ReceiveChat("You cannot sell this item here.", ChatType.System);
                    SendPacketToClient(p);
                    return;
                }

                if (temp.Info.StackSize > 1 && count != temp.Count)
                {
                    UserItem item = Envir.CreateFreshItem(temp.Info);
                    item.Count = count;

                    if (item.Price() / 2 + Account.Gold > uint.MaxValue)
                    {
                        SendPacketToClient(p);
                        return;
                    }

                    temp.Count -= count;
                    temp = item;
                }
                else Info.Inventory[index] = null;

                script.Sell(this, temp);

                if (Settings.GoodsOn)
                {
                    var callingNPC = NPCObjectSrv.Get(NPCObjectID);

                    if (callingNPC != null)
                    {
                        if (!callingNPC.BuyBack.ContainsKey(Name)) callingNPC.BuyBack[Name] = new List<UserItem>();

                        if (Settings.GoodsBuyBackMaxStored > 0 && callingNPC.BuyBack[Name].Count >= Settings.GoodsBuyBackMaxStored)
                            callingNPC.BuyBack[Name].RemoveAt(0);

                        temp.BuybackExpiryDate = Envir.Now;
                        callingNPC.BuyBack[Name].Add(temp);
                    }
                }

                p.Success = true;
                SendPacketToClient(p);
                GainGold(temp.Price() / 2);
                RefreshBagWeight();

                return;
            }

            SendPacketToClient(p);
        }
        public void RepairItem(ulong uniqueID, bool special = false)
        {
            SendPacketToClient(new ServerPacket.RepairItem { UniqueID = uniqueID });

            if (Dead) return;

            if (NPCPage == null || (!String.Equals(NPCPage.Key, NPCScript.RepairKey, StringComparison.CurrentCultureIgnoreCase) && !special) || (!String.Equals(NPCPage.Key, NPCScript.SRepairKey, StringComparison.CurrentCultureIgnoreCase) && special)) return;

            for (int n = 0; n < CurrentMap.NPCs.Count; n++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[n];
                if (ob.ObjectID != NPCObjectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                UserItem temp = null;
                int index = -1;

                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    temp = Info.Inventory[i];
                    if (temp == null || temp.UniqueID != uniqueID) continue;
                    index = i;
                    break;
                }

                if (temp == null || index == -1) return;

                if ((temp.Info.Bind.HasFlag(BindMode.DontRepair)) || (temp.Info.Bind.HasFlag(BindMode.NoSRepair) && special))
                {
                    ReceiveChat("You cannot Repair this item.", ChatType.System);
                    return;
                }

                NPCScript script = NPCScript.Get(NPCScriptID);

                if (script.Types.Count != 0 && !script.Types.Contains(temp.Info.Type))
                {
                    ReceiveChat("You cannot Repair this item here.", ChatType.System);
                    return;
                }

                uint cost;
                uint baseCost;
                if (!special)
                {
                    cost = (uint)(temp.RepairPrice() * script.PriceRate(this));
                    baseCost = (uint)(temp.RepairPrice() * script.PriceRate(this, true));
                }
                else
                {
                    cost = (uint)(temp.RepairPrice() * 3 * script.PriceRate(this));
                    baseCost = (uint)(temp.RepairPrice() * 3 * script.PriceRate(this, true));
                }

                if (cost > Account.Gold) return;

                Account.Gold -= cost;
                SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });
                if (ob.Conq != null) ob.Conq.GuildInfo.GoldStorage += (cost - baseCost);

                if (!special) temp.MaxDura = (ushort)Math.Max(0, temp.MaxDura - (temp.MaxDura - temp.CurrentDura) / 30);

                temp.CurrentDura = temp.MaxDura;
                temp.DuraChanged = false;

                SendPacketToClient(new ServerPacket.ItemRepaired { UniqueID = uniqueID, MaxDura = temp.MaxDura, CurrentDura = temp.CurrentDura });
                return;
            }
        }
        public void SendStorage()
        {
            if (Connection.StorageSent) return;
            Connection.StorageSent = true;

            for (int i = 0; i < Account.Storage.Length; i++)
            {
                UserItem item = Account.Storage[i];
                if (item == null) continue;
                //CheckItemInfo(item.Info);
                CheckItem(item);
            }

            SendPacketToClient(new ServerPacket.UserStorage { Storage = Account.Storage }); // Should be no alter before being sent.
        }

        #endregion

        #region Consignment
        public void ConsignItem(ulong uniqueID, uint price, MarketPanelType panelType)
        {
            ServerPacket.ConsignItem p = new ServerPacket.ConsignItem { UniqueID = uniqueID };

            if (Dead || NPCPage == null)
            {
                SendPacketToClient(p);
                return;
            }

            switch (panelType)
            {
                case MarketPanelType.Consign:
                    {
                        if (price < Globals.MinConsignment || price > Globals.MaxConsignment)
                        {
                            SendPacketToClient(p);
                            return;
                        }

                        if (Account.Gold < Globals.ConsignmentCost)
                        {
                            SendPacketToClient(p);
                            return;
                        }
                    }
                    break;
                
                default:
                    SendPacketToClient(p);
                    return;
            }

            for (int n = 0; n < CurrentMap.NPCs.Count; n++)
            {
                NPCObjectSrv ob = CurrentMap.NPCs[n];
                if (ob.ObjectID != NPCObjectID) continue;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange)) return;

                UserItem temp = null;
                int index = -1;

                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    temp = Info.Inventory[i];
                    if (temp == null || temp.UniqueID != uniqueID) continue;
                    index = i;
                    break;
                }

                if (temp == null || index == -1)
                {
                    SendPacketToClient(p);
                    return;
                }

                if (temp.Info.Bind.HasFlag(BindMode.DontSell))
                {
                    SendPacketToClient(p);
                    return;
                }

                MarketItemType type =MarketItemType.Consign;
                uint cost = Globals.ConsignmentCost;

                //TODO Check Max Consignment.

                p.Success = true;
                SendPacketToClient(p);

                Info.Inventory[index] = null;

                Account.Gold -= cost;

                SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });
                RefreshBagWeight();
            }

            SendPacketToClient(p);
        }
         

        
        public void RequestUserName(uint id)
        {
            CharacterInfo Character = Envir.GetCharacterInfo((int)id);
            if (Character != null)
                SendPacketToClient(new ServerPacket.UserName { Id = (uint)Character.Index, Name = Character.Name });
        }

        #endregion

        #region Groups

        public void SwitchGroup(bool allow)
        {
            SendPacketToClient(new ServerPacket.SwitchGroup { AllowGroup = allow });

            if (AllowGroup == allow) return;
            AllowGroup = allow;

            if (AllowGroup || GroupMembers == null) return;

            LeaveGroup();
        }

        public void LeaveGroup()
        {
            if (GroupMembers != null)
            {
                GroupMembers.Remove(this);

                if (GroupMembers.Count > 1)
                {
                    Packet p = new ServerPacket.DeleteMember { Name = Name };

                    for (int i = 0; i < GroupMembers.Count; i++)
                    {
                        GroupMembers[i].SendPacketToClient(p);
                    }
                }
                else
                {
                    GroupMembers[0].SendPacketToClient(new ServerPacket.DeleteGroup());
                    GroupMembers[0].GroupMembers = null;
                }

                GroupMembers = null;
            }
        }

        public void AddMember(string name)
        {
            if (Envir.Time < NextGroupInviteTime) return;
            NextGroupInviteTime = Envir.Time + Settings.GroupInviteDelay;
            if (GroupMembers != null && GroupMembers[0] != this)
            {
                ReceiveChat("You are not the group leader.", ChatType.System);
                return;
            }

            if (GroupMembers != null && GroupMembers.Count >= Globals.MaxGroup)
            {
                ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }

            PlayerObjectSrv player = Envir.GetPlayer(name);

            if (player == null)
            {
                ReceiveChat(name + " could not be found.", ChatType.System);
                return;
            }
            if (player == this)
            {
                ReceiveChat("You cannot group yourself.", ChatType.System);
                return;
            }

            if (!player.AllowGroup)
            {
                ReceiveChat(name + " is not allowing group.", ChatType.System);
                return;
            }

            if (player.GroupMembers != null)
            {
                ReceiveChat(name + " is already in another group.", ChatType.System);
                return;
            }

            if (player.GroupInvitation != null)
            {
                ReceiveChat(name + " is already receiving an invite from another player.", ChatType.System);
                return;
            }

            SwitchGroup(true);
            player.SendPacketToClient(new ServerPacket.GroupInvite { Name = Name });
            player.GroupInvitation = this;

        }
        public void DelMember(string name)
        {
            if (GroupMembers == null)
            {
                ReceiveChat("You are not in a group.", ChatType.System);
                return;
            }
            if (GroupMembers[0] != this)
            {
                ReceiveChat("You are not the group leader.", ChatType.System);
                return;
            }

            PlayerObjectSrv player = null;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                if (String.Compare(GroupMembers[i].Name, name, StringComparison.OrdinalIgnoreCase) != 0) continue;
                player = GroupMembers[i];
                break;
            }

            if (player == null)
            {
                ReceiveChat(name + " is not in your group.", ChatType.System);
                return;
            }

            player.SendPacketToClient(new ServerPacket.DeleteGroup());
            player.LeaveGroup();
        }

        public void GroupInvite(bool accept)
        {
            if (GroupInvitation == null)
            {
                ReceiveChat("You have not been invited to a group.", ChatType.System);
                return;
            }

            if (!accept)
            {
                GroupInvitation.ReceiveChat(Name + " has declined your group invite.", ChatType.System);
                GroupInvitation = null;
                return;
            }

            if (GroupMembers != null)
            {
                ReceiveChat(string.Format("You can no longer join {0}'s group", GroupInvitation.Name), ChatType.System);
                GroupInvitation = null;
                return;
            }

            if (GroupInvitation.GroupMembers != null && GroupInvitation.GroupMembers[0] != GroupInvitation)
            {
                ReceiveChat(GroupInvitation.Name + " is no longer the group leader.", ChatType.System);
                GroupInvitation = null;
                return;
            }

            if (GroupInvitation.GroupMembers != null && GroupInvitation.GroupMembers.Count >= Globals.MaxGroup)
            {
                ReceiveChat(GroupInvitation.Name + "'s group already has the maximum number of members.", ChatType.System);
                GroupInvitation = null;
                return;
            }
            if (!GroupInvitation.AllowGroup)
            {
                ReceiveChat(GroupInvitation.Name + " is not on allow group.", ChatType.System);
                GroupInvitation = null;
                return;
            }
            if (GroupInvitation.Node == null)
            {
                ReceiveChat(GroupInvitation.Name + " no longer online.", ChatType.System);
                GroupInvitation = null;
                return;
            }

            if (GroupInvitation.GroupMembers == null)
            {
                GroupInvitation.GroupMembers = new List<PlayerObjectSrv> { GroupInvitation };
                GroupInvitation.SendPacketToClient(new ServerPacket.AddMember { Name = GroupInvitation.Name });
                GroupInvitation.SendPacketToClient(new ServerPacket.GroupMembersMap { PlayerName = GroupInvitation.Name, PlayerMap = GroupInvitation.CurrentMap.Info.Title });
                GroupInvitation.SendPacketToClient(new ServerPacket.SendMemberLocation { MemberName = GroupInvitation.Name, MemberLocation = GroupInvitation.CurrentLocation });
            }

            Packet p = new ServerPacket.AddMember { Name = Name };
            GroupMembers = GroupInvitation.GroupMembers;
            GroupInvitation = null;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                PlayerObjectSrv member = GroupMembers[i];

                member.SendPacketToClient(p);
                SendPacketToClient(new ServerPacket.AddMember { Name = member.Name });

                if (CurrentMap != member.CurrentMap || !Functions.InRange(CurrentLocation, member.CurrentLocation, Globals.DataRange)) continue;

                byte time = Math.Min(byte.MaxValue, (byte)Math.Max(5, (RevTime - Envir.Time) / 1000));

                member.SendPacketToClient(new ServerPacket.ObjectHealth { ObjectID = ObjectID, Percent = PercentHealth, Expire = time });
                SendPacketToClient(new ServerPacket.ObjectHealth { ObjectID = member.ObjectID, Percent = member.PercentHealth, Expire = time });

               
            }

            GroupMembers.Add(this);

            
            SendPacketToClient(p);
            GroupMemberMapNameChanged();
            GetPlayerLocation();
        }
        public void GroupMemberMapNameChanged()
        {
            if (GroupMembers == null) return;

            for (int i = 0; i < GroupMembers.Count; i++)
            {
                PlayerObjectSrv member = GroupMembers[i];
                member.SendPacketToClient(new ServerPacket.GroupMembersMap { PlayerName = Name, PlayerMap = CurrentMap.Info.Title });
                SendPacketToClient(new ServerPacket.GroupMembersMap { PlayerName = member.Name, PlayerMap = member.CurrentMap.Info.Title });
            }
            SendPacketToClient(new ServerPacket.GroupMembersMap { PlayerName = Name, PlayerMap = CurrentMap.Info.Title });
        }

        #endregion

        

        #region Guilds

        public bool CreateGuild(string guildName)
        {
            if ((MyGuild != null) || (Info.GuildIndex != -1)) return false;
            if (Envir.GetGuild(guildName) != null) return false;

            if (Info.Level < Settings.Guild_RequiredLevel)
            {
                ReceiveChat(String.Format("Your level is not high enough to create a guild, required: {0}", Settings.Guild_RequiredLevel), ChatType.System);
                return false;
            }

            if(!Info.AccountInfo.AdminAccount && String.Equals(guildName, Settings.NewbieGuild, StringComparison.OrdinalIgnoreCase))
            {
                ReceiveChat($"You cannot make the newbie guild. Nice try mortal.", ChatType.System);
                return false;
            }

            if (!Info.AccountInfo.AdminAccount)
            {
                //check if we have the required items
                for (int i = 0; i < Settings.Guild_CreationCostList.Count; i++)
                {
                    GuildItemVolume Required = Settings.Guild_CreationCostList[i];
                    if (Required.Item == null)
                    {
                        if (Info.AccountInfo.Gold < Required.Amount)
                        {
                            ReceiveChat(String.Format("Insufficient gold. Creating a guild requires {0} gold.", Required.Amount), ChatType.System);
                            return false;
                        }
                    }
                    else
                    {
                        ushort count = (ushort)Math.Min(Required.Amount, ushort.MaxValue);

                        foreach (var item in Info.Inventory.Where(item => item != null && item.Info == Required.Item))
                        {
                            if ((Required.Item.Type == ItemType.Ore) && (item.CurrentDura / 1000 > Required.Amount))
                            {
                                count = 0;
                                break;
                            }
                            if (item.Count > count)
                                count = 0;
                            else
                                count = (ushort)(count - item.Count);
                            if (count == 0) break;
                        }
                        if (count != 0)
                        {
                            if (Required.Amount == 1)
                                ReceiveChat(String.Format("{0} is required to create a guild.", Required.Item.FriendlyName), ChatType.System);
                            else
                            {
                                if (Required.Item.Type == ItemType.Ore)
                                    ReceiveChat(string.Format("{0} with purity {1} is recuired to create a guild.", Required.Item.FriendlyName, Required.Amount / 1000), ChatType.System);
                                else
                                    ReceiveChat(string.Format("Insufficient {0}, you need {1} to create a guild.", Required.Item.FriendlyName, Required.Amount), ChatType.System);
                            }
                            return false;
                        }
                    }
                }

                //take the required items
                for (int i = 0; i < Settings.Guild_CreationCostList.Count; i++)
                {
                    GuildItemVolume Required = Settings.Guild_CreationCostList[i];
                    if (Required.Item == null)
                    {
                        if (Info.AccountInfo.Gold >= Required.Amount)
                        {
                            Info.AccountInfo.Gold -= Required.Amount;
                            SendPacketToClient(new ServerPacket.LoseGold { Gold = Required.Amount });
                        }
                    }
                    else
                    {
                        ushort count = (ushort)Math.Min(Required.Amount, ushort.MaxValue);

                        for (int o = 0; o < Info.Inventory.Length; o++)
                        {
                            UserItem item = Info.Inventory[o];
                            if (item == null) continue;
                            if (item.Info != Required.Item) continue;

                            if ((Required.Item.Type == ItemType.Ore) && (item.CurrentDura / 1000 > Required.Amount))
                            {
                                SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                                Info.Inventory[o] = null;
                                break;
                            }
                            if (count > item.Count)
                            {
                                SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                                Info.Inventory[o] = null;
                                count -= item.Count;
                                continue;
                            }

                            SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = item.UniqueID, Count = (ushort)count });
                            if (count == item.Count)
                                Info.Inventory[o] = null;
                            else
                                item.Count -= (ushort)count;
                            break;
                        }
                    }
                }
                RefreshStats();
            }
            
            //make the guild
            var guildInfo = new GuildInfo(this, guildName) { GuildIndex = ++Envir.NextGuildID };
            Envir.GuildList.Add(guildInfo);

            GuildObjectSrv guild = new GuildObjectSrv(guildInfo);
            Info.GuildIndex = guildInfo.GuildIndex;

            MyGuild = guild;
            MyGuildRank = guild.FindRank(Name);
            GuildMembersChanged = true;
            GuildNoticeChanged = true;
            GuildCanRequestItems = true;

            //tell us we now have a guild
            BroadcastInfo();
            MyGuild.SendGuildStatus(this);

            return true;
        }

        public void EditGuildMember(string Name, string RankName, byte RankIndex, byte ChangeType)
        {
            if ((MyGuild == null) || (MyGuildRank == null))
            {
                ReceiveChat(GameLanguage.NotInGuild, ChatType.System);
                return;
            }
            switch (ChangeType)
            {
                case 0: //add member
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanRecruit))
                    {
                        ReceiveChat("You are not allowed to recruit new members!", ChatType.System);
                        return;
                    }

                    if (Name == "") return;

                    PlayerObjectSrv player = Envir.GetPlayer(Name);
                    if (player == null)
                    {
                        ReceiveChat(String.Format("{0} is not online!", Name), ChatType.System);
                        return;
                    }
                    if ((player.MyGuild != null) || (player.MyGuildRank != null) || (player.Info.GuildIndex != -1))
                    {
                        ReceiveChat(String.Format("{0} is already in a guild!", Name), ChatType.System);
                        return;
                    }
                    if (!player.EnableGuildInvite)
                    {
                        ReceiveChat(String.Format("{0} is disabling guild invites!", Name), ChatType.System);
                        return;
                    }
                    if (player.PendingGuildInvite != null)
                    {
                        ReceiveChat(string.Format("{0} already has a guild invite pending.", Name), ChatType.System);
                        return;
                    }

                    if (MyGuild.IsAtWar())
                    {
                        ReceiveChat("Cannot recuit members whilst at war.", ChatType.System);
                        return;
                    }

                    player.SendPacketToClient(new ServerPacket.GuildInvite { Name = MyGuild.Name });
                    player.PendingGuildInvite = MyGuild;
                    break;
                case 1: //delete member
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanKick))
                    {
                        ReceiveChat("You are not allowed to remove members!", ChatType.System);
                        return;
                    }
                    if (Name == "") return;

                    if (!MyGuild.DeleteMember(this, Name))
                    {
                        return;
                    }
                    break;
                case 2: //promote member (and it'll auto create a new rank at bottom if the index > total ranks!)
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank))
                    {
                        ReceiveChat("You are not allowed to change other members rank!", ChatType.System);
                        return;
                    }
                    if (Name == "") return;
                    MyGuild.ChangeRank(this, Name, RankIndex, RankName);
                    break;
                case 3: //change rank name
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank))
                    {
                        ReceiveChat("You are not allowed to change ranks!", ChatType.System);
                        return;
                    }
                    if ((RankName == "") || (RankName.Length < 3))
                    {
                        ReceiveChat("Rank name to short!", ChatType.System);
                        return;
                    }
                    if (RankName.Contains("\\") || RankName.Length > 20)
                    {
                        return;
                    }
                    if (!MyGuild.ChangeRankName(this, RankName, RankIndex))
                        return;
                    break;
                case 4: //new rank
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank))
                    {
                        ReceiveChat("You are not allowed to change ranks!", ChatType.System);
                        return;
                    }
                    if (MyGuild.Ranks.Count > 254)
                    {
                        ReceiveChat("No more rank slots available.", ChatType.System);
                        return;
                    }
                    MyGuild.NewRank(this);
                    break;
                case 5: //change rank setting
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeRank))
                    {
                        ReceiveChat("You are not allowed to change ranks!", ChatType.System);
                        return;
                    }
                    int temp;

                    if (!int.TryParse(RankName, out temp))
                    {
                        return;
                    }
                    MyGuild.ChangeRankOption(this, RankIndex, temp, Name);
                    break;
            }
        }
        public void EditGuildNotice(List<string> notice)
        {
            if ((MyGuild == null) || (MyGuildRank == null))
            {
                ReceiveChat(GameLanguage.NotInGuild, ChatType.System);
                return;
            }
            if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanChangeNotice))
            {

                ReceiveChat("You are not allowed to change the guild notice!", ChatType.System);
                return;
            }
            if (notice.Count > 200)
            {
                ReceiveChat("Guild notice can not be longer then 200 lines!", ChatType.System);
                return;
            }
            MyGuild.NewNotice(notice);
        }
        public void GuildInvite(bool accept)
        {
            if (PendingGuildInvite == null)
            {
                ReceiveChat("You have not been invited to a guild.", ChatType.System);
                return;
            }
            if (!accept) return;
            if (!PendingGuildInvite.HasRoom())
            {
                ReceiveChat(String.Format("{0} is full.", PendingGuildInvite.Name), ChatType.System);
                return;
            }
            PendingGuildInvite.NewMember(this);
            Info.GuildIndex = PendingGuildInvite.Guildindex;
            MyGuild = PendingGuildInvite;
            MyGuildRank = PendingGuildInvite.FindRank(Name);
            GuildMembersChanged = true;
            GuildNoticeChanged = true;
            //tell us we now have a guild
            BroadcastInfo();
            MyGuild.SendGuildStatus(this);
            PendingGuildInvite = null;
            EnableGuildInvite = false;
            GuildCanRequestItems = true;
            //refresh guildbuffs
            RefreshStats();
            if (MyGuild.BuffList.Count > 0)
                SendPacketToClient(new ServerPacket.GuildBuffList() { ActiveBuffs = MyGuild.BuffList});
        }
        public void RequestGuildInfo(byte Type)
        {
            if (MyGuild == null) return;
            if (MyGuildRank == null) return;
            switch (Type)
            {
                case 0://notice
                    if (GuildNoticeChanged)
                        SendPacketToClient(new ServerPacket.GuildNoticeChange() { notice = MyGuild.Info.Notice });
                    GuildNoticeChanged = false;
                    break;
                case 1://memberlist
                    if (GuildMembersChanged)
                        SendPacketToClient(new ServerPacket.GuildMemberChange() { Status = 255, Ranks = MyGuild.Ranks });
                    break;
            }
        }
        public void GuildNameReturn(string Name)
        {
            if (Name == "") CanCreateGuild = false;
            if (!CanCreateGuild) return;
            if ((Name.Length < 3) || (Name.Length > 20))
            {
                ReceiveChat("Guild name too long.", ChatType.System);
                CanCreateGuild = false;
                return;
            }
            if (Name.Contains('\\'))
            {
                CanCreateGuild = false;
                return;
            }
            if (MyGuild != null)
            {
                ReceiveChat("You are already part of a guild.", ChatType.System);
                CanCreateGuild = false;
                return;
            }
            GuildObjectSrv guild = Envir.GetGuild(Name);
            if (guild != null)
            {
                ReceiveChat(string.Format("Guild {0} already exists.", Name), ChatType.System);
                CanCreateGuild = false;
                return;
            }

            CreateGuild(Name);
            CanCreateGuild = false;
        }
        public void GuildStorageGoldChange(byte type, uint amount)
        {
            if ((MyGuild == null) || (MyGuildRank == null))
            {
                ReceiveChat("You are not part of a guild.", ChatType.System);
                return;
            }

            if (!InSafeZone)
            {
                ReceiveChat("You cannot use guild storage outside safezones.", ChatType.System);
                return;
            }

            if (type == 0)//donate
            {
                if (Account.Gold < amount)
                {
                    ReceiveChat("Insufficient gold.", ChatType.System);
                    return;
                }

                if ((MyGuild.Gold + (ulong)amount) > uint.MaxValue)
                {
                    ReceiveChat("Guild gold limit reached.", ChatType.System);
                    return;
                }

                Account.Gold -= amount;
                MyGuild.Gold += amount;
                SendPacketToClient(new ServerPacket.LoseGold { Gold = amount });
                MyGuild.SendServerPacket(new ServerPacket.GuildStorageGoldChange() { Type = 0, Name = Info.Name, Amount = amount });
                MyGuild.NeedSave = true;
            }
            else
            {
                if (MyGuild.Gold < amount)
                {
                    ReceiveChat("Insufficient gold.", ChatType.System);
                    return;
                }

                if (!CanGainGold(amount))
                {
                    ReceiveChat("Gold limit reached.", ChatType.System);
                    return;
                }

                if (MyGuildRank.Index != 0)
                {
                    ReceiveChat("Insufficient rank.", ChatType.System);
                    return;
                }

                MyGuild.Gold -= amount;
                GainGold(amount);
                MyGuild.SendServerPacket(new ServerPacket.GuildStorageGoldChange() { Type = 1, Name = Info.Name, Amount = amount });
                MyGuild.NeedSave = true;
            }
        }
        public void GuildStorageItemChange(byte type, int from, int to)
        {
            ServerPacket.GuildStorageItemChange p = new ServerPacket.GuildStorageItemChange { Type = (byte)(3 + type), From = from, To = to };
            if ((MyGuild == null) || (MyGuildRank == null))
            {
                SendPacketToClient(p);
                ReceiveChat("You are not part of a guild.", ChatType.System);
                return;
            }

            if (!InSafeZone && type != 3)
            {
                SendPacketToClient(p);
                ReceiveChat("You cannot use guild storage outside safezones.", ChatType.System);
                return;
            }

            switch (type)
            {
                case 0://store
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanStoreItem))
                    {
                        SendPacketToClient(p);
                        ReceiveChat("You do not have permission to store items in guild storage.", ChatType.System);
                        return;
                    }
                    if (from < 0 || from >= Info.Inventory.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (to < 0 || to >= MyGuild.StoredItems.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (Info.Inventory[from] == null)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (Info.Inventory[from].Info.Bind.HasFlag(BindMode.DontStore))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (Info.Inventory[from].RentalInformation != null && Info.Inventory[from].RentalInformation.BindingFlags.HasFlag(BindMode.DontStore))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[to] != null)
                    {
                        ReceiveChat("Target slot not empty.", ChatType.System);
                        SendPacketToClient(p);
                        return;
                    }
                    MyGuild.StoredItems[to] = new GuildStorageItem() { Item = Info.Inventory[from], UserId = Info.Index };
                    Info.Inventory[from] = null;
                    RefreshBagWeight();
                    MyGuild.SendItemInfo(MyGuild.StoredItems[to].Item);
                    MyGuild.SendServerPacket(new ServerPacket.GuildStorageItemChange() { Type = 0, User = Info.Index, Item = MyGuild.StoredItems[to], To = to, From = from });
                    MyGuild.NeedSave = true;
                    break;
                case 1://retrieve
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanRetrieveItem))
                    {

                        ReceiveChat("You do not have permission to retrieve items from guild storage.", ChatType.System);
                        return;
                    }
                    if (from < 0 || from >= MyGuild.StoredItems.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (to < 0 || to >= Info.Inventory.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (Info.Inventory[to] != null)
                    {
                        ReceiveChat("Target slot not empty.", ChatType.System);
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[from] == null)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[from].Item.Info.Bind.HasFlag(BindMode.DontStore))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    Info.Inventory[to] = MyGuild.StoredItems[from].Item;
                    MyGuild.StoredItems[from] = null;
                    MyGuild.SendServerPacket(new ServerPacket.GuildStorageItemChange() { Type = 1, User = Info.Index, To = to, From = from });
                    RefreshBagWeight();
                    MyGuild.NeedSave = true;
                    break;
                case 2: // Move Item
                    GuildStorageItem q = null;
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanStoreItem))
                    {
                        SendPacketToClient(p);
                        ReceiveChat("You do not have permission to move items in guild storage.", ChatType.System);
                        return;
                    }
                    if (from < 0 || from >= MyGuild.StoredItems.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (to < 0 || to >= MyGuild.StoredItems.Length)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[from] == null)
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[from].Item.Info.Bind.HasFlag(BindMode.DontStore))
                    {
                        SendPacketToClient(p);
                        return;
                    }
                    if (MyGuild.StoredItems[to] != null)
                    {
                        q = MyGuild.StoredItems[to];
                    }
                    MyGuild.StoredItems[to] = MyGuild.StoredItems[from];
                    if (q != null) MyGuild.StoredItems[from] = q;
                    else MyGuild.StoredItems[from] = null;

                    MyGuild.SendItemInfo(MyGuild.StoredItems[to].Item);

                    if (MyGuild.StoredItems[from] != null) MyGuild.SendItemInfo(MyGuild.StoredItems[from].Item);

                    MyGuild.SendServerPacket(new ServerPacket.GuildStorageItemChange() { Type = 2, User = Info.Index, Item = MyGuild.StoredItems[to], To = to, From = from });
                    MyGuild.NeedSave = true;
                    break;
                case 3://request list
                    if (!GuildCanRequestItems) return;
                    GuildCanRequestItems = false;
                    for (int i = 0; i < MyGuild.StoredItems.Length; i++)
                    {
                        if (MyGuild.StoredItems[i] == null) continue;
                        UserItem item = MyGuild.StoredItems[i].Item;
                        if (item == null) continue;
                        //CheckItemInfo(item.Info);
                        CheckItem(item);
                    }
                    SendPacketToClient(new ServerPacket.GuildStorageList() { Items = MyGuild.StoredItems });
                    break;
            }

        }
        public void GuildWarReturn(string Name)
        {
            if (MyGuild == null || MyGuildRank != MyGuild.Ranks[0]) return;

            GuildObjectSrv enemyGuild = Envir.GetGuild(Name);

            if (enemyGuild == null)
            {
                ReceiveChat(string.Format("Could not find guild {0}.", Name), ChatType.System);
                return;
            }

            if (MyGuild == enemyGuild)
            {
                ReceiveChat("Cannot go to war with your own guild.", ChatType.System);
                return;
            }

            if (MyGuild.WarringGuilds.Contains(enemyGuild))
            {
                ReceiveChat("Already at war with this guild.", ChatType.System);
                return;
            }

            if (MyGuild.Gold < Settings.Guild_WarCost)
            {
                ReceiveChat("Not enough funds in guild bank.", ChatType.System);
                return;
            }

            if (MyGuild.GoToWar(enemyGuild))
            {
                ReceiveChat(string.Format("You started a war with {0}.", Name), ChatType.System);
                enemyGuild.SendMessage(string.Format("{0} has started a war", MyGuild.Name), ChatType.System);

                MyGuild.Gold -= Settings.Guild_WarCost;
                MyGuild.SendServerPacket(new ServerPacket.GuildStorageGoldChange() { Type = 2, Name = Info.Name, Amount = Settings.Guild_WarCost });
            }
        }

        public override bool AtWar(HumanObjectSrv attacker)
        {
            if (CurrentMap.Info.Fight) return true;

            if (MyGuild == null) return false;

            if (attacker is PlayerObjectSrv playerAttacker)
            {
                if (attacker == null || playerAttacker.MyGuild == null) return false;

                if (!MyGuild.WarringGuilds.Contains(playerAttacker.MyGuild)) return false;
            }

            return true;
        }
        protected override void CleanUp()
        {
            base.CleanUp();
            Account = null;            
        }

        public void GuildBuffUpdate(byte type, int id)
        {
            if (MyGuild == null) return;
            if (MyGuildRank == null) return;
            if (id < 0) return;
            switch (type)
            {
                case 0://request info list
                    if (RequestedGuildBuffInfo) return;
                    SendPacketToClient(new ServerPacket.GuildBuffList() { GuildBuffs = Settings.Guild_BuffList });
                    break;
                case 1://buy the buff
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanActivateBuff))
                    {
                        ReceiveChat("You do not have the correct guild rank.", ChatType.System);
                        return;
                    }
                    GuildBuffInfo BuffInfo = Envir.FindGuildBuffInfo(id);
                    if (BuffInfo == null)
                    {
                        ReceiveChat("Buff does not excist.", ChatType.System);
                        return;
                    }
                    if (MyGuild.GetBuff(id) != null)
                    {
                        ReceiveChat("Buff already obtained.", ChatType.System);
                        return;
                    }
                    if ((MyGuild.Info.Level < BuffInfo.LevelRequirement) || (MyGuild.Info.SparePoints < BuffInfo.PointsRequirement)) return;//client checks this so it shouldnt be possible without a moded client :p
                    MyGuild.NewBuff(id);
                    break;
                case 2://activate the buff
                    if (!MyGuildRank.Options.HasFlag(GuildRankOptions.CanActivateBuff))
                    {
                        ReceiveChat("You do not have the correct guild rank.", ChatType.System);
                        return;
                    }
                    GuildBuff Buff = MyGuild.GetBuff(id);
                    if (Buff == null)
                    {
                        ReceiveChat("Buff not obtained.", ChatType.System);
                        return;
                    }
                    if ((MyGuild.Gold < Buff.Info.ActivationCost) || (Buff.Active)) return;
                    MyGuild.ActivateBuff(id);
                    break;
            }
        }

        #endregion

        #region Trading

        public void DepositTradeItem(int from, int to)
        {
            ServerPacket.DepositTradeItem p = new ServerPacket.DepositTradeItem { From = from, To = to, Success = false };

            if (from < 0 || from >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Info.Trade.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Info.Inventory[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.Info.Bind.HasFlag(BindMode.DontTrade))
            {
                SendPacketToClient(p);
                return;
            }

            if (temp.RentalInformation != null && temp.RentalInformation.BindingFlags.HasFlag(BindMode.DontTrade))
            {
                SendPacketToClient(p);
                return;
            }

            if (Info.Trade[to] == null)
            {
                Info.Trade[to] = temp;
                Info.Inventory[from] = null;
                RefreshBagWeight();
                TradeItem();

                Report.ItemMoved(temp, MirGridType.Inventory, MirGridType.Trade, from, to);
                
                p.Success = true;
                SendPacketToClient(p);
                return;
            }
            SendPacketToClient(p);

        }
        public void RetrieveTradeItem(int from, int to)
        {
            ServerPacket.RetrieveTradeItem p = new ServerPacket.RetrieveTradeItem { From = from, To = to, Success = false };

            if (from < 0 || from >= Info.Trade.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Info.Trade[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (Info.Inventory[to] == null)
            {
                Info.Inventory[to] = temp;
                Info.Trade[from] = null;

                p.Success = true;
                RefreshBagWeight();
                TradeItem();

                Report.ItemMoved(temp, MirGridType.Trade, MirGridType.Inventory, from, to);
            }

            SendPacketToClient(p);
        }

        

        public void TradeRequest()
        {
            if (Envir.Time < NextTradeTime) return;
            NextTradeTime = Envir.Time + Settings.TradeDelay;

            if (TradePartner != null)
            {
                ReceiveChat("You are already trading.", ChatType.System);
                return;
            }

            Point target = Functions.PointMove(CurrentLocation, Direction, 1);
            Cell cell = CurrentMap.GetCell(target);
            PlayerObjectSrv player = null;

            if (cell.Objects == null || cell.Objects.Count == 0) 
            {
                ReceiveChat(GameLanguage.FaceToTrade, ChatType.System);
                return;
            } 

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObjectSrv ob = cell.Objects[i];
                if (ob.Race != ObjectType.Player) continue;

                player = Envir.GetPlayer(ob.Name);
            }

            if (player == null)
            {
                ReceiveChat(GameLanguage.FaceToTrade, ChatType.System);
                return;
            }

            if (player != null)
            {
                if (!Functions.FacingEachOther(Direction, CurrentLocation, player.Direction, player.CurrentLocation))
                {
                    ReceiveChat(GameLanguage.FaceToTrade, ChatType.System);
                    return;
                }

                if (player == this)
                {
                    ReceiveChat("You cannot trade with your self.", ChatType.System);
                    return;
                }

                if (player.Dead || Dead)
                {
                    ReceiveChat("Cannot trade when dead", ChatType.System);
                    return;
                }

                if (player.TradeInvitation != null)
                {
                    ReceiveChat(string.Format("Player {0} already has a trade invitation.", player.Info.Name), ChatType.System);
                    return;
                }

                if (!player.AllowTrade)
                {
                    ReceiveChat(string.Format("Player {0} is not allowing trade at the moment.", player.Info.Name), ChatType.System);
                    return;
                }

                if (!Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) || player.CurrentMap != CurrentMap)
                {
                    ReceiveChat(string.Format("Player {0} is not within trading range.", player.Info.Name), ChatType.System);
                    return;
                }

                if (player.TradePartner != null)
                {
                    ReceiveChat(string.Format("Player {0} is already trading.", player.Info.Name), ChatType.System);
                    return;
                }

                player.TradeInvitation = this;
                player.SendPacketToClient(new ServerPacket.TradeRequest { Name = Info.Name });
            }
        }
        public void TradeReply(bool accept)
        {
            if (TradeInvitation == null || TradeInvitation.Info == null)
            {
                TradeInvitation = null;
                return;
            }

            if (!accept)
            {
                TradeInvitation.ReceiveChat(string.Format("Player {0} has refused to trade.", Info.Name), ChatType.System);
                TradeInvitation = null;
                return;
            }

            if (TradePartner != null)
            {
                ReceiveChat("You are already trading.", ChatType.System);
                TradeInvitation = null;
                return;
            }

            if (TradeInvitation.TradePartner != null)
            {
                ReceiveChat(string.Format("Player {0} is already trading.", TradeInvitation.Info.Name), ChatType.System);
                TradeInvitation = null;
                return;
            }

            TradePartner = TradeInvitation;
            TradeInvitation.TradePartner = this;
            TradeInvitation = null;

            SendPacketToClient(new ServerPacket.TradeAccept { Name = TradePartner.Info.Name });
            TradePartner.SendPacketToClient(new ServerPacket.TradeAccept { Name = Info.Name });
        }
        public void TradeGold(uint amount)
        {
            TradeUnlock();

            if (TradePartner == null) return;

            if (amount < 1 || Account.Gold < amount)
            {
                return;
            }

            TradeGoldAmount += amount;
            Account.Gold -= amount;

            SendPacketToClient(new ServerPacket.LoseGold { Gold = amount });
            TradePartner.SendPacketToClient(new ServerPacket.TradeGold { Amount = TradeGoldAmount });
        }
        public void TradeItem()
        {
            TradeUnlock();

            if (TradePartner == null) return;

            for (int i = 0; i < Info.Trade.Length; i++)
            {
                UserItem u = Info.Trade[i];
                if (u == null) continue;

                //TradePartner.CheckItemInfo(u.Info);
                TradePartner.CheckItem(u);
            }

            TradePartner.SendPacketToClient(new ServerPacket.TradeItem { TradeItems = Info.Trade });
        }

        public void TradeUnlock()
        {
            TradeLocked = false;

            if (TradePartner != null)
            {
                TradePartner.TradeLocked = false;
            }
        }

        public void TradeConfirm(bool confirm)
        {
            if(!confirm)
            {
                TradeLocked = false;
                return;
            }

            if (TradePartner == null)
            {
                TradeCancel();
                return;
            }

            if (!Functions.InRange(TradePartner.CurrentLocation, CurrentLocation, Globals.DataRange) || TradePartner.CurrentMap != CurrentMap ||
                !Functions.FacingEachOther(Direction, CurrentLocation, TradePartner.Direction, TradePartner.CurrentLocation))
            {
                TradeCancel();
                return;
            }

            TradeLocked = true;

            if (TradeLocked && !TradePartner.TradeLocked)
            {
                TradePartner.ReceiveChat(string.Format("Player {0} is waiting for you to confirm trade.", Info.Name), ChatType.System);
            }

            if (!TradeLocked || !TradePartner.TradeLocked) return;

            PlayerObjectSrv[] TradePair = new PlayerObjectSrv[2] { TradePartner, this };

            bool CanTrade = true;
            UserItem u;

            //check if both people can accept the others items
            for (int p = 0; p < 2; p++)
            {
                int o = p == 0 ? 1 : 0;

                if (!TradePair[o].CanGainItems(TradePair[p].Info.Trade))
                {
                    CanTrade = false;
                    TradePair[p].ReceiveChat("Trading partner cannot accept all items.", ChatType.System);
                    TradePair[p].SendPacketToClient(new ServerPacket.TradeCancel { Unlock = true });

                    TradePair[o].ReceiveChat("Unable to accept all items.", ChatType.System);
                    TradePair[o].SendPacketToClient(new ServerPacket.TradeCancel { Unlock = true });

                    return;
                }

                if (!TradePair[o].CanGainGold(TradePair[p].TradeGoldAmount))
                {
                    CanTrade = false;
                    TradePair[p].ReceiveChat("Trading partner cannot accept any more gold.", ChatType.System);
                    TradePair[p].SendPacketToClient(new ServerPacket.TradeCancel { Unlock = true });

                    TradePair[o].ReceiveChat("Unable to accept any more gold.", ChatType.System);
                    TradePair[o].SendPacketToClient(new ServerPacket.TradeCancel { Unlock = true });

                    return;
                }
            }

            //swap items
            if (CanTrade)
            {
                for (int p = 0; p < 2; p++)
                {
                    int o = p == 0 ? 1 : 0;

                    for (int i = 0; i < TradePair[p].Info.Trade.Length; i++)
                    {
                        u = TradePair[p].Info.Trade[i];

                        if (u == null) continue;

                        TradePair[o].GainItem(u);
                        TradePair[p].Info.Trade[i] = null;

                        Report.ItemMoved(u, MirGridType.Trade, MirGridType.Inventory, i, -99, string.Format("Trade from {0} to {1}", TradePair[p].Name, TradePair[o].Name));
                    }

                    if (TradePair[p].TradeGoldAmount > 0)
                    {
                        Report.GoldChanged(TradePair[p].TradeGoldAmount, true, string.Format("Trade from {0} to {1}", TradePair[p].Name, TradePair[o].Name));

                        TradePair[o].GainGold(TradePair[p].TradeGoldAmount);
                        TradePair[p].TradeGoldAmount = 0;
                    }

                    TradePair[p].ReceiveChat("Trade successful.", ChatType.System);
                    TradePair[p].SendPacketToClient(new ServerPacket.TradeConfirm());

                    TradePair[p].TradeLocked = false;
                    TradePair[p].TradePartner = null;
                }
            }
        }
        public void TradeCancel()
        {
            TradeUnlock();

            if (TradePartner == null)
            {
                return;
            }

            PlayerObjectSrv[] TradePair = new PlayerObjectSrv[2] { TradePartner, this };

            for (int p = 0; p < 2; p++)
            {
                if (TradePair[p] != null)
                {
                    for (int t = 0; t < TradePair[p].Info.Trade.Length; t++)
                    {
                        UserItem temp = TradePair[p].Info.Trade[t];

                        if (temp == null) continue;

                        if(FreeSpace(TradePair[p].Info.Inventory) < 1)
                        {
                            
                            TradePair[p].SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = temp.UniqueID, Count = temp.Count });
                            TradePair[p].Info.Trade[t] = null;
                            continue;
                        }

                        for (int i = 0; i < TradePair[p].Info.Inventory.Length; i++)
                        {
                            if (TradePair[p].Info.Inventory[i] != null) continue;

                            //Put item back in inventory
                            if (TradePair[p].CanGainItem(temp))
                            {
                                TradePair[p].RetrieveTradeItem(t, i);
                            }
                            
                            TradePair[p].Info.Trade[t] = null;

                            break;
                        }
                    }

                    //Put back deposited gold
                    if (TradePair[p].TradeGoldAmount > 0)
                    {
                        Report.GoldChanged(TradePair[p].TradeGoldAmount, false);

                        TradePair[p].GainGold(TradePair[p].TradeGoldAmount);
                        TradePair[p].TradeGoldAmount = 0;
                    }

                    TradePair[p].TradeLocked = false;
                    TradePair[p].TradePartner = null;

                    TradePair[p].SendPacketToClient(new ServerPacket.TradeCancel { Unlock = false });
                }
            }
        }

        #endregion        

        #region Fishing

        public void FishingCast(bool cast, bool cancel = false)
        {
            UserItem rod = Info.Equipment[(int)EquipmentSlot.Weapon];

            byte flexibilityStat = 0;
            sbyte successStat = 0;
            byte nibbleMin = 0, nibbleMax = 0;
            byte failedAddSuccessMin = 0, failedAddSuccessMax = 0;
            FishingProgressMax = Settings.FishingAttempts;//30;

            if (rod == null || !rod.Info.IsFishingRod || rod.CurrentDura == 0)
            {
                Fishing = false;
                return;
            }

            Point fishingPoint = Functions.PointMove(CurrentLocation, Direction, 3);

            if (fishingPoint.X < 0 || fishingPoint.Y < 0 || CurrentMap.Width < fishingPoint.X || CurrentMap.Height < fishingPoint.Y)
            {
                Fishing = false;
                return;
            }

            Cell fishingCell = CurrentMap.Cells[fishingPoint.X, fishingPoint.Y];

            if (fishingCell.FishingAttribute < 0)
            {
                Fishing = false;
                return;
            }

            flexibilityStat = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, flexibilityStat + rod.Info.Stats[Stat.CriticalRate])));
            successStat = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, successStat + rod.Info.Stats[Stat.MaxAC])));

            if (cast)
            {
                DamageItem(rod, 1, true);
            }

            UserItem hook = rod.Slots[(int)FishingSlot.Hook];

            if (hook == null)
            {
                ReceiveChat("You need a hook.", ChatType.System);
                return;
            }
            else
            {
                DamagedFishingItem(FishingSlot.Hook, 1);
            }

            foreach (UserItem temp in rod.Slots)
            {
                if (temp == null) continue;

                ItemInfo realItem = Functions.GetRealItem(temp.Info, Info.Level, Info.Class, Envir.ItemInfoList);

                switch (realItem.Type)
                {
                    case ItemType.Hook:
                        {
                            flexibilityStat = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, flexibilityStat + temp.AddedStats[Stat.CriticalRate] + realItem.Stats[Stat.CriticalRate])));
                        }
                        break;
                    case ItemType.Float:
                        {
                            nibbleMin = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, nibbleMin + realItem.Stats[Stat.MinAC])));
                            nibbleMax = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, nibbleMax + realItem.Stats[Stat.MaxAC])));
                        }
                        break;
                    case ItemType.Bait:
                        {
                            successStat = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, successStat + realItem.Stats[Stat.MaxAC])));
                        }
                        break;
                    case ItemType.Finder:
                        {
                            failedAddSuccessMin = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, failedAddSuccessMin + realItem.Stats[Stat.MinAC])));
                            failedAddSuccessMax = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, failedAddSuccessMax + realItem.Stats[Stat.MaxAC])));
                        }
                        break;
                    case ItemType.Reel:
                        {
                            FishingAutoReelChance = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, FishingAutoReelChance + realItem.Stats[Stat.MaxMAC])));
                            successStat = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, successStat + realItem.Stats[Stat.MaxAC])));
                        }
                        break;
                    default:
                        break;
                }
            }

            FishingNibbleChance = 5 + Envir.Random.Next(nibbleMin, nibbleMax);

            if (cast) FishingChance = Settings.FishingSuccessStart + (int)successStat + (FishingChanceCounter != 0 ? Envir.Random.Next(failedAddSuccessMin, failedAddSuccessMax) : 0) + (FishingChanceCounter * Settings.FishingSuccessMultiplier); //10 //10
            if (FishingChanceCounter != 0) DamagedFishingItem(FishingSlot.Finder, 1);
            FishingChance += Stats[Stat.FishRatePercent];

            FishingChance = Math.Min(100, Math.Max(0, FishingChance));
            FishingNibbleChance = Math.Min(100, Math.Max(0, FishingNibbleChance));
            FishingAutoReelChance = Math.Min(100, Math.Max(0, FishingAutoReelChance));

            FishingTime = Envir.Time + FishingCastDelay + Settings.FishingDelay;

            if (cast)
            {
                if (Fishing) return;

                _fishCounter = 0;
                FishFound = false;

                UserItem item = GetBait(1);

                if (item == null)
                {
                    ReceiveChat("You need bait.", ChatType.System);
                    return;
                }

                ConsumeItem(item, 1);
                Fishing = true;
            }
            else
            {
                if (!Fishing)
                {
                    SendPacketToClient(GetFishInfo());
                    return;
                }

                Fishing = false;

                if (FishingProgress > 99)
                {
                    FishingChanceCounter++;
                }

                if (FishFound)
                {
                    int getChance = FishingChance + Envir.Random.Next(10, 24) + (FishingProgress > 50 ? flexibilityStat / 2 : 0);
                    getChance = Math.Min(100, Math.Max(0, getChance));

                    if (Envir.Random.Next(0, 100) <= getChance)
                    {
                        FishingChanceCounter = 0;

                        UserItem dropItem = null;

                        foreach (DropInfo drop in Envir.FishingDrops.Where(x => x.Type == fishingCell.FishingAttribute))
                        {
                            var reward = drop.AttemptDrop(EXPOwner?.Stats[Stat.ItemDropRatePercent] ?? 0, EXPOwner?.Stats[Stat.GoldDropRatePercent] ?? 0);

                            if (reward != null)
                            {
                                foreach (var dropitems in reward.Items)
                                {
                                    dropItem = Envir.CreateDropItem(drop.Item);
                                    break;
                                }
                            }
                        }

                        if (dropItem == null)
                        {
                            ReceiveChat("Your fish got away!", ChatType.System);
                        }
                        else if (FreeSpace(Info.Inventory) < 1)
                        {
                            ReceiveChat(GameLanguage.NoBagSpace, ChatType.System);
                        }
                        else
                        {
                            GainItem(dropItem);
                            Report.ItemChanged(dropItem, dropItem.Count, 2);
                        }

                        if (Envir.Random.Next(100 - Settings.FishingMobSpawnChance) == 0)
                        {
                            MonsterObjectSrv mob = MonsterObjectSrv.GetMonster(Envir.GetMonsterInfo(Settings.FishingMonster));

                            if (mob == null) return;

                            mob.Spawn(CurrentMap, Back);
                        }

                        DamagedFishingItem(FishingSlot.Reel, 1);

                        cancel = true;
                    }
                    else
                    {
                        ReceiveChat("Your fish got away!", ChatType.System);
                    }
                }

                FishFound = false;
                FishFirstFound = false;
            }

            SendPacketToClient(GetFishInfo());
            Broadcast(GetFishInfo());

            if (FishingAutocast && !cast && !cancel)
            {
                FishingTime = Envir.Time + (FishingCastDelay * 2);
                FishingFoundTime = Envir.Time;
                FishingAutoReelChance = 0;
                FishingNibbleChance = 0;
                FishFirstFound = false;

                FishingCast(true);
            }
        }
        public void FishingChangeAutocast(bool autoCast)
        {
            UserItem rod = Info.Equipment[(int)EquipmentSlot.Weapon];

            if (rod == null || !rod.Info.IsFishingRod) return;

            UserItem reel = rod.Slots[(int)FishingSlot.Reel];

            if (reel == null)
            {
                FishingAutocast = false;
                return;
            }

            FishingAutocast = autoCast;
        }
        public void UpdateFish()
        {
            if (FishFound != true && FishFirstFound != true)
            {
                FishFound = Envir.Random.Next(0, 100) <= FishingNibbleChance;
                FishingFoundTime = FishFound ? Envir.Time + 3000 : Envir.Time;

                if (FishFound)
                {
                    FishFirstFound = true;
                    DamagedFishingItem(FishingSlot.Float, 1);
                }
            }
            else
            {
                if (FishingAutoReelChance != 0 && Envir.Random.Next(0, 100) <= FishingAutoReelChance)
                {
                    FishingCast(false);
                }
            }

            if (FishingFoundTime < Envir.Time)
                FishFound = false;

            FishingTime = Envir.Time + FishingDelay;

            SendPacketToClient(GetFishInfo());

            if (FishingProgress > 100)
            {
                FishingCast(false);
            }
        }
        Packet GetFishInfo()
        {
            FishingProgress = _fishCounter > 0 ? (int)(((decimal)_fishCounter / FishingProgressMax) * 100) : 0;

            return new ServerPacket.FishingUpdate
            {
                ObjectID = ObjectID,
                Fishing = Fishing,
                ProgressPercent = FishingProgress,
                FishingPoint = Functions.PointMove(CurrentLocation, Direction, 3),
                ChancePercent = FishingChance,
                FoundFish = FishFound
            };
        }

        #endregion

        #region Quests

        public void AcceptQuest(int index)
        {
            bool canAccept = true;

            if (CurrentQuests.Exists(e => e.Index == index)) return; //e.Info.NpcIndex == npcIndex && 

            QuestInfo info = Envir.QuestInfoList.FirstOrDefault(d => d.Index == index);

            NPCObjectSrv npc = null;

            for (int i = CurrentMap.NPCs.Count - 1; i >= 0; i--)
            {
                if (CurrentMap.NPCs[i].ObjectID != info.NpcIndex) continue;

                if (!Functions.InRange(CurrentMap.NPCs[i].CurrentLocation, CurrentLocation, Globals.DataRange)) break;
                npc = CurrentMap.NPCs[i];
                break;
            }
            if (npc == null || !npc.VisibleLog[Info.Index] || !npc.Visible) return;

            if (!info.CanAccept(this))
            {
                canAccept = false;
            }

            if (CurrentQuests.Count >= Globals.MaxConcurrentQuests)
            {
                ReceiveChat("Maximum amount of quests already taken.", ChatType.System);
                return;
            }

            if (CompletedQuests.Contains(index))
            {
                ReceiveChat("Quest has already been completed.", ChatType.System);
                return;
            }

            //check previous chained quests have been completed
            QuestInfo tempInfo = info;
            while (tempInfo != null && tempInfo.RequiredQuest != 0)
            {
                if (!CompletedQuests.Contains(tempInfo.RequiredQuest))
                {
                    canAccept = false;
                    break;
                }

                tempInfo = Envir.QuestInfoList.FirstOrDefault(d => d.Index == tempInfo.RequiredQuest);
            }

            if (!canAccept)
            {
                ReceiveChat("Could not accept quest.", ChatType.System);
                return;
            }

            if (info.CarryItems.Count > 0)
            {
                foreach (QuestItemTask carryItem in info.CarryItems)
                {
                    ushort count = carryItem.Count;

                    while (count > 0)
                    {
                        UserItem item = Envir.CreateFreshItem(carryItem.Item);

                        if (item.Info.StackSize > count)
                        {
                            item.Count = count;
                            count = 0;
                        }
                        else
                        {
                            count -= item.Info.StackSize;
                            item.Count = item.Info.StackSize;
                        }

                        if (!CanGainQuestItem(item))
                        {
                            RecalculateQuestBag();
                            return;
                        }

                        GainQuestItem(item);

                        Report.ItemChanged(item, item.Count, 2);
                    }
                }
            }

            QuestProgressInfo quest = new QuestProgressInfo(index);

            quest.Init(this);
           
            SendUpdateQuest(quest, QuestState.Add, true);

            CallDefaultNPC(DefaultNPCType.OnAcceptQuest, index);
        }

        public void FinishQuest(int questIndex, int selectedItemIndex = -1)
        {
            QuestProgressInfo quest = CurrentQuests.FirstOrDefault(e => e.Info.Index == questIndex);

            if (quest == null || !quest.Completed) return;

            NPCObjectSrv npc = null;

            for (int i = CurrentMap.NPCs.Count - 1; i >= 0; i--)
            {
                if (CurrentMap.NPCs[i].ObjectID != quest.Info.FinishNpcIndex) continue;

                if (!Functions.InRange(CurrentMap.NPCs[i].CurrentLocation, CurrentLocation, Globals.DataRange)) break;
                npc = CurrentMap.NPCs[i];
                break;
            }
            if (npc == null || !npc.VisibleLog[Info.Index] || !npc.Visible) return;

            List<UserItem> rewardItems = new List<UserItem>();

            foreach (var reward in quest.Info.FixedRewards)
            {
                ushort count = reward.Count;

                UserItem rewardItem;

                while (count > 0)
                {
                    rewardItem = Envir.CreateFreshItem(reward.Item);
                    if (reward.Item.StackSize >= count)
                    {
                        rewardItem.Count = count;
                        count = 0;
                    }
                    else
                    {
                        rewardItem.Count = reward.Item.StackSize;
                        count -= reward.Item.StackSize;
                    }

                    rewardItems.Add(rewardItem);
                }
            }

            if (selectedItemIndex >= 0)
            {
                for (int i = 0; i < quest.Info.SelectRewards.Count; i++)
                {
                    if (selectedItemIndex != i) continue;

                    ushort count = quest.Info.SelectRewards[i].Count;
                    UserItem rewardItem;

                    while (count > 0)
                    {
                        rewardItem = Envir.CreateFreshItem(quest.Info.SelectRewards[i].Item);
                        if (quest.Info.SelectRewards[i].Item.StackSize >= count)
                        {
                            rewardItem.Count = count;
                            count = 0;
                        }
                        else
                        {
                            rewardItem.Count = quest.Info.SelectRewards[i].Item.StackSize;
                            count -= quest.Info.SelectRewards[i].Item.StackSize;
                        }

                        rewardItems.Add(rewardItem);
                    }
                }
            }

            if (!CanGainItems(rewardItems.ToArray()))
            {
                ReceiveChat("Cannot hand in quest whilst bag is full.", ChatType.System);
                return;
            }

            if (quest.Info.Type != QuestType.Repeatable)
            {
                Info.CompletedQuests.Add(quest.Index);
                GetCompletedQuests();
            }

            SendUpdateQuest(quest, QuestState.Remove);

            if (quest.Info.CarryItems.Count > 0)
            {
                foreach (QuestItemTask carryItem in quest.Info.CarryItems)
                {
                    TakeQuestItem(carryItem.Item, carryItem.Count);
                }
            }

            foreach (QuestItemTask iTask in quest.Info.ItemTasks)
            {
                TakeQuestItem(iTask.Item, iTask.Count);
            }

            foreach (UserItem item in rewardItems)
            {
                GainItem(item);
            }

            RecalculateQuestBag();

            GainGold(quest.Info.GoldReward);
            GainExp(quest.Info.ExpReward);
            GainCredit(quest.Info.CreditReward);

            CallDefaultNPC(DefaultNPCType.OnFinishQuest, questIndex);
        }
        public void AbandonQuest(int questIndex)
        {
            QuestProgressInfo quest = CurrentQuests.FirstOrDefault(e => e.Info.Index == questIndex);

            if (quest == null) return;
 
            SendUpdateQuest(quest, QuestState.Remove);

            RecalculateQuestBag();
        }
        public void ShareQuest(int questIndex)
        {
            bool shared = false;

            if (GroupMembers != null)
            {
                foreach (PlayerObjectSrv player in GroupMembers.
                    Where(player => player.CurrentMap == CurrentMap &&
                        Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) &&
                        !player.Dead && player != this))
                {
                    player.SendPacketToClient(new ServerPacket.ShareQuest { QuestIndex = questIndex, SharerName = Name });
                    shared = true;
                }
            }

            if (!shared)
            {
                ReceiveChat("Quest could not be shared with anyone.", ChatType.System);
            }
        }

        public void CheckGroupQuestKill(MonsterInfo mInfo)
        {
            if (GroupMembers != null)
            {
                foreach (PlayerObjectSrv player in GroupMembers.
                    Where(player => player.CurrentMap == CurrentMap &&
                        Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) &&
                        !player.Dead))
                {
                    player.CheckNeedQuestKill(mInfo);
                }
            }
            else
                CheckNeedQuestKill(mInfo);
        }
        public override bool CheckGroupQuestItem(UserItem item, bool gainItem = true)
        {
            bool itemCollected = false;

            if (GroupMembers != null)
            {
                foreach (PlayerObjectSrv player in GroupMembers.
                    Where(player => player != null && player.Node != null && player.CurrentMap == CurrentMap &&
                        Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) &&
                        !player.Dead))
                {
                    if (player.CheckNeedQuestItem(item, gainItem))
                    {
                        itemCollected = true;
                        player.Report.ItemChanged(item, item.Count, 2, "CheckGroupQuestItem (WinQuestItem)");
                    }
                }
            }
            else
            {
                if (CheckNeedQuestItem(item, gainItem))
                {
                    itemCollected = true;
                    Report.ItemChanged(item, item.Count, 2, "CheckGroupQuestItem (WinQuestItem)");
                }
            }

            return itemCollected;
        }

        public bool CheckNeedQuestItem(UserItem item, bool gainItem = true)
        {
            foreach (QuestProgressInfo quest in CurrentQuests.
                Where(e => e.ItemTaskCount.Count > 0).
                Where(e => e.NeedItem(item.Info)).
                Where(e => CanGainQuestItem(item)))
            {
                if (gainItem)
                {
                    GainQuestItem(item);
                    quest.ProcessItem(Info.QuestInventory);

                    SendPacketToClient(new ServerPacket.SendOutputMessage { Message = string.Format("You found {0}.", item.FriendlyName), Type = OutputMessageType.Quest });

                    SendUpdateQuest(quest, QuestState.Update);

                    Report.ItemChanged(item, item.Count, 2, "CheckNeedQuestItem (WinQuestItem)");
                }
                return true;
            }

            return false;
        }
        public bool CheckNeedQuestFlag(int flagNumber)
        {
            foreach (QuestProgressInfo quest in CurrentQuests.
                Where(e => e.FlagTaskSet.Count > 0).
                Where(e => e.NeedFlag(flagNumber)))
            {
                quest.ProcessFlag(Info.Flags);

                //Enqueue(new ServerPacket.SendOutputMessage { Message = string.Format("Location visited."), Type = OutputMessageType.Quest });

                SendUpdateQuest(quest, QuestState.Update);
                return true;
            }

            return false;
        }
        public void CheckNeedQuestKill(MonsterInfo mInfo)
        {
            foreach (QuestProgressInfo quest in CurrentQuests.
                    Where(e => e.KillTaskCount.Count > 0).
                    Where(quest => quest.NeedKill(mInfo)))
            {
                quest.ProcessKill(mInfo);

                SendPacketToClient(new ServerPacket.SendOutputMessage { Message = string.Format("You killed {0}.", mInfo.GameName), Type = OutputMessageType.Quest });

                SendUpdateQuest(quest, QuestState.Update);
            }
        }

        public void RecalculateQuestBag()
        {
            for (int i = Info.QuestInventory.Length - 1; i >= 0; i--)
            {
                UserItem itm = Info.QuestInventory[i];

                if (itm == null) continue;

                bool itemRequired = false;
                bool isCarryItem = false;

                foreach (QuestProgressInfo quest in CurrentQuests)
                {
                    foreach (QuestItemTask carryItem in quest.Info.CarryItems)
                    {
                        if (carryItem.Item == itm.Info)
                        {
                            isCarryItem = true;
                            break;
                        }
                    }

                    foreach (QuestItemTask task in quest.Info.ItemTasks)
                    {
                        if (task.Item == itm.Info)
                        {
                            itemRequired = true;
                            break;
                        }
                    }
                }

                if (!itemRequired && !isCarryItem)
                {
                    Info.QuestInventory[i] = null;
                    SendPacketToClient(new ServerPacket.DeleteQuestItem { UniqueID = itm.UniqueID, Count = itm.Count });
                }
            }
        }

        public void SendUpdateQuest(QuestProgressInfo quest, QuestState state, bool trackQuest = false)
        {
            quest.CheckCompleted();

            switch (state)
            {
                case QuestState.Add:
                    if (!CurrentQuests.Contains(quest))
                    {
                        CurrentQuests.Add(quest);
                    }
                    quest.SetTimer();
                    break;
                case QuestState.Remove:
                    if (CurrentQuests.Contains(quest))
                    {
                        CurrentQuests.Remove(quest);
                    }
                    quest.RemoveTimer();
                    break;
            }

            SendPacketToClient(new ServerPacket.ChangeQuest
            {
                Quest = quest.CreateClientQuestProgress(),
                QuestState = state,
                TrackQuest = trackQuest
            });
        }

        public void GetCompletedQuests()
        {
            SendPacketToClient(new ServerPacket.CompleteQuest
            {
                CompletedQuests = CompletedQuests
            });
        }

        #endregion

       
        #region Friends
        public void AddFriend(string name, bool blocked = false)
        {
            CharacterInfo info = Envir.GetCharacterInfo(name);

            if (info == null)
            {
                ReceiveChat("Player doesn't exist", ChatType.System);
                return;
            }

            if (Name == name)
            {
                ReceiveChat("Cannot add yourself", ChatType.System);
                return;
            }

            if (Info.Friends.Any(e => e.Index == info.Index))
            {
                ReceiveChat("Player already added", ChatType.System);
                return;
            }

            FriendInfo friend = new FriendInfo(info, blocked);

            Info.Friends.Add(friend);

            GetFriends();
        }

        public void RemoveFriend(int index)
        {
            FriendInfo friend = Info.Friends.FirstOrDefault(e => e.Index == index);

            if (friend == null)
            {
                return;
            }

            Info.Friends.Remove(friend);

            GetFriends();
        }
 

        public void GetFriends()
        {
            List<ClientFriend> friends = new List<ClientFriend>();

            foreach (FriendInfo friend in Info.Friends)
            {
                if (friend.Info != null)
                {
                    friends.Add(friend.CreateClientFriend());
                }
            }

            SendPacketToClient(new ServerPacket.FriendUpdate { Friends = friends });
        }

        #endregion

      

        #region Refining

        public void DepositRefineItem(int from, int to)
        {

            ServerPacket.DepositRefineItem p = new ServerPacket.DepositRefineItem { From = from, To = to, Success = false };

            if (NPCPage == null || !String.Equals(NPCPage.Key, NPCScript.RefineKey, StringComparison.CurrentCultureIgnoreCase))
            {
                SendPacketToClient(p);
                return;
            }
            NPCObjectSrv ob = null;
            for (int i = 0; i < CurrentMap.NPCs.Count; i++)
            {
                if (CurrentMap.NPCs[i].ObjectID != NPCObjectID) continue;
                ob = CurrentMap.NPCs[i];
                break;
            }

            if (ob == null || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Globals.DataRange))
            {
                SendPacketToClient(p);
                return;
            }


            if (from < 0 || from >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Info.Refine.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Info.Inventory[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (Info.Refine[to] == null)
            {
                Info.Refine[to] = temp;
                Info.Inventory[from] = null;
                RefreshBagWeight();

                Report.ItemMoved(temp, MirGridType.Inventory, MirGridType.Refine, from, to);

                p.Success = true;
                SendPacketToClient(p);
                return;
            }
            SendPacketToClient(p);

        }
        public void RetrieveRefineItem(int from, int to)
        {
            ServerPacket.RetrieveRefineItem p = new ServerPacket.RetrieveRefineItem { From = from, To = to, Success = false };

            if (from < 0 || from >= Info.Refine.Length)
            {
                SendPacketToClient(p);
                return;
            }

            if (to < 0 || to >= Info.Inventory.Length)
            {
                SendPacketToClient(p);
                return;
            }

            UserItem temp = Info.Refine[from];

            if (temp == null)
            {
                SendPacketToClient(p);
                return;
            }

            if (Info.Inventory[to] == null)
            {
                Info.Inventory[to] = temp;
                Info.Refine[from] = null;

                Report.ItemMoved(temp, MirGridType.Refine, MirGridType.Inventory, from, to);

                p.Success = true;
                RefreshBagWeight();
                SendPacketToClient(p);

                return;
            }
            SendPacketToClient(p);
        }
        public void RefineCancel()
        {
            for (int t = 0; t < Info.Refine.Length; t++)
            {
                UserItem temp = Info.Refine[t];

                if (temp == null) continue;

                for (int i = 0; i < Info.Inventory.Length; i++)
                {
                    if (Info.Inventory[i] != null) continue;

                    //Put item back in inventory
                    if (CanGainItem(temp))
                    {
                        RetrieveRefineItem(t, i);
                    }
                    else //Send item via mail if it can no longer be stored
                    {
                        SendPacketToClient(new ServerPacket.DeleteItem { UniqueID = temp.UniqueID, Count = temp.Count });
                         
                    }

                    Info.Refine[t] = null;

                    break;
                }
            }
        }
        public void RefineItem(ulong uniqueID)
        {
            SendPacketToClient(new ServerPacket.RepairItem { UniqueID = uniqueID }); //CHECK THIS.

            if (Dead) return;

            if (NPCPage == null || (!String.Equals(NPCPage.Key, NPCScript.RefineKey, StringComparison.CurrentCultureIgnoreCase))) return;

            int index = -1;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                if (Info.Inventory[i] == null || Info.Inventory[i].UniqueID != uniqueID) continue;
                index = i;
                break;
            }

            if (index == -1) return;

            if (Info.Inventory[index].RefineAdded != 0)
            {
                ReceiveChat(String.Format("Your {0} needs to be checked before you can attempt to refine it again.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }

            if ((Info.Inventory[index].Info.Type != ItemType.Weapon) && (Settings.OnlyRefineWeapon))
            {
                ReceiveChat(String.Format("Your {0} can't be refined.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }

            if (Info.Inventory[index].Info.Bind.HasFlag(BindMode.DontUpgrade))
            {
                ReceiveChat(String.Format("Your {0} can't be refined.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }

            if (Info.Inventory[index].RentalInformation != null && Info.Inventory[index].RentalInformation.BindingFlags.HasFlag(BindMode.DontUpgrade))
            {
                ReceiveChat(String.Format("Your {0} can't be refined.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }


            if (index == -1) return;




            //CHECK GOLD HERE
            uint cost = (uint)((Info.Inventory[index].Info.RequiredAmount * 10) * Settings.RefineCost);

            if (cost > Account.Gold)
            {
                ReceiveChat(String.Format("You don't have enough gold to refine your {0}.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }

            Account.Gold -= cost;
            SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });

            //START OF FORMULA

            Info.CurrentRefine = Info.Inventory[index];
            Info.Inventory[index] = null;
            Info.CollectTime = (Envir.Time + (Settings.RefineTime * Settings.Minute));
            SendPacketToClient(new ServerPacket.RefineItem { UniqueID = uniqueID });


            short orePurity = 0;
            byte oreAmount = 0;
            byte itemAmount = 0;
            short totalDC = 0;
            short totalMC = 0;
            short totalSC = 0;
            short requiredLevel = 0;
            short durability = 0;
            short currentDura = 0;
            short addedStats = 0;
            UserItem ingredient;

            for (int i = 0; i < Info.Refine.Length; i++)
            {
                ingredient = Info.Refine[i];

                if (ingredient == null) continue;
                if (ingredient.Info.Type == ItemType.Weapon)
                {
                    Info.Refine[i] = null;
                    continue;
                }

                if ((ingredient.Info.Stats[Stat.MaxDC] > 0) || (ingredient.Info.Stats[Stat.MaxMC] > 0) || (ingredient.Info.Stats[Stat.MaxSC] > 0))
                {
                    totalDC += (short)(ingredient.Info.Stats[Stat.MinDC] + ingredient.Info.Stats[Stat.MaxDC] + ingredient.AddedStats[Stat.MaxDC]);
                    totalMC += (short)(ingredient.Info.Stats[Stat.MinMC] + ingredient.Info.Stats[Stat.MaxMC] + ingredient.AddedStats[Stat.MaxMC]);
                    totalSC += (short)(ingredient.Info.Stats[Stat.MinSC] + ingredient.Info.Stats[Stat.MaxSC] + ingredient.AddedStats[Stat.MaxSC]);
                    requiredLevel += ingredient.Info.RequiredAmount;
                    if (Math.Floor(ingredient.MaxDura / 1000M) == Math.Floor(ingredient.Info.Durability / 1000M)) durability++;
                    if (Math.Floor(ingredient.CurrentDura / 1000M) == Math.Floor(ingredient.MaxDura / 1000M)) currentDura++;
                    itemAmount++;
                }

                if (ingredient.Info.FriendlyName == Settings.RefineOreName)
                {
                    orePurity += (short)Math.Floor(ingredient.CurrentDura / 1000M);
                    oreAmount++;
                }

                Info.Refine[i] = null;
            }

            if ((totalDC == 0) && (totalMC == 0) && (totalSC == 0))
            {
                Info.CurrentRefine.RefineSuccessChance = 0;
                //Info.CurrentRefine.RefinedValue = RefinedValue.None;
                Info.CurrentRefine.RefineAdded = Settings.RefineIncrease;

                if (Settings.RefineTime == 0)
                {
                    CollectRefine();
                }
                else
                {
                    ReceiveChat(String.Format("Your {0} is now being refined, please check back in {1} minute(s).", Info.CurrentRefine.FriendlyName, Settings.RefineTime), ChatType.System);
                }

                return;
            }

            if (oreAmount == 0)
            {
                Info.CurrentRefine.RefineSuccessChance = 0;
                //Info.CurrentRefine.RefinedValue = RefinedValue.None;
                Info.CurrentRefine.RefineAdded = Settings.RefineIncrease;
                if (Settings.RefineTime == 0)
                {
                    CollectRefine();
                }
                else
                {
                    ReceiveChat(String.Format("Your {0} is now being refined, please check back in {1} minute(s).", Info.CurrentRefine.FriendlyName, Settings.RefineTime), ChatType.System);
                }
                return;
            }


            short refineStat = 0;

            if ((totalDC > totalMC) && (totalDC > totalSC))
            {
                Info.CurrentRefine.RefinedValue = RefinedValue.DC;
                refineStat = totalDC;
            }

            if ((totalMC > totalDC) && (totalMC > totalSC))
            {
                Info.CurrentRefine.RefinedValue = RefinedValue.MC;
                refineStat = totalMC;
            }

            if ((totalSC > totalDC) && (totalSC > totalMC))
            {
                Info.CurrentRefine.RefinedValue = RefinedValue.SC;
                refineStat = totalSC;
            }

            Info.CurrentRefine.RefineAdded = Settings.RefineIncrease;


            int itemSuccess = 0; //Chance out of 35%

            itemSuccess += (refineStat * 5) - Info.CurrentRefine.Info.RequiredAmount;
            itemSuccess += 5;
            if (itemSuccess > 10) itemSuccess = 10;
            if (itemSuccess < 0) itemSuccess = 0; //10%


            if ((requiredLevel / itemAmount) > (Info.CurrentRefine.Info.RequiredAmount - 5)) itemSuccess += 10; //20%
            if (durability == itemAmount) itemSuccess += 10; //30%
            if (currentDura == itemAmount) itemSuccess += 5; //35%

            int oreSuccess = 0; //Chance out of 35%

            if (oreAmount >= itemAmount) oreSuccess += 15; //15%
            if ((orePurity / oreAmount) >= (refineStat / itemAmount)) oreSuccess += 15; //30%
            if (orePurity == refineStat) oreSuccess += 5; //35%

            int luckSuccess = (Info.CurrentRefine.AddedStats[Stat.Luck] + 5); //Chance out of 10%
            if (luckSuccess > 10) luckSuccess = 10;
            if (luckSuccess < 0) luckSuccess = 0;


            int baseSuccess = Settings.RefineBaseChance; //20% as standard

            int successChance = (itemSuccess + oreSuccess + luckSuccess + baseSuccess);

            addedStats = (byte)(Info.CurrentRefine.AddedStats[Stat.MaxDC] + Info.CurrentRefine.AddedStats[Stat.MaxMC] + Info.CurrentRefine.AddedStats[Stat.MaxSC]);
            if (Info.CurrentRefine.Info.Type == ItemType.Weapon) addedStats = (short)(addedStats * Settings.RefineWepStatReduce);
            else addedStats = (short)(addedStats * Settings.RefineItemStatReduce);
            if (addedStats > 50) addedStats = 50;

            successChance -= addedStats;

            Info.CurrentRefine.RefineSuccessChance = successChance;

            //END OF FORMULA

            if (Settings.RefineTime == 0)
            {
                CollectRefine();
            }
            else
            {
                ReceiveChat(String.Format("Your {0} is now being refined, please check back in {1} minute(s).", Info.CurrentRefine.FriendlyName, Settings.RefineTime), ChatType.System);
            }
        }
        public void CollectRefine()
        {
            ServerPacket.NPCCollectRefine p = new ServerPacket.NPCCollectRefine { Success = false };

            if (Info.CurrentRefine == null)
            {
                ReceiveChat("You aren't currently refining any items.", ChatType.System);
                SendPacketToClient(p);
                return;
            }

            if (Info.CollectTime > Envir.Time)
            {
                ReceiveChat(string.Format("Your {0} will be ready to collect in {1} minute(s).", Info.CurrentRefine.FriendlyName, ((Info.CollectTime - Envir.Time) / Settings.Minute)), ChatType.System);
                SendPacketToClient(p);
                return;
            }

            int index = -1;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                if (Info.Inventory[i] != null) continue;
                index = i;
                break;
            }

            if (index == -1)
            {
                ReceiveChat(String.Format("There isn't room in your bag for your {0}, make some space and try again.", Info.CurrentRefine.FriendlyName), ChatType.System);
                SendPacketToClient(p);
                return;
            }

            ReceiveChat(String.Format("Your item has been returned to you."), ChatType.System);
            p.Success = true;

            GainItem(Info.CurrentRefine);

            Info.CurrentRefine = null;
            Info.CollectTime = 0;
            SendPacketToClient(p);
        }
        public void CheckRefine(ulong uniqueID)
        {
            if (Dead) return;

            if (NPCPage == null || (!String.Equals(NPCPage.Key, NPCScript.RefineCheckKey, StringComparison.CurrentCultureIgnoreCase))) return;

            int index = -1;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                UserItem temp = Info.Inventory[i];
                if (temp == null || temp.UniqueID != uniqueID) continue;
                index = i;
                break;
            }

            if (index == -1) return;

            if (Info.Inventory[index].RefineAdded == 0)
            {
                ReceiveChat(String.Format("{0} doesn't need to be checked as it hasn't been refined yet.", Info.Inventory[index].FriendlyName), ChatType.System);
                return;
            }

            if (Envir.Random.Next(1, 100) > Info.Inventory[index].RefineSuccessChance)
            {
                Info.Inventory[index].RefinedValue = RefinedValue.None;
            }

            if (Envir.Random.Next(1, 100) < Settings.RefineCritChance)
            {
                Info.Inventory[index].RefineAdded = (byte)(Info.Inventory[index].RefineAdded * Settings.RefineCritIncrease);
            }

            if ((Info.Inventory[index].RefinedValue == RefinedValue.DC) && (Info.Inventory[index].RefineAdded > 0))
            {
                ReceiveChat(String.Format("Congratulations, your {0} now has +{1} extra DC.", Info.Inventory[index].FriendlyName, Info.Inventory[index].RefineAdded), ChatType.System);
                Info.Inventory[index].AddedStats[Stat.MaxDC] = (int)Math.Min(int.MaxValue, Info.Inventory[index].AddedStats[Stat.MaxDC] + Info.Inventory[index].RefineAdded);
                Info.Inventory[index].RefineAdded = 0;
                Info.Inventory[index].RefinedValue = RefinedValue.None;
                Info.Inventory[index].RefineSuccessChance = 0;

            }
            else if ((Info.Inventory[index].RefinedValue == RefinedValue.MC) && (Info.Inventory[index].RefineAdded > 0))
            {
                ReceiveChat(String.Format("Congratulations, your {0} now has +{1} extra MC.", Info.Inventory[index].FriendlyName, Info.Inventory[index].RefineAdded), ChatType.System);
                Info.Inventory[index].AddedStats[Stat.MaxMC] = (int)Math.Min(int.MaxValue, Info.Inventory[index].AddedStats[Stat.MaxMC] + Info.Inventory[index].RefineAdded);
                Info.Inventory[index].RefineAdded = 0;
                Info.Inventory[index].RefinedValue = RefinedValue.None;
                Info.Inventory[index].RefineSuccessChance = 0;

            }
            else if ((Info.Inventory[index].RefinedValue == RefinedValue.SC) && (Info.Inventory[index].RefineAdded > 0))
            {
                ReceiveChat(String.Format("Congratulations, your {0} now has +{1} extra SC.", Info.Inventory[index].FriendlyName, Info.Inventory[index].RefineAdded), ChatType.System);
                Info.Inventory[index].AddedStats[Stat.MaxSC] = (int)Math.Min(int.MaxValue, Info.Inventory[index].AddedStats[Stat.MaxSC] + Info.Inventory[index].RefineAdded);
                Info.Inventory[index].RefineAdded = 0;
                Info.Inventory[index].RefinedValue = RefinedValue.None;
                Info.Inventory[index].RefineSuccessChance = 0;
            }
            else if ((Info.Inventory[index].RefinedValue == RefinedValue.None) && (Info.Inventory[index].RefineAdded > 0))
            {
                ReceiveChat(String.Format("Your {0} smashed into a thousand pieces upon testing.", Info.Inventory[index].FriendlyName), ChatType.System);
                SendPacketToClient(new ServerPacket.RefineItem { UniqueID = Info.Inventory[index].UniqueID });
                Info.Inventory[index].RefineSuccessChance = 0;
                Info.Inventory[index] = null;
                return;
            }

            SendPacketToClient(new ServerPacket.ItemUpgraded { Item = Info.Inventory[index] });
            return;
        }

        #endregion

        #region Relationship

        public void NPCDivorce()
        {
            if (Info.Married == 0)
            {
                ReceiveChat(string.Format("You're not married."), ChatType.System);
                return;
            }

            CharacterInfo lover = Envir.GetCharacterInfo(Info.Married);
            PlayerObjectSrv player = Envir.GetPlayer(lover.Name);

            Info.Married = 0;
            Info.MarriedDate = Envir.Now;

            if (Info.Equipment[(int)EquipmentSlot.RingL] != null)
            {
                Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing = -1;
                SendPacketToClient(new ServerPacket.RefreshItem { Item = Info.Equipment[(int)EquipmentSlot.RingL] });
            }

            GetRelationship(false);
            
            lover.Married = 0;
            lover.MarriedDate = Envir.Now;
            if (lover.Equipment[(int)EquipmentSlot.RingL] != null)
                lover.Equipment[(int)EquipmentSlot.RingL].WeddingRing = -1;

            if (player != null)
            {
                player.GetRelationship(false);
                player.ReceiveChat(string.Format("You've just been forcefully divorced"), ChatType.System);
                if (player.Info.Equipment[(int)EquipmentSlot.RingL] != null)
                    player.SendPacketToClient(new ServerPacket.RefreshItem { Item = player.Info.Equipment[(int)EquipmentSlot.RingL] });
            }
        }

        public bool CheckMakeWeddingRing()
        {
            if (Info.Married == 0)
            {
                ReceiveChat(string.Format("You need to be married to make a Wedding Ring."), ChatType.System);
                return false;
            }

            if (Info.Equipment[(int)EquipmentSlot.RingL] == null)
            {
                ReceiveChat(string.Format("You need to wear a ring on your left finger to make a Wedding Ring."), ChatType.System);
                return false;
            }

            if (Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing != -1)
            {
                ReceiveChat(string.Format("You're already wearing a Wedding Ring."), ChatType.System);
                return false;
            }

            if (Info.Equipment[(int)EquipmentSlot.RingL].Info.Bind.HasFlag(BindMode.NoWeddingRing))
            {
                ReceiveChat(string.Format("You cannot use this type of ring."), ChatType.System);
                return false;
            }

            return true;
        }

        public void MakeWeddingRing()
        {
            if (CheckMakeWeddingRing())
            {
                Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing = Info.Married;
                SendPacketToClient(new ServerPacket.RefreshItem { Item = Info.Equipment[(int)EquipmentSlot.RingL] });
            }
        }

        public void ReplaceWeddingRing(ulong uniqueID)
        {
            if (Dead) return;

            if (NPCPage == null || (!String.Equals(NPCPage.Key, NPCScript.ReplaceWedRingKey, StringComparison.CurrentCultureIgnoreCase))) return;

            UserItem temp = null;
            UserItem CurrentRing = Info.Equipment[(int)EquipmentSlot.RingL];

            if (CurrentRing == null)
            {
                ReceiveChat(string.Format("You arn't wearing a  ring to upgrade."), ChatType.System);
                return;
            }

            if (CurrentRing.WeddingRing == -1)
            {
                ReceiveChat(string.Format("You arn't wearing a Wedding Ring to upgrade."), ChatType.System);
                return;
            }

            int index = -1;

            for (int i = 0; i < Info.Inventory.Length; i++)
            {
                temp = Info.Inventory[i];
                if (temp == null || temp.UniqueID != uniqueID) continue;
                index = i;
                break;
            }

            if (index == -1) return;

            temp = Info.Inventory[index];


            if (temp.Info.Type != ItemType.Ring)
            {
                ReceiveChat(string.Format("You can't replace a Wedding Ring with this item."), ChatType.System);
                return;
            }

            if (!CanEquipItem(temp, (int)EquipmentSlot.RingL))
            {
                ReceiveChat(string.Format("You can't equip the item you're trying to use."), ChatType.System);
                return;
            }

            if (temp.Info.Bind.HasFlag(BindMode.NoWeddingRing))
            {
                ReceiveChat(string.Format("You cannot use this type of ring."), ChatType.System);
                return;
            }

            uint cost = (uint)((Info.Inventory[index].Info.RequiredAmount * 10) * Settings.ReplaceWedRingCost);

            if (cost > Account.Gold)
            {
                ReceiveChat(String.Format("You don't have enough gold to replace your Wedding Ring."), ChatType.System);
                return;
            }

            Account.Gold -= cost;
            SendPacketToClient(new ServerPacket.LoseGold { Gold = cost });


            temp.WeddingRing = Info.Married;
            CurrentRing.WeddingRing = -1;

            Info.Equipment[(int)EquipmentSlot.RingL] = temp;
            Info.Inventory[index] = CurrentRing;

            SendPacketToClient(new ServerPacket.EquipItem { Grid = MirGridType.Inventory, UniqueID = temp.UniqueID, To = (int)EquipmentSlot.RingL, Success = true });

            SendPacketToClient(new ServerPacket.RefreshItem { Item = Info.Inventory[index] });
            SendPacketToClient(new ServerPacket.RefreshItem { Item = Info.Equipment[(int)EquipmentSlot.RingL] });

        }

        public void MarriageRequest()
        {

            #region  Marriage kor msg
            if (Info.Married != 0)
            {
                //ReceiveChat(string.Format("You're already married."), ChatType.System);
                ReceiveChat(string.Format("이미 결혼하셨군요."), ChatType.System);
                return;
            }

            if (Info.MarriedDate.AddDays(Settings.MarriageCooldown) > Envir.Now)
            {
                //ReceiveChat(string.Format("You can't get married again yet, there is a {0} day cooldown after a divorce.", Settings.MarriageCooldown), ChatType.System); 
                ReceiveChat(string.Format("아직 다시 결혼할 수는 없습니다. 이혼 후 {0}일의 쿨다운이 있습니다.", Settings.MarriageCooldown), ChatType.System); 
                return;
            }

            if (Info.Level < Settings.MarriageLevelRequired)
            {
                //ReceiveChat(string.Format("You need to be at least level {0} to get married.", Settings.MarriageLevelRequired), ChatType.System);
                ReceiveChat(string.Format("결혼하려면 레벨 {0} 이상이어야 합니다.", Settings.MarriageLevelRequired), ChatType.System);
                //결혼하려면 레벨 {0} 이상이어야 합니다.
                return;
            }
            #endregion 

            Point target = Functions.PointMove(CurrentLocation, Direction, 1); //바로 앞 1칸을 의미함
            Cell cell = CurrentMap.GetCell(target);
            PlayerObjectSrv player = null;

            if (cell.Objects == null || cell.Objects.Count < 1) return;

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObjectSrv ob = cell.Objects[i];
                if (ob.Race != ObjectType.Player) continue;

                player = Envir.GetPlayer(ob.Name);
            }

            if (player != null)
            {

                #region  Marriage kor msg
                if (!Functions.FacingEachOther(Direction, CurrentLocation, player.Direction, player.CurrentLocation))
                {
                    //ReceiveChat(string.Format("You need to be facing each other to perform a marriage."), ChatType.System);
                    ReceiveChat(string.Format("결혼을 하려면 서로 마주보고 있어야 합니다."), ChatType.System);
                    return;
                }

                if (player.Level < Settings.MarriageLevelRequired)
                {
                    //ReceiveChat(string.Format("Your lover needs to be at least level {0} to get married.", Settings.MarriageLevelRequired), ChatType.System);
                    ReceiveChat(string.Format("결혼하려면 연인이 {0}레벨 이상이어야 합니다.", Settings.MarriageLevelRequired), ChatType.System);
                    return;
                }

                if (player.Info.MarriedDate.AddDays(Settings.MarriageCooldown) > Envir.Now)
                {
                    //ReceiveChat(string.Format("{0} can't get married again yet, there is a {1} day cooldown after divorce", player.Name, Settings.MarriageCooldown), ChatType.System);
                    ReceiveChat(string.Format("{0}은 아직 다시 결혼할 수 없습니다. 이혼 후 {1}일의 쿨다운이 있습니다e", player.Name, Settings.MarriageCooldown), ChatType.System);
                    return;
                }

                if (!player.AllowMarriage)
                {
                    //ReceiveChat("The person you're trying to propose to isn't allowing marriage requests.", ChatType.System);
                    ReceiveChat("청혼하려는 사람이 결혼 요청을 허용하지 않습니다.", ChatType.System);
                    return;
                }

                if (player == this)
                {
                    //ReceiveChat("You cant marry yourself.", ChatType.System);
                    ReceiveChat("자신과 결혼할 수 없습니다.", ChatType.System);
                    return;
                }

                if (player.Dead || Dead)
                {
                    // ReceiveChat("You can't perform a marriage with a dead player.", ChatType.System);
                    ReceiveChat("사망한 플레이어와는 결혼을 할 수 없습니다.", ChatType.System);
                    return;
                }

                if (player.MarriageProposal != null)
                {
                    //ReceiveChat(string.Format("{0} already has a marriage invitation.", player.Info.Name), ChatType.System);
                    ReceiveChat(string.Format("{0}는 이미 청혼을 요청을 받있습니다.", player.Info.Name), ChatType.System);
                    return;
                }

                if (!Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) || player.CurrentMap != CurrentMap)
                {
                    //ReceiveChat(string.Format("{0} is not within marriage range.", player.Info.Name), ChatType.System);
                    ReceiveChat(string.Format("{0}은 결혼을 할수있는 거리에 있지 않습니다.", player.Info.Name), ChatType.System);
                    return;
                }

                if (player.Info.Married != 0)
                {
                    //ReceiveChat(string.Format("{0} is already married.", player.Info.Name), ChatType.System);
                    ReceiveChat(string.Format("{0}는 이미 결혼했습니다.", player.Info.Name), ChatType.System);
                    return;
                }

                player.MarriageProposal = this;
                player.SendPacketToClient(new ServerPacket.MarriageRequest { Name = Info.Name });
            }
            else
            {
                //ReceiveChat(string.Format("You need to be facing a player to request a marriage."), ChatType.System);
                ReceiveChat(string.Format("결혼을 신청하려면 플레이어와 대면해야 합니다."), ChatType.System);
                return;
            }
            #endregion  Marriage kor msg
        }

        public void MarriageReply(bool accept)
        {
            if (MarriageProposal == null || MarriageProposal.Info == null)
            {
                MarriageProposal = null;
                return;
            }

            if (!accept)
            {
                MarriageProposal.ReceiveChat(string.Format("{0} has refused to marry you.", Info.Name), ChatType.System);
                MarriageProposal = null;
                return;
            }

            if (Info.Married != 0)
            {
                ReceiveChat("You are already married.", ChatType.System);
                MarriageProposal = null;
                return;
            }

            if (MarriageProposal.Info.Married != 0)
            {
                ReceiveChat(string.Format("{0} is already married.", MarriageProposal.Info.Name), ChatType.System);
                MarriageProposal = null;
                return;
            }


            MarriageProposal.Info.Married = Info.Index;
            MarriageProposal.Info.MarriedDate = Envir.Now;

            Info.Married = MarriageProposal.Info.Index;
            Info.MarriedDate = Envir.Now;

            GetRelationship(false);
            MarriageProposal.GetRelationship(false);

            MarriageProposal.ReceiveChat(string.Format("Congratulations, you're now married to {0}.", Info.Name), ChatType.System);
            ReceiveChat(String.Format("Congratulations, you're now married to {0}.", MarriageProposal.Info.Name), ChatType.System);

            MarriageProposal = null;
        }

        public void DivorceRequest()
        {

            if (Info.Married == 0)
            {
                ReceiveChat(string.Format("You're not married."), ChatType.System);
                return;
            }


            Point target = Functions.PointMove(CurrentLocation, Direction, 1);
            Cell cell = CurrentMap.GetCell(target);
            PlayerObjectSrv player = null;

            if (cell.Objects == null || cell.Objects.Count < 1) return;

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObjectSrv ob = cell.Objects[i];
                if (ob.Race != ObjectType.Player) continue;

                player = Envir.GetPlayer(ob.Name);
            }

            if (player == null)
            {
                ReceiveChat(string.Format("You need to be facing your lover to divorce them."), ChatType.System);
                return;
            }

            if (player != null)
            {
                if (!Functions.FacingEachOther(Direction, CurrentLocation, player.Direction, player.CurrentLocation))
                {
                    ReceiveChat(string.Format("You need to be facing your lover to divorce them."), ChatType.System);
                    return;
                }

                if (player == this)
                {
                    ReceiveChat("You can't divorce yourself.", ChatType.System);
                    return;
                }

                if (player.Dead || Dead)
                {
                    ReceiveChat("You can't divorce a dead player.", ChatType.System); //GOT TO HERE, NEED TO KEEP WORKING ON IT.
                    return;
                }

                if (player.Info.Index != Info.Married)
                {
                    ReceiveChat(string.Format("You aren't married to {0}", player.Info.Name), ChatType.System);
                    return;
                }

                if (!Functions.InRange(player.CurrentLocation, CurrentLocation, Globals.DataRange) || player.CurrentMap != CurrentMap)
                {
                    ReceiveChat(string.Format("{0} is not within divorce range.", player.Info.Name), ChatType.System);
                    return;
                }

                player.DivorceProposal = this;
                player.SendPacketToClient(new ServerPacket.DivorceRequest { Name = Info.Name });
            }
            else
            {
                ReceiveChat(string.Format("You need to be facing your lover to divorce them."), ChatType.System);
                return;
            }
        }

        public void DivorceReply(bool accept)
        {
            if (DivorceProposal == null || DivorceProposal.Info == null)
            {
                DivorceProposal = null;
                return;
            }

            if (!accept)
            {
                DivorceProposal.ReceiveChat(string.Format("{0} has refused to divorce you.", Info.Name), ChatType.System);
                DivorceProposal = null;
                return;
            }

            if (Info.Married == 0)
            {
                ReceiveChat("You aren't married so you don't require a divorce.", ChatType.System);
                DivorceProposal = null;
                return;
            }

            DivorceProposal.Info.Married = 0;
            DivorceProposal.Info.MarriedDate = Envir.Now;
            if (DivorceProposal.Info.Equipment[(int)EquipmentSlot.RingL] != null)
            {
                DivorceProposal.Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing = -1;
                DivorceProposal.SendPacketToClient(new ServerPacket.RefreshItem { Item = DivorceProposal.Info.Equipment[(int)EquipmentSlot.RingL] });
            }

            Info.Married = 0;
            Info.MarriedDate = Envir.Now;
            if (Info.Equipment[(int)EquipmentSlot.RingL] != null)
            {
                Info.Equipment[(int)EquipmentSlot.RingL].WeddingRing = -1;
                SendPacketToClient(new ServerPacket.RefreshItem { Item = Info.Equipment[(int)EquipmentSlot.RingL] });
            }

            DivorceProposal.ReceiveChat(string.Format("You're now divorced", Info.Name), ChatType.System);
            ReceiveChat("You're now divorced", ChatType.System);

            GetRelationship(false);
            DivorceProposal.GetRelationship(false);
            DivorceProposal = null;
        }

        public void GetRelationship(bool CheckOnline = true)
        {
            if (Info.Married == 0)
            {
                SendPacketToClient(new ServerPacket.LoverUpdate { Name = "", Date = Info.MarriedDate, MapName = "", MarriedDays = 0 });
            }
            else
            {
                CharacterInfo Lover = Envir.GetCharacterInfo(Info.Married);

                PlayerObjectSrv player = Envir.GetPlayer(Lover.Name);

                if (player == null)
                    SendPacketToClient(new ServerPacket.LoverUpdate { Name = Lover.Name, Date = Info.MarriedDate, MapName = "", MarriedDays = (short)(Envir.Now - Info.MarriedDate).TotalDays });
                else
                {
                    SendPacketToClient(new ServerPacket.LoverUpdate { Name = Lover.Name, Date = Info.MarriedDate, MapName = player.CurrentMap.Info.Title, MarriedDays = (short)(Envir.Now - Info.MarriedDate).TotalDays });
                    if (CheckOnline)
                    {
                        player.GetRelationship(false);
                        player.ReceiveChat(String.Format("{0} has come online.", Info.Name), ChatType.System);
                    }
                }
            }
        }
        public void LogoutRelationship()
        {
            if (Info.Married == 0) return;
            CharacterInfo lover = Envir.GetCharacterInfo(Info.Married);

            if (lover == null)
            {
                MessageQueue.SendDebugMsg(Name + " is married but couldn't find marriage ID " + Info.Married);
                return;
            }

            PlayerObjectSrv player = Envir.GetPlayer(lover.Name);
            if (player != null)
            {
                player.SendPacketToClient(new ServerPacket.LoverUpdate { Name = Info.Name, Date = player.Info.MarriedDate, MapName = "", MarriedDays = (short)(Envir.Now - Info.MarriedDate).TotalDays });
                player.ReceiveChat(String.Format("{0} has gone offline.", Info.Name), ChatType.System);
            }
        }

        #endregion

        #region Mentorship

        public void MentorBreak(bool force = false)
        {
            if (Info.Mentor == 0)
            {
                ReceiveChat(GameLanguage.NoMentorship, ChatType.System);
                return;
            }

            CharacterInfo partner = Envir.GetCharacterInfo(Info.Mentor);
            PlayerObjectSrv partnerP = Envir.GetPlayer(partner.Name);

            if (force)
            {
                Info.MentorDate = Envir.Now.AddDays(Settings.MentorLength);
                ReceiveChat(String.Format("You now have a {0} day cooldown on starting a new Mentorship.", Settings.MentorLength), ChatType.System);
            }
            else
            {
                ReceiveChat("Your Mentorship has now expired.", ChatType.System);
            }

            if (Info.IsMentor)
            {
                if (partnerP != null)
                {
                    Info.MentorExp += partnerP.MenteeEXP;
                    partnerP.MenteeEXP = 0;
                }
            }
            else
            {
                if (partnerP != null)
                {
                    partner.MentorExp += MenteeEXP;
                    MenteeEXP = 0;
                }
            }

            Info.Mentor = 0;
            GetMentor(false);
           
            if (Info.IsMentor && Info.MentorExp > 0)
            {
                GainExp((uint)Info.MentorExp);
                Info.MentorExp = 0;
            }
            
            partner.Mentor = 0;
            
            if (partnerP != null)
            {
                partnerP.ReceiveChat("Your Mentorship has now expired.", ChatType.System);
                partnerP.GetMentor(false);
                if (partner.IsMentor && partner.MentorExp > 0)
                {
                    partnerP.GainExp((uint)partner.MentorExp);
                    Info.MentorExp = 0;
                }
            }
            else
            {
                if (partner.IsMentor && partner.MentorExp > 0)
                {
                    partner.Experience += partner.MentorExp;
                    partner.MentorExp = 0;
                }
            }

            Info.IsMentor = false;
            partner.IsMentor = false;
            Info.MentorExp = 0;
            partner.MentorExp = 0;
        }

        public void AddMentor(string Name)
        {
            if (Info.Mentor != 0)
            {
                ReceiveChat("You already have a Mentor.", ChatType.System);
                return;
            }

            if (Info.Name == Name)
            {
                ReceiveChat("You can't Mentor yourself.", ChatType.System);
                return;
            }

            if (Info.MentorDate > Envir.Now)
            {
                ReceiveChat("You can't start a new Mentorship yet.", ChatType.System);
                return;
            }

            PlayerObjectSrv mentor = Envir.GetPlayer(Name);

            if (mentor == null)
            {
                ReceiveChat(String.Format("Can't find anybody by the name {0}.", Name), ChatType.System);
            }
            else
            {
                mentor.MentorRequest = null;

                if (!mentor.AllowMentor)
                {
                    ReceiveChat(String.Format("{0} is not allowing Mentor requests.", mentor.Info.Name), ChatType.System);
                    return;
                }

                if (mentor.Info.MentorDate > Envir.Now)
                {
                    ReceiveChat(String.Format("{0} can't start another Mentorship yet.", mentor.Info.Name), ChatType.System);
                    return;
                }

                if (mentor.Info.Mentor != 0)
                {
                    ReceiveChat(String.Format("{0} is already a Mentor.", mentor.Info.Name), ChatType.System);
                    return;
                }

                if (Info.Class != mentor.Info.Class)
                {
                    ReceiveChat("You can only be mentored by someone of the same Class.", ChatType.System);
                    return;
                }
                if ((Info.Level + Settings.MentorLevelGap) > mentor.Level)
                {
                    ReceiveChat(String.Format("You can only be mentored by someone who at least {0} level(s) above you.", Settings.MentorLevelGap), ChatType.System);
                    return;
                }

                mentor.MentorRequest = this;
                mentor.SendPacketToClient(new ServerPacket.MentorRequest { Name = Info.Name, Level = Info.Level });
                ReceiveChat(String.Format("Request Sent."), ChatType.System);
            }

        }

        public void MentorReply(bool accept)
        {
            if (MentorRequest == null || MentorRequest.Info == null)
            {
                MentorRequest = null;
                return;
            }

            if (!accept)
            {
                MentorRequest.ReceiveChat(string.Format("{0} has refused to Mentor you.", Info.Name), ChatType.System);
                MentorRequest = null;
                return;
            }

            if (Info.Mentor != 0)
            {
                ReceiveChat("You already have a Student.", ChatType.System);
                return;
            }

            PlayerObjectSrv student = Envir.GetPlayer(MentorRequest.Info.Name);
            MentorRequest = null;

            if (student == null)
            {
                ReceiveChat(String.Format("{0} is no longer online.", student.Name), ChatType.System);
                return;
            }
            else
            {
                if (student.Info.Mentor != 0)
                {
                    ReceiveChat(String.Format("{0} already has a Mentor.", student.Info.Name), ChatType.System);
                    return;
                }
                if (Info.Class != student.Info.Class)
                {
                    ReceiveChat("You can only mentor someone of the same Class.", ChatType.System);
                    return;
                }
                if ((Info.Level - Settings.MentorLevelGap) < student.Level)
                {
                    ReceiveChat(String.Format("You can only mentor someone who at least {0} level(s) below you.", Settings.MentorLevelGap), ChatType.System);
                    return;
                }

                student.Info.Mentor = Info.Index;
                student.Info.IsMentor = false;
                Info.Mentor = student.Info.Index;
                Info.IsMentor = true;
                student.Info.MentorDate = Envir.Now;
                Info.MentorDate = Envir.Now;

                ReceiveChat(String.Format("You're now the Mentor of {0}.", student.Info.Name), ChatType.System);
                student.ReceiveChat(String.Format("You're now being Mentored by {0}.", Info.Name), ChatType.System);
                GetMentor(false);
                student.GetMentor(false);
            }
        }

        public void GetMentor(bool CheckOnline = true)
        {
            if (Info.Mentor == 0)
            {
                SendPacketToClient(new ServerPacket.MentorUpdate { Name = "", Level = 0, Online = false, MenteeEXP = 0 });
            }
            else
            {
                CharacterInfo mentor = Envir.GetCharacterInfo(Info.Mentor);

                PlayerObjectSrv player = Envir.GetPlayer(mentor.Name);

                SendPacketToClient(new ServerPacket.MentorUpdate { Name = mentor.Name, Level = mentor.Level, Online = player != null, MenteeEXP = Info.MentorExp });

                if (player != null && CheckOnline)
                {
                    player.GetMentor(false);
                    player.ReceiveChat(String.Format("{0} has come online.", Info.Name), ChatType.System);
                }
            }
        }

        public void LogoutMentor()
        {
            if (Info.Mentor == 0) return;

            CharacterInfo mentor = Envir.GetCharacterInfo(Info.Mentor);

            if (mentor == null)
            {
                MessageQueue.SendDebugMsg(Name + " is mentored but couldn't find mentor ID " + Info.Mentor);
                return;
            }

            PlayerObjectSrv player = Envir.GetPlayer(mentor.Name);

            if (!Info.IsMentor)
            {
                mentor.MentorExp += MenteeEXP;
            }

            if (player != null)
            {
                player.SendPacketToClient(new ServerPacket.MentorUpdate { Name = Info.Name, Level = Info.Level, Online = false, MenteeEXP = mentor.MentorExp });
                player.ReceiveChat(String.Format("{0} has gone offline.", Info.Name), ChatType.System);
            }
        }

        #endregion

        #region Gameshop

        public void GameShopStock(GameShopItem item)
        {
            int purchased;
            int StockLevel;

            if (item.iStock) //Invididual Stock
            {
                Info.GSpurchases.TryGetValue(item.Info.Index, out purchased);
            }
            else //Server Stock
            {
                Envir.GameshopLog.TryGetValue(item.Info.Index, out purchased);
            }

            if (item.Stock - purchased >= 0)
            {
                StockLevel = item.Stock - purchased;
                SendPacketToClient(new ServerPacket.GameShopStock { GIndex = item.Info.Index, StockLevel = StockLevel });
            }
              
        }

        public void GameshopBuy(int GIndex, byte Quantity)
        {
            if (Quantity < 1 || Quantity > 99) return;

            List<GameShopItem> shopList = Envir.GameShopList;
            GameShopItem Product = null;

            int purchased;
            bool stockAvailable = false;
            bool canAfford = false;
            uint CreditCost = 0;
            uint GoldCost = 0;

            List<UserItem> mailItems = new List<UserItem>();

            for (int i = 0; i < shopList.Count; i++)
            {
                if (shopList[i].GIndex == GIndex)
                {
                    Product = shopList[i];
                    break;
                }
            }

            if (Product == null)
            {
                ReceiveChat("You're trying to buy an item that isn't in the shop.", ChatType.System);
                MessageQueue.SendDebugMsg(Info.Name + " is trying to buy Something that doesn't exist.");
                return;
            }

            if (((decimal)(Quantity * Product.Count) / Product.Info.StackSize) > 5) return;

            if (Product.Stock != 0)
            {

                if (Product.iStock) //Invididual Stock
                {
                    Info.GSpurchases.TryGetValue(Product.Info.Index, out purchased);
                }
                else //Server Stock
                {
                    Envir.GameshopLog.TryGetValue(Product.Info.Index, out purchased);
                }

                if (Product.Stock - purchased - Quantity >= 0)
                {
                    stockAvailable = true;
                }
                else
                {
                    ReceiveChat("You're trying to buy more of this item than is available.", ChatType.System);
                    GameShopStock(Product);
                    MessageQueue.SendDebugMsg(Info.Name + " is trying to buy " + Product.Info.FriendlyName + " x " + Quantity + " - Stock isn't available.");
                    return;
                }
            }
            else
            {
                stockAvailable = true;
            }

            if (stockAvailable)
            {
                MessageQueue.SendDebugMsg(Info.Name + " is trying to buy " + Product.Info.FriendlyName + " x " + Quantity + " - Stock is available");
                
                var cost = Product.CreditPrice * Quantity;
                if (cost < Account.Credit || cost == 0)
                {
                    canAfford = true;
                    CreditCost = cost;
                }
                else
                {
                    //Needs to attempt to pay with gold and credits
                    var totalCost = ((Product.GoldPrice * Quantity) / cost) * (cost - Account.Credit);
                    if (Account.Gold >= totalCost)
                    {
                        GoldCost = totalCost;
                        CreditCost = Account.Credit;
                        canAfford = true;
                    }
                    else
                    {
                        ReceiveChat("You don't have enough currency for your purchase.", ChatType.System);
                        MessageQueue.SendDebugMsg(Info.Name + " is trying to buy " + Product.Info.FriendlyName + " x " + Quantity + " - not enough currency.");
                        return;
                    }
                }
            }
            else
            {
                return;
            }

            if (canAfford)
            {
                MessageQueue.SendDebugMsg(Info.Name + " is trying to buy " + Product.Info.FriendlyName + " x " + Quantity + " - Has enough currency.");
                Account.Gold -= GoldCost;
                Account.Credit -= CreditCost;

                Report.GoldChanged(GoldCost, true, Product.Info.FriendlyName);
                Report.CreditChanged(CreditCost, true, Product.Info.FriendlyName);

                if (GoldCost != 0) SendPacketToClient(new ServerPacket.LoseGold { Gold = GoldCost });
                if (CreditCost != 0) SendPacketToClient(new ServerPacket.LoseCredit { Credit = CreditCost });

                if (Product.iStock && Product.Stock != 0)
                {
                    Info.GSpurchases.TryGetValue(Product.Info.Index, out purchased);
                    if (purchased == 0)
                    {
                        Info.GSpurchases[Product.GIndex] = Quantity;
                    }
                    else
                    {
                        Info.GSpurchases[Product.GIndex] += Quantity;
                    }
                }

                Envir.GameshopLog.TryGetValue(Product.Info.Index, out purchased);
                if (purchased == 0)
                {
                    Envir.GameshopLog[Product.GIndex] = Quantity;
                }
                else
                {
                    Envir.GameshopLog[Product.GIndex] += Quantity;
                }

                if (Product.Stock != 0) GameShopStock(Product);
            }
            else
            {
                return;
            }

            Report.ItemGSBought(Product, Quantity, CreditCost, GoldCost);

            ushort quantity = (ushort)(Quantity * Product.Count);

            if (Product.Info.StackSize <= 1 || quantity == 1)
            {
                for (int i = 0; i < Quantity; i++)
                {
                    UserItem mailItem = Envir.CreateFreshItem(Envir.GetItemInfo(Product.Info.Index));

                    mailItems.Add(mailItem);
                }
            }
            else
            {
                while (quantity > 0)
                {
                    UserItem mailItem = Envir.CreateFreshItem(Envir.GetItemInfo(Product.Info.Index));
                    mailItem.Count = 0;
                    for (int i = 0; i < mailItem.Info.StackSize; i++)
                    {
                        mailItem.Count++;
                        quantity--;
                        if (quantity == 0) break;
                    }
                    if (mailItem.Count == 0) break;

                    mailItems.Add(mailItem);

                }
            }

            

            MessageQueue.SendDebugMsg(Info.Name + " is trying to buy " + Product.Info.FriendlyName + " x " + Quantity + " - Purchases Sent!");
            ReceiveChat("Your purchases have been sent to your Mailbox.", ChatType.Hint);
        }

        public void GetGameShop()
        {
            int purchased;
            int stockLevel;

            for (int i = 0; i < Envir.GameShopList.Count; i++)
            {
                var item = Envir.GameShopList[i];

                if (item.Stock != 0)
                {
                    if (item.iStock) //Individual Stock
                    {
                        Info.GSpurchases.TryGetValue(item.Info.Index, out purchased);
                    }
                    else //Server Stock
                    {
                        Envir.GameshopLog.TryGetValue(item.Info.Index, out purchased);
                    }

                    if (item.Stock - purchased >= 0)
                    {
                        stockLevel = item.Stock - purchased;
                        SendPacketToClient(new ServerPacket.GameShopInfo { Item = item, StockLevel = stockLevel });
                    }
                }
                else
                {
                    SendPacketToClient(new ServerPacket.GameShopInfo { Item = item, StockLevel = item.Stock });
                }  
            }
        }

        #endregion

        #region ConquestWall
        public void CheckConquest(bool checkPalace = false)
        {
            if (CurrentMap.tempConquest == null && CurrentMap.Conquest != null)
            {
                ConquestObjectSrv swi = CurrentMap.GetConquest(CurrentLocation);
                if (swi != null)
                    EnterSabuk();
                else
                    LeaveSabuk();
            }
            else if (CurrentMap.tempConquest != null)
            {
                if (checkPalace && CurrentMap.Info.Index == CurrentMap.tempConquest.PalaceMap.Info.Index && CurrentMap.tempConquest.GameType == ConquestGame.CapturePalace)
                    CurrentMap.tempConquest.TakeConquest(this);

                EnterSabuk();
            }
        }
        public void EnterSabuk()
        {
            if (WarZone) return;
            WarZone = true;
            RefreshNameColour();
        }

        public void LeaveSabuk()
        {
            if (!WarZone) return;
            WarZone = false;
            RefreshNameColour();
        }
        #endregion

     
        public Server.ExineEnvir.Timer GetTimer(string key)
        {
            var timerKey = Name + "-" + key;

            if (Envir.Timers.ContainsKey(timerKey))
            {
                return Envir.Timers[timerKey];
            }

            return null;
        }        
        public void SetTimer(string key, int seconds, byte type = 0)
        {
            if (seconds < 0) seconds = 0;

            var timerKey = Name + "-" + key;

            Timer t = new Timer(timerKey, seconds, type);

            Envir.Timers[timerKey] = t;

            SendPacketToClient(new ServerPacket.SetTimer { Key = t.Key, Seconds = t.Seconds, Type = t.Type });
        }
        public void ExpireTimer(string key)
        {
            var timerKey = Name + "-" + key;

            if (Envir.Timers.ContainsKey(timerKey))
            {
                Envir.Timers.Remove(timerKey);
            }

            SendPacketToClient(new ServerPacket.ExpireTimer { Key = timerKey });
        }
        public void SetCompass(Point location)
        {
            SendPacketToClient(new ServerPacket.SetCompass { Location = location });
        } 
         
    }
}
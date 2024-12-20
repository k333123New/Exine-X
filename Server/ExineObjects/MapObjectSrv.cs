﻿using Server.ExineDatabase;
using Server.ExineEnvir;
using Server.ExineObjects.Monsters;


namespace Server.ExineObjects
{
    public abstract class MapObjectSrv
    {
        protected static MessageQueue MessageQueue
        {
            get { return MessageQueue.Instance; }
        }

        protected static Envir Envir
        {
            get { return Envir.Main; }
        }

        public readonly uint ObjectID = Envir.ObjectID;

        public abstract ObjectType Race { get; }

        public abstract string Name { get; set; }

        public long ExplosionInflictedTime;
        public int ExplosionInflictedStage;

        private int SpawnThread;

        private Map _currentMap;
        public Map CurrentMap
        {
            set
            {
                _currentMap = value;
                CurrentMapIndex = _currentMap != null ? _currentMap.Info.Index : 0;
            }
            get { return _currentMap; }
        }

        public abstract int CurrentMapIndex { get; set; }
        public abstract Point CurrentLocation { get; set; }
        public abstract ExineDirection Direction { get; set; }

        public abstract ushort Level { get; set; }

        public abstract int Health { get; }
        public abstract int MaxHealth { get; }
        public byte PercentHealth
        {
            get { return (byte)(Health / (float)MaxHealth * 100); }
        }

        public byte Light;
        public int AttackSpeed;

        protected long brownTime;
        public virtual long BrownTime
        {
            get { return brownTime; }
            set { brownTime = value; }
        }

        public long CellTime, PKPointTime, LastHitTime, EXPOwnerTime;
        public Color NameColour = Color.White; 

        public bool Dead, Undead, Harvested, AutoRev;

        public List<KeyValuePair<string, string>> NPCVar = new List<KeyValuePair<string, string>>();

        public virtual int PKPoints { get; set; }

        public ushort PotHealthAmount, PotManaAmount, HealAmount, VampAmount;

        public bool CoolEye;
        private bool _hidden;

        public bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                if (_hidden == value) return;
                _hidden = value;
                CurrentMap.Broadcast(new ServerPacket.ObjectHidden { ObjectID = ObjectID, Hidden = value }, CurrentLocation);
            }
        }

        private bool _observer;
        public bool Observer
        {
            get
            {
                return _observer;
            }
            set
            {
                if (_observer == value) return;
                _observer = value;
                if (!_observer)
                    BroadcastInfo();
                else
                    Broadcast(new ServerPacket.ObjectRemove { ObjectID = ObjectID });
            }
        }

        
        public MapObjectSrv _target;
        public virtual MapObjectSrv Target
        {
            get { return _target; }
            set
            {
                if (_target == value) return;
                _target = value;
            }

        }

        protected MapObjectSrv master;
        public virtual MapObjectSrv Master
        {
            get { return master; }
            set { master = value; }
        }

        public MapObjectSrv LastHitter, EXPOwner, Owner;
        public long ExpireTime, OwnerTime, OperateTime;
        public int OperateDelay = 100;

        public Stats Stats;

        public virtual List<Buff> Buffs { get; set; } = new List<Buff>();

        public List<PlayerObjectSrv> GroupMembers;

        public virtual AttackMode AMode { get; set; }
        public virtual PetMode PMode { get; set; }

        private bool _inSafeZone;
        public bool InSafeZone {
            get { return _inSafeZone; }
            set
            {
                if (_inSafeZone == value) return;
                _inSafeZone = value;
                OnSafeZoneChanged();
            }
        }

        public float ArmourRate, DamageRate; //recieved not given

        public virtual List<Poison> PoisonList { get; set; } = new List<Poison>();
        public PoisonType CurrentPoison = PoisonType.None;
        public List<DelayedAction> ActionList = new List<DelayedAction>();

        public LinkedListNode<MapObjectSrv> Node;
        public LinkedListNode<MapObjectSrv> NodeThreaded;
        public long RevTime;

        public virtual bool Blocking
        {
            get { return true; }
        }

        public Point Front
        {
            get { return Functions.PointMove(CurrentLocation, Direction, 1); }
        }

        public Point Back
        {
            get { return Functions.PointMove(CurrentLocation, Direction, -1); }

        }

        public virtual void Process()
        {
            if (Master != null && Master.Node == null) Master = null;
            if (LastHitter != null && LastHitter.Node == null) LastHitter = null;
            if (EXPOwner != null && EXPOwner.Node == null) EXPOwner = null;
            if (Target != null && (Target.Node == null || Target.Dead)) Target = null;
            if (Owner != null && Owner.Node == null) Owner = null;

            if (PKPoints > 0 && Envir.Time > PKPointTime)
            {
                PKPointTime = Envir.Time + Settings.PKDelay * Settings.Second;
                PKPoints--;
            }

            if (LastHitter != null && Envir.Time > LastHitTime)
            {
                LastHitter = null;
            }

            if (EXPOwner != null && Envir.Time > EXPOwnerTime)
            {
                EXPOwner = null;
            }

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (Envir.Time < ActionList[i].Time) continue;
                Process(ActionList[i]);
                ActionList.RemoveAt(i);
            }
        }

        public virtual void OnSafeZoneChanged()
        {

        }

        public abstract void SetOperateTime();

        public int GetAttackPower(int min, int max)
        {
            if (min < 0) min = 0;
            if (min > max) max = min;

            if (Stats[Stat.Luck] > 0)
            {
                if (Stats[Stat.Luck] > Envir.Random.Next(Settings.MaxLuck))
                    return max;
            }
            else if (Stats[Stat.Luck] < 0)
            {
                if (Stats[Stat.Luck] < -Envir.Random.Next(Settings.MaxLuck))
                    return min;
            }

            return Envir.Random.Next(min, max + 1);
        }

        public int GetRangeAttackPower(int min, int max, int range)
        {
            //maxRange = highest possible damage
            //minRange = lowest possible damage

            decimal x = ((decimal)min / (Globals.MaxAttackRange)) * (Globals.MaxAttackRange - range);

            min -= (int)Math.Floor(x);

            return GetAttackPower(min, max);
        }

        public int GetDefencePower(int min, int max)
        {
            if (min < 0) min = 0;
            if (min > max) max = min;

            return Envir.Random.Next(min, max + 1);
        }

        public virtual void Remove(HumanObjectSrv player)
        {
            player.SendPacketToClient(new ServerPacket.ObjectRemove { ObjectID = ObjectID });
        }
        public virtual void Add(HumanObjectSrv player)
        {
            if (player.Race != ObjectType.Player) return;

            if (Race == ObjectType.Merchant)
            {
                NPCObjectSrv npc = (NPCObjectSrv)this;
                npc.CheckVisible((PlayerObjectSrv)player, true);
                return;
            }

            player.SendPacketToClient(GetInfo());

            //if (Race == ObjectType.Player)
            //{
            //    PlayerObject me = (PlayerObject)this;
            //    player.Enqueue(me.GetInfoEx(player));
            //}
            //else
            //{
            //    player.Enqueue(GetInfo());
            //}
        }
        public virtual void Remove(MonsterObjectSrv monster)
        {

        }
        public virtual void Add(MonsterObjectSrv monster)
        {

        }

        public abstract void Process(DelayedAction action);


        public bool CanFly(Point target)
        {
            Point location = CurrentLocation;

            while (location != target)
            {
                ExineDirection dir = Functions.DirectionFromPoint(location, target);

                location = Functions.PointMove(location, dir, 1);

                if (location.X < 0 || location.Y < 0 || location.X >= CurrentMap.Width || location.Y >= CurrentMap.Height) return false;

                if (!CurrentMap.GetCell(location).Valid) return false;
            }

            return true;
        }

        public virtual void Spawned()
        {
            Node = Envir.Objects.AddLast(this);
            if ((Race == ObjectType.Monster) && Settings.Multithreaded)
            {
                SpawnThread = CurrentMap.Thread;
                NodeThreaded = Envir.MobThreads[SpawnThread].ObjectsList.AddLast(this);
            }

            OperateTime = Envir.Time + Envir.Random.Next(OperateDelay);

            InSafeZone = CurrentMap != null && CurrentMap.GetSafeZone(CurrentLocation) != null;
            BroadcastInfo();
            BroadcastHealthChange();
        }
        public virtual void Despawn()
        {
            if (Node == null) return;
            
            Broadcast(new ServerPacket.ObjectRemove { ObjectID = ObjectID });
            Envir.Objects.Remove(Node);
            if (Settings.Multithreaded && (Race == ObjectType.Monster))
            {
                Envir.MobThreads[SpawnThread].ObjectsList.Remove(NodeThreaded);
            }

            ActionList.Clear();


            Node = null;
        }

        public MapObjectSrv FindObject(uint targetID, int dist)
        {
            for (int d = 0; d <= dist; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (!cell.Valid || cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObjectSrv ob = cell.Objects[i];
                            if (ob.ObjectID != targetID) continue;

                            return ob;
                        }
                    }
                }
            }
            return null;
        }

        public virtual void Broadcast(Packet p)
        {
            if (p == null || CurrentMap == null) return;

            for (int i = CurrentMap.Players.Count - 1; i >= 0; i--)
            {
                PlayerObjectSrv player = CurrentMap.Players[i];
                if (player == this) continue;

                if (Functions.InRange(CurrentLocation, player.CurrentLocation, Globals.DataRange))
                    player.SendPacketToClient(p);
            }
        }

        public virtual void BroadcastInfo()
        {
            Broadcast(GetInfo());
            return;
        } 

        public bool IsAttackTarget(MapObjectSrv attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead || InSafeZone || attacker.InSafeZone || attacker == this) return false;
            
            switch (attacker.Race)
            {
                case ObjectType.Player:
                    return IsAttackTarget((PlayerObjectSrv)attacker);
                case ObjectType.Monster:
                    return IsAttackTarget((MonsterObjectSrv)attacker);
                default:
                    throw new NotSupportedException();
            }
        }

        public abstract bool IsAttackTarget(HumanObjectSrv attacker);
        public abstract bool IsAttackTarget(MonsterObjectSrv attacker);
        public abstract int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true);
        public abstract int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility);

        public virtual int GetArmour(DefenceType type, MapObjectSrv attacker, out bool hit)
        {
            var armour = 0;
            hit = true;
            switch (type)
            {
                case DefenceType.ACAgility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy])
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        hit = false;
                    }
                    armour = GetDefencePower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.AC:
                    armour = GetDefencePower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.MACAgility:
                    if (Envir.Random.Next(Settings.MagicResistWeight) < Stats[Stat.MagicResist])
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        hit = false;
                    }
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy])
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        hit = false;
                    }
                    armour = GetDefencePower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.MAC:
                    if (Envir.Random.Next(Settings.MagicResistWeight) < Stats[Stat.MagicResist])
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        hit = false;
                    }
                    armour = GetDefencePower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.Agility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy])
                    {
                        BroadcastDamageIndicator(DamageType.Miss);
                        hit = false;
                    }
                    break;
            }
            return armour;
        }

        public virtual void ApplyNegativeEffects(HumanObjectSrv attacker, DefenceType type, ushort levelOffset)
        {
            if (attacker.SpecialMode.HasFlag(SpecialItemMode.Paralize) && type != DefenceType.MAC && type != DefenceType.MACAgility && 1 == Envir.Random.Next(1, 15))
            {
                ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = 5, TickSpeed = 1000 }, attacker);
            }
            if ((attacker.Stats[Stat.Freezing] > 0) && (Settings.PvpCanFreeze || Race != ObjectType.Player) && type != DefenceType.MAC && type != DefenceType.MACAgility)
            {
                if ((Envir.Random.Next(Settings.FreezingAttackWeight) < attacker.Stats[Stat.Freezing]) && (Envir.Random.Next(levelOffset) == 0))
                    ApplyPoison(new Poison { PType = PoisonType.Slow, Duration = Math.Min(10, (3 + Envir.Random.Next(attacker.Stats[Stat.Freezing]))), TickSpeed = 1000 }, attacker);
            }
            if (attacker.Stats[Stat.PoisonAttack] > 0 && type != DefenceType.MAC && type != DefenceType.MACAgility)
            {
                if ((Envir.Random.Next(Settings.PoisonAttackWeight) < attacker.Stats[Stat.PoisonAttack]) && (Envir.Random.Next(levelOffset) == 0))
                    ApplyPoison(new Poison { PType = PoisonType.Green, Duration = 5, TickSpeed = 1000, Value = Math.Min(10, 3 + Envir.Random.Next(attacker.Stats[Stat.PoisonAttack])) }, attacker);
            }
        }

        public abstract int Struck(int damage, DefenceType type = DefenceType.ACAgility);

        public bool IsFriendlyTarget(MapObjectSrv ally)
        {
            switch (ally.Race)
            {
                case ObjectType.Player:
                    return IsFriendlyTarget((PlayerObjectSrv)ally);
                case ObjectType.Monster:
                    return IsFriendlyTarget((MonsterObjectSrv)ally);
                default:
                    throw new NotSupportedException();
            }
        }

        public abstract bool IsFriendlyTarget(HumanObjectSrv ally);
        public abstract bool IsFriendlyTarget(MonsterObjectSrv ally);

        public abstract void ReceiveChat(string text, ChatType type);

        public abstract Packet GetInfo();

        public virtual void WinExp(uint amount, uint targetLevel = 0)
        {


        }

        public virtual bool CanGainGold(uint gold)
        {
            return false;
        }
        public virtual void WinGold(uint gold)
        {

        }

        public virtual bool Harvest(PlayerObjectSrv player) { return false; }

        public abstract void ApplyPoison(Poison p, MapObjectSrv Caster = null, bool NoResist = false, bool ignoreDefence = true);

        public virtual Buff AddBuff(BuffType type, MapObjectSrv owner, int duration, Stats stats, bool refreshStats = true, bool updateOnly = false, params int[] values)
        {
            if (!HasBuff(type, out Buff buff))
            {
                buff = new Buff(type)
                {
                    Caster = owner,
                    ObjectID = ObjectID,
                    ExpireTime = duration,
                    LastTime = Envir.Time,
                    Stats = stats
                };

                Buffs.Add(buff);
            }
            else
            {
                if (!updateOnly)
                {
                    switch (buff.StackType)
                    {
                        case BuffStackType.ResetDuration:
                            {
                                buff.ExpireTime = duration;
                            }
                            break;
                        case BuffStackType.StackDuration:
                            {
                                buff.ExpireTime += duration;
                            }
                            break;
                        case BuffStackType.StackStat:
                            {
                                if (stats != null)
                                {
                                    buff.Stats.Add(stats);
                                }
                            }
                            break;
                        case BuffStackType.StackStatAndDuration:
                            {
                                if (stats != null)
                                {
                                    buff.Stats.Add(stats);
                                }

                                buff.ExpireTime += duration;
                            }
                            break;
                        case BuffStackType.ResetStat:
                        {
                            if (stats != null)
                            {
                                buff.Stats = stats;
                            }
                        }
                            break;
                        case BuffStackType.ResetStatAndDuration:
                        {
                            buff.ExpireTime = duration;
                            if (stats != null)
                            {
                                buff.Stats = stats;
                            }
                        }
                            break;
                        case BuffStackType.Infinite:
                        case BuffStackType.None:
                            break;
                    }
                }
            }

            if (buff.Properties.HasFlag(BuffProperty.PauseInSafeZone) && InSafeZone)
            {
                buff.Paused = true;
            }

            buff.Stats ??= new Stats();
            buff.Values = values ?? new int[0];

            if (buff.Caster?.Node == null)
                buff.Caster = owner;

            switch (buff.Type)
            {
                case BuffType.MoonLight:
                case BuffType.DarkBody:
                    Hidden = true;
                    
                    HideFromTargets();
                    break;
                case BuffType.Hiding:
                case BuffType.ClearRing:
                    Hidden = true;
                    HideFromTargets();
                    break;
            }

            return buff;
        }

        public virtual void RemoveBuff(BuffType b)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Type != b) continue;

                Buffs[i].FlagForRemoval = true;
                Buffs[i].Paused = false;
                Buffs[i].ExpireTime = 0;

                switch(b)
                {
                    case BuffType.Hiding:
                    case BuffType.MoonLight:
                    case BuffType.DarkBody:
                        if (!HasAnyBuffs(b, BuffType.ClearRing, BuffType.Hiding, BuffType.MoonLight, BuffType.DarkBody))
                        {
                            Hidden = false;
                        }
                        break;
                }
            }
        }
        public bool HasBuff(BuffType type)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Type != type) continue;
                return true;
            }
            return false;
        }
        public bool HasBuff(BuffType type, out Buff buff)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Type != type) continue;

                buff = Buffs[i];
                return true;
            }

            buff = null;
            return false;
        }

        public bool HasAnyBuffs(BuffType exceptBuff, params BuffType[] types)
        {
            return Buffs.Select(x => x.Type).Except(new List<BuffType> { exceptBuff }).Intersect(types).Any();
        }

        public virtual void PauseBuff(Buff b)
        {
            if (b.Paused) return;

            b.Paused = true;
        }

        public virtual void UnpauseBuff(Buff b)
        {
            if (!b.Paused) return;

            b.Paused = false;
        }

        protected void HideFromTargets()
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
                        if (ob.Race != ObjectType.Monster) continue;

                        if (ob.Target == this && (!ob.CoolEye || ob.Level < Level)) ob.Target = null;
                    }
                }
            }
        }

        public bool CheckStacked()
        {
            Cell cell = CurrentMap.GetCell(CurrentLocation);

            if (cell.Objects != null)
                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    MapObjectSrv ob = cell.Objects[i];
                    if (ob == this || !ob.Blocking) continue;
                    return true;
                }

            return false;
        }

        public virtual bool Teleport(Map temp, Point location, bool effects = true, byte effectnumber = 0)
        {
            if (temp == null || !temp.ValidPoint(location)) return false;

            CurrentMap.RemoveObject(this);
            if (effects) Broadcast(new ServerPacket.ObjectTeleportOut {ObjectID = ObjectID, Type = effectnumber});
            Broadcast(new ServerPacket.ObjectRemove {ObjectID = ObjectID});
            
            CurrentMap = temp;
            CurrentLocation = location;

            InTrapRock = false;

            CurrentMap.AddObject(this);
            BroadcastInfo();

            if (effects) Broadcast(new ServerPacket.ObjectTeleportIn { ObjectID = ObjectID, Type = effectnumber });
            
            BroadcastHealthChange();
            
            return true;
        }

        public virtual bool TeleportRandom(int attempts, int distance, Map map = null)
        {
            if (map == null) map = CurrentMap;
            if (map.Cells == null) return false;
            if (map.WalkableCells.Count == 0) return false;

            int cellIndex = Envir.Random.Next(map.WalkableCells.Count);

            return Teleport(map, map.WalkableCells[cellIndex]);
        }

        public Point GetRandomPoint(int attempts, int distance, Map map)
        {
            byte edgeoffset = 0;

            if (map.Width < 150)
            {
                if (map.Height < 30) edgeoffset = 2;
                else edgeoffset = 20;
            }

            for (int i = 0; i < attempts; i++)
            {
                Point location;

                if (distance <= 0)
                    location = new Point(edgeoffset + Envir.Random.Next(map.Width - edgeoffset), edgeoffset + Envir.Random.Next(map.Height - edgeoffset)); //Can adjust Random Range...
                else
                    location = new Point(CurrentLocation.X + Envir.Random.Next(-distance, distance + 1),
                                         CurrentLocation.Y + Envir.Random.Next(-distance, distance + 1));


                if (map.ValidPoint(location)) return location;
            }

            return new Point(0, 0);
        }

        public virtual void BroadcastHealthChange()
        {
            if (Race != ObjectType.Player && Race != ObjectType.Monster) return;

            byte time = Math.Min(byte.MaxValue, (byte)Math.Max(5, (RevTime - Envir.Time) / 1000));
            Packet p = new ServerPacket.ObjectHealth { ObjectID = ObjectID, Percent = PercentHealth, Expire = time };

            if (Envir.Time < RevTime)
            {
                CurrentMap.Broadcast(p, CurrentLocation);
                return;
            }

            if (Race == ObjectType.Monster && !AutoRev && Master == null) return;

            if (Race == ObjectType.Player)
            {
                if (GroupMembers != null) //Send HP to group
                {
                    for (int i = 0; i < GroupMembers.Count; i++)
                    {
                        PlayerObjectSrv member = GroupMembers[i];

                        if (this == member) continue;
                        if (member.CurrentMap != CurrentMap || !Functions.InRange(member.CurrentLocation, CurrentLocation, Globals.DataRange)) continue;
                        member.SendPacketToClient(p);
                    }
                }

                return;
            }

            if (Master != null && Master.Race == ObjectType.Player)
            {
                PlayerObjectSrv player = (PlayerObjectSrv)Master;

                player.SendPacketToClient(p);

                if (player.GroupMembers != null) //Send pet HP to group
                {
                    for (int i = 0; i < player.GroupMembers.Count; i++)
                    {
                        PlayerObjectSrv member = player.GroupMembers[i];

                        if (player == member) continue;

                        if (member.CurrentMap != CurrentMap || !Functions.InRange(member.CurrentLocation, CurrentLocation, Globals.DataRange)) continue;
                        member.SendPacketToClient(p);
                    }
                }
            }


            if (EXPOwner != null && EXPOwner.Race == ObjectType.Player)
            {
                PlayerObjectSrv player = (PlayerObjectSrv)EXPOwner;

                if (player.IsMember(Master)) return;
                
                player.SendPacketToClient(p);

                if (player.GroupMembers != null)
                {
                    for (int i = 0; i < player.GroupMembers.Count; i++)
                    {
                        PlayerObjectSrv member = player.GroupMembers[i];

                        if (player == member) continue;
                        if (member.CurrentMap != CurrentMap || !Functions.InRange(member.CurrentLocation, CurrentLocation, Globals.DataRange)) continue;
                        member.SendPacketToClient(p);
                    }
                }
            }
        }

        public void BroadcastDamageIndicator(DamageType type, int damage = 0)
        {
            Packet p = new ServerPacket.DamageIndicator { ObjectID = ObjectID, Damage = damage, Type = type };

            if (Race == ObjectType.Player)
            {
                PlayerObjectSrv player = (PlayerObjectSrv)this;
                player.SendPacketToClient(p);
            }
            Broadcast(p);
        }

        public abstract void Die();

        public abstract int Pushed(MapObjectSrv pusher, ExineDirection dir, int distance);

        public bool IsMember(MapObjectSrv member)
        {
            if (member == this) return true;
            if (GroupMembers == null || member == null) return false;

            for (int i = 0; i < GroupMembers.Count; i++)
                if (GroupMembers[i] == member) return true;

            return false;
        }

        public abstract void SendHealth(HumanObjectSrv player);

        public bool InTrapRock
        {
            set
            {
                if (this is PlayerObjectSrv)
                {
                    var player = (PlayerObjectSrv)this;
                    player.SendPacketToClient(new ServerPacket.InTrapRock { Trapped = value });
                }
            }
            get
            {
                Point checklocation;

                for (int i = 0; i <= 6; i += 2)
                {
                    checklocation = Functions.PointMove(CurrentLocation, (ExineDirection)i, 1);

                    if (checklocation.X < 0) continue;
                    if (checklocation.X >= CurrentMap.Width) continue;
                    if (checklocation.Y < 0) continue;
                    if (checklocation.Y >= CurrentMap.Height) continue;

                    Cell cell = CurrentMap.GetCell(checklocation.X, checklocation.Y);
                    if (!cell.Valid || cell.Objects == null) continue;

                    for (int j = 0; j < cell.Objects.Count; j++)
                    {
                        MapObjectSrv ob = cell.Objects[j];
                        switch (ob.Race)
                        {
                            case ObjectType.Monster:
                                if (ob is TrapRock)
                                {
                                    TrapRock rock = (TrapRock)ob;
                                    if (rock.Dead) continue;
                                    if (rock.Target != this) continue;
                                    if (!rock.Visible) continue;
                                }
                                else continue;

                                return true;
                            default:
                                continue;
                        }
                    }
                }
                return false;
            }
        }

    }

    public class Poison
    {
        private MapObjectSrv owner;
        public MapObjectSrv Owner
        {
            get 
            { 
                return owner switch
                {
                    _ => owner
                };
            }
            set { owner = value; }
        }
        public PoisonType PType;
        public int Value;
        public long Duration, Time, TickTime, TickSpeed;

        public Poison() { }

        public Poison(BinaryReader reader)
        {
            Owner = null;
            PType = (PoisonType)reader.ReadByte();
            Value = reader.ReadInt32();
            Duration = reader.ReadInt64();
            Time = reader.ReadInt64();
            TickTime = reader.ReadInt64();
            TickSpeed = reader.ReadInt64();
        }
    }
}

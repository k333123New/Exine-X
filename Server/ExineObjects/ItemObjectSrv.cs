﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects
{
    public sealed class ItemObjectSrv : MapObjectSrv
    {
        public override ObjectType Race
        {
            get { return ObjectType.Item; }
        }

        public override string Name
        {
            get { return Item == null ? string.Empty : Item.Info.FriendlyName; }
            set { throw new NotSupportedException(); }
        }

        public override int CurrentMapIndex { get; set; }
        public override Point CurrentLocation { get; set; }
        public override ExineDirection Direction
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
        public override ushort Level
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool Blocking
        {
            get { return false; }
        }

        public uint Gold;
        public UserItem Item;


        public override int Health
        {
            get { throw new NotSupportedException(); }
        }

        public override int MaxHealth
        {
            get { throw new NotSupportedException(); }
        }

        public ItemObjectSrv(MapObjectSrv dropper, UserItem item, bool deathDrop = false)
        {
            if (deathDrop)//player dropped it when he died: allow for time to run back and pickup his drops
                ExpireTime = Envir.Time + Settings.PlayerDiedItemTimeOut * Settings.Minute;
            else
                ExpireTime = Envir.Time + Settings.ItemTimeOut * Settings.Minute;

            Item = item;

            if (Item.IsAdded)
                NameColour = Color.Cyan;
			else
			{
				if (item.Info.Grade == ItemGrade.None)
					NameColour = Color.White;
				if (item.Info.Grade == ItemGrade.Common)
					NameColour = Color.White;
				if (item.Info.Grade == ItemGrade.Rare)
					NameColour = Color.DeepSkyBlue;
				if (item.Info.Grade == ItemGrade.Legendary)
					NameColour = Color.DarkOrange;
				if (item.Info.Grade == ItemGrade.Mythical)
					NameColour = Color.Plum;
                if (item.Info.Grade == ItemGrade.Heroic)
                    NameColour = Color.Red;
            }

			CurrentMap = dropper.CurrentMap;
            CurrentLocation = dropper.CurrentLocation;
        }
        public ItemObjectSrv(MapObjectSrv dropper, UserItem item, Point manualpoint)
        {
            ExpireTime = Envir.Time + Settings.ItemTimeOut * Settings.Minute;

            Item = item;

			if (Item.IsAdded)
				NameColour = Color.Cyan;
			else
			{
				if (item.Info.Grade == ItemGrade.None)
					NameColour = Color.White;
				if (item.Info.Grade == ItemGrade.Common)
					NameColour = Color.White;
				if (item.Info.Grade == ItemGrade.Rare)
					NameColour = Color.DeepSkyBlue;
				if (item.Info.Grade == ItemGrade.Legendary)
					NameColour = Color.DarkOrange;
				if (item.Info.Grade == ItemGrade.Mythical)
					NameColour = Color.Plum;
                if (item.Info.Grade == ItemGrade.Heroic)
                    NameColour = Color.Red;
            }

            CurrentMap = dropper.CurrentMap;
            CurrentLocation = manualpoint;
        }
        public ItemObjectSrv(MapObjectSrv dropper, uint gold)
        {
            ExpireTime = Envir.Time + Settings.ItemTimeOut * Settings.Minute;

            Gold = gold;

            CurrentMap = dropper.CurrentMap;
            CurrentLocation = dropper.CurrentLocation;
        }
        public ItemObjectSrv(MapObjectSrv dropper, uint gold, Point manualLocation)
        {
            ExpireTime = Envir.Time + Settings.ItemTimeOut * Settings.Minute;

            Gold = gold;

            CurrentMap = dropper.CurrentMap;
            CurrentLocation = manualLocation;
        }
         
        public override void Process()
        {
            if (Envir.Time > ExpireTime)
            {
                CurrentMap.RemoveObject(this);
                Despawn();
                return;
            }

            if (Owner != null && Envir.Time > OwnerTime)
                Owner = null;

            base.Process();
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            if (OwnerTime < time && OwnerTime > Envir.Time)
                time = OwnerTime;

            if (ExpireTime < time && ExpireTime > Envir.Time)
                time = ExpireTime;

            if (PKPointTime < time && PKPointTime > Envir.Time)
                time = PKPointTime;

            if (LastHitTime < time && LastHitTime > Envir.Time)
                time = LastHitTime;

            if (EXPOwnerTime < time && EXPOwnerTime > Envir.Time)
                time = EXPOwnerTime;

            if (BrownTime < time && BrownTime > Envir.Time)
                time = BrownTime;

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Time >= time && ActionList[i].Time > Envir.Time) continue;
                time = ActionList[i].Time;
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].TickTime >= time && PoisonList[i].TickTime > Envir.Time) continue;
                time = PoisonList[i].TickTime;
            }

            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].NextTime >= time && Buffs[i].NextTime > Envir.Time) continue;
                time = Buffs[i].NextTime;
            }


            if (OperateTime <= Envir.Time || time < OperateTime)
                OperateTime = time;
        }


        public bool Drop(int distance)
        {
            if (CurrentMap == null) return false;

            Cell best = null;
            int bestCount = 0;
            Point bestLocation = Point.Empty;

            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d*2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;

                        bool movement = false;
                        for (int i = 0; i < CurrentMap.Info.Movements.Count; i++)
                        {
                            MovementInfo info = CurrentMap.Info.Movements[i];
                            if (info.Source != new Point(x,y)) continue;
                            movement = true;
                            break;
                        }

                        if (movement) continue;

                        Cell cell = CurrentMap.GetCell(x, y);

                        if (cell.Objects == null)
                        {
                            CurrentLocation = new Point(x, y);
                            CurrentMap.AddObject(this);
                            Spawned();
                            return true;
                        }

                        int count = 0;
                        bool blocking = false;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObjectSrv ob = cell.Objects[i];
                            if (ob.Blocking)
                            {
                                blocking = true;
                                break;
                            }
                            if (ob.Race == ObjectType.Item)
                                count++;
                        }

                        if (blocking || count >= Settings.DropStackSize) continue;

                        if (count == 0)
                        {
                            CurrentLocation = new Point(x, y);
                            CurrentMap.AddObject(this);
                            Spawned();
                            return true;
                        }

                        if (best == null || count < bestCount)
                        {
                            best = cell;
                            bestCount = count;
                            bestLocation = new Point(x, y);
                        }
                    }
                }
            }

            if (best == null)

                return false;

            CurrentLocation = bestLocation;
            CurrentMap.AddObject(this);
            Spawned();
            return true;
        }

        public bool DragonDrop(int distance)
        {
            if (CurrentMap == null) return false;

            Cell best = null;
            int bestCount = 0;
            Point bestLocation = Point.Empty;

            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y + 3; y <= CurrentLocation.Y + (d * 2); y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;

                        bool movement = false;
                        for (int i = 0; i < CurrentMap.Info.Movements.Count; i++)
                        {
                            MovementInfo info = CurrentMap.Info.Movements[i];
                            if (info.Source != new Point(x, y)) continue;
                            movement = true;
                            break;
                        }

                        if (movement) continue;

                        Cell cell = CurrentMap.GetCell(x, y);

                        if (cell.Objects == null)
                        {
                            CurrentLocation = new Point(x, y);
                            CurrentMap.AddObject(this);
                            Spawned();
                            return true;
                        }

                        int count = 0;
                        bool blocking = false;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObjectSrv ob = cell.Objects[i];
                            if (ob.Blocking)
                            {
                                blocking = true;
                                break;
                            }
                            if (ob.Race == ObjectType.Item)
                                count++;
                        }

                        if (blocking || count >= Settings.DropStackSize) continue;

                        if (count == 0)
                        {
                            CurrentLocation = new Point(x, y);
                            CurrentMap.AddObject(this);
                            Spawned();
                            return true;
                        }

                        if (best == null || count < bestCount)
                        {
                            best = cell;
                            bestCount = count;
                            bestLocation = new Point(x, y);
                        }
                    }
                }
            }

            if (best == null)

                return false;

            CurrentLocation = bestLocation;
            CurrentMap.AddObject(this);
            Spawned();
            return true;
        }


        public override Packet GetInfo()
        {
            if (Item != null)
                return new ServerPacket.ObjectItem
                    {
                        ObjectID = ObjectID,
                        Name = Item.Count > 1 ? string.Format("{0} ({1})", Name, Item.Count) : Name,
                        NameColour = NameColour,
                        Location = CurrentLocation,
                        Image = Item.Image
                    };

            return new ServerPacket.ObjectGold
                {
                    ObjectID =  ObjectID,
                    Gold = Gold,
                    Location = CurrentLocation,
                };
        }



        public override void Process(DelayedAction action)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(HumanObjectSrv attacker)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(MonsterObjectSrv attacker)
        {
            throw new NotSupportedException();
        }
        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            throw new NotSupportedException();
        }
        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            throw new NotSupportedException();
        }
        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            throw new NotSupportedException();
        }

        public override void ApplyPoison(Poison p, MapObjectSrv Caster = null, bool NoResist = false, bool ignoreDefence = true)
        {
            throw new NotSupportedException();
        }

        public override Buff AddBuff(BuffType type, MapObjectSrv owner, int duration, Stats stats, bool refreshStats = true, bool updateOnly = false, params int[] values)
        {
            throw new NotSupportedException();
        }

        public override bool IsFriendlyTarget(HumanObjectSrv ally)
        {
            throw new NotSupportedException();
        }

        public override bool IsFriendlyTarget(MonsterObjectSrv ally)
        {
            throw new NotSupportedException();
        }

        public override void Die()
        {
            throw new NotSupportedException();
        }

        public override void SendHealth(HumanObjectSrv player)
        {
            throw new NotSupportedException();
        }

        public override int Pushed(MapObjectSrv pusher, ExineDirection dir, int distance)
        {
            throw new NotSupportedException();
        }

        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }
    }
}

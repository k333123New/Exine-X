﻿using Server.ExineDatabase;
using Server.ExineEnvir;



namespace Server.ExineObjects.Monsters
{
    public class ZumaMonster : MonsterObjectSrv
    {
        public bool Stoned = true;
        public bool AvoidFireWall = true;

        protected override bool CanMove
        {
            get
            {
                return base.CanMove && !Stoned;
            }
        }
        protected override bool CanAttack
        {
            get
            {
                return base.CanAttack && !Stoned;
            }
        }


        protected internal ZumaMonster(MonsterInfo info) : base(info)
        {
        }

        public override int Pushed(MapObjectSrv pusher, ExineDirection dir, int distance)
        {
            return Stoned ? 0 : base.Pushed(pusher, dir, distance);
        }

        public override void ApplyPoison(Poison p, MapObjectSrv Caster = null, bool NoResist = false, bool ignoreDefence = true)
        {
            if (Stoned) return;

            base.ApplyPoison(p, Caster, NoResist, ignoreDefence);
        }
        public override Buff AddBuff(BuffType type, MapObjectSrv owner, int duration, Stats stats, bool refreshStats = true, bool updateOnly = false, params int[] values)
        {
            if (Stoned) return null;

            return base.AddBuff(type, owner, duration, stats, refreshStats, updateOnly, values);
        }

        public override bool IsFriendlyTarget(HumanObjectSrv ally)
        {
            if (Stoned) return false;

            return base.IsFriendlyTarget(ally);
        }

        protected override void ProcessAI()
        {
            if (!Dead && Envir.Time > ActionTime)
            {
                bool stoned = !FindNearby(2);
                
                if (Stoned && !stoned)
                {
                    Wake();
                    WakeAll(14);
                }
            }

            base.ProcessAI();
        }

        public void Wake()
        {
            if (!Stoned) return;

            Stoned = false;
            Broadcast(new ServerPacket.ObjectShow { ObjectID = ObjectID });
            ActionTime = Envir.Time + 1000;
        }

        public void WakeAll(int dist)
        {
            for (int y = CurrentLocation.Y - dist; y <= CurrentLocation.Y + dist; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = CurrentLocation.X - dist; x <= CurrentLocation.X + dist; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;

                    Cell cell = CurrentMap.GetCell(x, y);

                    if (!cell.Valid || cell.Objects == null) continue;

                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        ZumaMonster target = cell.Objects[i] as ZumaMonster;
                        if (target == null || !target.Stoned) continue;
                        target.Wake();
                        target.Target = Target;
                    }
                }
            }

        }
        public override bool IsAttackTarget(MonsterObjectSrv attacker)
        {
            return !Stoned && base.IsAttackTarget(attacker);
        }
        public override bool IsAttackTarget(HumanObjectSrv attacker)
        {
            return !Stoned && base.IsAttackTarget(attacker);
        }

        public override bool Walk(ExineDirection dir)
        {
            if (!CanMove) return false;

            Point location = Functions.PointMove(CurrentLocation, dir, 1);

            if (!CurrentMap.ValidPoint(location)) return false;

            Cell cell = CurrentMap.GetCell(location);

            if (cell.Objects != null)
                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    MapObjectSrv ob = cell.Objects[i];
                    if (AvoidFireWall && ob.Race == ObjectType.Spell)
                        if (((SpellObjectSrv)ob).Spell == Spell.FireWall) return false;

                    if (!ob.Blocking) continue;

                    return false;
                }

            CurrentMap.GetCell(CurrentLocation).Remove(this);

            Direction = dir;
            RemoveObjects(dir, 1);
            CurrentLocation = location;
            CurrentMap.GetCell(CurrentLocation).Add(this);
            AddObjects(dir, 1);

            if (Hidden)
            {
                RemoveBuff(BuffType.Hiding);
            }

            CellTime = Envir.Time + 500;
            ActionTime = Envir.Time + 300;
            MoveTime = Envir.Time + MoveSpeed;

            if (MoveTime > AttackTime)
                AttackTime = MoveTime;

            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;

            Broadcast(new ServerPacket.ObjectWalk { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                if (cell.Objects[i].Race != ObjectType.Spell) continue;
                SpellObjectSrv ob = (SpellObjectSrv)cell.Objects[i];

                ob.ProcessSpell(this);
                //break;
            }

            return true;
        }

        public override Packet GetInfo()
        {
            return new ServerPacket.ObjectMonster
            {
                ObjectID = ObjectID,
                Name = Name,
                NameColour = NameColour,
                Location = CurrentLocation,
                Image = Info.Image,
                Direction = Direction,
                Effect = Info.Effect,
                AI = Info.AI,
                Light = Info.Light,
                Dead = Dead,
                Skeleton = Harvested,
                Poison = CurrentPoison,
                Hidden = Hidden,
                Extra = Stoned,
            };
        }
    }
}

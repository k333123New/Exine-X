﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class HornedMage : AxeSkeleton
    {
        protected internal HornedMage(MonsterInfo info)
            : base(info)
        {
        }

        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            bool ranged = CurrentLocation != Target.CurrentLocation && !Functions.InRange(CurrentLocation, Target.CurrentLocation, 3);

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            ShockTime = 0;

            if (!ranged)
            {
                int damage = GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
                if (damage == 0) return;

                ActionTime = Envir.Time + 500;
                AttackTime = Envir.Time + AttackSpeed;

                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
            }
            else
            {
                if (Envir.Random.Next(5) > 0)
                {
                    int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                    if (damage == 0) return;

                    ActionTime = Envir.Time + 500;
                    AttackTime = Envir.Time + AttackSpeed;

                    Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID, Type = 0 });
                    DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + 800, Target, damage, DefenceType.AC);
                    ActionList.Add(action);
                }
                else
                {
                    Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID, Type = 1 });

                    TeleportTarget(4, 4);

                    Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                }
            }
        }


        public bool TeleportTarget(int attempts, int distance)
        {
            for (int i = 0; i < attempts; i++)
            {
                Point location;

                if (distance <= 0)
                    location = new Point(Envir.Random.Next(CurrentMap.Width), Envir.Random.Next(CurrentMap.Height));
                else
                    location = new Point(CurrentLocation.X + Envir.Random.Next(-distance, distance + 1),
                                         CurrentLocation.Y + Envir.Random.Next(-distance, distance + 1));

                if (Target.Teleport(CurrentMap, location, true, Info.Effect)) return true;
            }

            return false;
        }

        protected override void CompleteRangeAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);
        }

        protected override void CompleteAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            List<MapObjectSrv> targets = FindAllTargets(3, CurrentLocation);

            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].Attacked(this, damage, defence);
            }
            return;
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
                Buffs = Buffs.Where(d => d.Info.Visible).Select(e => e.Type).ToList(),
            };
        }
    }
}
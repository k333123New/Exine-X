﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class HoodedSummoner : MonsterObjectSrv
    {
        public long SlaveSpawnTime;
        public long FearTime;
        public bool slaves1 = false;
        public bool slaves2 = false;

        protected internal HoodedSummoner(MonsterInfo info)
            : base(info)
        {
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Info.ViewRange);
        }

        protected override void Attack()
        {

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ShockTime = 0;
            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            switch (Envir.Random.Next(6))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    {
                        RangedAttack();
                    }
                    break;
                case 4:
                    {
                        if (Envir.Time > SlaveSpawnTime)
                        {
                            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 0 });
                            slaves1 = true;
                            SpawnSlaves();

                            // Timer added so if many of these mobs are around it doesn't overwhelm players too quickly with scrolls.
                            SlaveSpawnTime = Envir.Time + (Settings.Second * 15);
                        }
                        else
                        {
                            RangedAttack();
                        }
                    }
                    break;
                case 5:
                    {
                        if (Envir.Time > SlaveSpawnTime)
                        {
                            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });
                            slaves2 = true;
                            SpawnSlaves();

                            // Timer added so if many of these mobs are around it doesn't overwhelm players too quickly with scrolls.
                            SlaveSpawnTime = Envir.Time + (Settings.Second * 15);
                        }
                        else
                        {
                            RangedAttack();
                        }
                    }
                    break;
                default:
                    {
                        RangedAttack();
                    }
                    break;
            }

            slaves1 = false;
            slaves2 = false;
        }

        private void RangedAttack()
        {
            Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID, Type = 0 });

            int damage = GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
            if (damage == 0) return;
            DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + 500, Target, damage, DefenceType.MAC);
            ActionList.Add(action);
        }

        private void SpawnSlaves()
        {
             
        }

        protected override void ProcessTarget()
        {
            if (Target == null || !CanAttack) return;

            if (InAttackRange() && Envir.Time < FearTime)
            {
                Attack();
                return;
            }

            FearTime = Envir.Time + 5000;

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }

            int dist = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation);

            if (dist >= Info.ViewRange)
                MoveTo(Target.CurrentLocation);
            else
            {
                ExineDirection dir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);

                if (Walk(dir)) return;

                switch (Envir.Random.Next(2)) //No favour
                {
                    case 0:
                        for (int i = 0; i < 7; i++)
                        {
                            dir = Functions.NextDir(dir);

                            if (Walk(dir))
                                return;
                        }
                        break;
                    default:
                        for (int i = 0; i < 7; i++)
                        {
                            dir = Functions.PreviousDir(dir);

                            if (Walk(dir))
                                return;
                        }
                        break;
                }

            }
        }
    }
}

﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class Behemoth : MonsterObjectSrv
    {
        public byte AttackRange = 10;

        protected internal Behemoth(MonsterInfo info)
            : base(info)
        {
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            bool ranged = CurrentLocation == Target.CurrentLocation || !Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);

            if (!ranged)
            {
                switch (Envir.Random.Next(5))
                {
                    case 0:
                    case 1:
                    case 2:
                        base.Attack(); //swipe
                        break;
                    case 3:
                        {
                            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });

                            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                            if (damage == 0) return;

                            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, damage, DefenceType.ACAgility, true);
                            ActionList.Add(action);
                        }
                        break;
                    case 4:
                        {
                            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 2 });

                            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, 0, DefenceType.ACAgility, false);
                            ActionList.Add(action);
                        }
                        break;
                }

                PoisonTarget(Target, 15, 5, PoisonType.Bleeding);
            }
            else
            {
                if (Envir.Random.Next(2) == 0)
                {
                    MoveTo(Target.CurrentLocation);
                }
                else
                {
                    switch (Envir.Random.Next(2))
                    {
                        case 0:
                            Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 0 });

                            SpawnSlaves(); //spawn huggers
                            break;
                        case 1:
                            {
                                Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });

                                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]) * 3;
                                if (damage == 0) return;

                                DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + 500, Target, damage, DefenceType.ACAgility);
                                ActionList.Add(action);


                            }
                            break;
                    }
                }

                ShockTime = 0;
                ActionTime = Envir.Time + 300;
                AttackTime = Envir.Time + AttackSpeed;
            }
        }

        protected override void CompleteRangeAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            List<MapObjectSrv> targets = FindAllTargets(AttackRange, CurrentLocation);
            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                Broadcast(new ServerPacket.ObjectEffect { ObjectID = targets[i].ObjectID, Effect = SpellEffect.Behemoth });

                if (targets[i].Attacked(this, damage, defence) <= 0) continue;

                PoisonTarget(targets[i], 15, 5, PoisonType.Paralysis, 1000);
            }
        }

        protected override void CompleteAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];
            bool fireCircle = data.Count >= 4 ? (bool)data[3] : false;

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            if (fireCircle) //Firecircle
            {
                List<MapObjectSrv> targets = FindAllTargets(1, CurrentLocation);

                if (targets.Count == 0) return;

                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].Attacked(this, damage, defence);
                }
            }
            else //Push back
            {
                Point point = Functions.PointMove(CurrentLocation, Direction, 1);

                Cell cell = CurrentMap.GetCell(point);

                if (cell.Objects != null)
                {
                    for (int o = 0; o < cell.Objects.Count; o++)
                    {
                        MapObjectSrv t = cell.Objects[o];
                        if (t == null || t.Race != ObjectType.Player) continue;

                        if (t.IsAttackTarget(this))
                        {
                            t.Pushed(this, Direction, 4);

                            PoisonTarget(t, 3, 15, PoisonType.Dazed, 1000);
                        }
                        break;
                    }
                }
            }
        }

        private void SpawnSlaves()
        {
            
        }

        public override void Die()
        {
           

            base.Die();
        }
    }
}
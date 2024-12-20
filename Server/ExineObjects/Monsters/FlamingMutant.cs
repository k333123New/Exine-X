﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class FlamingMutant : MonsterObjectSrv
    {
        protected internal FlamingMutant(MonsterInfo info)
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
            bool ranged = CurrentLocation == Target.CurrentLocation || !Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);

            if (!ranged)
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                if (damage == 0) return;

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.ACAgility, true);
                ActionList.Add(action);
            }
            else
            {
                Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });

                int damage = GetAttackPower(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
                if (damage == 0) return;

                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 20 + 500; //50 MS per Step

                DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + delay, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
            }
        }

        protected override void CompleteAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);

            List<MapObjectSrv> targets = FindAllTargets(3, CurrentLocation, false);

            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                ExineDirection dir = Functions.DirectionFromPoint(targets[i].CurrentLocation, CurrentLocation);
                int dist = Functions.MaxDistance(targets[i].CurrentLocation, CurrentLocation);

                targets[i].Pushed(this, dir, dist - 1);
            }
        }

        protected override void CompleteRangeAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            var targets = FindAllTargets(3, target.CurrentLocation, false);

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Attacked(this, damage, defence) <= 0) continue;

                if (Envir.Random.Next(2) == 0)
                {
                    PoisonTarget(targets[i], 1, 5, PoisonType.Paralysis, 1000);
                    Broadcast(new ServerPacket.ObjectEffect { ObjectID = targets[i].ObjectID, Effect = SpellEffect.FlamingMutantWeb, Time = 5000 });
                }
            }
        }

        protected override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack)
            {
                Attack();
                return;
            }

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }

            MoveTo(Target.CurrentLocation);

        }
    }
}

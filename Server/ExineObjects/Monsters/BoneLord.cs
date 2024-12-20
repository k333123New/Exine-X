﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class BoneLord : MonsterObjectSrv
    {
        public byte AttackRange = 7;
        public byte _stage = 3;

        protected internal BoneLord(MonsterInfo info)
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

            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            bool range = CurrentLocation == Target.CurrentLocation || !Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);

            AttackTime = Envir.Time + AttackSpeed;
            ActionTime = Envir.Time + 300;

            if (range)
            {
                Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID, Type = 0 });

                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                if (damage == 0) return;

                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500; //50 MS per Step
                DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + delay, Target, damage, DefenceType.MACAgility);
                ActionList.Add(action);
            }
            else
            {
                base.Attack();
            }
        }

        protected override void ProcessTarget()
        {
            if (Target == null) return;

            if (Stats[Stat.HP] >= 3)
            {
                byte stage = (byte)(HP / (Stats[Stat.HP] / 3));

                if (stage < _stage)
                {
                    SpawnSlaves();
                    _stage = stage;
                    return;
                    }
            }

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

        private void SpawnSlaves()
        {
            
        }
    }
}

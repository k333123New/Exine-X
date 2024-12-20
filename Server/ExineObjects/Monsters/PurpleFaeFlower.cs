﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    class PurpleFaeFlower : ZumaMonster
    {
        protected override bool CanMove { get { return false; } }

        protected internal PurpleFaeFlower(MonsterInfo info)
            : base(info)
        {
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && CanFly(Target.CurrentLocation) && Functions.InRange(CurrentLocation, Target.CurrentLocation, Info.ViewRange);
            // ADDED CANFLY SO THAT PROJECTILE MOBS CANNOT SHOOT THROUGH WALLS
        }

        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;
            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            ShockTime = 0;

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            bool ranged = CurrentLocation == Target.CurrentLocation || !Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);

            if (!ranged)
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                if (damage == 0) return;

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, damage, DefenceType.AC);
                ActionList.Add(action);
            }
            else
            {
                Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID, Type = 0 });
                AttackTime = Envir.Time + AttackSpeed + 500;
                if (damage == 0) return;

                int delay = Functions.MaxDistance(CurrentLocation, Target.CurrentLocation) * 50 + 500; //50 MS per Step

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + delay, Target, damage, DefenceType.MAC);
                ActionList.Add(action);
            }
        }

        public override bool Walk(ExineDirection dir) { return false; }
        protected override void ProcessRoam() { }
    }
}


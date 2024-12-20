﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class GuardianRock : MonsterObjectSrv
    {
        public bool Active = true;
        protected override bool CanMove { get { return false; } }

        protected internal GuardianRock(MonsterInfo info)
            : base(info)
        {
            Direction = ExineDirection.Up;
        }

        public override void Turn(ExineDirection dir)
        {
        }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Info.ViewRange);
        }
        protected override void CompleteAttack(IList<object> data)
        {
            if (Target == null) return;
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            if (Target.CurrentMap != CurrentMap || Target.Node == null) return;

            Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });
            PullAttack();

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;
        }

        private void PullAttack()
        {
            ExineDirection pushdir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);
            if (Envir.Random.Next(Settings.MagicResistWeight) < Target.Stats[Stat.MagicResist]) return;
            int distance = Functions.MaxDistance(Target.CurrentLocation, CurrentLocation) -1;
            if (distance <= 0) return;
            if (distance > 4) distance = 4;
            
            Target.Pushed(this, pushdir, distance);
        }

        protected override void ProcessTarget()
        {
            if (Target == null) return;
            if (!Active) return;
            if (InAttackRange() && CanAttack)
            {
                ActionList.Add(new DelayedAction(DelayedType.Damage, Envir.Time + 500));
                return;
            }

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            return 0;
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }

        public override void ChangeHP(int amount)
        {
            //make it immune to green poison
        }
    }
}

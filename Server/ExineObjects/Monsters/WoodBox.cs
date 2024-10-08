﻿using Server.ExineDatabase;

namespace Server.ExineObjects.Monsters
{
    public class WoodBox : MonsterObjectSrv
    {
        protected override bool CanMove { get { return false; } }
        protected internal WoodBox(MonsterInfo info)
            : base(info)
        {
        }

        protected override void Attack() { }

        public override void Turn(ExineDirection dir) { }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override void ProcessTarget()
        {
        }

        public override void Die()
        {
            ActionList.Add(new DelayedAction(DelayedType.Die, Envir.Time + 300));
            base.Die();
        }

        protected override void CompleteDeath(IList<object> data)
        {
            List<MapObjectSrv> targets = FindAllTargets(1, CurrentLocation, false);
            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                if (damage == 0) return;

                if (targets[i].Attacked(this, damage, DefenceType.ACAgility) <= 0) continue;
            }
        }
    }
}

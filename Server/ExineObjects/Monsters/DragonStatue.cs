﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class DragonStatue : MonsterObjectSrv
    {
        private bool Sleeping;
        private long WakeUpTime;
        private int WakeDelay = 15 * (1000 * 60);

        protected override bool CanMove { get { return false; } }

        protected internal DragonStatue(MonsterInfo info)
            : base(info)
        {
            Direction = (ExineDirection)Math.Min((byte)5, (byte)Direction);
        }

        public override void Spawned()
        {
            if (Respawn != null)
                Direction = (ExineDirection)Math.Min((byte)5, (byte)Respawn.Info.Direction);

            base.Spawned();
        }

        public override void Turn(ExineDirection dir)
        {
        }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override void ProcessSearch()
        {
            if (!Sleeping)
                base.ProcessSearch();
        }

        protected override void ProcessAI()
        {
            if (!Dead && Sleeping && Envir.Time > WakeUpTime)
            {
                Sleeping = false;
                HP = Stats[Stat.HP];
                return;
            }

            base.ProcessAI();
        }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Info.ViewRange);
        }
        protected override void CompleteAttack(IList<object> data)
        {
            if (Target == null || !Target.IsAttackTarget(this) || Target.CurrentMap != CurrentMap || Target.Node == null) return;

            Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });
            List<MapObjectSrv> targets = FindAllTargets(2, Target.CurrentLocation);
            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                if (damage == 0) continue;

                targets[i].Attacked(this, damage, DefenceType.MAC);
            }
        }

        protected override void ProcessTarget()
        {
            if (Target == null || Sleeping) return;
            if (!CanAttack) return;
            if (!FindNearby(Info.ViewRange)) return;

            ActionList.Add(new DelayedAction(DelayedType.Damage, Envir.Time + 500));
            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            return Sleeping ? 0 : base.Attacked(attacker, damage, type);
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            return Sleeping ? 0 : base.Attacked(attacker, damage, type, damageWeapon);
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }
        public override void ChangeHP(int amount)
        {
            if (Sleeping) return;
            base.ChangeHP(amount);
        }
        public override void Die()
        {
            if (Dead || Sleeping) return;

            Sleeping = true;
            WakeUpTime = Envir.Time + WakeDelay;
        }
    }
}

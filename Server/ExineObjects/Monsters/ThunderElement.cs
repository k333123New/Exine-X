using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class ThunderElement : MonsterObjectSrv
    {
        protected internal ThunderElement(MonsterInfo info)
            : base(info)
        {
        }

        protected override void CompleteAttack(IList<object> data)
        {
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            List<MapObjectSrv> targets = FindAllTargets(2, CurrentLocation);
            if (targets.Count == 0) return;

            for (int i = 0; i < targets.Count; i++)
            {
                MapObjectSrv target = targets[i];

                if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

                target.Attacked(this, damage, defence);
            }

            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
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
            if (damage == 0) return;

            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, damage, DefenceType.MAC);
            ActionList.Add(action);
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (type != DefenceType.Repulsion) return 0;

            return base.Attacked(attacker, damage, type);
        }
        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            if (type != DefenceType.Repulsion) return 0;

            return base.Attacked(attacker, damage, type, damageWeapon);
        }
        public override int Pushed(MapObjectSrv pusher, ExineDirection dir, int distance)
        {
            int result = base.Pushed(pusher, dir, distance);

            if (result > 0)
            {
                if (pusher is PlayerObjectSrv) Attacked((PlayerObjectSrv)pusher, Math.Max(50, Envir.Random.Next(Stats[Stat.HP])), DefenceType.Repulsion);
                else if (pusher is MonsterObjectSrv) Attacked((MonsterObjectSrv)pusher, Math.Max(50, Envir.Random.Next(Stats[Stat.HP])), DefenceType.Repulsion);
            }
            return result;
        }

        public override void PoisonDamage(int amount, MapObjectSrv Attacker)
        {
            return;
        }
    }
}

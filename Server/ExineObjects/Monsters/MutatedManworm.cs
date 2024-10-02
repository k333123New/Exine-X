using Server.ExineDatabase;
using Server.ExineEnvir;

namespace Server.ExineObjects.Monsters
{
    public class MutatedManworm : CrazyManworm
    {
        public virtual byte TeleportEffect { get { return 4; } }

        protected internal MutatedManworm(MonsterInfo info)
            : base(info)
        {
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            int attackerDamage = base.Attacked(attacker, damage, type, damageWeapon);

            int ownDamage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);

            if (attackerDamage > ownDamage && Envir.Random.Next(2) == 0)
            {
                FindWeakerTarget();
            }

            return attackerDamage;
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            int attackerDamage = base.Attacked(attacker, damage, type);

            int ownDamage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);

            if (attackerDamage > ownDamage && Envir.Random.Next(2) == 0)
            {
                FindWeakerTarget();
            }

            return attackerDamage;
        }

        private void FindWeakerTarget()
        {
            List<MapObjectSrv> targets = FindAllTargets(Info.ViewRange, CurrentLocation);

            if (targets.Count < 2) return;

            var newTarget = Target;

            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].Stats[Stat.MinDC] >= Target.Stats[Stat.MinDC]) continue;

                newTarget = targets[i];
            }

            if (newTarget != Target)
            {
                Target = newTarget;
                TeleportToTarget(Target);
            }
        }

        private bool TeleportToTarget(MapObjectSrv target)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, target.CurrentLocation);

            var reverse = Functions.ReverseDirection(Direction);

            var point = Functions.PointMove(target.CurrentLocation, reverse, 1);

            if (point != CurrentLocation)
            {
                if (Teleport(CurrentMap, point, true, TeleportEffect)) return true;
            }

            return false;
        }
    }
}

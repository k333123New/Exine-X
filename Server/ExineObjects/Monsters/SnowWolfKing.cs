using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    //TODO - Teleport
    public class SnowWolfKing : MonsterObjectSrv
    {
        private bool _SpawnedSlaves;

        protected internal SnowWolfKing(MonsterInfo info)
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

            if (attackerDamage > ownDamage && Envir.Random.Next(10) == 0)
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
                if (Teleport(CurrentMap, point, true, 11)) return true;
            }

            return false;
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

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            if (Envir.Random.Next(3) > 0)
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 0 });
                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                if (damage == 0) return;

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.ACAgility, false, false);
                ActionList.Add(action);
            }
            else
            {
                if (HealthPercent >= 60)
                {
                    Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });
                    int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                    if (damage == 0) return;

                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.ACAgility, true, false);
                    ActionList.Add(action);
                }
                else if (HealthPercent >= 30)
                {
                    Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 2 });
                    int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                    if (damage == 0) return;

                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.ACAgility, false, true);
                    ActionList.Add(action);
                }
                else
                {
                    Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 3 });
                    int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                    if (damage == 0) return;

                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 500, Target, damage, DefenceType.ACAgility, false, false);
                    ActionList.Add(action);
                }
            }

             
        }

        protected override void CompleteAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);
        }

        public override void Die()
        {
            ActionList.Add(new DelayedAction(DelayedType.Die, Envir.Time + 500));
            base.Die();
        }

        protected override void CompleteDeath(IList<object> data)
        {
            var targets = FindAllTargets(1, CurrentLocation, false);

            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            if (damage > 0)
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].Attacked(this, damage, DefenceType.MAC);
                }
            }

            
        }

      
    }
}

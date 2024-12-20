﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class Nadz : MonsterObjectSrv
    {
        protected internal Nadz(MonsterInfo info)
            : base(info)
        {
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 3 || y > 3) return false;


            return (x <= 3 && y <= 3) || (x == y || x % 3 == y % 3);
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

                DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, 0, DefenceType.AC, true);
                ActionList.Add(action);
            }
            else
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });

                int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);

                SinglePushAttack(damage);
            }
        }

        protected override void CompleteAttack(IList<object> data)
        {
            MapObjectSrv target = (MapObjectSrv)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];
            bool halfmoon = data.Count >= 4 && (bool)data[3];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            if (halfmoon)
            {
                List<MapObjectSrv> targets = FindAllTargets(3, CurrentLocation);
                if (targets.Count == 0) return;

                for (int i = 0; i < targets.Count; i++)
                {
                    int damage2 = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
                    if (damage2 == 0) return;
                    if (targets[i].Attacked(this, damage2, defence) <= 0) return;
                }
            }
            else
            {
                if (target.Attacked(this, damage, defence) <= 0) return;

                PoisonTarget(target, 3, 5, PoisonType.Paralysis);
            }
        }  
    }
}

﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    class FrozenMiner : MonsterObjectSrv
    {

        protected internal FrozenMiner(MonsterInfo info)
            : base(info)
        {
        }

        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            ExineDirection dir = Functions.PreviousDir(Direction);
            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            if (damage == 0) return;

            List<MapObjectSrv> targets = FindAllTargets(1, CurrentLocation);

            if ((targets.Count > 1 && Envir.Random.Next(2) == 0) || Envir.Random.Next(8) == 0)
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 1 });

                for (int i = 0; i < targets.Count; i++)
                {
                    DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 1000, targets[i], (int)(damage * 0.8), DefenceType.ACAgility);
                    ActionList.Add(action);
                }
            }
            else
            {
                Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 0 });
                DelayedAction action = new DelayedAction(DelayedType.RangeDamage, Envir.Time + 600, Target, damage, DefenceType.ACAgility);
                ActionList.Add(action);
            }

            ShockTime = 0;
            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            if (Target.Dead)
                FindTarget();
        }

    }
}
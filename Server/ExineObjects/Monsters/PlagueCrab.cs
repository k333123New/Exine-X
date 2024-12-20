﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class PlagueCrab : MonsterObjectSrv
    {
        protected internal PlagueCrab(MonsterInfo info)
            : base(info)
        {
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 4 || y > 4) return false;

            return (x == 0) || (y == 0) || (x == y);
        }

        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed + 500;
            ShockTime = 0;

            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, Type = 0 });

            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            if (damage == 0) return;

            LineAttack(damage, 4, 500, DefenceType.MACAgility);
        }
    }
}
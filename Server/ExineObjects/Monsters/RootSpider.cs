using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class RootSpider : BugBagMaggot
    {
        protected internal RootSpider(MonsterInfo info) 
            : base(info)
        {
            byte randomdirection = (byte)Envir.Random.Next(3);
            Direction = (ExineDirection)randomdirection;
        }

        protected override void Attack()
        {
            ShockTime = 0;

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

         

            MonsterObjectSrv spawn = GetMonster(Envir.GetMonsterInfo(Settings.BombSpiderName));

            if (spawn == null) return;

            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + 3000;

            spawn.Target = Target;
            spawn.ActionTime = Envir.Time + 1000;
            Point spawnlocation = Point.Empty;
            switch (Direction)
            {
                case ExineDirection.Up:
                    spawnlocation = Back;
                    break;
                case ExineDirection.UpRight:
                    spawnlocation = Functions.PointMove(CurrentLocation, ExineDirection.DownRight, 1);
                    break;
                case ExineDirection.Right:
                    spawnlocation = Functions.PointMove(CurrentLocation, ExineDirection.DownLeft, 1);
                    break;
            }

            CurrentMap.ActionList.Add(new DelayedAction(DelayedType.Spawn, Envir.Time + 500, spawn, spawnlocation, this));
        }
    }
}

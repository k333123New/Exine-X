using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class SnakeTotem : MonsterObjectSrv
    {
        public bool Summoned;
        public long AliveTime;
        public long DieTime;
        public int MaxMinions;

        protected internal SnakeTotem(MonsterInfo info) : base(info)
        {
            ActionTime = Envir.Time + 1000;
            Direction = ExineDirection.Up;
        }

        public override string Name
        {
            get { return Master == null ? Info.GameName : (Dead ? Info.GameName : string.Format("{0}({1})", Info.GameName, Master.Name)); }
            set { throw new NotSupportedException(); }
        }

        public override void Turn(ExineDirection dir)
        {
        }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        public override void Process()
        {
            MaxMinions = PetLevel + 1;
            if (!Dead && Summoned)
            {
                bool selfDestruct = false;
                if (Master != null)
                {
                    if (Master.CurrentMap != CurrentMap || !Functions.InRange(Master.CurrentLocation, CurrentLocation, 15)) selfDestruct = true;
                    if (Summoned && Envir.Time > AliveTime) selfDestruct = true;
                    if (selfDestruct)
                    {
                        Die();
                        DieTime = Envir.Time + 3000;
                    }
                }
                base.Process();
            }
            else if (Envir.Time >= DieTime) Despawn();
        }

        public override void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Damage:
                    CompleteAttack(action.Params);
                    break;
            }
        }

        protected override void ProcessAI()
        {
            if (Dead) return;

            //Search for target
            if (Envir.Time < SearchTime) return;
            SearchTime = Envir.Time + SearchDelay;

            //Cant agro when shocked
            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }
            //update targets all the time ?
            AgroAllMobsInRange();

           
        }

        public void AgroAllMobsInRange()
        {
            for (int d = 0; d <= Info.ViewRange; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (!cell.Valid || cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObjectSrv ob = cell.Objects[i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    if (((MonsterObjectSrv)ob).Info.CoolEye == 100) continue;
                                    ob.Target = this;//Agro the mobs in range - Very simple agro system overwriting mobs target
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
        }

        public bool SpawnMinion()
        {
            
            return true;
        }

        public override void Die()
        {
            base.Die();

            DeadTime = 0;
            
            
        }

        public override void Spawned()
        {
            base.Spawned();
            Summoned = true;
        }

        public override Packet GetInfo()
        {
            return new ServerPacket.ObjectMonster
            {
                ObjectID = ObjectID,
                Name = Name,
                NameColour = NameColour,
                Location = CurrentLocation,
                Image = Monster.SnakeTotem,
                Direction = Direction,
                Effect = Info.Effect,
                AI = Info.AI,
                Light = Info.Light,
                Dead = Dead,
                Skeleton = Harvested,
                Poison = CurrentPoison,
                Hidden = Hidden,
                Extra = Summoned,
            };
        }
    }
}

using Server.ExineDatabase;
using S = ServerPackets;

namespace Server.ExineObjects.Monsters
{
    public class BugBagMaggot : MonsterObject
    {
        protected override bool CanMove { get { return false; } }

        protected internal BugBagMaggot(MonsterInfo info) : base(info)
        {
            Direction = ExineDirection.Up;
        }

        public override void Turn(ExineDirection dir)
        {
        }
        public override bool Walk(ExineDirection dir) { return false; }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Globals.DataRange);
        }

        protected override void Attack()
        {
            ShockTime = 0;

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            if (SlaveList.Count >= 20) return;       
            
            MonsterObject spawn = GetMonster(Envir.GetMonsterInfo(Settings.BugBatName));

            if (spawn == null) return;

            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + 3000;

            spawn.Target = Target;
            spawn.ActionTime = Envir.Time + 1000;
            CurrentMap.ActionList.Add(new DelayedAction(DelayedType.Spawn, Envir.Time + 500, spawn, CurrentLocation, this));
        }

        protected override void ProcessRoam() { }
    }
}

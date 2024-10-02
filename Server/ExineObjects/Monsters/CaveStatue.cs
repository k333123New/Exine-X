using Server.ExineDatabase;

namespace Server.ExineObjects.Monsters
{
    public class CaveStatue : MonsterObject
    {
        protected override bool CanMove { get { return false; } }
        protected override bool CanAttack { get { return false; } }
        protected override bool CanRegen { get { return false; } }

        protected internal CaveStatue(MonsterInfo info)
            : base(info)
        {
            if (info.Effect == 1)
            {
                Direction = ExineDirection.UpRight;
            }
            else
            {
                Direction = ExineDirection.Up;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
        }

        public override void Turn(ExineDirection dir) { }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override void ProcessTarget() { }
    }
}

using Server.ExineDatabase;

namespace Server.ExineObjects.Monsters
{
    public class CargoBox : MonsterObject
    {
        protected override bool CanMove { get { return false; } }
        protected internal CargoBox(MonsterInfo info)
            : base(info)
        {
        }

        protected override void Attack() { }

        public override void Turn(ExineDirection dir) { }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override void ProcessTarget()
        {
        }
    }
}

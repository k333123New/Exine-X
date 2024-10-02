using Server.ExineDatabase;
using S = ServerPackets;

namespace Server.ExineObjects.Monsters
{
    public class ZumaTaurus : ZumaMonster
    {
        private byte _stage = 7;

        protected internal ZumaTaurus(MonsterInfo info) : base(info)
        {
            Direction = ExineDirection.DownLeft;
            AvoidFireWall = false;
        }

        protected override void ProcessAI()
        {
            if (Dead) return;
            
            if (Stats[Stat.HP] >= 7)
            {
                byte stage = (byte)(HP / (Stats[Stat.HP] / 7));

                if (stage < _stage) SpawnSlaves();
                _stage = stage;
            }


            base.ProcessAI();
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
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            if (damage == 0) return;

            DelayedAction action = new DelayedAction(DelayedType.Damage, Envir.Time + 300, Target, damage, DefenceType.MACAgility);
            ActionList.Add(action);
        }

        private void SpawnSlaves()
        {
            
        }
    }
}

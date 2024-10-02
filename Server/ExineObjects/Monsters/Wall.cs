using Server.ExineDatabase;
using S = ServerPackets;

namespace Server.ExineObjects.Monsters
{
    public class Wall : MonsterObjectSrv
    {
        public ConquestObjectSrv Conquest;
        public int WallIndex;

        protected internal Wall(MonsterInfo info) : base(info)
        {
            Direction = ExineDirection.Up;
        }
        public override void Despawn()
        {
            base.Despawn();
        }
        protected override bool CanAttack
        {
            get
            {
                return false;
            }
        }
        protected override bool CanMove { get { return false; } }

        public override void Turn(ExineDirection dir) { }

        public override bool Walk(ExineDirection dir) { return false; }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            CheckDirection();

            if (!Conquest.WarIsOn || attacker.MyGuild != null && Conquest.GuildInfo.Owner == attacker.MyGuild.Guildindex) damage = 0;

            return base.Attacked(attacker, damage, type, damageWeapon);
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            CheckDirection();

            if (!Conquest.WarIsOn) damage = 0;

            return base.Attacked(attacker, damage, type);
        }

        public void RepairWall()
        {
            if (HP == 0)
                Revive(Stats[Stat.HP], false);
            else
                SetHP(Stats[Stat.HP]);

            CheckDirection();
        }

        public void CheckDirection()
        {
            ExineDirection newDirection = (ExineDirection)(4 - GetDamageLevel());

            if (newDirection != Direction)
            {
                Direction = newDirection;
                Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            }
        }

        public override void Die()
        {
            base.Die();
        }

        protected int GetDamageLevel()
        {
            int level = (int)Math.Round((double)(4 * HP) / Stats[Stat.HP]);

            if (level < 1) level = 1;

            return level;
        }

        protected override bool CanRegen
        {
            get
            {
                return false;
            }
        }
    }
}


﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class Tree : MonsterObjectSrv
    {
        protected override bool CanMove { get { return false; } }
        protected override bool CanAttack { get { return false; } }
        protected override bool CanRegen { get { return false; } }

        protected internal Tree(MonsterInfo info)
            : base(info)
        {
            Direction = ExineDirection.Up;
        }

        protected override void Attack() { }
        protected override void FindTarget() { }

        public override void Turn(ExineDirection dir)
        {
        }
        public override bool Walk(ExineDirection dir) 
        { 
            return false; 
        }

        protected override void ProcessTarget() { }

        protected override void ProcessRegen() { }
        protected override void ProcessSearch() { }
        protected override void ProcessRoam() { }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.AC:
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.MACAgility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.MAC:
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.Agility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    break;
            }

            if (armour >= damage) return 0;
            
            ShockTime = 0;

            if (attacker.Info.AI == 6)
                EXPOwner = null;
            else if (attacker.Master != null)
            {
                if (EXPOwner == null || EXPOwner.Dead)
                    EXPOwner = attacker.Master switch
                    { 
                        _ => attacker.Master
                    };

                if (EXPOwner == attacker.Master)
                    EXPOwnerTime = Envir.Time + EXPOwnerDelay;

            }

            Broadcast(new ServerPacket.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            ChangeHP(-1);
            return 1;
        
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.AC:
                    armour = GetAttackPower(Stats[Stat.MinAC], Stats[Stat.MaxAC]);
                    break;
                case DefenceType.MACAgility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.MAC:
                    armour = GetAttackPower(Stats[Stat.MinMAC], Stats[Stat.MaxMAC]);
                    break;
                case DefenceType.Agility:
                    if (Envir.Random.Next(Stats[Stat.Agility] + 1) > attacker.Stats[Stat.Accuracy]) return 0;
                    break;
            }

            if (armour >= damage) return 0;

            if (damageWeapon)
                attacker.DamageWeapon();

            ShockTime = 0;

            if (Master != null && Master != attacker)
                if (Envir.Time > Master.BrownTime && Master.PKPoints < 200)
                    attacker.BrownTime = Envir.Time + Settings.Minute;

            if (EXPOwner == null || EXPOwner.Dead)
                EXPOwner = GetAttacker(attacker);

            if (EXPOwner == attacker)
                EXPOwnerTime = Envir.Time + EXPOwnerDelay;

            Broadcast(new ServerPacket.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });
            attacker.GatherElement();
            ChangeHP(-1);

            return 1;
        }

        public override void ApplyPoison(Poison p, MapObjectSrv Caster = null, bool NoResist = false, bool ignoreDefence = true) { }
    }
}

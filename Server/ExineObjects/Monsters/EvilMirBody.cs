﻿using Server.ExineDatabase;

namespace Server.ExineObjects.Monsters
{
    public class EvilMirBody : MonsterObjectSrv
    {
        private bool _dragonlink;
        public bool DragonLink
        {
            get { return _dragonlink && Envir.DragonSystem != null; }
            set
            {
                if (_dragonlink == value) return;

                _dragonlink = value;
            }
        }

        protected override bool CanAttack { get { return false; } }
        protected override bool CanMove { get { return false; } }

        protected internal EvilMirBody(MonsterInfo info)
            : base(info)
        {
            DragonLink = true;
        }

        public override void Turn(ExineDirection dir)
        {
        }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override void ProcessSearch() { }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (DragonLink)
            {
                if (Envir.DragonSystem.LinkedMonster != null)
                {
                    MonsterObjectSrv ob = Envir.DragonSystem.LinkedMonster;
                    if (attacker.Info.AI == 6)
                        EXPOwner = null;

                    else if (attacker.Master != null)
                    {
                        if (!Functions.InRange(attacker.CurrentLocation, attacker.Master.CurrentLocation, Globals.DataRange))
                            ob.EXPOwner = null;
                        else
                        {

                            if (ob.EXPOwner == null || ob.EXPOwner.Dead)
                                ob.EXPOwner = attacker.Master switch
                                {
                                    _ => attacker.Master
                                };

                            if (ob.EXPOwner == attacker.Master)
                                ob.EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                        }

                    }
                }
                Envir.DragonSystem.GainExp(Envir.Random.Next(1, 40));
                return 1;
            }

            return 0;
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            if (DragonLink)
            {
                if (Envir.DragonSystem.LinkedMonster != null)
                {
                    MonsterObjectSrv ob = Envir.DragonSystem.LinkedMonster;
                    if (ob.EXPOwner == null || ob.EXPOwner.Dead)
                        ob.EXPOwner = GetAttacker(attacker);

                    if (ob.EXPOwner == attacker)
                        ob.EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                }

                if (damageWeapon)
                    attacker.DamageWeapon();

                Envir.DragonSystem.GainExp(Envir.Random.Next(1, 40));
                return 1;
            }

            return 0;
        }
        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }
    }
}

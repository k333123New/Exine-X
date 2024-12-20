﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class TrapRock : MonsterObjectSrv
    {
        public bool Visible, ChildRock, FirstAttack;
        public long VisibleTime;
        public byte SpawnCorner;
        public TrapRock ParentRock;
        public Point TargetLocation;

        protected override bool CanAttack
        {
            get
            {
                return Visible && base.CanAttack;
            }
        }
        protected override bool CanMove { get { return false; } }
        public override bool Blocking
        {
            get
            {
                return Visible && base.Blocking;
            }
        }

        protected internal TrapRock(MonsterInfo info)
            : base(info)
        {
            Visible = false;
            VisibleTime = Envir.Time + 2000;
            FirstAttack = true;
        }

        public override MapObjectSrv Target
        {
            get { return _target; }
            set
            {
                if (_target != null && value != null) return;
                if (_target != null && value == null) _target.InTrapRock = false;
                _target = value;

                if (Visible && value == null) Die();
            }

        }

        protected override void ProcessAI()
        {
            if (Dead) return;

            if (Envir.Time > VisibleTime)
            {
                VisibleTime = Envir.Time + 2000;

                bool visible = Target != null;

                if (!Visible && visible && !Target.Dead && !Target.InTrapRock)
                {
                    SpawnCorner = (byte)(Envir.Random.Next(4) * 2);
                    if (Teleport(CurrentMap, Functions.PointMove(Target.CurrentLocation, (ExineDirection)SpawnCorner, 1), false))
                    {
                        ActionTime = Envir.Time + 1000;
                        Show();
                        return;
                    }
                }

                if (Visible && (Target.Dead || Target.CurrentLocation != TargetLocation))
                {
                    Die();
                    return;
                }
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

            if (!ChildRock)
            {
                Broadcast(new ServerPacket.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });
            }
            else Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            if (Envir.Random.Next(8) == 0 && !ChildRock)
            {
                Target.ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = 3, TickSpeed = 1000 }, this, true);
            }

            if (Target.Dead)
                Die();
        }

        public override void Die()
        {
            if (!ChildRock)
            {
                if (Target != null)
                {
                    if (CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, 1))
                        Target.InTrapRock = false;
                }
                if (Info.HasDieScript && (Envir.MonsterNPC != null))
                {
                    Envir.MonsterNPC.Call(this,string.Format("[@_DIE({0})]", Info.Index));
                }


               
            }

            base.Die();
        }

        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (ChildRock) ParentRock.FirstAttack = false;
            if (!ChildRock && FirstAttack == true)
            {
                Die();
                return 0;
            }
            return base.Attacked(attacker, damage, type);
        }

        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            if (ChildRock) ParentRock.FirstAttack = false;
            if (!ChildRock && FirstAttack == true)
            {
                Die();
                return 0;
            }
            return base.Attacked(attacker, damage, type, damageWeapon);
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }

        public override void ChangeHP(int amount)
        {
            if (ChildRock) return;

            base.ChangeHP(amount);
        }


        public override void Turn(ExineDirection dir)
        {
        }

        public override bool Walk(ExineDirection dir) { return false; }

        protected override void ProcessRoam() { }

        public override Packet GetInfo()
        {
            if (!Visible) return null;

            return base.GetInfo();
        }

        public void Show()
        {
            TargetLocation = Target.CurrentLocation;
            Visible = true;
            CellTime = ChildRock ? ParentRock.CellTime : Envir.Time + 500;

            Broadcast(GetInfo());
            Broadcast(new ServerPacket.ObjectShow { ObjectID = ObjectID });

            if (!ChildRock)
            {
                Target.ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = 3, TickSpeed = 1000 },this, true);
                Target.InTrapRock = true;

                for (byte i = 0; i <= 6; i += 2)
                {
                    if (i == SpawnCorner) continue;
                    var mob = GetMonster(Envir.GetMonsterInfo(Name));

                    if (mob == null) return;
                    var childmob = (TrapRock)mob;

                    if (childmob.Spawn(CurrentMap, Functions.PointMove(Target.CurrentLocation, (ExineDirection)i, 1)))
                    {
                        if (Target != null) childmob.Target = Target;

                        childmob.ChildRock = true;
                        childmob.ParentRock = this;
                        //SlaveList.Add(childmob);
                        childmob.Show();
                        childmob.ActionTime = this.ActionTime;
                        childmob.AttackTime = this.AttackTime;
                    }
                    else continue;
                }
            }
        }
    }
}

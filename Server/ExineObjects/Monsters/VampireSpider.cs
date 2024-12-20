﻿using Server.ExineDatabase;
using Server.ExineEnvir;


namespace Server.ExineObjects.Monsters
{
    public class VampireSpider : MonsterObjectSrv
    {
        public bool Summoned;
        public long AliveTime;
        public long deadTime;

        protected internal VampireSpider(MonsterInfo info) : base(info)
        {
            ActionTime = Envir.Time + 1000;
        }

        public override string Name
        {
            get { return Master == null ? Info.GameName : (Dead ? Info.GameName : string.Format("{0}({1})", Info.GameName, Master.Name)); }
            set { throw new NotSupportedException(); }
        }

        public override void Process()
        {
            if (!Dead && Summoned)
            {
                if (Master != null)
                {
                    bool selfDestruct = false;
                    if (FindObject(Master.ObjectID, 15) == null) selfDestruct = true;
                    if (Summoned && Envir.Time > AliveTime) selfDestruct = true;
                    if (selfDestruct && Master != null) Die();
                }
            }

            base.Process();
        }

        public override void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Damage:
                    CompleteAttack(action.Params);
                    break;
                case DelayedType.RangeDamage:
                    CompleteRangeAttack(action.Params);
                    break;
                case DelayedType.Recall:
                    PetRecall((MapObjectSrv)action.Params[0]);
                    break;
            }
        }

        public void PetRecall(MapObjectSrv target)
        {
            if (target == null) return;
            if (Master == null) return;
            Teleport(Master.CurrentMap, target.CurrentLocation);
        }

        protected override void ProcessAI()
        {
            if (Dead) return;

            ProcessSearch();
            //todo ProcessRoaming(); needs no master follow just target roaming
            ProcessTarget();
        }

        public override void Die()
        {
            base.Die();

            //Explosion
            for (int y = CurrentLocation.Y - 1; y <= CurrentLocation.Y + 1; y++)
            {
                if (y < 0) continue;
                if (y >= CurrentMap.Height) break;

                for (int x = CurrentLocation.X - 1; x <= CurrentLocation.X + 1; x++)
                {
                    if (x < 0) continue;
                    if (x >= CurrentMap.Width) break;

                    Cell cell = CurrentMap.GetCell(x, y);

                    if (!cell.Valid || cell.Objects == null) continue;

                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObjectSrv target = cell.Objects[i];
                        switch (target.Race)
                        {
                            case ObjectType.Monster:
                            case ObjectType.Player:
                                //Only targets
                                if (!target.IsAttackTarget(this) || target.Dead) break;
                                int value = target.Attacked(this,10*PetLevel,DefenceType.MACAgility);
                                if (value <= 0) break;
                                if (Master != null) MasterVampire(value, target);
                                break;
                        }
                    }
                }
            }
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 1 || y > 1) return false;
            return (x <= 1 && y <= 1) || (x == y || x % 2 == y % 2);
        }
        protected override void Attack()
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new ServerPacket.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            AttackLogic();

            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;
            ShockTime = 0;

            if (Target.Dead) FindTarget();
        }

        private void AttackLogic()
        {
            int damage = GetAttackPower(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            if (damage == 0) return;
            Point target = Functions.PointMove(CurrentLocation, Direction, 1);


            int value;
            if (target == Target.CurrentLocation)
            {
                value = Target.Attacked(this, damage, DefenceType.MACAgility);
                if (value > 0 && Master != null) MasterVampire(value, Target);
            }
            else
            {
                if (!CurrentMap.ValidPoint(target)) return;

                Cell cell = CurrentMap.GetCell(target);
                if (cell.Objects == null) return;

                for (int o = 0; o < cell.Objects.Count; o++)
                {
                    MapObjectSrv ob = cell.Objects[o];
                    if (ob.Race == ObjectType.Monster || ob.Race == ObjectType.Player)
                    {
                        if (!ob.IsAttackTarget(this)) continue;

                        value = ob.Attacked(this, damage, DefenceType.MACAgility);
                        if (value > 0 && Master != null) MasterVampire(value, ob);
                    }
                    else continue;

                    break;
                }
            }
        }

        private void MasterVampire(int value, MapObjectSrv ob)
        {
            if (Master == null) return;
            if (Master.VampAmount == 0) ((PlayerObjectSrv)Master).VampTime = Envir.Time + 1000;
            Master.VampAmount += (ushort)(value * (PetLevel + 1) * 0.25F);
            ob.Broadcast(new ServerPacket.ObjectEffect { ObjectID = ob.ObjectID, Effect = SpellEffect.Bleeding, EffectType = 0 });
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
                    Image = Monster.VampireSpider,
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

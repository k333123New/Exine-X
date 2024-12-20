﻿using Server.ExineEnvir;


namespace Server.ExineObjects
{
    public sealed class DecoObjectSrv : MapObjectSrv
    {
        public override ObjectType Race
        {
            get { return ObjectType.Deco; }
        }

        public override string Name { get; set; }
        public override int CurrentMapIndex { get; set; }
        public override Point CurrentLocation { get; set; }
        public override ExineDirection Direction { get; set; }
        public override ushort Level { get; set; }
        public override bool Blocking
        {
            get
            {
                return false;
            }
        }

        public int Image;

        public override int Health
        {
            get { throw new NotSupportedException(); }
        }
        public override int MaxHealth
        {
            get { throw new NotSupportedException(); }
        }


        public override void Process()
        {           
            //Cell cell = CurrentMap.GetCell(CurrentLocation);
            //for (int i = 0; i < cell.Objects.Count; i++)
            //    ProcessDeco(cell.Objects[i]);
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            //if (TickTime < time && TickTime > Envir.Time)
            //    time = TickTime;

            if (OwnerTime < time && OwnerTime > Envir.Time)
                time = OwnerTime;

            if (ExpireTime < time && ExpireTime > Envir.Time)
                time = ExpireTime;

            if (PKPointTime < time && PKPointTime > Envir.Time)
                time = PKPointTime;

            if (LastHitTime < time && LastHitTime > Envir.Time)
                time = LastHitTime;

            if (EXPOwnerTime < time && EXPOwnerTime > Envir.Time)
                time = EXPOwnerTime;

            if (BrownTime < time && BrownTime > Envir.Time)
                time = BrownTime;

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Time >= time && ActionList[i].Time > Envir.Time) continue;
                time = ActionList[i].Time;
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].TickTime >= time && PoisonList[i].TickTime > Envir.Time) continue;
                time = PoisonList[i].TickTime;
            }

            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].NextTime >= time && Buffs[i].NextTime > Envir.Time) continue;
                time = Buffs[i].NextTime;
            }

            if (OperateTime <= Envir.Time || time < OperateTime)
                OperateTime = time;
        }

        public override void Process(DelayedAction action)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(HumanObjectSrv attacker)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(MonsterObjectSrv attacker)
        {
            throw new NotSupportedException();
        }
        public override int Attacked(HumanObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            throw new NotSupportedException();
        }
        public override int Attacked(MonsterObjectSrv attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            throw new NotSupportedException();
        }

        public override int Struck(int damage, DefenceType type = DefenceType.ACAgility)
        {
            throw new NotSupportedException();
        }
        public override bool IsFriendlyTarget(HumanObjectSrv ally)
        {
            throw new NotSupportedException();
        }
        public override bool IsFriendlyTarget(MonsterObjectSrv ally)
        {
            throw new NotSupportedException();
        }
        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }

        public override Packet GetInfo()
        {
            return new ServerPacket.ObjectDeco
            {
                ObjectID = ObjectID,
                Location = CurrentLocation,
                Image = Image
            };
        }

        public override void ApplyPoison(Poison p, MapObjectSrv Caster = null, bool NoResist = false, bool ignoreDefence = true)
        {
            throw new NotSupportedException();
        }
        public override void Die()
        {
            throw new NotSupportedException();
        }
        public override int Pushed(MapObjectSrv pusher, ExineDirection dir, int distance)
        {
            throw new NotSupportedException();
        }
        public override void SendHealth(HumanObjectSrv player)
        {
            throw new NotSupportedException();
        }
        public override void Despawn()
        {
            base.Despawn();
        }
    }
}

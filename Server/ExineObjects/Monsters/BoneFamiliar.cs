﻿using Server.ExineDatabase;


namespace Server.ExineObjects.Monsters
{
    public class BoneFamiliar : MonsterObjectSrv
    {
        public bool Summoned;

        protected internal BoneFamiliar(MonsterInfo info) : base(info)
        {
            Direction = ExineDirection.DownLeft;
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
                Image = Info.Image,
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

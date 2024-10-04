﻿using System.Drawing;

namespace ClientPackets
{
    public sealed class ClientVersion : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.ClientVersion; }
        }

        public byte[] VersionHash;

        protected override void ReadPacket(BinaryReader reader)
        {
            VersionHash = reader.ReadBytes(reader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(VersionHash.Length);
            writer.Write(VersionHash);
        }
    }
    public sealed class Disconnect : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.Disconnect; }
        }
        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class KeepAlive : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.KeepAlive; }
        }
        public long Time;
        protected override void ReadPacket(BinaryReader reader)
        {
            Time = reader.ReadInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Time);
        }
    }
    public sealed class NewAccount: Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.NewAccount; }
        }

        public string AccountID = string.Empty;
        public string Password = string.Empty;
        public DateTime BirthDate;
        public string UserName = string.Empty;
        public string SecretQuestion = string.Empty;
        public string SecretAnswer = string.Empty;
        public string EMailAddress = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            Password = reader.ReadString();
            BirthDate = DateTime.FromBinary(reader.ReadInt64());
            UserName = reader.ReadString();
            SecretQuestion = reader.ReadString();
            SecretAnswer = reader.ReadString();
            EMailAddress = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(Password);
            writer.Write(BirthDate.ToBinary());
            writer.Write(UserName);
            writer.Write(SecretQuestion);
            writer.Write(SecretAnswer);
            writer.Write(EMailAddress);
        }
    }
    public sealed class ChangePassword: Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.ChangePassword; }
        }

        public string AccountID = string.Empty;
        public string CurrentPassword = string.Empty;
        public string NewPassword = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            CurrentPassword = reader.ReadString();
            NewPassword = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(CurrentPassword);
            writer.Write(NewPassword);
        }
    } 

    public sealed class Login : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.Login; }
        }

        public string AccountID = string.Empty;
        public string Password = string.Empty;
      

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            Password = reader.ReadString(); 
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(Password); 

        }
    }
    public sealed class NewCharacter : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.NewCharacter; } }

        public string Name = string.Empty;
        public ExineGender Gender;
        public ExineClass Class;
        public ExineStyle ExStyle; //add k333123
        public ExineColor ExColor;//add k333123
        public int ExPortraitLen;//add k333123
        public byte[] ExPortraitBytes = new byte[8000];//add k333123
        //public byte[] Portrait;

        protected override void ReadPacket(BinaryReader reader)
        {
            Console.WriteLine("NewCharacter ReadPacket");
            Name = reader.ReadString();
            Gender = (ExineGender)reader.ReadByte();
            Class = (ExineClass)reader.ReadByte();
            ExStyle = (ExineStyle)reader.ReadByte(); //add k333123
            ExColor = (ExineColor)reader.ReadByte(); //add k333123
            ExPortraitLen = (int)reader.ReadInt32();//add k333123
            ExPortraitBytes = reader.ReadBytes(8000);//add k333123

            Console.WriteLine("ExStyle #5 :" + ExStyle);
            //Portrait = reader.ReadBytes(50);//크기는 차후 확인 필요함.
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            Console.WriteLine("NewCharacter WritePacket");
            writer.Write(Name);
            writer.Write((byte)Gender);
            writer.Write((byte)Class);
            writer.Write((byte)ExStyle);//add k333123
            writer.Write((byte)ExColor);//add k333123
            writer.Write(ExPortraitLen);//add k333123
            writer.Write(ExPortraitBytes);//add k333123

            Console.WriteLine("ExStyle #6 :" + ExStyle);
        }
    }
    public sealed class DeleteCharacter : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DeleteCharacter; } }

        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            CharacterIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(CharacterIndex);
        }
    }
    public sealed class StartGame : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.StartGame; } }

        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            CharacterIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(CharacterIndex);
        }
    }
    public sealed class LogOut : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.LogOut; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class Turn : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Turn; } }

        public ExineDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }

    public sealed class Rest : Packet //add
    {
        public override short Index { get { return (short)ClientPacketIds.Rest; } }
        public ExineDirection Direction;
        public bool IsRest;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
            IsRest = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
            writer.Write(IsRest);
        }
    }

    public sealed class UpdatePhoto : Packet //add
    {
        public override short Index { get { return (short)ClientPacketIds.UpdatePhoto; } }
        public byte[] photoData;
        public int photoDataLen;

        protected override void ReadPacket(BinaryReader reader)
        {
            photoData = reader.ReadBytes(8000);
            photoDataLen = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(photoData);
            writer.Write(photoDataLen);
        }
    }

    public sealed class Walk : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Walk; } }

        public ExineDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class Run : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Run; } }

        public ExineDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class Chat : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Chat; } }

        public string Message = string.Empty;
        public List<ChatItem> LinkedItems = new List<ChatItem>();

        protected override void ReadPacket(BinaryReader reader)
        {
            Message = reader.ReadString();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                LinkedItems.Add(new ChatItem(reader));
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Message);

            writer.Write(LinkedItems.Count);

            for (int i = 0; i < LinkedItems.Count; i++)
                LinkedItems[i].Save(writer);
        }
    }
    public sealed class MoveItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MoveItem; } }

        public MirGridType Grid;
        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class StoreItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.StoreItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }

    public sealed class DepositRefineItem : Packet
    {

        public override short Index { get { return (short)ClientPacketIds.DepositRefineItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }

    public sealed class RetrieveRefineItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RetrieveRefineItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }

    public sealed class RefineCancel : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RefineCancel; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class RefineItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RefineItem; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }

    public sealed class CheckRefine : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CheckRefine; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }

    public sealed class ReplaceWedRing : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ReplaceWedRing; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }


    public sealed class DepositTradeItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DepositTradeItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }

    public sealed class RetrieveTradeItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RetrieveTradeItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class TakeBackItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TakeBackItem; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class MergeItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MergeItem; } }

        public MirGridType GridFrom, GridTo;
        public ulong IDFrom, IDTo;
        protected override void ReadPacket(BinaryReader reader)
        {
            GridFrom = (MirGridType)reader.ReadByte();
            GridTo = (MirGridType)reader.ReadByte();
            IDFrom = reader.ReadUInt64();
            IDTo = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)GridFrom);
            writer.Write((byte)GridTo);
            writer.Write(IDFrom);
            writer.Write(IDTo);
        }
    }
    public sealed class EquipItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.EquipItem; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(To);
        }
    }
    public sealed class RemoveItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RemoveItem; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(To);
        }
    }
    public sealed class RemoveSlotItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RemoveSlotItem; } }

        public MirGridType Grid;
        public MirGridType GridTo;
        public ulong UniqueID;
        public int To;
        public ulong FromUniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            GridTo = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
            FromUniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write((byte)GridTo);
            writer.Write(UniqueID);
            writer.Write(To);
            writer.Write(FromUniqueID);
        }
    }
    public sealed class SplitItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SplitItem; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public ushort Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt16();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(Count);
        }
    }
    public sealed class UseItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.UseItem; } }

        public ulong UniqueID;
        public MirGridType Grid;
        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Grid = (MirGridType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write((byte)Grid);
        }
    }
    public sealed class DropItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DropItem; } }

        public ulong UniqueID;
        public ushort Count;
        public bool HeroInventory = false;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt16();
            HeroInventory = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
            writer.Write(HeroInventory);
        }
    }

    public sealed class DropGold : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DropGold; } }

        public uint Amount;

        protected override void ReadPacket(BinaryReader reader)
        {
            Amount = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Amount);
        }
    }
    public sealed class PickUp : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.PickUp; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class Inspect : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.Inspect; }
        }

        public uint ObjectID;
        public bool Ranking = false;
        public bool Hero = false;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Ranking = reader.ReadBoolean();
            Hero = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Ranking);
            writer.Write(Hero);
        }
    }
    public sealed class Observe : Packet
    {
        public override short Index
        {
            get { return (short)ClientPacketIds.Observe; }
        }

        public string Name;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class ChangeAMode : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ChangeAMode; } }

        public AttackMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (AttackMode)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Mode);
        }
    }
    public sealed class ChangePMode : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ChangePMode; } }

        public PetMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (PetMode)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Mode);
        }
    }
    public sealed class ChangeTrade : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ChangeTrade; } }

        public bool AllowTrade;

        protected override void ReadPacket(BinaryReader reader)
        {
            AllowTrade = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AllowTrade);
        }
    }
    public sealed class Attack : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Attack; } }

        public ExineDirection Direction;
        public Spell Spell;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection) reader.ReadByte();
            Spell = (Spell) reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
            writer.Write((byte)Spell);
        }
    }
    public sealed class RangeAttack : Packet //ArcherTest
    {
        public override short Index { get { return (short)ClientPacketIds.RangeAttack; } }

        public ExineDirection Direction;
        public Point Location;
        public uint TargetID;
        public Point TargetLocation;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            TargetID = reader.ReadUInt32();
            TargetLocation = new Point(reader.ReadInt32(), reader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(TargetID);
            writer.Write(TargetLocation.X);
            writer.Write(TargetLocation.Y);
        }
    }
    public sealed class Harvest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Harvest; } }

        public ExineDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (ExineDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class CallNPC : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CallNPC; } }

        public uint ObjectID;
        public string Key = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Key = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Key);
        }
    }
    public sealed class BuyItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.BuyItem; } }

        public ulong ItemIndex;
        public ushort Count;
        public PanelType Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ItemIndex = reader.ReadUInt64();
            Count = reader.ReadUInt16();
            Type = (PanelType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ItemIndex);
            writer.Write(Count);
            writer.Write((byte)Type);
        }
    }
    public sealed class SellItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SellItem; } }

        public ulong UniqueID;
        public ushort Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt16();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
        }
    }
    public sealed class CraftItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CraftItem; } }

        public ulong UniqueID;
        public ushort Count;
        public int[] Slots;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt16();

            Slots = new int[reader.ReadInt32()];
            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i] = reader.ReadInt32();
            }
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);

            writer.Write(Slots.Length);
            for (int i = 0; i < Slots.Length; i++)
            {
                writer.Write(Slots[i]);
            }
        }
    }
    public sealed class RepairItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RepairItem; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }
    public sealed class BuyItemBack : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.BuyItemBack; } }

        public ulong UniqueID;
        public ushort Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt16();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
        }
    }
    public sealed class SRepairItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SRepairItem; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }

    public sealed class RequestMapInfo : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RequestMapInfo; } }

        public int MapIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            MapIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(MapIndex);
        }
    }

    public sealed class TeleportToNPC : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TeleportToNPC; } }

        public uint ObjectID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
        }
    }

    public sealed class SearchMap : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SearchMap; } }

        public string Text;

        protected override void ReadPacket(BinaryReader reader)
        {
            Text = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Text);
        }
    }
    public sealed class MagicKey : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MagicKey; } }

        public Spell Spell;
        public byte Key, OldKey;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Key = reader.ReadByte();
            OldKey = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Spell);
            writer.Write(Key);
            writer.Write(OldKey);
        }
    }
    public sealed class Magic : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Magic; } }

        public Spell Spell;
        public ExineDirection Direction;
        public uint TargetID;
        public Point Location;
        public uint ObjectID;
        public bool SpellTargetLock;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Spell = (Spell) reader.ReadByte();
            Direction = (ExineDirection)reader.ReadByte();
            TargetID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            SpellTargetLock = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write((byte) Spell);
            writer.Write((byte)Direction);
            writer.Write(TargetID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(SpellTargetLock);
        }
    }

    public sealed class SwitchGroup : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SwitchGroup; } }

        public bool AllowGroup;
        protected override void ReadPacket(BinaryReader reader)
        {
            AllowGroup = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AllowGroup);
        }
    }
    public sealed class AddMember : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AddMember; } }

        public string Name = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class DelMember : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DellMember; } }

        public string Name = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class GroupInvite : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GroupInvite; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }
     

    public sealed class MarriageRequest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MarriageRequest; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class MarriageReply : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MarriageReply; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }

    public sealed class ChangeMarriage : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ChangeMarriage; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class DivorceRequest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DivorceRequest; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class DivorceReply : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.DivorceReply; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }

    public sealed class AddMentor : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AddMentor; } }

        public string Name;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }

    public sealed class MentorReply : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.MentorReply; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }

    public sealed class AllowMentor : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AllowMentor; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class CancelMentor : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CancelMentor; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }


    public sealed class TradeReply : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TradeReply; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }
    public sealed class TradeRequest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TradeRequest; } }

        protected override void ReadPacket(BinaryReader reader)
        {  }

        protected override void WritePacket(BinaryWriter writer)
        { }
    }

    public sealed class TradeGold : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TradeGold; } }

        public uint Amount;
        protected override void ReadPacket(BinaryReader reader)
        {
            Amount = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Amount);
        }
    }
    public sealed class TradeConfirm : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TradeConfirm; } }

        public bool Locked;
        protected override void ReadPacket(BinaryReader reader)
        {
            Locked = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Locked);
        }
    }
    public sealed class TradeCancel : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TradeCancel; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class TownRevive : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.TownRevive; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class SpellToggle : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.SpellToggle; } }
        public Spell Spell;
        public SpellToggleState canUse = SpellToggleState.None;
        public bool CanUse
        {
            get { return Convert.ToBoolean(canUse); }
            set
            {
                canUse = (SpellToggleState)Convert.ToSByte(value);
            }
        }

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell)reader.ReadByte();
            canUse = (SpellToggleState)reader.ReadSByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Spell);
            writer.Write((sbyte)canUse);
        }
    }
    public sealed class ConsignItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ConsignItem; } }

        public ulong UniqueID;
        public uint Price;
        public MarketPanelType Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Price = reader.ReadUInt32();
            Type = (MarketPanelType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Price);
            writer.Write((byte)Type);
        }
    }
     
    public sealed class RequestUserName : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RequestUserName; } }

        public uint UserID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UserID = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UserID);
        }
    }
    public sealed class RequestChatItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RequestChatItem; } }

        public ulong ChatItemID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ChatItemID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ChatItemID);
        }
    }
    public sealed class EditGuildMember : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.EditGuildMember; } }

        public byte ChangeType = 0;
        public byte RankIndex = 0;
        public string Name = "";
        public string RankName = "";

        protected override void ReadPacket(BinaryReader reader)
        {
            ChangeType = reader.ReadByte();
            RankIndex = reader.ReadByte();
            Name = reader.ReadString();
            RankName = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ChangeType);
            writer.Write(RankIndex);
            writer.Write(Name);
            writer.Write(RankName);
        }
    }
    public sealed class EditGuildNotice : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.EditGuildNotice; } }

        public List<string> notice = new List<string>();

        protected override void ReadPacket(BinaryReader reader)
        {
            int LineCount = reader.ReadInt32();
            for (int i = 0; i < LineCount; i++)
                notice.Add(reader.ReadString());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(notice.Count);
            for (int i = 0; i < notice.Count; i++)
                writer.Write(notice[i]);
        }
    }
    public sealed class GuildInvite : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildInvite; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }
    public sealed class RequestGuildInfo : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RequestGuildInfo; } }

        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            Type = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Type);
        }
    }
    public sealed class GuildNameReturn : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildNameReturn; } }

        public string Name;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class GuildWarReturn : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildWarReturn; } }

        public string Name;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class GuildStorageGoldChange: Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildStorageGoldChange; } }

        public byte Type = 0;
        public uint Amount = 0;      
        
        protected override void ReadPacket(BinaryReader reader)
        {
            Type = reader.ReadByte();
            Amount = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(Amount);
        }
    }
    public sealed class GuildStorageItemChange: Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildStorageItemChange; } }

        public byte Type = 0;
        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            Type = reader.ReadByte();
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(From);
            writer.Write(To);
        }
    }

    public sealed class EquipSlotItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.EquipSlotItem; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;
        public MirGridType GridTo;
        public ulong ToUniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
            GridTo = (MirGridType)reader.ReadByte();
            ToUniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(To);
            writer.Write((byte)GridTo);
            writer.Write(ToUniqueID);
        }
    }

    public sealed class FishingCast : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.FishingCast; } }

        public bool Sitdown;

        protected override void ReadPacket(BinaryReader reader)
        {
            Sitdown = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Sitdown);
        }
    }

    public sealed class FishingChangeAutocast : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.FishingChangeAutocast; } }

        public bool AutoCast;

        protected override void ReadPacket(BinaryReader reader)
        {
            AutoCast = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AutoCast);
        }
    }

    public sealed class AcceptQuest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AcceptQuest; } }

        public uint NPCIndex;
        public int QuestIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            NPCIndex = reader.ReadUInt32();
            QuestIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(NPCIndex);
            writer.Write(QuestIndex);
        }
    }

    public sealed class FinishQuest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.FinishQuest; } }

        public int QuestIndex;
        public int SelectedItemIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            QuestIndex = reader.ReadInt32();
            SelectedItemIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(QuestIndex);
            writer.Write(SelectedItemIndex);
        }
    }

    public sealed class AbandonQuest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AbandonQuest; } }

        public int QuestIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            QuestIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(QuestIndex);
        }
    }

    public sealed class ShareQuest : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ShareQuest; } }

        public int QuestIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            QuestIndex = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(QuestIndex);
        }
    }

    public sealed class AcceptReincarnation : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AcceptReincarnation; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class CancelReincarnation : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CancelReincarnation; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }

    public sealed class CombineItem : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.CombineItem; } }

        public MirGridType Grid;
        public ulong IDFrom, IDTo;
        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            IDFrom = reader.ReadUInt64();
            IDTo = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(IDFrom);
            writer.Write(IDTo);
        }
    }
    

    public sealed class AddFriend : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.AddFriend; } }

        public string Name;
        public bool Blocked;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
            Blocked = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Blocked);
        }
    }

    public sealed class RemoveFriend : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RemoveFriend; } }

        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            CharacterIndex = reader.ReadInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(CharacterIndex);
        }
    }

    public sealed class RefreshFriends : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.RefreshFriends; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }

        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }


   
    public sealed class GuildBuffUpdate : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GuildBuffUpdate; } }

        public byte Action = 0; //0 = request list, 1 = request a buff to be enabled, 2 = request a buff to be activated
        public int Id;

        protected override void ReadPacket(BinaryReader reader)
        {
            Action = reader.ReadByte();
            Id = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Action);
            writer.Write(Id);
        }
    }

    public sealed class GameshopBuy : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GameshopBuy; } }

        public int GIndex;
        public byte Quantity;
        public int PType;

        protected override void ReadPacket(BinaryReader reader)
        {
            GIndex = reader.ReadInt32();
            Quantity = reader.ReadByte();
            PType = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(GIndex);
            writer.Write(Quantity);
            writer.Write(PType);
        }
    }

    public sealed class NPCConfirmInput : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.NPCConfirmInput; } }

        public uint NPCID;
        public string PageName;
        public string Value;

        protected override void ReadPacket(BinaryReader reader)
        {
            NPCID = reader.ReadUInt32();
            PageName = reader.ReadString();
            Value = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(NPCID);
            writer.Write(PageName);
            writer.Write(Value);
        }
    }

    public sealed class ReportIssue : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.ReportIssue; } }

        public byte[] Image;
        public int ImageSize;
        public int ImageChunk;

        public string Message;

        protected override void ReadPacket(BinaryReader reader)
        {
            Image = reader.ReadBytes(reader.ReadInt32());
            ImageSize = reader.ReadInt32();
            ImageChunk = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Image.Length);
            writer.Write(Image);
            writer.Write(ImageSize);
            writer.Write(ImageChunk);
        }
    }
    public sealed class GetRanking : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.GetRanking; } }
        public byte RankType;
        public int RankIndex;
        public bool OnlineOnly;

        protected override void ReadPacket(BinaryReader reader)
        {
            RankType = reader.ReadByte();
            RankIndex = reader.ReadInt32();
            OnlineOnly = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(RankType);
            writer.Write(RankIndex);
            writer.Write(OnlineOnly);
        }
    }

    public sealed class Opendoor : Packet
    {
        public override short Index { get { return (short)ClientPacketIds.Opendoor; } }
        public byte DoorIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            DoorIndex = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(DoorIndex);
        }
    }

}

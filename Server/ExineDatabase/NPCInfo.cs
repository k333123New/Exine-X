﻿using Server.ExineEnvir;

namespace Server.ExineDatabase
{
    public class NPCInfo
    {
        protected static Envir EditEnvir
        {
            get { return Envir.Edit; }
        }

        public int Index;

        public string FileName = string.Empty, Name = string.Empty;

        public int MapIndex;
        public Point Location;
        public ushort Rate = 100;
        public ushort Image;
        public Color Colour;
        
        public int Direction=0; //k333123 add 240314

        public bool TimeVisible = false;
        public byte HourStart = 0;
        public byte MinuteStart = 0;
        public byte HourEnd = 0;
        public byte MinuteEnd = 1;
        public short MinLev = 0;
        public short MaxLev = 0;
        public string DayofWeek = "";
        public string ClassRequired = "";
        public bool Sabuk = false;
        public int FlagNeeded = 0;
        public int Conquest;
        public bool ShowOnBigMap;
        public int BigMapIcon;
        public bool CanTeleportTo;
        public bool ConquestVisible = true;

        public List<int> CollectQuestIndexes = new List<int>();
        public List<int> FinishQuestIndexes = new List<int>();
        
        public NPCInfo() { }
        public NPCInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            MapIndex = reader.ReadInt32();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                CollectQuestIndexes.Add(reader.ReadInt32());

            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                FinishQuestIndexes.Add(reader.ReadInt32());

            FileName = reader.ReadString();
            Name = reader.ReadString();

            Location = new Point(reader.ReadInt32(), reader.ReadInt32());

            if (Envir.LoadVersion >= 72)
            {
                Image = reader.ReadUInt16();
            }
            else
            {
                Image = reader.ReadByte();
            }
            
            Rate = reader.ReadUInt16();

            if (Envir.LoadVersion >= 64)
            {
                TimeVisible = reader.ReadBoolean();
                HourStart = reader.ReadByte();
                MinuteStart = reader.ReadByte();
                HourEnd = reader.ReadByte();
                MinuteEnd = reader.ReadByte();
                MinLev = reader.ReadInt16();
                MaxLev = reader.ReadInt16();
                DayofWeek = reader.ReadString();
                ClassRequired = reader.ReadString();
                if (Envir.LoadVersion >= 66)
                    Conquest = reader.ReadInt32();
                else
                    Sabuk = reader.ReadBoolean();
                FlagNeeded = reader.ReadInt32();
            }

            if (Envir.LoadVersion > 95)
            {
                ShowOnBigMap = reader.ReadBoolean();
                BigMapIcon = reader.ReadInt32();
            }
            if (Envir.LoadVersion > 96)
                CanTeleportTo = reader.ReadBoolean();

            if (Envir.LoadVersion >= 107)
            {
                ConquestVisible = reader.ReadBoolean();
            }

            //add color k333123
            try
            {
                int colorR = reader.ReadByte();
                int colorG = reader.ReadByte();
                int colorB = reader.ReadByte();
                Colour = Color.FromArgb(colorR, colorG, colorB); 
            }
            catch(Exception)
            {
                Colour = Color.FromArgb(255, 255, 255);
            }

            //add direction k333123
            
            try
            {
                Direction = reader.ReadInt32(); 
            }
            catch (Exception)
            {
                Direction = 0;
            }
            
           

        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(MapIndex);

            writer.Write(CollectQuestIndexes.Count());
            for (int i = 0; i < CollectQuestIndexes.Count; i++)
                writer.Write(CollectQuestIndexes[i]);

            writer.Write(FinishQuestIndexes.Count());
            for (int i = 0; i < FinishQuestIndexes.Count; i++)
                writer.Write(FinishQuestIndexes[i]);

            writer.Write(FileName);
            writer.Write(Name);

            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Image);
            writer.Write(Rate);

            writer.Write(TimeVisible);
            writer.Write(HourStart);
            writer.Write(MinuteStart);
            writer.Write(HourEnd);
            writer.Write(MinuteEnd);
            writer.Write(MinLev);
            writer.Write(MaxLev);
            writer.Write(DayofWeek);
            writer.Write(ClassRequired);
            writer.Write(Conquest);
            writer.Write(FlagNeeded);

            writer.Write(ShowOnBigMap);
            writer.Write(BigMapIcon);
            writer.Write(CanTeleportTo);
            writer.Write(ConquestVisible);

            writer.Write(Colour.R);
            writer.Write(Colour.G);
            writer.Write(Colour.B);

            writer.Write(Direction);

        }

        public static void FromText(string text)
        {
          
            string[] data = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); 
            if (data.Length < 6) return;

            Console.WriteLine("!!!!! data.len:"+ data.Length);
            NPCInfo info = new NPCInfo { Name = data[0] };

            int x, y;

            info.FileName = data[0];
            info.MapIndex = EditEnvir.MapInfoList.Where(d => d.FileName == data[1]).FirstOrDefault().Index;

            if (!int.TryParse(data[2], out x)) return; 
            if (!int.TryParse(data[3], out y)) return; 

            info.Location = new Point(x, y);

            info.Name = data[4];

            if (!ushort.TryParse(data[5], out info.Image)) return; 
            if (!ushort.TryParse(data[6], out info.Rate)) return; 
            if (!bool.TryParse(data[7], out info.ShowOnBigMap)) return; 
            if (!int.TryParse(data[8], out info.BigMapIcon)) return; 
            if (!bool.TryParse(data[9], out info.CanTeleportTo)) return; 
            if (!bool.TryParse(data[10], out info.ConquestVisible)) return;

            Console.WriteLine("!!!@@");

            info.Colour = Color.FromArgb(int.Parse(data[11]), int.Parse(data[12]), int.Parse(data[13]));
            info.Direction =int.Parse(data[14]);

            info.Index = ++EditEnvir.NPCIndex;

           

            EditEnvir.NPCInfoList.Add(info); 
        }
        public string ToText()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                FileName, EditEnvir.MapInfoList.Where(d => d.Index == MapIndex).FirstOrDefault().FileName, Location.X, Location.Y, Name, Image, Rate, ShowOnBigMap, BigMapIcon, CanTeleportTo, Colour.R, Colour.G, Colour.B, Direction);
        }

        public override string ToString()
        {
            return string.Format("{0}:   {1}", FileName, Functions.PointToString(Location));
        }

        public string GameName
        {
            get
            {
                string name = Name;
                if (name.Contains("_"))
                {
                    string[] splitName = name.Split('_');
                    name = splitName[splitName.Length - 1];
                }
                return name;
            }
        }
    }
}

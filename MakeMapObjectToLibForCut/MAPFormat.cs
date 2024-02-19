using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MapObjectToLibTilesForCut
{
    
    class MAPFormat
    {
        public MAPHeader mapHeader = new MAPHeader();
        public MapAni mapAni = new MapAni();
        public MapTileInfo mapTileInfo = new MapTileInfo();
        public StaticObjectInfo staticObjectInfos = new StaticObjectInfo(); 

        public MAPFormat(byte[] datas, string filename)
        {
            int idx = mapHeader.FillData(datas, filename);
            idx = mapAni.FillData(datas, mapHeader.Height, mapHeader.Width, idx);
            idx = mapTileInfo.FillData(datas, idx, mapHeader.Height, mapHeader.Width, mapHeader.WorldId);
            idx = staticObjectInfos.FillData(datas, idx, mapHeader.WorldId[3], mapHeader.IsElsaMap);
            if (idx == -1) return;
        }
        public void ConvertToM2MAP(string filename)
        {
            
        }
    }



    class MAPHeader
    {
        ushort[] worldId = new ushort[4];
        byte width;
        byte height;
        bool isElsaMap = false;
        short mapId;

        public ushort[] WorldId { get => worldId; set => worldId = value; }
        public byte Width { get => width; set => width = value; }
        public byte Height { get => height; set => height = value; }
        public bool IsElsaMap { get => isElsaMap; set => isElsaMap = value; }
        public short MapId { get => mapId; set => mapId = value; }

        public int FillData(byte[] datas, string filename)
        {
            try
            {
                Console.WriteLine("datas.len" + datas.Length);

                mapId = Int16.Parse(Path.GetFileNameWithoutExtension(filename));

                int idx = 0;
                worldId[0] = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;
                worldId[1] = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;
                worldId[2] = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                int elsaIdx = idx;
                width = datas[idx];
                idx++;
                height = datas[idx];
                idx++;

                if (width == 0 || height == 0) isElsaMap = true;
                if (isElsaMap)
                {
                    worldId[3] = BitConverter.ToUInt16(datas, elsaIdx);
                    width = datas[idx];
                    idx++;
                    height = datas[idx];
                    idx++;
                }

                // StaticObjectInfo 

                PrintVal();

                return idx;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        private void PrintVal()
        {
            Console.WriteLine("worldId[0]:" + worldId[0]);
            Console.WriteLine("worldId[1]:" + worldId[1]);
            Console.WriteLine("worldId[2]:" + worldId[2]);
            Console.WriteLine("Width:" + width);
            Console.WriteLine("Height:" + height);
            Console.WriteLine("mapId:" + mapId);
        }

    }

    class StaticObject
    {
        byte isAnim;            // true : TS_ _AnimStatic[Shadows] false : TS_ _Static[Shadows]
        ushort imgIndex;       // image index
        ushort x;
        ushort y;
        int world;

        public byte IsAnim { get => isAnim; set => isAnim = value; }
        public ushort ImgIndex { get => imgIndex; set => imgIndex = value; }
        public ushort X { get => x; set => x = value; }
        public ushort Y { get => y; set => y = value; }
        public int World { get => world; set => world = value; }
    }

    class StaticObjectInfo
    {
        ushort count = 0;
        ushort count1 = 0;
        StaticObject[] staticObjects;

        public ushort Count { get => count;  } 
        public StaticObject[] StaticObjects { get => staticObjects;  }

        public int FillData(byte[] data, int idx, int worldId3, bool isElsaMap)
        {
            count = BitConverter.ToUInt16(data, idx);
            idx = idx + 2;
            if (count != 0)
            {
                count1 = BitConverter.ToUInt16(data, idx);
                idx = idx + 2;
            }
            staticObjects = new StaticObject[count];
            for (int i = 0; i < staticObjects.Length; i++)
            {
                staticObjects[i] = new StaticObject();
            }

            for (int i = 0; i < count; i++)
            {
                staticObjects[i].IsAnim = data[idx];
                idx++;
                staticObjects[i].ImgIndex = BitConverter.ToUInt16(data, idx);
                idx = idx + 2;
                staticObjects[i].X = BitConverter.ToUInt16(data, idx);
                idx = idx + 2;
                staticObjects[i].Y = BitConverter.ToUInt16(data, idx);
                idx = idx + 2;

                if (isElsaMap)
                {
                    staticObjects[i].World = worldId3;
                }
                else
                {
                    staticObjects[i].World = staticObjects[i].ImgIndex / 1000;
                }
            }

            return idx;
        }

    }

    class MapAni
    {
        public int FillData(byte[] data, int height, int width, int idx)
        {
            for (int i = 0; i <= (height - 1) / 4; i++)
            {
                for (int j = 0; j <= (width - 1) / 2; j++)
                {
                    idx = idx + 2;//의미없는구간
                }
            }
            return idx;
        }
    }

    class MapTileInfo
    {
        uint flag = 0;
        ushort tileIndex = 0;
        int[][] layers = new int[3][];
        uint[] layerFlags = new uint[0];

        uint size = 0;

        //public uint Flag { get => flag;   }
        //public ushort TileIndex { get => tileIndex;  }
        public int[][] Layers { get => layers;   }
        public uint[] LayerFlags { get => layerFlags;   }
        public uint Size { get => size;  }

        public int FillData(byte[] datas, int idx, uint height, uint width, ushort[] worldId)
        {
            size = height * width;
            layerFlags = new uint[size];

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = new int[size];
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    tileIndex = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;
                    //layers[j][i] = tileIndex == 0 ? 0 : tileIndex + worldId[j] * 100000;//maybe it is remove
                    layers[j][i] = tileIndex == 0 ? 0 : tileIndex;//추정임.
                }
                flag = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
                layerFlags[i] = flag == 0 ? 0 : flag + 1000000;
            }
            return idx;
        }

    }

}
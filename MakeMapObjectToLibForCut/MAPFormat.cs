using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MakeMapObjectToLibForCut
{
    class Portal
    {
        uint mapID;
        uint x;
        uint y;

        public uint MapID { get => mapID; set => mapID = value; }
        public uint X { get => x; set => x = value; }
        public uint Y { get => y; set => y = value; }
    }
    class PortalFormat
    {
        public Dictionary<ushort, List<Portal>> portals = new Dictionary<ushort, List<Portal>>();

        ushort unk;
        uint count;

        public int FillData()
        {
            int idx = 0;
            try
            {
                byte[] datas = ReadByteFile("portals.tbl");

                //바로 읽기
                unk = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                count = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                for (int i = 0; i < count; ++i)
                {
                    var mapID = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;
                    var x = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;
                    var y = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;

                    Portal portal = new Portal();
                    portal.MapID = mapID;
                    portal.X = x;
                    portal.Y = y;

                    if (!portals.ContainsKey(mapID))
                    {
                        portals.Add(mapID, new List<Portal>());
                    }
                    portals[mapID].Add(portal);
                }

                return idx;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private byte[] ReadByteFile(string filePath)
        {
            byte[] datas;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;
                datas = new byte[length];
                int count;
                int sum = 0;

                while ((count = fileStream.Read(datas, sum, length - sum)) > 0)
                {
                    sum += count;
                }
            }
            finally
            {
                fileStream.Close();
            }
            return datas;
        }

    }
    class MAPFormat
    {
        public MAPHeader mapHeader = new MAPHeader();
        public MapAni mapAni = new MapAni();
        public MapTileInfo mapTileInfo = new MapTileInfo();
        public StaticObjectInfo staticObjectInfos = new StaticObjectInfo();
        public PortalFormat portalFormat = new PortalFormat();

        public MAPFormat(byte[] datas, string filename)
        {
            int idx = portalFormat.FillData();
            if (idx == -1) Console.WriteLine("Portal Read Fail!");
            idx = mapHeader.FillData(datas, filename);
            idx = mapAni.FillData(datas, mapHeader.Height, mapHeader.Width, idx);
            idx = mapTileInfo.FillData(datas, idx, mapHeader.Height, mapHeader.Width, mapHeader.WorldId);
            idx = staticObjectInfos.FillData(datas, idx, mapHeader.WorldId[3], mapHeader.IsElsaMap);
            if (idx == -1) return;
        }

        public void ConvertToM2MAP(string filename,List<string> objectsPosition)
        {
            M2MapMgr m2MapMgr = new M2MapMgr();
            m2MapMgr.New(mapHeader.Width, mapHeader.Height);

            int exineMapIdx = 0;

            int mapNumber = Convert.ToInt32(filename.Replace(".map", ""));
            Console.WriteLine(mapNumber);

            
            for (int i = 0; i < mapHeader.Height; i++)
            {
                for (int j = 0; j < mapHeader.Width; j++)
                {
                    exineMapIdx = i * mapHeader.Width + j;


                    //ground
                    m2MapMgr.SetBackTileSetIdx((short)mapHeader.WorldId[0], j, i);
                    m2MapMgr.SetBackImgIdx((short)(mapTileInfo.Layers[0][exineMapIdx]), j, i);


                    //mid layer
                    m2MapMgr.SetMidTileSetIdx((short)mapHeader.WorldId[1], j, i);
                    //if (mapTileInfo.Layers[1][exineMapIdx] != 0)
                    {
                        m2MapMgr.SetMidImgIdx((short)(mapTileInfo.Layers[1][exineMapIdx]), j, i);
                    }

                    //커터를 이용하여 나온 Map_10xxx_FrontTile.lib 타일 파일을 읽어서 순서대로 반영시킴 
                    //Select tileset (number => 10007 -> -10000 + 1000 -> 1007 (from file name : 10007.map))
                    for(int objectIdx=0; objectIdx < objectsPosition.Count;objectIdx++)
                    {
                            if (objectsPosition[objectIdx].Equals(j + "," + i))
                            {
                                Console.WriteLine("objectsPosition[objectIdx].Equals(j + \",\" + i)!!! x:" + j + " y:" + i + " objectIdx:" + objectIdx);
                                //Console.ReadLine();
                                m2MapMgr.SetFrontTileSetIdx((short)(mapNumber - 9000), j, i);
                                m2MapMgr.SetFrontImgIdx((short)(objectIdx+1), j, i);
                                //tile index 
                                //It must +1
                            }
                    }

                    if (mapTileInfo.LayerFlags[exineMapIdx] != 0) m2MapMgr.SetFrontLimit(j, i);



                    #region old static object
                    /*
                    //front layer(front는 좌표값 그대로 찍게하면? 타일은 크기값 그대로 하게하면?) - 구조상 불가함.               
                    m2MapMgr.SetFrontTileSetIdx((short)mapHeader.WorldId[2], j, i);
                    //if (mapTileInfo.Layers[2][exineMapIdx] != 0)
                    {
                        m2MapMgr.SetFrontImgIdx((short)(mapTileInfo.Layers[2][exineMapIdx]), j, i);
                    }
                    if (mapTileInfo.LayerFlags[exineMapIdx] != 0) m2MapMgr.SetFrontLimit(j, i);


                    //Static Object 부분 FrontLayer에 올리는거 추가 시도 
                    foreach (var item in staticObjectInfos.StaticObjects)
                    {
                        int x = (int)Math.Truncate((double)item.X / 48);
                        int y = (int)Math.Truncate((double)item.Y / 24);
                        //if(x==j && y==i)

                        if (item.X / 48 == j && item.Y / 24 == i)
                        {
                            Console.WriteLine("Add Static Object World val:" + item.World);//x,y가 cell 좌표가 아닌 그냥 좌표일것임.
                            Console.WriteLine("Add Static Object ImgIndex val:" + item.ImgIndex);//x,y가 cell 좌표가 아닌 그냥 좌표일것임.

                            Console.WriteLine("Add Static Object val:" + (item.ImgIndex - item.World * 1000));//x,y가 cell 좌표가 아닌 그냥 좌표일것임.
                            m2MapMgr.SetFrontTileSetIdx((short)(item.World + 10), j, i);//object start idx:9 
                            m2MapMgr.SetFrontImgIdx((short)((item.ImgIndex + 1) % 1000), j, i);//object를 불러와야함  //object는 index 10~19까지임.

                            //m2MapMgr.SetFrontTileSetIdx((short)(item.World + 9), j, i);//object start idx:9 
                            //m2MapMgr.SetFrontImgIdx((short)((item.ImgIndex + 1) % 1000), j, i);//object를 불러와야함  //object는 index 10~19까지임.
                            
                            //staticObjectInfos.StaticObjects[i].IsAnim 
                        }
                        //m2MapMgr.SetFrontImgIdx(staticObjectInfos[i].is)
                    }
                    */

                    /*
                    //BackTiled is not exine use(this is for mir3 tile maybe)
                    //m2MapMgr.SetBackTileSetIdx((short)mapHeader.WorldId[0],j,i);
                    //m2MapMgr.SetBackImgIdx(mapTileInfo.Layers[0][exineMapIdx], j, i);
                    //if(mapTileInfo.LayerFlags[exineMapIdx]!=0) m2MapMgr.SetBackLimit( j, i);


                    //ground
                    m2MapMgr.SetMidTileSetIdx((short)mapHeader.WorldId[0], j, i);
                    m2MapMgr.SetMidImgIdx((short)(mapTileInfo.Layers[0][exineMapIdx]), j, i);


                    //front layer1
                    m2MapMgr.SetFrontTileSetIdx((short)mapHeader.WorldId[1], j, i);
                    if (mapTileInfo.Layers[1][exineMapIdx] != 0)
                    {
                        m2MapMgr.SetFrontImgIdx((short)(mapTileInfo.Layers[1][exineMapIdx]), j, i);
                    }
                    
                    
                    //front layer2                    
                    m2MapMgr.SetFrontTileSetIdx((short)mapHeader.WorldId[2], j, i);
                    if (mapTileInfo.Layers[2][exineMapIdx] != 0)
                    {
                        m2MapMgr.SetFrontImgIdx((short)(mapTileInfo.Layers[2][exineMapIdx]), j, i);
                    }
                    if (mapTileInfo.LayerFlags[exineMapIdx] != 0) m2MapMgr.SetFrontLimit(j, i);
                    */

                    //portal info is on server

                    //frontAnimationFrame is count?
                    //이부분은 TileCutter를 이용하여 처리할 예정임.

                    //test
                    //결국 방법은 하나로 만들어서 바로 올리기기
                    /*
                    foreach (var item in staticObjectInfos.StaticObjects)
                    {
                        int x = (int)Math.Truncate((double)item.X / 48);
                        int y = (int)Math.Truncate((double)item.Y / 24);
                        //if(x==j && y==i)

                        if (item.X / 48 == j && item.Y / 24 == i)
                        {
                            Console.WriteLine("Add Static Object val:" + (item.ImgIndex - item.World * 1000));//x,y가 cell 좌표가 아닌 그냥 좌표일것임.
                            m2MapMgr.SetFrontTileSetIdx((short)(item.World + 9), j, i);//object start idx:9
                            m2MapMgr.SetFrontImgIdx((short)((item.ImgIndex + 1 )% 1000), j, i);//object를 불러와야함

                            //staticObjectInfos.StaticObjects[i].IsAnim 
                        }
                        //m2MapMgr.SetFrontImgIdx(staticObjectInfos[i].is)
                    }*/
                    #endregion old static object
                }
            }
            /*
            for (int i = 0; i < mapHeader.Height; i++)
            {
                for (int j = 0; j < mapHeader.Width; j++)
                {
                    exineMapIdx = i * mapHeader.Width + j;


                    //draw object
                    //Console.WriteLine("staticObjectInfos.Count:" + staticObjectInfos.Count);
                    foreach (var item in staticObjectInfos.StaticObjects)
                    {
                        if (item.X / 48 == j && item.Y / 24 == i)
                        {
                            Console.WriteLine("Add Static Object val:"+ (item.ImgIndex - item.World * 1000));//x,y가 cell 좌표가 아닌 그냥 좌표일것임.
                            m2MapMgr.SetFrontTileSetIdx((short)(item.World + 9), j, i);//object start idx:9
                            m2MapMgr.SetFrontImgIdx((short)(item.ImgIndex- item.World*1000), j, i);//object를 불러와야함

                            //staticObjectInfos.StaticObjects[i].IsAnim 
                        }
                        //m2MapMgr.SetFrontImgIdx(staticObjectInfos[i].is)
                    }

                }
            }*/



            m2MapMgr.Save(filename);

            //idx (lib index in libs)
            //img (frame index in lib)

            //back => just layer
            //mid => just layer
            //front => just layer

            /*
             * -set-
             *   M2CellInfo[cellX, cellY].BackImage = M2CellInfo[cellX, cellY].BackImage | 0x20000000; 
             *   M2CellInfo[cellX, cellY].FrontImage = (short) (M2CellInfo[cellX, cellY].FrontImage | 0x8000);
             *  
             *  -clear-
             *   M2CellInfo[cellX, cellY].BackImage = M2CellInfo[cellX, cellY].BackImage & 0x1fffffff;
             *   M2CellInfo[cellX, cellY].FrontImage = (short) (M2CellInfo[cellX, cellY].FrontImage & 0x7fff);
             */

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

                Console.WriteLine("worldId3:" + worldId3);
                if (isElsaMap)
                {
                    Console.WriteLine("It is elsa map!");
                    staticObjects[i].World = worldId3;
                }
                else
                {
                    Console.WriteLine("It is exine map!");
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
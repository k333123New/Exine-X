using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewYPF
{ 
    public class CellInfo
    {
        public short BackIndex;
        public int BackImage;
        public short MiddleIndex;
        public short MiddleImage;
        public short FrontIndex;
        public short FrontImage;

        public byte DoorIndex;
        public byte DoorOffset;

        public byte FrontAnimationFrame;
        public byte FrontAnimationTick;

        public byte MiddleAnimationFrame;
        public byte MiddleAnimationTick;

        public short TileAnimationImage;
        public short TileAnimationOffset;
        public byte TileAnimationFrames;

        public byte Light;
        public byte Unknown;

        public bool FishingCell;

    }
    class M2MapMgr
    {
        CellInfo[,] mapInfo = new CellInfo[0, 0];
        int width = 1000;
        int height = 1000;

        public void New(int width, int height)
        {
            this.width = width;
            this.height = height;
            mapInfo = new CellInfo[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    mapInfo[x, y] = new CellInfo();
                }
            }
        }

        public void SetFrontTileSetIdx(short tileIdx, int x, int y)
        {
            mapInfo[x, y].FrontIndex = tileIdx;
        } 
        public void SetFrontImgIdx(short imgIdx, int x, int y)
        {
            mapInfo[x, y].FrontImage = imgIdx;
        } 
        public void SetFrontLimit(int x, int y)
        {
            mapInfo[x, y].FrontImage = (short)(mapInfo[x, y].FrontImage | 0x8000);
        }


        public void SetMidTileSetIdx(short tileIdx, int x, int y)
        {
            mapInfo[x, y].MiddleIndex = tileIdx;
        }
        public void SetMidImgIdx(short imgIdx, int x, int y)
        {
            mapInfo[x, y].MiddleImage = imgIdx;
        }

        public void SetBackTileSetIdx(short tileIdx, int x, int y)
        {
            mapInfo[x, y].BackIndex = tileIdx;
        }
        public void SetBackImgIdx(int imgIdx, int x, int y)
        {
            mapInfo[x, y].BackImage = imgIdx;
        }

        public void SetBackLimit(int x, int y)
        {
            mapInfo[x, y].BackImage = mapInfo[x, y].BackImage | 0x20000000;
        }

        public void Save(string filename)
        {
            DirectoryInfo di = new DirectoryInfo(".\\MAP_OUT");
            if (!di.Exists) di.Create();


            var fileStream = new FileStream(".\\MAP_OUT\\"+filename, FileMode.Create);
            var binaryWriter = new BinaryWriter(fileStream);
            short ver = 1;
            char[] tag = { 'C', '#' };
            binaryWriter.Write(ver);
            binaryWriter.Write(tag);

            binaryWriter.Write(Convert.ToInt16(width));
            binaryWriter.Write(Convert.ToInt16(height));
            for (var x = 0; x <width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    binaryWriter.Write(mapInfo[x, y].BackIndex); //lib index in libs
                    binaryWriter.Write(mapInfo[x, y].BackImage); //frame index in lib
                    binaryWriter.Write(mapInfo[x, y].MiddleIndex);
                    binaryWriter.Write(mapInfo[x, y].MiddleImage);
                    binaryWriter.Write(mapInfo[x, y].FrontIndex);
                    binaryWriter.Write(mapInfo[x, y].FrontImage);
                    binaryWriter.Write(mapInfo[x, y].DoorIndex);
                    binaryWriter.Write(mapInfo[x, y].DoorOffset);
                    binaryWriter.Write(mapInfo[x, y].FrontAnimationFrame);
                    binaryWriter.Write(mapInfo[x, y].FrontAnimationTick);
                    binaryWriter.Write(mapInfo[x, y].MiddleAnimationFrame);
                    binaryWriter.Write(mapInfo[x, y].MiddleAnimationTick);
                    binaryWriter.Write(mapInfo[x, y].TileAnimationImage);
                    binaryWriter.Write(mapInfo[x, y].TileAnimationOffset);
                    binaryWriter.Write(mapInfo[x, y].TileAnimationFrames);
                    binaryWriter.Write(mapInfo[x, y].Light);
                }
            }
            binaryWriter.Flush();
            binaryWriter.Dispose();

        }
    }


}

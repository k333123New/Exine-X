using MapObjectToLibTilesForCut;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMapObjectToLibForCut
{
    //기존방식 : 고정객체의 배치를 맞추고 이미지 레이어로 선택 -> 고정객체 앞뒤 판단이 불가
    //신규 예정 : 고정객체의 배치를 맞추고 동일한 고정 객체여도 좌표값을 새로 수정하여 별도로 저장 및 맵별 lib 저장 및 좌표 정보 저장
    class FrameWithPosition
    {
        public Frame frame;
        public int cellX;
        public int cellY;
        public bool isAnim = false;
        public int tilesetIdx;
        public int imgIdx;
    }

    class MainClass
    {
        public void Main(string[] args)
        {
            int cellWidth = 48;
            int cellHeight = 24;
            Console.WriteLine("Start!");

            string[] filenames = new string[] { "10005.map" };
            List<String> inputFileNames = new List<string>();

            if (args.Length > 0)
            {
                if (args[0] == "*.map")
                {
                    //Console.WriteLine("* Map");
                    //해당 경로에서 파일 목록 가져오기

                    DirectoryInfo di = new DirectoryInfo(@".\\");

                    FileInfo[] files = di.GetFiles("*.map");

                    foreach (FileInfo file in files)
                    {
                        Console.WriteLine(file.Name);

                        if (File.Exists(file.Name))
                        {
                            inputFileNames.Add(file.Name);
                        }
                    }
                }
                else
                {
                    filenames = args;
                }
            }
            else
            {
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename))
                    {
                        inputFileNames.Add(filename);
                    }
                }
            }

            foreach (string filename in inputFileNames)
            {

                Console.WriteLine("Start Convert MapAndObjects to MapStaticImageLib. Filename:" + filename);
                MAPFormat mapFormat = new MAPFormat(ReadByteFile(filename), filename);

                //이 지도에서 필요한 Object 파일 정보를 가져와서 해당 파일만 읽어온다.
                StaticObjectInfo staticObjectInfos = mapFormat.staticObjectInfos;
                if (staticObjectInfos == null) return;

                Console.WriteLine("static object count:" + staticObjectInfos.Count);
                Console.WriteLine("===========Static Object Info===========");


                System.IO.FileInfo fileObjectSizeInfo = new System.IO.FileInfo(".\\CachedObjectSize\\");
                fileObjectSizeInfo.Directory.Create();
                string fileType = "";

                var dictionary = new Dictionary<FrameWithPosition, int>(staticObjectInfos.Count);
                List<FrameWithPosition> frameWithPositions = new List<FrameWithPosition>();
                Frame frame = null;
                YPFFormat yPFFormat = null;

                for (int i = 0; i < staticObjectInfos.StaticObjects.Length; i++)
                {
                    fileType = "";
                    frame = null;
                    yPFFormat = null;

                    FrameWithPosition outputFrameWithPosition = new FrameWithPosition();

                    var staticObject = staticObjectInfos.StaticObjects[i];
                    int frameWidth = 0;
                    int frameHeight = 0;
                    int ypfImageSetIdx = (staticObject.ImgIndex - staticObject.World * 1000);
                    outputFrameWithPosition.tilesetIdx = staticObject.World;
                    outputFrameWithPosition.imgIdx = ypfImageSetIdx;
                    outputFrameWithPosition.isAnim = false;
                    if (staticObject.IsAnim == 0x01)
                    {
                        outputFrameWithPosition.isAnim = true;
                        fileType = "Anim";
                        Console.WriteLine("IsAnimObject! fileType:" + fileType + " item.World:" + staticObject.World + " ypfImageSetIdx:" + ypfImageSetIdx);
                    }

                    Console.WriteLine("ypfImageSetIdx:" + ypfImageSetIdx);
                    //anim's ypfImageSetIdx is ypfImageSets idx...!

                    yPFFormat = ConvertYpfToRGBA("TS_0" + staticObject.World + "_" + fileType + "Static.ypf");
                    //if (staticObject.IsAnim == 0x01)
                    if (fileType == "Anim")
                    {
                        Console.WriteLine("staticObject.IsAnim == 0x01");
                        frame = yPFFormat.ypfImageSets[ypfImageSetIdx].FrameInfo.frames[0];
                    }
                    else
                    {
                        try
                        {
                            Console.WriteLine("TS_0" + staticObject.World + "_" + fileType + "Static.ypf Load!!!");
                            Console.WriteLine("staticObject.IsAnim != 0x01 yPFFormat.ypfImageSets.Len:"+ yPFFormat.ypfImageSets.Length+ " yPFFormat.ypfImageSets[0].FrameInfo.frames.len:"+ yPFFormat.ypfImageSets[0].FrameInfo.frames.Length+ "ypfImageSetIdx:"+ ypfImageSetIdx);
                            frame = yPFFormat.ypfImageSets[0].FrameInfo.frames[ypfImageSetIdx];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            Console.ReadLine();
                            continue;
                        }
                    }
                    frameWidth = frame.FrameWidth;
                    frameHeight = frame.FrameHeight;

                    //no anime is frames[0]
                    //anime is frames[ypgImageSetIdx][0]
                    frame.left = staticObject.X % cellWidth + frame.left;
                    frame.right = staticObject.X % cellWidth + frame.right;
                    frame.top = staticObject.Y % cellHeight + frame.top;
                    frame.bottom = staticObject.Y % cellHeight + frame.bottom;
                    outputFrameWithPosition.frame = frame;

                    //int x = (int)Math.Truncate((double)item.X / 48);
                    //int y = (int)Math.Truncate((double)item.Y / 24);

                    //outputFrameWithPosition.cellX = staticObject.X / cellWidth; //for apply map front position
                    //outputFrameWithPosition.cellY = staticObject.Y / cellHeight; //for apply map front position
                    //outputFrameWithPosition.cellX = (int)Math.Truncate((double)staticObject.X / cellWidth); //for apply map front position
                    //outputFrameWithPosition.cellY = (int)Math.Truncate((double)staticObject.Y / cellHeight); //for apply map front position

                    outputFrameWithPosition.cellX = (int)Math.Truncate((double)(staticObject.X / cellWidth)); //for apply map front position
                    outputFrameWithPosition.cellY = (int)Math.Truncate((double)(staticObject.Y/ cellHeight)); //for apply map front position

                    dictionary.Add(outputFrameWithPosition, (staticObject.Y + frame.FrameHeight / 2) * 400000 + (staticObject.X + frame.FrameWidth / 2) * 1000);
                }

                //sorting
                Console.WriteLine("Sorting");
                var outputFrameWithPositions = from pair in dictionary
                                               orderby pair.Value ascending
                                               select pair;

                Console.WriteLine("Save Output");
                //int frameIdx = 0;
                File.Delete("Map_" + filename.Replace(".map", "") + "_FrontTile.lib");

                MLibraryV2 mLibraryV2 = new MLibraryV2("Map_" + filename.Replace(".map","") + "_FrontTile.lib");
                //front layer position to save txt tile.
                //Map Editor apply frontlayer index from txt
                //StringBuilder sb = new StringBuilder();
                List<string> objectsPosition = new List<string>();
                int total = outputFrameWithPositions.Count();
                int nowIndex = 0;

                Bitmap bitmap = null;
                foreach (KeyValuePair<FrameWithPosition, int> pair in outputFrameWithPositions)
                {
                    Console.WriteLine(nowIndex+"/" +total);
                    FrameWithPosition outputFrameWithPosition = pair.Key;
                    //outputFrameWithPosition.frame

                    fileType = "";
                    //Bitmap from file
                    if(outputFrameWithPosition.isAnim)
                    {
                        fileType = "Anim";
                    }
                    string imgFileName = @".\CachedObject\" + fileType+outputFrameWithPosition.tilesetIdx + "_" + outputFrameWithPosition.imgIdx + ".png";
                    Console.WriteLine("Try to find cached image data..." + imgFileName);

                    if (File.Exists(imgFileName))
                    {
                        Console.WriteLine("Found ImgFile : " + imgFileName);
                        bitmap = new Bitmap(imgFileName);
                       
                    }
                    else
                    {
                        Console.WriteLine("Not Found ImgFile : " + imgFileName);
                        Console.WriteLine("Try To Make ImgFile : " + imgFileName);

                        bitmap = outputFrameWithPosition.frame.ConvertToLib();
                        bitmap.Save(imgFileName);
                        Console.WriteLine("Make OK ImgFile : " + imgFileName);
                    }
                    mLibraryV2.AddImage(bitmap, (short)outputFrameWithPosition.frame.left, (short)outputFrameWithPosition.frame.top);

                    objectsPosition.Add(outputFrameWithPosition.cellX + "," + outputFrameWithPosition.cellY);

                    //frontlayer[frameWithPosition.CellX, frameWithPosition.CellY] = frameIdx;
                    //frameIdx++;
                    nowIndex++;
                }
                mLibraryV2.Save();
                File.WriteAllLines("Map_" + filename + "_FrontTile.txt", objectsPosition);

                //Convert to Exine-X Map With FrontLayer
                ConvertMapToM2MapWithFront(filename, objectsPosition);
            } 

        }

        public YPFFormat ConvertYpfToRGBA(string filename)
        {
            Console.WriteLine("Start ConvertYpfToRGBA. Filename:" + filename);
            byte[] datas = ReadByteFile(filename);

            YPFFormat ypfFormat = new YPFFormat(datas);
            //ypfFormat.SaveFile(filename);//사용 안하면 제거
            //ypfFormat.ConvertToLib(filename);
            Console.WriteLine("Finish ConvertYpfToRGBA. Filename:" + filename);
            return ypfFormat;
        }

        void ConvertMapToM2MapWithFront(string filename, List<string> objectsPosition)
        {
            //load object position txt
            //10005.map
            //Map_10005.map_FrontTile.txt

            Console.WriteLine("Start ConvertMapToM2Map. Filename:" + filename);
            byte[] datas = ReadByteFile(filename);
            MAPFormat mapFormat = new MAPFormat(datas, filename);
            mapFormat.ConvertToM2MAP(filename, objectsPosition); 
        }


        public byte[] ReadByteFile(string filePath)
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
}


//sudo code
/*
 * int CellWidth = 48;
 * int CellHeight = 24;
 * var dictionary = new Dictionary<FrameWithPosition, int>(map.staticObjs.Length);
 * List<FrameWithPosition> frameWithPositions = new FrameWithPosition();
 * 
 * for(int i=0;i<map.staticObjs.len;i++)
 * {
 *   FrameWithPosition outputFrameWithPosition = new FrameWithPosition();  
 *   Frame frame = Load(map.staticObjs[i].ObjectPack_Idx,map.staticObjs[i].ObjectFrame_Idx)
 *   
 *   outputFrameWithPosition.OffsetX = map.staticObj[i].x % CellWidth + frame.OffsetX;
 *   outputFrameWithPosition.OffsetY = map.staticObj[i].y % CellHeight + frame.OffsetY;
 *   //outputFrameWithPosition.frame = frame.ImageData;
 *   outputFrameWithPosition.frame = frame;
 *   outputFrameWithPosition.CellX = map.staticObj[i].x / CellWidth;
 *   outputFrameWithPosition.CellY = map.staticObj[i].y / CellHeight;  
 *   
 *   //frame.Height, frame.Width
 *   dictionary.add(outputFrameWithPosition,(map.staticObj[i].y + frameHeight / 2) * 400000 + (map.staticObj[i].x + frameWidth / 2) * 1000);
 *   
 * } 
 * 
 * -Sorting-
 *  var items = from pair in dictionary
                orderby pair.Value ascending
                select pair;
 * 
 * int frameIdx=0;
 * List<Frame> frames = new List<Frame>();
 * 
 * foreach (KeyValuePair<OutputFrameWithPosition, int> pair in items)
 * {
 *  FrameWithPosition frameWithPosition = pair.Key;
 *  frames.add(frameWithPosition.frame);
 * }
 * foreach (KeyValuePair<OutputFrameWithPosition, int> pair in items)
  {
    FrameWithPosition frameWithPosition = pair.Key;
    //map의 objectPack_Idx = 해당 파일의 libIdx
    //map의 frontlayer[frameWithPosition.CellX, frameWithPosition.CellY] = frameIdx;
    frameIdx++;
  }
 */
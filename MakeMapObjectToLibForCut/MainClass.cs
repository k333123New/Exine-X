using MapObjectToLibTilesForCut;
using NewYPF;
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
    class MainClass
    {
        public void Main(string[] args)
        {
            Console.WriteLine("Start!");

            string[] filenames = new string[] { "10000.map" };
            List<String> inputFileNames = new List<string>();

            if (args.Length > 0) filenames = args;

            foreach (string filename in filenames)
            {
                if (File.Exists(filename))
                {
                    inputFileNames.Add(filename);
                }
            }

            foreach (string filename in inputFileNames) 
            { 

                Console.WriteLine("Start Convert MapAndObjects to Png Image. Filename:" + filename); 
                MAPFormat mapFormat = new MAPFormat(ReadByteFile(filename), filename);
                int mapWidth = mapFormat.mapHeader.Width * 48;
                int mapHeight = mapFormat.mapHeader.Width * 24;

                //이 지도에서 필요한 Object 파일 정보를 가져와서 해당 파일만 읽어온다.
                Console.WriteLine("Real Map Image Size:" + "("+ mapWidth + "," + mapFormat.mapHeader.Height*24 + ")");
                byte[] outputImageData = new byte[mapWidth *4 * mapHeight * 4];//ARGB

                Console.WriteLine("static object count:"+mapFormat.staticObjectInfos.Count);
                if (mapFormat.staticObjectInfos == null) return;

                Console.WriteLine("===========Static Object Info===========");


                int prevWorld = -1;
                int prevYpfImageSetIdx = -1;
                byte[] prevColorDataRGBA = null;


                //draw order sort
                var dictionary = new Dictionary<StaticObject, int>(mapFormat.staticObjectInfos.StaticObjects.Length);
                for (int i = 0; i < mapFormat.staticObjectInfos.StaticObjects.Length; i++)
                {
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X*10 - mapFormat.staticObjectInfos.StaticObjects[i].Y);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X * 2 - mapFormat.staticObjectInfos.StaticObjects[i].Y);
                    dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X * 2 - mapFormat.staticObjectInfos.StaticObjects[i].Y*4);
                }
                
                // Order by values. 
                // ... Use LINQ to specify sorting by value. 
                var items = from pair in dictionary 
                            orderby pair.Value descending 
                            select pair;

                // Display results. 
                /*
                foreach (KeyValuePair<StaticObject, short> pair in items) 
                { 
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value); 
                }
                */


                //foreach (var item in mapFormat.staticObjectInfos.StaticObjects)
                foreach (KeyValuePair<StaticObject, int> pair in items)
                {
                    StaticObject item = pair.Key;

                    if (item.IsAnim==0x01)
                    {
                        Console.WriteLine("IsAnimObject!");
                      //  Console.ReadLine();
                    }
                    Console.Write("World(TS Number):" + item.World);
                    Console.Write("\tImage_Index:" + (item.ImgIndex - item.World * 1000));
                    Console.Write("\tImage_Real_Position:" +"("+item.X+","+item.Y+")");
                    Console.WriteLine("\titem.Y:" + item.Y + "item.X:" + item.X );

                    //staticObjectInfos.StaticObjects[i].IsAnim 
                    //m2MapMgr.SetFrontImgIdx(staticObjectInfos[i].is)

                    int ypfImageSetIdx = (item.ImgIndex - item.World * 1000);
                    //ts_object 이미지를 불러와서 비트맵 정보와 위치 정보를 기반으로 outputImageData에 데이터를 넣어준다.
                    //TS_04_Static.ypf

                    //한번에 정보를 다 일고 시작함!
                    //if(item.IsAnim==0x01)YPFImageSet[] yPFImageSets =  ConvertYpfToRGBA("TS_0"+ item.World + "_AnimStatic.ypf").ypfImageSets;


                    YPFImageSet[] yPFImageSets =  ConvertYpfToRGBA("TS_0"+ item.World + "_Static.ypf").ypfImageSets;
                    Console.WriteLine("yPFImageSets.Length:"+yPFImageSets.Length );

                    Frame image= yPFImageSets[0].FrameInfo.frames[ypfImageSetIdx];
                    int imageWidth = image.FrameWidth;
                    int imageHeight = image.FrameHeight;
                    //image.SaveImage("0_" + ypfImageSetIdx + "saveimg.png");


                    //새로운 이미지라면 해당 이미지 정보는 압축을 푼 상태로 그대로 raw파일로 저장해서 cacheFrame폴더에 만들어두기.
                    //해당되는 프레임의 이미지가 이미 풀려져 있는 경우 해당 데이터를 바로 읽어서 사용하는 것으로 처리할것.
                    //그릴때 순서를 정해서 그려야 할듯함.
                    //오른쪽 위부터 대각선으로 내려가고 위쪽을 먼저 그려야함.


                    byte[] colorDataRGBA = null;
                    System.IO.FileInfo file = new System.IO.FileInfo(".\\CachedObject\\");
                    file.Directory.Create();

                    //if (prevWorld == item.World && prevYpfImageSetIdx == ypfImageSetIdx)
                    if (File.Exists(".\\CachedObject\\" + item.World + "_" + ypfImageSetIdx+".raw"))
                    {
                        Console.WriteLine("Exist Frame Image! load frame image From Exist Data!");
                        colorDataRGBA =  File.ReadAllBytes(".\\CachedObject\\" + item.World + "_" + ypfImageSetIdx + ".raw");
                    }
                    else
                    {
                        prevWorld = item.World;
                        prevYpfImageSetIdx = ypfImageSetIdx;
                        colorDataRGBA = image.GetcolorDataRGBA();
                        File.WriteAllBytes(".\\CachedObject\\"+ item.World+"_"+ ypfImageSetIdx+".raw", colorDataRGBA);
                        
                        //prevColorDataRGBA = new byte[colorDataRGBA.Length];
                        //Buffer.BlockCopy(colorDataRGBA, 0, prevColorDataRGBA, 0, colorDataRGBA.Length);
                    }

                    Console.WriteLine("ypfImageSetIdx:"+ ypfImageSetIdx+" imageWidth:" + imageWidth + " imageHeight" + imageHeight);

                    for (int x = 0; x < imageWidth * 4; x = x + 4) //174 => 0~173*4 bytes
                    {
                        for (int y = 0; y < imageHeight ; y++)//153 => 174*4*0+0 ~ 174*4*0+152 , 174*4*1+0~174*4*1+152, ...174+2+0
                        {
                            //전체 맵 이미지 특정 위치에 계산해서 넣어야함.
                            /*
                            outputImageData[y * (mapWidth * 4) + x + 0] = colorDataRGBA[y * imageWidth * 4 + x]; //b//0x00;//b?
                            outputImageData[y * (mapWidth * 4) + x + 1] = colorDataRGBA[y * imageWidth * 4 + x + 1];//g
                            outputImageData[y * (mapWidth * 4) + x + 2] = colorDataRGBA[y * imageWidth * 4 + x + 2];//r
                            outputImageData[y * (mapWidth * 4) + x + 3] = colorDataRGBA[y * imageWidth * 4 + x + 3];//a
                            */
                            if (colorDataRGBA[y * (imageWidth * 4) + x + 3] != 0x00)
                            {
                                try
                                {
                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 0)] = colorDataRGBA[y * (imageWidth * 4) + x]; //b//0x00;//b?
                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 1)] = colorDataRGBA[y * (imageWidth * 4) + x + 1];//g
                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 2)] = colorDataRGBA[y * (imageWidth * 4) + x + 2];//r
                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 3)] = colorDataRGBA[y * (imageWidth * 4) + x + 3];//a
                                }
                                catch(Exception ex)
                                {
                                    continue;
                                }
                            }


                            //image.Top; //offset x ??
                            // image.Left; //offset y ???
                            //Console.WriteLine("item.Y:" + item.Y + "item.X:" + item.X );
                            //Console.ReadLine();
                        }
                    }

                    
                    //Bitmap bitmap = new Bitmap(mapWidth, mapHeight, PixelFormat.Format16bppRgb565);
                    Bitmap bitmap = new Bitmap(mapWidth, mapHeight, PixelFormat.Format32bppArgb);
                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                    int rowSize = Math.Abs(bmpData.Stride);
                    IntPtr bmpScan0 = bmpData.Scan0;
                    for (int y = 0; y < mapHeight; y++)
                    {
                        Marshal.Copy(outputImageData, y * mapWidth * 4, IntPtr.Add(bmpScan0, y * rowSize), mapWidth * 4);
                    }

                    bitmap.UnlockBits(bmpData);
                    bitmap.Save("0_" + filename + ".png", ImageFormat.Png);
                    bitmap.Dispose();
                    
                }

                /*
                Bitmap bitmap = new Bitmap(mapWidth, mapHeight, PixelFormat.Format32bppArgb);
                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                int rowSize = Math.Abs(bmpData.Stride);
                IntPtr bmpScan0 = bmpData.Scan0;
                for (int y = 0; y < mapHeight; y++)
                {
                    Marshal.Copy(outputImageData, y * mapWidth * 4, IntPtr.Add(bmpScan0, y * rowSize), mapWidth * 4);
                }

                bitmap.UnlockBits(bmpData);
                bitmap.Save("0_" + filename + ".png", ImageFormat.Png);
                bitmap.Dispose();
                */



                //Console.WriteLine("Start ConvertYpfToRGBA. Filename:" + filename); 
                //YPFFormat ypfFormat = new YPFFormat(ReadByteFile(filename));

                //ypfFormat.SaveFile(filename);//사용 안하면 제거
                //ypfFormat.ConvertToLib(filename); 
            }
             
        }
        public YPFFormat ConvertYpfToRGBA(string filename)
        {
            Console.WriteLine("Start ConvertYpfToRGBA. Filename:" + filename);
            byte[] datas = ReadByteFile(filename);

            YPFFormat ypfFormat = new YPFFormat(datas);
            //ypfFormat.SaveFile(filename);//사용 안하면 제거
            //ypfFormat.ConvertToLib(filename);
            return ypfFormat;
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

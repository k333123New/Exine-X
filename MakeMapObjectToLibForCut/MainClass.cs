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

            string[] filenames = new string[] { "10820.map" };
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
                    //return;
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

                Console.WriteLine("Start Convert MapAndObjects to Png Image. Filename:" + filename); 
                MAPFormat mapFormat = new MAPFormat(ReadByteFile(filename), filename);
                int mapWidth = mapFormat.mapHeader.Width * 48;
                int mapHeight = mapFormat.mapHeader.Height * 24;

                        //이 지도에서 필요한 Object 파일 정보를 가져와서 해당 파일만 읽어온다.
                Console.WriteLine("Real Map Image Size:" + "("+ mapWidth + "," + mapHeight + ")");
                byte[] outputImageData = new byte[mapWidth *4 * mapHeight * 4];//ARGB 넓게 쓴다고 가정하고 처리

                Console.WriteLine("static object count:"+mapFormat.staticObjectInfos.Count);
                if (mapFormat.staticObjectInfos == null) return;

                Console.WriteLine("===========Static Object Info===========");


                int prevWorld = -1;
                int prevYpfImageSetIdx = -1;
                byte[] prevColorDataRGBA = null;


                //draw order sort
                        //그릴때 순서를 정해서 그려야 할듯함. 
                        //오른쪽 위부터 대각선으로 내려가고 위쪽을 먼저 그려야함.(가중치를 부여하는 방식으로 변경하여 소팅 및 그리도록함.
                        //실제 이미지의 크기까지 고려하여 한번 더 계산하면 더 정밀해 질 것으로 보임.(Y에 실제 이미지의 크기를 더 반영해서 가중치에 넣을것)
                        //이미지를 실제로 그릴때 왼쪽에 붙어있거나 오른쪽에 붙어있으면 반대편에 이어서 그려지는 문제가 있음.
                        //그림을 그리는 시점에서 너비와 높이를 계산해서 필요 없는 부분은 제외하고 그려야 할듯함.
                        //애니타일 부분도 최소한 1프레임이라도 가져와서 그릴수 있도록 할것.(차후 추가보정)


                System.IO.FileInfo fileObjectSizeInfo = new System.IO.FileInfo(".\\CachedObjectSize\\");
                fileObjectSizeInfo.Directory.Create();
                string fileType = "";

                var dictionary = new Dictionary<StaticObject, int>(mapFormat.staticObjectInfos.StaticObjects.Length);
                for (int i = 0; i < mapFormat.staticObjectInfos.StaticObjects.Length; i++)
                {
                    //get staticobject frame size
                    fileType = "";
                    
                    int frameWidth = 0;
                    int frameHeight = 0; 
                    var item = mapFormat.staticObjectInfos.StaticObjects[i];
                    int ypfImageSetIdx = (item.ImgIndex - item.World * 1000);

                    if (item.IsAnim == 0x01)
                    {
                        fileType = "Anim";
                        Console.WriteLine("IsAnimObject! fileType:" + fileType + " item.World:" + item.World + " ypfImageSetIdx:" + ypfImageSetIdx);
                        //Console.ReadLine();
                    }

                               //기존에 크기 정보를 가져온 적이 있으면 그걸 그대로 사용하고 아니면 불러와서 저장한다.
                    if (File.Exists(".\\CachedObjectSize\\" + fileType + item.World + "_" + ypfImageSetIdx + ".txt"))
                    {
                        Console.WriteLine("Exist Frame size! load frame size From Exist Data!");
                        frameWidth = Convert.ToInt32(File.ReadAllLines(".\\CachedObjectSize\\" + fileType+item.World + "_" + ypfImageSetIdx + ".txt")[0]);
                        frameHeight = Convert.ToInt32(File.ReadAllLines(".\\CachedObjectSize\\" + fileType+item.World + "_" + ypfImageSetIdx + ".txt")[1]);
                    }
                    else
                    {
                        Console.WriteLine("ypfImageSetIdx:" + ypfImageSetIdx);
                        //anim's ypfImageSetIdx is ypfImageSets idx...!

                        Frame frame = null;
                        YPFFormat yPFFormat =  ConvertYpfToRGBA("TS_0" + item.World + "_" + fileType + "Static.ypf");
                        if (item.IsAnim == 0x01)
                        {
                            frame = yPFFormat.ypfImageSets[ypfImageSetIdx].FrameInfo.frames[0];
                        }
                        else
                        {
                            frame = yPFFormat.ypfImageSets[0].FrameInfo.frames[ypfImageSetIdx];
                        }
                        frameWidth = frame.FrameWidth;
                        frameHeight = frame.FrameHeight;
                        File.WriteAllText(".\\CachedObjectSize\\" + fileType+item.World + "_" + ypfImageSetIdx + ".txt", frameWidth + "\r\n" + frameHeight); 
                    }
                    #region hidden
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X*10 - mapFormat.staticObjectInfos.StaticObjects[i].Y);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X * 2 - mapFormat.staticObjectInfos.StaticObjects[i].Y);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], mapFormat.staticObjectInfos.StaticObjects[i].X * 2 - mapFormat.staticObjectInfos.StaticObjects[i].Y*4);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.X - frameWidth / 2) * 2 - (item.Y+ frameHeight / 2) * 4);
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.X - frameWidth / 2)*2  + (item.Y + frameHeight / 2)*4);

                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.X - frameWidth / 2));//OK
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.X - frameWidth / 2)*2 - (item.Y + frameHeight / 2)*4);//OK
                    //dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.X)*2 - (item.Y)*4);//OK
                    #endregion

                    dictionary.Add(mapFormat.staticObjectInfos.StaticObjects[i], (item.Y + frameHeight / 2) * 400000 + (item.X + frameHeight / 2) *1000 );
                     
                }

                // Order by values. 
                // ... Use LINQ to specify sorting by value. 
                var items = from pair in dictionary 
                            orderby pair.Value ascending 
                            select pair;
                
                bool aniTile = false;
                //foreach (var item in mapFormat.staticObjectInfos.StaticObjects)
                foreach (KeyValuePair<StaticObject, int> pair in items)
                {
                    StaticObject item = pair.Key;

                    aniTile = false;
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

                   
                    YPFImageSet[] yPFImageSets = null;
                    fileType = "";
                    if (item.IsAnim == 0x01)
                    {
                        aniTile = true;
                        fileType = "Anim";
                        Console.WriteLine("IsAnimObject! fileType:" + fileType + " item.World:" + item.World + " ypfImageSetIdx:" + ypfImageSetIdx);
                       // Console.ReadLine();
                    }

                    if(aniTile) yPFImageSets =  ConvertYpfToRGBA("TS_0"+ item.World + "_"+ fileType + "Static.ypf").ypfImageSets;
                    else yPFImageSets = ConvertYpfToRGBA("TS_0" + item.World + "_Static.ypf").ypfImageSets;

                    Console.WriteLine("yPFImageSets.Length:"+yPFImageSets.Length );


                    Frame image = null;
                    if (aniTile)  image=yPFImageSets[ypfImageSetIdx].FrameInfo.frames[0];
                    else image = yPFImageSets[0].FrameInfo.frames[ypfImageSetIdx];
                    int imageWidth = image.FrameWidth;
                    int imageHeight = image.FrameHeight;
                    //image.SaveImage("0_" + ypfImageSetIdx + "saveimg.png");


                    //새로운 이미지라면 해당 이미지 정보는 압축을 푼 상태로 그대로 raw파일로 저장해서 cacheFrame폴더에 만들어두기.
                    //해당되는 프레임의 이미지가 이미 풀려져 있는 경우 해당 데이터를 바로 읽어서 사용하는 것으로 처리할것.


                    
                    byte[] colorDataRGBA = null;
                    System.IO.FileInfo file = new System.IO.FileInfo(".\\CachedObject\\");
                    file.Directory.Create();
                     
                    if (File.Exists(".\\CachedObject\\" + fileType + item.World + "_" + ypfImageSetIdx+".raw"))
                    {
                        Console.WriteLine("Exist Frame Image! load frame image From Exist Data!");
                        colorDataRGBA =  File.ReadAllBytes(".\\CachedObject\\" + fileType + item.World + "_" + ypfImageSetIdx + ".raw");
                    }
                    else
                    { 
                        colorDataRGBA = image.GetcolorDataRGBA();
                        File.WriteAllBytes(".\\CachedObject\\" + fileType+ item.World+"_"+ ypfImageSetIdx+".raw", colorDataRGBA);
                        
                        //prevColorDataRGBA = new byte[colorDataRGBA.Length];
                        //Buffer.BlockCopy(colorDataRGBA, 0, prevColorDataRGBA, 0, colorDataRGBA.Length);
                    }

                    Console.WriteLine("ypfImageSetIdx:"+ ypfImageSetIdx+" imageWidth:" + imageWidth + " imageHeight" + imageHeight);

                    int boundCheck1 = 0;
                    int boundCheck2 = 0;

                    int boundCheck3 = 0;
                    int boundCheck4 = 0;
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
                                boundCheck1 = ((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 0);
                                boundCheck2 = boundCheck1 + 3;

                                boundCheck3 = y * (imageWidth * 4) + x;
                                boundCheck4 = boundCheck3 + 3;

                                if (0 > boundCheck1 || outputImageData.Length < boundCheck2) continue;
                                if (0 > boundCheck3 || colorDataRGBA.Length < boundCheck4) continue;


                                try
                                {
                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 0)] 
                                        = colorDataRGBA[y * (imageWidth * 4) + x]; //b//0x00;//b?

                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 1)] 
                                        = colorDataRGBA[y * (imageWidth * 4) + x + 1];//g

                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 2)] 
                                        = colorDataRGBA[y * (imageWidth * 4) + x + 2];//r

                                    outputImageData[((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 3)] 
                                        = colorDataRGBA[y * (imageWidth * 4) + x + 3];//a
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message+"!!!\r\n"+ "outputImageData.Len:"+ outputImageData.Length+ " outputImageDataIdx:" + ((item.Y + image.Top) + y) * (mapWidth * 4) + ((item.X + image.Left) * 4) + (x + 0));
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
            Console.WriteLine("Finish ConvertYpfToRGBA. Filename:" + filename);
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

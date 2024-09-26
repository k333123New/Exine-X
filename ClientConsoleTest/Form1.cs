using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Devices;
using ServerPackets;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows; 
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text.Unicode;
//using static System.Net.Mime.MediaTypeNames;
using C = ClientPackets;
using S = ServerPackets;

namespace ClientConsoleTest
{
    public partial class Form1 : RenderForm
    {
       
        public static Graphics Graphics;
        public static Point MPoint;

        public readonly static Stopwatch Timer = Stopwatch.StartNew();
        public readonly static DateTime StartTime = DateTime.UtcNow;
        public static long Time;
        public static DateTime Now { get { return StartTime.AddMilliseconds(Time); } }
        public static readonly Random Random = new Random();

        public static string DebugText = "";

        private static long _fpsTime;
        private static int _fps;
        private static long _cleanTime;
        private static long _drawTime;
        public static int FPS;
        public static int DPS;
        public static int DPSCounter;

        public static long PingTime;
        public static long NextPing = 10000;

        public static bool Shift, Alt, Ctrl, Tilde, SpellTargetLock;
        public static double BytesSent, BytesReceived;

        public class TestAccount
        {
            public string id = "";
            public string pw = "";
            public int style = 0;
            public int color = 0;
        }
        public static TestAccount testAccount = new TestAccount();
        public static byte[] exPortraitBytes = new byte[8000];

        public static byte[] GetBytesFromJpg(string path)
        {
            //max 8000bytes`
            //\Exine\RData\Profiles\charname.jpg  72*72
            Stream jpgStream = File.Open(Application.StartupPath + path, FileMode.Open);
            Image image = Image.FromStream(jpgStream);
            var stream = new MemoryStream();
            //image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] imageBytes = stream.ToArray();
            Console.WriteLine("imageBytes[0] : {0}, imageBytes.Length:{1}",imageBytes[0], imageBytes.Length);
            jpgStream.Close();
            return imageBytes;
        }

        public static Bitmap GetBitmapFromBytes(byte[] imageBytes)
        {
            Image recvImage = Image.FromStream(new MemoryStream(imageBytes));
            Bitmap recvBitmap = new Bitmap(recvImage);
            return recvBitmap;
        }

        public Form1()
        {
            InitializeComponent();
            /*
            Console.WriteLine("test");
            string path = @".\Data\Exine\AHW\";
            string toStringValue = "00";
            string extention = ".ypf";
            string suffix = "";


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Console.WriteLine("path:" + path);

            var allFiles = Directory.GetFiles(path, "*" + suffix + extention, SearchOption.TopDirectoryOnly).OrderBy(x => int.Parse(Regex.Match(x, @"\d+").Value));
            //var allFiles = Directory.GetFiles(path, "*.ypf");
            Console.WriteLine("allFiles.Count():"+allFiles.Count());
            Console.ReadLine();
            var lastFile = allFiles.Count() > 0 ? Path.GetFileName(allFiles.Last()) : "0";

            int i = 0;
            //MLibrary[] library = new MLibrary[allFiles.Count()];
            foreach (var file in allFiles)
            {
                Console.WriteLine(path+Path.GetFileName(file));
                //library[i] = new MLibrary(path+Path.GetFileName(file));


                //library[i] = new MLibrary(path + i.ToString(toStringValue) + suffix);
                //i++
            }

            //Console.ReadLine();
            */
            /*
            var count = int.Parse(Regex.Match(lastFile, @"\d+").Value) + 1;
            Console.WriteLine("count:"+count);
            Console.ReadLine();


            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(path + i.ToString(toStringValue) + suffix); 
            }

            Console.ReadLine();
            */


            
            TestJpg();
            testAccount = GetTestAccount();
            Console.WriteLine("testAccount.Style:" + testAccount.style + " testAccount.color:" + testAccount.color);

            
            //exPortraitBytes = GetBytesFromJpg("test.jpg");
            byte[] tempBytes = GetBytesFromJpg(testAccount.id+".jpg");
            Buffer.BlockCopy(tempBytes, 0, exPortraitBytes, 0, tempBytes.Length);
            //Read jpg to 
            Application.Idle += Application_Idle;
            Graphics = CreateGraphics();
            Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Graphics.TextContrast = 0;
            
        }

       
        void TestJpg()
        {
            GetBitmapFromBytes(GetBytesFromJpg("test.jpg")).Save("test2.bmp");
        }
        TestAccount GetTestAccount()
        {
            TestAccount result = new TestAccount();
            
            try
            {
                if (File.Exists("testAccount.txt"))
                {
                    string[] testAccountInfo = File.ReadAllLines("testAccount.txt");
                    result.id = testAccountInfo[0];
                    result.pw = testAccountInfo[1];
                    result.style = Convert.ToInt32(testAccountInfo[2]);
                    result.color = Convert.ToInt32(testAccountInfo[3]);
                }
            }
            catch(Exception ex)
            {
                return result;
            }
            return result;
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                while (AppStillIdle)
                {
                    UpdateTime();
                    UpdateFrameTime();
                    UpdateEnviroment(); 
                }
            }
            catch (Exception ex)
            {
               
            }
        }
         

        private static void UpdateTime()
        {
            Time = Timer.ElapsedMilliseconds;
        }

        private static void UpdateFrameTime()
        {
            if (Time >= _fpsTime)
            {
                _fpsTime = Time + 1000;
                FPS = _fps;
                _fps = 0;

                DPS = DPSCounter;
                DPSCounter = 0;
            }
            else
                _fps++;
        }

        private static void UpdateEnviroment()
        {
            if (Time >= _cleanTime)
            {
                _cleanTime = Time + 1000; 
            } 

            Network.Process();
            /*
            if (MirScene.ActiveScene != null)
                MirScene.ActiveScene.Process(); 
            */
        }
 
        #region Idle Check
        private static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin,
                                               uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }
        #endregion

       public static List<SelectInfo> charList = new List<SelectInfo>();


        static string ServerPackIdsToString(ServerPacketIds serverPacketIds)
        {
            return serverPacketIds.ToString();
        }

        static int tempDirection = 0;
        public static void ProcessPacket(Packet recvPacket)
        {
            //Console.WriteLine("Received :"+recvPacket.GetPacketBytes().Count()); 
            //Console.WriteLine("recvPacket.Index :" + recvPacket.Index);

            Console.WriteLine("recvPacket.Type :" + ServerPackIdsToString((ServerPacketIds)recvPacket.Index));
            switch (recvPacket.Index)
            {
                case (short)ServerPacketIds.Connected:
                    Network.Connected = true;
                    Console.WriteLine("Send [SendVersion()]");
                    
                    //Gen Send Version Packet
                    C.ClientVersion p1 = new C.ClientVersion();
                    byte[] sum;
                    using (MD5 md5 = MD5.Create())
                    using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                        sum = md5.ComputeHash(stream); 
                    p1.VersionHash = sum;
                    Network.Enqueue(p1);//Gen Send Version Packet

                    break;
                case (short)ServerPacketIds.ClientVersion:
                    Console.WriteLine("Send [NewAccount]");
                    Network.Enqueue(new C.NewAccount
                    { 
                        AccountID = testAccount.id,//"필11111",
                        Password = testAccount.pw,//"12481248",
                        EMailAddress = "",
                        BirthDate = !string.IsNullOrEmpty("")
                                       ? DateTime.Parse("")
                                       : DateTime.MinValue,
                        UserName = "",
                        SecretQuestion = "",
                        SecretAnswer = "",
                    }); 
                    break;

                    //Keep Alive를 주기로 동작 실행
                case (short)ServerPacketIds.KeepAlive: 
                    
                    Network.Enqueue(
                        new C.Turn
                        {
                            Direction = (ExineDirection)tempDirection,
                        }
                    );

                    Network.Enqueue(
                       new C.Chat
                       {
                           //Direction = (ExineDirection)tempDirection,
                           Message = "Test 테스트 " + tempDirection
                       }
                   );

                    if (tempDirection == 0 || tempDirection == 4)
                    {
                        Network.Enqueue(
                           new C.Walk
                           {
                               //Direction = (ExineDirection)tempDirection,
                               Direction = (ExineDirection)tempDirection,
                           }
                        );
                    }
                    else
                    {
                        Network.Enqueue(
                          new C.Attack
                          {
                              //Direction = (ExineDirection)tempDirection,
                              Direction = (ExineDirection)tempDirection,
                              Spell= Spell.None,
                    }
                       );
                    }


                    tempDirection++;
                    tempDirection = tempDirection % 7;
                    break;

                case (short)ServerPacketIds.UserInformation:
                    var p = ((S.UserInformation)recvPacket);
                    Console.WriteLine("name:{0} NameColour:{1} Style{2} Color{3} ExPortraitLen{4}", p.Name,p.NameColour,p.ExStyle, p.ExColor, p.ExPortraitLen);
                     
                    
                    break;

                case (short)ServerPacketIds.UserLocation:
                    Point localtion = ((S.UserLocation)recvPacket).Location;
                    ExineDirection direction = ((S.UserLocation)recvPacket).Direction; 
                    Console.WriteLine("localtion:"+((S.UserLocation)recvPacket).Location.ToString()+ " Direction:"+direction);
                    break;
                case (short)ServerPacketIds.NewAccount:
                    int result = ((S.NewAccount)recvPacket).Result;
                    Console.Write("result :" + result);
                    //add k333123
                    if (result == 0) MessageBox.Show("Disable New Account");
                    else if (result==1) MessageBox.Show("Bad AccountID");
                    else if (result == 2) MessageBox.Show("Bad Password");
                    /*
                    * 0: Disabled
                    * 1: Bad AccountID
                    * 2: Bad Password
                    * 3: Bad Email
                    * 4: Bad Name
                    * 5: Bad Question
                    * 6: Bad Answer
                    * 7: Account Exists.
                    * 8: Success
                    */
                    //exist : 7  //ok : 8
                    if (((S.NewAccount) recvPacket).Result==7 || ((S.NewAccount)recvPacket).Result == 8)
                    {
                        Console.WriteLine(" > Send [Login]");
                        Network.Enqueue(
                            new C.Login
                            {
                                AccountID = testAccount.id,
                                Password = testAccount.pw,
                            }
                        );
                    }
                    break;
               
                case (short)ServerPacketIds.LoginSuccess:
                    Console.WriteLine("Send [NewCha]");
                    charList.Clear();
                    charList.AddRange(((S.LoginSuccess)recvPacket).Characters);
                    for(int i=0;i< charList.Count;i++)
                    {
                        Console.WriteLine("name : " + charList[i].Name + "Index : " + charList[i].Index);
                    }

                    //have to Make CharacterPortraitUpdate Packet.

                    //byte[] exPortraitBytes = GetBytesFromJpg("test.jpg");

                    //Select Scene
                    Network.Enqueue(
                         new C.NewCharacter
                         {
                             Name = testAccount.id,//"필11111",
                             Class = 0, //Change to Color
                             Gender = 0, //0:male, 1:female
                             ExStyle = (ExineStyle)testAccount.style,//ExineStyle.Style0,
                             ExColor = (ExineColor)testAccount.color, //ExineColor.Color3, 
                             ExPortraitBytes = exPortraitBytes,
                             ExPortraitLen = exPortraitBytes.Length, 
                             //Portrait = new byte[50] { },
                             //231107
                         }
                    );
                    break;

                case (short)ServerPacketIds.NewCharacter:
                    Console.WriteLine(">Send [StartXam]"); 
                    for (int i = 0; i < charList.Count; i++)
                    {
                        Console.WriteLine("name : " + charList[i].Name + "Index : " + charList[i].Index);
                    } 
                    Console.WriteLine("charList[charList.Count - 1].Index:" + charList[charList.Count - 1].Index);
                    Network.Enqueue(
                        new C.StartGame { 
                        CharacterIndex = charList[charList.Count-1].Index,
                        }
                    );
                    break;


                case (short)ServerPacketIds.NewCharacterSuccess:
                    Console.WriteLine("> Send [StartXam]");
                    charList.Insert(0, ((S.NewCharacterSuccess)recvPacket).CharInfo);
                    for (int i = 0; i < charList.Count; i++)
                    {
                        Console.WriteLine("name : " + charList[i].Name + "Index : " + charList[i].Index);
                    }

                    Console.WriteLine("charList[charList.Count - 1].Index:" + charList[charList.Count - 1].Index);
                    Network.Enqueue(
                        new C.StartGame
                        {
                            CharacterIndex = charList[charList.Count - 1].Index,
                        }
                    );
                    break;

                case (short)ServerPacketIds.DeleteCharacter:
                    break;
                case (short)ServerPacketIds.DeleteCharacterSuccess:
                    break;
                case (short)14:break;
                
                case (short)ServerPacketIds.ObjectPlayer:
                    var temp = (S.ObjectPlayer)recvPacket;
                    Console.WriteLine("ObjectPlayer.ObjectId{0}, ObjectPlayer.name{1}, ObjectPlayer.ExStyle{2}, ObjectPlayer.ExColor{3} ObjectPlayer.ExPortraitLen{4} ObjectPlayer.ExPortraitBytes[0]{5} ", 
                        temp.ObjectID, temp.Name, temp.ExStyle, temp.ExColor,temp.ExPortraitLen, temp.ExPortraitBytes[0]);
                    //save object portrait
                    Bitmap objectPortrait = GetBitmapFromBytes(temp.ExPortraitBytes);
                    objectPortrait.Save(temp.Name + ".jpg");//save 32 bit!
                    //check portrate
                    break;

                case (short)ServerPacketIds.ObjectWalk:
                    Console.WriteLine("ObjectWalk.ObjectId{0}, ObjectWalk.Location{1}", ((S.ObjectWalk)recvPacket).ObjectID, ((S.ObjectWalk)recvPacket).Location.ToString()); 
                    break;

                case (short)15:
                    
                    break;
                case (short)16:
                   
                    break;
                default:
                    //base.ProcessPacket(p);
                    Console.WriteLine("p.Index:" + recvPacket.Index);
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using System.Linq;


namespace Map_Editor
{
    public partial class Main : Form
    {
        public delegate void DelJump(int x, int y);

        public delegate void DelSetAnimationProperty(bool blend, byte frame, byte tick);

        public delegate void DelSetDoorProperty(bool door, byte index, byte offSet);

        public delegate void DelSetLightProperty(byte light);

        public delegate void DelSetMapSize(int w, int h);

        
        private const int CellWidth = 48;
        private const int CellHeight = 24;
        private const int Exine2BigTileBlock = 50;
        private const int Exine3BigTileBlock = 30;
        private const int smTileBlock = 60;
        public static Font font = new Font("Tahoma", 10, FontStyle.Bold);
        private static int zoomMIN;
        private static readonly int zoomMAX = zoomMIN = 20;

        public static readonly Stopwatch Timer = Stopwatch.StartNew();
        public static readonly DateTime StartTime = DateTime.Now;
        public static long Time, OldTime;
        private static long _fpsTime;
        private static int _fps;
        public static int FPS;
        public static long MoveTime;
        private static readonly Vector2[] vector2S = new Vector2[5];
        private static readonly Vector2[] line = new Vector2[2];

        private static readonly Random random = new Random();

        private static int AutoTileRange;
        private static int AutoTileChanges;
        private readonly Editor _editor = new Editor();
        private readonly Dictionary<int, int> _tilesIndexList = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _wemadeExine2IndexList = new Dictionary<int, int>();
        private readonly List<CellInfoData> bigTilePoints = new List<CellInfoData>();
        private readonly CellInfoControl cellInfoControl = new CellInfoControl();
        private readonly int[] Mir2BigTilesPreviewIndex = {5, 15, 6, 20, 0, 21, 7, 17, 8};
        private readonly List<CellInfoData> smTilePoints = new List<CellInfoData>();
        private readonly int[] smTilesPreviewIndex = {39, 11, 15, 35, 0, 19, 31, 25, 23};
        public int AnimationCount;
        private CellInfoData[] cellInfoDatas;
        private int cellX, cellY;
        private int drawY, drawX, libIndex, index;
        private Graphics graphics;

        private bool Grasping;
        private bool keyDown;

        private Layer layer = Layer.None;

        public CellInfo[,] X2CellInfo;
        private MapReader map;
        private string mapFileName;
        private Point mapPoint;

        private int mapWidth, mapHeight;
        public CellInfo[,] NewCellInfo;
        private CellInfoData[] objectDatas;

        private int OffSetX;
        private int OffSetY;

        private Point p1, p2;
        private int selectImageIndex;
        private XLibrary.MImage selectLibMImage;
        private ListItem selectListItem;
        private int selectTilesIndex = -1;
          
        private CellInfoData[] unTemp, reTemp;

        private ListItem wemadeExine2ListItem; 

        //TileCutter
        private bool grid = true;
        public static Point MPoint;
        private Bitmap cellHighlight = new Bitmap(48, 24);
        public int CellSizeX;
        public int CellSizeY;
        public int[,] SelectedCells;
        private XLibrary _library;
        //private MLibrary.MImage _selectedImage;
        public Bitmap _mainImage;
        private bool pictureBox_loaded = false;

        public Main()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Application.Idle += Application_Idle;
            
            //Tilecutter
            pictureBox_Grid.Parent = pictureBox_Image;
            pictureBox_Highlight.Parent = pictureBox_Grid;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                while (AppStillIdle)
                {
                    UpdateTime();
                    UpdateEnviroment();
                    RenderEnviroment();
                }
            }
            catch (Exception)
            {
            }
        }

        private void UpdateTime()
        {
            Time = Timer.ElapsedMilliseconds;
        }

        private void UpdateEnviroment()
        {
            if (Time >= _fpsTime)
            {
                _fpsTime = Time + 1000;
                FPS = _fps;
                _fps = 0;
            }
            else
                _fps++;

            if (Time >= MoveTime)
            {
                MoveTime += 100;

                AnimationCount++;
            }

            Text = string.Format("FPS: {0}---Map:W {1}:H {2} ----W,S,A,D,<{3}>", FPS, mapWidth, mapHeight,mapFileName);
        }

        private void RenderEnviroment()
        {
            try
            {
                if (DXManager.DeviceLost)
                {
                    DXManager.AttemptReset();
                    Thread.Sleep(1);
                    return;
                }

                if (X2CellInfo == null)
                {
                }
                else
                {
                    DXManager.Device.Clear(ClearFlags.Target, Color.White, 0, 0);
                    DXManager.Device.BeginScene();
                    DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
                    DXManager.TextSprite.Begin(SpriteFlags.AlphaBlend);

                    OffSetX = MapPanel.Width/(CellWidth*zoomMIN/zoomMAX);
                    OffSetY = MapPanel.Height/(CellHeight*zoomMIN/zoomMAX);
                     
                    DrawBack(chkBack.Checked); //back
                    DrawMidd(chkMidd.Checked); //midd
                    DrawFront(chkFront.Checked);//front
                    DrawObject(objectDatas);//Draw Select Object
                    DrawSelectTextureImage();//Draw Select TextureImage
                    DrawLimit();//Draw Limit
                    DrawDoorTag(chkDoorSign.Checked); //door mark
                    DrawFrontAnimationTag(chkFrontAnimationTag.Checked);//Foreground animated marker  
                    DrawMiddleAnimationTag(chkMiddleAnimationTag.Checked);//Background animated markers
                    DrawLightTag(chkLightTag.Checked);//highlight mark
                    DrawBackLimit(chkBackMask.Checked);//Background movement restrictions
                    DrawFrontMask(chkFrontMask.Checked);//Foreground movement restrictions
                    DrawFrontTag(chkFrontTag.Checked);//foreground mark
                    DrawMiddleTag(chkMiddleTag.Checked);//middle layer markup

                    DXManager.Sprite.End();
                    DXManager.TextSprite.End();

                    //grid
                    DrawGrids(chkDrawGrids.Checked);
                    GraspingRectangle();
                    DXManager.Device.EndScene();
                    DXManager.Device.Present();
                }
            }
            catch (DeviceLostException)
            {
            }
            catch (Exception)
            {
                DXManager.AttemptRecovery();
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet =
            CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize,
            int maximumWorkingSetSize);

        public new void Dispose()
        {
            GC.Collect();
            GC.SuppressFinalize(this);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        public void SetMapSize(int w, int h)
        {
            mapWidth = w;
            mapHeight = h;
            graphics = Graphics.FromHwnd(MapPanel.Handle);
        }

        private void Main_Load(object sender, EventArgs e)
        {

            Libraries.LoadGameLibraries();
              
            ReadWemadeExine2LibToListBox();

            ReadObjectsToListBox();

            DXManager.Create(MapPanel);

            //TileCutter
            comboBox_cellSize.SelectedIndex = 0;
            gridUpdate(false);

            //loads embedded icons
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file = thisExe.GetManifestResourceStream("Map_Editor.Resources.Square.png");
            using (Graphics g = Graphics.FromImage(cellHighlight))
            {
                g.DrawImage(Image.FromStream(file), new Point(0, 0));
            }
        }


        private void Draw(int libIndex, int index, int drawX, int drawY)
        {
            Libraries.MapLibs[libIndex].CheckImage(index);
            var mi = Libraries.MapLibs[libIndex].Images[index];
            if (mi.Image == null || mi.ImageTexture == null) return;
            int w = mi.Width;
            int h = mi.Height;
            DXManager.Sprite.Draw2D(mi.ImageTexture, Rectangle.Empty, new SizeF(w*zoomMIN/zoomMAX, h*zoomMIN/zoomMAX), new PointF(drawX, drawY), Color.White);

            //DXManager.Sprite.Draw2D(mi.ImageTexture, new Rectangle(Point.Empty, new Size(w * zoomMIN / zoomMAX, h * zoomMIN / zoomMAX)), new Rectangle(Point.Empty, new Size(w * zoomMIN / zoomMAX, h * zoomMIN / zoomMAX)), new Point(drawX, drawY), Color.White);
        }

        //여기서 잡아야함!
        private void Draw(int libIndex, int index, int drawX, int drawY, bool offSet)
        {
            Libraries.MapLibs[libIndex].CheckImage(index);
            var mi = Libraries.MapLibs[libIndex].Images[index];
            if (mi.Image == null || mi.ImageTexture == null) return;
            int w = mi.Width;
            int h = mi.Height;


            //drawX = (x - mapPoint.X) * (CellWidth * zoomMIN / zoomMAX);


            //Point point = new Point(drawX, drawY); 
            //if (offSet) point.Offset(mi.X/CellWidth, mi.Y/CellHeight);
            //if (index == 46) Console.WriteLine("drawX:" + drawX + " mi.X:" + mi.X+ " w:"+ w);

            drawX = (drawX + mi.X);// * zoomMIN / zoomMAX;
            drawY = (drawY + mi.Y + mi.Height);// * zoomMIN / zoomMAX;

            DXManager.Sprite.Draw2D(mi.ImageTexture, Rectangle.Empty, new SizeF(w * zoomMIN / zoomMAX, h * zoomMIN / zoomMAX), new PointF(drawX , drawY ), Color.White);
        }
         


        public void DrawBlend(int libindex, int index, Point point, Color colour, bool offSet = false, float rate = 1f)
        {
            Libraries.MapLibs[libIndex].CheckImage(index);
            var mi = Libraries.MapLibs[libIndex].Images[index];
            if (mi.Image == null || mi.ImageTexture == null) return;
            int w = mi.Width;
            int h = mi.Height;

            if (offSet) point.Offset(mi.X*zoomMIN/zoomMAX, mi.Y*zoomMIN/zoomMAX);
            var oldBlend = DXManager.Blending;
            DXManager.SetBlend(true, rate);
            DXManager.Sprite.Draw2D(mi.ImageTexture, Rectangle.Empty, new SizeF(w*zoomMIN/zoomMAX, h*zoomMIN/zoomMAX), point, Color.White);

            //DXManager.Sprite.Draw2D(mi.ImageTexture, new Rectangle(Point.Empty, new Size(w * zoomMIN / zoomMAX, h * zoomMIN / zoomMAX)), new Rectangle(Point.Empty, new Size(w * zoomMIN / zoomMAX, h * zoomMIN / zoomMAX)), point, Color.White);
            DXManager.SetBlend(oldBlend);
        }

        private string GetLibName(int index)
        {
            if (index < 0 || index >= Libraries.ListItems.Length)
            {
                return string.Empty;
            }
            try
            {
                return Libraries.ListItems[index].Text;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //
            if (X2CellInfo == null) return;
            var p = MapPanel.PointToClient(MousePosition);
            cellX = p.X/(CellWidth*zoomMIN/zoomMAX) + mapPoint.X;
            cellY = p.Y/(CellHeight*zoomMIN/zoomMAX) + mapPoint.Y;

            //
            if (cellX >= mapWidth || cellY >= mapHeight || cellX < 0 || cellY < 0)
            {
                return;
            }

            //
            ShowCellInfo(chkShowCellInfo.Checked);
            //
            if (keyDown)
            {
                MapPanel_MouseClick(sender, e);
            }
            //
            if (X2CellInfo != null)
            {
                switch (layer)
                {
                    case Layer.GraspingMir2Front:
                        if (Grasping)
                        {
                            if (p1.IsEmpty)
                            {
                                return;
                            }
                            p2 = new Point(cellX, cellY);
                        }
                        break;
                    case Layer.GraspingInvertMir3FrontMiddle:
                        if (Grasping)
                        {
                            if (p1.IsEmpty)
                            {
                                return;
                            }
                            p2 = new Point(cellX, cellY);
                        }
                        break;
                }
            }
        }

        private void DrawLimit()
        {
            //index:99
            //56:0,57:1,58:2,59:3,116:4
            var drawX = (cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
            var drawY = (cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
            switch (layer)
            {
                case Layer.BackLimit:
                    //Draw(1, 58, drawX, drawY);
                    Draw(99, 2, drawX, drawY);
                    break;
                case Layer.FrontLimit:
                    //Draw(1, 59, drawX, drawY);
                    Draw(99, 3, drawX, drawY);
                    break;
                case Layer.BackFrontLimit:
                    //Draw(1, 58, drawX, drawY);
                    //Draw(1, 59, drawX, drawY);
                    Draw(99, 2, drawX, drawY);
                    Draw(99, 3, drawX, drawY);
                    break;
            }
        }

        private void DrawSelectTextureImage()
        {
            if (selectLibMImage == null) return;
            if (selectLibMImage.ImageTexture == null) return;
            if (selectListItem == null) return;
            if (layer == Layer.MiddleImage || layer == Layer.FrontImage)
            {
                var libIndex = selectListItem.Value;
                var index = selectImageIndex;
                var drawX = (cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                var drawY = (cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                var s = Libraries.MapLibs[libIndex].GetSize(index);
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (cellY - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                    //Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY - s.Height * zoomMIN / zoomMAX,true); //k333123
                }
                else
                {
                    drawY = (cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY);
                    
                }
            }
            else if (layer == Layer.BackImage)
            {
                /*
                var temp = CheckPointIsEven(new Point(cellX, cellY));
                cellX = temp.X;
                cellY = temp.Y;
                */
                var libIndex = selectListItem.Value;
                var index = selectImageIndex;
                var drawX = (cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                var drawY = (cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                var s = Libraries.MapLibs[libIndex].GetSize(index);
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (cellY - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                }
                else
                {
                    drawY = (cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY);
                }
            }
        }

        private void DrawObject(CellInfoData[] datas)
        {
            if (datas == null) return;
            if (layer != Layer.PlaceObjects) return;


            for (var i = 0; i < datas.Length; i++)
            {
                if (datas[i].X%2 != 0) continue;
                if (datas[i].Y%2 != 0) continue;
                if (datas[i].X + cellX >= mapWidth) continue;
                if (datas[i].Y + cellY >= mapWidth) continue;


                drawX = (datas[i].X + cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);

                index = (datas[i].CellInfo.BackImage & 0x1FFFFFFF) - 1;
                libIndex = datas[i].CellInfo.BackIndex;
                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;
                Draw(libIndex, index, drawX, drawY);
            }


            for (var i = 0; i < datas.Length; i++)
            {
                byte animation;
                bool blend;
                if (datas[i].X + cellX >= mapWidth) continue;
                if (datas[i].Y + cellY >= mapWidth) continue;

                drawX = (datas[i].X + cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                index = datas[i].CellInfo.MiddleImage - 1;
                libIndex = datas[i].CellInfo.MiddleIndex;
                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                animation = datas[i].CellInfo.MiddleAnimationFrame;
                blend = false;
                if ((animation > 0) && (animation < 255))
                {
                    if ((animation & 0x0f) > 0)
                    {
                        blend = true;
                        animation &= 0x0f;
                    }
                    if (animation > 0)
                    {
                        var animationTick = datas[i].CellInfo.MiddleAnimationTick;
                        index += AnimationCount%(animation + animation*animationTick)/(1 + animationTick);
                    }
                }

                var s = Libraries.MapLibs[libIndex].GetSize(index);
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                }
                else
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY);
                }
                if ((datas[i].CellInfo.MiddleImage & 0x7FFF) - 1 >= 0)
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    //Draw(1, 56, drawX, drawY);
                    Draw(99, 0, drawX, drawY);
                }
            }

            for (var i = 0; i < datas.Length; i++)
            {
                if (datas[i].X + cellX >= mapWidth) continue;
                if (datas[i].Y + cellY >= mapWidth) continue;

                drawX = (datas[i].X + cellX - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                index = (datas[i].CellInfo.FrontImage & 0x7FFF) - 1;
                libIndex = datas[i].CellInfo.FrontIndex;
                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;
                var s = Libraries.MapLibs[libIndex].GetSize(index);
                byte animation;
                bool blend;
                animation = datas[i].CellInfo.FrontAnimationFrame;
                if ((animation & 0x80) > 0)
                {
                    blend = true;
                    animation &= 0x7F;
                }
                else
                {
                    blend = false;
                }

                if (animation > 0)
                {
                    var animationTick = datas[i].CellInfo.FrontAnimationTick;
                    index += AnimationCount%(animation + animation*animationTick)/(1 + animationTick);
                }

                //It's not a 48*24 or 96*48 floor tile, it's a big object
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                    //If there is animation
                    if (animation > 0)
                    {
                        //Mix if needed
                        if (blend)
                        {
                            //New Shanda Map
                            if ((libIndex > 99) & (libIndex < 199))
                            {
                                DrawBlend(libIndex, index, new Point(drawX, drawY - 3*CellHeight*zoomMIN/zoomMAX),
                                    Color.White, true);
                            }
                            //Old map lamppost index >= 2723 && index <= 2732
                            else
                            {
                                DrawBlend(libIndex, index, new Point(drawX, drawY - s.Height*zoomMIN/zoomMAX),
                                    Color.White, index >= 2723 && index <= 2732);
                            }
                        }
                        //No mixing required
                        else
                        {
                            Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                        }
                    }
                    //If there is no animation
                    else
                    {
                        Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                    }
                }
                //Is 48*24 or 96*48 floor tiles
                else
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    Draw(libIndex, index, drawX, drawY);
                }


                if ((datas[i].CellInfo.FrontImage & 0x7FFF) - 1 >= 0)
                {
                    drawY = (datas[i].Y + cellY - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    //Draw(1, 56, drawX, drawY);
                    Draw(99, 0, drawX, drawY);
                }
            }
        }

        private void DrawGrids(bool blGrids)
        {
            if (blGrids)
            {
                if (FPS < 25) return;
                for (var y = mapPoint.Y; y <= mapPoint.Y + OffSetY + 2; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 2; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        DrawCube(drawX, drawY);
                        //Draw(99, 1, drawX, drawY);
                    }
                }
            }
        }

        private void DrawGrids2(bool blGrids)
        {
            if (blGrids)
            {
                if (FPS < 25) return;
                for (var x = 0; x <= OffSetX; x++)
                {
                    if (x >= mapHeight || x < 0) continue;
                    for (var y = 0; y <= OffSetY; y++)
                    {
                        if (y >= mapWidth || y < 0) continue;
                        DrawHorizontalLine(x, y);
                        DrawVerticalLine(x, y);
                    }
                }
            }
        }

        private void DrawCube(int x, int y)
        {
            vector2S[0] = new Vector2(x, y);
            vector2S[1] = new Vector2(CellWidth*zoomMIN/zoomMAX, y);
            vector2S[2] = new Vector2(CellWidth*zoomMIN/zoomMAX, CellHeight*zoomMIN/zoomMAX);
            vector2S[3] = new Vector2(x, CellHeight*zoomMIN/zoomMAX);
            vector2S[4] = new Vector2(x, y);
            DXManager.Line.Width = 0.5F;
            DXManager.Line.Draw(vector2S, Color.Magenta);
        }

        private void DrawHorizontalLine(int x, int y)
        {
            line[0] = new Vector2(x*CellWidth*zoomMIN/zoomMAX, y*CellHeight*zoomMIN/zoomMAX);
            line[1] = new Vector2(MapPanel.Width, y*CellHeight*zoomMIN/zoomMAX);

            DXManager.Line.Width = 0.5F;
            DXManager.Line.Draw(line, Color.Magenta);
        }

        private void DrawVerticalLine(int x, int y)
        {
            line[0] = new Vector2(x*CellWidth*zoomMIN/zoomMAX, y*CellHeight*zoomMIN/zoomMAX);
            line[1] = new Vector2(x*CellWidth*zoomMIN/zoomMAX, MapPanel.Height);

            DXManager.Line.Width = 0.5F;
            DXManager.Line.Draw(line, Color.Magenta);
        }

        private void DrawCube(Point p1, Point p2)
        {
            //When scaling, you need to first calculate the scaling factor plus parentheses.
            var vector2S = new Vector2[5];
            vector2S[0] = new Vector2((p1.X - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX),
                (p1.Y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX));
            vector2S[1] = new Vector2((p2.X - mapPoint.X + 1)*(CellWidth*zoomMIN/zoomMAX),
                (p1.Y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX));
            vector2S[2] = new Vector2((p2.X - mapPoint.X + 1)*(CellWidth*zoomMIN/zoomMAX),
                (p2.Y - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX));
            vector2S[3] = new Vector2((p1.X - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX),
                (p2.Y - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX));
            vector2S[4] = new Vector2((p1.X - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX),
                (p1.Y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX));
            DXManager.Line.Width = 2F;
            DXManager.Line.Draw(vector2S, Color.Red);
        }

        private void GraspingRectangle()
        {
            if (p1.IsEmpty || p2.IsEmpty) return;
            if ((layer == Layer.GraspingMir2Front) || (layer == Layer.GraspingInvertMir3FrontMiddle))
            {
                DrawCube(p1, p2);
            }
        }

        private void DrawFrontMask(bool blFrontMask)
        {
            if (blFrontMask)
            {
                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        if (Convert.ToBoolean(X2CellInfo[x, y].FrontImage & 0x8000))
                        {
                            //Draw(1, 59, drawX, drawY);
                            Draw(99, 3, drawX, drawY);
                        }
                    }
                }
            }
        }

        private void DrawBackLimit(bool blBackMask)
        {
            if (blBackMask)
            {
                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        if (Convert.ToBoolean(X2CellInfo[x, y].BackImage & 0x20000000))
                        {
                            //Draw(1, 58, drawX, drawY);
                            Draw(99, 2, drawX, drawY);
                        }
                    }
                }
            }
        }

        private void DrawFront(bool blFront)
        {
            if (blFront)
            {
                byte animation;
                bool blend;

                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    //drawY = (y + 1) * (cellHeight * zoomMIN / zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        index = (X2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                        libIndex = X2CellInfo[x, y].FrontIndex;
                        if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                        if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                        animation = X2CellInfo[x, y].FrontAnimationFrame;
                        if ((animation & 0x80) > 0)
                        {
                            blend = true;
                            animation &= 0x7F;
                        }
                        else
                        {
                            blend = false;
                        }

                        if (animation > 0)
                        {
                            var animationTick = X2CellInfo[x, y].FrontAnimationTick;
                            index += AnimationCount%(animation + animation*animationTick)/(1 + animationTick);
                        }

                        var doorOffset = X2CellInfo[x, y].DoorOffset;
                        var s = Libraries.MapLibs[libIndex].GetSize(index);
                        //It's not a 48*24 or 96*48 floor tile, it's a big object
                        if ((s.Width != CellWidth || s.Height != CellHeight) &&
                            (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            drawY = (y - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                            //If there is animation
                            if (animation > 0)
                            {
                                //Mix if needed
                                if (blend)
                                {
                                    //New Shanda Map
                                    if ((libIndex > 99) & (libIndex < 199))
                                    {
                                        DrawBlend(libIndex, index,
                                            new Point(drawX, drawY - 3*CellHeight*zoomMIN/zoomMAX), Color.White, true);
                                    }
                                    //Old map lamppost index >= 2723 && index <= 2732
                                    else
                                    {
                                        DrawBlend(libIndex, index, new Point(drawX, drawY - s.Height*zoomMIN/zoomMAX),
                                            Color.White, index >= 2723 && index <= 2732);
                                    }
                                }
                                //No mixing required
                                else
                                {
                                    Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                                }
                            }
                            //If there is no animation
                            else
                            {
                                //여기서 움직이지 않는 객체를 그린다.
                                //Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                                Draw(libIndex, index, drawX, drawY - s.Height * zoomMIN / zoomMAX,true);//k333123

                                //DrawExineObject(libIndex, index, drawX, drawY - s.Height * zoomMIN / zoomMAX);//실제 좌표를 그린다.
                            }
                        }
                        //Is 48*24 or 96*48 floor tiles
                        else
                        {
                            drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                            Draw(libIndex, index, drawX, drawY);
                        }
                        //Show door open
                        if (chkDoor.Checked && (doorOffset > 0))
                        {
                            drawY = (y - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                            Draw(libIndex, index + doorOffset, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                        }
                    }
                }
            }
        }

        private void DrawFrontTag(bool blFront)
        {
            if (blFront)
            {
                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        if ((X2CellInfo[x, y].FrontImage & 0x7FFF) - 1 >= 0)
                        {
                            //Draw(1, 56, drawX, drawY);
                            Draw(99, 0, drawX, drawY);
                        }
                    }
                }
            }
        }

        private void DrawMiddleTag(bool blMiddle)
        {
            if (blMiddle)
            {
                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        if ((X2CellInfo[x, y].MiddleImage & 0x7FFF) - 1 >= 0)
                        {
                            //Draw(1, 56, drawX, drawY);
                            Draw(99, 0, drawX, drawY);
                        }
                    }
                }
            }
        }

        private void DrawMidd(bool blMidd)
        {
            if (blMidd)
            {
                byte animation;
                bool blend;

                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
                {
                    if (y >= mapHeight || y < 0) continue;
                    for (var x = mapPoint.X - 1; x <= mapPoint.X + OffSetX + 35; x++)
                    {
                        if (x >= mapWidth || x < 0) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        index = X2CellInfo[x, y].MiddleImage - 1;
                        libIndex = X2CellInfo[x, y].MiddleIndex;
                        if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                        if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                        animation = X2CellInfo[x, y].MiddleAnimationFrame;
                        blend = false;
                        if ((animation > 0) && (animation < 255))
                        {
                            if ((animation & 0x0f) > 0)
                            {
                                blend = true;
                                animation &= 0x0f;
                            }
                            if (animation > 0)
                            {
                                var animationTick = X2CellInfo[x, y].MiddleAnimationTick;
                                index += AnimationCount%(animation + animation*animationTick)/(1 + animationTick);
                            }
                        }

                        var s = Libraries.MapLibs[libIndex].GetSize(index);
                        if ((s.Width != CellWidth || s.Height != CellHeight) &&
                            (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                        {
                            drawY = (y - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                            Draw(libIndex, index, drawX, drawY - s.Height*zoomMIN/zoomMAX);
                        }
                        else
                        {
                            drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                            Draw(libIndex, index, drawX, drawY);
                        }
                    }
                }
            }
        }

        private void DrawBack(bool blBack)
        {
            if (blBack)
            {
                for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY; y++)
                {
                    //if (y%2 != 0) continue;
                    if (y < 0) continue;
                    if (y >= mapHeight) continue;
                    drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                    for (var x = mapPoint.X - 1; x <= mapPoint.X + OffSetX; x++)
                    {
                        //if (x%2 != 0) continue;
                        if (x < 0) continue;
                        if (x >= mapWidth) continue;
                        drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                        index = (X2CellInfo[x, y].BackImage & 0x1FFFFFFF) - 1;
                        libIndex = X2CellInfo[x, y].BackIndex;
                        if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                        if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;
                        Draw(libIndex, index, drawX, drawY);
                    }
                }
            }
        }

        private void ReadWemadeExine2LibToListBox()
        {
            //for (var i = 0; i < 100; i++)
            for (var i = 0; i < 2000; i++) //k333123
            {
                if (Libraries.ListItems[i] != null)
                {
                    WemadeExine2LibListBox.Items.Add(Libraries.ListItems[i]);
                }
            }
        }
         
         
        private void Clear()
        {
            _wemadeExine2IndexList.Clear();
            WemadeExine2ImageList.Images.Clear();
            TilesImageList.Images.Clear();
            _tilesIndexList.Clear();
        }

        private void ClearImage()
        {
            for (var i = 0; i < Libraries.MapLibs.Length; i++)
            {
                for (var j = 0; j < Libraries.MapLibs[i].Images.Count; j++)
                {
                    Libraries.MapLibs[i].Images[j] = null;
                }
            }
        }

        public void EnlargeZoom()
        {
            graphics.Clear(Color.Black);
            zoomMIN += 3;
            if (zoomMIN >= zoomMAX)
            {
                zoomMIN = zoomMAX;
            }
        }

        public void NarrowZoom()
        {
            graphics.Clear(Color.Black);
            zoomMIN -= 3;
            if (zoomMIN <= 0)
            {
                zoomMIN = 1;
            }
        }

        private void btnDispose_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void chkBack_Click(object sender, EventArgs e)
        {
            chkBack.Checked = !chkBack.Checked;
        }

        private void chkMidd_Click(object sender, EventArgs e)
        {
            chkMidd.Checked = !chkMidd.Checked;
        }

        private void chkFront_Click(object sender, EventArgs e)
        {
            chkFront.Checked = !chkFront.Checked;
        }

        private void chkBackMask_Click(object sender, EventArgs e)
        {
            chkBackMask.Checked = !chkBackMask.Checked;
        }

        private void chkFrontMask_Click(object sender, EventArgs e)
        {
            chkFrontMask.Checked = !chkFrontMask.Checked;
        }

        private void btnToImage_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Too lazy to write the .........");
            //var bit = new Bitmap(Width, Height); //Instantiate a bitmap as large as the form
            //var g = Graphics.FromImage(bit);
            //g.CompositingQuality = CompositingQuality.HighQuality; //Quality set to highest
            //g.CopyFromScreen(Left, Top, 0, 0, new Size(Width, Height)); //Save the entire form as a picture
            ////g.CopyFromScreen(panel游戏区 .PointToScreen(Point.Empty), Point.Empty, panel游戏区.Size);//Only save a certain control (here is the panel game area)
            //bit.Save("weiboTemp.png"); //The default saving format is PNG, and the quality when saving in jpg format is not very good.
        }

        private void chkDoor_Click(object sender, EventArgs e)
        {
            chkDoor.Checked = !chkDoor.Checked;
        }

        private void DrawDoorTag(bool blDoorTag)
        {
            if (!blDoorTag)
            {
                return;
            }
            var szText = "";
            int DoorIndex;
            byte DoorOffset;
            bool EntityDoor;
            var font = new Font("Comic Sans MS", 10, FontStyle.Bold);
            var dxFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, font);
            for (var y = mapPoint.Y; y <= mapPoint.Y + OffSetY; y++)
            {
                if (y >= mapHeight || y < 0) continue;
                drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                {
                    if (x >= mapWidth || x < 0) continue;
                    drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                    DoorIndex = X2CellInfo[x, y].DoorIndex & 0x7F;
                    DoorOffset = X2CellInfo[x, y].DoorOffset;
                    EntityDoor = Convert.ToBoolean(X2CellInfo[x, y].DoorIndex & 0x80);

                    if (DoorIndex > 0)
                    {
                        if (EntityDoor)
                        {
                            szText = string.Format("Dx{0}/{1}", DoorIndex, DoorOffset);
                        }
                        else
                        {
                            szText = string.Format("D{0}/{1}", DoorIndex, DoorOffset);
                        }
                        dxFont.DrawText(DXManager.TextSprite, szText, drawX, drawY, Color.AliceBlue);
                    }
                }
            }
            font.Dispose();
            dxFont.Dispose();
        }

        private void chkDoorSign_Click(object sender, EventArgs e)
        {
            chkDoorSign.Checked = !chkDoorSign.Checked;
        }

        private void DrawFrontAnimationTag(bool blFrontAnimationTag)
        {
            if (!blFrontAnimationTag)
            {
                return;
            }
            var szText = "";
            int FrontAnimationFrame;
            byte FrontAnimationTick;
            bool CoreAnimation;
            var font = new Font("Comic Sans MS", 10, FontStyle.Bold);
            var dxFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, font);
            for (var y = mapPoint.Y; y <= mapPoint.Y + OffSetY; y++)
            {
                if (y >= mapHeight || y < 0) continue;
                drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                {
                    if (x >= mapWidth || x < 0) continue;
                    drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                    if (X2CellInfo[x, y].FrontAnimationFrame == 255)
                    {
                        continue;
                    }
                    FrontAnimationFrame = X2CellInfo[x, y].FrontAnimationFrame & 0x7F;
                    FrontAnimationTick = X2CellInfo[x, y].FrontAnimationTick;
                    CoreAnimation = Convert.ToBoolean(X2CellInfo[x, y].FrontAnimationFrame & 0x80);

                    if (FrontAnimationFrame > 0)
                    {
                        if (CoreAnimation)
                        {
                            szText = string.Format("FAb{0}/{1}", FrontAnimationFrame, FrontAnimationTick);
                        }
                        else
                        {
                            szText = string.Format("FA{0}/{1}", FrontAnimationFrame, FrontAnimationTick);
                        }
                        dxFont.DrawText(DXManager.TextSprite, szText, drawX, drawY, Color.AliceBlue);
                    }
                }
            }
            font.Dispose();
            dxFont.Dispose();
        }

        private void DrawMiddleAnimationTag(bool blMiddleAnimationTag)
        {
            if (!blMiddleAnimationTag)
            {
                return;
            }
            var szText = "";
            int MiddleAnimationFrame;
            byte MiddleAnimationTick;
            bool BlendAnimation;
            var font = new Font("Comic Sans MS", 10, FontStyle.Bold);
            var dxFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, font);
            for (var y = mapPoint.Y; y <= mapPoint.Y + OffSetY; y++)
            {
                if (y >= mapHeight || y < 0) continue;
                drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                {
                    if (x >= mapWidth || x < 0) continue;
                    drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                    if (X2CellInfo[x, y].MiddleAnimationFrame == 255)
                    {
                        continue;
                    }
                    MiddleAnimationFrame = X2CellInfo[x, y].MiddleAnimationFrame & 0x0F;
                    MiddleAnimationTick = X2CellInfo[x, y].MiddleAnimationTick;
                    BlendAnimation = Convert.ToBoolean(X2CellInfo[x, y].MiddleAnimationFrame & 0x08);

                    if (MiddleAnimationFrame > 0)
                    {
                        if (BlendAnimation)
                        {
                            szText = string.Format("MAb{0}/{1}", MiddleAnimationFrame, MiddleAnimationTick);
                        }
                        else
                        {
                            szText = string.Format("MA{0}/{1}", MiddleAnimationFrame, MiddleAnimationTick);
                        }
                        dxFont.DrawText(DXManager.TextSprite, szText, drawX, drawY, Color.Black);
                    }
                }
            }
            font.Dispose();
            dxFont.Dispose();
        }

        private void chkFrontAnimationTag_Click(object sender, EventArgs e)
        {
            chkFrontAnimationTag.Checked = !chkFrontAnimationTag.Checked;
        }

        private void DrawLightTag(bool blLightTag)
        {
            if (!blLightTag)
            {
                return;
            }
            var szText = "";
            int Light;
            var font = new Font("Comic Sans MS", 10, FontStyle.Bold);
            var dxFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, font);
            for (var y = mapPoint.Y - 1; y <= mapPoint.Y + OffSetY + 35; y++)
            {
                if (y >= mapHeight || y < 0) continue;
                drawY = (y - mapPoint.Y)*(CellHeight*zoomMIN/zoomMAX);
                for (var x = mapPoint.X; x <= mapPoint.X + OffSetX + 35; x++)
                {
                    if (x >= mapWidth || x < 0) continue;
                    drawX = (x - mapPoint.X)*(CellWidth*zoomMIN/zoomMAX);
                    Light = X2CellInfo[x, y].Light;

                    if (Light > 0)
                    {
                        //Draw(1, 57, drawX, drawY);
                        Draw(99, 1, drawX, drawY);
                        szText = string.Format("L{0}", Light);
                        dxFont.DrawText(DXManager.TextSprite, szText, drawX + 24 * zoomMIN / zoomMAX, drawY, Color.AliceBlue);
                    }
                }
            }
            font.Dispose();
            dxFont.Dispose();
        }

        private void chkLightTag_Click(object sender, EventArgs e)
        {
            chkLightTag.Checked = !chkLightTag.Checked;
        }

        private void Save()
        {
            if (X2CellInfo == null)
            {
            }
            else
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Custom  Map (*.Map)|*.Map";
                saveFileDialog.FileName = "Custom  Map";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    var binaryWriter = new BinaryWriter(fileStream);
                    short ver = 1;
                    char[] tag = {'C', '#'};
                    binaryWriter.Write(ver);
                    binaryWriter.Write(tag);

                    binaryWriter.Write(Convert.ToInt16(mapWidth));
                    binaryWriter.Write(Convert.ToInt16(mapHeight));
                    for (var x = 0; x < mapWidth; x++)
                    {
                        for (var y = 0; y < mapHeight; y++)
                        {
                            binaryWriter.Write(X2CellInfo[x, y].BackIndex);
                            binaryWriter.Write(X2CellInfo[x, y].BackImage);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleIndex);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleImage);
                            binaryWriter.Write(X2CellInfo[x, y].FrontIndex);
                            binaryWriter.Write(X2CellInfo[x, y].FrontImage);
                            binaryWriter.Write(X2CellInfo[x, y].DoorIndex);
                            binaryWriter.Write(X2CellInfo[x, y].DoorOffset);
                            binaryWriter.Write(X2CellInfo[x, y].FrontAnimationFrame);
                            binaryWriter.Write(X2CellInfo[x, y].FrontAnimationTick);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleAnimationFrame);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleAnimationTick);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationImage);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationOffset);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationFrames);
                            binaryWriter.Write(X2CellInfo[x, y].Light);
                        }
                    }
                    binaryWriter.Flush();
                    binaryWriter.Dispose();
                    MessageBox.Show("Map Saved");
                }
            }
        }

        /*
        private void InvertMir3Layer()
        {
            if (X2CellInfo != null)
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Custom  Map (*.Mir3)|*.Mir3";
                saveFileDialog.FileName = "Invert Mir3 Layer";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    var binaryWriter = new BinaryWriter(fileStream);
                    short ver = 1;
                    char[] tag = {'C', '#'};
                    string str;
                    binaryWriter.Write(ver);
                    binaryWriter.Write(tag);

                    binaryWriter.Write(Convert.ToInt16(mapWidth));
                    binaryWriter.Write(Convert.ToInt16(mapHeight));
                    for (var x = 0; x < mapWidth; x++)
                    {
                        for (var y = 0; y < mapHeight; y++)
                        {
                            binaryWriter.Write(X2CellInfo[x, y].BackIndex);
                            binaryWriter.Write(X2CellInfo[x, y].BackImage);

                            if ((X2CellInfo[x, y].MiddleImage != 0) && ((X2CellInfo[x, y].FrontImage & 0x7FFF) == 0))
                            {
                                str = GetLibName(X2CellInfo[x, y].MiddleIndex);
                                if (!(str.IndexOf("SmTiles", StringComparison.Ordinal) > -1))
                                {
                                    if ((X2CellInfo[x, y].MiddleAnimationFrame != 0) &&
                                        (X2CellInfo[x, y].MiddleAnimationFrame != 255) &&
                                        (X2CellInfo[x, y].FrontAnimationFrame == 0))
                                    {
                                        X2CellInfo[x, y].FrontAnimationFrame =
                                            (byte) (X2CellInfo[x, y].MiddleAnimationFrame & 0x0F);
                                        X2CellInfo[x, y].FrontAnimationTick = X2CellInfo[x, y].MiddleAnimationTick;
                                        X2CellInfo[x, y].MiddleAnimationFrame = 0;
                                        X2CellInfo[x, y].MiddleAnimationTick = 0;
                                    }
                                    X2CellInfo[x, y].FrontImage = X2CellInfo[x, y].MiddleImage;
                                    X2CellInfo[x, y].FrontIndex = X2CellInfo[x, y].MiddleIndex;
                                    X2CellInfo[x, y].MiddleImage = 0;
                                    X2CellInfo[x, y].MiddleIndex = 0;
                                }
                            }
                            else if ((X2CellInfo[x, y].MiddleImage != 0) && ((X2CellInfo[x, y].FrontImage & 0x7FFF) != 0))
                            {
                                str = GetLibName(X2CellInfo[x, y].MiddleIndex);
                                if (!(str.IndexOf("SmTiles", StringComparison.Ordinal) > -1))
                                {
                                    if ((X2CellInfo[x, y].MiddleAnimationFrame == 255) ||
                                        (X2CellInfo[x, y].MiddleAnimationFrame == 0))
                                    {
                                        if (X2CellInfo[x, y].FrontAnimationFrame == 0)
                                        {
                                            var temp = X2CellInfo[x, y].MiddleImage;
                                            X2CellInfo[x, y].MiddleImage =
                                                (short) (X2CellInfo[x, y].FrontImage & 0x7FFF);
                                            X2CellInfo[x, y].FrontImage = temp;
                                            temp = X2CellInfo[x, y].MiddleIndex;
                                            X2CellInfo[x, y].MiddleIndex = X2CellInfo[x, y].FrontIndex;
                                            X2CellInfo[x, y].FrontIndex = temp;
                                        }
                                    }
                                }
                            }


                            binaryWriter.Write(X2CellInfo[x, y].MiddleIndex);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleImage);
                            binaryWriter.Write(X2CellInfo[x, y].FrontIndex);
                            binaryWriter.Write(X2CellInfo[x, y].FrontImage);
                            binaryWriter.Write(X2CellInfo[x, y].DoorIndex);
                            binaryWriter.Write(X2CellInfo[x, y].DoorOffset);
                            binaryWriter.Write(X2CellInfo[x, y].FrontAnimationFrame);
                            binaryWriter.Write(X2CellInfo[x, y].FrontAnimationTick);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleAnimationFrame);
                            binaryWriter.Write(X2CellInfo[x, y].MiddleAnimationTick);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationImage);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationOffset);
                            binaryWriter.Write(X2CellInfo[x, y].TileAnimationFrames);
                            binaryWriter.Write(X2CellInfo[x, y].Light);
                        }
                    }
                    binaryWriter.Flush();
                    binaryWriter.Dispose();
                    fileStream.Dispose();
                    MessageBox.Show("Map Saved");
                }
            }
        }

        */  

        private void WemadeMir2LibListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_wemadeExine2IndexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
                return;
            }

            _wemadeExine2IndexList.Add(e.ItemIndex, WemadeExine2ImageList.Images.Count);
            Libraries.MapLibs[wemadeExine2ListItem.Value].CheckImage(e.ItemIndex);
            WemadeExine2ImageList.Images.Add(Libraries.MapLibs[wemadeExine2ListItem.Value].GetPreview(e.ItemIndex));
            e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
            Libraries.MapLibs[wemadeExine2ListItem.Value].Images[e.ItemIndex] = null;
        }

        private void WemadeMir2LibListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Clear();
            wemadeExine2ListItem = (ListItem) WemadeExine2LibListBox.SelectedItem;
            selectListItem = wemadeExine2ListItem;
            selectListItem.Version = (byte) ExineVerSion.Exine;
            WemadeMir2LibListView.VirtualListSize = Libraries.MapLibs[wemadeExine2ListItem.Value].Images.Count;
            TileslistView.VirtualListSize = Libraries.MapLibs[wemadeExine2ListItem.Value].Images.Count/Exine2BigTileBlock;
        }
        
        //k333123 remove
        /*
        private void WemadeMir3LibListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_wemadeExine3IndexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
                return;
            }

            _wemadeExine3IndexList.Add(e.ItemIndex, WemadeExine3ImageList.Images.Count);
            Libraries.MapLibs[wemadeExine3ListItem.Value].CheckImage(e.ItemIndex);
            WemadeExine3ImageList.Images.Add(Libraries.MapLibs[wemadeExine3ListItem.Value].GetPreview(e.ItemIndex));
            e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
            Libraries.MapLibs[wemadeExine3ListItem.Value].Images[e.ItemIndex] = null;
        }
        */
        

        private void chkMiddleAnimationTag_Click(object sender, EventArgs e)
        {
            chkMiddleAnimationTag.Checked = !chkMiddleAnimationTag.Checked;
        }

        private void cmbEditorLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbEditorLayer.SelectedIndex)
            {
                case 0:
                    layer = Layer.None;
                    break;
                case 1:
                    layer = Layer.BackImage;
                    break;
                case 2:
                    layer = Layer.MiddleImage;
                    break;
                case 3:
                    layer = Layer.FrontImage;
                    break;
                case 4:
                    layer = Layer.BackLimit;
                    break;
                case 5:
                    layer = Layer.FrontLimit;
                    break;
                case 6:
                    layer = Layer.BackFrontLimit;
                    break;
                case 7:
                    layer = Layer.GraspingMir2Front;
                    break;
                case 8:
                    layer = Layer.GraspingInvertMir3FrontMiddle;
                    break;
                case 9:
                    layer = Layer.PlaceObjects;
                    break;
                case 10:
                    layer = Layer.ClearAll;
                    break;
                case 11:
                    layer = Layer.ClearBack;
                    break;
                case 12:
                    layer = Layer.ClearMidd;
                    break;
                case 13:
                    layer = Layer.ClearFront;
                    break;
                case 14:
                    layer = Layer.ClearBackFrontLimit;
                    break;
                case 15:
                    layer = Layer.ClearBackLimit;
                    break;
                case 16:
                    layer = Layer.ClearFrontLimit;
                    break;
                case 17:
                    layer = Layer.BrushMir2BigTiles;
                    break;
                case 18:
                    layer = Layer.BrushSmTiles;
                    break;
                case 19:
                    layer = Layer.BrushMir3BigTiles;
                    break;
            }
        }

        private void WemadeMir2LibListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectListItem = wemadeExine2ListItem;
            //Set a breakpoint here and find that this event is actually executed twice after clicking on different Items.
            //The first time is to cancel the current Item selected state, causing the SelectedIndices of the entire ListView to become 0.
            //The newly selected Item is set to the selected state for the second time, and SelectedIndices becomes 1.
            //If you do not add listview.SelectedIndices.Count>0 to determine, it will result in an out-of-bounds exception in obtaining listview.Items[] index.
            if (WemadeMir2LibListView.SelectedIndices.Count > 0)
            {
                selectLibMImage =
                    Libraries.MapLibs[wemadeExine2ListItem.Value].GetMImage(WemadeMir2LibListView.SelectedIndices[0]);
                if (selectLibMImage != null)
                {
                    selectImageIndex = WemadeMir2LibListView.SelectedIndices[0];

                    picWemdeMir2.Image = selectLibMImage.Image;
                    LabWemadeMir2Width.Text = "Width : " + selectLibMImage.Width;
                    LabWemadeMir2Height.Text = "Height : " + selectLibMImage.Height;
                    labeWemadeMir2OffSetX.Text = "OffSetX : " + selectLibMImage.X;
                    labWemadeMir2OffSetY.Text = "OffSetY : " + selectLibMImage.Y;
                    Libraries.MapLibs[wemadeExine2ListItem.Value].Images[WemadeMir2LibListView.SelectedIndices[0]] = null;
                }
            }
        }

        private bool CheckImageSizeIsBigTile(int libIndex, int index)
        {
            var s = Libraries.MapLibs[libIndex].GetSize(index);
            if ((s.Width != 2*CellWidth) && (s.Height != 2*CellHeight))
            {
                return false;
            }
            return true;
        }

        private Point CheckPointIsEven(Point point)
        {
            if ((point.X%2 == 0) && (point.Y%2 == 0))
            {
                return point;
            }
            if (point.X%2 != 0)
            {
                point.X = point.X - 1;
            }
            if (point.Y%2 != 0)
            {
                point.Y = point.Y - 1;
            }
            return point;
        }

        private void MapPanel_MouseClick(object sender, MouseEventArgs e)
        {
            Point[] points;
            Point temp;
            if (X2CellInfo == null) return;
            switch (layer)
            {
                case Layer.BackImage:
                    if (selectListItem == null) return;
                    /*
                    if (CheckImageSizeIsBigTile(selectListItem.Value, selectImageIndex))
                    {
                        temp = CheckPointIsEven(new Point(cellX, cellY));
                        cellX = temp.X;
                        cellY = temp.Y;
                        points = new[] {new Point(cellX, cellY)};
                        AddCellInfoPoints(points);
                        X2CellInfo[cellX, cellY].BackIndex = Convert.ToInt16(selectListItem.Value);
                        X2CellInfo[cellX, cellY].BackImage = selectImageIndex + 1;
                    }*/
                    points = new[] { new Point(cellX, cellY) };
                    AddCellInfoPoints(points);
                    X2CellInfo[cellX, cellY].BackIndex = Convert.ToInt16(selectListItem.Value);
                    X2CellInfo[cellX, cellY].BackImage = selectImageIndex + 1;
                    break;
                case Layer.MiddleImage:
                    if (selectListItem == null) return;
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    X2CellInfo[cellX, cellY].MiddleIndex = Convert.ToInt16(selectListItem.Value);
                    X2CellInfo[cellX, cellY].MiddleImage = Convert.ToInt16(selectImageIndex + 1);

                    break;
                case Layer.FrontImage:
                    if (selectListItem == null) return;
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    X2CellInfo[cellX, cellY].FrontIndex = Convert.ToInt16(selectListItem.Value);
                    X2CellInfo[cellX, cellY].FrontImage = Convert.ToInt16(selectImageIndex + 1);
                    break;
                case Layer.BackLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    SetBackLimit();
                    break;
                case Layer.FrontLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    SetFrontLimit();
                    break;
                case Layer.BackFrontLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    SetBackLimit();
                    SetFrontLimit();
                    break;
                case Layer.PlaceObjects:
                    points = GetObjectDatasPoints(objectDatas);
                    if (AddCellInfoPoints(points) && (objectDatas != null))
                    {
                        ModifyM2CellInfo(objectDatas);
                    }
                    break;
                case Layer.ClearAll:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearAll();
                    break;
                case Layer.ClearBack:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearBack();
                    break;
                case Layer.ClearMidd:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearMidd();
                    break;
                case Layer.ClearFront:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearFront();
                    break;
                case Layer.ClearBackFrontLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearBackLimit();
                    ClearFrontLimit();
                    break;
                case Layer.ClearBackLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearBackLimit();
                    break;
                case Layer.ClearFrontLimit:
                    points = new[] {new Point(cellX, cellY)};
                    AddCellInfoPoints(points);
                    ClearFrontLimit();
                    break;
                case Layer.BrushMir2BigTiles:
                    temp = CheckPointIsEven(new Point(cellX, cellY));
                    cellX = temp.X;
                    cellY = temp.Y;
                    CreateMir2BigTiles();
                    AddCellInfoPoints(bigTilePoints.ToArray());
                    break;
                case Layer.BrushSmTiles:
                    CreateSmTiles();
                    AddCellInfoPoints(smTilePoints.ToArray());
                    break;
                case Layer.BrushMir3BigTiles:
                    temp = CheckPointIsEven(new Point(cellX, cellY));
                    cellX = temp.X;
                    cellY = temp.Y;
                    CreateMir3BigTiles();
                    AddCellInfoPoints(bigTilePoints.ToArray());
                    break;
            }
        }

        private Point[] GetObjectDatasPoints(CellInfoData[] datas)
        {
            if (datas == null) return null;
            var list = new List<Point>();
            for (var i = 0; i < datas.Length; i++)
            {
                list.Add(new Point(datas[i].X + cellX, datas[i].Y + cellY));
            }
            return list.ToArray();
        }

        private void ModifyM2CellInfo(CellInfoData[] datas)
        {
            for (var i = 0; i < objectDatas.Length; i++)
            {
                if ((datas[i].CellInfo.BackImage & 0x20000000) != 0)
                {
                    if (((datas[i].CellInfo.BackImage & 0x1FFFF) - 1) == -1 || datas[i].CellInfo.BackIndex == -1)
                    {
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].BackImage |= 0x20000000;

                    }
                    else
                    {
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].BackImage = datas[i].CellInfo.BackImage;
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].BackIndex = datas[i].CellInfo.BackIndex;
                    }
                }
                else
                {
                    if (((datas[i].CellInfo.BackImage & 0x1FFFF) - 1) == -1 || datas[i].CellInfo.BackIndex == -1)
                    {
                        //do nothing
                    }
                    else
                    {
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].BackImage = datas[i].CellInfo.BackImage;
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].BackIndex = datas[i].CellInfo.BackIndex;
                    }
                }

                //middle
                if (objectDatas[i].CellInfo.MiddleImage != 0)
                {
                    if ((X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleAnimationFrame & 0x7F) != 0)
                    {
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleAnimationFrame = 0;
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleAnimationTick = 0;
                    }
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleImage = datas[i].CellInfo.MiddleImage;
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleIndex = datas[i].CellInfo.MiddleIndex;
                }
                if (objectDatas[i].CellInfo.MiddleIndex != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleIndex =
                        datas[i].CellInfo.MiddleIndex;
                }
                //front
                if (objectDatas[i].CellInfo.FrontImage != 0)
                {
                    if ((X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontAnimationFrame & 0x7F) != 0)
                    {
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontAnimationFrame = 0;
                        X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontAnimationTick = 0;
                    }
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontImage = datas[i].CellInfo.FrontImage;
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontIndex = datas[i].CellInfo.FrontIndex;
                }
                if (objectDatas[i].CellInfo.FrontIndex != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontIndex = datas[i].CellInfo.FrontIndex;
                }
                //Door
                if (datas[i].CellInfo.DoorIndex != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].DoorIndex = datas[i].CellInfo.DoorIndex;
                }
                if (datas[i].CellInfo.DoorOffset != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].DoorOffset = datas[i].CellInfo.DoorOffset;
                }
                //Animation
                if (datas[i].CellInfo.MiddleAnimationFrame != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleAnimationFrame = datas[i].CellInfo.MiddleAnimationFrame;
                }
                if (datas[i].CellInfo.MiddleAnimationTick != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].MiddleAnimationTick = datas[i].CellInfo.MiddleAnimationTick;
                }
                if (datas[i].CellInfo.FrontAnimationFrame != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontAnimationFrame = datas[i].CellInfo.FrontAnimationFrame;
                }
                if (datas[i].CellInfo.FrontAnimationTick != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].FrontAnimationTick = datas[i].CellInfo.FrontAnimationTick;
                }
                //light
                if (datas[i].CellInfo.Light != 0)
                {
                    X2CellInfo[cellX + datas[i].X, cellY + datas[i].Y].Light = datas[i].CellInfo.Light;
                }
            }
        }

        private bool CheckCellInfoIsZero(CellInfo cellInfo)
        {
            if ((cellInfo.BackImage == 0) &&
                (cellInfo.BackIndex == 0) &&
                (cellInfo.MiddleImage == 0) &&
                (cellInfo.MiddleIndex == 0) &&
                (cellInfo.FrontImage == 0) &&
                (cellInfo.FrontIndex == 0) &&
                (cellInfo.DoorIndex == 0) &&
                (cellInfo.DoorOffset == 0) &&
                (cellInfo.FrontAnimationFrame == 0) &&
                (cellInfo.FrontAnimationTick == 0) &&
                (cellInfo.MiddleAnimationFrame == 0) &&
                (cellInfo.MiddleAnimationTick == 0) &&
                (cellInfo.TileAnimationImage == 0) &&
                (cellInfo.TileAnimationOffset == 0) &&
                (cellInfo.TileAnimationFrames == 0) &&
                (cellInfo.Light == 0))
            {
                return true;
            }
            return false;
        }
         

        private void chkDrawGrids_Click(object sender, EventArgs e)
        {
            chkDrawGrids.Checked = !chkDrawGrids.Checked;
        }

        private bool AddCellInfoPoints(Point[] points)
        {
            if (points == null) return false;
            if (X2CellInfo != null)
            {
                cellInfoDatas = new CellInfoData[points.Length];
                for (var i = 0; i < cellInfoDatas.Length; i++)
                {
                    if (points[i].X >= mapWidth || points[i].Y >= mapHeight || points[i].X < 0 || points[i].Y < 0)
                    {
                        MessageBox.Show("Object Outside Map Boundary!");
                        return false;
                    }
                    cellInfoDatas[i] = new CellInfoData(points[i].X, points[i].Y,
                        X2CellInfo[points[i].X, points[i].Y]);
                }
                _editor.UnDo = cellInfoDatas;
                return true;
            }
            return false;
        }

        private bool AddCellInfoPoints(CellInfoData[] datas)
        {
            if (datas.Length <= 0)
            {
                return false;
            }
            _editor.UnDo = datas;
            return true;
        }

        private void UnDo()
        {
            unTemp = _editor.UnDo;
            if (unTemp == null) return;
            reTemp = new CellInfoData[unTemp.Length];
            for (var i = 0; i < unTemp.Length; i++)
            {
                var x = unTemp[i].X;
                var y = unTemp[i].Y;
                reTemp[i] = new CellInfoData(unTemp[i].X, unTemp[i].Y, X2CellInfo[unTemp[i].X, unTemp[i].Y]);
                X2CellInfo[x, y] = unTemp[i].CellInfo;
            }
            _editor.ReDo = reTemp;
        }

        private void ReDo()
        {
            reTemp = _editor.ReDo;
            if (reTemp == null) return;
            unTemp = new CellInfoData[reTemp.Length];
            for (var i = 0; i < reTemp.Length; i++)
            {
                var x = reTemp[i].X;
                var y = reTemp[i].Y;
                unTemp[i] = new CellInfoData(reTemp[i].X, reTemp[i].Y, X2CellInfo[reTemp[i].X, reTemp[i].Y]);
                X2CellInfo[x, y] = reTemp[i].CellInfo;
            }
            _editor.UnDo = unTemp;
        }

        private void SaveObjectsFile()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Object (*.X)|*.X";
            saveFileDialog.FileName = "Object";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite);
                var binaryWriter = new BinaryWriter(file);
                var flag = false;
                int tempX = 0, tempY = 0;
                var list = new List<CellInfoData>();
                for (var y = p1.Y; y <= p2.Y; y++)
                {
                    for (var x = p1.X; x <= p2.X; x++)
                    {
                        if (!flag)
                        {
                            if (CheckCellInfoIsZero(X2CellInfo[x, y]))
                            {
                            }
                            else
                            {
                                flag = true;
                                tempX = x;
                                tempY = y;
                                list.Add(new CellInfoData(x - tempX, y - tempY, X2CellInfo[x, y]));
                            }
                        }
                        else
                        {
                            if (CheckCellInfoIsZero(X2CellInfo[x, y]))
                            {
                            }
                            else
                            {
                                list.Add(new CellInfoData(x - tempX, y - tempY, X2CellInfo[x, y]));
                            }
                        }
                    }
                }

                binaryWriter.Write(list.Count);
                for (var i = 0; i < list.Count; i++)
                {
                    binaryWriter.Write(list[i].X);
                    binaryWriter.Write(list[i].Y);
                    binaryWriter.Write(list[i].CellInfo.BackIndex);
                    binaryWriter.Write(list[i].CellInfo.BackImage);
                    binaryWriter.Write(list[i].CellInfo.MiddleIndex);
                    binaryWriter.Write(list[i].CellInfo.MiddleImage);
                    binaryWriter.Write(list[i].CellInfo.FrontIndex);
                    binaryWriter.Write(list[i].CellInfo.FrontImage);
                    binaryWriter.Write(list[i].CellInfo.DoorIndex);
                    binaryWriter.Write(list[i].CellInfo.DoorOffset);
                    binaryWriter.Write(list[i].CellInfo.FrontAnimationFrame);
                    binaryWriter.Write(list[i].CellInfo.FrontAnimationTick);
                    binaryWriter.Write(list[i].CellInfo.MiddleAnimationFrame);
                    binaryWriter.Write(list[i].CellInfo.MiddleAnimationTick);
                    binaryWriter.Write(list[i].CellInfo.TileAnimationImage);
                    binaryWriter.Write(list[i].CellInfo.TileAnimationOffset);
                    binaryWriter.Write(list[i].CellInfo.TileAnimationFrames);
                    binaryWriter.Write(list[i].CellInfo.Light);
                }
                binaryWriter.Flush();
                binaryWriter.Dispose();
                file.Dispose();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e)
        {
            //stops the p1 and p2 values resetting when right clicking to get the menu
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }



            if (X2CellInfo != null)
            {
                switch (layer)
                {
                    case Layer.GraspingMir2Front:
                        p1 = new Point(cellX, cellY);
                        p2 = p1; //sets p2 to the first cell clicked so we don't have to move mouse to update the area selected

                        //p2 = Point.Empty;
                        Grasping = true;
                        break;
                    case Layer.GraspingInvertMir3FrontMiddle:
                        p1 = new Point(cellX, cellY);
                        p2 = p1;
                        //p2 = Point.Empty;
                        Grasping = true;
                        break;
                }
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (X2CellInfo != null)
            {
                switch (layer)
                {
                    case Layer.GraspingMir2Front:
                        GraspingData();
                        Grasping = false;
                        break;
                    case Layer.GraspingInvertMir3FrontMiddle:
                        GraspingData();
                        Grasping = false;
                        break;
                }
            }
        }

        private void GraspingData()
        {
            if (p1.IsEmpty || p2.IsEmpty) return;
            if ((layer == Layer.GraspingMir2Front) || (layer == Layer.GraspingInvertMir3FrontMiddle))
            {
                if (X2CellInfo != null)
                {
                    var w = Math.Abs(p2.X - p1.X + 1);
                    var h = Math.Abs(p2.Y - p1.Y + 1);
                    objectDatas = new CellInfoData[w*h];
                    var z = 0;
                    for (int x = p1.X, i = 0; x <= p2.X; x++, i++)
                    {
                        for (int y = p1.Y, j = 0; y <= p2.Y; y++, j++)
                        {
                            objectDatas[z] = new CellInfoData();
                            objectDatas[z].CellInfo = new CellInfo();
                            objectDatas[z].X = x - p1.X - 1;
                            objectDatas[z].Y = y - p2.Y - 1;
                            objectDatas[z].CellInfo.BackImage = X2CellInfo[x, y].BackImage & 0x20000000;
                            objectDatas[z].CellInfo.BackIndex = 0;
                            if (layer == Layer.GraspingMir2Front)
                            {
                                objectDatas[z].CellInfo.MiddleImage = 0;
                                objectDatas[z].CellInfo.MiddleIndex = 0;
                            }
                            else if (layer == Layer.GraspingInvertMir3FrontMiddle)
                            {
                                objectDatas[z].CellInfo.MiddleImage = X2CellInfo[x, y].MiddleImage;
                                objectDatas[z].CellInfo.MiddleIndex = X2CellInfo[x, y].MiddleIndex;
                            }

                            objectDatas[z].CellInfo.MiddleAnimationFrame = 0;
                            objectDatas[z].CellInfo.MiddleAnimationTick = 0;
                            objectDatas[z].CellInfo.FrontImage = X2CellInfo[x, y].FrontImage;
                            objectDatas[z].CellInfo.FrontIndex = X2CellInfo[x, y].FrontIndex;
                            objectDatas[z].CellInfo.FrontAnimationFrame = X2CellInfo[x, y].FrontAnimationFrame;
                            objectDatas[z].CellInfo.FrontAnimationTick = X2CellInfo[x, y].FrontAnimationTick;
                            objectDatas[z].CellInfo.DoorIndex = X2CellInfo[x, y].DoorIndex;
                            objectDatas[z].CellInfo.DoorOffset = X2CellInfo[x, y].DoorOffset;
                            objectDatas[z].CellInfo.TileAnimationImage = X2CellInfo[x, y].TileAnimationImage;
                            objectDatas[z].CellInfo.TileAnimationFrames = X2CellInfo[x, y].TileAnimationFrames;
                            objectDatas[z].CellInfo.TileAnimationOffset = X2CellInfo[x, y].TileAnimationOffset;
                            objectDatas[z].CellInfo.Light = X2CellInfo[x, y].Light;
                            objectDatas[z].CellInfo.FishingCell = X2CellInfo[x, y].FishingCell;
                            z++;
                        }
                    }
                }
            }
        }

        private void ReadObjectsToListBox()
        {
            if (!Directory.Exists(Libraries.ObjectsPath))
            {
                Directory.CreateDirectory(Libraries.ObjectsPath);
            }

            var files = (from x in Directory.EnumerateFileSystemEntries(Libraries.ObjectsPath, "*.X", SearchOption.AllDirectories)
                        orderby x
                        select x).ToArray();

            Array.Sort(files, new AlphanumComparatorFast());

            foreach (string file in files)
            {
                ObjectslistBox.Items.Add(file.Replace(Libraries.ObjectsPath, "").Replace(".X", ""));
            }
        }

        private CellInfoData[] ReadObjectsFile(string objectFile)
        {
            byte[] Bytes;
            var offset = 0;
            var count = 0;
            CellInfoData[] objectDatas = null;
            if (File.Exists(objectFile))
            {
                Bytes = File.ReadAllBytes(objectFile);
                count = BitConverter.ToInt32(Bytes, offset);
                offset += 4;
                objectDatas = new CellInfoData[count];
                for (var i = 0; i < count; i++)
                {
                    objectDatas[i] = new CellInfoData();
                    objectDatas[i].CellInfo = new CellInfo();
                    objectDatas[i].X = BitConverter.ToInt32(Bytes, offset);
                    offset += 4;
                    objectDatas[i].Y = BitConverter.ToInt32(Bytes, offset);
                    offset += 4;
                    objectDatas[i].CellInfo.BackIndex = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.BackImage = BitConverter.ToInt32(Bytes, offset);
                    offset += 4;
                    objectDatas[i].CellInfo.MiddleIndex = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.MiddleImage = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.FrontIndex = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.FrontImage = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.DoorIndex = Bytes[offset++];
                    objectDatas[i].CellInfo.DoorOffset = Bytes[offset++];
                    objectDatas[i].CellInfo.FrontAnimationFrame = Bytes[offset++];
                    objectDatas[i].CellInfo.FrontAnimationTick = Bytes[offset++];
                    objectDatas[i].CellInfo.MiddleAnimationFrame = Bytes[offset++];
                    objectDatas[i].CellInfo.MiddleAnimationTick = Bytes[offset++];
                    objectDatas[i].CellInfo.TileAnimationImage = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.TileAnimationOffset = BitConverter.ToInt16(Bytes, offset);
                    offset += 2;
                    objectDatas[i].CellInfo.TileAnimationFrames = Bytes[offset++];
                    objectDatas[i].CellInfo.Light = Bytes[offset++];

                    if (objectDatas[i].CellInfo.Light == 100 || objectDatas[i].CellInfo.Light == 101)
                        objectDatas[i].CellInfo.FishingCell = true;
                }
            }
            return objectDatas;
        }

        private Bitmap GetObjectPreview(int w, int h, CellInfoData[] datas)
        {
            if (datas == null)
            {
                return null;
            }
            var preview = new Bitmap(w*CellWidth, h*CellHeight);
            var graphics = Graphics.FromImage(preview);
            graphics.InterpolationMode = InterpolationMode.Low;

            for (var i = 0; i < datas.Length; i++)
            {
                if (datas[i].Y%2 != 0) continue;
                drawY = (datas[i].Y + h/4)*CellHeight;
                if (datas[i].X%2 != 0) continue;
                drawX = (datas[i].X + w/4)*CellWidth;

                index = (datas[i].CellInfo.BackImage & 0x1FFFFFFF) - 1;
                libIndex = datas[i].CellInfo.BackIndex;

                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                Libraries.MapLibs[libIndex].CheckImage(index);
                var mi = Libraries.MapLibs[libIndex].Images[index];

                if (mi.Image == null) continue;

                var destRect = new Rectangle(drawX, drawY, mi.Width, mi.Height);
                var srcRect = new Rectangle(0, 0, mi.Width, mi.Height);
                graphics.DrawImage(mi.Image, destRect, srcRect, GraphicsUnit.Pixel);
                Libraries.MapLibs[libIndex].Images[index] = null;
            }

            for (var i = 0; i < datas.Length; i++)
            {
                drawX = (datas[i].X + w/4)*CellWidth;
                index = datas[i].CellInfo.MiddleImage - 1;
                libIndex = datas[i].CellInfo.MiddleIndex;
                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                var s = Libraries.MapLibs[libIndex].GetSize(index);
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (datas[i].Y + 1 + h/4)*CellHeight - s.Height;
                }
                else
                {
                    drawY = (datas[i].Y + h/4)*CellHeight;
                }
                Libraries.MapLibs[libIndex].CheckImage(index);
                var mi = Libraries.MapLibs[libIndex].Images[index];
                if (mi.Image == null) continue;
                var destRect = new Rectangle(drawX, drawY, mi.Width, mi.Height);
                var srcRect = new Rectangle(0, 0, mi.Width, mi.Height);
                graphics.DrawImage(mi.Image, destRect, srcRect, GraphicsUnit.Pixel);
                Libraries.MapLibs[libIndex].Images[index] = null;
            }

            for (var i = 0; i < datas.Length; i++)
            {
                drawX = (datas[i].X + w/4)*CellWidth;
                index = (datas[i].CellInfo.FrontImage & 0x7FFF) - 1;
                libIndex = datas[i].CellInfo.FrontIndex;
                if (libIndex < 0 || libIndex >= Libraries.MapLibs.Length) continue;
                if (index < 0 || index >= Libraries.MapLibs[libIndex].Images.Count) continue;

                var s = Libraries.MapLibs[libIndex].GetSize(index);
                if ((s.Width != CellWidth || s.Height != CellHeight) &&
                    (s.Width != CellWidth*2 || s.Height != CellHeight*2))
                {
                    drawY = (datas[i].Y + 1 + h/4)*CellHeight - s.Height;
                }
                else
                {
                    drawY = (datas[i].Y + h/4)*CellHeight;
                }
                Libraries.MapLibs[libIndex].CheckImage(index);
                var mi = Libraries.MapLibs[libIndex].Images[index];
                if (mi.Image == null) continue;
                var destRect = new Rectangle(drawX, drawY, mi.Width, mi.Height);
                var srcRect = new Rectangle(0, 0, mi.Width, mi.Height);
                graphics.DrawImage(mi.Image, destRect, srcRect, GraphicsUnit.Pixel);
                Libraries.MapLibs[libIndex].Images[index] = null;
            }

            graphics.Save();
            graphics.Dispose();

            return preview;
        }

        private void chkFrontTag_Click(object sender, EventArgs e)
        {
            chkFrontTag.Checked = !chkFrontTag.Checked;
        }

        private void btnDeleteObjects_Click(object sender, EventArgs e)
        {
            var name = ObjectslistBox.SelectedItem + ".X";
            var objectFile = Application.StartupPath + "\\Data\\Objects\\" + name;
            if (File.Exists(objectFile))
            {
                File.Delete(objectFile);
                ObjectslistBox.Items.Clear();
                ReadObjectsToListBox();
            }
        }

        private void ClearAll()
        {
            X2CellInfo[cellX, cellY] = new CellInfo();
        }

        private void ClearBack()
        {
            X2CellInfo[cellX, cellY].BackIndex = 0;
            X2CellInfo[cellX, cellY].BackImage = 0;
        }

        private void ClearMidd()
        {
            X2CellInfo[cellX, cellY].MiddleImage = 0;
            X2CellInfo[cellX, cellY].MiddleIndex = 0;
            X2CellInfo[cellX, cellY].MiddleAnimationFrame = 0;
            X2CellInfo[cellX, cellY].MiddleAnimationTick = 0;
        }

        private void ClearFront()
        {
            X2CellInfo[cellX, cellY].FrontImage = 0;
            X2CellInfo[cellX, cellY].FrontIndex = 0;
            X2CellInfo[cellX, cellY].FrontAnimationFrame = 0;
            X2CellInfo[cellX, cellY].FrontAnimationTick = 0;
        }

        private void ClearBackLimit()
        {
            X2CellInfo[cellX, cellY].BackImage = X2CellInfo[cellX, cellY].BackImage & 0x1fffffff;
        }

        private void ClearFrontLimit()
        {
            X2CellInfo[cellX, cellY].FrontImage = (short) (X2CellInfo[cellX, cellY].FrontImage & 0x7fff);
        }

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnDo();
        }

        private void 返回ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReDo();
        }

        private void btnSetDoor_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                Form setDoorForm = new FrmSetDoor(SetDoorProperty);
                if (setDoorForm.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void SetDoorProperty(bool blCoreDoor, byte index, byte offSet)
        {
            Point[] points = {new Point(cellX, cellY)};
            AddCellInfoPoints(points);

            X2CellInfo[cellX, cellY].DoorIndex = (byte) (X2CellInfo[cellX, cellY].DoorIndex | index);
            X2CellInfo[cellX, cellY].DoorOffset = offSet;
            if (blCoreDoor)
            {
                X2CellInfo[cellX, cellY].DoorIndex = (byte) (X2CellInfo[cellX, cellY].DoorIndex | 0x80);
            }
            else
            {
                X2CellInfo[cellX, cellY].DoorIndex = (byte) (X2CellInfo[cellX, cellY].DoorIndex & 0x7F);
            }
        }

        private void btnSetAnimation_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                Form setAnimation = new FrmSetAnimation(SetAnimationProperty);
                if (setAnimation.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void SetAnimationProperty(bool blend, byte frame, byte tick)
        {
            Point[] points = {new Point(cellX, cellY)};
            AddCellInfoPoints(points);
            //(byte)(M2CellInfo[cellX, cellY].FrontAnimationFrame | frame)
            X2CellInfo[cellX, cellY].FrontAnimationFrame = frame;
            X2CellInfo[cellX, cellY].FrontAnimationTick = tick;
            if (blend)
            {
                X2CellInfo[cellX, cellY].FrontAnimationFrame =
                    (byte) (X2CellInfo[cellX, cellY].FrontAnimationFrame | 0x80);
            }
            else
            {
                X2CellInfo[cellX, cellY].FrontAnimationFrame =
                    (byte) (X2CellInfo[cellX, cellY].FrontAnimationFrame & 0x7F);
            }
        }

        private void btnSetLight_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                Form setLight = new FrmSetLight(SetLightProperty);
                if (setLight.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void SetLightProperty(byte light)
        {
            Point[] points = {new Point(cellX, cellY)};
            AddCellInfoPoints(points);
            X2CellInfo[cellX, cellY].Light = light;
        }

        private void SetBackLimit()
        {
            if (X2CellInfo != null)
            {
                X2CellInfo[cellX, cellY].BackImage = X2CellInfo[cellX, cellY].BackImage | 0x20000000;
            }
        }

        private void SetFrontLimit()
        {
            if (X2CellInfo != null)
            {
                X2CellInfo[cellX, cellY].FrontImage =
                    (short) (X2CellInfo[cellX, cellY].FrontImage | 0x8000);
            }
        }

        private void btnJump_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                Form jump = new FrmJump(Jump);
                if (jump.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        private void Jump(int x, int y)
        {
            //if (x - OffSetX/2 >= mapWidth || y - OffSetY/2 >= mapHeight)
            //{
            //    MessageBox.Show("X,Y is error point");
            //    return;
            //}
            //if (x - OffSetX/2 < 0 || y - OffSetY/2 < 0)
            //{
            //    mapPoint.X = x;
            //    mapPoint.Y = y;
            //    return;
            //}
            //mapPoint.X = x - OffSetX/2;
            //mapPoint.Y = y - OffSetX/2;

            //sets the mapPoint
            mapPoint.X = x;
            mapPoint.Y = y;


            //checks if the mapPoint is within the map limits and sets it to min or max position if not
            if (mapPoint.X + OffSetX >= mapWidth)
            {
                mapPoint.X = mapWidth - OffSetX - 1;
            }

            if (mapPoint.X < 0)
            {
                mapPoint.X = 0;
            }

            if (mapPoint.Y < 0)
            {
                mapPoint.Y = 0;
            }

            if (mapPoint.Y + OffSetY >= mapHeight)
            {
                mapPoint.Y = mapHeight - OffSetY - 1;
            }
            setScrollBar();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //Movement
                case Keys.D:
                    Jump(mapPoint.X + 1, mapPoint.Y);
                    break;
                case Keys.A:
                    Jump(mapPoint.X - 1, mapPoint.Y);
                    break;
                case Keys.W:
                    Jump(mapPoint.X, mapPoint.Y - 1);
                    break;
                case Keys.S:
                    Jump(mapPoint.X, mapPoint.Y + 1);
                    break;

                //Shortcuts
                case Keys.J:
                    if (X2CellInfo != null)
                    {
                        Form jump = new FrmJump(Jump);
                        if (jump.ShowDialog() == DialogResult.OK)
                        {
                        }
                    }
                    break;
                case Keys.B: //Added by M2P
                    layer = Layer.BackLimit;
                    cmbEditorLayer.SelectedIndex = 4;
                    break;
                case Keys.F: //Added by M2P
                    layer = Layer.FrontLimit;
                    cmbEditorLayer.SelectedIndex = 5;
                    break;
                case Keys.C: //Added by M2P
                    layer = Layer.ClearBackFrontLimit;
                    cmbEditorLayer.SelectedIndex = 14;
                    break;
                case Keys.G:
                    chkDrawGrids.Checked = !chkDrawGrids.Checked;
                    break;
                case Keys.H: //Added by M2P
                    tabControl1.SelectedTab = tabHelp;
                    break;
                case Keys.M: //Added by M2P
                    createMiniMap();
                    break;
                case Keys.N:
                    NewMap();
                    break;
                case Keys.O:
                    OpenMap();
                    break;
                case Keys.R:
                    tabControl1.SelectedTab = tabObjects;
                    break;
               // case Keys.S: //Added by M2P - If set 'Save Map' Shortcut 'Ctrl-S' in 'Properties -> menuSave', if in back tiles placing mode, Saving map triggers back tiles brushing since 'Ctrl' key is not 'released'!
               //     if (e.Control)
               //         Save();
               //     break;
                case Keys.T:
                    tabControl1.SelectedTab = tabTiles;
                    break;
                case Keys.X: //Added by M2P
                    if (e.Control)
                SaveObjectsFile();
                break;
                case Keys.Z:
                    if (e.Control)
                    {
                        ReDo();
                    }
                    else
                    {
                        UnDo();
                    }
                    break;
                case Keys.Oemcomma:
                    selectImageIndex--;
                    break;
                case Keys.OemPeriod:
                    selectImageIndex++;
                    break;
                case Keys.Add:
                    
                    if (e.Shift)
                    {
                        zoomMIN = zoomMAX;
                    }
                    ZoomIn();
                        break;
                case Keys.Subtract:
                    if (e.Shift)
                    {
                        zoomMIN = 1;
                    }
                    ZoomOut();
                    break;
                    

                case Keys.Oem8: //The ` key next to the number keys ... not a tilde and not a ' 
                    layer = Layer.None;
                    cmbEditorLayer.SelectedIndex = 0;
                    break;
                case Keys.Escape:
                    layer = Layer.None;
                    cmbEditorLayer.SelectedIndex = 0;
                    break;
                case Keys.Oemtilde: 
                    layer = Layer.None;
                    cmbEditorLayer.SelectedIndex = 0;
                    break;


                case Keys.D1:
                    layer = Layer.FrontImage;
                    cmbEditorLayer.SelectedIndex = 3;
                    break;
                case Keys.D2:
                    layer = Layer.MiddleImage;
                    cmbEditorLayer.SelectedIndex = 2;
                    break;
                case Keys.D3:
                    layer = Layer.BackImage;
                    cmbEditorLayer.SelectedIndex = 1;
                    break;
                case Keys.D4: //Added by M2P
                    layer = Layer.GraspingMir2Front;
                    cmbEditorLayer.SelectedIndex = 7;
                    break;
                case Keys.D5:
                    layer = Layer.PlaceObjects;
                    cmbEditorLayer.SelectedIndex = 9;
                    break;
                case Keys.D6: //Added by M2P
                    layer = Layer.GraspingInvertMir3FrontMiddle;
                    cmbEditorLayer.SelectedIndex = 8;
                    break;
                case Keys.D7:
                    layer = Layer.ClearFront;
                    cmbEditorLayer.SelectedIndex = 13;
                    break;
                case Keys.D8:
                    layer = Layer.ClearMidd;
                    cmbEditorLayer.SelectedIndex = 12;
                    break;
                case Keys.D9:
                    layer = Layer.ClearBack;
                    cmbEditorLayer.SelectedIndex = 11;
                    break;
                case Keys.D0:
                    layer = Layer.BrushMir2BigTiles;
                    cmbEditorLayer.SelectedIndex = 17;
                    break;
                case Keys.Oemplus:
                    layer = Layer.BrushMir3BigTiles;
                    cmbEditorLayer.SelectedIndex = 19;
                    break;
                case Keys.OemMinus:
                    layer = Layer.BrushSmTiles;
                    cmbEditorLayer.SelectedIndex = 18;
                    break;


                case Keys.F1:
                    e.SuppressKeyPress = true;
                    tabControl1.SelectedTab = tabExine;
                    break;
                
               
                case Keys.F4: //Added by M2P
                    e.SuppressKeyPress = true;
                    tabControl1.SelectedTab = tabTileCutter;
                    break;
                case Keys.F5:
                    e.SuppressKeyPress = true;
                    tabControl1.SelectedTab = tabMap;
                    break;
                case Keys.F6:
                    e.SuppressKeyPress = true;
                    chkFront.Checked = !chkFront.Checked;
                    break;
                case Keys.F7:
                    e.SuppressKeyPress = true;
                    chkMidd.Checked = !chkMidd.Checked;
                    break;
                case Keys.F8:
                    e.SuppressKeyPress = true;
                    chkBack.Checked = !chkBack.Checked;
                    break;
                case Keys.F9:
                    e.SuppressKeyPress = true;
                    chkShowCellInfo.Checked = !chkShowCellInfo.Checked;
                    cellInfoControl.Visible = chkShowCellInfo.Checked;
                    break;
                case Keys.F10:
                    e.SuppressKeyPress = true; //Stops the F10 key switching focus to the File Menu
                    chkFrontTag.Checked = !chkFrontTag.Checked;
                    break;
                case Keys.F11:
                    e.SuppressKeyPress = true;
                    chkMiddleTag.Checked = !chkMiddleTag.Checked;
                    break;
                case Keys.F12: //Reserved XX - M2P added (Ctrl-F12)F12 shortcut to toggle (View Front Limit)'View Back Limit' via GUI element property panel!
                    e.SuppressKeyPress = true;
                    //chkTopTag.Checked = !chkTopTag.Checked;
                    break;
            }

            if (X2CellInfo != null)
            {
                if (layer == Layer.BackImage)
                {
                    if (e.Control)
                    {
                        keyDown = true;
                    }
                }

                if (layer == Layer.BrushMir2BigTiles || layer == Layer.BrushSmTiles || layer == Layer.BrushMir3BigTiles)
                {
                    if (e.Control)
                    {
                        keyDown = true;
                    }
                }
            }
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.ControlKey)
            {
                keyDown = false;
            }
        }

        private void ObjectslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = ObjectslistBox.SelectedItem + ".X";
            var objectFile = Application.StartupPath + "\\Data\\Objects\\" + name;
            objectDatas = ReadObjectsFile(objectFile);


            picObjects.Image = GetObjectPreview(26, 24, objectDatas);
        }

        private void chkMiddleTag_Click(object sender, EventArgs e)
        {
            chkMiddleTag.Checked = !chkMiddleTag.Checked;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }

        private void chkShowCellInfo_Click(object sender, EventArgs e)
        {
            chkShowCellInfo.Checked = !chkShowCellInfo.Checked;
            cellInfoControl.Visible = chkShowCellInfo.Checked;
        }

        private void ShowCellInfo(bool blShow)
        {
            if (blShow)
            {
                cellInfoControl.SetText(
                    cellX,
                    cellY,
                    (X2CellInfo[cellX, cellY].BackImage & 0x1FFFF) - 1,
                    X2CellInfo[cellX, cellY].MiddleImage - 1,
                    (X2CellInfo[cellX, cellY].FrontImage & 0x7FFF) - 1,
                    X2CellInfo[cellX, cellY].BackIndex,
                    X2CellInfo[cellX, cellY].MiddleIndex,
                    X2CellInfo[cellX, cellY].FrontIndex,
                    GetLibName(X2CellInfo[cellX, cellY].BackIndex),
                    GetLibName(X2CellInfo[cellX, cellY].MiddleIndex),
                    GetLibName(X2CellInfo[cellX, cellY].FrontIndex),
                    X2CellInfo[cellX, cellY].BackImage & 0x20000000,
                    X2CellInfo[cellX, cellY].FrontImage & 0x8000,
                    (byte) (X2CellInfo[cellX, cellY].FrontAnimationFrame & 0x7F),
                    X2CellInfo[cellX, cellY].FrontAnimationTick,
                    Convert.ToBoolean(X2CellInfo[cellX, cellY].FrontAnimationFrame & 0x80),
                    X2CellInfo[cellX, cellY].MiddleAnimationFrame,
                    X2CellInfo[cellX, cellY].MiddleAnimationTick,
                    Convert.ToBoolean(X2CellInfo[cellX, cellY].MiddleAnimationFrame),
                    X2CellInfo[cellX, cellY].DoorOffset,
                    (byte) (X2CellInfo[cellX, cellY].DoorIndex & 0x7F),
                    Convert.ToBoolean(X2CellInfo[cellX, cellY].DoorIndex & 0x80),
                    X2CellInfo[cellX, cellY].Light,
                    X2CellInfo[cellX, cellY].FishingCell
                    );
                drawX = (cellX - mapPoint.X + 1)*(CellWidth*zoomMIN/zoomMAX);
                drawY = (cellY - mapPoint.Y + 1)*(CellHeight*zoomMIN/zoomMAX);
                if (drawX + cellInfoControl.Width >= MapPanel.Width)
                {
                    drawX = drawX - cellInfoControl.Width - 1;
                }
                cellInfoControl.Location = new Point(drawX, drawY);
                if (!MapPanel.Controls.Contains(cellInfoControl))
                {
                    MapPanel.Controls.Add(cellInfoControl);
                }
            }
        }

        private void WemadeMir2LibListView_Click(object sender, EventArgs e)
        {
            selectListItem = wemadeExine2ListItem;
            //Set a breakpoint here and find that this event is actually executed twice after clicking on different Items.
            //The first time is to cancel the current Item selected state, causing the SelectedIndices of the entire ListView to become 0.
            //The newly selected Item is set to the selected state for the second time, and SelectedIndices becomes 1.
            //If you do not add listview.SelectedIndices.Count>0 to determine, it will result in an out-of-bounds exception in obtaining listview.Items[] index.
            if (WemadeMir2LibListView.SelectedIndices.Count > 0)
            {
                selectLibMImage =
                    Libraries.MapLibs[wemadeExine2ListItem.Value].GetMImage(WemadeMir2LibListView.SelectedIndices[0]);
                if (selectLibMImage != null)
                {
                    selectImageIndex = WemadeMir2LibListView.SelectedIndices[0];

                    picWemdeMir2.Image = selectLibMImage.Image;
                    LabWemadeMir2Width.Text = "Width : " + selectLibMImage.Width;
                    LabWemadeMir2Height.Text = "Height : " + selectLibMImage.Height;
                    labeWemadeMir2OffSetX.Text = "OffSetX : " + selectLibMImage.X;
                    labWemadeMir2OffSetY.Text = "OffSetY : " + selectLibMImage.Y;
                    Libraries.MapLibs[wemadeExine2ListItem.Value].Images[WemadeMir2LibListView.SelectedIndices[0]] = null;
                }
            }
        }
         

        private void CreateMir2BigTiles()
        {
            if (selectListItem == null || selectTilesIndex == -1) return;
            bigTilePoints.Clear();
            PutAutoTile(cellX, cellY, RandomAutoMir2Tile(TileType.Center));
            DrawAutoMir2TileSide(cellX, cellY);
            AutoTileRange = 2;

            for (var i = 0; i < 30; ++i)
            {
                AutoTileChanges = 0;
                DrawAutoMir2TilePattern(cellX, cellY);
                if (AutoTileChanges == 0)
                {
                    break;
                }
                AutoTileRange += 2;
            }
        }

        private void CreateMir3BigTiles()
        {
            if (selectListItem == null || selectTilesIndex == -1) return;
            bigTilePoints.Clear();
            PutAutoTile(cellX, cellY, RandomAutoMir3Tile(TileType.Center));
            DrawAutoMir3TileSide(cellX, cellY);
            AutoTileRange = 2;

            for (var i = 0; i < 30; ++i)
            {
                AutoTileChanges = 0;
                DrawAutoMir3TilePattern(cellX, cellY);
                if (AutoTileChanges == 0)
                {
                    break;
                }
                AutoTileRange += 2;
            }
        }

        private void CreateSmTiles()
        {
            if (selectListItem == null || selectTilesIndex == -1) return;
            smTilePoints.Clear();
            PutAutoSmTile(cellX, cellY, RandomAutoSmTile(TileType.Center));
            DrawAutoSmTileSide(cellX, cellY);
            AutoTileRange = 2;
            for (var i = 0; i < 30; ++i)
            {
                AutoTileChanges = 0;
                DrawAutoSmTilePattern(cellX, cellY);
                if (AutoTileChanges == 0)
                {
                    break;
                }
                AutoTileRange += 2;
            }
        }

        private int GetTile(int x, int y)
        {
            if (x < 0 || (y < 0) || (x >= mapWidth) || (y >= mapHeight))
            {
                return -1;
            }
            return (X2CellInfo[x, y].BackImage & 0X1FFFFFFF) - 1;
        }

        private int GetSmTile(int x, int y)
        {
            if (x < 0 || (y < 0) || (x >= mapWidth) || (y >= mapHeight))
            {
                return -1;
            }
            return X2CellInfo[x, y].MiddleImage - 1;
        }

        private TileType GetAutoMir2TileType(int x, int y)
        {
            var imageIndex = GetTile(x, y);
            if (imageIndex/Exine2BigTileBlock != selectTilesIndex)
            {
                return TileType.None;
            }
            switch (imageIndex%Exine2BigTileBlock)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    return TileType.Center;
                case 5:
                    return TileType.UpLeft;
                case 6:
                    return TileType.UpRight;
                case 7:
                    return TileType.DownLeft;
                case 8:
                    return TileType.DownRight;
                case 10:
                    return TileType.InUpLeft;
                case 11:
                    return TileType.InUpRight;
                case 12:
                    return TileType.InDownLeft;
                case 13:
                    return TileType.InDownRight;
                case 15:
                case 16:
                    return TileType.Up;
                case 17:
                case 18:
                    return TileType.Down;
                case 20:
                case 22:
                    return TileType.Left;
                case 21:
                case 23:
                    return TileType.Right;
            }
            return TileType.None;
        }

        private TileType GetAutoMir3TileType(int x, int y)
        {
            var imageIndex = GetTile(x, y);

            int flag;
            if (Libraries.MapLibs[selectListItem.Value].Images.Count%10 != 0)
            {
                flag = (Libraries.MapLibs[selectListItem.Value].Images.Count + 1)/Exine3BigTileBlock;
            }
            else
            {
                flag = Libraries.MapLibs[selectListItem.Value].Images.Count/Exine3BigTileBlock;
            }

            if ((imageIndex/Exine3BigTileBlock != selectTilesIndex) &&
                imageIndex/Exine3BigTileBlock != selectTilesIndex - flag)
            {
                return TileType.None;
            }
            if (selectTilesIndex < flag)
            {
                switch (imageIndex%Exine3BigTileBlock)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return TileType.Center;
                    case 10:
                        return TileType.UpLeft;
                    case 11:
                        return TileType.UpRight;
                    case 12:
                        return TileType.DownLeft;
                    case 13:
                        return TileType.DownRight;
                    case 15:
                        return TileType.InUpLeft;
                    case 16:
                        return TileType.InUpRight;
                    case 17:
                        return TileType.InDownLeft;
                    case 18:
                        return TileType.InDownRight;
                    case 20:
                    case 21:
                        return TileType.Up;
                    case 22:
                    case 23:
                        return TileType.Down;
                    case 25:
                    case 27:
                        return TileType.Left;
                    case 26:
                    case 28:
                        return TileType.Right;
                }
            }
            else
            {
                switch (imageIndex%Exine3BigTileBlock)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        return TileType.Center;
                    case 18:
                        return TileType.UpLeft;
                    case 17:
                        return TileType.UpRight;
                    case 16:
                        return TileType.DownLeft;
                    case 15:
                        return TileType.DownRight;
                    case 13:
                        return TileType.InUpLeft;
                    case 12:
                        return TileType.InUpRight;
                    case 11:
                        return TileType.InDownLeft;
                    case 10:
                        return TileType.InDownRight;
                    case 22:
                    case 23:
                        return TileType.Up;
                    case 20:
                    case 21:
                        return TileType.Down;
                    case 26:
                    case 28:
                        return TileType.Left;
                    case 25:
                    case 27:
                        return TileType.Right;
                }
            }
            return TileType.None;
        }

        private TileType GetAutoSmTileType(int iX, int iY)
        {
            //Get the floor tile type of the grid iX, iY small and medium floor tiles, top, middle, bottom, etc. . .
            int iImageIndex;

            iImageIndex = GetSmTile(iX, iY); //Get image index of small floor tiles
            //Then determine whether this index is the currently selected style
            if (iImageIndex >= selectTilesIndex*smTileBlock && iImageIndex < (selectTilesIndex + 1)*smTileBlock)
            {
                //If so, you can calculate which type of floor tile it is based on the layout of various types of floor tiles in the small floor tile style.
                iImageIndex -= selectTilesIndex*smTileBlock;
                if (iImageIndex < 8)
                {
                    return 0;
                }
                return (TileType) ((iImageIndex - 8)/4 + 1);
            }

            //return -1;	//If it does not belong to the current style, it returns -1
            return TileType.None;
        }

        private void PutAutoTile(int x, int y, int imageIndex)
        {
            if (x < 0 || (y < 0) || (x >= mapWidth) || (y >= mapHeight)) return;

            ++AutoTileChanges;

            for (var i = 0; i < bigTilePoints.Count; i++)
            {
                if (bigTilePoints[i].X == x && bigTilePoints[i].Y == y)
                {
                    X2CellInfo[x, y].BackImage = imageIndex + 1;
                    X2CellInfo[x, y].BackIndex = (short) selectListItem.Value;
                    return;
                }
            }
            bigTilePoints.Add(new CellInfoData(x, y, X2CellInfo[x, y]));
            X2CellInfo[x, y].BackImage = imageIndex + 1;
            X2CellInfo[x, y].BackIndex = (short) selectListItem.Value;
        }

        private void PutAutoSmTile(int x, int y, int imageIndex)
        {
            if (x < 0 || (y < 0) || (x >= mapWidth) || (y >= mapHeight)) return;
            ++AutoTileChanges;

            for (var i = 0; i < smTilePoints.Count; i++)
            {
                if (smTilePoints[i].X == x && smTilePoints[i].Y == y)
                {
                    X2CellInfo[x, y].MiddleImage = (short) (imageIndex + 1);
                    X2CellInfo[x, y].MiddleIndex = (short) selectListItem.Value;
                    return;
                }
            }
            smTilePoints.Add(new CellInfoData(x, y, X2CellInfo[x, y]));
            X2CellInfo[x, y].MiddleImage = (short) (imageIndex + 1);
            X2CellInfo[x, y].MiddleIndex = (short) selectListItem.Value;
        }

        private void DrawAutoMir2TileSide(int iX, int iY)
        {
            if (GetAutoMir2TileType(iX, iY - 2) < 0) //Up
            {
                PutAutoTile(iX, iY - 2, RandomAutoMir2Tile(TileType.Up));
            }
            if (GetAutoMir2TileType(iX + 2, iY - 2) < 0) //UpRight
            {
                PutAutoTile(iX + 2, iY - 2, RandomAutoMir2Tile(TileType.UpRight));
            }
            if (GetAutoMir2TileType(iX + 2, iY) < 0) //right
            {
                PutAutoTile(iX + 2, iY, RandomAutoMir2Tile(TileType.Right));
            }
            if (GetAutoMir2TileType(iX + 2, iY + 2) < 0) //DownRight
            {
                PutAutoTile(iX + 2, iY + 2, RandomAutoMir2Tile(TileType.DownRight));
            }
            if (GetAutoMir2TileType(iX, iY + 2) < 0) //Down
            {
                PutAutoTile(iX, iY + 2, RandomAutoMir2Tile(TileType.Down));
            }
            if (GetAutoMir2TileType(iX - 2, iY + 2) < 0) //DownLeft
            {
                PutAutoTile(iX - 2, iY + 2, RandomAutoMir2Tile(TileType.DownLeft));
            }
            if (GetAutoMir2TileType(iX - 2, iY) < 0) //left
            {
                PutAutoTile(iX - 2, iY, RandomAutoMir2Tile(TileType.Left));
            }
            if (GetAutoMir2TileType(iX - 2, iY - 2) < 0) //UpLeft
            {
                PutAutoTile(iX - 2, iY - 2, RandomAutoMir2Tile(TileType.UpLeft));
            }
        }

        private void DrawAutoMir3TileSide(int iX, int iY)
        {
            if (GetAutoMir3TileType(iX, iY - 2) < 0) //Up
            {
                PutAutoTile(iX, iY - 2, RandomAutoMir3Tile(TileType.Up));
            }
            if (GetAutoMir3TileType(iX + 2, iY - 2) < 0) //UpRight
            {
                PutAutoTile(iX + 2, iY - 2, RandomAutoMir3Tile(TileType.UpRight));
            }
            if (GetAutoMir3TileType(iX + 2, iY) < 0) //Right
            {
                PutAutoTile(iX + 2, iY, RandomAutoMir3Tile(TileType.Right));
            }
            if (GetAutoMir3TileType(iX + 2, iY + 2) < 0) //DownRight
            {
                PutAutoTile(iX + 2, iY + 2, RandomAutoMir3Tile(TileType.DownRight));
            }
            if (GetAutoMir3TileType(iX, iY + 2) < 0) //Down
            {
                PutAutoTile(iX, iY + 2, RandomAutoMir3Tile(TileType.Down));
            }
            if (GetAutoMir3TileType(iX - 2, iY + 2) < 0) //DownLeft
            {
                PutAutoTile(iX - 2, iY + 2, RandomAutoMir3Tile(TileType.DownLeft));
            }
            if (GetAutoMir3TileType(iX - 2, iY) < 0) //Left
            {
                PutAutoTile(iX - 2, iY, RandomAutoMir3Tile(TileType.Left));
            }
            if (GetAutoMir3TileType(iX - 2, iY - 2) < 0) //UpLeft
            {
                PutAutoTile(iX - 2, iY - 2, RandomAutoMir3Tile(TileType.UpLeft));
            }
        }

        private void DrawAutoSmTileSide(int iX, int iY)
        {
            //This is to draw an edge
            if (GetAutoSmTileType(iX, iY - 1) < 0) //Draw up, down, left, and right one by one, but before drawing, you must first check whether the grid already has floor tiles of the current style. If so, it will not be drawn.
            {
                PutAutoSmTile(iX, iY - 1, RandomAutoSmTile(TileType.Up)); //Randomly returns a tile on top and then draws
            }
            if (GetAutoSmTileType(iX + 1, iY - 1) < 0) //UpRight
            {
                PutAutoSmTile(iX + 1, iY - 1, RandomAutoSmTile(TileType.UpRight));
            }
            if (GetAutoSmTileType(iX + 1, iY) < 0) //Right
            {
                PutAutoSmTile(iX + 1, iY, RandomAutoSmTile(TileType.Right));
            }
            if (GetAutoSmTileType(iX + 1, iY + 1) < 0) //DownRight
            {
                PutAutoSmTile(iX + 1, iY + 1, RandomAutoSmTile(TileType.DownRight));
            }
            if (GetAutoSmTileType(iX, iY + 1) < 0) //Down
            {
                PutAutoSmTile(iX, iY + 1, RandomAutoSmTile(TileType.Down));
            }
            if (GetAutoSmTileType(iX - 1, iY + 1) < 0) //DownLeft
            {
                PutAutoSmTile(iX - 1, iY + 1, RandomAutoSmTile(TileType.DownLeft));
            }
            if (GetAutoSmTileType(iX - 1, iY) < 0) //Left
            {
                PutAutoSmTile(iX - 1, iY, RandomAutoSmTile(TileType.Left));
            }
            if (GetAutoSmTileType(iX - 1, iY - 1) < 0) //UpLeft
            {
                PutAutoSmTile(iX - 1, iY - 1, RandomAutoSmTile(TileType.UpLeft));
            }
        }

        private void DrawAutoMir2TilePattern(int iX, int iY)
        {
            int i, j, c;
            TileType n1, n2;

            for (j = iY - AutoTileRange; j <= iY + AutoTileRange; j += 2) //The interval is 2
            {
                for (i = iX - AutoTileRange; i <= iX + AutoTileRange; i += 2) //The interval is 2, and other algorithms are the same as those for small floor tiles.
                {
                    if (i > 1 && j > 1)
                    {
                        if (GetAutoMir2TileType(i, j) > 0)
                        {
                            //Check CENTER
                            if (GetAutoMir2TileType(i, j) != TileType.Center)
                            {
                                c = 0;
                                if (GetAutoMir2TileType(i, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i + 2, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i + 2, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i + 2, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i - 2, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i - 2, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir2TileType(i - 2, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (c >= 8)
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Center));
                                }
                            }

                            //Check UP
                            if (GetAutoMir2TileType(i, j) != TileType.Up)
                            {
                                n1 = GetAutoMir2TileType(i - 2, j);
                                n2 = GetAutoMir2TileType(i + 2, j);
                                if ((n1 == TileType.Up || n1 == TileType.UpLeft || n1 == TileType.InDownLeft) &&
                                    (n2 == TileType.Up || n2 == TileType.UpRight || n2 == TileType.InDownRight) &&
                                    GetAutoMir2TileType(i, j - 2) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Up));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.UpRight &&
                                    ((GetAutoMir2TileType(i, j + 2) == TileType.Center &&
                                      GetAutoMir2TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i + 2, j) == TileType.UpLeft ||
                                     GetAutoMir2TileType(i + 2, j) == TileType.Left))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Up));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoMir2TileType(i, j + 2) == TileType.Center &&
                                      GetAutoMir2TileType(i - 2, j + 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i - 2, j) == TileType.UpRight ||
                                     GetAutoMir2TileType(i - 2, j) == TileType.Right))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Up));
                                }
                            }

                            //Check RIGHT
                            if (GetAutoMir2TileType(i, j) != TileType.Right)
                            {
                                n1 = GetAutoMir2TileType(i, j - 2);
                                n2 = GetAutoMir2TileType(i, j + 2);
                                if ((n1 == TileType.Right || n1 == TileType.UpRight || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Right || n2 == TileType.DownRight || n2 == TileType.InDownLeft) &&
                                    GetAutoMir2TileType(i + 2, j) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Right));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.DownRight &&
                                    ((GetAutoMir2TileType(i - 2, j) == TileType.Center &&
                                      GetAutoMir2TileType(i - 2, j + 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i, j + 2) == TileType.UpRight ||
                                     GetAutoMir2TileType(i, j + 2) == TileType.Up))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Right));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.UpRight &&
                                    ((GetAutoMir2TileType(i - 2, j) == TileType.Center &&
                                      GetAutoMir2TileType(i - 2, j - 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i, j - 2) == TileType.DownRight ||
                                     GetAutoMir2TileType(i, j - 2) == TileType.Down))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Right));
                                }
                            }

                            //Check DOWN
                            if (GetAutoMir2TileType(i, j) != TileType.Down)
                            {
                                n1 = GetAutoMir2TileType(i - 2, j);
                                n2 = GetAutoMir2TileType(i + 2, j);
                                if ((n1 == TileType.Down || n1 == TileType.DownLeft || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Down || n2 == TileType.DownRight || n2 == TileType.InUpRight) &&
                                    GetAutoMir2TileType(i, j + 2) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Down));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.DownRight &&
                                    ((GetAutoMir2TileType(i, j - 2) == TileType.Center &&
                                      GetAutoMir2TileType(i + 2, j - 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i + 2, j) == TileType.DownLeft ||
                                     GetAutoMir2TileType(i + 2, j) == TileType.Left))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Down));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoMir2TileType(i - 2, j - 2) == TileType.Center &&
                                      GetAutoMir2TileType(i, j - 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i - 2, j) == TileType.DownRight ||
                                     GetAutoMir2TileType(i - 2, j) == TileType.Right))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Down));
                                }
                            }

                            //Check LEFT
                            if (GetAutoMir2TileType(i, j) != TileType.Left)
                            {
                                n1 = GetAutoMir2TileType(i, j - 2);
                                n2 = GetAutoMir2TileType(i, j + 2);
                                if ((n1 == TileType.Left || n1 == TileType.UpLeft || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.Left || n2 == TileType.DownLeft || n2 == TileType.InDownRight) &&
                                    GetAutoMir2TileType(i - 2, j) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Left));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoMir2TileType(i + 2, j) == TileType.Center &&
                                      GetAutoMir2TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i, j + 2) == TileType.UpLeft ||
                                     GetAutoMir2TileType(i, j + 2) == TileType.Up))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Left));
                                }
                                if (GetAutoMir2TileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoMir2TileType(i + 2, j) == TileType.Center &&
                                      GetAutoMir2TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir2TileType(i, j - 2) == TileType.DownLeft ||
                                     GetAutoMir2TileType(i, j - 2) == TileType.Down))
                                {
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Left));
                                }
                            }

                            //Check INUPRIGHT
                            n1 = GetAutoMir2TileType(i - 2, j);
                            n2 = GetAutoMir2TileType(i, j + 2);
                            if (GetAutoMir2TileType(i, j) != TileType.InDownLeft)
                            {
                                if ((n1 == TileType.DownLeft || n1 == TileType.Down || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.DownLeft || n2 == TileType.Left || n2 == TileType.InDownRight))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InUpRight));
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InDownLeft));
                                }
                            }

                            //Check INDOWNRIGHT
                            n1 = GetAutoMir2TileType(i, j - 2);
                            n2 = GetAutoMir2TileType(i - 2, j);
                            if (GetAutoMir2TileType(i, j) != TileType.InUpLeft)
                            {
                                if ((n1 == TileType.UpLeft || n1 == TileType.Left || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.UpLeft || n2 == TileType.Up || n2 == TileType.InDownLeft))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InDownRight));
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InUpLeft));
                                }
                            }

                            //Check INDOWNLEFT
                            n1 = GetAutoMir2TileType(i, j - 2);
                            n2 = GetAutoMir2TileType(i + 2, j);
                            if (GetAutoMir2TileType(i, j) != TileType.InUpRight)
                            {
                                if ((n1 == TileType.UpRight || n1 == TileType.Right || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.UpRight || n2 == TileType.Up || n2 == TileType.InDownRight))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InDownLeft));
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InUpRight));
                                }
                            }

                            //Check INUPLEFT
                            n1 = GetAutoMir2TileType(i + 2, j);
                            n2 = GetAutoMir2TileType(i, j + 2);
                            if (GetAutoMir2TileType(i, j) != TileType.InDownRight)
                            {
                                if ((n1 == TileType.DownRight || n1 == TileType.Down || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.DownRight || n2 == TileType.Right || n2 == TileType.InDownLeft))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InUpLeft));
                                    PutAutoTile(i, j, RandomAutoMir2Tile(TileType.InDownRight));
                                }
                            }

                            //Check Paradox
                            if ((GetAutoMir2TileType(i - 2, j) == TileType.Down &&
                                 GetAutoMir2TileType(i, j - 2) == TileType.Right &&
                                 GetAutoMir2TileType(i + 2, j) == TileType.Up &&
                                 GetAutoMir2TileType(i, j + 2) == TileType.Left) ||
                                (GetAutoMir2TileType(i - 2, j) == TileType.Up &&
                                 GetAutoMir2TileType(i, j + 2) == TileType.Right &&
                                 GetAutoMir2TileType(i, j - 2) == TileType.Left &&
                                 GetAutoMir2TileType(i + 2, j) == TileType.Down))
                            {
                                PutAutoTile(i, j, RandomAutoMir2Tile(TileType.Center));
                                DrawAutoMir2TileSide(i, j);
                            }
                        }
                    }
                }
            }
        }

        private void DrawAutoMir3TilePattern(int iX, int iY)
        {
            int i, j, c;
            TileType n1, n2;

            for (j = iY - AutoTileRange; j <= iY + AutoTileRange; j += 2) //间隔为2
            {
                for (i = iX - AutoTileRange; i <= iX + AutoTileRange; i += 2) //间隔为2, 其他算法跟小地砖一样
                {
                    if (i > 1 && j > 1)
                    {
                        if (GetAutoMir3TileType(i, j) > 0)
                        {
                            //Check CENTER
                            if (GetAutoMir3TileType(i, j) != TileType.Center)
                            {
                                c = 0;
                                if (GetAutoMir3TileType(i, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i + 2, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i + 2, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i + 2, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i - 2, j + 2) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i - 2, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoMir3TileType(i - 2, j - 2) >= 0)
                                {
                                    ++c;
                                }
                                if (c >= 8)
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Center));
                                }
                            }

                            //Check UP
                            if (GetAutoMir3TileType(i, j) != TileType.Up)
                            {
                                n1 = GetAutoMir3TileType(i - 2, j);
                                n2 = GetAutoMir3TileType(i + 2, j);
                                if ((n1 == TileType.Up || n1 == TileType.UpLeft || n1 == TileType.InDownLeft) &&
                                    (n2 == TileType.Up || n2 == TileType.UpRight || n2 == TileType.InDownRight) &&
                                    GetAutoMir3TileType(i, j - 2) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Up));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.UpRight &&
                                    ((GetAutoMir3TileType(i, j + 2) == TileType.Center &&
                                      GetAutoMir3TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i + 2, j) == TileType.UpLeft ||
                                     GetAutoMir3TileType(i + 2, j) == TileType.Left))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Up));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoMir3TileType(i, j + 2) == TileType.Center &&
                                      GetAutoMir3TileType(i - 2, j + 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i - 2, j) == TileType.UpRight ||
                                     GetAutoMir3TileType(i - 2, j) == TileType.Right))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Up));
                                }
                            }

                            //Check RIGHT
                            if (GetAutoMir3TileType(i, j) != TileType.Right)
                            {
                                n1 = GetAutoMir3TileType(i, j - 2);
                                n2 = GetAutoMir3TileType(i, j + 2);
                                if ((n1 == TileType.Right || n1 == TileType.UpRight || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Right || n2 == TileType.DownRight || n2 == TileType.InDownLeft) &&
                                    GetAutoMir3TileType(i + 2, j) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Right));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.DownRight &&
                                    ((GetAutoMir3TileType(i - 2, j) == TileType.Center &&
                                      GetAutoMir3TileType(i - 2, j + 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i, j + 2) == TileType.UpRight ||
                                     GetAutoMir3TileType(i, j + 2) == TileType.Up))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Right));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.UpRight &&
                                    ((GetAutoMir3TileType(i - 2, j) == TileType.Center &&
                                      GetAutoMir3TileType(i - 2, j - 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i, j - 2) == TileType.DownRight ||
                                     GetAutoMir3TileType(i, j - 2) == TileType.Down))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Right));
                                }
                            }

                            //Check DOWN
                            if (GetAutoMir3TileType(i, j) != TileType.Down)
                            {
                                n1 = GetAutoMir3TileType(i - 2, j);
                                n2 = GetAutoMir3TileType(i + 2, j);
                                if ((n1 == TileType.Down || n1 == TileType.DownLeft || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Down || n2 == TileType.DownRight || n2 == TileType.InUpRight) &&
                                    GetAutoMir3TileType(i, j + 2) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Down));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.DownRight &&
                                    ((GetAutoMir3TileType(i, j - 2) == TileType.Center &&
                                      GetAutoMir3TileType(i + 2, j - 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i + 2, j) == TileType.DownLeft ||
                                     GetAutoMir3TileType(i + 2, j) == TileType.Left))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Down));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoMir3TileType(i - 2, j - 2) == TileType.Center &&
                                      GetAutoMir3TileType(i, j - 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i - 2, j) == TileType.DownRight ||
                                     GetAutoMir3TileType(i - 2, j) == TileType.Right))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Down));
                                }
                            }

                            //Check LEFT
                            if (GetAutoMir3TileType(i, j) != TileType.Left)
                            {
                                n1 = GetAutoMir3TileType(i, j - 2);
                                n2 = GetAutoMir3TileType(i, j + 2);
                                if ((n1 == TileType.Left || n1 == TileType.UpLeft || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.Left || n2 == TileType.DownLeft || n2 == TileType.InDownRight) &&
                                    GetAutoMir3TileType(i - 2, j) < 0)
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Left));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoMir3TileType(i + 2, j) == TileType.Center &&
                                      GetAutoMir3TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i, j + 2) == TileType.UpLeft ||
                                     GetAutoMir3TileType(i, j + 2) == TileType.Up))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Left));
                                }
                                if (GetAutoMir3TileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoMir3TileType(i + 2, j) == TileType.Center &&
                                      GetAutoMir3TileType(i + 2, j + 2) == TileType.Center) ||
                                     GetAutoMir3TileType(i, j - 2) == TileType.DownLeft ||
                                     GetAutoMir3TileType(i, j - 2) == TileType.Down))
                                {
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Left));
                                }
                            }

                            //Check INUPRIGHT
                            n1 = GetAutoMir3TileType(i - 2, j);
                            n2 = GetAutoMir3TileType(i, j + 2);
                            if (GetAutoMir3TileType(i, j) != TileType.InDownLeft)
                            {
                                if ((n1 == TileType.DownLeft || n1 == TileType.Down || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.DownLeft || n2 == TileType.Left || n2 == TileType.InDownRight))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InUpRight));
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InDownLeft));
                                }
                            }

                            //Check INDOWNRIGHT
                            n1 = GetAutoMir3TileType(i, j - 2);
                            n2 = GetAutoMir3TileType(i - 2, j);
                            if (GetAutoMir3TileType(i, j) != TileType.InUpLeft)
                            {
                                if ((n1 == TileType.UpLeft || n1 == TileType.Left || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.UpLeft || n2 == TileType.Up || n2 == TileType.InDownLeft))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InDownRight));
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InUpLeft));
                                }
                            }

                            //Check INDOWNLEFT
                            n1 = GetAutoMir3TileType(i, j - 2);
                            n2 = GetAutoMir3TileType(i + 2, j);
                            if (GetAutoMir3TileType(i, j) != TileType.InUpRight)
                            {
                                if ((n1 == TileType.UpRight || n1 == TileType.Right || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.UpRight || n2 == TileType.Up || n2 == TileType.InDownRight))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InDownLeft));
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InUpRight));
                                }
                            }

                            //Check INUPLEFT
                            n1 = GetAutoMir3TileType(i + 2, j);
                            n2 = GetAutoMir3TileType(i, j + 2);
                            if (GetAutoMir3TileType(i, j) != TileType.InDownRight)
                            {
                                if ((n1 == TileType.DownRight || n1 == TileType.Down || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.DownRight || n2 == TileType.Right || n2 == TileType.InDownLeft))
                                {
                                    //PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InUpLeft));
                                    PutAutoTile(i, j, RandomAutoMir3Tile(TileType.InDownRight));
                                }
                            }

                            //Check Paradox
                            if ((GetAutoMir3TileType(i - 2, j) == TileType.Down &&
                                 GetAutoMir3TileType(i, j - 2) == TileType.Right &&
                                 GetAutoMir3TileType(i + 2, j) == TileType.Up &&
                                 GetAutoMir3TileType(i, j + 2) == TileType.Left) ||
                                (GetAutoMir3TileType(i - 2, j) == TileType.Up &&
                                 GetAutoMir3TileType(i, j + 2) == TileType.Right &&
                                 GetAutoMir3TileType(i, j - 2) == TileType.Left &&
                                 GetAutoMir3TileType(i + 2, j) == TileType.Down))
                            {
                                PutAutoTile(i, j, RandomAutoMir3Tile(TileType.Center));
                                DrawAutoMir3TileSide(i, j);
                            }
                        }
                    }
                }
            }
        }

        private void DrawAutoSmTilePattern(int iX, int iY)
        {
            //This algorithm is more complicated. .
            //It automatically draws by checking the types of surrounding floor tiles, and dynamically adjusts and adds the floor tiles that need to be drawn based on the already drawn floor tiles,
            //thereby achieving the purpose of automatic drawing.
            int i, j, c;
            TileType n1, n2;
            for (j = iY - AutoTileRange; j <= iY + AutoTileRange; ++j)
            {
                for (i = iX - AutoTileRange; i <= iX + AutoTileRange; ++i)
                //Start checking and adjusting based on the cells in the m_iAutoTileRange range surrounding the cell currently pointed by the mouse.
                {
                    if (i > 0 && j > 0) //First, make sure the checked grid is legal.
                    {
                        if (GetAutoSmTileType(i, j) > 0) //Then get the small floor tile type of the grid, whether it is the grid of the current style, if so, you need to check and adjust
                        {
                            //Check CENTER
                            if (GetAutoSmTileType(i, j) != TileType.Center) //First check whether you need to adjust to an intermediate type of grid
                            {
                                c = 0;
                                if (GetAutoSmTileType(i, j - 1) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i + 1, j - 1) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i + 1, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i + 1, j + 1) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i, j + 1) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i - 1, j + 1) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i - 1, j) >= 0)
                                {
                                    ++c;
                                }
                                if (GetAutoSmTileType(i - 1, j - 1) >= 0)
                                {
                                    ++c;
                                }
                                if (c >= 8) //As long as the grid is surrounded by 8 grids, it will be adjusted to an intermediate type of grid.
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Center));
                                }
                            }

                            //Check UP
                            if (GetAutoSmTileType(i, j) != TileType.Up) //Then check whether it needs to be adjusted to the above type of grid. 
                            {
                                //There are three situations in which the following algorithm needs to be adjusted to the upper grid. . . You can take a look for yourself. .
                                n1 = GetAutoSmTileType(i - 1, j);
                                n2 = GetAutoSmTileType(i + 1, j);
                                if ((n1 == TileType.Up || n1 == TileType.UpLeft || n1 == TileType.InDownLeft) &&
                                    (n2 == TileType.Up || n2 == TileType.UpRight || n2 == TileType.InDownRight) &&
                                    GetAutoSmTileType(i, j - 1) < 0)
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Up));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.UpRight &&
                                    ((GetAutoSmTileType(i, j + 1) == TileType.Center &&
                                      GetAutoSmTileType(i + 1, j + 1) == TileType.Center) ||
                                     GetAutoSmTileType(i + 1, j) == TileType.UpLeft ||
                                     GetAutoSmTileType(i + 1, j) == TileType.Left))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Up));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoSmTileType(i, j + 1) == TileType.Center &&
                                      GetAutoSmTileType(i - 1, j + 1) == TileType.Center) ||
                                     GetAutoSmTileType(i - 1, j) == TileType.UpRight ||
                                     GetAutoSmTileType(i - 1, j) == TileType.Right))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Up));
                                }
                            }

                            //Check RIGHT  //Then check whether it needs to be adjusted to the right type of grid. 
                            if (GetAutoSmTileType(i, j) != TileType.Right)
                            {
                                n1 = GetAutoSmTileType(i, j - 1);
                                n2 = GetAutoSmTileType(i, j + 1);
                                if ((n1 == TileType.Right || n1 == TileType.UpRight || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Right || n2 == TileType.DownRight || n2 == TileType.InDownLeft) &&
                                    GetAutoSmTileType(i + 1, j) < 0)
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Right));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.DownRight &&
                                    ((GetAutoSmTileType(i - 1, j) == TileType.Center &&
                                      GetAutoSmTileType(i - 1, j + 1) == TileType.Center) ||
                                     GetAutoSmTileType(i, j + 1) == TileType.UpRight ||
                                     GetAutoSmTileType(i, j + 1) == TileType.Up))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Right));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.UpRight &&
                                    ((GetAutoSmTileType(i - 1, j) == TileType.Center &&
                                      GetAutoSmTileType(i - 1, j - 1) == TileType.Center) ||
                                     GetAutoSmTileType(i, j - 1) == TileType.DownRight ||
                                     GetAutoSmTileType(i, j - 1) == TileType.Down))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Right));
                                }
                            }

                            //Check DOWN //Then check whether you need to adjust it to the following type of grid. .
                            if (GetAutoSmTileType(i, j) != TileType.Down)
                            {
                                n1 = GetAutoSmTileType(i - 1, j);
                                n2 = GetAutoSmTileType(i + 1, j);
                                if ((n1 == TileType.Down || n1 == TileType.DownLeft || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.Down || n2 == TileType.DownRight || n2 == TileType.InUpRight) &&
                                    GetAutoSmTileType(i, j + 1) < 0)
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Down));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.DownRight &&
                                    ((GetAutoSmTileType(i, j - 1) == TileType.Center &&
                                      GetAutoSmTileType(i + 1, j - 1) == TileType.Center) ||
                                     GetAutoSmTileType(i + 1, j) == TileType.DownLeft ||
                                     GetAutoSmTileType(i + 1, j) == TileType.Left))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Down));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoSmTileType(i - 1, j - 1) == TileType.Center &&
                                      GetAutoSmTileType(i, j - 1) == TileType.Center) ||
                                     GetAutoSmTileType(i - 1, j) == TileType.DownRight ||
                                     GetAutoSmTileType(i - 1, j) == TileType.Right))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Down));
                                }
                            }

                            //Check LEFT  //Then check whether it needs to be adjusted to the left type of grid. 
                            if (GetAutoSmTileType(i, j) != TileType.Left)
                            {
                                n1 = GetAutoSmTileType(i, j - 1);
                                n2 = GetAutoSmTileType(i, j + 1);
                                if ((n1 == TileType.Left || n1 == TileType.UpLeft || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.Left || n2 == TileType.DownLeft || n2 == TileType.InDownRight) &&
                                    GetAutoSmTileType(i - 1, j) < 0)
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Left));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.DownLeft &&
                                    ((GetAutoSmTileType(i + 1, j) == TileType.Center &&
                                      GetAutoSmTileType(i + 1, j + 1) == TileType.Center) ||
                                     GetAutoSmTileType(i, j + 1) == TileType.UpLeft ||
                                     GetAutoSmTileType(i, j + 1) == TileType.Up))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Left));
                                }
                                if (GetAutoSmTileType(i, j) == TileType.UpLeft &&
                                    ((GetAutoSmTileType(i + 1, j) == TileType.Center &&
                                      GetAutoSmTileType(i + 1, j + 1) == TileType.Center) ||
                                     GetAutoSmTileType(i, j - 1) == TileType.DownLeft ||
                                     GetAutoSmTileType(i, j - 1) == TileType.Down))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Left));
                                }
                            }

                            //Check INUPRIGHT  //Then check whether you need to adjust it to the inner upper right type of grid. .
                            if (GetAutoSmTileType(i, j) != TileType.InUpRight)
                            {
                                n1 = GetAutoSmTileType(i - 1, j);
                                n2 = GetAutoSmTileType(i, j + 1);
                                if ((n1 == TileType.DownLeft || n1 == TileType.Down || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.DownLeft || n2 == TileType.Left || n2 == TileType.InDownRight))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.InUpRight));
                                }
                            }

                            //Check INDOWNRIGHT //Then check whether you need to adjust it to the inner and lower right type of grid. .
                            if (GetAutoSmTileType(i, j) != TileType.InDownRight)
                            {
                                n1 = GetAutoSmTileType(i, j - 1);
                                n2 = GetAutoSmTileType(i - 1, j);
                                if ((n1 == TileType.UpLeft || n1 == TileType.Left || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.UpLeft || n2 == TileType.Up || n2 == TileType.InDownLeft))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.InDownRight));
                                }
                            }

                            //Check INDOWNLEFT  //Then check whether it needs to be adjusted to the inner lower left type of grid. .
                            if (GetAutoSmTileType(i, j) != TileType.InDownLeft)
                            {
                                n1 = GetAutoSmTileType(i, j - 1);
                                n2 = GetAutoSmTileType(i + 1, j);
                                if ((n1 == TileType.UpRight || n1 == TileType.Right || n1 == TileType.InUpLeft) &&
                                    (n2 == TileType.UpRight || n2 == TileType.Up || n2 == TileType.InDownRight))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.InDownLeft));
                                }
                            }

                            //Check INUPLEFT //Then check whether it needs to be adjusted to the inner upper left type of grid. 
                            if (GetAutoSmTileType(i, j) != TileType.InUpLeft)
                            {
                                n1 = GetAutoSmTileType(i + 1, j);
                                n2 = GetAutoSmTileType(i, j + 1);
                                if ((n1 == TileType.DownRight || n1 == TileType.Down || n1 == TileType.InUpRight) &&
                                    (n2 == TileType.DownRight || n2 == TileType.Right || n2 == TileType.InDownLeft))
                                {
                                    PutAutoSmTile(i, j, RandomAutoSmTile(TileType.InUpLeft));
                                }
                            }

                            //There is no need to check the four outer corners. . .

                            //Check Paradox //After the final inspection, see if there is any contradiction.
                            //If so, you may need to add floor tiles to reconcile the contradiction, so draw a pattern around the contradiction.
                            if ((GetAutoSmTileType(i - 1, j) == TileType.Down &&
                                 GetAutoSmTileType(i, j - 1) == TileType.Right &&
                                 GetAutoSmTileType(i + 1, j) == TileType.Up &&
                                 GetAutoSmTileType(i, j + 1) == TileType.Left) ||
                                (GetAutoSmTileType(i - 1, j) == TileType.Up &&
                                 GetAutoSmTileType(i, j + 1) == TileType.Right &&
                                 GetAutoSmTileType(i, j - 1) == TileType.Left &&
                                 GetAutoSmTileType(i + 1, j) == TileType.Down))
                            {
                                PutAutoSmTile(i, j, RandomAutoSmTile(TileType.Center));
                                DrawAutoSmTileSide(i, j);
                            }
                        }
                    }
                }
            } //After the final inspection, if it is found that no floor tiles need to be changed or adjusted, it means that the adjustment is completed. .
              //The algorithm is relatively complex.
              //You can slowly read the code to understand it.
              //There is more than one algorithm for adjusting floor tiles.
              //If you are interested, you can think of some of your own algorithms to draw automated styles.
        }
        
        private Bitmap GetTilesPreview(ListItem selectListView, int index)
        {
            var preview = new Bitmap(6*CellWidth, 6*CellHeight);
            var graphics = Graphics.FromImage(preview);
            graphics.InterpolationMode = InterpolationMode.Low;
            switch (selectListItem.Version)
            {
                case (byte) ExineVerSion.Exine:
                    if ((selectListView.Text.IndexOf("SmTiles", StringComparison.Ordinal) > -1) ||
                        (selectListView.Text.IndexOf("Smtiles", StringComparison.Ordinal) > -1))
                    {
                        var i = 0;
                        preview = new Bitmap(3*CellWidth, 3*CellHeight);
                        graphics = Graphics.FromImage(preview);
                        graphics.InterpolationMode = InterpolationMode.Low;
                        for (var y = 0; y < 3; y++)
                        {
                            drawY = y*CellHeight;
                            for (var x = 0; x < 3; x++)
                            {
                                drawX = x*CellWidth;
                                if (index*smTileBlock + smTilesPreviewIndex[i] >=
                                    Libraries.MapLibs[selectListView.Value].Images.Count)
                                {
                                    continue;
                                }
                                Libraries.MapLibs[selectListView.Value].CheckImage(index*smTileBlock +
                                                                                   smTilesPreviewIndex[i]);
                                var mi =
                                    Libraries.MapLibs[selectListView.Value].Images[
                                        index*smTileBlock + smTilesPreviewIndex[i]];
                                if (mi.Image == null)
                                {
                                    continue;
                                }
                                var destRect = new Rectangle(drawX, drawY, mi.Width, mi.Height);
                                var srcRect = new Rectangle(0, 0, mi.Width, mi.Height);
                                graphics.DrawImage(mi.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                Libraries.MapLibs[libIndex].Images[index] = null;
                                i++;
                            }
                        }
                    }
                    else if (selectListView.Text.IndexOf("Tiles", StringComparison.Ordinal) > -1)
                    {
                        var i = 0;
                        preview = new Bitmap(6*CellWidth, 6*CellHeight);
                        graphics = Graphics.FromImage(preview);
                        graphics.InterpolationMode = InterpolationMode.Low;
                        for (var y = 0; y < 3; y++)
                        {
                            drawY = y*2*CellHeight;
                            for (var x = 0; x < 3; x++)
                            {
                                drawX = x*2*CellWidth;
                                if (index*Exine2BigTileBlock + Mir2BigTilesPreviewIndex[i] >=
                                    Libraries.MapLibs[selectListView.Value].Images.Count)
                                {
                                    continue;
                                }
                                Libraries.MapLibs[selectListView.Value].CheckImage(index*Exine2BigTileBlock +
                                                                                   Mir2BigTilesPreviewIndex[i]);
                                var mi =
                                    Libraries.MapLibs[selectListView.Value].Images[
                                        index*Exine2BigTileBlock + Mir2BigTilesPreviewIndex[i]];
                                if (mi.Image == null)
                                {
                                    continue;
                                }
                                var destRect = new Rectangle(drawX, drawY, mi.Width, mi.Height);
                                var srcRect = new Rectangle(0, 0, mi.Width, mi.Height);
                                graphics.DrawImage(mi.Image, destRect, srcRect, GraphicsUnit.Pixel);
                                Libraries.MapLibs[libIndex].Images[index] = null;
                                i++;
                            }
                        }
                    }
                    break;
                
            }
            return preview;
        }

        private void TileslistView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int index;

            if (_tilesIndexList.TryGetValue(e.ItemIndex, out index))
            {
                e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
                return;
            }

            _tilesIndexList.Add(e.ItemIndex, TilesImageList.Images.Count);

            TilesImageList.Images.Add(GetTilesPreview(selectListItem, e.ItemIndex));
            e.Item = new ListViewItem {ImageIndex = index, Text = e.ItemIndex.ToString()};
            Libraries.MapLibs[selectListItem.Value].Images[e.ItemIndex] = null;
        }

        private void TileslistView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TileslistView.SelectedIndices.Count > 0)
            {
                picTile.Image = GetTilesPreview(selectListItem, TileslistView.SelectedIndices[0]);
                selectTilesIndex = TileslistView.SelectedIndices[0];
            }
        }

        private int RandomAutoMir2Tile(TileType tileType)
        {
            if (selectTilesIndex < 0) return -1;

            switch (tileType)
            {
                case TileType.Center:
                    return selectTilesIndex*Exine2BigTileBlock + random.Next(5);
                case TileType.Up:
                    return selectTilesIndex*Exine2BigTileBlock + random.Next(15, 17);
                case TileType.Down:
                    return selectTilesIndex*Exine2BigTileBlock + random.Next(17, 19);
                case TileType.Left:
                    if (random.Next(2) == 0)
                    {
                        return selectTilesIndex*Exine2BigTileBlock + 20;
                    }
                    return selectTilesIndex*Exine2BigTileBlock + 22;
                case TileType.Right:
                    if (random.Next(2) == 0)
                    {
                        return selectTilesIndex*Exine2BigTileBlock + 21;
                    }
                    return selectTilesIndex*Exine2BigTileBlock + 23;
                case TileType.UpLeft:
                    return selectTilesIndex*Exine2BigTileBlock + 5;

                case TileType.UpRight:
                    return selectTilesIndex*Exine2BigTileBlock + 6;

                case TileType.DownLeft:
                    return selectTilesIndex*Exine2BigTileBlock + 7;
                case TileType.DownRight:
                    return selectTilesIndex*Exine2BigTileBlock + 8;

                case TileType.InUpLeft:
                    return selectTilesIndex*Exine2BigTileBlock + 10;
                case TileType.InUpRight:
                    return selectTilesIndex*Exine2BigTileBlock + 11;
                case TileType.InDownLeft:
                    return selectTilesIndex*Exine2BigTileBlock + 12;
                case TileType.InDownRight:
                    return selectTilesIndex*Exine2BigTileBlock + 13;
            }
            return -1;
        }

        private void menuClearMap_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are You sure you want to clear map tiles?", "Clear Map", MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)
            {
                if (X2CellInfo != null)
                {
                    for (var x = 0; x < mapWidth; x++)
                    {
                        for (var y = 0; y < mapHeight; y++)
                        {
                            X2CellInfo[x, y] = new CellInfo();
                        }
                    }
                }
            }
        }

        private void setScrollBar()
        {
            OffSetX = MapPanel.Width / (CellWidth * zoomMIN / zoomMAX);
            OffSetY = MapPanel.Height / (CellHeight * zoomMIN / zoomMAX);

            if (mapWidth - OffSetX >= 0) hScrollBar.Maximum = mapWidth - OffSetX;
            else hScrollBar.Maximum = 0;
            if (mapHeight - OffSetY >= 0) vScrollBar.Maximum = mapHeight - OffSetY;
            else vScrollBar.Maximum = 0;

            if (mapPoint.X >= 0)
            {
                if (mapPoint.X <= mapWidth) hScrollBar.Value = mapPoint.X;
                else hScrollBar.Value = mapWidth - 1;
            }
            else hScrollBar.Value = 0;

            if (mapPoint.Y >= 0)
            {
                if (mapPoint.Y <= mapHeight) vScrollBar.Value = mapPoint.Y;
                else vScrollBar.Value = mapHeight - 1;
            }
            else vScrollBar.Value = 0;
        }

        private void menuNew_Click(object sender, EventArgs e)
        {
            NewMap();
        }

        private void NewMap()
        {
            var frm = new NewFileFrm(SetMapSize);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                X2CellInfo = new CellInfo[mapWidth, mapHeight];
                for (var x = 0; x < mapWidth; x++)
                {
                    for (var y = 0; y < mapHeight; y++)
                    {
                        X2CellInfo[x, y] = new CellInfo();
                    }
                }
                mapPoint = new Point(0, 0);
                setScrollBar();
            }
            
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            OpenMap();
        }

        private void OpenMap()
        {
            ClearImage();
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog.FileName;
                map = new MapReader(filePath);
                X2CellInfo = map.MapCells;
                mapPoint = new Point(0, 0);
                SetMapSize(map.Width, map.Height);
                mapFileName = openFileDialog.FileName;
                setScrollBar();
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            UnDo();
        }

        private void menuRedo_Click(object sender, EventArgs e)
        {
            ReDo();
        }

        private void menuZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void ZoomIn()
        {
            if (map != null)
            {
                EnlargeZoom();
                SetMapSize(mapWidth, mapHeight);
            }

        }

        private void menuZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void ZoomOut()
        {
            if (map != null)
            {
                NarrowZoom();
                SetMapSize(mapWidth, mapHeight);
            }
        }


        //#region Tool panel buttons
        ////Buttons for the Tool panel
        //private void btn_Move_Click(object sender, EventArgs e)
        //{
        //    layer = Layer.None;
        //}

        //private void btn_Select_Click(object sender, EventArgs e)
        //{
        //    layer = Layer.GraspingMir2Front;
        //}



        //#endregion

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Jump(hScrollBar.Value, mapPoint.Y);
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Jump(mapPoint.X , vScrollBar.Value);
        }

        private void menu_DeleteSelectedCellData_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                DialogResult dr = MessageBox.Show("Are You sure you want to delete selected Cell Data?", "Delete", MessageBoxButtons.OKCancel);

                if (dr == DialogResult.OK)
                {
                    //this should swap the points if point 2 is not a higher value
                    if (p1.X > p2.X)
                    {
                        p1.X += p2.X;
                        p2.X = p1.X - p2.X - 1;
                        p1.X -= p2.X;
                    }

                    if (p1.Y > p2.Y)
                    {
                        p1.Y += p2.Y;
                        p2.Y = p1.Y - p2.Y - 1;
                        p1.Y -= p2.Y;
                    }

                    for (var x = p1.X; x <= p2.X; x++)
                    {
                        for (var y = p1.Y; y <= p2.Y; y++)
                        {
                            X2CellInfo[x, y] = new CellInfo();
                        }
                    }
                }
            }
        }

        private void menu_SaveObject_Click(object sender, EventArgs e)
        {
            if (X2CellInfo == null) return;

            //this should swap the points if point 2 is not a higher value
            if (p1.X > p2.X)
            {
                p1.X += p2.X;
                p2.X = p1.X - p2.X - 1;
                p1.X -= p2.X;
            }

            if (p1.Y > p2.Y)
            {
                p1.Y += p2.Y;
                p2.Y = p1.Y - p2.Y - 1;
                p1.Y -= p2.Y;
            }

            SaveObjectsFile();
            ObjectslistBox.Items.Clear();
            ReadObjectsToListBox();
        }

        private void menuFreeMemory_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void menuJump_Click(object sender, EventArgs e)
        {
            if (X2CellInfo != null)
            {
                Form jump = new FrmJump(Jump);
                if (jump.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }

        /*
        private void menuInvertMir3Layer_Click(object sender, EventArgs e)
        {
            InvertMir3Layer();
        }
        */

        private void menuAbout_Click(object sender, EventArgs e)
        {
            Form frmAbout = new FrmAbout();
            frmAbout.ShowDialog();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenMap();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            NewMap();
        }

        private int RandomAutoMir3Tile(TileType tileType)
        {
            //传奇3 bigTile 30块 2 中组合

            //中  	0-4
            //左上	10
            //右上	11
            //左下	12	
            //右下	13
            //上	20，21
            //下	22，23
            //左	25，27
            //右	26，28
            //内左上 	15
            //内右上	16
            //内左下	17	
            //内右下	18

            //中	5-9
            //左上	18
            //右上	17
            //左下	16
            //右下	15
            //上	22，23
            //下	20，21	
            //左	26，28
            //右	25 ，27	
            //内左上 	13
            //内右上	12
            //内左下	11	
            //内右下	10
            if (selectTilesIndex < 0) return -1;
            int flag;
            if (Libraries.MapLibs[selectListItem.Value].Images.Count%10 != 0)
            {
                flag = (Libraries.MapLibs[selectListItem.Value].Images.Count + 1)/Exine3BigTileBlock;
            }
            else
            {
                flag = Libraries.MapLibs[selectListItem.Value].Images.Count/Exine3BigTileBlock;
            }

            if (selectTilesIndex < flag)
            {
                switch (tileType)
                {
                    case TileType.Center:
                        return selectTilesIndex*Exine3BigTileBlock + random.Next(5);
                    case TileType.Up:
                        return selectTilesIndex*Exine3BigTileBlock + random.Next(20, 22);
                    case TileType.Down:
                        return selectTilesIndex*Exine3BigTileBlock + random.Next(22, 24);
                    case TileType.Left:
                        if (random.Next(2) == 0)
                        {
                            return selectTilesIndex*Exine3BigTileBlock + 25;
                        }
                        return selectTilesIndex*Exine3BigTileBlock + 27;
                    case TileType.Right:
                        if (random.Next(2) == 0)
                        {
                            return selectTilesIndex*Exine3BigTileBlock + 26;
                        }
                        return selectTilesIndex*Exine3BigTileBlock + 28;
                    case TileType.UpLeft:
                        return selectTilesIndex*Exine3BigTileBlock + 10;

                    case TileType.UpRight:
                        return selectTilesIndex*Exine3BigTileBlock + 11;

                    case TileType.DownLeft:
                        return selectTilesIndex*Exine3BigTileBlock + 12;
                    case TileType.DownRight:
                        return selectTilesIndex*Exine3BigTileBlock + 13;

                    case TileType.InUpLeft:
                        return selectTilesIndex*Exine3BigTileBlock + 15;
                    case TileType.InUpRight:
                        return selectTilesIndex*Exine3BigTileBlock + 16;
                    case TileType.InDownLeft:
                        return selectTilesIndex*Exine3BigTileBlock + 17;
                    case TileType.InDownRight:
                        return selectTilesIndex*Exine3BigTileBlock + 18;
                }
            }
            else
            {
                switch (tileType)
                {
                    case TileType.Center:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + random.Next(5, 10);
                    case TileType.Up:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + random.Next(22, 24);
                    case TileType.Down:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + random.Next(20, 22);
                    case TileType.Left:
                        if (random.Next(2) == 0)
                        {
                            return (selectTilesIndex - flag)*Exine3BigTileBlock + 26;
                        }
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 28;
                    case TileType.Right:
                        if (random.Next(2) == 0)
                        {
                            return (selectTilesIndex - flag)*Exine3BigTileBlock + 25;
                        }
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 27;
                    case TileType.UpLeft:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 18;

                    case TileType.UpRight:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 17;

                    case TileType.DownLeft:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 16;
                    case TileType.DownRight:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 15;

                    case TileType.InUpLeft:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 13;
                    case TileType.InUpRight:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 12;
                    case TileType.InDownLeft:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 11;
                    case TileType.InDownRight:
                        return (selectTilesIndex - flag)*Exine3BigTileBlock + 10;
                }
            }

            return -1;
        }

        private void btnMiniMap_Click(object sender, EventArgs e)
        {
            createMiniMap();
        }

        private void btnFreeMemory_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        //loads image for TileCutter
        private void btn_load_Click(object sender, EventArgs e)
        {
            if (loadImageDialog.ShowDialog() != DialogResult.OK) return;
            string fileName = loadImageDialog.FileName;

            try
            {
                _mainImage = new Bitmap(fileName);
                pictureBox_Image.Image = _mainImage;

                //resize the picturebox to force scrollbars to appear if too large to fit on screen
                pictureBox_Image.Width = _mainImage.Width;
                pictureBox_Image.Height = _mainImage.Height;
                pictureBox_Grid.Width = _mainImage.Width;
                pictureBox_Grid.Height = _mainImage.Height;
                pictureBox_Highlight.Width = _mainImage.Width;
                pictureBox_Highlight.Height = _mainImage.Height;
                pictureBox_loaded = true;
            }
            catch
            {
                //error loading image
            }

            //loads array for storing selected cells
            CellSizeX = (comboBox_cellSize.SelectedIndex + 1) * 48;
            CellSizeY = (comboBox_cellSize.SelectedIndex + 1) * 24;
            SelectedCells = new int[(pictureBox_Image.Image.Width / CellSizeX + 2), (pictureBox_Image.Image.Height / CellSizeY + 2)];

            //update grid
            gridUpdate(false);
        }

        //TileCutter
        private void gridUpdate(bool toggle)
        {
            if (pictureBox_Image.Image != null)
            {
                if (toggle)
                    grid = grid == true ? false : true;


                Bitmap image;
                if (grid)
                {

                    image = new Bitmap(pictureBox_Grid.Width, pictureBox_Grid.Height);
                    CellSizeX = (comboBox_cellSize.SelectedIndex + 1) * 48;
                    CellSizeY = (comboBox_cellSize.SelectedIndex + 1) * 24;


                    //vert
                    for (int y = 0; y < ((pictureBox_Image.Image.Height / CellSizeY) + 2) * CellSizeY + 1; y++)
                    {
                        for (int x = 0; x < ((pictureBox_Image.Image.Width / CellSizeX) + 2) * CellSizeX; x++)
                        {
                            if (x >= pictureBox_Grid.Width || y >= pictureBox_Grid.Height) continue;
                            image.SetPixel(x, y, Color.HotPink);
                        }
                        y += CellSizeY - 1;
                    }

                    //horiz
                    for (int x = 0; x < ((pictureBox_Image.Image.Width / CellSizeX) + 2) * CellSizeX + 1; x++)
                    {
                        for (int y = 0; y < ((pictureBox_Image.Image.Height / CellSizeY) + 2) * CellSizeY; y++)
                        {
                            if (x >= pictureBox_Grid.Width || y >= pictureBox_Grid.Height) continue;
                            image.SetPixel(x, y, Color.HotPink);
                        }
                        x += CellSizeX - 1;
                    }



                    pictureBox_Grid.Image = image;
                }
                else
                {
                    image = new Bitmap(1, 1);
                    pictureBox_Grid.Image = image;
                }
            }
        }

        //TileCutter
        #region Move TileCutter Image
        private void btn_up_Click(object sender, EventArgs e)
        {
            pictureBox_Image.Padding = new Padding(pictureBox_Image.Padding.Left, pictureBox_Image.Padding.Top - 1, 0, 0);
            pictureBox_Image.Image = pictureBox_Image.Image;
        }

        private void btn_down_Click(object sender, EventArgs e)
        {
            pictureBox_Image.Padding = new Padding(pictureBox_Image.Padding.Left, pictureBox_Image.Padding.Top + 1, 0, 0);
            pictureBox_Image.Image = pictureBox_Image.Image;
        }

        private void btn_left_Click(object sender, EventArgs e)
        {
            pictureBox_Image.Padding = new Padding(pictureBox_Image.Padding.Left - 1, pictureBox_Image.Padding.Top, 0, 0);
            pictureBox_Image.Image = pictureBox_Image.Image;
            //MessageBox.Show(pictureBox_Image.Padding.Left.ToString());
        }

        private void btn_right_Click(object sender, EventArgs e)
        {
            pictureBox_Image.Padding = new Padding(pictureBox_Image.Padding.Left + 1, pictureBox_Image.Padding.Top, 0, 0);
            pictureBox_Image.Image = pictureBox_Image.Image;
        }
        #endregion

        //TileCutter
        private void btn_grid_Click(object sender, EventArgs e)
        {
            gridUpdate(true);
        }

        //TileCutter
        private void pictureBox_Highlight_Click(object sender, EventArgs e)
        {
            if (pictureBox_loaded != true) return;

            //Find mouse position
            MPoint = pictureBox_Grid.PointToClient(Cursor.Position);

            //test mouse select cells
            if (MPoint.Y >= 0)
            {
                MPoint.Y = (MPoint.Y / CellSizeY);
                if (MPoint.X >= 0)
                {
                    MPoint.X = (MPoint.X / CellSizeX);

                }
                else
                    MPoint.X = -1;
            }
            else
                MPoint.Y = -1;

            if (MPoint.X >= 0 && MPoint.Y >= 0 && MPoint.X <= (pictureBox_Image.Image.Width / CellSizeX) + 1 && MPoint.Y <= (pictureBox_Image.Image.Height / CellSizeY) + 1)
            {
                Bitmap image;
                if (grid)
                {
                    image = new Bitmap(pictureBox_Grid.Width, pictureBox_Grid.Height);

                    SelectedCells[MPoint.X, MPoint.Y] = SelectedCells[MPoint.X, MPoint.Y] == 1 ? 0 : 1;


                    for (int y = 0; y <= (pictureBox_Image.Image.Height / CellSizeY) + 1; y++)
                    {
                        for (int x = 0; x <= (pictureBox_Image.Image.Width / CellSizeX) + 1; x++)
                        {
                            if (SelectedCells[x, y] == 1)
                            {
                                //MessageBox.Show(x + " " + y);
                                using (Graphics g = Graphics.FromImage(image))
                                {
                                    g.DrawImage(cellHighlight, new Point(x * CellSizeX, y * CellSizeY));
                                }
                            }
                        }
                    }


                    //using (Graphics g = Graphics.FromImage(image))
                    //{
                    //g.DrawImage(cellHighlight, new Point(MPoint.X * CellSizeX, MPoint.Y * CellSizeY));
                    //}
                    pictureBox_Highlight.Image = image;
                }
                else
                {
                    image = new Bitmap(1, 1);
                    pictureBox_Highlight.Image = image;
                }
            }
        }

        //TileCutter
        private void comboBox_cellSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridUpdate(false);
        }

        //TileCutter
        private void Main_ResizeEnd(object sender, EventArgs e)
        {
            gridUpdate(false);
        }
        
        private void btn_vCut_Click(object sender, EventArgs e)
        {
            //_mainImage : load full image

            //Create a Library file to save the images in
            SaveLibraryDialog.FileName = loadImageDialog.FileName.Replace(".png", "");
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;
            if (_library != null) _library.Close();
            _library = new XLibrary(SaveLibraryDialog.FileName);

            //adds a single blank cell to the library
            //_library.AddImage(null, 0, 0);

            for (int j = 0; j < _mainImage.Height / CellSizeY; j++)
            {
                for (int i = 0; i < _mainImage.Width / CellSizeX; i++)
                {
                    Bitmap cutImage = new Bitmap(CellSizeX, CellSizeY, PixelFormat.Format32bppArgb);

                    using (Graphics g = Graphics.FromImage(cutImage))
                    {
                        g.DrawImage(_mainImage,
                            new Rectangle(0, 0, cutImage.Width, cutImage.Height),
                            new Rectangle(CellSizeX * i, CellSizeY * j, cutImage.Width, cutImage.Height),
                            GraphicsUnit.Pixel);

                        bool isBlank = true;
                        for (int h = 0; h < cutImage.Height; h++)
                        {
                            for (int w = 0; w < cutImage.Width; w++)
                            {
                                Color col = cutImage.GetPixel(w, h);
                                if (col.A != 0)
                                {
                                    isBlank = false;
                                    break;
                                }
                                //MessageBox.Show(col.A.ToString());
                            }
                            if (!isBlank)
                            {
                                _library.AddImage(cutImage, 0, 0);
                                break;
                            }
                        }
                        if(isBlank) _library.AddImage(null, 0, 0);

                        //_library.AddImage(cutImage, 0, 0);
                    }
                }
            }
            _library.Save();
        }


        private void btn_vCut_Click_Old(object sender, EventArgs e)
        {

            /*
            //add the padding to the mainimage
            Bitmap _mainImageTemp = new Bitmap(_mainImage.Width + pictureBox_Image.Padding.Left, _mainImage.Height + pictureBox_Image.Padding.Top);
            using (Graphics g = Graphics.FromImage(_mainImageTemp))
            {
                g.DrawImage(_mainImage, pictureBox_Image.Padding.Left, pictureBox_Image.Padding.Top, _mainImage.Width, _mainImage.Height);
            }

            //Create a Library file to save the images in
            if (SaveLibraryDialog.ShowDialog() != DialogResult.OK) return;
            if (_library != null) _library.Close();
            _library = new XLibrary(SaveLibraryDialog.FileName);

            //adds a single blank cell to the library
            _library.AddImage(null, 0, 0);

            //cycle through and find image segments
            int tempY = 0;
            for (int x = 0; x < ((pictureBox_Image.Image.Width / CellSizeX) + 2); x++)
            {
                for (int y = 0; y < ((pictureBox_Image.Image.Height / CellSizeY) + 2); y++)
                {
                    //looks for blank parts at top of cell
                    bool isBlank = true;
                    if (tempY == 0)
                    {
                        for (int h = 0; h < CellSizeY; h++)
                        {
                            for (int w = 0; w < CellSizeX; w++)
                            {
                                if (w * x > _mainImageTemp.Width || h * y > _mainImageTemp.Height) continue;
                                Color col = _mainImageTemp.GetPixel(w * x, h * y);

                                if (col.A != 0)
                                {
                                    isBlank = false;
                                    break;
                                }
                                //MessageBox.Show(w + " " + h);
                            }
                            if (!isBlank) break;
                        }
                        if (isBlank) continue;
                    }

                    if (SelectedCells[x, y] == 1)
                    {
                        Bitmap image = new Bitmap(CellSizeX, CellSizeY * (tempY + 1), PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            g.DrawImage(_mainImageTemp, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(x * CellSizeX, (y - tempY) * CellSizeY, image.Width, image.Height), GraphicsUnit.Pixel);
                        }
                        //checks if cell image is blank
                        //bool isBlank = true;
                        for (int h = 0; h < image.Height; h++)
                        {
                            for (int w = 0; w < image.Width; w++)
                            {
                                Color col = image.GetPixel(w, h);
                                if (col.A != 0) isBlank = false;
                                //MessageBox.Show(col.A.ToString());
                            }
                        }

                        //if (!isBlank) _library.AddImage(image, 0, 0); //k333123
                        _library.AddImage(image, 0, 0); //k333123
                        tempY = 0;
                        continue;
                    }
                    tempY++;
                }

                //force last image in column to save if it has not already
                if (tempY > 0)
                {
                    Bitmap image = new Bitmap(CellSizeX, CellSizeY * (tempY), PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawImage(_mainImageTemp, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(x * CellSizeX, (((pictureBox_Image.Image.Height / CellSizeY) + 2) - tempY) * CellSizeY, image.Width, image.Height), GraphicsUnit.Pixel);
                    }
                    //checks if cell image is blank
                    bool isBlank = true;
                    for (int h = 0; h < image.Height; h++)
                    {
                        for (int w = 0; w < image.Width; w++)
                        {
                            Color col = image.GetPixel(w, h);
                            if (col.A != 0) isBlank = false;
                            //MessageBox.Show(col.A.ToString());
                        }
                    }

                    //if (!isBlank) //k333123
                    {
                        //trim image

                        _library.AddImage(image, 0, 0);
                    }
                    tempY = 0; //reset when moving to next column
                }
            }
            _library.Save();
            */
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            ObjectslistBox.Items.Clear();
            ReadObjectsToListBox();
        }

        private void menuSelectAllCells_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(pictureBox_Grid.Width, pictureBox_Grid.Height);
            for (int y = 0; y <= (pictureBox_Image.Image.Height / CellSizeY) + 1; y++)
            {
                for (int x = 0; x <= (pictureBox_Image.Image.Width / CellSizeX) + 1; x++)
                {
                    SelectedCells[x, y] = 1;
                    
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        g.DrawImage(cellHighlight, new Point(x * CellSizeX, y * CellSizeY));
                    }
                }
            }
            pictureBox_Highlight.Image = image;
        }

        private void menuDeselectAllCells_Click(object sender, EventArgs e)
        {
            for (int y = 0; y <= (pictureBox_Image.Image.Height / CellSizeY) + 1; y++)
            {
                for (int x = 0; x <= (pictureBox_Image.Image.Width / CellSizeX) + 1; x++)
                {
                    SelectedCells[x, y] = 0;
                }
            }
            pictureBox_Highlight.Image = new Bitmap(pictureBox_Grid.Width, pictureBox_Grid.Height);
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void cmbEditorLayer_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private int RandomAutoSmTile(TileType iTileType)
        {
            //Then we can return a certain floor tile type based on this rule. .
            if ((int) iTileType >= 1) //In addition to this type of floor tiles
            {
                return selectTilesIndex*smTileBlock + 8 + ((int) iTileType - 1)*4 + random.Next(4);
            }
            //middle floor tile
            return selectTilesIndex *smTileBlock + random.Next(8);
        }

        //Akaras: This should create the correct size MiniMap to use in game... just photoshop it to add caves and doorway icons if needed
        public void createMiniMap()
        {
            Bitmap miniBitmap = new Bitmap(mapWidth * 12, mapHeight * 8, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //backimage
            for (int y = 0; y <= mapHeight - 1; y++)
            {
                for (int x = 0; x <= mapWidth - 1; x++)
                {
                    if ((X2CellInfo[x, y].BackImage & 0x1FFFFFFF) != 0)
                    {
                        try
                        {
                            Libraries.MapLibs[X2CellInfo[x, y].BackIndex].CheckImage((X2CellInfo[x, y].BackImage & 0x1FFFFFFF) - 1);
                            var mi = Libraries.MapLibs[X2CellInfo[x, y].BackIndex].Images[(X2CellInfo[x, y].BackImage & 0x1FFFFFFF) - 1];
                            if (mi.Image != null || mi.ImageTexture != null)
                            {
                                using (Graphics g = Graphics.FromImage(miniBitmap))
                                {
                                    Rectangle temprect = new Rectangle((x * 12), (y * 8), 24, 16);
                                    g.DrawImage(mi.Image, temprect);
                                }
                            }
                        }
                        catch
                        {

                        }

                    }
                    x++;
                }
                y++;
            }

            //MiddleImage
            for (int y = 0; y <= mapHeight - 1; y++)
            {
                for (int x = 0; x <= mapWidth - 1; x++)
                {
                    if ((X2CellInfo[x, y].MiddleImage) != 0)
                    {
                        try
                        {
                            Libraries.MapLibs[X2CellInfo[x, y].MiddleIndex].CheckImage((X2CellInfo[x, y].MiddleImage) - 1);
                            var mi = Libraries.MapLibs[X2CellInfo[x, y].MiddleIndex].Images[(X2CellInfo[x, y].MiddleImage) - 1];
                            if (mi.Image != null || mi.ImageTexture != null)
                            {
                                using (Graphics g = Graphics.FromImage(miniBitmap))
                                {
                                    Rectangle temprect = new Rectangle((x * 12), (y * 8) - (mi.Image.Height / 4) + 8, mi.Image.Width / 4, mi.Image.Height / 4);
                                    g.DrawImage(mi.Image, temprect);
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                }
            }



            //FrontImage
            for (int y = 0; y <= mapHeight - 1; y++)
            {
                for (int x = 0; x <= mapWidth - 1; x++)
                {
                    if ((X2CellInfo[x, y].FrontImage & 0x7FFF) != 0 && (X2CellInfo[x, y].FrontAnimationFrame & 0x80) < 1) //skips drawing if image is animated <<will this cause missing spots for waterfalls and such?
                    {
                        try
                        {
                            Libraries.MapLibs[X2CellInfo[x, y].FrontIndex].CheckImage((X2CellInfo[x, y].FrontImage & 0x7FFF) - 1);
                            var mi = Libraries.MapLibs[X2CellInfo[x, y].FrontIndex].Images[(X2CellInfo[x, y].FrontImage & 0x7FFF) - 1];
                            if (mi.Image != null || mi.ImageTexture != null)
                            {
                                using (Graphics g = Graphics.FromImage(miniBitmap))
                                {
                                    Rectangle temprect = new Rectangle((x * 12), (y * 8) - (mi.Image.Height / 4) + 8, mi.Image.Width / 4, mi.Image.Height / 4);
                                    g.DrawImage(mi.Image, temprect);
                                }
                            }
                        }
                        catch
                        {

                        }

                    }

                }
            }

            miniBitmap = new Bitmap(miniBitmap, (miniBitmap.Width / 8), (miniBitmap.Height / 8));

            string SimpleFileName = Path.GetDirectoryName(mapFileName) + @"\" + Path.GetFileNameWithoutExtension(mapFileName);
            //MessageBox.Show(SimpleFileName);
            miniBitmap.Save(SimpleFileName + "_MiniMap.png", ImageFormat.Png);
            MessageBox.Show("Saved... " + SimpleFileName + "_MiniMap.png"); //TODO: this shows even if failed to save


            //Akaras: Removed this as it was not popular as it is probably easier to photoshop the main MiniMap to 1/2 size and add borders
            //kept the code here if anyone finds a use for it :)
            //BigMap

            //miniBitmap = new Bitmap(miniBitmap, miniBitmap.Width / 2, miniBitmap.Height / 2);
            //using (Graphics g = Graphics.FromImage(miniBitmap))
            //{
            //    Rectangle temprect = new Rectangle(0, 0, 40, 40);
            //    g.DrawImage(BigMapTL, temprect);
            //    temprect = new Rectangle(miniBitmap.Width - 40, 0, 40, 40);
            //    g.DrawImage(BigMapTR, temprect);
            //    temprect = new Rectangle(0, miniBitmap.Height - 40, 40, 40);
            //    g.DrawImage(BigMapBL, temprect);
            //    temprect = new Rectangle(miniBitmap.Width - 40, miniBitmap.Height - 40, 40, 40);
            //    g.DrawImage(BigMapBR, temprect);

            //    // colour the side lines
            //    Color tcol1 = Color.FromArgb(255, 225, 212, 186);
            //    Color tcol2 = Color.FromArgb(255, 165, 133, 70);
            //    for (int i = 38; i < miniBitmap.Width - 38; i++)
            //    {
            //        miniBitmap.SetPixel(i, 0, tcol1); //top first colour
            //        miniBitmap.SetPixel(i, 1, tcol2); //top second colour
            //        miniBitmap.SetPixel(i, miniBitmap.Height - 1, tcol1); //Bottom first colour
            //        miniBitmap.SetPixel(i, miniBitmap.Height - 2, tcol2); //Bottom second colour
            //    }
            //    for (int i = 38; i < miniBitmap.Height - 38; i++)
            //    {
            //        miniBitmap.SetPixel(0, i, tcol1); //Left first colour
            //        miniBitmap.SetPixel(1, i, tcol2); //Left second colour
            //        miniBitmap.SetPixel(miniBitmap.Width - 1, i, tcol1); //Right first colour
            //        miniBitmap.SetPixel(miniBitmap.Width - 2, i, tcol2); //Right second colour
            //    }
            //}

            //SimpleFileName = Path.GetDirectoryName(SelectedMapName) + @"\" + Path.GetFileNameWithoutExtension(SelectedMapName);
            //MessageBox.Show("Saved... " + SimpleFileName);
            //miniBitmap.Save(SimpleFileName + "_BigMap.png", ImageFormat.Png);
            //miniBitmap.Dispose();

        }

        private enum Layer
        {
            None,
            BackImage,
            MiddleImage,
            FrontImage,
            BackLimit,
            FrontLimit,
            BackFrontLimit,
            GraspingMir2Front,
            GraspingInvertMir3FrontMiddle,
            PlaceObjects,
            ClearAll,
            ClearBack,
            ClearMidd,
            ClearFront,
            ClearBackFrontLimit,
            ClearBackLimit,
            ClearFrontLimit,
            BrushMir2BigTiles,
            BrushSmTiles,
            BrushMir3BigTiles
        }

        /*
        private enum MirVerSion : byte
        {
            None,
            WemadeMir2,
            ShandaMir2,
            WemadeMir3,
            ShandaMir3
        }
        */
        private enum ExineVerSion : byte
        {
            None,
            Exine
        }
        private enum TileType
        {
            None = -1,
            Center,
            Up,
            UpRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpLeft,
            InUpRight,
            InDownRight,
            InDownLeft,
            InUpLeft
        }

        #region Idle Check

        public static bool AppStillIdle
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
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Map_Editor
{
    public static class Libraries
    {
        public static bool Loaded;
        public static int Count, Progress;
         
        public const string LibPath = @".\Data\Map\Exine\"; 

        public const string ObjectsPath = @".\Data\Objects\";
        //Map
        //public static readonly XLibrary[] MapLibs = new XLibrary[400];
        //public static readonly ListItem[] ListItems = new ListItem[400];
        public static readonly XLibrary[] MapLibs = new XLibrary[2000];
        public static readonly ListItem[] ListItems = new ListItem[2000];


        static Libraries()
        {
            //Exine-X (allowed from 0-99) 

            //MapLibs[0] = new XLibrary(@".\Data\Map\Exine\Tiles");
            //Tiles mean big tiles and big tiles is apply to background image
            for (int i = 0; i < 10; i++)
            {
                MapLibs[i] = new XLibrary(@".\Data\Map\Exine\TS_0"+i+"_Tile");
                ListItems[i] = new ListItem("Tiles"+i, i);
            }
            
            for (int i = 0; i < 10; i++)
            {
                MapLibs[i+10] = new XLibrary(@".\Data\Map\Exine\TS_0"+i+"_Static");
                ListItems[i+10] = new ListItem("Objects"+i, i+10);
            }

            MapLibs[20] = new XLibrary(@".\Data\Map\Exine\Smtiles");
            ListItems[20] = new ListItem("Smtiles", 20);

            for (int i = 1000; i < 2000; i++)
            {
                if (File.Exists(@".\Data\Map\Exine\Map_" + (9000 + i) + "_FrontTile.lib"))
                {
                    MapLibs[i] = new XLibrary(@".\Data\Map\Exine\Map_" + (9000+i) + "_FrontTile");
                    ListItems[i] = new ListItem("Map_" + (9000+i) + "_FrontTile", i);
                }
            }

            //21~
            /*
             * 00000.map, 00001.map                                                                                             //21~22
             * 10000.map, 10001.map, 10002.map, 10005.map, 10006.map, 10007.map, 10008.map, 10009.map,                          //23~30
             * 10010.map, 10011.map, 10012.map, 10015.map, 10016.map, 10017.map, 10018.map, 10019.map,                          //31~38
             * 10020.map, 10021.map, 10040.map, 10041.map, 10042.map, 10060.map,                                                //39~44
             * 10100.map, 10101.map, 10102.map,                                                                                 //45~47
             * 10200.map, 10201.map, 10202.map, 10203.map, 10204.map, 10205.map, 10206.map, 10207.map, 10208.map, 10209.map,    //48~47
             * 10210.map, 10211.map, 10213.map, 10214.map, 10215.map, 10216.map, 10217.map, 
             * 10230.map, 10231.map, 10232.map, 10233.map, 10234.map, 10235.map, 
             * 10240.map, 10241.map, 10242.map, 10243.map, 10244.map, 10245.map, 
             * 10250.map, 10251.map, 10252.map, 10253.map, 10254.map, 10255.map, 
             * 10260.map, 10261.map, 10262.map, 10263.map, 10264.map, 10265.map, 
             * 10300.map, 10301.map, 10302.map, 10303.map, 10304.map, 10306.map, 10307.map, 10308.map, 10309.map, 10310.map, 
             * 10400.map, 10401.map, 10402.map, 10403.map, 10404.map, 10405.map, 10406.map,10407.map, 10408.map, 10409.map, 
             * 10410.map, 10411.map, 10412.map, 
             * 10500.map,10501.map, 10502.map, 10503.map, 10504.map, 10505.map, 10506.map, 
             * 10600.map, 10601.map, 10602.map, 10603.map, 
             * 10700.map, 10701.map, 10702.map, 10703.map, 10704.map, 10705.map, 10706.map, 10707.map, 
             * 10800.map, 10801.map, 10802.map, 10803.map, 10804.map, 10805.map, 10806.map, 10807.map, 10808.map, 10809.map, 
             * 10810.map, 10811.map, 10812.map, 10813.map, 10814.map, 10815.map, 10816.map, 10817.map, 10818.map, 10819.map, 
             * 10820.map
             */


            //56:0,57:1,58:2,59:3,116:4
            //index 99
            MapLibs[99] = new XLibrary(@".\Data\Map\Exine\LimitTiles");
            ListItems[99] = new ListItem("LimitTiles", 99);



            /*
            for (int i = 2; i < 99; i++)
            {
                if (File.Exists(@".\Data\Map\Exine\Objects" + i + ".lib"))
                {
                    MapLibs[i + 1] = new XLibrary(@".\Data\Map\Exine\Objects" + i);
                    ListItems[i + 1] = new ListItem("Objects" + i, i + 1);
                }
            }*/

            //Thread thread = new Thread(LoadGameLibraries) { IsBackground = true };
            //thread.Start();
        }


        public static void LoadGameLibraries()
        {
            Count = MapLibs.Length;

            for (int i = 0; i < MapLibs.Length; i++)
            {
                if (MapLibs[i] == null)
                    MapLibs[i] = new XLibrary("");
                else
                    MapLibs[i].Initialize();
                Progress++;
            }
            Loaded = true;
        }

    }
    public sealed class XLibrary
    {
        public const int LibVersion = 3;
        public static bool Load = true;
        public string FileName;

        public List<MImage> Images = new List<MImage>();
        public List<int> IndexList = new List<int>();
        public int Count;
        private bool _initialized;

        private BinaryReader _reader;
        private FileStream _stream;

        public XLibrary(string filename)
        {
            FileName = filename + ".lib";
            Initialize();
        }

        public void Initialize()
        {
            int CurrentVersion;
            _initialized = true;

            if (!File.Exists(FileName))
                return;

            _stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            _reader = new BinaryReader(_stream);
            CurrentVersion = _reader.ReadInt32();
            if (CurrentVersion < 2)
            {
                MessageBox.Show("Wrong version, expecting lib version: " + LibVersion.ToString() + " found version: " + CurrentVersion.ToString() + ".", "Failed to open", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            Count = _reader.ReadInt32();
            Images = new List<MImage>();
            IndexList = new List<int>();


            int frameSeek = 0;
            if (CurrentVersion >= 3)
            {
                frameSeek = _reader.ReadInt32();
            }

            for (int i = 0; i < Count; i++)
                IndexList.Add(_reader.ReadInt32());

            for (int i = 0; i < Count; i++)
                Images.Add(null);

            //for (int i = 0; i < Count; i++)
            //    CheckImage(i);
        }


        public void Close()
        {
            if (_stream != null)
                _stream.Dispose();
            if (_reader != null)
                _reader.Dispose();
        }

        public void Save()
        {
            Close();

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            Count = Images.Count;
            IndexList.Clear();

            int offSet = (4 + 4 + 4) + (Count * 4);
            for (int i = 0; i < Count; i++)
            {
                IndexList.Add((int)stream.Length + offSet);
                Images[i].Save(writer); 
            }
            var frameSeek = (int)stream.Length + offSet;

            writer.Flush();
            byte[] fBytes = stream.ToArray(); 

            _stream = File.Create(FileName);
            writer = new BinaryWriter(_stream);

            writer.Write(LibVersion);
            writer.Write(Count);
            writer.Write(frameSeek);

            for (int i = 0; i < Count; i++)
                writer.Write(IndexList[i]);

            writer.Write(fBytes);

            writer.Write(0);
              
            writer.Flush();
            writer.Close();
            writer.Dispose();
            Close();
        }

        /*
        public void Save()
        {
            Close();

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            Count = Images.Count;
            IndexList.Clear();

            //int offSet = 8 + Count * 4;
            int offSet = 8 + Count * 4;
            for (int i = 0; i < Count; i++)
            {
                IndexList.Add((int)stream.Length + offSet);
                Images[i].Save(writer);
                //Images[i] = null;
            }

            writer.Flush();
            byte[] fBytes = stream.ToArray();
            //  writer.Dispose();

            _stream = File.Create(FileName);
            writer = new BinaryWriter(_stream);
            writer.Write(LibVersion);
            writer.Write(Count);
            for (int i = 0; i < Count; i++)
                writer.Write(IndexList[i]);

            writer.Write(fBytes);
            writer.Flush();
            writer.Close();
            writer.Dispose();
            Close();
        }*/

        public void CheckImage(int index)
        {
            if (!_initialized)
                Initialize();

            if (Images == null || index < 0 || index >= Images.Count)
                return;
            if (_stream == null)
            {
                return;
            }
            if (Images[index] == null)
            {
                _stream.Position = IndexList[index];
                Images[index] = new MImage(_reader);
            }

            if (!Load) return;

            //MImage mi = Images[index];
            //if (!mi.TextureValid)
            //{
            //    _stream.Seek(IndexList[index] + 12, SeekOrigin.Begin);
            //    mi.CreateTexture(_reader);
            //}
            if (!Images[index].TextureValid)
            {
                _stream.Seek(IndexList[index] + 12, SeekOrigin.Begin);
                Images[index].CreateBmpTexture(_reader);
            }

            if (!Images[index].TextureValid)
            {

                _stream.Seek(IndexList[index] + 17, SeekOrigin.Begin);
                Images[index].CreateTexture(_reader);
            }
        }

        public Point GetOffSet(int index)
        {
            if (!_initialized)
                Initialize();

            if (Images == null || index < 0 || index >= Images.Count)
                return Point.Empty;

            if (Images[index] == null)
            {
                _stream.Seek(IndexList[index], SeekOrigin.Begin);
                Images[index] = new MImage(_reader);
            }

            return new Point(Images[index].X, Images[index].Y);
        }

        public Size GetSize(int index)
        {
            if (!_initialized)
                Initialize();
            if (Images == null || index < 0 || index >= Images.Count)
                return Size.Empty;

            if (Images[index] == null)
            {
                _stream.Seek(IndexList[index], SeekOrigin.Begin);
                Images[index] = new MImage(_reader);
            }

            return new Size(Images[index].Width, Images[index].Height);
        }

        public MImage GetMImage(int index)
        {
            if (index < 0 || index >= Images.Count)
                return null;
            CheckImage(index);
            return Images[index];
        }

        public Bitmap GetPreview(int index)
        {
            if (index < 0 || index >= Images.Count)
                return new Bitmap(1, 1);

            MImage image = Images[index];

            if (image == null || image.Image == null)
                return new Bitmap(1, 1);

            if (image.Preview == null)
                image.CreatePreview();
            Bitmap preview = image.Preview;
            return preview;
        }

        public void AddImage(Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Count++;
            Images.Add(mImage);
        }

        public void ReplaceImage(int Index, Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Images[Index] = mImage;
        }

        public void InsertImage(int index, Bitmap image, short x, short y)
        {
            MImage mImage = new MImage(image) { X = x, Y = y };

            Count++;
            Images.Insert(index, mImage);
        }

        public void RemoveImage(int index)
        {
            if (Images == null || Count <= 1)
            {
                Count = 0;
                Images = new List<MImage>();
                return;
            }
            Count--;

            Images.RemoveAt(index);
        }

        public static bool CompareBytes(byte[] a, byte[] b)
        {
            if (a == b) return true;

            if (a == null || b == null || a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

            return true;
        }

        public void RemoveBlanks(bool safe = false)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (Images[i].FBytes == null || Images[i].FBytes.Length <= 24)
                {
                    if (!safe)
                        RemoveImage(i);
                    else if (Images[i].X == 0 && Images[i].Y == 0)
                        RemoveImage(i);
                }
            }
        }

        public sealed class MImage
        {
            public short Width, Height, X, Y, ShadowX, ShadowY;
            public byte Shadow;
            public int Length;
            public byte[] FBytes;
            public bool TextureValid;
            public Bitmap Image, Preview;
            public Texture ImageTexture;

            //layer 2:
            public short MaskWidth, MaskHeight, MaskX, MaskY;

            public int MaskLength;
            public byte[] MaskFBytes;
            public Bitmap MaskImage;
            public Texture MaskImageTexture;
            public Boolean HasMask;

            public unsafe byte* Data;

            public MImage(BinaryReader reader)
            {
                //read layer 1
                Width = reader.ReadInt16();
                Height = reader.ReadInt16();
                X = reader.ReadInt16();
                Y = reader.ReadInt16();
                ShadowX = reader.ReadInt16();
                ShadowY = reader.ReadInt16();
                Shadow = reader.ReadByte();
                Length = reader.ReadInt32();
                FBytes = reader.ReadBytes(Length);
                //check if there's a second layer and read it
                HasMask = ((Shadow >> 7) == 1) ? true : false;
                if (HasMask)
                {
                    MaskWidth = reader.ReadInt16();
                    MaskHeight = reader.ReadInt16();
                    MaskX = reader.ReadInt16();
                    MaskY = reader.ReadInt16();
                    MaskLength = reader.ReadInt32();
                    MaskFBytes = reader.ReadBytes(MaskLength);
                }
            }

            public MImage(byte[] image, short Width, short Height)//only use this when converting from old to new type!
            {
                FBytes = image;
                this.Width = Width;
                this.Height = Height;
            }

            public MImage(Bitmap image)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;

                Image = image;// FixImageSize(image);
                FBytes = ConvertBitmapToArray(Image);
            }

            public MImage(Bitmap image, Bitmap Maskimage)
            {
                if (image == null)
                {
                    FBytes = new byte[0];
                    return;
                }

                Width = (short)image.Width;
                Height = (short)image.Height;
                Image = image;// FixImageSize(image);
                FBytes = ConvertBitmapToArray(Image);
                if (Maskimage == null)
                {
                    MaskFBytes = new byte[0];
                    return;
                }
                HasMask = true;
                MaskWidth = (short)Maskimage.Width;
                MaskHeight = (short)Maskimage.Height;
                MaskImage = Maskimage;// FixImageSize(Maskimage);
                MaskFBytes = ConvertBitmapToArray(MaskImage);
            }

            private Bitmap FixImageSize(Bitmap input)
            {
                int w = input.Width + (4 - input.Width % 4) % 4;
                int h = input.Height + (4 - input.Height % 4) % 4;

                if (input.Width != w || input.Height != h)
                {
                    Bitmap temp = new Bitmap(w, h);
                    using (Graphics g = Graphics.FromImage(temp))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.DrawImage(input, 0, 0);
                        g.Save();
                    }
                    input.Dispose();
                    input = temp;
                }

                return input;
            }

            private unsafe byte[] ConvertBitmapToArray(Bitmap input)
            {
                BitmapData data = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

                byte[] pixels = new byte[input.Width * input.Height * 4];

                Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);

                input.UnlockBits(data);

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    if (pixels[i] == 0 && pixels[i + 1] == 0 && pixels[i + 2] == 0)
                        pixels[i + 3] = 0; //Make Transparent
                }

                byte[] compressedBytes;
                compressedBytes = Compress(pixels);

                return compressedBytes;
            }

            public unsafe void CreateBmpTexture(BinaryReader reader)
            {

                //bmp
                int w = Width;// +(4 - Width % 4) % 4;
                int h = Height;// +(4 - Height % 4) % 4;

                if (w == 0 || h == 0)
                    return;
                if ((w < 2) || (h < 2)) return;
                Image = new Bitmap(w, h);

                BitmapData data = Image.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                                                 PixelFormat.Format32bppArgb);

                byte[] dest = Decompress(FBytes);

                Marshal.Copy(dest, 0, data.Scan0, dest.Length);

                Image.UnlockBits(data);

                //if (Image.Width > 0 && Image.Height > 0)
                //{
                //    Guid id = Guid.NewGuid();
                //    Image.Save(id + ".bmp", ImageFormat.Bmp);
                //}

                dest = null;

                if (HasMask)
                {
                    w = MaskWidth;// +(4 - MaskWidth % 4) % 4;
                    h = MaskHeight;// +(4 - MaskHeight % 4) % 4;

                    if (w == 0 || h == 0)
                    {
                        return;
                    }

                    try
                    {
                        MaskImage = new Bitmap(w, h);

                        data = MaskImage.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                                                         PixelFormat.Format32bppArgb);

                        dest = Decompress(MaskFBytes);

                        Marshal.Copy(dest, 0, data.Scan0, dest.Length);

                        MaskImage.UnlockBits(data);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@".\Error.txt",
                                       string.Format("[{0}] {1}{2}", DateTime.Now, ex, Environment.NewLine));
                    }
                }


            }

            public unsafe void CreateTexture(BinaryReader reader)
            {
                int w = Width;// + (4 - Width % 4) % 4;
                int h = Height;// + (4 - Height % 4) % 4;
                if (w == 0 || h == 0)
                    return;
                if ((w < 2) || (h < 2)) return;

                GraphicsStream stream = null;

                ImageTexture = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                stream = ImageTexture.LockRectangle(0, LockFlags.Discard);
                Data = (byte*)stream.InternalDataPointer;

                byte[] decomp = DecompressImage(reader.ReadBytes(Length));

                stream.Write(decomp, 0, decomp.Length);

                stream.Dispose();
                ImageTexture.UnlockRectangle(0);

                if (HasMask)
                {
                    reader.ReadBytes(12);
                    w = Width;// + (4 - Width % 4) % 4;
                    h = Height;// + (4 - Height % 4) % 4;

                    MaskImageTexture = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                    stream = MaskImageTexture.LockRectangle(0, LockFlags.Discard);

                    decomp = DecompressImage(reader.ReadBytes(Length));

                    stream.Write(decomp, 0, decomp.Length);

                    stream.Dispose();
                    MaskImageTexture.UnlockRectangle(0);
                }

                //DXManager.TextureList.Add(this);
                TextureValid = true;
                //Image.Disposing += (o, e) =>
                //{
                //    TextureValid = false;
                //    Image = null;
                //    MaskImage = null;
                //    Data = null;
                //    DXManager.TextureList.Remove(this);
                //};
            }

            public void Save(BinaryWriter writer)
            {
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(X);
                writer.Write(Y);
                writer.Write(ShadowX);
                writer.Write(ShadowY);
                writer.Write(HasMask ? (byte)(Shadow | 0x80) : (byte)Shadow);
                writer.Write(FBytes.Length);
                writer.Write(FBytes);
                if (HasMask)
                {
                    writer.Write(MaskWidth);
                    writer.Write(MaskHeight);
                    writer.Write(MaskX);
                    writer.Write(MaskY);
                    writer.Write(MaskFBytes.Length);
                    writer.Write(MaskFBytes);
                }
            }

            public static byte[] Compress(byte[] raw)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                    {
                        gzip.Write(raw, 0, raw.Length);
                    }
                    return memory.ToArray();
                }
            }

            static byte[] Decompress(byte[] gzip)
            {
                // Create a GZIP stream with decompression mode.
                // ... Then create a buffer and write into while reading from the GZIP stream.
                using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }

            private static byte[] DecompressImage(byte[] image)
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(image), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }

            public void CreatePreview()
            {
                if (Image == null)
                {
                    Preview = new Bitmap(1, 1);
                    return;
                }

                Preview = new Bitmap(64, 64);

                using (Graphics g = Graphics.FromImage(Preview))
                {
                    g.InterpolationMode = InterpolationMode.Low;//HighQualityBicubic
                    g.Clear(Color.Transparent);
                    int w = Math.Min((int)Width, 64);
                    int h = Math.Min((int)Height, 64);
                    g.DrawImage(Image, new Rectangle((64 - w) / 2, (64 - h) / 2, w, h), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);

                    g.Save();
                }
            }
        }
    }
}
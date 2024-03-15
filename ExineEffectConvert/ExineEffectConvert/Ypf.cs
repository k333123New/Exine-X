//#define DETAIL_DEBUG
#define SAVE_PNG
//#define SAVE_DEPTH_PNG
#define SAVE_JSON

using ExineUnpacker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ExineEffectConvert
{
    class Ypf
    {
        [Conditional("DETAIL_DEBUG")]
        public static void DWrite(string mes)
        {
            Console.Write(mes);
        }

        [Conditional("DETAIL_DEBUG")]
        public static void DWriteLine()
        {
            Console.WriteLine();
        }

        [Conditional("DETAIL_DEBUG")]
        public static void DWriteLine(string mes)
        {
            Console.WriteLine(mes);
        }

        public static void UnYpfs(string targetDir, string outputDir)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DirectoryInfo targetRoot = new DirectoryInfo(targetDir);
            DirectoryInfo[] subDirs = targetRoot.GetDirectories("*.*", System.IO.SearchOption.AllDirectories);


            string title = $"| {"file path",-70} | {"ypfVer",-6} | {"w",-5} | {"h",-5} | {"depthRes",-8} | {"hasPalette",-11} | {"dataPos",-8} | {"imageCount",-10} |";

            Console.WriteLine(title);
            Console.WriteLine("".PadLeft(title.Length, '-'));


            string[] files = Directory.GetFiles(targetDir, $"*.ypf", System.IO.SearchOption.AllDirectories);

            foreach (string file in files)
            {
                UnYpf(file, Path.Combine(outputDir, Path.GetDirectoryName(file.Substring(targetDir.Length + 1))));
            }
        }

        public static void UnYpf(string target, string output)
        {
            // if (Path.GetFileNameWithoutExtension(target) != "Effect01")
            //     return;

            ByteReader br = new ByteReader(target);

            ushort ypfVer;
            ushort width;
            ushort height;
            uint depthRes;
            bool hasPalette = false;
            uint dataPos;
            ushort imageCount;

            {
                if (br.buffer.Length == 0) return;

                if (br.Read(out ypfVer)) return;            // unknown.w //旧framecountとのコメントあり
                if (br.Read(out width)) return;             // width.w
                if (br.Read(out height)) return;            // height.w
                if (br.Read(out depthRes)) return;          // depthRes.l
                if (br.Read(out hasPalette)) return;        // paletteFlag.l(bool)
                if (hasPalette) br.index += 512;            // palette
                if (br.Read(out dataPos)) return;           // dataPos.l //データ部の開始アドレス
                if (br.Read(out imageCount)) return;        // imageCount.w //ImageSet

                Console.WriteLine($"| {target.PadRight(target.Length + 70 - Encoding.GetEncoding("euc-kr").GetByteCount(target))} | {ypfVer,6:D} | {width,5} | {height,5} | {depthRes,8:D} | {hasPalette,-11:s} | {dataPos,8:X8} |{imageCount,10} |");


                ushort actionCount;
                ushort stateCount;
                ushort stateTransValueCount;

                for (int imageIndex = 0; imageIndex < imageCount; ++imageIndex)
                {
                    int startByteIndex = br.index;

                    //if (ypfVer == 14)
                    //{

                    //}
                    //else
                    uint unknown1;
                    uint unknown2 = 0;
                    ushort frameCount;

                    br.Read(out unknown1);                                                          // unknown1.l
                    if (unknown1 != 0)                                                              // if unknown1
                    {
                        br.Read(out unknown2);                                                      //   unknown2.l

                        DWriteLine($"imageIndex {imageIndex} : {br.index:X8} unknown1 {unknown1}, unknown2 {unknown2}");

                        ByteReader unkown2br = new ByteReader(br.buffer, (int)unknown2);

                        for (int i = 0; i < unknown1; ++i)
                        {
                            byte data;
                            unkown2br.Read(out data);

                            DWriteLine($"unknown2 data : {data}");
                        }
                    }


                    br.Read(out frameCount);                                                        // frameCount.w

                    uint preBaseLength = 0;
                    uint preBaseOffset = 0;

                    for (int frameIndex = 0; frameIndex < frameCount; ++frameIndex)                 // for frameCount
                    {
                        Console.Title = $"{target} {imageIndex,6:D} {frameIndex,6:D}";


                        if (dataPos <= br.index)
                        {
                            Console.WriteLine($"ㄴ !!Check!! Header out of index");
                            return;
                        }

                        short top;
                        short left;
                        short bottom;
                        short right;
                        uint flag;

                        br.Read(out top);                                                           //   top.w
                        br.Read(out left);                                                          //   left.w
                        br.Read(out bottom);                                                        //   bottom.w
                        br.Read(out right);                                                         //   right.w
                        br.Read(out flag);                                                          //   flag.l

                        int frameWidth = right - left;
                        int frameHeight = bottom - top;

                        DWriteLine($"{br.index:X8} rect {top}, {left}, {bottom}, {right} flag {flag}");

                        bool hasAlpha = (flag & 8) == 0x0;
                        bool hasBase = (flag & 4) != 0x0;
                        bool hasDepth = (flag & 1) != 0x0;
                        DWriteLine($"{br.index:X8} hasAlpha {hasAlpha}, hasBase {hasBase}, hasDepth {hasDepth}");


                        uint alphaLength = 0;
                        uint alphaOffset = 0;
                        uint baseLength = 0;
                        uint baseOffset = 0;

                        if (hasAlpha)                                                        //   if flag & 8 == 0 // hasAlpha
                        {
                            br.Read(out alphaLength);                                               //     length.l
                            br.Read(out alphaOffset);                                               //     dataOffset.l
                        }

                        if (ypfVer == 14)
                        {
                            if (hasBase)                                                         //   if flag & 4 // hasBase
                            {
                                br.Read(out baseOffset);                                                //     length.l
                                baseLength = (uint)frameWidth * (uint)frameHeight;
                            }
                        }
                        else
                        {
                            if (hasBase)                                                         //   if flag & 4 // hasBase
                            {
                                br.Read(out baseOffset);                                                //     dataOffset.l
                                br.Read(out baseLength);                                                //     length.l
                            }
                        }

                        byte depthType = 0;
                        ushort val2 = 0;
                        ushort nearestDist = 0;
                        uint depthOffset = 0;
                        uint depthSize = 0;

                        if (hasDepth)                                                        //   if flag & 1 // hasDepth
                        {
                            if (ypfVer == 16)                                                       //     if ypfVer == 16
                            {
                                ushort tempDepthSize;
                                br.Read(out tempDepthSize);                                             //       depthSize.l
                                ushort tempDepthSize2;
                                br.Read(out tempDepthSize2);                                             //       depthSize.l
                                depthSize = (uint)tempDepthSize2;
                                br.Read(out depthOffset);                                           //       depthOffset.l
                                 

                                DWriteLine($"{br.index:X8} tempDepthSize {tempDepthSize}, tempDepthSize2 {tempDepthSize2}, depthOffset {depthOffset}");
                                //Console.WriteLine($"{br.index:X8} tempDepthSize {tempDepthSize}, tempDepthSize2 {tempDepthSize2}, depthOffset {depthOffset}");
                                //Console.WriteLine($"{br.index:X8} tempDepthSize {tempDepthSize}, tempDepthSize2 {tempDepthSize2}, depthOffset {depthOffset}");
                                //Console.WriteLine($"{imageIndex}-{frameIndex}");
                                //Console.WriteLine($"width {right - left}, heigth {bottom - top}");
                                //Console.WriteLine($"alphaLength {alphaLength}, alphaOffset {dataPos + alphaOffset}");
                                //Console.WriteLine($"baseLength {baseLength}, baseOffset {dataPos + baseOffset}");
                                //Console.WriteLine($"depthSize {depthSize}, depthOffset {dataPos + depthOffset}");
                                //Console.WriteLine($"depthSize {depthSize}, depthOffset {dataPos + depthOffset}");
                            }
                            else if (ypfVer == 14)
                            {
                                uint ver14_depth_1 = 0;
                                uint ver14_depth_2 = 0;
                                br.Read(out ver14_depth_1);
                                br.Read(out ver14_depth_2);
                            }
                            else                                                                    //     else
                            {
                                br.Read(out depthType);                                             //       depthType.b //0がWORD / depthRes, 1がBYTE / depthRes, 2はstride使って何かしてて3はnearestDistでfill
                                br.Read(out val2);                                                  //       val2.w
                                br.Read(out nearestDist);                                           //       nearestDist.w
                                br.Read(out depthOffset);                                           //       depthOffset.l
                                br.Read(out depthSize);                                             //       depthSize.l

                                DWriteLine($"{br.index:X8} depthType {depthType}, val2 {val2}, nearestDist {nearestDist}, depthOffset {depthOffset}, depthSize {depthSize}");

                                //Console.WriteLine($"{imageIndex}-{frameIndex}");
                                //Console.WriteLine($"width {right - left}, heigth {bottom - top}");
                                //Console.WriteLine($"alphaLength {alphaLength}, alphaOffset {dataPos + alphaOffset}");
                                //Console.WriteLine($"baseLength {baseLength}, baseOffset {dataPos + baseOffset}");
                                //Console.WriteLine($"depthSize {depthSize}, depthOffset {dataPos + depthOffset}");
                            }
                        }
                        DWriteLine($"imageIndex {imageIndex} : {br.index:X8} alphaLength {alphaLength}, alphaOffset {alphaOffset}");
                        DWriteLine($"imageIndex {imageIndex} : {br.index:X8} baseLength {baseLength}, baseOffset {baseOffset}");
                        DWriteLine($"imageIndex {imageIndex} : {br.index:X8} depthOffset {depthOffset}, depthSize {depthSize}");

                        uint unk2Length;
                        uint unk2offset = 0;

                        br.Read(out unk2Length);                                                    //   unk2Length.l
                        if (unk2Length != 0)                                                        //   if unk2Length
                        {
                            br.Read(out unk2offset);                                                //     unk2offset.l

                            DWriteLine($"unk2offset {unk2offset} unk2Length {unk2Length}");

                            ByteReader unk2br = new ByteReader(br.buffer, (int)unk2offset);

                            for (int i = 0; i < unk2Length; ++i)
                            {
                                byte data;
                                unk2br.Read(out data);

                                DWriteLine($"unk2br data : {data}");
                            }
                        }


                        if (ypfVer != 14 && dataPos + baseOffset < dataPos + preBaseOffset + preBaseLength)
                        {
                            Console.WriteLine($"ㄴ !!Check!! Wrong offset");
                            continue;
                        }
                        preBaseOffset = baseOffset;
                        preBaseLength = baseLength;

                        DWriteLine($"{br.index:X8} {startByteIndex:X8} {$"{imageIndex}-{frameIndex}",-10} : width = {frameWidth}, height = {frameHeight}, rect {top}, {left}, {bottom}, {right}");

                        if (height < frameHeight || width < frameWidth || 0 > frameHeight || 0 > frameWidth)
                        {
                            Console.WriteLine($"ㄴ !!Check!! {startByteIndex:X8} {$"{imageIndex}-{frameIndex}",-10} : width = {frameWidth}, height = {frameHeight}, rect {top}, {left}, {bottom}, {right}");
                            //return;
                        }
#if SAVE_PNG
                        else if (0 == frameHeight || 0 == frameWidth)
                        {
                            Console.WriteLine($"ㄴ IgnoreEmptyImage {startByteIndex:X8} {$"{imageIndex}-{frameIndex}",-10} : width = {frameWidth}, height = {frameHeight}, rect {top}, {left}, {bottom}, {right}");
                            //return;
                        }
                        else
                        {
                            float pivotX = -left;
                            float pivotY = bottom;

                            string savePath =
                                Path.Combine(
                                    output,
                                    Path.GetFileNameWithoutExtension(target),
                                    $"{Path.GetFileNameWithoutExtension(target)}_{imageIndex}_{frameIndex}_size_{frameWidth}_{frameHeight}_pivot_{pivotX}_{pivotY}.bmp");

                            if (savePath.IndexOf("Shadows") != -1)
                            {
                                int stop = 0;
                            }

                            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(savePath));

                            if (directoryInfo.Exists == false)
                            {
                                directoryInfo.Create();
                            }


                            int leftSize = Math.Abs(left);
                            int rightSize = Math.Abs(right);
                            int topSize = Math.Abs(top);
                            int bottomSize = Math.Abs(bottom);

                            int offsetX = 0;
                            int offsetY = 0;

                            int imageWidth = 0;
                            int imageHeight = 0;

                            if (leftSize > rightSize)
                            {
                                imageWidth = leftSize * 2;
                            }
                            else
                            {
                                imageWidth = rightSize * 2;
                                offsetX = imageWidth - frameWidth;
                            }

                            if (topSize > bottomSize)
                            {
                                imageHeight = topSize * 2;
                            }
                            else
                            {
                                imageHeight = bottomSize * 2;
                                offsetY = imageHeight - frameHeight;
                            } 

                            if (hasAlpha)
                            {
                                int index = (int)(alphaOffset + dataPos);

                                int[] rows = new int[frameHeight];
                                int rowSize1 = 0;

                                for (int i = 0; i < frameHeight; ++i)
                                {
                                    int rowSize2 = BitConverter.ToInt32(br.buffer, index);
                                    index += 4;
                                    rows[i] = (rowSize2 - rowSize1) / 2;
                                    rowSize1 = rowSize2;
                                }

                                byte[] alphaMask = new byte[frameWidth * frameHeight];

                                for (int y = 0; y < frameHeight; ++y)
                                {
                                    int x = 0;
                                    for (int i = 0; rows[y] < frameWidth; ++i)
                                    {
                                        if (index >= dataPos + alphaOffset + alphaLength) break;

                                        byte data1 = br.buffer[index++];
                                        byte data2 = br.buffer[index++];

                                        for (int j = 0; j < ((data1 & 7) << 8 | data2); j++)
                                        {
                                            int dataIndex = (x++) + y * frameWidth;
                                            if (alphaMask.Length <= dataIndex)
                                                goto AlphaReadEnd;

                                            alphaMask[dataIndex] = (byte)(data1 & 0xf8);
                                        }
                                    }
                                }
                            AlphaReadEnd:

#endif

#if SAVE_DEPTH_PNG
                                //연구중
                                if (hasDepth != 0x0)
                                {
                                    DWriteLine();
                                    DWriteLine($"alphaLength {alphaLength}, alphaOffset {dataPos + alphaOffset}");
                                    DWriteLine($"baseLength {baseLength}, baseOffset {dataPos + baseOffset}");
                                    DWriteLine($"depthSize {depthSize}, depthOffset {dataPos + depthOffset}");

                                    DWriteLine($"depthType {depthType}");
                                    DWriteLine($"val2 {val2}");
                                    DWriteLine($"nearestDist {nearestDist}");

                                    int depthWidth = (int)Math.Ceiling((float)frameWidth / depthRes);
                                    int depthHeight = (int)Math.Ceiling((float)frameHeight / depthRes);

                                    int depthCount = depthWidth * depthHeight;
                                    DWriteLine($"depthWidth {depthWidth}, depthHeight {depthHeight}, depthCount {depthCount}");

                                    if (depthType == 0)
                                    {
                                        byte[] colorData = new byte[depthCount * 3];

                                        ByteReader dr = new ByteReader(br.buffer, (int)(depthOffset + dataPos));


                                        ushort depthData;
                                        byte[] depthDatas;

                                        for (int i = 0; i < depthCount; ++i)
                                        {
                                            //depthData /= 2;
                                            //depthData = (ushort)((depthData / 128.0) * 255.0);
                                            dr.Read(out depthData);
                                            depthDatas = BitConverter.GetBytes((ushort)(nearestDist + depthData));

                                            colorData[i * 3 + 0] = depthDatas[0];
                                            colorData[i * 3 + 1] = depthDatas[1];
                                        }

                                        Bitmap bitmap = new Bitmap(depthWidth, depthHeight, PixelFormat.Format24bppRgb);
                                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                        int rowSize = Math.Abs(bmpData.Stride);
                                        IntPtr bmpScan0 = bmpData.Scan0;
                                        for (int y = 0; y < bitmap.Height; y++)
                                        {
                                            Marshal.Copy(colorData, y * bitmap.Width * 3, IntPtr.Add(bmpScan0, y * rowSize), bitmap.Width * 3);
                                        }
                                        bitmap.UnlockBits(bmpData);
                                        bitmap.Save(savePath + "_depth.png", ImageFormat.Png);
                                        bitmap.Dispose();
                                    }

                                    if (depthType == 1)
                                    {
                                        byte[] colorData = new byte[depthCount * 3];

                                        ByteReader dr = new ByteReader(br.buffer, (int)(depthOffset + dataPos));

                                        byte depthData;
                                        byte[] depthDatas;

                                        for (int i = 0; i < depthCount; ++i)
                                        {
                                            dr.Read(out depthData);
                                            depthDatas = BitConverter.GetBytes((ushort)(nearestDist + depthData));

                                            colorData[i * 3 + 0] = depthDatas[0];
                                            colorData[i * 3 + 1] = depthDatas[1];
                                        }

                                        Bitmap bitmap = new Bitmap(depthWidth, depthHeight, PixelFormat.Format24bppRgb);
                                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                        int rowSize = Math.Abs(bmpData.Stride);
                                        IntPtr bmpScan0 = bmpData.Scan0;
                                        for (int y = 0; y < bitmap.Height; y++)
                                        {
                                            Marshal.Copy(colorData, y * bitmap.Width * 3, IntPtr.Add(bmpScan0, y * rowSize), bitmap.Width * 3);
                                        }
                                        bitmap.UnlockBits(bmpData);
                                        bitmap.Save(savePath + "_depth.png", ImageFormat.Png);
                                        bitmap.Dispose();
                                    }

                                    if (depthType == 2)
                                    {
                                        byte[] colorData = new byte[depthCount * 3];

                                        ByteReader dr = new ByteReader(br.buffer, (int)(depthOffset + dataPos));

                                        byte depthData;
                                        byte[] depthDatas;

                                        int stride = (int)(depthSize / depthHeight - depthWidth);

                                        for (int i = 0; i < depthCount; ++i)
                                        {
                                            dr.Read(out depthData);
                                            depthDatas = BitConverter.GetBytes((ushort)(nearestDist + depthData));

                                            colorData[i * 3 + 0] = depthDatas[0];
                                            colorData[i * 3 + 1] = depthDatas[1];
                                            if (i % depthHeight == 0) dr.index += stride;
                                        }

                                        Bitmap bitmap = new Bitmap(depthWidth, depthHeight, PixelFormat.Format24bppRgb);
                                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                        int rowSize = Math.Abs(bmpData.Stride);
                                        IntPtr bmpScan0 = bmpData.Scan0;
                                        for (int y = 0; y < bitmap.Height; y++)
                                        {
                                            Marshal.Copy(colorData, y * bitmap.Width * 3, IntPtr.Add(bmpScan0, y * rowSize), bitmap.Width * 3);
                                        }
                                        bitmap.UnlockBits(bmpData);
                                        bitmap.Save(savePath + "_depth.png", ImageFormat.Png);
                                        bitmap.Dispose();
                                    }

                                    if (depthType == 3)
                                    {
                                        byte[] colorData = new byte[3];

                                        ByteReader dr = new ByteReader(br.buffer, (int)(depthOffset + dataPos));


                                        ushort depthData;
                                        byte[] depthDatas;

                                        depthData = nearestDist;
                                        depthDatas = BitConverter.GetBytes(depthData);

                                        for (int i = 0; i < 1; ++i)
                                        {
                                            depthDatas = BitConverter.GetBytes((ushort)(nearestDist));

                                            colorData[i * 3 + 0] = depthDatas[0];
                                            colorData[i * 3 + 1] = depthDatas[1];
                                        }

                                        Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format48bppRgb);
                                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                        int rowSize = Math.Abs(bmpData.Stride);
                                        IntPtr bmpScan0 = bmpData.Scan0;
                                        for (int y = 0; y < bitmap.Height; y++)
                                        {
                                            Marshal.Copy(colorData, y * bitmap.Width * 3, IntPtr.Add(bmpScan0, y * rowSize), bitmap.Width * 3);
                                        }
                                        bitmap.UnlockBits(bmpData);
                                        bitmap.Save(savePath + "_depth.png", ImageFormat.Png);
                                        bitmap.Dispose();
                                    }
                                }
#endif

#if SAVE_PNG
                                if (hasPalette)
                                {
                                    byte[] colorData = new byte[frameWidth * frameHeight * 2];
                                    uint colorIndex = baseOffset + dataPos;
                                    for (int i = 0; i < alphaMask.Length; ++i)
                                    {
                                        if (alphaMask[i] == 0x00)
                                        {
                                            colorData[i * 2] = 0x1f;
                                            colorData[i * 2 + 1] = 0xf8;
                                        }
                                        else
                                        {
                                            if (baseOffset == 0)
                                            {
                                                colorData[i * 2] = 0x00;
                                                colorData[i * 2 + 1] = 0x00;
                                            }
                                            else
                                            {
                                                colorData[i * 2] = br.buffer[14 + br.buffer[colorIndex] * 2];
                                                colorData[i * 2 + 1] = br.buffer[14 + br.buffer[colorIndex] * 2 + 1];
                                                colorIndex++;
                                            }
                                        }
                                    }

                                    Bitmap bitmap = new Bitmap(frameWidth, frameHeight, PixelFormat.Format16bppRgb565);
                                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    int rowSize = Math.Abs(bmpData.Stride);
                                    IntPtr bmpScan0 = bmpData.Scan0;
                                    for (int y = 0; y < frameHeight; y++)
                                    {
                                        Marshal.Copy(colorData, y * frameWidth * 2, IntPtr.Add(bmpScan0, y * rowSize), frameWidth * 2);
                                    }
                                    bitmap.UnlockBits(bmpData);

                                    //bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                                    //bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    //rowSize = Math.Abs(bmpData.Stride);
                                    //bmpScan0 = bmpData.Scan0;
                                    //for (int y = 0; y < bitmap.Height; y++)
                                    //{
                                    //    IntPtr rowPtr = IntPtr.Add(bmpScan0, y * rowSize);

                                    //    for (int x = 0; x < bitmap.Width; ++x)
                                    //    {
                                    //        if (Marshal.ReadInt32(rowPtr) == Color.Magenta.ToArgb())
                                    //        {
                                    //            Marshal.WriteInt32(rowPtr, Color.Transparent.ToArgb());
                                    //        }

                                    //        rowPtr = IntPtr.Add(rowPtr, 4);
                                    //    }
                                    //}
                                    //bitmap.UnlockBits(bmpData);

                                    //bitmap.Save(savePath, ImageFormat.Png);
                                    bitmap.Save(savePath, ImageFormat.Bmp);
                                    bitmap.Dispose();
                                }
                                else
                                {
                                    byte[] colorData = new byte[frameWidth * frameHeight * 2];
                                    uint colorIndex = baseOffset + dataPos;
                                    for (int i = 0; i < alphaMask.Length; ++i)
                                    {
                                        if (alphaMask[i] == 0x00)
                                        {
                                            //colorData[i * 2] = 0x1f; //k333123
                                            //colorData[i * 2 + 1] = 0xf8; //k333123
                                            colorData[i * 2] = 0x00;
                                            colorData[i * 2 + 1] = 0x00;
                                        }
                                        else
                                        {
                                            if (baseOffset == 0)
                                            {
                                                colorData[i * 2] = 0x00;
                                                colorData[i * 2 + 1] = 0x00;
                                            }
                                            else
                                            {
                                                colorData[i * 2] = br.buffer[colorIndex++];
                                                colorData[i * 2 + 1] = br.buffer[colorIndex++];
                                            }
                                        }
                                    }

                                    Bitmap bitmap = new Bitmap(frameWidth, frameHeight, PixelFormat.Format16bppRgb565);
                                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    int rowSize = Math.Abs(bmpData.Stride);
                                    IntPtr bmpScan0 = bmpData.Scan0;
                                    for (int y = 0; y < frameHeight; y++)
                                    {
                                        Marshal.Copy(colorData, y * frameWidth * 2, IntPtr.Add(bmpScan0, y * rowSize), frameWidth * 2);
                                    }
                                    bitmap.UnlockBits(bmpData);

                                    //bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                                    //bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    //rowSize = Math.Abs(bmpData.Stride);
                                    //bmpScan0 = bmpData.Scan0;
                                    //for (int y = 0; y < bitmap.Height; y++)
                                    //{
                                    //    IntPtr rowPtr = IntPtr.Add(bmpScan0, y * rowSize);

                                    //    for (int x = 0; x < bitmap.Width; ++x)
                                    //    {
                                    //        if (Marshal.ReadInt32(rowPtr) == Color.Magenta.ToArgb())
                                    //        {
                                    //            Marshal.WriteInt32(rowPtr, Color.Transparent.ToArgb());
                                    //        }

                                    //        rowPtr = IntPtr.Add(rowPtr, 4);
                                    //    }
                                    //}
                                    //bitmap.UnlockBits(bmpData);

                                    //bitmap.Save(savePath, ImageFormat.Png);
                                    bitmap.Save(savePath, ImageFormat.Bmp);
                                    bitmap.Dispose();
                                }
                            }
                            else
                            {
                                if (hasPalette)
                                {

                                    byte[] colorData = new byte[frameWidth * frameHeight * 2];

                                    for (int i = 0; i < baseLength; ++i)
                                    {
                                        int colorDataIndex = i * 2;

                                        int colorIdIndex = br.buffer[baseOffset + dataPos + i] * 2;

                                        colorData[colorDataIndex] = br.buffer[14 + colorIdIndex];
                                        colorData[colorDataIndex + 1] = br.buffer[14 + colorIdIndex + 1];
                                    }

                                    Bitmap bitmap = new Bitmap(frameWidth, frameHeight, PixelFormat.Format16bppRgb565);
                                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    int rowSize = Math.Abs(bmpData.Stride);
                                    IntPtr bmpScan0 = bmpData.Scan0;
                                    for (int y = 0; y < frameHeight; y++)
                                    {
                                        Marshal.Copy(colorData, y * frameWidth * 2, IntPtr.Add(bmpScan0, y * rowSize), frameWidth * 2);
                                    }
                                    bitmap.UnlockBits(bmpData);

                                    //bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                                    //bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    //rowSize = Math.Abs(bmpData.Stride);
                                    //bmpScan0 = bmpData.Scan0;
                                    //for (int y = 0; y < bitmap.Height; y++)
                                    //{
                                    //    IntPtr rowPtr = IntPtr.Add(bmpScan0, y * rowSize);

                                    //    for (int x = 0; x < bitmap.Width; ++x)
                                    //    {
                                    //        if (Marshal.ReadInt32(rowPtr) == Color.Magenta.ToArgb())
                                    //        {
                                    //            Marshal.WriteInt32(rowPtr, Color.Transparent.ToArgb());
                                    //        }

                                    //        rowPtr = IntPtr.Add(rowPtr, 4);
                                    //    }
                                    //}
                                    //bitmap.UnlockBits(bmpData);

                                    //bitmap.Save(savePath, ImageFormat.Png);
                                    bitmap.Save(savePath, ImageFormat.Bmp);
                                    bitmap.Dispose();
                                }
                                else
                                {
                                    Bitmap bitmap = new Bitmap(frameWidth, frameHeight, PixelFormat.Format16bppRgb565);
                                    BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    int rowSize = Math.Abs(bmpData.Stride);
                                    IntPtr bmpScan0 = bmpData.Scan0;
                                    for (int y = 0; y < frameHeight; y++)
                                    {
                                        Marshal.Copy(br.buffer, (int)(baseOffset + dataPos) + y * frameWidth * 2, IntPtr.Add(bmpScan0, y * rowSize), frameWidth * 2);
                                    }
                                    bitmap.UnlockBits(bmpData);

                                    //bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);
                                    //bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                    //rowSize = Math.Abs(bmpData.Stride);
                                    //bmpScan0 = bmpData.Scan0;
                                    //for (int y = 0; y < bitmap.Height; y++)
                                    //{
                                    //    IntPtr rowPtr = IntPtr.Add(bmpScan0, y * rowSize);

                                    //    for (int x = 0; x < bitmap.Width; ++x)
                                    //    {
                                    //        if (Marshal.ReadInt32(rowPtr) == Color.Magenta.ToArgb())
                                    //        {
                                    //            Marshal.WriteInt32(rowPtr, Color.Transparent.ToArgb());
                                    //        }

                                    //        rowPtr = IntPtr.Add(rowPtr, 4);
                                    //    }
                                    //}
                                    //bitmap.UnlockBits(bmpData);

                                    //bitmap.Save(savePath, ImageFormat.Png);
                                    bitmap.Save(savePath, ImageFormat.Bmp);
                                    bitmap.Dispose();
                                }
                            }
                        }
#endif
                    }

                    DWriteLine($"{br.index:X8} frameCount {frameCount}");
                    DWriteLine($"{br.index:X8} End image");


                    {
                        actionCount = 0;
                        br.Read(out actionCount);                                                                               // actionCount.w

                        DWriteLine($"{br.index:X8} actionCount {actionCount}");


                        if (actionCount > 0)
                        {
#if SAVE_JSON
                            string savePath = $"{Path.Combine(output, Path.GetFileNameWithoutExtension(target))}_action_{imageIndex}.json";
                            string outputDirectory = Path.GetDirectoryName(savePath);

                            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(savePath));

                            if (directoryInfo.Exists == false)
                            {
                                directoryInfo.Create();
                            }

                            FileStream fileStream;
                            if (File.Exists(savePath))
                                fileStream = new FileStream(savePath, FileMode.Truncate);
                            else
                                fileStream = new FileStream(savePath, FileMode.OpenOrCreate);

                            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);

                            streamWriter.Write(
                                "{" +
                                "\n    \"actions\": [");
#endif
                            for (int actionIndex = 0; actionIndex < actionCount; ++actionIndex)                                 // for actionCount
                            {
                                uint actionSize = 0;
                                uint actionOffset = 0;

                                br.Read(out actionSize);                                                                        //   actionSize.l
                                if (actionSize != 0)                                                                            //   if actionSize > 0
                                    br.Read(out actionOffset);                                                                  //     actionOffset.l

                                byte[] time1 = new byte[6];
                                byte[] time2 = new byte[6];

#if SAVE_JSON
                                streamWriter.Write(
                                    $"\n        {{" +
                                    $"\n            \"actionSize\" : {actionSize}," +
                                    $"\n            \"actionOffset\" : {actionOffset}," +
                                    $"\n            \"times\" : [");
#endif
                                for (int timeIndex = 0; timeIndex <= 2; ++timeIndex)                                            //   for i=0 to 2
                                {
                                    br.Read(out time1[timeIndex * 2]);                                                          //     time1[i*2].b
                                    br.Read(out time1[timeIndex * 2 + 1]);                                                      //     time1[i*2+1].b
                                    br.Read(out time2[timeIndex * 2]);                                                          //     time2[i*2].b
                                    br.Read(out time2[timeIndex * 2 + 1]);                                                      //     time2[i*2+1].b

#if SAVE_JSON
                                    streamWriter.Write($"[{time1[timeIndex * 2]}, {time1[timeIndex * 2 + 1]}, {time2[timeIndex * 2]}, {time2[timeIndex * 2 + 1]}]");

                                    if (timeIndex != 2)
                                        streamWriter.Write(", ");
#endif
                                }

                                short actionElemCount = 0;
                                br.Read(out actionElemCount);                                                                   //   actionElemCount.w
#if SAVE_JSON
                                streamWriter.WriteLine("],");
                                streamWriter.WriteLine($"            \"actionElems\" : [");
#endif
                                for (int actionElemIndex = 0; actionElemIndex < actionElemCount; ++actionElemIndex)             //   for actionElemCount
                                {
                                    short frameIndex = 0;
                                    uint time = 0;
                                    uint actionElemLength = 0;
                                    int actionElemOffset = 0;

                                    br.Read(out frameIndex);                                                                    //     frameIndex.w
                                    br.Read(out time);                                                                          //     time.l
                                    br.Read(out actionElemLength);                                                              //     actionElemLength.l
                                    if (actionElemLength != 0)                                                                  //     if actionElemLength
                                        br.Read(out actionElemOffset);                                                          //       actionElemOffset.l

#if SAVE_JSON
                                    streamWriter.Write(
                                        $"                {{\n" +
                                        $"                    \"frameIndex\": {frameIndex},\n" +
                                        $"                    \"time\": {time},\n" +
                                        $"                    \"actionElem\": [\n");
#endif

                                    if (actionElemLength != 0)
                                    {
                                        ByteReader brActionElem = new ByteReader(br.buffer, actionElemOffset);

                                        if (actionElemLength < 10 && actionElemOffset > 0)
                                        {
                                            for (int i = 0; i < actionElemLength; ++i)
                                            {
                                                byte offsetX = 0;
                                                byte offsetY = 0;
                                                ushort unknown = 0;

                                                brActionElem.Read(out offsetX);
                                                brActionElem.Read(out offsetY);
                                                brActionElem.Read(out unknown);

#if SAVE_JSON
                                                streamWriter.Write($"                    {{\"offsetX\": {offsetX}, \"offsetY\": {offsetY}, \"unkown\": {unknown}}}");

                                                if (i != actionElemLength - 1)
                                                    streamWriter.WriteLine(", ");
                                                else
                                                    streamWriter.WriteLine("");
#endif
                                            }
                                        }
                                    }

#if SAVE_JSON
                                    streamWriter.Write(
                                        $"                    ]\n" +
                                        $"                }}");

                                    if (actionElemIndex != actionElemCount - 1)
                                        streamWriter.WriteLine(", ");
                                    else
                                        streamWriter.WriteLine("");
#endif
                                }
#if SAVE_JSON
                                streamWriter.WriteLine("            ]");
                                streamWriter.Write("        }");

                                if (actionIndex != actionCount - 1)
                                    streamWriter.WriteLine(",");
                                else
                                    streamWriter.WriteLine();
#endif
                            }

#if SAVE_JSON
                            streamWriter.WriteLine("    ]");
                            streamWriter.WriteLine("}");
                            streamWriter.Close();
                            streamWriter.Dispose();
                            fileStream.Close();
                            fileStream.Dispose();
#endif
                        }
                    }


                    DWriteLine($"{br.index:X8} End anim");

                    {
                        stateCount = 0;
                        br.Read(out stateCount);                                                                                // stateCount.w

                        DWriteLine($"{br.index:X8} stateCount {stateCount}");

                        for (int stateIndex = 0; stateIndex < stateCount; ++stateIndex)                                         // for stateCount
                        {
                            uint stateSize;
                            int stateOffset = 0;
                            ushort stateElemCount;

                            br.Read(out stateSize);                                                                             //   stateSize.l
                            if (stateSize != 0)                                                                                 //   if stateSize
                                br.Read(out stateOffset);                                                                       //     stateOffset.l
                            br.Read(out stateElemCount);                                                                        //   stateElemCount.w


                            DWriteLine($"{br.index:X8} : {stateIndex,3}, stateSize {stateSize}, stateOffset {stateOffset}, stateElemCount {stateElemCount}");

                            //br.index += stateOffset;


                            for (int stateElemIndex = 0; stateElemIndex < stateElemCount; ++stateElemIndex)                     //   for stateElemCount
                            {
                                byte isStateElemFrame;
                                byte unk1;
                                byte unk2;
                                ushort unk3;
                                int unk4 = 0;
                                uint stateElemSize;
                                uint stateElemOffset = 0;

                                br.Read(out isStateElemFrame);                                                                  //     isStateElemFrame.b
                                br.Read(out unk1);                                                                              //     unk1.b
                                br.Read(out unk2);                                                                              //     unk2.b
                                br.Read(out unk3);                                                                              //     unk3.w

                                if (isStateElemFrame != 0)                                                                      //     if isStateElemFrame
                                    br.Read(out unk4);                                                                          //       unk4.l (frameIndex?)

                                br.Read(out stateElemSize);                                                                     //     stateElemSize.l
                                if (stateElemSize != 0)                                                                         //     if stateElemSize
                                    br.Read(out stateElemOffset);                                                               //       stateElemOffset.l

                                //DEBUG
                                DWriteLine($"{br.index:X8} : isStateElemFrame {isStateElemFrame}, unk1 {unk1}, unk2 {unk2}, unk3 {unk3}, unk4 {unk4}, stateElemSize {stateElemSize}");
                            }
                        }
                    }

                    DWriteLine($"{br.index:X8} End state");

                    {
                        stateTransValueCount = 0;
                        br.Read(out stateTransValueCount);                                                                      // stateTransValueCount.w

                        DWriteLine($"{br.index:X8} stateTransValueCount {stateTransValueCount}");

                        for (int stateTransValueIndex = 0; stateTransValueIndex < stateTransValueCount; ++stateTransValueIndex) // for stateTransValueCount
                        {
                            ushort key1;
                            ushort key2;
                            ushort value;
                            uint stateTransValueSize;
                            int stateTransValueOffset = 0;

                            br.Read(out key1);                                                                                  //   key1.w
                            br.Read(out key2);                                                                                  //   key2.w
                            br.Read(out value);                                                                                 //   value.w
                            br.Read(out stateTransValueSize);                                                                   //   stateTransValueSize.l
                            if (stateTransValueSize != 0)                                                                       //   if stateTransValueSize
                                br.Read(out stateTransValueOffset);                                                             //     stateTransValueOffset.l

                            br.index += stateTransValueOffset;

                            //DEBUG
                            DWriteLine($"{br.index:X8} key1 {key1}, key2 {key2}, value {value}, stateTransValueSize {stateTransValueSize}, stateTransValueOffset {stateTransValueOffset}");
                        }
                    }

                    DWriteLine($"{br.index:X8} End stateTransValue");

                }
            }
        }
    }
}

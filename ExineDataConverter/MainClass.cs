using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace NewYPF
{
    class MainClass
    {
        public void Main(string[] args)
        {
            //AHM_0000.ypg
            //string[] filenames = new string[] { "PANEL0000.YPF" };
            string[] filenames = new string[] { "00000.map" };
            List<String> inputFileNames = new List<string>();

            if (args.Length > 0) filenames = args;
            
            
            //merge add
            if (args[0]=="-m")
            {
                if (Path.GetFileNameWithoutExtension(filenames[0]).Equals("*"))
                {
                    inputFileNames.AddRange(System.IO.Directory.GetFiles(".", filenames[1]));
                    Console.WriteLine("Scan File List");
                    //scan *.dat files
                    inputFileNames.AddRange(System.IO.Directory.GetFiles(".", filenames[0]));
                    foreach (var fileName in inputFileNames)
                    {
                        Console.WriteLine(fileName);
                    }
                }
                else
                {
                    foreach (var filename in filenames)
                    {
                        inputFileNames.Add(filename);
                        Console.WriteLine(filename);
                    }
                }
                
                //merge
                DirectoryInfo di = new DirectoryInfo(".\\YPF_OUT_MERGE");
                if (!di.Exists) di.Create();
                MLibraryV2 mergeMFile = new MLibraryV2(".\\YPF_OUT_MERGE\\merge.lib");
                foreach (var filename in inputFileNames)
                {
                    MLibraryV2 mlibFile = new MLibraryV2(filename);
                    /*
                    for(int i=0;i<mlibFile.Count;i++)
                    {
                        //mergeMFile.AddImage(mlibFile.GetMImage(i).Image, mlibFile.GetMImage(i).MaskImage, mlibFile.GetMImage(i).X, mlibFile.GetMImage(i).Y);
                    }*/
                }
                mergeMFile.Save();
            }

            
            if (!File.Exists(filenames[0]))
            {
                if (Path.GetFileNameWithoutExtension(filenames[0]).Equals("*"))
                {
                    Console.WriteLine("Scan File List");
                    //scan *.dat files 
                    foreach(var fileName in System.IO.Directory.GetFiles(".", filenames[0]))
                    {
                        Console.WriteLine(fileName.Replace(".\\", ""));
                        inputFileNames.Add(fileName.Replace(".\\", ""));
                    }
                } 
            }

            else
            {
                foreach (var filename in filenames)
                {
                    inputFileNames.Add(filename);
                }
            }

            foreach (var filename in inputFileNames)
            {
                Console.WriteLine(filename);

                if (!File.Exists(filename))
                {  
                    Console.WriteLine(filename + "Is Not Exist!");
                    continue;
                }
                try
                {
                    switch (GetExt(filename).ToLower())
                    {
                        case ".ypf": ConvertYpfToRGBA(filename).ConvertToLib(filename); break;
                        case ".map": ConvertMapToM2Map(filename); break;//Todo : Apply Shadow At Map Object
                        case ".png": ConvertPngToLib(filename, -48, +24); break;//To One Lib
                        case ".dat": ExtractDat(filename); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }
            }
            Console.WriteLine("FIN!");

        }

        string GetExt(string filename)
        {
            string ext = Path.GetFileName(filename);
            ext = ext.Replace(Path.GetFileNameWithoutExtension(filename), "");
            Console.WriteLine("Ext : " + ext);
            return ext;
        }

        void ExtractDat(string filename)
        {
            byte[] datas = ReadByteFile(filename);
            DATFormat datFormat = new DATFormat(datas);
            datFormat.Save();
        }

        void ConvertPngToLib(string filename, short offsetX = 0, short offsetY = 0)
        {
            DirectoryInfo di = new DirectoryInfo(".\\PNG_OUT");
            if (!di.Exists) di.Create();

            MLibraryV2 mLibraryV2 = new MLibraryV2(".\\PNG_OUT\\"+filename + ".lib");
            Bitmap bitmap = new Bitmap(filename);
            mLibraryV2.AddImage(bitmap, offsetX, offsetY);
            mLibraryV2.Save(); 
        }

        void ConvertMapToM2Map(string filename)
        {
            Console.WriteLine("Start ConvertMapToM2Map. Filename:" + filename);
            byte[] datas = ReadByteFile(filename);
            MAPFormat mapFormat = new MAPFormat(datas,filename);
            mapFormat.ConvertToM2MAP(filename);

            Console.WriteLine(datas.Length);
        }

        YPFFormat ConvertYpfToRGBA(string filename)
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

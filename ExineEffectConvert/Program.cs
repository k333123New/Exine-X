using System;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filename = "Effect00.ypf";

            Console.WriteLine("Hello World!");

            byte[] datas = ReadByteFile(filename);

            YPFFormat ypfFormat = new YPFFormat(datas);
            ypfFormat.SaveFile("output.png");
        }

        static public byte[] ReadByteFile(string filePath)
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

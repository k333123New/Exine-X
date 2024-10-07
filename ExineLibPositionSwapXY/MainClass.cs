using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExineLibPositionSwapXY
{
    class MainClass
    {
        public void Main(string[] args)
        {
            Console.WriteLine("Start");
            DirectoryInfo di = new DirectoryInfo(".\\swapOffsetXY");

            if (!di.Exists) di.Create();
            foreach (var fileName in System.IO.Directory.GetFiles(".", "*.lib"))
            {
                Console.WriteLine($"{fileName}");

                MLibraryV2LocationMgr mLibraryV2LocationMgr = new MLibraryV2LocationMgr(fileName);
                mLibraryV2LocationMgr.SaveSwapXY(".\\swapOffsetXY\\"+ fileName);
                mLibraryV2LocationMgr.Close();
            }
             
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
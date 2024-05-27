using System;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleServerTest // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Packet.IsServer = true;
            
            Settings.Load();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SMain());

            Settings.Save();
        }
    }
}
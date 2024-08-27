using System;
using System.Net.NetworkInformation;
using System.Text;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static List<ExFrameSetStringWithName> exFrameSetStringWithNames = new List<ExFrameSetStringWithName>();
        static void Main(string[] args)
        {
            var directoryInfo = new DirectoryInfo("data");
            foreach(var file in directoryInfo.GetFiles())
            {
                string temp = ConvertTxtToFrameStructure(File.ReadAllText(file.FullName));
                bool isDuplication = false;
                foreach(var item in exFrameSetStringWithNames)
                {
                    if(item.exFrameSetString==temp)
                    {
                        isDuplication= true;
                        item.mobNames.Add(file.Name.Replace(".ypf.txt", ""));
                        break;
                    }
                }
                if(!isDuplication)
                {
                    exFrameSetStringWithNames.Add(new ExFrameSetStringWithName(file.Name.Replace(".ypf.txt",""), temp));
                }
            }

            //final
            for (int i= 0 ; i < exFrameSetStringWithNames.Count;i++)
            {
                Console.Write("//");
                foreach(var mobName in exFrameSetStringWithNames[i].mobNames)
                {
                    Console.Write(mobName + " ");
                }
                Console.WriteLine();
                Console.WriteLine("ExMonsterFrameSet["+i+"] = new FrameSet");
                Console.WriteLine(exFrameSetStringWithNames[i].exFrameSetString);
            }
        }

        static string ConvertTxtToFrameStructure(string text) 
        {
            var stringBuilder = new StringBuilder();
            var exStandingFrameSet = new ExFrameSet();
            var exWalkingFrameSet = new ExFrameSet();
            var exAttack1FrameSet = new ExFrameSet();
            var exStruckFrameSet = new ExFrameSet();
            var exDieFrameSet = new ExFrameSet();
            var exDeadFrameSet = new ExFrameSet();
            var exReviveFrameSet = new ExFrameSet();

            //Console.WriteLine(text.Length);
            var textLine =  text.Split('\n');
            for(uint i=0;i<textLine.Length;i++)
            {
                # region ExAction.Standing
                if (textLine[i].IndexOf("ONEHAND_STAND:UP:0")!=-1)
                {
                    exStandingFrameSet.startIdx = i;
                    exStandingFrameSet.skipCount = 0;
                    exStandingFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("ONEHAND_STAND:UPRIGHT:0") != -1)
                {
                    exStandingFrameSet.count = i  - exStandingFrameSet.startIdx;
                }
                #endregion ExAction.Standing

                # region ExAction.Walking
                if (textLine[i].IndexOf("ONEHAND_RUN_LEFT:UP:0") != -1)
                {
                    exWalkingFrameSet.startIdx = i;
                    exWalkingFrameSet.skipCount = 0;
                    exWalkingFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_RUN_LEFT:UPRIGHT:0") != -1)
                {
                    exWalkingFrameSet.count = i - exWalkingFrameSet.startIdx;
                }
                #endregion ExAction.Walking

                #region ExAction.Attack1
                if (textLine[i].IndexOf("ONEHAND_ATTACK2:UP:0") != -1)
                {
                    exAttack1FrameSet.startIdx = i;
                    exAttack1FrameSet.skipCount = 0;
                    exAttack1FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_ATTACK2:UPRIGHT:0") != -1)
                {
                    exAttack1FrameSet.count = i - exAttack1FrameSet.startIdx;
                }
                #endregion ExAction.Attack1

                #region ExAction.Struck
                if (textLine[i].IndexOf("ONEHAND_STUCK:UP:0") != -1)
                {
                    exStruckFrameSet.startIdx = i;
                    exStruckFrameSet.skipCount = 0;
                    exStruckFrameSet.delay = 200;
                }
                if (textLine[i].IndexOf("ONEHAND_STUCK:UPRIGHT:0") != -1)
                {
                    exStruckFrameSet.count = i - exStruckFrameSet.startIdx;
                }
                # endregion ExAction.Struck

                #region ExAction.Die Dead Revive
                if (textLine[i].IndexOf("DIE:UP:0") != -1)
                {
                    exDieFrameSet.startIdx = i;
                    exDieFrameSet.skipCount = 0;
                    exDieFrameSet.delay = 100;

                    exReviveFrameSet.startIdx = i;
                    exReviveFrameSet.skipCount = 0;
                    exReviveFrameSet.delay = 100;
                    exReviveFrameSet.isReverse = true;

                }
                if (textLine[i].IndexOf("DIE:UPRIGHT:0") != -1)
                {
                    exDieFrameSet.count = i - exDieFrameSet.startIdx;
                    exReviveFrameSet.count = i - exDieFrameSet.startIdx;

                    exDeadFrameSet.startIdx = i - 1;
                    exDeadFrameSet.count = 1;
                    exDeadFrameSet.skipCount = exDieFrameSet.count - 1;
                    exDeadFrameSet.delay = 1000;

                }
                # endregion ExAction.Die
            } 

            stringBuilder.AppendLine("\t\t{");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Standing, " + exStandingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Walking, " + exWalkingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Attack1, " + exAttack1FrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Struck, " + exStruckFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Die, " + exDieFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Dead, " + exDeadFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.Revive, " + exReviveFrameSet.ToString() + "}");
            stringBuilder.AppendLine("\t\t};");


            return stringBuilder.ToString();
        }
    }

    public class ExFrameSetStringWithName
    {
        public List<string> mobNames = new List<string>();
        public string exFrameSetString = "";
        public ExFrameSetStringWithName(string mobName, string exFrameSetString)
        {
            this.mobNames.Add(mobName);
            this.exFrameSetString = exFrameSetString;
        }

    }
    public class ExFrameSet
    {
        public uint startIdx = 0;
        public uint count = 0;
        public uint skipCount = 0;
        public uint delay = 0;
        public bool isReverse = false;

        override
        public string ToString()
        {
            string result = "new Frame(" + startIdx + ", " + count + ", " + skipCount + ", " + delay+")";

            if (isReverse) result += " { Reverse = true }";
            return result;
        }
    }
}


/*
 * 
 * 
 * ExDefaultMonster = new FrameSet
            {
                { ExAction.Standing, new Frame(0, 4, 0, 500) },
                { ExAction.Walking, new Frame(128, 3, 0, 100) },
                { ExAction.Attack1, new Frame(288, 6, 0, 100) },
                { ExAction.Struck, new Frame(216, 1, 0, 200) },
                { ExAction.Die, new Frame(240, 6, 0, 100) },
                { ExAction.Dead, new Frame(245, 1, 5, 1000) },
                { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true } }
            };

 *  ExDefaultMonster = new FrameSet

 {

     { ExAction.Standing, new Frame(0, 4, 0, 500) },

     { ExAction.Walking, new Frame(128, 3, 0, 100) },

     { ExAction.Attack1, new Frame(288, 6, 0, 100) },

     { ExAction.Struck, new Frame(216, 1, 0, 200) },

     { ExAction.Die, new Frame(240, 6, 0, 100) },

     { ExAction.Dead, new Frame(245, 1, 5, 1000) },

     { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true } }

 };



 ExDefaultMonster = new FrameSet

 {

     { ExAction.Standing, new Frame(ONEHAND_STAND:UP:0(idx), ONEHAND_STAND:UP(count), 0, 500) },

     { ExAction.Walking, new Frame(ONEHAND_RUN_LEFT:UP:0(idx), ONEHAND_RUN_LEFT:UP(count), 0, 100) },

     { ExAction.Attack1, new Frame(ONEHAND_ATTACK2:UP:0(idx), ONEHAND_ATTACK2:UP(count), 0, 100) },

     { ExAction.Struck, new Frame(ONEHAND_STUCK:UP:0(idx), ONEHAND_STUCK:UP(count), 0, 200) },

     { ExAction.Die, new Frame(DIE:UP:0(idx), DIE:UP:UP(count), 0, 100) },

     { ExAction.Dead, new Frame(DIE:UP:0(idx)+DIE:UP:UP(count)-1, 1, DIE:UP:UP(count)-1, 1000) },

     { ExAction.Revive, new Frame(DIE:UP:0(idx), DIE:UP:UP(count), 0, 100) { Reverse = true } }

 };
 */
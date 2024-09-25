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
            //var directoryInfo = new DirectoryInfo("data");//for mob
            var directoryInfo = new DirectoryInfo("data1"); //for human
            foreach (var file in directoryInfo.GetFiles())
            {
                //string temp = ConvertTxtToFrameStructure(File.ReadAllText(file.FullName));//for mob
                string temp = ConvertTxtToFrameStructure1(File.ReadAllText(file.FullName));
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
        static string ConvertTxtToFrameStructure1(string text)
        {
            var stringBuilder = new StringBuilder();
            var exPeaceModeStandFrameSet = new ExFrameSet();
            var exPeaceModeStandWaitFrameSet = new ExFrameSet();

            var exOnehandStandingFrameSet = new ExFrameSet();
            var exTwohandStandingFrameSet = new ExFrameSet();
            var exBowhandStandingFrameSet = new ExFrameSet();

            var exPeaceModeWalkingFrameSet = new ExFrameSet();
            var exOnehandWalkingFrameSet = new ExFrameSet();
            var exTwohandWalkingFrameSet = new ExFrameSet();
            var exBowhandWalkingFrameSet = new ExFrameSet();

            var exPeaceModeRunFrameSet = new ExFrameSet();
            var exOnehandRunFrameSet = new ExFrameSet();
            var exTwohandRunFrameSet = new ExFrameSet();
            var exBowhandRunFrameSet = new ExFrameSet();

            var exPeaceModeSitdownFrameSet = new ExFrameSet();
            var exPeaceModeSitdownWaitFrameSet = new ExFrameSet();


            var exOnehandAttack1FrameSet = new ExFrameSet();
            var exOnehandAttack2FrameSet = new ExFrameSet();
            var exOnehandAttack3FrameSet = new ExFrameSet();
            var exTwohandAttack1FrameSet = new ExFrameSet();
            var exTwohandAttack2FrameSet = new ExFrameSet();
            var exTwohandAttack3FrameSet = new ExFrameSet();
            var exBowhandAttack1FrameSet = new ExFrameSet(); 

             
            var exStuckFrameSet = new ExFrameSet();

            var exDieFrameSet = new ExFrameSet();
            var exDeadFrameSet = new ExFrameSet();
            var exReviveFrameSet = new ExFrameSet();

            var exMagicCastFrameSet = new ExFrameSet();
            var exMagicAttackFrameSet = new ExFrameSet();


            var exPeaceModeWalkingRFrameSet = new ExFrameSet();
            var exOnehandWalkingRFrameSet = new ExFrameSet();
            var exTwohandWalkingRFrameSet = new ExFrameSet();
            var exBowhandWalkingRFrameSet = new ExFrameSet();

            var exPeaceModeRunRFrameSet = new ExFrameSet();
            var exOnehandRunRFrameSet = new ExFrameSet();
            var exTwohandRunRFrameSet = new ExFrameSet();
            var exBowhandRunRFrameSet = new ExFrameSet();

            var exPeaceModeStandUpFrameSet = new ExFrameSet(); 
           


            //Console.WriteLine(text.Length);
            var textLine = text.Split('\n');
            for (uint i = 0; i < textLine.Length; i++)
            {

                #region ExAction.PEACEMODE_STAND
                if (textLine[i].IndexOf("PEACEMODE_STAND:UP:0") != -1)
                {
                    exPeaceModeStandFrameSet.startIdx = i;
                    exPeaceModeStandFrameSet.skipCount = 0;
                    exPeaceModeStandFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("PEACEMODE_STAND:UPRIGHT:0") != -1)
                {
                    exPeaceModeStandFrameSet.count = i - exPeaceModeStandFrameSet.startIdx;
                }
                #endregion ExAction.PEACEMODE_STAND

                #region ExAction.PEACEMODE_STAND_WAIT
                if (textLine[i].IndexOf("PEACEMODE_STAND_WAIT:UP:0") != -1)
                {
                    exOnehandStandingFrameSet.startIdx = i;
                    exOnehandStandingFrameSet.skipCount = 0;
                    exOnehandStandingFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("PEACEMODE_STAND_WAIT:UPRIGHT:0") != -1)
                {
                    exOnehandStandingFrameSet.count = i - exOnehandStandingFrameSet.startIdx;
                }
                #endregion 

                #region ExAction.ONEHAND_STAND
                if (textLine[i].IndexOf("ONEHAND_STAND:UP:0") != -1)
                {
                    exOnehandStandingFrameSet.startIdx = i;
                    exOnehandStandingFrameSet.skipCount = 0;
                    exOnehandStandingFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("ONEHAND_STAND:UPRIGHT:0") != -1)
                {
                    exOnehandStandingFrameSet.count = i - exOnehandStandingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_STAND
                if (textLine[i].IndexOf("TWOHAND_STAND:UP:0") != -1)
                {
                    exTwohandStandingFrameSet.startIdx = i;
                    exTwohandStandingFrameSet.skipCount = 0;
                    exTwohandStandingFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("TWOHAND_STAND:UPRIGHT:0") != -1)
                {
                    exTwohandStandingFrameSet.count = i - exTwohandStandingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_STAND
                if (textLine[i].IndexOf("BOWHAND_STAND:UP:0") != -1)
                {
                    exBowhandStandingFrameSet.startIdx = i;
                    exBowhandStandingFrameSet.skipCount = 0;
                    exBowhandStandingFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("BOWHAND_STAND:UPRIGHT:0") != -1)
                {
                    exBowhandStandingFrameSet.count = i - exBowhandStandingFrameSet.startIdx;
                }
                #endregion 

                # region ExAction.PEACEMODE_WALK_LEFT
                if (textLine[i].IndexOf("PEACEMODE_WALK_LEFT:UP:0") != -1)
                {
                    exPeaceModeWalkingFrameSet.startIdx = i;
                    exPeaceModeWalkingFrameSet.skipCount = 0;
                    exPeaceModeWalkingFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("PEACEMODE_WALK_LEFT:UPRIGHT:0") != -1)
                {
                    exPeaceModeWalkingFrameSet.count = i - exPeaceModeWalkingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_WALK_LEFT
                if (textLine[i].IndexOf("ONEHAND_WALK_LEFT:UP:0") != -1)
                {
                    exOnehandWalkingFrameSet.startIdx = i;
                    exOnehandWalkingFrameSet.skipCount = 0;
                    exOnehandWalkingFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_WALK_LEFT:UPRIGHT:0") != -1)
                {
                    exOnehandWalkingFrameSet.count = i - exOnehandWalkingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_WALK_LEFT
                if (textLine[i].IndexOf("TWOHAND_WALK_LEFT:UP:0") != -1)
                {
                    exTwohandWalkingFrameSet.startIdx = i;
                    exTwohandWalkingFrameSet.skipCount = 0;
                    exTwohandWalkingFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_WALK_LEFT:UPRIGHT:0") != -1)
                {
                    exTwohandWalkingFrameSet.count = i - exTwohandWalkingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_WALK_LEFT
                if (textLine[i].IndexOf("BOWHAND_WALK_LEFT:UP:0") != -1)
                {
                    exBowhandWalkingFrameSet.startIdx = i;
                    exBowhandWalkingFrameSet.skipCount = 0;
                    exBowhandWalkingFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("BOWHAND_WALK_LEFT:UPRIGHT:0") != -1)
                {
                    exBowhandWalkingFrameSet.count = i - exBowhandWalkingFrameSet.startIdx;
                }
                #endregion

                #region ExAction.PEACEMODE_RUN_LEFT
                if (textLine[i].IndexOf("PEACEMODE_RUN_LEFT:UP:0") != -1)
                {
                    exPeaceModeRunFrameSet.startIdx = i;
                    exPeaceModeRunFrameSet.skipCount = 0;
                    exPeaceModeRunFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("PEACEMODE_RUN_LEFT:UPRIGHT:0") != -1)
                {
                    exPeaceModeRunFrameSet.count = i - exPeaceModeRunFrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_RUN_LEFT
                if (textLine[i].IndexOf("ONEHAND_RUN_LEFT:UP:0") != -1)
                {
                    exOnehandRunFrameSet.startIdx = i;
                    exOnehandRunFrameSet.skipCount = 0;
                    exOnehandRunFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_RUN_LEFT:UPRIGHT:0") != -1)
                {
                    exOnehandRunFrameSet.count = i - exOnehandRunFrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_RUN_LEFT
                if (textLine[i].IndexOf("TWOHAND_RUN_LEFT:UP:0") != -1)
                {
                    exTwohandRunFrameSet.startIdx = i;
                    exTwohandRunFrameSet.skipCount = 0;
                    exTwohandRunFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_RUN_LEFT:UPRIGHT:0") != -1)
                {
                    exTwohandRunFrameSet.count = i - exTwohandRunFrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_RUN_LEFT
                if (textLine[i].IndexOf("BOWHAND_RUN_LEFT:UP:0") != -1)
                {
                    exBowhandRunFrameSet.startIdx = i;
                    exBowhandRunFrameSet.skipCount = 0;
                    exBowhandRunFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("BOWHAND_RUN_LEFT:UPRIGHT:0") != -1)
                {
                    exBowhandRunFrameSet.count = i - exBowhandRunFrameSet.startIdx;
                }
                #endregion

                #region ExAction.PEACEMODE_SITDOWM
                if (textLine[i].IndexOf("PEACEMODE_SITDOWN:UP:0") != -1)
                {
                    exPeaceModeSitdownFrameSet.startIdx = i;
                    exPeaceModeSitdownFrameSet.skipCount = 0;
                    exPeaceModeSitdownFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("PEACEMODE_SITDOWN:UPRIGHT:0") != -1)
                {
                    exPeaceModeSitdownFrameSet.count = i - exPeaceModeSitdownFrameSet.startIdx;
                }
                #endregion ExAction.PEACEMODE_SITDOWM

                #region ExAction.PEACEMODE_SITDOWN_WAIT
                if (textLine[i].IndexOf("PEACEMODE_SITDOWN_WAIT:UP:0") != -1)
                {
                    exPeaceModeSitdownWaitFrameSet.startIdx = i;
                    exPeaceModeSitdownWaitFrameSet.skipCount = 0;
                    exPeaceModeSitdownWaitFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("PEACEMODE_SITDOWN_WAIT:UPRIGHT:0") != -1)
                {
                    exPeaceModeSitdownWaitFrameSet.count = i - exPeaceModeSitdownWaitFrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_STUCK
                if (textLine[i].IndexOf("ONEHAND_STUCK:UP:0") != -1)
                {
                    exStuckFrameSet.startIdx = i;
                    exStuckFrameSet.skipCount = 0;
                    exStuckFrameSet.delay = 200;
                }
                if (textLine[i].IndexOf("ONEHAND_STUCK:UPRIGHT:0") != -1)
                {
                    exStuckFrameSet.count = i - exStuckFrameSet.startIdx;
                }
                #endregion

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


                #region ExAction.ONEHAND_ATTACK1
                if (textLine[i].IndexOf("ONEHAND_ATTACK1:UP:0") != -1)
                {
                    exOnehandAttack1FrameSet.startIdx = i;
                    exOnehandAttack1FrameSet.skipCount = 0;
                    exOnehandAttack1FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_ATTACK1:UPRIGHT:0") != -1)
                {
                    exOnehandAttack1FrameSet.count = i - exOnehandAttack1FrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_ATTACK2
                if (textLine[i].IndexOf("ONEHAND_ATTACK2:UP:0") != -1)
                {
                    exOnehandAttack2FrameSet.startIdx = i;
                    exOnehandAttack2FrameSet.skipCount = 0;
                    exOnehandAttack2FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_ATTACK2:UPRIGHT:0") != -1)
                {
                    exOnehandAttack2FrameSet.count = i - exOnehandAttack2FrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_ATTACK3
                if (textLine[i].IndexOf("ONEHAND_ATTACK3:UP:0") != -1)
                {
                    exOnehandAttack3FrameSet.startIdx = i;
                    exOnehandAttack3FrameSet.skipCount = 0;
                    exOnehandAttack3FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_ATTACK3:UPRIGHT:0") != -1)
                {
                    exOnehandAttack3FrameSet.count = i - exOnehandAttack3FrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_ATTACK1
                if (textLine[i].IndexOf("TWOHAND_ATTACK1:UP:0") != -1)
                {
                    exTwohandAttack1FrameSet.startIdx = i;
                    exTwohandAttack1FrameSet.skipCount = 0;
                    exTwohandAttack1FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_ATTACK1:UPRIGHT:0") != -1)
                {
                    exTwohandAttack1FrameSet.count = i - exTwohandAttack1FrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_ATTACK2
                if (textLine[i].IndexOf("TWOHAND_ATTACK2:UP:0") != -1)
                {
                    exTwohandAttack2FrameSet.startIdx = i;
                    exTwohandAttack2FrameSet.skipCount = 0;
                    exTwohandAttack2FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_ATTACK2:UPRIGHT:0") != -1)
                {
                    exTwohandAttack2FrameSet.count = i - exTwohandAttack2FrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_ATTACK3
                if (textLine[i].IndexOf("TWOHAND_ATTACK3:UP:0") != -1)
                {
                    exTwohandAttack3FrameSet.startIdx = i;
                    exTwohandAttack3FrameSet.skipCount = 0;
                    exTwohandAttack3FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_ATTACK3:UPRIGHT:0") != -1)
                {
                    exTwohandAttack3FrameSet.count = i - exTwohandAttack3FrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_ATTACK1
                if (textLine[i].IndexOf("BOWHAND_ATTACK1:UP:0") != -1)
                {
                    exBowhandAttack1FrameSet.startIdx = i;
                    exBowhandAttack1FrameSet.skipCount = 0;
                    exBowhandAttack1FrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("BOWHAND_ATTACK1:UPRIGHT:0") != -1)
                {
                    exBowhandAttack1FrameSet.count = i - exBowhandAttack1FrameSet.startIdx;
                }
                #endregion

                #region ExAction.MAGIC_CAST
                if (textLine[i].IndexOf("MAGIC_CAST:UP:0") != -1)
                {
                    exMagicCastFrameSet.startIdx = i;
                    exMagicCastFrameSet.skipCount = 0;
                    exMagicCastFrameSet.delay = 200;
                }
                if (textLine[i].IndexOf("MAGIC_CAST:UPRIGHT:0") != -1)
                {
                    exMagicCastFrameSet.count = i - exMagicCastFrameSet.startIdx;
                }
                #endregion

                #region ExAction.MAGIC_ATTACK
                if (textLine[i].IndexOf("MAGIC_ATTACK:UP:0") != -1)
                {
                    exMagicAttackFrameSet.startIdx = i;
                    exMagicAttackFrameSet.skipCount = 0;
                    exMagicAttackFrameSet.delay = 200;
                }
                if (textLine[i].IndexOf("MAGIC_ATTACK:UPRIGHT:0") != -1)
                {
                    exMagicAttackFrameSet.count = i - exMagicAttackFrameSet.startIdx;
                }
                #endregion


                #region ExAction.PEACEMODE_WALK_RIGHT
                if (textLine[i].IndexOf("PEACEMODE_WALK_RIGHT:UP:0") != -1)
                {
                    exPeaceModeWalkingRFrameSet.startIdx = i;
                    exPeaceModeWalkingRFrameSet.skipCount = 0;
                    exPeaceModeWalkingRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("PEACEMODE_WALK_RIGHT:UPRIGHT:0") != -1)
                {
                    exPeaceModeWalkingRFrameSet.count = i - exPeaceModeWalkingRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_WALK_RIGHT
                if (textLine[i].IndexOf("ONEHAND_WALK_RIGHT:UP:0") != -1)
                {
                    exOnehandWalkingRFrameSet.startIdx = i;
                    exOnehandWalkingRFrameSet.skipCount = 0;
                    exOnehandWalkingRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_WALK_RIGHT:UPRIGHT:0") != -1)
                {
                    exOnehandWalkingRFrameSet.count = i - exOnehandWalkingRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_WALK_RIGHT
                if (textLine[i].IndexOf("TWOHAND_WALK_RIGHT:UP:0") != -1)
                {
                    exTwohandWalkingRFrameSet.startIdx = i;
                    exTwohandWalkingRFrameSet.skipCount = 0;
                    exTwohandWalkingRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_WALK_RIGHT:UPRIGHT:0") != -1)
                {
                    exTwohandWalkingRFrameSet.count = i - exTwohandWalkingRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_WALK_RIGHT
                if (textLine[i].IndexOf("BOWHAND_WALK_RIGHT:UP:0") != -1)
                {
                    exBowhandWalkingRFrameSet.startIdx = i;
                    exBowhandWalkingRFrameSet.skipCount = 0;
                    exBowhandWalkingRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("BOWHAND_WALK_RIGHT:UPRIGHT:0") != -1)
                {
                    exBowhandWalkingRFrameSet.count = i - exBowhandWalkingRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.PEACEMODE_RUN_RIGHT
                if (textLine[i].IndexOf("PEACEMODE_RUN_RIGHT:UP:0") != -1)
                {
                    exPeaceModeRunRFrameSet.startIdx = i;
                    exPeaceModeRunRFrameSet.skipCount = 0;
                    exPeaceModeRunRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("PEACEMODE_RUN_RIGHT:UPRIGHT:0") != -1)
                {
                    exPeaceModeRunRFrameSet.count = i - exPeaceModeRunRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.ONEHAND_RUN_RIGHT
                if (textLine[i].IndexOf("ONEHAND_RUN_RIGHT:UP:0") != -1)
                {
                    exOnehandRunRFrameSet.startIdx = i;
                    exOnehandRunRFrameSet.skipCount = 0;
                    exOnehandRunRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("ONEHAND_RUN_RIGHT:UPRIGHT:0") != -1)
                {
                    exOnehandRunRFrameSet.count = i - exOnehandRunRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.TWOHAND_RUN_RIGHT
                if (textLine[i].IndexOf("TWOHAND_RUN_RIGHT:UP:0") != -1)
                {
                    exTwohandRunRFrameSet.startIdx = i;
                    exTwohandRunRFrameSet.skipCount = 0;
                    exTwohandRunRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("TWOHAND_RUN_RIGHT:UPRIGHT:0") != -1)
                {
                    exTwohandRunRFrameSet.count = i - exTwohandRunRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.BOWHAND_RUN_RIGHT
                if (textLine[i].IndexOf("BOWHAND_RUN_RIGHT:UP:0") != -1)
                {
                    exBowhandRunRFrameSet.startIdx = i;
                    exBowhandRunRFrameSet.skipCount = 0;
                    exBowhandRunRFrameSet.delay = 100;
                }
                if (textLine[i].IndexOf("BOWHAND_RUN_RIGHT:UPRIGHT:0") != -1)
                {
                    exBowhandRunRFrameSet.count = i - exBowhandRunRFrameSet.startIdx;
                }
                #endregion

                #region ExAction.PEACEMODE_STANDUP
                if (textLine[i].IndexOf("PEACEMODE_STANDUP:UP:0") != -1)
                {
                    exPeaceModeStandUpFrameSet.startIdx = i;
                    exPeaceModeStandUpFrameSet.skipCount = 0;
                    exPeaceModeStandUpFrameSet.delay = 500;
                }
                if (textLine[i].IndexOf("PEACEMODE_STANDUP:UPRIGHT:0") != -1)
                {
                    exPeaceModeStandUpFrameSet.count = i - exPeaceModeStandUpFrameSet.startIdx;
                }
                #endregion ExAction.PEACEMODE_STANDUP
            }


           
            // ExPlayer.Add
            stringBuilder.AppendLine("\t\t{");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_STAND, " + exPeaceModeStandFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_STAND_WAIT, " + exPeaceModeStandWaitFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_STAND, " + exOnehandStandingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_STAND, " + exTwohandStandingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_STAND, " + exBowhandStandingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_WALK_LEFT, " + exPeaceModeWalkingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_WALK_LEFT, " + exOnehandWalkingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_WALK_LEFT, " + exTwohandWalkingFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_WALK_LEFT, " + exBowhandWalkingFrameSet.ToString() + "},"); 

            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_RUN_LEFT, " + exPeaceModeRunFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_RUN_LEFT, " + exOnehandRunFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_RUN_LEFT, " + exTwohandRunFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_RUN_LEFT, " + exBowhandRunFrameSet.ToString() + "},");


            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_SITDOWN, " + exPeaceModeSitdownFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_SITDOWN_WAIT, " + exPeaceModeSitdownWaitFrameSet.ToString() + "},");

             
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_STUCK, " + exStuckFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.DIE, " + exDieFrameSet.ToString() + "},");


            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_ATTACK1, " + exOnehandAttack1FrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_ATTACK2, " + exOnehandAttack2FrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_ATTACK3, " + exOnehandAttack3FrameSet.ToString() + "},");



            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_ATTACK1, " + exTwohandAttack1FrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_ATTACK2, " + exTwohandAttack2FrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_ATTACK3, " + exTwohandAttack3FrameSet.ToString() + "},");


            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_ATTACK1, " + exBowhandAttack1FrameSet.ToString() + "},");


            stringBuilder.AppendLine("\t\t\t\t{ ExAction.MAGIC_CAST, " + exMagicCastFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.MAGIC_ATTACK, " + exMagicAttackFrameSet.ToString() + "},");


            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_WALK_RIGHT, " + exPeaceModeWalkingRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_WALK_RIGHT, " + exOnehandWalkingRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_WALK_RIGHT, " + exTwohandWalkingRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_WALK_RIGHT, " + exBowhandWalkingRFrameSet.ToString() + "},");

            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_RUN_RIGHT, " + exPeaceModeRunRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.ONEHAND_RUN_RIGHT, " + exOnehandRunRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.TWOHAND_RUN_RIGHT, " + exTwohandRunRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.BOWHAND_RUN_RIGHT, " + exBowhandRunRFrameSet.ToString() + "},");
            stringBuilder.AppendLine("\t\t\t\t{ ExAction.PEACEMODE_STANDUP, " + exPeaceModeStandUpFrameSet.ToString() + "},");

            /*   

            
             
           var exDeadFrameSet = new ExFrameSet();
           var exReviveFrameSet = new ExFrameSet();
             
              
            */
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
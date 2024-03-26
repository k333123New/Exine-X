using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace NewYPF
{
    class YPFFormat
    {
        public YPFHeader ypfHeader = new YPFHeader();
        public YPFImageSet[] ypfImageSets = null;

        public YPFFormat(byte[] datas)
        {

            Console.WriteLine("datas.Len:" + datas.Length);

            int idx = ypfHeader.FillData(datas);
            if (idx == -1) return;

            ypfImageSets = new YPFImageSet[ypfHeader.ImageSetCount];

            Console.WriteLine("ypfHeader.ImageSetCount:" + ypfHeader.ImageSetCount);

            for (int i = 0; i < ypfImageSets.Length; i++)
            {
                ypfImageSets[i] = new YPFImageSet();
                idx = ypfImageSets[i].FillData(datas, idx, ypfHeader.Version,
                                            (int)ypfHeader.DataPotision,
                                            ypfHeader.HasPalette,
                                            ypfHeader.Palatte
                                            );
                //ypfImageSets[i].SaveFile(filename+"_ImageSet_"+i.ToString());
            }
        }

        public void SaveFile(string filename)
        {
            if (ypfImageSets == null) return;

            for (int i = 0; i < ypfImageSets.Length; i++)
            {
                ypfImageSets[i].SaveFile(filename + "_ImageSet_" + i.ToString());
            }
        }
 
      

        //class for ExActionInfo
        class ExActionIndexMap
        {
            public string actionType = "";
            public string actionDirection = "";
            public int actionIdx = 0;
            public int frameIdx = 0;
            public ExActionIndexMap(int actionsIndex, int actionElementIndex, Action[] actions )
            {
                actionType = ExineActionIdxToActionTypeString(actionsIndex);
                actionDirection = ExineActionIdxToActionDirectionString(actionsIndex);
                actionIdx = actionElementIndex;
                frameIdx = actions[actionsIndex].actionElements[actionElementIndex].frameIdx;
            }
            public ExActionIndexMap(int actionsIndex, int actionElementIndex,int frameIdx)
            {
                actionType = ExineActionIdxToActionTypeString(actionsIndex);
                actionDirection = ExineActionIdxToActionDirectionString(actionsIndex);
                actionIdx = actionElementIndex;
                this.frameIdx = frameIdx;
            }

            override
            public string ToString()
            {
                string result = actionType + ":" + actionDirection + ":" + actionIdx + ":" + frameIdx;
                return result;
            }

            private  string ExineActionIdxToActionTypeString(int actionIdx)
            {
                if (actionIdx >= 0 && actionIdx < 8) return "PEACEMODE_STAND";
                else if (actionIdx >= 8 && actionIdx < 16) return "ONEHAND_STAND";
                else if (actionIdx >= 16 && actionIdx < 24) return "TWOHAND_STAND";
                else if (actionIdx >= 24 && actionIdx < 32) return "BOWHAND_STAND";
                else if (actionIdx >= 32 && actionIdx < 40) return "PEACEMODE_RUN_LEFT";
                else if (actionIdx >= 40 && actionIdx < 48) return "ONEHAND_RUN_LEFT";
                else if (actionIdx >= 48 && actionIdx < 56) return "TWOHAND_RUN_LEFT";
                else if (actionIdx >= 56 && actionIdx < 64) return "BOWHAND_RUN_LEFT";
                else if (actionIdx >= 64 && actionIdx < 72) return "PEACEMODE_SITDOWN";
                else if (actionIdx >= 72 && actionIdx < 80) return "ONEHAND_STUCK";
                else if (actionIdx >= 80 && actionIdx < 88) return "TWOHAND_STUCK";
                else if (actionIdx >= 88 && actionIdx < 96) return "BOWHAND_STUCK";
                else if (actionIdx >= 96 && actionIdx < 104) return "DIE";
                else if (actionIdx >= 104 && actionIdx < 112) return "ONEHAND_ATTACK1";
                else if (actionIdx >= 112 && actionIdx < 120) return "ONEHAND_ATTACK2";
                else if (actionIdx >= 120 && actionIdx < 128) return "ONEHAND_ATTACK3";
                else if (actionIdx >= 128 && actionIdx < 136) return "TWOHAND_ATTACK1";
                else if (actionIdx >= 136 && actionIdx < 144) return "TWOHAND_ATTACK2";
                else if (actionIdx >= 144 && actionIdx < 152) return "TWOHAND_ATTACK3";
                else if (actionIdx >= 152 && actionIdx < 160) return "BOWHAND_ATTACK1";
                else if (actionIdx == 160) return "MAGIC_CAST";
                else if (actionIdx >= 161 && actionIdx < 169) return "MAGIC_ATTACK";
                else if (actionIdx >= 169 && actionIdx < 177) return "PEACEMODE_RUN_RIGHT";
                else if (actionIdx >= 177 && actionIdx < 185) return "ONEHAND_RUN_RIGHT";
                else if (actionIdx >= 185 && actionIdx < 193) return "TWOHAND_RUN_RIGHT";
                else if (actionIdx >= 193 && actionIdx < 201) return "BOWHAND_RUN_RIGHT";
                else if (actionIdx == 201) return "UNKNOWN";
                else if (actionIdx >= 202 && actionIdx < 210) return "PEACEMODE_STANDUP";
                else return "";
            }

            private string ExineActionIdxToActionDirectionString(int actionIdx)
            {
                string result = "";
                int positionNum = 0;

                if (actionIdx >= 0 && actionIdx < 160)
                {
                    positionNum = actionIdx % 8;
                }
                else if (actionIdx >= 161 && actionIdx < 201)
                {
                    positionNum = (actionIdx - 1) % 8;
                }
                else if (actionIdx >= 202 && actionIdx < 210)
                {
                    positionNum = (actionIdx - 2) % 8;
                }

                switch (positionNum)
                {
                    case 0: result = "UP"; break;
                    case 1: result = "RIGHT"; break;
                    case 2: result = "DOWN"; break;
                    case 3: result = "LEFT"; break;
                    case 4: result = "UPRIGHT"; break;
                    case 5: result = "DOWNRIGHT"; break;
                    case 6: result = "DOWNLEFT"; break;
                    case 7: result = "UPLEFT"; break;
                }

                return result;
            }
        }

        

        public void ConvertToLib(string filename)
        {
            List<ExActionIndexMap> tempMap2 = new List<ExActionIndexMap>();//reidx

            List<ExActionIndexMap> actionIndexMaps = new List<ExActionIndexMap>();
            DirectoryInfo di = new DirectoryInfo(".\\YPF_OUT");
            if (!di.Exists) di.Create();

            Console.WriteLine("ConvertToLib start");
            bool hasMaskImg = false;
            if (ypfImageSets == null) return;
            //if(ypfImageSets.count==2) //has mask
            List<ImagesWithPosition> imagesWithPosition = new List<ImagesWithPosition>();
             

            for (int i = 0; i < ypfImageSets.Length; i++)
            {
                //if (ypfImageSets[i] == null) continue;//add 230530
                Console.WriteLine("Convert to lib, ypfImageSetIdx:" + i);
                var imageData = ypfImageSets[i].ConvertToLib();
                if (imageData != null)
                {
                    //imagesWithPosition.Add(ypfImageSets[i].ConvertToLib(filename + "_ImageSet_" + i.ToString()));
                    imagesWithPosition.Add(imageData);
                }

                //add k333123 save action info to text
                if (i == 0)
                {
                    var actions = ypfImageSets[i].actionInfo.actions;
                    for (int j = 0; j < actions.Length; j++)
                    {
                        for (int k = 0; k < actions[j].actionElements.Length; k++)
                        {
                            byte[] temp = new byte[8];
                            byte[] temp2 = new byte[8];
                            Buffer.BlockCopy(actions[j].time1, 0, temp, 0, actions[j].time1.Length);
                            Buffer.BlockCopy(actions[j].time2, 0, temp2, 0, actions[j].time2.Length);
                            long time1 = BitConverter.ToInt64(temp);
                            long time2 = BitConverter.ToInt64(temp2);
                            //save to list
                            Console.WriteLine("ActionIdx:{0} ActionElementsIdx:{1} FrameIdx:{2} time1:{3} time2:{4}",
                                j, k, actions[j].actionElements[k].frameIdx, time1, time2);
                            //Convert Number To ActionTypeString and DirectionString
                            /*
                            ExActionIndexMap 
                            actionIndexMaps.Add(ExineActionIdxToActionTypeString(j) + ":" + 
                                                  ExineActionIdxToActionDirectionString(j) + ":" +
                                                  k + ":" +
                                                  actions[j].actionElements[k].frameIdx);
                            */
                            actionIndexMaps.Add(new ExActionIndexMap(j, k,actions));
                        } 
                    }


                    //re-idx
                    List<ExActionIndexMap> tempMap = new List<ExActionIndexMap>();
                    
                    tempMap.Clear();
                    tempMap2.Clear();

                    string[] types = new string[28];
                    types[0]=  "PEACEMODE_STAND";
                    types[1] = "ONEHAND_STAND";
                    types[2] = "TWOHAND_STAND";
                    types[3] = "BOWHAND_STAND";
                    types[4] = "PEACEMODE_RUN_LEFT";
                    types[5] = "ONEHAND_RUN_LEFT";
                    types[6] = "TWOHAND_RUN_LEFT";
                    types[7] = "BOWHAND_RUN_LEFT";
                    types[8] = "PEACEMODE_SITDOWN";
                    types[9] = "ONEHAND_STUCK";
                    types[10] = "TWOHAND_STUCK";
                    types[11] = "BOWHAND_STUCK";
                    types[12] = "DIE";
                    types[13] = "ONEHAND_ATTACK1";
                    types[14] = "ONEHAND_ATTACK2";
                    types[15] = "ONEHAND_ATTACK3";
                    types[16] = "TWOHAND_ATTACK1";
                    types[17] = "TWOHAND_ATTACK2";
                    types[18] = "TWOHAND_ATTACK3";
                    types[19] = "BOWHAND_ATTACK1";
                    types[20] = "MAGIC_CAST";
                    types[21] = "MAGIC_ATTACK";
                    types[22] = "PEACEMODE_RUN_RIGHT";
                    types[23] = "ONEHAND_RUN_RIGHT";
                    types[24] = "TWOHAND_RUN_RIGHT";
                    types[25] = "BOWHAND_RUN_RIGHT";
                    types[26] = "UNKNOWN";
                    types[27] = "PEACEMODE_STANDUP";

                    int pcStandStateLen = 20; //19 frames, 8 direction
                    int pcRunLeftStateLen = 4;
                    int pcSitdownStateLen = 5;
                    int pcDieStateLen = 8;
                    int pcRunRightStateLen = 4;
                    int pcStandUpStateLen = 5;
                    bool isExistPeaceModeFrame = false;


                    int oneHandStandStateLen = 6;
                    int oneHandRunLeftStateLen = 4;  
                    int oneHandStuckStateLen = 1;
                    int oneHandAtk1StateLen = 8;
                    int oneHandAtk2StateLen = 8;
                    int oneHandAtk3StateLen = 8;
                    int oneHandRunRightStateLen = 4;
                    bool isOneHandFrame = false;

                    int twoHandStandStateLen = 6;
                    int twoHandRunLeftStateLen = 4;
                    int twoHandStuckStateLen = 1;
                    int twoHandAtk1StateLen = 8;
                    int twoHandAtk2StateLen = 8;
                    int twoHandAtk3StateLen = 8;
                    int twoHandRunRightStateLen = 4;
                    bool isTwoHandFrame = false;

                    int bowHandStandStateLen = 6;
                    int bowHandRunLeftStateLen = 4;
                    int bowHandStuckStateLen = 1;
                    int bowHandAtk1StateLen = 8;
                    int bowHandRunRightStateLen = 4;
                    bool isBowHandFrame = false;

                    int magicCastStateLen = 8;
                    int magicAtk1StateLen = 1;
                    bool isMagicFrame = false;

                    int unknownStateLen = 1;
                    bool isUnknownFrame = false;

                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        if (actionIndexMaps[j].actionType == types[0])
                        {
                            isExistPeaceModeFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[4])
                        {

                            isExistPeaceModeFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[8])
                        {

                            isExistPeaceModeFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[12])
                        {

                            isExistPeaceModeFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[22])
                        {

                            isExistPeaceModeFrame = true;
                            break;
                        } 
                        else if (actionIndexMaps[j].actionType == types[27])
                        {

                            isExistPeaceModeFrame = true;
                            break;
                        }
                    }
                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        //OneHand Frame Check
                        if (actionIndexMaps[j].actionType == types[5])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[9])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[13])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[14])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[15])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[23])
                        {
                            isOneHandFrame = true;
                            break;
                        }
                    }
                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {

                        //TwoHand Frame Check
                        if (actionIndexMaps[j].actionType == types[6])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[10])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[16])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[17])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[18])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[24])
                        {
                            isTwoHandFrame = true;
                            break;
                        }
                    }
                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        //Bow Frame Check
                        if (actionIndexMaps[j].actionType == types[7])
                        {
                            isBowHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[11])
                        {
                            isBowHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[19])
                        {
                            isBowHandFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[25])
                        {
                            isBowHandFrame = true;
                            break;
                        }
                    }

                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        //Magic Frame Check
                        if (actionIndexMaps[j].actionType == types[20])
                        {
                            isMagicFrame = true;
                            break;
                        }
                        else if (actionIndexMaps[j].actionType == types[21])
                        {
                            isMagicFrame = true;
                            break;
                        } 
                    }

                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        //Unknown Frame Check
                        if (actionIndexMaps[j].actionType == types[26])
                        {
                            isUnknownFrame = true;
                            break;
                        } 
                    }

                    /*
                   * sudo code
                   * int pcStandStateLen = 19 * 8; //19 frames, 8 direction
                   * int pcRunLeftStateLen = 4 * 8;
                   * int pcSitdownStateLen = 5 * 8;
                   * int pcDieStateLen = 8 * 8;
                   * int pcRunRightStateLen = 4 * 8;
                   * int pcStandUpStateLen = 5 * 8;
                   * bool isPcMode=false
                   * for(int i=0;i<temp.lenth;i++)
                   * {
                   *  if(temp[i]=="pc")
                   *  {
                   *      isPcMode=true;
                   *      break;
                   *   }
                   *  }
                   *  
                   *  if(isPcMode)
                   *  {
                   *      for(int i=0;i<pcStandStateLen;i++)
                   *      {
                   *          add (pcStandState,0);
                   *      }
                   *      for(int i=0;i<pcRestStateLen;i++)
                   *      {
                   *          add (pcRestState,0);
                   *      }
                   *  }
                   */

                    #region not exist Peace

                    if (!isExistPeaceModeFrame)
                    {
                        //Console.WriteLine("No Peace Mode Frame!");
                        //Console.ReadLine();
                        //have to set start position

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcStandStateLen; j++)
                            {
                                //use only frameidx;
                                actionIndexMaps.Add(new ExActionIndexMap(0+ directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcRunLeftStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(32 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcSitdownStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(64 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcDieStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(96 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcRunRightStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(169 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < pcStandUpStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(202 + directionIdx, j, 0));
                            }
                        }

                        //Console.ReadLine();
                    }
                  
                    #endregion not exist Peace

                    //add k333123 240305
                    #region not exist ownhand 
                    if (!isOneHandFrame)
                    {  
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandStandStateLen; j++)
                            {
                                //use only frameidx;
                                actionIndexMaps.Add(new ExActionIndexMap(8 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandRunLeftStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(40 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandStuckStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(72 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandAtk1StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(104 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandAtk2StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(112 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandAtk3StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(120 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < oneHandRunRightStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(177 + directionIdx, j, 0));
                            }
                        }  
                    }
                    #endregion

                    //add k333123 240305
                    #region not exist twohand 
                    if (!isTwoHandFrame)
                    {
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandStandStateLen; j++)
                            {
                                //use only frameidx;
                                actionIndexMaps.Add(new ExActionIndexMap(16 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandRunLeftStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(48 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandStuckStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(80 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandAtk1StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(128 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandAtk2StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(136 + directionIdx, j, 0));
                            }
                        }
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandAtk3StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(144 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < twoHandRunRightStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(185 + directionIdx, j, 0));
                            }
                        }
                    }
                    #endregion

                    //add k333123 240305
                    #region not exist bowhand 
                    if (!isBowHandFrame)
                    {
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < bowHandStandStateLen; j++)
                            {
                                //use only frameidx;
                                actionIndexMaps.Add(new ExActionIndexMap(24 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < bowHandRunLeftStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(56 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < bowHandStuckStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(88 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < bowHandAtk1StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(152 + directionIdx, j, 0));
                            }
                        }
                       
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < bowHandRunRightStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(193 + directionIdx, j, 0));
                            }
                        }
                    }
                    #endregion
                    
                    //add k333123 240305
                    #region not exist magicFrame
                    if (!isMagicFrame)
                    { 
                        for (int directionIdx = 0; directionIdx < 1; directionIdx++)//Direction only up
                        {
                            for (int j = 0; j < magicCastStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(160 + directionIdx, j, 0));
                            }
                        }

                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j < magicAtk1StateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(161 + directionIdx, j, 0));
                            }
                        }
                         
                    }
                    #endregion

                    //add k333123 240305
                    #region not exist Unknown
                    if (!isUnknownFrame)
                    {
                        for (int directionIdx = 0; directionIdx < 8; directionIdx++)
                        {
                            for (int j = 0; j <unknownStateLen; j++)
                            {
                                actionIndexMaps.Add(new ExActionIndexMap(201 + directionIdx, j, 0));
                            }
                        } 
                    }
                    #endregion



                    #region ReIdx
                    for (int typesIdx = 0; typesIdx < types.Length; typesIdx++)
                    {
                        tempMap.Clear();
                        for (int j = 0; j < actionIndexMaps.Count; j++)
                        {
                            if(typesIdx==27)
                            {
                                Console.WriteLine("Check PC Standup typesIdx==27");
                                Console.WriteLine("actionIndexMaps[j].actionType:" + actionIndexMaps[j].actionType + " types[typesIdx]:" + types[typesIdx]);
                                //Console.ReadLine();
                            }
                            if (actionIndexMaps[j].actionType == types[typesIdx])
                            {
                                tempMap.Add(actionIndexMaps[j]);
                            }
                        }

                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "UP")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }

                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "UPRIGHT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "RIGHT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "DOWNRIGHT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "DOWN")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "DOWNLEFT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "LEFT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                        for (int j = 0; j < tempMap.Count; j++)
                        {
                            if (tempMap[j].actionDirection == "UPLEFT")
                            {
                                tempMap2.Add(tempMap[j]);
                            }
                        }
                    }
                    #endregion ReIdx


                   

                    /*
                    List<string> actionIndexMapTextList = new List<string>();
                    for (int j = 0; j < actionIndexMaps.Count; j++)
                    {
                        actionIndexMapTextList.Add(actionIndexMaps[j].ToString());
                        //System.IO.File.WriteAllText(filename + ".txt", actionIndexMaps[j].ToString());
                        //Console.ReadLine();
                    }*/


                    List<string> actionIndexMapTextList = new List<string>();
                    for (int j = 0; j < tempMap2.Count; j++)
                    {
                        actionIndexMapTextList.Add(tempMap2[j].ToString());
                    }

                    System.IO.File.WriteAllLines(filename + ".txt", actionIndexMapTextList);
                }
            }

            //ÄÄ\ #D .
            Console.WriteLine("imagesWithPosition.Count:" + imagesWithPosition.Count);


            if (imagesWithPosition.Count % 2== 0) //newChar
            {
                // Console.WriteLine("#1");
                //how to check mask image!
                //check [1] images colors

                Console.WriteLine("#imagesWithPosition[1].bitmaps.Count:" + imagesWithPosition[1].bitmaps.Count);
                hasMaskImg = true;
                for (int i = 0; i < imagesWithPosition[1].bitmaps.Count; i++)
                {
                    var img = imagesWithPosition[1].bitmaps[i];
                    if (img == null) continue;//231101
                    //if (img == null) img = new Bitmap(0,0);


                    for (int j = 0; j < img.Width; j++)
                    { 
                        for (int k = 0; k < img.Height; k++)
                        {
                            //Console.WriteLine("#4");
                            Color pixel = img.GetPixel(j, k);
                            //Console.WriteLine("pixel R:" + pixel.R + " G:" + pixel.G + " B:" + pixel.B + " A:" + pixel.A);
                            if (pixel.R != 192 || pixel.G != 96 || pixel.B != 16)
                            {
                                if (pixel.R == pixel.B && pixel.G == pixel.B) { }
                                else if (pixel.R == pixel.B && pixel.G == pixel.B + 4) { }
                                else
                                {
                                    Console.WriteLine("Has Mask false!!!");
                                    hasMaskImg = false;
                                    break;
                                }
                            }
                        }
                        if (hasMaskImg == false) break;
                    }
                    if (hasMaskImg == false) break;
                }
            }

            //real save lib file
            MLibraryV2 mLibraryV2 = new MLibraryV2(".\\YPF_OUT\\" + Path.GetFileNameWithoutExtension(filename) + ".lib");

            //temp for reidx
            List<ImageWithPosition> mLibraryV2DataTemps = new List<ImageWithPosition>();
            MLibraryV2 mLibraryV2ReIndex = new MLibraryV2(".\\YPF_OUT\\" + Path.GetFileNameWithoutExtension(filename) + "_reidx.lib");

            if (hasMaskImg)
            {
                Console.WriteLine("Has Mask Img");

                //imagesWithPosition.Count
                for (int idx = 0; idx < imagesWithPosition.Count / 2; idx++)
                {
                    for (int i = 0; i < imagesWithPosition[2*idx].bitmaps.Count; i++)
                    {
                        Console.WriteLine("imagesWithPosition[2*idx].bitmaps.Count:" + imagesWithPosition[2 * idx].bitmaps.Count);
                        var img = imagesWithPosition[2 * idx].bitmaps[i];
                        var x = (short)imagesWithPosition[2 * idx].xVals[i];
                        var y = (short)imagesWithPosition[2 * idx].yVals[i];

                        var maskImg = imagesWithPosition[2 * idx+1].bitmaps[i];
                        //if (maskImg == null) continue;//231101
                        if (maskImg == null)
                        {
                            mLibraryV2.AddImage(img, x, y);
                            mLibraryV2DataTemps.Add(new ImageWithPosition(img, null, x, y)); //add k333123
                            continue;
                        }

                        var maskX = (short)imagesWithPosition[2 * idx + 1].xVals[i];
                        var maskY = (short)imagesWithPosition[2 * idx + 1].yVals[i];


                        //!!! potision x <-> y!

                        //draw maskImg to canvase
                        //Console.WriteLine("Create Canvas");
                        Bitmap newMaskImg = new Bitmap(img);
                        for (int j = 0; j < newMaskImg.Height; j++)
                        {
                            for (int k = 0; k < newMaskImg.Width; k++)
                            {
                                newMaskImg.SetPixel(k, j, Color.Transparent);
                            }
                        }

                        //Console.WriteLine("Create newMask");
                        int loopX = 0;
                        int loopY = 0;
                        for (int newMaskY = (maskX - x); newMaskY < (maskX - x) + maskImg.Height; newMaskY++)
                        {
                            for (int newMaskX = (maskY - y); newMaskX < (maskY - y) + maskImg.Width; newMaskX++)
                            {
                                //Console.WriteLine("newMaskX:" + newMaskX + " newMaskY:" + newMaskY + " loopX:" + loopX + " loopY:" + loopY);
                                //newMaskImg.SetPixel(newMaskX, newMaskY, maskImg.GetPixel(loopX, loopY)); //color to gray?
                                //Console.WriteLine("MaskImgcolor R:" + maskImg.GetPixel(loopX, loopY).R + " G:" + maskImg.GetPixel(loopX, loopY).G + " B:" + maskImg.GetPixel(loopX, loopY).B + " A:" + maskImg.GetPixel(loopX, loopY).A);

                                if (newMaskX < 0) { loopX = 0; continue; }//231101
                                if (newMaskY < 0) { loopY = 0; continue; }//231101
                                  
                                Color maskPixel = maskImg.GetPixel(loopX, loopY);
                                int brightness = (int)(0.299 * maskPixel.R + 0.587 * maskPixel.G + 0.114 * maskPixel.B);
                                Color maskGrayPixel = Color.FromArgb(maskPixel.A, brightness, brightness, brightness);
                                try
                                {
                                    newMaskImg.SetPixel(newMaskX, newMaskY, maskGrayPixel); //color to gray?
                                }
                                catch(Exception)
                                {
                                    continue;
                                }
                                loopX++;
                            }
                            loopX = 0;
                            loopY++;
                        }
                        mLibraryV2.AddImage(img, newMaskImg, x, y);
                        mLibraryV2DataTemps.Add(new ImageWithPosition(img, newMaskImg, x, y));//add k333123
                        Console.Write(".");
                    }
                } 
            }
            else
            {
                //| ¬t  ¥X ÞDÀ?
                Console.WriteLine("Has No Mask Img");
                for (int i = 0; i < imagesWithPosition.Count; i++)
                {
                    for (int j = 0; j < imagesWithPosition[i].bitmaps.Count; j++)
                    {
                        var img = imagesWithPosition[i].bitmaps[j];
                        //var x = (short)imagesWithPosition[i].xVals[j];
                        //var y = (short)imagesWithPosition[i].yVals[j];

                        //no mask img x<->y !!! k333123 240326
                        var x = (short)imagesWithPosition[i].yVals[j];
                        var y = (short)imagesWithPosition[i].xVals[j];
                        mLibraryV2.AddImage(img, x, y);
                        mLibraryV2DataTemps.Add(new ImageWithPosition(img, null, x, y)); //add k333123
                        Console.Write(".");
                    }
                }
            }

            //

            //mLibraryV2.Frames.Add(MirAction.Stance, new M2Frame(0,1,0,10,0,0,0,0));

            //mLibraryV2 realocation
            /////////////////////////////////////////
            //save mLibraryv2 based ActionIndex frames 
            //reindex
            //List<ImageWithPosition> mLibraryV2DataTempsReIdx = new List<ImageWithPosition>();

            //0+8*0 1+8*0 2+8*0 3+8*0 4+8*0 5+8*0 6+8*0 7+8*0 => 0+8*0 4+8*0 1+8*0 5+8*0 2+8*0 6+8*0 3+8*0 7+8*0
            //0+8*1 1+8*1 2+8*1 3+8*1 4+8*1 5+8*1 6+8*1 7+8*1 => 0+8*1 4+8*1 1+8*1 5+8*1 2+8*1 6+8*1 3+8*1 7+8*1

            /*
            var mLibraryV2DataTempsReIdxs =  ExineConvertDirection(mLibraryV2DataTemps.ToArray()); 
            for(int i=0;i< mLibraryV2DataTempsReIdxs.Length;i++)
            {
                if (mLibraryV2DataTempsReIdxs[i].bitmapMask == null)
                {
                    mLibraryV2ReIndex.AddImage(mLibraryV2DataTempsReIdxs[i].bitmap, mLibraryV2DataTempsReIdxs[i].xVal, mLibraryV2DataTempsReIdxs[i].yVal);
                }
                else
                {
                    mLibraryV2ReIndex.AddImage(mLibraryV2DataTempsReIdxs[i].bitmap, mLibraryV2DataTempsReIdxs[i].bitmapMask, mLibraryV2DataTempsReIdxs[i].xVal, mLibraryV2DataTempsReIdxs[i].yVal);
                }
            }
            mLibraryV2ReIndex.Save();*/
            /////////////////////////////////////////
            for (int i=0;i< tempMap2.Count;i++)
            {
                int j = tempMap2[i].frameIdx;

                if (mLibraryV2DataTemps[j].bitmapMask == null)
                {
                    //k333123 x와y가 뒤집어졌으나 캐릭터만 그런지 알수없으므로 일단 ReIndex 되는것에 대해서만 다시 뒤집어줌.
                    //mLibraryV2ReIndex.AddImage(mLibraryV2DataTemps[j].bitmap, mLibraryV2DataTemps[j].xVal, mLibraryV2DataTemps[j].yVal);
                    mLibraryV2ReIndex.AddImage(mLibraryV2DataTemps[j].bitmap, mLibraryV2DataTemps[j].yVal, mLibraryV2DataTemps[j].xVal);
                }
                else
                {
                    //k333123 x와y가 뒤집어졌으나 캐릭터만 그런지 알수없으므로 일단 ReIndex 되는것에 대해서만 다시 뒤집어줌.
                    //mLibraryV2ReIndex.AddImage(mLibraryV2DataTemps[j].bitmap, mLibraryV2DataTemps[j].bitmapMask, mLibraryV2DataTemps[j].xVal, mLibraryV2DataTemps[j].yVal);
                    mLibraryV2ReIndex.AddImage(mLibraryV2DataTemps[j].bitmap, mLibraryV2DataTemps[j].bitmapMask, mLibraryV2DataTemps[j].yVal, mLibraryV2DataTemps[j].xVal);
                }
            }
            mLibraryV2ReIndex.Save();
            /////////////////////////////////////////

            mLibraryV2.Save();
        }

        
        int[] ExineConvertDirection(int[] inputDatas)
        {
            int[] outputDatas = new int[inputDatas.Length];

            for (int i = 0; i < outputDatas.Length; i++)
            {
                int a = i % 8; //0~8
                int b = i / 8; //8*b
                int frameIdx = 0;

                if (a % 2 == 0)
                {
                    frameIdx = (a / 2) + 8 * b;
                }
                else if (a % 2 == 1)
                {
                    frameIdx = (((a - 1) / 2) + 4) + 8 * b;
                }

                outputDatas[i] = inputDatas[frameIdx];

            }
            return outputDatas;
        }
    }
    class YPFHeader
    {
        ushort version = 0;
        ushort width = 0;
        ushort height = 0;
        uint depth = 0;
        uint hasPalette = 0;// 0:false 1:true
        byte[] palatte = new byte[512];
        uint dataPotision = 0;
        ushort imageSetCount = 0;


        public int FillData(byte[] datas)
        {
            try
            {
                int idx = 0;

                version = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                width = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                height = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                depth = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                hasPalette = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
                if (HasPalette)
                {
                    Buffer.BlockCopy(datas, idx, palatte, 0, 512);
                    idx = idx + 512;
                }
                dataPotision = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                imageSetCount = BitConverter.ToUInt16(datas, idx);
                idx = idx + 2;

                PrintVal();

                return idx;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
        private void PrintVal()
        {
            Console.WriteLine("Version:" + version);
            Console.WriteLine("Width:" + width);
            Console.WriteLine("Height:" + height);
            Console.WriteLine("Depth:" + depth);
            Console.WriteLine("HasPalette:" + HasPalette);
            Console.WriteLine("Palatte[0]:" + Palatte[0]);
            Console.WriteLine("DataPotision:" + dataPotision);
            Console.WriteLine("ImageSetCount:" + imageSetCount);
        }


        public ushort Version { get => version; }
        public ushort Width { get => width; }
        public ushort Height { get => height; }
        public uint Depth { get => depth; }
        public bool HasPalette { get { return (hasPalette != 0); } }
        public byte[] Palatte { get => palatte; }
        public uint DataPotision { get => dataPotision; }
        public ushort ImageSetCount { get => imageSetCount; }
    }

    class YPFImageSet
    {
        Dummy1Info dummy1Info = new Dummy1Info();
        FrameInfo frameInfo = new FrameInfo();
        public ActionInfo actionInfo = new ActionInfo();
        StateInfo stateInfo = new StateInfo();
        StateTransValue stateTransValue = new StateTransValue();


        public int FillData(byte[] datas, int idx, int version, int dataPotision, bool hasPalette, byte[] palette)
        {
            Console.WriteLine("Start DummyInfo IDX:" + idx);
            //Console.ReadLine();

            idx = dummy1Info.FillData(datas, idx);

            Console.WriteLine("Start frameInfo IDX:" + idx);
            //Console.ReadLine();
            idx = frameInfo.FillData(datas, idx, version, dataPotision, hasPalette, palette);

            Console.WriteLine("Start actionInfo IDX:" + idx);
            //Console.ReadLine();

            idx = actionInfo.FillData(datas, idx);
            Console.WriteLine("Start stateInfo IDX:" + idx);
            //Console.ReadLine();

            idx = stateInfo.FillData(datas, idx);
            Console.WriteLine("Start stateTransValue IDX:" + idx);
            //Console.ReadLine();


            idx = stateTransValue.FillData(datas, idx);
            Console.WriteLine("END stateTransValue IDX:" + idx);
            //Console.ReadLine();

            return idx;
        }
        public void SaveFile(string filename)
        {
            frameInfo.SaveFrame(filename);
        }

        public ImagesWithPosition ConvertToLib()
        {
            return frameInfo.ConvertToLib();
        }


        public Dummy1Info Dummy1Info { get => dummy1Info; }
        public FrameInfo FrameInfo { get => frameInfo; }
        public ActionInfo ActionInfo { get => actionInfo; }
        public StateInfo StateInfo { get => stateInfo; }
        public StateTransValue StateTransValue { get => stateTransValue; }
    }


    class Dummy1Info
    {
        uint len = 0;
        uint position = 0;
        byte[] dummy1Datas = new byte[0];


        public int FillData(byte[] datas, int idx)
        {
            len = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;
            Console.WriteLine("Dummy1Len:" + len);
            if (len == 0) return idx;

            position = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            //idx0\
            //ä\ D4 Xø  ÆL
            /*
            int idx4 = (int)position; 
            dummy1Datas = new byte[len];  
            Buffer.BlockCopy(datas,idx4,dummy1Datas,0,(int)len);
            idx4 = idx4+(int)len;
            */

            //PrintVal();
            return idx;
        }

        private void PrintVal()
        {
            Console.WriteLine("len:" + len);
            Console.WriteLine("position:{0} ", position);
        }

        public uint Len { get => len; }
        public uint Position { get => position; }
    }


    class FrameInfo
    {
        uint frameCount = 0;
        Frame[] frames = null;

        public int FillData(byte[] datas, int idx, int version, int dataPotision, bool hasPalette, byte[] palette)
        {
            frameCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;
            if (frameCount != 0)
            {
                frames = new Frame[frameCount];
                for (int i = 0; i < frames.Length; i++)//#Ü\ |è 1Ì ¬
                {
                    frames[i] = new Frame();
                    idx = frames[i].FillData(datas, idx, version, dataPotision, hasPalette, palette);

                    //Console.WriteLine("");
                    //Console.WriteLine("");
                    //frames[i].SaveImage(idx.ToString());
                }
            }
            //PrintVal();

            return idx;
        }

        private void PrintVal()
        {
            Console.WriteLine("frameCount:" + frameCount);
        }

        public void SaveFrame(string filename)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                Console.WriteLine("Save Frame:" + i + "/" + frames.Length);
                frames[i].SaveImage(filename + "_" + i);
            }
        }


        //imageSetè\ Ì$

        public ImagesWithPosition ConvertToLib()
        {
            //imageSetCount==2 => with mask
            /*
            List<Bitmap> bitmaps = new List<Bitmap>();
            List<int> xVals = new List<int>();
            List<int> yVals = new List<int>();
            */

            ImagesWithPosition imagesWithPosition = new ImagesWithPosition();

            if (frames == null) return null;//add 230530

            for (int i = 0; i < frames.Length; i++)
            {
                Console.WriteLine("FrameIdx:" + i + "/" + frames.Length);

                imagesWithPosition.bitmaps.Add(frames[i].ConvertToLib());
                imagesWithPosition.xVals.Add(frames[i].Top);
                imagesWithPosition.yVals.Add(frames[i].Left);
            }
            return imagesWithPosition;


        }
    }

    class ImagesWithPosition
    {
        public List<Bitmap> bitmaps = new List<Bitmap>();
        public List<Bitmap> bitmapMasks = new List<Bitmap>();
        public List<int> xVals = new List<int>();
        public List<int> yVals = new List<int>(); 
    }
    class ImageWithPosition
    {
        public Bitmap bitmap = null;
        public Bitmap bitmapMask = null;
        public short xVal = 0;
        public short yVal = 0;
        public ImageWithPosition(Bitmap bitmap, Bitmap bitmapMask, short xVal, short yVal)
        {
            this.bitmap = bitmap;
            this.bitmapMask = bitmapMask;
            this.xVal = xVal;
            this.yVal = yVal;
        }
    }

    class Frame
    {
        byte[] colorData = null;
        byte[] colorDataRGBA = null;

        int top = 0;
        int left = 0;
        int bottom = 0;
        int right = 0;
        uint flag = 0;

        uint alphaLen = 0;
        uint alphaOffset = 0;
        uint baseOffset = 0;
        uint baseLen = 0;
        byte depthType = 0;
        ushort depthVal2 = 0;
        ushort depthNearestDist = 0;
        uint depthOffset = 0;
        uint depthSize = 0;
        uint dummy2Len = 0;
        uint dummy2Position = 0;
        byte[] dummy2Data = new byte[0];
        uint[] alphaData = new uint[0];
        byte[] alphaMaskAndLen = new byte[0];


        public int FillData(byte[] datas, int idx, int version, int dataPotision, bool hasPalette, byte[] palette)
        {
            //Console.WriteLine("####1####Frame FillData IDX:" +idx);
            //Console.ReadLine();

            top = BitConverter.ToInt16(datas, idx);
            idx = idx + 2;

            left = BitConverter.ToInt16(datas, idx);
            idx = idx + 2;

            bottom = BitConverter.ToInt16(datas, idx);
            idx = idx + 2;

            right = BitConverter.ToInt16(datas, idx);
            idx = idx + 2;

            flag = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            if (HasAlpha)
            {
                alphaLen = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                alphaOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }
            if (HasBase)
            {
                baseOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                if (version == 14)
                {
                    baseLen = (uint)(FrameHeight * FrameWidth);
                }
                else
                {
                    baseLen = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;
                }
            }
            if (HasDepth)
            {
                if (version == 14)
                {
                    uint ver14_depth_1 = 0;
                    uint ver14_depth_2 = 0;

                    ver14_depth_1 = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;

                    ver14_depth_2 = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;
                }
                else if (version == 16)
                {
                    ushort tempDepthSize;
                    ushort tempDepthSize2;

                    tempDepthSize = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;

                    tempDepthSize2 = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;

                    depthSize = (uint)tempDepthSize2;
                    depthSize = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;
                }
                else
                {
                    depthType = datas[idx];
                    idx++;

                    depthVal2 = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;

                    depthNearestDist = BitConverter.ToUInt16(datas, idx);
                    idx = idx + 2;

                    depthOffset = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;

                    depthSize = BitConverter.ToUInt32(datas, idx);
                    idx = idx + 4;
                }
            }
            //PrintVal();
            dummy2Len = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;
            if (dummy2Len != 0)
            {
                dummy2Position = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
                int idx3 = (int)dummy2Position;

                dummy2Data = new byte[dummy2Len];
                for (int i = 0; i < dummy2Data.Length; i++)
                {
                    dummy2Data[i] = datas[idx3];
                    idx3++;
                }
            }

            if (HasAlpha)
            {
                int idx2 = (int)(dataPotision + alphaOffset); //tÐ 0\ Ì¼
                alphaData = new uint[FrameHeight];
                for (int i = 0; i < alphaData.Length; i++)
                {
                    alphaData[i] = BitConverter.ToUInt32(datas, idx2);
                    idx2 = idx2 + 4;
                }

                alphaMaskAndLen = new byte[FrameHeight * FrameHeight];

                int aMaskLenIdx = 0;
                while (true)
                {
                    if (idx2 >= dataPotision + alphaOffset + alphaLen) break;
                    if (aMaskLenIdx >= alphaMaskAndLen.Length) break;

                    alphaMaskAndLen[aMaskLenIdx] = datas[idx2];
                    idx2++;
                    aMaskLenIdx++;
                }

                if (hasPalette)
                {
                    //idx=0;
                    colorData = new byte[FrameWidth * FrameHeight * 2];
                    uint colorIdx = (uint)(baseOffset + dataPotision);


                    byte[] AlphaMask = GetAlphaMask();
                    for (int i = 0; i < AlphaMask.Length; i++)
                    {
                        if (AlphaMask[i] == 0x00)
                        {
                            //Magenta
                            colorData[i * 2] = 0x1f;
                            colorData[i * 2 + 1] = 0xf8; //C302

                            //orange
                            colorData[i * 2] = 0x02;
                            colorData[i * 2 + 1] = 0xC3; //C302
                        }
                        else
                        {
                            if (baseOffset == 0)
                            {
                                colorData[i * 2] = 0x00;
                                colorData[i * 2 + 1] = 0x00;

                                //orange
                                colorData[i * 2] = 0x02;
                                colorData[i * 2 + 1] = 0xC3; //C302
                            }
                            else
                            {
                                colorData[i * 2] = palette[datas[colorIdx] * 2];
                                colorData[i * 2 + 1] = palette[datas[colorIdx] * 2 + 1];
                                colorIdx++;
                            }
                        }
                        idx2++;
                    }

                    //PrintVal();
                }
                else
                {
                    colorData = new byte[FrameWidth * FrameHeight * 2];
                    uint colorIdx = (uint)(baseOffset + dataPotision);

                    //alpha mask data is must save file!
                    byte[] AlphaMask = GetAlphaMask();
                    for (int i = 0; i < AlphaMask.Length; i++)
                    {
                        //AlphaMask Print
                        //230530
                        //Console.WriteLine("AlphaMask{0}:{1}", i, AlphaMask[i]);
                        //AlphaMask : 0,8,32,56,216,224,248
                        //0000 0000, 0000 1000,0010 0000,0011 1000,1101 1000, 1110 0000,1111 1000 
                        if (AlphaMask[i] == 0x00)
                        {
                            //Magenta

                            colorData[i * 2] = 0x1f;
                            colorData[i * 2 + 1] = 0xf8;
                            /*
                            colorData[i * 2] = 0x00;
                            colorData[i * 2 + 1] = 0x00;
                            */

                            colorData[i * 2] = 0x02;
                            colorData[i * 2 + 1] = 0xC3; //C302
                        }
                        else
                        {
                            if (baseOffset == 0)
                            {

                                colorData[i * 2] = 0x00;
                                colorData[i * 2 + 1] = 0x00;

                                colorData[i * 2] = 0x02;
                                colorData[i * 2 + 1] = 0xC3; //C302
                            }
                            else
                            {
                                colorData[i * 2] = datas[colorIdx++];
                                colorData[i * 2 + 1] = datas[colorIdx++];
                            }
                        }
                    }
                }
            }
            else
            {
                if (hasPalette)
                {
                    colorData = new byte[FrameWidth * FrameHeight * 2];
                    uint colorIdx = (uint)(baseOffset + dataPotision);

                    for (int i = 0; i < baseLen; ++i)
                    {
                        colorData[i * 2] = palette[datas[colorIdx] * 2];
                        colorData[i * 2 + 1] = palette[datas[colorIdx] * 2 + 1];
                        colorIdx++;
                    }
                }
                else
                {
                    colorData = new byte[FrameWidth * FrameHeight * 2];
                    uint colorIdx = (uint)(baseOffset + dataPotision);

                    for (int i = 0; i < colorData.Length; i++)
                    {
                        colorData[i] = datas[colorIdx++];
                    }
                }
            }
            return idx;
        }

        public bool SaveImage(string filename)
        {
            if (colorData == null) return false;
            if (FrameHeight == 0 || FrameWidth == 0)
            {
                Console.WriteLine("Image Size Err. FrameHeight:" + FrameHeight + " FrameWidth:" + FrameWidth);
                return false;
            }

            colorDataRGBA = new byte[colorData.Length * 2];
            int argbIdx = 0;
            for (int i = 0; i < colorData.Length; i = i + 2)
            {
                ushort data = BitConverter.ToUInt16(colorData, i);
                byte r = (byte)(((data & 0xF800) >> 11) << 3);
                byte g = (byte)(((data & 0x7E0) >> 5) << 2);
                byte b = (byte)((data) << 3);

                colorDataRGBA[argbIdx] = b;//b
                colorDataRGBA[argbIdx + 1] = g;//g
                colorDataRGBA[argbIdx + 2] = r;//r
                colorDataRGBA[argbIdx + 3] = 0xff;//a//

                //transperent apply
                if (HasAlpha)
                {
                    byte[] AlphaMask = GetAlphaMask();
                    /*
                    if (AlphaMask[i/2] == 0x00)
                    {
                        colorDataRGBA[argbIdx] = 0x00;//b
                        colorDataRGBA[argbIdx + 1] = 0x00;//g
                        colorDataRGBA[argbIdx + 2] = 0x00;//r
                        colorDataRGBA[argbIdx + 3] = 0x00;//a
                    }*/

                    colorDataRGBA[argbIdx + 3] = AlphaMask[i / 2];
                }

                argbIdx = argbIdx + 4;
            }

            DirectoryInfo di = new DirectoryInfo(".\\YPF_OUT");
            if (!di.Exists) di.Create();


            Bitmap bitmap = new Bitmap(FrameWidth, FrameHeight, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            int rowSize = Math.Abs(bmpData.Stride);
            IntPtr bmpScan0 = bmpData.Scan0;
            for (int y = 0; y < FrameHeight; y++)
            {
                Marshal.Copy(colorDataRGBA, y * FrameWidth * 4, IntPtr.Add(bmpScan0, y * rowSize), FrameWidth * 4);
            }
            bitmap.UnlockBits(bmpData);
            bitmap.Save(".\\YPF_OUT\\" + filename + ".png", ImageFormat.Png);
            //bitmap.Save(".\\out\\"+filename+".png", ImageFormat.Png);
            bitmap.Dispose();


            /*
            //Bitmap bitmap = new Bitmap(FrameWidth, FrameHeight, PixelFormat.Format32bppArgb);
            Bitmap bitmap = new Bitmap(FrameWidth, FrameHeight, PixelFormat.Format16bppRgb565);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            int rowSize = Math.Abs(bmpData.Stride);
            IntPtr bmpScan0 = bmpData.Scan0;
            for (int y = 0; y < FrameHeight; y++)
            {
                Marshal.Copy(colorData, y * FrameWidth * 2, IntPtr.Add(bmpScan0, y * rowSize), FrameWidth * 2);
            }
            bitmap.UnlockBits(bmpData);
            bitmap.Save(".\\out\\"+filename+".bmp", ImageFormat.Bmp);
            //bitmap.Save(".\\out\\"+filename+".png", ImageFormat.Png);
            bitmap.Dispose();
            */


            return true;
        }


        //public Bitmap ConvertToLib()
        public Bitmap ConvertToLib()
        {
            if (colorData == null) return null;
            if (FrameHeight == 0 || FrameWidth == 0)
            {
                Console.WriteLine("Image Size Err. FrameHeight:" + FrameHeight + " FrameWidth:" + FrameWidth);
                return null;
            }

            //alphaMaskDataRGBA = new byte[colorData.Length * 2];//alpha mask is not mask image!
            colorDataRGBA = new byte[colorData.Length * 2];
            int argbIdx = 0;
            for (int i = 0; i < colorData.Length; i = i + 2)
            {

                ushort data = BitConverter.ToUInt16(colorData, i);
                byte r = (byte)(((data & 0xF800) >> 11) << 3);
                byte g = (byte)(((data & 0x7E0) >> 5) << 2);
                byte b = (byte)((data) << 3);

                colorDataRGBA[argbIdx] = b;//b
                colorDataRGBA[argbIdx + 1] = g;//g
                colorDataRGBA[argbIdx + 2] = r;//r
                colorDataRGBA[argbIdx + 3] = 0xff;//a //from mask...


                //transperent apply
                if (HasAlpha)
                {
                    byte[] AlphaMask = GetAlphaMask();
                    colorDataRGBA[argbIdx + 3] = AlphaMask[i / 2];//a  
                    /*
                    if (AlphaMask[i / 2] > 0x80)
                    {
                        //colorDataRGBA[argbIdx + 3] = 0xff;//AlphaMask[i / 2]; //0 => no 255=>yes
                        colorDataRGBA[argbIdx + 3] = AlphaMask[i / 2];//AlphaMask[i / 2]; //0 => no 255=>yes
                    }
                    else
                    {
                        colorDataRGBA[argbIdx + 3] = 0x00;
                    }
                    */
                }
                argbIdx = argbIdx + 4;
            }

            DirectoryInfo di = new DirectoryInfo(".\\out");
            if (!di.Exists) di.Create();

            Bitmap bitmap = new Bitmap(FrameWidth, FrameHeight, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            int rowSize = Math.Abs(bmpData.Stride);
            IntPtr bmpScan0 = bmpData.Scan0;
            for (int y = 0; y < FrameHeight; y++)
            {
                Marshal.Copy(colorDataRGBA, y * FrameWidth * 4, IntPtr.Add(bmpScan0, y * rowSize), FrameWidth * 4);
            }
            bitmap.UnlockBits(bmpData);
            //bitmap.Dispose(); 

            return bitmap;
        }

        public byte[] GetAlphaMask()
        {
            int index = 0;
            byte[] alphaMask = new byte[FrameHeight * FrameWidth];
            int alphaMaskIdx = 0;
            while (true)
            {
                if (index + 1 >= alphaMaskAndLen.Length) break;

                var alphaMaskRowlen = ((alphaMaskAndLen[index] & 7) << 8 | alphaMaskAndLen[index + 1]);
                byte alphaMaskVal = (byte)(alphaMaskAndLen[index] & 0xf8);
                index = index + 2;

                //for (int j = 0; j < (alphaMaskRowlen); j++)
                for (int j = 0; j < (alphaMaskRowlen); j++)
                {
                    if (alphaMask.Length <= alphaMaskIdx) break;

                    //alphaMask[alphaMaskIdx] = alphaMaskVal; 
                    alphaMask[alphaMaskIdx] = (byte)((alphaMaskVal >> 3) * 8); //230530 0000 1000 ~1111 1000 => right 3 shift 
                    alphaMaskIdx++;
                }
            }
            return alphaMask;
        }

        //how to use rows?
        public uint[] ROWS
        {
            get
            {
                uint[] rows = new uint[FrameHeight];
                rows[0] = alphaData[0] / 2;
                for (int i = 1; i < rows.Length; i++)
                {
                    rows[i] = (alphaData[i] - alphaData[i - 1]) / 2;
                }
                return rows;
            }
        }


        public bool HasAlpha { get => (flag & 8) == 0x0; }//bits[3] == 0 => hasAlpha=true
        public bool HasBase { get => (flag & 4) != 0x0; }//bits[2] == 1 => hasBase=true
        public bool HasDepth { get => (flag & 1) != 0x0; }//bits[0] == 1 => hasDepth=true

        public int Top { get => top; }
        public int Left { get => left; }

        public int FrameHeight { get => bottom - top; }
        public int FrameWidth { get => right - left; }

        private void PrintVal()
        {
            Console.WriteLine("top :" + top);
            Console.WriteLine("left :" + left);
            Console.WriteLine("bottom :" + bottom);
            Console.WriteLine("right :" + right);
            Console.WriteLine("FrameWidth :" + FrameWidth);
            Console.WriteLine("FrameWidth :" + FrameHeight);
            Console.WriteLine("HasAlpha :" + HasAlpha);
            Console.WriteLine("HasBase :" + HasBase);
            Console.WriteLine("HasDepth :" + HasDepth);
            Console.WriteLine("alphaLen :" + alphaLen);
            Console.WriteLine("alphaOffset :" + alphaOffset);
            Console.WriteLine("baseOffset :" + baseOffset);
            Console.WriteLine("baseLen :" + baseLen);
            Console.WriteLine("depthType :" + depthType);
            Console.WriteLine("depthVal2 :" + depthVal2);
            Console.WriteLine("depthNearestDist :" + depthNearestDist);
            Console.WriteLine("depthOffset :" + depthOffset);
            Console.WriteLine("depthSize :" + depthSize);
            Console.WriteLine("dummy2Len :" + dummy2Len);
            Console.WriteLine("dummy2Position :" + dummy2Position);
            //Console.WriteLine("dummy2Data[0] :" +  dummy2Data[0] );  
            //Console.WriteLine("alphaData[0] :" +   alphaData[0] );  
            //Console.WriteLine("alphaMaskAndLen[0] :" +   alphaMaskAndLen[0] );  
            //Console.WriteLine("ROWS[0] :" +   ROWS[0] );   
            //Console.WriteLine("AlphaMask[0] :" +   GetAlphaMask()[0] );   
        }

    }

    //Action Index(+0 ~ +7)
    //Action Type Start Index(8*type)

    //8*0+0 ~ 8*0+7 (0:UP, 1:RIGHT, ..), PMode,Standing
    //8*1+0 ~ 8*1+7 (0:UP, 1:RIGHT, ..), AMode,OneH,Standing
    //...

    //how to get action info from main class..
   

    class ActionInfo
    {
        ushort actionCount = 0;
        public Action[] actions = new Action[0];

        public int FillData(byte[] datas, int idx)
        {
            actionCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            actions = new Action[actionCount];
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = new Action();
                idx = actions[i].FillData(datas, idx);
                /*
                for (int j = 0; j < actions[i].actionElements.Length; j++)
                {
                    //save to list
                    Console.WriteLine("ActionIdx:{0} ActionElementsIdx:{1} FrameIdx:{2}",i,j,actions[i].actionElements[j].frameIdx); 
                }
                */
            }
            //Console.ReadLine();
            return idx;
        }

        public void PrintVal()
        {
            Console.WriteLine("actionCount :" + actionCount);
            Console.WriteLine("actions.Len :" + actions.Length);
        }
    }


    class Action
    {
        uint actionSize = 0;
        uint actionOffset = 0;
        public byte[] time1 = new byte[6];
        public byte[] time2 = new byte[6];
        ushort actionElementCount = 0;
        public ActionElement[] actionElements = new ActionElement[0];

        public int FillData(byte[] datas, int idx)
        {
            actionSize = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            if (actionSize != 0)
            {
                actionOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }

            //Ü  ¬ä@ 2t¸ è\ õt ¬(
            /*
              for (int timeIndex = 0; timeIndex <= 2; ++timeIndex)                                            //   for i=0 to 2
            {                                                                                                    
                br.Read(out time1[timeIndex * 2]);                                                          //     time1[i*2].b
                br.Read(out time1[timeIndex * 2 + 1]);                                                      //     time1[i*2+1].b
                br.Read(out time2[timeIndex * 2]);                                                          //     time2[i*2].b
                br.Read(out time2[timeIndex * 2 + 1]);                                                      //     time2[i*2+1].b
            }
            */
            Buffer.BlockCopy(datas, idx, time1, 0, time1.Length);
            idx = idx + 6;

            Buffer.BlockCopy(datas, idx, time2, 0, time2.Length);
            idx = idx + 6;

            actionElementCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;


            actionElements = new ActionElement[actionElementCount];
            for (int i = 0; i < actionElements.Length; i++)
            {
                actionElements[i] = new ActionElement();
                idx = actionElements[i].FillData(datas, idx);
                //add
                //Console.WriteLine("ActionIndex:{0} FrameIdx{1}", i, actionElements[i].frameIdx); 
            }

            return idx;
        }
    }


    class ActionElement
    {
        public ushort frameIdx = 0;
        public uint time = 0;
        public uint actionElementLen = 0;
        public uint actionElementOffset = 0;
        public byte offsetX = 0;
        public byte offsetY = 0;
        public ushort dummy = 0;

        public int FillData(byte[] datas, int idx)
        {

            frameIdx = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            time = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            actionElementLen = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;


            if (actionElementLen != 0)
            {
                actionElementOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;

                uint idx5 = actionElementOffset;
                for (int i = 0; i < actionElementLen; i++)
                {
                    offsetX = datas[idx5];
                    idx5 = idx5 + 1;

                    offsetY = datas[idx5];
                    idx5 = idx5 + 1;

                    dummy = BitConverter.ToUInt16(datas, (int)idx5);
                    idx5 = idx5 + 2;
                }
            }

            return idx;
        }
    }

    class StateInfo
    {
        uint stateCount = 0;
        State[] states = new State[0];
        public int FillData(byte[] datas, int idx)
        {

            stateCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;


            states = new State[stateCount];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new State();
                idx = states[i].FillData(datas, idx);
            }
            return idx;
        }
    }

    class State
    {
        uint stateSize;
        uint stateOffset = 0;
        ushort stateElemCount;
        StateElement[] stateElements = new StateElement[0];


        public int FillData(byte[] datas, int idx)
        {
            stateSize = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            if (stateSize != 0)
            {
                stateOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }

            stateElemCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;



            stateElements = new StateElement[stateElemCount];
            for (int i = 0; i < stateElemCount; i++)
            {
                stateElements[i] = new StateElement();
                idx = stateElements[i].FillData(datas, idx);
            }
            return idx;
        }
    }


    class StateElement
    {
        byte isStateElementFrame;
        byte dummy1;
        byte dummy2;
        ushort dummy3;
        uint dummy4 = 0;
        uint stateElementSize;
        uint stateElementOffset = 0;
        public int FillData(byte[] datas, int idx)
        {

            isStateElementFrame = datas[idx];
            idx = idx + 1;

            dummy1 = datas[idx];
            idx = idx + 1;

            dummy2 = datas[idx];
            idx = idx + 1;

            dummy3 = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            if (isStateElementFrame != 0)
            {

                dummy4 = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }

            stateElementSize = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;

            if (stateElementSize != 0)
            {
                stateElementOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }

            return idx;
        }
    }

    class StateTransValue
    {
        ushort stateTransValueCount = 0;
        StateTrans[] stateTranses = new StateTrans[0];
        public int FillData(byte[] datas, int idx)
        {
            stateTransValueCount = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            stateTranses = new StateTrans[stateTransValueCount];

            for (int i = 0; i < stateTranses.Length; i++)
            {
                stateTranses[i] = new StateTrans();
                idx = stateTranses[i].FillData(datas, idx);
            }

            return idx;
        }
    }

    class StateTrans
    {
        ushort key1;
        ushort key2;
        ushort value;
        uint stateTransValueSize = 0;
        uint stateTransValueOffset = 0;

        public int FillData(byte[] datas, int idx)
        {
            key1 = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            key2 = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            value = BitConverter.ToUInt16(datas, idx);
            idx = idx + 2;

            stateTransValueSize = BitConverter.ToUInt32(datas, idx);
            idx = idx + 4;
            if (stateTransValueSize != 0)
            {
                stateTransValueOffset = BitConverter.ToUInt32(datas, idx);
                idx = idx + 4;
            }

            idx += (int)stateTransValueOffset;

            return idx;
        }
    }
}
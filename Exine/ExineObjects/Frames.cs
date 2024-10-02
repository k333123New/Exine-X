namespace Exine.ExineObjects
{
    public class FrameSet : Dictionary<ExAction, Frame>
    {
        // public static FrameSet ExPlayer, ExDefaultMonster, ExDefaultMonster2, ExDefaultMonster3, ExDefaultMonster4;
        // public static FrameSet ExDefaultMonster5, ExDefaultMonster6, ExDefaultMonster7;
        public static FrameSet ExPlayer, ExPlayerWoman;
        public static FrameSet[] ExMonsterFrameSet= new FrameSet[34];//For Exine Mob
        public static FrameSet ExineDefaultNPC, ExineGuardNPC, ExineStaticObjectNPC, DefaultMonster;
        public static List<FrameSet> DragonStatue, GreatFoxSpirit, HellBomb, CaveStatue;

        static FrameSet()
        {
            FrameSet frame;

            ExPlayer = new FrameSet();
            ExPlayerWoman = new FrameSet();

            /*
             * NPC Frame Set
             * 
             * yj, sn1, djj , kn1,ygji, sy1, jjji, Gy, 161~, 4f
             * sn2, gb 0~,4f
             * 
             * ji 263 New!!!
             * jds New2!!!
             * ms New3!!!
             * as New4!!!
             * elf New5!!!
             */

            //Default NPC
            ExineDefaultNPC = new FrameSet
            { 
                { ExAction.Standing, new Frame(161-1, 4, 0, 200) },
                { ExAction.Harvest, new Frame(161-1, 4, 0, 200) }
            };

            ExineGuardNPC = new FrameSet
            { 
                { ExAction.Standing, new Frame(0, 4, 0, 200) },
                { ExAction.Harvest, new Frame(0, 4, 0, 200) }
            };
            ExineStaticObjectNPC = new FrameSet
            { 
                { ExAction.Standing, new Frame(0, 1, 0, 200) },
                { ExAction.Harvest, new Frame(0, 1, 0, 200) }
            }; 
            //Default Monster
            DefaultMonster = new FrameSet
            {
                { ExAction.Standing, new Frame(0, 4, 0, 500) },
                { ExAction.Walking, new Frame(32, 6, 0, 100) },
                { ExAction.Attack1, new Frame(80, 6, 0, 100) },
                { ExAction.Struck, new Frame(128, 2, 0, 200) },
                { ExAction.Die, new Frame(144, 10, 0, 100) },
                { ExAction.Dead, new Frame(153, 1, 9, 1000) },
                { ExAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true } }
            };

            #region ExineMonsterFrameset
            //ZM_03_768_플래티
            ExMonsterFrameSet[0] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(288, 6, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 6, 0, 100)},
                                { ExAction.Dead, new Frame(245, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true }}
                };

            //ZM_03_769_아르마딜
            ExMonsterFrameSet[1] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 3, 0, 500)},
                                { ExAction.Walking, new Frame(120, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(264, 5, 0, 100)},
                                { ExAction.Struck, new Frame(208, 1, 0, 200)},
                                { ExAction.Die, new Frame(232, 4, 0, 100)},
                                { ExAction.Dead, new Frame(235, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(232, 4, 0, 100) { Reverse = true }}
                };

            //ZM_03_770_크릭켓 ZM_06_1545_스노우자이언트 ZM_06_1548_ ZM_06_1550_ ZM_06_1551_ ZM_06_1555_ ZM_06_1556_ ZM_06_1557_ ZM_06_1558_ ZM_06_1559_
            ExMonsterFrameSet[2] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(272, 6, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 4, 0, 100)},
                                { ExAction.Dead, new Frame(243, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(240, 4, 0, 100) { Reverse = true }}
                };

            //ZM_03_771_스콜 ZM_04_1024_거스웜
            ExMonsterFrameSet[3] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(288, 5, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 6, 0, 100)},
                                { ExAction.Dead, new Frame(245, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true }}
                };

            //ZM_03_772_개미 ZM_03_774_
            ExMonsterFrameSet[4] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 5, 0, 500)},
                                { ExAction.Walking, new Frame(136, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(288, 5, 0, 100)},
                                { ExAction.Struck, new Frame(224, 1, 0, 200)},
                                { ExAction.Die, new Frame(248, 5, 0, 100)},
                                { ExAction.Dead, new Frame(252, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(248, 5, 0, 100) { Reverse = true }}
                };

            //ZM_03_773_테나가
            ExMonsterFrameSet[5] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(272, 8, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 4, 0, 100)},
                                { ExAction.Dead, new Frame(243, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(240, 4, 0, 100) { Reverse = true }}
                };

            //ZM_04_1025_딩고
            ExMonsterFrameSet[6] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 6, 0, 100)},
                                { ExAction.Attack1, new Frame(304, 9, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 5, 0, 100)},
                                { ExAction.Dead, new Frame(268, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(264, 5, 0, 100) { Reverse = true }}
                };

            //ZM_04_1026_쇼크 ZM_05_1281_사마엘 ZM_05_1282_하피 ZM_05_1283_ ZM_05_1284_ ZM_05_1286_ ZM_05_1287_ ZM_05_1290_ ZM_06_1543_물고기
            ExMonsterFrameSet[7] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(272, 5, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 4, 0, 100)},
                                { ExAction.Dead, new Frame(243, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(240, 4, 0, 100) { Reverse = true }}
                };

            //ZM_04_1027_크로틀
            ExMonsterFrameSet[8] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(304, 6, 0, 100)},
                                { ExAction.Struck, new Frame(232, 1, 0, 200)},
                                { ExAction.Die, new Frame(256, 6, 0, 100)},
                                { ExAction.Dead, new Frame(261, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(256, 6, 0, 100) { Reverse = true }}
                };

            //ZM_04_1028_스토마
            ExMonsterFrameSet[9] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(160, 2, 0, 100)},
                                { ExAction.Attack1, new Frame(320, 5, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 7, 0, 100)},
                                { ExAction.Dead, new Frame(270, 1, 6, 1000)},
                                { ExAction.Revive, new Frame(264, 7, 0, 100) { Reverse = true }}
                };

            //ZM_04_1029_빅프로거 ZM_04_1030_스몰 프로거
            ExMonsterFrameSet[10] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(256, 4, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 2, 0, 100)},
                                { ExAction.Dead, new Frame(241, 1, 1, 1000)},
                                { ExAction.Revive, new Frame(240, 2, 0, 100) { Reverse = true }}
                };

            //ZM_04_1031_호아친
            ExMonsterFrameSet[11] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(296, 7, 0, 100)},
                                { ExAction.Struck, new Frame(232, 1, 0, 200)},
                                { ExAction.Die, new Frame(256, 5, 0, 100)},
                                { ExAction.Dead, new Frame(260, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(256, 5, 0, 100) { Reverse = true }}
                };

            //ZM_04_1032_켈피 ZM_05_1291_세이렌 ZM_08_2048_파이러스
            ExMonsterFrameSet[12] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(280, 5, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 5, 0, 100)},
                                { ExAction.Dead, new Frame(244, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(240, 5, 0, 100) { Reverse = true }}
                };

            //ZM_05_1280_델피네 ZM_06_1553_크루델오크 ZM_06_1554_쿠겔오크
            ExMonsterFrameSet[13] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(312, 7, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 6, 0, 100)},
                                { ExAction.Dead, new Frame(269, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(264, 6, 0, 100) { Reverse = true }}
                };

            //ZM_05_1285_로비아탈
            ExMonsterFrameSet[14] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(304, 6, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 5, 0, 100)},
                                { ExAction.Dead, new Frame(268, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(264, 5, 0, 100) { Reverse = true }}
                };

            //ZM_05_1288_반트
            ExMonsterFrameSet[15] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(160, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(336, 8, 0, 100)},
                                { ExAction.Struck, new Frame(256, 1, 0, 200)},
                                { ExAction.Die, new Frame(280, 7, 0, 100)},
                                { ExAction.Dead, new Frame(286, 1, 6, 1000)},
                                { ExAction.Revive, new Frame(280, 7, 0, 100) { Reverse = true }}
                };

            //ZM_05_1289_카룬 ZM_06_1539_닌트
            ExMonsterFrameSet[16] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(304, 7, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 5, 0, 100)},
                                { ExAction.Dead, new Frame(268, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(264, 5, 0, 100) { Reverse = true }}
                };

            //ZM_05_1292_즈라촉 ZM_05_1293_켈베로스리치 ZM_05_1294_ ZM_05_1295_ ZM_05_1296_ ZM_05_1297_ ZM_05_1298_ ZM_05_1299_
            ExMonsterFrameSet[17] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(264, 6, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 3, 0, 100)},
                                { ExAction.Dead, new Frame(242, 1, 2, 1000)},
                                { ExAction.Revive, new Frame(240, 3, 0, 100) { Reverse = true }}
                };

            //ZM_06_1536_ ZM_06_1537_ ZM_06_1540_빅혼
            ExMonsterFrameSet[18] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(280, 4, 0, 100)},
                                { ExAction.Struck, new Frame(216, 1, 0, 200)},
                                { ExAction.Die, new Frame(240, 5, 0, 100)},
                                { ExAction.Dead, new Frame(244, 1, 4, 1000)},
                                { ExAction.Revive, new Frame(240, 5, 0, 100) { Reverse = true }}
                };

            //ZM_06_1538_라돈 ZM_06_1564_라돈
            ExMonsterFrameSet[19] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(0, 0, 0, 0)},
                                { ExAction.Attack1, new Frame(288, 8, 0, 100)},
                                { ExAction.Struck, new Frame(0, 0, 0, 0)},
                                { ExAction.Die, new Frame(240, 6, 0, 100)},
                                { ExAction.Dead, new Frame(245, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1541_차쿠 ZM_06_1542_
            ExMonsterFrameSet[20] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(312, 5, 0, 100)},
                                { ExAction.Struck, new Frame(232, 1, 0, 200)},
                                { ExAction.Die, new Frame(256, 7, 0, 100)},
                                { ExAction.Dead, new Frame(262, 1, 6, 1000)},
                                { ExAction.Revive, new Frame(256, 7, 0, 100) { Reverse = true }}
                };

            //ZM_06_1544_바트라코스
            ExMonsterFrameSet[21] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 6, 0, 100)},
                                { ExAction.Attack1, new Frame(312, 4, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 6, 0, 100)},
                                { ExAction.Dead, new Frame(269, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(264, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1546_루츠
            ExMonsterFrameSet[22] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(312, 8, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 6, 0, 100)},
                                { ExAction.Dead, new Frame(269, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(264, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1547_카엘
            ExMonsterFrameSet[23] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(304, 8, 0, 100)},
                                { ExAction.Struck, new Frame(232, 1, 0, 200)},
                                { ExAction.Die, new Frame(256, 6, 0, 100)},
                                { ExAction.Dead, new Frame(261, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(256, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1549_리트라코
            ExMonsterFrameSet[24] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(312, 6, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 6, 0, 100)},
                                { ExAction.Dead, new Frame(269, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(264, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1552_가르콘오크
            ExMonsterFrameSet[25] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(320, 8, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 7, 0, 100)},
                                { ExAction.Dead, new Frame(270, 1, 6, 1000)},
                                { ExAction.Revive, new Frame(264, 7, 0, 100) { Reverse = true }}
                };

            //ZM_06_1560_그란트라코
            ExMonsterFrameSet[26] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(336, 9, 0, 100)},
                                { ExAction.Struck, new Frame(240, 1, 0, 200)},
                                { ExAction.Die, new Frame(264, 9, 0, 100)},
                                { ExAction.Dead, new Frame(272, 1, 8, 1000)},
                                { ExAction.Revive, new Frame(264, 9, 0, 100) { Reverse = true }}
                };

            //ZM_06_1561_일룩스오크
            ExMonsterFrameSet[27] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(160, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(328, 8, 0, 100)},
                                { ExAction.Struck, new Frame(256, 1, 0, 200)},
                                { ExAction.Die, new Frame(280, 6, 0, 100)},
                                { ExAction.Dead, new Frame(285, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(280, 6, 0, 100) { Reverse = true }}
                };

            //ZM_06_1562_슬라임
            ExMonsterFrameSet[28] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 6, 0, 500)},
                                { ExAction.Walking, new Frame(144, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(288, 6, 0, 100)},
                                { ExAction.Struck, new Frame(232, 1, 0, 200)},
                                { ExAction.Die, new Frame(256, 4, 0, 100)},
                                { ExAction.Dead, new Frame(259, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(256, 4, 0, 100) { Reverse = true }}
                };

            //ZM_06_1563_라돈
            ExMonsterFrameSet[29] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(0, 0, 0, 0)},
                                { ExAction.Attack1, new Frame(288, 12, 0, 100)},
                                { ExAction.Struck, new Frame(0, 0, 0, 0)},
                                { ExAction.Die, new Frame(240, 6, 0, 100)},
                                { ExAction.Dead, new Frame(245, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(240, 6, 0, 100) { Reverse = true }}
                };

            //ZM_07_1792_아이스 우드
            ExMonsterFrameSet[30] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 2, 0, 500)},
                                { ExAction.Walking, new Frame(112, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(248, 4, 0, 100)},
                                { ExAction.Struck, new Frame(200, 1, 0, 200)},
                                { ExAction.Die, new Frame(224, 3, 0, 100)},
                                { ExAction.Dead, new Frame(226, 1, 2, 1000)},
                                { ExAction.Revive, new Frame(224, 3, 0, 100) { Reverse = true }}
                };

            //ZM_07_1793_블라디터마
            ExMonsterFrameSet[31] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 8, 0, 500)},
                                { ExAction.Walking, new Frame(0, 0, 0, 0)},
                                { ExAction.Attack1, new Frame(272, 8, 0, 100)},
                                { ExAction.Struck, new Frame(0, 0, 0, 0)},
                                { ExAction.Die, new Frame(240, 4, 0, 100)},
                                { ExAction.Dead, new Frame(243, 1, 3, 1000)},
                                { ExAction.Revive, new Frame(240, 4, 0, 100) { Reverse = true }}
                };

            //ZM_09_2304_벽
            ExMonsterFrameSet[32] = new FrameSet
                {
                                { ExAction.Standing, new Frame(0, 4, 0, 500)},
                                { ExAction.Walking, new Frame(128, 3, 0, 100)},
                                { ExAction.Attack1, new Frame(0, 0, 0, 0)},
                                { ExAction.Struck, new Frame(216, 2, 0, 200)},
                                { ExAction.Die, new Frame(248, 3, 0, 100)},
                                { ExAction.Dead, new Frame(250, 1, 2, 1000)},
                                { ExAction.Revive, new Frame(248, 3, 0, 100) { Reverse = true }}
                };

            //ZM_09_2305_용의심장 ZM_09_2306_용의심장 ZM_09_2307_용의심장
            ExMonsterFrameSet[33] = new FrameSet
                {
                                { ExAction.Standing, new Frame(9, 15, 0, 500)},
                                { ExAction.Walking, new Frame(216, 4, 0, 100)},
                                { ExAction.Attack1, new Frame(448, 8, 0, 100)},
                                { ExAction.Struck, new Frame(312, 1, 0, 200)},
                                { ExAction.Die, new Frame(336, 6, 0, 100)},
                                { ExAction.Dead, new Frame(341, 1, 5, 1000)},
                                { ExAction.Revive, new Frame(336, 6, 0, 100) { Reverse = true }}
                };
            #endregion ExineMonsterFrameset
            
             

            #region DragonStatue
            //DragonStatue 1
            DragonStatue = new List<FrameSet> { (frame = new FrameSet()) };
            frame.Add(ExAction.Standing, new Frame(300, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(300, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(300, 1, -1, 200));

            //DragonStatue 2
            DragonStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(301, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(301, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(301, 1, -1, 200));

            //DragonStatue 3
            DragonStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(302, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(302, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(302, 1, -1, 200));

            //DragonStatue 4
            DragonStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(320, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(320, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(320, 1, -1, 200));

            //DragonStatue 5
            DragonStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(321, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(321, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(321, 1, -1, 200));

            //DragonStatue 6
            DragonStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(322, 1, -1, 1000));
            frame.Add(ExAction.AttackRange1, new Frame(322, 1, -1, 120));
            frame.Add(ExAction.Struck, new Frame(322, 1, -1, 200));
            #endregion

            #region GreatFoxSpirit
            //GreatFoxSpirit level 0
            GreatFoxSpirit = new List<FrameSet> { (frame = new FrameSet()) };
            frame.Add(ExAction.Standing, new Frame(0, 20, -20, 100));
            frame.Add(ExAction.Attack1, new Frame(22, 8, -8, 120));
            frame.Add(ExAction.Struck, new Frame(20, 2, -2, 200));
            frame.Add(ExAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(ExAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(ExAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //GreatFoxSpirit level 1
            GreatFoxSpirit.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(60, 20, -20, 100));
            frame.Add(ExAction.Attack1, new Frame(82, 8, -8, 120));
            frame.Add(ExAction.Struck, new Frame(80, 2, -2, 200));
            frame.Add(ExAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(ExAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(ExAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //GreatFoxSpirit level 2
            GreatFoxSpirit.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(120, 20, -20, 100));
            frame.Add(ExAction.Attack1, new Frame(142, 8, -8, 120));
            frame.Add(ExAction.Struck, new Frame(140, 2, -2, 200));
            frame.Add(ExAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(ExAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(ExAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //GreatFoxSpirit level 3
            GreatFoxSpirit.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(180, 20, -20, 100));
            frame.Add(ExAction.Attack1, new Frame(202, 8, -8, 120));
            frame.Add(ExAction.Struck, new Frame(200, 2, -2, 200));
            frame.Add(ExAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(ExAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(ExAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //GreatFoxSpirit level 4
            GreatFoxSpirit.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(240, 20, -20, 100));
            frame.Add(ExAction.Attack1, new Frame(262, 8, -8, 120));
            frame.Add(ExAction.Struck, new Frame(260, 2, -2, 200));
            frame.Add(ExAction.Die, new Frame(300, 18, -18, 120));
            frame.Add(ExAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Add(ExAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });
            #endregion

            #region HellBombs
            //HellBomb1
            HellBomb = new List<FrameSet> { (frame = new FrameSet()) };
            frame.Add(ExAction.Standing, new Frame(52, 9, -9, 100) { Blend = true });
            frame.Add(ExAction.Attack1, new Frame(999, 1, -1, 120) { Blend = true });
            frame.Add(ExAction.Struck, new Frame(52, 9, -9, 100) { Blend = true });

            //HellBomb2
            HellBomb.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(70, 9, -9, 100) { Blend = true });
            frame.Add(ExAction.Attack1, new Frame(999, 1, -1, 120) { Blend = true });
            frame.Add(ExAction.Struck, new Frame(70, 9, -9, 100) { Blend = true });

            //HellBomb3
            HellBomb.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(88, 9, -9, 100) { Blend = true });
            frame.Add(ExAction.Attack1, new Frame(999, 1, -1, 120) { Blend = true });
            frame.Add(ExAction.Struck, new Frame(88, 9, -9, 100) { Blend = true });
            #endregion

            #region CaveStatues
            //CaveStatue1
            CaveStatue = new List<FrameSet> { (frame = new FrameSet()) };
            frame.Add(ExAction.Standing, new Frame(0, 1, -1, 100) { Blend = false });
            frame.Add(ExAction.Struck, new Frame(0, 1, -1, 100) { Blend = false });
            frame.Add(ExAction.Die, new Frame(2, 8, -8, 100) { Blend = false });
            frame.Add(ExAction.Dead, new Frame(9, 1, -1, 100) { Blend = false });

            //CaveStatue2
            CaveStatue.Add(frame = new FrameSet());
            frame.Add(ExAction.Standing, new Frame(18, 1, -1, 100) { Blend = false });
            frame.Add(ExAction.Struck, new Frame(18, 1, -1, 100) { Blend = false });
            frame.Add(ExAction.Die, new Frame(20, 8, -8, 100) { Blend = false });
            frame.Add(ExAction.Dead, new Frame(27, 1, -1, 100) { Blend = false });
            #endregion

            #region Player

            //k333123 231205
            //k333123 check woman frame (man / women frame index is deferent!)
            ExPlayer.Add(ExAction.PEACEMODE_STAND, new Frame(0, 20, 0, 100));//split!0~6 : PEACEMODE_STAND_WAIT
            ExPlayer.Add(ExAction.PEACEMODE_STAND_WAIT, new Frame(0, 6, 14, 100));//split!0~6 : PEACEMODE_STAND_WAIT


            ExPlayer.Add(ExAction.ONEHAND_STAND, new Frame(160, 6, 0, 100, 0, 8, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_STAND, new Frame(208, 6, 0, 100, 0, 8, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_STAND, new Frame(256, 6, 0, 100, 0, 8, 0, 100));

            ExPlayer.Add(ExAction.PEACEMODE_WALK_LEFT, new Frame(304, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_WALK_LEFT, new Frame(336, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_WALK_LEFT, new Frame(368, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_WALK_LEFT, new Frame(400, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_RUN_LEFT, new Frame(304, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_RUN_LEFT, new Frame(336, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_RUN_LEFT, new Frame(368, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_RUN_LEFT, new Frame(400, 4, 0, 100, 112, 6, 0, 100));

            ExPlayer.Add(ExAction.PEACEMODE_SITDOWN, new Frame(432, 5, 0, 100));  //###
            ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_WAIT, new Frame(432+3, 1, 4, 100));//433,5,0,100//###

            //ExPlayer.Add(ExAction.ONEHAND_STUCK, new Frame(473, 1, 0, 100, 392, 3, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_STUCK, new Frame(472, 1, 0, 100, 392, 3, 0, 100));
            //ExPlayer.Add(ExAction.DIE, new Frame(497, 8, 0, 1000));// { Reverse = true });
            ExPlayer.Add(ExAction.DIE, new Frame(496, 8, 0, 1000));// { Reverse = true });

            ExPlayer.Add(ExAction.ONEHAND_ATTACK1, new Frame(560, 8, 0, 100, 168, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_ATTACK2, new Frame(624, 8, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_ATTACK3, new Frame(688, 8, 0, 100, 448, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK1, new Frame(752, 8, 0, 100, 168, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK2, new Frame(816, 8, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK3, new Frame(880, 8, 0, 100, 448, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_ATTACK1, new Frame(944, 8, 0, 100, 96, 8, 0, 100));
            ExPlayer.Add(ExAction.MAGIC_CAST, new Frame(1008, 1, 0, 1000, 160, 1, 0, 1000));
            ExPlayer.Add(ExAction.MAGIC_ATTACK, new Frame(1016, 1, 0, 1000, 332, 1, 5, 1000));


            ExPlayer.Add(ExAction.PEACEMODE_WALK_RIGHT, new Frame(1024, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_WALK_RIGHT, new Frame(1056, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_WALK_RIGHT, new Frame(1088, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_WALK_RIGHT, new Frame(1120, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_RUN_RIGHT, new Frame(1024, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_RUN_RIGHT, new Frame(1056, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_RUN_RIGHT, new Frame(1088, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_RUN_RIGHT, new Frame(1120, 4, 0, 100, 112, 6, 0, 100));
            //ExPlayer.Add(ExAction.PEACEMODE_STANDUP, new Frame(1153, 5, 1, 100));
            ExPlayer.Add(ExAction.PEACEMODE_STANDUP, new Frame(1158, 5, 1, 100));


            //Common
            ExPlayer.Add(ExAction.Standing, new Frame(0, 4, 0, 500, 0, 8, 0, 250));
            ExPlayer.Add(ExAction.Walking, new Frame(32, 6, 0, 100, 64, 6, 0, 100));
            ExPlayer.Add(ExAction.Running, new Frame(80, 6, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.Stance, new Frame(128, 1, 0, 1000, 160, 1, 0, 1000));
            ExPlayer.Add(ExAction.Stance2, new Frame(300, 1, 5, 1000, 332, 1, 5, 1000));
            ExPlayer.Add(ExAction.Attack1, new Frame(136, 6, 0, 100, 168, 6, 0, 100));
            ExPlayer.Add(ExAction.Attack2, new Frame(184, 6, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.Attack3, new Frame(232, 8, 0, 100, 264, 8, 0, 100));
            ExPlayer.Add(ExAction.Attack4, new Frame(416, 6, 0, 100, 448, 6, 0, 100));
            //ExPlayer.Add(ExAction.Spell, new Frame(296, 6, 0, 100, 328, 6, 0, 100));
            ExPlayer.Add(ExAction.Spell, new Frame(1016, 1, 0, 1000, 332, 1, 5, 1000));
            ExPlayer.Add(ExAction.Harvest, new Frame(344, 2, 0, 300, 376, 2, 0, 300));
            ExPlayer.Add(ExAction.Struck, new Frame(360, 3, 0, 100, 392, 3, 0, 100));
            ExPlayer.Add(ExAction.Die, new Frame(384, 4, 0, 100, 416, 4, 0, 100));
            ExPlayer.Add(ExAction.Dead, new Frame(387, 1, 3, 1000, 419, 1, 3, 1000));
            ExPlayer.Add(ExAction.Revive, new Frame(384, 4, 0, 100, 416, 4, 0, 100) { Reverse = true });
            ExPlayer.Add(ExAction.Mine, new Frame(184, 6, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.Lunge, new Frame(139, 1, 5, 1000, 300, 1, 5, 1000));

            //Assassin
            ExPlayer.Add(ExAction.Sneek, new Frame(464, 6, 0, 100, 496, 6, 0, 100));
            ExPlayer.Add(ExAction.DashAttack, new Frame(80, 3, 3, 100, 112, 3, 3, 100));

            //Archer
            ExPlayer.Add(ExAction.WalkingBow, new Frame(0, 6, 0, 100, 0, 6, 0, 100));
            ExPlayer.Add(ExAction.RunningBow, new Frame(48, 6, 0, 100, 48, 6, 0, 100));
            ExPlayer.Add(ExAction.AttackRange1, new Frame(96, 8, 0, 100, 96, 8, 0, 100));
            ExPlayer.Add(ExAction.AttackRange2, new Frame(160, 8, 0, 100, 160, 8, 0, 100));
            ExPlayer.Add(ExAction.AttackRange3, new Frame(224, 8, 0, 100, 224, 8, 0, 100));
            ExPlayer.Add(ExAction.Jump, new Frame(288, 8, 0, 100, 288, 8, 0, 100));

           

            //Fishing
            //ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_1, new Frame(632, 8, 0, 100));
            //ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_WAIT_1, new Frame(696, 6, 0, 120));
            //ExPlayer.Add(ExAction.PEACEMODE_STANDUP_1, new Frame(744, 8, 0, 100));
            ExPlayer.Add(ExAction.FishingCast, new Frame(433 - 1, 4, 0, 100));
            ExPlayer.Add(ExAction.FishingReel, new Frame(435, 1, 4, 100));
            ExPlayer.Add(ExAction.FishingWait, new Frame(1153, 8, 0, 100));
            #endregion

            #region Player_woman
            
            //k333123 check woman frame (man / women frame index is deferent!)
            ExPlayerWoman.Add(ExAction.PEACEMODE_STAND, new Frame(0, 23, 0, 100));//split!0~6 : PEACEMODE_STAND_WAIT
            ExPlayerWoman.Add(ExAction.PEACEMODE_STAND_WAIT, new Frame(0, 6, 14+3, 100));//split!0~6 : PEACEMODE_STAND_WAIT


            ExPlayerWoman.Add(ExAction.ONEHAND_STAND, new Frame(184, 4, 0, 100, 0, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_STAND, new Frame(216, 4, 0, 100, 0, 8, 0, 100));//216-192=
            ExPlayerWoman.Add(ExAction.BOWHAND_STAND, new Frame(248, 4, 0, 100, 0, 8, 0, 100));

            ExPlayerWoman.Add(ExAction.PEACEMODE_WALK_LEFT, new Frame(280, 4, 0, 100, 112, 6, 0, 100));//###
            ExPlayerWoman.Add(ExAction.ONEHAND_WALK_LEFT, new Frame(312, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_WALK_LEFT, new Frame(344, 4 , 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.BOWHAND_WALK_LEFT, new Frame(376, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.PEACEMODE_RUN_LEFT, new Frame(280, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.ONEHAND_RUN_LEFT, new Frame(312, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_RUN_LEFT, new Frame(344, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.BOWHAND_RUN_LEFT, new Frame(376, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.PEACEMODE_SITDOWN, new Frame(408, 5, 0, 100));
            ExPlayerWoman.Add(ExAction.PEACEMODE_SITDOWN_WAIT, new Frame(408 + 3, 1, 4, 100)); 

            ExPlayerWoman.Add(ExAction.ONEHAND_STUCK, new Frame(448, 1, 0, 100, 392, 3, 0, 100));
            ExPlayerWoman.Add(ExAction.DIE, new Frame(472, 7, 0, 1000));// { Reverse = true });
            ExPlayerWoman.Add(ExAction.ONEHAND_ATTACK1, new Frame(528, 8, 0, 100, 168, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.ONEHAND_ATTACK2, new Frame(592, 8, 0, 100, 216, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.ONEHAND_ATTACK3, new Frame(656, 8, 0, 100, 448, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_ATTACK1, new Frame(720, 8, 0, 100, 168, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_ATTACK2, new Frame(784, 8, 0, 100, 216, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_ATTACK3, new Frame(848, 8, 0, 100, 448, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.BOWHAND_ATTACK1, new Frame(912, 8, 0, 100, 96, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.MAGIC_CAST, new Frame(976, 1, 0, 1000, 160, 1, 0, 1000));
            ExPlayerWoman.Add(ExAction.MAGIC_ATTACK, new Frame(980, 1, 0, 1000, 332, 1, 5, 1000));
            ExPlayerWoman.Add(ExAction.PEACEMODE_WALK_RIGHT, new Frame(988, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.ONEHAND_WALK_RIGHT, new Frame(1020, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_WALK_RIGHT, new Frame(1052, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.BOWHAND_WALK_RIGHT, new Frame(1084, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.PEACEMODE_RUN_RIGHT, new Frame(988, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.ONEHAND_RUN_RIGHT, new Frame(1020, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.TWOHAND_RUN_RIGHT, new Frame(1052, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.BOWHAND_RUN_RIGHT, new Frame(1084, 4, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.PEACEMODE_STANDUP, new Frame(1122, 5, 1, 100));
            /*
             * { ExAction.PEACEMODE_STAND, new Frame(0, 23, 0, 500)},
                                { ExAction.PEACEMODE_STAND_WAIT, new Frame(0, 0, 0, 0)},
                                { ExAction.ONEHAND_STAND, new Frame(184, 4, 0, 500)},
                                { ExAction.TWOHAND_STAND, new Frame(216, 4, 0, 500)},
                                { ExAction.BOWHAND_STAND, new Frame(248, 4, 0, 500)},
                                { ExAction.PEACEMODE_WALK_LEFT, new Frame(0, 0, 0, 0)},
                                { ExAction.ONEHAND_WALK_LEFT, new Frame(0, 0, 0, 0)},
                                { ExAction.TWOHAND_WALK_LEFT, new Frame(0, 0, 0, 0)},
                                { ExAction.BOWHAND_WALK_LEFT, new Frame(0, 0, 0, 0)},
                                { ExAction.PEACEMODE_RUN_LEFT, new Frame(280, 4, 0, 100)},
                                { ExAction.ONEHAND_RUN_LEFT, new Frame(312, 4, 0, 100)},
                                { ExAction.TWOHAND_RUN_LEFT, new Frame(344, 4, 0, 100)},
                                { ExAction.BOWHAND_RUN_LEFT, new Frame(376, 4, 0, 100)},
                                { ExAction.PEACEMODE_SITDOWN, new Frame(408, 5, 0, 500)},
                                { ExAction.PEACEMODE_SITDOWN_WAIT, new Frame(0, 0, 0, 0)},
                                { ExAction.ONEHAND_STUCK, new Frame(448, 1, 0, 200)},
                                { ExAction.DIE, new Frame(472, 7, 0, 100)},
                                { ExAction.ONEHAND_ATTACK1, new Frame(528, 8, 0, 100)},
                                { ExAction.ONEHAND_ATTACK2, new Frame(592, 8, 0, 100)},
                                { ExAction.ONEHAND_ATTACK3, new Frame(656, 8, 0, 100)},
                                { ExAction.TWOHAND_ATTACK1, new Frame(720, 8, 0, 100)},
                                { ExAction.TWOHAND_ATTACK2, new Frame(784, 8, 0, 100)},
                                { ExAction.TWOHAND_ATTACK3, new Frame(848, 8, 0, 100)},
                                { ExAction.BOWHAND_ATTACK1, new Frame(912, 8, 0, 100)},
                                { ExAction.MAGIC_CAST, new Frame(976, 0, 0, 200)},
                                { ExAction.MAGIC_ATTACK, new Frame(980, 1, 0, 200)},
                                { ExAction.PEACEMODE_WALK_RIGHT, new Frame(0, 0, 0, 0)},
                                { ExAction.ONEHAND_WALK_RIGHT, new Frame(0, 0, 0, 0)},
                                { ExAction.TWOHAND_WALK_RIGHT, new Frame(0, 0, 0, 0)},
                                { ExAction.BOWHAND_WALK_RIGHT, new Frame(0, 0, 0, 0)},
                                { ExAction.PEACEMODE_RUN_RIGHT, new Frame(988, 4, 0, 100)},
                                { ExAction.ONEHAND_RUN_RIGHT, new Frame(1020, 4, 0, 100)},
                                { ExAction.TWOHAND_RUN_RIGHT, new Frame(1052, 4, 0, 100)},
                                { ExAction.BOWHAND_RUN_RIGHT, new Frame(1084, 4, 0, 100)},
                                { ExAction.PEACEMODE_STANDUP, new Frame(1122, 6, 0, 500)},
             */


            //Common
            ExPlayerWoman.Add(ExAction.Standing, new Frame(0, 4, 0, 500, 0, 8, 0, 250));
            ExPlayerWoman.Add(ExAction.Walking, new Frame(32, 6, 0, 100, 64, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Running, new Frame(80, 6, 0, 100, 112, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Stance, new Frame(128, 1, 0, 1000, 160, 1, 0, 1000));
            ExPlayerWoman.Add(ExAction.Stance2, new Frame(300, 1, 5, 1000, 332, 1, 5, 1000));
            ExPlayerWoman.Add(ExAction.Attack1, new Frame(136, 6, 0, 100, 168, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Attack2, new Frame(184, 6, 0, 100, 216, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Attack3, new Frame(232, 8, 0, 100, 264, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.Attack4, new Frame(416, 6, 0, 100, 448, 6, 0, 100));
            //ExPlayerWoman.Add(ExAction.Spell, new Frame(296, 6, 0, 100, 328, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Spell, new Frame(1016, 1, 0, 1000, 332, 1, 5, 1000));
            ExPlayerWoman.Add(ExAction.Harvest, new Frame(344, 2, 0, 300, 376, 2, 0, 300));
            ExPlayerWoman.Add(ExAction.Struck, new Frame(360, 3, 0, 100, 392, 3, 0, 100));
            ExPlayerWoman.Add(ExAction.Die, new Frame(384, 4, 0, 100, 416, 4, 0, 100));
            ExPlayerWoman.Add(ExAction.Dead, new Frame(387, 1, 3, 1000, 419, 1, 3, 1000));
            ExPlayerWoman.Add(ExAction.Revive, new Frame(384, 4, 0, 100, 416, 4, 0, 100) { Reverse = true });
            ExPlayerWoman.Add(ExAction.Mine, new Frame(184, 6, 0, 100, 216, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.Lunge, new Frame(139, 1, 5, 1000, 300, 1, 5, 1000));

            //Assassin
            ExPlayerWoman.Add(ExAction.Sneek, new Frame(464, 6, 0, 100, 496, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.DashAttack, new Frame(80, 3, 3, 100, 112, 3, 3, 100));

            //Archer
            ExPlayerWoman.Add(ExAction.WalkingBow, new Frame(0, 6, 0, 100, 0, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.RunningBow, new Frame(48, 6, 0, 100, 48, 6, 0, 100));
            ExPlayerWoman.Add(ExAction.AttackRange1, new Frame(96, 8, 0, 100, 96, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.AttackRange2, new Frame(160, 8, 0, 100, 160, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.AttackRange3, new Frame(224, 8, 0, 100, 224, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.Jump, new Frame(288, 8, 0, 100, 288, 8, 0, 100));

          

            //Fishing
            //ExPlayerWoman.Add(ExAction.PEACEMODE_SITDOWN_1, new Frame(632, 8, 0, 100));
            //ExPlayerWoman.Add(ExAction.PEACEMODE_SITDOWN_WAIT_1, new Frame(696, 6, 0, 120));
            //ExPlayerWoman.Add(ExAction.PEACEMODE_STANDUP_1, new Frame(744, 8, 0, 100));
            ExPlayerWoman.Add(ExAction.FishingCast, new Frame(433 - 1, 4, 0, 100));
            ExPlayerWoman.Add(ExAction.FishingReel, new Frame(435, 1, 4, 100));
            ExPlayerWoman.Add(ExAction.FishingWait, new Frame(1153, 8, 0, 100));
            #endregion


        }
    }

    public class Frame
    {
        public int Start, Count, Skip, EffectStart, EffectCount, EffectSkip;
        public int Interval, EffectInterval;
        public bool Reverse, Blend;

        public int OffSet
        {
            get { return Count + Skip; }
        }

        public int EffectOffSet
        {
            get { return EffectCount + EffectSkip; }
        }

        public Frame(int start, int count, int skip, int interval, int effectstart = 0, int effectcount = 0, int effectskip = 0, int effectinterval = 0)
        {
            Start = start;
            Count = count;
            Skip = skip;
            Interval = interval;
            EffectStart = effectstart;
            EffectCount = effectcount;
            EffectSkip = effectskip;
            EffectInterval = effectinterval;
        }

        public Frame(BinaryReader reader)
        {
            Start = reader.ReadInt32();
            Count = reader.ReadInt32();
            Skip = reader.ReadInt32();
            Interval = reader.ReadInt32();
            EffectStart = reader.ReadInt32();
            EffectCount = reader.ReadInt32();
            EffectSkip = reader.ReadInt32();
            EffectInterval = reader.ReadInt32();
            Reverse = reader.ReadBoolean();
            Blend = reader.ReadBoolean();
        }
    }

}

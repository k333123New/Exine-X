namespace Exine.ExineObjects
{
    public class FrameSet : Dictionary<ExAction, Frame>
    {
        public static FrameSet ExPlayer;
        public static FrameSet ExineDefaultNPC, ExineGuardNPC, ExineStaticObjectNPC, DefaultMonster;
        public static List<FrameSet> DragonStatue, GreatFoxSpirit, HellBomb, CaveStatue;

        static FrameSet()
        {
            FrameSet frame;

            ExPlayer = new FrameSet();


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
            ExPlayer.Add(ExAction.PEACEMODE_STAND, new Frame(0, 20, 0, 100));//split!0~6 : PEACEMODE_STAND_WAIT
            ExPlayer.Add(ExAction.PEACEMODE_STAND_WAIT, new Frame(0, 6, 14, 100));//split!0~6 : PEACEMODE_STAND_WAIT


            ExPlayer.Add(ExAction.ONEHAND_STAND, new Frame(161-1, 6, 0, 100, 0, 8, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_STAND, new Frame(209-1, 6, 0, 100, 0, 8, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_STAND, new Frame(257-1, 6, 0, 100, 0, 8, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_WALK_LEFT, new Frame(305-1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_WALK_LEFT, new Frame(337 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_WALK_LEFT, new Frame(369 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_WALK_LEFT, new Frame(401 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_RUN_LEFT, new Frame(305 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_RUN_LEFT, new Frame(337 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_RUN_LEFT, new Frame(369 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_RUN_LEFT, new Frame(401 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_SITDOWN, new Frame(433-1, 5, 0, 100)); 
            ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_WAIT, new Frame(435, 1, 4, 100));//433,5,0,100

            ExPlayer.Add(ExAction.ONEHAND_STUCK, new Frame(473, 1, 0, 100, 392, 3, 0, 100));
            ExPlayer.Add(ExAction.DIE, new Frame(497, 8, 0, 1000));// { Reverse = true });
            ExPlayer.Add(ExAction.ONEHAND_ATTACK1, new Frame(561, 8, 0, 100, 168, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_ATTACK2, new Frame(625, 8, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_ATTACK3, new Frame(689, 8, 0, 100, 448, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK1, new Frame(753, 8, 0, 100, 168, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK2, new Frame(817, 8, 0, 100, 216, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_ATTACK3, new Frame(881, 8, 0, 100, 448, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_ATTACK1, new Frame(945, 8, 0, 100, 96, 8, 0, 100));
            ExPlayer.Add(ExAction.MAGIC_CAST, new Frame(1009, 1, 0, 1000, 160, 1, 0, 1000));
            ExPlayer.Add(ExAction.MAGIC_ATTACK, new Frame(1017, 1, 0, 1000, 332, 1, 5, 1000));
            ExPlayer.Add(ExAction.PEACEMODE_WALK_RIGHT, new Frame(1025 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_WALK_RIGHT, new Frame(1057 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_WALK_RIGHT, new Frame(1089 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_WALK_RIGHT, new Frame(1121 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_RUN_RIGHT, new Frame(1025 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.ONEHAND_RUN_RIGHT, new Frame(1057 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.TWOHAND_RUN_RIGHT, new Frame(1089 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.BOWHAND_RUN_RIGHT, new Frame(1121 - 1, 4, 0, 100, 112, 6, 0, 100));
            ExPlayer.Add(ExAction.PEACEMODE_STANDUP, new Frame(1153, 5, 1, 100));
             
            
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
            ExPlayer.Add(ExAction.Spell, new Frame(296, 6, 0, 100, 328, 6, 0, 100));
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

            //Mounts
            ExPlayer.Add(ExAction.MountStanding, new Frame(416, 4, 0, 500, 448, 4, 0, 500));
            ExPlayer.Add(ExAction.MountWalking, new Frame(448, 8, 0, 100, 480, 8, 0, 500));
            ExPlayer.Add(ExAction.MountRunning, new Frame(512, 6, 0, 100, 544, 6, 0, 100));
            ExPlayer.Add(ExAction.MountStruck, new Frame(560, 3, 0, 100, 592, 3, 0, 100));
            ExPlayer.Add(ExAction.MountAttack, new Frame(584, 6, 0, 100, 616, 6, 0, 100));

            //Fishing
            //ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_1, new Frame(632, 8, 0, 100));
            //ExPlayer.Add(ExAction.PEACEMODE_SITDOWN_WAIT_1, new Frame(696, 6, 0, 120));
            //ExPlayer.Add(ExAction.PEACEMODE_STANDUP_1, new Frame(744, 8, 0, 100));
            ExPlayer.Add(ExAction.FishingCast, new Frame(433 - 1, 4, 0, 100));
            ExPlayer.Add(ExAction.FishingReel, new Frame(435, 1, 4, 100));
            ExPlayer.Add(ExAction.FishingWait, new Frame(1153, 8, 0, 100)); 
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

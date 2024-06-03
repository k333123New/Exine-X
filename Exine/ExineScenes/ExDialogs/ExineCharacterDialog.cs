using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineObjects;
using Exine.ExineSounds;

namespace Exine.ExineScenes.ExDialogs
{
    /// <summary>
    /// 캐릭터 장비 창
    /// </summary>
    public sealed class ExineCharacterDialog : ExineImageControl
    {
        public MirButton CloseButton, SkillTabButton, MagicTabButton,MakerSkillTabButton, RingSkillTabButton, DivienSkillTabButton, SkillButton;//,  CharacterButton,StatusButton, StateButton;

        public MirButton[] SkillTypeButton = new MirButton[9];//,  CharacterButton,StatusButton, StateButton;


        public ExineImageControl CharacterPage, SkillPage, ActionTree; //StatusPage, StatePage,  ClassImage;

        public ExineLabel NameLabel, GuildLabel, LoverLabel, GoldLabel, WeightLabel, WeightMaxLabel;
        //public ExineLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        //public ExineLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        //public ExineLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        //public ExineLabel HeadingLabel, StatLabel;
        //public MirButton NextButton, BackButton;

        public MirItemCell[] Grid;
        private MirGridType GridType;
        public ExMagicInactiveButton[] exMagicInactiveButtons; //background Magic Icon!(Not Active Magic Icon)
        public ExMagicButton[] Magics;//Active Magic Icon!


        public int StartIndex;
        private UserObject Actor;

        public ExineCharacterDialog(MirGridType gridType, UserObject actor)
        {
            Actor = actor;
            GridType = gridType;

            //Index = 505;
            //Library = Libraries.Title;
            //Movable = true;
            //Sort = true;            
            Index = 0;
            Library = Libraries.PANEL0600;
            //Location = new Point(Settings.ScreenWidth - 264, 0);
            Location = new Point(Settings.ScreenWidth/2, 0+85);
            Movable = false; 
            BeforeDraw += (o, e) => RefreshInterface();
            


            CharacterPage = new ExineImageControl
            {
                //Index = 340,
                Index = 0,
                Parent = this,
                //Library = Libraries.Prguse,
                Library = Libraries.PANEL0600,
                //Location = new Point(8, 90),
                Location = new Point(0, 0),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null) return;

                //실제 장비 착용 보여주는 부분
                ItemInfo RealItem = null;
                if (Grid[(int)EquipmentSlot.Armour].Item != null)
                {
                    if (actor.WingEffect == 1 || actor.WingEffect == 2)
                    {
                        int wingOffset = actor.WingEffect == 1 ? 2 : 4;

                        int genderOffset = actor.Gender == ExineGender.Male ? 0 : 1;

                        Libraries.Prguse2.DrawBlend(1200 + wingOffset + genderOffset, DisplayLocation, Color.White, true, 1F);
                        //Equipment Item으로 변경할것.
                    }
                    //64 75
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Armour].Item.Info, actor.Level, actor.Class, ExineMainScene.ItemInfoList);
                    //Libraries.StateItems.Draw(RealItem.Image, DisplayLocation, Color.White, true, 1F);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X+105+63, DisplayLocation.Y+124-39), Color.White, true, 1F);

                }

                //26 69
                if (Grid[(int)EquipmentSlot.Weapon].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Weapon].Item.Info, actor.Level, actor.Class, ExineMainScene.ItemInfoList);
                    //Libraries.StateItems.Draw(RealItem.Image, DisplayLocation, Color.White, true, 1F);
                    Libraries.StateItems.Draw(RealItem.Image, new Point( DisplayLocation.X+23, DisplayLocation.Y+104), Color.White, true, 1F);

                }

                if (Grid[(int)EquipmentSlot.Helmet].Item != null)
                    Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Helmet].Item.Info.Image, new Point(DisplayLocation.X+170 , DisplayLocation.Y+10), Color.White, true, 1F);
                else
                {
                    int hair = 441 + actor.Hair + (actor.Class == ExineClass.Assassin ? 20 : 0) + (actor.Gender == ExineGender.Male ? 0 : 40);

                    int offSetX = actor.Class == ExineClass.Assassin ? (actor.Gender == ExineGender.Male ? 6 : 4) : 0;
                    int offSetY = actor.Class == ExineClass.Assassin ? (actor.Gender == ExineGender.Male ? 25 : 18) : 0;

                    Libraries.Prguse.Draw(hair, new Point(DisplayLocation.X + offSetX, DisplayLocation.Y + offSetY), Color.White, true, 1F);
                }

                //add 
                if (Grid[(int)EquipmentSlot.Boots].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Boots].Item.Info, actor.Level, actor.Class, ExineMainScene.ItemInfoList);
                    //Libraries.StateItems.Draw(RealItem.Image, DisplayLocation, Color.White, true, 1F);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 23+128, DisplayLocation.Y + 204+37), Color.White, true, 1F);

                }

                //add 
                if (Grid[(int)EquipmentSlot.Belt].Item != null)
                {
                    RealItem = Functions.GetRealItem(Grid[(int)EquipmentSlot.Belt].Item.Info, actor.Level, actor.Class, ExineMainScene.ItemInfoList);
                    //Libraries.StateItems.Draw(RealItem.Image, DisplayLocation, Color.White, true, 1F);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X+309 , DisplayLocation.Y+80 ), Color.White, true, 1F);
                }
            };
            /*
            StatusPage = new ExineImageControl
            {
                Index = 506,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false,
            };
            StatusPage.BeforeDraw += (o, e) =>
            {
                ACLabel.Text = string.Format("{0}-{1}", actor.Stats[Stat.MinAC], actor.Stats[Stat.MaxAC]);
                MACLabel.Text = string.Format("{0}-{1}", actor.Stats[Stat.MinMAC], actor.Stats[Stat.MaxMAC]);
                DCLabel.Text = string.Format("{0}-{1}", actor.Stats[Stat.MinDC], actor.Stats[Stat.MaxDC]);
                MCLabel.Text = string.Format("{0}-{1}", actor.Stats[Stat.MinMC], actor.Stats[Stat.MaxMC]);
                SCLabel.Text = string.Format("{0}-{1}", actor.Stats[Stat.MinSC], actor.Stats[Stat.MaxSC]);
                HealthLabel.Text = string.Format("{0}/{1}", actor.HP, actor.Stats[Stat.HP]);
                ManaLabel.Text = string.Format("{0}/{1}", actor.MP, actor.Stats[Stat.MP]);
                CritRLabel.Text = string.Format("{0}%", actor.Stats[Stat.CriticalRate]);
                CritDLabel.Text = string.Format("{0}", actor.Stats[Stat.CriticalDamage]);
                AttkSpdLabel.Text = string.Format("{0}", actor.Stats[Stat.AttackSpeed]);
                AccLabel.Text = string.Format("+{0}", actor.Stats[Stat.Accuracy]);
                AgilLabel.Text = string.Format("+{0}", actor.Stats[Stat.Agility]);
                LuckLabel.Text = string.Format("{0}", actor.Stats[Stat.Luck]);
            };

            StatePage = new ExineImageControl
            {
                Index = 507,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };
            StatePage.BeforeDraw += (o, e) =>
            {
                ExpPLabel.Text = string.Format("{0:0.##%}", actor.Experience / (double)actor.MaxExperience);
                BagWLabel.Text = string.Format("{0}/{1}", actor.CurrentBagWeight, actor.Stats[Stat.BagWeight]);
                WearWLabel.Text = string.Format("{0}/{1}", actor.CurrentWearWeight, actor.Stats[Stat.WearWeight]);
                HandWLabel.Text = string.Format("{0}/{1}", actor.CurrentHandWeight, actor.Stats[Stat.HandWeight]);
                MagicRLabel.Text = string.Format("+{0}", actor.Stats[Stat.MagicResist]);
                PoisonResLabel.Text = string.Format("+{0}", actor.Stats[Stat.PoisonResist]);
                HealthRLabel.Text = string.Format("+{0}", actor.Stats[Stat.HealthRecovery]);
                ManaRLabel.Text = string.Format("+{0}", actor.Stats[Stat.SpellRecovery]);
                PoisonRecLabel.Text = string.Format("+{0}", actor.Stats[Stat.PoisonRecovery]);
                HolyTLabel.Text = string.Format("+{0}", actor.Stats[Stat.Holy]);
                FreezeLabel.Text = string.Format("+{0}", actor.Stats[Stat.Freezing]);
                PoisonAtkLabel.Text = string.Format("+{0}", actor.Stats[Stat.PoisonAttack]);
            };
            */

            /*
            SkillPage = new ExineImageControl
            {
                Index = 508,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };*/
            SkillPage = new ExineImageControl
            {
                Index = 0,
                Parent = this,
                Library = Libraries.PANEL0601,
                Location = new Point(0, 0),
                Visible = false
            };

            //ActionTree
            ActionTree = new ExineImageControl
            {
                Index = 0,
                Parent = SkillPage,
                Library = Libraries.ActionTree,
                Location = new Point(62+2, 20+32+8+23-2),
                Visible = true,
            };

            //606
            void SkillTabIndexInit()
            {
                MagicTabButton.Index = 0;
                SkillTabButton.Index = 9;
                MakerSkillTabButton.Index = 18;
                RingSkillTabButton.Index = 27;
                DivienSkillTabButton.Index = 36;
            }
           
            MagicTabButton = new MirButton
            {
                Index = 0,
                PressedIndex = 5,
                Parent = SkillPage,
                Library = Libraries.PANEL0606,
                Location = new Point(356, 72 * 0),
                Visible = true
            };
            MagicTabButton.Click += (o, e) =>
            {
                SkillTabIndexInit();
                MagicTabButton.Index = MagicTabButton.PressedIndex;

                SkillPage.Library = Libraries.PANEL0602;
                SkillTypeButton = new MirButton[9];

                for (int i = 0; i < SkillTypeButton.Length; i++)
                {
                    int x = 0;
                    int y = 0;
                    y = (i / 7) * 20;
                    x = (i * 46) % 322;

                    SkillTypeButton[i] = new MirButton
                    {
                        Index = i * 8 + 1,
                        PressedIndex = i * 8 + 1 + 5,
                        Parent = SkillPage,
                        Library = SkillPage.Library,
                        Location = new Point(x + 21, y + 35),
                        Visible = true
                    };
                }
                ActionTree.Index = 0;
                ActionTree.Visible = true;
                SkillTypeButton[0].Click += (o, e) => { ActionTree.Index = 0;  };
                SkillTypeButton[1].Click += (o, e) => { ActionTree.Index = 1; };
                SkillTypeButton[2].Click += (o, e) => { ActionTree.Index = 2; };
                SkillTypeButton[3].Click += (o, e) => { ActionTree.Index = 3; };
                SkillTypeButton[4].Click += (o, e) => { ActionTree.Index = 4; };
                SkillTypeButton[5].Click += (o, e) => { ActionTree.Index = 5; };
                SkillTypeButton[6].Click += (o, e) => { ActionTree.Index = 6; };
                SkillTypeButton[7].Click += (o, e) => { ActionTree.Index = 7; };
                SkillTypeButton[8].Click += (o, e) => { ActionTree.Index = 8; };
                //RefreshInterface();
            };
           

            SkillTabButton = new MirButton
            {
                Index = 9,
                PressedIndex = 14,
                Parent = SkillPage,
                Library = Libraries.PANEL0606,
                Location = new Point(356, 72 * 1),
                Visible = true
            };
            SkillTabButton.Click += (o, e) =>
            {
                SkillTabIndexInit();
                SkillTabButton.Index = SkillTabButton.PressedIndex;

                SkillPage.Library = Libraries.PANEL0601;
                SkillTypeButton = new MirButton[9];

                for (int i = 0; i < SkillTypeButton.Length; i++)
                {
                    int x = 0;
                    int y = 0;
                    y = (i / 7) * 20;
                    x = (i * 46) % 322;

                    SkillTypeButton[i] = new MirButton
                    {
                        Index = i * 8 + 1,
                        PressedIndex = i * 8 + 1 + 5,
                        Parent = SkillPage,
                        Library = SkillPage.Library,
                        Location = new Point(x + 21, y + 35),
                        Visible = true
                    };
                }
                ActionTree.Index = 9;
                ActionTree.Visible = true;
                SkillTypeButton[0].Click += (o, e) => { ActionTree.Index = 9; };
                SkillTypeButton[1].Click += (o, e) => { ActionTree.Index = 10; };
                SkillTypeButton[2].Click += (o, e) => { ActionTree.Index = 11; };
                SkillTypeButton[3].Click += (o, e) => { ActionTree.Index = 12; };
                SkillTypeButton[4].Click += (o, e) => { ActionTree.Index = 13; };
                SkillTypeButton[5].Click += (o, e) => { ActionTree.Index = 14; };
                SkillTypeButton[6].Click += (o, e) => { ActionTree.Index = 15; };
                SkillTypeButton[7].Click += (o, e) => { ActionTree.Index = 16; };
                SkillTypeButton[8].Click += (o, e) => { ActionTree.Index = 17; };
                //RefreshInterface();//k333123
            };

            MakerSkillTabButton = new MirButton
            {
                Index = 18,
                PressedIndex = 23,
                Parent = SkillPage,
                Library = Libraries.PANEL0606,
                Location = new Point(356, 72 * 2),
                Visible = true
            };
            MakerSkillTabButton.Click += (o, e) =>
            {
                SkillTabIndexInit();
                MakerSkillTabButton.Index = MakerSkillTabButton.PressedIndex;

                SkillPage.Library = Libraries.PANEL0603;
                SkillTypeButton = new MirButton[5];

                for (int i = 0; i < SkillTypeButton.Length; i++)
                {
                    int x = 0;
                    int y = 0;
                    y = (i / 7) * 20;
                    x = (i * 46) % 322;

                    SkillTypeButton[i] = new MirButton
                    {
                        Index = i * 8 + 1,
                        PressedIndex = i * 8 + 1 + 5,
                        Parent = SkillPage,
                        Library = SkillPage.Library,
                        Location = new Point(x + 21, y + 35),
                        Visible = true
                    };
                }
                ActionTree.Index = 18;
                ActionTree.Visible = true;
                SkillTypeButton[0].Click += (o, e) => { ActionTree.Index = 18; };
                SkillTypeButton[1].Click += (o, e) => { ActionTree.Index = 19; };
                SkillTypeButton[2].Click += (o, e) => { ActionTree.Index = 20; };
                SkillTypeButton[3].Click += (o, e) => { ActionTree.Index = 21; };
                SkillTypeButton[4].Click += (o, e) => { ActionTree.Index = 22; };
                //RefreshInterface();
            };

            RingSkillTabButton = new MirButton
            {
                Index = 27,
                PressedIndex = 32,
                Parent = SkillPage,
                Library = Libraries.PANEL0606,
                Location = new Point(356, 72 * 3),
                Visible = true
            };
            RingSkillTabButton.Click += (o, e) =>
            {
                SkillTabIndexInit();
                RingSkillTabButton.Index = RingSkillTabButton.PressedIndex;

                SkillPage.Library = Libraries.PANEL0604;

                ActionTree.Visible = false; 
                for (int i = 0; i < SkillTypeButton.Length; i++)
                {
                    SkillTypeButton[i].Visible = false;
                    SkillTypeButton[i].Click += (o, e) => {  };
                }
                //RefreshInterface();
            };

            DivienSkillTabButton = new MirButton
            {
                Index = 36,
                PressedIndex = 41,
                Parent = SkillPage,
                Library = Libraries.PANEL0606,
                Location = new Point(356, 72 * 4),
                Visible = true
            };
            DivienSkillTabButton.Click += (o, e) =>
            {
                SkillTabIndexInit();
                RingSkillTabButton.Index = RingSkillTabButton.PressedIndex;

                SkillPage.Library = Libraries.PANEL0605;
                ActionTree.Visible = false;
                 
                for (int i = 0; i < SkillTypeButton.Length; i++)
                {
                    SkillTypeButton[i].Visible = false;
                    SkillTypeButton[i].Click += (o, e) => { };
                }
                //RefreshInterface();
            };

            //MagicTabButton,MakerSkillTab, RingSkillTab, CreationSkillTab,
             

            /*
           CharacterButton = new MirButton
           {
               Index = 500,
               Library = Libraries.Title,
               Location = new Point(8, 70),
               Parent = this,
               PressedIndex = 500,
               Size = new Size(64, 20),
               Sound = SoundList.ButtonA,
           };
           CharacterButton.Click += (o, e) => ShowCharacterPage();

           StatusButton = new MirButton
           {
               Library = Libraries.Title,
               Location = new Point(70, 70),
               Parent = this,
               PressedIndex = 501,
               Size = new Size(64, 20),
               Sound = SoundList.ButtonA
           };
           StatusButton.Click += (o, e) => ShowStatusPage();

           StateButton = new MirButton
           {
               Library = Libraries.Title,
               Location = new Point(132, 70),
               Parent = this,
               PressedIndex = 502,
               Size = new Size(64, 20),
               Sound = SoundList.ButtonA
           };
           StateButton.Click += (o, e) => ShowStatePage();
           */
            SkillButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(194, 70),
                Parent = this,
                PressedIndex = 503,
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Visible =false, //k333123 add
            };
            //SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
            {
                HoverIndex = 90,
                Index = 89,
                Location = new Point(241+102+20, 3+434-16),
                Library = Libraries.PANEL0600,
                Parent = this,
                PressedIndex = 91,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) =>
            {
                Hide();
                ExineMainScene.Scene.ExInventoryDialog.Hide();
            };

            NameLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(0, 12),
                Size = new Size(264, 20),
                NotControl = true,
            };
            GuildLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(0, 33),
                Size = new Size(264, 30),
                NotControl = true,
            };
            GoldLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(0+231+74, 55+207+6),
                Size = new Size(111, 14),
                Sound = SoundList.Gold,
                NotControl = true,
                //Sound = SoundList.Gold,
                //Text = "111",
            };
            
            GoldLabel.Click += (o, e) =>
            {
                if (ExineMainScene.SelectedCell == null)
                    ExineMainScene.PickedUpGold = !ExineMainScene.PickedUpGold && ExineMainScene.Gold > 0;
            };

            WeightLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(0 + 231 + 74-32, 55 + 207 + 6-27),
                Size = new Size(111, 14), 
                NotControl = true,
                //Sound = SoundList.Gold,
                //Text = "111",
            };
            WeightMaxLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(0 + 231 + 74, 55 + 207 + 6-27),
                Size = new Size(111, 14), 
                NotControl = true,
                //Sound = SoundList.Gold,
                //Text = "111",
            };

            /*
            ClassImage = new ExineImageControl
            {
                Index = 100,
                Library = Libraries.Prguse,
                Location = new Point(15, 33),
                Parent = this,
                NotControl = true,
            };*/

            Grid = new MirItemCell[Enum.GetNames(typeof(EquipmentSlot)).Length];


            Grid[(int)EquipmentSlot.Helmet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203 - 21, 7 + 29),
            };


         

            Grid[(int)EquipmentSlot.Weapon] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(123-65, 7+135),
            };

            Grid[(int)EquipmentSlot.BraceletL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(8 + 99, 170 - 117),
            };

            Grid[(int)EquipmentSlot.BraceletR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203 + 53, 170 - 117),
            };
            Grid[(int)EquipmentSlot.Necklace] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203 - 95, 98 - 5),
            };

            Grid[(int)EquipmentSlot.Armour] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(163 + 18, 7 + 135),
            };
             
        

            Grid[(int)EquipmentSlot.RingL] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(8+ 105, 206-18),
            };

            Grid[(int)EquipmentSlot.RingR] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203+52, 206-18),
            };

            Grid[(int)EquipmentSlot.Boots] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Boots,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(48 + 128, 242),
            };

            Grid[(int)EquipmentSlot.Torch] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Torch,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203 - 140, 134 + 110),
            };

            //Belt를 Sheild로 변경할 예정임.
            Grid[(int)EquipmentSlot.Belt] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(88+ 229, 242-97),
            };

            /*
            Grid[(int)EquipmentSlot.Amulet] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Amulet,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(8, 242),
            };*/


            /*
            Grid[(int)EquipmentSlot.Belt] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(88, 242),
            };


            Grid[(int)EquipmentSlot.Stone] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Stone,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(128, 242),
            };

            Grid[(int)EquipmentSlot.Mount] = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Mount,
                GridType = gridType,
                Parent = CharacterPage,
                Location = new Point(203, 62),
            };*/
            /*
            // STATS I
            HealthLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 20),
                NotControl = true,
                Text = "0-0",
            };

            ManaLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 38),
                NotControl = true,
                Text = "0-0",
            };

            ACLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 56),
                NotControl = true,
                Text = "0-0",
            };

            MACLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 74),
                NotControl = true,
                Text = "0-0",
            };
            DCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 92),
                NotControl = true,
                Text = "0-0"
            };
            MCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 110),
                NotControl = true,
                Text = "0/0"
            };
            SCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 128),
                NotControl = true,
                Text = "0/0"
            };
            //Breezer - New Labels
            CritRLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 146),
                NotControl = true
            };
            CritDLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 164),
                NotControl = true
            };
            AttkSpdLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 182),
                NotControl = true
            };
            AccLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 200),
                NotControl = true
            };
            AgilLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 218),
                NotControl = true
            };
            LuckLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126, 236),
                NotControl = true
            };
            // STATS II 
            ExpPLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 20),
                NotControl = true,
                Text = "0-0",
            };

            BagWLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 38),
                NotControl = true,
                Text = "0-0",
            };

            WearWLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 56),
                NotControl = true,
                Text = "0-0",
            };

            HandWLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 74),
                NotControl = true,
                Text = "0-0",
            };
            MagicRLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 92),
                NotControl = true,
                Text = "0-0"
            };
            PoisonResLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 110),
                NotControl = true,
                Text = "0/0"
            };
            HealthRLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 128),
                NotControl = true,
                Text = "0/0"
            };
            //Breezer
            ManaRLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 146),
                NotControl = true
            };
            PoisonRecLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 164),
                NotControl = true
            };
            HolyTLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 182),
                NotControl = true
            };
            FreezeLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 200),
                NotControl = true
            };
            PoisonAtkLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(126, 218),
                NotControl = true
            };
            */

            //k333123 Skill Page Button

            exMagicInactiveButtons = new ExMagicInactiveButton[12];
            for(int i=0;i<exMagicInactiveButtons.Length;i++)
            {
                
                exMagicInactiveButtons[i] = new ExMagicInactiveButton();
                exMagicInactiveButtons[i].Parent = SkillPage;
                exMagicInactiveButtons[i].Visible = true;
                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.ArtsIcon; 
                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = false;
                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(0, 0);
                exMagicInactiveButtons[i].MagicInactiveIcon.GrayScale = true;
                exMagicInactiveButtons[i].MagicInactiveIcon.Border = false;
                exMagicInactiveButtons[i].MagicInactiveIcon.BorderColour = Color.DarkGreen; 
            }
            

            /*
            Magics = new MagicButton[7]; 
            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new MagicButton 
                { 
                    Parent = SkillPage, 
                    Visible = false, 
                    Location = new Point(8, 8 + i * 33),
                    HeroMagic = gridType == MirGridType.HeroEquipment
                };
            */
            Magics = new ExMagicButton[12];
            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new ExMagicButton
                {
                    Parent = SkillPage,
                    Visible = false,
                    //Location = new Point(8, 8 + i * 33),
                    Location = new Point(0, 0),
                    HeroMagic = false
                };


            /*
            NextButton = new MirButton
            {
                Index = 396,
                Location = new Point(140, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 397,
                Sound = SoundList.ButtonA,
            };
            NextButton.Click += (o, e) =>
            {
                if (StartIndex + 7 >= actor.Magics.Count) return;

                StartIndex += 7;
                RefreshInterface();
            };

            BackButton = new MirButton
            {
                Index = 398,
                Location = new Point(90, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 399,
                Sound = SoundList.ButtonA,
            };
            BackButton.Click += (o, e) =>
            {
                if (StartIndex - 7 < 0) return;

                StartIndex -= 7;
                RefreshInterface();
            };*/
        }

       

        public override void Show()
        {
            if (Visible) return;
            Visible = true;
        }

        public override void Hide()
        {
            ExineMainScene.Scene.SocketDialog.Hide();
            base.Hide();
        }

        public void ShowCharacterPage()
        {
            CharacterPage.Visible = true;
            //StatusPage.Visible = false;
            //StatePage.Visible = false;
            SkillPage.Visible = false;
            //CharacterButton.Index = 500;
            //StatusButton.Index = -1;
            //StateButton.Index = -1;
            SkillButton.Index = -1;
        }
        /*
        private void ShowStatusPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = 501;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatePage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = 502;
            SkillButton.Index = -1;
        }
        */
        public void ShowSkillPage()
        {
            Console.WriteLine("ShowSkillPage()");
            CharacterPage.Visible = false;
            //StatusPage.Visible = false;
            //StatePage.Visible = false;
            SkillPage.Visible = true;
            //CharacterButton.Index = -1;
            //StatusButton.Index = -1;
            //StateButton.Index = -1;
            SkillButton.Index = 503;
            StartIndex = 0;
        }
       

        private void RefreshInterface()
        {
            Console.WriteLine("RefreshInterface()");
            int offSet = Actor.Gender == ExineGender.Male ? 0 : 1;
            /*
            Index = 504;// +offSet;
            CharacterPage.Index = 340 + offSet;
            */
            /*
            switch (Actor.Class)
            {
                case ExineClass.Warrior:
                    ClassImage.Index = 100;// + offSet * 5;
                    break;
                case ExineClass.Wizard:
                    ClassImage.Index = 101;// + offSet * 5;
                    break;
                case ExineClass.Taoist:
                    ClassImage.Index = 102;// + offSet * 5;
                    break;
                case ExineClass.Assassin:
                    ClassImage.Index = 103;// + offSet * 5;
                    break;
                case ExineClass.Archer:
                    ClassImage.Index = 104;// + offSet * 5;
                    break;
            }
            */
            NameLabel.Text = Actor.Name;
            GuildLabel.Text = Actor.GuildName + " " + Actor.GuildRankName;

            int positionX1 = 68;
            int positionX2 = 108;//maybe
            int positionX3 = 148;
            int positionX4 = 188;//maybe
            int positionX5 = 228;
            int positionX6 = 268;//maybe
            int positionX7 = 308;

            int positionY1 = 98;
            int positionY2 = 168;
            int positionY3 = 237;
            int positionY4 = 308;
            int positionY5 = 378;

            int basicIndex = 0;
            //for (int i = 0; i < Magics.Length; i++)
            for (int i = 0; i < exMagicInactiveButtons.Length; i++) 
            { 
                /*
                 if (i + StartIndex >= Actor.Magics.Count)
                 {
                   MagicInactiveIcons[i].Visible = false;
                   continue;
                 }
                */

                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = false;

                //Assign img at update function???
                if (SkillPage.Library == Libraries.PANEL0601) //skill
                {
                    exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.ArtsIcon;
                    if (ActionTree.Index == 9)//sword
                    {
                        basicIndex = 1;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                exMagicInactiveButtons[i].Hint = string.Format("slash\n\n basic sword skill");
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 10)//assasin weapon
                    {
                        basicIndex = 1 + 8;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 11)//Spear Weapon
                    {
                        basicIndex = 1 + 8 + 10;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                                //add 1,7

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 12)//Heavy Weapon
                    {
                        basicIndex = 1 + 8 + 10 + 8;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 11:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 13)//Bow Weapon
                    {
                        basicIndex = 1 + 8 + 10 + 8 + 12;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 14)//Hand Weapon
                    {
                        basicIndex = 1 + 8 + 10 + 8 + 12 + 9;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 15)//Guard
                    {
                        basicIndex = 1 + 8 + 10 + 8 + 12 + 9 + 11;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 11:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 16)//Search
                    {
                        basicIndex = 1 + 8 + 10 + 8 + 12 + 9 + 11 + 12;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 17)//Trap
                    {
                        //미구현
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                }

                else if (SkillPage.Library == Libraries.PANEL0602) //magic
                {
                    exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.MagicIcon;
                    if (ActionTree.Index == 0)//Fire Magic
                    {
                        basicIndex = 1;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 1)//Water Magic
                    {
                        basicIndex = 1 + 8;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 2)//Air Magic
                    {
                        basicIndex = 1 + 8 + 8;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 3)//Land Magic
                    {
                        basicIndex = 1 + 8 + 8 + 7;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 4)//Curse Magic
                    {
                        basicIndex = 1 + 8 + 8 + 7 + 10;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 5)//Bless Magic
                    {
                        basicIndex = 1 + 8 + 8 + 7 + 10 + 9;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 6)//Status abnormality Magic
                    {
                        basicIndex = 1 + 8 + 8 + 7 + 10 + 9 + 9;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 7)//Healing Magic
                    {
                        basicIndex = 1 + 8 + 8 + 7 + 10 + 9 + 9 + 10;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 8)//summoner Magic
                    {
                        //미구현
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                }

                else if (SkillPage.Library == Libraries.PANEL0603) //maker
                {//maker (1~52 items)

                    
                    exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.ManufactureSkillIcon;
                    //우선 미구현
                    if (ActionTree.Index == 18)//weapon make
                    {
                       /*mk1
                   * 1,1  3,1 5,1 7,1
                   * 3,2
                   * 3,3
                   * 3,4
                   * 1,5 3,5 5,5
                   */
                      basicIndex = 1;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 19)//guard maker
                    {
                        /* mk2
                     * 1,1 3,1 5,1 7,1
                     * 1,2 3,2
                     * 3,3
                     * 3,4
                     * 1,5 3,5 5,5 7,5
                     */
                        basicIndex = 1 + 10;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 11:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 20)//accessory maker
                    {
                        /* mk3
                     * 3,1 7,1
                     * 3,2 5,2
                     * 3,3 7,3
                     * 3,4 7,4
                     * 1,5 3,5 5,5
                     */ 
                        basicIndex = 1 + 10 + 12;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;
                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 21)//magic equipmen
                    {
                        /*mk4
                     * 1,1 4,1 7,1
                     * 3,2 5,2
                     * 3,3 5,3
                     * 3,4 5,4
                     * 3,5 5,5
                     */
                        basicIndex = 1 + 10 + 12 + 11;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX1, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX7, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX3, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 10:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX5, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                    else if (ActionTree.Index == 22)//magic item
                    {/*   
                     * mk5
                     * 4,1
                     * 2,2 4,2 6,2
                     * 2,3 4,3 6,3
                     * 2,4 6,4
                     * 4,5 ->?
                     */ 
                        basicIndex = 1 + 10 + 12 + 11 +11;
                        switch (i)
                        {
                            case 0:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY1);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 1:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 2:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 3:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY2);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 4:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 5:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 6:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY3);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 7:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX2, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 8:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX6, positionY4);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = i + basicIndex;
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            case 9:
                                exMagicInactiveButtons[i].MagicInactiveIcon.Location = new Point(positionX4, positionY5);
                                exMagicInactiveButtons[i].MagicInactiveIcon.Index = 0; //must have check!
                                exMagicInactiveButtons[i].MagicInactiveIcon.Visible = true;
                                break;

                            default:
                                break;
                        }
                    }
                }

                else if (SkillPage.Library == Libraries.PANEL0604) //Ring
                {
                    //ring skill page
                    //not implement (No Icon)
                    //maker (1~52 items)

                    exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.RingSkillIcon;  
                }
                
                else if (SkillPage.Library == Libraries.PANEL0605) //Divine
                {
                    //Divine skill page
                    //(ICON : 1~20)
                    //not implement (Skill Tree)
                    //maker (1~52 items)

                    exMagicInactiveButtons[i].MagicInactiveIcon.Library = Libraries.DivineSkillIcon; 
                }

                // MagicInactiveIcons[i].Visible = true; //k333123 test
                // Magics[i].Visible = true; //k333123 test
                // Magics[i].Visible = false; //k333123 임시로 꺼둠
                // Magics[i].Update(Actor.Magics[i + StartIndex]); //assign icon
            }
        }

        private void ExineCharacterDialog_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Process()
        { 
            GoldLabel.Text = ExineMainScene.Gold.ToString("###,###,##0");

            WeightLabel.Text = MapObject.User.CurrentBagWeight.ToString();
            WeightMaxLabel.Text= MapObject.User.Stats[Stat.BagWeight].ToString();
        }

        public MirItemCell GetCell(ulong id)
        {

            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }

    }
}

using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineObjects;
using Exine.ExineSounds;

namespace Exine.ExineScenes.ExDialogs
{
    /// <summary>
    /// 캐릭터 장비 창
    /// </summary>
    public sealed class ExineStateDialog : ExineImageControl
    {
        public ExineButton CloseButton, StatusButton,  RingButton, FamilyButton, AKAButton, SkillButton;//,  CharacterButton,StatusButton, StateButton;
        public ExineImageControl StatusPage, RingPage, FamilyPage, AKAPage, SkillPage;//,  ClassImage;

        public ExineLabel NameLabel, GuildLabel, LoverLabel;//, GoldLabel, WeightLabel, WeightMaxLabel;
        public ExineLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        public ExineLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        public ExineLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        public ExineLabel HeadingLabel, StatLabel;

        //family label
        //Father/Mother mean lover
        public ExineLabel GrandFatherLabel, GrandMotherLabel, FatherLabel, MotherLabel, Daughter1stLabel, Daughter2ndLabel, Daughter3rdLabel, Son1stLabel, Son2ndLabel, Son3rdLabel;


        public ExineButton NextButton, BackButton;

        public MirItemCell[] Grid;
        private MirGridType GridType;
        public ExMagicButton[] Magics;

        public int StartIndex;
        private UserObject Actor;

        public ExineStateDialog(MirGridType gridType, UserObject actor)
        {
            Actor = actor;
            GridType = gridType; 
            Index = 0;
            Library = Libraries.PANEL0500; 
            Location = new Point(0+113, 0+85);
            Movable = false; 
            BeforeDraw += (o, e) => RefreshInterface();

            StatusPage = new ExineImageControl
            {
                Index = 0,
                Parent = this,
                Library = Libraries.PANEL0500,
                Location = new Point(0, 0),
                Visible = true,
            };
            StatusPage.BeforeDraw += (o, e) =>
            { 
                ACLabel.Text = string.Format("물방{0}-{1}", actor.Stats[Stat.MinAC], actor.Stats[Stat.MaxAC]);
                MACLabel.Text = string.Format("마방{0}-{1}", actor.Stats[Stat.MinMAC], actor.Stats[Stat.MaxMAC]);
                DCLabel.Text = string.Format("물공{0}-{1}", actor.Stats[Stat.MinDC], actor.Stats[Stat.MaxDC]);
                MCLabel.Text = string.Format("마공{0}-{1}", actor.Stats[Stat.MinMC], actor.Stats[Stat.MaxMC]);
                SCLabel.Text = string.Format("정신공격력{0}-{1}", actor.Stats[Stat.MinSC], actor.Stats[Stat.MaxSC]);
                HealthLabel.Text = string.Format("체력{0}/{1}", actor.HP, actor.Stats[Stat.HP]);
                ManaLabel.Text = string.Format("마력{0}/{1}", actor.MP, actor.Stats[Stat.MP]);
                CritRLabel.Text = string.Format("크리비율{0}%", actor.Stats[Stat.CriticalRate]);
                CritDLabel.Text = string.Format("크리뎀{0}", actor.Stats[Stat.CriticalDamage]);
                AttkSpdLabel.Text = string.Format("공속{0}", actor.Stats[Stat.AttackSpeed]);
                AccLabel.Text = string.Format("정확도+{0}", actor.Stats[Stat.Accuracy]);
                AgilLabel.Text = string.Format("민첩+{0}", actor.Stats[Stat.Agility]);
                LuckLabel.Text = string.Format("운{0}", actor.Stats[Stat.Luck]);

                ExpPLabel.Text = string.Format("경험치{0:0.##%}", actor.Experience / (double)actor.MaxExperience);
                BagWLabel.Text = string.Format("가방무게{0}/{1}", actor.CurrentBagWeight, actor.Stats[Stat.BagWeight]);
                WearWLabel.Text = string.Format("입는무게{0}/{1}", actor.CurrentWearWeight, actor.Stats[Stat.WearWeight]);
                HandWLabel.Text = string.Format("손무게{0}/{1}", actor.CurrentHandWeight, actor.Stats[Stat.HandWeight]);
                MagicRLabel.Text = string.Format("마법저항+{0}", actor.Stats[Stat.MagicResist]);
                PoisonResLabel.Text = string.Format("독저항+{0}", actor.Stats[Stat.PoisonResist]);
                HealthRLabel.Text = string.Format("체력회복+{0}", actor.Stats[Stat.HealthRecovery]);
                ManaRLabel.Text = string.Format("마법회복+{0}", actor.Stats[Stat.SpellRecovery]);
                PoisonRecLabel.Text = string.Format("독회복+{0}", actor.Stats[Stat.PoisonRecovery]);
                HolyTLabel.Text = string.Format("성스러운+{0}", actor.Stats[Stat.Holy]);
                FreezeLabel.Text = string.Format("동결+{0}", actor.Stats[Stat.Freezing]);
                PoisonAtkLabel.Text = string.Format("독공격+{0}", actor.Stats[Stat.PoisonAttack]);
            };

            RingPage = new ExineImageControl
            {
                Index = 0,
                Parent = this,
                Library = Libraries.PANEL0504,
                Location = new Point(0, 0),
                Visible = false
            };
            RingPage.BeforeDraw += (o, e) =>
            { 

            };

            FamilyPage = new ExineImageControl
            {
                Index = 0,
                Parent = this,
                Library = Libraries.PANEL0505,
                Location = new Point(0, 0),
                Visible = false
            };
            FamilyPage.BeforeDraw += (o, e) =>
            {

            };

            AKAPage = new ExineImageControl
            {
                Index = 0,
                Parent = this,
                Library = Libraries.PANEL0506,
                Location = new Point(0, 0),
                Visible = false
            };
            AKAPage.BeforeDraw += (o, e) =>
            {

            }; 

            SkillPage = new ExineImageControl
            {
                Index = 508,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };
             
            StatusButton = new ExineButton
            {
                Library = Libraries.PANEL0503,
                Location = new Point(365-10, 50-13),
                Parent = this,
                PressedIndex = 0,
                Size = new Size(36, 74),
                Sound = SoundList.ButtonA,
                Visible = true
            };
            StatusButton.Click += (o, e) => ShowStatusPage();

            RingButton = new ExineButton
            {
                Library = Libraries.PANEL0503,
                Location = new Point(365-10, 124- 13),
                Parent = this,
                PressedIndex = 8,
                Size = new Size(36, 74),
                Sound = SoundList.ButtonA
            };
            RingButton.Click += (o, e) => ShowRingPage();

            FamilyButton = new ExineButton
            {
                Library = Libraries.PANEL0503,
                Location = new Point(365-10, 198- 13),
                Parent = this,
                PressedIndex = 16,
                Size = new Size(36, 74),
                Sound = SoundList.ButtonA
            };
            FamilyButton.Click += (o, e) => ShowFamilyPage();


            AKAButton = new ExineButton
            {
                Library = Libraries.PANEL0503,
                Location = new Point(365-10, 272- 13),
                Parent = this,
                PressedIndex = 24,
                Size = new Size(36, 74),
                Sound = SoundList.ButtonA
            };
            AKAButton.Click += (o, e) => ShowAKAPage();

            SkillButton = new ExineButton
            {
                Library = Libraries.Title,
                Location = new Point(365, 70),
                Parent = this,
                PressedIndex = 503,
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA,
                Visible = false//k333123
            };
            //SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new ExineButton
            {
                HoverIndex = 90,
                Index = 89,
                Location = new Point(365-1, 3+434-17+2),
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
                Location = new Point(0+136, 12+43),
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
               
            // STATS I
            HealthLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126-61, 20+117),
                NotControl = true,
                Text = "0-0",
            };

            ManaLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126+111, 38+97),
                NotControl = true,
                Text = "0-0",
            };

            //물방
            ACLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126-13, 56-276),
                NotControl = true,
                Text = "0-0",
            };

           
            DCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126-13, 92-219),
                NotControl = true,
                Text = "0-0"
            };

            //명중률
            AccLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126-13, 200+153),
                NotControl = true
            };

            //민첩
            AgilLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126-66, 218+28),
                NotControl = true
            };
            LuckLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126+95, 236+9),
                NotControl = true
            };

            // STATS II 
            ExpPLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126-23, 20+236-147),
                NotControl = true,
                Text = "0-0",
            };
   
            MagicRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126+157, 92 + 236-19),
                NotControl = true,
                Text = "0-0"
            };

            //보류
            #region 보류
            MACLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 74),
                NotControl = true,
                Text = "0-0",
            };
            MCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 110),
                NotControl = true,
                Text = "0/0"
            };
            SCLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 128),
                NotControl = true,
                Text = "0/0"
            };
            //Breezer - New Labels
            CritRLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 146),
                NotControl = true
            };
            CritDLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 164),
                NotControl = true
            };
            AttkSpdLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(126 + 255, 182),
                NotControl = true
            };
            BagWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 38 + 236),
                NotControl = true,
                Text = "0-0",
            };

            WearWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 56 + 236),
                NotControl = true,
                Text = "0-0",
            };

            HandWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 74 + 236),
                NotControl = true,
                Text = "0-0",
            };
            PoisonResLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 110 + 236),
                NotControl = true,
                Text = "0/0"
            };
            HealthRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 128 + 236),
                NotControl = true,
                Text = "0/0"
            };
            //Breezer
            ManaRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 146 + 236),
                NotControl = true
            };
            PoisonRecLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 164 + 236),
                NotControl = true
            };
            HolyTLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 182 + 236),
                NotControl = true
            };
            FreezeLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 200 + 236),
                NotControl = true
            };
            PoisonAtkLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126 + 255, 218 + 236),
                NotControl = true
            };
            #endregion 보류

            Magics = new ExMagicButton[7];

            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new ExMagicButton
                { 
                    Parent = SkillPage, 
                    Visible = false, 
                    Location = new Point(8, 8 + i * 33),
                    HeroMagic = false
                };
             
            NextButton = new ExineButton
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

            BackButton = new ExineButton
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
            }; 
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

        
        
        private void ShowStatusPage()
        {
            StatusPage.Visible = true;
            RingPage.Visible = false;
            FamilyPage.Visible = false;
            AKAPage.Visible = false;
            SkillPage.Visible = false;
            StatusButton.Index = 0+4;
            RingButton.Index = 8;
            FamilyButton.Index = 16;
            AKAButton.Index = 24;
            SkillButton.Index = -1;
        }

        private void ShowRingPage()
        {
            StatusPage.Visible = false;
            RingPage.Visible = true;
            FamilyPage.Visible = false;
            AKAPage.Visible = false;
            SkillPage.Visible = false;
            StatusButton.Index = 0;
            RingButton.Index = 8 + 4;
            FamilyButton.Index = 16;
            AKAButton.Index = 24;
            SkillButton.Index = -1;
        }

        private void ShowFamilyPage()
        {
            StatusPage.Visible = false;
            RingPage.Visible = false;
            FamilyPage.Visible = true;
            AKAPage.Visible = false;
            SkillPage.Visible = false;
            StatusButton.Index = 0;
            RingButton.Index = 8;
            FamilyButton.Index = 16 + 4;
            AKAButton.Index = 24;
            SkillButton.Index = -1;
        }
         
        private void ShowAKAPage()
        {
            StatusPage.Visible = false;
            RingPage.Visible = false;
            FamilyPage.Visible = false;
            AKAPage.Visible = true;
            SkillPage.Visible = false;
            StatusButton.Index = 0;
            RingButton.Index = 8;
            FamilyButton.Index = 16;
            AKAButton.Index = 24 + 4;
            SkillButton.Index = -1;
        }

        public void ShowSkillPage()
        {
            StatusPage.Visible = false;
            RingPage.Visible = false;
            FamilyPage.Visible = false;
            AKAPage.Visible = false;
            SkillPage.Visible = true;
            StatusButton.Index = -1;
            RingButton.Index = -1;
            FamilyButton.Index = -1;
            AKAButton.Index = -1;
            SkillButton.Index = 503;
            StartIndex = 0;
        }
        
        private void RefreshInterface()
        {
            int offSet = Actor.Gender == ExineGender.Male ? 0 : 1;
            
            NameLabel.Text = Actor.Name;
            GuildLabel.Text = Actor.GuildName + " " + Actor.GuildRankName;
              
            for (int i = 0; i < Magics.Length; i++)
            {
                if (i + StartIndex >= Actor.Magics.Count)
                {
                    Magics[i].Visible = false;
                    continue;
                }

                Magics[i].Visible = true;
                Magics[i].Update(Actor.Magics[i + StartIndex]);
            }
        }
        public void Process()
        { 
            
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

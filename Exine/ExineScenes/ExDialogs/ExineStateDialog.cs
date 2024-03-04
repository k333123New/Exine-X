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
        public MirButton CloseButton, StatusButton, SkillButton, StateButton;//,  CharacterButton,StatusButton, StateButton;
        public ExineImageControl StatusPage, StatePage, SkillPage;//,  ClassImage;

        public ExineLabel NameLabel, GuildLabel, LoverLabel;//, GoldLabel, WeightLabel, WeightMaxLabel;
        public ExineLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        public ExineLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        public ExineLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        public ExineLabel HeadingLabel, StatLabel;
        public MirButton NextButton, BackButton;

        public MirItemCell[] Grid;
        private MirGridType GridType;
        public MagicButton[] Magics;

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

            StatePage = new ExineImageControl
            {
                Index = 507,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };
            StatePage.BeforeDraw += (o, e) =>
            {/*
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
                */
            };
            

            SkillPage = new ExineImageControl
            {
                Index = 508,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };

             
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
           
            SkillButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(194, 70),
                Parent = this,
                PressedIndex = 503,
                Size = new Size(64, 20),
                Sound = SoundList.ButtonA
            };
            //SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
            {
                HoverIndex = 90,
                Index = 89,
                Location = new Point(241+102+22, 3+434-17),
                Library = Libraries.PANEL0600,
                Parent = this,
                PressedIndex = 91,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) =>
            {
                Hide();
                ExineMainScene.Scene.InventoryDialog.Hide();
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
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 20+236),
                NotControl = true,
                Text = "0-0",
            };

            BagWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 38 + 236),
                NotControl = true,
                Text = "0-0",
            };

            WearWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 56 + 236),
                NotControl = true,
                Text = "0-0",
            };

            HandWLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 74 + 236),
                NotControl = true,
                Text = "0-0",
            };
            MagicRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 92 + 236),
                NotControl = true,
                Text = "0-0"
            };
            PoisonResLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 110 + 236),
                NotControl = true,
                Text = "0/0"
            };
            HealthRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 128 + 236),
                NotControl = true,
                Text = "0/0"
            };
            //Breezer
            ManaRLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 146 + 236),
                NotControl = true
            };
            PoisonRecLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 164 + 236),
                NotControl = true
            };
            HolyTLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 182 + 236),
                NotControl = true
            };
            FreezeLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 200 + 236),
                NotControl = true
            };
            PoisonAtkLabel = new ExineLabel
            {
                AutoSize = true,
                //Parent = StatePage,
                Parent = StatusPage,
                Location = new Point(126, 218 + 236),
                NotControl = true
            };
            
            Magics = new MagicButton[7];

            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new MagicButton 
                { 
                    Parent = SkillPage, 
                    Visible = false, 
                    Location = new Point(8, 8 + i * 33),
                    HeroMagic = gridType == MirGridType.HeroEquipment
                };
             
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
            StatePage.Visible = false;
            SkillPage.Visible = false;
            StatusButton.Index = 501;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatePage()
        {
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            StatusButton.Index = -1;
            StateButton.Index = 502;
            SkillButton.Index = -1;
        }
        
        public void ShowSkillPage()
        {
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = true;
            StatusButton.Index = -1;
            StateButton.Index = -1;
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

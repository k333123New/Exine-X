﻿using Exine.ExineControls;
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
        public MirButton CloseButton, SkillButton;//,  CharacterButton,StatusButton, StateButton;
        public ExineImageControl CharacterPage, SkillPage; //StatusPage, StatePage,  ClassImage;

        public ExineLabel NameLabel, GuildLabel, LoverLabel, GoldLabel, WeightLabel, WeightMaxLabel;
        //public ExineLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        //public ExineLabel CritRLabel, CritDLabel, LuckLabel, AttkSpdLabel, AccLabel, AgilLabel;
        //public ExineLabel ExpPLabel, BagWLabel, WearWLabel, HandWLabel, MagicRLabel, PoisonRecLabel, HealthRLabel, ManaRLabel, PoisonResLabel, HolyTLabel, FreezeLabel, PoisonAtkLabel;
        //public ExineLabel HeadingLabel, StatLabel;
        //public MirButton NextButton, BackButton;

        public MirItemCell[] Grid;
        private MirGridType GridType;
        public MagicButton[] Magics;

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

            SkillPage = new ExineImageControl
            {
                Index = 508,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };

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
                Sound = SoundList.ButtonA
            };
            //SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
            {
                HoverIndex = 90,
                Index = 89,
                Location = new Point(241+102, 3+434),
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
            Magics = new MagicButton[7];

            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new MagicButton 
                { 
                    Parent = SkillPage, 
                    Visible = false, 
                    Location = new Point(8, 8 + i * 33),
                    HeroMagic = gridType == MirGridType.HeroEquipment
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

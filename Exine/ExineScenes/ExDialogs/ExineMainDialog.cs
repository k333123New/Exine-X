﻿using System.Text.RegularExpressions;
using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineSounds;
using SlimDX;
using Font = System.Drawing.Font;

using Microsoft.VisualBasic.ApplicationServices;
using static System.Collections.Specialized.BitVector32;

namespace Exine.ExineScenes.ExDialogs
{
    public sealed class ExineMainDialog : ExineImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public ExineImageControl WeightBar, LeftCap, RightCap;//ExperienceBar,
        public ExineButton  MenuButton, InventoryButton, CharacterButton, SkillButton, QuestButton, OptionButton;
        public ExineControl HealthOrb;
        public ExineLabel HealthLabel, ManaLabel, TopLabel, BottomLabel,  GoldLabel, WeightLabel, SpaceLabel, AModeLabel, PModeLabel, SModeLabel;//ExperienceLabel,

        //203,563
        //exine ui _ExExperienceBar는 챗다이어로그에 들어가있음.
        public ExineImageControl _ExPortraitDialog, _ExPhoto, _ExHPBar, _ExMPBar, _ExChatDialog, _ExBelt, _ExAPModeBtn, _ExMinimapDialog;//, _ExExperienceBar;
        public ExineLabel _ExRingLabel, _ExRingAKALabel, _ExAKALabel, _ExNameLabel, _ExLevelLabel, _ExHPLabel, _ExMPLabel;

        public bool HPOnly
        {
            // get { return User != null && User.Class == ExineClass.Warrior && User.Level < 26; }
            get { return false; } //k333123
        }

        public ExineMainDialog()
        {
            /*
            Index = Settings.Resolution == 800 ? 0 : Settings.Resolution == 1024 ? 1 : 2;
            Library = Libraries.Prguse;
            //Location = new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)), Settings.ScreenHeight - Size.Height);
            Location = new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)), Settings.ScreenHeight - Size.Height);
            PixelDetect = true;
            */
            //Index = 0;//all
            Index = 1;//black
            Library = Libraries.MAINSAMPLE;
            Location = new Point((1024 - 800) / 2, (768 - 600) / 2);
            ForeColour = Color.Black;
            BackColour = Color.Black;
            PixelDetect = true;
            

            _ExPortraitDialog = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.PANEL0100,
                Location = new Point(0, 468),
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2 + 468),
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Visible = true,
                Parent = this,
                //PixelDetect = true;
                //NotControl = true,
                
            };

            _ExPhoto = new ExineImageControl
            { 
                //bitmap to lib!(bitmap is info.xxx)
                //GetBytesFromJpg("test.jpg")
                //Bitmap objectPortrait = GetBitmapFromBytes(MapObject.User.ExPortraitBytes)
                //objectPortrait.Save(temp.Name + ".jpg");
                Index = 5,
                Library = new MLibrary(Settings.ExineUIPath + "PANEL0100"),

                Location = new Point(15, 15), 
                Visible = true,
                Parent = _ExPortraitDialog,  
            };
            _ExPhoto.BeforeDraw += _ExPhoto_BeforeDraw;


            //이 안쪽에 피가 추가되어야함.
            //33,34
            _ExHPBar = new ExineImageControl
            {
                //Index = 33,
                //Library = Libraries.PANEL0100,
               

                Location = new Point(22, 98),
                Visible = true,
                Parent = _ExPortraitDialog,
            };
            _ExHPBar.AfterDraw += _ExHPBar_AfterDraw;
            //_ExHPBar.BeforeDraw += _ExHPBar_BeforeDraw;

            _ExHPLabel = new ExineLabel
            {
                Font = new Font(Settings.FontName, 9F, FontStyle.Bold),
                ForeColour = Color.FromArgb(10, Color.White),
                AutoSize = true,
                Parent = _ExHPBar,
                NotControl = true,
                Visible = true,//add k333123
            };

            _ExMPBar = new ExineImageControl
            {
                //Index = 34,
                //Library = Libraries.PANEL0100,
                Location = new Point(22, 116),
                Visible = true,
                Parent = _ExPortraitDialog,
            };
            _ExMPBar.AfterDraw += _ExMPBar_AfterDraw;
            //_ExMPBar.BeforeDraw += _ExMPBar_BeforeDraw


            _ExMPLabel = new ExineLabel
            {
                Font = new Font(Settings.FontName, 9F, FontStyle.Bold),
                ForeColour = Color.FromArgb(10, Color.White),
                AutoSize = true,
                Parent = _ExMPBar,
                NotControl = true,
                Visible = true,//add k333123
            };
            /*
            ExperienceBar = new ExineImageControl
            {
                //Index = Settings.Resolution != 800 ? 8 : 7,
                //Library = Libraries.Prguse,
                
                //DrawImage = false,
                //NotControl = true,
                Location = new Point(9, 143),
                Visible = true,//add k333123
                Parent = this,
                //Visible = false,//add k333123
            };
            ExperienceBar.AfterDraw += ExperienceBar_AfterDraw;
            //ExperienceBar.BeforeDraw += ExperienceBar_BeforeDraw;
           
            ExperienceLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = ExperienceBar,
                NotControl = true,
                Visible = false,//add k333123
            };

             */
            //ring, ringaka, aka, name, lv
            _ExRingLabel = new ExineLabel
            {
                Parent = _ExPortraitDialog,
                Location = new Point(92, 10),
                Size = new Size(99, 20),
                Visible = true,
                NotControl = true,
            };

            _ExRingAKALabel = new ExineLabel
            {
                Parent = _ExPortraitDialog,
                Location = new Point(92, 30),
                Size = new Size(99, 20),
                Visible = true,
                NotControl = true,
            };

            _ExAKALabel = new ExineLabel
            {
                Parent = _ExPortraitDialog,
                Location = new Point(92, 50),
                Size = new Size(99, 20),
                Visible = true,
                NotControl = true,
            };

            _ExNameLabel = new ExineLabel
            {
                Parent = _ExPortraitDialog,
                Location = new Point(92, 72),
                Size = new Size(99, 20),
                Visible = true,
                NotControl = true,
            };

            _ExLevelLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = _ExPortraitDialog,
                Visible = true,//add k333123
                Location = new Point(173, 72)
            };

            _ExBelt = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.PANEL0200,
                Location = new Point(200, 556),
                Visible = true,
                Parent = this,
            };

            _ExAPModeBtn = new ExineImageControl
            {
                Index = 1,
                Library = Libraries.PANEL0200,
                Location = new Point(202, 561),
                Visible = true,
                Parent = this,
            };

            //196,468
            /*
            _ExChatDialog = new MirImageControl
            {
                Index = 0,
                Library = Libraries.PANEL0201,
                Location = new Point(200, 466),
                Visible = true,
                Parent = this,
            };*/

           

            LeftCap = new ExineImageControl
            {
                Index = 12,
                Library = Libraries.Prguse,
                Location = new Point(-67, this.Size.Height - 96),
                Parent = this,
                Visible = false
            };
            RightCap = new ExineImageControl
            {
                Index = 13,
                Library = Libraries.Prguse,
                Location = new Point(1024, this.Size.Height - 104),
                Parent = this,
                Visible = false
            };

            if (Settings.Resolution > 1024)
            {
                LeftCap.Visible = true;
                RightCap.Visible = true;
            }

            InventoryButton = new ExineButton
            {
                HoverIndex = 1904,
                Index = 1903,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 96, 76),
                Parent = this,
                PressedIndex = 1905,
                Sound = SoundList.ButtonA,
                Visible = false,//add k333123
                Hint = string.Format(GameLanguage.Inventory, CMain.InputKeys.GetKey(KeybindOptions.Inventory))
                
            };
            /*
            InventoryButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.InventoryDialog.Visible)
                    ExineMainScene.Scene.InventoryDialog.Hide();
                else
                    ExineMainScene.Scene.InventoryDialog.Show();
            };*/

            CharacterButton = new ExineButton
            {
                HoverIndex = 1901,
                Index = 1900,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 119, 76),
                Parent = this,
                PressedIndex = 1902,
                Sound = SoundList.ButtonA,
                Visible = false,//add k333123
                Hint = string.Format(GameLanguage.Character, CMain.InputKeys.GetKey(KeybindOptions.Equipment))
            };
            CharacterButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.ExCharacterDialog.Visible && ExineMainScene.Scene.ExCharacterDialog.CharacterPage.Visible)
                    ExineMainScene.Scene.ExCharacterDialog.Hide();
                else
                {
                    ExineMainScene.Scene.ExCharacterDialog.Show();
                    ExineMainScene.Scene.ExCharacterDialog.ShowCharacterPage();
                }
            };

            SkillButton = new ExineButton
            {
                HoverIndex = 1907,
                Index = 1906,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 73, 76),
                Parent = this,
                PressedIndex = 1908,
                Sound = SoundList.ButtonA,
                Visible = false,//add k333123
                Hint = string.Format(GameLanguage.Skills, CMain.InputKeys.GetKey(KeybindOptions.Skills))
            };
            SkillButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.ExCharacterDialog.Visible && ExineMainScene.Scene.ExCharacterDialog.SkillPage.Visible)
                    ExineMainScene.Scene.ExCharacterDialog.Hide();
                else
                {
                    ExineMainScene.Scene.ExCharacterDialog.Show();
                    ExineMainScene.Scene.ExCharacterDialog.ShowSkillPage();
                }
            };

            QuestButton = new ExineButton
            {
                HoverIndex = 1910,
                Index = 1909,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 50, 76),
                Parent = this,
                PressedIndex = 1911,
                Sound = SoundList.ButtonA,
                Visible = false,//add k333123
                Hint = string.Format(GameLanguage.Quests, CMain.InputKeys.GetKey(KeybindOptions.Quests))
            };
            QuestButton.Click += (o, e) =>
            {
                if (!ExineMainScene.Scene.QuestLogDialog.Visible)
                    ExineMainScene.Scene.QuestLogDialog.Show();
                else ExineMainScene.Scene.QuestLogDialog.Hide();
            };

            OptionButton = new ExineButton
            {
                HoverIndex = 1913,
                Index = 1912,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 27, 76),
                Parent = this,
                PressedIndex = 1914,
                Sound = SoundList.ButtonA,
                Visible = false,//add k333123
                Hint = string.Format(GameLanguage.Options, CMain.InputKeys.GetKey(KeybindOptions.Options))
            };
            OptionButton.Click += (o, e) =>
            {
                if (!ExineMainScene.Scene.OptionDialog.Visible)
                    ExineMainScene.Scene.OptionDialog.Show();
                else ExineMainScene.Scene.OptionDialog.Hide();
            };

            MenuButton = new ExineButton
            {
                HoverIndex = 1961,
                Index = 1960,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 55, 35),
                Parent = this,
                PressedIndex = 1962,
                Sound = SoundList.ButtonC,
                Visible = false,//add k333123
                Hint = GameLanguage.Menu
            };
            MenuButton.Click += (o, e) =>
            {
                if (!ExineMainScene.Scene.MenuDialog.Visible) ExineMainScene.Scene.MenuDialog.Show();
                else ExineMainScene.Scene.MenuDialog.Hide();
            };

             

            HealthOrb = new ExineControl
            {
                Parent = this,
                Location = new Point(0, 30),
                NotControl = true,
                Visible = false,//add k333123
            };

            HealthOrb.BeforeDraw += HealthOrb_BeforeDraw;

            HealthLabel = new ExineLabel
            {
                AutoSize = true,
                Location = new Point(0, 27),
                Visible = false,//add k333123
                Parent = HealthOrb
            };
            HealthLabel.SizeChanged += Label_SizeChanged;

            ManaLabel = new ExineLabel
            {
                AutoSize = true,
                Location = new Point(0, 42),
                Visible = false,//add k333123
                Parent = HealthOrb
            };
            ManaLabel.SizeChanged += Label_SizeChanged;

            TopLabel = new ExineLabel
            {
                Size = new Size(85, 30),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(9, 20),
                Visible = false,//add k333123
                Parent = HealthOrb,
            };

            BottomLabel = new ExineLabel
            {
                Size = new Size(85, 30),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Location = new Point(9, 50),
                Visible = false,//add k333123
                Parent = HealthOrb,
            };

          
            _ExMinimapDialog = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.PANEL0301,
                Location = new Point(600, 468),
                Visible = true,
                BackColour = Color.Black,
                Parent = this,
            };

            GoldLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(this.Size.Width - 105, 119),
                Parent = this,
                Size = new Size(99, 13),
                Sound = SoundList.Gold,
                Visible = false,//add k333123
            };
            GoldLabel.Click += (o, e) =>
            {
                if (ExineMainScene.SelectedCell == null)
                    ExineMainScene.PickedUpGold = !ExineMainScene.PickedUpGold && ExineMainScene.Gold > 0;
            };

            WeightBar = new ExineImageControl
            {
                Index = 76,
                Library = Libraries.Prguse,
                Location = new Point(this.Size.Width - 105, 103),
                Parent = this,
                DrawImage = false,
                NotControl = true,
                Visible = false,//add k333123
            };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;

            WeightLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(this.Size.Width - 105, 101),
                Size = new Size(40, 14),
                Visible = false,//add k333123
            };

            SpaceLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(this.Size.Width - 30, 101),
                Size = new Size(26, 14),
                Visible = false,//add k333123
            };

            
            AModeLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 899 : 675, Settings.Resolution != 800 ? -448 : -280),
                //Visible = Settings.ModeView
                Visible = false,//add k333123
            };

            PModeLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.Orange,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(230, 125),
                //Visible = Settings.ModeView
                Visible = false,//add k333123
            };

            SModeLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.LimeGreen,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 899 : 675, Settings.Resolution != 800 ? -463 : -295),
                //Visible = Settings.ModeView
                Visible = false,//add k333123
            };
            
        }

        

        bool isPhotoUpdateOK = false;
        private void _ExPhoto_BeforeDraw(object sender, EventArgs e)
        {
            if (isPhotoUpdateOK == true) return;
            //bitmap to lib!(bitmap is info.xxx)
            //GetBytesFromJpg("test.jpg")
            //Bitmap objectPortrait = GetBitmapFromBytes(MapObject.User.ExPortraitBytes)
            //objectPortrait.Save(temp.Name + ".jpg");
            if (MapObject.User.ExPortraitLen!=0)
            {
                //_ExPhoto.Index = 0; 
                Console.WriteLine("_ExPhoto_BeforeDraw and MapObject.User.ExPortraitLen!=0");
                Console.WriteLine("MapObject.User.ExPortraitLen:" + MapObject.User.ExPortraitLen);
                byte[] photoDatas = new byte[MapObject.User.ExPortraitLen];
                Buffer.BlockCopy(MapObject.User.ExPortraitBytes, 0, photoDatas, 0, photoDatas.Length);
                Bitmap objectPortrait = GetBitmapFromBytes(photoDatas);
                
                isPhotoUpdateOK = true;

                // 사이즈가 변경된 이미지(1/2로 축소)
                int width = objectPortrait.Width / 2;
                int height = objectPortrait.Height / 2;
                Size resize = new Size(width, height);
                Bitmap objectPortraitSmall = new Bitmap(objectPortrait, resize);

                File.Delete("photo.lib");
                NewYPF.MLibraryForSave temp = new NewYPF.MLibraryForSave("photo.lib");
                if (temp.Images.Count != 0)
                {
                    for (int i = 0; i < temp.Images.Count; i++)
                    {
                        temp.RemoveImage(0);
                    }
                }
                temp.AddImage(objectPortrait, 0, 0);
                temp.AddImage(objectPortraitSmall, 0, 0);
                temp.Save();
                temp.Close();

                _ExPhoto.Library = new MLibrary("photo");
                _ExPhoto.Index = 0; 
            }
        }
        public Bitmap GetBitmapFromBytes(byte[] imageBytes)
        {
            Image recvImage = Image.FromStream(new MemoryStream(imageBytes));
            Bitmap recvBitmap = new Bitmap(recvImage);
            return recvBitmap;
        }


        bool toggleTime = false;
        private void _ExHPBar_AfterDraw(object sender, EventArgs e)
        {
            toggleTime = !toggleTime;
            Console.WriteLine("_ExHPBar_AfterDraw!!!!!!!!!!!!!!@@@@@@@@@");
            double percent = (double)MapObject.User.PercentHealth;//MAXHP ?
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            //k333123 test
            if (toggleTime) percent = 0.8;
            else percent = 0.3;

            Rectangle section = new Rectangle
            {
                //Size = new Size((int)((_ExHPBar.Size.Width - 3) * percent), _ExHPBar.Size.Height) 
                Size = new Size((int)(154 * percent), 11)
            };
            Libraries.PANEL0100.Draw(33, section, _ExHPBar.DisplayLocation, Color.White, false);
            _ExHPLabel.Draw();
        }

        private void _ExMPBar_AfterDraw(object sender, EventArgs e)
        {
            Console.WriteLine("_ExMPBar_AfterDraw!!!!!!!!!!!!!!@@@@@@@@@");
            double percent = (double)MapObject.User.PercentMana;//MAXHP ?
            if (percent > 1) percent = 1;
            if (percent <= 0) percent = 0;
            //if (percent <= 0) return; //퍼센트 부분 나오게 해야함!!! 0으로 잡히는듯

            //k333123 test
            if (toggleTime) percent = 0.5;
            else percent = 0.9;
            

            Rectangle section = new Rectangle
            {
                //Size = new Size((int)((_ExHPBar.Size.Width - 3) * percent), _ExHPBar.Size.Height) 
                Size = new Size((int)(154 * percent), 11)
            }; 
            Libraries.PANEL0100.Draw(34, section, _ExMPBar.DisplayLocation, Color.White, false); 
            _ExMPLabel.Draw(); 
        }
        /*
        private void ExperienceBar_AfterDraw(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("ExperienceBar_AfterDraw!!!!!!!!!!!!!!@@@@@@@@@");
            double percent = MapObject.User.Experience / (double)MapObject.User.MaxExperience;
            if (percent > 1) percent = 1;
            if (percent <= 0) percent = 0;
            //if (percent <= 0) return; //퍼센트 부분 나오게 해야함!!! 0으로 잡히는듯

            //k333123 test
            if (toggleTime) percent = 0.5;
            else percent = 0.9;


            Rectangle section = new Rectangle
            {
                //Size = new Size((int)((_ExHPBar.Size.Width - 3) * percent), _ExHPBar.Size.Height) 
                Size = new Size((int)(314 * percent), 8)
            };
            //Libraries.PANEL0100.Draw(34, section, _ExMPBar.DisplayLocation, Color.White, false);
            Libraries.PANEL0201.Draw(14, section, ExperienceBar.DisplayLocation, Color.White, false);
        }

        //HP MP 참고하여 반영할것
        private void ExperienceBar_BeforeDraw(object sender, EventArgs e)
        {
            if (ExperienceBar.Library == null) return;

            double percent = MapObject.User.Experience / (double)MapObject.User.MaxExperience;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((ExperienceBar.Size.Width - 3) * percent), ExperienceBar.Size.Height)
            };

            ExperienceBar.Library.Draw(ExperienceBar.Index, section, ExperienceBar.DisplayLocation, Color.White, false);
        }
        */


        public void Process()
        {
            switch (ExineMainScene.Scene.AMode)
            {
                case AttackMode.Peace:
                    AModeLabel.Text = GameLanguage.AttackMode_Peace;
                    break;
                case AttackMode.Group:
                    AModeLabel.Text = GameLanguage.AttackMode_Group;
                    break;
                case AttackMode.Guild:
                    AModeLabel.Text = GameLanguage.AttackMode_Guild;
                    break;
                case AttackMode.EnemyGuild:
                    AModeLabel.Text = GameLanguage.AttackMode_EnemyGuild;
                    break;
                case AttackMode.RedBrown:
                    AModeLabel.Text = GameLanguage.AttackMode_RedBrown;
                    break;
                case AttackMode.All:
                    AModeLabel.Text = GameLanguage.AttackMode_All;
                    break;
            }

            switch (ExineMainScene.Scene.PMode)
            {
                case PetMode.Both:
                    PModeLabel.Text = GameLanguage.PetMode_Both;
                    break;
                case PetMode.MoveOnly:
                    PModeLabel.Text = GameLanguage.PetMode_MoveOnly;
                    break;
                case PetMode.AttackOnly:
                    PModeLabel.Text = GameLanguage.PetMode_AttackOnly;
                    break;
                case PetMode.None:
                    PModeLabel.Text = GameLanguage.PetMode_None;
                    break;
                case PetMode.FocusMasterTarget:
                    PModeLabel.Text = GameLanguage.PetMode_FocusMasterTarget;
                    break;
            }

            switch (Settings.SkillMode)
            {
                case true:
                    SModeLabel.Text = "[Skill Mode: ~]";
                    break;
                case false:
                    SModeLabel.Text = "[Skill Mode: Ctrl]";
                    break;
            }

            if (Settings.HPView)
            {
                HealthLabel.Text = string.Format("HP {0}/{1}", User.HP, User.Stats[Stat.HP]);
                ManaLabel.Text = HPOnly ? "" : string.Format("MP {0}/{1} ", User.MP, User.Stats[Stat.MP]);
                TopLabel.Text = string.Empty;
                BottomLabel.Text = string.Empty;
            }
            else
            {
                if (HPOnly)
                {
                    TopLabel.Text = string.Format("{0}\n" + "--", User.HP);
                    BottomLabel.Text = string.Format("{0}", User.Stats[Stat.HP]);
                }
                else
                {
                    TopLabel.Text = string.Format(" {0}    {1} \n" + "---------------", User.HP, User.MP);
                    BottomLabel.Text = string.Format(" {0}    {1} ", User.Stats[Stat.HP], User.Stats[Stat.MP]);
                }
                HealthLabel.Text = string.Empty;
                ManaLabel.Text = string.Empty;
            }

            _ExNameLabel.Text = User.Name;
            _ExNameLabel.ForeColour = User.NameColour;
            _ExRingLabel.Text = User.GuildName;
            _ExRingAKALabel.Text = ""; //k333123 must add!!!
            _ExAKALabel.Text = "";//k333123 must add!!!
            _ExLevelLabel.Text = User.Level.ToString();

            _ExHPLabel.Text = string.Format("{0}/{1}", MapObject.User.HP , MapObject.User.Stats[Stat.HP]);
            _ExHPLabel.Location = new Point((154 / 2) - 20, -2);

            _ExMPLabel.Text = string.Format("{0}/{1}", MapObject.User.MP , MapObject.User.Stats[Stat.MP]);
            _ExMPLabel.Location = new Point((154 / 2) - 20, -2);

            /*

            ExperienceLabel.Text = string.Format("{0:#0.##%}", User.Experience / (double)User.MaxExperience);
            ExperienceLabel.Location = new Point((ExperienceBar.Size.Width / 2) - 20, -2);
            */

            GoldLabel.Text = ExineMainScene.Gold.ToString("###,###,##0"); 
            SpaceLabel.Text = User.Inventory.Count(t => t == null).ToString();
            WeightLabel.Text = (MapObject.User.Stats[Stat.BagWeight] - MapObject.User.CurrentBagWeight).ToString();
        }

        private void Label_SizeChanged(object sender, EventArgs e)
        {
            if (!(sender is ExineLabel l)) return;

            l.Location = new Point(50 - (l.Size.Width / 2), l.Location.Y);
        }

        private void HealthOrb_BeforeDraw(object sender, EventArgs e)
        {
            if (Libraries.Prguse == null) return;

            int height;
            if (User != null && User.HP != User.Stats[Stat.HP])
                height = (int)(80 * User.HP / (float)User.Stats[Stat.HP]);
            else
                height = 80;

            if (height < 0) height = 0;
            if (height > 80) height = 80;

            int orbImage = 4;

            bool hpOnly = false;

            if (HPOnly)
            {
                hpOnly = true;
                orbImage = 6;
            }

            Rectangle r = new Rectangle(0, 80 - height, hpOnly ? 100 : 50, height);
            Libraries.Prguse.Draw(orbImage, r, new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)), HealthOrb.DisplayLocation.Y + 80 - height), Color.White, false);

            if (hpOnly) return;
            if (User != null)//add k333123
            {
                if (User.MP != User.Stats[Stat.MP])
                    height = (int)(80 * User.MP / (float)User.Stats[Stat.MP]);
                else
                    height = 80;
            }
            else height = 80;

            if (height < 0) height = 0;
            if (height > 80) height = 80;
            r = new Rectangle(51, 80 - height, 50, height);

            Libraries.Prguse.Draw(4, r, new Point(((Settings.ScreenWidth / 2) - (Size.Width / 2)) + 51, HealthOrb.DisplayLocation.Y + 80 - height), Color.White, false);
        }

       

        private void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library == null) return;
            double percent = MapObject.User.CurrentBagWeight / (double)MapObject.User.Stats[Stat.BagWeight];
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((WeightBar.Size.Width - 2) * percent), WeightBar.Size.Height)
            };

            WeightBar.Library.Draw(WeightBar.Index, section, WeightBar.DisplayLocation, Color.White, false);
        }
    }
    public sealed class ExineChatControlBar : ExineImageControl
    {
        //public MirButton SizeButton, SettingsButton, NormalButton, ShoutButton, WhisperButton, LoverButton, MentorButton, GroupButton, GuildButton, ReportButton, TradeButton;
        public ExineButton NormalButton, ShoutButton, WhisperButton, GroupButton, GuildButton;

        //이부분이 엔터가 눌러지면 같이 나와야 할듯함.
        public ExineChatControlBar()
        {
            //200,559
            //Index = Settings.Resolution != 800 ? 2034 : 2035;
            //Index = 2035;
            //Library = Libraries.Prguse;
            //Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 112);
            Index = 0;
            Library = Libraries.PANEL0202;
            Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 112);
            //(1024 - 800) / 2, (768 - 600) / 2
            Location = new Point((1024 - 800) / 2+200, (768 - 600) / 2 +556);
            /*
            SizeButton = new MirButton
            {
                Index = 2057,
                HoverIndex = 2058,
                PressedIndex = 2059,
                Library = Libraries.Prguse,
                Parent = this,
                //Location = new Point(Settings.Resolution != 800 ? 574 : 350, 1),
                Visible = true,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Size
            };
            SizeButton.Click += (o, e) =>
            {
                ExineMainScene.Scene.ExChatDialog.ChangeSize();
                Location = new Point(Location.X, ExineMainScene.Scene.ExChatDialog.DisplayRectangle.Top - Size.Height);
                if (ExineMainScene.Scene.BeltDialog.Index == 1932)
                    ExineMainScene.Scene.BeltDialog.Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Location.Y - ExineMainScene.Scene.BeltDialog.Size.Height);
            };

            SettingsButton = new MirButton
            {
                Index = 2060,
                HoverIndex = 2061,
                PressedIndex = 2062,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 596 : 372, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.ChatSettings
            };
            SettingsButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.ChatOptionDialog.Visible)
                    ExineMainScene.Scene.ChatOptionDialog.Hide();
                else
                    ExineMainScene.Scene.ChatOptionDialog.Show();

                //GameScene.Scene.ChatDialog.Transparent = !GameScene.Scene.ChatDialog.Transparent;
                //GameScene.Scene.ChatDialog.UpdateBackground();
            };
            */
            NormalButton = new ExineButton
            {
                /*
                Index = 2036,
                HoverIndex = 2037,
                PressedIndex = 2038,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(12, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_All
                */

                Index = 1,
                HoverIndex = 1,
                PressedIndex = 8,
                Library = Libraries.PANEL0202,
                Parent = this,
                Location = new Point(6, 6),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_All
            };
            NormalButton.Click += (o, e) =>
            {
                ToggleChatFilter("All");
            };

            ShoutButton = new ExineButton
            {/*
                Index = 2039,
                HoverIndex = 2040,
                PressedIndex = 2041,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(34, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Short
                */
                Index = 9,
                HoverIndex = 9,
                PressedIndex = 16,
                Library = Libraries.PANEL0202,
                Parent = this,
                Location = new Point(27, 6),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Short
            };
            ShoutButton.Click += (o, e) =>
            {
                ToggleChatFilter("Shout");
            };

            GroupButton = new ExineButton
            {
                /*
                Index = 2051,
                HoverIndex = 2052,
                PressedIndex = 2053,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(122, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Group
                */
                Index = 17,
                HoverIndex = 17,
                PressedIndex = 24,
                Library = Libraries.PANEL0202,
                Parent = this,
                Location = new Point(47, 6),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Group

            };
            GroupButton.Click += (o, e) =>
            {
                ToggleChatFilter("Group");
            };


            WhisperButton = new ExineButton
            {
                /*
                Index = 2042,
                HoverIndex = 2043,
                PressedIndex = 2044,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(56, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Whisper
                */
                Index = 25,
                HoverIndex = 25,
                PressedIndex = 32,
                Library = Libraries.PANEL0202,
                Parent = this,
                Location = new Point(67, 6),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Whisper
            };
            WhisperButton.Click += (o, e) =>
            {
                ToggleChatFilter("Whisper");
            };

            GuildButton = new ExineButton
            {
                /*
                Index = 2054,
                HoverIndex = 2055,
                PressedIndex = 2056,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(144, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Guild
                */
                Index = 33,
                HoverIndex = 33,
                PressedIndex = 40,
                Library = Libraries.PANEL0202,
                Parent = this,
                Location = new Point(87, 6),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Guild
            };
            GuildButton.Click += (o, e) =>
            {
                Settings.ShowGuildChat = !Settings.ShowGuildChat;
                ToggleChatFilter("Guild");
            };

            /*
            LoverButton = new MirButton
            {
                Index = 2045,
                HoverIndex = 2046,
                PressedIndex = 2047,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(78, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Lover
            };
            LoverButton.Click += (o, e) =>
            {
                ToggleChatFilter("Lover");
            };

            MentorButton = new MirButton
            {
                Index = 2048,
                HoverIndex = 2049,
                PressedIndex = 2050,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(100, 1),
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Chat_Mentor
            };
            MentorButton.Click += (o, e) =>
            {
                ToggleChatFilter("Mentor");
            };

            
            TradeButton = new MirButton
            {
                Index = 2004,
                HoverIndex = 2005,
                PressedIndex = 2006,
                Library = Libraries.Prguse,
                Location = new Point(166, 1),
                Parent = this,
                Sound = SoundList.ButtonC,
                Hint = string.Format(GameLanguage.Trade, CMain.InputKeys.GetKey(KeybindOptions.Trade)),
            };
            TradeButton.Click += (o, e) => Network.Enqueue(new ClientPacket.TradeRequest());

            ReportButton = new MirButton
            {
                Index = 2063,
                HoverIndex = 2064,
                PressedIndex = 2065,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(Settings.Resolution != 800 ? 552 : 328, 1),
                Sound = SoundList.ButtonA,
                Hint = "Report",
                Visible = false
            };
            ReportButton.Click += (o, e) =>
            {
                ExineMainScene.Scene.ReportDialog.Visible = !ExineMainScene.Scene.ReportDialog.Visible;
            };*/

            ToggleChatFilter("All");
        }

        public void ToggleChatFilter(string chatFilter)
        {
            /*
            NormalButton.Index = 2036;
            NormalButton.HoverIndex = 2037;
            ShoutButton.Index = 2039;
            ShoutButton.HoverIndex = 2040;
            WhisperButton.Index = 2042;
            WhisperButton.HoverIndex = 2043;

            //LoverButton.Index = 2045;
            //LoverButton.HoverIndex = 2046;
            //MentorButton.Index = 2048;
            //MentorButton.HoverIndex = 2049;
            GroupButton.Index = 2051;
            GroupButton.HoverIndex = 2052;
            GuildButton.Index = 2054;
            GuildButton.HoverIndex = 2055;
            */

            NormalButton.Index = 1;
            NormalButton.HoverIndex = 1;
            ShoutButton.Index = 9;
            ShoutButton.HoverIndex = 9;
            GroupButton.Index = 17;
            GroupButton.HoverIndex = 17;
            WhisperButton.Index = 25;
            WhisperButton.HoverIndex = 25;
            GuildButton.Index = 33;
            GuildButton.HoverIndex = 40;

           
            ExineMainScene.Scene.ExChatDialog.ChatPrefix = "";

            switch (chatFilter)
            {/*
                case "All":
                    NormalButton.Index = 2038;
                    NormalButton.HoverIndex = 2038;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "";
                    break;
                case "Shout":
                    ShoutButton.Index = 2041;
                    ShoutButton.HoverIndex = 2041;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!";
                    break;
                case "Whisper":
                    WhisperButton.Index = 2044;
                    WhisperButton.HoverIndex = 2044;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "/";
                    break;
                case "Group":
                    GroupButton.Index = 2053;
                    GroupButton.HoverIndex = 2053;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!!";
                    break;
                case "Guild":
                    GuildButton.Index = 2056;
                    GuildButton.HoverIndex = 2056;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!~";
                    break;
                    
                case "Lover":
                    LoverButton.Index = 2047;
                    LoverButton.HoverIndex = 2047;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = ":)";
                    break;
                case "Mentor":
                    MentorButton.Index = 2050;
                    MentorButton.HoverIndex = 2050;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!#";
                    break;
                    
                */

                case "All":
                    NormalButton.Index = 8;
                    NormalButton.HoverIndex = 8;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "";
                    ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "";
                    break;
                case "Shout":
                    ShoutButton.Index = 16;
                    ShoutButton.HoverIndex = 16;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!";
                    ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "!";
                    break;
                case "Whisper":
                    WhisperButton.Index = 32;
                    WhisperButton.HoverIndex = 32;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "/";
                    ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "/";
                    break;
                case "Group":
                    GroupButton.Index = 24;
                    GroupButton.HoverIndex = 24;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!!";
                    ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "!!";
                    break;
                case "Guild":
                    GuildButton.Index = 40;
                    GuildButton.HoverIndex = 40;
                    ExineMainScene.Scene.ExChatDialog.ChatPrefix = "!~";
                    ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "!~";
                    break;
            }

            ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
            ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionStart = ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text.Length;
        }
    }

    public sealed class ExineChatDialog : ExineImageControl
    {
        public List<ChatHistory> FullHistory = new List<ChatHistory>();
        public List<ChatHistory> History = new List<ChatHistory>();
        public List<ExineLabel> ChatLines = new List<ExineLabel>();

        public List<ChatItem> LinkedItems = new List<ChatItem>();
        public List<ExineLabel> LinkedItemButtons = new List<ExineLabel>();

        public ExineButton HomeButton, UpButton, EndButton, DownButton, PositionBar;
        public ExineImageControl CountBar, _ExExperienceBar;
        public ExineTextBox ChatTextBox;
        public ExineLabel _ExExperienceLabel;
        public Font ChatFont = new Font(Settings.FontName, 8F);
        public string LastPM = string.Empty;

        //public int StartIndex, LineCount = 4, WindowSize; //add k333123 mod
        public int StartIndex, LineCount = 5, WindowSize;
        public string ChatPrefix = "";

        public bool Transparent;

        public ExineChatDialog()
        {
            //Index = Settings.Resolution != 800 ? 2221 : 2201;
            Index = 0;
            Library = Libraries.PANEL0201;
            //Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 97);
            //Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 97);
            Location = new Point((1024 - 800) / 2+200, (768 - 600) / 2 + 466);
            PixelDetect = true;

            KeyPress += ChatPanel_KeyPress;
            KeyDown += ChatPanel_KeyDown;
            MouseWheel += ChatPanel_MouseWheel;

            

            //채팅 입력 상자 부분
            ChatTextBox = new ExineTextBox
            {
                //BackColour = Color.DarkGray,
                //ForeColour = Color.Black,
                BackColour = Color.Black,
                ForeColour = Color.White,
                Parent = this,
                //Size = new Size(Settings.Resolution != 800 ? 627 : 403, 13),
                //Size = new Size(403, 13),
                //Location = new Point(1, 54),
                Size = new Size(270, 12),
                Location = new Point(121, 117), //add k333123 mod
                MaxLength = Globals.MaxChatLength,
                Visible = false,
                Font = ChatFont,
            };
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;
            ChatTextBox.TextBox.KeyDown += ChatTextBox_KeyDown;
            ChatTextBox.TextBox.KeyUp += ChatTextBox_KeyUp;

            HomeButton = new ExineButton
            {
                Index = 2018,
                HoverIndex = 2019,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 618 : 394, 1),
                Parent = this,
                PressedIndex = 2020,
                Sound = SoundList.ButtonA,
            };
            HomeButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex = 0;
                Update();
            };


            UpButton = new ExineButton
            {
                Index = 2021,
                HoverIndex = 2022,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 618 : 394, 9),
                Parent = this,
                PressedIndex = 2023,
                Sound = SoundList.ButtonA,
            };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };


            EndButton = new ExineButton
            {
                Index = 2027,
                HoverIndex = 2028,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45),
                Parent = this,
                PressedIndex = 2029,
                Sound = SoundList.ButtonA,
            };
            EndButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex = History.Count - 1;
                Update();
            };

            DownButton = new ExineButton
            {
                Index = 2024,
                HoverIndex = 2025,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39),
                Parent = this,
                PressedIndex = 2026,
                Sound = SoundList.ButtonA,
            };
            DownButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex++;
                Update();
            };

            CountBar = new ExineImageControl
            {
                Index = 2012,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 622 : 398, 16),
                Parent = this,
            };

            PositionBar = new ExineButton
            {
                Index = 2015,
                HoverIndex = 2016,
                Library = Libraries.Prguse,
                Location = new Point(Settings.Resolution != 800 ? 619 : 395, 16),
                Parent = this,
                PressedIndex = 2017,
                Movable = true,
                Sound = SoundList.None,
            };
            PositionBar.OnMoving += PositionBar_OnMoving;

            _ExExperienceBar = new ExineImageControl
            {
                //Index = 14,
                //Library = Libraries.PANEL0201,
                Location = new Point(43,87),
                Parent = this,
                DrawImage = false,
                NotControl = true,
                Visible = true,//add k333123
            };
            _ExExperienceBar.AfterDraw += _ExExperienceBar_AfterDraw;
            //_ExExperienceBar.BeforeDraw += _ExExperienceBar_BeforeDraw;
            _ExExperienceBar.MouseUp += _ExExperienceBar_IsMouseUp;
            _ExExperienceBar.MouseLeave += _ExExperienceBar_MouseLeave;

            _ExExperienceLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = _ExExperienceBar,
                NotControl = true,
                Visible = false,//add k333123
            };
        }

       

        public void Process()
        {
            _ExExperienceLabel.Text = string.Format("{0:#0.##%}", MapObject.User.Experience / (double)MapObject.User.MaxExperience);
            _ExExperienceLabel.Location = new Point((_ExExperienceLabel.Size.Width / 2) - 20, -10);
        }

        bool toggleTime = false;
        private void _ExExperienceBar_AfterDraw(object sender, EventArgs e)
        {
            toggleTime = !toggleTime;
            Console.WriteLine("ExperienceBar_AfterDraw!!!!!!!!!!!!!!@@@@@@@@@");
            double percent = MapObject.User.Experience / (double)MapObject.User.MaxExperience;
            if (percent > 1) percent = 1;
            if (percent <= 0) percent = 0;
            //if (percent <= 0) return; //퍼센트 부분 나오게 해야함!!! 0으로 잡히는듯

            //k333123 test
            if (toggleTime) percent = 0.5;
            else percent = 1.0;


            Rectangle section = new Rectangle
            {
                Size = new Size((int)(314 * percent), 4)
            };
            Libraries.PANEL0201.Draw(14, section, _ExExperienceBar.DisplayLocation, Color.White, false);


        }

        private void _ExExperienceBar_BeforeDraw(object sender, EventArgs e)
        {
            if (_ExExperienceBar.Library == null) return;

            double percent = MapObject.User.Experience / (double)MapObject.User.MaxExperience;
            percent = 99;//test
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((_ExExperienceBar.Size.Width - 3) * percent), _ExExperienceBar.Size.Height/2)
            };
            _ExExperienceBar.Library.Draw(_ExExperienceBar.Index, section, _ExExperienceBar.DisplayLocation, Color.White, false);
        }

        private void _ExExperienceBar_IsMouseUp(object sender, EventArgs e)
        {
            _ExExperienceLabel.Visible = true;
        }

        private void _ExExperienceBar_MouseLeave(object sender, EventArgs e)
        {
            _ExExperienceLabel.Visible = false;
        }

        public void SetChatText(string newText)
        {
            string newMsg = ChatTextBox.Text += newText;

            if (newMsg.Length > Globals.MaxChatLength) return;

            ChatTextBox.Text = newMsg;
            ChatTextBox.SetFocus();
            ChatTextBox.Visible = true;
            ExineMainScene.Scene.ExChatControl.Show(); //add k333123 add
            ChatTextBox.TextBox.SelectionLength = 0;
            ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(ChatTextBox.Text))
                    {
                        string msg = ChatTextBox.Text;

                        if (msg.ToUpper() == "@LEVELEFFECT")
                        {
                            Settings.LevelEffect = !Settings.LevelEffect;
                        }

                        if (msg.ToUpper() == "@TARGETDEAD")
                        {
                            Settings.TargetDead = !Settings.TargetDead;
                        }

                        Network.SendPacketToServer(new ClientPacket.Chat
                        {
                            Message = msg,
                            LinkedItems = new List<ChatItem>(LinkedItems)
                        });

                        if (ChatTextBox.Text[0] == '/')
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 0)
                                LastPM = parts[0];
                        }
                    }
                    ChatTextBox.Visible = false;
                    ExineMainScene.Scene.ExChatControl.Hide(); //add k333123 add
                    ChatTextBox.Text = string.Empty;
                    LinkedItems.Clear();
                    break;

                case (char)Keys.Escape:
                    e.Handled = true;
                    ChatTextBox.Visible = false;
                    ExineMainScene.Scene.ExChatControl.Hide(); //add k333123 add
                    ChatTextBox.Text = string.Empty;
                    LinkedItems.Clear();
                    break;
            }
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int x = Settings.Resolution != 800 ? 619 : 395;
            int y = PositionBar.Location.Y;
            if (y >= 16 + CountBar.Size.Height - PositionBar.Size.Height) y = 16 + CountBar.Size.Height - PositionBar.Size.Height;
            if (y < 16) y = 16;

            int h = CountBar.Size.Height - PositionBar.Size.Height;
            h = (int)((y - 16) / (h / (float)(History.Count - 1)));

            if (h != StartIndex)
            {
                StartIndex = h;
                Update();
            }

            PositionBar.Location = new Point(x, y);
        }

        public void ReceiveChat(string text, ChatType type)
        {
            Color foreColour, backColour;

            switch (type)
            {
                case ChatType.Hint:
                    backColour = Color.White;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.DarkGreen;
                    break;
                case ChatType.Announcement:
                    backColour = Color.Blue;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    ExineMainScene.Scene.ChatNoticeDialog.ShowNotice(RegexFunctions.CleanChatString(text));
                    break;
                case ChatType.LineMessage:
                    backColour = Color.Blue;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    break;
                case ChatType.Shout:
                    backColour = Color.Yellow;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.Black;
                    break;
                case ChatType.Shout2:
                    backColour = Color.Green;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    break;
                case ChatType.Shout3:
                    backColour = Color.Purple;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    break;
                case ChatType.System:
                    backColour = Color.Red;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    break;
                case ChatType.System2:
                    backColour = Color.DarkRed;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.White;
                    break;
                case ChatType.Group:
                    backColour = Color.White;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.Brown;
                    break;
                case ChatType.WhisperOut:
                    foreColour = Color.CornflowerBlue;
                    backColour = Color.Transparent; //add k333123 change
                    backColour = Color.White;
                    break;
                case ChatType.WhisperIn:
                    foreColour = Color.DarkBlue;
                    backColour = Color.Transparent; //add k333123 change
                    backColour = Color.White;
                    break;
                case ChatType.Guild:
                    backColour = Color.White;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.Green;
                    break;
                case ChatType.LevelUp:
                    backColour = Color.FromArgb(255, 225, 185, 250);
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.Blue;
                    break;
                case ChatType.Relationship:
                    backColour = Color.Transparent;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.HotPink;
                    break;
                case ChatType.Mentor:
                    backColour = Color.White;
                    backColour = Color.Transparent; //add k333123 change
                    foreColour = Color.Purple;
                    break;
                default:
                    backColour = Color.White;
                    backColour = Color.Transparent; //add k333123 change
                    //foreColour = Color.Black; //add k333123 change
                    foreColour = Color.Yellow;
                    break;
            }

            List<string> chat = new List<string>();

            //int chatWidth = Settings.Resolution != 800 ? 614 : 390; //add k333123 change
            int chatWidth = 390;
            int index = 0;

            for (int i = 1; i < text.Length; i++)
            {
                if (i - index < 0) continue;

                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    int offset = i - index;
                    int newIndex = i - 1;

                    var itemLinkMatches = RegexFunctions.ChatItemLinks.Matches(text.Substring(index)).Cast<Match>();

                    if (itemLinkMatches.Any())
                    {
                        var match = itemLinkMatches.SingleOrDefault(x => (x.Index < (i - index)) && (x.Index + x.Length > offset - 1));

                        if (match != null)
                        {
                            offset = match.Index;
                            newIndex = match.Index;
                        }
                    }

                    chat.Add(text.Substring(index, offset - 1));
                    index = newIndex;
                }
            }

            chat.Add(text.Substring(index, text.Length - index));

            if (StartIndex == History.Count - LineCount)
                StartIndex += chat.Count;

            for (int i = 0; i < chat.Count; i++)
                FullHistory.Add(new ChatHistory { Text = chat[i], BackColour = backColour, ForeColour = foreColour, Type = type });

            Update();
        }

        public void Update()
        {
            History = new List<ChatHistory>();

            for (int i = 0; i < FullHistory.Count; i++)
            {
                switch (FullHistory[i].Type)
                {
                    case ChatType.Normal:
                    case ChatType.LineMessage:
                        if (Settings.FilterNormalChat) continue;
                        break;
                    case ChatType.WhisperIn:
                    case ChatType.WhisperOut:
                        if (Settings.FilterWhisperChat) continue;
                        break;
                    case ChatType.Shout:
                    case ChatType.Shout2:
                    case ChatType.Shout3:
                        if (Settings.FilterShoutChat) continue;
                        break;
                    case ChatType.System:
                    case ChatType.System2:
                        if (Settings.FilterSystemChat) continue;
                        break;
                    case ChatType.Group:
                        if (Settings.FilterGroupChat) continue;
                        break;
                    case ChatType.Guild:
                        if (Settings.FilterGuildChat) continue;
                        break;
                }

                History.Add(FullHistory[i]);
            }

            for (int i = 0; i < ChatLines.Count; i++)
                ChatLines[i].Dispose();

            for (int i = 0; i < LinkedItemButtons.Count; i++)
                LinkedItemButtons[i].Dispose();

            ChatLines.Clear();
            LinkedItemButtons.Clear();

            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;

            if (History.Count > 1)
            {
                int h = CountBar.Size.Height - PositionBar.Size.Height;
                h = (int)((h / (float)(History.Count - 1)) * StartIndex);
                PositionBar.Location = new Point(Settings.Resolution != 800 ? 619 : 395, 16 + h);
            }


            //History Area Size
            //int y = 1;//add k333123 change
            int y = 19;

            for (int i = StartIndex; i < History.Count; i++)
            {
                ExineLabel temp = new ExineLabel
                {
                    AutoSize = true, //add k333123
                    BackColour = History[i].BackColour,
                    ForeColour = History[i].ForeColour,
                    //Location = new Point(1, y), //add k333123 change
                    Location = new Point(41, y), 
                    OutLine = false,
                    Parent = this,
                    Text = History[i].Text,
                    Font = ChatFont,
                };

                if(temp.Size.Width>300)
                {
                    temp.Size = new Size(300,temp.Size.Height);
                    y++;
                }

                temp.MouseWheel += ChatPanel_MouseWheel;
                ChatLines.Add(temp);

                temp.Click += (o, e) =>
                {
                    if (!(o is ExineLabel l)) return;

                    string[] parts = l.Text.Split(':', ' ');
                    if (parts.Length == 0) return;

                    string name = Regex.Replace(parts[0], "[^A-Za-z0-9]", "");

                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = string.Format("/{0} ", name);
                    ChatTextBox.Visible = true;
                    ExineMainScene.Scene.ExChatControl.Show(); //add k333123 add
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                };

                string currentLine = History[i].Text;

                int oldLength = currentLine.Length;

                Capture capture = null;

                foreach (Match match in RegexFunctions.ChatItemLinks.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList())
                {
                    try
                    {
                        int offSet = oldLength - currentLine.Length;

                        capture = match.Groups[1].Captures[0];
                        string[] values = capture.Value.Split('/');
                        currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, values[0]);
                        string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                        Size size = TextRenderer.MeasureText(CMain.Graphics, text, temp.Font, temp.Size, TextFormatFlags.TextBoxControl);

                        ChatLink(values[0], ulong.Parse(values[1]), temp.Location.Add(new Point(size.Width - 10, 0)));
                    }
                    catch (Exception ex)
                    {
                        //Temporary debug to catch unknown error
                        CMain.SaveError(ex.ToString());
                        CMain.SaveError(currentLine);
                        CMain.SaveError(capture.Value);
                        throw;
                    }
                }

                temp.Text = currentLine;

                y += 13;
                if (i - StartIndex == LineCount - 1) break;
            }

        }

        private void ChatLink(string name, ulong uniqueID, Point p)
        {
            UserItem item = ExineMainScene.ChatItemList.FirstOrDefault(x => x.UniqueID == uniqueID);

            if (item != null)
            {
                ExineLabel temp = new ExineLabel
                {
                    AutoSize = true,
                    Visible = true,
                    Parent = this,
                    Location = p,
                    Text = name,
                    ForeColour = Color.Blue,
                    Sound = SoundList.ButtonC,
                    Font = ChatFont,
                    OutLine = false,
                };

                temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
                temp.MouseLeave += (o, e) =>
                {
                    ExineMainScene.Scene.DisposeItemLabel();
                    temp.ForeColour = Color.Blue;
                };
                temp.MouseDown += (o, e) => temp.ForeColour = Color.Blue;
                temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

                temp.Click += (o, e) =>
                {
                    ExineMainScene.Scene.CreateItemLabel(item);
                };

                LinkedItemButtons.Add(temp);
            }
        }

        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (StartIndex == 0) return;
                    StartIndex--;
                    break;
                case Keys.Home:
                    if (StartIndex == 0) return;
                    StartIndex = 0;
                    break;
                case Keys.Down:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex++;
                    break;
                case Keys.End:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex = History.Count - 1;
                    break;
                case Keys.PageUp:
                    if (StartIndex == 0) return;
                    StartIndex -= LineCount;
                    break;
                case Keys.PageDown:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex += LineCount;
                    break;
                default:
                    return;
            }
            Update();
            e.Handled = true;
        }
        private void ChatPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '@':
                case '!':
                case ' ':
                case (char)Keys.Enter:
                    ChatTextBox.SetFocus();
                    if (e.KeyChar == '!') ChatTextBox.Text = "!";
                    if (e.KeyChar == '@') ChatTextBox.Text = "@";
                    if (ChatPrefix != "") ChatTextBox.Text = ChatPrefix;

                    ChatTextBox.Visible = true; 
                    ExineMainScene.Scene.ExChatControl.Show(); //add k333123 add
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;

                    e.Handled = true;
                    break;
                case '/':
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = LastPM + " ";
                    ChatTextBox.Visible = true;
                    
                    ExineMainScene.Scene.ExChatControl.Show();//add k333123 add
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        private void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == History.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }
        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }


        public void ChangeSize()
        {
            if (++WindowSize >= 3) WindowSize = 0;

            int y = DisplayRectangle.Bottom;
            switch (WindowSize)
            {
                case 0:
                    LineCount = 4;
                    Index = Settings.Resolution != 800 ? 2221 : 2201;
                    CountBar.Index = 2012;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45);
                    ChatTextBox.Location = new Point(1, 54);
                    break;
                case 1:
                    LineCount = 7;
                    Index = Settings.Resolution != 800 ? 2224 : 2204;
                    CountBar.Index = 2013;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 48);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 48);
                    ChatTextBox.Location = new Point(1, 54 + 48);
                    break;
                case 2:
                    LineCount = 11;
                    Index = Settings.Resolution != 800 ? 2227 : 2207;
                    CountBar.Index = 2014;
                    DownButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 39 + 96);
                    EndButton.Location = new Point(Settings.Resolution != 800 ? 618 : 394, 45 + 96);
                    ChatTextBox.Location = new Point(1, 54 + 96);
                    break;
            }

            Location = new Point(Location.X, y - Size.Height);

            UpdateBackground();

            Update();
        }

        public void UpdateBackground()
        {
            int offset = Transparent ? 1 : 0;

            switch (WindowSize)
            {
                case 0:
                    Index = Settings.Resolution != 800 ? 2221 : 2201;
                    break;
                case 1:
                    Index = Settings.Resolution != 800 ? 2224 : 2204;
                    break;
                case 2:
                    Index = Settings.Resolution != 800 ? 2227 : 2207;
                    break;
            }

            Index -= offset;
            
        }

        public class ChatHistory
        {
            public string Text;
            public Color ForeColour, BackColour;
            public ChatType Type;
        }
    }

    /// <summary>
    /// skill bar!!!
    /// </summary>
    public sealed class SkillBarDialog : ExineImageControl
    {
        private readonly ExineButton _switchBindsButton;

        public bool AltBind;
        public bool HasSkill = false;
        public byte BarIndex;

        //public bool TopBind = !Settings.SkillMode;
        public ExineImageControl[] Cells = new ExineImageControl[8];
        public ExineLabel[] KeyNameLabels = new ExineLabel[8];
        public ExineLabel BindNumberLabel = new ExineLabel();

        public ExineImageControl[] CoolDowns = new ExineImageControl[8];

        public SkillBarDialog()
        {
            Index = 2190;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point(0, BarIndex * 20);
            Visible = true;

            BeforeDraw += MagicKeyDialog_BeforeDraw;

            _switchBindsButton = new ExineButton
            {
                Index = 2247,
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(16, 28),
                Location = new Point(0, 0)
            };
            _switchBindsButton.Click += (o, e) =>
            {
                //Settings.SkillSet = !Settings.SkillSet;

                Update();
            };

            for (var i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new ExineImageControl
                {
                    Index = -1,
                    Library = Libraries.MagIcon,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                };
                int j = i + 1;
                Cells[i].Click += (o, e) =>
                {
                    ExineMainScene.Scene.UseSpell(j + (8 * BarIndex));
                };

                CoolDowns[i] = new ExineImageControl
                {
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Location = new Point(i * 25 + 15, 3),
                    NotControl = true,
                    UseOffSet = true,
                    Opacity = 0.6F
                };
            }

            BindNumberLabel = new ExineLabel
            {
                Text = "1",
                Font = new Font(Settings.FontName, 8F),
                ForeColour = Color.White,
                Parent = this,
                Location = new Point(0, 1),
                Size = new Size(10, 25),
                NotControl = true
            };

            for (var i = 0; i < KeyNameLabels.Length; i++)
            {
                KeyNameLabels[i] = new ExineLabel
                {
                    Text = "F" + (i + 1),
                    Font = new Font(Settings.FontName, 8F),
                    ForeColour = Color.White,
                    Parent = this,
                    Location = new Point(i * 25 + 13, 0),
                    Size = new Size(25, 25),
                    NotControl = true
                };
            }
            OnMoving += SkillBar_OnMoving;
        }

        private void SkillBar_OnMoving(object sender, MouseEventArgs e)
        {
            if (BarIndex * 2 >= Settings.SkillbarLocation.Length) return;
            Settings.SkillbarLocation[BarIndex, 0] = this.Location.X;
            Settings.SkillbarLocation[BarIndex, 1] = this.Location.Y;
        }

        private string GetKey(int barindex, int i)
        {
            //KeybindOptions Type = KeybindOptions.Bar1Skill1;
            if ((barindex == 0) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill1);
            if ((barindex == 0) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill2);
            if ((barindex == 0) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill3);
            if ((barindex == 0) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill4);
            if ((barindex == 0) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill5);
            if ((barindex == 0) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill6);
            if ((barindex == 0) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill7);
            if ((barindex == 0) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar1Skill8);
            if ((barindex == 1) && (i == 1))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill1);
            if ((barindex == 1) && (i == 2))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill2);
            if ((barindex == 1) && (i == 3))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill3);
            if ((barindex == 1) && (i == 4))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill4);
            if ((barindex == 1) && (i == 5))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill5);
            if ((barindex == 1) && (i == 6))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill6);
            if ((barindex == 1) && (i == 7))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill7);
            if ((barindex == 1) && (i == 8))
                return CMain.InputKeys.GetKey(KeybindOptions.Bar2Skill8);
            return "";
        }


        void MagicKeyDialog_BeforeDraw(object sender, EventArgs e)
        {
            Libraries.Prguse.Draw(2193, new Point(DisplayLocation.X + 12, DisplayLocation.Y), Color.White, true, 0.5F);
        }

        public void Update()
        {
            HasSkill = false;
            foreach (var m in ExineMainScene.User.Magics)
            {
                if ((m.Key < (BarIndex * 8) + 1) || (m.Key > ((BarIndex + 1) * 8) + 1)) continue;
                HasSkill = true;
            }

            if (!Visible) return;
            Index = 2190;
            _switchBindsButton.Index = 2247;
            BindNumberLabel.Text = (BarIndex + 1).ToString();
            BindNumberLabel.Location = new Point(0, 1);

            for (var i = 1; i <= 8; i++)
            {
                Cells[i - 1].Index = -1;

                int offset = BarIndex * 8;
                string key = GetKey(BarIndex, i);
                KeyNameLabels[i - 1].Text = key;

                foreach (var m in ExineMainScene.User.Magics)
                {
                    if (m.Key != i + offset) continue;
                    HasSkill = true;
                    ClientMagic magic = MapObject.User.GetMagic(m.Spell);
                    if (magic == null) continue;

                    //string key = m.Key > 8 ? string.Format("CTRL F{0}", i) : string.Format("F{0}", m.Key);

                    Cells[i - 1].Index = magic.Icon * 2;
                    Cells[i - 1].Hint = string.Format("{0}\nMP: {1}\nCooldown: {2}\nKey: {3}", magic.Name,
                        (magic.BaseCost + (magic.LevelCost * magic.Level)), Functions.PrintTimeSpanFromMilliSeconds(magic.Delay), key);

                    KeyNameLabels[i - 1].Text = "";
                }
            }
        }


        public void Process()
        {

            ProcessSkillDelay();
        }

        private void ProcessSkillDelay()
        {
            if (!Visible) return;

            int offset = BarIndex * 8;

            for (int i = 0; i < Cells.Length; i++)
            {
                foreach (var magic in ExineMainScene.User.Magics)
                {
                    if (magic.Key != i + offset + 1) continue;

                    int totalFrames = 22;
                    long timeLeft = magic.CastTime + magic.Delay - CMain.Time;

                    if (timeLeft < 100)
                    {
                        if (timeLeft > 0)
                        {
                            CoolDowns[i].Visible = false;
                            // CoolDowns[i].Dispose();
                        }
                        else
                            continue;
                    }

                    int delayPerFrame = (int)(magic.Delay / totalFrames);
                    int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

                    if ((CMain.Time <= magic.CastTime + magic.Delay))
                    {
                        CoolDowns[i].Visible = true;
                        CoolDowns[i].Index = 1260 + startFrame;
                    }
                }
            }
        }

        public override void Show()
        {
            if (Visible) return;
            if (!HasSkill) return;
            Settings.SkillBar = true;
            Visible = true;
            Update();
        }

        public override void Hide()
        {
            if (!Visible) return;
            Settings.SkillBar = false;
            Visible = false;
        }
    }

    public sealed class MiniMapDialog : ExineImageControl
    {
        //public ExineImageControl LightSetting, NewMail;
        //public MirButton ToggleButton, BigMapButton, MailButton;
        public ExineImageControl LightSetting;
        public ExineLabel LocationLabel, MapNameLabel;
        private float _fade = 1F;
        //private bool _bigMode = true;

        //public ExineLabel AModeLabel, PModeLabel;

        public List<ExineLabel> QuestIcons = new List<ExineLabel>();

        public MiniMapDialog()
        {
            //Index = 2090;
            //Library = Libraries.Prguse;
            Index = 0;
            Library = Libraries.PANEL0301;
            Location = new Point(715, 555);
            PixelDetect = true;
            Visible = true;

            BeforeDraw += MiniMap_BeforeDraw;
            AfterDraw += MiniMapDialog_AfterDraw;

            MapNameLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Size = new Size(120, 18),
                Location = new Point(2-18, 2+110),
                NotControl = true,
            };

            LocationLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Size = new Size(56, 18),
                Location = new Point(46 + 83, 30 + 77),
                NotControl = true,
            };

            LightSetting = new ExineImageControl
            {
                Index = 2093,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(102, 30),
                Visible = false,
            };

        }

        private void MiniMap_BeforeDraw(object sender, EventArgs e)
        {

        } 

        private void MiniMapDialog_AfterDraw(object sender, EventArgs e)
        {  
            foreach (var icon in QuestIcons)
                icon.Dispose();

            QuestIcons.Clear();

            MapControl map = ExineMainScene.Scene.MapControl;
            if (map == null) return; 

            //if (map.MiniMap == 0 && Index != 2091)
            //SetSmallMode();
             
            //if (map.MiniMap <= 0 || Index != 2090 || Libraries.ExineMiniMap == null)
            if (map.MiniMap < 0  || Libraries.ExineMiniMap == null)
            {
                return;
            } 

            ////182,92
            //Rectangle viewRect = new Rectangle(0, 0, 120, 108);
            Rectangle viewRect = new Rectangle(0, 0, 182, 92); //k333123
            Point drawLocation = Location;
            drawLocation.Offset(3+8, 22-8);

            Size miniMapSize = Libraries.ExineMiniMap.GetSize(map.MiniMap);
            float scaleX = miniMapSize.Width / (float)map.Width;
            float scaleY = miniMapSize.Height / (float)map.Height;
             
            viewRect.Location = new Point(
                (int)(scaleX * MapObject.User.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.User.CurrentLocation.Y) - viewRect.Height / 2);

            //   viewRect.Location = viewRect.Location.Subtract(1, 1);
            if (viewRect.Right >= miniMapSize.Width)
                viewRect.X = miniMapSize.Width - viewRect.Width;
            if (viewRect.Bottom >= miniMapSize.Height)
                viewRect.Y = miniMapSize.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            //Console.WriteLine("%%%map.MiniMap:" + map.MiniMap+ "drawLocation.x:"+ drawLocation.X+ " drawLocation.y:"+ drawLocation.Y);

            Libraries.ExineMiniMap.Draw(map.MiniMap, viewRect, drawLocation, Color.FromArgb(255, 255, 255), _fade);
            //Libraries.ExineMiniMap.Draw(map.MiniMap, viewRect, new Point(727,571), Color.FromArgb(255, 255, 255), _fade);


            int startPointX = (int)(viewRect.X / scaleX);
            int startPointY = (int)(viewRect.Y / scaleY);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];

                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell || ob.Sneaking) continue;
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + drawLocation.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + drawLocation.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.User != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else
                    if (ob is PlayerObject)
                {
                    colour = Color.FromArgb(255, 255, 255);
                }
                else if (ob is NPCObject || ob.AI == 6)
                {
                    colour = Color.FromArgb(0, 255, 50);
                }
                else
                    colour = Color.FromArgb(255, 0, 0);

                DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 2, 2), new Vector3((float)(x - 0.5), (float)(y - 0.5), 0.0F), colour);

                #region NPC Quest Icons

                if (ob is NPCObject npc && npc.GetAvailableQuests(true).Any())
                {
                    string text = "";
                    Color color = Color.Empty;

                    switch (npc.QuestIcon)
                    {
                        case QuestIcon.ExclamationBlue:
                            color = Color.DodgerBlue;
                            text = "!";
                            break;
                        case QuestIcon.ExclamationYellow:
                            color = Color.Yellow;
                            text = "!";
                            break;
                        case QuestIcon.ExclamationGreen:
                            color = Color.Green;
                            text = "!";
                            break;
                        case QuestIcon.QuestionBlue:
                            color = Color.DodgerBlue;
                            text = "?";
                            break;
                        case QuestIcon.QuestionWhite:
                            color = Color.White;
                            text = "?";
                            break;
                        case QuestIcon.QuestionYellow:
                            color = Color.Yellow;
                            text = "?";
                            break;
                        case QuestIcon.QuestionGreen:
                            color = Color.Green;
                            text = "?";
                            break;
                    }

                    QuestIcons.Add(new ExineLabel
                    {
                        AutoSize = true,
                        Parent = ExineMainScene.Scene.MiniMapDialog,
                        Font = new Font(Settings.FontName, 9f, FontStyle.Bold),
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                        Text = text,
                        ForeColour = color,
                        Location = new Point((int)(x - Settings.ScreenWidth + ExineMainScene.Scene.MiniMapDialog.Size.Width) - 6, (int)(y) - 10),
                        NotControl = true,
                        Visible = true,
                        Modal = true
                    });
                }

                #endregion

            }
        }
         

        private void SetSmallMode()
        {
            Index = 2091;
            int y = Size.Height - 23;
             
            LocationLabel.Location = new Point(46, y);
            LightSetting.Location = new Point(102, y);

            ExineMainScene.Scene.DuraStatusPanel.Location = new Point(ExineMainScene.Scene.MiniMapDialog.Location.X + 86,
            ExineMainScene.Scene.MiniMapDialog.Size.Height);
        }

        private void SetBigMode()
        {
             
        }

        public void Process()
        {
            MapControl map = ExineMainScene.Scene.MapControl;
            if (map == null) return;

            MapNameLabel.Text = map.Title;
            LocationLabel.Text = Functions.PointToString(MapObject.User.CurrentLocation);

            ExineMainScene.Scene.ExMainDialog.SModeLabel.Location = new Point((ExineMainScene.Scene.MiniMapDialog.Location.X - 3) - ExineMainScene.Scene.ExMainDialog.Location.X,
            (ExineMainScene.Scene.MiniMapDialog.Size.Height + 150) - Settings.ScreenHeight);
            ExineMainScene.Scene.ExMainDialog.AModeLabel.Location = new Point((ExineMainScene.Scene.MiniMapDialog.Location.X - 3) - ExineMainScene.Scene.ExMainDialog.Location.X,
            (ExineMainScene.Scene.MiniMapDialog.Size.Height + 165) - Settings.ScreenHeight);
            ExineMainScene.Scene.ExMainDialog.PModeLabel.Location = new Point((ExineMainScene.Scene.MiniMapDialog.Location.X - 3) - ExineMainScene.Scene.ExMainDialog.Location.X,
            (ExineMainScene.Scene.MiniMapDialog.Size.Height + 180) - Settings.ScreenHeight);

            /*
            if (ExineMainScene.Scene.NewMail)
            {
                double time = (CMain.Time) / 100D;

                if (Math.Round(time) % 10 < 5 || ExineMainScene.Scene.NewMailCounter >= 10)
                {
                    NewMail.Visible = true;
                }
                else
                {
                    if (NewMail.Visible)
                    {
                        ExineMainScene.Scene.NewMailCounter++;
                    }

                    NewMail.Visible = false;
                }
            }
            else
            {
                NewMail.Visible = false;
            }
            */
        }
    }
    public sealed class InspectDialog : ExineImageControl
    {
        public static UserItem[] Items = new UserItem[14];
        public static uint InspectID;

        public string Name;
        public string GuildName;
        public string GuildRank;
        public ExineClass Class;
        public ExineGender Gender;
        public byte Hair;
        public ushort Level;
        public string LoverName;
        public bool AllowObserve;

        public ExineButton CloseButton, GroupButton, FriendButton, TradeButton, LoverButton, ObserveButton;
        public ExineImageControl CharacterPage, ClassImage;
        public ExineLabel NameLabel;
        public ExineLabel GuildLabel, LoverLabel;



        public MirItemCell
            WeaponCell,
            ArmorCell,
            HelmetCell,
            TorchCell,
            NecklaceCell,
            BraceletLCell,
            BraceletRCell,
            RingLCell,
            RingRCell,
            AmuletCell,
            BeltCell,
            BootsCell,
            StoneCell;

        public InspectDialog()
        {
            Index = 430;
            Library = Libraries.Prguse;
            Location = new Point(536, 0);
            Movable = true;
            Sort = true;

            CharacterPage = new ExineImageControl
            {
                Index = 340,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(8, 70),
            };
            CharacterPage.AfterDraw += (o, e) =>
            {
                if (Libraries.StateItems == null) return;

                ItemInfo RealItem = null;

                if (ArmorCell.Item != null)
                {
                    RealItem = Functions.GetRealItem(ArmorCell.Item.Info, Level, Class, ExineMainScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X + 0, DisplayLocation.Y + -20), Color.White, true, 1F);

                    if (RealItem.Effect > 0)
                    {
                        int wingOffset = RealItem.Effect == 1 ? 2 : 4;

                        int genderOffset = MapObject.User.Gender == ExineGender.Male ? 0 : 1;

                        Libraries.Prguse2.DrawBlend(1200 + wingOffset + genderOffset, new Point(DisplayLocation.X, DisplayLocation.Y - 20), Color.White, true, 1F);
                    }
                }

                if (WeaponCell.Item != null)
                {
                    RealItem = Functions.GetRealItem(WeaponCell.Item.Info, Level, Class, ExineMainScene.ItemInfoList);
                    Libraries.StateItems.Draw(RealItem.Image, new Point(DisplayLocation.X, DisplayLocation.Y - 20),
                    Color.White, true, 1F);

                }

                if (HelmetCell.Item != null)
                    Libraries.StateItems.Draw(HelmetCell.Item.Info.Image, new Point(DisplayLocation.X, DisplayLocation.Y - 20), Color.White, true, 1F);
                else
                {
                    int hair = 441 + Hair + (Class == ExineClass.Assassin ? 20 : 0) + (Gender == ExineGender.Male ? 0 : 40);

                    int offSetX = Class == ExineClass.Assassin ? (Gender == ExineGender.Male ? 6 : 4) : 0;
                    int offSetY = Class == ExineClass.Assassin ? (Gender == ExineGender.Male ? 25 : 18) : 0;

                    Libraries.Prguse.Draw(hair, new Point(DisplayLocation.X + offSetX, DisplayLocation.Y + offSetY - 20), Color.White, true, 1F);
                }
            };


            CloseButton = new ExineButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(241, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();



            GroupButton = new ExineButton
            {
                HoverIndex = 432,
                Index = 431,
                Location = new Point(75, 357),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 433,
                Sound = SoundList.ButtonA,
                Hint = "Invite to Group",
            };
            GroupButton.Click += (o, e) =>
            {

                if (GroupDialog.GroupList.Count >= Globals.MaxGroup)
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("그룹멤버가 꽉찼습니다.", ChatType.System);
                    return;
                }
                if (GroupDialog.GroupList.Count > 0 && GroupDialog.GroupList[0] != MapObject.User.Name)
                {

                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 당신의 그룹의 리더가 아닙니다.", ChatType.System);
                }

                Network.SendPacketToServer(new ClientPacket.AddMember { Name = Name });
                return;
            };

            FriendButton = new ExineButton
            {
                HoverIndex = 435,
                Index = 434,
                Location = new Point(105, 357),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 436,
                Sound = SoundList.ButtonA,
                Hint = "Add to Friends List",
            };
            FriendButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.AddFriend { Name = Name, Blocked = false });
            };

            

            TradeButton = new ExineButton
            {
                HoverIndex = 524,
                Index = 523,
                Location = new Point(165, 357),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 525,
                Sound = SoundList.ButtonA,
                Hint = "Trade",
            };
            TradeButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.TradeRequest());

            ObserveButton = new ExineButton
            {
                Index = 854,
                HoverIndex = 855,
                PressedIndex = 856,
                Location = new Point(16, 357),
                Library = Libraries.Title,
                Parent = this,
                Sound = SoundList.ButtonA,
                Visible = false,
                Hint = "Observe",
            };
            ObserveButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.Observe { Name = Name });
            };

            NameLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(50, 12),
                Size = new Size(190, 20),
                NotControl = true
            };
            NameLabel.Click += (o, e) =>
            {
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.SetFocus();
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = string.Format("/{0} ", Name);
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Visible = true;
                ExineMainScene.Scene.ExChatControl.Show(); //add k333123 add
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionStart = Name.Length + 2;

            };
            LoverButton = new ExineButton
            {
                Index = 604,
                Location = new Point(17, 17),
                Library = Libraries.Prguse,
                Parent = this,
                Sound = SoundList.None
            };

            GuildLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(50, 33),
                Size = new Size(190, 30),
                NotControl = true,
            };

            ClassImage = new ExineImageControl
            {
                Index = 100,
                Library = Libraries.Prguse,
                Location = new Point(15, 33),
                Parent = this,
                NotControl = true,
            };


            WeaponCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Weapon,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(123, 7),
            };

            ArmorCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Armour,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(163, 7),
            };

            HelmetCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Helmet,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(203, 7),
            };


            TorchCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Torch,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(203, 134),
            };

            NecklaceCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Necklace,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(203, 98),
            };

            BraceletLCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletL,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 170),
            };
            BraceletRCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.BraceletR,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(203, 170),
            };
            RingLCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingL,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 206),
            };
            RingRCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.RingR,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(203, 206),
            };

            AmuletCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Amulet,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(8, 242),
            };

            BootsCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Boots,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(48, 242),
            };
            BeltCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Belt,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(88, 242),
            };

            StoneCell = new MirItemCell
            {
                ItemSlot = (int)EquipmentSlot.Stone,
                GridType = MirGridType.Inspect,
                Parent = CharacterPage,
                Location = new Point(128, 242),
            };

             
        }

        public void RefreshInferface(bool IsHero)
        {
            int offSet = Gender == ExineGender.Male ? 0 : 1;

            CharacterPage.Index = 340 + offSet;

            switch (Class)
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

            NameLabel.Text = Name;
            GuildLabel.Text = GuildName + " " + GuildRank;
            if (LoverName != "")
            {
                LoverButton.Visible = true;
                LoverButton.Hint = LoverName;
            }
            else
                LoverButton.Visible = false;


            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;
                ExineMainScene.Bind(Items[i]);
            }

            ObserveButton.Visible = AllowObserve;

            TradeButton.Visible = !IsHero;
             
            FriendButton.Visible = !IsHero;
            GroupButton.Visible = !IsHero;

        }


    }
    public sealed class OptionDialog : ExineImageControl
    {
        public ExineButton SkillModeOn, SkillModeOff;
        public ExineButton SkillBarOn, SkillBarOff;
        public ExineButton EffectOn, EffectOff;
        public ExineButton DropViewOn, DropViewOff;
        public ExineButton NameViewOn, NameViewOff;
        public ExineButton HPViewOn, HPViewOff;
        public ExineButton NewMoveOn, NewMoveOff;
        public ExineButton ObserveOn, ObserveOff;
        public ExineImageControl SoundBar, MusicSoundBar;
        public ExineImageControl VolumeBar, MusicVolumeBar;

        public ExineButton CloseButton;


        public OptionDialog()
        {
            Index = 411;
            Library = Libraries.Title;
            Movable = true;
            Sort = true;

            Location = new Point((Settings.ScreenWidth - Size.Width) / 2, (Settings.ScreenHeight - Size.Height) / 2);

            BeforeDraw += OptionPanel_BeforeDraw;

            CloseButton = new ExineButton
            {
                Index = 360,
                HoverIndex = 361,
                Library = Libraries.Prguse2,
                Location = new Point(Size.Width - 26, 5),
                Parent = this,
                Sound = SoundList.ButtonA,
                PressedIndex = 362,
            };
            CloseButton.Click += (o, e) => Hide();

            //tilde option
            SkillModeOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 451,
            };
            SkillModeOn.Click += (o, e) =>
            {
                ExineMainScene.Scene.ChangeSkillMode(false);
            };

            //ctrl option
            SkillModeOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 68),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 454
            };
            SkillModeOff.Click += (o, e) =>
            {
                ExineMainScene.Scene.ChangeSkillMode(true);
            };

            SkillBarOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            SkillBarOn.Click += (o, e) => Settings.SkillBar = true;

            SkillBarOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 93),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            SkillBarOff.Click += (o, e) => Settings.SkillBar = false;

            EffectOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            EffectOn.Click += (o, e) => Settings.Effect = true;

            EffectOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 118),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            EffectOff.Click += (o, e) => Settings.Effect = false;

            DropViewOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            DropViewOn.Click += (o, e) => Settings.DropView = true;

            DropViewOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 143),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            DropViewOff.Click += (o, e) => Settings.DropView = false;

            NameViewOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            NameViewOn.Click += (o, e) => Settings.NameView = true;

            NameViewOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 168),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            NameViewOff.Click += (o, e) => Settings.NameView = false;

            HPViewOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 463,
            };
            HPViewOn.Click += (o, e) =>
            {
                Settings.HPView = true;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[HP/MP Mode 1]", ChatType.Hint);
            };

            HPViewOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 193),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 466
            };
            HPViewOff.Click += (o, e) =>
            {
                Settings.HPView = false;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[HP/MP Mode 2]", ChatType.Hint);
            };

            SoundBar = new ExineImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 225),
                Parent = this,
                DrawImage = false,
            };
            SoundBar.MouseDown += SoundBar_MouseMove;
            SoundBar.MouseMove += SoundBar_MouseMove;
            SoundBar.BeforeDraw += SoundBar_BeforeDraw;

            VolumeBar = new ExineImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 218),
                Parent = this,
                NotControl = true,
            };

            MusicSoundBar = new ExineImageControl
            {
                Index = 468,
                Library = Libraries.Prguse2,
                Location = new Point(159, 251),
                Parent = this,
                DrawImage = false
            };
            MusicSoundBar.MouseDown += MusicSoundBar_MouseMove;
            MusicSoundBar.MouseMove += MusicSoundBar_MouseMove;
            MusicSoundBar.BeforeDraw += MusicSoundBar_BeforeDraw;

            MusicVolumeBar = new ExineImageControl
            {
                Index = 20,
                Library = Libraries.Prguse,
                Location = new Point(155, 244),
                Parent = this,
                NotControl = true,
            };

            NewMoveOn = new ExineButton
            {
                Library = Libraries.Title,
                Location = new Point(159, 296),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 853,
            };
            NewMoveOn.Click += (o, e) =>
            {
                Settings.NewMove = true;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[New Movement Style]", ChatType.Hint);
            };

            NewMoveOff = new ExineButton
            {
                Library = Libraries.Title,
                Location = new Point(201, 296),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 850
            };
            NewMoveOff.Click += (o, e) =>
            {
                Settings.NewMove = false;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[Old Movement Style]", ChatType.Hint);
            };

            ObserveOn = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(159, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 457,
            };
            ObserveOn.Click += (o, e) => ToggleObserve(true);

            ObserveOff = new ExineButton
            {
                Library = Libraries.Prguse2,
                Location = new Point(201, 271),
                Parent = this,
                Sound = SoundList.ButtonA,
                Size = new Size(36, 17),
                PressedIndex = 460
            };
            ObserveOff.Click += (o, e) => ToggleObserve(false);
        }

        private void ToggleObserve(bool allow)
        {
            if (ExineMainScene.AllowObserve == allow) return;

            Network.SendPacketToServer(new ClientPacket.Chat
            {
                Message = "@ALLOWOBSERVE",
            });
        }

        public void ToggleSkillButtons(bool Ctrl)
        {
            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if ((KeyCheck.function < KeybindOptions.Bar1Skill1) || (KeyCheck.function > KeybindOptions.Bar2Skill8)) continue;
                //need to test this 
                if ((KeyCheck.RequireCtrl != 1) && (KeyCheck.RequireTilde != 1)) continue;
                KeyCheck.RequireCtrl = (byte)(Ctrl ? 1 : 0);
                KeyCheck.RequireTilde = (byte)(Ctrl ? 0 : 1);
            }
        }

        private void SoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || SoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(SoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)SoundBar.Size.Width * 100);
            Settings.Volume = volume;

            double percent = Settings.Volume / 100D;

            SoundBar.Hint = $"{Settings.Volume}%";

            if (percent > 1) percent = 1;

            VolumeBar.Location = percent > 0 ? new Point(159 + (int)((SoundBar.Size.Width - 2) * percent), 218) : new Point(159, 218);
        }

        private void SoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (SoundBar.Library == null) return;

            double percent = Settings.Volume / 100D;

            SoundBar.Hint = $"{Settings.Volume}%";

            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((SoundBar.Size.Width - 2) * percent), SoundBar.Size.Height)
                };

                SoundBar.Library.Draw(SoundBar.Index, section, SoundBar.DisplayLocation, Color.White, false);
                VolumeBar.Location = new Point(159 + section.Size.Width, 218);
            }
            else
                VolumeBar.Location = new Point(159, 218);
        }

        private void MusicSoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (MusicSoundBar.Library == null) return;

            double percent = Settings.MusicVolume / 100D;

            MusicSoundBar.Hint = $"{Settings.MusicVolume}%";

            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                {
                    Size = new Size((int)((MusicSoundBar.Size.Width - 2) * percent), MusicSoundBar.Size.Height)
                };

                MusicSoundBar.Library.Draw(MusicSoundBar.Index, section, MusicSoundBar.DisplayLocation, Color.White, false);
                MusicVolumeBar.Location = new Point(159 + section.Size.Width, 244);
            }
            else
                MusicVolumeBar.Location = new Point(159, 244);
        }

        private void MusicSoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || MusicSoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(MusicSoundBar.DisplayLocation);

            byte volume = (byte)(p.X / (double)MusicSoundBar.Size.Width * 100);
            Settings.MusicVolume = volume;

            double percent = Settings.MusicVolume / 100D;

            MusicSoundBar.Hint = $"{Settings.MusicVolume}%";

            if (percent > 1) percent = 1;

            MusicVolumeBar.Location = percent > 0 ? new Point(159 + (int)((MusicSoundBar.Size.Width - 2) * percent), 244) : new Point(159, 244);
        }

        private void OptionPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (Settings.SkillMode)
            {
                SkillModeOn.Index = 452;
                SkillModeOff.Index = 453;
            }
            else
            {
                SkillModeOn.Index = 450;
                SkillModeOff.Index = 455;
            }

            if (Settings.SkillBar)
            {
                SkillBarOn.Index = 458;
                SkillBarOff.Index = 459;
            }
            else
            {
                SkillBarOn.Index = 456;
                SkillBarOff.Index = 461;
            }

            if (Settings.Effect)
            {
                EffectOn.Index = 458;
                EffectOff.Index = 459;
            }
            else
            {
                EffectOn.Index = 456;
                EffectOff.Index = 461;
            }

            if (Settings.DropView)
            {
                DropViewOn.Index = 458;
                DropViewOff.Index = 459;
            }
            else
            {
                DropViewOn.Index = 456;
                DropViewOff.Index = 461;
            }

            if (Settings.NameView)
            {
                NameViewOn.Index = 458;
                NameViewOff.Index = 459;
            }
            else
            {
                NameViewOn.Index = 456;
                NameViewOff.Index = 461;
            }

            if (Settings.HPView)
            {
                HPViewOn.Index = 464;
                HPViewOff.Index = 465;
            }
            else
            {
                HPViewOn.Index = 462;
                HPViewOff.Index = 467;
            }

            if (Settings.NewMove)
            {
                NewMoveOn.Index = 853;
                NewMoveOff.Index = 848;
            }
            else
            {
                NewMoveOn.Index = 851;
                NewMoveOff.Index = 850;
            }

            if (ExineMainScene.AllowObserve)
            {
                ObserveOn.Index = 458;
                ObserveOff.Index = 459;
            }
            else
            {
                ObserveOn.Index = 456;
                ObserveOff.Index = 461;
            }
        }

    }
    public sealed class MenuDialog : ExineImageControl
    {
        public ExineButton ExitButton,
                         LogOutButton,
                         HelpButton,
                         KeyboardLayoutButton,
                         RankingButton,
                         CraftingButton,  
                         FishingButton,
                         FriendButton,
                         MentorButton,
                         RelationshipButton,
                         GroupButton,
                         GuildButton;

        public MenuDialog()
        {
            Index = 567;
            Parent = ExineMainScene.Scene;
            Library = Libraries.Title;
            Location = new Point(Settings.ScreenWidth - Size.Width, ExineMainScene.Scene.ExMainDialog.Location.Y - this.Size.Height + 15);
            Sort = true;
            Visible = false;
            Movable = true;

            ExitButton = new ExineButton
            {
                HoverIndex = 634,
                Index = 633,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(3, 12),
                PressedIndex = 635,
                Hint = string.Format(GameLanguage.Exit, CMain.InputKeys.GetKey(KeybindOptions.Exit))
            };
            ExitButton.Click += (o, e) => ExineMainScene.Scene.QuitGame();

            LogOutButton = new ExineButton
            {
                HoverIndex = 637,
                Index = 636,
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(3, 31),
                PressedIndex = 638,
                Hint = string.Format(GameLanguage.LogOut, CMain.InputKeys.GetKey(KeybindOptions.Logout))
            };
            LogOutButton.Click += (o, e) => ExineMainScene.Scene.LogOut();


            HelpButton = new ExineButton
            {
                Index = 1970,
                HoverIndex = 1971,
                PressedIndex = 1972,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 50),
                Hint = string.Format(GameLanguage.Help, CMain.InputKeys.GetKey(KeybindOptions.Help))
            };
            HelpButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.HelpDialog.Visible)
                    ExineMainScene.Scene.HelpDialog.Hide();
                else ExineMainScene.Scene.HelpDialog.Show();
            };

            KeyboardLayoutButton = new ExineButton
            {
                Index = 1973,
                HoverIndex = 1974,
                PressedIndex = 1975,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 69),
                Visible = true,
                Hint = "Keyboard (" + CMain.InputKeys.GetKey(KeybindOptions.Keybind) + ")"
            };
            KeyboardLayoutButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.KeyboardLayoutDialog.Visible)
                    ExineMainScene.Scene.KeyboardLayoutDialog.Hide();
                else ExineMainScene.Scene.KeyboardLayoutDialog.Show();
            };

            RankingButton = new ExineButton
            {
                Index = 2000,
                HoverIndex = 2001,
                PressedIndex = 2002,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 88),
                Hint = string.Format(GameLanguage.Ranking, CMain.InputKeys.GetKey(KeybindOptions.Ranking))
                //Visible = false
            };
            RankingButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.RankingDialog.Visible)
                    ExineMainScene.Scene.RankingDialog.Hide();
                else ExineMainScene.Scene.RankingDialog.Show();
            };

            CraftingButton = new ExineButton
            {
                Index = 2000,
                HoverIndex = 2001,
                PressedIndex = 2002,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 107),
                Visible = false
            };
            CraftingButton.Click += (o, e) =>
            {

            };

             
            FriendButton = new ExineButton
            {
                Index = 1982,
                HoverIndex = 1983,
                PressedIndex = 1984,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 183),
                Visible = true,
                Hint = string.Format(GameLanguage.Friends, CMain.InputKeys.GetKey(KeybindOptions.Friends))
            };
            FriendButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.FriendDialog.Visible)
                    ExineMainScene.Scene.FriendDialog.Hide();
                else ExineMainScene.Scene.FriendDialog.Show();
            };

            MentorButton = new ExineButton
            {
                Index = 1985,
                HoverIndex = 1986,
                PressedIndex = 1987,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 202),
                Visible = true,
                Hint = string.Format(GameLanguage.Mentor, CMain.InputKeys.GetKey(KeybindOptions.Mentor))
            };
            MentorButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.MentorDialog.Visible)
                    ExineMainScene.Scene.MentorDialog.Hide();
                else ExineMainScene.Scene.MentorDialog.Show();
            };


            RelationshipButton = new ExineButton  /* lover button */
            {
                Index = 1988,
                HoverIndex = 1989,
                PressedIndex = 1990,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 221),
                Visible = true,
                Hint = string.Format(GameLanguage.Relationship, CMain.InputKeys.GetKey(KeybindOptions.Relationship))
            };
            RelationshipButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.RelationshipDialog.Visible)
                    ExineMainScene.Scene.RelationshipDialog.Hide();
                else ExineMainScene.Scene.RelationshipDialog.Show();
            };

            GroupButton = new ExineButton
            {
                Index = 1991,
                HoverIndex = 1992,
                PressedIndex = 1993,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 240),
                Hint = string.Format(GameLanguage.Groups, CMain.InputKeys.GetKey(KeybindOptions.Group))
            };
            GroupButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.GroupDialog.Visible)
                    ExineMainScene.Scene.GroupDialog.Hide();
                else ExineMainScene.Scene.GroupDialog.Show();
            };

            GuildButton = new ExineButton
            {
                Index = 1994,
                HoverIndex = 1995,
                PressedIndex = 1996,
                Parent = this,
                Library = Libraries.Prguse,
                Location = new Point(3, 259),
                Hint = string.Format(GameLanguage.Guild, CMain.InputKeys.GetKey(KeybindOptions.Guilds))
            };
            GuildButton.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.GuildDialog.Visible)
                    ExineMainScene.Scene.GuildDialog.Hide();
                else ExineMainScene.Scene.GuildDialog.Show();
            };

        }


    }

    //k333123 add 240324

    public sealed class ExMagicInactiveButton : ExineControl
    {
        public ExineButton MagicInactiveIcon;
        public ExMagicInactiveButton()
        {
            MagicInactiveIcon = new ExineButton
            {
                Index = 0,
                PressedIndex = 0,
                Library = Libraries.ArtsIcon,//Libraries.ArtsIcon,//4-Arts.lib
                Parent = this,
                Location = new Point(0, 0),
                //Location = new Point(36, 0),
                Sound = SoundList.ButtonA,
            };

            MagicInactiveIcon.Click += (o, e) =>
            {
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@Click!!!");
            };
        }
    }


    public sealed class ExMagicButton : ExineControl
    {
        public ExineImageControl LevelImage, ExpImage;
        public ExineButton SkillButton;
        public ExineLabel LevelLabel, NameLabel, ExpLabel, KeyLabel;
        public ClientMagic Magic;
        public ExineImageControl CoolDown;
        public bool HeroMagic;

        string[] Prefixes = new string[] { "", "CTRL", "Shift" };

        public ExMagicButton()
        {
            Size = new Size(231, 33);

            SkillButton = new ExineButton
            {
                Index = 0,
                PressedIndex = 0,
                Library = Libraries.ArtsIcon,//Libraries.ArtsIcon,//4-Arts.lib
                Parent = this,
                Location = new Point(0, 0),
                //Location = new Point(36, 0),
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) =>
            {
               
                    new AssignKeyPanel(Magic, 1, new string[]
                        {
                            "F1",
                            "F2",
                            "F3",
                            "F4",
                            "F5",
                            "F6",
                            "F7",
                            "F8",
                            "Ctrl" + Environment.NewLine + "F1",
                            "Ctrl" + Environment.NewLine + "F2",
                            "Ctrl" + Environment.NewLine + "F3",
                            "Ctrl" + Environment.NewLine + "F4",
                            "Ctrl" + Environment.NewLine + "F5",
                            "Ctrl" + Environment.NewLine + "F6",
                            "Ctrl" + Environment.NewLine + "F7",
                            "Ctrl" + Environment.NewLine + "F8"
                        })
                    { Actor = ExineMainScene.User };
                
            };

            LevelImage = new ExineImageControl
            {
                Index = 516,
                Library = Libraries.Title,
                Location = new Point(73, 7),
                Parent = this,
                NotControl = true,
            };

            ExpImage = new ExineImageControl
            {
                Index = 517,
                Library = Libraries.Title,
                Location = new Point(73, 19),
                Parent = this,
                NotControl = true,
            };

            LevelLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(88, 2),
                NotControl = true,
            };

            NameLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 2),
                NotControl = true,
            };

            ExpLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 15),
                NotControl = true,
            };

            KeyLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(2, 2),
                NotControl = true,
            };

            CoolDown = new ExineImageControl
            {
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(36, 0),
                Opacity = 0.6F,
                NotControl = true,
                UseOffSet = true,
            };
        }

        public void Update(ClientMagic magic)
        {
            Magic = magic;

            NameLabel.Text = Magic.Name;

            LevelLabel.Text = Magic.Level.ToString();
            switch (Magic.Level)
            {
                case 0:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need1);
                    break;
                case 1:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need2);
                    break;
                case 2:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need3);
                    break;
                case 3:
                    ExpLabel.Text = "-";
                    break;
            }

            KeyLabel.Text = Magic.Key == 0 ? string.Empty : string.Format("{0}{1}F{2}",
                Prefixes[(Magic.Key - 1) / 8],
                Magic.Key > 8 ? Environment.NewLine : string.Empty,
                (Magic.Key - 1) % 8 + 1);

            switch (magic.Spell)
            {  //Warrior
                case Spell.Fencing:
                    SkillButton.Hint = string.Format("Fencing \n\nHitting accuracy will be increased in accordance\nwith practice level.\nPassive Skill\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;

                default:

                    break;
            }

            SkillButton.Index = Magic.Icon ;
            SkillButton.PressedIndex = Magic.Icon ;

            //SkillButton.Index = Magic.Icon * 2;
            //SkillButton.PressedIndex = Magic.Icon * 2 + 1;

            SetDelay();
        }

        public void SetDelay()
        {
            if (Magic == null) return;

            int totalFrames = 34;

            long timeLeft = Magic.CastTime + Magic.Delay - CMain.Time;

            if (timeLeft < 100)
            {
                CoolDown.Visible = false;
                return;
            }

            int delayPerFrame = (int)(Magic.Delay / totalFrames);
            int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

            if ((CMain.Time <= Magic.CastTime + Magic.Delay))
            {
                CoolDown.Visible = true;
                CoolDown.Index = 1290 + startFrame;
            }
        }
    }


    /*
    public sealed class MagicButton : ExineControl
    {
        public ExineImageControl LevelImage, ExpImage;
        public MirButton SkillButton;
        public ExineLabel LevelLabel, NameLabel, ExpLabel, KeyLabel;
        public ClientMagic Magic;
        public ExineImageControl CoolDown;
        public bool HeroMagic;

        string[] Prefixes = new string[] { "", "CTRL", "Shift" };

        public MagicButton()
        {
            Size = new Size(231, 33);

            SkillButton = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.MagIcon2,
                Parent = this,
                Location = new Point(36, 0),
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) =>
            {
                if (HeroMagic)
                {
                    if (ExineMainScene.Hero == null || ExineMainScene.Hero.Dead)
                        return;
                    new AssignKeyPanel(Magic, 17, new string[]
                        {
                            "Shift" + Environment.NewLine + "F1",
                            "Shift" + Environment.NewLine + "F2",
                            "Shift" + Environment.NewLine + "F3",
                            "Shift" + Environment.NewLine + "F4",
                            "Shift" + Environment.NewLine + "F5",
                            "Shift" + Environment.NewLine + "F6",
                            "Shift" + Environment.NewLine + "F7",
                            "Shift" + Environment.NewLine + "F8"
                        })
                    { Actor = ExineMainScene.Hero };
                }
                else
                {
                    new AssignKeyPanel(Magic, 1, new string[]
                        {
                            "F1",
                            "F2",
                            "F3",
                            "F4",
                            "F5",
                            "F6",
                            "F7",
                            "F8",
                            "Ctrl" + Environment.NewLine + "F1",
                            "Ctrl" + Environment.NewLine + "F2",
                            "Ctrl" + Environment.NewLine + "F3",
                            "Ctrl" + Environment.NewLine + "F4",
                            "Ctrl" + Environment.NewLine + "F5",
                            "Ctrl" + Environment.NewLine + "F6",
                            "Ctrl" + Environment.NewLine + "F7",
                            "Ctrl" + Environment.NewLine + "F8"
                        })
                    { Actor = ExineMainScene.User };
                }
            };

            LevelImage = new ExineImageControl
            {
                Index = 516,
                Library = Libraries.Title,
                Location = new Point(73, 7),
                Parent = this,
                NotControl = true,
            };

            ExpImage = new ExineImageControl
            {
                Index = 517,
                Library = Libraries.Title,
                Location = new Point(73, 19),
                Parent = this,
                NotControl = true,
            };

            LevelLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(88, 2),
                NotControl = true,
            };

            NameLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 2),
                NotControl = true,
            };

            ExpLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 15),
                NotControl = true,
            };

            KeyLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(2, 2),
                NotControl = true,
            };

            CoolDown = new ExineImageControl
            {
                Library = Libraries.Prguse2,
                Parent = this,
                Location = new Point(36, 0),
                Opacity = 0.6F,
                NotControl = true,
                UseOffSet = true,
            };
        }

        public void Update(ClientMagic magic)
        {
            Magic = magic;

            NameLabel.Text = Magic.Name;

            LevelLabel.Text = Magic.Level.ToString();
            switch (Magic.Level)
            {
                case 0:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need1);
                    break;
                case 1:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need2);
                    break;
                case 2:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need3);
                    break;
                case 3:
                    ExpLabel.Text = "-";
                    break;
            }

            KeyLabel.Text = Magic.Key == 0 ? string.Empty : string.Format("{0}{1}F{2}",
                Prefixes[(Magic.Key - 1) / 8],
                Magic.Key > 8 ? Environment.NewLine : string.Empty,
                (Magic.Key - 1) % 8 + 1);

            switch (magic.Spell)
            {  //Warrior
                case Spell.Fencing:
                    SkillButton.Hint = string.Format("Fencing \n\nHitting accuracy will be increased in accordance\nwith practice level.\nPassive Skill\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Slaying:
                    SkillButton.Hint = string.Format("Slaying \n\nHitting accuracy and destructive power will\nbe increased in accordance with practive level.\nPassive Skill\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Thrusting:
                    SkillButton.Hint = string.Format("Dark Damage\nThrusting \n\nIncreases the reach of your hits destructive power\nwill increase in accordance with practive level.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Rage:
                    SkillButton.Hint = string.Format("Rage \n\nMana Cost {2}\n\nEnhances your inner force to increase its power\nfor a certain time. Attack power and duration time\nwill depend on the skill level. Once the skill has been used\n you will have to wait to use it again.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ProtectionField:
                    SkillButton.Hint = string.Format("ProtectionField \n\nMana Cost {2}\n\nConcentrates inner force and spreads it to all\n the parts of your body. This will enhance the\nprotection from enemies. Defense power and duration\nwill be depend on the skill level. Once the skill\n has been used, you will have to wait to use it again.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.HalfMoon:
                    SkillButton.Hint = string.Format("Wind Damage\nHalfMoon \n\nCause damage to mobs in a semi circle with\nthe shock waves from your fast moving weapon.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.FlamingSword:
                    SkillButton.Hint = string.Format("Fire Damage\nFlamingSword \n\nCause additional damage by summoning the spirit\nof fire into weapon\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.ShoulderDash:
                    SkillButton.Hint = string.Format("ShoulderDash \n\nA warrior can push away mobs by charging\nthem with his shoulder, inflicting damage\nif they hit any obstacle.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.CrossHalfMoon:
                    SkillButton.Hint = string.Format("Wind Damage\nCrossHalfMoon \n\nA warrior uses two powerfull waves of Half Moon\nto inflict damage on all mobs stood next to them.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.TwinDrakeBlade:
                    SkillButton.Hint = string.Format("Dark Damage\nTwinDrakeBlade \n\nThe art of making multiple power attacks. It has a\nlow chance of stunning the mob temporarly. Stunned\nmobs get 1.5 times more damage inflicted.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.Entrapment:
                    SkillButton.Hint = string.Format("Entrapment \n\nParalyses mobs and draws them to the caster.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.LionRoar:
                    SkillButton.Hint = string.Format("LionRoar \n\nParalyses mobs , duration increases with skill level.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                case Spell.CounterAttack:
                    SkillButton.Hint = string.Format("CounterAttack \n\nMana Cost {2}\n\nIncreases AC and AMC for a short period of time\nChance to defend an attack and counter.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ImmortalSkin:
                    SkillButton.Hint = string.Format("ImmortalSkin \n\nMana Cost {2}\n\nIncrease defense to reduce attacks.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Fury:
                    SkillButton.Hint = string.Format("Fury \n\nMana Cost {2}\n\nIncreases the warriors Accuracy for a set period of time.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SlashingBurst:
                    SkillButton.Hint = string.Format("SlashingBurst \n\nMana Cost {2}\n\nAllows The Warrior to Jump 1 Space Over a Obejct or Monster.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.BladeAvalanche:
                    SkillButton.Hint = string.Format("Ice Damage\nBladeAvalanche \n\n3-Way Thrusting.\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0);
                    break;
                //Wizard
                case Spell.FireBall:
                    SkillButton.Hint = string.Format("Fireball \n\nInstant Casting\nMana Cost {2}\n\nElements of fire are gathered to form\na fireball. Throw at monsters for damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ThunderBolt:
                    SkillButton.Hint = string.Format("Thundebolt \n\nInstant Casting\nMana Cost {2}\n\nStrikes the foe with a lightning bolt \ninflicting high damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.GreatFireBall:
                    SkillButton.Hint = string.Format("GreatFireBall \n\nInstant Casting\nMana Cost {2}\n\nStronger then fire ball, Great Fire Ball\nwill fire up the mobs.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Repulsion:
                    SkillButton.Hint = string.Format("Repulsion \n\nInstant Casting\nMana Cost {2}\n\nPush away mobs useing the power of fire.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.HellFire:
                    SkillButton.Hint = string.Format("Hellfire \n\nInstant Casting\nMana Cost {2}\n\nShoots out a streak of fire attack\nthe monster in front.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Lightning:
                    SkillButton.Hint = string.Format("Lightning \n\nInstant Casting\nMana Cost {2}\n\nShoots out a steak of lightning to attack\nthe monster in front.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ElectricShock:
                    SkillButton.Hint = string.Format("ElectrickShock \n\nInstant Casting\nMana Cost {2}\n\nStrong shock wave hits the mob and the\nmob will not be able to move or the mob\nwill get confused and fight for you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Teleport:
                    SkillButton.Hint = string.Format("Teleport \n\nInstant Casting\nMana Cost {2}\n\nTeleport to a random spot.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FireWall:
                    SkillButton.Hint = string.Format("FireWall \n\nInstant Casting\nMana Cost {2}\n\nThis skill will build a fire wall at a designated\nspot to attack the monster passing the area.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FireBang:
                    SkillButton.Hint = string.Format("FireBang \n\nInstant Casting\nMana Cost {2}\n\nFirebang will burst out fire at a designated spot to\nburn all the monster within the area.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.ThunderStorm:
                    SkillButton.Hint = string.Format("Thunderstorm \n\nInstant Casting\nMana Cost {2}\n\nThis skill will make a thunder storm with in a designated area \nto attack the monster with in.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MagicShield:
                    SkillButton.Hint = string.Format("MagicShield \n\nInstant Casting\nMana Cost {2}\n\nThis skill will use Mp to create protective\nlayer around you\nAttack will be absorbed by the protective layer\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.TurnUndead:
                    SkillButton.Hint = string.Format("TurnUndead \n\nInstant Casting\nMana Cost {2}\n\nThis magic will bring birght light into \npower and attack undead monsters\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.IceStorm:
                    SkillButton.Hint = string.Format("IceStorm \n\nInstant Castin\nMana Cost {2}\n\nThis skill will make an ice storm with in a designated \narea to attack the monsters with in\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FlameDisruptor:
                    SkillButton.Hint = string.Format("FlameDisruptor \n\nInstant Casting\nMana Cost {2}\n\nFlame from the underground will be brought\ninto surface to attack the mobs.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FrostCrunch:
                    SkillButton.Hint = string.Format("FrostCrunch \n\nInstant Casting\nMana Cost {2}\n\nFreeze the elements in the air around the \nmonster to slow them down\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Mirroring:
                    SkillButton.Hint = string.Format("Mirroring \n\nInstant Casting\nMana Cost {2}\n\nCreate a mirror image of yourself to attack\nthe monsters together\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FlameField:
                    SkillButton.Hint = string.Format("FlameField \n\nInstant Casting\nMana Cost {2}\n\nA powerful spell of fire is used to \ndamage surrounding enemies.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Vampirism:
                    SkillButton.Hint = string.Format("Vampirism \n\nInstant Casting\nMana Cost {2}\n\nUsing Mp take away monsters Hp to\nincrease your Hp.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Blizzard:
                    SkillButton.Hint = string.Format("Blizzard \n\nChanneling Casting\nMana Cost {2}\n\nConcentrate inner force and spreads it to all\nthe parts of your body.This will enhance the\nprotection from enemies. Defense power and duration\ntime will depend on the skill level. Once the skill\nhas been used, you will have to wait to use it again.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MeteorStrike:
                    SkillButton.Hint = string.Format("MeteorStrike \n\nChanneling Casting\nMana Cost {2}\n\nAttacks all monsters within 5x5 square area with lumps \nof fire falling from the sky.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.IceThrust:
                    SkillButton.Hint = string.Format("IceThrust \n\nInstant CastingMana Cost {2}\n\nAttack monsters by creating an ice pillar.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MagicBooster:
                    SkillButton.Hint = string.Format("MagicBooster \n\nLasting EffectMana Cost {2}\n\nIncrease magical damage, but comsume additional MP.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FastMove:
                    SkillButton.Hint = string.Format("FastMove \n\nChanneling Casting\nMana Cost {2}\n\nIncrease movemoent with rooted skills.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.StormEscape:
                    SkillButton.Hint = string.Format("StormEscape \n\nChanneling Casting\nMana Cost {2}\n\nParalyze nearby enemies and teleport to the designated location.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Blink:
                    SkillButton.Hint = string.Format("Blink \n\nInstant Casting\nMana Cost {2}\n\nTeleport to a random spot near you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                //Taoist
                case Spell.SpiritSword:
                    SkillButton.Hint = string.Format("SpiritSword \n\nIncreases the chance of hitting the target in\n melee combat.\nPassive Skill\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Healing:
                    SkillButton.Hint = string.Format("Healing \n\nInstant Casting\nMana Cost {2}\n\nHeals a single target \nrecovering HP over time.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Poisoning:
                    SkillButton.Hint = string.Format("Poisoning \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Poison Powder\n\nThrow poison at mobs to weaken them.\nUse green poison to weaken Hp.\nUse red poison to weaken defense.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SoulFireBall:
                    SkillButton.Hint = string.Format("SoulFireBall \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nPut power into a scroll and throw it at \na mob. The scroll will burst into fire.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SoulShield:
                    SkillButton.Hint = string.Format("SoulShield \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nBless the partymembers to strengthen there magic\ndefence.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.BlessedArmour:
                    SkillButton.Hint = string.Format("BlessedArmour \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nBless the partymemebers to strenghten there defence.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.TrapHexagon:
                    SkillButton.Hint = string.Format("TrapHexagon \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nTrap the monster with this magical power\n to stop them from moving. Any damages\nfrom outside source will allow the monsters\nto move again.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SummonSkeleton:
                    SkillButton.Hint = string.Format("SummonSkeleton \n\n\nInstant Casting\nMana Cost {2}\n\nSummons a Powerful AOE Skeleton, Which will Fight Side By Side With You\n\nRequired Items: Amulet.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Hiding:
                    SkillButton.Hint = string.Format("Hiding \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nMobs will not be able to spot you for a short\nmoment.Mobs will notice you if you start\nto move around.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MassHiding:
                    SkillButton.Hint = string.Format("MassHiding \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nMobs will not be able to spot you or your \nparty members for a short moment. \nMobs will notice you and your party if \nyou start to move around.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Revelation:
                    SkillButton.Hint = string.Format("Revelation \n\nInstant Casting\nMana Cost {2}\n\nYou will be able to read Hp of others\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MassHealing:
                    SkillButton.Hint = string.Format("MassHealing \n\nInstant Casting\nMana Cost {2}\n\nHeal all injured players in the specified\narea by surrounding them with mana.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SummonShinsu:
                    SkillButton.Hint = string.Format("SummonShinsu \n\nInstant Casting\nMana Cost {2}\n\nSummons a Dog, That Will fight Side By Side with you.\nRequired Items: Amulet.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.UltimateEnhancer:
                    SkillButton.Hint = string.Format("UltimateEnhancer \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nAbsorb the energy from the surroundings to increase the stats.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.EnergyRepulsor:
                    SkillButton.Hint = string.Format("EnergyRepulsor \n\nInstant Casting\nMana Cost {2}\n\nConcentrate your energy for one big blast to push away the monsters around you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Purification:
                    SkillButton.Hint = string.Format("Purification \n\nInstant Casting\nMana Cost {2}\n\nHelp others to recover from poisoning and\nparalysis useing this skill.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SummonHolyDeva:
                    SkillButton.Hint = string.Format("SummonHolyDeva \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nSummon a holy spirit.This holy spirit will\nuse strong thunder to attack monsters.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Curse:
                    SkillButton.Hint = string.Format("Curse \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet + Poison\n\nReduces mob attacks (Attack Speed, DC ,MC ,SC)\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Hallucination:
                    SkillButton.Hint = string.Format("Hallucination \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nThe monster will only see hallucination \nand attack anyone on the way\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Reincarnation:
                    SkillButton.Hint = string.Format("Reincarnation \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nRevives a dead players\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.PoisonCloud:
                    SkillButton.Hint = string.Format("PoisonCloud \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: GreenPoison\n\nThrow the amulet and a very strong\npoison cloud will appear in the area.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.EnergyShield:
                    SkillButton.Hint = string.Format("EnergyShield \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet\n\nCreate an enegy shield to heal immediately when attacked by monsters.\nCurrent Skill Level {0}\n\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Plague:
                    SkillButton.Hint = string.Format("Plague \n\nInstant Casting\nMana Cost {2}\n\nRequired Items: Amulet + Poison\n\nDecreases targets MP and inflict target with various debuffs\nExample: Stun , Curse , Poison and Slow.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.HealingCircle:
                    SkillButton.Hint = string.Format("HealingCircle \n\nInstant Casting\nMana Cost {2}\n\nTreatment area friendly target, and the enemy caused spell damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.PetEnhancer:
                    SkillButton.Hint = string.Format("PetEnhancer \n\nInstant Casting\nMana Cost {2}\n\nStrengthening pets defense and power.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;

                //Assassin
                case Spell.FatalSword:
                    SkillButton.Hint = string.Format("FatalSword \n\nIncrease attack damage on the monsters.\nalso increases accuracy a little.\nPassive Skill\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.DoubleSlash:
                    SkillButton.Hint = string.Format("DoubleSlash \n\nMana Cost {2}\n\nSlash the monster twice in a quick motion\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Haste:
                    SkillButton.Hint = string.Format("Haste \n\nMana Cost {2}\n\nIncrease the attack speed\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FlashDash:
                    SkillButton.Hint = string.Format("FlashDash \n\nMana Cost {2}\n\nAttack a monster with quick slash and\nparalize the monster\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.HeavenlySword:
                    SkillButton.Hint = string.Format("HeavenlySword \n\nMana Cost {2}\n\nAttack monsters with in 2 steps radius\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.FireBurst:
                    SkillButton.Hint = string.Format("FireBurst \n\nMana Cost {2}\n\nPush away mobs surrounding you\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Trap:
                    SkillButton.Hint = string.Format("Trap \n\nInstant casting CoolTime 60 secs\n\nMana Cost {2}\n\nTrap the monster for a short while.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MoonLight:
                    SkillButton.Hint = string.Format("Moonlight \n\nMana Cost {2}\n\nHide yourself from monster by turning invisible\nGreater damage is done when you attack monster using\nthis skill.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MPEater:
                    SkillButton.Hint = string.Format("MpEater \n\nPassive\nMana Cost {2}\n\nAbsord monsters MP to recharge your MP\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.SwiftFeet:
                    SkillButton.Hint = string.Format("SwiftFeet \n\nMana Cost {2}\n\nIncreased Runing Speed\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.LightBody:
                    SkillButton.Hint = string.Format("LightBody \n\nMana Cost {2}\n\nLighten your body using this skill and move faster\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.PoisonSword:
                    SkillButton.Hint = string.Format("PoisonSword \n\nMana Cost {2}\n\nPoison the monsters with a slash of you\nsword.Poison effect will damage the monster\nover time.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.DarkBody:
                    SkillButton.Hint = string.Format("DarkBody \n\nMana Cost {2}\n\nCreate an illusion of yourself to attack\nthe monster while you become invisible.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.CrescentSlash:
                    SkillButton.Hint = string.Format("CrescentSlash \n\nMana Cost {2}\n\nBurst out of the power of your sword and attack all monsters around you.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.Hemorrhage:
                    SkillButton.Hint = string.Format("Hemorrhage \n\nMana Cost {2}\n\nPassive\nChance to deal cristical damage and inflict bleeding damage.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;
                case Spell.MoonMist:
                    SkillButton.Hint = string.Format("Moon Mist\nActive\n\nMana Cost {2}\n\nAbility to hide your self from Monster\nYour first attack will be stronger than normal.\n\nCurrent Skill Level {0}\nNext Level {1}", Magic.Level, Magic.Level == 0 ? Magic.Level1 : Magic.Level == 1 ? Magic.Level2 : Magic.Level == 2 ? Magic.Level3 : 0, Magic.BaseCost);
                    break;

                default:

                    break;
            }


            SkillButton.Index = Magic.Icon * 2;
            SkillButton.PressedIndex = Magic.Icon * 2 + 1;

            SetDelay();
        }

        public void SetDelay()
        {
            if (Magic == null) return;

            int totalFrames = 34;

            long timeLeft = Magic.CastTime + Magic.Delay - CMain.Time;

            if (timeLeft < 100)
            {
                CoolDown.Visible = false;
                return;
            }

            int delayPerFrame = (int)(Magic.Delay / totalFrames);
            int startFrame = totalFrames - (int)(timeLeft / delayPerFrame);

            if ((CMain.Time <= Magic.CastTime + Magic.Delay))
            {
                CoolDown.Visible = true;
                CoolDown.Index = 1290 + startFrame;
            }
        }
    }
    */
    public sealed class AssignKeyPanel : ExineImageControl
    {
        public ExineButton SaveButton, NoneButton;
        public UserObject Actor;
        public ExineLabel TitleLabel;
        public ExineImageControl MagicImage;
        public ExineButton[] FKeys;

        public ClientMagic Magic;
        public byte Key;
        public byte KeyOffset;

        public AssignKeyPanel(ClientMagic magic, byte keyOffset, string[] keyStrings)
        {
            Magic = magic;
            Key = magic.Key;
            KeyOffset = keyOffset;

            Modal = true;
            Index = 710;
            Library = Libraries.Prguse;
            Location = Center;
            Parent = ExineMainScene.Scene;
            Visible = true;

            MagicImage = new ExineImageControl
            {
                Location = new Point(16, 16),
                Index = magic.Icon * 2,
                Library = Libraries.MagIcon2,
                Parent = this,
            };

            TitleLabel = new ExineLabel
            {
                Location = new Point(49, 17),
                Parent = this,
                Size = new Size(230, 32),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                Text = string.Format(GameLanguage.SelectKey, magic.Name)
            };

            NoneButton = new ExineButton
            {
                Index = 287, //154
                HoverIndex = 288,
                PressedIndex = 289,
                Library = Libraries.Title,
                Parent = this,
                Location = new Point(284, 64),
            };
            NoneButton.Click += (o, e) => Key = 0;

            SaveButton = new ExineButton
            {
                Library = Libraries.Title,
                Parent = this,
                Location = new Point(284, 101),
                Index = 156,
                HoverIndex = 157,
                PressedIndex = 158,
            };
            SaveButton.Click += (o, e) =>
            {
                for (int i = 0; i < Actor.Magics.Count; i++)
                {
                    if (Actor.Magics[i].Key == Key)
                        Actor.Magics[i].Key = 0;
                }

                Network.SendPacketToServer(new ClientPacket.MagicKey { Spell = Magic.Spell, Key = Key, OldKey = Magic.Key });
                Magic.Key = Key;
                foreach (SkillBarDialog Bar in ExineMainScene.Scene.SkillBarDialogs)
                    Bar.Update();

                Dispose();
            };

            FKeys = new ExineButton[keyStrings.Length];

            for (byte i = 0; i < FKeys.Length; i++)
            {
                FKeys[i] = new ExineButton
                {
                    Index = 0,
                    PressedIndex = 1,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(17 + 32 * (i % 8) + 5 * (i % 8 / 4), 58 + 37 * (i / 8)),
                    Sound = SoundList.ButtonA,
                    Text = keyStrings[i]
                };
                int num = i + keyOffset;
                FKeys[i].Click += (o, e) =>
                {
                    Key = (byte)num;
                };
            }

            BeforeDraw += AssignKeyPanel_BeforeDraw;
        }

        private void AssignKeyPanel_BeforeDraw(object sender, EventArgs e)
        {
            for (int i = 0; i < FKeys.Length; i++)
            {
                FKeys[i].Index = 1656;
                FKeys[i].HoverIndex = 1657;
                FKeys[i].PressedIndex = 1658;
                FKeys[i].Visible = true;
            }

            int key = Key - KeyOffset;
            if (key < 0 || key > FKeys.Length) return;

            FKeys[key].Index = 1658;
            FKeys[key].HoverIndex = 1658;
            FKeys[key].PressedIndex = 1658;
        }
    }
    public sealed class DuraStatusDialog : ExineImageControl
    {
        public ExineButton Character;

        public DuraStatusDialog()
        {
            Size = new Size(40, 19);
            Location = new Point((ExineMainScene.Scene.MiniMapDialog.Location.X + 86), ExineMainScene.Scene.MiniMapDialog.Size.Height);

            Character = new ExineButton()
            {
                Index = 2113,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(20, 19),
                Location = new Point(20, 0),
                HoverIndex = 2111,
                PressedIndex = 2112,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.DuraPanel
            };
            Character.Click += (o, e) =>
            {
                if (ExineMainScene.Scene.CharacterDuraPanel.Visible == true)
                {
                    ExineMainScene.Scene.CharacterDuraPanel.Hide();
                    Settings.DuraView = false;
                }
                else
                {
                    ExineMainScene.Scene.CharacterDuraPanel.Show();
                    Settings.DuraView = true;
                }
            };
        }

    }
    public sealed class CharacterDuraPanel : ExineImageControl
    {
        public ExineImageControl GrayBackground, Background, Helmet, Armour, Belt, Boots, Weapon, Necklace, RightBracelet, LeftBracelet, RightRing, LeftRing, Torch, Stone, Amulet, Mount, Item1, Item2;

        public CharacterDuraPanel()
        {
            Index = 2105;
            Library = Libraries.Prguse;
            Movable = false;
            Location = new Point(Settings.ScreenWidth - 61, 200);

            GrayBackground = new ExineImageControl()
            {
                Index = 2161,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(56, 80),
                Location = new Point(3, 3),
                Opacity = 0.4F
            };
            Background = new ExineImageControl()
            {
                Index = 2162,
                Library = Libraries.Prguse,
                Parent = this,
                Size = new Size(56, 80),
                Location = new Point(3, 3),
            };

            #region Pieces

            Helmet = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(24, 3) };
            Belt = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 7), Location = new Point(23, 23) };
            Armour = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(28, 32), Location = new Point(16, 11) };
            Boots = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(24, 9), Location = new Point(17, 43) };
            Weapon = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 33), Location = new Point(4, 5) };
            Necklace = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(3, 67) };
            LeftBracelet = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 8), Location = new Point(3, 43) };
            RightBracelet = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 8), Location = new Point(43, 43) };
            LeftRing = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(3, 54) };
            RightRing = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(43, 54) };
            Torch = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 32), Location = new Point(44, 5) };
            Stone = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(30, 54) };
            Amulet = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(16, 54) };
            Mount = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(12, 12), Location = new Point(43, 68) };
            Item1 = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 12), Location = new Point(19, 67) };
            Item2 = new ExineImageControl() { Index = -1, Library = Libraries.Prguse, Parent = Background, Size = new Size(8, 12), Location = new Point(31, 67) };

            #endregion
        }

        public void GetCharacterDura()
        {
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[0].Item == null) { Weapon.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[1].Item == null) { Armour.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[2].Item == null) { Helmet.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[3].Item == null) { Torch.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[4].Item == null) { Necklace.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[5].Item == null) { LeftBracelet.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[6].Item == null) { RightBracelet.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[7].Item == null) { LeftRing.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[8].Item == null) { RightRing.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[9].Item == null) { Amulet.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[10].Item == null) { Belt.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[11].Item == null) { Boots.Index = -1; }
            if (ExineMainScene.Scene.ExCharacterDialog.Grid[12].Item == null) { Stone.Index = -1; }

            for (int i = 0; i < MapObject.User.Equipment.Length; i++)
            {
                if (MapObject.User.Equipment[i] == null) continue;
                UpdateCharacterDura(MapObject.User.Equipment[i]);
            }
        }
        public void UpdateCharacterDura(UserItem item)
        {
            int Warning = item.MaxDura / 2;
            int Danger = item.MaxDura / 5;
            ushort AmuletWarning = (ushort)(item.Info.StackSize / 2);
            ushort AmuletDanger = (ushort)(item.Info.StackSize / 5);

            switch (item.Info.Type)
            {
                case ItemType.Amulet: //Based on stacks of 5000
                    if (item.Count > AmuletWarning)
                        Amulet.Index = 2134;
                    if (item.Count <= AmuletWarning)
                        Amulet.Index = 2135;
                    if (item.Count <= AmuletDanger)
                        Amulet.Index = 2136;
                    if (item.Count == 0)
                        Amulet.Index = -1;
                    break;
                case ItemType.Armour:
                    if (item.CurrentDura > Warning)
                        Armour.Index = 2149;
                    if (item.CurrentDura <= Warning)
                        Armour.Index = 2150;
                    if (item.CurrentDura <= Danger)
                        Armour.Index = 2151;
                    if (item.CurrentDura == 0)
                        Armour.Index = -1;
                    break;
                case ItemType.Belt:
                    if (item.CurrentDura > Warning)
                        Belt.Index = 2158;
                    if (item.CurrentDura <= Warning)
                        Belt.Index = 2159;
                    if (item.CurrentDura <= Danger)
                        Belt.Index = 2160;
                    if (item.CurrentDura == 0)
                        Belt.Index = -1;
                    break;
                case ItemType.Boots:
                    if (item.CurrentDura > Warning)
                        Boots.Index = 2152;
                    if (item.CurrentDura <= Warning)
                        Boots.Index = 2153;
                    if (item.CurrentDura <= Danger)
                        Boots.Index = 2154;
                    if (item.CurrentDura == 0)
                        Boots.Index = -1;
                    break;
                case ItemType.Bracelet:
                    if (ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.BraceletR].Item != null && item.UniqueID == ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.BraceletR].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            RightBracelet.Index = 2143;
                        if (item.CurrentDura <= Warning)
                            RightBracelet.Index = 2144;
                        if (item.CurrentDura <= Danger)
                            RightBracelet.Index = 2145;
                        if (item.CurrentDura == 0)
                            RightBracelet.Index = -1;
                    }
                    else if (ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.BraceletL].Item != null && item.UniqueID == ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.BraceletL].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            LeftBracelet.Index = 2143;
                        if (item.CurrentDura <= Warning)
                            LeftBracelet.Index = 2144;
                        if (item.CurrentDura <= Danger)
                            LeftBracelet.Index = 2145;
                        if (item.CurrentDura == 0)
                            LeftBracelet.Index = -1;
                    }
                    break;
                case ItemType.Helmet:
                    if (item.CurrentDura > Warning)
                        Helmet.Index = 2155;
                    if (item.CurrentDura <= Warning)
                        Helmet.Index = 2156;
                    if (item.CurrentDura <= Danger)
                        Helmet.Index = 2157;
                    if (item.CurrentDura == 0)
                        Helmet.Index = -1;
                    break;
                case ItemType.Necklace:
                    if (item.CurrentDura > Warning)
                        Necklace.Index = 2122;
                    if (item.CurrentDura <= Warning)
                        Necklace.Index = 2123;
                    if (item.CurrentDura <= Danger)
                        Necklace.Index = 2124;
                    if (item.CurrentDura == 0)
                        Necklace.Index = -1;
                    break;
                case ItemType.Ring:
                    if (ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.RingR].Item != null && item.UniqueID == ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.RingR].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            RightRing.Index = 2131;
                        if (item.CurrentDura <= Warning)
                            RightRing.Index = 2132;
                        if (item.CurrentDura <= Danger)
                            RightRing.Index = 2133;
                        if (item.CurrentDura == 0)
                            RightRing.Index = -1;
                    }
                    else if (ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.RingL].Item != null && item.UniqueID == ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.RingL].Item.UniqueID)
                    {
                        if (item.CurrentDura > Warning)
                            LeftRing.Index = 2131;
                        if (item.CurrentDura <= Warning)
                            LeftRing.Index = 2132;
                        if (item.CurrentDura <= Danger)
                            LeftRing.Index = 2133;
                        if (item.CurrentDura == 0)
                            LeftRing.Index = -1;
                    }
                    break;
                case ItemType.Stone:
                    if (item.CurrentDura == 0)
                        Stone.Index = 2137;
                    break;
                 
                case ItemType.Torch:
                    if (item.CurrentDura > Warning)
                        Torch.Index = 2146;
                    if (item.CurrentDura <= Warning)
                        Torch.Index = 2147;
                    if (item.CurrentDura <= Danger)
                        Torch.Index = 2148;
                    if (item.CurrentDura == 0)
                        Torch.Index = -1;
                    break;
                case ItemType.Weapon:
                    if (item.CurrentDura > Warning)
                        Weapon.Index = 2125;
                    if (item.CurrentDura <= Warning)
                        Weapon.Index = 2126;
                    if (item.CurrentDura <= Danger)
                        Weapon.Index = 2127;
                    if (item.CurrentDura == 0)
                        Weapon.Index = -1;
                    break;
            }
        }

        public override void Hide()
        {
            if (!Visible) return;
            Visible = false;
            ExineMainScene.Scene.DuraStatusPanel.Character.Index = 2113;
        }
        public override void Show()
        {
            if (Visible) return;
            Visible = true;
            ExineMainScene.Scene.DuraStatusPanel.Character.Index = 2110;

            GetCharacterDura();
        }
    }
}

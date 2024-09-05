using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
using S = ServerPackets;
using C = ClientPackets;

namespace Exine.ExineScenes
{
    public sealed class ExineNewCharScene : ExineScene
    {
        
        private ExineAnimatedControl  _background, _light1, _light2, _light3, _light4, _genderOrb;//_background2
        private MirButton _okBtn, _cancelBtn, _styleBtnL, _styleBtnR, _colorBtnL, _colorBtnR;
        private ExineImageControl _charImage;
        private ExineTextBox _newIdTextBox, _newPwTextBox, _newConfirmPwTextBox;
        int colorIdx = 0;
        int ganderType = 0;
        int styleIdx = 0;

        public ExineNewCharScene()
        {
            SoundManager.PlayMusic(SoundList.ExineIntroMusic, true);
            Disposing += (o, e) => SoundManager.StopMusic();

            /*
            _background2 = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 193,
                AnimationDelay = 20,
                Index = 0,
                Library = Libraries.ExineNewChar,
                Loop = true,
                Parent = this,
                Visible = false,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
            };*/

           

            _background = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 0,
                Library = Libraries.PANEL0001,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
            };

           
            _newPwTextBox = new ExineTextBox
            {
                ForeColour = Color.White,
                Parent = this,
                Font = new Font(Settings.FontName, 10F),
                Location = new Point((1024 - 800) / 2+168, (768 - 600) / 2+ 185),
                Size = new Size(121, 21),
                Visible = true,
                Password=true,
                Text = "",
            };
            _newConfirmPwTextBox = new ExineTextBox
            {
                ForeColour = Color.White,
                Parent = this,
                Font = new Font(Settings.FontName, 10F),
                Location = new Point((1024 - 800) / 2 + 168, (768 - 600) / 2+ 221),
                Size = new Size(121, 21),
                Visible = true,
                Password = true,
                Text = "",
            };
            _newIdTextBox = new ExineTextBox
            {
                ForeColour = Color.White,
                Parent = this,
                Font = new Font(Settings.FontName, 10F),
                Location = new Point((1024 - 800) / 2 + 168, (768 - 600) / 2 + 148),
                Size = new Size(121, 21),
                Visible = true,
                Text = "",

            }; 
            //PANEL0001 


            //light3 116,11 size:475,210
            _light3 = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 151,
                AnimationDelay = 35,
                Index = 0,
                Library = Libraries.BIK_023_Light_3,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2 + 115, (768 - 600) / 2 + 10),
            };

            //light4 686,11  size:45,120
            _light4 = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 88,
                AnimationDelay = 35,
                Index = 0,
                Library = Libraries.BIK_024_Light_4,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2 + 685, (768 - 600) / 2 + 10),
            };

            
            Color[] tintColors = new Color[28];
            tintColors[0] = Color.FromArgb(150, Color.Black);
            tintColors[1] = Color.FromArgb(150, Color.Blue);
            tintColors[2] = Color.FromArgb(150, Color.Brown);
            tintColors[3] = Color.FromArgb(150, Color.Coral);
            tintColors[4] = Color.FromArgb(150, Color.Crimson);
            tintColors[5] = Color.FromArgb(150, Color.Cyan);
            tintColors[6] = Color.FromArgb(150, Color.Fuchsia);
            tintColors[7] = Color.FromArgb(150, Color.Gold);
            tintColors[8] = Color.FromArgb(150, Color.Gray);
            tintColors[9] = Color.FromArgb(150, Color.Green);
            tintColors[10] = Color.FromArgb(150, Color.Indigo);
            tintColors[11] = Color.FromArgb(150, Color.Khaki);
            tintColors[12] = Color.FromArgb(150, Color.Lavender);
            tintColors[13] = Color.FromArgb(150, Color.LawnGreen);
            tintColors[14] = Color.FromArgb(150, Color.Lime);
            tintColors[15] = Color.FromArgb(150, Color.Linen);
            tintColors[16] = Color.FromArgb(150, Color.Magenta);
            tintColors[17] = Color.FromArgb(150, Color.Maroon);
            tintColors[18] = Color.FromArgb(150, Color.Navy);
            tintColors[19] = Color.FromArgb(150, Color.Olive);
            tintColors[20] = Color.FromArgb(150, Color.Orange);
            tintColors[21] = Color.FromArgb(150, Color.Pink);
            tintColors[22] = Color.FromArgb(150, Color.Purple);
            tintColors[23] = Color.FromArgb(150, Color.Red);
            tintColors[24] = Color.FromArgb(150, Color.Silver);
            tintColors[25] = Color.FromArgb(150, Color.Violet);
            tintColors[26] = Color.FromArgb(150, Color.White);
            tintColors[27] = Color.FromArgb(150, Color.Yellow);


            _charImage = new ExineImageControl
            {
                Index = 0,
                //Library = Libraries.AHM_0000,
                Library = Libraries.NewChar,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 400-23, (768 - 600) / 2 + 100+9),
                Visible = true,
                TintColour = tintColors[0],
                //TintColour = Color.FromArgb(100, Color.DarkRed),
                //ForeColour = Color.FromArgb(230, Color.White),
                //Blending=true,
            };

            //light1 10,430 size:400,100
            _light1 = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 151,
                AnimationDelay = 35,
                Index = 0,
                Library = Libraries.BIK_021_Light_1,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2+ 10, (768 - 600) / 2+430),
            };

            //light2 11,241 size:400,100
            _light2 = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 144,
                AnimationDelay = 35,
                Index = 0,
                Library = Libraries.BIK_022_Light_2,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2 +10, (768 - 600) / 2 + 240),
            };



            
            //_genderOrb 0~59 645,445 size:156,155
            _genderOrb = new ExineAnimatedControl
            {
                Animated = true,
                //AnimationCount = 193,
                AnimationCount = 59,
                AnimationDelay = 35,
                Index = 0,
                Library = Libraries.BIK_017_Orb3_2,
                Loop = true,
                Parent = this,
                Visible = true,
                Location = new Point((1024 - 800) / 2 + 644, (768 - 600) / 2 + 445),
            };
            _genderOrb.Click += (o, e) =>
            {
                if (_genderOrb.Library == Libraries.BIK_017_Orb3_2)
                {
                    _genderOrb.Library = Libraries.BIK_018_Orb3_3;
                    _charImage.Index = 3;
                    //ganderType = 3;
                    ganderType = (int)ExineGender.Female;
                    _genderOrb.AfterAnimation += (o, e) =>
                    {
                        _genderOrb.Library = Libraries.BIK_019_Orb3_4;

                    };
                }
                else if (_genderOrb.Library == Libraries.BIK_019_Orb3_4)
                {
                    _genderOrb.Library = Libraries.BIK_016_Orb3_1;
                    _charImage.Index = 0;
                    //ganderType = 0;
                    ganderType = (int)ExineGender.Male;
                    _genderOrb.AfterAnimation += (o, e) =>
                    {
                        _genderOrb.Library = Libraries.BIK_017_Orb3_2;

                    };
                }
            };


            //_styleBtnL 1~4 73,537
            _styleBtnL = new MirButton
            {
                Index = 1,
                HoverIndex = 2,
                PressedIndex = 3,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 73, (768 - 600) / 2 + 537),
                Visible = true,
            };
            _styleBtnL.Click += (o, e) =>
            {
               
                _charImage.Index--;

                if(ganderType==(int)ExineGender.Male && _charImage.Index < 0)
                {
                    _charImage.Index = 0;
                }
                //else if (ganderType == 0 && _charImage.Index > 2)
                else if (ganderType == (int)ExineGender.Male && _charImage.Index > 2)
                {
                    _charImage.Index = 2;
                }

                //else if (ganderType == 3 && _charImage.Index < 3)
                else if (ganderType == (int)ExineGender.Female && _charImage.Index < 3)
                {
                    _charImage.Index = 3;
                }
                //else if (ganderType == 3 && _charImage.Index > 5)
                else if (ganderType == (int)ExineGender.Female && _charImage.Index > 5)
                {
                    _charImage.Index = 5;
                }

                styleIdx = _charImage.Index % 3;

            };

                //_styleBtnR 5~8 ,223,537
            _styleBtnR = new MirButton
            {
                Index = 5,
                HoverIndex = 6,
                PressedIndex = 7,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 223, (768 - 600) / 2 + 537),
                Visible = true,
            };
            _styleBtnR.Click += (o, e) =>
            {
                _charImage.Index++;

                if (ganderType == (int)ExineGender.Male && _charImage.Index < 0)
                {
                    _charImage.Index = 0;
                } 
                else if (ganderType == (int)ExineGender.Male && _charImage.Index > 2)
                {
                    _charImage.Index = 2;
                }

                else if (ganderType == (int)ExineGender.Female && _charImage.Index < 3)
                {
                    _charImage.Index = 3;
                }
                else if (ganderType == (int)ExineGender.Female && _charImage.Index > 5)
                {
                    _charImage.Index = 5;
                }
                styleIdx = _charImage.Index % 3;
            };

            //_colorBtnL 9~12 300,537
            

           

            _colorBtnL = new MirButton
            {
                Index = 9,
                HoverIndex = 10,
                PressedIndex = 11,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 300, (768 - 600) / 2 + 537),
                Visible = true,
            };
            _colorBtnL.Click += (o, e) =>
            {
                
                colorIdx--;
                if (colorIdx<0) colorIdx = 0;
                _charImage.TintColour = tintColors[colorIdx];
            }; 

            //_colorBtnR 13~16 ,453,537
            _colorBtnR = new MirButton
            {
                Index = 13,
                HoverIndex = 14,
                PressedIndex = 15,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 453, (768 - 600) / 2 + 537),
                Visible = true,
            };
            _colorBtnR.Click += (o, e) =>
            {
                colorIdx++;
                if (colorIdx > tintColors.Length-1) colorIdx = tintColors.Length-1;
                _charImage.TintColour = tintColors[colorIdx]; 
            };

            //ok 17~20 69,430
            _okBtn = new MirButton
            {
                Index = 17,
                HoverIndex = 18,
                PressedIndex = 19,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 66, (768 - 600) / 2 + 421),
                Visible = true,
            };

            //_newPwTextBox
            _okBtn.Click += (o, e) =>
            {
                if (_newPwTextBox.Text != _newConfirmPwTextBox.Text) return;

                Console.WriteLine("id:{0} pw:{1} rePw:{2}", _newIdTextBox.Text, _newPwTextBox.Text, _newConfirmPwTextBox.Text);
                Network.Enqueue(new C.NewAccount
                {
                    AccountID = _newIdTextBox.Text,//"필11111",
                    Password = _newPwTextBox.Text,//"12481248",
                    EMailAddress = "",
                    BirthDate = !string.IsNullOrEmpty("")
                                       ? DateTime.Parse("")
                                       : DateTime.MinValue,
                    UserName = "",
                    SecretQuestion = "",
                    SecretAnswer = "",
                });
               
            };


            //cancel 21~24 10,480
            _cancelBtn = new MirButton
            {
                Index = 21,
                HoverIndex = 22,
                PressedIndex = 23,
                Library = Libraries.PANEL0001,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 14, (768 - 600) / 2 + 466),
                Visible = true,
            };
            _cancelBtn.Click += (o, e) =>
            {
                ExineScene.ActiveScene = new ExineLoginScene();
                Dispose();
            };
        }


        static string ServerPackIdsToString(ServerPacketIds serverPacketIds)
        {
            return serverPacketIds.ToString();
        }

        public override void Process()
        {

        }

        public static List<SelectInfo> charList = new List<SelectInfo>();

        public static byte[] GetBytesFromJpg(string path)
        {
            //max 8000bytes`
            //\Exine\RData\Profiles\charname.jpg  72*72
            Stream jpgStream = File.Open(Application.StartupPath + path, FileMode.Open);
            Image image = Image.FromStream(jpgStream);
            var stream = new MemoryStream();
            //image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] imageBytes = stream.ToArray();
            Console.WriteLine("imageBytes[0] : {0}, imageBytes.Length:{1}", imageBytes[0], imageBytes.Length);
            jpgStream.Close();
            return imageBytes;
        }

        override
        public void ProcessPacket(Packet recvPacket)
        {
            Console.WriteLine("recvPacket.Type :" + ServerPackIdsToString((ServerPacketIds)recvPacket.Index));
            switch (recvPacket.Index)
            {
                case (short)ServerPacketIds.NewAccount:
                    Console.Write("Recv ServerPacketIds.NewAccount " + "result :" + ((S.NewAccount)recvPacket).Result);
                    //password not apply special charactor

                    //exist : 7  //ok : 8
                    if (((S.NewAccount)recvPacket).Result == 7 || ((S.NewAccount)recvPacket).Result == 8)
                    {
                        Console.WriteLine(" > Send [Login]");

                        //load Photo data from file 
                        /*
                        int photoDataLen = 0;
                        byte[] photoData = new byte[8000];
                        if (File.Exists(_newIdTextBox.Text + ".jpg"))
                        {
                            byte[] filedata = GetBytesFromJpg(_newIdTextBox.Text + ".jpg");
                            if (filedata.Length <= 8000)
                            {
                                Buffer.BlockCopy(filedata, 0, photoData, 0, filedata.Length);
                                photoDataLen = filedata.Length;
                            }
                        }
                        */
                        Network.Enqueue(
                            new C.Login
                            {
                                AccountID = _newIdTextBox.Text,
                                Password = _newPwTextBox.Text
                            }
                        );
                    }
                    break;

                case (short)ServerPacketIds.LoginSuccess:
                    Console.WriteLine("Recv ServerPacketIds.LoginSuccess -> Send [NewCha]");
                    charList.Clear();
                    charList.AddRange(((S.LoginSuccess)recvPacket).Characters);
                    for (int i = 0; i < charList.Count; i++)
                    {
                        Console.WriteLine("name : " + charList[i].Name + "Index : " + charList[i].Index);
                    }

                    //have to Make CharacterPortraitUpdate Packet.

                    ////byte[] exPortraitBytes = GetBytesFromJpg("test.jpg");
                    int photoDataLen = 0;
                    byte[] photoData = new byte[8000];
                    if (File.Exists(_newIdTextBox.Text + ".jpg"))
                    {
                        byte[] filedata = GetBytesFromJpg(_newIdTextBox.Text + ".jpg");
                        if (filedata.Length <= 8000)
                        {
                            Buffer.BlockCopy(filedata, 0, photoData, 0, filedata.Length);
                            photoDataLen = filedata.Length;
                        }
                    }

                    //Select Scene
                    Network.Enqueue(
                         new C.NewCharacter
                         {
                             Name = _newIdTextBox.Text,//"필11111",
                             Class = 0, //Change to Color
                             Gender = (ExineGender)ganderType, //0:male, 1:female
                             ExStyle = (ExineStyle)styleIdx,//ExineStyle.Style0,
                             ExColor = (ExineColor)colorIdx, //ExineColor.Color3, 
                             ExPortraitBytes = photoData,
                             ExPortraitLen = photoDataLen,
                             //ExPortraitBytes =new byte[8000],
                             //ExPortraitLen = 8000,
                             //Portrait = new byte[50] { },
                             //231107
                         }
                    );
                    break;

                case (short)ServerPacketIds.NewCharacterSuccess: 
                    Network.Enqueue(new C.LogOut { });
                    ActiveScene = new ExineLoginScene();
                    Dispose();
                    break;

                case (short)ServerPacketIds.NewCharacter: 
                    Network.Enqueue(new C.LogOut{});
                    ActiveScene = new ExineLoginScene();
                    Dispose();
                    break;
            }
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _background = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}

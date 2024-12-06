using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
//

using System.Net.Http.Headers;
//using static Client.MirScenes.LoginScene;

namespace Exine.ExineScenes
{
    public sealed class ExineLoginScene : ExineScene
    {
        public static bool isClientCheckOK = false;
        public static string idFromNewChar = "";


        private ExineAnimatedControl _background, _orb1, _orb2, _gargoyle;//_background2, 
        private ExineButton _okBtn, _newBtn, _cancelBtn, _quitBtn, _pkBtn, _npkBtn, _upBtn, _downBtn, _serverBtn1, _serverBtn2, _serverBtn3, _serverBtn4, _loginDialog;
        private ExineTextBox _loginIdTextBox, _loginPwTextBox;
        private ExineImageControl _lastLogin;
        private ExineLabel _lastLoginInfo;


        public ExineLabel Version;

        //private LoginDialog _login;
        //private NewAccountDialog _account;
        //private ChangePasswordDialog _password;

        private ExineMessageBox _connectBox;

        //private InputKeyDialog _ViewKey;

        public ExineImageControl TestLabel, ViolenceLabel, MinorLabel, YouthLabel;

        public ExineLoginScene()
        {
            Console.WriteLine("ExineLoginScene()");

            SoundManager.PlayMusic(SoundList.ExineIntroMusic, true);
            SoundManager.PlaySound(SoundList.ExineGargoyle_Stop, false);

            Disposing += (o, e) =>
            {
                Console.WriteLine("ExineLoginScene() Disposing");
                SoundManager.StopSound(SoundList.ExineGargoyle_Halwling);
                SoundManager.StopMusic();
            };

            _background = new ExineAnimatedControl
            {
                Animated = false,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 0,
                Library = Libraries.PANEL0000,
                Loop = true,
                Parent = this,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Visible = true,
            };

            //차후 개별 조합으로 변경할것.
            /*
            _background2 = new ExineAnimatedControl
            {
                Animated = true,
                AnimationCount = 176,
                AnimationDelay = 20,
                Index = 0,
                Library = Libraries.ExineLogin,
                Loop = true,
                Parent = this,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Visible = false,
            };*/

            //1 => npk -> pk
            //2 => pk sel
            //3 => pk wait
            //4 => pk -> npk
            //5 => npk sel
            //6 => npk wait 
            //count 1-49, 2-56,  3-49, 4-48, 5-55, 6-48
            _gargoyle = new ExineAnimatedControl
            {
                Animated = true,
                AnimationCount = 48, 
                AnimationDelay = 34,
                Index = 0,
                //Library = Libraries.BIK_015_Gargoyle_6,
                Library = Libraries.BIK_015_Gargoyle_6,
                Loop = true,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 170, (768 - 600) / 2),
                //Location = new Point((1024 - 800) + 58, (768 - 600) - 83),
                Visible = true,
            };


            //down
            _orb1 = new ExineAnimatedControl
            {
                Animated = true,
                AnimationCount = 60,
                AnimationDelay = 34,
                Index = 0,
                Library = Libraries.BIK_005_Orb1_4,
                Loop = true,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 690, (768 - 600) / 2 + 500),
                Visible = true,
            };

            //up
            _orb2 = new ExineAnimatedControl
            {
                Animated = true,
                AnimationCount = 60,
                AnimationDelay = 34,
                Index = 0,
                Library = Libraries.BIK_009_Orb2_4,
                Loop = true,
                Parent = this,
                Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Visible = true,
            };

            _loginDialog = new ExineButton
            {
                Index = 2,
                HoverIndex = 3,
                PressedIndex = 4,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 525, (768 - 600) / 2 + 94),
                Visible = true,
                //BackColour=Color.Black,
            };
            //login dialog - id
            //login dialog - pw

            
            //_loginIdTextBox.MultiLine();
            _loginPwTextBox = new ExineTextBox
            {
                ForeColour = Color.White,
                Parent = _loginDialog,
                Font = new Font(Settings.FontName, 10F),
                Location = new Point(104, 149),
                Size = new Size(114, 21),
                Visible=true,
                Password=true,
                Text = "test123",
            };
            _loginIdTextBox = new ExineTextBox
            {
                ForeColour = Color.White,
                Parent = _loginDialog,
                Font = new Font(Settings.FontName, 10F),
                Location = new Point(58, 103),
                Size = new Size(114, 21),
                Visible = true,
                Text = "test123",//필순이
            };
            _loginDialog.Hide();


            //0,8
            _serverBtn1 = new ExineButton
            { 
                //0~2
                Index = 0,
                HoverIndex = 1,
                PressedIndex = 0,

                Library = Libraries.ServerButton,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 58, (768 - 600) / 2 + 188),
                Visible = true,
            };
            _serverBtn1.Click += (o, e) =>
            {
                 
            };

            //16,24
            _serverBtn2 = new ExineButton
            { 
                Index = 18,
                HoverIndex = 17,
                PressedIndex = 16,

                Library = Libraries.ServerButton,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 58, (768 - 600) / 2 + 267),
                Visible = true,
            };
            _serverBtn2.Click += (o, e) =>
            {

            };

            //32, 40
            _serverBtn3 = new ExineButton
            {
                //10~13
                //618 ,319
                Index = 34,
                HoverIndex = 33,
                PressedIndex = 32,

                Library = Libraries.ServerButton,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 58, (768 - 600) / 2 + 343),
                Visible = true,
            };
            _serverBtn3.Click += (o, e) =>
            {

            };

            //48,56
            _serverBtn4 = new ExineButton
            {
                //10~13
                //618 ,319
                Index = 50,
                HoverIndex = 49,
                PressedIndex = 48,

                Library = Libraries.ServerButton,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 58, (768 - 600) / 2 + 414),
                Visible = true,
            };
            _serverBtn4.Click += (o, e) =>
            {

            };


            _newBtn = new ExineButton
            {
                //10~13
                //618 ,319
                Index = 10,
                HoverIndex = 11,
                PressedIndex = 12,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 618, (768 - 600) / 2 + 319),
                Visible = true,
            };
            _newBtn.Click += (o, e) =>
            {
                ExineScene.ActiveScene = new ExineNewCharScene();
                Dispose();
            };

            _lastLogin = new ExineImageControl
            {
                Index = 1,
                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 523, (768 - 600) / 2 + 93),
                Visible = true,
            };
            //552,123
            //30,30
            _lastLoginInfo = new ExineLabel
            {
                Text = "2002년 12월 11일 오픈\r\n\r\nPK가 허용되지 않은 서버\r\n\r\n마지막 접속 아이디 : 필링이",
                AutoSize = true,
                NotControl = true,
                ForeColour = Color.WhiteSmoke,
                Parent = _lastLogin,
                Location = new Point(30,30),
                Visible =true,
            };


            _okBtn = new ExineButton
            {
                //6~9
                Index = 6,
                HoverIndex = 7,
                PressedIndex = 8,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 663-1, (768 - 600) / 2 + 364-2),
                Visible = true,
            };
            _okBtn.Click += (o, e) =>
            {
                //server select
                if (_lastLogin.Visible)
                {
                    _lastLogin.Hide();
                    _loginDialog.Show();

                    // SoundManager.StopSound(SoundList.Gargoyle_Breath);
                    SoundManager.PlaySound(SoundList.ExineGargoyle_Halwling, true);
                    
                    if (_pkBtn.Index == 24) //pk pressed
                    {
                        _gargoyle.OffSet = 0;
                        _gargoyle.AnimationCount = 55;
                        _gargoyle.Library = Libraries.BIK_011_Gargoyle_2;
                        _gargoyle.AfterAnimation += (o, e) =>
                        {
                            _gargoyle.OffSet = 0;
                            _gargoyle.Library = Libraries.BIK_012_Gargoyle_3;
                            _gargoyle.AnimationCount = 48;
                            //SoundManager.StopSound(SoundList.ExineGargoyle_Halwling);
                            SoundManager.PlaySound(SoundList.ExineGargoyle_Breath, false);
                        };
                    }

                    else
                    {
                        _gargoyle.OffSet = 0;
                        _gargoyle.AnimationCount = 55;
                        _gargoyle.Library = Libraries.BIK_014_Gargoyle_5;
                        _gargoyle.AfterAnimation += (o, e) =>
                        {
                            _gargoyle.OffSet = 0;
                            _gargoyle.Library = Libraries.BIK_015_Gargoyle_6;
                            _gargoyle.AnimationCount = 48;
                            //SoundManager.StopSound(SoundList.ExineGargoyle_Halwling);
                            SoundManager.PlaySound(SoundList.ExineGargoyle_Breath, false);
                        };
                    }
                }

                //login
                else
                {
                    //MessageBox.Show("Login!");
                    Console.WriteLine("Login! id:" + _loginIdTextBox.Text + " pw:"+ _loginPwTextBox.Text);
                    Login();
                }


            };
            _okBtn.Hide();
            _newBtn.Hide();

            _cancelBtn = new ExineButton
            {
                //14~17
                //527,409
                Index = 14,
                HoverIndex = 15,
                PressedIndex = 16,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 527, (768 - 600) / 2 + 409),
                Visible = false,
            }; 
            _quitBtn = new ExineButton
            {
                //18~21
                //527,409
                Index = 18,
                HoverIndex = 19,
                PressedIndex = 20,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 527, (768 - 600) / 2 + 409),
                Visible = true,
            };
            _quitBtn.Click += (o, e) =>
            {
                Application.Exit();
            };

            
            _pkBtn = new ExineButton
            {
                //22~24
                //9,246
                //Index = 22, 
                Index = 22,
                HoverIndex = 23,
                PressedIndex = 24,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 9, (768 - 600) / 2 + 246),
                Visible = true,
            };
            //1 => npk -> pk
            //2 => pk sel
            //3 => pk wait
            //4 => pk -> npk
            //5 => npk sel
            //6 => npk wait 
            _pkBtn.Click += (o, e) =>
            {
                //_gargoyle.OffSet = 0;
                _gargoyle.AfterAnimation += (o, e) =>
                {
                    _gargoyle.Library = Libraries.BIK_010_Gargoyle_1;
                    _gargoyle.AfterAnimation += (o, e) =>
                    {
                        _gargoyle.OffSet = 0;
                        _gargoyle.Library = Libraries.BIK_012_Gargoyle_3;
                    };
                };


                _orb2.AfterAnimation += (o, e) =>
                {
                    _orb2.Library = Libraries.BIK_006_Orb2_1;
                    _orb2.AfterAnimation += (o, e) =>
                    {
                        _orb2.OffSet = 0;
                        _orb2.Library = Libraries.BIK_007_Orb2_2;
                    };
                };

                _orb1.AfterAnimation += (o, e) =>
                {
                    _orb1.Library = Libraries.BIK_002_Orb1_1;
                    _orb1.AfterAnimation += (o, e) =>
                    {
                        _orb1.OffSet = 0;
                        _orb1.Library = Libraries.BIK_003_Orb1_2;
                    };
                };


                _pkBtn.Index = 24;
                _pkBtn.HoverIndex = 24;

                _npkBtn.Index = 30;
                _npkBtn.HoverIndex = 31;

                //1 => npk -> pk
                //2 => pk sel
                //3 => pk wait
                //4 => pk -> npk
                //5 => npk sel
                //6 => npk wait 

               
                _serverBtn1.Index = 0 + 8;
                _serverBtn1.HoverIndex = 1 + 8;
                _serverBtn1.PressedIndex = 1 + 8;

                _serverBtn2.Index = 18 + 8;
                _serverBtn2.HoverIndex = 17 + 8;
                _serverBtn2.PressedIndex = 16 + 8;

                _serverBtn3.Index = 34 + 8;
                _serverBtn3.HoverIndex = 33 + 8;
                _serverBtn3.PressedIndex = 32 + 8;

                _serverBtn4.Index = 50 + 8;
                _serverBtn4.HoverIndex = 49 + 8;
                _serverBtn4.PressedIndex = 48 + 8;

            };

            _npkBtn = new ExineButton
            {
                //30~32
                //9,349
                //Index = 30,
                Index = 32,
                HoverIndex = 31,
                PressedIndex = 32,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 9, (768 - 600) / 2 + 349),
                Visible = true,
            };
            
            _npkBtn.Click += (o, e) =>
            {
                //_gargoyle.OffSet = 0;
                //_gargoyle.Library = Libraries.BIK_013_Gargoyle_4;
                _gargoyle.AfterAnimation += (o, e) =>
                {
                    _gargoyle.Library = Libraries.BIK_013_Gargoyle_4;
                    _gargoyle.AfterAnimation += (o, e) =>
                    {
                        _gargoyle.OffSet = 0;
                        _gargoyle.Library = Libraries.BIK_015_Gargoyle_6;
                    };
                };

                _orb2.AfterAnimation += (o, e) =>
                {
                    _orb2.Library = Libraries.BIK_008_Orb2_3;
                    _orb2.AfterAnimation += (o, e) =>
                    {
                        _orb2.OffSet = 0;
                        _orb2.Library = Libraries.BIK_009_Orb2_4;
                    };
                };

                _orb1.AfterAnimation += (o, e) =>
                {
                    _orb1.Library = Libraries.BIK_004_Orb1_3;
                    _orb1.AfterAnimation += (o, e) =>
                    {
                        _orb1.OffSet = 0;
                        _orb1.Library = Libraries.BIK_005_Orb1_4;
                    };
                };

                _npkBtn.Index = 32;
                _npkBtn.HoverIndex = 32;

                _pkBtn.Index = 22;
                _pkBtn.HoverIndex = 23;

                //server list change
                _serverBtn1.Index = 0;
                _serverBtn1.HoverIndex = 1;
                _serverBtn1.PressedIndex = 1;

                _serverBtn2.Index = 18;
                _serverBtn2.HoverIndex = 17;
                _serverBtn2.PressedIndex = 16;

                _serverBtn3.Index = 34;
                _serverBtn3.HoverIndex = 33;
                _serverBtn3.PressedIndex = 32;

                _serverBtn4.Index = 50;
                _serverBtn4.HoverIndex = 49;
                _serverBtn4.PressedIndex = 48;
 
            };

            _upBtn = new ExineButton
            {
                //38~41
                //18,109
                Index = 38,
                HoverIndex = 39,
                PressedIndex = 40,
                
                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 27, (768 - 600) / 2 + 109),
                Visible = true,
            };
            _downBtn = new ExineButton
            {
                //42~45
                //18,507
                Index = 42,
                HoverIndex = 43,
                PressedIndex = 44,

                Library = Libraries.PANEL0000,
                Parent = this,
                Location = new Point((1024 - 800) / 2 + 19, (768 - 600) / 2 + 507),
                Visible = true,
            };

            _background.AfterAnimation += (o, e) =>
            {
               // MirScene.ActiveScene = new ExineNewCharScene();
            }; 
             
        }

        
        private void Login()
        { 
             Network.SendPacketToServer(new ClientPacket.Login { AccountID = _loginIdTextBox.Text, Password = _loginPwTextBox.Text });
        }


        static string ServerPackIdsToString(ServerPacketIds serverPacketIds)
        {
            return serverPacketIds.ToString();
        }


        public override void Process()
        {
            
        }
        public override void ProcessRecvPacket(Packet p)
        {
            Console.WriteLine("recvPacket.Type :" + ServerPackIdsToString((ServerPacketIds)p.Index));
            switch (p.Index)
            {
                case (short)ServerPacketIds.Connected:
                    Network.Connected = true;
                    SendVersion();
                    break;
                case (short)ServerPacketIds.ClientVersion:
                    OnRecvClientVersionHandler((ServerPacket.ClientVersion)p);
                    break;
                case (short)ServerPacketIds.NewAccount:
                    OnRecvNewAccountResultHandler((ServerPacket.NewAccount)p);
                    break;
                case (short)ServerPacketIds.ChangePassword:
                    OnRecvChangePasswordResultHandler((ServerPacket.ChangePassword)p);
                    break;
                case (short)ServerPacketIds.ChangePasswordBanned:
                    OnRecvChangePasswordResultHandler((ServerPacket.ChangePasswordBanned)p);
                    break;
                case (short)ServerPacketIds.Login:
                    OnRecvLoginFailHandler((ServerPacket.Login)p);
                    break;
                case (short)ServerPacketIds.LoginBanned:
                    OnRecvLoginFailHandler((ServerPacket.LoginBanned)p);
                    break;
                case (short)ServerPacketIds.LoginSuccess:
                    OnRecvLoginSuccessHandler((ServerPacket.LoginSuccess)p);
                    break;
                case (short)ServerPacketIds.StartGame:
                    OnRecvStartGameHandler((ServerPacket.StartGame)p);
                    break;


                default:
                    base.ProcessRecvPacket(p);
                    break;
            }
        }

        public static List<SelectInfo> charList = new List<SelectInfo>();
        private void SendVersion()
        {
            //_connectBox.Label.Text = "Sending Client Version.";

            ClientPacket.ClientVersion p = new ClientPacket.ClientVersion();
            try
            {
                byte[] sum;
                using (MD5 md5 = MD5.Create())
                using (FileStream stream = File.OpenRead(Application.ExecutablePath))
                    sum = md5.ComputeHash(stream);

                p.VersionHash = sum;
                Network.SendPacketToServer(p);
            }
            catch (Exception ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }
        }
        private void OnRecvClientVersionHandler(ServerPacket.ClientVersion p)
        {
            switch (p.Result)
            {
                case 0:
                    ExineMessageBox.Show("버전이 잘못되었습니다.Exine을 업데이트하세요.\nExine이 종료됩니다.", true); 
                    Network.Disconnect();
                    break;
                case 1:
                    //_connectBox.Dispose();
                    //_loginDialog.Show();
                    //_login.Show();
                    _okBtn.Show();
                    _newBtn.Show();
                    break;
            }
        }
        
        
        
        private void OnRecvNewAccountResultHandler(ServerPacket.NewAccount p)
        { 
            switch (p.Result)
            {
                case 0:
                    ExineMessageBox.Show("Account creation is currently disabled."); 
                    break;
                case 1:
                    ExineMessageBox.Show("Your AccountID is not acceptable."); 
                    break;
                case 2:
                    ExineMessageBox.Show("Your Password is not acceptable."); 
                    break;
                case 3:
                    ExineMessageBox.Show("Your E-Mail Address is not acceptable."); 
                    break;
                case 4:
                    ExineMessageBox.Show("Your User Name is not acceptable."); 
                    break;
                case 5:
                    ExineMessageBox.Show("Your Secret Question is not acceptable."); 
                    break;
                case 6:
                    ExineMessageBox.Show("Your Secret Answer is not acceptable."); 
                    break;
                case 7:
                    ExineMessageBox.Show("An Account with this ID already exists."); 
                    break;
                case 8:
                    ExineMessageBox.Show("Your account was created successfully."); 
                    break;
            }

        }
        private void OnRecvChangePasswordResultHandler(ServerPacket.ChangePassword p)
        {
            /*
            _password.OKButton.Enabled = true;

            switch (p.Result)
            {
                case 0:
                    MirMessageBox.Show("Password Changing is currently disabled.");
                    _password.Dispose();
                    break;
                case 1:
                    MirMessageBox.Show("Your AccountID is not acceptable.");
                    _password.AccountIDTextBox.SetFocus();
                    break;
                case 2:
                    MirMessageBox.Show("The current Password is not acceptable.");
                    _password.CurrentPasswordTextBox.SetFocus();
                    break;
                case 3:
                    MirMessageBox.Show("Your new Password is not acceptable.");
                    _password.NewPassword1TextBox.SetFocus();
                    break;
                case 4:
                    MirMessageBox.Show(GameLanguage.NoAccountID);
                    _password.AccountIDTextBox.SetFocus();
                    break;
                case 5:
                    MirMessageBox.Show(GameLanguage.IncorrectPasswordAccountID);
                    _password.CurrentPasswordTextBox.SetFocus();
                    _password.CurrentPasswordTextBox.Text = string.Empty;
                    break;
                case 6:
                    MirMessageBox.Show("Your password was changed successfully.");
                    _password.Dispose();
                    break;
            
            }*/
        }
        private void OnRecvChangePasswordResultHandler(ServerPacket.ChangePasswordBanned p)
        {/*
            _password.Dispose();

            TimeSpan d = p.ExpiryDate - CMain.Now;
            MirMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds));
        */
        }
        
        private void OnRecvLoginFailHandler(ServerPacket.Login p)
        {
           // _login.OKButton.Enabled = true;
            switch (p.Result)
            {
                case 0:
                    ExineMessageBox.Show("Logging in is currently disabled.");
                    break;
                case 1:
                    ExineMessageBox.Show("Your AccountID is not acceptable.");
                    break;
                case 2:
                    ExineMessageBox.Show("Your Password is not acceptable.");
                    break;
                case 3:
                    ExineMessageBox.Show(GameLanguage.NoAccountID);
                    break;
                case 4:
                    ExineMessageBox.Show(GameLanguage.IncorrectPasswordAccountID);
                    break;
                case 5:
                    ExineMessageBox.Show("The account's password must be changed before logging in.");
                    break;
            }
        }

       
        private void OnRecvLoginFailHandler(ServerPacket.LoginBanned p)
        {
            //_login.OKButton.Enabled = true;

            TimeSpan d = p.ExpiryDate - CMain.Now;
            ExineMessageBox.Show(string.Format("This account is banned.\n\nReason: {0}\nExpiryDate: {1}\nDuration: {2:#,##0} Hours, {3} Minutes, {4} Seconds", p.Reason,
                                             p.ExpiryDate, Math.Floor(d.TotalHours), d.Minutes, d.Seconds));
        }
        private void OnRecvLoginSuccessHandler(ServerPacket.LoginSuccess p)
        {
            charList.Clear();
            charList.AddRange(((ServerPacket.LoginSuccess)p).Characters);
            Console.WriteLine("Characters.Count:"+((ServerPacket.LoginSuccess)p).Characters.Count);
            for (int i = 0; i < charList.Count; i++)
            {
                Console.WriteLine("name : " + charList[i].Name + "Index : " + charList[i].Index);
            }
            if (charList.Count > 0)
            {
                Console.WriteLine("charList[charList.Count - 1].Index:" + charList[charList.Count - 1].Index);
                Network.SendPacketToServer(
                    new ClientPacket.StartGame
                    {
                        CharacterIndex = charList[charList.Count - 1].Index,
                    }
                );
            } 
        }
        public void OnRecvStartGameHandler(ServerPacket.StartGame p)
        {
            Console.WriteLine("StartGame(ServerPacket.StartGame p");
            switch (p.Result)
            {
                case 0:
                    ExineMessageBox.Show("Starting the game is currently disabled.");
                    break;
                case 1:
                    ExineMessageBox.Show("You are not logged in.");
                    break;
                case 2:
                    ExineMessageBox.Show("Your character could not be found.");
                    break;
                case 3:
                    ExineMessageBox.Show("No active map and/or start point found.");
                    break;
                case 4:
                    /*
                    if (p.Resolution < Settings.Resolution || Settings.Resolution == 0) Settings.Resolution = p.Resolution;

                    switch (Settings.Resolution)
                    {
                        default:
                        case 1024:
                            Settings.Resolution = 1024;
                            CMain.SetResolution(1024, 768);
                            break;
                        case 1280:
                            CMain.SetResolution(1280, 800);
                            break;
                        case 1366:
                            CMain.SetResolution(1366, 768);
                            break;
                        case 1920:
                            CMain.SetResolution(1920, 1080);
                            break;
                    }*/
                    ActiveScene = new ExineMainScene();
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
                //Version = null;

                //_login = null;
                //_account = null;
                //_password = null;

                //_connectBox = null;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}

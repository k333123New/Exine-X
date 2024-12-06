using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;


namespace Exine.ExineScenes.ExDialogs
{
    //modify to family dialog
    public sealed class RelationshipDialog : ExineImageControl
    {
        public ExineImageControl TitleLabel;
        public ExineButton CloseButton, AllowButton, RequestButton, DivorceButton, WhisperButton;
        public ExineLabel LoverNameLabel, LoverDateLabel, LoverOnlineLabel, LoverLengthLabel;


        public string LoverName = "";
        public DateTime Date;
        public string MapName = "";
        public short MarriedDays = 0;


        public RelationshipDialog()
        {
            /*
            Index = 583;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;
            */
            Index = 0;
            Library = Libraries.PANEL0505;
            Location = new Point(0 + 113, 0 + 85);
            //Visible = false;

            TitleLabel = new ExineImageControl
            {
                Index = 52,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            CloseButton = new ExineButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(260, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            AllowButton = new ExineButton
            {
                HoverIndex = 611,
                Index = 610,
                Location = new Point(50, 164),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 612,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.SwitchMarriage
            };
            AllowButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.ChangeMarriage());

            RequestButton = new ExineButton
            {
                HoverIndex = 601,
                Index = 600,
                Location = new Point(85, 164),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 602,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.RequestMarriage
            };
            RequestButton.Click += (o, e) =>
            {
                if (LoverName != "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("이미 결혼하셨군요.", ChatType.System);
                    return;
                }

                Network.SendPacketToServer(new ClientPacket.MarriageRequest());
            };

            DivorceButton = new ExineButton
            {
                HoverIndex = 617,
                Index = 616,
                Location = new Point(120, 164),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 618,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.RequestDivorce
            };
            DivorceButton.Click += (o, e) =>
            {
                if (LoverName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 결혼하지 않았습니다.", ChatType.System);
                    return;
                }

                Network.SendPacketToServer(new ClientPacket.DivorceRequest());
            };

            

            WhisperButton = new ExineButton
            {
                HoverIndex = 567,
                Index = 566,
                Location = new Point(190, 164),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 568,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.WhisperLover
            };
            WhisperButton.Click += (o, e) =>
            {
                if (LoverName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 결혼하지 않았습니다.", ChatType.System);
                    return;
                }

                if (MapName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("연인이 온라인 상태가 아닙니다.", ChatType.System);
                    return;
                }
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.SetFocus();
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = ":)";
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Visible = true;
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionStart = ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text.Length;
            };

            LoverNameLabel = new ExineLabel
            {
                Location = new Point(30, 40),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            LoverDateLabel = new ExineLabel
            {
                Location = new Point(30, 65),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
                
                Visible=false, //240923 mod k333123
            };

            LoverLengthLabel = new ExineLabel
            {
                Location = new Point(30, 90),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            LoverOnlineLabel = new ExineLabel
            {
                Location = new Point(30, 115),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),

                Visible = false, //240923 mod k333123
            };
        }

        public void UpdateInterface()
        {
            LoverNameLabel.Text = "Lover:  " + LoverName;

            if (MapName != "")
            {
                LoverOnlineLabel.Text = "Location:  " + MapName;
            }
            else
                LoverOnlineLabel.Text = "Location:  Offline";

            if ((LoverName == "") && (Date != default))
            {
                if (Date < new DateTime(2000))
                {
                    LoverDateLabel.Text = "Date: ";
                    LoverLengthLabel.Text = "Length: ";
                }
                else
                {
                    LoverDateLabel.Text = "Divorced Date:  " + Date.ToShortDateString();
                    LoverLengthLabel.Text = "Time Since: " + MarriedDays + " Days";
                }


                LoverOnlineLabel.Text = "Location: ";
                AllowButton.Hint = GameLanguage.SwitchMarriage;
            }
            else
            {
                LoverDateLabel.Text = "Marriage Date:  " + Date.ToShortDateString();
                LoverLengthLabel.Text = "Length: " + MarriedDays.ToString() + " Days";
                AllowButton.Hint = "Allow/Block Recall";
            }


        }
    }
}

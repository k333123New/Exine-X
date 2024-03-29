﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
using C = ClientPackets;

namespace Exine.ExineScenes.ExDialogs
{
    public sealed class RelationshipDialog : ExineImageControl
    {
        public ExineImageControl TitleLabel;
        public MirButton CloseButton, AllowButton, RequestButton, DivorceButton, MailButton, WhisperButton;
        public ExineLabel LoverNameLabel, LoverDateLabel, LoverOnlineLabel, LoverLengthLabel;


        public string LoverName = "";
        public DateTime Date;
        public string MapName = "";
        public short MarriedDays = 0;


        public RelationshipDialog()
        {
            Index = 583;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;

            TitleLabel = new ExineImageControl
            {
                Index = 52,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            CloseButton = new MirButton
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

            AllowButton = new MirButton
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
            AllowButton.Click += (o, e) => Network.Enqueue(new C.ChangeMarriage());

            RequestButton = new MirButton
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
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("You're already married.", ChatType.System);
                    return;
                }

                Network.Enqueue(new C.MarriageRequest());
            };

            DivorceButton = new MirButton
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
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("You're not married.", ChatType.System);
                    return;
                }

                Network.Enqueue(new C.DivorceRequest());
            };

            MailButton = new MirButton
            {
                HoverIndex = 438,
                Index = 437,
                Location = new Point(155, 164),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 439,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.MailLover
            };
            MailButton.Click += (o, e) =>
            {
                if (LoverName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("You're not married.", ChatType.System);
                    return;
                }

                ExineMainScene.Scene.MailComposeLetterDialog.ComposeMail(LoverName);
            };

            WhisperButton = new MirButton
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
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("You're not married.", ChatType.System);
                    return;
                }

                if (MapName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("Lover is not online", ChatType.System);
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

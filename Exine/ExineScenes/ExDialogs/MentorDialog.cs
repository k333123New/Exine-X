using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
using C = ClientPackets;

namespace Exine.ExineScenes.ExDialogs
{
    public sealed class MentorDialog : ExineImageControl
    {
        public ExineImageControl TitleLabel;
        public MirButton CloseButton, AllowButton, AddButton, RemoveButton;
        public ExineLabel MentorNameLabel, MentorLevelLabel, MentorOnlineLabel, StudentNameLabel, StudentLevelLabel, StudentOnlineLabel, MentorLabel, StudentLabel, MenteeEXPLabel;

        public string MentorName;
        public ushort MentorLevel;
        public bool MentorOnline;
        public long MenteeEXP;

        public MentorDialog()
        {
            Index = 170;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;


            TitleLabel = new ExineImageControl
            {
                Index = 51,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };



            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(219, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            AllowButton = new MirButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(30, 178),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.MentorRequests
            };
            AllowButton.Click += (o, e) =>
            {
                if (AllowButton.Index == 116)
                {
                    AllowButton.Index = 117;
                    AllowButton.HoverIndex = 118;
                    AllowButton.PressedIndex = 119;
                }
                else
                {
                    AllowButton.Index = 114;
                    AllowButton.HoverIndex = 115;
                    AllowButton.PressedIndex = 116;
                }

                Network.Enqueue(new C.AllowMentor());
            };


            AddButton = new MirButton
            {
                HoverIndex = 214,
                Index = 213,
                Location = new Point(60, 178),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 215,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.AddMentor
            };
            AddButton.Click += (o, e) =>
            {
                if (MentorLevel != 0)
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("이미 멘토가 있습니다.", ChatType.System);
                    return;
                }

                string message = GameLanguage.MentorEnterName;

                MirInputBox inputBox = new MirInputBox(message);

                inputBox.OKButton.Click += (o1, e1) =>
                {
                    Network.Enqueue(new C.AddMentor { Name = inputBox.InputTextBox.Text });
                    inputBox.Dispose();
                };

                inputBox.Show();

            };

            RemoveButton = new MirButton
            {
                HoverIndex = 217,
                Index = 216,
                Location = new Point(135, 178),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 218,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.RemoveMentorMentee
            };
            RemoveButton.Click += (o, e) =>
            {
                if (MentorName == "")
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.NoMentorship, ChatType.System);
                    return;
                }

                ExineMessageBox messageBox = new ExineMessageBox(string.Format("멘토십을 일찍 취소하면 쿨다운이 발생합니다. 정말 진행할까요?"), MirMessageBoxButtons.YesNo);

                messageBox.YesButton.Click += (oo, ee) => Network.Enqueue(new C.CancelMentor { });
                messageBox.NoButton.Click += (oo, ee) => { messageBox.Dispose(); };

                messageBox.Show();

            };

            MentorNameLabel = new ExineLabel
            {
                Location = new Point(20, 58),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            MentorLevelLabel = new ExineLabel
            {
                Location = new Point(170, 58),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            MentorOnlineLabel = new ExineLabel
            {
                Location = new Point(125, 58),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.Green,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Visible = false,
                Text = "ONLINE",
            };

            StudentNameLabel = new ExineLabel
            {
                Location = new Point(20, 112),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            StudentLevelLabel = new ExineLabel
            {
                Location = new Point(170, 111),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.LightGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 10F),
            };

            StudentOnlineLabel = new ExineLabel
            {
                Location = new Point(125, 112),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.Green,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Visible = false,
                Text = "ONLINE",
            };

            MentorLabel = new ExineLabel
            {
                Location = new Point(15, 41),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Text = "MENTOR",
            };

            StudentLabel = new ExineLabel
            {
                Location = new Point(15, 94),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
                Text = "MENTEE",
            };

            MenteeEXPLabel = new ExineLabel
            {
                Location = new Point(15, 147),
                Size = new Size(200, 30),
                BackColour = Color.Empty,
                ForeColour = Color.DimGray,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Font = new Font(Settings.FontName, 7F),
            };




        }

        public void UpdateInterface()
        {
            if (MentorLevel == 0)
            {
                MentorNameLabel.Visible = false;
                MentorLevelLabel.Visible = false;
                MentorOnlineLabel.Visible = false;
                StudentNameLabel.Visible = false;
                StudentLevelLabel.Visible = false;
                StudentOnlineLabel.Visible = false;
                MenteeEXPLabel.Visible = false;
                return;
            }

            MentorNameLabel.Visible = true;
            MentorLevelLabel.Visible = true;
            MentorOnlineLabel.Visible = true;
            StudentNameLabel.Visible = true;
            StudentLevelLabel.Visible = true;
            StudentOnlineLabel.Visible = true;

            if (ExineMainScene.User.Level > MentorLevel)
            {
                MentorNameLabel.Text = ExineMainScene.User.Name;
                MentorLevelLabel.Text = "Lv " + ExineMainScene.User.Level.ToString();
                MentorOnlineLabel.Visible = false;

                StudentNameLabel.Text = MentorName;
                StudentLevelLabel.Text = "Lv " + MentorLevel.ToString();
                if (MentorOnline)
                    StudentOnlineLabel.Visible = true;
                else
                    StudentOnlineLabel.Visible = false;

                MenteeEXPLabel.Visible = true;
                MenteeEXPLabel.Text = "MENTEE EXP: " + MenteeEXP;
            }
            else
            {
                MentorNameLabel.Text = MentorName;
                MentorLevelLabel.Text = "Lv " + MentorLevel.ToString();
                if (MentorOnline)
                    MentorOnlineLabel.Visible = true;
                else
                    MentorOnlineLabel.Visible = false;

                StudentNameLabel.Text = ExineMainScene.User.Name;
                StudentLevelLabel.Text = "Lv " + ExineMainScene.User.Level.ToString();
                StudentOnlineLabel.Visible = false;
            }
        }

    }
}

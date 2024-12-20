﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;


namespace Exine.ExineScenes.ExDialogs
{
    public sealed class FriendDialog : ExineImageControl
    {
        public ExineImageControl TitleLabel, FriendLabel, BlacklistLabel;
        public ExineLabel PageNumberLabel;
        public ExineButton CloseButton, PreviousButton, NextButton;
        public ExineButton AddButton, RemoveButton, EmailButton, WhisperButton;
        public FriendRow[] Rows = new FriendRow[12];

        public List<ClientFriend> Friends = new List<ClientFriend>();
        private ClientFriend SelectedFriend = null;
        private bool _tempBlockedTab = false;
        private bool _blockedTab = false;

        public int SelectedIndex = 0;
        public int StartIndex = 0;
        public int Page = 0;

        public FriendDialog()
        {
            Index = 199;
            Library = Libraries.Title;
            Movable = true;
            Sort = true;
            Location = Center;

            TitleLabel = new ExineImageControl
            {
                Index = 6,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            FriendLabel = new ExineImageControl
            {
                Index = 163,
                Library = Libraries.Title,
                Location = new Point(10, 34),
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            FriendLabel.Click += (o, e) =>
            {
                _tempBlockedTab = false;
                UpdateDisplay();
            };

            BlacklistLabel = new ExineImageControl
            {
                Index = 167,
                Library = Libraries.Title,
                Location = new Point(128, 34),
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            BlacklistLabel.Click += (o, e) =>
            {
                _tempBlockedTab = true;
                UpdateDisplay();
            };

            PageNumberLabel = new ExineLabel
            {
                Text = "",
                Parent = this,
                Size = new Size(83, 17),
                Location = new Point(87, 216),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            };

            #region Buttons

            PreviousButton = new ExineButton
            {
                Index = 240,
                HoverIndex = 241,
                PressedIndex = 242,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 16),
                Location = new Point(70, 218),
                Sound = SoundList.ButtonA,
            };
            PreviousButton.Click += (o, e) =>
            {
                Page--;
                if (Page < 0) Page = 0;
                StartIndex = Rows.Length * Page;
                Update();
            };

            NextButton = new ExineButton
            {
                Index = 243,
                HoverIndex = 244,
                PressedIndex = 245,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 16),
                Location = new Point(171, 218),
                Sound = SoundList.ButtonA,
            };
            NextButton.Click += (o, e) =>
            {
                Page++;
                if (Page > Friends.Count() / Rows.Length) Page = Friends.Count() / Rows.Length;
                StartIndex = Rows.Length * Page;

                Update();
            };

            CloseButton = new ExineButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(237, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            AddButton = new ExineButton
            {
                Index = 554,
                HoverIndex = 555,
                PressedIndex = 556,
                Library = Libraries.Prguse,
                Location = new Point(60, 241),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.AddFriend
            };
            AddButton.Click += (o, e) =>
            {
                string message = GameLanguage.FriendEnterAddName;
                if (_blockedTab)
                {
                    message = GameLanguage.FriendEnterBlockName;
                }

                ExineInputBox inputBox = new ExineInputBox(message);

                inputBox.OKButton.Click += (o1, e1) =>
                {
                    Network.SendPacketToServer(new ClientPacket.AddFriend { Name = inputBox.InputTextBox.Text, Blocked = _blockedTab });
                    inputBox.Dispose();
                };

                inputBox.Show();
            };

            RemoveButton = new ExineButton
            {
                Index = 557,
                HoverIndex = 558,
                PressedIndex = 559,
                Library = Libraries.Prguse,
                Location = new Point(88, 241),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.RemoveFriend,
            };
            RemoveButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                ExineMessageBox messageBox = new ExineMessageBox(string.Format("'{0}'을 제거하시겠습니까?", SelectedFriend.Name), MirMessageBoxButtons.YesNo);

                messageBox.YesButton.Click += (o1, e1) =>
                {
                    Network.SendPacketToServer(new ClientPacket.RemoveFriend { CharacterIndex = SelectedFriend.Index });
                    messageBox.Dispose();
                };

                messageBox.Show();
            };
 

            WhisperButton = new ExineButton
            {
                Index = 566,
                HoverIndex = 567,
                PressedIndex = 568,
                Library = Libraries.Prguse,
                Location = new Point(172, 241),
                Parent = this,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.FriendWhisper
            };
            WhisperButton.Click += (o, e) =>
            {
                if (SelectedFriend == null) return;

                if (!SelectedFriend.Online)
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("플레이어가 온라인이 아닙니다", ChatType.System);
                    return;
                }

                ExineMainScene.Scene.ExChatDialog.ChatTextBox.SetFocus();
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text = "/" + SelectedFriend.Name + " ";
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.Visible = true;
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                ExineMainScene.Scene.ExChatDialog.ChatTextBox.TextBox.SelectionStart = ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text.Length;
            };
            #endregion
        }

        private void UpdateDisplay()
        {
            if (!Visible) return;

            if (_blockedTab != _tempBlockedTab)
            {
                _blockedTab = _tempBlockedTab;

                if (_blockedTab)
                {
                    FriendLabel.Index = 164;
                    BlacklistLabel.Index = 166;
                }
                else
                {
                    FriendLabel.Index = 163;
                    BlacklistLabel.Index = 167;
                }
                Update(); 
            }
        }

        public void Update(bool clearSelection = true)
        {
            if (clearSelection)
                SelectedFriend = null;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] != null) Rows[i].Dispose();

                Rows[i] = null;
            }

            List<ClientFriend> filteredFriends = new List<ClientFriend>();

            if (_blockedTab)
                filteredFriends = Friends.Where(e => e.Blocked).ToList();
            else
                filteredFriends = Friends.Where(e => !e.Blocked).ToList();

            int maxPage = filteredFriends.Count / Rows.Length + 1;
            if (maxPage < 1) maxPage = 1;

            PageNumberLabel.Text = (Page + 1) + " / " + maxPage;

            int maxIndex = filteredFriends.Count - 1;

            if (StartIndex > maxIndex) StartIndex = maxIndex;
            if (StartIndex < 0) StartIndex = 0;

            for (int i = 0; i < Rows.Length; i++)
            {
                if (i + StartIndex >= filteredFriends.Count) break;

                if (Rows[i] != null)
                    Rows[i].Dispose();

                Rows[i] = new FriendRow
                {
                    Friend = filteredFriends[i + StartIndex],
                    Location = new Point((i % 2) * 115 + 16, 55 + ((i) / 2) * 22),
                    Parent = this,
                };
                Rows[i].Click += (o, e) =>
                {
                    FriendRow row = (FriendRow)o;

                    if (row.Friend != SelectedFriend)
                    {
                        SelectedFriend = row.Friend;
                        SelectedIndex = FindSelectedIndex();
                        UpdateRows();
                    }
                };

                if (SelectedFriend != null)
                {
                    if (SelectedIndex == i)
                    {
                        SelectedFriend = Rows[i].Friend;
                    }
                }
            }
        }

        public void UpdateRows()
        {
            if (SelectedFriend == null)
            {
                if (Rows[0] == null) return;

                SelectedFriend = Rows[0].Friend;
            }

            for (int i = 0; i < Rows.Length; i++)
            {
                if (Rows[i] == null) continue;

                Rows[i].Selected = false;

                if (Rows[i].Friend == SelectedFriend)
                {
                    Rows[i].Selected = true;
                }

                Rows[i].UpdateInterface();
            }
        }

        public int FindSelectedIndex()
        {
            int selectedIndex = 0;
            if (SelectedFriend != null)
            {
                for (int i = 0; i < Rows.Length; i++)
                {
                    if (Rows[i] == null || SelectedFriend != Rows[i].Friend) continue;

                    selectedIndex = i;
                }
            }

            return selectedIndex;
        }

        public override void Hide()
        {
            if (!Visible) return;
            Visible = false;
             
        }
        public override void Show()
        {
            if (Visible) return;
            Visible = true;
            UpdateDisplay();
            Network.SendPacketToServer(new ClientPacket.RefreshFriends());
        }
    }
    public sealed class FriendRow : ExineControl
    {
        public ClientFriend Friend;
        public ExineLabel NameLabel, OnlineLabel;

        public bool Selected = false;

        public FriendRow()
        {
            Sound = SoundList.ButtonA;
            Size = new Size(115, 17);

            BeforeDraw += FriendRow_BeforeDraw;

            NameLabel = new ExineLabel
            {
                Location = new Point(0, 0),
                Size = new Size(115, 17),
                BackColour = Color.Empty,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
            };

            UpdateInterface();
        }

        void FriendRow_BeforeDraw(object sender, EventArgs e)
        {
            UpdateInterface();
        }

        public void UpdateInterface()
        {
            if (Friend == null) return;

            NameLabel.Text = Friend.Name;

            if (Friend.Online)
            {
                NameLabel.ForeColour = Color.Green;
            }
            else
            {
                NameLabel.ForeColour = Color.White;
            }

            if (Selected)
            {
                NameLabel.BackColour = Color.Gray;
            }
            else
            {
                NameLabel.BackColour = Color.Empty;
            }
        }


        protected override void OnMouseEnter()
        {
            if (Friend == null || Friend.Memo.Length < 1) return;

            base.OnMouseEnter(); 
        }
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave(); 
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            Friend = null;
            NameLabel = null;

            Selected = false;
        }
    }
 
}

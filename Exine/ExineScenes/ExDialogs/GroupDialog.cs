﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineSounds;


namespace Exine.ExineScenes.ExDialogs
{
    public sealed class GroupDialog : ExineImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();
        public static Dictionary<string, string> GroupMembersMap = new Dictionary<string, string>();

        public ExineImageControl TitleLabel;
        public ExineButton SwitchButton, CloseButton, AddButton, DelButton;
        public ExineLabel[] GroupMembers;

        public GroupDialog()
        {
            Index = 120;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = Center;

            GroupMembers = new ExineLabel[Globals.MaxGroup];

            GroupMembers[0] = new ExineLabel
            {
                AutoSize = true,
                Location = new Point(16, 33),
                Parent = this,
                NotControl = false,
            };

            for (int i = 1; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new ExineLabel
                {
                    AutoSize = true,
                    Location = new Point(((i + 1) % 2) * 100 + 16, 55 + ((i - 1) / 2) * 20),
                    Parent = this,
                    NotControl = false,
                };
            }

            TitleLabel = new ExineImageControl
            {
                Index = 5,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            CloseButton = new ExineButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(206, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            SwitchButton = new ExineButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(25, 219),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.GroupSwitch
            };
            SwitchButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.SwitchGroup { AllowGroup = !AllowGroup });

            AddButton = new ExineButton
            {
                HoverIndex = 134,
                Index = 133,
                Location = new Point(70, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 135,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.GroupAdd
            };
            AddButton.Click += (o, e) => AddMember();

            DelButton = new ExineButton
            {
                HoverIndex = 137,
                Index = 136,
                Location = new Point(140, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 138,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.GroupRemove
            };
            DelButton.Click += (o, e) => DelMember();

            BeforeDraw += GroupPanel_BeforeDraw;

            GroupList.Clear();
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (GroupList.Count == 0)
            {
                AddButton.Index = 130;
                AddButton.HoverIndex = 131;
                AddButton.PressedIndex = 132;
            }
            else
            {
                AddButton.Index = 133;
                AddButton.HoverIndex = 134;
                AddButton.PressedIndex = 135;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                AddButton.Visible = false;
                DelButton.Visible = false;
            }
            else
            {
                AddButton.Visible = true;
                DelButton.Visible = true;
            }

            if (AllowGroup)
            {
                SwitchButton.Index = 117;
                SwitchButton.HoverIndex = 118;
                SwitchButton.PressedIndex = 119;
            }
            else
            {
                SwitchButton.Index = 114;
                SwitchButton.HoverIndex = 115;
                SwitchButton.PressedIndex = 116;
            }

            for (int i = 0; i < GroupMembers.Length; i++)
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupList[i];

            foreach (var player in GroupMembersMap)
            {
                for (int i = 0; i < GroupMembers.Length; i++)
                {
                    string playersName = GroupMembers[i].Text;

                    if (player.Key == playersName)
                        GroupMembers[i].Hint = player.Value;
                }
            }


        }

        public void AddMember(string name)
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("그룹멤버가 꽉찼습니다.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 당신의 그룹의 리더가 아닙니다.", ChatType.System);
                return;
            }

            Network.SendPacketToServer(new ClientPacket.AddMember { Name = name });
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("그룹멤버가 꽉찼습니다.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 당신의 그룹의 리더가 아닙니다.", ChatType.System);
                return;
            }

            ExineInputBox inputBox = new ExineInputBox(GameLanguage.GroupAddEnterName);

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                ExineMainScene.Scene.ExChatDialog.ReceiveChat("당신은 당신의 그룹의 리더가 아닙니다.", ChatType.System);
                return;
            }

            ExineInputBox inputBox = new ExineInputBox(GameLanguage.GroupRemoveEnterName);

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
    }
}

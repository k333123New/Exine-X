﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineSounds;


namespace Exine.ExineScenes.ExDialogs
{
    public sealed class ExineInventoryDialog : ExineImageControl
    {
        public ExineImageControl WeightBar;
        public ExineImageControl[] LockBar = new ExineImageControl[10];
        public MirItemCell[] Grid;
        public MirItemCell[] QuestGrid;

        public ExineButton CloseButton, ItemButton, ItemButton2, QuestButton, AddButton;
        public ExineLabel GoldLabel, WeightLabel;

        public ExineInventoryDialog()
        {
            Index = 0;
            Library = Libraries.ITEMSLOT;
            //Movable = true;
            Movable = false;
            //Sort = true;
            Sort = true;
            Visible = false;
            //Location = new Point(Settings.ScreenWidth / 2, 0 + 85+300);

            WeightBar = new ExineImageControl
            {
                Index = 24,
                Library = Libraries.Prguse,
                Location = new Point(182, 217),
                Parent = this,
                DrawImage = false,
                NotControl = true,
            };
           
          ItemButton = new ExineButton
          {
              Index = 197,
              Library = Libraries.Title,
              Location = new Point(6, 7),
              Parent = this,
              Size = new Size(72, 23),
              Sound = SoundList.ButtonA,
              Visible=false,
          };
          ItemButton.Click += Button_Click;
            
         ItemButton2 = new ExineButton
         {
             Index = 738,
             Library = Libraries.Title,
             Location = new Point(76, 7),
             Parent = this,
             Size = new Size(72, 23),
             Sound = SoundList.ButtonA,
             Visible=false,
         };
         ItemButton2.Click += Button_Click;

         QuestButton = new ExineButton
         {
             Index = 739,
             Library = Libraries.Title,
             Location = new Point(146, 7),
             Parent = this,
             Size = new Size(72, 23),
             Sound = SoundList.ButtonA,
             Visible=false,
         };
         QuestButton.Click += Button_Click;

         AddButton = new ExineButton
         {
             Index = 483,
             HoverIndex = 484,
             PressedIndex = 485,
             Library = Libraries.Title,
             Location = new Point(235, 5),
             Parent = this,
             Size = new Size(72, 23),
             Sound = SoundList.ButtonA,
             Visible = false,
         };
         AddButton.Click += (o1, e) =>
         {
             int openLevel = (ExineMainScene.User.Inventory.Length - 46) / 4;
             int openGold = (1000000 + openLevel * 1000000);
             ExineMessageBox messageBox = new ExineMessageBox(string.Format(GameLanguage.ExtraSlots4, openGold), MirMessageBoxButtons.OKCancel);

             messageBox.OKButton.Click += (o, a) =>
             {
                 Network.SendPacketToServer(new ClientPacket.Chat { Message = "@ADDINVENTORY" });
             };
             messageBox.Show();
         };
            /*
           CloseButton = new MirButton
           {
               HoverIndex = 361,
               Index = 360,
               Location = new Point(289, 3),
               Library = Libraries.Prguse2,
               Parent = this,
               PressedIndex = 362,
               Sound = SoundList.ButtonA,
           };
           CloseButton.Click += (o, e) => Hide();
           
            GoldLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(40+283, 212+241),
                Size = new Size(111, 14),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (ExineMainScene.SelectedCell == null)
                    ExineMainScene.PickedUpGold = !ExineMainScene.PickedUpGold && ExineMainScene.Gold > 0;
            };
             */

            Grid = new MirItemCell[8 * 10];

            //for (int x = 0; x < 8; x++)
            for (int x = 0; x < 10; x++)
            {
                //for (int y = 0; y < 10; y++)
                for (int y = 0; y < 8; y++)
                {
                    //int idx = 8 * y + x;
                    int idx = 10 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = 6 + idx,
                        GridType = MirGridType.Inventory,
                        Library = Libraries.Items,
                        Parent = this,
                        //Location = new Point(x * 33 + 9 + x -8, y % 5 * 33 + 37 + y % 5 -36),
                        Location = new Point(x * 33 + 1 + x, y % 5 * 33 + 1 + y % 5),
                    };

                    if (idx >= 40)
                        Grid[idx].Visible = false;
                }
            }

            QuestGrid = new MirItemCell[8 * 5];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    QuestGrid[8 * y + x] = new MirItemCell
                    {
                        ItemSlot = 8 * y + x,
                        GridType = MirGridType.QuestInventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 9 + x, y * 32 + 37 + y),
                        Visible = false
                    };
                }
            }

            WeightLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(268, 212),
                Size = new Size(26, 14)
            };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;

            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i] = new ExineImageControl
                {
                    Index = 307,
                    Library = Libraries.Prguse2,
                    Location = new Point(9 + i % 2 * 148, 37 + i / 2 * 33),
                    Parent = this,
                    DrawImage = true,
                    NotControl = true,
                    Visible = false,
                };
            }

        }

        void Button_Click(object sender, EventArgs e)
        {
            if (ExineMainScene.User.Inventory.Length == 46 && sender == ItemButton2)
            {
                ExineMessageBox messageBox = new ExineMessageBox(GameLanguage.ExtraSlots8, MirMessageBoxButtons.OKCancel);

                messageBox.OKButton.Click += (o, a) =>
                {
                    Network.SendPacketToServer(new ClientPacket.Chat { Message = "@ADDINVENTORY" });
                };
                messageBox.Show();
            }
            else
            {
                if (sender == ItemButton)
                {
                    RefreshInventory();
                }
                else if (sender == ItemButton2)
                {
                    RefreshInventory2();
                }
                else if (sender == QuestButton)
                {
                    Reset();

                    ItemButton.Index = 737;
                    ItemButton2.Index = 738;
                    QuestButton.Index = 198;

                    if (ExineMainScene.User.Inventory.Length == 46)
                    {
                        ItemButton2.Index = 169;
                    }

                    foreach (var grid in QuestGrid)
                    {
                        grid.Visible = true;
                    }
                }
            }
        }

        void Reset()
        {
            foreach (MirItemCell grid in QuestGrid)
            {
                grid.Visible = false;
            }

            foreach (MirItemCell grid in Grid)
            {
                grid.Visible = false;
            }

            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i].Visible = false;
            }

            AddButton.Visible = false;
        }



        public void RefreshInventory()
        {
            Reset();

            ItemButton.Index = 197;
            ItemButton2.Index = 738;
            QuestButton.Index = 739;

            if (ExineMainScene.User.Inventory.Length == 46)
            {
                ItemButton2.Index = 169;
            }

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 46)
                    grid.Visible = true;
                else
                    grid.Visible = false;
            }
        }

        public void RefreshInventory2()
        {
            Reset();

            ItemButton.Index = 737;
            ItemButton2.Index = 168;
            QuestButton.Index = 739;

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 46 || grid.ItemSlot >= ExineMainScene.User.Inventory.Length)
                    grid.Visible = false;
                else
                    grid.Visible = true;
            }

            int openLevel = (ExineMainScene.User.Inventory.Length - 46) / 4;
            for (int i = 0; i < LockBar.Length; i++)
            {
                LockBar[i].Visible = (i < openLevel) ? false : true;
            }

            AddButton.Visible = openLevel >= 10 ? false : true;
        }

        public void Process()
        {
            WeightLabel.Text = ExineMainScene.User.Inventory.Count(t => t == null).ToString();
            //WeightLabel.Text = (MapObject.User.MaxBagWeight - MapObject.User.CurrentBagWeight).ToString();
            //GoldLabel.Text = ExineMainScene.Gold.ToString("###,###,##0");
        }


        private void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library == null) return;

            double percent = MapObject.User.CurrentBagWeight / (double)MapObject.User.Stats[Stat.BagWeight];
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
            {
                Size = new Size((int)((WeightBar.Size.Width - 3) * percent), WeightBar.Size.Height)
            };

            WeightBar.Library.Draw(WeightBar.Index, section, WeightBar.DisplayLocation, Color.White, false);
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

        public MirItemCell GetQuestCell(ulong id)
        {
            return QuestGrid.FirstOrDefault(t => t.Item != null && t.Item.UniqueID == id);
        }

        public void DisplayItemGridEffect(ulong id, int type = 0)
        {
            MirItemCell cell = GetCell(id);

            if (cell.Item == null) return;

            ExineAnimatedControl animEffect = null;

            switch (type)
            {
                case 0:
                    animEffect = new ExineAnimatedControl
                    {
                        Animated = true,
                        AnimationCount = 9,
                        AnimationDelay = 150,
                        Index = 410,
                        Library = Libraries.Prguse,
                        Location = cell.Location,
                        Parent = this,
                        Loop = false,
                        NotControl = true,
                        UseOffSet = true,
                        Blending = true,
                        BlendingRate = 1F
                    };
                    animEffect.AfterAnimation += (o, e) => animEffect.Dispose();
                    SoundManager.PlaySound(20000 + (ushort)Spell.MagicShield * 10);
                    break;
            }
        }
    }
    public sealed class BeltDialog : ExineImageControl
    {
        public ExineLabel[] Key = new ExineLabel[6];
        public ExineButton CloseButton, RotateButton;
        public MirItemCell[] Grid;

        public BeltDialog()
        {
            Index = 1932;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Visible = true;
            Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 150);

            BeforeDraw += BeltPanel_BeforeDraw;

            for (int i = 0; i < Key.Length; i++)
            {
                Key[i] = new ExineLabel
                {
                    Parent = this,
                    Size = new Size(26, 14),
                    Location = new Point(8 + i * 35, 2),
                    Text = (i + 1).ToString()
                };
            }

            RotateButton = new ExineButton
            {
                HoverIndex = 1927,
                Index = 1926,
                Location = new Point(222, 3),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 1928,
                Sound = SoundList.ButtonA,
                Hint = GameLanguage.Rotate
            };
            RotateButton.Click += (o, e) => Flip();

            CloseButton = new ExineButton
            {
                HoverIndex = 1924,
                Index = 1923,
                Location = new Point(222, 19),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 1925,
                Sound = SoundList.ButtonA,
                Hint = string.Format(GameLanguage.Close, CMain.InputKeys.GetKey(KeybindOptions.Belt))
            };
            CloseButton.Click += (o, e) => Hide();

            Grid = new MirItemCell[6];

            for (int x = 0; x < 6; x++)
            {
                Grid[x] = new MirItemCell
                {
                    ItemSlot = x,
                    Size = new Size(32, 32),
                    GridType = MirGridType.Inventory,
                    Library = Libraries.Items,
                    Parent = this,
                    Location = new Point(x * 35 + 12, 3),
                };
            }

        }

        private void BeltPanel_BeforeDraw(object sender, EventArgs e)
        {
            //if Transparent return

            if (Libraries.Prguse != null)
                Libraries.Prguse.Draw(Index + 1, DisplayLocation, Color.White, false, 0.5F);
        }

        public void Flip()
        {
            //0,70 LOCATION
            if (Index == 1932)
            {
                Index = 1944;
                Location = new Point(0, 200);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(3, x * 35 + 12);

                CloseButton.Index = 1935;
                CloseButton.HoverIndex = 1936;
                CloseButton.Location = new Point(3, 222);
                CloseButton.PressedIndex = 1937;

                RotateButton.Index = 1938;
                RotateButton.HoverIndex = 1939;
                RotateButton.Location = new Point(19, 222);
                RotateButton.PressedIndex = 1940;

            }
            else
            {
                Index = 1932;
                Location = new Point(ExineMainScene.Scene.ExMainDialog.Location.X + 230, Settings.ScreenHeight - 150);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(x * 35 + 12, 3);

                CloseButton.Index = 1923;
                CloseButton.HoverIndex = 1924;
                CloseButton.Location = new Point(222, 19);
                CloseButton.PressedIndex = 1925;

                RotateButton.Index = 1926;
                RotateButton.HoverIndex = 1927;
                RotateButton.Location = new Point(222, 3);
                RotateButton.PressedIndex = 1928;
            }

            for (int i = 0; i < Key.Length; i++)
            {
                Key[i].Location = (Index != 1932) ? new Point(-1, 11 + i * 35) : new Point(8 + i * 35, 2);
            }
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

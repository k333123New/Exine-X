using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineSounds;
using C = ClientPackets;

namespace Exine.ExineScenes.ExDialogs
{
    public sealed class TradeDialog : ExineImageControl
    {
        public MirItemCell[] Grid;
        public ExineLabel NameLabel, GoldLabel;
        public MirButton ConfirmButton, CloseButton;

        public TradeDialog()
        {
            Index = 389;
            Library = Libraries.Prguse;
            Movable = true;
            Size = new Size(204, 152);
            Location = new Point((Settings.ScreenWidth / 2) - Size.Width - 10, Settings.ScreenHeight - 350);
            Sort = true;

            #region Buttons
            ConfirmButton = new MirButton
            {
                Index = 520,
                HoverIndex = 521,
                Location = new Point(135, 120),
                Size = new Size(48, 25),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 522,
                Sound = SoundList.ButtonA,
            };
            ConfirmButton.Click += (o, e) => 
            {
                ChangeLockState(!ExineMainScene.User.TradeLocked);
                Network.Enqueue(new C.TradeConfirm { Locked = ExineMainScene.User.TradeLocked });
            };

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(Size.Width - 23, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) =>
            {
                Hide();
                ExineMainScene.Scene.GuestTradeDialog.Hide();
                TradeCancel();
            };

            #endregion

            #region Host labels
            NameLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(20, 10),
                Size = new Size(150, 14),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            GoldLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(35, 123),
                Parent = this,
                Size = new Size(90, 15),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (ExineMainScene.SelectedCell == null && ExineMainScene.Gold > 0)
                {
                    MirAmountBox amountBox = new MirAmountBox("Trade Amount:", 116, ExineMainScene.Gold);

                    amountBox.OKButton.Click += (c, a) =>
                    {
                        if (amountBox.Amount > 0)
                        {
                            ExineMainScene.User.TradeGoldAmount += amountBox.Amount;
                            Network.Enqueue(new C.TradeGold { Amount = amountBox.Amount });

                            RefreshInterface();
                        }
                    };

                    amountBox.Show();
                    ExineMainScene.PickedUpGold = false;
                }
            };
            #endregion

            #region Grids
            Grid = new MirItemCell[5 * 2];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Grid[2 * x + y] = new MirItemCell
                    {
                        ItemSlot = 2 * x + y,
                        GridType = MirGridType.Trade,
                        Parent = this,
                        Location = new Point(x * 36 + 10 + x, y * 32 + 39 + y),
                    };
                }
            }
            #endregion
        }

        public void ChangeLockState(bool lockState, bool cancelled = false)
        {
            ExineMainScene.User.TradeLocked = lockState;

            if (ExineMainScene.User.TradeLocked)
            {
                ConfirmButton.Index = 521;
            }
            else
            {
                ConfirmButton.Index = 520;
            }

            //if (!cancelled)
            //{
            //    //Send lock info to server
            //    Network.Enqueue(new C.TradeConfirm { Locked = lockState });
            //}
        }

        public void RefreshInterface()
        {
            NameLabel.Text = ExineMainScene.User.Name;
            GoldLabel.Text = ExineMainScene.User.TradeGoldAmount.ToString("###,###,##0");

            ExineMainScene.Scene.GuestTradeDialog.RefreshInterface();

            Redraw();
        }

        public void TradeAccept()
        {
            ExineMainScene.Scene.InventoryDialog.Location = new Point(Settings.ScreenWidth - ExineMainScene.Scene.InventoryDialog.Size.Width, 0);
            ExineMainScene.Scene.InventoryDialog.Show();

            RefreshInterface();

            Show();
            ExineMainScene.Scene.GuestTradeDialog.Show();
        }

        public void TradeReset()
        {
            ExineMainScene.Scene.GuestTradeDialog.TradeReset();

            for (int i = 0; i < ExineMainScene.User.Trade.Length; i++)
                ExineMainScene.User.Trade[i] = null;

            ExineMainScene.User.TradeGoldAmount = 0;
            ChangeLockState(false, true);

            RefreshInterface();

            Hide();
            ExineMainScene.Scene.GuestTradeDialog.Hide();
        }

        public void TradeCancel()
        {
            Network.Enqueue(new C.TradeCancel());
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
    public sealed class GuestTradeDialog : ExineImageControl
    {
        public MirItemCell[] GuestGrid;
        public static UserItem[] GuestItems = new UserItem[10];
        public string GuestName;
        public uint GuestGold;

        public ExineLabel GuestNameLabel, GuestGoldLabel;

        public MirButton ConfirmButton;

        public GuestTradeDialog()
        {
            Index = 390;
            Library = Libraries.Prguse;
            Movable = true;
            Size = new Size(204, 152);
            Location = new Point((Settings.ScreenWidth / 2) + 10, Settings.ScreenHeight - 350);
            Sort = true;

            #region Host labels
            GuestNameLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(0, 10),
                Size = new Size(204, 14),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
            };

            GuestGoldLabel = new ExineLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(35, 123),
                Parent = this,
                Size = new Size(90, 15),
                Sound = SoundList.Gold,
                NotControl = true,
            };
            #endregion

            #region Grids
            GuestGrid = new MirItemCell[5 * 2];

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    GuestGrid[2 * x + y] = new MirItemCell
                    {
                        ItemSlot = 2 * x + y,
                        GridType = MirGridType.GuestTrade,
                        Parent = this,
                        Location = new Point(x * 36 + 10 + x, y * 32 + 39 + y),
                    };
                }
            }
            #endregion
        }

        public void RefreshInterface()
        {
            GuestNameLabel.Text = GuestName;
            GuestGoldLabel.Text = string.Format("{0:###,###,##0}", GuestGold);

            for (int i = 0; i < GuestItems.Length; i++)
            {
                if (GuestItems[i] == null) continue;
                ExineMainScene.Bind(GuestItems[i]);
            }

            Redraw();
        }

        public void TradeReset()
        {
            for (int i = 0; i < GuestItems.Length; i++)
                GuestItems[i] = null;

            GuestName = string.Empty;
            GuestGold = 0;

            Hide();
        }
    }
}

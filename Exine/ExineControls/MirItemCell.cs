using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineScenes;
using Exine.ExineSounds;
//using Client.MirScenes.Dialogs;
using Exine.ExineScenes.ExDialogs;
//using ClientPacket = ClientPacket1;

namespace Exine.ExineControls
{
    public sealed class MirItemCell : ExineImageControl
    {
        public UserItem Item
        {
            get
            {
                if (GridType == MirGridType.DropPanel)
                    return NPCDropDialog.TargetItem;
  
                if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                    return ItemArray[_itemSlot];
                return null;
            }
            set
            {
                if (GridType == MirGridType.DropPanel)
                    NPCDropDialog.TargetItem = value;
                
                else if (ItemArray != null && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                    ItemArray[_itemSlot] = value;

                SetEffect();
                Redraw();
            }
        }

        public UserItem ShadowItem
        {
            get
            {
                if ((GridType == MirGridType.Craft) && _itemSlot >= 0 && _itemSlot < ItemArray.Length)
                    return CraftDialog.ShadowItems[_itemSlot];

                return null;
            }
        }

        public UserItem[] ItemArray
        {
            get
            {
                switch (GridType)
                {
                    case MirGridType.Inventory:
                        return MapObject.User.Inventory;
                    case MirGridType.Equipment:
                        return MapObject.User.Equipment;
                    case MirGridType.Storage:
                        return ExineMainScene.Storage;
                    case MirGridType.Inspect:
                        return InspectDialog.Items;
                    case MirGridType.GuildStorage:
                        return ExineMainScene.GuildStorage;
                    case MirGridType.Trade:
                        return ExineMainScene.User.Trade;
                    case MirGridType.GuestTrade:
                        return GuestTradeDialog.GuestItems;
                   
                    case MirGridType.Fishing:
                        return MapObject.User.Equipment[(int)EquipmentSlot.Weapon].Slots;
                    case MirGridType.QuestInventory:
                        return MapObject.User.QuestInventory;
                     
                    case MirGridType.Refine:
                        return ExineMainScene.Refine;
                    case MirGridType.Craft:
                        return CraftDialog.Slots;
                    case MirGridType.Socket:
                        return ExineMainScene.SelectedItem?.Slots;

                    default:
                        throw new NotImplementedException();
                }

            }
        }

        public override bool Border
        {
            get { return (ExineMainScene.SelectedCell == this || MouseControl == this || Locked) && !(GridType == MirGridType.DropPanel || GridType == MirGridType.Craft); }
        }

        private bool _locked;

        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (_locked == value) return;
                _locked = value;
                Redraw();
            }
        }



        #region GridType

        private MirGridType _gridType;
        public event EventHandler GridTypeChanged;
        public MirGridType GridType
        {
            get { return _gridType; }
            set
            {
                if (_gridType == value) return;
                _gridType = value;
                OnGridTypeChanged();
            }
        }

        private void OnGridTypeChanged()
        {
            if (GridTypeChanged != null)
                GridTypeChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region ItemSlot

        private int _itemSlot;
        public event EventHandler ItemSlotChanged;
        public int ItemSlot
        {
            get { return _itemSlot; }
            set
            {
                if (_itemSlot == value) return;
                _itemSlot = value;
                OnItemSlotChanged();
            }
        }

        private void OnItemSlotChanged()
        {
            if (ItemSlotChanged != null)
                ItemSlotChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Count Label

        private ExineLabel CountLabel { get; set; }

        #endregion

        public MirItemCell()
        {
            //Size = new Size(36, 32);
            Size = new Size(32, 32);//k333123
            GridType = MirGridType.None;
            DrawImage = false;

            //BorderColour = Color.Lime;//k333123
            BorderColour = Color.White;//k333123

            BackColour = Color.FromArgb(255, 255, 125, 125);
            Opacity = 0.5F;
            DrawControlTexture = true;
            Library = Libraries.Items;
        }

        public void SetEffect()
        {
            //put effect stuff here??
        }


        public override void OnMouseClick(MouseEventArgs e)
        {
            if (Locked || ExineMainScene.Observing) return;

            if (ExineMainScene.PickedUpGold || GridType == MirGridType.Inspect || GridType == MirGridType.QuestInventory) return;

          

            base.OnMouseClick(e);
            
            Redraw();

            switch (e.Button)
            {
                case MouseButtons.Right:
                    if (CMain.Ctrl)
                    {
                        if (Item != null)
                        {
                            OpenItem();
                        }
                        break;
                    }

                    if (CMain.Shift)
                    {
                        if (Item != null)
                        {
                            string text = string.Format("<{0}> ", Item.FriendlyName);

                            if (ExineMainScene.Scene.ExChatDialog.ChatTextBox.Text.Length + text.Length > Globals.MaxChatLength)
                            {
                                ExineMainScene.Scene.ExChatDialog.ReceiveChat("항목을 연결할 수 없습니다. 메시지 길이가 허용된 길이를 초과합니다", ChatType.System);
                                return;
                            }

                            ExineMainScene.Scene.ExChatDialog.LinkedItems.Add(new ChatItem { UniqueID = Item.UniqueID, Title = Item.FriendlyName, Grid = GridType });
                            ExineMainScene.Scene.ExChatDialog.SetChatText(text);
                        }

                        break;
                    }

                    UseItem();
                    break;
                case MouseButtons.Left:
                    if (Item != null && ExineMainScene.SelectedCell == null)
                        PlayItemSound();

                    if (CMain.Shift)
                    {
                        if (GridType == MirGridType.Inventory || GridType == MirGridType.Storage)
                        {
                            if (ExineMainScene.SelectedCell == null && Item != null)
                            {
                                if (FreeSpace() == 0)
                                {
                                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("스택을 분할할 공간이 없습니다.", ChatType.System);
                                    return;
                                }

                                if (Item.Count > 1)
                                {
                                    MirAmountBox amountBox = new MirAmountBox("나눌 수량:", Item.Image, (uint)(Item.Count - 1));

                                    amountBox.OKButton.Click += (o, a) =>
                                    {
                                        if (amountBox.Amount == 0 || amountBox.Amount >= Item.Count) return;
                                        Network.SendPacketToServer(new ClientPacket.SplitItem { Grid = GridType, UniqueID = Item.UniqueID, Count = (ushort)amountBox.Amount });
                                        Locked = true;
                                    };

                                    amountBox.Show();
                                }
                            }
                        }
                    }
                    
                    //Add support for ALT + click to sell quickly
                    else if (CMain.Alt && ExineMainScene.Scene.NPCDropDialog.Visible && GridType == MirGridType.Inventory) // alt sell/repair
                    {
                        MoveItem(); // pickup item
                        ExineMainScene.Scene.NPCDropDialog.ItemCell.OnMouseClick(e); // emulate click to drop control
                        ExineMainScene.Scene.NPCDropDialog.ConfirmButton.OnMouseClick(e); //emulate OK to confirm trade
                    }

                    else MoveItem();
                    break;
            }
        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (Locked) return;

            if (ExineMainScene.PickedUpGold || GridType == MirGridType.Inspect || GridType == MirGridType.Craft) return;

            base.OnMouseClick(e);

            Redraw();

            ExineMainScene.SelectedCell = null;
            UseItem();
        }


        private void BuyItem()
        {
            if (Item == null || Item.Price() * ExineMainScene.NPCRate > ExineMainScene.Gold) return;

            MirAmountBox amountBox;
            if (Item.Count > 1)
            {
                amountBox = new MirAmountBox("Purchase Amount:", Item.Image, Item.Count);

                amountBox.OKButton.Click += (o, e) =>
                {
                    Network.SendPacketToServer(new ClientPacket.BuyItemBack { UniqueID = Item.UniqueID, Count = (ushort)amountBox.Amount });
                    Locked = true;
                };
            }
            else
            {
                amountBox = new MirAmountBox("Purchase", Item.Image, string.Format("Value: {0:#,##0} Gold", Item.Price()));

                amountBox.OKButton.Click += (o, e) =>
                {
                    Network.SendPacketToServer(new ClientPacket.BuyItemBack { UniqueID = Item.UniqueID, Count = 1 });
                    Locked = true;
                };
            }

            amountBox.Show();
        }

        public void OpenItem()
        {
            if ((GridType != MirGridType.Equipment && GridType != MirGridType.Inventory) || Item == null || ExineMainScene.SelectedCell == this) return;

            ExineMainScene.Scene.SocketDialog.Show(GridType, Item);
        }



        public void UseItem()
        {
            if (Locked || GridType == MirGridType.Inspect || GridType == MirGridType.GuildStorage || GridType == MirGridType.Craft) return;

           
            if ( Item.Info.Type != ItemType.Scroll && Item.Info.Type != ItemType.Potion && Item.Info.Type != ItemType.Torch) return;

            if (GridType == MirGridType.BuyBack)
            {
                BuyItem();
                return;
            }

            if (GridType == MirGridType.Equipment || GridType == MirGridType.Fishing || GridType == MirGridType.Socket)
            {
                RemoveItem();
                return;
            }

            if ((GridType != MirGridType.Inventory && GridType != MirGridType.Storage ) || Item == null || !CanUseItem() || ExineMainScene.SelectedCell == this) return;

            ExineCharacterDialog dialog = ExineMainScene.Scene.ExCharacterDialog;
            UserObject actor = ExineMainScene.User;

            if (Item.SoulBoundId != -1 && MapObject.User.Id != Item.SoulBoundId)
                return;
            

            switch (Item.Info.Type)
            {
                case ItemType.Weapon:
                    if (dialog.Grid[(int)EquipmentSlot.Weapon].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Weapon });
                        dialog.Grid[(int)EquipmentSlot.Weapon].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Armour:
                    if (dialog.Grid[(int)EquipmentSlot.Armour].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Armour });
                        dialog.Grid[(int)EquipmentSlot.Armour].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Helmet:
                    if (dialog.Grid[(int)EquipmentSlot.Helmet].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Helmet });
                        dialog.Grid[(int)EquipmentSlot.Helmet].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Necklace:
                    if (dialog.Grid[(int)EquipmentSlot.Necklace].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Necklace });
                        dialog.Grid[(int)EquipmentSlot.Necklace].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Bracelet:
                    if ((dialog.Grid[(int)EquipmentSlot.BraceletR].Item == null || dialog.Grid[(int)EquipmentSlot.BraceletR].Item.Info.Type == ItemType.Amulet) && dialog.Grid[(int)EquipmentSlot.BraceletR].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.BraceletR });
                        dialog.Grid[(int)EquipmentSlot.BraceletR].Locked = true;
                        Locked = true;
                    }
                    else if (dialog.Grid[(int)EquipmentSlot.BraceletL].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.BraceletL });
                        dialog.Grid[(int)EquipmentSlot.BraceletL].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Ring:
                    if (dialog.Grid[(int)EquipmentSlot.RingR].Item == null && dialog.Grid[(int)EquipmentSlot.RingR].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.RingR });
                        dialog.Grid[(int)EquipmentSlot.RingR].Locked = true;
                        Locked = true;
                    }
                    else if (dialog.Grid[(int)EquipmentSlot.RingL].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.RingL });
                        dialog.Grid[(int)EquipmentSlot.RingL].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Amulet:
                    //if (Item.Info.Shape == 0) return;

                    if (dialog.Grid[(int)EquipmentSlot.Amulet].Item != null && Item.Info.Type == ItemType.Amulet)
                    {
                        if (dialog.Grid[(int)EquipmentSlot.Amulet].Item.Info == Item.Info && dialog.Grid[(int)EquipmentSlot.Amulet].Item.Count < dialog.Grid[(int)EquipmentSlot.Amulet].Item.Info.StackSize)
                        {
                            Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = GridType, GridTo = MirGridType.Equipment, IDFrom = Item.UniqueID, IDTo = dialog.Grid[(int)EquipmentSlot.Amulet].Item.UniqueID });

                            Locked = true;
                            return;
                        }
                    }

                    if (dialog.Grid[(int)EquipmentSlot.Amulet].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Amulet });
                        dialog.Grid[(int)EquipmentSlot.Amulet].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Belt:
                    if (dialog.Grid[(int)EquipmentSlot.Belt].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Belt });
                        dialog.Grid[(int)EquipmentSlot.Belt].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Boots:
                    if (dialog.Grid[(int)EquipmentSlot.Boots].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Boots });
                        dialog.Grid[(int)EquipmentSlot.Boots].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Stone:
                    if (dialog.Grid[(int)EquipmentSlot.Stone].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Stone });
                        dialog.Grid[(int)EquipmentSlot.Stone].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Torch:
                    if (dialog.Grid[(int)EquipmentSlot.Torch].CanWearItem(actor, Item))
                    {
                        Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = GridType, UniqueID = Item.UniqueID, To = (int)EquipmentSlot.Torch });
                        dialog.Grid[(int)EquipmentSlot.Torch].Locked = true;
                        Locked = true;
                    }
                    break;
                case ItemType.Potion:
                case ItemType.Scroll:
                case ItemType.Book:
                case ItemType.Food:
                case ItemType.Script: 
                case ItemType.Transform:
                case ItemType.Deco:
                case ItemType.MonsterSpawn:
                    if (CanUseItem() && (GridType == MirGridType.Inventory))
                    {
                        if (CMain.Time < ExineMainScene.UseItemTime) return;
                        if (Item.Info.Type == ItemType.Potion && Item.Info.Shape == 4)
                        {
                            ExineMessageBox messageBox = new ExineMessageBox("이 물약을 사용하시겠습니까?", MirMessageBoxButtons.YesNo);
                            messageBox.YesButton.Click += (o, e) =>
                            {
                                Network.SendPacketToServer(new ClientPacket.UseItem { UniqueID = Item.UniqueID, Grid = GridType });

                                if (Item.Count == 1 && ItemSlot < ExineMainScene.User.BeltIdx)
                                {
                                    for (int i = ExineMainScene.User.BeltIdx; i < ExineMainScene.User.Inventory.Length; i++)
                                        if (ItemArray[i] != null && ItemArray[i].Info == Item.Info)
                                        {
                                            Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = MirGridType.Inventory, From = i, To = ItemSlot });
                                            ExineMainScene.Scene.ExInventoryDialog.Grid[i - ExineMainScene.User.BeltIdx].Locked = true;
                                            break;
                                        }
                                }

                                ExineMainScene.UseItemTime = CMain.Time + 100;
                                PlayItemSound();
                            };

                            messageBox.Show();
                            return;
                        }

                        Network.SendPacketToServer(new ClientPacket.UseItem { UniqueID = Item.UniqueID, Grid = GridType });

                       
                            if (Item.Count == 1 && ItemSlot < ExineMainScene.User.BeltIdx)
                            {
                                for (int i = ExineMainScene.User.BeltIdx; i < ExineMainScene.User.Inventory.Length; i++)
                                    if (ItemArray[i] != null && ItemArray[i].Info == Item.Info)
                                    {
                                    Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = MirGridType.Inventory, From = i, To = ItemSlot });
                                        ExineMainScene.Scene.ExInventoryDialog.Grid[i - ExineMainScene.User.BeltIdx].Locked = true;
                                        break;
                                    }
                            }                   

                        Locked = true;
                    }
                    break;
                
                case ItemType.Reins:
                case ItemType.Bells:
                case ItemType.Ribbon:
                case ItemType.Saddle:
                case ItemType.Mask:
                case ItemType.Hook:
                case ItemType.Float:
                case ItemType.Bait:
                case ItemType.Finder:
                case ItemType.Reel:
                case ItemType.Socket:
                    UseSlotItem();
                    break;
            }

            ExineMainScene.UseItemTime = CMain.Time + 300;
            PlayItemSound();
        }
        public void UseSlotItem()
        { 

            if (!CanUseItem()) return;

            switch (Item.Info.Type)
            {
                case ItemType.Socket:
                    if (ExineMainScene.SelectedItem != null && !ExineMainScene.SelectedItem.Info.IsFishingRod )
                    {
                        switch (Item.Info.Shape)
                        {
                            case 1:
                                if (ExineMainScene.SelectedItem.Info.Type != ItemType.Weapon) return;
                                break;
                            case 2:
                                if (ExineMainScene.SelectedItem.Info.Type != ItemType.Armour) return;
                                break;
                            case 3:
                                if (ExineMainScene.SelectedItem.Info.Type != ItemType.Ring && ExineMainScene.SelectedItem.Info.Type != ItemType.Bracelet && ExineMainScene.SelectedItem.Info.Type != ItemType.Necklace) return;
                                break;
                        }

                        MirItemCell cell = null;
                        for (int i = 0; i < ExineMainScene.Scene.SocketDialog.Grid.Length; i++)
                        {
                            if (!ExineMainScene.Scene.SocketDialog.Grid[i].Visible || ExineMainScene.Scene.SocketDialog.Grid[i].Item != null) continue;
                            cell = ExineMainScene.Scene.SocketDialog.Grid[i];
                            break;
                        }

                        if (cell != null && cell.CanWearItem(ExineMainScene.User, Item))
                        {
                            Network.SendPacketToServer(new ClientPacket.EquipSlotItem { Grid = GridType, UniqueID = Item.UniqueID, To = cell.ItemSlot, GridTo = MirGridType.Socket, ToUniqueID = ExineMainScene.SelectedItem.UniqueID });
                            cell.Locked = true;
                            Locked = true;
                        }
                    }
                    break;
                
                 
            }
        }

        public void RemoveItem()
        {
            int count = 0;

            for (int i = 0; i < ExineMainScene.User.Inventory.Length; i++)
            {
                MirItemCell itemCell = i < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[i] : ExineMainScene.Scene.ExInventoryDialog.Grid[i - ExineMainScene.User.BeltIdx];

                if (itemCell.Item == null) count++;
            }

            if (Item == null || count < 1 || ( Item.Info.Type != ItemType.Torch)) return;

            if (Item.Info.StackSize > 1)
            {
                UserItem item = null;

                for (int i = 0; i < ExineMainScene.User.Inventory.Length; i++)
                {
                    MirItemCell itemCell = i < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[i] : ExineMainScene.Scene.ExInventoryDialog.Grid[i - ExineMainScene.User.BeltIdx];

                    if (itemCell.Item == null || itemCell.Item.Info != Item.Info) continue;

                    item = itemCell.Item;
                }

                if (item != null && ((item.Count + Item.Count) <= item.Info.StackSize))
                {
                    //Merge.
                    Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = GridType, GridTo = MirGridType.Inventory, IDFrom = Item.UniqueID, IDTo = item.UniqueID });

                    Locked = true;

                    PlayItemSound();
                    return;
                }
            }

            for (int i = 0; i < ExineMainScene.User.Inventory.Length; i++)
            {
                MirItemCell itemCell;

                if (Item.Info.Type == ItemType.Amulet)
                {
                    itemCell = i < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[i] : ExineMainScene.Scene.ExInventoryDialog.Grid[i - ExineMainScene.User.BeltIdx];
                }
                else
                {
                    itemCell = i < (ExineMainScene.User.Inventory.Length - ExineMainScene.User.BeltIdx) ? ExineMainScene.Scene.ExInventoryDialog.Grid[i] : ExineMainScene.Scene.BeltDialog.Grid[i - ExineMainScene.User.Inventory.Length];
                }

                if (itemCell.Item != null) continue;

                if (GridType != MirGridType.Equipment)
                {
                    ulong fromID;

                    if (GridType == MirGridType.Fishing)
                    {
                        if (ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.Weapon].Item == null) return;

                        fromID = ExineMainScene.Scene.ExCharacterDialog.Grid[(byte)EquipmentSlot.Weapon].Item.UniqueID;
                    }
                   
                    else
                    {
                        if (ExineMainScene.SelectedItem == null) return;

                        fromID = ExineMainScene.SelectedItem.UniqueID;
                    }

                    Network.SendPacketToServer(new ClientPacket.RemoveSlotItem { Grid = GridType, UniqueID = Item.UniqueID, To = itemCell.ItemSlot, GridTo = MirGridType.Inventory, FromUniqueID = fromID });
                }
                else
                {
                    Network.SendPacketToServer(new ClientPacket.RemoveItem { Grid = MirGridType.Inventory, UniqueID = Item.UniqueID, To = itemCell.ItemSlot });
                }

                Locked = true;

                PlayItemSound();
                break;
            }
        }

       
        private void MoveItem()
        {
            if (GridType == MirGridType.BuyBack || GridType == MirGridType.DropPanel || GridType == MirGridType.Inspect || GridType == MirGridType.Craft) return;

            if (ExineMainScene.SelectedCell != null)
            {
                if (ExineMainScene.SelectedCell.Item == null || ExineMainScene.SelectedCell == this)
                {
                    ExineMainScene.SelectedCell = null;
                    return;
                }

                switch (GridType)
                {
                    #region To Inventory
                    case MirGridType.Inventory: // To Inventory
                        switch (ExineMainScene.SelectedCell.GridType)
                        {
                            #region From Inventory
                            case MirGridType.Inventory: //From Invenotry
                                if (Item != null)
                                {
                                    if (CMain.Ctrl)
                                    {
                                        ExineMessageBox messageBox = new ExineMessageBox("이 아이템들을 조합하시겠습니까?", MirMessageBoxButtons.YesNo);
                                        messageBox.YesButton.Click += (o, e) =>
                                        {
                                            //Combine
                                            Network.SendPacketToServer(new ClientPacket.CombineItem { Grid = ExineMainScene.SelectedCell.GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });
                                            Locked = true;
                                            ExineMainScene.SelectedCell.Locked = true;
                                            ExineMainScene.SelectedCell = null;
                                        };

                                        messageBox.Show();
                                        return;
                                    }

                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = GridType, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });

                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                            #endregion
                            #region From Equipment
                            case MirGridType.Equipment: //From Equipment
                                if (Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (!CanRemoveItem(ExineMainScene.SelectedCell.Item))
                                {
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }
                                if (Item == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.RemoveItem { Grid = GridType, UniqueID = ExineMainScene.SelectedCell.Item.UniqueID, To = ItemSlot });

                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 6; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.RemoveItem { Grid = GridType, UniqueID = ExineMainScene.SelectedCell.Item.UniqueID, To = x });

                                        MirItemCell temp = x < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[x] : ExineMainScene.Scene.ExInventoryDialog.Grid[x - ExineMainScene.User.BeltIdx];

                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Storage
                            case MirGridType.Storage: //From Storage
                                if (Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (Item == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.TakeBackItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });

                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 6; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.TakeBackItem { From = ExineMainScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = x < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[x] : ExineMainScene.Scene.ExInventoryDialog.Grid[x - ExineMainScene.User.BeltIdx];

                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Guild Storage
                            case MirGridType.GuildStorage:
                                if (Item != null)
                                {
                                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("아이템을 교환할 수 없습니다.", ChatType.System);
                                    return;
                                }
                                if (!GuildDialog.MyOptions.HasFlag(GuildRankOptions.CanRetrieveItem))
                                {
                                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("아이템을 저장할 수 있는 권한이 부족합니다.", ChatType.System);
                                    return;
                                }
                                Network.SendPacketToServer(new ClientPacket.GuildStorageItemChange { Type = 1, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                break;
                            #endregion
                            #region From Trade
                            case MirGridType.Trade: //From Trade
                                if (Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (Item == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.RetrieveTradeItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });

                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 6; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.RetrieveTradeItem { From = ExineMainScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = x < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[x] : ExineMainScene.Scene.ExInventoryDialog.Grid[x - ExineMainScene.User.BeltIdx];

                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                             
                            #region From Refine
                            case MirGridType.Refine: //From AwakenItem
                                if (Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (Item == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.RetrieveRefineItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });

                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 6; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.RetrieveRefineItem { From = ExineMainScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = x < ExineMainScene.User.BeltIdx ? ExineMainScene.Scene.BeltDialog.Grid[x] : ExineMainScene.Scene.ExInventoryDialog.Grid[x - ExineMainScene.User.BeltIdx];

                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                           
                           
                        }
                        break;
                    #endregion
                    #region To Equipment
                    case MirGridType.Equipment: //To Equipment

                        if (ExineMainScene.SelectedCell.GridType != MirGridType.Inventory && ExineMainScene.SelectedCell.GridType != MirGridType.Storage) return;


                        if (Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Amulet)
                        {
                            if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                            {
                                Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                            }
                        }

                        if (CorrectSlot(ExineMainScene.SelectedCell.Item, ExineMainScene.SelectedCell.GridType))
                        {
                            if (CanWearItem(ExineMainScene.User, ExineMainScene.SelectedCell.Item))
                            {
                                Network.SendPacketToServer(new ClientPacket.EquipItem { Grid = ExineMainScene.SelectedCell.GridType, UniqueID = ExineMainScene.SelectedCell.Item.UniqueID, To = ItemSlot });
                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                            }
                            ExineMainScene.SelectedCell = null;
                        }
                        return;
                    #endregion
                    #region To Storage
                    case MirGridType.Storage: //To Storage
                        switch (ExineMainScene.SelectedCell.GridType)
                        {
                            #region From Inventory
                            case MirGridType.Inventory: //From Invenotry
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (ItemArray[ItemSlot] == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.StoreItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.StoreItem { From = ExineMainScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = ExineMainScene.Scene.StorageDialog.Grid[x];
                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Equipment
                            case MirGridType.Equipment: //From Equipment
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                if (!CanRemoveItem(ExineMainScene.SelectedCell.Item))
                                {
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                if (Item == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.RemoveItem { Grid = GridType, UniqueID = ExineMainScene.SelectedCell.Item.UniqueID, To = ItemSlot });

                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.RemoveItem { Grid = GridType, UniqueID = ExineMainScene.SelectedCell.Item.UniqueID, To = x });

                                        MirItemCell temp = ExineMainScene.Scene.StorageDialog.Grid[x];
                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                            #endregion
                            #region From Storage
                            case MirGridType.Storage:
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = GridType, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                                #endregion

                        }
                        break;

                    #endregion
                    #region To guild storage
                    case MirGridType.GuildStorage: //To Guild Storage
                        switch (ExineMainScene.SelectedCell.GridType)
                        {
                            case MirGridType.GuildStorage: //From Guild Storage
                                if (ExineMainScene.SelectedCell.GridType == MirGridType.GuildStorage)
                                {
                                    if (!GuildDialog.MyOptions.HasFlag(GuildRankOptions.CanStoreItem))
                                    {
                                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("아이템을 저장할 수 있는 권한이 부족합니다.", ChatType.System);
                                        return;
                                    }

                                    //if (ItemArray[ItemSlot] == null)
                                    //{
                                    Network.SendPacketToServer(new ClientPacket.GuildStorageItemChange { Type = 2, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                    //}
                                }
                                return;

                            case MirGridType.Inventory:

                                if (ExineMainScene.SelectedCell.GridType == MirGridType.Inventory)
                                {
                                    if (Item != null)
                                    {
                                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("아이템을 교환할 수 없습니다.", ChatType.System);
                                        return;
                                    }
                                    if (!GuildDialog.MyOptions.HasFlag(GuildRankOptions.CanStoreItem))
                                    {
                                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("아이템을 저장할 수 있는 권한이 부족합니다.", ChatType.System);
                                        return;
                                    }
                                    if (ItemArray[ItemSlot] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.GuildStorageItemChange { Type = 0, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }
                                return;
                        }
                        break;
                    #endregion
                    #region To Trade

                    case MirGridType.Trade:
                        if (Item != null && Item.Info.Bind.HasFlag(BindMode.DontTrade)) return;

                        switch (ExineMainScene.SelectedCell.GridType)
                        {
                            #region From Trade
                            case MirGridType.Trade: //From Trade
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = GridType, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });

                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                            #endregion

                            #region From Inventory
                            case MirGridType.Inventory: //From Inventory
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }


                                if (ItemArray[ItemSlot] == null)
                                {
                                    Network.SendPacketToServer(new ClientPacket.DepositTradeItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                    Locked = true;
                                    ExineMainScene.SelectedCell.Locked = true;
                                    ExineMainScene.SelectedCell = null;
                                    return;
                                }

                                for (int x = 0; x < ItemArray.Length; x++)
                                    if (ItemArray[x] == null)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.DepositTradeItem { From = ExineMainScene.SelectedCell.ItemSlot, To = x });

                                        MirItemCell temp = ExineMainScene.Scene.TradeDialog.Grid[x];
                                        if (temp != null) temp.Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                break;
                                #endregion
                        }
                        break;

                    #endregion
                    #region To Refine 

                    case MirGridType.Refine:

                        switch (ExineMainScene.SelectedCell.GridType)
                        {
                            #region From Refine
                            case MirGridType.Refine: //From Refine
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        //Merge.
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.SendPacketToServer(new ClientPacket.MoveItem { Grid = GridType, From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                            #endregion

                            #region From Inventory
                            case MirGridType.Inventory: //From Inventory
                                if (Item != null)
                                {
                                    if (ExineMainScene.SelectedCell.Item.Info == Item.Info && Item.Count < Item.Info.StackSize)
                                    {
                                        Network.SendPacketToServer(new ClientPacket.MergeItem { GridFrom = ExineMainScene.SelectedCell.GridType, GridTo = GridType, IDFrom = ExineMainScene.SelectedCell.Item.UniqueID, IDTo = Item.UniqueID });

                                        Locked = true;
                                        ExineMainScene.SelectedCell.Locked = true;
                                        ExineMainScene.SelectedCell = null;
                                        return;
                                    }
                                }

                                Network.SendPacketToServer(new ClientPacket.DepositRefineItem { From = ExineMainScene.SelectedCell.ItemSlot, To = ItemSlot });
                                Locked = true;
                                ExineMainScene.SelectedCell.Locked = true;
                                ExineMainScene.SelectedCell = null;
                                return;
                                #endregion
                        }
                        break;

                    #endregion
                     
                }

                return;
            }

            if (Item != null)
            {
                ExineMainScene.SelectedCell = this;
            }
        }
        private void PlayItemSound()
        {
            if (Item == null) return;

            switch (Item.Info.Type)
            {
                case ItemType.Weapon:
                    SoundManager.PlaySound(SoundList.ClickWeapon);
                    break;
                case ItemType.Armour:
                    SoundManager.PlaySound(SoundList.ClickArmour);
                    break;
                case ItemType.Helmet:
                    SoundManager.PlaySound(SoundList.ClickHelmet);
                    break;
                case ItemType.Necklace:
                    SoundManager.PlaySound(SoundList.ClickNecklace);
                    break;
                case ItemType.Bracelet:
                    SoundManager.PlaySound(SoundList.ClickBracelet);
                    break;
                case ItemType.Ring:
                    SoundManager.PlaySound(SoundList.ClickRing);
                    break;
                case ItemType.Boots:
                    SoundManager.PlaySound(SoundList.ClickBoots);
                    break;
                case ItemType.Potion:
                    SoundManager.PlaySound(SoundList.ClickDrug);
                    break;
                default:
                    SoundManager.PlaySound(SoundList.ClickItem);
                    break;
            }
        }

        private int FreeSpace()
        {
            int count = 0;

            for (int i = 0; i < ItemArray.Length; i++)
                if (ItemArray[i] == null) count++;

            return count;
        }


        private bool CanRemoveItem(UserItem i)
        {
            if( i.Info.Type != ItemType.Torch)
            {
                return false;
            }
            //stuck
            return FreeSpace() > 0;
        }

        private bool CorrectSlot(UserItem i, MirGridType grid)
        {
            ItemType type = i.Info.Type;

            switch (GridType)
            {
                case MirGridType.Equipment:
                    if (grid != MirGridType.Inventory && grid != MirGridType.Storage)
                        return false;
                    break;
            }

            switch ((EquipmentSlot)ItemSlot)
            {
                case EquipmentSlot.Weapon:
                    return type == ItemType.Weapon;
                case EquipmentSlot.Armour:
                    return type == ItemType.Armour;
                case EquipmentSlot.Helmet:
                    return type == ItemType.Helmet;
                case EquipmentSlot.Torch:
                    return type == ItemType.Torch;
                case EquipmentSlot.Necklace:
                    return type == ItemType.Necklace;
                case EquipmentSlot.BraceletL:
                    return i.Info.Type == ItemType.Bracelet;
                case EquipmentSlot.BraceletR:
                    return i.Info.Type == ItemType.Bracelet || i.Info.Type == ItemType.Amulet;
                case EquipmentSlot.RingL:
                case EquipmentSlot.RingR:
                    return type == ItemType.Ring;
                case EquipmentSlot.Amulet:
                    return type == ItemType.Amulet;// && i.Info.Shape > 0;
                case EquipmentSlot.Boots:
                    return type == ItemType.Boots;
                case EquipmentSlot.Belt:
                    return type == ItemType.Belt;
                case EquipmentSlot.Stone:
                    return type == ItemType.Stone;
               
                default:
                    return false;
            }

        }
        private bool CanUseItem()
        {
            if (Item == null) return false;

            UserObject actor = ExineMainScene.User;
            
            switch (actor.Gender)
            {
                case ExineGender.Male:
                    if (!Item.Info.RequiredGender.HasFlag(RequiredGender.Male))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.NotFemale, ChatType.System);
                        return false;
                    }
                    break;
                case ExineGender.Female:
                    if (!Item.Info.RequiredGender.HasFlag(RequiredGender.Female))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.NotMale, ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (actor.Class)
            {
                case ExineClass.Warrior:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("전사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Wizard:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("마법사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Taoist:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("도사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Assassin:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("암살자는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Archer:
                    if (!Item.Info.RequiredClass.HasFlag(RequiredClass.Archer))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("궁수는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (Item.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (actor.Level < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowLevel, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxAC:
                    if (actor.Stats[Stat.MaxAC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("AC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMAC:
                    if (actor.Stats[Stat.MaxMAC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("MAC이 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxDC:
                    if (actor.Stats[Stat.MaxDC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowDC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMC:
                    if (actor.Stats[Stat.MaxMC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowMC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxSC:
                    if (actor.Stats[Stat.MaxSC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowSC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxLevel:
                    if (actor.Level > Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("최대 레벨을 초과했습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinAC:
                    if (actor.Stats[Stat.MinAC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 AC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMAC:
                    if (actor.Stats[Stat.MinMAC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 MAC이 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinDC:
                    if (actor.Stats[Stat.MinDC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 DC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMC:
                    if (actor.Stats[Stat.MinMC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 MC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinSC:
                    if (actor.Stats[Stat.MinSC] < Item.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 SC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (Item.Info.Type)
            {
               
                case ItemType.Hook:
                case ItemType.Float:
                case ItemType.Bait:
                case ItemType.Finder:
                case ItemType.Reel:
                    if (actor.Equipment[(int)EquipmentSlot.Weapon] == null || !actor.Equipment[(int)EquipmentSlot.Weapon].Info.IsFishingRod)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("낚싯대를 장착하지 않았습니다.", ChatType.System);
                        return false;
                    }
                    break;
            }
            return true;
        }

        private bool CanWearItem(UserObject actor, UserItem i)
        {
            if (i == null) return false;

            //If Can remove;

            switch (actor.Gender)
            {
                case ExineGender.Male:
                    if (!i.Info.RequiredGender.HasFlag(RequiredGender.Male))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.NotFemale, ChatType.System);
                        return false;
                    }
                    break;
                case ExineGender.Female:
                    if (!i.Info.RequiredGender.HasFlag(RequiredGender.Female))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.NotMale, ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (actor.Class)
            {
                case ExineClass.Warrior:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("전사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Wizard:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("마법사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Taoist:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("도사는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Assassin:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("암살자는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case ExineClass.Archer:
                    if (!i.Info.RequiredClass.HasFlag(RequiredClass.Archer))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("궁수는 이 아이템을 사용할 수 없습니다.", ChatType.System);
                        return false;
                    }
                    break;
            }

            switch (i.Info.RequiredType)
            {
                case RequiredType.Level:
                    if (actor.Level < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowLevel, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxAC:
                    if (actor.Stats[Stat.MaxAC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("AC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMAC:
                    if (actor.Stats[Stat.MaxMAC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("MAC이 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxDC:
                    if (actor.Stats[Stat.MaxDC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowDC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxMC:
                    if (actor.Stats[Stat.MaxMC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowMC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxSC:
                    if (actor.Stats[Stat.MaxSC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowSC, ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MaxLevel:
                    if (actor.Level > i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("최대 레벨을 초과했습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinAC:
                    if (actor.Stats[Stat.MinAC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 AC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMAC:
                    if (actor.Stats[Stat.MinMAC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 MAC이 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinDC:
                    if (actor.Stats[Stat.MinDC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 DC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinMC:
                    if (actor.Stats[Stat.MinMC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 MC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
                case RequiredType.MinSC:
                    if (actor.Stats[Stat.MinSC] < i.Info.RequiredAmount)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("기본 SC가 충분하지 않습니다.", ChatType.System);
                        return false;
                    }
                    break;
            }

            if (i.Info.Type == ItemType.Weapon || i.Info.Type == ItemType.Torch)
            {
                if (i.Weight - (Item != null ? Item.Weight : 0) + actor.CurrentHandWeight > actor.Stats[Stat.HandWeight])
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.TooHeavyToHold, ChatType.System);
                    return false;
                }
            }
            else
            {
                if (i.Weight - (Item != null ? Item.Weight : 0) + actor.CurrentWearWeight > actor.Stats[Stat.WearWeight])
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("너무 무거워서 착용할 수 없습니다.", ChatType.System);
                    return false;
                }
            }

            switch (i.Info.Type)
            {
                case ItemType.Bait:
                case ItemType.Finder:
                case ItemType.Hook:
                case ItemType.Reel:
                case ItemType.Float:
                    if (!actor.HasFishingRod)
                    {
                        return false;
                    }
                    break;
                
                case ItemType.Socket:
                    if (ExineMainScene.SelectedItem == null  || (ExineMainScene.SelectedItem.Info.Type == ItemType.Weapon && ExineMainScene.SelectedItem.Info.IsFishingRod))
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        protected internal override void DrawControl()
        {
            if (Item != null && ExineMainScene.SelectedCell != this && Locked != true)
            {
                CreateDisposeLabel();

                if (Library != null)
                {
                    ushort image = Item.Image;

                    Size imgSize = Library.GetTrueSize(image);

                    Point offSet = new Point((Size.Width - imgSize.Width) / 2, (Size.Height - imgSize.Height) / 2);

                    if (GridType == MirGridType.Craft)
                    {
                        Libraries.Prguse.Draw(1121, DisplayLocation.Add(new Point(-2, -1)), Color.White, UseOffSet, 0.8F);
                    }

                    Library.Draw(image, DisplayLocation.Add(offSet), ForeColour, UseOffSet, 1F);

                    if (Item.SealedInfo != null && Item.SealedInfo.ExpiryDate > CMain.Now)
                    {
                        Libraries.StateItems.Draw(3590, DisplayLocation.Add(new Point(2, 2)), Color.White, UseOffSet, 1F);
                    }
                }
            }
            else if (Item != null && (ExineMainScene.SelectedCell == this || Locked))
            {
                CreateDisposeLabel();

                if (Library != null)
                {
                    ushort image = Item.Image;

                    Size imgSize = Library.GetTrueSize(image);

                    Point offSet = new Point((Size.Width - imgSize.Width) / 2, (Size.Height - imgSize.Height) / 2);

                    Library.Draw(image, DisplayLocation.Add(offSet), Color.DimGray, UseOffSet, 0.8F);
                }
            }
            else if (ShadowItem != null)
            {
                CreateDisposeLabel();

                if (Library != null)
                {
                    ushort image = ShadowItem.Info.Image;

                    Size imgSize = Library.GetTrueSize(image);

                    Point offSet = new Point((Size.Width - imgSize.Width) / 2, (Size.Height - imgSize.Height) / 2);

                    if (GridType == MirGridType.Craft)
                    {
                        Libraries.Prguse.Draw(1121, DisplayLocation.Add(new Point(-2, -1)), Color.White, UseOffSet, 0.8F);
                    }

                    Library.Draw(image, DisplayLocation.Add(offSet), Color.DimGray, UseOffSet, 0.8F);
                }
            }
            else
                DisposeCountLabel();
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            if (GridType == MirGridType.Inspect)
                ExineMainScene.Scene.CreateItemLabel(Item, true);
            else
            {
                if (Item != null)
                    ExineMainScene.Scene.CreateItemLabel(Item);
                else if (ShadowItem != null)
                    ExineMainScene.Scene.CreateItemLabel(ShadowItem, false, ShadowItem.CurrentDura == ShadowItem.MaxDura);
            }
        }
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            ExineMainScene.Scene.DisposeItemLabel();
            ExineMainScene.HoverItem = null;
        }

        private void CreateDisposeLabel()
        {
            if (Item == null && ShadowItem == null)
                return;

            if (Item != null && ShadowItem == null && Item.Info.StackSize <= 1)
            {
                DisposeCountLabel();
                return;
            }

            if (CountLabel == null || CountLabel.IsDisposed)
            {
                CountLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    NotControl = true,
                    OutLine = false,
                    Parent = this,
                };
            }

            if (ShadowItem != null)
            {
                CountLabel.ForeColour = (Item == null || ShadowItem.Count > Item.Count) ? Color.Red : Color.LimeGreen;
                CountLabel.Text = string.Format("{0}/{1}", Item == null ? 0 : Item.Count, ShadowItem.Count);
            }
            else
            {
                CountLabel.Text = Item.Count.ToString("###0");
            }

            CountLabel.Location = new Point(Size.Width - CountLabel.Size.Width, Size.Height - CountLabel.Size.Height);
        }
        private void DisposeCountLabel()
        {
            if (CountLabel != null && !CountLabel.IsDisposed)
                CountLabel.Dispose();
            CountLabel = null;
        }
    }
}

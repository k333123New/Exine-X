using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineSounds;

namespace Exine.ExineScenes.Dialogs
{
    public sealed class SocketDialog : ExineImageControl
    {
        public MirItemCell[] Grid;
        public MirButton CloseButton;

        public SocketDialog()
        {
            Index = 20;
            Library = Libraries.Prguse3;
            Movable = true;
            Sort = true;
            Location = new Point(0, 0);

            Grid = new MirItemCell[6 * 2];

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int idx = 6 * y + x;

                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,
                        GridType = MirGridType.Socket,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 23 + x, y * 33 + 15 + y),
                    };
                }
            }

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
            CloseButton.Click += (o, e) => Hide();
        }

        private void BindGrid()
        {
            int count = 0;

            if (ExineMainScene.SelectedItem != null)
            {
                count = ExineMainScene.SelectedItem.Slots.Length;
            }

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int idx = 6 * y + x;

                    Grid[idx].Visible = idx < count;
                }
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

        public override void Hide()
        {
            ExineMainScene.SelectedItem = null;
            base.Hide();
        }

        public void Show(MirGridType grid, UserItem item)
        {
            if (item.Slots.Length == 0)
            {
                ExineMainScene.SelectedItem = null;
                Visible = false;
                return;
            }

            ExineMainScene.SelectedItem = item;

            Index = 20 + (ExineMainScene.SelectedItem.Slots.Length - 1);

            BindGrid();

            CloseButton.Location = new Point(Size.Width - 23, 3);

            switch (grid)
            {
                case MirGridType.Inventory:
                    Location = new Point(
                        ExineMainScene.Scene.ExInventoryDialog.Location.X + ((ExineMainScene.Scene.ExInventoryDialog.Size.Width - Size.Width) / 2),
                        ExineMainScene.Scene.ExInventoryDialog.Location.Y + ExineMainScene.Scene.ExInventoryDialog.Size.Height + 5);
                    break;
                case MirGridType.Equipment:
                    Location = new Point(
                        ExineMainScene.Scene.ExCharacterDialog.Location.X + ((ExineMainScene.Scene.ExCharacterDialog.Size.Width - Size.Width) / 2),
                        ExineMainScene.Scene.ExCharacterDialog.Location.Y + ExineMainScene.Scene.ExCharacterDialog.Size.Height + 5);
                    break;
            }

            Visible = true;
        }
    }

}

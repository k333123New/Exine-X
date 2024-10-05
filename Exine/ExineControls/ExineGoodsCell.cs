using Exine.ExineGraphics;
using Exine.ExineScenes;
using SlimDX;

namespace Exine.ExineControls
{
    public sealed class ExineGoodsCell : ExineControl
    {
        public UserItem Item;

        public ExineLabel NameLabel, PriceLabel, CountLabel;
        public bool UsePearls = false;
        public bool Recipe = false;

        public bool MultipleAvailable = false;
        public ExineImageControl NewIcon;

        public const int height = 24;

        public ExineGoodsCell()
        {
            //Size = new Size(205, 32);
            Size = new Size(205, height);
            BorderColour = Color.Lime;

            NameLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(44, 0),
            };

            CountLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                DrawControlTexture = true,
                Location = new Point(23, 17),
                ForeColour = Color.Yellow,
            };

            PriceLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(44+70+47, 14-14),
            };

            NewIcon = new ExineImageControl
            {
                Index = 550,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(190, 5),
                NotControl = true,
                Visible = false
            };

            BeforeDraw += (o, e) => Update();
            AfterDraw += (o, e) => DrawItem();
        }

        private void Update()
        {
            NewIcon.Visible = false;

            if (Item == null || Item.Info == null) return;
            NameLabel.Text = Item.Info.FriendlyName;
            CountLabel.Text = (Item.Count <= 1) ? "" : Item.Count.ToString();

            NewIcon.Visible = !Item.IsShopItem || MultipleAvailable;

            if (UsePearls)
            {
                PriceLabel.Text = string.Format("Price: {0} pearl{1}", (uint)(Item.Price() * ExineMainScene.NPCRate), Item.Price() > 1 ? "s" : "");
            }
            else if (Recipe)
            {
                ClientRecipeInfo recipe = ExineMainScene.RecipeInfoList.SingleOrDefault(x => x.Item.ItemIndex == Item.ItemIndex);

                //PriceLabel.Text = string.Format("Price: {0} gold", (uint)(recipe.Gold * ExineMainScene.NPCRate));
                PriceLabel.Text = string.Format("{0}", (uint)(recipe.Gold * ExineMainScene.NPCRate));
            }
            else
            {
                //PriceLabel.Text = string.Format("Price: {0} gold", (uint)(Item.Price() * ExineMainScene.NPCRate));
                PriceLabel.Text = string.Format("{0}", (uint)(Item.Price() * ExineMainScene.NPCRate));
            }
        }

        protected override Vector2[] BorderInfo
        {
            get
            {
                if (Size == Size.Empty) return null;

                if (BorderRectangle != DisplayRectangle)
                {
                    _borderInfo = new[]
                        {
                            new Vector2(DisplayRectangle.Left - 1, DisplayRectangle.Top - 1),
                            new Vector2(DisplayRectangle.Right, DisplayRectangle.Top - 1),

                            new Vector2(DisplayRectangle.Left - 1, DisplayRectangle.Top - 1),
                            new Vector2(DisplayRectangle.Left - 1, DisplayRectangle.Bottom),

                            new Vector2(DisplayRectangle.Left - 1, DisplayRectangle.Bottom),
                            new Vector2(DisplayRectangle.Right, DisplayRectangle.Bottom),

                            new Vector2(DisplayRectangle.Right, DisplayRectangle.Top - 1),
                            new Vector2(DisplayRectangle.Right, DisplayRectangle.Bottom),

                            new Vector2(DisplayRectangle.Left + 40, DisplayRectangle.Bottom),
                            new Vector2(DisplayRectangle.Left + 40, DisplayRectangle.Top - 1)
                        };

                    BorderRectangle = DisplayRectangle;
                }
                return _borderInfo;
            }
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();

            ExineMainScene.Scene.CreateItemLabel(Item, hideAdded: ExineMainScene.HideAddedStoreStats);
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            ExineMainScene.Scene.DisposeItemLabel();
            ExineMainScene.HoverItem = null;
        }

        private void DrawItem()
        {
            if (Item == null || Item.Info == null) return;

            Size size = Libraries.Items.GetTrueSize(Item.Image);
            //Point offSet = new Point((40 - size.Width)/2, (32 - size.Height)/2);
            Point offSet = new Point((40 - size.Width) / 2, (height - size.Height) / 2);
            Libraries.Items.Draw(Item.Image, offSet.X + DisplayLocation.X, offSet.Y + DisplayLocation.Y);

            CountLabel.Draw();
        }
    }
}

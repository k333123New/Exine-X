using System.Globalization;
using System.Text.RegularExpressions;
using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineSounds;
using Font = System.Drawing.Font;
using C = ClientPackets;
using System.Diagnostics;
using System.Xml;

namespace Exine.ExineScenes.ExDialogs
{
    public sealed class ExineNPCDialog : ExineImageControl
    {
        public static Regex R = new Regex(@"<((.*?)\/(\@.*?))>");
        public static Regex C = new Regex(@"{((.*?)\/(.*?))}");
        public static Regex L = new Regex(@"\(((.*?)\/(.*?))\)");
        public static Regex B = new Regex(@"<<((.*?)\/(\@.*?))>>");

        //public MirButton CloseButton, UpButton, DownButton, PositionBar, QuestButton, HelpButton;
        public ExineButton CloseButton, UpButton, DownButton, PositionBar, QuestButton;
        public ExineLabel[] TextLabel;
        public List<ExineLabel> TextButtons;
        public List<BigButton> BigButtons;
        public BigButtonDialog BigButtonDialog;

        public ExineLabel NameLabel, JobNameLabel;//k333123
        public ExineImageControl PortraitImage;

        Font font = new Font(Settings.FontName, 9F);

        public List<string> CurrentLines = new List<string>();
        private int _index = 0;
        public int MaximumLines = 8;

        public ExineNPCDialog()
        {
            //talk dialog! k333123
            //Index = 995;
            //Library = Libraries.Prguse;
            Index = 0;
            Library = Libraries.PANEL0510;
            Location = new Point(114, 82);
            

            TextLabel = new ExineLabel[30];
            TextButtons = new List<ExineLabel>();
            BigButtons = new List<BigButton>();
            //Size = Size;
            //AutoSize = false;
            AutoSize = true;

            MouseWheel += NPCDialog_MouseWheel;

            //Sort = true;
             
            PortraitImage = new ExineImageControl
            {
                Index = 1,
                Library = Libraries.ExineNPCPortrait,
                Parent = this,
                Location = new Point(15, 28),
                Visible = true
            };
            
            JobNameLabel = new ExineLabel
            {
                Text = "",
                Parent = this,
                Font = new Font(Settings.FontName, 10F, FontStyle.Bold),
                //ForeColour = Color.BurlyWood,
                ForeColour = Color.White,
                Location = new Point(30 - 14, 6 + 178),
                AutoSize = true
            };

            NameLabel = new ExineLabel
            {
                Text = "",
                Parent = this,
                Font = new Font(Settings.FontName, 10F, FontStyle.Bold),
                //ForeColour = Color.BurlyWood,
                ForeColour = Color.White,
                Location = new Point(30 - 14, 6 + 178 + 15),
                AutoSize = true
            };

            UpButton = new ExineButton
            {
                Index = 197,
                HoverIndex = 198,
                PressedIndex = 199,
                Library = Libraries.Prguse2,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(417, 34),
                Sound = SoundList.ButtonA,
                Visible = false
            };
            UpButton.Click += (o, e) =>
            {
                if (_index <= 0) return;

                _index--;

                NewText(CurrentLines, false);
                UpdatePositionBar();
            };

            DownButton = new ExineButton
            {
                Index = 207,
                HoverIndex = 208,
                Library = Libraries.Prguse2,
                PressedIndex = 209,
                Parent = this,
                Size = new Size(16, 14),
                Location = new Point(417, 175),
                Sound = SoundList.ButtonA,
                Visible = false
            };
            DownButton.Click += (o, e) =>
            {
                if (_index + MaximumLines >= CurrentLines.Count) return;

                _index++;

                NewText(CurrentLines, false);
                UpdatePositionBar();
            };

            PositionBar = new ExineButton
            {
                Index = 205,
                HoverIndex = 206,
                PressedIndex = 206,
                Library = Libraries.Prguse2,
                Location = new Point(417, 47),
                Parent = this,
                Movable = true,
                Sound = SoundList.None,
                Visible = false
            };
            PositionBar.OnMoving += PositionBar_OnMoving;            

            CloseButton = new ExineButton
            {
                HoverIndex = 11,
                Index = 9,
                Location = new Point(413-67+3, 3+493-57),
                Library = Libraries.PANEL0510,
                Parent = this,
                PressedIndex = 10,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();
             
            BigButtonDialog = new BigButtonDialog()
            {
                Parent = this,               
            };

            QuestButton = new ExineAnimatedButton()
            {
                Animated = true,
                AnimationCount = 10,
                Loop = true,
                AnimationDelay = 130,
                Index = 530,
                HoverIndex = 284,
                PressedIndex = 286,
                Library = Libraries.Title,
                Parent = this,
                Size = new Size(96, 25),
                Sound = SoundList.ButtonA,
                Visible = false
            };

            QuestButton.Click += (o, e) => ExineMainScene.Scene.QuestListDialog.Toggle();
        }

       

        void NPCDialog_MouseWheel(object sender, MouseEventArgs e)
        { 
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (_index == 0 && count >= 0) return;
            if (_index == CurrentLines.Count - 1 && count <= 0) return;
            if (CurrentLines.Count <= MaximumLines) return;

            _index -= count;

            if (_index < 0) _index = 0;
            if (_index + MaximumLines > CurrentLines.Count - 1) _index = CurrentLines.Count - MaximumLines;

            NewText(CurrentLines, false);

            UpdatePositionBar();
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        { 

            int x = 417;
            int y = PositionBar.Location.Y;

            if (y >= 155) y = 155;
            if (y <= 47) y = 47;

            int location = y - 47;
            int interval = 108 / (CurrentLines.Count - MaximumLines);

            double yPoint = location / interval;

            _index = Convert.ToInt16(Math.Floor(yPoint));

            NewText(CurrentLines, false);

            PositionBar.Location = new Point(x, y);
        }

        private void UpdatePositionBar()
        {
            //return;

            if (CurrentLines.Count <= MaximumLines) return;

            int interval = 108 / (CurrentLines.Count - MaximumLines);

            int x = 417;
            int y = 48 + (_index * interval);

            if (y >= 155) y = 155;
            if (y <= 47) y = 47;

            PositionBar.Location = new Point(x, y);
        }

        private void ButtonClicked(string action)
        {
            
            if (action == "@Exit")
            {
                Hide();
                return;
            }

            if (CMain.Time <= ExineMainScene.NPCTime) return;

            ExineMainScene.NPCTime = CMain.Time + 5000;
            Network.Enqueue(new C.CallNPC { ObjectID = ExineMainScene.NPCID, Key = $"[{action}]" });
        }

        public void UpdatePortrait()
        {
            NPCObject npc = (NPCObject)MapControl.GetObject(ExineMainScene.NPCID);
            PortraitImage.Index = npc.Image;
            //PortraitImage.Location = new Point(13-npc.OffSetMove.X,28 - npc.OffSetMove.Y);// Location = new Point(13, 28),
            PortraitImage.Location = new Point(13, 28);
        }

        public void NewText(List<string> lines, bool resetIndex = true)
        {
            
            Size = TrueSize;

            if (resetIndex)
            {
                _index = 0;
                CurrentLines = lines;
                UpdatePositionBar();
                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    string currentLine = lines[i];

                    List<Match> bigButtonSortedList = B.Matches(currentLine).Cast<Match>().OrderBy(o => o.Index).ToList();

                    for (int j = 0; j < bigButtonSortedList.Count; j++)
                    {
                        Match match = bigButtonSortedList[j];
                        Capture capture = match.Groups[1].Captures[0];
                        string txt = match.Groups[2].Captures[0].Value;
                        string action = match.Groups[3].Captures[0].Value;
                        string colourString = "RoyalBlue";

                        string[] actionSplit = action.Split('/');

                        action = actionSplit[0];
                        if (actionSplit.Length > 1)
                            colourString = actionSplit[1];

                        Color color = Color.FromName(colourString);

                        BigButton button = new BigButton
                        {
                            Index = 841,
                            HoverIndex = 842,
                            PressedIndex = 843,
                            Library = Libraries.Title,
                            Sound = SoundList.ButtonA,
                            Text = txt,
                            FontColour = Color.White,
                            ForeColour = color
                        };

                        button.Click += (o, e) =>
                        {
                            ButtonClicked(action);
                        };
                        BigButtons.Insert(0, button);
                    }

                    var bigButtonString = B.ToString();

                    var oldCurrentLine = currentLine;

                    currentLine = Regex.Replace(currentLine, bigButtonString, "");

                    if (string.IsNullOrWhiteSpace(currentLine) && oldCurrentLine != currentLine)
                        lines.RemoveAt(i);
                }

                if (BigButtons.Count > 0)
                {
                    int minimumButtons = 0;
                    if (string.IsNullOrWhiteSpace(string.Concat(lines)))
                    {
                        BigButtonDialog.Location = new Point(1, 27);
                        minimumButtons = 4;
                    }
                    else
                        BigButtonDialog.Location = new Point(1, Size.Height - 33);

                    BigButtonDialog.Show(BigButtons, minimumButtons);
                    Size = new Size(Size.Width, BigButtonDialog.Location.Y + BigButtonDialog.Size.Height);
                }
            }                
            
            if (lines.Count > MaximumLines)
            {
                //Index = 385; //k333123 remove
                UpButton.Visible = true;
                DownButton.Visible = true;
                PositionBar.Visible = true;
            }
            else
            {
                //Index = 384; //k333123 remove
                UpButton.Visible = false;
                DownButton.Visible = false;
                PositionBar.Visible = false;                
            }         

            QuestButton.Location = new Point(172, Size.Height - 30);

            for (int i = 0; i < TextButtons.Count; i++)
                TextButtons[i].Dispose();

            for (int i = 0; i < TextLabel.Length; i++)
            {
                if (TextLabel[i] != null) TextLabel[i].Text = "";
            }

            TextButtons.Clear();

            int lastLine = lines.Count > MaximumLines ? ((MaximumLines + _index) > lines.Count ? lines.Count : (MaximumLines + _index)) : lines.Count;

            for (int i = _index; i < lastLine; i++)
            {
                TextLabel[i] = new ExineLabel
                {
                    Font = font,
                    DrawFormat = TextFormatFlags.WordBreak,
                    Visible = true,
                    Parent = this,
                    //Size = new Size(420, 20),
                    //Location = new Point(8, 34 + (i - _index) * 18),
                    Size = new Size(330, 20),
                    Location = new Point(8 + 25, 34 + (i - _index) * 18 + 210),
                    NotControl = true
                };

                if (i >= lines.Count)
                {
                    TextLabel[i].Text = string.Empty;
                    continue;
                }

                string currentLine = lines[i];

                List<Match> matchList = R.Matches(currentLine).Cast<Match>().ToList();
                matchList.AddRange(C.Matches(currentLine).Cast<Match>());
                matchList.AddRange(L.Matches(currentLine).Cast<Match>());

                int oldLength = currentLine.Length;

                foreach (Match match in matchList.OrderBy(o => o.Index).ToList())
                {
                    int offSet = oldLength - currentLine.Length;

                    Capture capture = match.Groups[1].Captures[0];
                    string txt = match.Groups[2].Captures[0].Value;
                    string action = match.Groups[3].Captures[0].Value;

                    currentLine = currentLine.Remove(capture.Index - 1 - offSet, capture.Length + 2).Insert(capture.Index - 1 - offSet, txt);
                    string text = currentLine.Substring(0, capture.Index - 1 - offSet) + " ";
                    Size size = TextRenderer.MeasureText(CMain.Graphics, text, TextLabel[i].Font, TextLabel[i].Size, TextFormatFlags.TextBoxControl);
                     
                    if (R.Match(match.Value).Success)
                        NewButton(txt, action, TextLabel[i].Location.Add(new Point(size.Width - 10, 0))); 

                    if (C.Match(match.Value).Success)
                        NewColour(txt, action, TextLabel[i].Location.Add(new Point(size.Width - 10, 0))); 

                    if (L.Match(match.Value).Success)
                        NewButton(txt, null, TextLabel[i].Location.Add(new Point(size.Width - 10, 0)), action); 
                     
                }

                TextLabel[i].Text = currentLine;
                TextLabel[i].MouseWheel += NPCDialog_MouseWheel;
            }
        }

        private void NewButton(string text, string key, Point p, string link = "")
        {
            ExineLabel temp = new ExineLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = this,
                Location = p,
                Text = text,
                //ForeColour = Color.Yellow,
                ForeColour = Color.LimeGreen,
                Sound = SoundList.ButtonC,
                Font = font
            };

            temp.Location = new Point(temp.Location.X, temp.Location.Y + 95);//k333123
            temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
            temp.MouseLeave += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseDown += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

            if (!string.IsNullOrEmpty(link))
            {
                temp.Click += (o, e) =>
                {
                    if (link.StartsWith("http://", true, CultureInfo.InvariantCulture))
                    {
                        System.Diagnostics.Process.Start(new ProcessStartInfo
                        {
                            FileName = link,
                            UseShellExecute = true
                        });
                    }
                };
            }
            else
            {
                temp.Click += (o, e) =>
                {
                    ButtonClicked(key);
                };
            }

            temp.MouseWheel += NPCDialog_MouseWheel;

            TextButtons.Add(temp);
        }

        private void NewColour(string text, string colour, Point p)
        {
            Color textColour = Color.FromName(colour);

            ExineLabel temp = new ExineLabel
            {
                AutoSize = true,
                Visible = true,
                Parent = this,
                Location = p,
                Text = text,
                ForeColour = textColour,
                Font = font
            };
            temp.MouseWheel += NPCDialog_MouseWheel;

            TextButtons.Add(temp);
        }

        public void CheckQuestButtonDisplay()
        { 
            NameLabel.Text = string.Empty;
            JobNameLabel.Text = String.Empty;

            QuestButton.Visible = false;

            NPCObject npc = (NPCObject)MapControl.GetObject(ExineMainScene.NPCID);
            if (npc != null)
            {
                
                string[] nameSplit = npc.Name.Split('_');
                JobNameLabel.Text = nameSplit[0];
                NameLabel.Text = nameSplit[1];

                if (npc.GetAvailableQuests().Any())
                    QuestButton.Visible = true;
            }
        }

        public override void Hide()
        {
            Visible = false;
            ExineMainScene.Scene.NPCGoodsDialog.Hide();
            ExineMainScene.Scene.NPCSubGoodsDialog.Hide();
            ExineMainScene.Scene.NPCCraftGoodsDialog.Hide();
            ExineMainScene.Scene.NPCDropDialog.Hide(); 
            ExineMainScene.Scene.RefineDialog.Hide();
            ExineMainScene.Scene.StorageDialog.Hide();
             
            ExineMainScene.Scene.QuestListDialog.Hide();
            ExineMainScene.Scene.ExInventoryDialog.Location = new Point(0, 0);
            //ExineMainScene.Scene.RollControl.Hide();
            BigButtonDialog.Hide();
        }

        public override void Show()
        {
            ExineMainScene.Scene.ExInventoryDialog.Location = new Point(Size.Width + 5, 0);
            Visible = true;

            CheckQuestButtonDisplay();
            UpdatePortrait();
        }
    }
    public sealed class NPCGoodsDialog : ExineImageControl
    {
        public PanelType PType;
        public bool UsePearls;

        public int StartIndex;
        public UserItem SelectedItem;

        public List<UserItem> Goods = new List<UserItem>();
        public List<UserItem> DisplayGoods = new List<UserItem>();
        public ExineGoodsCell[] Cells;
        public ExineButton BuyButton, CloseButton;
        //public ExineImageControl BuyLabel;

        public ExineButton UpButton, DownButton, PositionBar;

        public const int goodsLineCount = 6;//add k333123 241005

        public ExineImageControl SelectedItemImage;
        public ExineLabel SelectedItemNameLabel, SelectedItemInfoLabel, SelectedItemPriceLabel;

        public NPCGoodsDialog(PanelType type)
        {
            
            PType = type;

            //물건 목록
            //Index = 1000;
            //Library = Libraries.Prguse;
            
            Index = 0;
            Library = Libraries.PANEL0501;

            Location = new Point(0 + 112, -471);
            //Cells = new ExineGoodsCell[8];
            Cells = new ExineGoodsCell[goodsLineCount];
            //Sort = true;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new ExineGoodsCell
                {
                    Parent = this,
                    //Location = new Point(10, 34 + i * 33 ),
                    Location = new Point(10+47, 34 + i * 33 +186+15),
                    Sound = SoundList.ButtonC
                };
                Cells[i].Click += (o, e) =>
                {
                    SelectedItem = ((ExineGoodsCell)o).Item;

                    //여기서 위쪽 정보창쪽에 표시할 수 있어야함.
                    SelectedItemImage.Index = SelectedItem.Image;
                    SelectedItemNameLabel.Text = SelectedItem.FriendlyName;
                    
                    SelectedItemPriceLabel.Text = SelectedItem.Info.Price.ToString();

                    Update();

                    if (PType == PanelType.Craft)
                    {
                        ExineMainScene.Scene.CraftDialog.ResetCells();
                        ExineMainScene.Scene.CraftDialog.RefreshCraftCells(SelectedItem);

                        if (!ExineMainScene.Scene.CraftDialog.Visible)
                        {
                            ExineMainScene.Scene.CraftDialog.Show();
                        }
                    }
                };
                Cells[i].MouseWheel += NPCGoodsPanel_MouseWheel;
                Cells[i].DoubleClick += (o, e) =>
                {
                    if (PType == PanelType.Craft) return;

                    BuyItem();
                };
            }
             
            SelectedItemNameLabel = new ExineLabel
            {
                Text="선택 아이템 이름부분",
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(44+87, 0+44),
            };

            SelectedItemInfoLabel = new ExineLabel
            {
                Text = "선택 아이템 정보부분",
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(44+91, 30+36),
            };

            SelectedItemPriceLabel = new ExineLabel
            {
                Text = "선택 아이템 가격부분",
                AutoSize = true,
                Parent = this,
                NotControl = true,
                Location = new Point(44+277, 30+10),
            };


            SelectedItemImage = new ExineImageControl
            {
                Index = 1,
                Library = Libraries.StateItems,
                Parent = this,
                Location = new Point(44-45, 9 + 50-31),
                UseOffSet = true,
            };

            CloseButton = new ExineButton
            {
                HoverIndex = 106,
                //Index = 360,
                Index = 105,
                Location = new Point(217+47+29+49, 3+186+247+5),
                //Library = Libraries.Prguse2,
                Library = Libraries.PANEL0501,
                Parent = this,
                //PressedIndex = 362,
                PressedIndex = 107,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            BuyButton = new ExineButton
            {
                //HoverIndex = 313,
                HoverIndex = 102,
                //Index = 312,
                Index = 101,
                Location = new Point(77+47-170, 304+186+51),
                //Library = Libraries.Title,
                Library = Libraries.PANEL0501,
                Parent = this,
                //PressedIndex = 314,
                PressedIndex = 103,
                Sound = SoundList.ButtonA,
            };
            BuyButton.Click += (o, e) => BuyItem();

            /*
            BuyLabel = new ExineImageControl
            {
                Index = 27,
                Library = Libraries.Title,
                Parent = this,
                Location = new Point(20+47, 9+186),
            };
            */

            if (PType == PanelType.Craft)
            {
                //BuyLabel.Index = 12;
                BuyButton.Visible = false;
            }

            UpButton = new ExineButton
            {
                Index = 197,
                HoverIndex = 198,
                Library = Libraries.Prguse2,
                Location = new Point(219+47, 35+186),
                Parent = this,
                PressedIndex = 199,
                Sound = SoundList.ButtonA
            };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };

            DownButton = new ExineButton
            {
                Index = 207,
                HoverIndex = 208,
                Library = Libraries.Prguse2,
                Location = new Point(219+47, 284+186),
                Parent = this,
                PressedIndex = 209,
                Sound = SoundList.ButtonA
            };
            DownButton.Click += (o, e) =>
            {
                //if (DisplayGoods.Count <= 8) return;
                if (DisplayGoods.Count <= goodsLineCount) return;

                //if (StartIndex == DisplayGoods.Count - 8) return;
                if (StartIndex == DisplayGoods.Count - goodsLineCount) return;
                StartIndex++;
                Update();
            };

            PositionBar = new ExineButton
            {
                Index = 205,
                HoverIndex = 206,
                Library = Libraries.Prguse2,
                Location = new Point(219+47, 49+186),
                Parent = this,
                PressedIndex = 206,
                Movable = true,
                Sound = SoundList.None
            };
            PositionBar.OnMoving += PositionBar_OnMoving;
            PositionBar.MouseUp += (o, e) => Update();
        }

        private bool CheckSubGoods()
        {
            if (SelectedItem == null) return false;

            if (PType == PanelType.Buy && !UsePearls)
            {
                var list = Goods.Where(x => x.Info.Index == SelectedItem.Info.Index).ToList();

                if (list.Count > 1 || ExineMainScene.Scene.NPCSubGoodsDialog.Visible)
                {
                    ExineMainScene.Scene.NPCSubGoodsDialog.NewGoods(list);
                    ExineMainScene.Scene.NPCSubGoodsDialog.Show();
                    return true;
                }
            }

            return false;
        }

        private void BuyItem()
        {
            if (SelectedItem == null) return;

            if (CheckSubGoods())
            {
                return;
            }

            if (SelectedItem.Info.StackSize > 1)
            {
                ushort tempCount = SelectedItem.Count;
                ushort maxQuantity = SelectedItem.Info.StackSize;

                SelectedItem.Count = maxQuantity;

                if (UsePearls)
                {
                    if (SelectedItem.Price() > ExineMainScene.User.PearlCount)
                    {
                        maxQuantity = Math.Min(ushort.MaxValue, (ushort)(ExineMainScene.Gold / (SelectedItem.Price() / SelectedItem.Count)));
                        if (maxQuantity == 0)
                        {
                            ExineMainScene.Scene.ExChatDialog.ReceiveChat("진주가 충분하지 않습니다.", ChatType.System);
                            return;
                        }
                    }
                }

                else if (SelectedItem.Price() > ExineMainScene.Gold)
                {
                    maxQuantity = Math.Min(ushort.MaxValue, (ushort)(ExineMainScene.Gold / (SelectedItem.Price() / SelectedItem.Count)));
                    if (maxQuantity == 0)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                        return;
                    }
                }

                MapObject.User.GetMaxGain(SelectedItem);

                if (SelectedItem.Count > tempCount)
                {
                    SelectedItem.Count = tempCount;
                }

                if (SelectedItem.Count == 0) return;

                MirAmountBox amountBox = new MirAmountBox("Purchase Amount:", SelectedItem.Image, maxQuantity, 0, SelectedItem.Count);

                amountBox.OKButton.Click += (o, e) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        Network.Enqueue(new C.BuyItem { ItemIndex = SelectedItem.UniqueID, Count = (ushort)amountBox.Amount, Type = PanelType.Buy });
                    }
                };

                amountBox.Show();
            }
            else
            {
                if (SelectedItem.Info.Price > ExineMainScene.Gold)
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    return;
                }

                for (int i = 0; i < MapObject.User.Inventory.Length; i++)
                {
                    if (MapObject.User.Inventory[i] == null) break;
                    if (i == MapObject.User.Inventory.Length - 1)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("더 많은 아이템을 구매할 수 없습니다.", ChatType.System);
                        return;
                    }
                }

                Network.Enqueue(new C.BuyItem { ItemIndex = SelectedItem.UniqueID, Count = 1, Type = PanelType.Buy });
            }
        }

        private void NPCGoodsPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == DisplayGoods.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void Update()
        {
            //if (StartIndex > DisplayGoods.Count - 8) StartIndex = DisplayGoods.Count - 8;
            if (StartIndex > DisplayGoods.Count - goodsLineCount) StartIndex = DisplayGoods.Count - goodsLineCount;
            if (StartIndex <= 0) StartIndex = 0;

            //if (DisplayGoods.Count > 8)
            if (DisplayGoods.Count > goodsLineCount)
            {
                PositionBar.Visible = true;
                int h = 233 - PositionBar.Size.Height;
                //h = (int)((h / (float)(DisplayGoods.Count - 8)) * StartIndex);
                h = (int)((h / (float)(DisplayGoods.Count - goodsLineCount)) * StartIndex);
                PositionBar.Location = new Point(219+47, 49 + h+186);
            }
            else
                PositionBar.Visible = false;


            //for (int i = 0; i < 8; i++)
            for (int i = 0; i < goodsLineCount; i++)
            {
                if (i + StartIndex >= DisplayGoods.Count)
                {
                    Cells[i].Visible = false;
                    continue;
                }
                Cells[i].Visible = true;

                var matchingGoods = Goods.Where(x => x.Info.Index == Cells[i].Item.Info.Index);

                Cells[i].Item = DisplayGoods[i + StartIndex];
                Cells[i].MultipleAvailable = matchingGoods.Count() > 1 && matchingGoods.Any(x => x.IsShopItem == false);
                Cells[i].Border = SelectedItem != null && Cells[i].Item == SelectedItem;
                Cells[i].UsePearls = UsePearls;
            }
        }

        private void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            const int x = 219;
            int y = PositionBar.Location.Y;
            if (y >= 282 - PositionBar.Size.Height) y = 282 - PositionBar.Size.Height;
            if (y < 49) y = 49;

            int h = 233 - PositionBar.Size.Height;
            //h = (int)Math.Round(((y - 49) / (h / (float)(DisplayGoods.Count - 8))));
            h = (int)Math.Round(((y - 49) / (h / (float)(DisplayGoods.Count - goodsLineCount))));

            PositionBar.Location = new Point(x+47, y+186);

            if (h == StartIndex) return;
            StartIndex = h;
            Update();
        }

        public void NewGoods(IEnumerable<UserItem> list)
        {
            Goods.Clear();
            DisplayGoods.Clear();

            if (PType == PanelType.BuySub)
            {
                StartIndex = 0;
                SelectedItem = null;

                list = list.OrderBy(x => x.Price());
            }

            foreach (UserItem item in list)
            {
                //Normal shops just want to show one of each item type
                if (PType == PanelType.Buy && !UsePearls)
                {
                    Goods.Add(item);

                    if (DisplayGoods.Any(x => x.Info.Index == item.Info.Index)) continue;
                }

                DisplayGoods.Add(item);
            }

            if (ExineMainScene.Scene.NPCSubGoodsDialog.Visible)
            {
                CheckSubGoods();
            }

            Update();
        }

        public override void Hide()
        {
            Visible = false;

            if (ExineMainScene.Scene.CraftDialog.Visible)
            {
                ExineMainScene.Scene.CraftDialog.Hide();
            }

            //add 241005
            if(ExineMainScene.Scene.ExCharacterDialog!=null && ExineMainScene.Scene.ExCharacterDialog.Visible)
            {
                ExineMainScene.Scene.ExCharacterDialog.Hide();
            }
            if(ExineMainScene.Scene.ExInventoryDialog != null && ExineMainScene.Scene.ExInventoryDialog.Visible)
            {
                ExineMainScene.Scene.ExInventoryDialog.Hide();
            }
            //ExineMainScene.Scene.ExCharacterDialog.Hide(); 
            //ExineMainScene.Scene.ExInventoryDialog.Hide();//k33123 add
        }
    

        public override void Show()
        {
            //add 241005
            ExineMainScene.Scene.ExCharacterDialog.Show();
            ExineMainScene.Scene.ExCharacterDialog.ShowCharacterPage();
            ExineMainScene.Scene.ExInventoryDialog.Location = new Point(541, 378);//k333123
            ExineMainScene.Scene.ExInventoryDialog.Show();//k33123 add

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i].Recipe = PType == PanelType.Craft;
            }

            //Location = new Point(Location.X, ExineMainScene.Scene.ExNPCDialog.Size.Height); //포지션 동적 변경되는 원인이였음.
            Location = new Point(Location.X+3, ExineMainScene.Scene.ExNPCDialog.Size.Height-400+13); //포지션 동적 변경되는 원인이였음.
            Visible = true;


            //ExineMainScene.Scene.ExInventoryDialog.Show();

          
        }
    }
    public sealed class NPCDropDialog : ExineImageControl
    {

        public readonly ExineButton ConfirmButton, HoldButton;
        public readonly MirItemCell ItemCell;
        public MirItemCell OldCell;
        public readonly ExineLabel InfoLabel;
        public PanelType PType;

        public static UserItem TargetItem;
        public bool Hold;


        public NPCDropDialog()
        {
            
            //올려두는 원형쟁반 모양
            Index = 392;
            Library = Libraries.Prguse;
            //Location = new Point(264, 224);
            Location = new Point(264+50+125, 224-169);
            Sort = true;

            Click += NPCDropPanel_Click;

            HoldButton = new ExineButton
            {
                HoverIndex = 294,
                Index = 293,
                Location = new Point(114, 36),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 295,
                Sound = SoundList.ButtonA,
            };
            HoldButton.Click += (o, e) => Hold = !Hold;

            ConfirmButton = new ExineButton
            {
                HoverIndex = 291,
                Index = 290,
                Location = new Point(114, 62),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 292,
                Sound = SoundList.ButtonA,
            };
            ConfirmButton.Click += (o, e) => Confirm();

            InfoLabel = new ExineLabel
            {
                AutoSize = true,
                Location = new Point(30, 10),
                Parent = this,
                NotControl = true,
            };

            ItemCell = new MirItemCell
            {
                BorderColour = Color.Lime,
                GridType = MirGridType.DropPanel,
                Library = Libraries.Items,
                Parent = this,
                Location = new Point(38, 72),
            };
            ItemCell.Click += (o, e) => ItemCell_Click();

            BeforeDraw += NPCDropPanel_BeforeDraw;
            AfterDraw += NPCDropPanel_AfterDraw;
        }

        private void NPCDropPanel_AfterDraw(object sender, EventArgs e)
        {
            if (Hold)
                Libraries.Title.Draw(295, 114 + DisplayLocation.X, 36 + DisplayLocation.Y);
        }

        private void NPCDropPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;

            if (me == null) return;
            int x = me.X - DisplayLocation.X;
            int y = me.Y - DisplayLocation.Y;

            if (new Rectangle(20, 55, 75, 75).Contains(x, y))
                ItemCell_Click();
        }

        private void Confirm()
        {
            if (TargetItem == null) return;

            switch (PType)
            {
                case PanelType.Sell:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontSell))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("이 아이템을 판매할 수 없습니다.", ChatType.System);
                        return;
                    }
                    if (ExineMainScene.Gold + TargetItem.Price() / 2 <= uint.MaxValue)
                    {
                        Network.Enqueue(new C.SellItem { UniqueID = TargetItem.UniqueID, Count = TargetItem.Count });
                        TargetItem = null;
                        return;
                    }
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("더 이상 데니를 휴대할 수 없습니다.", ChatType.System);
                    break;
                case PanelType.Repair:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontRepair))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("이 아이템은 수리할 수 없습니다.", ChatType.System);
                        return;
                    }
                    if (ExineMainScene.Gold >= TargetItem.RepairPrice() * ExineMainScene.NPCRate)
                    {
                        Network.Enqueue(new C.RepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    break;
                case PanelType.SpecialRepair:
                    if ((TargetItem.Info.Bind.HasFlag(BindMode.DontRepair)) || (TargetItem.Info.Bind.HasFlag(BindMode.NoSRepair)))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("이 아이템은 수리할 수 없습니다.", ChatType.System);
                        return;
                    }
                    if (ExineMainScene.Gold >= (TargetItem.RepairPrice() * 3) * ExineMainScene.NPCRate)
                    {
                        Network.Enqueue(new C.SRepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(GameLanguage.LowGold, ChatType.System);
                    break;
                case PanelType.Consign:
                    if (TargetItem.Info.Bind.HasFlag(BindMode.DontStore) || TargetItem.Info.Bind.HasFlag(BindMode.DontSell))
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat("이 아이템은 위탁할 수 없습니다.", ChatType.System);
                        return;
                    }
                    MirAmountBox box = new MirAmountBox("위탁판매 가격:", TargetItem.Image, Globals.MaxConsignment, Globals.MinConsignment)
                    {
                        InputTextBox = { Text = string.Empty },
                        Amount = 0
                    };

                    box.Show();
                    box.OKButton.Click += (o, e) =>
                    {
                        Network.Enqueue(new C.ConsignItem { UniqueID = TargetItem.UniqueID, Price = box.Amount, Type = MarketPanelType.Consign });
                        TargetItem = null;
                    };
                    return;
                 
                case PanelType.Refine:

                    for (int i = 0; i < ExineMainScene.Scene.RefineDialog.Grid.Length; i++)
                    {
                        if (ExineMainScene.Scene.RefineDialog.Grid[i].Item != null)
                        {
                            if (ExineMainScene.Gold >= ((TargetItem.Info.RequiredAmount * 10) * ExineMainScene.NPCRate))
                            {
                                Network.Enqueue(new C.RefineItem { UniqueID = TargetItem.UniqueID });
                                TargetItem = null;
                                return;
                            }
                            ExineMainScene.Scene.ExChatDialog.ReceiveChat(String.Format("Y{0}을(를) 정제할 만큼의 데니가 없습니다.", TargetItem.FriendlyName), ChatType.System);
                            return;
                        }

                    }
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(String.Format("{0}을(를) 정제할 아이템을 제출하지 않았습니다.", TargetItem.FriendlyName), ChatType.System);
                    break;
                case PanelType.CheckRefine:

                    if (TargetItem.RefineAdded == 0)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(String.Format("{0}이(가) 정제되지 않았으므로 확인할 필요가 없습니다.", TargetItem.FriendlyName), ChatType.System);
                        return;
                    }
                    Network.Enqueue(new C.CheckRefine { UniqueID = TargetItem.UniqueID });
                    break;

                case PanelType.ReplaceWedRing:

                    if (TargetItem.Info.Type != ItemType.Ring)
                    {
                        ExineMainScene.Scene.ExChatDialog.ReceiveChat(String.Format("{0}은 반지가 아닙니다.", TargetItem.FriendlyName), ChatType.System);
                        return;
                    }

                    Network.Enqueue(new C.ReplaceWedRing { UniqueID = TargetItem.UniqueID });
                    break;
            }


            TargetItem = null;
            OldCell.Locked = false;
            OldCell = null;
        }

        private void ItemCell_Click()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }

          

            //////////////////////////////////////

            if (ExineMainScene.SelectedCell == null || ExineMainScene.SelectedCell.GridType != MirGridType.Inventory ||
                (PType != PanelType.Sell && PType != PanelType.Consign && ExineMainScene.SelectedCell.Item != null && ExineMainScene.SelectedCell.Item.Info.Durability == 0))
                return;
            /*
            if (GameScene.SelectedCell.Item != null && (GameScene.SelectedCell.Item.Info.StackSize > 1 && GameScene.SelectedCell.Item.Count > 1))
            {
                MirAmountBox amountBox = new MirAmountBox("Sell Amount:", GameScene.SelectedCell.Item.Image, GameScene.SelectedCell.Item.Count);

                amountBox.OKButton.Click += (o, a) =>
                {
                    TargetItem = GameScene.SelectedCell.Item.Clone();
                    TargetItem.Count = amountBox.Amount;

                    OldCell = GameScene.SelectedCell;
                    OldCell.Locked = true;
                    GameScene.SelectedCell = null;
                    if (Hold) Confirm();
                };

                amountBox.Show();
            }
            else
            {
                TargetItem = GameScene.SelectedCell.Item;
                OldCell = GameScene.SelectedCell;
                OldCell.Locked = true;
                GameScene.SelectedCell = null;
                if (Hold) Confirm();
            }
            */
            TargetItem = ExineMainScene.SelectedCell.Item;
            OldCell = ExineMainScene.SelectedCell;
            OldCell.Locked = true;
            ExineMainScene.SelectedCell = null;
            if (Hold) Confirm();
        }

        private void NPCDropPanel_BeforeDraw(object sender, EventArgs e)
        {
            string text;

            HoldButton.Visible = true;

            Index = 351;
            Library = Libraries.Prguse2;
            Location = new Point(264+125, ExineMainScene.Scene.ExNPCDialog.Size.Height-172);//k333123 mod 241005

            ConfirmButton.HoverIndex = 291;
            ConfirmButton.Index = 290;
            ConfirmButton.PressedIndex = 292;
            ConfirmButton.Location = new Point(114, 62);

            InfoLabel.Location = new Point(30, 10);

            ItemCell.Location = new Point(38, 72);

            switch (PType)
            {
                case PanelType.Sell:
                    text = "Sale: ";
                    break;
                case PanelType.Repair:
                    text = "Repair: ";
                    break;
                case PanelType.SpecialRepair:
                    text = "S. Repair: ";
                    break;
                case PanelType.Consign:
                    InfoLabel.Text = "Consignment: ";
                    return;
                
                
                case PanelType.Refine:
                    text = "Refine: ";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    ExineMainScene.Scene.RefineDialog.Show();
                    break;
                case PanelType.CheckRefine:
                    text = "Check Refine";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    break;
                case PanelType.ReplaceWedRing:
                    text = "Replace: ";
                    HoldButton.Visible = false;
                    ConfirmButton.Visible = true;
                    break;

                default: return;

            }

            if (TargetItem != null)
            {

                switch (PType)
                {
                    case PanelType.Sell:
                        text += (TargetItem.Price() / 2).ToString();
                        break;
                    case PanelType.Repair:
                        text += (TargetItem.RepairPrice() * ExineMainScene.NPCRate).ToString();
                        break;
                    case PanelType.SpecialRepair:
                        text += ((TargetItem.RepairPrice() * 3) * ExineMainScene.NPCRate).ToString();
                        break;
                   
                     
                    case PanelType.Refine:
                        text += ((TargetItem.Info.RequiredAmount * 10) * ExineMainScene.NPCRate).ToString();
                        break;
                    case PanelType.ReplaceWedRing:
                        text += ((TargetItem.Info.RequiredAmount * 10) * ExineMainScene.NPCRate).ToString();
                        break;
                    default: return;
                }

                text += " Gold";
            }

            InfoLabel.Text = text;
        }

        public override void Hide()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }
            Visible = false;
        }
        public override void Show()
        {
            Hold = false;
            ExineMainScene.Scene.ExInventoryDialog.Show();
            Visible = true;
        }
    }
    
    public sealed class CraftDialog : ExineImageControl
    {
        public UserItem RecipeItem;
        public ClientRecipeInfo Recipe;

        private const int _toolCount = 3;
        private const int _ingredientCount = 6;
        private static int _totalCount { get { return _toolCount + _ingredientCount; } }

        public static UserItem[] Slots = new UserItem[_totalCount];
        public static UserItem[] ShadowItems = new UserItem[_totalCount];

        public Dictionary<MirItemCell, ulong> Selected = new Dictionary<MirItemCell, ulong>();

        public MirItemCell[] Grid;

        public ExineButton CraftButton, AutoFillButton, CloseButton;

        public ExineLabel RecipeLabel;
        public ExineLabel PossibilityLabel;
        public ExineLabel GoldLabel;

        public CraftDialog()
        {
            Index = 1109;
            Library = Libraries.Prguse;
            Location = new Point(0, 0);
            Sort = true;
            BeforeDraw += CraftDialog_BeforeDraw;
            Movable = true;

            RecipeLabel = new ExineLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(22, 5),
                Font = new Font(Settings.FontName, 10F, FontStyle.Bold),
                ForeColour = Color.BurlyWood,
                Visible = true,
                NotControl = true
            };

            PossibilityLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Parent = this,
                Location = new Point(10, 135),
                Font = new Font(Settings.FontName, 8F),
                Visible = true,
                NotControl = true
            };

            GoldLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Parent = this,
                Location = new Point(30, 190),
                Font = new Font(Settings.FontName, 8F),
                Visible = true,
                NotControl = true
            };

            Grid = new MirItemCell[_totalCount];
            for (int x = 0; x < _totalCount; x++)
            {
                if (x >= _toolCount)
                {
                    Grid[x] = new MirItemCell
                    {
                        ItemSlot = x,
                        GridType = MirGridType.Craft,
                        Library = Libraries.Items,
                        Parent = this,
                        Size = new Size(35, 32),
                        Location = new Point(((x - _toolCount) * 40) + 52, 86),
                        Border = true,
                        BorderColour = Color.Lime
                    };
                }
                else
                {
                    Grid[x] = new MirItemCell
                    {
                        ItemSlot = x,
                        GridType = MirGridType.Craft,
                        Library = Libraries.Items,
                        Parent = this,
                        Size = new Size(35, 32),
                        Location = new Point((x * 44) + 108, 44),
                        Border = true,
                        BorderColour = Color.Lime
                    };
                }

                Grid[x].Click += Grid_Click;
            }

            CloseButton = new ExineButton
            {
                Index = 360,
                HoverIndex = 361,
                PressedIndex = 362,
                Location = new Point(312, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            AutoFillButton = new ExineButton
            {
                HoverIndex = 181,
                Index = 180,
                Location = new Point(165, 185),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 182,
                Sound = SoundList.ButtonA
            };
            AutoFillButton.Click += (o, e) => AutoFill();

            CraftButton = new ExineButton
            {
                HoverIndex = 337,
                Index = 336,
                Location = new Point(215, 185),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 338,
                Sound = SoundList.ButtonA,
                GrayScale = true,
                Enabled = false
            };
            CraftButton.Click += (o, e) => CraftItem();
        }

        void CraftDialog_BeforeDraw(object sender, EventArgs e)
        {
            if (!ExineMainScene.Scene.ExInventoryDialog.Visible)
            {
                Hide();
                return;
            }
        }

        private void Grid_Click(object sender, EventArgs e)
        {
            MirItemCell cell = (MirItemCell)sender;

            if (cell == null || cell.ShadowItem == null)
                return;

            if (ExineMainScene.SelectedCell == null || ExineMainScene.SelectedCell.GridType != MirGridType.Inventory || ExineMainScene.SelectedCell.Locked)
                return;

            if (ExineMainScene.SelectedCell.Item.Info != cell.ShadowItem.Info || cell.Item != null)
                return;

            if (cell.ItemSlot >= _toolCount)
            {
                if (ExineMainScene.SelectedCell.Item.Count < cell.ShadowItem.Count)
                    return;

                if (cell.ShadowItem.CurrentDura < cell.ShadowItem.MaxDura && ExineMainScene.SelectedCell.Item.CurrentDura < cell.ShadowItem.CurrentDura)
                    return;
            }
            else
            {
                if (ExineMainScene.SelectedCell.Item.CurrentDura < 1000M)
                    return;
            }

            cell.Item = ExineMainScene.SelectedCell.Item;

            Selected.Add(ExineMainScene.SelectedCell, ExineMainScene.SelectedCell.Item.UniqueID);
            ExineMainScene.SelectedCell.Locked = true;
            ExineMainScene.SelectedCell = null;

            RefreshCraftCells(RecipeItem);
        }

        public override void Hide()
        {
            if (!Visible) return;

            Visible = false;

            ResetCells();
        }

        public override void Show()
        {
            Visible = true;

            Location = new Point(ExineMainScene.Scene.ExInventoryDialog.Location.X - 12, ExineMainScene.Scene.ExInventoryDialog.Location.Y + 236);
        }

        private void AutoFill()
        {
            ResetCells(false);

            if (RecipeItem == null) return;

            List<int> usedSlots = new List<int>();

            int j = 0;
            foreach (var tool in Recipe.Tools)
            {
                for (int i = 0; i < UserObject.User.Inventory.Length; i++)
                {
                    if (usedSlots.Contains(i)) continue;

                    var slot = UserObject.User.Inventory[i];

                    if (slot == null || tool.Info.Index != slot.Info.Index || slot.CurrentDura < 1000M) continue;

                    var cell = ExineMainScene.Scene.ExInventoryDialog.GetCell(slot.UniqueID) ?? ExineMainScene.Scene.BeltDialog.GetCell(slot.UniqueID);

                    if (cell.Locked) continue;

                    Selected.Add(cell, cell.Item.UniqueID);
                    cell.Locked = true;

                    Grid[j].Item = cell.Item;
                    break;
                }

                j++;
            }

            j = 3;
            foreach (var ingredient in Recipe.Ingredients)
            {
                for (int i = 0; i < UserObject.User.Inventory.Length; i++)
                {
                    if (usedSlots.Contains(i)) continue;

                    var slot = UserObject.User.Inventory[i];

                    if (slot == null || ingredient.Info.Index != slot.Info.Index) continue;
                    if (slot.Count < ingredient.Count) continue;
                    if (ingredient.CurrentDura < ingredient.MaxDura && slot.CurrentDura < ingredient.CurrentDura) continue;

                    var cell = ExineMainScene.Scene.ExInventoryDialog.GetCell(slot.UniqueID) ?? ExineMainScene.Scene.BeltDialog.GetCell(slot.UniqueID);

                    if (cell.Locked) continue;

                    Selected.Add(cell, cell.Item.UniqueID);
                    cell.Locked = true;

                    Grid[j].Item = cell.Item;
                    break;
                }

                j++;
            }

            RefreshCraftCells(RecipeItem);
        }

        private void CraftItem()
        {
            if (RecipeItem == null) return;

            if (Selected.Count < Recipe.Tools.Count + Recipe.Ingredients.Count) return;

            ushort max = 99;

            //Max quantity based on available ingredients/tools
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i] == null || Grid[i].Item == null) continue;

                ushort temp = 0;
                if (i >= _toolCount)
                {
                    temp = (ushort)(Grid[i].Item.Count / Grid[i].ShadowItem.Count);
                }
                else
                {
                    temp = (ushort)Math.Floor(Grid[i].Item.CurrentDura / 1000M);
                }

                if (temp < max) max = temp;
            }

            if (max > (RecipeItem.Info.StackSize / RecipeItem.Count))
            {
                max = (ushort)(RecipeItem.Info.StackSize / RecipeItem.Count);
            }

            //TODO - Check Max slots spare against slots to be used (stacksize/quantity)
            //TODO - GetMaxItemGain

            if (max == 1)
            {
                if (Recipe.Gold > ExineMainScene.Gold)
                {
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat("데니가 부족합니다.", ChatType.System);
                    return;
                }

            }

            if (max > 1)
            {
                MirAmountBox amountBox = new MirAmountBox("제작 수량:", RecipeItem.Info.Image, max, 0, max);

                amountBox.OKButton.Click += (o, e) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        if (!HasCraftItems((ushort)amountBox.Amount))
                        {
                            ExineMainScene.Scene.ExChatDialog.ReceiveChat("필요한 도구나 재료가 없습니다.", ChatType.System);
                            return;
                        }
                        
                        if ((Recipe.Gold * amountBox.Amount) > ExineMainScene.Gold)
                        {
                            ExineMainScene.Scene.ExChatDialog.ReceiveChat("데니가 부족합니다.", ChatType.System);
                            return;
                        }

                        Network.Enqueue(new C.CraftItem
                        {
                            UniqueID = RecipeItem.UniqueID,
                            Count = (ushort)amountBox.Amount,
                            Slots = Selected.Select(x => x.Key.ItemSlot).ToArray()
                        });
                    }
                };

                amountBox.Show();
            }
            else
            {
                Network.Enqueue(new C.CraftItem
                {
                    UniqueID = RecipeItem.UniqueID,
                    Count = 1,
                    Slots = Selected.Select(x => x.Key.ItemSlot).ToArray()
                });
            }
        }

        private bool HasCraftItems(ushort count)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].ShadowItem == null) continue;

                if (i >= _toolCount)
                {
                    if (Grid[i].Item == null || Grid[i].Item.Count < (Grid[i].ShadowItem.Count * count)) return false;
                }
                else
                {
                    if (Grid[i].Item == null || (uint)Math.Floor(Grid[i].Item.CurrentDura / 1000M) < count) return false;
                }
            }

            return true;
        }

        public void ResetCells(bool clearRecipe = true)
        {
            if (clearRecipe)
            {
                RecipeItem = null;
            }

            for (int j = 0; j < Grid.Length; j++)
            {
                Slots[j] = null;
                ShadowItems[j] = null;
            }

            foreach (var key in Selected.Keys)
            {
                key.Locked = false;
            }

            Selected.Clear();
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

        public void UpdateCraftCells()
        {
            List<MirItemCell> invalidCells = new List<MirItemCell>();

            foreach (var key in Selected.Keys)
            {
                MirItemCell cell = key;
                ulong oldItem = Selected[key];

                if (cell.Item == null || cell.Item.UniqueID != oldItem || (cell.Item.MaxDura > 1000M && cell.Item.CurrentDura < 1000M))
                {
                    MirItemCell gridCell = GetCell(oldItem);

                    if (gridCell != null)
                    {
                        gridCell.Item = null;
                    }
                    cell.Locked = false;

                    invalidCells.Add(key);
                }
            }

            foreach (var cell in invalidCells)
            {
                Selected.Remove(cell);
            }

            RefreshCraftCells(RecipeItem);
        }

        public void RefreshCraftCells(UserItem selectedItem)
        {
            RecipeItem = selectedItem;

            CraftButton.Enabled = true;
            CraftButton.GrayScale = false;

            Recipe = ExineMainScene.RecipeInfoList.SingleOrDefault(x => x.Item.ItemIndex == selectedItem.ItemIndex);

            RecipeLabel.Text = Recipe.Item.FriendlyName;
            PossibilityLabel.Text = (UserObject.User.Stats[Stat.CraftRatePercent] > 0 ? $"{Math.Min(100, Recipe.Chance + UserObject.User.Stats[Stat.CraftRatePercent])}% (+{UserObject.User.Stats[Stat.CraftRatePercent]}%)" : $"{Recipe.Chance}%") + " Chance of Success";
            GoldLabel.Text = Recipe.Gold.ToString("###,###,##0");

            for (int i = 0; i < Slots.Length; i++)
            {
                bool need;

                if (i >= _toolCount)
                {
                    if ((i - _toolCount) >= Recipe.Ingredients.Count) continue;

                    ShadowItems[i] = Recipe.Ingredients[i - _toolCount];
                    need = Grid[i].Item == null || Grid[i].Item.Count < Grid[i].ShadowItem.Count;
                }
                else
                {
                    if (i >= Recipe.Tools.Count) continue;

                    ShadowItems[i] = Recipe.Tools[i];
                    need = Grid[i].Item == null || Grid[i].Item.Count < Grid[i].ShadowItem.Count;
                }

                if (need)
                {
                    CraftButton.Enabled = false;
                    CraftButton.GrayScale = true;
                }
            }
        }
    }
    public sealed class RefineDialog : ExineImageControl
    {
        public MirItemCell[] Grid;
        public ExineButton RefineButton;

        public RefineDialog()
        {
            Index = 1002;
            Library = Libraries.Prguse;
            Location = new Point(0, 225);
            Sort = true;

            ExineImageControl TitleLabel = new ExineImageControl
            {
                Index = 18,
                Library = Libraries.Title,
                Location = new Point(28, 8),
                Parent = this
            };


            Grid = new MirItemCell[4 * 4];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    int idx = 4 * y + x;
                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,
                        GridType = MirGridType.Refine,
                        Library = Libraries.Items,
                        Parent = this,
                        Size = new Size(34, 32),
                        Location = new Point(x * 34 + 12 + x, y * 32 + 37 + y),
                    };
                }
            }
        }

        public override void Hide()
        {
            if (!Visible) return;

            Visible = false;
            RefineCancel();
        }

        public void RefineCancel()
        {
            Network.Enqueue(new C.RefineCancel());
        }

        public void RefineReset()
        {
            for (int i = 0; i < Grid.Length; i++)
                Grid[i].Item = null;
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
    public sealed class StorageDialog : ExineImageControl
    {
        public MirItemCell[] Grid;
        public ExineButton Storage1Button, Storage2Button, RentButton, ProtectButton, CloseButton;
        public ExineImageControl LockedPage;
        public ExineLabel RentalLabel;

        public StorageDialog()
        {
            Index = 586;
            Library = Libraries.Prguse;
            Location = new Point(0, 0);
            Sort = true;

            ExineImageControl TitleLabel = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.Title,
                Location = new Point(18, 8),
                Parent = this
            };

            LockedPage = new ExineImageControl
            {
                Index = 2443,
                Library = Libraries.Prguse,
                Location = new Point(8, 59),
                Parent = this,
                Visible = false
            };

            Storage1Button = new ExineButton
            {
                HoverIndex = 743,
                Index = 743,
                Location = new Point(8, 36),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 744,
                Sound = SoundList.ButtonA,
            };
            Storage1Button.Click += (o, e) =>
            {
                RefreshStorage1();
            };

            Storage2Button = new ExineButton
            {
                HoverIndex = 746,
                Index = 746,
                Location = new Point(80, 36),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 746,
                Sound = SoundList.ButtonA,
                Visible = true
            };
            Storage2Button.Click += (o, e) =>
            {
                RefreshStorage2();
            };
            RentButton = new ExineButton
            {
                Index = 483,
                HoverIndex = 484,
                PressedIndex = 485,
                Library = Libraries.Title,
                Location = new Point(283, 33),
                Parent = this,
                Sound = SoundList.ButtonA,
                Visible = true,
            };
            RentButton.Click += (o, e) =>
            {
                ExineMessageBox messageBox;

                if (ExineMainScene.User.HasExpandedStorage)
                    messageBox = new ExineMessageBox(GameLanguage.ExtendYourRentalPeriod, MirMessageBoxButtons.OKCancel);
                else
                    messageBox = new ExineMessageBox(GameLanguage.ExtraStorage, MirMessageBoxButtons.OKCancel);

                messageBox.OKButton.Click += (o1, a) =>
                {
                    Network.Enqueue(new C.Chat { Message = "@ADDSTORAGE" });
                };
                messageBox.Show();
            };

            ProtectButton = new ExineButton
            {
                HoverIndex = 114,
                Index = 113,
                Location = new Point(328, 33),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 115,
                Sound = SoundList.ButtonA,
                Visible = true
            };
            CloseButton = new ExineButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(363, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();

            RentalLabel = new ExineLabel
            {
                Parent = this,
                Location = new Point(40, 322),
                AutoSize = true,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                NotControl = true,
                Text = GameLanguage.ExpandedStorageLocked,
                ForeColour = Color.Red
            };

            Grid = new MirItemCell[10 * 16];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    int idx = 10 * y + x;

                    Grid[idx] = new MirItemCell
                    {
                        ItemSlot = idx,
                        GridType = MirGridType.Storage,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x * 36 + 9 + x, y % 8 * 32 + 60 + y % 8),
                    };

                    if (idx >= 80)
                        Grid[idx].Visible = false;
                }
            }
        }

        public override void Show()
        {
            ExineMainScene.Scene.ExInventoryDialog.Show();
            RefreshStorage1();

            Visible = true;
        }

        public void RefreshStorage1()
        {
            if (ExineMainScene.User == null) return;

            Storage1Button.Index = 743;
            Storage1Button.HoverIndex = 743;
            Storage2Button.Index = 746;
            Storage2Button.HoverIndex = 746;

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 80)
                    grid.Visible = true;
                else
                    grid.Visible = false;
            }

            RentButton.Visible = false;
            LockedPage.Visible = false;
            RentalLabel.Visible = false;
        }

        public void RefreshStorage2()
        {
            if (ExineMainScene.User == null) return;

            Storage1Button.Index = 744;
            Storage1Button.HoverIndex = 744;
            Storage2Button.Index = 745;
            Storage2Button.HoverIndex = 745;

            RentalLabel.Visible = true;

            if (ExineMainScene.User.HasExpandedStorage)
            {
                RentButton.Visible = true;
                LockedPage.Visible = false;
                RentalLabel.Text = GameLanguage.ExpandedStorageExpiresOn + ExineMainScene.User.ExpandedStorageExpiryTime.ToString();
                RentalLabel.ForeColour = Color.White;
            }
            else
            {
                RentalLabel.Text = GameLanguage.ExpandedStorageLocked;
                RentalLabel.ForeColour = Color.Red;
                RentButton.Visible = true;
                LockedPage.Visible = true;
            }

            foreach (var grid in Grid)
            {
                if (grid.ItemSlot < 80 || !ExineMainScene.User.HasExpandedStorage)
                    grid.Visible = false;
                else
                    grid.Visible = true;
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
    public sealed class BigButtonDialog : ExineImageControl
    {
        const int MaximumRows = 8;
        private List<BigButton> CurrentButtons;
        private int ScrollOffset = 0;
        public BigButtonDialog()
        {
            Visible = false;
        }

        public void Show(List<BigButton> buttons, int minimumButtons)
        {
            if (Visible) return;
            CurrentButtons = buttons;

            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Dispose();
            Controls.Clear();
            Size = Size.Empty;
            ScrollOffset = 0;

            CurrentButtons.ToList().ForEach(b => b.MouseWheel += (o, e) => BigButtonDialog_MouseWheel(o, e));
            int count = Math.Max(minimumButtons, buttons.Count);
            for (int i = 0; i < Math.Min(count, MaximumRows); i++)
            {
                ExineImageControl background = new ExineImageControl()
                {
                    Parent = this,
                    Library = Libraries.Title,
                    Location = new Point(buttons.Count == 1 ? -1 : 0, Size.Height),
                    Index = count == 1 ? 836 : (i == 0 ? 838 : (i == count - 1 ? 840 : 839)),
                    NotControl = false,
                    Visible = true,
                };
                background.MouseWheel += (o, e) => BigButtonDialog_MouseWheel(o, e);
                Size = new Size(background.Size.Width, Size.Height + background.Size.Height);
            }

            RefreshButtons();

            ExineImageControl footer = new ExineImageControl()
            {
                Parent = this,
                Library = Libraries.Title,
                Location = new Point(-1, Size.Height),
                Index = 837,
                NotControl = false,
                Visible = true,
            };
            Size = new Size(Size.Width, Size.Height + footer.Size.Height);

            if (buttons.Count > MaximumRows)
            {
                ExineButton upButton = new ExineButton
                {
                    Index = 197,
                    HoverIndex = 198,
                    PressedIndex = 199,
                    Library = Libraries.Prguse2,
                    Parent = this,
                    Size = new Size(16, 14),
                    Sound = SoundList.ButtonA,
                    Location = new Point(Size.Width - 26, 17)
                };
                upButton.Click += (o, e) =>
                {
                    ScrollUp();
                };

                ExineButton downButton = new ExineButton
                {
                    Index = 207,
                    HoverIndex = 208,
                    Library = Libraries.Prguse2,
                    PressedIndex = 209,
                    Parent = this,
                    Size = new Size(16, 14),
                    Sound = SoundList.ButtonA,
                    Location = new Point(Size.Width - 26, Size.Height - 57)
                };
                downButton.Click += (o, e) =>
                {
                    ScrollDown();
                };
            }

            Visible = true;
        }

        public override void Hide()
        {
            Size = Size.Empty;
            Visible = false;
        }

        private void RefreshButtons()
        {
            CurrentButtons.ToList().ForEach(b => b.Visible = false);

            for (int i = 0; i < Math.Min(CurrentButtons.Count, MaximumRows); i++)
            {
                CurrentButtons[i + ScrollOffset].Parent = this;
                CurrentButtons[i + ScrollOffset].Visible = true;
                CurrentButtons[i + ScrollOffset].Location = new Point(97, 7 + i * 40);
            }            
        }

        private void BigButtonDialog_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta / SystemInformation.MouseWheelScrollDelta;

            if (count > 0)
                ScrollUp();
            else if (count < 0)
                ScrollDown();
        }

        private void ScrollUp()
        {
            if (ScrollOffset <= 0) return;

            ScrollOffset--;
            RefreshButtons();
        }

        private void ScrollDown()
        {
            if (ScrollOffset + MaximumRows >= CurrentButtons.Count) return;

            ScrollOffset++;
            RefreshButtons();
        }
    }
    public sealed class BigButton : ExineButton
    {
        #region Label
        private ExineLabel _shadowLabel;
        #endregion

        #region CenterText
        public override bool CenterText
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                if (_center)
                {
                    _label.Size = Size;
                    _label.DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                    _shadowLabel.Size = Size;
                    _shadowLabel.DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
                }
                else
                {
                    _label.AutoSize = true;
                    _shadowLabel.AutoSize = true;
                }
            }
        }
        #endregion

        #region Font Colour
        public override Color FontColour
        {
            get
            {
                if (_label != null && !_label.IsDisposed)
                    return _label.ForeColour;
                return Color.Empty;
            }
            set
            {
                if (_label != null && !_label.IsDisposed)
                    _label.ForeColour = value;
            }
        }
        #endregion

        #region Size
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                _shadowLabel.Size = Size;
        }
        #endregion

        #region Text
        public override string Text
        {
            set
            {
                if (_label != null && !_label.IsDisposed)
                {
                    _label.Text = value;
                    _label.Visible = !string.IsNullOrEmpty(value);
                }

                if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                {
                    _shadowLabel.Text = value;
                    _shadowLabel.Visible = !string.IsNullOrEmpty(value);
                }
            }
        }
        #endregion
        public BigButton()
        {
            HoverIndex = -1;
            PressedIndex = -1;
            DisabledIndex = -1;
            Sound = SoundList.ButtonB;

            _shadowLabel = new ExineLabel
            {
                NotControl = true,
                Parent = this,
                Location = new Point(2, 7),
                AutoSize = false,
                Size = new Size(237, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                ForeColour = Color.Black,
                Font = ScaleFont(new Font(Settings.FontName, 12F, FontStyle.Bold))
            };

            _label = new ExineLabel
            {
                NotControl = true,
                Parent = this,
                Location = new Point(0, 5),
                AutoSize = false,
                Size = new Size(237, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter,                
                Font = ScaleFont(new Font(Settings.FontName, 12F, FontStyle.Bold))
            };
        }

        protected internal override void DrawControl()
        {
            base.DrawControl();

            if (DrawImage && Library != null)
            {
                bool oldGray = DXManager.GrayScale;

                if (GrayScale)
                {
                    DXManager.SetGrayscale(true);
                }

                if (Blending)
                    Library.DrawBlend(Index + 3, DisplayLocation, Color.White, false, BlendingRate);
                else
                    Library.Draw(Index + 3, DisplayLocation, Color.White, false, Opacity);

                if (GrayScale) DXManager.SetGrayscale(oldGray);
            }
        }

        #region Disposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            if (_shadowLabel != null && !_shadowLabel.IsDisposed)
                _shadowLabel.Dispose();
            _shadowLabel = null;
        }
        #endregion
    }
}

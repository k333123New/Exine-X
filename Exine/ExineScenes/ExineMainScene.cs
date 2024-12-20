﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineNetwork;
using Exine.ExineObjects;
using Exine.ExineSounds;
using SlimDX;
using SlimDX.Direct3D9;
using Font = System.Drawing.Font;
// 

using Effect = Exine.ExineObjects.Effect;
//using Client.MirScenes.Dialogs;
using Exine.Utils;
using Exine.ExineScenes.ExDialogs;
using System;

//effect2 58~67 pawn

namespace Exine.ExineScenes
{
    public sealed class ExineMainScene : ExineScene
    {
        public static ExineMainScene Scene;
        public static bool Observing;
        public static bool AllowObserve;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static long MoveTime, AttackTime, NextRunTime, LogTime, LastRunTime;
        public static bool CanMove, CanRun;
         
         
        public MapControl MapControl;
        //public MainDialog MainDialog;

        #region UI variable
        public ExineAnimatedControl _bazel_up, _bazel_down, _bazel_left, _bazel_right;//add k333123
        public ExineMainDialog ExMainDialog;
        public ExineChatDialog ExChatDialog;
        public ExineChatControlBar ExChatControl; 
        public ExineInventoryDialog ExInventoryDialog;
        public ExineCharacterDialog ExCharacterDialog;
        public ExineStateDialog ExStateDialog;

        public ExineNPCDialog ExNPCDialog;
        public ExineNPCGoodsDialog ExNPCGoodsDialog;
        public ExineNPCGoodsDialog NPCSubGoodsDialog;
        public ExineNPCGoodsDialog NPCCraftGoodsDialog;
        public NPCDropDialog NPCDropDialog; 
        
        public CraftDialog CraftDialog;
        public StorageDialog StorageDialog;
        public BeltDialog BeltDialog;
        public MiniMapDialog MiniMapDialog;
        public InspectDialog InspectDialog;
        public OptionDialog OptionDialog;
        public MenuDialog MenuDialog;
       
        public HelpDialog HelpDialog;  
        public RefineDialog RefineDialog;

        public GroupDialog GroupDialog;
        public GuildDialog GuildDialog;

        //public NewCharacterDialog NewHeroDialog; 
        public BigMapDialog BigMapDialog; 
        public CharacterDuraPanel CharacterDuraPanel;
        public DuraStatusDialog DuraStatusPanel;
        public TradeDialog TradeDialog;
        public GuestTradeDialog GuestTradeDialog;

        public SocketDialog SocketDialog;

        public List<SkillBarDialog> SkillBarDialogs = new List<SkillBarDialog>();
        public ChatOptionDialog ChatOptionDialog;
        public ChatNoticeDialog ChatNoticeDialog;

        public QuestListDialog QuestListDialog;
        public QuestDetailDialog QuestDetailDialog;
        public QuestDiaryDialog QuestLogDialog;
        public QuestTrackingDialog QuestTrackingDialog;

        public RankingDialog RankingDialog;

       

        public FriendDialog FriendDialog;
        
        public RelationshipDialog RelationshipDialog;
        public MentorDialog MentorDialog; 

        public ReportDialog ReportDialog;
          
        public BuffDialog BuffsDialog;
        public BuffDialog HeroBuffsDialog;

        public KeyboardLayoutDialog KeyboardLayoutDialog;
        public NoticeDialog NoticeDialog;

        public TimerDialog TimerControl;
        public CompassDialog CompassControl;
        #endregion UI variable

        public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public static List<UserId> UserIdList = new List<UserId>();
        public static List<UserItem> ChatItemList = new List<UserItem>();
        public static List<ClientQuestInfo> QuestInfoList = new List<ClientQuestInfo>();
        public static List<GameShopItem> GameShopInfoList = new List<GameShopItem>();
        public static List<ClientRecipeInfo> RecipeInfoList = new List<ClientRecipeInfo>();
        public static Dictionary<int, BigMapRecord> MapInfoList = new Dictionary<int, BigMapRecord>();
        public static List<ClientHeroInformation> HeroInfoList = new List<ClientHeroInformation>();
        public static ClientHeroInformation[] HeroStorage = new ClientHeroInformation[8];
        public static Dictionary<long, RankCharacterInfo> RankingList = new Dictionary<long, RankCharacterInfo>();
        public static int TeleportToNPCCost;
        public static int MaximumHeroCount;

        public static UserItem[] Storage = new UserItem[80];
        public static UserItem[] GuildStorage = new UserItem[112];
        public static UserItem[] Refine = new UserItem[16];
        public static UserItem HoverItem, SelectedItem;
        public static MirItemCell SelectedCell;

        public static bool PickedUpGold;
        public ExineControl ItemLabel, MailLabel, MemoLabel, GuildBuffLabel;
        public static long UseItemTime, PickUpTime, DropViewTime, TargetDeadTime;
        public static uint Gold, Credit;
        public static long InspectTime;
        public bool ShowReviveMessage;

        public bool NewMail;
        public int NewMailCounter = 0;

        public AttackMode AMode;
        public PetMode PMode;
        public LightSetting Lights;

        public static long NPCTime;
        public static uint NPCID;
        public static float NPCRate;
        public static uint DefaultNPCID;
        public static bool HideAddedStoreStats;

        public long ToggleTime;
        public static long SpellTime;

        public ExineLabel[] OutputLines = new ExineLabel[10];
        public List<OutPutMessage> OutputMessages = new List<OutPutMessage>();

        public long OutputDelay;

        public ExineMainScene()
        {
            

            MapControl.AutoRun = false;
            MapControl.AutoHit = false;

            Scene = this;
            BackColour = Color.Transparent;
            //BackColour = Color.Black;//add k333123
            MoveTime = CMain.Time;

            KeyDown += GameScene_KeyDown;
            
            ExMainDialog = new ExineMainDialog { Parent = this, BackColour = Color.Black}; //add k333123 mod
            ExChatDialog = new ExineChatDialog { Parent = this, Visible=true }; //add k333123 mod
            ExChatControl = new ExineChatControlBar { Parent = this }; //add k333123 mod   

            ExInventoryDialog = new ExineInventoryDialog { Parent = this };
            BeltDialog = new BeltDialog { Parent = this, Visible=false };//임시로 꺼둠
            ExNPCDialog = new ExineNPCDialog { Parent = this, Visible = false };
            MiniMapDialog = new MiniMapDialog { Parent = this };
            ExNPCGoodsDialog = new ExineNPCGoodsDialog(PanelType.Buy) { Parent = this, Visible = false };







            StorageDialog = new StorageDialog { Parent = this, Visible = false };
            CraftDialog = new CraftDialog { Parent = this, Visible = false };
            
            InspectDialog = new InspectDialog { Parent = this, Visible = false };
            OptionDialog = new OptionDialog { Parent = this, Visible = false };
            MenuDialog = new MenuDialog { Parent = this, Visible = false };
            
            
            
            NPCSubGoodsDialog = new ExineNPCGoodsDialog(PanelType.BuySub) { Parent = this, Visible = false };
            NPCCraftGoodsDialog = new ExineNPCGoodsDialog(PanelType.Craft) { Parent = this, Visible = false };
            NPCDropDialog = new NPCDropDialog { Parent = this, Visible = false };
             

            HelpDialog = new HelpDialog { Parent = this, Visible = false };
            KeyboardLayoutDialog = new KeyboardLayoutDialog { Parent = this, Visible = false };
            NoticeDialog = new NoticeDialog { Parent = this, Visible = false };
             
            

            GroupDialog = new GroupDialog { Parent = this, Visible = false };
            GuildDialog = new GuildDialog { Parent = this, Visible = false };
            
            BigMapDialog = new BigMapDialog { Parent = this, Visible = false };
             
            CharacterDuraPanel = new CharacterDuraPanel { Parent = this, Visible = false };
            //DuraStatusPanel = new DuraStatusDialog { Parent = this, Visible = true };
            DuraStatusPanel = new DuraStatusDialog { Parent = this, Visible = false };
            TradeDialog = new TradeDialog { Parent = this, Visible = false };
            GuestTradeDialog = new GuestTradeDialog { Parent = this, Visible = false };

            SocketDialog = new SocketDialog { Parent = this, Visible = false };

            SkillBarDialog Bar1 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 0 };
            SkillBarDialogs.Add(Bar1);
            SkillBarDialog Bar2 = new SkillBarDialog { Parent = this, Visible = false, BarIndex = 1 };
            SkillBarDialogs.Add(Bar2);
            ChatOptionDialog = new ChatOptionDialog { Parent = this, Visible = false };
            ChatNoticeDialog = new ChatNoticeDialog { Parent = this, Visible = false };

            QuestListDialog = new QuestListDialog { Parent = this, Visible = false };
            QuestDetailDialog = new QuestDetailDialog { Parent = this, Visible = false };
            QuestTrackingDialog = new QuestTrackingDialog { Parent = this, Visible = false };
            QuestLogDialog = new QuestDiaryDialog { Parent = this, Visible = false };

            RankingDialog = new RankingDialog { Parent = this, Visible = false };

            

            RefineDialog = new RefineDialog { Parent = this, Visible = false };
            RelationshipDialog = new RelationshipDialog { Parent = this, Visible = false };
            FriendDialog = new FriendDialog { Parent = this, Visible = false };
            
            MentorDialog = new MentorDialog { Parent = this, Visible = false }; 
            ReportDialog = new ReportDialog { Parent = this, Visible = false }; 

            BuffsDialog = new BuffDialog
            {
                Parent = this,
                Visible = true,
                GetExpandedParameter = () => { return Settings.ExpandedBuffWindow; },
                SetExpandedParameter = (value) => { Settings.ExpandedBuffWindow = value; }
            };

            KeyboardLayoutDialog = new KeyboardLayoutDialog { Parent = this, Visible = false };

            TimerControl = new TimerDialog { Parent = this, Visible = false };
            CompassControl = new CompassDialog { Parent = this, Visible = false }; 

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i] = new ExineLabel
                {
                    AutoSize = true,
                    BackColour = Color.Transparent,
                    Font = new Font(Settings.FontName, 10F),
                    ForeColour = Color.LimeGreen,
                    Location = new Point(20, 25 + i * 13),
                    OutLine = true,
                };

            if (MapInfoList.Count > 0)
                RecreateBigMapButtons();



            /////////////////////////////////////
            //Exine UI
            
           
           

            /////////////////////////////////////
            
            _bazel_left = new ExineAnimatedControl
            {
                Animated = false,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 0,
                Library = Libraries.BAZEL,
                Loop = true,
                Parent = this,
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Location = new Point(0,0),
                ForeColour = Color.Black,
                Visible = true,
            };
            _bazel_right = new ExineAnimatedControl
            {
                Animated = false,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 0,
                Library = Libraries.BAZEL,
                Loop = true,
                Parent = this,
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Location = new Point(1024- 113, 0),
                ForeColour = Color.Black,
                Visible = true,
            };
            _bazel_up = new ExineAnimatedControl
            {
                Animated = false,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 1,
                Library = Libraries.BAZEL,
                Loop = true,
                Parent = this,
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Location = new Point(113, 0),
                ForeColour = Color.Black,
                Visible = true,
            };
            _bazel_down = new ExineAnimatedControl
            {
                Animated = false,
                AnimationCount = 1,
                AnimationDelay = 0,
                Index = 1,
                Library = Libraries.BAZEL,
                Loop = true,
                Parent = this,
                //Location = new Point((1024 - 800) / 2, (768 - 600) / 2),
                Location = new Point(113, 768-84),
                ForeColour = Color.Black,
                Visible = true,
            };

            ExChatControl.Hide();

        }

        private void UpdateMouseCursor()
        {
            if (!Settings.UseMouseCursors) return;

            if (ExineMainScene.HoverItem != null)
            {
                if (ExineMainScene.SelectedCell != null && ExineMainScene.SelectedCell.Item != null && ExineMainScene.SelectedCell.Item.Info.Type == ItemType.Gem && CMain.Ctrl)
                {
                    CMain.SetMouseCursor(MouseCursor.Upgrade);
                }
                else
                {
                    CMain.SetMouseCursor(MouseCursor.Default);
                }
            }
            else if (MapObject.MouseObject != null)
            {
                switch (MapObject.MouseObject.Race)
                {
                    case ObjectType.Monster:
                        CMain.SetMouseCursor(MouseCursor.Attack);
                        break;
                    case ObjectType.Merchant:
                        CMain.SetMouseCursor(MouseCursor.NPCTalk);
                        break;
                    case ObjectType.Player:
                        if (CMain.Shift)
                        {
                            CMain.SetMouseCursor(MouseCursor.AttackRed);
                        }
                        else
                        {
                            CMain.SetMouseCursor(MouseCursor.Default);
                        }
                        break;
                    default:
                        CMain.SetMouseCursor(MouseCursor.Default);
                        break;
                }
            }
            else
            {
                CMain.SetMouseCursor(MouseCursor.Default);
            }

        }

        public void OutputMessage(string message, OutputMessageType type = OutputMessageType.Normal)
        {
            OutputMessages.Add(new OutPutMessage { Message = message, ExpireTime = CMain.Time + 5000, Type = type });
            if (OutputMessages.Count > 10)
                OutputMessages.RemoveAt(0);
        }

        private void ProcessOuput()
        {
            for (int i = 0; i < OutputMessages.Count; i++)
            {
                if (CMain.Time >= OutputMessages[i].ExpireTime)
                    OutputMessages.RemoveAt(i);
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                if (OutputMessages.Count > i)
                {
                    Color color;
                    switch (OutputMessages[i].Type)
                    {
                        case OutputMessageType.Quest:
                            color = Color.Gold;
                            break;
                        case OutputMessageType.Guild:
                            color = Color.DeepPink;
                            break;
                        default:
                            color = Color.LimeGreen;
                            break;
                    }

                    OutputLines[i].Text = OutputMessages[i].Message;
                    OutputLines[i].ForeColour = color;
                    OutputLines[i].Visible = true;
                }
                else
                {
                    OutputLines[i].Text = string.Empty;
                    OutputLines[i].Visible = false;
                }
            }
        }
        private void GameScene_KeyDown(object sender, KeyEventArgs e)
        {
            if (ExineMainScene.Scene.KeyboardLayoutDialog.WaitingForBind != null)
            {
                ExineMainScene.Scene.KeyboardLayoutDialog.CheckNewInput(e);
                return;
            }

            foreach (KeyBind KeyCheck in CMain.InputKeys.Keylist)
            {
                if (KeyCheck.Key == Keys.None)
                    continue;
                if (KeyCheck.Key != e.KeyCode)
                    continue;
                if ((KeyCheck.RequireAlt != 2) && (KeyCheck.RequireAlt != (CMain.Alt ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireShift != 2) && (KeyCheck.RequireShift != (CMain.Shift ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireCtrl != 2) && (KeyCheck.RequireCtrl != (CMain.Ctrl ? 1 : 0)))
                    continue;
                if ((KeyCheck.RequireTilde != 2) && (KeyCheck.RequireTilde != (CMain.Tilde ? 1 : 0)))
                    continue;
                //now run the real code
                switch (KeyCheck.function)
                {
                    case KeybindOptions.Bar1Skill1: UseSpell(1); break;
                    case KeybindOptions.Bar1Skill2: UseSpell(2); break;
                    case KeybindOptions.Bar1Skill3: UseSpell(3); break;
                    case KeybindOptions.Bar1Skill4: UseSpell(4); break;
                    case KeybindOptions.Bar1Skill5: UseSpell(5); break;
                    case KeybindOptions.Bar1Skill6: UseSpell(6); break;
                    case KeybindOptions.Bar1Skill7: UseSpell(7); break;
                    case KeybindOptions.Bar1Skill8: UseSpell(8); break;
                    case KeybindOptions.Bar2Skill1: UseSpell(9); break;
                    case KeybindOptions.Bar2Skill2: UseSpell(10); break;
                    case KeybindOptions.Bar2Skill3: UseSpell(11); break;
                    case KeybindOptions.Bar2Skill4: UseSpell(12); break;
                    case KeybindOptions.Bar2Skill5: UseSpell(13); break;
                    case KeybindOptions.Bar2Skill6: UseSpell(14); break;
                    case KeybindOptions.Bar2Skill7: UseSpell(15); break;
                    case KeybindOptions.Bar2Skill8: UseSpell(16); break;
                    case KeybindOptions.HeroSkill1: UseSpell(17); break;
                    case KeybindOptions.HeroSkill2: UseSpell(18); break;
                    case KeybindOptions.HeroSkill3: UseSpell(19); break;
                    case KeybindOptions.HeroSkill4: UseSpell(20); break;
                    case KeybindOptions.HeroSkill5: UseSpell(21); break;
                    case KeybindOptions.HeroSkill6: UseSpell(22); break;
                    case KeybindOptions.HeroSkill7: UseSpell(23); break;
                    case KeybindOptions.HeroSkill8: UseSpell(24); break;

                    case KeybindOptions.Inventory:
                        //if (!InventoryDialog.Visible) InventoryDialog.Show();
                        //else InventoryDialog.Hide();
                        break;

                    case KeybindOptions.Inventory2:
                        if (!ExStateDialog.Visible) ExStateDialog.Show();
                        else ExStateDialog.Hide();
                        break;


                    case KeybindOptions.Equipment:
                    case KeybindOptions.Equipment2:
                        if (!ExCharacterDialog.Visible || !ExCharacterDialog.CharacterPage.Visible)
                        {
                            ExCharacterDialog.Show();
                            ExCharacterDialog.ShowCharacterPage();
                            ExInventoryDialog.Location = new Point(541, 378);//k333123
                            ExInventoryDialog.Show();//k33123 add

                        }
                        else
                        {
                            ExCharacterDialog.Hide();
                            ExInventoryDialog.Hide();//k33123 add
                        }
                        break;

                    case KeybindOptions.Skills:
                    case KeybindOptions.Skills2:
                        if (!ExCharacterDialog.Visible || !ExCharacterDialog.SkillPage.Visible)
                        {
                            ExCharacterDialog.Show();
                            ExCharacterDialog.ShowSkillPage();
                        }
                        else ExCharacterDialog.Hide();
                        break;
                     
                   
                    case KeybindOptions.Skillbar:
                        if (!Settings.SkillBar)
                            foreach (SkillBarDialog Bar in SkillBarDialogs)
                                Bar.Show();
                        else
                            foreach (SkillBarDialog Bar in SkillBarDialogs)
                                Bar.Hide();
                        break;
                     
                    case KeybindOptions.Mentor:
                        if (!MentorDialog.Visible) MentorDialog.Show();
                        else MentorDialog.Hide();
                        break;
                    case KeybindOptions.Relationship:
                        if (!RelationshipDialog.Visible) RelationshipDialog.Show();
                        else RelationshipDialog.Hide();
                        break;
                    case KeybindOptions.Friends:
                        if (!FriendDialog.Visible) FriendDialog.Show();
                        else FriendDialog.Hide();
                        break;
                    case KeybindOptions.Guilds:
                        if (!GuildDialog.Visible) GuildDialog.Show();
                        else
                        {
                            GuildDialog.Hide();
                        }
                        break;

                    case KeybindOptions.Ranking:
                        if (!RankingDialog.Visible) RankingDialog.Show();
                        else RankingDialog.Hide();
                        break;
                    case KeybindOptions.Quests:
                        if (!QuestLogDialog.Visible) QuestLogDialog.Show();
                        else QuestLogDialog.Hide();
                        break;
                    case KeybindOptions.Exit:
                        QuitGame();
                        return;

                    case KeybindOptions.Closeall:
                        ExInventoryDialog.Hide();
                        ExCharacterDialog.Hide();
                        OptionDialog.Hide();
                        MenuDialog.Hide();
                        if (ExNPCDialog.Visible) ExNPCDialog.Hide();
                        HelpDialog.Hide();
                        KeyboardLayoutDialog.Hide();
                        RankingDialog.Hide();   
                        FriendDialog.Hide();
                        RelationshipDialog.Hide();
                        MentorDialog.Hide(); 
                        GroupDialog.Hide();
                        GuildDialog.Hide();
                        InspectDialog.Hide();
                        StorageDialog.Hide();
                         
                        //CharacterDuraPanel.Hide();
                        QuestListDialog.Hide();
                        QuestDetailDialog.Hide();
                        QuestLogDialog.Hide();
                        RefineDialog.Hide();
                        BigMapDialog.Hide(); 
                       
                        NoticeDialog.Hide();


                        ExineMainScene.Scene.DisposeItemLabel();
                        break;
                    case KeybindOptions.Options:
                    case KeybindOptions.Options2:
                        if (!OptionDialog.Visible) OptionDialog.Show();
                        else OptionDialog.Hide();
                        break;
                    case KeybindOptions.Group:
                        if (!GroupDialog.Visible) GroupDialog.Show();
                        else GroupDialog.Hide();
                        break;
                    case KeybindOptions.Belt:
                        if (!BeltDialog.Visible) BeltDialog.Show();
                        else BeltDialog.Hide();
                        break;
                    case KeybindOptions.BeltFlip:
                        BeltDialog.Flip();
                        break;
                    case KeybindOptions.Pickup:
                        if (CMain.Time > PickUpTime)
                        {
                            PickUpTime = CMain.Time + 200;
                            Network.SendPacketToServer(new ClientPacket.PickUp());
                        }
                        break;
                    case KeybindOptions.Belt1:
                    case KeybindOptions.Belt1Alt:
                        BeltDialog.Grid[0].UseItem();
                        break;
                    case KeybindOptions.Belt2:
                    case KeybindOptions.Belt2Alt:
                        BeltDialog.Grid[1].UseItem();
                        break;
                    case KeybindOptions.Belt3:
                    case KeybindOptions.Belt3Alt:
                        BeltDialog.Grid[2].UseItem();
                        break;
                    case KeybindOptions.Belt4:
                    case KeybindOptions.Belt4Alt:
                        BeltDialog.Grid[3].UseItem();
                        break;
                    case KeybindOptions.Belt5:
                    case KeybindOptions.Belt5Alt:
                        BeltDialog.Grid[4].UseItem();
                        break;
                    case KeybindOptions.Belt6:
                    case KeybindOptions.Belt6Alt:
                        BeltDialog.Grid[5].UseItem();
                        break;
                    case KeybindOptions.Belt7:
                    case KeybindOptions.Belt7Alt:

                        break;
                    case KeybindOptions.Belt8:
                    case KeybindOptions.Belt8Alt:

                        break;
                    case KeybindOptions.Logout:
                        LogOut();
                        break;
                    
                    case KeybindOptions.Bigmap:
                        BigMapDialog.Toggle();
                        break;
                    case KeybindOptions.Trade:
                        Network.SendPacketToServer(new ClientPacket.TradeRequest());
                        break;
                    case KeybindOptions.Rental:
                        break;
                    case KeybindOptions.ChangePetmode:
                        ChangePetMode();
                        break;
                    case KeybindOptions.PetmodeBoth:
                        Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.Both });
                        return;
                    case KeybindOptions.PetmodeMoveonly:
                        Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.MoveOnly });
                        return;
                    case KeybindOptions.PetmodeAttackonly:
                        Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.AttackOnly });
                        return;
                    case KeybindOptions.PetmodeNone:
                        Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.None });
                        return;
                    case KeybindOptions.PetmodeFocusMasterTarget:
                        Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.FocusMasterTarget });
                        return;
                    
                    case KeybindOptions.ChangeAttackmode:
                        ChangeAttackMode();
                        break;
                    case KeybindOptions.AttackmodePeace:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Peace });
                        return;
                    case KeybindOptions.AttackmodeGroup:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Group });
                        return;
                    case KeybindOptions.AttackmodeGuild:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Guild });
                        return;
                    case KeybindOptions.AttackmodeEnemyguild:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.EnemyGuild });
                        return;
                    case KeybindOptions.AttackmodeRedbrown:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.RedBrown });
                        return;
                    case KeybindOptions.AttackmodeAll:
                        Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.All });
                        return;

                    case KeybindOptions.Help:
                        if (!HelpDialog.Visible) HelpDialog.Show();
                        else HelpDialog.Hide();
                        break;
                    case KeybindOptions.Keybind:
                        if (!KeyboardLayoutDialog.Visible) KeyboardLayoutDialog.Show();
                        else KeyboardLayoutDialog.Hide();
                        break;
                    case KeybindOptions.Autorun:
                        MapControl.AutoRun = !MapControl.AutoRun;
                        break;
                    case KeybindOptions.Cameramode:

                        if (!ExMainDialog.Visible)
                        {
                            ExMainDialog.Show();
                            ExChatDialog.Show();
                            BeltDialog.Show();
                            ExChatControl.Show();
                            MiniMapDialog.Show();
                            CharacterDuraPanel.Show();
                            DuraStatusPanel.Show();
                            BuffsDialog.Show();
                        }
                        else
                        {
                            ExMainDialog.Hide();
                            ExChatDialog.Hide();
                            BeltDialog.Hide();
                            ExChatControl.Hide();
                            MiniMapDialog.Hide();
                            CharacterDuraPanel.Hide();
                            DuraStatusPanel.Hide();
                            BuffsDialog.Hide();
                        }
                        break;
                    case KeybindOptions.DropView:
                        if (CMain.Time > DropViewTime)
                            DropViewTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.TargetDead:
                        if (CMain.Time > TargetDeadTime)
                            TargetDeadTime = CMain.Time + 5000;
                        break;
                    case KeybindOptions.AddGroupMember:
                        if (MapObject.MouseObject == null) break;
                        if (MapObject.MouseObject.Race != ObjectType.Player) break;

                        ExineMainScene.Scene.GroupDialog.AddMember(MapObject.MouseObject.Name);
                        break;

                    case KeybindOptions.ExinePeaceModeChange:
                        Console.WriteLine("ExinePeaceModeChange!!!");
                        MapObject.User.ExinePeaceMode = !MapObject.User.ExinePeaceMode; //k333123 add 240311
                        Console.WriteLine("ExinePeaceModeChange!!! Now : "+ MapObject.User.ExinePeaceMode);
                        break;
                }
            }
        }

        public void ChangeSkillMode(bool? ctrl)
        {
            if (Settings.SkillMode || ctrl == true)
            {
                Settings.SkillMode = false;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[SkillMode Ctrl]", ChatType.Hint);
                ExineMainScene.Scene.OptionDialog.ToggleSkillButtons(true);
            }
            else if (!Settings.SkillMode || ctrl == false)
            {
                Settings.SkillMode = true;
                ExineMainScene.Scene.ExChatDialog.ReceiveChat("[SkillMode ~]", ChatType.Hint);
                ExineMainScene.Scene.OptionDialog.ToggleSkillButtons(false);
            }
        }

        public void ChangePetMode()
        {
            switch (PMode)
            {
                case PetMode.Both:
                    Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.MoveOnly });
                    return;
                case PetMode.MoveOnly:
                    Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.AttackOnly });
                    return;
                case PetMode.AttackOnly:
                    Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.None });
                    return;
                case PetMode.None:
                    Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.FocusMasterTarget });
                    return;
                case PetMode.FocusMasterTarget:
                    Network.SendPacketToServer(new ClientPacket.ChangePMode { Mode = PetMode.Both });
                    return;
            }
        }

        public void ChangeAttackMode()
        {
            Console.WriteLine(AMode);
            switch (AMode)
            {
                case AttackMode.Peace:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Group });
                    return;
                case AttackMode.Group:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Guild });
                    return;
                case AttackMode.Guild:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.EnemyGuild });
                    return;
                case AttackMode.EnemyGuild:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.RedBrown });
                    return;
                case AttackMode.RedBrown:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.All });
                    return;
                case AttackMode.All:
                    Network.SendPacketToServer(new ClientPacket.ChangeAMode { Mode = AttackMode.Peace });
                    return;
            }
        }

        public void UseSpell(int key)
        {
            UserObject actor = User;
            
            if (actor.Dead ||  actor.Fishing) return;

            if (!actor.HasClassWeapon && actor.Weapon >= 0)
            {
                ExChatDialog.ReceiveChat("이 기술을 수행하려면 적절한 무기를 착용하고 있어야 합니다.", ChatType.System);
                return;
            }

            if (CMain.Time < actor.BlizzardStopTime || CMain.Time < actor.ReincarnationStopTime) return;

            ClientMagic magic = null;

            for (int i = 0; i < actor.Magics.Count; i++)
            {
                if (actor.Magics[i].Key != key) continue;
                magic = actor.Magics[i];
                break;
            }

            if (magic == null) return;

            switch (magic.Spell)
            {
                case Spell.CounterAttack:
                    if ((CMain.Time < magic.CastTime + magic.Delay))
                    {
                        if (CMain.Time >= OutputDelay)
                        {
                            OutputDelay = CMain.Time + 1000;
                            Scene.OutputMessage(string.Format("{0}을 {1}초 동안 시전할 수 없습니다.", magic.Spell.ToString(), ((magic.CastTime + magic.Delay) - CMain.Time - 1) / 1000 + 1));
                        }

                        return;
                    }
                    magic.CastTime = CMain.Time;
                    break;
            }

            int cost;
            string prefix = string.Empty;
            switch (magic.Spell)
            {
                case Spell.Fencing:
                case Spell.FatalSword:
                case Spell.MPEater:
                case Spell.Hemorrhage:
                case Spell.SpiritSword:
                case Spell.Slaying:
                case Spell.Focus:
                case Spell.Meditation:
                    return;
                case Spell.Thrusting:
                    if (CMain.Time < ToggleTime) return;
                    actor.Thrusting = !actor.Thrusting;
                    ExChatDialog.ReceiveChat(prefix + (actor.Thrusting ? "Use Thrusting." : "Do not use Thrusting."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.Thrusting);
                    break;
                case Spell.HalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    actor.HalfMoon = !actor.HalfMoon;
                    ExChatDialog.ReceiveChat(prefix + (actor.HalfMoon ? "Use Half Moon." : "Do not use Half Moon."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.HalfMoon);
                    break;
                case Spell.CrossHalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    actor.CrossHalfMoon = !actor.CrossHalfMoon;
                    ExChatDialog.ReceiveChat(prefix + (actor.CrossHalfMoon ? "Use Cross Half Moon." : "Do not use Cross Half Moon."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.CrossHalfMoon);
                    break;
                case Spell.DoubleSlash:
                    if (CMain.Time < ToggleTime) return;
                    actor.DoubleSlash = !actor.DoubleSlash;
                    ExChatDialog.ReceiveChat(prefix + (actor.DoubleSlash ? "Use Double Slash." : "Do not use Double Slash."), ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    SendSpellToggle(actor, magic.Spell, actor.DoubleSlash);
                    break;
                case Spell.TwinDrakeBlade:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > actor.MP)
                    {
                        Scene.OutputMessage(GameLanguage.LowMana);
                        return;
                    }
                    actor.TwinDrakeBlade = true;
                    SendSpellToggle(actor, magic.Spell, true);
                    actor.Effects.Add(new Effect(Libraries.Magic2, 210, 6, 500, actor));
                    break;
                case Spell.FlamingSword:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > actor.MP)
                    {
                        Scene.OutputMessage(GameLanguage.LowMana);
                        return;
                    }
                    SendSpellToggle(actor, magic.Spell, true);
                    break;
                case Spell.CounterAttack:
                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > actor.MP)
                    {
                        Scene.OutputMessage(GameLanguage.LowMana);
                        return;
                    }

                    SoundManager.PlaySound(20000 + (ushort)Spell.CounterAttack * 10);
                    SendSpellToggle(actor, magic.Spell, true);
                    break;
                case Spell.MentalState:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;
                    SendSpellToggle(actor, magic.Spell, true);
                    break;
                default:
                    actor.NextMagic = magic;
                    actor.NextMagicLocation = MapControl.MapLocation;
                    actor.NextMagicObject = MapObject.MouseObject;
                    actor.NextMagicDirection = MapControl.MouseDirection();

                    break;
            }
        }
       
        private void SendSpellToggle(UserObject Actor, Spell Spell, bool CanUse)
        {
            if (Actor == User)
                Network.SendPacketToServer(new ClientPacket.SpellToggle { Spell = Spell, CanUse = CanUse });
            else
                Network.SendPacketToServer(new ClientPacket.SpellToggle { Spell = Spell });
        }
       
        public void QuitGame()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                ExineMessageBox messageBox = new ExineMessageBox(GameLanguage.ExitTip, MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) => Program.Form.Close();
                messageBox.Show();
            }
            else
            {
                ExChatDialog.ReceiveChat(string.Format(GameLanguage.CannotLeaveGame, (LogTime - CMain.Time) / 1000), ChatType.System);
            }
        }
      
        public void LogOut()
        {
            if (CMain.Time >= LogTime)
            {
                //If Last Combat < 10 CANCEL
                ExineMessageBox messageBox = new ExineMessageBox(GameLanguage.LogOutTip, MirMessageBoxButtons.YesNo);
                messageBox.YesButton.Click += (o, e) =>
                {
                    Network.SendPacketToServer(new ClientPacket.LogOut());
                    Enabled = false;
                };
                messageBox.Show();
            }
            else
            {
                ExChatDialog.ReceiveChat(string.Format(GameLanguage.CannotLeaveGame, (LogTime - CMain.Time) / 1000), ChatType.System);
            }
        }

        protected internal override void DrawControl()
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.DrawControl();
            base.DrawControl();


            if (PickedUpGold || (SelectedCell != null && SelectedCell.Item != null))
            {
                int image = PickedUpGold ? 116 : SelectedCell.Item.Image;
                Size imgSize = Libraries.Items.GetTrueSize(image);
                Point p = CMain.MPoint.Add(-imgSize.Width / 2, -imgSize.Height / 2);

                if (p.X + imgSize.Width >= Settings.ScreenWidth)
                    p.X = Settings.ScreenWidth - imgSize.Width;

                if (p.Y + imgSize.Height >= Settings.ScreenHeight)
                    p.Y = Settings.ScreenHeight - imgSize.Height;

                Libraries.Items.Draw(image, p.X, p.Y);
            }

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i].Draw();
        }
       
        public override void Process()
        {
            if (MapControl == null || User == null)
                return;

            if (CMain.Time >= MoveTime)
            {
                MoveTime += 100; //Move Speed
                CanMove = true;
                MapControl.AnimationCount++;
                MapControl.TextureValid = false;
            }
            else
                CanMove = false;

            if (CMain.Time >= CMain.NextPing)
            {
                CMain.NextPing = CMain.Time + 60000;
                Network.SendPacketToServer(new ClientPacket.KeepAlive() { Time = CMain.Time });
            }

            TimerControl.Process();
            CompassControl.Process();
            RankingDialog.Process();

            MirItemCell cell = MouseControl as MirItemCell;

            if (cell != null && HoverItem != cell.Item && HoverItem != cell.ShadowItem)
            {
                DisposeItemLabel();
                HoverItem = null;
                CreateItemLabel(cell.Item);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                ItemLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + ItemLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - ItemLabel.Size.Width;

                if (y + ItemLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - ItemLabel.Size.Height;
                ItemLabel.Location = new Point(x, y);
            }

            if (MailLabel != null && !MailLabel.IsDisposed)
            {
                MailLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MailLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MailLabel.Size.Width;

                if (y + MailLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MailLabel.Size.Height;
                MailLabel.Location = new Point(x, y);
            }

            if (MemoLabel != null && !MemoLabel.IsDisposed)
            {
                MemoLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + MemoLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - MemoLabel.Size.Width;

                if (y + MemoLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - MemoLabel.Size.Height;
                MemoLabel.Location = new Point(x, y);
            }

            if (GuildBuffLabel != null && !GuildBuffLabel.IsDisposed)
            {
                GuildBuffLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + GuildBuffLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - GuildBuffLabel.Size.Width;

                if (y + GuildBuffLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - GuildBuffLabel.Size.Height;
                GuildBuffLabel.Location = new Point(x, y);
            }

            if (!User.Dead) ShowReviveMessage = false;

            if (ShowReviveMessage && CMain.Time > User.DeadTime && User.CurrentAction == ExAction.Dead)
            {
                ShowReviveMessage = false;
                ExineMessageBox messageBox = new ExineMessageBox(GameLanguage.DiedTip, MirMessageBoxButtons.YesNo, false);

                messageBox.YesButton.Click += (o, e) =>
                {
                    if (User.Dead) Network.SendPacketToServer(new ClientPacket.TownRevive());
                };

                messageBox.AfterDraw += (o, e) =>
                {
                    if (!User.Dead) messageBox.Dispose();
                };

                messageBox.Show();
            }

            BuffsDialog.Process();
            HeroBuffsDialog?.Process();

            MapControl.Process();
            ExMainDialog.Process();
            ExChatDialog.Process(); //k333123
            ExCharacterDialog.Process();  //k333123
            ExInventoryDialog.Process(); 
            MiniMapDialog.Process();

            foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                Bar.Process();

            DialogProcess();

            ProcessOuput();

            UpdateMouseCursor();

            SoundManager.ProcessDelayedSounds();
        }

        public void DialogProcess()
        {
            if (Settings.SkillBar)
            {
                foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                    Bar.Show();
            }
            else
            {
                foreach (SkillBarDialog Bar in Scene.SkillBarDialogs)
                    Bar.Hide();
            }

            for (int i = 0; i < Scene.SkillBarDialogs.Count; i++)
            {
                if (i * 2 > Settings.SkillbarLocation.Length) break;
                if ((Settings.SkillbarLocation[i, 0] > Settings.Resolution - 100) || (Settings.SkillbarLocation[i, 1] > 700)) continue;//in theory you'd want the y coord to be validated based on resolution, but since client only allows for wider screens and not higher :(
                Scene.SkillBarDialogs[i].Location = new Point(Settings.SkillbarLocation[i, 0], Settings.SkillbarLocation[i, 1]);
            }

            if (Settings.DuraView)
                CharacterDuraPanel.Show();
            else
                CharacterDuraPanel.Hide();
        }

        public override void ProcessRecvPacket(Packet p)
        {
            switch (p.Index)
            {
                case (short)ServerPacketIds.KeepAlive:
                    OnRecvKeepAliveHandler((ServerPacket.KeepAlive)p);
                    break;
                case (short)ServerPacketIds.MapInformation: //MapInfo
                    OnRecvMapInformationHandler((ServerPacket.MapInformation)p);
                    break;
                case (short)ServerPacketIds.NewMapInfo:
                    OnRecvNewMapInfoHandler((ServerPacket.NewMapInfo)p);
                    break;
                case (short)ServerPacketIds.WorldMapSetup:
                    OnRecvWorldMapSetupHandler((ServerPacket.WorldMapSetupInfo)p);
                    break;
                case (short)ServerPacketIds.SearchMapResult:
                    OnRecvSearchMapResultHandler((ServerPacket.SearchMapResult)p);
                    break;
                case (short)ServerPacketIds.UserInformation:
                    OnRecvUserInformationHandler((ServerPacket.UserInformation)p);
                    break;
                case (short)ServerPacketIds.UserSlotsRefresh:
                    OnRecvUserSlotsRefreshHandler((ServerPacket.UserSlotsRefresh)p);
                    break;
                case (short)ServerPacketIds.UserLocation:
                    OnRecvUserLocationHandler((ServerPacket.UserLocation)p);
                    break;
                case (short)ServerPacketIds.ObjectPlayer:
                    OnRecvObjectPlayerHandler((ServerPacket.ObjectPlayer)p);
                    break;
                case (short)ServerPacketIds.ObjectRemove:
                    OnRecvObjectRemoveHandler((ServerPacket.ObjectRemove)p);
                    break;
                case (short)ServerPacketIds.ObjectTurn:
                    OnRecvObjectTurnHandler((ServerPacket.ObjectTurn)p);
                    break;
                case (short)ServerPacketIds.ObjectRest: //add k333123 240926
                    OnRecvObjectRestHandler((ServerPacket.ObjectRest)p);
                    break; 

                case (short)ServerPacketIds.ObjectWalk:
                    OnRecvObjectWalkHandler((ServerPacket.ObjectWalk)p);
                    break;
                case (short)ServerPacketIds.ObjectRun:
                    OnRecvObjectRunHandler((ServerPacket.ObjectRun)p);
                    break;
                case (short)ServerPacketIds.Chat:
                    OnRecvReceiveChatHandler((ServerPacket.Chat)p);
                    break;
                case (short)ServerPacketIds.ObjectChat:
                    OnRecvObjectChatHandler((ServerPacket.ObjectChat)p);
                    break;
                case (short)ServerPacketIds.MoveItem:
                    OnRecvMoveItemHandler((ServerPacket.MoveItem)p);
                    break;
                case (short)ServerPacketIds.EquipItem:
                    OnRecvEquipItemHandler((ServerPacket.EquipItem)p);
                    break;
                case (short)ServerPacketIds.MergeItem:
                    OnRecvMergeItemHandler((ServerPacket.MergeItem)p);
                    break;
                case (short)ServerPacketIds.RemoveItem:
                    OnRecvRemoveItemHandler((ServerPacket.RemoveItem)p);
                    break;
                case (short)ServerPacketIds.RemoveSlotItem:
                    OnRecvRemoveSlotItemHandler((ServerPacket.RemoveSlotItem)p);
                    break;
                case (short)ServerPacketIds.TakeBackItem:
                    OnRecvTakeBackItemHandler((ServerPacket.TakeBackItem)p);
                    break;
                case (short)ServerPacketIds.StoreItem:
                    OnRecvStoreItemHandler((ServerPacket.StoreItem)p);
                    break;
                case (short)ServerPacketIds.DepositRefineItem:
                    OnRecvDepositRefineItemHandler((ServerPacket.DepositRefineItem)p);
                    break;
                case (short)ServerPacketIds.RetrieveRefineItem:
                    OnRecvRetrieveRefineItemHandler((ServerPacket.RetrieveRefineItem)p);
                    break;
                case (short)ServerPacketIds.RefineCancel:
                    OnRecvRefineCancelHandler((ServerPacket.RefineCancel)p);
                    break;
                case (short)ServerPacketIds.RefineItem:
                    OnRecvRefineItemHandler((ServerPacket.RefineItem)p);
                    break;
                case (short)ServerPacketIds.DepositTradeItem:
                    OnRecvDepositTradeItemHandler((ServerPacket.DepositTradeItem)p);
                    break;
                case (short)ServerPacketIds.RetrieveTradeItem:
                    OnRecvRetrieveTradeItemHandler((ServerPacket.RetrieveTradeItem)p);
                    break;
                case (short)ServerPacketIds.SplitItem:
                    OnRecvSplitItemHandler((ServerPacket.SplitItem)p);
                    break;
                case (short)ServerPacketIds.SplitItem1:
                    OnRecvSplitItem1Handler((ServerPacket.SplitItem1)p);
                    break;
                case (short)ServerPacketIds.UseItem:
                    OnRecvUseItemHandler((ServerPacket.UseItem)p);
                    break;
                case (short)ServerPacketIds.DropItem:
                    OnRecvDropItemHandler((ServerPacket.DropItem)p);
                    break;
                
                case (short)ServerPacketIds.PlayerUpdate:
                    OnRecvPlayerUpdateHandler((ServerPacket.PlayerUpdate)p);
                    break;
                case (short)ServerPacketIds.PlayerInspect:
                    OnRecvPlayerInspectHandler((ServerPacket.PlayerInspect)p);
                    break;
                case (short)ServerPacketIds.LogOutSuccess:
                    OnRecvLogOutSuccessHandler((ServerPacket.LogOutSuccess)p);
                    break;
                case (short)ServerPacketIds.LogOutFailed:
                    OnRecvLogOutFailedHandler((ServerPacket.LogOutFailed)p);
                    break;
                case (short)ServerPacketIds.ReturnToLogin:
                    OnRecvReturnToLoginHandler((ServerPacket.ReturnToLogin)p);
                    break;
                case (short)ServerPacketIds.TimeOfDay:
                    OnRecvTimeOfDayHandler((ServerPacket.TimeOfDay)p);
                    break;
                case (short)ServerPacketIds.ChangeAMode:
                    OnRecvChangeAModeHandler((ServerPacket.ChangeAMode)p);
                    break;
                case (short)ServerPacketIds.ChangePMode:
                    OnRecvChangePModeHandler((ServerPacket.ChangePMode)p);
                    break;
                case (short)ServerPacketIds.ObjectItem:
                    OnRecvObjectItemHandler((ServerPacket.ObjectItem)p);
                    break;
                case (short)ServerPacketIds.ObjectGold:
                    OnRecvObjectGoldHandler((ServerPacket.ObjectGold)p);
                    break;
                case (short)ServerPacketIds.GainedItem:
                    OnRecvGainedItemHandler((ServerPacket.GainedItem)p);
                    break;
                case (short)ServerPacketIds.GainedGold:
                    OnRecvGainedGoldHandler((ServerPacket.GainedGold)p);
                    break;
                case (short)ServerPacketIds.LoseGold:
                    OnRecvLoseGoldHandler((ServerPacket.LoseGold)p);
                    break;
                case (short)ServerPacketIds.GainedCredit:
                    OnRecvGainedCreditHandler((ServerPacket.GainedCredit)p);
                    break;
                case (short)ServerPacketIds.LoseCredit:
                    OnRecvLoseCreditHandler((ServerPacket.LoseCredit)p);
                    break;
                case (short)ServerPacketIds.ObjectMonster:
                    OnRecvObjectMonsterHandler((ServerPacket.ObjectMonster)p);
                    break;
                case (short)ServerPacketIds.ObjectAttack:
                    OnRecvObjectAttackHandler((ServerPacket.ObjectAttack)p);
                    break;
                case (short)ServerPacketIds.Struck:
                    OnRecvStruckHandler((ServerPacket.Struck)p);
                    break;
                case (short)ServerPacketIds.DamageIndicator:
                    OnRecvDamageIndicatorHandler((ServerPacket.DamageIndicator)p);
                    break;
                case (short)ServerPacketIds.ObjectStruck:
                    OnRecvObjectStruckHandler((ServerPacket.ObjectStruck)p);
                    break;
                case (short)ServerPacketIds.DuraChanged:
                    OnRecvDuraChangedHandler((ServerPacket.DuraChanged)p);
                    break;
                case (short)ServerPacketIds.HealthChanged:
                    OnRecvHealthChangedHandler((ServerPacket.HealthChanged)p);
                    break;
                case (short)ServerPacketIds.DeleteItem:
                    OnRecvDeleteItemHandler((ServerPacket.DeleteItem)p);
                    break;
                case (short)ServerPacketIds.Death:
                    OnRecvDeathHandler((ServerPacket.Death)p);
                    break;
                case (short)ServerPacketIds.ObjectDied:
                    OnRecvObjectDiedHandler((ServerPacket.ObjectDied)p);
                    break;
                case (short)ServerPacketIds.ColourChanged:
                    OnRecvColourChangedHandler((ServerPacket.ColourChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectColourChanged:
                    OnRecvObjectColourChangedHandler((ServerPacket.ObjectColourChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectGuildNameChanged:
                    OnRecvObjectGuildNameChangedHandler((ServerPacket.ObjectGuildNameChanged)p);
                    break;
                case (short)ServerPacketIds.GainExperience:
                    OnRecvGainExperienceHandler((ServerPacket.GainExperience)p);
                    break;
                case (short)ServerPacketIds.LevelChanged:
                    OnRecvLevelChangedHandler((ServerPacket.LevelChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectLeveled:
                    OnRecvObjectLeveledHandler((ServerPacket.ObjectLeveled)p);
                    break;
                case (short)ServerPacketIds.ObjectHarvest:
                    OnRecvObjectHarvestHandler((ServerPacket.ObjectHarvest)p);
                    break;
                case (short)ServerPacketIds.ObjectHarvested:
                    OnRecvObjectHarvestedHandler((ServerPacket.ObjectHarvested)p);
                    break;
                case (short)ServerPacketIds.ObjectNpc:
                    OnRecvObjectNPCHandler((ServerPacket.ObjectNPC)p);
                    break;
                case (short)ServerPacketIds.NPCResponse:
                    OnRecvNPCResponseHandler((ServerPacket.NPCResponse)p);
                    break;
                case (short)ServerPacketIds.ObjectHide:
                    OnRecvObjectHideHandler((ServerPacket.ObjectHide)p);
                    break;
                case (short)ServerPacketIds.ObjectShow:
                    OnRecvObjectShowHandler((ServerPacket.ObjectShow)p);
                    break;
                case (short)ServerPacketIds.Poisoned:
                    OnRecvPoisonedHandler((ServerPacket.Poisoned)p);
                    break;
                case (short)ServerPacketIds.ObjectPoisoned:
                    OnRecvObjectPoisonedHandler((ServerPacket.ObjectPoisoned)p);
                    break;
                case (short)ServerPacketIds.MapChanged:
                    OnRecvMapChangedHandler((ServerPacket.MapChanged)p);
                    break;
                case (short)ServerPacketIds.ObjectTeleportOut:
                    OnRecvObjectTeleportOutHandler((ServerPacket.ObjectTeleportOut)p);
                    break;
                case (short)ServerPacketIds.ObjectTeleportIn:
                    OnRecvObjectTeleportInHandler((ServerPacket.ObjectTeleportIn)p);
                    break;
                case (short)ServerPacketIds.TeleportIn:
                    OnRecvTeleportInHandler();
                    break;
                case (short)ServerPacketIds.NPCGoods:
                    OnRecvNPCGoodsHandler((ServerPacket.NPCGoods)p);
                    break;
                case (short)ServerPacketIds.NPCSell:
                    OnRecvNPCSellHandler();
                    break;
                case (short)ServerPacketIds.NPCRepair:
                    OnRecvNPCRepairHandler((ServerPacket.NPCRepair)p);
                    break;
                case (short)ServerPacketIds.NPCSRepair:
                    OnRecvNPCSRepairHandler((ServerPacket.NPCSRepair)p);
                    break;
                case (short)ServerPacketIds.NPCRefine:
                    OnRecvNPCRefineHandler((ServerPacket.NPCRefine)p);
                    break;
                case (short)ServerPacketIds.NPCCheckRefine:
                    OnRecvNPCCheckRefineHandler((ServerPacket.NPCCheckRefine)p);
                    break;
                case (short)ServerPacketIds.NPCCollectRefine:
                    OnRecvNPCCollectRefineHandler((ServerPacket.NPCCollectRefine)p);
                    break;
                case (short)ServerPacketIds.NPCReplaceWedRing:
                    OnRecvNPCReplaceWedRingHandler((ServerPacket.NPCReplaceWedRing)p);
                    break;
                case (short)ServerPacketIds.NPCStorage:
                    OnRecvNPCStorageHandler();
                    break;
                case (short)ServerPacketIds.NPCRequestInput:
                    OnRecvNPCRequestInputHandler((ServerPacket.NPCRequestInput)p);
                    break;
                case (short)ServerPacketIds.SellItem:
                    OnRecvSellItemHandler((ServerPacket.SellItem)p);
                    break;
                case (short)ServerPacketIds.CraftItem:
                    OnRecvCraftItemHandler((ServerPacket.CraftItem)p);
                    break;
                case (short)ServerPacketIds.RepairItem:
                    OnRecvRepairItemHandler((ServerPacket.RepairItem)p);
                    break;
                case (short)ServerPacketIds.ItemRepaired:
                    OnRecvItemRepairedHandler((ServerPacket.ItemRepaired)p);
                    break;
                case (short)ServerPacketIds.ItemSlotSizeChanged:
                    OnRecvItemSlotSizeChangedHandler((ServerPacket.ItemSlotSizeChanged)p);
                    break;
                case (short)ServerPacketIds.ItemSealChanged:
                    OnRecvItemSealChangedHandler((ServerPacket.ItemSealChanged)p);
                    break;
                case (short)ServerPacketIds.NewMagic:
                    OnRecvNewMagicHandler((ServerPacket.NewMagic)p);
                    break;
                case (short)ServerPacketIds.MagicLeveled:
                    OnRecvMagicLeveledHandler((ServerPacket.MagicLeveled)p);
                    break;
                case (short)ServerPacketIds.Magic:
                    OnRecvMagicHandler((ServerPacket.Magic)p);
                    break;
                case (short)ServerPacketIds.MagicDelay:
                    OnRecvMagicDelayHandler((ServerPacket.MagicDelay)p);
                    break;
                case (short)ServerPacketIds.MagicCast:
                    OnRecvMagicCastHandler((ServerPacket.MagicCast)p);
                    break;
                case (short)ServerPacketIds.ObjectMagic:
                    OnRecvObjectMagicHandler((ServerPacket.ObjectMagic)p);
                    break;
                case (short)ServerPacketIds.ObjectProjectile:
                    OnRecvObjectProjectileHandler((ServerPacket.ObjectProjectile)p);
                    break;
                case (short)ServerPacketIds.ObjectEffect:
                    OnRecvObjectEffectHandler((ServerPacket.ObjectEffect)p);
                    break;
                case (short)ServerPacketIds.RangeAttack:
                    OnRecvRangeAttackHandler((ServerPacket.RangeAttack)p);
                    break;
                case (short)ServerPacketIds.Pushed:
                    OnRecvPushedHandler((ServerPacket.Pushed)p);
                    break;
                case (short)ServerPacketIds.ObjectPushed:
                    OnRecvObjectPushedHandler((ServerPacket.ObjectPushed)p);
                    break;
                case (short)ServerPacketIds.ObjectName:
                    OnRecvObjectNameHandler((ServerPacket.ObjectName)p);
                    break;
                case (short)ServerPacketIds.UserStorage:
                    OnRecvUserStorageHandler((ServerPacket.UserStorage)p);
                    break;
                case (short)ServerPacketIds.SwitchGroup:
                    OnRecvSwitchGroupHandler((ServerPacket.SwitchGroup)p);
                    break;
                case (short)ServerPacketIds.DeleteGroup:
                    OnRecvDeleteGroupHandler();
                    break;
                case (short)ServerPacketIds.DeleteMember:
                    OnRecvDeleteMemberHandler((ServerPacket.DeleteMember)p);
                    break;
                case (short)ServerPacketIds.GroupInvite:
                    OnRecvGroupInviteHandler((ServerPacket.GroupInvite)p);
                    break;
                case (short)ServerPacketIds.AddMember:
                    OnRecvAddMemberHandler((ServerPacket.AddMember)p);
                    break;
                case (short)ServerPacketIds.GroupMembersMap:
                    OnRecvGroupMembersMapHandler((ServerPacket.GroupMembersMap)p);
                    break;
                case (short)ServerPacketIds.SendMemberLocation:
                    OnRecvSendMemberLocationHandler((ServerPacket.SendMemberLocation)p);
                    break;
                case (short)ServerPacketIds.Revived:
                    OnRecvRevivedHandler();
                    break;
                case (short)ServerPacketIds.ObjectRevived:
                    OnRecvObjectRevivedHandler((ServerPacket.ObjectRevived)p);
                    break;
                case (short)ServerPacketIds.SpellToggle:
                    OnRecvSpellToggleHandler((ServerPacket.SpellToggle)p);
                    break;
                case (short)ServerPacketIds.ObjectHealth:
                    OnRecvObjectHealthHandler((ServerPacket.ObjectHealth)p);
                    break;
                case (short)ServerPacketIds.ObjectMana:
                    OnRecvObjectManaHandler((ServerPacket.ObjectMana)p);
                    break;
                case (short)ServerPacketIds.MapEffect:
                    OnRecvMapEffectHandler((ServerPacket.MapEffect)p);
                    break;
                case (short)ServerPacketIds.AllowObserve:
                    AllowObserve = ((ServerPacket.AllowObserve)p).Allow;
                    break;
                case (short)ServerPacketIds.ObjectRangeAttack:
                    OnRecvObjectRangeAttackHandler((ServerPacket.ObjectRangeAttack)p);
                    break;
                case (short)ServerPacketIds.AddBuff:
                    OnRecvAddBuffHandler((ServerPacket.AddBuff)p);
                    break;
                case (short)ServerPacketIds.RemoveBuff:
                    OnRecvRemoveBuffHandler((ServerPacket.RemoveBuff)p);
                    break;
                case (short)ServerPacketIds.PauseBuff:
                    OnRecvPauseBuffHandler((ServerPacket.PauseBuff)p);
                    break;
                case (short)ServerPacketIds.ObjectHidden:
                    OnRecvObjectHiddenHandler((ServerPacket.ObjectHidden)p);
                    break;
                case (short)ServerPacketIds.RefreshItem:
                    OnRecvRefreshItemHandler((ServerPacket.RefreshItem)p);
                    break;
                case (short)ServerPacketIds.ObjectSpell:
                    OnRecvObjectSpellHandler((ServerPacket.ObjectSpell)p);
                    break;
                case (short)ServerPacketIds.UserDash:
                    OnRecvUserDashHandler((ServerPacket.UserDash)p);
                    break;
                case (short)ServerPacketIds.ObjectDash:
                    OnRecvObjectDashHandler((ServerPacket.ObjectDash)p);
                    break;
                case (short)ServerPacketIds.UserDashFail:
                    OnRecvUserDashFailHandler((ServerPacket.UserDashFail)p);
                    break;
                case (short)ServerPacketIds.ObjectDashFail:
                    OnRecvObjectDashFailHandler((ServerPacket.ObjectDashFail)p);
                    break;
                case (short)ServerPacketIds.NPCConsign:
                    OnRecvNPCConsignHandler();
                    break;
                 
                case (short)ServerPacketIds.ConsignItem:
                    OnRecvConsignItemHandler((ServerPacket.ConsignItem)p);
                    break;
                 
                case (short)ServerPacketIds.ObjectSitDown:
                    OnRecvObjectSitDownHandler((ServerPacket.ObjectSitDown)p);
                    break;
                case (short)ServerPacketIds.InTrapRock:
                    ServerPacket.InTrapRock packetdata = (ServerPacket.InTrapRock)p;
                    User.InTrapRock = packetdata.Trapped;
                    break;
                case (short)ServerPacketIds.RemoveMagic:
                    OnRecvRemoveMagicHandler((ServerPacket.RemoveMagic)p);
                    break;
                case (short)ServerPacketIds.BaseStatsInfo:
                    OnRecvBaseStatsInfoHandler((ServerPacket.BaseStatsInfo)p);
                    break;
                case (short)ServerPacketIds.UserName:
                    OnRecvUserNameHandler((ServerPacket.UserName)p);
                    break;
                case (short)ServerPacketIds.ChatItemStats:
                    OnRecvChatItemStatsHandler((ServerPacket.ChatItemStats)p);
                    break;
                case (short)ServerPacketIds.GuildInvite:
                    OnRecvGuildInviteHandler((ServerPacket.GuildInvite)p);
                    break;
                case (short)ServerPacketIds.GuildMemberChange:
                    OnRecvGuildMemberChangeHandler((ServerPacket.GuildMemberChange)p);
                    break;
                case (short)ServerPacketIds.GuildNoticeChange:
                    OnRecvGuildNoticeChangeHandler((ServerPacket.GuildNoticeChange)p);
                    break;
                case (short)ServerPacketIds.GuildStatus:
                    OnRecvGuildStatusHandler((ServerPacket.GuildStatus)p);
                    break;
                case (short)ServerPacketIds.GuildExpGain:
                    OnRecvGuildExpGainHandler((ServerPacket.GuildExpGain)p);
                    break;
                case (short)ServerPacketIds.GuildNameRequest:
                    OnRecvGuildNameRequestHandler((ServerPacket.GuildNameRequest)p);
                    break;
                case (short)ServerPacketIds.GuildStorageGoldChange:
                    OnRecvGuildStorageGoldChangeHandler((ServerPacket.GuildStorageGoldChange)p);
                    break;
                case (short)ServerPacketIds.GuildStorageItemChange:
                    OnRecvGuildStorageItemChangeHandler((ServerPacket.GuildStorageItemChange)p);
                    break;
                case (short)ServerPacketIds.GuildStorageList:
                    OnRecvGuildStorageListHandler((ServerPacket.GuildStorageList)p);
                    break;
                case (short)ServerPacketIds.GuildRequestWar:
                    OnRecvGuildRequestWarHandler((ServerPacket.GuildRequestWar)p);
                    break;
                case (short)ServerPacketIds.DefaultNPC:
                    OnRecvDefaultNPCHandler((ServerPacket.DefaultNPC)p);
                    break;
                case (short)ServerPacketIds.NPCUpdate:
                    OnRecvNPCUpdateHandler((ServerPacket.NPCUpdate)p);
                    break;
                case (short)ServerPacketIds.NPCImageUpdate:
                    OnRecvNPCImageUpdateHandler((ServerPacket.NPCImageUpdate)p);
                    break;
                case (short)ServerPacketIds.MarriageRequest:
                    OnRecvMarriageRequestHandler((ServerPacket.MarriageRequest)p);
                    break;
                case (short)ServerPacketIds.DivorceRequest:
                    OnRecvDivorceRequestHandler((ServerPacket.DivorceRequest)p);
                    break;
                case (short)ServerPacketIds.MentorRequest:
                    OnRecvMentorRequestHandler((ServerPacket.MentorRequest)p);
                    break;
                case (short)ServerPacketIds.TradeRequest:
                    OnRecvTradeRequestHandler((ServerPacket.TradeRequest)p);
                    break;
                case (short)ServerPacketIds.TradeAccept:
                    OnRecvTradeAcceptHandler((ServerPacket.TradeAccept)p);
                    break;
                case (short)ServerPacketIds.TradeGold:
                    OnRecvTradeGoldHandler((ServerPacket.TradeGold)p);
                    break;
                case (short)ServerPacketIds.TradeItem:
                    OnRecvTradeItemHandler((ServerPacket.TradeItem)p);
                    break;
                case (short)ServerPacketIds.TradeConfirm:
                    OnRecvTradeConfirm();
                    break;
                case (short)ServerPacketIds.TradeCancel:
                    OnRecvTradeCancelHandler((ServerPacket.TradeCancel)p);
                    break;
                 
                case (short)ServerPacketIds.TransformUpdate:
                    OnRecvTransformUpdateHandler((ServerPacket.TransformUpdate)p);
                    break;
                case (short)ServerPacketIds.EquipSlotItem:
                    OnRecvEquipSlotItemHandler((ServerPacket.EquipSlotItem)p);
                    break;
                case (short)ServerPacketIds.FishingUpdate: 
                    break;
                case (short)ServerPacketIds.ChangeQuest:
                    OnRecvChangeQuestHandler((ServerPacket.ChangeQuest)p);
                    break;
                case (short)ServerPacketIds.CompleteQuest:
                    OnRecvCompleteQuestHandler((ServerPacket.CompleteQuest)p);
                    break;
                case (short)ServerPacketIds.ShareQuest:
                    OnRecvShareQuestHandler((ServerPacket.ShareQuest)p);
                    break;
                case (short)ServerPacketIds.GainedQuestItem:
                    OnRecvGainedQuestItemHandler((ServerPacket.GainedQuestItem)p);
                    break;
                case (short)ServerPacketIds.DeleteQuestItem:
                    OnRecvDeleteQuestItemHandler((ServerPacket.DeleteQuestItem)p);
                    break;
                case (short)ServerPacketIds.CancelReincarnation:
                    User.ReincarnationStopTime = 0;
                    break;
                case (short)ServerPacketIds.RequestReincarnation:
                    if (!User.Dead) return;
                    OnRecvRequestReincarnationHandler();
                    break;
                case (short)ServerPacketIds.UserBackStep:
                    OnRecvUserBackStepHandler((ServerPacket.UserBackStep)p);
                    break;
                case (short)ServerPacketIds.ObjectBackStep:
                    OnRecvObjectBackStepHandler((ServerPacket.ObjectBackStep)p);
                    break;
                case (short)ServerPacketIds.UserDashAttack:
                    OnRecvUserDashAttackHandler((ServerPacket.UserDashAttack)p);
                    break;
                case (short)ServerPacketIds.ObjectDashAttack:
                    OnRecvObjectDashAttackHandler((ServerPacket.ObjectDashAttack)p);
                    break;
                case (short)ServerPacketIds.UserAttackMove://Warrior Skill - SlashingBurst
                    OnRecvUserAttackMoveHandler((ServerPacket.UserAttackMove)p);
                    break;
                case (short)ServerPacketIds.CombineItem:
                    OnRecvCombineItemHandler((ServerPacket.CombineItem)p);
                    break;
                case (short)ServerPacketIds.ItemUpgraded:
                    OnRecvItemUpgradedHandler((ServerPacket.ItemUpgraded)p);
                    break;
                case (short)ServerPacketIds.SetConcentration:
                    OnRecvSetConcentrationHandler((ServerPacket.SetConcentration)p);
                    break;
                case (short)ServerPacketIds.SetElemental:
                    OnRecvSetElementalHandler((ServerPacket.SetElemental)p);
                    break;
                case (short)ServerPacketIds.RemoveDelayedExplosion:
                    OnRecvRemoveDelayedExplosionHandler((ServerPacket.RemoveDelayedExplosion)p);
                    break;
                case (short)ServerPacketIds.ObjectDeco:
                    OnRecvObjectDecoHandler((ServerPacket.ObjectDeco)p);
                    break;
                case (short)ServerPacketIds.ObjectSneaking:
                    OnRecvObjectSneakingHandler((ServerPacket.ObjectSneaking)p);
                    break;
                case (short)ServerPacketIds.ObjectLevelEffects:
                    OnRecvObjectLevelEffectsHandler((ServerPacket.ObjectLevelEffects)p);
                    break;
                case (short)ServerPacketIds.SetBindingShot:
                    OnRecvSetBindingShotHandler((ServerPacket.SetBindingShot)p);
                    break;
                case (short)ServerPacketIds.SendOutputMessage:
                    OnRecvSendOutputMessageHandler((ServerPacket.SendOutputMessage)p);
                    break;
               
                 
                case (short)ServerPacketIds.ResizeInventory:
                    OnRecvResizeInventoryHandler((ServerPacket.ResizeInventory)p);
                    break;
                case (short)ServerPacketIds.ResizeStorage:
                    OnRecvResizeStorageHandler((ServerPacket.ResizeStorage)p);
                    break;
               
                case (short)ServerPacketIds.NPCPearlGoods:
                    OnRecvNPCPearlGoodsHandler((ServerPacket.NPCPearlGoods)p);
                    break;
                case (short)ServerPacketIds.FriendUpdate:
                    OnRecvFriendUpdateHandler((ServerPacket.FriendUpdate)p);
                    break;
                case (short)ServerPacketIds.LoverUpdate:
                    OnRecvLoverUpdateHandler((ServerPacket.LoverUpdate)p);
                    break;
                case (short)ServerPacketIds.MentorUpdate:
                    OnRecvMentorUpdateHandler((ServerPacket.MentorUpdate)p);
                    break;
                case (short)ServerPacketIds.GuildBuffList:
                    OnRecvGuildBuffListHandler((ServerPacket.GuildBuffList)p);
                    break;
                case (short)ServerPacketIds.GameShopInfo:
                     
                    break;
                case (short)ServerPacketIds.GameShopStock:
                     
                    break;
                case (short)ServerPacketIds.Rankings:
                    OnRecvRankingsHandler((ServerPacket.Rankings)p);
                    break;
                case (short)ServerPacketIds.Opendoor:
                    OnRecvOpendoorHandler((ServerPacket.Opendoor)p);
                    break;
                
                case (short)ServerPacketIds.OpenBrowser:
                    OnRecvOpenBrowserHandler((ServerPacket.OpenBrowser)p);
                    break;
                case (short)ServerPacketIds.PlaySound:
                    OnRecvPlaySoundHandler((ServerPacket.PlaySound)p);
                    break;
                case (short)ServerPacketIds.SetTimer:
                    OnRecvSetTimerHandler((ServerPacket.SetTimer)p);
                    break;
                case (short)ServerPacketIds.ExpireTimer:
                    OnRecvExpireTimerHandler((ServerPacket.ExpireTimer)p);
                    break;
                case (short)ServerPacketIds.UpdateNotice:
                    OnRecvShowNoticeHandler((ServerPacket.UpdateNotice)p);
                    break;
                case (short)ServerPacketIds.Roll:
                    OnRecvRollHandler((ServerPacket.Roll)p);
                    break;
                case (short)ServerPacketIds.SetCompass:
                    OnRecvSetCompassHandler((ServerPacket.SetCompass)p);
                    break;
                default:
                    base.ProcessRecvPacket(p);
                    break;
            }
        }

        #region OnRecv From Server Handler 
        private void OnRecvKeepAliveHandler(ServerPacket.KeepAlive p)
        {
            if (p.Time == 0) return;
            CMain.PingTime = (CMain.Time - p.Time);
        }
       
        private void OnRecvMapInformationHandler(ServerPacket.MapInformation p)
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.Dispose();
            MapControl = new MapControl { Index = p.MapIndex, FileName = Path.Combine(Settings.MapPath, p.FileName + ".map"), Title = p.Title, MiniMap = p.MiniMap, BigMap = p.BigMap, Lights = p.Lights, Lightning = p.Lightning, Fire = p.Fire, MapDarkLight = p.MapDarkLight, Music = p.Music };
            MapControl.LoadMap();
            InsertControl(0, MapControl);
        }

        private void OnRecvWorldMapSetupHandler(ServerPacket.WorldMapSetupInfo info)
        {
            BigMapDialog.WorldMapSetup(info.Setup);
            TeleportToNPCCost = info.TeleportToNPCCost;
        }

        private void OnRecvNewMapInfoHandler(ServerPacket.NewMapInfo info)
        {
            BigMapRecord newRecord = new BigMapRecord() { Index = info.MapIndex, MapInfo = info.Info };
            CreateBigMapButtons(newRecord);
            MapInfoList.Add(info.MapIndex, newRecord);
        }

        private void CreateBigMapButtons(BigMapRecord record)
        {
            record.MovementButtons.Clear();
            record.NPCButtons.Clear();

            foreach (ClientMovementInfo mInfo in record.MapInfo.Movements)
            {
                ExineButton button = new ExineButton()
                {
                    Library = Libraries.MapLinkIcon,
                    Index = mInfo.Icon,
                    PressedIndex = mInfo.Icon,
                    Sound = SoundList.ButtonA,
                    Parent = BigMapDialog.ViewPort,
                    Location = new Point(20, 38),
                    Hint = mInfo.Title,
                    Visible = false
                };
                button.MouseEnter += (o, e) =>
                {
                    BigMapDialog.MouseLocation = mInfo.Location;
                };

                button.Click += (o, e) =>
                {
                    BigMapDialog.SetTargetMap(mInfo.Destination);
                };
                record.MovementButtons.Add(mInfo, button);
            }

            foreach (ClientNPCInfo npcInfo in record.MapInfo.NPCs)
            {
                BigMapNPCRow row = new BigMapNPCRow(npcInfo) { Parent = BigMapDialog };
                record.NPCButtons.Add(row);
            }
        }

        private void RecreateBigMapButtons()
        {
            foreach (var record in MapInfoList.Values)
                CreateBigMapButtons(record);
        }

        private void OnRecvSearchMapResultHandler(ServerPacket.SearchMapResult info)
        {
            if (info.MapIndex == -1 && info.NPCIndex == 0)
            {
                ExineMessageBox messageBox = new ExineMessageBox("아무것도 발견되지 않았습니다.", MirMessageBoxButtons.OK);
                messageBox.OKButton.Click += (o, a) =>
                {
                    BigMapDialog.SearchTextBox.SetFocus();
                };
                messageBox.Show();
                return;
            }

            BigMapDialog.SetTargetMap(info.MapIndex);
            BigMapDialog.SetTargetNPC(info.NPCIndex);
        }

        public byte[] GetBytesFromJpg(string path)
        {
            //max 8000bytes`
            //\Exine\RData\Profiles\charname.jpg  72*72
            Stream jpgStream = File.Open(Application.StartupPath + path, FileMode.Open);
            Image image = Image.FromStream(jpgStream);
            var stream = new MemoryStream();
            //image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

            byte[] imageBytes = stream.ToArray();
            Console.WriteLine("imageBytes[0] : {0}, imageBytes.Length:{1}", imageBytes[0], imageBytes.Length);
            jpgStream.Close();
            return imageBytes;
        }

        private void OnRecvUserInformationHandler(ServerPacket.UserInformation p)
        {
            User = new UserObject(p.ObjectID); 
            User.Load(p);
             

            Console.WriteLine("***111222 File.Exists Check " + User.Name + ".jpg");

            if (File.Exists(User.Name + ".jpg"))
            {
                Console.WriteLine("***222333 File.Exists!");
                byte[] tempPhotoData = GetBytesFromJpg(User.Name + ".jpg");
                User.ExPortraitLen = tempPhotoData.Length;

                if (tempPhotoData.Length <= 8000)
                {
                    Buffer.BlockCopy(tempPhotoData, 0, User.ExPortraitBytes, 0, tempPhotoData.Length);
                    Console.WriteLine("Send C.UpdatePhoto from UserInformation!!!");


                    //k333123 240307 now!
                    //SendToServer(ClientPacket.UpdatePhoto)

                    Console.WriteLine("@@@444 UpdatePhoto");
                    Network.SendPacketToServer(new ClientPacket.UpdatePhoto { photoData = User.ExPortraitBytes, photoDataLen = User.ExPortraitLen }); //this cause server disconnect!

                }
            } 

            ExMainDialog.PModeLabel.Visible = User.Class == ExineClass.Wizard || User.Class == ExineClass.Taoist; 
            Gold = p.Gold;
            Credit = p.Credit;

            ExCharacterDialog = new ExineCharacterDialog(MirGridType.Equipment, User) { Parent = this, Visible = false };
            ExStateDialog = new ExineStateDialog(MirGridType.Equipment, User) { Parent = this, Visible = false };
            ExInventoryDialog.RefreshInventory();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
                Bar.Update();
            AllowObserve = p.AllowObserve;
            Observing = p.Observer;
        }
        
        private void OnRecvUserSlotsRefreshHandler(ServerPacket.UserSlotsRefresh p)
        {
            User.SetSlots(p);
        }

        private void OnRecvUserLocationHandler(ServerPacket.UserLocation p)
        {
            MapControl.NextAction = 0;
            if (User.CurrentLocation == p.Location && User.Direction == p.Direction) return;

            if (Settings.DebugMode)
            {
                OnRecvReceiveChatHandler(new ServerPacket.Chat { Message = "Displacement", Type = ChatType.System });
            }

            MapControl.RemoveObject(User);
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;

            if (User.Dead) return;

            User.ClearMagic();
            User.QueuedAction = null;

            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == ExAction.Pushed) continue;
                User.ActionFeed.RemoveAt(i);
            }

            User.SetAction();
        }
       
        private void OnRecvReceiveChatHandler(ServerPacket.Chat p)
        {
            ExChatDialog.ReceiveChat(p.Message, p.Type);
        }
        
        private void OnRecvObjectPlayerHandler(ServerPacket.ObjectPlayer p)
        { 
            PlayerObject player = new PlayerObject(p.ObjectID);
            player.Load(p); 
        }

        private void OnRecvObjectRemoveHandler(ServerPacket.ObjectRemove p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Remove();
            }
        }

        private void OnRecvObjectTurnHandler(ServerPacket.ObjectTurn p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

            
            //!!!!!!! //standing!!! human ->  ExAction.ONEHAND_STAND or ExAction.TWOHAND_STAND or ExAction.BOWHAND_STAND, Mob -> ExAction.standing 
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob is PlayerObject) //add 240926 k333123
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = p.Direction, Location = p.Location });
                }
                else
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Standing, Direction = p.Direction, Location = p.Location });
                }

                return;
            }
        }

        
        private void OnRecvObjectRestHandler(ServerPacket.ObjectRest p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

             
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob is PlayerObject) //add 240926 k333123
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.PEACEMODE_SITDOWN, Direction = p.Direction, Location = p.Location });
                } 
                return;
            }
        }

        private void OnRecvObjectWalkHandler(ServerPacket.ObjectWalk p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;
              
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob is PlayerObject) //add 240926 k333123
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = p.Direction, Location = p.Location });
                }
                else
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Walking, Direction = p.Direction, Location = p.Location });
                }
                return;
            }
        }
        
        private void OnRecvObjectRunHandler(ServerPacket.ObjectRun p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;
             
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob is PlayerObject) //add 240926 k333123
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.ONEHAND_RUN_LEFT, Direction = p.Direction, Location = p.Location });
                }
                else
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Running, Direction = p.Direction, Location = p.Location });
                }
                return;
            }
        }
        
        private void OnRecvObjectChatHandler(ServerPacket.ObjectChat p)
        {
            ExChatDialog.ReceiveChat(p.Text, p.Type);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                //add k333123 
                if (ob is UserObject)//Me
                {
                    ob.Chat(RegexFunctions.CleanChatString(p.Text),0, ob);
                }
                else if (ob is PlayerObject)//Other Player
                {
                    ob.Chat(RegexFunctions.CleanChatString(p.Text),3, ob); 
                }

                else if (ob is NPCObject)
                {
                    ob.Chat(RegexFunctions.CleanChatString(p.Text),2, ob);
                }
                else
                {
                    ob.Chat(RegexFunctions.CleanChatString(p.Text),1, ob);
                }
                return;
            }

        }
       
        private void OnRecvMoveItemHandler(ServerPacket.MoveItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.Grid[p.From];
                    break;
                case MirGridType.Trade:
                    fromCell = TradeDialog.Grid[p.From];
                    break;
                case MirGridType.Refine:
                    fromCell = RefineDialog.Grid[p.From];
                    break;
                default:
                    return;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                case MirGridType.Trade:
                    toCell = TradeDialog.Grid[p.To];
                    break;
                case MirGridType.Refine:
                    toCell = RefineDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.Grid == MirGridType.Trade)
                TradeDialog.ChangeLockState(false);

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;

            User.RefreshStats();
            CharacterDuraPanel.GetCharacterDura();
        }
        
        private void OnRecvEquipItemHandler(ServerPacket.EquipItem p)
        {
            MirItemCell fromCell, toCell;

            switch (p.Grid)
            {
                default:
                    toCell = ExCharacterDialog.Grid[p.To];
                    break;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;
            CharacterDuraPanel.UpdateCharacterDura(i);
            User.RefreshStats();
        }
       
        //hero equipment item
        private void OnRecvEquipSlotItemHandler(ServerPacket.EquipSlotItem p)
        {
            MirItemCell fromCell;
            MirItemCell toCell;

            switch (p.GridTo)
            {
                case MirGridType.Socket:
                    toCell = SocketDialog.Grid[p.To];
                    break;
                
                 
                default:
                    return;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            //if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = null;
            toCell.Item = i;
            User.RefreshStats();
        }

        private void OnRecvCombineItemHandler(ServerPacket.CombineItem p)
        {
            MirItemCell fromCell = null;
            MirItemCell toCell = null;
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = ExInventoryDialog.GetCell(p.IDFrom) ?? BeltDialog.GetCell(p.IDFrom);
                    toCell = ExInventoryDialog.GetCell(p.IDTo) ?? BeltDialog.GetCell(p.IDTo);
                    break;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.Destroy) toCell.Item = null;

            if (!p.Success) return;

            fromCell.Item = null;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    User.RefreshStats();
                    break;
            }
        }

        private void OnRecvMergeItemHandler(ServerPacket.MergeItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.GridFrom)
            {
                case MirGridType.Inventory:
                    fromCell = ExInventoryDialog.GetCell(p.IDFrom) ?? BeltDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Equipment:
                    fromCell = ExCharacterDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Trade:
                    fromCell = TradeDialog.GetCell(p.IDFrom);
                    break; 
                default:
                    return;
            }

            switch (p.GridTo)
            {
                case MirGridType.Inventory:
                    toCell = ExInventoryDialog.GetCell(p.IDTo) ?? BeltDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Equipment:
                    toCell = ExCharacterDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Trade:
                    toCell = TradeDialog.GetCell(p.IDTo);
                    break; 
                
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (p.GridFrom == MirGridType.Trade || p.GridTo == MirGridType.Trade)
                TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            if (fromCell.Item.Count <= toCell.Item.Info.StackSize - toCell.Item.Count)
            {
                toCell.Item.Count += fromCell.Item.Count;
                fromCell.Item = null;
            }
            else
            {
                fromCell.Item.Count -= (ushort)(toCell.Item.Info.StackSize - toCell.Item.Count);
                toCell.Item.Count = toCell.Item.Info.StackSize;
            }

            User.RefreshStats();
        }
       
        private void OnRecvRemoveItemHandler(ServerPacket.RemoveItem p)
        {
            MirItemCell toCell;

            int index = -1;
            MirItemCell fromCell = null;
            for (int i = 0; i < MapObject.User.Equipment.Length; i++)
            {
                if (MapObject.User.Equipment[i] == null || MapObject.User.Equipment[i].UniqueID != p.UniqueID) continue;
                index = i;
                fromCell = ExCharacterDialog.Grid[index];
                break;
            }

           
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
               
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            CharacterDuraPanel.GetCharacterDura();
            User.RefreshStats();

        }
       
        private void OnRecvRemoveSlotItemHandler(ServerPacket.RemoveSlotItem p)
        {
            MirItemCell fromCell;
            MirItemCell toCell;

            switch (p.Grid)
            {
                case MirGridType.Socket:
                    fromCell = SocketDialog.GetCell(p.UniqueID);
                    break;
                 
                default:
                    return;
            }

            switch (p.GridTo)
            {
                case MirGridType.Inventory:
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            CharacterDuraPanel.GetCharacterDura();
            User.RefreshStats();
        }
       
        private void OnRecvTakeBackItemHandler(ServerPacket.TakeBackItem p)
        {
            MirItemCell fromCell = StorageDialog.Grid[p.From];

            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
            CharacterDuraPanel.GetCharacterDura();
        }
      
        private void OnRecvStoreItemHandler(ServerPacket.StoreItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = StorageDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void OnRecvDepositRefineItemHandler(ServerPacket.DepositRefineItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = RefineDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }

        private void OnRecvRetrieveRefineItemHandler(ServerPacket.RetrieveRefineItem p)
        {
            MirItemCell fromCell = RefineDialog.Grid[p.From];
            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }

        private void OnRecvRefineCancelHandler(ServerPacket.RefineCancel p)
        {
            RefineDialog.RefineReset();
        }

        private void OnRecvRefineItemHandler(ServerPacket.RefineItem p)
        {
            RefineDialog.RefineReset();
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    User.Inventory[i] = null;
                    break;
                }
            }
            ExNPCDialog.Hide();
        }
        
        private void OnRecvDepositTradeItemHandler(ServerPacket.DepositTradeItem p)
        {
            MirItemCell fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];

            MirItemCell toCell = TradeDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;
            TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
       
        private void OnRecvRetrieveTradeItemHandler(ServerPacket.RetrieveTradeItem p)
        {
            MirItemCell fromCell = TradeDialog.Grid[p.From];
            MirItemCell toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;
            TradeDialog.ChangeLockState(false);

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
       
        private void OnRecvSplitItemHandler(ServerPacket.SplitItem p)
        {
            Bind(p.Item);

            UserItem[] array;
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    array = MapObject.User.Inventory;
                    break;
                case MirGridType.Storage:
                    array = Storage;
                    break;
                default:
                    return;
            }

            if (p.Grid == MirGridType.Inventory && (p.Item.Info.Type == ItemType.Potion || p.Item.Info.Type == ItemType.Scroll || p.Item.Info.Type == ItemType.Amulet || (p.Item.Info.Type == ItemType.Script && p.Item.Info.Effect == 1)))
            {
                if (p.Item.Info.Type == ItemType.Potion || p.Item.Info.Type == ItemType.Scroll || (p.Item.Info.Type == ItemType.Script && p.Item.Info.Effect == 1))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (array[i] != null) continue;
                        array[i] = p.Item;
                        User.RefreshStats();
                        return;
                    }
                }
                else if (p.Item.Info.Type == ItemType.Amulet)
                {
                    for (int i = 4; i < ExineMainScene.User.BeltIdx; i++)
                    {
                        if (array[i] != null) continue;
                        array[i] = p.Item;
                        User.RefreshStats();
                        return;
                    }
                }
            }

            for (int i = ExineMainScene.User.BeltIdx; i < array.Length; i++)
            {
                if (array[i] != null) continue;
                array[i] = p.Item;
                User.RefreshStats();
                return;
            }

            for (int i = 0; i < ExineMainScene.User.BeltIdx; i++)
            {
                if (array[i] != null) continue;
                array[i] = p.Item;
                User.RefreshStats();
                return;
            }
        }

        private void OnRecvSplitItem1Handler(ServerPacket.SplitItem1 p)
        {
            MirItemCell cell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    cell = StorageDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            cell.Item.Count -= p.Count;
            User.RefreshStats();
        }
        
        private void OnRecvUseItemHandler(ServerPacket.UseItem p)
        {
            MirItemCell cell = null;
            bool hero = false;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
            }

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            if (cell.Item.Count > 1) cell.Item.Count--;
            else cell.Item = null;

            User.RefreshStats();
        }
       
        private void OnRecvDropItemHandler(ServerPacket.DropItem p)
        {
            MirItemCell cell;
            cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);


            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;
            
            User.RefreshStats();

        }
 
        private void OnRecvTransformUpdateHandler(ServerPacket.TransformUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].ObjectID != p.ObjectID) continue;

                if (MapControl.Objects[i] is PlayerObject player)
                {
                    player.TransformType = p.TransformType;
                }
                break;
            }
        }
 
        private void OnRecvCompleteQuestHandler(ServerPacket.CompleteQuest p)
        {
            User.CompletedQuests = p.CompletedQuests;
        }

        private void OnRecvShareQuestHandler(ServerPacket.ShareQuest p)
        {
            ClientQuestInfo quest = ExineMainScene.QuestInfoList.FirstOrDefault(e => e.Index == p.QuestIndex);

            if (quest == null) return;

            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}) 퀘스트를 공유하고 싶습니다. 수락하시겠습니까?", p.SharerName), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.AcceptQuest { NPCIndex = 0, QuestIndex = quest.Index });

            messageBox.Show();
        }

        private void OnRecvChangeQuestHandler(ServerPacket.ChangeQuest p)
        {
            switch (p.QuestState)
            {
                case QuestState.Add:
                    User.CurrentQuests.Add(p.Quest);

                    foreach (ClientQuestProgress quest in User.CurrentQuests)
                        BindQuest(quest);
                    if (Settings.TrackedQuests.Contains(p.Quest.Id))
                    {
                        ExineMainScene.Scene.QuestTrackingDialog.AddQuest(p.Quest, true);
                    }

                    if (p.TrackQuest)
                    {
                        ExineMainScene.Scene.QuestTrackingDialog.AddQuest(p.Quest);
                    }

                    break;
                case QuestState.Update:
                    for (int i = 0; i < User.CurrentQuests.Count; i++)
                    {
                        if (User.CurrentQuests[i].Id != p.Quest.Id) continue;

                        User.CurrentQuests[i] = p.Quest;
                    }

                    foreach (ClientQuestProgress quest in User.CurrentQuests)
                        BindQuest(quest);

                    break;
                case QuestState.Remove:

                    for (int i = User.CurrentQuests.Count - 1; i >= 0; i--)
                    {
                        if (User.CurrentQuests[i].Id != p.Quest.Id) continue;

                        User.CurrentQuests.RemoveAt(i);
                    }

                    ExineMainScene.Scene.QuestTrackingDialog.RemoveQuest(p.Quest);

                    break;
            }

            ExineMainScene.Scene.QuestTrackingDialog.DisplayQuests();

            if (Scene.QuestListDialog.Visible)
            {
                Scene.QuestListDialog.DisplayInfo();
            }

            if (Scene.QuestLogDialog.Visible)
            {
                Scene.QuestLogDialog.DisplayQuests();
            }
        }

        private void OnRecvPlayerUpdateHandler(ServerPacket.PlayerUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].ObjectID != p.ObjectID) continue;

                PlayerObject player = MapControl.Objects[i] as PlayerObject;
                if (player != null) player.Update(p);
                return;
            }
        }
        private void OnRecvPlayerInspectHandler(ServerPacket.PlayerInspect p)
        {
            InspectDialog.Items = p.Equipment;

            InspectDialog.Name = p.Name;
            InspectDialog.GuildName = p.GuildName;
            InspectDialog.GuildRank = p.GuildRank;
            InspectDialog.Class = p.Class;
            InspectDialog.Gender = p.Gender;
            InspectDialog.Hair = p.Hair;
            InspectDialog.Level = p.Level;
            InspectDialog.LoverName = p.LoverName;
            InspectDialog.AllowObserve = p.AllowObserve;

            InspectDialog.RefreshInferface(p.IsHero);
            InspectDialog.Show();
        }
        private void OnRecvLogOutSuccessHandler(ServerPacket.LogOutSuccess p)
        {
            for (int i = 0; i <= 3; i++)//Fix for orbs sound
                SoundManager.StopSound(20000 + 126 * 10 + 5 + i);

            User = null;
            if (Settings.Resolution != 1024)
            {
                CMain.SetResolution(1024, 768);
            }

            //ActiveScene = new LoginScene(p.Characters);
            ActiveScene = new ExineLoginScene();

            Dispose();
        }
        private void OnRecvLogOutFailedHandler(ServerPacket.LogOutFailed p)
        {
            Enabled = true;
        }

        private void OnRecvReturnToLoginHandler(ServerPacket.ReturnToLogin p)
        {
            User = null;
            if (Settings.Resolution != 1024)
                CMain.SetResolution(1024, 768);

            ActiveScene = new ExineLoginScene();
            Dispose();
            ExineMessageBox.Show("The person you was observing has logged off.");
        }

        private void OnRecvTimeOfDayHandler(ServerPacket.TimeOfDay p)
        {
            Lights = p.Lights;
            switch (Lights)
            {
                case LightSetting.Day:
                case LightSetting.Normal:
                    MiniMapDialog.LightSetting.Index = 2093;
                    break;
                case LightSetting.Dawn:
                    MiniMapDialog.LightSetting.Index = 2095;
                    break;
                case LightSetting.Evening:
                    MiniMapDialog.LightSetting.Index = 2094;
                    break;
                case LightSetting.Night:
                    MiniMapDialog.LightSetting.Index = 2092;
                    break;
            }
        }
        private void OnRecvChangeAModeHandler(ServerPacket.ChangeAMode p)
        {
            AMode = p.Mode;

            switch (p.Mode)
            {
                case AttackMode.Peace:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_Peace, ChatType.Hint);
                    break;
                case AttackMode.Group:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_Group, ChatType.Hint);
                    break;
                case AttackMode.Guild:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_Guild, ChatType.Hint);
                    break;
                case AttackMode.EnemyGuild:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_EnemyGuild, ChatType.Hint);
                    break;
                case AttackMode.RedBrown:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_RedBrown, ChatType.Hint);
                    break;
                case AttackMode.All:
                    ExChatDialog.ReceiveChat(GameLanguage.AttackMode_All, ChatType.Hint);
                    break;
            }
        }
        private void OnRecvChangePModeHandler(ServerPacket.ChangePMode p)
        {
            PMode = p.Mode;
            switch (p.Mode)
            {
                case PetMode.Both:
                    ExChatDialog.ReceiveChat(GameLanguage.PetMode_Both, ChatType.Hint);
                    break;
                case PetMode.MoveOnly:
                    ExChatDialog.ReceiveChat(GameLanguage.PetMode_MoveOnly, ChatType.Hint);
                    break;
                case PetMode.AttackOnly:
                    ExChatDialog.ReceiveChat(GameLanguage.PetMode_AttackOnly, ChatType.Hint);
                    break;
                case PetMode.None:
                    ExChatDialog.ReceiveChat(GameLanguage.PetMode_None, ChatType.Hint);
                    break;
                case PetMode.FocusMasterTarget:
                    ExChatDialog.ReceiveChat(GameLanguage.PetMode_FocusMasterTarget, ChatType.Hint);
                    break;
            }
        }

        private void OnRecvObjectItemHandler(ServerPacket.ObjectItem p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
            /*
            string[] Warnings = new string[] {"HeroNecklace","AdamantineNecklace","8TrigramWheel","HangMaWheel","BaekTaGlove","SpiritReformer","BokMaWheel","BoundlessRing","ThunderRing","TaeGukRing","OmaSpiritRing","NobleRing"};
            if (Warnings.Contains(p.Name))
            {
                ChatDialog.ReceiveChat(string.Format("{0} at {1}", p.Name, p.Location), ChatType.Hint);
            }
            */
        }
        private void OnRecvObjectGoldHandler(ServerPacket.ObjectGold p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
        }
        private void OnRecvGainedItemHandler(ServerPacket.GainedItem p)
        {
            Bind(p.Item);
            AddItem(p.Item);
            User.RefreshStats();

            OutputMessage(string.Format(GameLanguage.YouGained, p.Item.FriendlyName));
        }
        private void OnRecvGainedQuestItemHandler(ServerPacket.GainedQuestItem p)
        {
            Bind(p.Item);
            AddQuestItem(p.Item);
        }

        private void OnRecvGainedGoldHandler(ServerPacket.GainedGold p)
        {
            if (p.Gold == 0) return;

            Gold += p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
            OutputMessage(string.Format(GameLanguage.YouGained2, p.Gold, GameLanguage.Gold));
        }
        private void OnRecvLoseGoldHandler(ServerPacket.LoseGold p)
        {
            Gold -= p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
        }
        private void OnRecvGainedCreditHandler(ServerPacket.GainedCredit p)
        {
            if (p.Credit == 0) return;

            Credit += p.Credit;
            SoundManager.PlaySound(SoundList.Gold);
            OutputMessage(string.Format(GameLanguage.YouGained2, p.Credit, GameLanguage.Credit));
        }
        private void OnRecvLoseCreditHandler(ServerPacket.LoseCredit p)
        {
            Credit -= p.Credit;
            SoundManager.PlaySound(SoundList.Gold);
        }
        private void OnRecvObjectMonsterHandler(ServerPacket.ObjectMonster p)
        {
            var found = false;
            var mob = (MonsterObject)MapControl.Objects.Find(ob => ob.ObjectID == p.ObjectID);
            if (mob != null)
                found = true;
            if (!found)
                mob = new MonsterObject(p.ObjectID);
            mob.Load(p, found);
        }
        private void OnRecvObjectAttackHandler(ServerPacket.ObjectAttack p)
        {
            if (p.ObjectID == User.ObjectID && !Observing) return;

            QueuedAction action = null;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race == ObjectType.Player)
                {
                    //action = new QueuedAction { Action = ExAction.Attack1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                    action = new QueuedAction { Action = ExAction.ONEHAND_ATTACK1, Direction = p.Direction, Location = p.Location, Params = new List<object>() }; //mod 240926 k333123
                }
                
                else
                {
                    switch (p.Type)
                    {
                        default:
                            {
                                action = new QueuedAction { Action = ExAction.Attack1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 1:
                            {
                                action = new QueuedAction { Action = ExAction.Attack2, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 2:
                            {
                                action = new QueuedAction { Action = ExAction.Attack3, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 3:
                            {
                                action = new QueuedAction { Action = ExAction.Attack4, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 4:
                            {
                                action = new QueuedAction { Action = ExAction.Attack5, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                    }
                }
                action.Params.Add(p.Spell);
                action.Params.Add(p.Level);
                ob.ActionFeed.Add(action);
                return;
            }
        }
        private void OnRecvStruckHandler(ServerPacket.Struck p)
        {
            LogTime = CMain.Time + Globals.LogDelay;

            NextRunTime = CMain.Time + 2500;
            User.BlizzardStopTime = 0;
            User.ClearMagic();
            if (User.ReincarnationStopTime > CMain.Time)
                Network.SendPacketToServer(new ClientPacket.CancelReincarnation { });

            ExineDirection dir = User.Direction;
            Point location = User.CurrentLocation;

            for (int i = 0; i < User.ActionFeed.Count; i++)
                if (User.ActionFeed[i].Action == ExAction.Struck) return;


            if (User.ActionFeed.Count > 0)
            {
                dir = User.ActionFeed[User.ActionFeed.Count - 1].Direction;
                location = User.ActionFeed[User.ActionFeed.Count - 1].Location;
            }

            if (User.Buffs.Any(a => a == BuffType.EnergyShield))
            {
                for (int j = 0; j < User.Effects.Count; j++)
                {
                    BuffEffect effect = null;
                    effect = User.Effects[j] as BuffEffect;

                    if (effect != null && effect.BuffType == BuffType.EnergyShield)
                    {
                        effect.Clear();
                        effect.Remove();

                        User.Effects.Add(effect = new BuffEffect(Libraries.Magic2, 1890, 6, 600, User, true, BuffType.EnergyShield) { Repeat = false });
                        SoundManager.PlaySound(20000 + (ushort)Spell.EnergyShield * 10 + 1);

                        effect.Complete += (o, e) =>
                        {
                            User.Effects.Add(new BuffEffect(Libraries.Magic2, 1900, 2, 800, User, true, BuffType.EnergyShield) { Repeat = true });
                        };


                        break;
                    }
                }
            }

            QueuedAction action = new QueuedAction { Action = ExAction.Struck, Direction = dir, Location = location, Params = new List<object>() };
            action.Params.Add(p.AttackerID);
            User.ActionFeed.Add(action);

        }
        private void OnRecvObjectStruckHandler(ServerPacket.ObjectStruck p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.SkipFrames) return;
                if (ob.ActionFeed.Count > 0 && ob.ActionFeed[ob.ActionFeed.Count - 1].Action == ExAction.Struck) return;

                if (ob.Race == ObjectType.Player)
                    ((PlayerObject)ob).BlizzardStopTime = 0;

                QueuedAction action = new QueuedAction { Action = ExAction.Struck, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.AttackerID);
                ob.ActionFeed.Add(action);

                if (ob.Buffs.Any(a => a == BuffType.EnergyShield))
                {
                    for (int j = 0; j < ob.Effects.Count; j++)
                    {
                        BuffEffect effect = null;
                        effect = ob.Effects[j] as BuffEffect;

                        if (effect != null && effect.BuffType == BuffType.EnergyShield)
                        {
                            effect.Clear();
                            effect.Remove();

                            ob.Effects.Add(effect = new BuffEffect(Libraries.Magic2, 1890, 6, 600, ob, true, BuffType.EnergyShield) { Repeat = false });
                            SoundManager.PlaySound(20000 + (ushort)Spell.EnergyShield * 10 + 1);

                            effect.Complete += (o, e) =>
                            {
                                ob.Effects.Add(new BuffEffect(Libraries.Magic2, 1900, 2, 800, ob, true, BuffType.EnergyShield) { Repeat = true });
                            };

                            break;
                        }
                    }
                }

                return;
            }
        }

        private void OnRecvDamageIndicatorHandler(ServerPacket.DamageIndicator p)
        {
            if (Settings.DisplayDamage)
            {
                for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject obj = MapControl.Objects[i];
                    if (obj.ObjectID != p.ObjectID) continue;

                    if (obj.Damages.Count >= 10) return;

                    switch (p.Type)
                    {
                        case DamageType.Hit: //add damage level colours
                            obj.Damages.Add(new Damage(p.Damage.ToString("#,##0"), 1000, obj.Race == ObjectType.Player ? Color.Red : Color.White, 50));
                            break;
                        case DamageType.Miss:
                            obj.Damages.Add(new Damage("Miss", 1200, obj.Race == ObjectType.Player ? Color.LightCoral : Color.LightGray, 50));
                            break;
                        case DamageType.Critical:
                            obj.Damages.Add(new Damage("Crit", 1000, obj.Race == ObjectType.Player ? Color.DarkRed : Color.DarkRed, 50) { Offset = 15 });
                            break;
                    }
                }
            }
        }

        private void OnRecvDuraChangedHandler(ServerPacket.DuraChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }


            if (item == null)
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                    if (User.Equipment[i] != null && User.Equipment[i].Slots != null)
                    {
                        for (int j = 0; j < User.Equipment[i].Slots.Length; j++)
                        {
                            if (User.Equipment[i].Slots[j] != null && User.Equipment[i].Slots[j].UniqueID == p.UniqueID)
                            {
                                item = User.Equipment[i].Slots[j];
                                break;
                            }
                        }

                        if (item != null) break;
                    }
                }

            if (item == null) return;

            item.CurrentDura = p.CurrentDura;

            if (item.CurrentDura == 0)
            {
                User.RefreshStats();
                switch (item.Info.Type)
                {
                   
                    default:
                        ExChatDialog.ReceiveChat(string.Format("{0}의 내구도가 0으로 감소했습니다.", item.Info.FriendlyName), ChatType.System);
                        break;
                }

            }

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }

            CharacterDuraPanel.UpdateCharacterDura(item);
        }
        private void OnRecvHealthChangedHandler(ServerPacket.HealthChanged p)
        {
            User.HP = p.HP;
            User.MP = p.MP;

            User.PercentHealth = (byte)(User.HP / (float)User.Stats[Stat.HP] * 100);
        }

        private void OnRecvDeleteQuestItemHandler(ServerPacket.DeleteQuestItem p)
        {
            for (int i = 0; i < User.QuestInventory.Length; i++)
            {
                UserItem item = User.QuestInventory[i];

                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.QuestInventory[i] = null;
                else
                    item.Count -= p.Count;
                break;
            }
        }

        private void OnRecvDeleteItemHandler(ServerPacket.DeleteItem p)
        {
            UserObject actor = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (actor != null) break;
                UserItem item = User.Inventory[i];

                if (item != null && item.Slots.Length > 0)
                {
                    for (int j = 0; j < item.Slots.Length; j++)
                    {
                        UserItem slotItem = item.Slots[j];

                        if (slotItem == null || slotItem.UniqueID != p.UniqueID) continue;

                        if (slotItem.Count == p.Count)
                            item.Slots[j] = null;
                        else
                            slotItem.Count -= p.Count;
                        actor = User;
                        break;
                    }
                }

                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.Inventory[i] = null;
                else
                    item.Count -= p.Count;
                actor = User;
            }

            if (actor == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (actor != null) break;
                    UserItem item = User.Equipment[i];

                    if (item != null && item.Slots.Length > 0)
                    {
                        for (int j = 0; j < item.Slots.Length; j++)
                        {
                            UserItem slotItem = item.Slots[j];

                            if (slotItem == null || slotItem.UniqueID != p.UniqueID) continue;

                            if (slotItem.Count == p.Count)
                                item.Slots[j] = null;
                            else
                                slotItem.Count -= p.Count;
                            actor = User;
                            break;
                        }
                    }

                    if (item == null || item.UniqueID != p.UniqueID) continue;

                    if (item.Count == p.Count)
                        User.Equipment[i] = null;
                    else
                        item.Count -= p.Count;
                    actor = User;
                }
            }

            if (actor == null)
            {
                for (int i = 0; i < Storage.Length; i++)
                {
                    var item = Storage[i];
                    if (item == null || item.UniqueID != p.UniqueID) continue;

                    if (item.Count == p.Count)
                        Storage[i] = null;
                    else
                        item.Count -= p.Count;
                    break;
                }
            }
            actor?.RefreshStats();
        }
        private void OnRecvDeathHandler(ServerPacket.Death p)
        {
            User.Dead = true;

            User.ActionFeed.Add(new QueuedAction { Action = ExAction.Die, Direction = p.Direction, Location = p.Location });
            ShowReviveMessage = true;

            LogTime = 0;
        }
        private void OnRecvObjectDiedHandler(ServerPacket.ObjectDied p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                switch (p.Type)
                {
                    default:
                        if (ob is PlayerObject) //add k333123 240926
                        {
                            ob.ActionFeed.Add(new QueuedAction { Action = ExAction.DIE, Direction = p.Direction, Location = p.Location });
                        }
                        else
                        {
                            ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Die, Direction = p.Direction, Location = p.Location });
                        }
                        ob.Dead = true;
                        break;
                    case 1:
                        MapControl.Effects.Add(new Effect(Libraries.Magic2, 690, 10, 1000, ob.CurrentLocation));
                        ob.Remove();
                        break;
                    
                }
                return;
            }
        }
        private void OnRecvColourChangedHandler(ServerPacket.ColourChanged p)
        {
            User.NameColour = p.NameColour;
        }
        private void OnRecvObjectColourChangedHandler(ServerPacket.ObjectColourChanged p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.NameColour = p.NameColour;
                return;
            }
        }

        private void OnRecvObjectGuildNameChangedHandler(ServerPacket.ObjectGuildNameChanged p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                PlayerObject obPlayer = (PlayerObject)ob;
                obPlayer.GuildName = p.GuildName;
                return;
            }
        }
        private void OnRecvGainExperienceHandler(ServerPacket.GainExperience p)
        {
            OutputMessage(string.Format(GameLanguage.ExperienceGained, p.Amount));
            MapObject.User.Experience += p.Amount;
        }

        private void OnRecvLevelChangedHandler(ServerPacket.LevelChanged p)
        {
            User.Level = p.Level;
            User.Experience = p.Experience;
            User.MaxExperience = p.MaxExperience;
            User.RefreshStats();
            OutputMessage(GameLanguage.LevelUp);
            User.Effects.Add(new Effect(Libraries.Magic2, 1200, 20, 2000, User));
            SoundManager.PlaySound(SoundList.LevelUp);
            ExChatDialog.ReceiveChat(GameLanguage.LevelUp, ChatType.LevelUp);
        }
        
        private void OnRecvObjectLeveledHandler(ServerPacket.ObjectLeveled p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Effects.Add(new Effect(Libraries.Magic2, 1180, 16, 2500, ob));
                SoundManager.PlaySound(SoundList.LevelUp);
                return;
            }
        }
        private void OnRecvObjectHarvestHandler(ServerPacket.ObjectHarvest p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Harvest, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void OnRecvObjectHarvestedHandler(ServerPacket.ObjectHarvested p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Skeleton, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void OnRecvObjectNPCHandler(ServerPacket.ObjectNPC p)
        {
            NPCObject ob = new NPCObject(p.ObjectID);
            ob.Load(p); 
        }
        private void OnRecvNPCResponseHandler(ServerPacket.NPCResponse p)
        {
            NPCTime = 0;
            ExNPCDialog.BigButtons.Clear();
            ExNPCDialog.BigButtonDialog.Hide();
            ExNPCDialog.NewText(p.Page);
            

            if (p.Page.Count > 0 || ExNPCDialog.BigButtons.Count > 0)
                ExNPCDialog.Show();
            else
                ExNPCDialog.Hide();

            ExNPCGoodsDialog.Hide();
            NPCSubGoodsDialog.Hide();
            NPCCraftGoodsDialog.Hide();
            NPCDropDialog.Hide();
            StorageDialog.Hide(); ;
            RefineDialog.Hide();
            StorageDialog.Hide();
             
            QuestListDialog.Hide();
        }

        private void OnRecvNPCUpdateHandler(ServerPacket.NPCUpdate p)
        {
            ExineMainScene.NPCID = p.NPCID; //Updates the client with the correct NPC ID if it's manually called from the client
        }

        private void OnRecvNPCImageUpdateHandler(ServerPacket.NPCImageUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID || ob.Race != ObjectType.Merchant) continue;

                NPCObject npc = (NPCObject)ob;
                npc.Image = p.Image;
                npc.Colour = p.Colour;

                npc.LoadLibrary();
                return;
            }
        }
        private void OnRecvDefaultNPCHandler(ServerPacket.DefaultNPC p)
        {
            ExineMainScene.DefaultNPCID = p.ObjectID; //Updates the client with the correct Default NPC ID
        }


        private void OnRecvObjectHideHandler(ServerPacket.ObjectHide p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Hide, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void OnRecvObjectShowHandler(ServerPacket.ObjectShow p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Show, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void OnRecvPoisonedHandler(ServerPacket.Poisoned p)
        {
            var previousPoisons = User.Poison;

            User.Poison = p.Poison;
            if (p.Poison.HasFlag(PoisonType.Stun) || p.Poison.HasFlag(PoisonType.Dazed) || p.Poison.HasFlag(PoisonType.Frozen) || p.Poison.HasFlag(PoisonType.Paralysis) || p.Poison.HasFlag(PoisonType.LRParalysis))
            {
                User.ClearMagic();
            }

            if (previousPoisons.HasFlag(PoisonType.Blindness) && !User.Poison.HasFlag(PoisonType.Blindness))
            {
                User.BlindCount = 0;
            }
        }

        private void OnRecvObjectPoisonedHandler(ServerPacket.ObjectPoisoned p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Poison = p.Poison;
                return;
            }
        }
        private void OnRecvMapChangedHandler(ServerPacket.MapChanged p)
        {
            var isCurrentMap = (MapControl.Index == p.MapIndex);

            if (isCurrentMap)
                MapControl.ResetMap();
            else
            {
                MapControl.Index = p.MapIndex;
                MapControl.FileName = Path.Combine(Settings.MapPath, p.FileName + ".map");
                MapControl.Title = p.Title;
                MapControl.MiniMap = p.MiniMap;
                MapControl.BigMap = p.BigMap;
                MapControl.Lights = p.Lights;
                MapControl.MapDarkLight = p.MapDarkLight;
                MapControl.Music = p.Music;
                MapControl.LoadMap();
            }

            MapControl.NextAction = 0;
            Scene.MapControl.AutoPath = false;
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            User.Direction = p.Direction;

            User.QueuedAction = null;
            User.ActionFeed.Clear();
            User.ClearMagic();
            User.SetAction();

            ExineMainScene.CanRun = false;

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;
        }
        private void OnRecvObjectTeleportOutHandler(ServerPacket.ObjectTeleportOut p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                Effect effect = null;

                bool playDefaultSound = true;

                switch (p.Type)
                {
                    case 1: //Yimoogi
                        {
                            effect = new Effect(Libraries.Magic2, 1300, 10, 500, ob.CurrentLocation);
                            break;
                        }
                    case 2: //RedFoxman
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.RedFoxman], 243, 10, 500, ob.CurrentLocation);
                            break;
                        }
                    case 4: //MutatedManWorm
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.MutatedManworm], 272, 6, 500, ob.CurrentLocation);

                            SoundManager.PlaySound(((ushort)Monster.MutatedManworm) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 5: //WitchDoctor
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.WitchDoctor], 328, 20, 1000, ob.CurrentLocation);
                            SoundManager.PlaySound(((ushort)Monster.WitchDoctor) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 6: //TurtleKing
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.TurtleKing], 946, 10, 500, ob.CurrentLocation);
                            break;
                        }
                    case 7: //Mandrill
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.Mandrill], 280, 10, 1000, ob.CurrentLocation);
                            SoundManager.PlaySound(((ushort)Monster.Mandrill) * 10 + 6);
                            playDefaultSound = false;
                            break;
                        }
                    case 8: //DarkCaptain
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.DarkCaptain], 1224, 10, 1000, ob.CurrentLocation));
                            SoundManager.PlaySound(((ushort)Monster.DarkCaptain) * 10 + 8);
                            playDefaultSound = false;
                            break;
                        }
                    case 9: //Doe
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.Doe], 208, 10, 1000, ob.CurrentLocation);
                            SoundManager.PlaySound(((ushort)Monster.Doe) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 10: //HornedCommander
                        {
                            MapControl.Effects.Add(effect = new Effect(Libraries.Monsters[(ushort)Monster.HornedCommander], 928, 10, 1000, ob.CurrentLocation));
                            SoundManager.PlaySound(8455);
                            playDefaultSound = false;
                            break;
                        }
                    case 11: //SnowWolfKing
                        {
                            MapControl.Effects.Add(effect = new Effect(Libraries.Monsters[(ushort)Monster.SnowWolfKing], 561, 10, 1000, ob.CurrentLocation));
                            SoundManager.PlaySound(8455);
                            playDefaultSound = false;
                            break;
                        }
                    default:
                        {
                            effect = new Effect(Libraries.Magic, 250, 10, 500, ob.CurrentLocation);
                            break;
                        }
                }

                //Doesn't seem to have ever worked properly - Meant to remove object after animation complete, however due to server mechanics will always
                //instantly remove object and never play TeleportOut animation. Changing to a MapEffect - not ideal as theres no delay.

                MapControl.Effects.Add(effect);

                //if (effect != null)
                //{
                //    effect.Complete += (o, e) => ob.Remove();
                //    ob.Effects.Add(effect);
                //}

                if (playDefaultSound)
                {
                    SoundManager.PlaySound(SoundList.Teleport);
                }

                return;
            }
        }
        private void OnRecvObjectTeleportInHandler(ServerPacket.ObjectTeleportIn p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                bool playDefaultSound = true;

                switch (p.Type)
                {
                    case 1: //Yimoogi
                        {
                            ob.Effects.Add(new Effect(Libraries.Magic2, 1310, 10, 500, ob));
                            break;
                        }
                    case 2: //RedFoxman
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.RedFoxman], 253, 10, 500, ob));
                            break;
                        }
                    case 4: //MutatedManWorm
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.MutatedManworm], 278, 7, 500, ob));
                            SoundManager.PlaySound(((ushort)Monster.MutatedManworm) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 5: //WitchDoctor
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.WitchDoctor], 348, 20, 1000, ob));
                            SoundManager.PlaySound(((ushort)Monster.WitchDoctor) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 6: //TurtleKing
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.TurtleKing], 956, 10, 500, ob));
                            break;
                        }
                    case 7: //Mandrill
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Mandrill], 290, 10, 1000, ob));
                            SoundManager.PlaySound(((ushort)Monster.Mandrill) * 10 + 6);
                            playDefaultSound = false;
                            break;
                        }
                    case 8: //DarkCaptain
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.DarkCaptain], 1224, 10, 1000, ob));
                            SoundManager.PlaySound(((ushort)Monster.DarkCaptain) * 10 + 9);
                            playDefaultSound = false;
                            break;
                        }
                    case 9: //Doe
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Doe], 208, 10, 1000, ob));
                            SoundManager.PlaySound(((ushort)Monster.Doe) * 10 + 7);
                            playDefaultSound = false;
                            break;
                        }
                    case 10: //HornedCommander
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.HornedCommander], 928, 10, 1000, ob));
                            SoundManager.PlaySound(8455);
                            playDefaultSound = false;
                            break;
                        }
                    case 11: //SnowWolfKing
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.SnowWolfKing], 571, 10, 1000, ob));
                            SoundManager.PlaySound(8455);
                            playDefaultSound = false;
                            break;
                        }
                    default:
                        {
                            ob.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, ob));
                            break;
                        }
                }

                if (p.ObjectID == User.ObjectID)
                {
                    User.TargetID = User.LastTargetObjectId;
                }

                if (playDefaultSound)
                {
                    SoundManager.PlaySound(SoundList.Teleport);
                }

                return;
            }
        }

        private void OnRecvTeleportInHandler()
        {
            User.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, User));
            SoundManager.PlaySound(SoundList.Teleport);
        }
        private void OnRecvNPCGoodsHandler(ServerPacket.NPCGoods p)
        {
            for (int i = 0; i < p.List.Count; i++)
            {
                p.List[i].Info = GetInfo(p.List[i].ItemIndex);
            }

            NPCRate = p.Rate;
            HideAddedStoreStats = p.HideAddedStats;

            if (!ExNPCDialog.Visible) return;

            switch (p.Type)
            {
                case PanelType.Buy:
                    ExNPCGoodsDialog.UsePearls = false;
                    ExNPCGoodsDialog.NewGoods(p.List);
                    ExNPCGoodsDialog.Show();
                    break;
                case PanelType.BuySub:
                    NPCSubGoodsDialog.UsePearls = false;
                    NPCSubGoodsDialog.NewGoods(p.List);
                    NPCSubGoodsDialog.Show();
                    break;
                case PanelType.Craft:
                    NPCCraftGoodsDialog.UsePearls = false;
                    NPCCraftGoodsDialog.NewGoods(p.List);
                    NPCCraftGoodsDialog.Show();
                    CraftDialog.Show();
                    break;
            }
        }
        private void OnRecvNPCPearlGoodsHandler(ServerPacket.NPCPearlGoods p)
        {
            for (int i = 0; i < p.List.Count; i++)
            {
                p.List[i].Info = GetInfo(p.List[i].ItemIndex);
            }

            NPCRate = p.Rate;

            if (!ExNPCDialog.Visible) return;

            ExNPCGoodsDialog.UsePearls = true;
            ExNPCGoodsDialog.NewGoods(p.List);
            ExNPCGoodsDialog.Show();
        }

        private void OnRecvNPCSellHandler()
        {
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Sell;
            NPCDropDialog.Show();
        }
        private void OnRecvNPCRepairHandler(ServerPacket.NPCRepair p)
        {
            NPCRate = p.Rate;
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Repair;
            NPCDropDialog.Show();
        }
        private void OnRecvNPCStorageHandler()
        {
            if (ExNPCDialog.Visible)
                StorageDialog.Show();
        }
        private void OnRecvNPCRequestInputHandler(ServerPacket.NPCRequestInput p)
        {
            ExineInputBox inputBox = new ExineInputBox("요청된 정보를 입력해주세요.");

            inputBox.OKButton.Click += (o1, e1) =>
            {
                Network.SendPacketToServer(new ClientPacket.NPCConfirmInput { Value = inputBox.InputTextBox.Text, NPCID = p.NPCID, PageName = p.PageName });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void OnRecvNPCSRepairHandler(ServerPacket.NPCSRepair p)
        {
            NPCRate = p.Rate;
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.SpecialRepair;
            NPCDropDialog.Show();
        }

        private void OnRecvNPCRefineHandler(ServerPacket.NPCRefine p)
        {
            NPCRate = p.Rate;
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Refine;
            if (p.Refining)
            {
                NPCDropDialog.Hide();
                ExNPCDialog.Hide();
            }
            else
                NPCDropDialog.Show();
        }

        private void OnRecvNPCCheckRefineHandler(ServerPacket.NPCCheckRefine p)
        {
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.CheckRefine;
            NPCDropDialog.Show();
        }

        private void OnRecvNPCCollectRefineHandler(ServerPacket.NPCCollectRefine p)
        {
            if (!ExNPCDialog.Visible) return;
            ExNPCDialog.Hide();
        }

        private void OnRecvNPCReplaceWedRingHandler(ServerPacket.NPCReplaceWedRing p)
        {
            if (!ExNPCDialog.Visible) return;
            NPCRate = p.Rate;
            NPCDropDialog.PType = PanelType.ReplaceWedRing;
            NPCDropDialog.Show();
        }


        private void OnRecvSellItemHandler(ServerPacket.SellItem p)
        {
            MirItemCell cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;

            User.RefreshStats();
        }
        private void OnRecvRepairItemHandler(ServerPacket.RepairItem p)
        {
            MirItemCell cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;
        }
        private void OnRecvCraftItemHandler(ServerPacket.CraftItem p)
        {
            if (!p.Success) return;

            CraftDialog.UpdateCraftCells();
            User.RefreshStats();
        }
        private void OnRecvItemRepairedHandler(ServerPacket.ItemRepaired p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }
            }

            if (item == null) return;

            item.MaxDura = p.MaxDura;
            item.CurrentDura = p.CurrentDura;

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void OnRecvItemSlotSizeChangedHandler(ServerPacket.ItemSlotSizeChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }
            }

            
            if (item == null) return;

            item.SetSlotSize(p.SlotSize);
        }

        private void OnRecvItemSealChangedHandler(ServerPacket.ItemSealChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }

            if (item == null)
            {
                for (int i = 0; i < User.Equipment.Length; i++)
                {
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }
                }
            }

            
            if (item == null) return;

            item.SealedInfo = new SealedInfo { ExpiryDate = p.ExpiryDate };

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void OnRecvItemUpgradedHandler(ServerPacket.ItemUpgraded p)
        {
            UserItem item = null;
            MirGridType grid = MirGridType.Inventory;
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.Item.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }
            }


            if (item == null) return;

            item.AddedStats.Clear();
            item.AddedStats.Add(p.Item.AddedStats);

            item.MaxDura = p.Item.MaxDura;
            item.RefineAdded = p.Item.RefineAdded;

            switch (grid)
            {
                case MirGridType.Inventory:
                    ExInventoryDialog.DisplayItemGridEffect(item.UniqueID, 0);
                    break;
                
            }


            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }

        private void OnRecvNewMagicHandler(ServerPacket.NewMagic p)
        {
            ClientMagic magic = p.Magic;

            UserObject actor = User;
            
            actor.Magics.Add(magic);
            actor.RefreshStats();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
            {
                Bar.Update();
            }
        }

        private void OnRecvRemoveMagicHandler(ServerPacket.RemoveMagic p)
        {
            User.Magics.RemoveAt(p.PlaceId);
            User.RefreshStats();
            foreach (SkillBarDialog Bar in SkillBarDialogs)
            {
                Bar.Update();
            }
        }

        private void OnRecvMagicLeveledHandler(ServerPacket.MagicLeveled p)
        {
            UserObject actor = User;

            for (int i = 0; i < actor.Magics.Count; i++)
            {
                ClientMagic magic = actor.Magics[i];
                if (magic.Spell != p.Spell) continue;

                if (magic.Level != p.Level)
                {
                    magic.Level = p.Level;
                    actor.RefreshStats();
                }

                magic.Experience = p.Experience;
                break;
            }
        }
        private void OnRecvMagicHandler(ServerPacket.Magic p)
        {
            User.Spell = p.Spell;
            User.Cast = p.Cast;
            User.TargetID = p.TargetID;
            User.TargetPoint = p.Target;
            User.SpellLevel = p.Level;
            User.SecondaryTargetIDs = p.SecondaryTargetIDs;

            if (!p.Cast) return;

            ClientMagic magic = User.GetMagic(p.Spell);
            magic.CastTime = CMain.Time;
        }

        private void OnRecvMagicDelayHandler(ServerPacket.MagicDelay p)
        {
            ClientMagic magic;
           
            magic = User.GetMagic(p.Spell);
            magic.Delay = p.Delay;
        }

        private void OnRecvMagicCastHandler(ServerPacket.MagicCast p)
        {
            ClientMagic magic = User.GetMagic(p.Spell);
            magic.CastTime = CMain.Time;
        }

        private void OnRecvObjectMagicHandler(ServerPacket.ObjectMagic p)
        {
            if (p.SelfBroadcast == false && p.ObjectID == User.ObjectID && !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                //QueuedAction action = new QueuedAction { Action = ExAction.MAGIC_CAST, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                QueuedAction action = new QueuedAction { Action = ExAction.Spell, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.Spell);
                action.Params.Add(p.TargetID);
                action.Params.Add(p.Target);
                action.Params.Add(p.Cast);
                action.Params.Add(p.Level);
                action.Params.Add(p.SecondaryTargetIDs);

                ob.ActionFeed.Add(action);
                return;
            }
        }

        private void OnRecvObjectProjectileHandler(ServerPacket.ObjectProjectile p)
        {
            MapObject source = MapControl.GetObject(p.Source);

            if (source == null) return;

            switch (p.Spell)
            {
                case Spell.FireBounce:
                    {
                        SoundManager.PlaySound(20000 + (ushort)Spell.GreatFireBall * 10 + 1);

                        Missile missile = source.CreateProjectile(410, Libraries.Magic, true, 6, 30, 4, targetID: p.Destination);

                        if (missile.Target != null)
                        {
                            missile.Complete += (o, e) =>
                            {
                                var sender = (Missile)o;

                                if (sender.Target.CurrentAction == ExAction.Dead) return;
                                sender.Target.Effects.Add(new Effect(Libraries.Magic, 570, 10, 600, sender.Target));
                                SoundManager.PlaySound(20000 + (ushort)Spell.GreatFireBall * 10 + 2);
                            };
                        }
                    }
                    break;
            }
        }

        private void OnRecvObjectEffectHandler(ServerPacket.ObjectEffect p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                PlayerObject player;

                switch (p.Effect)
                {
                    // Sanjian
                    case SpellEffect.FurbolgWarriorCritical:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.FurbolgWarrior], 400, 6, 600, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.FatalSword * 10);
                        break;

                    case SpellEffect.FatalSword:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1940, 4, 400, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.FatalSword * 10);
                        break;
                    case SpellEffect.StormEscape:
                        ob.Effects.Add(new Effect(Libraries.Magic3, 610, 10, 600, ob));
                        SoundManager.PlaySound(SoundList.Teleport);
                        break;
                    case SpellEffect.Teleport:
                        ob.Effects.Add(new Effect(Libraries.Magic, 1600, 10, 600, ob));
                        SoundManager.PlaySound(SoundList.Teleport);
                        break;
                    case SpellEffect.Healing:
                        SoundManager.PlaySound(20000 + (ushort)Spell.Healing * 10 + 1);
                        ob.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, ob));
                        break;
                    case SpellEffect.RedMoonEvil:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.RedMoonEvil], 32, 6, 400, ob) { Blend = false });
                        break;
                    case SpellEffect.TwinDrakeBlade:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 380, 6, 800, ob));
                        break;
                    case SpellEffect.MPEater:
                        for (int j = MapControl.Objects.Count - 1; j >= 0; j--)
                        {
                            MapObject ob2 = MapControl.Objects[j];
                            if (ob2.ObjectID == p.EffectType)
                            {
                                ob2.Effects.Add(new Effect(Libraries.Magic2, 2411, 19, 1900, ob2));
                                break;
                            }
                        }
                        ob.Effects.Add(new Effect(Libraries.Magic2, 2400, 9, 900, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.FatalSword * 10);
                        break;
                    case SpellEffect.Bleeding:
                        ob.Effects.Add(new Effect(Libraries.Magic3, 60, 3, 400, ob));
                        break;
                    case SpellEffect.Hemorrhage:
                        SoundManager.PlaySound(20000 + (ushort)Spell.Hemorrhage * 10);
                        ob.Effects.Add(new Effect(Libraries.Magic3, 0, 4, 400, ob));
                        ob.Effects.Add(new Effect(Libraries.Magic3, 28, 6, 600, ob));
                        ob.Effects.Add(new Effect(Libraries.Magic3, 46, 8, 800, ob));
                        break;
                    case SpellEffect.MagicShieldUp:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }
                        player.MagicShield = true;
                        player.Effects.Add(player.ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, ob) { Repeat = true });
                        break;
                    case SpellEffect.MagicShieldDown:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }
                        player.ShieldEffect = null;
                        player.MagicShield = false;
                        break;
                    case SpellEffect.GreatFoxSpirit:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.GreatFoxSpirit], 375 + (CMain.Random.Next(3) * 20), 20, 1400, ob));
                        SoundManager.PlaySound(((ushort)Monster.GreatFoxSpirit * 10) + 5);
                        break;
                    case SpellEffect.Entrapment:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1010, 10, 1500, ob));
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1020, 8, 1200, ob));
                        break;
                    case SpellEffect.Critical:
                        //ob.Effects.Add(new Effect(Libraries.CustomEffects, 0, 12, 60, ob));
                        break;
                    case SpellEffect.Reflect:
                        ob.Effects.Add(new Effect(Libraries.Effect, 580, 10, 70, ob));
                        break;
                    case SpellEffect.ElementalBarrierUp:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ElementalBarrierEffect != null)
                        {
                            player.ElementalBarrierEffect.Clear();
                            player.ElementalBarrierEffect.Remove();
                        }

                        player.ElementalBarrier = true;
                        player.Effects.Add(player.ElementalBarrierEffect = new Effect(Libraries.Magic3, 1890, 10, 2000, ob) { Repeat = true });
                        break;
                    case SpellEffect.ElementalBarrierDown:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject)ob;
                        if (player.ElementalBarrierEffect != null)
                        {
                            player.ElementalBarrierEffect.Clear();
                            player.ElementalBarrierEffect.Remove();
                        }
                        player.ElementalBarrierEffect = null;
                        player.ElementalBarrier = false;
                        player.Effects.Add(player.ElementalBarrierEffect = new Effect(Libraries.Magic3, 1910, 7, 1400, ob));
                        SoundManager.PlaySound(20000 + 131 * 10 + 5);
                        break;
                    case SpellEffect.DelayedExplosion:
                        int effectid = DelayedExplosionEffect.GetOwnerEffectID(ob.ObjectID);
                        if (effectid < 0)
                        {
                            ob.Effects.Add(new DelayedExplosionEffect(Libraries.Magic3, 1590, 8, 1200, ob, true, 0, 0));
                        }
                        else if (effectid >= 0)
                        {
                            if (DelayedExplosionEffect.effectlist[effectid].stage < p.EffectType)
                            {
                                DelayedExplosionEffect.effectlist[effectid].Remove();
                                ob.Effects.Add(new DelayedExplosionEffect(Libraries.Magic3, 1590 + ((int)p.EffectType * 10), 8, 1200, ob, true, (int)p.EffectType, 0));
                            }
                        }
                        break;
                    
                    case SpellEffect.TurtleKing:
                        {
                            Effect ef = new Effect(Libraries.Monsters[(ushort)Monster.TurtleKing], CMain.Random.Next(2) == 0 ? 922 : 934, 12, 1200, ob);
                            ef.Played += (o, e) => SoundManager.PlaySound(20000 + (ushort)Spell.HellFire * 10 + 1);
                            ob.Effects.Add(ef);
                        }
                        break;
                    case SpellEffect.Behemoth:
                        {
                            MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Behemoth], 788, 10, 1500, ob.CurrentLocation));
                            MapControl.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.Behemoth], 778, 10, 1500, ob.CurrentLocation, 0, true) { Blend = false });
                        }
                        break;
                    case SpellEffect.Stunned:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.StoningStatue], 632, 10, 1000, ob)
                        {
                            Repeat = p.Time > 0,
                            RepeatUntil = p.Time > 0 ? CMain.Time + p.Time : 0
                        });
                        break;
                    case SpellEffect.IcePillar:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.IcePillar], 18, 8, 800, ob));
                        break;
                    case SpellEffect.KingGuard:
                        if (p.EffectType == 0)
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.KingGuard], 753, 10, 1000, ob) { Blend = false });
                        }
                        else
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.KingGuard], 763, 10, 1000, ob) { Blend = false });
                        }
                        break;
                    case SpellEffect.FlamingMutantWeb:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.FlamingMutant], 330, 10, 1000, ob)
                        {
                            Repeat = p.Time > 0,
                            RepeatUntil = p.Time > 0 ? CMain.Time + p.Time : 0
                        });
                        break;
                    case SpellEffect.DeathCrawlerBreath:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.DeathCrawler], 272 + ((int)ob.Direction * 4), 4, 400, ob) { Blend = true });
                        break;
                    case SpellEffect.MoonMist:
                        ob.Effects.Add(new Effect(Libraries.Magic3, 705, 10, 800, ob));
                        break;
                }

                return;
            }
        }

        private void OnRecvRangeAttackHandler(ServerPacket.RangeAttack p)
        {
            User.TargetID = p.TargetID;
            User.TargetPoint = p.Target;
            User.Spell = p.Spell;
        }

        private void OnRecvPushedHandler(ServerPacket.Pushed p)
        {
            User.ActionFeed.Add(new QueuedAction { Action = ExAction.Pushed, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvObjectPushedHandler(ServerPacket.ObjectPushed p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Pushed, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void OnRecvObjectNameHandler(ServerPacket.ObjectName p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Name = p.Name;
                return;
            }
        }
        private void OnRecvUserStorageHandler(ServerPacket.UserStorage p)
        {
            if (Storage.Length != p.Storage.Length)
            {
                Array.Resize(ref Storage, p.Storage.Length);
            }

            Storage = p.Storage;

            for (int i = 0; i < Storage.Length; i++)
            {
                if (Storage[i] == null) continue;
                Bind(Storage[i]);
            }
        }
        private void OnRecvSwitchGroupHandler(ServerPacket.SwitchGroup p)
        {
            GroupDialog.AllowGroup = p.AllowGroup;

            if (!p.AllowGroup && GroupDialog.GroupList.Count > 0)
                OnRecvDeleteGroupHandler();
        }

        private void OnRecvDeleteGroupHandler()
        {
            GroupDialog.GroupList.Clear();
            GroupDialog.GroupMembersMap.Clear();
            BigMapViewPort.PlayerLocations.Clear();
            ExChatDialog.ReceiveChat("그룹을 탈퇴했습니다.", ChatType.Group);
        }

        private void OnRecvDeleteMemberHandler(ServerPacket.DeleteMember p)
        {
            GroupDialog.GroupList.Remove(p.Name);
            GroupDialog.GroupMembersMap.Remove(p.Name);
            BigMapViewPort.PlayerLocations.Remove(p.Name);
            ExChatDialog.ReceiveChat(string.Format("-{0}이 그룹을 떠났습니다.", p.Name), ChatType.Group);
        }

        private void OnRecvGroupInviteHandler(ServerPacket.GroupInvite p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}과(와) 그룹을 하시겠습니까?", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.GroupInvite { AcceptInvite = true });
                GroupDialog.Show();
            };
            messageBox.NoButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.GroupInvite { AcceptInvite = false });
            messageBox.Show();
        }
        private void OnRecvAddMemberHandler(ServerPacket.AddMember p)
        {
            GroupDialog.GroupList.Add(p.Name);
            ExChatDialog.ReceiveChat(string.Format("-{0}이(가) 그룹에 가입했습니다.", p.Name), ChatType.Group);
        }
        private void OnRecvGroupMembersMapHandler(ServerPacket.GroupMembersMap p)
        {
            if (!GroupDialog.GroupMembersMap.ContainsKey(p.PlayerName))
                GroupDialog.GroupMembersMap.Add(p.PlayerName, p.PlayerMap);
            else
            {
                GroupDialog.GroupMembersMap.Remove(p.PlayerName);
                GroupDialog.GroupMembersMap.Add(p.PlayerName, p.PlayerMap);
            }
        }
        private void OnRecvSendMemberLocationHandler(ServerPacket.SendMemberLocation p)
        {
            if (!BigMapViewPort.PlayerLocations.ContainsKey(p.MemberName))
                BigMapViewPort.PlayerLocations.Add(p.MemberName, p.MemberLocation);
            else
            {
                BigMapViewPort.PlayerLocations.Remove(p.MemberName);
                BigMapViewPort.PlayerLocations.Add(p.MemberName, p.MemberLocation);
            }
        }
        private void OnRecvRevivedHandler()
        {
            User.SetAction();
            User.Dead = false;
            User.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, User));
            SoundManager.PlaySound(SoundList.Revive);
        }
        private void OnRecvObjectRevivedHandler(ServerPacket.ObjectRevived p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (p.Effect)
                {
                    ob.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, ob));
                    SoundManager.PlaySound(SoundList.Revive);
                }
                ob.Dead = false;
                ob.ActionFeed.Clear();
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Revive, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void OnRecvSpellToggleHandler(ServerPacket.SpellToggle p)
        {
            UserObject actor = User;
            string prefix = string.Empty; 

            switch (p.Spell)
            {
                //Warrior
                case Spell.Slaying:
                    actor.Slaying = p.CanUse;
                    break;
                case Spell.Thrusting:
                    actor.Thrusting = p.CanUse;
                    ExChatDialog.ReceiveChat(prefix + (actor.Thrusting ? "Use Thrusting." : "Do not use Thrusting."), ChatType.Hint);
                    break;
                case Spell.HalfMoon:
                    actor.HalfMoon = p.CanUse;
                    ExChatDialog.ReceiveChat(prefix + (actor.HalfMoon ? "Use HalfMoon." : "Do not use HalfMoon."), ChatType.Hint);
                    break;
                case Spell.CrossHalfMoon:
                    actor.CrossHalfMoon = p.CanUse;
                    ExChatDialog.ReceiveChat(prefix + (actor.CrossHalfMoon ? "Use CrossHalfMoon." : "Do not use CrossHalfMoon."), ChatType.Hint);
                    break;
                case Spell.DoubleSlash:
                    actor.DoubleSlash = p.CanUse;
                    ExChatDialog.ReceiveChat(prefix + (actor.DoubleSlash ? "Use DoubleSlash." : "Do not use DoubleSlash."), ChatType.Hint);
                    break;
                case Spell.FlamingSword:
                    actor.FlamingSword = p.CanUse;
                    if (actor.FlamingSword)
                        ExChatDialog.ReceiveChat(prefix + GameLanguage.WeaponSpiritFire, ChatType.Hint);
                    else
                        ExChatDialog.ReceiveChat(prefix + GameLanguage.SpiritsFireDisappeared, ChatType.System);
                    break;
            }
        }

        private void OnRecvObjectHealthHandler(ServerPacket.ObjectHealth p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.PercentHealth = p.Percent;
                ob.HealthTime = CMain.Time + p.Expire * 1000;
                return;
            }
        }

        private void OnRecvObjectManaHandler(ServerPacket.ObjectMana p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.PercentMana = p.Percent;
                return;
            }
        }

        private void OnRecvMapEffectHandler(ServerPacket.MapEffect p)
        {
            switch (p.Effect)
            {
                case SpellEffect.Mine:
                    SoundManager.PlaySound(10091);
                    Effect HitWall = new Effect(Libraries.Effect, 8 * p.Value, 3, 240, p.Location) { Light = 0 };
                    MapControl.Effects.Add(HitWall);
                    break;
                case SpellEffect.Tester:
                    Effect eff = new Effect(Libraries.Effect, 328, 10, 500, p.Location) { Light = 0 };
                    MapControl.Effects.Add(eff);
                    break;
            }
        }

        private void OnRecvObjectRangeAttackHandler(ServerPacket.ObjectRangeAttack p)
        {
            if (p.ObjectID == User.ObjectID &&
                !Observing) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                QueuedAction action = null;
                if (ob.Race == ObjectType.Player)
                {
                    switch (p.Type)
                    {
                        default:
                            {
                                action = new QueuedAction { Action = ExAction.AttackRange1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                    }
                }
                else
                {
                    switch (p.Type)
                    {
                        case 1:
                            {
                                action = new QueuedAction { Action = ExAction.AttackRange2, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 2:
                            {
                                action = new QueuedAction { Action = ExAction.AttackRange3, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        default:
                            {
                                action = new QueuedAction { Action = ExAction.AttackRange1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                    }
                }
                action.Params.Add(p.TargetID);
                action.Params.Add(p.Target);
                action.Params.Add(p.Spell);
                action.Params.Add(new List<uint>());
                action.Params.Add(p.Level);

                ob.ActionFeed.Add(action);
                return;
            }
        }

        private void OnRecvAddBuffHandler(ServerPacket.AddBuff p)
        {
            ClientBuff buff = p.Buff;

            if (!buff.Paused)
            {
                buff.ExpireTime += CMain.Time;
            }

            if (buff.ObjectID == User.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != buff.Type) continue;

                    BuffsDialog.Buffs[i] = buff;
                    User.RefreshStats();
                    return;
                }

                BuffsDialog.Buffs.Add(buff);
                BuffsDialog.CreateBuff(buff);

                User.RefreshStats();
            }


            if (!buff.Visible || buff.ObjectID <= 0) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != buff.ObjectID) continue;
                if ((ob is PlayerObject) || (ob is MonsterObject))
                {
                    if (!ob.Buffs.Contains(buff.Type))
                    {
                        ob.Buffs.Add(buff.Type);
                    }

                    ob.AddBuffEffect(buff.Type);
                    return;
                }
            }
        }

        private void OnRecvRemoveBuffHandler(ServerPacket.RemoveBuff p)
        {
            if (User.ObjectID == p.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != p.Type) continue;

                    switch (BuffsDialog.Buffs[i].Type)
                    {
                        case BuffType.SwiftFeet:
                            User.Sprint = false;
                            break;
                        case BuffType.Transform:
                            User.TransformType = -1;
                            break;
                    }

                    BuffsDialog.RemoveBuff(i);
                    BuffsDialog.Buffs.RemoveAt(i);
                }
                User.RefreshStats();
            }


            if (p.ObjectID <= 0) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];

                if (ob.ObjectID != p.ObjectID) continue;

                ob.Buffs.Remove(p.Type);
                ob.RemoveBuffEffect(p.Type);
                return;
            }
        }

        private void OnRecvPauseBuffHandler(ServerPacket.PauseBuff p)
        {
            if (User.ObjectID == p.ObjectID)
            {
                for (int i = 0; i < BuffsDialog.Buffs.Count; i++)
                {
                    if (BuffsDialog.Buffs[i].Type != p.Type) continue;

                    User.RefreshStats();

                    if (BuffsDialog.Buffs[i].Paused == p.Paused) return;

                    BuffsDialog.Buffs[i].Paused = p.Paused;

                    if (p.Paused)
                    {
                        BuffsDialog.Buffs[i].ExpireTime -= CMain.Time;
                    }
                    else
                    {
                        BuffsDialog.Buffs[i].ExpireTime += CMain.Time;
                    }
                }
            }
        }

        private void OnRecvObjectHiddenHandler(ServerPacket.ObjectHidden p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Hidden = p.Hidden;
                return;
            }
        }

        private void OnRecvObjectSneakingHandler(ServerPacket.ObjectSneaking p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                // ob.SneakingActive = p.SneakingActive;
                return;
            }
        }

        private void OnRecvObjectLevelEffectsHandler(ServerPacket.ObjectLevelEffects p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID || ob.Race != ObjectType.Player) continue;

                PlayerObject temp = (PlayerObject)ob;

                temp.LevelEffects = p.LevelEffects;

                temp.SetEffects();
                return;
            }
        }

        private void OnRecvRefreshItemHandler(ServerPacket.RefreshItem p)
        {
            Bind(p.Item);

            if (SelectedCell != null && SelectedCell.Item.UniqueID == p.Item.UniqueID)
                SelectedCell = null;

            if (HoverItem != null && HoverItem.UniqueID == p.Item.UniqueID)
            {
                DisposeItemLabel();
                CreateItemLabel(p.Item);
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.Item.UniqueID)
                {
                    User.Inventory[i] = p.Item;
                    User.RefreshStats();
                    return;
                }
            }

            for (int i = 0; i < User.Equipment.Length; i++)
            {
                if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.Item.UniqueID)
                {
                    User.Equipment[i] = p.Item;
                    User.RefreshStats();
                    return;
                }
            }
        }

        private void OnRecvObjectSpellHandler(ServerPacket.ObjectSpell p)
        {
            SpellObject ob = new SpellObject(p.ObjectID);
            ob.Load(p);
        }

        private void OnRecvObjectDecoHandler(ServerPacket.ObjectDeco p)
        {
            DecoObject ob = new DecoObject(p.ObjectID);
            ob.Load(p);
        }

        private void OnRecvUserDashHandler(ServerPacket.UserDash p)
        {
            if (User.Direction == p.Direction && User.CurrentLocation == p.Location)
            {
                MapControl.NextAction = 0;
                return;
            }
            ExAction action = User.CurrentAction == ExAction.DashL ? ExAction.DashR : ExAction.DashL;
            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == ExAction.DashR)
                {
                    action = ExAction.DashL;
                    break;
                }
                if (User.ActionFeed[i].Action == ExAction.DashL)
                {
                    action = ExAction.DashR;
                    break;
                }
            }

            User.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvUserDashFailHandler(ServerPacket.UserDashFail p)
        {
            MapControl.NextAction = 0;
            User.ActionFeed.Add(new QueuedAction { Action = ExAction.DashFail, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvObjectDashHandler(ServerPacket.ObjectDash p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ExAction action = ExAction.DashL;

                if (ob.ActionFeed.Count > 0 && ob.ActionFeed[ob.ActionFeed.Count - 1].Action == action)
                    action = ExAction.DashR;

                ob.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void OnRecvObjectDashFailHandler(ServerPacket.ObjectDashFail p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.DashFail, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void OnRecvUserBackStepHandler(ServerPacket.UserBackStep p)
        {
            if (User.Direction == p.Direction && User.CurrentLocation == p.Location)
            {
                MapControl.NextAction = 0;
                return;
            }
            User.ActionFeed.Add(new QueuedAction { Action = ExAction.Jump, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvObjectBackStepHandler(ServerPacket.ObjectBackStep p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.JumpDistance = p.Distance;

                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.Jump, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void OnRecvUserDashAttackHandler(ServerPacket.UserDashAttack p)
        {
            if (User.Direction == p.Direction && User.CurrentLocation == p.Location)
            {
                MapControl.NextAction = 0;
                return;
            }
            //User.JumpDistance = p.Distance;
            User.ActionFeed.Add(new QueuedAction { Action = ExAction.DashAttack, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvObjectDashAttackHandler(ServerPacket.ObjectDashAttack p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                ob.JumpDistance = p.Distance;

                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.DashAttack, Direction = p.Direction, Location = p.Location });

                return;
            }
        }

        private void OnRecvUserAttackMoveHandler(ServerPacket.UserAttackMove p)//Warrior Skill - SlashingBurst
        {
            MapControl.NextAction = 0;
            if (User.CurrentLocation == p.Location && User.Direction == p.Direction) return;


            MapControl.RemoveObject(User);
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);


            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;


            if (User.Dead) return;


            User.ClearMagic();
            User.QueuedAction = null;


            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == ExAction.Pushed) continue;
                User.ActionFeed.RemoveAt(i);
            }


            User.SetAction();

            User.ActionFeed.Add(new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = p.Direction, Location = p.Location });
        }

        private void OnRecvSetConcentrationHandler(ServerPacket.SetConcentration p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].Race != ObjectType.Player) continue;

                PlayerObject ob = MapControl.Objects[i] as PlayerObject;
                if (ob.ObjectID != p.ObjectID) continue;

                ob.Concentrating = p.Enabled;
                ob.ConcentrateInterrupted = p.Interrupted;

                if (p.Enabled && !p.Interrupted)
                {
                    int idx = InterruptionEffect.GetOwnerEffectID(ob.ObjectID);

                    if (idx < 0)
                    {
                        ob.Effects.Add(new InterruptionEffect(Libraries.Magic3, 1860, 8, 8 * 100, ob, true));
                        SoundManager.PlaySound(20000 + 129 * 10);
                    }
                }
                break;
            }
        }

        private void OnRecvSetElementalHandler(ServerPacket.SetElemental p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].Race != ObjectType.Player) continue;

                PlayerObject ob = MapControl.Objects[i] as PlayerObject;
                if (ob.ObjectID != p.ObjectID) continue;

                ob.HasElements = p.Enabled;
                ob.ElementCasted = p.Casted && User.ObjectID != p.ObjectID;
                ob.ElementsLevel = (int)p.Value;
                int elementType = (int)p.ElementType;
                int maxExp = (int)p.ExpLast;

                if (p.Enabled && p.ElementType > 0)
                {
                    ob.Effects.Add(new ElementsEffect(Libraries.Magic3, 1630 + ((elementType - 1) * 10), 10, 10 * 100, ob, true, 1 + (elementType - 1), maxExp, User.ObjectID == p.ObjectID && ((elementType == 4 || elementType == 3))));
                }
            }
        }

        private void OnRecvRemoveDelayedExplosionHandler(ServerPacket.RemoveDelayedExplosion p)
        {
            //if (p.ObjectID == User.ObjectID) return;

            int effectid = DelayedExplosionEffect.GetOwnerEffectID(p.ObjectID);
            if (effectid >= 0)
                DelayedExplosionEffect.effectlist[effectid].Remove();
        }

        private void OnRecvSetBindingShotHandler(ServerPacket.SetBindingShot p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race != ObjectType.Monster) continue;

                TrackableEffect NetCast = new TrackableEffect(new Effect(Libraries.MagicC, 0, 8, 700, ob));
                NetCast.EffectName = "BindingShotDrop";

                //TrackableEffect NetDropped = new TrackableEffect(new Effect(Libraries.ArcherMagic, 7, 1, 1000, ob, CMain.Time + 600) { Repeat = true, RepeatUntil = CMain.Time + (p.Value - 1500) });
                TrackableEffect NetDropped = new TrackableEffect(new Effect(Libraries.MagicC, 7, 1, 1000, ob) { Repeat = true, RepeatUntil = CMain.Time + (p.Value - 1500) });
                NetDropped.EffectName = "BindingShotDown";

                TrackableEffect NetFall = new TrackableEffect(new Effect(Libraries.MagicC, 8, 8, 700, ob));
                NetFall.EffectName = "BindingShotFall";

                NetDropped.Complete += (o1, e1) =>
                {
                    SoundManager.PlaySound(20000 + 130 * 10 + 6);//sound M130-6
                    ob.Effects.Add(NetFall);
                };
                NetCast.Complete += (o, e) =>
                {
                    SoundManager.PlaySound(20000 + 130 * 10 + 5);//sound M130-5
                    ob.Effects.Add(NetDropped);
                };
                ob.Effects.Add(NetCast);
                break;
            }
        }

        private void OnRecvSendOutputMessageHandler(ServerPacket.SendOutputMessage p)
        {
            OutputMessage(p.Message, p.Type);
        }

        private void OnRecvNPCConsignHandler()
        {
            if (!ExNPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Consign;
            NPCDropDialog.Show();
        }
        
        private void OnRecvConsignItemHandler(ServerPacket.ConsignItem p)
        {
            MirItemCell cell = ExInventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            cell.Item = null;

            User.RefreshStats();
        }
        
        private void OnRecvObjectSitDownHandler(ServerPacket.ObjectSitDown p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race != ObjectType.Monster) continue;
                ob.SitDown = p.Sitting;
                ob.ActionFeed.Add(new QueuedAction { Action = ExAction.SitDown, Direction = p.Direction, Location = p.Location });
                return;
            }
        }

        private void OnRecvBaseStatsInfoHandler(ServerPacket.BaseStatsInfo p)
        {
            User.CoreStats = p.Stats;
            User.RefreshStats();
        }

        private void OnRecvUserNameHandler(ServerPacket.UserName p)
        {
            for (int i = 0; i < UserIdList.Count; i++)
                if (UserIdList[i].Id == p.Id)
                {
                    UserIdList[i].UserName = p.Name;
                    break;
                }
            DisposeItemLabel();
            HoverItem = null;
        }

        private void OnRecvChatItemStatsHandler(ServerPacket.ChatItemStats p)
        {
            //for (int i = 0; i < ChatItemList.Count; i++)
            //    if (ChatItemList[i].ID == p.ChatItemId)
            //    {
            //        ChatItemList[i].ItemStats = p.Stats;
            //        ChatItemList[i].RecievedTick = CMain.Time;
            //    }
        }

        private void OnRecvGuildInviteHandler(ServerPacket.GuildInvite p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0} 링에 가입하시겠습니까?", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.GuildInvite { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.GuildInvite { AcceptInvite = false });

            messageBox.Show();
        }

        private void OnRecvGuildNameRequestHandler(ServerPacket.GuildNameRequest p)
        {
            ExineInputBox inputBox = new ExineInputBox("링 이름을 입력하세요. 3~20자 이내로 입력하세요.");
            inputBox.InputTextBox.TextBox.KeyPress += (o, e) =>
            {
                string Allowed = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                if (!Allowed.Contains(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                    e.Handled = true;
            };
            inputBox.OKButton.Click += (o, e) =>
            {
                if (inputBox.InputTextBox.Text.Contains('\\'))
                {
                    ExChatDialog.ReceiveChat("링 이름에는 \\ 기호를 사용할 수 없습니다!", ChatType.System);
                    inputBox.InputTextBox.Text = "";
                }
                Network.SendPacketToServer(new ClientPacket.GuildNameReturn { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void OnRecvGuildRequestWarHandler(ServerPacket.GuildRequestWar p)
        {
            ExineInputBox inputBox = new ExineInputBox("전쟁하고 싶은 길드를 입력하세요.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.SendPacketToServer(new ClientPacket.GuildWarReturn { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }

        private void OnRecvGuildNoticeChangeHandler(ServerPacket.GuildNoticeChange p)
        {
            if (p.update == -1)
                GuildDialog.NoticeChanged = true;
            else
                GuildDialog.NoticeChange(p.notice);
        }
        private void OnRecvGuildMemberChangeHandler(ServerPacket.GuildMemberChange p)
        {
            switch (p.Status)
            {
                case 0: // logged of
                    GuildDialog.MemberStatusChange(p.Name, false);
                    break;
                case 1: // logged on
                    ExChatDialog.ReceiveChat(String.Format("{0}이(가) 로그인했습니다.", p.Name), ChatType.Guild);
                    GuildDialog.MemberStatusChange(p.Name, true);
                    break;
                case 2://new member
                    ExChatDialog.ReceiveChat(String.Format("{0}님이 링에 가입했습니다.", p.Name), ChatType.Guild);
                    GuildDialog.MemberCount++;
                    GuildDialog.MembersChanged = true;
                    break;
                case 3://kicked member
                    ExChatDialog.ReceiveChat(String.Format("{0}이(가) 링에서 추방되었습니다.", p.Name), ChatType.Guild);
                    GuildDialog.MembersChanged = true;
                    break;
                case 4://member left
                    ExChatDialog.ReceiveChat(String.Format("{0}님이 링을 탈퇴했습니다.", p.Name), ChatType.Guild);
                    GuildDialog.MembersChanged = true;
                    break;
                case 5://rank change (name or different rank)
                    GuildDialog.MembersChanged = true;
                    break;
                case 6: //new rank
                    if (p.Ranks.Count > 0)
                        GuildDialog.NewRankRecieved(p.Ranks[0]);
                    break;
                case 7: //rank option changed
                    if (p.Ranks.Count > 0)
                        GuildDialog.RankChangeRecieved(p.Ranks[0]);
                    break;
                case 8: //my rank changed
                    if (p.Ranks.Count > 0)
                        GuildDialog.MyRankChanged(p.Ranks[0]);
                    break;
                case 255:
                    GuildDialog.NewMembersList(p.Ranks);
                    break;
            }
        }

        private void OnRecvGuildStatusHandler(ServerPacket.GuildStatus p)
        {
            if ((User.GuildName == "") && (p.GuildName != ""))
            {
                GuildDialog.NoticeChanged = true;
                GuildDialog.MembersChanged = true;
            }
            if (p.GuildName == "")
            {
                GuildDialog.Hide();
            }

            if ((User.GuildName == p.GuildName) && (GuildDialog.Level < p.Level))
            {
                //guild leveled
            }
            bool GuildChange = User.GuildName != p.GuildName;
            User.GuildName = p.GuildName;
            User.GuildRankName = p.GuildRankName;
            GuildDialog.Level = p.Level;
            GuildDialog.Experience = p.Experience;
            GuildDialog.MaxExperience = p.MaxExperience;
            GuildDialog.Gold = p.Gold;
            GuildDialog.SparePoints = p.SparePoints;
            GuildDialog.MemberCount = p.MemberCount;
            GuildDialog.MaxMembers = p.MaxMembers;
            GuildDialog.Voting = p.Voting;
            GuildDialog.ItemCount = p.ItemCount;
            GuildDialog.BuffCount = p.BuffCount;
            GuildDialog.StatusChanged(p.MyOptions);
            GuildDialog.MyRankId = p.MyRankId;
            GuildDialog.UpdateMembers();
            //reset guildbuffs
            if (GuildChange)
            {
                GuildDialog.EnabledBuffs.Clear();
                GuildDialog.UpdateActiveStats();
                OnRecvRemoveBuffHandler(new ServerPacket.RemoveBuff { ObjectID = User.ObjectID, Type = BuffType.Guild });
                User.RefreshStats();
            }
        }

        private void OnRecvGuildExpGainHandler(ServerPacket.GuildExpGain p)
        {
            //OutputMessage(string.Format("Guild Experience Gained {0}.", p.Amount));
            GuildDialog.Experience += p.Amount;
        }

        private void OnRecvGuildStorageGoldChangeHandler(ServerPacket.GuildStorageGoldChange p)
        {
            switch (p.Type)
            {
                case 0:
                    ExChatDialog.ReceiveChat(String.Format("{0}님이 길드 자금에 {1}골드를 기부했습니다.", p.Name, p.Amount), ChatType.Guild);
                    GuildDialog.Gold += p.Amount;
                    break;
                case 1:
                    ExChatDialog.ReceiveChat(String.Format("{0}이(가) 길드 자금에서 {1}골드를 가져갔습니다.", p.Name, p.Amount), ChatType.Guild);
                    if (GuildDialog.Gold > p.Amount)
                        GuildDialog.Gold -= p.Amount;
                    else
                        GuildDialog.Gold = 0;
                    break;
                case 2:
                    if (GuildDialog.Gold > p.Amount)
                        GuildDialog.Gold -= p.Amount;
                    else
                        GuildDialog.Gold = 0;
                    break;
                case 3:
                    GuildDialog.Gold += p.Amount;
                    break;
            }
        }

        private void OnRecvGuildStorageItemChangeHandler(ServerPacket.GuildStorageItemChange p)
        {
            MirItemCell fromCell = null;
            MirItemCell toCell = null;
            switch (p.Type)
            {
                case 0://store
                    toCell = GuildDialog.StorageGrid[p.To];

                    if (toCell == null) return;

                    toCell.Locked = false;
                    toCell.Item = p.Item.Item;
                    Bind(toCell.Item);
                    if (p.User != User.Id) return;
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];
                    fromCell.Locked = false;
                    if (fromCell != null)
                        fromCell.Item = null;
                    User.RefreshStats();
                    break;
                case 1://retrieve
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (fromCell == null) return;
                    fromCell.Locked = false;

                    if (p.User != User.Id)
                    {
                        fromCell.Item = null;
                        return;
                    }
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];
                    if (toCell == null) return;
                    toCell.Locked = false;
                    toCell.Item = fromCell.Item;
                    fromCell.Item = null;
                    break;

                case 2:
                    toCell = GuildDialog.StorageGrid[p.To];
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    fromCell.Item = toCell.Item;
                    toCell.Item = p.Item.Item;

                    Bind(toCell.Item);
                    if (fromCell.Item != null)
                        Bind(fromCell.Item);
                    break;
                case 3://failstore
                    fromCell = p.From < User.BeltIdx ? BeltDialog.Grid[p.From] : ExInventoryDialog.Grid[p.From - User.BeltIdx];
                    toCell = GuildDialog.StorageGrid[p.To];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    break;
                case 4://failretrieve
                    toCell = p.To < User.BeltIdx ? BeltDialog.Grid[p.To] : ExInventoryDialog.Grid[p.To - User.BeltIdx];
                    fromCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    toCell.Locked = false;
                    fromCell.Locked = false;
                    break;
                case 5://failmove
                    fromCell = GuildDialog.StorageGrid[p.To];
                    toCell = GuildDialog.StorageGrid[p.From];

                    if (toCell == null || fromCell == null) return;

                    GuildDialog.StorageGrid[p.From].Locked = false;
                    GuildDialog.StorageGrid[p.To].Locked = false;
                    break;
            }
        }
        private void OnRecvGuildStorageListHandler(ServerPacket.GuildStorageList p)
        {
            for (int i = 0; i < p.Items.Length; i++)
            {
                if (i >= GuildDialog.StorageGrid.Length) break;
                if (p.Items[i] == null)
                {
                    GuildDialog.StorageGrid[i].Item = null;
                    continue;
                }
                GuildDialog.StorageGrid[i].Item = p.Items[i].Item;
                Bind(GuildDialog.StorageGrid[i].Item);
            }
        }
         
         
        

        private void OnRecvMarriageRequestHandler(ServerPacket.MarriageRequest p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}이 당신에게 결혼을 요청했습니다.", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.MarriageReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.SendPacketToServer(new ClientPacket.MarriageReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }

        private void OnRecvDivorceRequestHandler(ServerPacket.DivorceRequest p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}이 이혼을 요청했습니다.", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.DivorceReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.SendPacketToServer(new ClientPacket.DivorceReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }

        private void OnRecvMentorRequestHandler(ServerPacket.MentorRequest p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}(레벨 {1})에서 {2}의 방법을 가르쳐 달라고 요청했습니다.", p.Name, p.Level, ExineMainScene.User.Class.ToString()), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.MentorReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.SendPacketToServer(new ClientPacket.MentorReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }

        private bool UpdateGuildBuff(GuildBuff buff, bool Remove = false)
        {
            for (int i = 0; i < GuildDialog.EnabledBuffs.Count; i++)
            {
                if (GuildDialog.EnabledBuffs[i].Id == buff.Id)
                {
                    if (Remove)
                    {
                        GuildDialog.EnabledBuffs.RemoveAt(i);
                    }
                    else
                        GuildDialog.EnabledBuffs[i] = buff;
                    return true;
                }
            }
            return false;
        }

        private void OnRecvGuildBuffListHandler(ServerPacket.GuildBuffList p)
        {
            //getting the list of all guildbuffs on server?
            if (p.GuildBuffs.Count > 0)
                GuildDialog.GuildBuffInfos.Clear();
            for (int i = 0; i < p.GuildBuffs.Count; i++)
            {
                GuildDialog.GuildBuffInfos.Add(p.GuildBuffs[i]);
            }
            //getting the list of all active/removedbuffs?
            for (int i = 0; i < p.ActiveBuffs.Count; i++)
            {
                //if (p.ActiveBuffs[i].ActiveTimeRemaining > 0)
                //    p.ActiveBuffs[i].ActiveTimeRemaining = Convert.ToInt32(CMain.Time / 1000) + (p.ActiveBuffs[i].ActiveTimeRemaining * 60);
                if (UpdateGuildBuff(p.ActiveBuffs[i], p.Remove == 1)) continue;
                if (!(p.Remove == 1))
                {
                    GuildDialog.EnabledBuffs.Add(p.ActiveBuffs[i]);
                    //CreateGuildBuff(p.ActiveBuffs[i]);
                }
            }

            for (int i = 0; i < GuildDialog.EnabledBuffs.Count; i++)
            {
                if (GuildDialog.EnabledBuffs[i].Info == null)
                {
                    GuildDialog.EnabledBuffs[i].Info = GuildDialog.FindGuildBuffInfo(GuildDialog.EnabledBuffs[i].Id);
                }
            }

            ClientBuff buff = BuffsDialog.Buffs.FirstOrDefault(e => e.Type == BuffType.Guild);

            if (GuildDialog.EnabledBuffs.Any(e => e.Active))
            {
                if (buff == null)
                {
                    buff = new ClientBuff { Type = BuffType.Guild, ObjectID = User.ObjectID, Caster = "Guild", Infinite = true, Values = new int[0] };

                    BuffsDialog.Buffs.Add(buff);
                    BuffsDialog.CreateBuff(buff);
                }

                GuildDialog.UpdateActiveStats();
            }
            else
            {
                OnRecvRemoveBuffHandler(new ServerPacket.RemoveBuff { ObjectID = User.ObjectID, Type = BuffType.Guild });
            }

            User.RefreshStats();
        }

        private void OnRecvTradeRequestHandler(ServerPacket.TradeRequest p)
        {
            ExineMessageBox messageBox = new ExineMessageBox(string.Format("플레이어 {0}이(가) 귀하와 거래를 요청했습니다.", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.TradeReply { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => { Network.SendPacketToServer(new ClientPacket.TradeReply { AcceptInvite = false }); messageBox.Dispose(); };

            messageBox.Show();
        }
        private void OnRecvTradeAcceptHandler(ServerPacket.TradeAccept p)
        {
            GuestTradeDialog.GuestName = p.Name;
            TradeDialog.TradeAccept();
        }
        private void OnRecvTradeGoldHandler(ServerPacket.TradeGold p)
        {
            GuestTradeDialog.GuestGold = p.Amount;
            TradeDialog.ChangeLockState(false);
            TradeDialog.RefreshInterface();
        }
        private void OnRecvTradeItemHandler(ServerPacket.TradeItem p)
        {
            GuestTradeDialog.GuestItems = p.TradeItems;
            TradeDialog.ChangeLockState(false);
            TradeDialog.RefreshInterface();
        }
        private void OnRecvTradeConfirm()
        {
            TradeDialog.TradeReset();
        }
        private void OnRecvTradeCancelHandler(ServerPacket.TradeCancel p)
        {
            if (p.Unlock)
            {
                TradeDialog.ChangeLockState(false);
            }
            else
            {
                TradeDialog.TradeReset();

                ExineMessageBox messageBox = new ExineMessageBox("거래가 취소되었습니다.\r\n거래를 하려면 상대방과 직접 대면해야 합니다.", MirMessageBoxButtons.OK);
                messageBox.Show();
            }
        } 

        
        

        private void OnRecvResizeInventoryHandler(ServerPacket.ResizeInventory p)
        {
            Array.Resize(ref User.Inventory, p.Size);
            ExInventoryDialog.RefreshInventory2();
        }

        private void OnRecvResizeStorageHandler(ServerPacket.ResizeStorage p)
        {
            Array.Resize(ref Storage, p.Size);
            User.HasExpandedStorage = p.HasExpandedStorage;
            User.ExpandedStorageExpiryTime = p.ExpiryTime;

            StorageDialog.RefreshStorage2();
        }
         

        public void AddQuestItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.QuestInventory.Length; i++)
                {
                    UserItem temp = User.QuestInventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= (ushort)(temp.Info.StackSize - temp.Count);
                    temp.Count = temp.Info.StackSize;
                }
            }

            for (int i = 0; i < User.QuestInventory.Length; i++)
            {
                if (User.QuestInventory[i] != null) continue;
                User.QuestInventory[i] = item;
                return;
            }
        }

        private void OnRecvRequestReincarnationHandler()
        {
            if (CMain.Time > User.DeadTime && User.CurrentAction == ExAction.Dead)
            {
                ExineMessageBox messageBox = new ExineMessageBox("부활하시겠습니까?", MirMessageBoxButtons.YesNo);

                messageBox.YesButton.Click += (o, e) => Network.SendPacketToServer(new ClientPacket.AcceptReincarnation());

                messageBox.Show();
            }
        }

        private void OnRecvFriendUpdateHandler(ServerPacket.FriendUpdate p)
        {
            ExineMainScene.Scene.FriendDialog.Friends = p.Friends;

            if (ExineMainScene.Scene.FriendDialog.Visible)
            {
                ExineMainScene.Scene.FriendDialog.Update(false);
            }
        }

        private void OnRecvLoverUpdateHandler(ServerPacket.LoverUpdate p)
        {
            ExineMainScene.Scene.RelationshipDialog.LoverName = p.Name;
            ExineMainScene.Scene.RelationshipDialog.Date = p.Date;
            ExineMainScene.Scene.RelationshipDialog.MapName = p.MapName;
            ExineMainScene.Scene.RelationshipDialog.MarriedDays = p.MarriedDays;
            ExineMainScene.Scene.RelationshipDialog.UpdateInterface();
        }

        private void OnRecvMentorUpdateHandler(ServerPacket.MentorUpdate p)
        {
            ExineMainScene.Scene.MentorDialog.MentorName = p.Name;
            ExineMainScene.Scene.MentorDialog.MentorLevel = p.Level;
            ExineMainScene.Scene.MentorDialog.MentorOnline = p.Online;
            ExineMainScene.Scene.MentorDialog.MenteeEXP = p.MenteeEXP;

            ExineMainScene.Scene.MentorDialog.UpdateInterface();
        }


        public void OnRecvRankingsHandler(ServerPacket.Rankings p)
        {
            foreach (RankCharacterInfo info in p.ListingDetails)
            {
                if (RankingList.ContainsKey(info.PlayerId))
                    RankingList[info.PlayerId] = info;
                else
                    RankingList.Add(info.PlayerId, info);
            }
            List<RankCharacterInfo> listings = new List<RankCharacterInfo>();
            foreach (long id in p.Listings)
                listings.Add(RankingList[id]);

            RankingDialog.RecieveRanks(listings, p.RankType, p.MyRank, p.Count);
        }

        public void OnRecvOpendoorHandler(ServerPacket.Opendoor p)
        {
            MapControl.OpenDoor(p.DoorIndex, p.Close);
        }


        private void OnRecvOpenBrowserHandler(ServerPacket.OpenBrowser p)
        {
            BrowserHelper.OpenDefaultBrowser(p.Url);
        }

        public void OnRecvPlaySoundHandler(ServerPacket.PlaySound p)
        {
            SoundManager.PlaySound(p.Sound, false);
        }
        private void OnRecvSetTimerHandler(ServerPacket.SetTimer p)
        {
            ExineMainScene.Scene.TimerControl.AddTimer(p);
        }

        private void OnRecvExpireTimerHandler(ServerPacket.ExpireTimer p)
        {
            ExineMainScene.Scene.TimerControl.ExpireTimer(p.Key);
        }

        private void OnRecvSetCompassHandler(ServerPacket.SetCompass p)
        {
            ExineMainScene.Scene.CompassControl.SetPoint(p.Location);
        }

        private void OnRecvRollHandler(ServerPacket.Roll p)
        {
            //ExineMainScene.Scene.RollControl.Setup(p.Type, p.Page, p.Result, p.AutoRoll);
        }

        public void OnRecvShowNoticeHandler(ServerPacket.UpdateNotice p)
        {
            NoticeDialog.Update(p.Notice);
        }

        #endregion OnRecv From Server Handler 

        public void AddItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.Inventory.Length; i++)
                {
                    UserItem temp = User.Inventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= (ushort)(temp.Info.StackSize - temp.Count);
                    temp.Count = temp.Info.StackSize;
                }
            }

            if (item.Info.Type == ItemType.Potion || item.Info.Type == ItemType.Scroll || (item.Info.Type == ItemType.Script && item.Info.Effect == 1))
            {
                for (int i = 0; i < User.BeltIdx - 2; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }
            else if (item.Info.Type == ItemType.Amulet)
            {
                for (int i = 4; i < User.BeltIdx; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }
            else
            {
                for (int i = User.BeltIdx; i < User.Inventory.Length; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        public static void Bind(UserItem item)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                if (ItemInfoList[i].Index != item.ItemIndex) continue;

                item.Info = ItemInfoList[i];

                for (int s = 0; s < item.Slots.Length; s++)
                {
                    if (item.Slots[s] == null) continue;

                    Bind(item.Slots[s]);
                }

                return;
            }
        }

        public ItemInfo GetItemInfo(int index)
        {
            for (var i = 0; i < ItemInfoList.Count; i++)
            {
                var info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }
            return null;
        }

        public static void BindQuest(ClientQuestProgress quest)
        {
            for (int i = 0; i < QuestInfoList.Count; i++)
            {
                if (QuestInfoList[i].Index != quest.Id) continue;

                quest.QuestInfo = QuestInfoList[i];

                return;
            }
        }

        public Color GradeNameColor(ItemGrade grade)
        {
            switch (grade)
            {
                case ItemGrade.Common:
                    return Color.Yellow;
                case ItemGrade.Rare:
                    return Color.DeepSkyBlue;
                case ItemGrade.Legendary:
                    return Color.DarkOrange;
                case ItemGrade.Mythical:
                    return Color.Plum;
                case ItemGrade.Heroic:
                    return Color.Red;
                default:
                    return Color.Yellow;
            }
        }

        public void DisposeItemLabel()
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Dispose();
            ItemLabel = null;
        } 
        public void DisposeGuildBuffLabel()
        {
            if (GuildBuffLabel != null && !GuildBuffLabel.IsDisposed)
                GuildBuffLabel.Dispose();
            GuildBuffLabel = null;
        }


        public ExineControl NameInfoLabel(UserItem item, bool inspect = false, bool hideDura = false)
        {
            ushort level = inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            string GradeString = "";
            switch (HoverItem.Info.Grade)
            {
                case ItemGrade.None:
                    break;
                case ItemGrade.Common:
                    GradeString = GameLanguage.ItemGradeCommon;
                    break;
                case ItemGrade.Rare:
                    GradeString = GameLanguage.ItemGradeRare;
                    break;
                case ItemGrade.Legendary:
                    GradeString = GameLanguage.ItemGradeLegendary;
                    break;
                case ItemGrade.Mythical:
                    GradeString = GameLanguage.ItemGradeMythical;
                    break;
                case ItemGrade.Heroic:
                    GradeString = GameLanguage.ItemGradeHeroic;
                    break;
            }
            ExineLabel nameLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = GradeNameColor(HoverItem.Info.Grade),
                Location = new Point(4, 4),
                OutLine = true,
                Parent = ItemLabel,
                Text = HoverItem.Info.Grade != ItemGrade.None ? string.Format("{0}{1}{2}", HoverItem.Info.FriendlyName, "\n", GradeString) : HoverItem.Info.FriendlyName,
            };

            if (HoverItem.RefineAdded > 0)
                nameLabel.Text = "(*)" + nameLabel.Text;

            ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, nameLabel.DisplayRectangle.Right + 4),
                Math.Max(ItemLabel.Size.Height, nameLabel.DisplayRectangle.Bottom));

            string text = "";

            if (HoverItem.Info.Durability > 0 && !hideDura)
            {
                switch (HoverItem.Info.Type)
                {
                    case ItemType.Amulet:
                        text += string.Format(" Usage {0}/{1}", HoverItem.CurrentDura, HoverItem.MaxDura);
                        break;
                    case ItemType.Ore:
                        text += string.Format(" Purity {0}", Math.Floor(HoverItem.CurrentDura / 1000M));
                        break;
                    case ItemType.Meat:
                        text += string.Format(" Quality {0}", Math.Floor(HoverItem.CurrentDura / 1000M));
                        break;
                    
                    case ItemType.Food:
                        text += string.Format(" Nutrition {0}", HoverItem.CurrentDura);
                        break;
                    case ItemType.Gem:
                    case ItemType.Potion:
                    case ItemType.Transform: 
                        break;
                    
                    default:
                        text += string.Format(" {0} {1}/{2}", GameLanguage.Durability, Math.Floor(HoverItem.CurrentDura / 1000M),
                                                   Math.Floor(HoverItem.MaxDura / 1000M));
                        break;
                }
            }

            string baseText = "";
            switch (HoverItem.Info.Type)
            {
                case ItemType.Nothing:
                    break;
                case ItemType.Weapon:
                    baseText = GameLanguage.ItemTypeWeapon;
                    break;
                case ItemType.Armour:
                    baseText = GameLanguage.ItemTypeArmour;
                    break;
                case ItemType.Helmet:
                    baseText = GameLanguage.ItemTypeHelmet;
                    break;
                case ItemType.Necklace:
                    baseText = GameLanguage.ItemTypeNecklace;
                    break;
                case ItemType.Bracelet:
                    baseText = GameLanguage.ItemTypeBracelet;
                    break;
                case ItemType.Ring:
                    baseText = GameLanguage.ItemTypeRing;
                    break;
                case ItemType.Amulet:
                    baseText = GameLanguage.ItemTypeAmulet;
                    break;
                case ItemType.Belt:
                    baseText = GameLanguage.ItemTypeBelt;
                    break;
                case ItemType.Boots:
                    baseText = GameLanguage.ItemTypeBoots;
                    break;
                case ItemType.Stone:
                    baseText = GameLanguage.ItemTypeStone;
                    break;
                case ItemType.Torch:
                    baseText = GameLanguage.ItemTypeTorch;
                    break;
                case ItemType.Potion:
                    baseText = GameLanguage.ItemTypePotion;
                    break;
                case ItemType.Ore:
                    baseText = GameLanguage.ItemTypeOre;
                    break;
                case ItemType.Meat:
                    baseText = GameLanguage.ItemTypeMeat;
                    break;
                case ItemType.CraftingMaterial:
                    baseText = GameLanguage.ItemTypeCraftingMaterial;
                    break;
                case ItemType.Scroll:
                    baseText = GameLanguage.ItemTypeScroll;
                    break;
                case ItemType.Gem:
                    baseText = GameLanguage.ItemTypeGem;
                    break;
                
                case ItemType.Book:
                    baseText = GameLanguage.ItemTypeBook;
                    break;
                case ItemType.Script:
                    baseText = GameLanguage.ItemTypeScript;
                    break;
                case ItemType.Reins:
                    baseText = GameLanguage.ItemTypeReins;
                    break;
                case ItemType.Bells:
                    baseText = GameLanguage.ItemTypeBells;
                    break;
                case ItemType.Saddle:
                    baseText = GameLanguage.ItemTypeSaddle;
                    break;
                case ItemType.Ribbon:
                    baseText = GameLanguage.ItemTypeRibbon;
                    break;
                case ItemType.Mask:
                    baseText = GameLanguage.ItemTypeMask;
                    break;
                case ItemType.Food:
                    baseText = GameLanguage.ItemTypeFood;
                    break;
                case ItemType.Hook:
                    baseText = GameLanguage.ItemTypeHook;
                    break;
                case ItemType.Float:
                    baseText = GameLanguage.ItemTypeFloat;
                    break;
                case ItemType.Bait:
                    baseText = GameLanguage.ItemTypeBait;
                    break;
                case ItemType.Finder:
                    baseText = GameLanguage.ItemTypeFinder;
                    break;
                case ItemType.Reel:
                    baseText = GameLanguage.ItemTypeReel;
                    break;
                case ItemType.Fish:
                    baseText = GameLanguage.ItemTypeFish;
                    break;
                case ItemType.Quest:
                    baseText = GameLanguage.ItemTypeQuest;
                    break;
                case ItemType.Awakening:
                    baseText = GameLanguage.ItemTypeAwakening;
                    break;
                
                case ItemType.Transform:
                    baseText = GameLanguage.ItemTypeTransform;
                    break;
                case ItemType.Deco:
                    baseText = GameLanguage.ItemTypeDeco;
                    break;
                case ItemType.MonsterSpawn:
                    baseText = GameLanguage.ItemTypeMonsterSpawn;
                    break;
            }

            if (HoverItem.WeddingRing != -1)
            {
                baseText = GameLanguage.WeddingRing;
            }

            baseText = string.Format(GameLanguage.ItemTextFormat, baseText, string.IsNullOrEmpty(baseText) ? "" : "\n", GameLanguage.Weight, HoverItem.Weight + text);

            ExineLabel etcLabel = new ExineLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Location = new Point(4, nameLabel.DisplayRectangle.Bottom),
                OutLine = true,
                Parent = ItemLabel,
                Text = baseText
            };

            ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, etcLabel.DisplayRectangle.Right + 4),
                Math.Max(ItemLabel.Size.Height, etcLabel.DisplayRectangle.Bottom + 4));

            #region OUTLINE
            ExineControl outLine = new ExineControl
            {
                BackColour = Color.FromArgb(255, 50, 50, 50),
                Border = true,
                BorderColour = Color.Gray,
                NotControl = true,
                Parent = ItemLabel,
                Opacity = 0.4F,
                Location = new Point(0, 0)
            };
            outLine.Size = ItemLabel.Size;
            #endregion

            return outLine;
        }
        public ExineControl AttackInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            bool fishingItem = false;

            switch (HoverItem.Info.Type)
            {
                case ItemType.Hook:
                case ItemType.Float:
                case ItemType.Bait:
                case ItemType.Finder:
                case ItemType.Reel:
                    fishingItem = true;
                    break;
                case ItemType.Weapon:
                    if (Globals.FishingRodShapes.Contains(HoverItem.Info.Shape))
                        fishingItem = true;
                    break;
                default:
                    fishingItem = false;
                    break;
            }

            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;
            string text = "";

            #region Dura gem
            minValue = realItem.Durability;

            if (minValue > 0 && realItem.Type == ItemType.Gem)
            {
                switch (realItem.Shape)
                {
                    default:
                        text = string.Format("Adds +{0} Durability", minValue / 1000);
                        break;
                    case 8:
                        text = string.Format("Seals for {0}", Functions.PrintTimeSpanFromSeconds(minValue * 60));
                        break;
                }

                count++;
                ExineLabel DuraLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DuraLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DuraLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DC
            minValue = realItem.Stats[Stat.MinDC];
            maxValue = realItem.Stats[Stat.MaxDC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxDC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.DC : GameLanguage.DC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} DC", minValue + maxValue + addValue);
                ExineLabel DCLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MC

            minValue = realItem.Stats[Stat.MinMC];
            maxValue = realItem.Stats[Stat.MaxMC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxMC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.MC : GameLanguage.MC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} MC", minValue + maxValue + addValue);
                ExineLabel MCLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SC

            minValue = realItem.Stats[Stat.MinSC];
            maxValue = realItem.Stats[Stat.MaxSC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxSC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.SC : GameLanguage.SC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} SC", minValue + maxValue + addValue);
                ExineLabel SCLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("SC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region LUCK / SUCCESS

            minValue = realItem.Stats[Stat.Luck];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Luck] : 0;

            if (minValue != 0 || addValue != 0)
            {
                count++;

                if (realItem.Type == ItemType.Potion && realItem.Shape == 4)
                {
                    text = string.Format("Exp + {0}% ", minValue + addValue);
                }
                else if (realItem.Type == ItemType.Potion && realItem.Shape == 5)
                {
                    text = string.Format("Drop + {0}% ", minValue + addValue);
                }
                else
                {
                    text = string.Format(minValue + addValue > 0 ? GameLanguage.Luck : "Curse + {0}", Math.Abs(minValue + addValue));
                }

                ExineLabel LUCKLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, LUCKLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, LUCKLabel.DisplayRectangle.Bottom));
            }

            #endregion



            #region ACC

            minValue = realItem.Stats[Stat.Accuracy];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Accuracy] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.Accuracy : GameLanguage.Accuracy2, minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Accuracy", minValue + maxValue + addValue);
                ExineLabel ACCLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Accuracy + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ACCLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ACCLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region HOLY

            minValue = realItem.Stats[Stat.Holy];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel HOLYLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Holy + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? GameLanguage.Holy : GameLanguage.Holy2, minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HOLYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HOLYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region ASPEED

            minValue = realItem.Stats[Stat.AttackSpeed];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.AttackSpeed] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                {
                    string negative = "+";
                    if (addValue < 0) negative = "";
                    text = string.Format(addValue != 0 ? "A.Speed: " + plus + "{0} ({2}{1})" : "A.Speed: " + plus + "{0}", minValue + addValue, addValue, negative);
                    //text = string.Format(addValue > 0 ? "A.Speed: + {0} (+{1})" : "A.Speed: + {0}", minValue + addValue, addValue);
                }
                else
                    text = string.Format("Adds +{0} A.Speed", minValue + maxValue + addValue);
                ExineLabel ASPEEDLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("A.Speed + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ASPEEDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ASPEEDLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region FREEZING

            minValue = realItem.Stats[Stat.Freezing];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Freezing] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Freezing: + {0} (+{1})" : "Freezing: + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Freezing", minValue + maxValue + addValue);
                ExineLabel FREEZINGLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Freezing + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, FREEZINGLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, FREEZINGLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON

            minValue = realItem.Stats[Stat.PoisonAttack];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonAttack] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Poison: + {0} (+{1})" : "Poison: + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Poison", minValue + maxValue + addValue);
                ExineLabel POISONLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Poison + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISONLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISONLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CRITICALRATE / FLEXIBILITY

            minValue = realItem.Stats[Stat.CriticalRate];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.CriticalRate] : 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                ExineLabel CRITICALRATELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Critical Chance + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Critical Chance: + {0} (+{1})" : "Critical Chance: + {0}", minValue + addValue, addValue)
                };

                if (fishingItem)
                {
                    CRITICALRATELabel.Text = string.Format(addValue > 0 ? "Flexibility: + {0} (+{1})" : "Flexibility: + {0}", minValue + addValue, addValue);
                }

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CRITICALRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CRITICALRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CRITICALDAMAGE

            minValue = realItem.Stats[Stat.CriticalDamage];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.CriticalDamage] : 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                ExineLabel CRITICALDAMAGELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Critical Damage + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Critical Damage: + {0} (+{1})" : "Critical Damage: + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CRITICALDAMAGELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CRITICALDAMAGELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Reflect

            minValue = realItem.Stats[Stat.Reflect];
            maxValue = 0;
            addValue = 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                ExineLabel ReflectLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Reflect chance: {0}", minValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ReflectLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ReflectLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Hpdrain

            minValue = realItem.Stats[Stat.HPDrainRatePercent];
            maxValue = 0;
            addValue = 0;

            if ((minValue > 0 || maxValue > 0 || addValue > 0) && (realItem.Type != ItemType.Gem))
            {
                count++;
                ExineLabel HPdrainLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("HP Drain Rate: {0}%", minValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HPdrainLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HPdrainLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Exp Rate

            minValue = realItem.Stats[Stat.ExpRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.ExpRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Exp Rate: " + plus + "{0}% ({2}{1}%)" : "Exp Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                ExineLabel expRateLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, expRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, expRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Drop Rate

            minValue = realItem.Stats[Stat.ItemDropRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.ItemDropRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Drop Rate: " + plus + "{0}% ({2}{1}%)" : "Drop Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                ExineLabel dropRateLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, dropRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, dropRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Gold Rate

            minValue = realItem.Stats[Stat.GoldDropRatePercent];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.GoldDropRatePercent] : 0;

            if (minValue != 0 || maxValue != 0 || addValue != 0)
            {
                string plus = (addValue + minValue < 0) ? "" : "+";

                count++;
                string negative = "+";
                if (addValue < 0) negative = "";
                text = string.Format(addValue != 0 ? "Gold Rate: " + plus + "{0}% ({2}{1}%)" : "Gold Rate: " + plus + "{0}%", minValue + addValue, addValue, negative);

                ExineLabel goldRateLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, goldRateLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, goldRateLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Hero

            if (HoverItem.AddedStats[Stat.Hero] > 0)
            {
                ClientHeroInformation heroInfo = HeroInfoList.FirstOrDefault(x => x.Index == HoverItem.AddedStats[Stat.Hero]);
                if (heroInfo != null)
                {
                    count++;
                    text = heroInfo.ToString();

                    ExineLabel heroLabel = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = text
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, heroLabel.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, heroLabel.DisplayRectangle.Bottom));
                }
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl DefenceInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            bool fishingItem = false;

            switch (HoverItem.Info.Type)
            {
                case ItemType.Hook:
                case ItemType.Float:
                case ItemType.Bait:
                case ItemType.Finder:
                case ItemType.Reel:
                    fishingItem = true;
                    break;
                case ItemType.Weapon:
                    if (HoverItem.Info.Shape == 49 || HoverItem.Info.Shape == 50)
                        fishingItem = true;
                    break;
                default:
                    fishingItem = false;
                    break;
            }

            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;

            string text = "";
            #region AC

            minValue = realItem.Stats[Stat.MinAC];
            maxValue = realItem.Stats[Stat.MaxAC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxAC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.AC : GameLanguage.AC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} AC", minValue + maxValue + addValue);
                ExineLabel ACLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("AC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                if (fishingItem)
                {
                    if (HoverItem.Info.Type == ItemType.Float)
                    {
                        ACLabel.Text = string.Format("Nibble Chance + " + (addValue > 0 ? "{0}~{1}% (+{2})" : "{0}~{1}%"), minValue, maxValue + addValue);
                    }
                    else if (HoverItem.Info.Type == ItemType.Finder)
                    {
                        ACLabel.Text = string.Format("Finder Increase + " + (addValue > 0 ? "{0}~{1}% (+{2})" : "{0}~{1}%"), minValue, maxValue + addValue);
                    }
                    else
                    {
                        ACLabel.Text = string.Format("Success Chance + " + (addValue > 0 ? "{0}% (+{1})" : "{0}%"), maxValue, maxValue + addValue);
                    }
                }

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, ACLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, ACLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAC

            minValue = realItem.Stats[Stat.MinMAC];
            maxValue = realItem.Stats[Stat.MaxMAC];
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MaxMAC] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.MAC : GameLanguage.MAC2, minValue, maxValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} MAC", minValue + maxValue + addValue);
                ExineLabel MACLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("MAC + {0}~{1}", minValue, maxValue + addValue)
                    Text = text
                };

                if (fishingItem)
                {
                    MACLabel.Text = string.Format("AutoReel Chance + {0}%", maxValue + addValue);
                }

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MACLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MACLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXHP

            if (HoverItem.Info.Type != ItemType.MonsterSpawn)
            {
                minValue = realItem.Stats[Stat.HP];
                maxValue = 0;
                addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.HP] : 0;

                if (minValue > 0 || maxValue > 0 || addValue > 0)
                {
                    count++;
                    ExineLabel MAXHPLabel = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        //Text = string.Format(realItem.Type == ItemType.Potion ? "HP + {0} Recovery" : "MAXHP + {0}", minValue + addValue)
                        Text = string.Format(addValue > 0 ? "Max HP + {0} (+{1})" : "Max HP + {0}", minValue + addValue, addValue)
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXHPLabel.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, MAXHPLabel.DisplayRectangle.Bottom));
                }
            }

            #endregion

            #region MAXMP

            minValue = realItem.Stats[Stat.MP];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MP] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXMPLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format(realItem.Type == ItemType.Potion ? "MP + {0} Recovery" : "MAXMP + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Max MP + {0} (+{1})" : "Max MP + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXHPRATE

            minValue = realItem.Stats[Stat.HPRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXHPRATELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max HP + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXHPRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXHPRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXMPRATE

            minValue = realItem.Stats[Stat.MPRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXMPRATELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MP + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMPRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMPRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXACRATE

            minValue = realItem.Stats[Stat.MaxACRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXACRATE = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max AC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXACRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXACRATE.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAXMACRATE

            minValue = realItem.Stats[Stat.MaxMACRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXMACRATELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MAC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMACRATELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMACRATELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region HEALTH_RECOVERY

            minValue = realItem.Stats[Stat.HealthRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.HealthRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel HEALTH_RECOVERYLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(addValue > 0 ? "Health Recovery + {0} (+{1})" : "Health Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HEALTH_RECOVERYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HEALTH_RECOVERYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MANA_RECOVERY

            minValue = realItem.Stats[Stat.SpellRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.SpellRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MANA_RECOVERYLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("ManaRecovery + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Mana Recovery + {0} (+{1})" : "Mana Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MANA_RECOVERYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MANA_RECOVERYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON_RECOVERY

            minValue = realItem.Stats[Stat.PoisonRecovery];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonRecovery] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel POISON_RECOVERYabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Poison Recovery + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Poison Recovery + {0} (+{1})" : "Poison Recovery + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISON_RECOVERYabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISON_RECOVERYabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region AGILITY

            minValue = realItem.Stats[Stat.Agility];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Agility] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? GameLanguage.Agility : GameLanguage.Agility2, minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Agility", minValue + maxValue + addValue);

                ExineLabel AGILITYLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, AGILITYLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, AGILITYLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region STRONG

            minValue = realItem.Stats[Stat.Strong];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.Strong] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel STRONGLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Strong + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Strong + {0} (+{1})" : "Strong + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, STRONGLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, STRONGLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region POISON_RESIST

            minValue = realItem.Stats[Stat.PoisonResist];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.PoisonResist] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Poison Resist + {0} (+{1})" : "Poison Resist + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Poison Resist", minValue + maxValue + addValue);
                ExineLabel POISON_RESISTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, POISON_RESISTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, POISON_RESISTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAGIC_RESIST

            minValue = realItem.Stats[Stat.MagicResist];
            maxValue = 0;
            addValue = (!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) ? HoverItem.AddedStats[Stat.MagicResist] : 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                if (HoverItem.Info.Type != ItemType.Gem)
                    text = string.Format(addValue > 0 ? "Magic Resist + {0} (+{1})" : "Magic Resist + {0}", minValue + addValue, addValue);
                else
                    text = string.Format("Adds +{0} Magic Resist", minValue + maxValue + addValue);
                ExineLabel MAGIC_RESISTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Magic Resist + {0}", minValue + addValue)
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAGIC_RESISTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAGIC_RESISTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region MAX_DC_RATE

            minValue = realItem.Stats[Stat.MaxDCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXDCRATE = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max DC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXDCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXDCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region MAX_MC_RATE

            minValue = realItem.Stats[Stat.MaxMCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXMCRATE = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max MC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXMCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXMCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region MAX_SC_RATE

            minValue = realItem.Stats[Stat.MaxSCRatePercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel MAXSCRATE = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Max SC + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, MAXSCRATE.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, MAXSCRATE.DisplayRectangle.Bottom));
            }
            #endregion

            #region DAMAGE_REDUCTION

            minValue = realItem.Stats[Stat.DamageReductionPercent];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel DAMAGEREDUC = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("All Damage Reduction + {0}%", minValue + addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DAMAGEREDUC.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DAMAGEREDUC.DisplayRectangle.Bottom));
            }
            #endregion
            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl WeightInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;
            int minValue = 0;
            int maxValue = 0;
            int addValue = 0;

            #region HANDWEIGHT

            minValue = realItem.Stats[Stat.HandWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel HANDWEIGHTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Hand Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Hand Weight + {0} (+{1})" : "Hand Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, HANDWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, HANDWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region WEARWEIGHT

            minValue = realItem.Stats[Stat.WearWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel WEARWEIGHTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Wear Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Wear Weight + {0} (+{1})" : "Wear Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, WEARWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, WEARWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BAGWEIGHT

            minValue = realItem.Stats[Stat.BagWeight];
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel BAGWEIGHTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    //Text = string.Format("Bag Weight + {0}", minValue + addValue)
                    Text = string.Format(addValue > 0 ? "Bag Weight + {0} (+{1})" : "Bag Weight + {0}", minValue + addValue, addValue)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BAGWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BAGWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region FASTRUN
            minValue = realItem.CanFastRun == true ? 1 : 0;
            maxValue = 0;
            addValue = 0;

            if (minValue > 0 || maxValue > 0 || addValue > 0)
            {
                count++;
                ExineLabel BAGWEIGHTLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Instant Run")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BAGWEIGHTLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BAGWEIGHTLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region TIME & RANGE
            minValue = 0;
            maxValue = 0;
            addValue = 0;

            if (HoverItem.Info.Type == ItemType.Potion && HoverItem.Info.Durability > 0)
            {
                count++;
                ExineLabel TNRLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Time : {0}", Functions.PrintTimeSpanFromSeconds(HoverItem.Info.Durability * 60))
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, TNRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, TNRLabel.DisplayRectangle.Bottom));
            }

            if (HoverItem.Info.Type == ItemType.Transform && HoverItem.Info.Durability > 0)
            {
                count++;
                ExineLabel TNRLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = addValue > 0 ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Time : {0}", Functions.PrintTimeSpanFromSeconds(HoverItem.Info.Durability, false))
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, TNRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, TNRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
      
        public ExineControl SocketInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);


            int count = 0;

            #region SOCKET

            for (int i = 0; i < item.Slots.Length; i++)
            {
                count++;
                ExineLabel SOCKETLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = (count > realItem.Slots && !realItem.IsFishingRod ) ? Color.Cyan : Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Socket : {0}", item.Slots[i] == null ? "Empty" : item.Slots[i].FriendlyName)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SOCKETLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SOCKETLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                #region SOCKET

                count++;
                ExineLabel SOCKETLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = "Ctrl + Right Click To Open Sockets"
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SOCKETLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SOCKETLabel.DisplayRectangle.Bottom));

                #endregion

                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl NeedInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region LEVEL
            if (realItem.RequiredAmount > 0)
            {
                count++;
                string text;
                Color colour = Color.White;
                switch (realItem.RequiredType)
                {
                    case RequiredType.Level:
                        text = string.Format(GameLanguage.RequiredLevel, realItem.RequiredAmount);
                        if (MapObject.User.Level < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxAC:
                        text = string.Format("Required AC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxMAC:
                        text = string.Format("Required MAC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxMAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxDC:
                        text = string.Format(GameLanguage.RequiredDC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxDC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxMC:
                        text = string.Format(GameLanguage.RequiredMC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxMC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxSC:
                        text = string.Format(GameLanguage.RequiredSC, realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MaxSC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MaxLevel:
                        text = string.Format("Maximum Level : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Level > realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinAC:
                        text = string.Format("Required Base AC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinMAC:
                        text = string.Format("Required Base MAC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinMAC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinDC:
                        text = string.Format("Required Base DC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinDC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinMC:
                        text = string.Format("Required Base MC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinMC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MinSC:
                        text = string.Format("Required Base SC : {0}", realItem.RequiredAmount);
                        if (MapObject.User.Stats[Stat.MinSC] < realItem.RequiredAmount)
                            colour = Color.Red;
                        break;
                    default:
                        text = "Unknown Type Required";
                        break;
                }

                ExineLabel LEVELLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, LEVELLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, LEVELLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CLASS
            if (realItem.RequiredClass != RequiredClass.None)
            {
                count++;
                Color colour = Color.White;

                switch (MapObject.User.Class)
                {
                    case ExineClass.Warrior:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case ExineClass.Wizard:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case ExineClass.Taoist:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                    case ExineClass.Assassin:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Assassin))
                            colour = Color.Red;
                        break;
                    case ExineClass.Archer:
                        if (!realItem.RequiredClass.HasFlag(RequiredClass.Archer))
                            colour = Color.Red;
                        break;
                }

                ExineLabel CLASSLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(GameLanguage.ClassRequired, realItem.RequiredClass)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CLASSLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CLASSLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BUYING - SELLING PRICE
            if (item.Price() > 0)
            {
                count++;
                string text;
                var colour = Color.White;

                text = $"Selling Price : {((long)(item.Price() / 2)).ToString("###,###,##0")} Gold";

                var costLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = colour,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, costLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, costLabel.DisplayRectangle.Bottom));
            }


            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl BindInfoLabel(UserItem item, bool Inspect = false, bool hideAdded = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region DONT_DEATH_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontDeathdrop))
            {
                count++;
                ExineLabel DONT_DEATH_DROPLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't drop on death")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DEATH_DROPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DEATH_DROPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontDrop))
            {
                count++;
                ExineLabel DONT_DROPLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't drop")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DROPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DROPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_UPGRADE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontUpgrade))
            {
                count++;
                ExineLabel DONT_UPGRADELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't upgrade")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_UPGRADELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_UPGRADELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_SELL

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontSell))
            {
                count++;
                ExineLabel DONT_SELLLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't sell")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_SELLLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_SELLLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_TRADE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontTrade))
            {
                count++;
                ExineLabel DONT_TRADELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't trade")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_TRADELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_TRADELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_STORE

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontStore))
            {
                count++;
                ExineLabel DONT_STORELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't store")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_STORELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_STORELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_REPAIR

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DontRepair))
            {
                count++;
                ExineLabel DONT_REPAIRLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't repair")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_SPECIALREPAIR

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.NoSRepair))
            {
                count++;
                ExineLabel DONT_REPAIRLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Can't special repair")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BREAK_ON_DEATH

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.BreakOnDeath))
            {
                count++;
                ExineLabel DONT_REPAIRLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Breaks on death")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_REPAIRLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_REPAIRLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region DONT_DESTROY_ON_DROP

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.DestroyOnDrop))
            {
                count++;
                ExineLabel DONT_DODLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Destroyed when dropped")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, DONT_DODLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, DONT_DODLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region NoWeddingRing

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.NoWeddingRing))
            {
                count++;
                ExineLabel No_WedLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Cannot be a Wedding Ring")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, No_WedLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, No_WedLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region NoHero

            if (HoverItem.Info.Bind != BindMode.None && HoverItem.Info.Bind.HasFlag(BindMode.NoHero))
            {
                count++;
                ExineLabel No_HeroLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Cannot be used by Hero")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, No_HeroLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, No_HeroLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region BIND_ON_EQUIP

            if ((HoverItem.Info.Bind.HasFlag(BindMode.BindOnEquip)) & HoverItem.SoulBoundId == -1)
            {
                count++;
                ExineLabel BOELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("SoulBinds on equip")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BOELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BOELabel.DisplayRectangle.Bottom));
            }
            else if (HoverItem.SoulBoundId != -1)
            {
                count++;
                ExineLabel BOELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = "Soulbound to: " + GetUserName((uint)HoverItem.SoulBoundId)
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, BOELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, BOELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region CURSED

            if ((!hideAdded && (!HoverItem.Info.NeedIdentify || HoverItem.Identified)) && HoverItem.Cursed)
            {
                count++;
                ExineLabel CURSEDLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format("Cursed")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CURSEDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, CURSEDLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region Gems

            if (HoverItem.Info.Type == ItemType.Gem)
            {
                #region UseOn text
                count++;
                string Text = "";
                if (HoverItem.Info.Unique == SpecialItemMode.None)
                {
                    Text = "Cannot be used on any item.";
                }
                else
                {
                    Text = "Can be used on: ";
                }
                ExineLabel GemUseOn = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = Text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemUseOn.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, GemUseOn.DisplayRectangle.Bottom));
                #endregion
                #region Weapon text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Paralize))
                {
                    ExineLabel GemWeapon = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Weapon"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemWeapon.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemWeapon.DisplayRectangle.Bottom));
                }
                #endregion
                #region Armour text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Teleport))
                {
                    ExineLabel GemArmour = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Armour"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemArmour.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemArmour.DisplayRectangle.Bottom));
                }
                #endregion
                #region Helmet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.ClearRing))
                {
                    ExineLabel Gemhelmet = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Helmet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemhelmet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemhelmet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Necklace text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Protection))
                {
                    ExineLabel Gemnecklace = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Necklace"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemnecklace.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemnecklace.DisplayRectangle.Bottom));
                }
                #endregion
                #region Bracelet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Revival))
                {
                    ExineLabel GemBracelet = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Bracelet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemBracelet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemBracelet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Ring text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Muscle))
                {
                    ExineLabel GemRing = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Ring"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GemRing.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, GemRing.DisplayRectangle.Bottom));
                }
                #endregion
                #region Amulet text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Flame))
                {
                    ExineLabel Gemamulet = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Amulet"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemamulet.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemamulet.DisplayRectangle.Bottom));
                }
                #endregion
                #region Belt text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Healing))
                {
                    ExineLabel Gembelt = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Belt"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gembelt.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gembelt.DisplayRectangle.Bottom));
                }
                #endregion
                #region Boots text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Probe))
                {
                    ExineLabel Gemboots = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Boots"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemboots.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemboots.DisplayRectangle.Bottom));
                }
                #endregion
                #region Stone text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.Skill))
                {
                    ExineLabel Gemstone = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Stone"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemstone.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemstone.DisplayRectangle.Bottom));
                }
                #endregion
                #region Torch text
                count++;
                if (HoverItem.Info.Unique.HasFlag(SpecialItemMode.NoDuraLoss))
                {
                    ExineLabel Gemtorch = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = "-Candle"
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, Gemtorch.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, Gemtorch.DisplayRectangle.Bottom));
                }
                #endregion
            }

            #endregion

            #region CANTAWAKEN

            //if ((HoverItem.Info.CanAwakening != true) && (HoverItem.Info.Type != ItemType.Gem))
            //{
            //    count++;
            //    MirLabel CANTAWAKENINGLabel = new MirLabel
            //    {
            //        AutoSize = true,
            //        ForeColour = Color.Yellow,
            //        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
            //        OutLine = true,
            //        Parent = ItemLabel,
            //        Text = string.Format("Can't awaken")
            //    };

            //    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, CANTAWAKENINGLabel.DisplayRectangle.Right + 4),
            //        Math.Max(ItemLabel.Size.Height, CANTAWAKENINGLabel.DisplayRectangle.Bottom));
            //}

            #endregion

            #region EXPIRE

            if (HoverItem.ExpireInfo != null)
            {
                double remainingSeconds = (HoverItem.ExpireInfo.ExpiryDate - CMain.Now).TotalSeconds;

                count++;
                ExineLabel EXPIRELabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = remainingSeconds > 0 ? string.Format("Expires in {0}", Functions.PrintTimeSpanFromSeconds(remainingSeconds)) : "Expired"
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, EXPIRELabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, EXPIRELabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SEALED

            if (HoverItem.SealedInfo != null)
            {
                double remainingSeconds = (HoverItem.SealedInfo.ExpiryDate - CMain.Now).TotalSeconds;

                if (remainingSeconds > 0)
                {
                    count++;
                    ExineLabel SEALEDLabel = new ExineLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.Red,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = true,
                        Parent = ItemLabel,
                        Text = remainingSeconds > 0 ? string.Format("Sealed for {0}", Functions.PrintTimeSpanFromSeconds(remainingSeconds)) : ""
                    };

                    ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SEALEDLabel.DisplayRectangle.Right + 4),
                        Math.Max(ItemLabel.Size.Height, SEALEDLabel.DisplayRectangle.Bottom));
                }
            }

            #endregion

            if (HoverItem.RentalInformation?.RentalLocked == false)
            {
                count++;
                ExineLabel OWNERLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.DarkKhaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = "Item rented from: " + HoverItem.RentalInformation.OwnerName
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, OWNERLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, OWNERLabel.DisplayRectangle.Bottom));

                double remainingTime = (HoverItem.RentalInformation.ExpiryDate - CMain.Now).TotalSeconds;

                count++;
                ExineLabel RENTALLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Khaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = remainingTime > 0 ? string.Format("Rental expires in: {0}", Functions.PrintTimeSpanFromSeconds(remainingTime)) : "Rental expired"
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, RENTALLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, RENTALLabel.DisplayRectangle.Bottom));
            }
            else if (HoverItem.RentalInformation?.RentalLocked == true && HoverItem.RentalInformation.ExpiryDate > CMain.Now)
            {
                count++;
                var remainingTime = (HoverItem.RentalInformation.ExpiryDate - CMain.Now).TotalSeconds;
                var RentalLockLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.DarkKhaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = remainingTime > 0 ? string.Format("Rental lock expires in: {0}", Functions.PrintTimeSpanFromSeconds(remainingTime)) : "Rental lock expired"
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, RentalLockLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, RentalLockLabel.DisplayRectangle.Bottom));
            }

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl OverlapInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;


            #region GEM

            if (realItem.Type == ItemType.Gem)
            {
                string text = "";

                switch (realItem.Shape)
                {
                    case 1:
                        text = "Hold CTRL and left click to repair weapons.";
                        break;
                    case 2:
                        text = "Hold CTRL and left click to repair armour\nand accessory items.";
                        break;
                    case 3:
                    case 4:
                        text = "Hold CTRL and left click to combine with an item.";
                        break;
                    case 8:
                        text = "Hold CTRL and left click to seal an item.";
                        break;
                }
                count++;
                ExineLabel GEMLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = text
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, GEMLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, GEMLabel.DisplayRectangle.Bottom));
            }

            #endregion

            #region SPLITUP

            if (realItem.StackSize > 1 && realItem.Type != ItemType.Gem)
            {
                count++;
                ExineLabel SPLITUPLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = string.Format(GameLanguage.MaxCombine, realItem.StackSize, "\n")
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, SPLITUPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, SPLITUPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }
        public ExineControl StoryInfoLabel(UserItem item, bool Inspect = false)
        {
            ushort level = Inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = Inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

            int count = 0;

            #region TOOLTIP

            if (realItem.Type == ItemType.Scroll && realItem.Shape == 7)//Credit Scroll
            {
                HoverItem.Info.ToolTip = string.Format("Adds {0} Credits to your Account.", HoverItem.Info.Price);
            }

            if (!string.IsNullOrEmpty(HoverItem.Info.ToolTip))
            {
                count++;

                ExineLabel IDLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.DarkKhaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = GameLanguage.ItemDescription
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, IDLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, IDLabel.DisplayRectangle.Bottom));

                ExineLabel TOOLTIPLabel = new ExineLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Khaki,
                    Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                    OutLine = true,
                    Parent = ItemLabel,
                    Text = HoverItem.Info.ToolTip
                };

                ItemLabel.Size = new Size(Math.Max(ItemLabel.Size.Width, TOOLTIPLabel.DisplayRectangle.Right + 4),
                    Math.Max(ItemLabel.Size.Height, TOOLTIPLabel.DisplayRectangle.Bottom));
            }

            #endregion

            if (count > 0)
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height + 4);

                #region OUTLINE
                ExineControl outLine = new ExineControl
                {
                    BackColour = Color.FromArgb(255, 50, 50, 50),
                    Border = true,
                    BorderColour = Color.Gray,
                    NotControl = true,
                    Parent = ItemLabel,
                    Opacity = 0.4F,
                    Location = new Point(0, 0)
                };
                outLine.Size = ItemLabel.Size;
                #endregion

                return outLine;
            }
            else
            {
                ItemLabel.Size = new Size(ItemLabel.Size.Width, ItemLabel.Size.Height - 4);
            }
            return null;
        }

        public void CreateItemLabel(UserItem item, bool inspect = false, bool hideDura = false, bool hideAdded = false)
        {
            CMain.DebugText = CMain.Random.Next(1, 100).ToString();

            if (item == null || HoverItem != item)
            {
                DisposeItemLabel();

                if (item == null)
                {
                    HoverItem = null;
                    return;
                }
            }

            if (item == HoverItem && ItemLabel != null && !ItemLabel.IsDisposed) return;
            ushort level = inspect ? InspectDialog.Level : MapObject.User.Level;
            ExineClass job = inspect ? InspectDialog.Class : MapObject.User.Class;
            HoverItem = item;
            ItemInfo realItem = Functions.GetRealItem(item.Info, level, job, ItemInfoList);

            ItemLabel = new ExineControl
            {
                BackColour = Color.FromArgb(255, 0, 0, 0),
                Border = true,
                BorderColour = ((HoverItem.CurrentDura == 0 && HoverItem.MaxDura != 0) ? Color.Red : Color.FromArgb(255, 148, 146, 148)),
                DrawControlTexture = true,
                NotControl = true,
                Parent = this,
                Opacity = 0.8F
            };

            //Name Info Label
            ExineControl[] outlines = new ExineControl[10];
            outlines[0] = NameInfoLabel(item, inspect, hideDura);
            //Attribute Info1 Label - Attack Info
            outlines[1] = AttackInfoLabel(item, inspect, hideAdded);
            //Attribute Info2 Label - Defence Info
            outlines[2] = DefenceInfoLabel(item, inspect, hideAdded);
            //Attribute Info3 Label - Weight Info
            outlines[3] = WeightInfoLabel(item, inspect);
             
            //Socket Info Label
            outlines[5] = SocketInfoLabel(item, inspect);
            //need Info Label
            outlines[6] = NeedInfoLabel(item, inspect);
            //Bind Info Label
            outlines[7] = BindInfoLabel(item, inspect, hideAdded);
            //Overlap Info Label
            outlines[8] = OverlapInfoLabel(item, inspect);
            //Story Label
            outlines[9] = StoryInfoLabel(item, inspect);

            foreach (var outline in outlines)
            {
                if (outline != null)
                {
                    outline.Size = new Size(ItemLabel.Size.Width, outline.Size.Height);
                }
            }

            //ItemLabel.Visible = true;
        }
      
        public static ItemInfo GetInfo(int index)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }

            return null;
        }

        public string GetUserName(uint id)
        {
            for (int i = 0; i < UserIdList.Count; i++)
            {
                UserId who = UserIdList[i];
                if (id == who.Id)
                    return who.UserName;
            }
            Network.SendPacketToServer(new ClientPacket.RequestUserName { UserID = id });
            UserIdList.Add(new UserId() { Id = id, UserName = "Unknown" });
            return "";
        }

        public class UserId
        {
            public long Id = 0;
            public string UserName = "";
        }

        public class OutPutMessage
        {
            public string Message;
            public long ExpireTime;
            public OutputMessageType Type;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Scene = null;
                User = null;

                MoveTime = 0;
                AttackTime = 0;
                NextRunTime = 0;
                LastRunTime = 0;
                CanMove = false;
                CanRun = false;

                MapControl = null;
                ExMainDialog = null;
                ExChatDialog = null;
                ExChatControl = null;
                ExInventoryDialog = null;
                ExCharacterDialog = null;
                StorageDialog = null;
                BeltDialog = null;
                MiniMapDialog = null;
                InspectDialog = null;
                OptionDialog = null;
                MenuDialog = null;
                ExNPCDialog = null;
                QuestDetailDialog = null;
                QuestListDialog = null;
                QuestLogDialog = null;
                QuestTrackingDialog = null; 
                MentorDialog = null;

                //NewHeroDialog = null; //add k333123

                RelationshipDialog = null;
                CharacterDuraPanel = null;
                DuraStatusPanel = null;

                HoverItem = null;
                SelectedCell = null;
                PickedUpGold = false;

                UseItemTime = 0;
                PickUpTime = 0;
                InspectTime = 0;

                DisposeItemLabel();

                AMode = 0;
                PMode = 0;
                Lights = 0;

                NPCTime = 0;
                NPCID = 0;
                DefaultNPCID = 0;

                for (int i = 0; i < OutputLines.Length; i++)
                    if (OutputLines[i] != null && OutputLines[i].IsDisposed)
                        OutputLines[i].Dispose();

                OutputMessages.Clear();
                OutputMessages = null;
            }

            base.Dispose(disposing);
        }

        #endregion

    }

    public sealed class MapControl : ExineControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }


        public static List<MapObject> Objects = new List<MapObject>();

        public const int CellWidth = 48;
        //public const int CellHeight = 32;
        public const int CellHeight = 24;

        public static int OffSetX;
        public static int OffSetY;

        public static int ViewRangeX;
        public static int ViewRangeY;

        private bool _autoPath;
        public bool AutoPath
        {
            get
            {
                return _autoPath;
            }
            set
            {
                if (_autoPath == value) return;
                _autoPath = value;

                if (!_autoPath)
                    CurrentPath = null;
            }
        }

        public PathFinder PathFinder;
        public List<Node> CurrentPath = null;

        public static Point MapLocation
        {
            get { return ExineMainScene.User == null ? Point.Empty : new Point(MouseLocation.X / CellWidth - OffSetX, MouseLocation.Y / CellHeight - OffSetY).Add(ExineMainScene.User.CurrentLocation); }
        }

        public static Point ToMouseLocation(Point p)
        {
            return new Point((p.X - MapObject.User.Movement.X + OffSetX) * CellWidth, (p.Y - MapObject.User.Movement.Y + OffSetY) * CellHeight).Add(MapObject.User.OffSetMove);
        }

        public static MouseButtons MapButtons;
        public static Point MouseLocation;
        public static long InputDelay;

        private static long nextAction;
        public static long NextAction
        {
            get { return nextAction; }
            set
            {
                if (ExineMainScene.Observing) return;
                nextAction = value;
            }
        }

        public CellInfo[,] M2CellInfo;
        public List<Door> Doors = new List<Door>();
        public int Width, Height;

        public int Index;
        public string FileName = String.Empty;
        public string Title = String.Empty;
        public ushort MiniMap, BigMap, Music, SetMusic;
        public LightSetting Lights;
        public bool Lightning, Fire;
        public byte MapDarkLight;
        public long LightningTime, FireTime;

        public bool FloorValid, LightsValid;

        public long OutputDelay;

        private static bool _awakeningAction;
        public static bool AwakeningAction
        {
            get { return _awakeningAction; }
            set
            {
                if (_awakeningAction == value) return;
                _awakeningAction = value;
            }
        }

        private static bool _autoRun;
        public static bool AutoRun
        {
            get { return _autoRun; }
            set
            {
                if (_autoRun == value) return;
                _autoRun = value;
                if (ExineMainScene.Scene != null)
                    ExineMainScene.Scene.ExChatDialog.ReceiveChat(value ? "[AutoRun: On]" : "[AutoRun: Off]", ChatType.Hint);
            }

        }
        public static bool AutoHit;

        public int AnimationCount;

        public static List<Effect> Effects = new List<Effect>();

        public MapControl()
        {
            MapButtons = MouseButtons.None;

            OffSetX = Settings.ScreenWidth / 2 / CellWidth;
            OffSetY = Settings.ScreenHeight / 2 / CellHeight - 1;

            //ViewRangeX = OffSetX + 6;
            //ViewRangeY = OffSetY + 6;
            ViewRangeX = OffSetX + 15; //mod k333123
            ViewRangeY = OffSetY + 15; //mod k333123

            Size = new Size(Settings.ScreenWidth, Settings.ScreenHeight);
            DrawControlTexture = true;
            BackColour = Color.Black;

            MouseDown += OnMouseDown;
            MouseMove += (o, e) => MouseLocation = e.Location;
            Click += OnMouseClick;
        }

        public void ResetMap()
        {
            ExineMainScene.Scene.ExNPCDialog.Hide();

            MapObject.MouseObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.MagicObjectID = 0;

            if (M2CellInfo != null)
            {
                for (var i = Objects.Count - 1; i >= 0; i--)
                {
                    var obj = Objects[i];
                    if (obj == null) continue;

                    obj.Remove();
                }
            }

            Objects.Clear();
            Effects.Clear();
            Doors.Clear();

            if (User != null)
                Objects.Add(User);
        }

        public void LoadMap()
        {
            ResetMap();

            MapObject.MouseObjectID = 0;
            MapObject.TargetObjectID = 0;
            MapObject.MagicObjectID = 0;

            MapReader Map = new MapReader(FileName);
            M2CellInfo = Map.MapCells;
            Width = Map.Width;
            Height = Map.Height;

            PathFinder = new PathFinder(this);

            Console.WriteLine("Music:" + Music + " SetMusic:" + SetMusic);

            try
            {
                if (SetMusic != Music)
                {
                    //SoundManager.Music?.Dispose();
                    SoundManager.StopMusic();//add k333123
                    SoundManager.PlayMusic((int)Music, true);
                }
            }
            catch (Exception ex)
            {
                // Do nothing. index was not valid.
                Console.WriteLine(ex.ToString());
            }

            SetMusic = Music;
            SoundList.Music = Music;
        }


        public void Process()
        {
            Processdoors();
            User.Process();
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = Objects[i];
                if (ob == User) continue;
                //  if (ob.ActionFeed.Count > 0 || ob.Effects.Count > 0 || GameScene.CanMove || CMain.Time >= ob.NextMotion)
                ob.Process();
            }

            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Process();

            if (MapObject.TargetObject != null && MapObject.TargetObject is MonsterObject && MapObject.TargetObject.AI == 64)
                MapObject.TargetObjectID = 0;
            if (MapObject.MagicObject != null && MapObject.MagicObject is MonsterObject && MapObject.MagicObject.AI == 64)
                MapObject.MagicObjectID = 0;

            CheckInput();


            MapObject bestmouseobject = null;
            for (int y = MapLocation.Y + 2; y >= MapLocation.Y - 2; y--)
            {
                if (y >= Height) continue;
                if (y < 0) break;
                for (int x = MapLocation.X + 2; x >= MapLocation.X - 2; x--)
                {
                    if (x >= Width) continue;
                    if (x < 0) break;
                    CellInfo cell = M2CellInfo[x, y];
                    if (cell.CellObjects == null) continue;

                    for (int i = cell.CellObjects.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = cell.CellObjects[i];
                        if (ob == MapObject.User || !ob.MouseOver(CMain.MPoint)) continue;

                        if (MapObject.MouseObject != ob)
                        {
                            if (ob.Dead)
                            {
                                if (!Settings.TargetDead && ExineMainScene.TargetDeadTime <= CMain.Time) continue;

                                bestmouseobject = ob;
                                //continue;
                            }
                            MapObject.MouseObjectID = ob.ObjectID;
                            Redraw();
                        }
                        if (bestmouseobject != null && MapObject.MouseObject == null)
                        {
                            MapObject.MouseObjectID = bestmouseobject.ObjectID;
                            Redraw();
                        }
                        return;
                    }
                }
            }


            if (MapObject.MouseObject != null)
            {
                MapObject.MouseObjectID = 0;
                Redraw();
            }
        }

        public static MapObject GetObject(uint targetID)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.ObjectID != targetID) continue;
                return ob;
            }
            return null;
        }

        public override void Draw()
        {
            //Do nothing.
        }

        protected override void CreateTexture()
        { 
            if (User == null) return;
             
            if (!FloorValid)
                DrawFloor(); 
             
            if (Size != TextureSize)
                DisposeTexture();
             
            if (ControlTexture == null || ControlTexture.Disposed)
            {
                DXManager.ControlList.Add(this);
                ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                TextureSize = Size;
            }

            Surface oldSurface = DXManager.CurrentSurface;
            Surface surface = ControlTexture.GetSurfaceLevel(0);
            DXManager.SetSurface(surface);
            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0, 0);
             
            DrawBackground();
             
            if (FloorValid)
            {
                DXManager.Draw(DXManager.FloorTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White);
            }
             
            DrawObjects(); 

            //Render Death, 

            LightSetting setting = Lights == LightSetting.Normal ? ExineMainScene.Scene.Lights : Lights;

            if (setting != LightSetting.Day || ExineMainScene.User.Poison.HasFlag(PoisonType.Blindness))
            {
                DrawLights(setting);
            }

            if (Settings.DropView || ExineMainScene.DropViewTime > CMain.Time)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    ItemObject ob = Objects[i] as ItemObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation))
                        ob.DrawName();
                }
            }

            if (MapObject.MouseObject != null && !(MapObject.MouseObject is ItemObject))
                MapObject.MouseObject.DrawName();

            int offSet = 0;

            if (Settings.DisplayBodyName)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    MonsterObject ob = Objects[i] as MonsterObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation)) continue;
                    ob.DrawName();
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                ItemObject ob = Objects[i] as ItemObject;
                if (ob == null) continue;

                if (!ob.MouseOver(MouseLocation)) continue;
                ob.DrawName(offSet);
                offSet -= ob.NameLabel.Size.Height + (ob.NameLabel.Border ? 1 : 0);
            }

            if (MapObject.User.MouseOver(MouseLocation))
                MapObject.User.DrawName();

            DXManager.SetSurface(oldSurface);
            surface.Dispose();
            TextureValid = true;

        }
        protected internal override void DrawControl()
        {
            if (!DrawControlTexture)
                return;

            if (!TextureValid)
                CreateTexture();

            if (ControlTexture == null || ControlTexture.Disposed)
                return;

            float oldOpacity = DXManager.Opacity;

            if (MapObject.User.Dead) DXManager.SetGrayscale(true);

            DXManager.DrawOpaque(ControlTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White, Opacity);

            if (MapObject.User.Dead) DXManager.SetGrayscale(false);

            CleanTime = CMain.Time + Settings.CleanDelay;
        }

        private void DrawFloor()
        { 
            if (DXManager.FloorTexture == null || DXManager.FloorTexture.Disposed)
            {
                DXManager.FloorTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                DXManager.FloorSurface = DXManager.FloorTexture.GetSurfaceLevel(0);
            }


            Surface oldSurface = DXManager.CurrentSurface;

            DXManager.SetSurface(DXManager.FloorSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0); //Color.Black

            int index;
            int drawY, drawX;
             
            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY; y++)
            {
                //if (y <= 0 || y % 2 == 1) continue; //k333123  
                if (y <= 0) continue; //k333123 240816! this is solution for graphic freeze
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    //if (x <= 0 || x % 2 == 1) continue; //k333123
                    if (x <= 0) continue; //k333123 240816! this is solution for graphic freeze
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet
                    if ((M2CellInfo[x, y].BackImage == 0) || (M2CellInfo[x, y].BackIndex == -1)) continue;
                    index = (M2CellInfo[x, y].BackImage & 0x1FFFFFFF) - 1;
                    Libraries.MapLibs[M2CellInfo[x, y].BackIndex].Draw(index, drawX, drawY);
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 5; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet

                    index = M2CellInfo[x, y].MiddleImage - 1;

                    if ((index < 0) || (M2CellInfo[x, y].MiddleIndex == -1)) continue;
                    if (M2CellInfo[x, y].MiddleIndex >= 0)    //M2P '> 199' changed to '>= 0' to include mir2 libraries. Fixes middle layer tile strips draw. Also changed in 'Draw mir3 middle layer' bellow.
                    {//mir3 mid layer is same level as front layer not real middle + it cant draw index -1 so 2 birds in one stone :p
                        Size imgSize = Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].GetSize(index);

                        if (imgSize.Width != CellWidth || imgSize.Height != CellHeight) continue;
                    }
                    Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].Draw(index, drawX, drawY);
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 5; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X; //Moving OffSet

                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (index == -1) continue;
                    int fileIndex = M2CellInfo[x, y].FrontIndex;
                    if (fileIndex == -1) continue;
                    Size imgSize = Libraries.MapLibs[fileIndex].GetSize(index);
                    if (fileIndex == 200) continue; //fixes random bad spots on old school 4.map
                    if (M2CellInfo[x, y].DoorIndex > 0)
                    {
                        Door DoorInfo = GetDoor(M2CellInfo[x, y].DoorIndex);
                        if (DoorInfo == null)
                        {
                            DoorInfo = new Door() { index = M2CellInfo[x, y].DoorIndex, DoorState = 0, ImageIndex = 0, LastTick = CMain.Time };
                            Doors.Add(DoorInfo);
                        }
                        else
                        {
                            if (DoorInfo.DoorState != 0)
                            {
                                index += (DoorInfo.ImageIndex + 1) * M2CellInfo[x, y].DoorOffset;//'bad' code if you want to use animation but it's gonna depend on the animation > has to be custom designed for the animtion
                            }
                        }
                    }

                    if (index < 0 || ((imgSize.Width != CellWidth || imgSize.Height != CellHeight) && ((imgSize.Width != CellWidth * 2) || (imgSize.Height != CellHeight * 2)))) continue;
                    Libraries.MapLibs[fileIndex].Draw(index, drawX, drawY);
                }
            }

            DXManager.SetSurface(oldSurface);

            FloorValid = true;
        }

        private void DrawBackground()
        { 
        }

        private void DrawObjects()
        { 
            if (Settings.Effect)
            {
                for (int i = Effects.Count - 1; i >= 0; i--)
                {
                    if (!Effects[i].DrawBehind) continue;
                    Effects[i].Draw();
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawDeadObjects();
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                int drawY = (y - User.Movement.Y + OffSetY + 1) * CellHeight + User.OffSetMove.Y;

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X;
                    int index;
                    byte animation;
                    bool blend;
                    Size imgSize;
                    #region Draw shanda's tile animation layer
                    index = M2CellInfo[x, y].TileAnimationImage;
                    animation = M2CellInfo[x, y].TileAnimationFrames;
                    if ((index > 0) & (animation > 0))
                    {
                        index--;
                        int animationoffset = M2CellInfo[x, y].TileAnimationOffset ^ 0x2000;
                        index += animationoffset * (AnimationCount % animation);
                        Libraries.MapLibs[190].DrawUp(index, drawX, drawY);
                    }

                    #endregion

                    #region Draw mir3 middle layer
                    if ((M2CellInfo[x, y].MiddleIndex >= 0) && (M2CellInfo[x, y].MiddleIndex != -1))   //M2P '> 199' changed to '>= 0' to include mir2 libraries. Fixes middle layer tile strips draw. Also changed in 'DrawFloor' above.
                    {
                        index = M2CellInfo[x, y].MiddleImage - 1;
                        if (index > 0)
                        {
                            animation = M2CellInfo[x, y].MiddleAnimationFrame;
                            blend = false;
                            if ((animation > 0) && (animation < 255))
                            {
                                if ((animation & 0x0f) > 0)
                                {
                                    blend = true;
                                    animation &= 0x0f;
                                }
                                if (animation > 0)
                                {
                                    byte animationTick = M2CellInfo[x, y].MiddleAnimationTick;
                                    index += (AnimationCount % (animation + (animation * animationTick))) / (1 + animationTick);

                                    if (blend && (animation == 10 || animation == 8)) //diamond mines, abyss blends
                                    {
                                        Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUpBlend(index, new Point(drawX, drawY));
                                    }
                                    else
                                    {
                                        Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUp(index, drawX, drawY);
                                    }
                                }
                            }
                            imgSize = Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].GetSize(index);
                            if ((imgSize.Width != CellWidth || imgSize.Height != CellHeight) && (imgSize.Width != (CellWidth * 2) || imgSize.Height != (CellHeight * 2)) && !blend)
                            {
                                Libraries.MapLibs[M2CellInfo[x, y].MiddleIndex].DrawUp(index, drawX, drawY);
                            }
                        }
                    }
                    #endregion

                    #region Draw front layer
                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;

                    if (index < 0) continue;

                    int fileIndex = M2CellInfo[x, y].FrontIndex;
                    if (fileIndex == -1) continue;

                    //k333123 240610 map static object check!
                    //fileIndex : 10000 ~ 20000 => Map_10000_FrontTile.lib
                    //if(fileIndex>9999) Console.WriteLine("fileIndex:" + fileIndex);
                    //Console.WriteLine("!!!!!!!!!!!!!fileIndex:" + fileIndex + " index:"+ index);


                    animation = M2CellInfo[x, y].FrontAnimationFrame;

                    if ((animation & 0x80) > 0)
                    {
                        blend = true;
                        animation &= 0x7F;
                    }
                    else
                        blend = false;


                    if (animation > 0)
                    {
                        byte animationTick = M2CellInfo[x, y].FrontAnimationTick;
                        index += (AnimationCount % (animation + (animation * animationTick))) / (1 + animationTick);
                    }


                    if (M2CellInfo[x, y].DoorIndex > 0)
                    {
                        Door DoorInfo = GetDoor(M2CellInfo[x, y].DoorIndex);
                        if (DoorInfo == null)
                        {
                            DoorInfo = new Door() { index = M2CellInfo[x, y].DoorIndex, DoorState = 0, ImageIndex = 0, LastTick = CMain.Time };
                            Doors.Add(DoorInfo);
                        }
                        else
                        {
                            if (DoorInfo.DoorState != 0)
                            {
                                index += (DoorInfo.ImageIndex + 1) * M2CellInfo[x, y].DoorOffset;//'bad' code if you want to use animation but it's gonna depend on the animation > has to be custom designed for the animtion
                            }
                        }
                    }

                    imgSize = Libraries.MapLibs[fileIndex].GetSize(index);
                    if (imgSize.Width == CellWidth && imgSize.Height == CellHeight && animation == 0) continue;                   //k333123 front tile to object draw!
                    if ((imgSize.Width == CellWidth * 2) && (imgSize.Height == CellHeight * 2) && (animation == 0)) continue;     //k333123 front tile to object draw!

                    if (blend)
                    {
                        /*
                        if ((fileIndex > 99) & (fileIndex < 199))
                            Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - (3 * CellHeight)), Color.White, true);
                        else
                        */
                        //Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - s.Height), Color.White, (index >= 2723 && index <= 2732)); //k333123 maybe apply offset! k333123 now
                        // Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY - s.Height), Color.White, true); //k333123 maybe apply offset!
                        Libraries.MapLibs[fileIndex].DrawBlend(index, new Point(drawX, drawY), Color.White, true); //k333123 maybe apply offset!
                    }
                    else
                        //Libraries.MapLibs[fileIndex].Draw(index, drawX, drawY - s.Height); //k333123 maybe apply offset!
                        //Libraries.MapLibs[fileIndex].Draw(index, new Point(drawX, drawY - s.Height),Color.White,true); //k333123 maybe apply offset! k333123 now
                        Libraries.MapLibs[fileIndex].Draw(index, new Point(drawX, drawY), Color.White, true); //k333123 maybe apply offset! k333123 now
                    #endregion
                }

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawObjects();
                }
            }

            DXManager.Sprite.Flush();
            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(0.4F);

            //MapObject.User.DrawMount();
             
            MapObject.User.DrawBody();

            if ((MapObject.User.Direction == ExineDirection.Up) ||
                (MapObject.User.Direction == ExineDirection.UpLeft) ||
                (MapObject.User.Direction == ExineDirection.UpRight) ||
                (MapObject.User.Direction == ExineDirection.Right) ||
                (MapObject.User.Direction == ExineDirection.Left))
            {
                MapObject.User.DrawHead();
                MapObject.User.DrawWings();
            }
            else
            {
                MapObject.User.DrawWings();
                MapObject.User.DrawHead();
            }

            DXManager.SetOpacity(oldOpacity);

            if (Settings.HighlightTarget)
            {
                if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && MapObject.MouseObject != MapObject.TargetObject && MapObject.MouseObject.Blend)
                    MapObject.MouseObject.DrawBlend();

                if (MapObject.TargetObject != null)
                    MapObject.TargetObject.DrawBlend();
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].DrawEffects(Settings.Effect);

                if (Settings.NameView && !(Objects[i] is ItemObject) && !Objects[i].Dead)
                    Objects[i].DrawName();

                Objects[i].DrawChat(); //add k333123 test OK
                Objects[i].DrawHealth();
                Objects[i].DrawPoison();

                Objects[i].DrawDamages();
            }

            if (Settings.Effect)
            {
                for (int i = Effects.Count - 1; i >= 0; i--)
                {
                    if (Effects[i].DrawBehind) continue;
                    Effects[i].Draw();
                }
            }
        }

        private Color GetBlindLight(Color light)
        {
            if (MapObject.User.BlindTime <= CMain.Time && MapObject.User.BlindCount < 25)
            {
                MapObject.User.BlindTime = CMain.Time + 100;
                MapObject.User.BlindCount++;
            }

            int count = MapObject.User.BlindCount;
            light = Color.FromArgb(255, Math.Max(20, light.R - (count * 10)), Math.Max(20, light.G - (count * 10)), Math.Max(20, light.B - (count * 10)));

            return light;
        }

        private void DrawLights(LightSetting setting)
        {
            if (DXManager.Lights == null || DXManager.Lights.Count == 0) return;

            if (DXManager.LightTexture == null || DXManager.LightTexture.Disposed)
            {
                DXManager.LightTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                DXManager.LightSurface = DXManager.LightTexture.GetSurfaceLevel(0);
            }

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.LightSurface);

            #region Night Lights
            Color darkness;

            switch (setting)
            {
                case LightSetting.Night:
                    {
                        switch (MapDarkLight)
                        {
                            case 1:
                                darkness = Color.FromArgb(255, 20, 20, 20);
                                break;
                            case 2:
                                darkness = Color.LightSlateGray;
                                break;
                            case 3:
                                darkness = Color.SkyBlue;
                                break;
                            case 4:
                                darkness = Color.Goldenrod;
                                break;
                            default:
                                darkness = Color.Black;
                                break;
                        }
                    }
                    break;
                case LightSetting.Evening:
                case LightSetting.Dawn:
                    darkness = Color.FromArgb(255, 50, 50, 50);
                    break;
                default:
                case LightSetting.Day:
                    darkness = Color.FromArgb(255, 255, 255, 255);
                    break;
            }

            if (MapObject.User.Poison.HasFlag(PoisonType.Blindness))
            {
                darkness = GetBlindLight(darkness);
            }

            DXManager.Device.Clear(ClearFlags.Target, darkness, 0, 0);

            #endregion

            int light;
            Point p;
            DXManager.SetBlend(true);
            DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.One);

            #region Object Lights (Player/Mob/NPC)
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                {
                    light = ob.Light;

                    int lightRange = light % 15;
                    if (lightRange >= DXManager.Lights.Count)
                        lightRange = DXManager.Lights.Count - 1;

                    p = ob.DrawLocation;

                    Color lightColour = ob.LightColour;

                    if (ob.Race == ObjectType.Player)
                    {
                        switch (light / 15)
                        {
                            case 0://no light source
                                lightColour = Color.FromArgb(255, 60, 60, 60);
                                break;
                            case 1:
                                lightColour = Color.FromArgb(255, 120, 120, 120);
                                break;
                            case 2://Candle
                                lightColour = Color.FromArgb(255, 180, 180, 180);
                                break;
                            case 3://Torch
                                lightColour = Color.FromArgb(255, 240, 240, 240);
                                break;
                            default://Peddler Torch
                                lightColour = Color.FromArgb(255, 255, 255, 255);
                                break;
                        }
                    }
                    else if (ob.Race == ObjectType.Merchant)
                    {
                        lightColour = Color.FromArgb(255, 120, 120, 120);
                    }

                    if (MapObject.User.Poison.HasFlag(PoisonType.Blindness))
                    {
                        lightColour = GetBlindLight(lightColour);
                    }

                    if (DXManager.Lights[lightRange] != null && !DXManager.Lights[lightRange].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[lightRange].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[lightRange].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[lightRange], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }
                }

                #region Object Effect Lights
                if (!Settings.Effect) continue;
                for (int e = 0; e < ob.Effects.Count; e++)
                {
                    Effect effect = ob.Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start || (!(effect is Missile) && effect.Light < ob.Light)) continue;

                    light = effect.Light;

                    p = effect.DrawLocation;

                    var lightColour = effect.LightColour;

                    if (MapObject.User.Poison.HasFlag(PoisonType.Blindness))
                    {
                        lightColour = GetBlindLight(lightColour);
                    }

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }

                }
                #endregion
            }
            #endregion

            #region Map Effect Lights
            if (Settings.Effect)
            {
                for (int e = 0; e < Effects.Count; e++)
                {
                    Effect effect = Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start) continue;

                    light = effect.Light;
                    if (light == 0) continue;

                    p = effect.DrawLocation;

                    var lightColour = Color.White;

                    if (MapObject.User.Poison.HasFlag(PoisonType.Blindness))
                    {
                        lightColour = GetBlindLight(lightColour);
                    }

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2), -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightColour);
                    }
                }
            }
            #endregion

            #region Map Lights
            for (int y = MapObject.User.Movement.Y - ViewRangeY - 24; y <= MapObject.User.Movement.Y + ViewRangeY + 24; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = MapObject.User.Movement.X - ViewRangeX - 24; x < MapObject.User.Movement.X + ViewRangeX + 24; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int imageIndex = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (M2CellInfo[x, y].Light <= 0 || M2CellInfo[x, y].Light >= 10) continue;
                    if (M2CellInfo[x, y].Light == 0) continue;

                    Color lightIntensity;

                    light = (M2CellInfo[x, y].Light % 10) * 3;

                    switch (M2CellInfo[x, y].Light / 10)
                    {
                        case 1:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                        case 2:
                            lightIntensity = Color.FromArgb(255, 120, 180, 255);
                            break;
                        case 3:
                            lightIntensity = Color.FromArgb(255, 255, 180, 120);
                            break;
                        case 4:
                            lightIntensity = Color.FromArgb(255, 22, 160, 5);
                            break;
                        default:
                            lightIntensity = Color.FromArgb(255, 255, 255, 255);
                            break;
                    }

                    if (MapObject.User.Poison.HasFlag(PoisonType.Blindness))
                    {
                        lightIntensity = GetBlindLight(lightIntensity);
                    }

                    int fileIndex = M2CellInfo[x, y].FrontIndex;

                    p = new Point(
                        (x + OffSetX - MapObject.User.Movement.X) * CellWidth + MapObject.User.OffSetMove.X,
                        (y + OffSetY - MapObject.User.Movement.Y) * CellHeight + MapObject.User.OffSetMove.Y + 24); //k333123 height?
                      //(y + OffSetY - MapObject.User.Movement.Y) * CellHeight + MapObject.User.OffSetMove.Y + 32); //k333123 height?


                    if (M2CellInfo[x, y].FrontAnimationFrame > 0)
                        p.Offset(Libraries.MapLibs[fileIndex].GetOffSet(imageIndex));

                    if (light >= DXManager.Lights.Count)
                        light = DXManager.Lights.Count - 1;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                    {
                        p.Offset(-(DXManager.LightSizes[light].X / 2) - (CellWidth / 2) + 10, -(DXManager.LightSizes[light].Y / 2) - (CellHeight / 2) - 5);
                        DXManager.Draw(DXManager.Lights[light], null, new Vector3((float)p.X, (float)p.Y, 0.0F), lightIntensity);
                    }
                }
            }
            #endregion

            DXManager.SetBlend(false);
            DXManager.SetSurface(oldSurface);

            DXManager.Device.SetRenderState(RenderState.SourceBlend, Blend.Zero);
            DXManager.Device.SetRenderState(RenderState.DestinationBlend, Blend.SourceColor);

            DXManager.Draw(DXManager.LightTexture, new Rectangle(0, 0, Settings.ScreenWidth, Settings.ScreenHeight), Vector3.Zero, Color.White);

            DXManager.Sprite.End();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
        }

        private static void OnMouseClick(object sender, EventArgs e)
        {
            if (!(e is MouseEventArgs me)) return;

            if (AwakeningAction == true) return;
            switch (me.Button)
            {
                case MouseButtons.Left:
                    {
                        AutoRun = false;
                        ExineMainScene.Scene.MapControl.AutoPath = false;
                        if (MapObject.MouseObject == null) return;
                        NPCObject npc = MapObject.MouseObject as NPCObject;
                        if (npc != null)
                        {
                            if (npc.ObjectID == ExineMainScene.NPCID &&
                                (CMain.Time <= ExineMainScene.NPCTime || ExineMainScene.Scene.ExNPCDialog.Visible))
                            {
                                return;
                            }

                            //GameScene.Scene.NPCDialog.Hide();

                            ExineMainScene.NPCTime = CMain.Time + 5000;
                            ExineMainScene.NPCID = npc.ObjectID;
                            Network.SendPacketToServer(new ClientPacket.CallNPC { ObjectID = npc.ObjectID, Key = "[@Main]" });
                        }
                    }
                    break;
                case MouseButtons.Right:
                    {
                        AutoRun = false;
                        if (MapObject.MouseObject == null)
                        {
                            if (Settings.NewMove && MapLocation != MapObject.User.CurrentLocation && ExineMainScene.Scene.MapControl.EmptyCell(MapLocation))
                            {
                                var path = ExineMainScene.Scene.MapControl.PathFinder.FindPath(MapObject.User.CurrentLocation, MapLocation, 20);

                                if (path != null && path.Count > 0)
                                {
                                    ExineMainScene.Scene.MapControl.CurrentPath = path;
                                    ExineMainScene.Scene.MapControl.AutoPath = true;
                                    var offset = MouseLocation.Subtract(ToMouseLocation(MapLocation));
                                    Effects.Add(new Effect(Libraries.Magic3, 500, 10, 600, MapLocation) { DrawOffset = offset.Subtract(8, 15) });
                                }
                            }
                            return;
                        }

                        if (CMain.Ctrl)
                        {
                            PlayerObject player = MapObject.MouseObject as PlayerObject;

                            if (player != null &&
                                player != User &&
                                CMain.Time >= ExineMainScene.InspectTime)
                            {
                                ExineMainScene.InspectTime = CMain.Time + 500;
                                InspectDialog.InspectID = player.ObjectID;
                                Network.SendPacketToServer(new ClientPacket.Inspect { ObjectID = player.ObjectID });
                                return;
                            }
                        }
                    }
                    break;
                case MouseButtons.Middle:
                    AutoRun = !AutoRun;
                    break;
            }
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            MapButtons |= e.Button;
            if (e.Button != MouseButtons.Right || !Settings.NewMove)
                ExineMainScene.CanRun = false;

            if (AwakeningAction == true) return;

            if (e.Button != MouseButtons.Left) return;

            if (ExineMainScene.SelectedCell != null)
            {
                //if (GameScene.SelectedCell.GridType != MirGridType.Inventory)
                //{
                //    GameScene.SelectedCell = null;
                //    return;
                //}

                MirItemCell cell = ExineMainScene.SelectedCell;
                if (cell.Item.Info.Bind.HasFlag(BindMode.DontDrop))
                {
                    ExineMessageBox messageBox = new ExineMessageBox(string.Format("{0}을 드롭할 수 없습니다", cell.Item.FriendlyName), MirMessageBoxButtons.OK);
                    messageBox.Show();
                    ExineMainScene.SelectedCell = null;
                    return;
                }
                if (cell.Item.Count == 1)
                {
                    ExineMessageBox messageBox = new ExineMessageBox(string.Format(GameLanguage.DropTip, cell.Item.FriendlyName), MirMessageBoxButtons.YesNo);

                    messageBox.YesButton.Click += (o, a) =>
                    {
                        Network.SendPacketToServer(new ClientPacket.DropItem
                        {
                            UniqueID = cell.Item.UniqueID,
                            Count = 1,
                            HeroInventory = false
                        });

                        cell.Locked = true;
                    };
                    messageBox.Show();
                }
                else
                {
                    MirAmountBox amountBox = new MirAmountBox(GameLanguage.DropAmount, cell.Item.Info.Image, cell.Item.Count);

                    amountBox.OKButton.Click += (o, a) =>
                    {
                        if (amountBox.Amount <= 0) return;
                        Network.SendPacketToServer(new ClientPacket.DropItem
                        {
                            UniqueID = cell.Item.UniqueID,
                            Count = (ushort)amountBox.Amount,
                            HeroInventory = false
                        });

                        cell.Locked = true;
                    };

                    amountBox.Show();
                }
                ExineMainScene.SelectedCell = null;

                return;
            }

            if (ExineMainScene.PickedUpGold)
            {
                MirAmountBox amountBox = new MirAmountBox(GameLanguage.DropAmount, 116, ExineMainScene.Gold);

                amountBox.OKButton.Click += (o, a) =>
                {
                    if (amountBox.Amount > 0)
                    {
                        Network.SendPacketToServer(new ClientPacket.DropGold { Amount = amountBox.Amount });
                    }
                };

                amountBox.Show();
                ExineMainScene.PickedUpGold = false;
            }

            if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && !(MapObject.MouseObject is ItemObject) &&
                !(MapObject.MouseObject is NPCObject) && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 64)
                 && !(MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70))
            {
                MapObject.TargetObjectID = MapObject.MouseObject.ObjectID;
                if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI != 6)
                    MapObject.MagicObjectID = MapObject.TargetObject.ObjectID;
            }
            else
                MapObject.TargetObjectID = 0;
        }

        int test = 0;//k333123

        private void CheckInput()
        {
            if (AwakeningAction == true) return;

            if ((MouseControl == this) && (MapButtons != MouseButtons.None)) AutoHit = false;//mouse actions stop mining even when frozen!
            if (!CanRideAttack()) AutoHit = false;

            if (CMain.Time < InputDelay || User.Poison.HasFlag(PoisonType.Paralysis) || User.Poison.HasFlag(PoisonType.LRParalysis) || User.Poison.HasFlag(PoisonType.Frozen) || User.Fishing) return;

            if (User.NextMagic != null )
            {
                UseMagic(User.NextMagic, User);
                return;
            }

            if (CMain.Time < User.BlizzardStopTime || CMain.Time < User.ReincarnationStopTime) return;

            if (MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (((MapObject.TargetObject.Name.EndsWith(")") || MapObject.TargetObject is PlayerObject) && CMain.Shift) ||
                    (!MapObject.TargetObject.Name.EndsWith(")") && MapObject.TargetObject is MonsterObject))
                {
                    ExineMainScene.LogTime = CMain.Time + Globals.LogDelay;

                    if (User.Class == ExineClass.Archer && User.HasClassWeapon  && !User.Fishing)//ArcherTest - non aggressive targets (player / pets)
                    {
                        if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                        {
                            if (CMain.Time > ExineMainScene.AttackTime)
                            {
                                User.QueuedAction = new QueuedAction { Action = ExAction.AttackRange1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation, Params = new List<object>() };
                                User.QueuedAction.Params.Add(MapObject.TargetObject != null ? MapObject.TargetObject.ObjectID : (uint)0);
                                User.QueuedAction.Params.Add(MapObject.TargetObject.CurrentLocation);

                                // MapObject.TargetObject = null; //stop constant attack when close up
                            }
                        }
                        else
                        {
                            if (CMain.Time >= OutputDelay)
                            {
                                OutputDelay = CMain.Time + 1000;
                                ExineMainScene.Scene.OutputMessage("Target is too far.");
                            }
                        }
                        //  return;
                    }

                    else if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1))
                    {
                        if (CMain.Time > ExineMainScene.AttackTime && CanRideAttack() && !User.Poison.HasFlag(PoisonType.Dazed))
                        {
                            //int weaponType = Libraries.weaponMapperMgr.GetShapeToLibIndexFromShapeIdx(User.Weapon, User.Gender != ExineGender.Male).weaponType;
                            int weaponType = -1;
                            var shapeToLibIndex = Libraries.weaponMapperMgr.GetShapeToLibIndexFromShapeIdx(User.Weapon, User.Gender != ExineGender.Male);
                            if(shapeToLibIndex!=null) weaponType = shapeToLibIndex.weaponType; 

                            switch (weaponType)
                            {
                                case 1:
                                    User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_ATTACK1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                                    break;
                                case 2:
                                    User.QueuedAction = new QueuedAction { Action = ExAction.TWOHAND_ATTACK1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                                    break;
                                case 3:
                                    User.QueuedAction = new QueuedAction { Action = ExAction.TWOHAND_ATTACK1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                                    break;
                                default:
                                    User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_ATTACK1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                                    break;

                            }
                            //User.QueuedAction = new QueuedAction { Action = ExAction.Attack1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                            return;
                        }
                    }
                }
            }
            if (AutoHit )
            {
                if (CMain.Time > ExineMainScene.AttackTime)
                {
                    User.QueuedAction = new QueuedAction { Action = ExAction.Mine, Direction = User.Direction, Location = User.CurrentLocation };
                    return;
                }
            }



           

            ExineDirection direction;
            if (MouseControl == this)
            {
                direction = MouseDirection();
                if (AutoRun)
                {
                    if (ExineMainScene.CanRun && CanRun(direction) && CMain.Time > ExineMainScene.NextRunTime && User.HP >= 10 && (!User.Sneaking || (User.Sneaking && User.Sprint))) //slow remove
                    {
                        int distance =User.Sprint && !User.Sneaking ? 3 : 2;
                        bool fail = false;
                        for (int i = 1; i <= distance; i++)
                        {
                            if (!CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, i)))
                                fail = true;
                        }
                        if (!fail)
                        {
                            User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_RUN_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, distance) };
                            return;
                        }
                    }
                    if ((CanWalk(direction, out direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                    {
                        Console.WriteLine("@111");
                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                        return;
                    }
                    if (direction != User.Direction)
                    {
                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = direction, Location = User.CurrentLocation };
                        return;
                    }
                    return;
                }

                switch (MapButtons)
                {
                    case MouseButtons.Left:
                        if (MapObject.MouseObject is NPCObject || (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User)) break;
                        if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI == 70) break;

                        if (CMain.Alt)
                        {
                            User.QueuedAction = new QueuedAction { Action = ExAction.Harvest, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }

                        if (CMain.Shift)
                        {
                            if (CMain.Time > ExineMainScene.AttackTime && CanRideAttack()) //ArcherTest - shift click
                            {
                                MapObject target = null;
                                if (MapObject.MouseObject is MonsterObject || MapObject.MouseObject is PlayerObject) target = MapObject.MouseObject;

                                if (User.Class == ExineClass.Archer && User.HasClassWeapon  && !User.Poison.HasFlag(PoisonType.Dazed))
                                {
                                    if (target != null)
                                    {
                                        if (!Functions.InRange(MapObject.MouseObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                                        {
                                            if (CMain.Time >= OutputDelay)
                                            {
                                                OutputDelay = CMain.Time + 1000;
                                                ExineMainScene.Scene.OutputMessage("Target is too far.");
                                            }
                                            return;
                                        }
                                    }

                                    User.QueuedAction = new QueuedAction { Action = ExAction.AttackRange1, Direction = MouseDirection(), Location = User.CurrentLocation, Params = new List<object>() };
                                    User.QueuedAction.Params.Add(target != null ? target.ObjectID : (uint)0);
                                    User.QueuedAction.Params.Add(Functions.PointMove(User.CurrentLocation, MouseDirection(), 9));
                                    return;
                                }

                                //stops double slash from being used without empty hand or assassin weapon (otherwise bugs on second swing)
                                if (ExineMainScene.User.DoubleSlash && (!User.HasClassWeapon && User.Weapon > -1)) return;
                                if (User.Poison.HasFlag(PoisonType.Dazed)) return;

                                //int weaponType = Libraries.weaponMapperMgr.GetShapeToLibIndexFromShapeIdx(User.Weapon, User.Gender != ExineGender.Male).weaponType;
                                int weaponType = -1;
                                var shapeToLibIndex = Libraries.weaponMapperMgr.GetShapeToLibIndexFromShapeIdx(User.Weapon, User.Gender != ExineGender.Male);
                                if (shapeToLibIndex != null) weaponType = shapeToLibIndex.weaponType;
                                switch (weaponType)
                                {
                                    case 1:
                                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_ATTACK1, Direction = direction, Location = User.CurrentLocation };
                                        break;
                                    case 2:
                                        User.QueuedAction = new QueuedAction { Action = ExAction.TWOHAND_ATTACK1, Direction = direction, Location = User.CurrentLocation };
                                        break;
                                    case 3:
                                        User.QueuedAction = new QueuedAction { Action = ExAction.BOWHAND_ATTACK1, Direction = direction, Location = User.CurrentLocation };
                                        break;
                                    default:
                                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_ATTACK1, Direction = direction, Location = User.CurrentLocation };
                                        break;
                                }
                                //User.QueuedAction = new QueuedAction { Action = ExAction.Attack1, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        if (MapObject.MouseObject is MonsterObject && User.Class == ExineClass.Archer && MapObject.TargetObject != null && !MapObject.TargetObject.Dead && User.HasClassWeapon ) //ArcherTest - range attack
                        {
                            if (Functions.InRange(MapObject.MouseObject.CurrentLocation, User.CurrentLocation, Globals.MaxAttackRange))
                            {
                                if (CMain.Time > ExineMainScene.AttackTime)
                                {
                                    User.QueuedAction = new QueuedAction { Action = ExAction.AttackRange1, Direction = direction, Location = User.CurrentLocation, Params = new List<object>() };
                                    User.QueuedAction.Params.Add(MapObject.TargetObject.ObjectID);
                                    User.QueuedAction.Params.Add(MapObject.TargetObject.CurrentLocation);
                                }
                            }
                            else
                            {
                                if (CMain.Time >= OutputDelay)
                                {
                                    OutputDelay = CMain.Time + 1000;
                                    ExineMainScene.Scene.OutputMessage("Target is too far.");
                                }
                            }
                            return;
                        }

                        if (MapLocation == User.CurrentLocation)
                        {
                            if (CMain.Time > ExineMainScene.PickUpTime)
                            {
                                ExineMainScene.PickUpTime = CMain.Time + 200;
                                Network.SendPacketToServer(new ClientPacket.PickUp());
                            }
                            return;
                        }

                        //mine
                        if (!ValidPoint(Functions.PointMove(User.CurrentLocation, direction, 1)))
                        {
                            if ((MapObject.User.Equipment[(int)EquipmentSlot.Weapon] != null) && (MapObject.User.Equipment[(int)EquipmentSlot.Weapon].Info.CanMine))
                            {
                                if (direction != User.Direction)
                                {
                                    User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = direction, Location = User.CurrentLocation };
                                    return;
                                }
                                AutoHit = true;
                                return;
                            }
                        }
                        if ((CanWalk(direction, out direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                        {

                           // test++;

                            //Console.WriteLine("@222");
                           // if (test % 2 == 0)
                           //{
                               // Console.WriteLine("@222 ONEHAND_WALK_LEFT");
                                User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };

                              //  test = 0;
                            //}
                            //else
                           // {
                            //    Console.WriteLine("@222 ONEHAND_WALK_RIGHT");
                           //     User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_RIGHT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };

                            //}
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }

                        if (CanFish(direction))
                        {
                            User.FishingTime = CMain.Time;
                            Network.SendPacketToServer(new ClientPacket.FishingCast { Sitdown = true });
                            return;
                        }

                        break;
                    case MouseButtons.Right:
                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User && CMain.Ctrl) break;

                        //peacemode rest add!
                        if (MapObject.User.ExinePeaceMode && (CMain.Time > ExineMainScene.NextRunTime))
                        {
                            //Console.WriteLine("PeaceMode and right click -> Goto Toggle Rest Mode!");
                            Console.WriteLine("@@@Goto Toggle Rest Mode! ExineRestMode:" + MapObject.User.ExineRestMode);
                            MapObject.User.ExineRestMode = !MapObject.User.ExineRestMode;
                            ExineMainScene.NextRunTime = CMain.Time + 250; //add 
                            //break;
                            return;
                        } 

                        if (Settings.NewMove) break;

                        if (Functions.InRange(MapLocation, User.CurrentLocation, 2))
                        {
                            if (direction != User.Direction)
                            {
                                User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }

                        ExineMainScene.CanRun = User.FastRun ? true : ExineMainScene.CanRun;

                        if (ExineMainScene.CanRun && CanRun(direction) && CMain.Time > ExineMainScene.NextRunTime && User.HP >= 10 && (!User.Sneaking || (User.Sneaking && User.Sprint))) //slow removed
                        {
                            int distance = User.Sprint && !User.Sneaking ? 3 : 2;
                            bool fail = false;
                            for (int i = 0; i <= distance; i++)
                            {
                                if (!CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, i)))
                                    fail = true;
                            }
                            if (!fail)
                            {
                                User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_RUN_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction,  (User.Sprint && !User.Sneaking) ? 3 : 2) };
                                return;
                            }
                        }
                        if ((CanWalk(direction, out direction)) && (CheckDoorOpen(Functions.PointMove(User.CurrentLocation, direction, 1))))
                        {
                           // Console.WriteLine("@333");
                            test++;

                            //Console.WriteLine("@222");
                            if (test % 2 == 0)
                            {
                                //Console.WriteLine("@333 ONEHAND_WALK_LEFT");
                                User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                                test = 0;
                            }
                            else
                            {
                                //Console.WriteLine("@333 ONEHAND_WALK_RIGHT");
                                User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_RIGHT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            }
                            //User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_STAND, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        break;
                }
            }

            if (AutoPath)
            {
                if (CurrentPath == null || CurrentPath.Count == 0)
                {
                    AutoPath = false;
                    return;
                }

                var path = ExineMainScene.Scene.MapControl.PathFinder.FindPath(MapObject.User.CurrentLocation, CurrentPath.Last().Location);

                if (path != null && path.Count > 0)
                    ExineMainScene.Scene.MapControl.CurrentPath = path;
                else
                {
                    AutoPath = false;
                    return;
                }

                Node currentNode = CurrentPath.SingleOrDefault(x => User.CurrentLocation == x.Location);
                if (currentNode != null)
                {
                    while (true)
                    {
                        Node first = CurrentPath.First();
                        CurrentPath.Remove(first);

                        if (first == currentNode)
                            break;
                    }
                }

                if (CurrentPath.Count > 0)
                {
                    ExineDirection dir = Functions.DirectionFromPoint(User.CurrentLocation, CurrentPath.First().Location);

                    if (ExineMainScene.CanRun && CanRun(dir) && CMain.Time > ExineMainScene.NextRunTime && User.HP >= 10 && CurrentPath.Count > 1)
                    {
                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_RUN_LEFT, Direction = dir, Location = Functions.PointMove(User.CurrentLocation, dir,  2 )};
                        return;
                    }
                    if (CanWalk(dir))
                    {
                        Console.WriteLine("@444");
                        User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = dir, Location = Functions.PointMove(User.CurrentLocation, dir, 1) };

                        return;
                    }
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if (((!MapObject.TargetObject.Name.EndsWith(")") && !(MapObject.TargetObject is PlayerObject)) || !CMain.Shift) &&
                (MapObject.TargetObject.Name.EndsWith(")") || !(MapObject.TargetObject is MonsterObject))) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1)) return;
            if (User.Class == ExineClass.Archer && User.HasClassWeapon && (MapObject.TargetObject is MonsterObject || MapObject.TargetObject is PlayerObject)) return; //ArcherTest - stop walking
            direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanWalk(direction, out direction)) return;

            Console.WriteLine("@555");
            User.QueuedAction = new QueuedAction { Action = ExAction.ONEHAND_WALK_LEFT, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
        }

        public void UseMagic(ClientMagic magic, UserObject actor)
        {
            if (CMain.Time < ExineMainScene.SpellTime || actor.Poison.HasFlag(PoisonType.Stun))
            {
                actor.ClearMagic();
                return;
            }

            if ((CMain.Time <= magic.CastTime + magic.Delay))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    ExineMainScene.Scene.OutputMessage(string.Format("You cannot cast {0} for another {1} seconds.", magic.Spell.ToString(), ((magic.CastTime + magic.Delay) - CMain.Time - 1) / 1000 + 1));
                }

                actor.ClearMagic();
                return;
            }

            int cost = magic.Level * magic.LevelCost + magic.BaseCost;

            if (magic.Spell == Spell.Teleport || magic.Spell == Spell.Blink || magic.Spell == Spell.StormEscape)
            {
                if (actor.Stats[Stat.TeleportManaPenaltyPercent] > 0)
                {
                    cost += (cost * actor.Stats[Stat.TeleportManaPenaltyPercent]) / 100;
                }
            }

            if (actor.Stats[Stat.ManaPenaltyPercent] > 0)
            {
                cost += (cost * actor.Stats[Stat.ManaPenaltyPercent]) / 100;
            }

            if (cost > actor.MP)
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    ExineMainScene.Scene.OutputMessage(GameLanguage.LowMana);
                }
                actor.ClearMagic();
                return;
            }

            //bool isTargetSpell = true;

            MapObject target = null;

            //Targeting
            switch (magic.Spell)
            {
                case Spell.FireBall:
                case Spell.GreatFireBall:
                case Spell.ElectricShock:
                case Spell.Poisoning:
                case Spell.ThunderBolt:
                case Spell.FlameDisruptor:
                case Spell.SoulFireBall:
                case Spell.TurnUndead:
                case Spell.FrostCrunch:
                case Spell.Vampirism:
                case Spell.Revelation:
                case Spell.Entrapment:
                case Spell.Hallucination: 
                case Spell.FireBounce:
                case Spell.MeteorShower:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObjectID = target.ObjectID;
                    break;
                case Spell.StraightShot:
                case Spell.DoubleShot:
                case Spell.ElementalShot:
                case Spell.DelayedExplosion:
                case Spell.BindingShot:
                case Spell.VampireShot:
                case Spell.PoisonShot:
                case Spell.CrippleShot:
                case Spell.NapalmShot:
               
                    if (!actor.HasClassWeapon)
                    {
                        ExineMainScene.Scene.OutputMessage("You must be wearing a bow to perform this skill.");
                        actor.ClearMagic();
                        return;
                    }
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObjectID = target.ObjectID;
                    break;
                 
                case Spell.Purification:
                case Spell.Healing:
                case Spell.UltimateEnhancer:
                case Spell.EnergyShield:
                case Spell.PetEnhancer:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }

                    if (target == null) target = User;
                    break;
                case Spell.FireBang:
                case Spell.MassHiding:
                case Spell.FireWall:
                case Spell.TrapHexagon:
                case Spell.HealingCircle:
                case Spell.CatTongue:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }
                    break;
                case Spell.PoisonCloud:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }
                    break;
                case Spell.Blizzard:
                case Spell.MeteorStrike:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && actor.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }
                    break;
                case Spell.Reincarnation:
                    
                    if (actor.NextMagicObject != null)
                    {
                        if (actor.NextMagicObject.Dead && actor.NextMagicObject.Race == ObjectType.Player)
                            target = actor.NextMagicObject;
                    }
                    break;
                case Spell.Trap:
                    if (actor.NextMagicObject != null)
                    {
                        if (!actor.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && actor.NextMagicObject.Race != ObjectType.Merchant)
                            target = actor.NextMagicObject;
                    }
                    break;
                case Spell.FlashDash:
                    if (actor.GetMagic(Spell.FlashDash).Level <= 1 && actor.IsDashAttack() == false)
                    {
                        actor.ClearMagic();
                        return;
                    }
                    //isTargetSpell = false;
                    break;
                default:
                    //isTargetSpell = false;
                    break;
            }

            ExineDirection dir = (target == null || target == User) ? actor.NextMagicDirection : Functions.DirectionFromPoint(actor.CurrentLocation, target.CurrentLocation);

            Point location = target != null ? target.CurrentLocation : actor.NextMagicLocation;

            uint targetID = target != null ? target.ObjectID : 0;

            if (magic.Spell == Spell.FlashDash)
                dir = actor.Direction;

            if ((magic.Range != 0) && (!Functions.InRange(actor.CurrentLocation, location, magic.Range)))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    ExineMainScene.Scene.OutputMessage("Target is too far.");
                }
                actor.ClearMagic();
                return;
            }

            ExineMainScene.LogTime = CMain.Time + Globals.LogDelay;

            if (actor == User)
            {
                //User.QueuedAction = new QueuedAction { Action = ExAction.MAGIC_CAST, Direction = dir, Location = User.CurrentLocation, Params = new List<object>() };
                User.QueuedAction = new QueuedAction { Action = ExAction.Spell, Direction = dir, Location = User.CurrentLocation, Params = new List<object>() };
                User.QueuedAction.Params.Add(magic.Spell);
                User.QueuedAction.Params.Add(targetID);
                User.QueuedAction.Params.Add(location);
                User.QueuedAction.Params.Add(magic.Level);
            }
            else
            {
                Network.SendPacketToServer(new ClientPacket.Magic { ObjectID = actor.ObjectID, Spell = magic.Spell, Direction = dir, TargetID = targetID, Location = location, SpellTargetLock = CMain.SpellTargetLock });
            }
        }

        public static ExineDirection MouseDirection(float ratio = 45F) //22.5 = 16
        {
            Point p = new Point(MouseLocation.X / CellWidth, MouseLocation.Y / CellHeight);
            if (Functions.InRange(new Point(OffSetX, OffSetY), p, 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), p);

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += ratio / 2;
            if (angle > 360) angle -= 360;

            return (ExineDirection)(angle / ratio);
        }

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new PointF(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static double Distance(PointF p1, PointF p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public bool EmptyCell(Point p)
        {
            if ((M2CellInfo[p.X, p.Y].BackImage & 0x20000000) != 0 || (M2CellInfo[p.X, p.Y].FrontImage & 0x8000) != 0) // + (M2CellInfo[P.X, P.Y].FrontImage & 0x7FFF) != 0)
                return false;

            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return false;
            }

            return true;
        }

        private bool CanWalk(ExineDirection dir)
        {
            return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)) && !User.InTrapRock;
        }

        private bool CanWalk(ExineDirection dir, out ExineDirection outDir)
        {
            outDir = dir;
            if (User.InTrapRock) return false;

            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
                return true;

            dir = Functions.NextDir(outDir);
            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
            {
                outDir = dir;
                return true;
            }

            dir = Functions.PreviousDir(outDir);
            if (EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)))
            {
                outDir = dir;
                return true;
            }

            return false;
        }

        private bool CheckDoorOpen(Point p)
        {
            if (M2CellInfo[p.X, p.Y].DoorIndex == 0) return true;
            Door DoorInfo = GetDoor(M2CellInfo[p.X, p.Y].DoorIndex);
            if (DoorInfo == null) return false;//if the door doesnt exist then it isnt even being shown on screen (and cant be open lol)
            if ((DoorInfo.DoorState == DoorState.Closed) || (DoorInfo.DoorState == DoorState.Closing))
            {
                if (CMain.Time > _doorTime)
                {
                    _doorTime = CMain.Time + 4000;
                    Network.SendPacketToServer(new ClientPacket.Opendoor() { DoorIndex = DoorInfo.index });
                }

                return false;
            }
            if ((DoorInfo.DoorState == DoorState.Open) && (DoorInfo.LastTick + 4000 > CMain.Time))
            {
                if (CMain.Time > _doorTime)
                {
                    _doorTime = CMain.Time + 4000;
                    Network.SendPacketToServer(new ClientPacket.Opendoor() { DoorIndex = DoorInfo.index });
                }
            }
            return true;
        }

        private long _doorTime = 0;


        private bool CanRun(ExineDirection dir)
        {
            if (User.InTrapRock) return false;
            if (User.CurrentBagWeight > User.Stats[Stat.BagWeight]) return false;
            if (User.CurrentWearWeight > User.Stats[Stat.BagWeight]) return false;
            if (CanWalk(dir) && EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 2)))
            {
                if ( User.Sprint && !User.Sneaking)
                {
                    return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 3));
                }

                return true;
            }

            return false;
        }

        private bool CanRideAttack()
        {
           
            return true;
        }

        public bool CanFish(ExineDirection dir)
        {
            if (!ExineMainScene.User.HasFishingRod || ExineMainScene.User.FishingTime + 1000 > CMain.Time) return false;
            if (ExineMainScene.User.CurrentAction != ExAction.ONEHAND_STAND) return false;
            if (ExineMainScene.User.Direction != dir) return false;
            if (ExineMainScene.User.TransformType >= 6 && ExineMainScene.User.TransformType <= 9) return false;

            Point point = Functions.PointMove(User.CurrentLocation, dir, 3);

            if (!M2CellInfo[point.X, point.Y].FishingCell) return false;

            return true;
        }

        public bool CanFly(Point target)
        {
            Point location = User.CurrentLocation;
            while (location != target)
            {
                ExineDirection dir = Functions.DirectionFromPoint(location, target);

                location = Functions.PointMove(location, dir, 1);

                if (location.X < 0 || location.Y < 0 || location.X >= ExineMainScene.Scene.MapControl.Width || location.Y >= ExineMainScene.Scene.MapControl.Height) return false;

                if (!ExineMainScene.Scene.MapControl.ValidPoint(location)) return false;
            }

            return true;
        }


        public bool ValidPoint(Point p)
        {
            //GameScene.Scene.ChatDialog.ReceiveChat(string.Format("cell: {0}", (M2CellInfo[p.X, p.Y].BackImage & 0x20000000)), ChatType.Hint);
            return (M2CellInfo[p.X, p.Y].BackImage & 0x20000000) == 0;
        }
        public bool HasTarget(Point p)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return true;
            }
            return false;
        }
        public bool CanHalfMoon(Point p, ExineDirection d)
        {
            d = Functions.PreviousDir(d);
            for (int i = 0; i < 4; i++)
            {
                if (HasTarget(Functions.PointMove(p, d, 1))) return true;
                d = Functions.NextDir(d);
            }
            return false;
        }
        public bool CanCrossHalfMoon(Point p)
        {
            ExineDirection dir = ExineDirection.Up;
            for (int i = 0; i < 8; i++)
            {
                if (HasTarget(Functions.PointMove(p, dir, 1))) return true;
                dir = Functions.NextDir(dir);
            }
            return false;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Objects.Clear();

                MapButtons = 0;
                MouseLocation = Point.Empty;
                InputDelay = 0;
                NextAction = 0;

                M2CellInfo = null;
                Width = 0;
                Height = 0;

                Index = 0;
                FileName = String.Empty;
                Title = String.Empty;
                MiniMap = 0;
                BigMap = 0;
                Lights = 0;
                FloorValid = false;
                LightsValid = false;
                MapDarkLight = 0;
                Music = 0;

                AnimationCount = 0;
                Effects.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion

        public void RemoveObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].RemoveObject(ob);
        }
        public void AddObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].AddObject(ob);
        }
        public MapObject FindObject(uint ObjectID, int x, int y)
        {
            return M2CellInfo[x, y].FindObject(ObjectID);
        }
        public void SortObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].Sort();
        }

        public Door GetDoor(byte Index)
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                if (Doors[i].index == Index)
                    return Doors[i];
            }
            return null;
        }
        public void Processdoors()
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                if ((Doors[i].DoorState == DoorState.Opening) || (Doors[i].DoorState == DoorState.Closing))
                {
                    if (Doors[i].LastTick + 50 < CMain.Time)
                    {
                        Doors[i].LastTick = CMain.Time;
                        Doors[i].ImageIndex++;

                        if (Doors[i].ImageIndex == 1)//change the 1 if you want to animate doors opening/closing
                        {
                            Doors[i].ImageIndex = 0;
                            Doors[i].DoorState = (DoorState)Enum.ToObject(typeof(DoorState), ((byte)++Doors[i].DoorState % 4));
                        }

                        FloorValid = false;
                    }
                }
                if (Doors[i].DoorState == DoorState.Open)
                {
                    if (Doors[i].LastTick + 5000 < CMain.Time)
                    {
                        Doors[i].LastTick = CMain.Time;
                        Doors[i].DoorState = DoorState.Closing;
                        FloorValid = false;
                    }
                }
            }
        }
        public void OpenDoor(byte Index, bool closed)
        {
            Door Info = GetDoor(Index);
            if (Info == null) return;
            Info.DoorState = (closed ? DoorState.Closing : Info.DoorState == DoorState.Open ? DoorState.Open : DoorState.Opening);
            Info.ImageIndex = 0;
            Info.LastTick = CMain.Time;
        }
    }
}


﻿using Exine.ExineControls;
using Exine.ExineGraphics;
using Exine.ExineScenes;
using Exine.ExineSounds;
//using Exine.ExineScenes.Dialogs;
using Exine.ExineScenes.ExDialogs;
using SlimDX;
using NAudio.Gui;
using System.Xml;

namespace Exine.ExineObjects
{
    public abstract class MapObject
    {
        public static Font ChatFont = new Font(Settings.FontName, 10F);
        public static List<ExineLabel> LabelList = new List<ExineLabel>();

        public static UserObject User;
        public static MapObject MouseObject, TargetObject, MagicObject;

        private static uint mouseObjectID;
        public static uint MouseObjectID
        {
            get { return mouseObjectID; }
            set
            {
                if (mouseObjectID == value) return;
                mouseObjectID = value;
                MouseObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        private static uint lastTargetObjectId;
        private static uint targetObjectID;
        public static uint TargetObjectID
        {
            get { return targetObjectID; }
            set
            {
                if (targetObjectID == value) return;
                lastTargetObjectId = targetObjectID;
                targetObjectID = value;
                TargetObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        private static uint magicObjectID;
        public static uint MagicObjectID
        {
            get { return magicObjectID; }
            set
            {
                if (magicObjectID == value) return;
                magicObjectID = value;
                MagicObject = MapControl.Objects.Find(x => x.ObjectID == value);
            }
        }

        public abstract ObjectType Race { get; }
        public abstract bool Blocking { get; }

        public uint ObjectID;
        public string Name = string.Empty;


        public Point CurrentLocation, MapLocation;
        public ExineDirection Direction;
        public bool Dead, Hidden, SitDown, Sneaking;
        public PoisonType Poison;
        public long DeadTime;
        public byte AI;
        public Color TintColor;

        public bool InTrapRock;
        public int JumpDistance;

        public bool Blend = true;

        public long BlindTime;
        public byte BlindCount;

        private byte percentHealth;
        public virtual byte PercentHealth
        {
            get { return percentHealth; }
            set
            {
                if (percentHealth == value) return;

                percentHealth = value;
            }
        }
        public long HealthTime;

        private byte percentMana;
        public virtual byte PercentMana
        {
            get { return percentMana; }
            set
            {
                if (percentMana == value) return;

                percentMana = value;
            }
        }

        public uint LastTargetObjectId => lastTargetObjectId;

        public List<QueuedAction> ActionFeed = new List<QueuedAction>();
        public QueuedAction NextAction
        {
            get { return ActionFeed.Count > 0 ? ActionFeed[0] : null; }
        }

        public List<Effect> Effects = new List<Effect>();
        public List<BuffType> Buffs = new List<BuffType>();

        public MLibrary BodyLibrary;
        public Color DrawColour = Color.White, NameColour = Color.White, LightColour = Color.White;
        public ExineLabel NameLabel, ChatLabel, GuildLabel;
        public ExineImageControl ChatLabelBackImage, ChatLabelRightDownBackImage, ChatMiniPortrait; //add k333123 


        public long ChatTime;
        public int DrawFrame, DrawWingFrame;
        public Point DrawLocation, Movement, FinalDrawLocation, OffSetMove;
        public Rectangle DisplayRectangle;
        public int Light, DrawY;
        public long NextMotion, NextMotion2;
        public ExAction CurrentAction;
        public byte CurrentActionLevel;
        public bool SkipFrames;
        public FrameLoop FrameLoop = null;

        //Sound
        public int StruckWeapon;

        public ExineLabel TempLabel;

        public static List<ExineLabel> DamageLabelList = new List<ExineLabel>();
        public List<Damage> Damages = new List<Damage>();

        protected Point GlobalDisplayLocationOffset
        {
            get { return new Point(0, 0); }
        }

        protected MapObject() { }

        protected MapObject(uint objectID)
        {
            ObjectID = objectID;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != ObjectID) continue;
                ob.Remove();
            }

            MapControl.Objects.Add(this);

            RestoreTargetStates();
        }
        public void Remove()
        {
            if (MouseObject == this) MouseObjectID = 0;
            if (TargetObject == this) TargetObjectID = 0;
            if (MagicObject == this) MagicObjectID = 0;

            if (this == User.NextMagicObject)
                User.ClearMagic();

            MapControl.Objects.Remove(this);
            ExineMainScene.Scene.MapControl.RemoveObject(this);

            if (ObjectID != ExineMainScene.NPCID) return;

            ExineMainScene.NPCID = 0;
            ExineMainScene.Scene.ExNPCDialog.Hide();
        }

        public abstract void Process();
        public abstract void Draw();
        public abstract bool MouseOver(Point p);

        private void RestoreTargetStates()
        {
            if (MouseObjectID == ObjectID)
                MouseObject = this;

            if (TargetObjectID == ObjectID)
                TargetObject = this;

            if (MagicObjectID == ObjectID)
                MagicObject = this;

            if (!this.Dead &&
                TargetObject == null &&
                LastTargetObjectId == ObjectID)
            {
                switch (Race)
                {
                    case ObjectType.Player:
                    case ObjectType.Monster:
                        TargetObject = this;
                        break;
                }
            }
        }

        public void AddBuffEffect(BuffType type)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                if (!(Effects[i] is BuffEffect)) continue;
                if (((BuffEffect)(Effects[i])).BuffType == type) return;
            }

            PlayerObject ob = null;

            if (Race == ObjectType.Player)
            {
                ob = (PlayerObject)this;
            }

            switch (type)
            {
                case BuffType.Fury:
                    Effects.Add(new BuffEffect(Libraries.Magic3, 190, 7, 1400, this, true, type) { Repeat = true });
                    break;
                case BuffType.ImmortalSkin:
                    Effects.Add(new BuffEffect(Libraries.Magic3, 570, 5, 1400, this, true, type) { Repeat = true });
                    break;
                case BuffType.SwiftFeet:
                    if (ob != null) ob.Sprint = true;
                    break;
                case BuffType.MoonLight:
                case BuffType.DarkBody:
                    if (ob != null) ob.Sneaking = true;
                    break;
                case BuffType.VampireShot:
                    Effects.Add(new BuffEffect(Libraries.Magic3, 2110, 6, 1400, this, true, type) { Repeat = false });
                    break;
                case BuffType.PoisonShot:
                    Effects.Add(new BuffEffect(Libraries.Magic3, 2310, 7, 1400, this, true, type) { Repeat = false });
                    break;
                case BuffType.EnergyShield:
                    BuffEffect effect;

                    Effects.Add(effect = new BuffEffect(Libraries.Magic2, 1880, 9, 900, this, true, type) { Repeat = false });
                    SoundManager.PlaySound(20000 + (ushort)Spell.EnergyShield * 10 + 0);

                    effect.Complete += (o, e) =>
                    {
                        Effects.Add(new BuffEffect(Libraries.Magic2, 1900, 2, 800, this, true, type) { Repeat = true });
                    };
                    break;
                case BuffType.MagicBooster:
					Effects.Add(new BuffEffect(Libraries.Magic3, 90, 6, 1200, this, true, type) { Repeat = true });
                    break;
                case BuffType.PetEnhancer:
                    Effects.Add(new BuffEffect(Libraries.Magic3, 230, 6, 1200, this, true, type) { Repeat = true });
                    break;
				case BuffType.GameMaster:
					Effects.Add(new BuffEffect(Libraries.CHumEffect[5], 0, 1, 1200, this, true, type) { Repeat = true });
					break;
                case BuffType.GeneralMeowMeowShield:
                    Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.GeneralMeowMeow], 529, 7, 700, this, true, type) { Repeat = true, Light = 1 });
                    ExineSounds.SoundManager.PlaySound(8322);
                    break;
                case BuffType.PowerBeadBuff:
                    Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.PowerUpBead], 64, 6, 600, this, true, type) { Blend = true, Repeat = true });
                    break;
                case BuffType.HornedArcherBuff:
                    Effects.Add(effect = new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedArcher], 468, 6, 600, this, true, type) { Repeat = false });
                    effect.Complete += (o, e) =>
                    {
                        Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedArcher], 474, 3, 1000, this, true, type) { Blend = true, Repeat = true });
                    };
                    break;
                case BuffType.ColdArcherBuff:
                    Effects.Add(effect = new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedArcher], 477, 7, 700, this, true, type) { Repeat = false });
                    effect.Complete += (o, e) =>
                    {
                        Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedArcher], 484, 3, 1000, this, true, type) { Blend = true, Repeat = true });
                    };
                    break;
                case BuffType.HornedWarriorShield:
                    Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedWarrior], 912, 18, 1800, this, true, type) { Repeat = true });
                    break;
                case BuffType.HornedCommanderShield:
                    Effects.Add(effect = new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedCommander], 1173, 1, 100, this, true, type) { Repeat = false, Light = 1 });
                    effect.Complete += (o, e) =>
                    {
                        Effects.Add(new BuffEffect(Libraries.Monsters[(ushort)Monster.HornedCommander], 1174, 16, 1600, this, true, type) { Repeat = true, Light = 1 });
                    };
                    break;
            }
        }
        public void RemoveBuffEffect(BuffType type)
        {
            PlayerObject ob = null;

            if (Race == ObjectType.Player)
            {
                ob = (PlayerObject)this;
            }

            for (int i = 0; i < Effects.Count; i++)
            {
                if (!(Effects[i] is BuffEffect)) continue;
                if (((BuffEffect)(Effects[i])).BuffType != type) continue;
                Effects[i].Repeat = false;
            }

            switch (type)
            {
                case BuffType.SwiftFeet:
                    if (ob != null) ob.Sprint = false;
                    break;
                case BuffType.MoonLight:
                case BuffType.DarkBody:
                    if (ob != null) ob.Sneaking = false;
                    break;
            }
        }

        public Color ApplyDrawColour()
        {
            Color drawColour = DrawColour;
            if (drawColour == Color.Gray)
            {
                drawColour = Color.White;
                DXManager.SetGrayscale(true);
            }
            return drawColour;
        }

        public virtual Missile CreateProjectile(int baseIndex, MLibrary library, bool blend, int count, int interval, int skip, int lightDistance = 6, bool direction16 = true, Color? lightColour = null, uint targetID = 0)
        {
            return null;
        }

        public void Chat(string text, int type=0, MapObject ob= null)
        {
            //type==0 user
            //type==1 other thing(mob...)
            //type==2 npc
            //type==3 other player
            if (ChatLabel != null && !ChatLabel.IsDisposed)
            {
                ChatLabel.Dispose();
                ChatLabel = null;

                
                ChatLabelBackImage.Dispose();//add k333123
                ChatLabelBackImage = null;

               
                ChatLabelRightDownBackImage.Dispose();
                ChatLabelRightDownBackImage = null; //add k333123

                ChatMiniPortrait.Dispose();//add k333123
                ChatMiniPortrait = null;//add k333123 
                
            }

            //const int chatWidth = 200;
            const int chatWidth = 160;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            text = chat[0];
            for (int i = 1; i < chat.Count; i++)
                text += string.Format("\n{0}", chat[i]);

            //add k333123
            ChatLabelBackImage = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.SAYFRAME,
                Location = new Point(0, 0),
                Parent = ExineMainScene.ActiveScene,
                NotControl = true,
                Visible = true, 

            };

            //add k333123
            ChatLabelRightDownBackImage = new ExineImageControl
            {
                Index = 1,
                Library = Libraries.SAYFRAME,
                Location = new Point(0, 0),
                Parent = ExineMainScene.ActiveScene,
                NotControl = true,
                Visible = true,

            };

            ChatMiniPortrait = new ExineImageControl
            {
                Index = 0,
                Library = Libraries.SAYFRAME, //ID Name - Chatting with Contaion user name?
                Size = new Size(10, 10),
                Location = new Point(0, 0),
                Parent = ChatLabelBackImage,
                NotControl = true,
                Visible = true,
            };
            //ChatMiniPortrait.BeforeDraw += ChatMiniPortrait_BeforeDraw;
           

            ChatLabel = new ExineLabel
            {
                AutoSize = true,
                //BackColour = Color.Transparent,
                //Size = new Size(100,100),
                BackColour = Color.FromArgb(30,Color.Black),
                ForeColour = Color.White,
                //OutLine = true,
                //OutLineColour = Color.Black,
                //DrawFormat = TextFormatFlags.HorizontalCenter,
                //DrawFormat = TextFormatFlags.Default,
                Text = "\r\n "+text+ "\r\n",
            };
            
            if (ChatLabel.Size.Height<40)
            {
                ChatLabel.Size = new Size(ChatLabel.Size.Width, 40);
            }
             
            if (type == 0) //user 
            {
                //if (File.Exists(User.Name + ".jpg") && File.Exists("photo.lib"))
                if (File.Exists("photo.lib"))
                {
                    ChatMiniPortrait.Index = 1;
                    ChatMiniPortrait.Library = new MLibrary("photo.lib"); //ID Name - Chatting with Contaion user name?
                    ChatMiniPortrait.Location = new Point(-20, -28);
                }
                ChatLabel.ForeColour = Color.WhiteSmoke;
                ChatMiniPortrait.Visible = true;
            }

            else if (type == 3) //Other user 
            {
                if (File.Exists(ob.Name+".lib"))
                {
                    ChatMiniPortrait.Index = 1;
                    //ChatMiniPortrait.Library = new MLibrary("photo.lib"); //ID Name - Chatting with Contaion user name?
                    ChatMiniPortrait.Library = new MLibrary(ob.Name+".lib"); 
                    ChatMiniPortrait.Location = new Point(-20, -28);
                }
                else
                {
                    ChatMiniPortrait.Index = 0;
                    ChatMiniPortrait.Library = Libraries.MINI_PORTRAIT;
                    ChatMiniPortrait.Location = new Point(-20, -28);
                    ChatMiniPortrait.Visible = true;
                    ChatLabel.ForeColour = Color.WhiteSmoke;
                }

                ChatLabel.ForeColour = Color.WhiteSmoke;
                ChatMiniPortrait.Visible = true;
            }


            else if (type == 2) //npc speech
            {
                //ChatMiniPortrait.Visible = false;
                ChatMiniPortrait.Index = 0;
                ChatMiniPortrait.Library = Libraries.MINI_PORTRAIT;
                ChatMiniPortrait.Location = new Point(-20, -28);
                ChatMiniPortrait.Visible = true;
                ChatLabel.ForeColour = Color.LimeGreen;
            }
            else //Other
            {
                //ChatMiniPortrait.Visible = false;
                ChatMiniPortrait.Index = 0;
                ChatMiniPortrait.Library = Libraries.MINI_PORTRAIT;
                ChatMiniPortrait.Location = new Point(-20, -28);
                ChatMiniPortrait.Visible = true;
                ChatLabel.ForeColour = Color.WhiteSmoke;
            } 
            ChatTime = CMain.Time + 5000;

            //panel203 - 0
           
        }
        //remove
        private void ChatMiniPortrait_BeforeDraw(object sender, EventArgs e)
        { /*
            if (File.Exists(User.Name + ".jpg") && File.Exists("photo.lib"))
            {
                ChatMiniPortrait.Index = 1;
                ChatMiniPortrait.Library = new MLibrary("photo.lib"); //ID Name - Chatting with Contaion user name?
                ChatMiniPortrait.Location = new Point(-20, -28);
            } 
            */
        }

        public virtual void DrawChat()
        {

            if (ChatLabel == null || ChatLabel.IsDisposed) return;

            if (CMain.Time > ChatTime)
            {
                ChatLabel.Dispose();
                ChatLabel = null;
                ChatLabelBackImage.Hide();
                ChatLabelRightDownBackImage.Hide();//add k333123
                ChatMiniPortrait.Hide();
                return;
            }



            //ChatLabel.ForeColour = Dead ? Color.Gray : Color.White;
            //ChatLabel.Location = new Point(DisplayRectangle.X + (48 - ChatLabel.Size.Width) / 2, DisplayRectangle.Y - (60 + ChatLabel.Size.Height) - (Dead ? 35 : 0));
            
            //ChatLabel.ForeColour = Dead ? Color.Gray : Color.WhiteSmoke; //remove
            ChatLabel.Location = new Point(DisplayRectangle.X + (48 - ChatLabel.Size.Width) / 2 - 20, DisplayRectangle.Y - (60 + ChatLabel.Size.Height) - (Dead ? 35 : 0) - 26); //k333123

            ChatLabelBackImage.Location = 
                new Point
                (
                    //DisplayRectangle.X + (40 - ChatLabel.Size.Width) / 2 ,
                    //DisplayRectangle.Y - (50 + ChatLabel.Size.Height)   - (Dead ? 35 : 0)
                    DisplayRectangle.X + (40 - ChatLabel.Size.Width) / 2 - 24,
                    DisplayRectangle.Y - (50 + ChatLabel.Size.Height) - (Dead ? 35 : 0) - 27
                
                );

            ChatLabelRightDownBackImage.Location =
               new Point
               (
                   //DisplayRectangle.X + (40 - ChatLabel.Size.Width) / 2 ,
                   //DisplayRectangle.Y - (50 + ChatLabel.Size.Height)   - (Dead ? 35 : 0)
                   DisplayRectangle.X + (40 - ChatLabel.Size.Width) / 2 - 24 + ChatLabel.Size.Width+9,
                   DisplayRectangle.Y - (50 + ChatLabel.Size.Height) - (Dead ? 35 : 0) - 47 + ChatLabel.Size.Height+7

               );

            //add k333123
            //이 시점에서 그리기 전에 초상화 부분 불러온 정보를 가지고 mirimageobject를 만든것을 Location잡고 그려준다. 그후 채팅을 그려준다. 
            ChatLabel.Draw();
            ChatLabelBackImage.Show();
            ChatLabelRightDownBackImage.Show();
            ChatMiniPortrait.Show();
        }

        public virtual void CreateLabel()
        {
            NameLabel = null;

            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != Name || LabelList[i].ForeColour != NameColour) continue;
                NameLabel = LabelList[i];
                break;
            }


            if (NameLabel != null && !NameLabel.IsDisposed) return;

            NameLabel = new ExineLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = NameColour,
                OutLine = true,
                OutLineColour = Color.Black,
                Text = Name,
            };
            NameLabel.Disposing += (o, e) => LabelList.Remove(NameLabel);
            LabelList.Add(NameLabel); 
        }
        public virtual void DrawName()
        {
            CreateLabel();

            if (NameLabel == null) return;
            
            NameLabel.Text = Name;
            NameLabel.Location = new Point(DisplayRectangle.X + (50 - NameLabel.Size.Width) / 2, DisplayRectangle.Y - (32 - NameLabel.Size.Height / 2) + (Dead ? 35 : 8) ); //was 48 -
            //NameLabel.Location = new Point(DisplayRectangle.X + 8, DisplayRectangle.Y - 64);
            NameLabel.Draw();
        }
        public virtual void DrawBlend()
        {
            DXManager.SetBlend(true, 0.3F); //0.8
            Draw();
            DXManager.SetBlend(false);
        }
        public void DrawDamages()
        {
            for (int i = Damages.Count - 1; i >= 0; i--)
            {
                Damage info = Damages[i];
                if (CMain.Time > info.ExpireTime)
                {
                    if (info.DamageLabel != null)
                    {
                        info.DamageLabel.Dispose();
                    }

                    Damages.RemoveAt(i);
                }
                else
                {
                    info.Draw(DisplayRectangle.Location);
                }
            }
        }
        public virtual bool ShouldDrawHealth()
        {
            return false;
        }
        public void DrawHealth()
        {
            string name = Name;
            if (Name.Contains("(")) name = Name.Substring(Name.IndexOf("(") + 1, Name.Length - Name.IndexOf("(") - 2);

            if (Dead) return;
            if (Race != ObjectType.Player && Race != ObjectType.Monster ) return;

            if (CMain.Time >= HealthTime)
            {
                if (!ShouldDrawHealth()) return;
            }

            //Libraries.Prguse2.Draw(0, DisplayRectangle.X + 8, DisplayRectangle.Y - 64);
            Libraries.Prguse2.Draw(0, DisplayRectangle.X + 8-25, DisplayRectangle.Y - 64 -18);//k333123 240310
            int index = 1;

            switch (Race)
            {
                case ObjectType.Player:
                    if (GroupDialog.GroupList.Contains(name)) index = 10;
                    break;
                case ObjectType.Monster:
                    if (GroupDialog.GroupList.Contains(name) || name == User.Name) index = 11;
                    break;
            }

            //Libraries.Prguse2.Draw(index, new Rectangle(0, 0, (int)(32 * PercentHealth / 100F), 4), new Point(DisplayRectangle.X + 8, DisplayRectangle.Y - 64), Color.White, false);
            Libraries.Prguse2.Draw(index, new Rectangle(0, 0, (int)(32 * PercentHealth / 100F), 4), new Point(DisplayRectangle.X + 8 -25, DisplayRectangle.Y - 64 -18), Color.White, false);
        }

        public void DrawPoison()
        {
            byte poisoncount = 0;
            if (Poison != PoisonType.None)
            {
                if (Poison.HasFlag(PoisonType.Green))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Green);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Red))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Red);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Bleeding))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.DarkRed);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Slow))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Purple);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Stun) || Poison.HasFlag(PoisonType.Dazed))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Yellow);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Blindness))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.MediumVioletRed);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Frozen))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Blue);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.Paralysis) || Poison.HasFlag(PoisonType.LRParalysis))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Gray);
                    poisoncount++;
                }
                if (Poison.HasFlag(PoisonType.DelayedExplosion))
                {
                    DXManager.Draw(DXManager.PoisonDotBackground, new Rectangle(0, 0, 6, 6), new Vector3((float)(DisplayRectangle.X + 7 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 21), 0.0F), Color.Black);
                    DXManager.Draw(DXManager.RadarTexture, new Rectangle(0, 0, 4, 4), new Vector3((float)(DisplayRectangle.X + 8 + (poisoncount * 5)), (float)(DisplayRectangle.Y - 20), 0.0F), Color.Orange);
                    poisoncount++;
                }
            }
        }

        public abstract void DrawBehindEffects(bool effectsEnabled);

        public abstract void DrawEffects(bool effectsEnabled);

        protected void LoopFrame(int start, int frameCount, int frameInterval, int duration)
        {
            if (FrameLoop == null)
            {
                FrameLoop = new FrameLoop
                {
                    Start = start,
                    End = start + frameCount - 1,
                    Loops = (duration / (frameInterval * frameCount)) - 1 //Remove 1 count as we've already done a loop before this is checked
                };
            }
        }
    }

    public class FrameLoop
    {
        public ExAction Action { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Loops { get; set; }

        public int CurrentCount { get; set; }
    }

}

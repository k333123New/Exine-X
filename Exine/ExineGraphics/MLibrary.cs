using SlimDX;
using SlimDX.Direct3D9;
using System.IO.Compression;
using Frame = Exine.ExineObjects.Frame;
using Exine.ExineObjects;
using System.Text.RegularExpressions;
using System.Text;

namespace Exine.ExineGraphics
{
    public static class Libraries
    {
        public static bool Loaded;
        public static int Count, Progress;

        /*
        public static readonly MLibrary
            AHM_0000 = new MLibrary(Settings.DataPath + "AHM_0000");
        */

        public static readonly MLibrary

            ExineOpening = new MLibrary(Settings.ExineVideoPath + "001-Intro"),
            //ExineLogin = new MLibrary(Settings.ExineUIPath + "ExineLogin"),
            //ExineNewChar = new MLibrary(Settings.ExineUIPath + "ExineNewChar"),
            NewChar = new MLibrary(Settings.ExineUIPath + "NewChar"),

            MAINSAMPLE = new MLibrary(Settings.ExineUIPath + "MainSample"),
            BAZEL = new MLibrary(Settings.ExineUIPath + "bazel"),
            PANEL0000 = new MLibrary(Settings.ExineUIPath + "PANEL0000"),
            PANEL0001 = new MLibrary(Settings.ExineUIPath + "PANEL0001"),
            PANEL0100 = new MLibrary(Settings.ExineUIPath + "PANEL0100"),
            PANEL0200 = new MLibrary(Settings.ExineUIPath + "PANEL0200"),
            PANEL0201 = new MLibrary(Settings.ExineUIPath + "PANEL0201"),
            PANEL0202 = new MLibrary(Settings.ExineUIPath + "PANEL0202"),
            PANEL0203 = new MLibrary(Settings.ExineUIPath + "PANEL0203"),
            PANEL0204 = new MLibrary(Settings.ExineUIPath + "PANEL0204"),
            PANEL0205 = new MLibrary(Settings.ExineUIPath + "PANEL0205"),
            PANEL0300 = new MLibrary(Settings.ExineUIPath + "PANEL0300"),
            PANEL0301 = new MLibrary(Settings.ExineUIPath + "PANEL0301"),
            PANEL0400 = new MLibrary(Settings.ExineUIPath + "PANEL0400"),
            PANEL0401 = new MLibrary(Settings.ExineUIPath + "PANEL0401"),
            PANEL0500 = new MLibrary(Settings.ExineUIPath + "PANEL0500"),
            PANEL0501 = new MLibrary(Settings.ExineUIPath + "PANEL0501"),
            PANEL0502 = new MLibrary(Settings.ExineUIPath + "PANEL0502"),
            PANEL0503 = new MLibrary(Settings.ExineUIPath + "PANEL0503"),
            PANEL0504 = new MLibrary(Settings.ExineUIPath + "PANEL0504"),
            PANEL0505 = new MLibrary(Settings.ExineUIPath + "PANEL0505"),
            PANEL0506 = new MLibrary(Settings.ExineUIPath + "PANEL0506"),
            PANEL0507 = new MLibrary(Settings.ExineUIPath + "PANEL0507"),
            PANEL0509 = new MLibrary(Settings.ExineUIPath + "PANEL0509"),
            PANEL0510 = new MLibrary(Settings.ExineUIPath + "PANEL0510"),
            PANEL0511 = new MLibrary(Settings.ExineUIPath + "PANEL0511"),
            PANEL0512 = new MLibrary(Settings.ExineUIPath + "PANEL0512"),
            PANEL0600 = new MLibrary(Settings.ExineUIPath + "PANEL0600"),
            PANEL0601 = new MLibrary(Settings.ExineUIPath + "PANEL0601"),
            PANEL0602 = new MLibrary(Settings.ExineUIPath + "PANEL0602"),
            PANEL0603 = new MLibrary(Settings.ExineUIPath + "PANEL0603"),
            PANEL0604 = new MLibrary(Settings.ExineUIPath + "PANEL0604"),
            PANEL0605 = new MLibrary(Settings.ExineUIPath + "PANEL0605"),
            PANEL0606 = new MLibrary(Settings.ExineUIPath + "PANEL0606"),
            PANEL0700 = new MLibrary(Settings.ExineUIPath + "PANEL0700"),
            PANEL0710 = new MLibrary(Settings.ExineUIPath + "PANEL0710"),
            PANEL0800 = new MLibrary(Settings.ExineUIPath + "PANEL0800"),
            PANEL0900 = new MLibrary(Settings.ExineUIPath + "PANEL0900"),
            PANEL0901 = new MLibrary(Settings.ExineUIPath + "PANEL0901"),
            PANEL0902 = new MLibrary(Settings.ExineUIPath + "PANEL0902"),
            PANEL0903 = new MLibrary(Settings.ExineUIPath + "PANEL0903"),
            PANEL0904 = new MLibrary(Settings.ExineUIPath + "PANEL0904"),
            PANEL0905 = new MLibrary(Settings.ExineUIPath + "PANEL0905"),
            PANEL1000 = new MLibrary(Settings.ExineUIPath + "PANEL1000"),
            PANEL1100 = new MLibrary(Settings.ExineUIPath + "PANEL1100"),
            PANEL1200 = new MLibrary(Settings.ExineUIPath + "PANEL1200"),
            SAYFRAME = new MLibrary(Settings.ExineUIPath + "SayFrame"),



            ServerButton = new MLibrary(Settings.ExineUIPath + "ServerButton"),

            BIK_002_Orb1_1 = new MLibrary(Settings.ExineVideoPath + "002-Orb1-1"),
            BIK_003_Orb1_2 = new MLibrary(Settings.ExineVideoPath + "003-Orb1-2"),
            BIK_004_Orb1_3 = new MLibrary(Settings.ExineVideoPath + "004-Orb1-3"),
            BIK_005_Orb1_4 = new MLibrary(Settings.ExineVideoPath + "005-Orb1-4"),
            BIK_006_Orb2_1 = new MLibrary(Settings.ExineVideoPath + "006-Orb2-1"),
            BIK_007_Orb2_2 = new MLibrary(Settings.ExineVideoPath + "007-Orb2-2"),
            BIK_008_Orb2_3 = new MLibrary(Settings.ExineVideoPath + "008-Orb2-3"),
            BIK_009_Orb2_4 = new MLibrary(Settings.ExineVideoPath + "009-Orb2-4"),

            BIK_010_Gargoyle_1 = new MLibrary(Settings.ExineVideoPath + "010-Gargoyle-1"),
            BIK_011_Gargoyle_2 = new MLibrary(Settings.ExineVideoPath + "011-Gargoyle-2"),
            BIK_012_Gargoyle_3 = new MLibrary(Settings.ExineVideoPath + "012-Gargoyle-3"),
            BIK_013_Gargoyle_4 = new MLibrary(Settings.ExineVideoPath + "013-Gargoyle-4"),
            BIK_014_Gargoyle_5 = new MLibrary(Settings.ExineVideoPath + "014-Gargoyle-5"),
            BIK_015_Gargoyle_6 = new MLibrary(Settings.ExineVideoPath + "015-Gargoyle-6"),

            BIK_016_Orb3_1 = new MLibrary(Settings.ExineVideoPath + "016-Orb3-1"),
            BIK_017_Orb3_2 = new MLibrary(Settings.ExineVideoPath + "017-Orb3-2"),
            BIK_018_Orb3_3 = new MLibrary(Settings.ExineVideoPath + "018-Orb3-3"),
            BIK_019_Orb3_4 = new MLibrary(Settings.ExineVideoPath + "019-Orb3-4"),
            
            BIK_021_Light_1 = new MLibrary(Settings.ExineVideoPath + "021-Light-1"),
            BIK_022_Light_2 = new MLibrary(Settings.ExineVideoPath + "022-Light-2"),
            BIK_023_Light_3 = new MLibrary(Settings.ExineVideoPath + "023-Light-3"),
            BIK_024_Light_4 = new MLibrary(Settings.ExineVideoPath + "024-Light-4");



        /*
        library = new MLibrary[count];

        for (int i = 0; i < count; i++)
        {
            library[i] = new MLibrary(path + i.ToString(toStringValue) + suffix);
        }*/
        public static MLibrary[] HHMs = new MLibrary[10];
        //AHM
        //AHW
        //HHM
        //HHW
        //SHM
        //SHW
        //WHM
        //WHW



        public static readonly MLibrary
            
            Prguse = new MLibrary(Settings.DataPath + "Prguse"),
            Prguse2 = new MLibrary(Settings.DataPath + "Prguse2"),
            Prguse3 = new MLibrary(Settings.DataPath + "Prguse3"),
            BuffIcon = new MLibrary(Settings.DataPath + "BuffIcon"),
            Help = new MLibrary(Settings.DataPath + "Help"),
            MiniMap = new MLibrary(Settings.DataPath + "MMap"),
            MapLinkIcon = new MLibrary(Settings.DataPath + "MapLinkIcon"),
            Title = new MLibrary(Settings.DataPath + "Title"),
            MagIcon = new MLibrary(Settings.DataPath + "MagIcon"),
            MagIcon2 = new MLibrary(Settings.DataPath + "MagIcon2"),
            Magic = new MLibrary(Settings.DataPath + "Magic"),
            Magic2 = new MLibrary(Settings.DataPath + "Magic2"),
            Magic3 = new MLibrary(Settings.DataPath + "Magic3"),
            Effect = new MLibrary(Settings.DataPath + "Effect"),
            MagicC = new MLibrary(Settings.DataPath + "MagicC"),
            GuildSkill = new MLibrary(Settings.DataPath + "GuildSkill");

        public static readonly MLibrary
            Background = new MLibrary(Settings.DataPath + "Background");


        public static readonly MLibrary
            Dragon = new MLibrary(Settings.DataPath + "Dragon");

        //Map
        //public static readonly MLibrary[] MapLibs = new MLibrary[400];
        public static readonly MLibrary[] MapLibs = new MLibrary[2000];//k333123

        //Items
        public static readonly MLibrary
            Items = new MLibrary(Settings.DataPath + "Items"),
            StateItems = new MLibrary(Settings.DataPath + "StateItem"),
            FloorItems = new MLibrary(Settings.DataPath + "DNItems");

        //Deco
        public static readonly MLibrary
            Deco = new MLibrary(Settings.DataPath + "Deco");



        public static MLibrary[] CArmours,
                                          CWeapons,
										  CWeaponEffect,
										  CHair,
                                          CHumEffect,
                                          AArmours,
                                          AWeaponsL,
                                          AWeaponsR,
                                          AHair,
                                          AHumEffect,
                                          ARArmours,
                                          ARWeapons,
                                          ARWeaponsS,
                                          ARHair,
                                          ARHumEffect,
                                          Monsters,
                                          Gates,
                                          Flags,
                                          Siege,
                                          Mounts,
                                          NPCs,
                                          Fishing,
                                          Pets,
                                          Transform,
                                          TransformMounts,
                                          TransformEffect,
                                          TransformWeaponEffect;

        static Libraries()
        {
            //Wiz/War/Tao
            InitLibrary(ref CArmours, Settings.CArmourPath, "00");
            InitLibrary(ref CHair, Settings.CHairPath, "00");
            InitLibrary(ref CWeapons, Settings.CWeaponPath, "00");
            InitLibrary(ref CWeaponEffect, Settings.CWeaponEffectPath, "00");
            InitLibrary(ref CHumEffect, Settings.CHumEffectPath, "00");

            //Assassin
            InitLibrary(ref AArmours, Settings.AArmourPath, "00");
            InitLibrary(ref AHair, Settings.AHairPath, "00");
            InitLibrary(ref AWeaponsL, Settings.AWeaponPath, "00", " L");
            InitLibrary(ref AWeaponsR, Settings.AWeaponPath, "00", " R");
            InitLibrary(ref AHumEffect, Settings.AHumEffectPath, "00");

            //Archer
            InitLibrary(ref ARArmours, Settings.ARArmourPath, "00");
            InitLibrary(ref ARHair, Settings.ARHairPath, "00");
            InitLibrary(ref ARWeapons, Settings.ARWeaponPath, "00");
            InitLibrary(ref ARWeaponsS, Settings.ARWeaponPath, "00", " S");
            InitLibrary(ref ARHumEffect, Settings.ARHumEffectPath, "00");

            //Other
            InitLibrary(ref Monsters, Settings.MonsterPath, "000");
            InitLibrary(ref Gates, Settings.GatePath, "00");
            InitLibrary(ref Flags, Settings.FlagPath, "00");
            InitLibrary(ref Siege, Settings.SiegePath, "00");
            InitLibrary(ref NPCs, Settings.NPCPath, "00");
            InitLibrary(ref Mounts, Settings.MountPath, "00");
            InitLibrary(ref Fishing, Settings.FishingPath, "00");
            InitLibrary(ref Pets, Settings.PetsPath, "00");
            InitLibrary(ref Transform, Settings.TransformPath, "00");
            InitLibrary(ref TransformMounts, Settings.TransformMountsPath, "00");
            InitLibrary(ref TransformEffect, Settings.TransformEffectPath, "00");
            InitLibrary(ref TransformWeaponEffect, Settings.TransformWeaponEffectPath, "00");




            #region Maplibs

            for (int i = 0; i < 10; i++)
            {
                MapLibs[i] = new MLibrary(Settings.DataPath + "Map\\Exine\\TS_0" + i + "_Tile"); 
            }

            for (int i = 0; i < 10; i++)
            {
                MapLibs[i + 10] = new MLibrary(Settings.DataPath + "Map\\Exine\\TS_0" + i + "_Static"); 
            }

            MapLibs[20] = new MLibrary(Settings.DataPath + "Map\\Exine\\Smtiles"); 

            for (int i = 1000; i < 2000; i++)
            {
                if (File.Exists(Settings.DataPath + "Map\\Exine\\Map_" + (9000 + i) + "_FrontTile.lib"))
                {
                    MapLibs[i] = new MLibrary(Settings.DataPath + "Map\\Exine\\Map_" + (9000 + i) + "_FrontTile"); 
                }
            }

            #endregion

            LoadLibraries();

            Thread thread = new Thread(LoadGameLibraries) { IsBackground = true };
            thread.Start();
        }

        //k333123 231205
        static void ExInitLibrary(ref MLibrary[] library, string path, string toStringValue, string suffix = "")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var allFiles = Directory.GetFiles(path, "*" + suffix + MLibrary.Extention, SearchOption.TopDirectoryOnly).OrderBy(x => int.Parse(Regex.Match(x, @"\d+").Value));

            var lastFile = allFiles.Count() > 0 ? Path.GetFileName(allFiles.Last()) : "0";
             
            library = new MLibrary[allFiles.Count()];
            int i = 0;
            foreach (var file in allFiles)
            {
                library[i] = new MLibrary(path+ Path.GetFileName(file) + suffix);
                i++;
            }
        }

        static void InitLibrary(ref MLibrary[] library, string path, string toStringValue, string suffix = "")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var allFiles = Directory.GetFiles(path, "*" + suffix + MLibrary.Extention, SearchOption.TopDirectoryOnly).OrderBy(x => int.Parse(Regex.Match(x, @"\d+").Value));

            var lastFile = allFiles.Count() > 0 ? Path.GetFileName(allFiles.Last()) : "0";

            var count = int.Parse(Regex.Match(lastFile, @"\d+").Value) + 1;

            library = new MLibrary[count];

            for (int i = 0; i < count; i++)
            {
                library[i] = new MLibrary(path + i.ToString(toStringValue) + suffix);
            }
        }

        static void LoadLibraries()
        {
            //k333123
            ExineOpening.Initialize();
            Progress++;
            //ExineLogin.Initialize();
            //ExineNewChar.Initialize();
            //Progress++;
            NewChar.Initialize();
            Progress++;
            MAINSAMPLE.Initialize();
            Progress++;
            BAZEL.Initialize();
            Progress++;
            PANEL0000.Initialize();
            Progress++;
            PANEL0001.Initialize();
            Progress++;
            PANEL0100.Initialize();
            Progress++;
            PANEL0200.Initialize();
            Progress++;
            PANEL0201.Initialize();
            Progress++;
            PANEL0202.Initialize();
            Progress++;
            PANEL0203.Initialize();
            Progress++;
            PANEL0204.Initialize();
            Progress++;
            PANEL0205.Initialize();
            Progress++;
            PANEL0300.Initialize();
            Progress++;
            PANEL0301.Initialize();
            Progress++;
            PANEL0400.Initialize();
            Progress++;
            PANEL0401.Initialize();
            Progress++;
            PANEL0500.Initialize();
            Progress++;
            PANEL0501.Initialize();
            Progress++;
            PANEL0502.Initialize();
            Progress++;
            PANEL0503.Initialize();
            Progress++;
            PANEL0504.Initialize();
            Progress++;
            PANEL0505.Initialize();
            Progress++;
            PANEL0506.Initialize();
            Progress++;
            PANEL0507.Initialize();
            Progress++;
            PANEL0509.Initialize();
            Progress++;
            PANEL0510.Initialize();
            Progress++;
            PANEL0511.Initialize();
            Progress++;
            PANEL0512.Initialize();
            Progress++;
            PANEL0600.Initialize();
            Progress++;
            PANEL0601.Initialize();
            Progress++;
            PANEL0602.Initialize();
            Progress++;
            PANEL0603.Initialize();
            Progress++;
            PANEL0604.Initialize();
            Progress++;
            PANEL0605.Initialize();
            Progress++;
            PANEL0606.Initialize();
            Progress++;
            PANEL0700.Initialize();
            Progress++;
            PANEL0710.Initialize();
            Progress++;
            PANEL0800.Initialize();
            Progress++;
            PANEL0900.Initialize();
            Progress++;
            PANEL0901.Initialize();
            Progress++;
            PANEL0902.Initialize();
            Progress++;
            PANEL0903.Initialize();
            Progress++;
            PANEL0904.Initialize();
            Progress++;
            PANEL0905.Initialize();
            Progress++;
            PANEL1000.Initialize();
            Progress++;
            PANEL1100.Initialize();
            Progress++;
            PANEL1200.Initialize();
            Progress++;
            SAYFRAME.Initialize();
            Progress++;
            ServerButton.Initialize();
            Progress++;

            BIK_002_Orb1_1.Initialize();
            Progress++;
            BIK_003_Orb1_2.Initialize();
            Progress++;
            BIK_004_Orb1_3.Initialize();
            Progress++;
            BIK_005_Orb1_4.Initialize();
            Progress++;
            BIK_006_Orb2_1.Initialize();
            Progress++;
            BIK_007_Orb2_2.Initialize();
            Progress++;
            BIK_008_Orb2_3.Initialize();
            Progress++;
            BIK_009_Orb2_4.Initialize();
            Progress++;

            BIK_010_Gargoyle_1.Initialize();
            Progress++;
            BIK_011_Gargoyle_2.Initialize();
            Progress++;
            BIK_012_Gargoyle_3.Initialize();
            Progress++;
            BIK_013_Gargoyle_4.Initialize();
            Progress++;
            BIK_014_Gargoyle_5.Initialize();
            Progress++;
            BIK_015_Gargoyle_6.Initialize();
            Progress++;

            BIK_016_Orb3_1.Initialize();
            Progress++;
            BIK_017_Orb3_2.Initialize();
            Progress++;
            BIK_018_Orb3_3.Initialize();
            Progress++;
            BIK_019_Orb3_4.Initialize();
            Progress++;

            BIK_021_Light_1.Initialize();
            Progress++;
            BIK_022_Light_2.Initialize();
            Progress++;
            BIK_023_Light_3.Initialize();
            Progress++;
            BIK_024_Light_4.Initialize();
            Progress++;

            
            Prguse.Initialize();
            Progress++;

            Prguse2.Initialize();
            Progress++;

            Prguse3.Initialize();
            Progress++;

            Title.Initialize();
            Progress++;
        }

        private static void LoadGameLibraries()
        {
            Count = MapLibs.Length + Monsters.Length + Gates.Length + Flags.Length + Siege.Length + NPCs.Length + CArmours.Length +
                CHair.Length + CWeapons.Length + CWeaponEffect.Length + AArmours.Length + AHair.Length + AWeaponsL.Length + AWeaponsR.Length +
                ARArmours.Length + ARHair.Length + ARWeapons.Length + ARWeaponsS.Length +
                CHumEffect.Length + AHumEffect.Length + ARHumEffect.Length + Mounts.Length + Fishing.Length + Pets.Length +
                Transform.Length + TransformMounts.Length + TransformEffect.Length + TransformWeaponEffect.Length + 18;

            Dragon.Initialize();
            Progress++;

            BuffIcon.Initialize();
            Progress++;

            Help.Initialize();
            Progress++;

            MiniMap.Initialize();
            Progress++;
            MapLinkIcon.Initialize();
            Progress++;

            MagIcon.Initialize();
            Progress++;
            MagIcon2.Initialize();
            Progress++;

            Magic.Initialize();
            Progress++;
            Magic2.Initialize();
            Progress++;
            Magic3.Initialize();
            Progress++;
            MagicC.Initialize();
            Progress++;

            Effect.Initialize();
            Progress++;

            GuildSkill.Initialize();
            Progress++;

            Background.Initialize();
            Progress++;

            Deco.Initialize();
            Progress++;

            Items.Initialize();
            Progress++;
            StateItems.Initialize();
            Progress++;
            FloorItems.Initialize();
            Progress++;

            for (int i = 0; i < MapLibs.Length; i++)
            {
                if (MapLibs[i] == null)
                    MapLibs[i] = new MLibrary("");
                else
                    MapLibs[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Monsters.Length; i++)
            {
                Monsters[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Gates.Length; i++)
            {
                Gates[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Flags.Length; i++)
            {
                Flags[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Siege.Length; i++)
            {
                Siege[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < NPCs.Length; i++)
            {
                NPCs[i].Initialize();
                Progress++;
            }


            for (int i = 0; i < CArmours.Length; i++)
            {
                CArmours[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CHair.Length; i++)
            {
                CHair[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CWeapons.Length; i++)
            {
                CWeapons[i].Initialize();
                Progress++;
            }

			for (int i = 0; i < CWeaponEffect.Length; i++)
			{
				CWeaponEffect[i].Initialize();
				Progress++;
			}

			for (int i = 0; i < AArmours.Length; i++)
            {
                AArmours[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AHair.Length; i++)
            {
                AHair[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AWeaponsL.Length; i++)
            {
                AWeaponsL[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AWeaponsR.Length; i++)
            {
                AWeaponsR[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < ARArmours.Length; i++)
            {
                ARArmours[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < ARHair.Length; i++)
            {
                ARHair[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < ARWeapons.Length; i++)
            {
                ARWeapons[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < ARWeaponsS.Length; i++)
            {
                ARWeaponsS[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < CHumEffect.Length; i++)
            {
                CHumEffect[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < AHumEffect.Length; i++)
            {
                AHumEffect[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < ARHumEffect.Length; i++)
            {
                ARHumEffect[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Mounts.Length; i++)
            {
                Mounts[i].Initialize();
                Progress++;
            }


            for (int i = 0; i < Fishing.Length; i++)
            {
                Fishing[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Pets.Length; i++)
            {
                Pets[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < Transform.Length; i++)
            {
                Transform[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < TransformEffect.Length; i++)
            {
                TransformEffect[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < TransformWeaponEffect.Length; i++)
            {
                TransformWeaponEffect[i].Initialize();
                Progress++;
            }

            for (int i = 0; i < TransformMounts.Length; i++)
            {
                TransformMounts[i].Initialize();
                Progress++;
            }
            
            Loaded = true;
        }

    }

    public sealed class MLibrary
    {
        public const string Extention = ".Lib";
        public const int LibVersion = 3;

        private readonly string _fileName;

        private MImage[] _images;
        private FrameSet _frames;
        private int[] _indexList;
        private int _count;
        private bool _initialized;

        private BinaryReader _reader;
        private FileStream _fStream;

        public FrameSet Frames
        {
            get { return _frames; }
        }

        public MLibrary(string filename)
        {
            _fileName = Path.ChangeExtension(filename, Extention);
        }

        public void Initialize()
        {
            _initialized = true;

            if (!File.Exists(_fileName))
                return;

            try
            {
                _fStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                _reader = new BinaryReader(_fStream);
                int currentVersion = _reader.ReadInt32();
                if (currentVersion < 2)
                {
                    System.Windows.Forms.MessageBox.Show("Wrong version, expecting lib version: " + LibVersion.ToString() + " found version: " + currentVersion.ToString() + ".", _fileName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error, System.Windows.Forms.MessageBoxDefaultButton.Button1);
                    System.Windows.Forms.Application.Exit();
                    return;
                }
                _count = _reader.ReadInt32();

                int frameSeek = 0;
                if (currentVersion >= 3)
                {
                    frameSeek = _reader.ReadInt32();
                }

                _images = new MImage[_count];
                _indexList = new int[_count];

                for (int i = 0; i < _count; i++)
                    _indexList[i] = _reader.ReadInt32();

                if (currentVersion >= 3)
                {
                    _fStream.Seek(frameSeek, SeekOrigin.Begin);

                    var frameCount = _reader.ReadInt32();

                    if (frameCount > 0)
                    {
                        _frames = new FrameSet();
                        for (int i = 0; i < frameCount; i++)
                        {
                            _frames.Add((ExAction)_reader.ReadByte(), new Frame(_reader));
                        }
                    }
                }
            }
            catch (Exception)
            {
                _initialized = false;
                throw;
            }
        }

        private bool CheckImage(int index)
        {
            if (!_initialized)
                Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return false;

            if (_images[index] == null)
            {
                _fStream.Position = _indexList[index];
                _images[index] = new MImage(_reader);
            }
            MImage mi = _images[index];
            if (!mi.TextureValid)
            {
                if ((mi.Width == 0) || (mi.Height == 0))
                    return false;
                _fStream.Seek(_indexList[index] + 17, SeekOrigin.Begin);
                mi.CreateTexture(_reader);
            }

            return true;
        }

        public Point GetOffSet(int index)
        {
            if (!_initialized) Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return Point.Empty;

            if (_images[index] == null)
            {
                _fStream.Seek(_indexList[index], SeekOrigin.Begin);
                _images[index] = new MImage(_reader);
            }

            return new Point(_images[index].X, _images[index].Y);
        }
        public Size GetSize(int index)
        {
            if (!_initialized) Initialize();
            if (_images == null || index < 0 || index >= _images.Length)
                return Size.Empty;

            if (_images[index] == null)
            {
                _fStream.Seek(_indexList[index], SeekOrigin.Begin);
                _images[index] = new MImage(_reader);
            }

            return new Size(_images[index].Width, _images[index].Height);
        }
        public Size GetTrueSize(int index)
        {
            if (!_initialized)
                Initialize();

            if (_images == null || index < 0 || index >= _images.Length)
                return Size.Empty;

            if (_images[index] == null)
            {
                _fStream.Position = _indexList[index];
                _images[index] = new MImage(_reader);
            }
            MImage mi = _images[index];
            if (mi.TrueSize.IsEmpty)
            {
                if (!mi.TextureValid)
                {
                    if ((mi.Width == 0) || (mi.Height == 0))
                        return Size.Empty;

                    _fStream.Seek(_indexList[index] + 17, SeekOrigin.Begin);
                    mi.CreateTexture(_reader);
                }
                return mi.GetTrueSize();
            }
            return mi.TrueSize;
        }

        public void Draw(int index, int x, int y)
        {
            if (x >= Settings.ScreenWidth || y >= Settings.ScreenHeight)
                return;

            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (x + mi.Width < 0 || y + mi.Height < 0)
                return;


            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)x, (float)y, 0.0F), Color.White);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Point point, Color colour, bool offSet = false)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), colour);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void Draw(int index, Point point, Color colour, bool offSet, float opacity)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            DXManager.DrawOpaque(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), colour, opacity); 

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void DrawBlend(int index, Point point, Color colour, bool offSet = false, float rate = 1)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            bool oldBlend = DXManager.Blending;
            DXManager.SetBlend(true, rate);

            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), colour);

            DXManager.SetBlend(oldBlend);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Rectangle section, Point point, Color colour, bool offSet)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            if (section.Right > mi.Width)
                section.Width -= section.Right - mi.Width;

            if (section.Bottom > mi.Height)
                section.Height -= section.Bottom - mi.Height;

            DXManager.Draw(mi.Image, section, new Vector3((float)point.X, (float)point.Y, 0.0F), colour);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Rectangle section, Point point, Color colour, float opacity)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            if (section.Right > mi.Width)
                section.Width -= section.Right - mi.Width;

            if (section.Bottom > mi.Height)
                section.Height -= section.Bottom - mi.Height;

            DXManager.DrawOpaque(mi.Image, section, new Vector3((float)point.X, (float)point.Y, 0.0F), colour, opacity); 

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void Draw(int index, Point point, Size size, Color colour)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + size.Width < 0 || point.Y + size.Height < 0)
                return;

            float scaleX = (float)size.Width / mi.Width;
            float scaleY = (float)size.Height / mi.Height;

            Matrix matrix = Matrix.Scaling(scaleX, scaleY, 0);
            DXManager.Sprite.Transform = matrix;
            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X / scaleX, (float)point.Y / scaleY, 0.0F), Color.White); 

            DXManager.Sprite.Transform = Matrix.Identity;

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void DrawTinted(int index, Point point, Color colour, Color Tint, bool offSet = false)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), colour);

            if (mi.HasMask)
            {
                DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Tint);
            }

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void ExineDrawTinted(int index, Point point, Color colour, Color Tint, bool offSet = false, bool isBlending=false, float rate = 1 )
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            if (offSet) point.Offset(mi.X, mi.Y);

            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            bool oldBlend = DXManager.Blending;
            //if(isBlending)DXManager.SetBlend(true, rate,BlendMode.INVLIGHT);
            //if (isBlending) DXManager.SetBlend(true, rate, BlendMode.NONE);//NONE
            if (isBlending) DXManager.SetBlend(true, rate, BlendMode.NORMAL);//NONE
            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), colour);
            if (isBlending) DXManager.SetBlend(oldBlend);

            if (mi.HasMask)
            {
                DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Tint);
            }

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void DrawUp(int index, int x, int y)
        {
            if (x >= Settings.ScreenWidth)
                return;

            if (!CheckImage(index))
                return;

            MImage mi = _images[index];
            y -= mi.Height;
            if (y >= Settings.ScreenHeight)
                return;
            if (x + mi.Width < 0 || y + mi.Height < 0)
                return;

            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3(x, y, 0.0F), Color.White);

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }
        public void DrawUpBlend(int index, Point point)
        {
            if (!CheckImage(index))
                return;

            MImage mi = _images[index];

            point.Y -= mi.Height;


            if (point.X >= Settings.ScreenWidth || point.Y >= Settings.ScreenHeight || point.X + mi.Width < 0 || point.Y + mi.Height < 0)
                return;

            bool oldBlend = DXManager.Blending;
            DXManager.SetBlend(true, 1);

            DXManager.Draw(mi.Image, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Color.White);

            DXManager.SetBlend(oldBlend);
            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public bool VisiblePixel(int index, Point point, bool accuate)
        {
            if (!CheckImage(index))
                return false;

            if (accuate)
                return _images[index].VisiblePixel(point);

            int accuracy = 2;

            for (int x = -accuracy; x <= accuracy; x++)
                for (int y = -accuracy; y <= accuracy; y++)
                    if (_images[index].VisiblePixel(new Point(point.X + x, point.Y + y)))
                        return true;

            return false;
        }
    }

    public sealed class MImage
    {
        public short Width, Height, X, Y, ShadowX, ShadowY;
        public byte Shadow;
        public int Length;

        public bool TextureValid;
        public Texture Image;
        //layer 2:
        public short MaskWidth, MaskHeight, MaskX, MaskY;
        public int MaskLength;

        public Texture MaskImage;
        public Boolean HasMask;

        public long CleanTime;
        public Size TrueSize;

        public unsafe byte* Data;

        public MImage(BinaryReader reader)
        {
            //read layer 1
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            ShadowX = reader.ReadInt16();
            ShadowY = reader.ReadInt16();
            Shadow = reader.ReadByte();
            Length = reader.ReadInt32();

            //check if there's a second layer and read it
            HasMask = ((Shadow >> 7) == 1) ? true : false;
            if (HasMask)
            {
                reader.ReadBytes(Length);
                MaskWidth = reader.ReadInt16();
                MaskHeight = reader.ReadInt16();
                MaskX = reader.ReadInt16();
                MaskY = reader.ReadInt16();
                MaskLength = reader.ReadInt32();
            }
        }

        public unsafe void CreateTexture(BinaryReader reader)
        {
            int w = Width;// + (4 - Width % 4) % 4;
            int h = Height;// + (4 - Height % 4) % 4;

            Image = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            DataRectangle stream = Image.LockRectangle(0, LockFlags.Discard);
            Data = (byte*)stream.Data.DataPointer;

            DecompressImage(reader.ReadBytes(Length), stream.Data);

            stream.Data.Dispose();
            Image.UnlockRectangle(0);

            if (HasMask)
            {
                reader.ReadBytes(12);
                w = Width;// + (4 - Width % 4) % 4;
                h = Height;// + (4 - Height % 4) % 4;

                MaskImage = new Texture(DXManager.Device, w, h, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                stream = MaskImage.LockRectangle(0, LockFlags.Discard);

                DecompressImage(reader.ReadBytes(Length), stream.Data);

                stream.Data.Dispose();
                MaskImage.UnlockRectangle(0);
            }

            DXManager.TextureList.Add(this);
            TextureValid = true;

            CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public unsafe void DisposeTexture()
        {
            DXManager.TextureList.Remove(this);

            if (Image != null && !Image.Disposed)
            {
                Image.Dispose();
            }

            if (MaskImage != null && !MaskImage.Disposed)
            {
                MaskImage.Dispose();
            }

            TextureValid = false;
            Image = null;
            MaskImage = null;
            Data = null;
        }

        public unsafe bool VisiblePixel(Point p)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Width || p.Y >= Height)
                return false;

            int w = Width;

            bool result = false;
            if (Data != null)
            {
                int x = p.X;
                int y = p.Y;
                
                int index = (y * (w << 2)) + (x << 2);
                
                byte col = Data[index];

                if (col == 0) return false;
                else return true;
            }
            return result;
        }

        public Size GetTrueSize()
        {
            if (TrueSize != Size.Empty) return TrueSize;

            int l = 0, t = 0, r = Width, b = Height;

            bool visible = false;
            for (int x = 0; x < r; x++)
            {
                for (int y = 0; y < b; y++)
                {
                    if (!VisiblePixel(new Point(x, y))) continue;

                    visible = true;
                    break;
                }

                if (!visible) continue;

                l = x;
                break;
            }

            visible = false;
            for (int y = 0; y < b; y++)
            {
                for (int x = l; x < r; x++)
                {
                    if (!VisiblePixel(new Point(x, y))) continue;

                    visible = true;
                    break;

                }
                if (!visible) continue;

                t = y;
                break;
            }

            visible = false;
            for (int x = r - 1; x >= l; x--)
            {
                for (int y = 0; y < b; y++)
                {
                    if (!VisiblePixel(new Point(x, y))) continue;

                    visible = true;
                    break;
                }

                if (!visible) continue;

                r = x + 1;
                break;
            }

            visible = false;
            for (int y = b - 1; y >= t; y--)
            {
                for (int x = l; x < r; x++)
                {
                    if (!VisiblePixel(new Point(x, y))) continue;

                    visible = true;
                    break;

                }
                if (!visible) continue;

                b = y + 1;
                break;
            }

            TrueSize = Rectangle.FromLTRB(l, t, r, b).Size;

            return TrueSize;
        }

        private static byte[] DecompressImage(byte[] image)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(image), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        private static void DecompressImage(byte[] data, Stream destination)
        {
            using (var stream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress))
            {
                stream.CopyTo(destination);
            }
        }
    }
}

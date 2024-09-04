using SlimDX;
using SlimDX.Direct3D9;
using System.IO.Compression;
using Frame = Exine.ExineObjects.Frame;
using Exine.ExineObjects;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using static Exine.ExineGraphics.WeaponMapperMgr;
using System;
using System.Drawing;
using System.Reflection;
//using static System.Net.WebRequestMethods;

namespace Exine.ExineGraphics
{
   

    public class WeaponMapperMgr
    {
        public class ShapeToLibIndex
        {
            public int shapeIdx;
            public int libIndex;
            public int weaponType;  //1(onthand) 2(twohand) 3(bow) 4(sheild) 5(armor)
            public bool isFemale;

            public ShapeToLibIndex(int shapeIdx, int libIndex, int weaponType, bool isFemale)
            {
                this.shapeIdx = shapeIdx;
                this.libIndex = libIndex;
                this.weaponType = weaponType;
                this.isFemale = isFemale;
            }
            override
            public string ToString()
            {
                return "ShapeIdx:" + shapeIdx + " libIndex:" + libIndex + " weaponType:" + weaponType + " isFemale:" + isFemale.ToString().ToLower();
            }
        }

        public List<ShapeToLibIndex> shapeToLibIndexs = new List<ShapeToLibIndex>();

        public WeaponMapperMgr(){
            shapeToLibIndexs.Clear();
            shapeToLibIndexs.Add(new ShapeToLibIndex(1, 0, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(2, 1, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(3, 2, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(4, 3, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(5, 4, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(6, 5, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(7, 6, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(8, 7, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(9, 8, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(10, 9, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(11, 10, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(18, 11, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(19, 12, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(20, 13, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(21, 14, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(22, 15, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(23, 16, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(24, 17, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(25, 18, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(26, 19, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(28, 20, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(29, 21, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(30, 22, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(50, 23, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(51, 24, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(53, 25, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(55, 26, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(57, 27, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(62, 28, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(63, 29, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(411, 30, 1, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(1, 0, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(2, 1, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(3, 2, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(4, 3, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(5, 4, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(6, 5, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(7, 6, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(8, 7, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(9, 8, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(10, 9, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(11, 10, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(18, 11, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(19, 12, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(20, 13, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(21, 14, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(22, 15, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(23, 16, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(24, 17, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(25, 18, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(26, 19, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(28, 20, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(29, 21, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(30, 22, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(50, 23, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(51, 24, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(53, 25, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(55, 26, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(57, 27, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(62, 28, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(63, 29, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(411, 30, 1, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(12, 0, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(13, 1, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(14, 2, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(15, 3, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(16, 4, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(33, 5, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(34, 6, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(35, 7, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(36, 8, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(37, 9, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(38, 10, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(39, 11, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(40, 12, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(41, 13, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(42, 14, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(43, 15, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(44, 16, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(48, 17, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(49, 18, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(52, 19, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(54, 20, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(56, 21, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(58, 22, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(59, 23, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(60, 24, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(61, 25, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(64, 26, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(65, 27, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(66, 28, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(68, 29, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(69, 30, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(70, 31, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(71, 32, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(385, 33, 2, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(12, 0, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(13, 1, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(14, 2, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(15, 3, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(16, 4, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(33, 5, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(34, 6, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(35, 7, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(36, 8, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(37, 9, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(38, 10, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(39, 11, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(40, 12, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(41, 13, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(42, 14, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(43, 15, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(44, 16, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(48, 17, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(49, 18, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(52, 19, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(54, 20, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(56, 21, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(58, 22, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(59, 23, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(60, 24, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(61, 25, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(64, 26, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(65, 27, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(66, 28, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(68, 29, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(69, 30, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(70, 31, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(71, 32, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(385, 33, 2, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(72, 0, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(73, 1, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(74, 2, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(75, 3, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(76, 4, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(77, 5, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(78, 6, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(79, 7, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(80, 8, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(81, 9, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(82, 10, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(83, 11, 3, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(72, 0, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(73, 1, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(74, 2, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(75, 3, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(76, 4, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(77, 5, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(78, 6, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(79, 7, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(80, 8, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(81, 9, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(82, 10, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(83, 11, 3, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(86, 0, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(87, 1, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(88, 2, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(89, 3, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(90, 4, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(91, 5, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(92, 6, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(93, 7, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(94, 8, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(95, 9, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(96, 10, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(420, 11, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(421, 12, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(422, 13, 4, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(86, 0, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(87, 1, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(88, 2, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(89, 3, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(90, 4, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(91, 5, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(92, 6, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(93, 7, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(94, 8, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(95, 9, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(96, 10, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(420, 11, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(421, 12, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(422, 13, 4, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(97, 1, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(98, 2, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(99, 3, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(100, 4, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(101, 5, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(102, 6, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(103, 7, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(104, 8, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(105, 9, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(106, 10, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(107, 11, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(108, 12, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(109, 13, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(110, 14, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(112, 15, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(113, 16, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(114, 17, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(115, 18, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(116, 19, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(117, 20, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(118, 21, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(427, 22, 5, false));

            shapeToLibIndexs.Add(new ShapeToLibIndex(97, 1, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(98, 2, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(99, 3, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(100, 4, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(101, 5, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(102, 6, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(103, 7, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(104, 8, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(105, 9, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(106, 10, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(107, 11, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(108, 12, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(109, 13, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(110, 14, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(112, 15, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(113, 16, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(114, 17, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(115, 18, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(116, 19, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(117, 20, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(118, 21, 5, true));

            shapeToLibIndexs.Add(new ShapeToLibIndex(427, 22, 5, true));
        }
        
        public ShapeToLibIndex GetShapeToLibIndexFromShapeIdx(int shapeIdx, bool isFemale)
        {
            foreach(var item in shapeToLibIndexs)
            {
                if(item.shapeIdx==shapeIdx && item.isFemale==isFemale)
                {
                    return item;
                }
            }
            return null;
        }
     
    }

    public static class Libraries
    {
        public static WeaponMapperMgr weaponMapperMgr = new WeaponMapperMgr();
        public static bool Loaded;
        public static int Count, Progress;
         
        public static MLibrary[] ExineManHair = new MLibrary[4];
        public static MLibrary[] ExineWomenHair = new MLibrary[4];

        public static MLibrary[] ExineManArmor = new MLibrary[23];
        public static MLibrary[] ExineWomenArmor = new MLibrary[23];

        public static MLibrary[] ExineManSheild = new MLibrary[14];
        public static MLibrary[] ExineWomenSheild = new MLibrary[14];
        

        public static MLibrary[] ExineManOneWeapon = new MLibrary[31];
        public static MLibrary[] ExineWomenOneWeapon = new MLibrary[31];

        public static MLibrary[] ExineManTwoWeapon = new MLibrary[34];
        public static MLibrary[] ExineWomenTwoWeapon = new MLibrary[34];

        public static MLibrary[] ExineManBowWeapon = new MLibrary[12];
        public static MLibrary[] ExineWomenBowWeapon = new MLibrary[12];


        public static MLibrary[] ExineNPCs = new MLibrary[36];
        public static MLibrary[] ExineMonsters = new MLibrary[73];

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
            CORVERICON = new MLibrary(Settings.ExineUIPath + "CoverIcon"),
            MINI_PORTRAIT = new MLibrary(Settings.ExineUIPath + "MiniPortrait"),
            ITEMSLOT = new MLibrary(Settings.ExineUIPath + "ItemSlot"),
            ServerButton = new MLibrary(Settings.ExineUIPath + "ServerButton"),
            ExineMiniMap = new MLibrary(Settings.ExineUIPath + "ExineMiniMap"),
            //ExineNPCPortrait = new MLibrary(Settings.ExineUIPath + "NPC"),
            ExineNPCPortrait = new MLibrary(Settings.ExineUIPath + "NPC_ZEROPOSITION"),

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
            BIK_024_Light_4 = new MLibrary(Settings.ExineVideoPath + "024-Light-4"),

            ExEffect00 = new MLibrary(Settings.ExineEffectPath + "Effect00"),
            ExEffect01 = new MLibrary(Settings.ExineEffectPath + "Effect01"),
            ExEffect02 = new MLibrary(Settings.ExineEffectPath + "Effect02"),
            ExEffect03 = new MLibrary(Settings.ExineEffectPath + "Effect03"),

            ActionTree = new MLibrary(Settings.ExineUIPath + "ActionTree"),
            MagicIcon = new MLibrary(Settings.ExineUIPath + "3-Magic"),
            ArtsIcon = new MLibrary(Settings.ExineUIPath + "4-Arts"),
            ManufactureSkillIcon = new MLibrary(Settings.ExineUIPath + "5-ManufactureSkill"),
            RingSkillIcon = new MLibrary(Settings.ExineUIPath + "6-RingSkill"),
            DivineSkillIcon = new MLibrary(Settings.ExineUIPath + "7-DivineSkill");
            
            
            



        #region Old
        public static readonly MLibrary
            
            Prguse = new MLibrary(Settings.DataPath + "Prguse"),
            Prguse2 = new MLibrary(Settings.DataPath + "Prguse2"),
            Prguse3 = new MLibrary(Settings.DataPath + "Prguse3"),
            BuffIcon = new MLibrary(Settings.DataPath + "BuffIcon"),
            Help = new MLibrary(Settings.DataPath + "Help"), 
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
            //Items = new MLibrary(Settings.DataPath + "Items"),
            //StateItems = new MLibrary(Settings.DataPath + "StateItem"),
            //FloorItems = new MLibrary(Settings.DataPath + "DNItems");
            Items = new MLibrary(Settings.ExineUIPath + "0-Item"),
            StateItems = new MLibrary(Settings.ExineUIPath + "1-EquippedItem"),
            FloorItems = new MLibrary(Settings.ExineUIPath + "2-DroppedItem");

        //Deco
        public static readonly MLibrary
            Deco = new MLibrary(Settings.DataPath + "Deco");



        public static MLibrary[]  CWeapons, CHumEffect, Monsters, Gates, Flags;
        #endregion Old

        static Libraries()
        {
            #region Old
            //Wiz/War/Tao 
            InitLibrary(ref CWeapons, Settings.CWeaponPath, "00"); 
            InitLibrary(ref CHumEffect, Settings.CHumEffectPath, "00");
              
            //Other
            InitLibrary(ref Monsters, Settings.MonsterPath, "000");
            InitLibrary(ref Gates, Settings.GatePath, "00");
            InitLibrary(ref Flags, Settings.FlagPath, "00");  
            #endregion Old
            
            #region Exine Human

            ExineManHair[0] = new MLibrary(Settings.ExHairPath + "HHM_0000_불꽃머리_reidx");
            ExineManHair[1] = new MLibrary(Settings.ExHairPath + "HHM_0001_말총머리_reidx");
            ExineManHair[2] = new MLibrary(Settings.ExHairPath + "HHM_0002_바람머리_reidx");
            ExineManHair[3] = new MLibrary(Settings.ExHairPath + "HHM_1000_대머리_reidx");

            ExineWomenHair[0] = new MLibrary(Settings.ExHairPath + "HHW_0001_세일러문머리_reidx");
            ExineWomenHair[1] = new MLibrary(Settings.ExHairPath + "HHW_0002_단발머리_reidx");
            ExineWomenHair[2] = new MLibrary(Settings.ExHairPath + "HHW_0002_올림묶음머리_reidx");
            ExineWomenHair[3] = new MLibrary(Settings.ExHairPath + "HHW_1000_대머리_reidx");

            ExineManArmor[0] = new MLibrary(Settings.ExArmorPath + "AHM_0000_평상복_reidx");
            ExineManArmor[1] = new MLibrary(Settings.ExArmorPath + "AHM_0001_퀼트아머_reidx");
            ExineManArmor[2] = new MLibrary(Settings.ExArmorPath + "AHM_0002_레더아머_reidx");
            ExineManArmor[3] = new MLibrary(Settings.ExArmorPath + "AHM_0003_스케일아머_reidx");
            ExineManArmor[4] = new MLibrary(Settings.ExArmorPath + "AHM_0004_브리간딘_reidx");
            ExineManArmor[5] = new MLibrary(Settings.ExArmorPath + "AHM_0005_브레스트플레이트_reidx");
            ExineManArmor[6] = new MLibrary(Settings.ExArmorPath + "AHM_0006_하프플레이트_reidx");
            ExineManArmor[7] = new MLibrary(Settings.ExArmorPath + "AHM_0007_체인메일_reidx");
            ExineManArmor[8] = new MLibrary(Settings.ExArmorPath + "AHM_0008_오베르_reidx");
            ExineManArmor[9] = new MLibrary(Settings.ExArmorPath + "AHM_0009_본아머_reidx");
            ExineManArmor[10] = new MLibrary(Settings.ExArmorPath + "AHM_0010_플레이트메일_reidx");
            ExineManArmor[11] = new MLibrary(Settings.ExArmorPath + "AHM_0011_컴포지트아머_reidx");
            ExineManArmor[12] = new MLibrary(Settings.ExArmorPath + "AHM_0012_플레이트아머_reidx");
            ExineManArmor[13] = new MLibrary(Settings.ExArmorPath + "AHM_0013_풀플레이트메일_reidx");
            ExineManArmor[14] = new MLibrary(Settings.ExArmorPath + "AHM_0014_플루티드아머_reidx");
            ExineManArmor[15] = new MLibrary(Settings.ExArmorPath + "AHM_0016_로브_reidx");
            ExineManArmor[16] = new MLibrary(Settings.ExArmorPath + "AHM_0017_수탄_reidx");
            ExineManArmor[17] = new MLibrary(Settings.ExArmorPath + "AHM_0018_실크로브_reidx");
            ExineManArmor[18] = new MLibrary(Settings.ExArmorPath + "AHM_0019_가운_reidx");
            ExineManArmor[19] = new MLibrary(Settings.ExArmorPath + "AHM_0020_브로케이드_reidx");
            ExineManArmor[20] = new MLibrary(Settings.ExArmorPath + "AHM_0021_달마티카_reidx");
            ExineManArmor[21] = new MLibrary(Settings.ExArmorPath + "AHM_0022_팔리움_reidx");
            ExineManArmor[22] = new MLibrary(Settings.ExArmorPath + "AHM_1001_이벤트복01_reidx");

            ExineWomenArmor[0] = new MLibrary(Settings.ExArmorPath + "AHW_0000_평상복_reidx");
            ExineWomenArmor[1] = new MLibrary(Settings.ExArmorPath + "AHW_0001_퀼트아머_reidx");
            ExineWomenArmor[2] = new MLibrary(Settings.ExArmorPath + "AHW_0002_레더아머_reidx");
            ExineWomenArmor[3] = new MLibrary(Settings.ExArmorPath + "AHW_0003_스케일아머_reidx");
            ExineWomenArmor[4] = new MLibrary(Settings.ExArmorPath + "AHW_0004_브리간딘_reidx");
            ExineWomenArmor[5] = new MLibrary(Settings.ExArmorPath + "AHW_0005_브레스트플레이트_reidx");
            ExineWomenArmor[6] = new MLibrary(Settings.ExArmorPath + "AHW_0006_하프플레이트_reidx");
            ExineWomenArmor[7] = new MLibrary(Settings.ExArmorPath + "AHW_0007_체인메일_reidx");
            ExineWomenArmor[8] = new MLibrary(Settings.ExArmorPath + "AHW_0008_오베르_reidx");
            ExineWomenArmor[9] = new MLibrary(Settings.ExArmorPath + "AHW_0009_본아머_reidx");
            ExineWomenArmor[10] = new MLibrary(Settings.ExArmorPath + "AHW_0010_플레이트메일_reidx");
            ExineWomenArmor[11] = new MLibrary(Settings.ExArmorPath + "AHW_0011_컴포지트아머_reidx");
            ExineWomenArmor[12] = new MLibrary(Settings.ExArmorPath + "AHW_0012_플레이트아머_reidx");
            ExineWomenArmor[13] = new MLibrary(Settings.ExArmorPath + "AHW_0013_풀플레이트메일_reidx");
            ExineWomenArmor[14] = new MLibrary(Settings.ExArmorPath + "AHW_0014_플루티드아머_reidx");
            ExineWomenArmor[15] = new MLibrary(Settings.ExArmorPath + "AHW_0016_로브_reidx");
            ExineWomenArmor[16] = new MLibrary(Settings.ExArmorPath + "AHW_0017_수탄_reidx");
            ExineWomenArmor[17] = new MLibrary(Settings.ExArmorPath + "AHW_0018_실크로브_reidx");
            ExineWomenArmor[18] = new MLibrary(Settings.ExArmorPath + "AHW_0019_가운_reidx");
            ExineWomenArmor[19] = new MLibrary(Settings.ExArmorPath + "AHW_0020_브로케이드_reidx");
            ExineWomenArmor[20] = new MLibrary(Settings.ExArmorPath + "AHW_0021_달마티카_reidx");
            ExineWomenArmor[21] = new MLibrary(Settings.ExArmorPath + "AHW_0022_팔리움_reidx");
            ExineWomenArmor[22] = new MLibrary(Settings.ExArmorPath + "AHW_1001_이벤트복01_reidx");
             
            ExineManSheild[0] = new MLibrary(Settings.ExSheildPath + "SHM_0001_(방)_나무방패_reidx");
            ExineManSheild[1] = new MLibrary(Settings.ExSheildPath + "SHM_0002_(방)_버클러_reidx");
            ExineManSheild[2] = new MLibrary(Settings.ExSheildPath + "SHM_0003_(방)_아스피스_reidx");
            ExineManSheild[3] = new MLibrary(Settings.ExSheildPath + "SHM_0004_(방)_호플론_reidx");
            ExineManSheild[4] = new MLibrary(Settings.ExSheildPath + "SHM_0005_(방)_본쉴드_reidx");
            ExineManSheild[5] = new MLibrary(Settings.ExSheildPath + "SHM_0006_(방)_청동카이트_reidx");
            ExineManSheild[6] = new MLibrary(Settings.ExSheildPath + "SHM_0007_(방)_카이트쉴드_reidx");
            ExineManSheild[7] = new MLibrary(Settings.ExSheildPath + "SHM_0008_(방)_브론즈파비스_reidx");
            ExineManSheild[8] = new MLibrary(Settings.ExSheildPath + "SHM_0009_(방)_파비스_reidx");
            ExineManSheild[9] = new MLibrary(Settings.ExSheildPath + "SHM_0010_(방)_엘다라크_reidx");
            ExineManSheild[10] = new MLibrary(Settings.ExSheildPath + "SHM_0011_(방)_스큐톰_reidx");
            ExineManSheild[11] = new MLibrary(Settings.ExSheildPath + "SHM_0012_(방)_둥근풍선_reidx");
            ExineManSheild[12] = new MLibrary(Settings.ExSheildPath + "SHM_0013_(방)_별풍선_reidx");
            ExineManSheild[13] = new MLibrary(Settings.ExSheildPath + "SHM_0014_(방)_하트풍선_reidx");

            ExineWomenSheild[0] = new MLibrary(Settings.ExSheildPath + "SHW_0001_(방)_나무방패_reidx");
            ExineWomenSheild[1] = new MLibrary(Settings.ExSheildPath + "SHW_0002_(방)_버클러_reidx");
            ExineWomenSheild[2] = new MLibrary(Settings.ExSheildPath + "SHW_0003_(방)_아스피스_reidx");
            ExineWomenSheild[3] = new MLibrary(Settings.ExSheildPath + "SHW_0004_(방)_호플론_reidx");
            ExineWomenSheild[4] = new MLibrary(Settings.ExSheildPath + "SHW_0005_(방)_본쉴드_reidx");
            ExineWomenSheild[5] = new MLibrary(Settings.ExSheildPath + "SHW_0006_(방)_청동카이트_reidx");
            ExineWomenSheild[6] = new MLibrary(Settings.ExSheildPath + "SHW_0007_(방)_카이트쉴드_reidx");
            ExineWomenSheild[7] = new MLibrary(Settings.ExSheildPath + "SHW_0008_(방)_브론즈파비스_reidx");
            ExineWomenSheild[8] = new MLibrary(Settings.ExSheildPath + "SHW_0009_(방)_파비스_reidx");
            ExineWomenSheild[9] = new MLibrary(Settings.ExSheildPath + "SHW_0010_(방)_엘다라크_reidx");
            ExineWomenSheild[10] = new MLibrary(Settings.ExSheildPath + "SHW_0011_(방)_스큐톰_reidx");
            ExineWomenSheild[11] = new MLibrary(Settings.ExSheildPath + "SHW_0012_(방)_둥근풍선_reidx");
            ExineWomenSheild[12] = new MLibrary(Settings.ExSheildPath + "SHW_0013_(방)_별풍선_reidx");
            ExineWomenSheild[13] = new MLibrary(Settings.ExSheildPath + "SHW_0014_(방)_하트풍선_reidx");


            ExineManOneWeapon[0] = new MLibrary(Settings.ExOneHandPath + "WHM_1001_(원)_숏소드_reidx");
            ExineManOneWeapon[1] = new MLibrary(Settings.ExOneHandPath + "WHM_1002_(원)_스몰소드_reidx");
            ExineManOneWeapon[2] = new MLibrary(Settings.ExOneHandPath + "WHM_1003_(원)_글래스소드_reidx");
            ExineManOneWeapon[3] = new MLibrary(Settings.ExOneHandPath + "WHM_1004_(원)_롱소드_reidx");
            ExineManOneWeapon[4] = new MLibrary(Settings.ExOneHandPath + "WHM_1005_(원)_샴쉬르_reidx");
            ExineManOneWeapon[5] = new MLibrary(Settings.ExOneHandPath + "WHM_1006_(원)_세이버_reidx");
            ExineManOneWeapon[6] = new MLibrary(Settings.ExOneHandPath + "WHM_1007_(원)_바스타드소드_reidx");
            ExineManOneWeapon[7] = new MLibrary(Settings.ExOneHandPath + "WHM_1008_(원)_레이피어_reidx");
            ExineManOneWeapon[8] = new MLibrary(Settings.ExOneHandPath + "WHM_1009_(원)_글라디우스_reidx");
            ExineManOneWeapon[9] = new MLibrary(Settings.ExOneHandPath + "WHM_1010_(원)_다마스커스_reidx");
            ExineManOneWeapon[10] = new MLibrary(Settings.ExOneHandPath + "WHM_1011_(원)_파타_reidx");
            ExineManOneWeapon[11] = new MLibrary(Settings.ExOneHandPath + "WHM_1012_(원)_대거_reidx");
            ExineManOneWeapon[12] = new MLibrary(Settings.ExOneHandPath + "WHM_1013_(원)_발럭나이프_reidx");
            ExineManOneWeapon[13] = new MLibrary(Settings.ExOneHandPath + "WHM_1014_(원)_배즐러드_reidx");
            ExineManOneWeapon[14] = new MLibrary(Settings.ExOneHandPath + "WHM_1015_(원)_더크_reidx");
            ExineManOneWeapon[15] = new MLibrary(Settings.ExOneHandPath + "WHM_1016_(원)_헌팅나이프_reidx");
            ExineManOneWeapon[16] = new MLibrary(Settings.ExOneHandPath + "WHM_1017_(원)_크리스_reidx");
            ExineManOneWeapon[17] = new MLibrary(Settings.ExOneHandPath + "WHM_1018_(원)_쿠크리_reidx");
            ExineManOneWeapon[18] = new MLibrary(Settings.ExOneHandPath + "WHM_1019_(원)_맹고슈_reidx");
            ExineManOneWeapon[19] = new MLibrary(Settings.ExOneHandPath + "WHM_1020_(원)_스틸레토_reidx");
            ExineManOneWeapon[20] = new MLibrary(Settings.ExOneHandPath + "WHM_1022_(원)_다트_reidx");
            ExineManOneWeapon[21] = new MLibrary(Settings.ExOneHandPath + "WHM_1023_(원)_아자가이_reidx");
            ExineManOneWeapon[22] = new MLibrary(Settings.ExOneHandPath + "WHM_1024_(원)_챠크람_reidx");
            ExineManOneWeapon[23] = new MLibrary(Settings.ExOneHandPath + "WHM_1027_(원)_완드_reidx");
            ExineManOneWeapon[24] = new MLibrary(Settings.ExOneHandPath + "WHM_1028_(원)_바통_reidx");
            ExineManOneWeapon[25] = new MLibrary(Settings.ExOneHandPath + "WHM_1029_(원)_숏스태프_reidx");
            ExineManOneWeapon[26] = new MLibrary(Settings.ExOneHandPath + "WHM_1030_(원)_본스태프_reidx");
            ExineManOneWeapon[27] = new MLibrary(Settings.ExOneHandPath + "WHM_1031_(원)_클럽_reidx");
            ExineManOneWeapon[28] = new MLibrary(Settings.ExOneHandPath + "WHM_1032_(원)_핸드액스_reidx");
            ExineManOneWeapon[29] = new MLibrary(Settings.ExOneHandPath + "WHM_1033_(원)_액스_reidx");
            ExineManOneWeapon[30] = new MLibrary(Settings.ExOneHandPath + "WHM_1034_(원)_태극기_reidx");


            ExineWomenOneWeapon[0] = new MLibrary(Settings.ExOneHandPath + "WHW_1001_(원)_숏소드_reidx");
            ExineWomenOneWeapon[1] = new MLibrary(Settings.ExOneHandPath + "WHW_1002_(원)_스몰소드_reidx");
            ExineWomenOneWeapon[2] = new MLibrary(Settings.ExOneHandPath + "WHW_1003_(원)_글래스소드_reidx");
            ExineWomenOneWeapon[3] = new MLibrary(Settings.ExOneHandPath + "WHW_1004_(원)_롱소드_reidx");
            ExineWomenOneWeapon[4] = new MLibrary(Settings.ExOneHandPath + "WHW_1005_(원)_샴쉬르_reidx");
            ExineWomenOneWeapon[5] = new MLibrary(Settings.ExOneHandPath + "WHW_1006_(원)_세이버_reidx");
            ExineWomenOneWeapon[6] = new MLibrary(Settings.ExOneHandPath + "WHW_1007_(원)_바스타드소드_reidx");
            ExineWomenOneWeapon[7] = new MLibrary(Settings.ExOneHandPath + "WHW_1008_(원)_레이피어_reidx");
            ExineWomenOneWeapon[8] = new MLibrary(Settings.ExOneHandPath + "WHW_1009_(원)_글라디우스_reidx");
            ExineWomenOneWeapon[9] = new MLibrary(Settings.ExOneHandPath + "WHW_1010_(원)_다마스커스_reidx");
            ExineWomenOneWeapon[10] = new MLibrary(Settings.ExOneHandPath + "WHW_1011_(원)_파타_reidx");
            ExineWomenOneWeapon[11] = new MLibrary(Settings.ExOneHandPath + "WHW_1012_(원)_대거_reidx");
            ExineWomenOneWeapon[12] = new MLibrary(Settings.ExOneHandPath + "WHW_1013_(원)_발럭나이프_reidx");
            ExineWomenOneWeapon[13] = new MLibrary(Settings.ExOneHandPath + "WHW_1014_(원)_배즐러드_reidx");
            ExineWomenOneWeapon[14] = new MLibrary(Settings.ExOneHandPath + "WHW_1015_(원)_더크_reidx");
            ExineWomenOneWeapon[15] = new MLibrary(Settings.ExOneHandPath + "WHW_1016_(원)_헌팅나이프_reidx");
            ExineWomenOneWeapon[16] = new MLibrary(Settings.ExOneHandPath + "WHW_1017_(원)_크리스_reidx");
            ExineWomenOneWeapon[17] = new MLibrary(Settings.ExOneHandPath + "WHW_1018_(원)_쿠크리_reidx");
            ExineWomenOneWeapon[18] = new MLibrary(Settings.ExOneHandPath + "WHW_1019_(원)_맹고슈_reidx");
            ExineWomenOneWeapon[19] = new MLibrary(Settings.ExOneHandPath + "WHW_1020_(원)_스틸레토_reidx");
            ExineWomenOneWeapon[20] = new MLibrary(Settings.ExOneHandPath + "WHW_1022_(원)_다트_reidx");
            ExineWomenOneWeapon[21] = new MLibrary(Settings.ExOneHandPath + "WHW_1023_(원)_아자가이_reidx");
            ExineWomenOneWeapon[22] = new MLibrary(Settings.ExOneHandPath + "WHW_1024_(원)_챠크람_reidx");
            ExineWomenOneWeapon[23] = new MLibrary(Settings.ExOneHandPath + "WHW_1027_(원)_완드_reidx");
            ExineWomenOneWeapon[24] = new MLibrary(Settings.ExOneHandPath + "WHW_1028_(원)_바통_reidx");
            ExineWomenOneWeapon[25] = new MLibrary(Settings.ExOneHandPath + "WHW_1029_(원)_숏스태프_reidx");
            ExineWomenOneWeapon[26] = new MLibrary(Settings.ExOneHandPath + "WHW_1030_(원)_본스태프_reidx");
            ExineWomenOneWeapon[27] = new MLibrary(Settings.ExOneHandPath + "WHW_1031_(원)_클럽_reidx");
            ExineWomenOneWeapon[28] = new MLibrary(Settings.ExOneHandPath + "WHW_1032_(원)_핸드액스_reidx");
            ExineWomenOneWeapon[29] = new MLibrary(Settings.ExOneHandPath + "WHW_1033_(원)_액스_reidx");
            ExineWomenOneWeapon[30] = new MLibrary(Settings.ExOneHandPath + "WHW_1034_(원)_태극기_reidx");

            ExineManTwoWeapon[0] = new MLibrary(Settings.ExTwoHandPath + "WHM_2001_(투)_투핸드소드_reidx");
            ExineManTwoWeapon[1] = new MLibrary(Settings.ExTwoHandPath + "WHM_2002_(투)_크루세이더_reidx");
            ExineManTwoWeapon[2] = new MLibrary(Settings.ExTwoHandPath + "WHM_2003_(투)_클레이모어_reidx");
            ExineManTwoWeapon[3] = new MLibrary(Settings.ExTwoHandPath + "WHM_2004_(투)_플람베르그_reidx");
            ExineManTwoWeapon[4] = new MLibrary(Settings.ExTwoHandPath + "WHM_2005_(투)_터크_reidx");
            ExineManTwoWeapon[5] = new MLibrary(Settings.ExTwoHandPath + "WHM_2007_(투)_숏스피어_reidx");
            ExineManTwoWeapon[6] = new MLibrary(Settings.ExTwoHandPath + "WHM_2008_(투)_롱스피어_reidx");
            ExineManTwoWeapon[7] = new MLibrary(Settings.ExTwoHandPath + "WHM_2009_(투)_트라이던트_reidx");
            ExineManTwoWeapon[8] = new MLibrary(Settings.ExTwoHandPath + "WHM_2010_(투)_아스타_reidx");
            ExineManTwoWeapon[9] = new MLibrary(Settings.ExTwoHandPath + "WHM_2011_(투)_파이크_reidx");
            ExineManTwoWeapon[10] = new MLibrary(Settings.ExTwoHandPath + "WHM_2012_(투)_올파이크_reidx");
            ExineManTwoWeapon[11] = new MLibrary(Settings.ExTwoHandPath + "WHM_2013_(투)_할베르트_reidx");
            ExineManTwoWeapon[12] = new MLibrary(Settings.ExTwoHandPath + "WHM_2014_(투)_부주_reidx");
            ExineManTwoWeapon[13] = new MLibrary(Settings.ExTwoHandPath + "WHM_2015_(투)_글레이브_reidx");
            ExineManTwoWeapon[14] = new MLibrary(Settings.ExTwoHandPath + "WHM_2016_(투)_사이드_reidx");
            ExineManTwoWeapon[15] = new MLibrary(Settings.ExTwoHandPath + "WHM_2017_(투)_귀자르므_reidx");
            ExineManTwoWeapon[16] = new MLibrary(Settings.ExTwoHandPath + "WHM_2018_(투)_버디슈_reidx");
            ExineManTwoWeapon[17] = new MLibrary(Settings.ExTwoHandPath + "WHM_2022_(투)_지팡이_reidx");
            ExineManTwoWeapon[18] = new MLibrary(Settings.ExTwoHandPath + "WHM_2023_(투)_클로저_reidx");
            ExineManTwoWeapon[19] = new MLibrary(Settings.ExTwoHandPath + "WHM_2024_(투)_스톤완드_reidx");
            ExineManTwoWeapon[20] = new MLibrary(Settings.ExTwoHandPath + "WHM_2025_(투)_롱스태프_reidx");
            ExineManTwoWeapon[21] = new MLibrary(Settings.ExTwoHandPath + "WHM_2026_(투)_오브_reidx");
            ExineManTwoWeapon[22] = new MLibrary(Settings.ExTwoHandPath + "WHM_2027_(투)_스파이크클럽_reidx");
            ExineManTwoWeapon[23] = new MLibrary(Settings.ExTwoHandPath + "WHM_2028_(투)_구르즈_reidx");
            ExineManTwoWeapon[24] = new MLibrary(Settings.ExTwoHandPath + "WHM_2029_(투)_메이스_reidx");
            ExineManTwoWeapon[25] = new MLibrary(Settings.ExTwoHandPath + "WHM_2030_(투)_모닝스타_reidx");
            ExineManTwoWeapon[26] = new MLibrary(Settings.ExTwoHandPath + "WHM_2031_(투)_브로드액스_reidx");
            ExineManTwoWeapon[27] = new MLibrary(Settings.ExTwoHandPath + "WHM_2032_(투)_더블액스_reidx");
            ExineManTwoWeapon[28] = new MLibrary(Settings.ExTwoHandPath + "WHM_2033_(투)_자이언트액스_reidx");
            ExineManTwoWeapon[29] = new MLibrary(Settings.ExTwoHandPath + "WHM_2035_(투)_해머_reidx");
            ExineManTwoWeapon[30] = new MLibrary(Settings.ExTwoHandPath + "WHM_2036_(투)_워해머_reidx");
            ExineManTwoWeapon[31] = new MLibrary(Settings.ExTwoHandPath + "WHM_2037_(투)_워피크_reidx");
            ExineManTwoWeapon[32] = new MLibrary(Settings.ExTwoHandPath + "WHM_2038_(투)_말렛_reidx");
            ExineManTwoWeapon[33] = new MLibrary(Settings.ExTwoHandPath + "WHM_2039_(투)_장미_reidx");

            ExineWomenTwoWeapon[0] = new MLibrary(Settings.ExTwoHandPath + "WHW_2001_(투)_투핸드소드_reidx");
            ExineWomenTwoWeapon[1] = new MLibrary(Settings.ExTwoHandPath + "WHW_2002_(투)_크루세이더_reidx");
            ExineWomenTwoWeapon[2] = new MLibrary(Settings.ExTwoHandPath + "WHW_2003_(투)_클레이모어_reidx");
            ExineWomenTwoWeapon[3] = new MLibrary(Settings.ExTwoHandPath + "WHW_2004_(투)_플람베르그_reidx");
            ExineWomenTwoWeapon[4] = new MLibrary(Settings.ExTwoHandPath + "WHW_2005_(투)_터크_reidx");
            ExineWomenTwoWeapon[5] = new MLibrary(Settings.ExTwoHandPath + "WHW_2007_(투)_숏스피어_reidx");
            ExineWomenTwoWeapon[6] = new MLibrary(Settings.ExTwoHandPath + "WHW_2008_(투)_롱스피어_reidx");
            ExineWomenTwoWeapon[7] = new MLibrary(Settings.ExTwoHandPath + "WHW_2009_(투)_트라이던트_reidx");
            ExineWomenTwoWeapon[8] = new MLibrary(Settings.ExTwoHandPath + "WHW_2010_(투)_아스타_reidx");
            ExineWomenTwoWeapon[9] = new MLibrary(Settings.ExTwoHandPath + "WHW_2011_(투)_파이크_reidx");
            ExineWomenTwoWeapon[10] = new MLibrary(Settings.ExTwoHandPath + "WHW_2012_(투)_올파이크_reidx");
            ExineWomenTwoWeapon[11] = new MLibrary(Settings.ExTwoHandPath + "WHW_2013_(투)_할베르트_reidx");
            ExineWomenTwoWeapon[12] = new MLibrary(Settings.ExTwoHandPath + "WHW_2014_(투)_부주_reidx");
            ExineWomenTwoWeapon[13] = new MLibrary(Settings.ExTwoHandPath + "WHW_2015_(투)_글레이브_reidx");
            ExineWomenTwoWeapon[14] = new MLibrary(Settings.ExTwoHandPath + "WHW_2016_(투)_사이드_reidx");
            ExineWomenTwoWeapon[15] = new MLibrary(Settings.ExTwoHandPath + "WHW_2017_(투)_귀자르므_reidx");
            ExineWomenTwoWeapon[16] = new MLibrary(Settings.ExTwoHandPath + "WHW_2018_(투)_버디슈_reidx");
            ExineWomenTwoWeapon[17] = new MLibrary(Settings.ExTwoHandPath + "WHW_2022_(투)_지팡이_reidx");
            ExineWomenTwoWeapon[18] = new MLibrary(Settings.ExTwoHandPath + "WHW_2023_(투)_클로저_reidx");
            ExineWomenTwoWeapon[19] = new MLibrary(Settings.ExTwoHandPath + "WHW_2024_(투)_스톤완드_reidx");
            ExineWomenTwoWeapon[20] = new MLibrary(Settings.ExTwoHandPath + "WHW_2025_(투)_롱스태프_reidx");
            ExineWomenTwoWeapon[21] = new MLibrary(Settings.ExTwoHandPath + "WHW_2026_(투)_오브_reidx");
            ExineWomenTwoWeapon[22] = new MLibrary(Settings.ExTwoHandPath + "WHW_2027_(투)_스파이크클럽_reidx");
            ExineWomenTwoWeapon[23] = new MLibrary(Settings.ExTwoHandPath + "WHW_2028_(투)_구르즈_reidx");
            ExineWomenTwoWeapon[24] = new MLibrary(Settings.ExTwoHandPath + "WHW_2029_(투)_메이스_reidx");
            ExineWomenTwoWeapon[25] = new MLibrary(Settings.ExTwoHandPath + "WHW_2030_(투)_모닝스타_reidx");
            ExineWomenTwoWeapon[26] = new MLibrary(Settings.ExTwoHandPath + "WHW_2031_(투)_브로드액스_reidx");
            ExineWomenTwoWeapon[27] = new MLibrary(Settings.ExTwoHandPath + "WHW_2032_(투)_더블액스_reidx");
            ExineWomenTwoWeapon[28] = new MLibrary(Settings.ExTwoHandPath + "WHW_2033_(투)_자이언트액스_reidx");
            ExineWomenTwoWeapon[29] = new MLibrary(Settings.ExTwoHandPath + "WHW_2035_(투)_해머_reidx");
            ExineWomenTwoWeapon[30] = new MLibrary(Settings.ExTwoHandPath + "WHW_2036_(투)_워해머_reidx");
            ExineWomenTwoWeapon[31] = new MLibrary(Settings.ExTwoHandPath + "WHW_2037_(투)_워피크_reidx");
            ExineWomenTwoWeapon[32] = new MLibrary(Settings.ExTwoHandPath + "WHW_2038_(투)_말렛_reidx");
            ExineWomenTwoWeapon[33] = new MLibrary(Settings.ExTwoHandPath + "WHW_2039_(투)_장미_reidx");

            ExineManBowWeapon[0] = new MLibrary(Settings.ExBowPath + "WHM_3001_(활)_숏보우_reidx");
            ExineManBowWeapon[1] = new MLibrary(Settings.ExBowPath + "WHM_3002_(활)_보우_reidx");
            ExineManBowWeapon[2] = new MLibrary(Settings.ExBowPath + "WHM_3003_(활)_가스트라페테_reidx");
            ExineManBowWeapon[3] = new MLibrary(Settings.ExBowPath + "WHM_3004_(활)_롱보우_reidx");
            ExineManBowWeapon[4] = new MLibrary(Settings.ExBowPath + "WHM_3005_(활)_헌터보우_reidx");
            ExineManBowWeapon[5] = new MLibrary(Settings.ExBowPath + "WHM_3006_(활)_헤비헌터보우_reidx");
            ExineManBowWeapon[6] = new MLibrary(Settings.ExBowPath + "WHM_3007_(활)_크로스보우_reidx");
            ExineManBowWeapon[7] = new MLibrary(Settings.ExBowPath + "WHM_3008_(활)_더블보우_reidx");
            ExineManBowWeapon[8] = new MLibrary(Settings.ExBowPath + "WHM_3009_(활)_컴포지트보우_reidx");
            ExineManBowWeapon[9] = new MLibrary(Settings.ExBowPath + "WHM_3010_(활)_아발리스트_reidx");
            ExineManBowWeapon[10] = new MLibrary(Settings.ExBowPath + "WHM_3011_(활)_배틀보우_reidx");
            ExineManBowWeapon[11] = new MLibrary(Settings.ExBowPath + "WHM_3012_(활)_나이트보우_reidx");

            ExineWomenBowWeapon[0] = new MLibrary(Settings.ExBowPath + "WHW_3001_(활)_숏보우_reidx");
            ExineWomenBowWeapon[1] = new MLibrary(Settings.ExBowPath + "WHW_3002_(활)_보우_reidx");
            ExineWomenBowWeapon[2] = new MLibrary(Settings.ExBowPath + "WHW_3003_(활)_가스트라페테_reidx");
            ExineWomenBowWeapon[3] = new MLibrary(Settings.ExBowPath + "WHW_3004_(활)_롱보우_reidx");
            ExineWomenBowWeapon[4] = new MLibrary(Settings.ExBowPath + "WHW_3005_(활)_헌터보우_reidx");
            ExineWomenBowWeapon[5] = new MLibrary(Settings.ExBowPath + "WHW_3006_(활)_헤비헌터보우_reidx");
            ExineWomenBowWeapon[6] = new MLibrary(Settings.ExBowPath + "WHW_3007_(활)_크로스보우_reidx");
            ExineWomenBowWeapon[7] = new MLibrary(Settings.ExBowPath + "WHW_3008_(활)_더블보우_reidx");
            ExineWomenBowWeapon[8] = new MLibrary(Settings.ExBowPath + "WHW_3009_(활)_컴포지트보우_reidx");
            ExineWomenBowWeapon[9] = new MLibrary(Settings.ExBowPath + "WHW_3010_(활)_아발리스트_reidx");
            ExineWomenBowWeapon[10] = new MLibrary(Settings.ExBowPath + "WHW_3011_(활)_배틀보우_reidx");
            ExineWomenBowWeapon[11] = new MLibrary(Settings.ExBowPath + "WHW_3012_(활)_나이트보우_reidx");

            #endregion  Exine Human
             
            #region Exine Maplibs

            for (int i = 0; i < 10; i++)
            {
                MapLibs[i] = new MLibrary(Settings.DataPath + "Map\\Exine\\TS_0" + i + "_Tile"); 
            }

            for (int i = 0; i < 10; i++)
            {
                MapLibs[i + 10] = new MLibrary(Settings.DataPath + "Map\\Exine\\TS_0" + i + "_Static"); 
            }

            MapLibs[20] = new MLibrary(Settings.DataPath + "Map\\Exine\\Smtiles"); 

            //object tile
            for (int i = 1000; i < 2000; i++)
            {
                if (File.Exists(Settings.DataPath + "Map\\Exine\\Map_" + (9000 + i) + "_FrontTile.lib"))
                {
                    MapLibs[i] = new MLibrary(Settings.DataPath + "Map\\Exine\\Map_" + (9000 + i) + "_FrontTile"); 
                }
            }

            #endregion Exine Maplibs

            #region ExineNPC
            ExineNPCs[0] = new MLibrary(Settings.ExineNPCPath + "empty");
            ExineNPCs[1] = new MLibrary(Settings.ExineNPCPath + "ZM_01_256_영주01_reidx");//베르제스 영주
            ExineNPCs[2] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//잭 주점
            ExineNPCs[3] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//???
            ExineNPCs[4] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//랄크 감정
            ExineNPCs[5] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//아론 신관
            ExineNPCs[6] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//레블리 신관
            ExineNPCs[7] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//오토 신관
            ExineNPCs[8] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//오스틴 격장관리
            ExineNPCs[9] = new MLibrary(Settings.ExineNPCPath + "ZM_01_259_대장장이01_reidx");//마이트 대장장
            ExineNPCs[10] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//오스틴 격장관리
            ExineNPCs[11] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//로디 병사
            ExineNPCs[12] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//레이스 병사
            ExineNPCs[13] = new MLibrary(Settings.ExineNPCPath + "ZM_01_261_꼬마남01_reidx");//텔레포터(남)
            ExineNPCs[14] = new MLibrary(Settings.ExineNPCPath + "ZM_01_262_장인01_reidx");//올랜드 가구
            ExineNPCs[15] = new MLibrary(Settings.ExineNPCPath + "empty");
            ExineNPCs[16] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//hard loader
            ExineNPCs[17] = new MLibrary(Settings.ExineNPCPath + "ZM_02_512_여관주인01_reidx");//실버레인 여관
            ExineNPCs[18] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//???
            ExineNPCs[19] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//그레이스 잡화
            ExineNPCs[20] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//luilui
            ExineNPCs[21] = new MLibrary(Settings.ExineNPCPath + "ZM_02_514_주점주인01_reidx");//마틸다 주점
            ExineNPCs[22] = new MLibrary(Settings.ExineNPCPath + "ZM_02_515_꼬마여01_reidx");//텔레포터(여)
            ExineNPCs[23] = new MLibrary(Settings.ExineNPCPath + "ZM_02_516_재단사01_reidx");//재단사 루피아
            ExineNPCs[24] = new MLibrary(Settings.ExineNPCPath + "ZM_02_517_마법상인01_reidx");//일랜 시약
            ExineNPCs[25] = new MLibrary(Settings.ExineNPCPath + "ZM_02_518_악세사리상인01_reidx");//티파니 악세
            ExineNPCs[26] = new MLibrary(Settings.ExineNPCPath + "ZM_02_519_요정01_reidx");//요정
            ExineNPCs[27] = new MLibrary(Settings.ExineNPCPath + "empty");
            ExineNPCs[28] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//안토니오 재단
            ExineNPCs[29] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//아니타 무기
            ExineNPCs[30] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//bank man
            ExineNPCs[31] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//bank woman
            ExineNPCs[32] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//hair man
            ExineNPCs[33] = new MLibrary(Settings.ExineNPCPath + "bronze_statue");//용사동
            /*
            ExineNPCs[0] = new MLibrary(Settings.ExineNPCPath + "ZM_02_517_마법상인01_reidx");//일랜 시약
            ExineNPCs[1] = new MLibrary(Settings.ExineNPCPath + "ZM_02_517_마법상인01_reidx");//다이나 시약
            ExineNPCs[2] = new MLibrary(Settings.ExineNPCPath + "ZM_02_512_여관주인01_reidx");//실버레인 여관
            ExineNPCs[3] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//안토니오 재단
            ExineNPCs[4] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//그레이스 잡화
            ExineNPCs[5] = new MLibrary(Settings.ExineNPCPath + "ZM_01_259_대장장이01_reidx");//마이트 대장장
            ExineNPCs[6] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//아니타 무기
            ExineNPCs[7] = new MLibrary(Settings.ExineNPCPath + "ZM_02_514_주점주인01_reidx");//마틸다 주점
            ExineNPCs[8] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//잭 주점
            ExineNPCs[9] = new MLibrary(Settings.ExineNPCPath + "ZM_02_518_악세사리상인01_reidx");//티파니 악세
            ExineNPCs[10] = new MLibrary(Settings.ExineNPCPath + "ZM_01_262_장인01_reidx");//올랜드 가구
            ExineNPCs[11] = new MLibrary(Settings.ExineNPCPath + "ZM_01_256_영주01_reidx");//베르제스 영주
            ExineNPCs[12] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//아론 신관
            ExineNPCs[13] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//아론 레블리
            ExineNPCs[14] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//아론 오토
            ExineNPCs[15] = new MLibrary(Settings.ExineNPCPath + "ZM_01_258_성인남02_reidx");//아론 마케르
            ExineNPCs[16] = new MLibrary(Settings.ExineNPCPath + "ZM_02_516_재단사01_reidx");//재단사 루피아
            ExineNPCs[17] = new MLibrary(Settings.ExineNPCPath + "ZM_02_517_마법상인01_reidx");//마리 시약
            ExineNPCs[18] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//랄크 감정
            ExineNPCs[19] = new MLibrary(Settings.ExineNPCPath + "ZM_01_261_꼬마남01_reidx");//텔레포터(남)
            ExineNPCs[20] = new MLibrary(Settings.ExineNPCPath + "ZM_02_515_꼬마여01_reidx");//텔레포터(여)
            ExineNPCs[21] = new MLibrary(Settings.ExineNPCPath + "ZM_01_261_꼬마남01_reidx");//꼬마(남)
            ExineNPCs[22] = new MLibrary(Settings.ExineNPCPath + "ZM_02_515_꼬마여01_reidx");//꼬마(여)
            ExineNPCs[23] = new MLibrary(Settings.ExineNPCPath + "ZM_02_513_성인여01_reidx");//루이루이 도박
            ExineNPCs[24] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//오스틴 격장관리
            ExineNPCs[25] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//로디 병사
            ExineNPCs[26] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//레이스 병사
            ExineNPCs[27] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//비에모크 병사
            ExineNPCs[28] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//루제르 병사
            ExineNPCs[29] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//신전경비 병사
            ExineNPCs[30] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//글루디오 병사
            ExineNPCs[31] = new MLibrary(Settings.ExineNPCPath + "ZM_01_260_경비원01_reidx");//발켄 병사
            ExineNPCs[32] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//하드로더 상인
            ExineNPCs[33] = new MLibrary(Settings.ExineNPCPath + "ZM_01_257_성인남01_reidx");//그랑쉬에르 미용
            ExineNPCs[34] = new MLibrary(Settings.ExineNPCPath + "ZM_02_519_요정01_reidx");//요정
            ExineNPCs[35] = new MLibrary(Settings.ExineNPCPath + "bronze_statue");//용사동
            */
            /*

ZM_01_256_영주01_reidx.lib
ZM_01_257_성인남01_reidx.lib
ZM_01_258_성인남02_reidx.lib
ZM_01_259_대장장이01_reidx.lib
ZM_01_260_경비원01_reidx.lib
ZM_01_261_꼬마남01_reidx.lib
ZM_01_262_장인01_reidx.lib
ZM_01_263__reidx.lib
ZM_02_512_여관주인01_reidx.lib
ZM_02_513_성인여01_reidx.lib
ZM_02_514_주점주인01_reidx.lib
ZM_02_515_꼬마여01_reidx.lib
ZM_02_516_재단사01_reidx.lib
ZM_02_517_마법상인01_reidx.lib
ZM_02_518_악세사리상인01_reidx.lib
ZM_02_519_요정01_reidx.lib
            bronze_statue.Lib
            */

            #endregion ExineNPC
             
            #region ExMonsters
             
            ExineMonsters[0] = new MLibrary(Settings.ExineMonsterPath + "empty");
            //ExineMonsters[1] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_768_flaty");
            ExineMonsters[1] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_768_플래티_reidx");
            ExineMonsters[2] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_769_아르마딜_reidx");
            ExineMonsters[3] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_770_크릭켓_reidx");
            ExineMonsters[4] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_771_스콜_reidx");
            ExineMonsters[5] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_772_개미_reidx");
            ExineMonsters[6] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_773_테나가_reidx");
            ExineMonsters[7] = new MLibrary(Settings.ExineMonsterPath + "ZM_03_774__reidx");
            ExineMonsters[8] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1024_거스웜_reidx");
            ExineMonsters[9] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1025_딩고_reidx");
            ExineMonsters[10] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1026_쇼크_reidx");
            ExineMonsters[11] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1027_크로틀_reidx");
            ExineMonsters[12] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1028_스토마_reidx");
            ExineMonsters[13] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1029_빅프로거_reidx");
            ExineMonsters[14] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1030_스몰 프로거_reidx");
            ExineMonsters[15] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1031_호아친_reidx");
            ExineMonsters[16] = new MLibrary(Settings.ExineMonsterPath + "ZM_04_1032_켈피_reidx");
            ExineMonsters[17] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1280_델피네_reidx");
            ExineMonsters[18] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1281_사마엘_reidx");
            ExineMonsters[19] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1282_하피_reidx");
            ExineMonsters[20] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1283__reidx");
            ExineMonsters[21] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1284__reidx"); 
            ExineMonsters[22] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1285_로비아탈_reidx");
            ExineMonsters[23] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1286__reidx");
            ExineMonsters[24] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1287__reidx");
            ExineMonsters[25] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1288_반트_reidx");
            ExineMonsters[26] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1289_카룬_reidx");
            ExineMonsters[27] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1290__reidx");
            ExineMonsters[28] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1291_세이렌_reidx");
            ExineMonsters[29] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1292_즈라촉_reidx");
            ExineMonsters[30] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1293_켈베로스리치_reidx");
            ExineMonsters[31] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1294__reidx");
            ExineMonsters[32] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1295__reidx"); 
            ExineMonsters[33] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1296__reidx");
            ExineMonsters[34] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1297__reidx");
            ExineMonsters[35] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1298__reidx");
            ExineMonsters[36] = new MLibrary(Settings.ExineMonsterPath + "ZM_05_1299__reidx");
            ExineMonsters[37] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1536__reidx");
            ExineMonsters[38] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1537__reidx");
            ExineMonsters[39] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1538_라돈_reidx");
            ExineMonsters[40] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1539_닌트_reidx");
            ExineMonsters[41] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1540_빅혼_reidx");
            ExineMonsters[42] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1541_차쿠_reidx");
            ExineMonsters[43] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1542__reidx"); 
            ExineMonsters[44] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1543_물고기_reidx");
            ExineMonsters[45] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1544_바트라코스_reidx");
            ExineMonsters[46] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1545_스노우자이언트_reidx");
            ExineMonsters[47] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1546_루츠_reidx");
            ExineMonsters[48] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1547_카엘_reidx");
            ExineMonsters[49] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1548__reidx");
            ExineMonsters[50] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1549_리트라코_reidx");
            ExineMonsters[51] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1550__reidx");
            ExineMonsters[52] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1551__reidx");
            ExineMonsters[53] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1552_가르콘오크_reidx");
            ExineMonsters[54] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1553_크루델오크_reidx"); 
            ExineMonsters[55] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1554_쿠겔오크_reidx");
            ExineMonsters[56] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1555__reidx");
            ExineMonsters[57] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1556__reidx");
            ExineMonsters[58] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1557__reidx");
            ExineMonsters[59] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1558__reidx");
            ExineMonsters[60] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1559__reidx");
            ExineMonsters[61] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1560_그란트라코_reidx");
            ExineMonsters[62] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1561_일룩스오크_reidx");
            ExineMonsters[63] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1562_슬라임_reidx");
            ExineMonsters[64] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1563_라돈_reidx");
            ExineMonsters[65] = new MLibrary(Settings.ExineMonsterPath + "ZM_06_1564_라돈_reidx");
            ExineMonsters[66] = new MLibrary(Settings.ExineMonsterPath + "ZM_07_1792_아이스 우드_reidx");
            ExineMonsters[67] = new MLibrary(Settings.ExineMonsterPath + "ZM_07_1793_블라디터마_reidx");
            ExineMonsters[68] = new MLibrary(Settings.ExineMonsterPath + "ZM_08_2048_파이러스_reidx");
            ExineMonsters[69] = new MLibrary(Settings.ExineMonsterPath + "ZM_09_2304_벽_reidx");
            ExineMonsters[70] = new MLibrary(Settings.ExineMonsterPath + "ZM_09_2305_용의심장_reidx");
            ExineMonsters[71] = new MLibrary(Settings.ExineMonsterPath + "ZM_09_2306_용의심장_reidx");
            ExineMonsters[72] = new MLibrary(Settings.ExineMonsterPath + "ZM_09_2307_용의심장_reidx");
            #endregion ExMonsters

            //EffectMonster

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
            ExineMiniMap.Initialize();
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

            ActionTree.Initialize();
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
            Count = MapLibs.Length + Monsters.Length + Gates.Length + Flags.Length +  CWeapons.Length +
                CHumEffect.Length + 18 ;

            Dragon.Initialize();
            Progress++;

            BuffIcon.Initialize();
            Progress++;

            Help.Initialize();
            Progress++;

            //ExineMiniMap.Initialize();
            //Progress++;
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
            
             
            /*
            for (int i = 0; i < NPCs.Length; i++)
            {
                NPCs[i].Initialize();
                Progress++;
            }
            */

            ///////////////////////////////////////
            //k333123 add 
           

            for (int i = 0; i < ExineManHair.Length; i++)
            {
                ExineManHair[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenHair.Length; i++)
            {
                ExineWomenHair[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineManArmor.Length; i++)
            {
                ExineManArmor[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenArmor.Length; i++)
            {
                ExineWomenArmor[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineManSheild.Length; i++)
            {
                ExineManSheild[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenSheild.Length; i++)
            {
                ExineWomenSheild[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineManOneWeapon.Length; i++)
            {
                ExineManOneWeapon[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenOneWeapon.Length; i++)
            {
                ExineWomenOneWeapon[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineManTwoWeapon.Length; i++)
            {
                ExineManTwoWeapon[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenTwoWeapon.Length; i++)
            {
                ExineWomenTwoWeapon[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineManBowWeapon.Length; i++)
            {
                ExineManBowWeapon[i].Initialize();
                Progress++;
            }
            for (int i = 0; i < ExineWomenBowWeapon.Length; i++)
            {
                ExineWomenBowWeapon[i].Initialize();
                Progress++;
            }
            ///////////////////////////////////////

             
            for (int i = 0; i < CWeapons.Length; i++)
            {
                CWeapons[i].Initialize();
                Progress++;
            }
             
            for (int i = 0; i < CHumEffect.Length; i++)
            {
                CHumEffect[i].Initialize();
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
                //DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Tint);
                DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Color.FromArgb(200,Tint.R,Tint.G,Tint.B));
            }

            mi.CleanTime = CMain.Time + Settings.CleanDelay;
        }

        public void ExineDrawTinted(int index, Point point, Color colour, Color Tint, bool offSet = false, bool isBlending = false, float rate = 1)
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
                //add alpha?
                //DXManager.SetBlend(true, rate, BlendMode.LIGHTINV);//NONE
                DXManager.SetBlend(true, rate, BlendMode.NONE);//NONE

                
                //DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Tint);
                DXManager.Draw(mi.MaskImage, new Rectangle(0, 0, mi.Width, mi.Height), new Vector3((float)point.X, (float)point.Y, 0.0F), Color.FromArgb(255, Tint.R, Tint.G, Tint.B));
                DXManager.SetBlend(oldBlend);
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

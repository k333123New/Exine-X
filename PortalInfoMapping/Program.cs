using System.Text;

namespace MyApp // Note: actual namespace depends on the project name.
{
    class Program
    {
 

        static void Main(string[] args)
        {
            Dictionary<short, List<Portal>> portals = new Dictionary<short, List<Portal>>();
            FileStream fs = new FileStream("Portals.tbl", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            short unknown = br.ReadInt16();
            uint count = br.ReadUInt32();
            Console.WriteLine("Portal Count:" + count);
            Console.WriteLine("Read Bytes:" + (count * 6 + 2 + 4));
            Console.ReadLine();

            for (int i = 0; i < count; ++i)
            {
                Portal portal = new Portal();
                portal.mapId = br.ReadInt16();
                portal.x = br.ReadInt16();
                portal.y = br.ReadInt16();

                if (portals.ContainsKey(portal.mapId) == false)
                {
                    portals.Add(portal.mapId, new List<Portal>());
                }
                portals[portal.mapId].Add(portal);
            }

            br.Close();

            PortalLinkInfoMgr portalLinkInfoMgr = new PortalLinkInfoMgr();
            
            string[] sample = File.ReadAllLines("MapInfoExport.txt");
            List<MapInfo> mapInfos = new List<MapInfo>();

            foreach (string line in sample)
            {
                string[] items = line.Split(' ');
                if (items.Length > 7)//map basic info
                {
                    MapInfo mapInfo = new MapInfo();
                    for (int i = 0; i < items.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                items[i] = items[i].Replace("[", "");
                                mapInfo.mapId = Convert.ToInt16(items[i]);
                                break;
                            case 1:
                                items[i] = items[i].Replace("]", "");
                                mapInfo.name = items[i];
                                break;
                            case 2:
                                items[i] = items[i].Replace("LIGHT(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.light = items[i];
                                break;
                            case 3:
                                items[i] = items[i].Replace("MINIMAP(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.minimap = Convert.ToInt16(items[i]);
                                break;
                            case 4:
                                items[i] = items[i].Replace("BIGMAP(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.bigmap = Convert.ToInt16(items[i]);
                                break;
                            case 5:
                                items[i] = items[i].Replace("MAPLIGHT(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.maplight = Convert.ToInt16(items[i]);
                                break;
                            case 6:
                                items[i] = items[i].Replace("MINE(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.mine = Convert.ToInt16(items[i]);
                                break;
                            case 7:
                                items[i] = items[i].Replace("MUSIC(", "");
                                items[i] = items[i].Replace(")", "");
                                mapInfo.music = Convert.ToInt16(items[i]);
                                break;

                            default: continue;
                        }
                    }
                    mapInfos.Add(mapInfo);
                }
            }
            for (int i = 0; i < mapInfos.Count; i++)
            {
                if (portals.ContainsKey(mapInfos[i].mapId))
                {
                    List<Portal> fromPortals = portals[mapInfos[i].mapId];

                    foreach (Portal fromPortal in fromPortals)
                    {
                        if (mapInfos[i].portalLinks == null)
                        {
                            mapInfos[i].portalLinks = new List<PortalLink>();
                        }
                        mapInfos[i].portalLinks.Add(new PortalLink(fromPortal, portalLinkInfoMgr.GetLinkPortal(fromPortal)));
                    }
                }
            }

            foreach (MapInfo mapInfo in mapInfos)
            {
                Console.WriteLine(mapInfo.ToString());
            }

            StringBuilder sb = new StringBuilder();
            foreach (MapInfo mapInfo in mapInfos)
            {
                sb.Append(mapInfo.ToString());
                sb.AppendLine();
            }
            File.WriteAllText("portalMapping.txt", sb.ToString());

        }
    }


    public class PortalLinkInfoMgr
    {
        List<PortalLink> portalLinks = new List<PortalLink>();

        public PortalLinkInfoMgr()
        {
            string[] portalLinkInfo = File.ReadAllLines("PortalLinkInfo.txt");
            
            for (int i = 0; i < portalLinkInfo.Length; ++i)
            {
                string fromPortal = portalLinkInfo[i].Split('/')[0];
                short map = Convert.ToInt16(fromPortal.Split(' ')[0]);
                short x = Convert.ToInt16(fromPortal.Split(' ')[1].Split(',')[0]);
                short y = Convert.ToInt16(fromPortal.Split(' ')[1].Split(',')[1]);
                Portal from = new Portal(map, x, y);


                string toPortal = portalLinkInfo[i].Split('/')[1];
                map = Convert.ToInt16(toPortal.Split(' ')[0]);
                x = Convert.ToInt16(toPortal.Split(' ')[1].Split(',')[0]);
                y = Convert.ToInt16(toPortal.Split(' ')[1].Split(',')[1]);
                Portal to = new Portal(map, x, y);

                portalLinks.Add(new PortalLink(from, to));
            }
        }
        public Portal GetLinkPortal(Portal fromPortal)
        {
            foreach(var portalLink in portalLinks)
            {
                if(portalLink.fromPortal.mapId == fromPortal.mapId
                    && portalLink.fromPortal.x == fromPortal.x
                    && portalLink.fromPortal.y == fromPortal.y)
                { 
                    return portalLink.toPortal;
                }
            }
            return null;
        } 
    }

    public class Portal
    {
        public short mapId = 10000;
        public short x = 0;
        public short y = 0;
        public Portal()
        {

        }
        public Portal(short mapId, short x, short y)
        {
            this.mapId = mapId;
            this.x= x;
            this.y= y;
        }
        override
        public string ToString()
        {
            return String.Format("{0} {1},{2}", mapId, x, y);
        }
    }

    public class PortalLink
    {
        public Portal fromPortal = new Portal();
        public Portal toPortal = new Portal();

        public PortalLink(Portal fromPortal, Portal toPortal)
        {
            this.fromPortal = fromPortal;
            this.toPortal = toPortal==null?new Portal(10000,0,0):toPortal;

        }
        override
        public string ToString()
        {
            return String.Format("{0} -> {1} ", fromPortal.ToString(), toPortal.ToString());
        }
    }

    public class MapInfo
    {
        public short mapId = 10000;
        public string name = "이름";
        public string light = LIGHT.Day;
        public short minimap = 0;
        public short bigmap = 0;
        public short maplight = 0;
        public short mine = 0;
        public short music = 1004;
        public List<PortalLink> portalLinks = null; //10006 0,0 -> 10002 10,12 

        public MapInfo()
        {

        }
        public MapInfo(short mapId, string name, string light, short minimap, short music, short maplight = 0, short mine = 0, short bigmap = 0)
        {
            this.mapId = mapId;
            this.name = name;
            this.light = light;
            this.minimap = minimap;
            this.bigmap = bigmap;
            this.maplight = maplight;
            this.mine = mine;
            this.music = music;
        }
        override
        public string ToString()
        {
            string result = String.Format("[{0} {1}] LIGHT({2}) MINIMAP({3}) BIGMAP({4}) MAPLIGHT({5}) MINE({6}) MUSIC({7})", mapId, name, light, minimap, bigmap, maplight, mine, music);
            if (portalLinks != null)
            {
                foreach (PortalLink portalLink in portalLinks)
                {
                    result += "\r\n";
                    result += portalLink.ToString();
                }
            }
            return result;
        }
    }
    static public class LIGHT
    {
        public const string Day = "Day";
        public const string Evening = "Evening";
    }
}

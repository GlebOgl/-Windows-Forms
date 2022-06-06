using Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Model
{
    public static class MapModel
    {
        private const string mapWithPlayerWall = @"
WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
WE           E   E          EW
W                            W
W        E          E        W
WWWWWW WWW          WWWW WWWWW
W        W    E E   WE      EW
W        W     E    W        W
WE      EW          W        W
WWWWWWWWWW          WWWWWWWWWW
WWWWWE        WWW       EWWWWW
WWWWW         WWW        WWWWW
WWWWWE         S        EWWWWW
WWWWWWWWWW WWWWWWWWWWW WWWWWWW
WE            E E          E W
WWW W W                WWWWWWW
W         WWWW WWWWW       E W
W   WWWW  W   E    W     WWWWW
W   WE W EW        W    E   WW
W      WWWW   CP   WW      RWW
WWWWWWWWWWWWWWWWWWWWWWWWWWWWWW";
        public static Cell[,] Map;
        public static Point PointClick = new Point(-1,-1);
        public static Point PlayerCoordinates;
        public static Point CloneCoordinates;
        public static int MapWidth => Map.GetLength(0);
        public static int MapHeight => Map.GetLength(1);
        public static Keys KeyPressed;

        public static void CreateMap()
        {
            var structureMap = MapCreator.CreateMap(mapWithPlayerWall);
            Map = new Cell[structureMap.GetLength(0), structureMap.GetLength(1)];
            for (var i = 0; i < MapWidth; i++)
                for (var j = 0; j < MapHeight; j++)
                {
                    if (structureMap[i, j] != null)
                        Map[i, j] = new Cell(structureMap[i, j], new Point(i, j), structureMap[i, j].GetImageFileName());
                }
        }
    }
}

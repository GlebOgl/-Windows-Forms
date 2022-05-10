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
WWW W
WWP W
W W W
WW WW";
        public static Cell[,] Map;
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

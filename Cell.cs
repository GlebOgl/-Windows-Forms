using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public class Cell
    {
        string ImageFileName { get; }
        public Point Coordinates { get ;}
        public IStructure Structure { get; set; }

        public Cell(IStructure structure, Point coordinates, string imageFileName)
        {
            Structure = structure;
            Coordinates = coordinates;
            ImageFileName = imageFileName;
        }
    }
}

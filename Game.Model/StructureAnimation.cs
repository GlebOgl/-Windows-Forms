using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public class StructureAnimation
    {
        public StructureCommand Command;
        public IStructure Structure;
        public Point Location;
        public Point TargetLogicalLocation;
    }
}

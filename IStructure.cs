using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public interface IStructure
    {
        string GetImageFileName();
        bool IsFreeCell();
        int GetDrawingPriority();
        StructureCommand Act(int x, int y);
    }
}

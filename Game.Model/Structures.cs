using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Model
{
    class Player : IStructure
    {
        public int Health = 100;

        public StructureCommand Act(int x, int y)
        {
            var newX = x;
            var newY = y;
            if (MapModel.KeyPressed == Keys.Up)
                newY -= 1;
            if (MapModel.KeyPressed == Keys.Down)
                newY += 1;
            if (MapModel.KeyPressed == Keys.Right)
                newX += 1;
            if (MapModel.KeyPressed == Keys.Left)
                newX -= 1;
            if (newX >= MapModel.MapWidth || newX < 0 || newY >= MapModel.MapHeight || newY < 0 ||
                MapModel.Map[newX, newY] != null && MapModel.Map[newX, newY].Structure != null
                && !MapModel.Map[newX, newY].Structure.IsFreeCell())
                return new StructureCommand { };
            return new StructureCommand { DeltaX = newX - x, DeltaY = newY - y };
        }

        public int GetDrawingPriority()
        {
            return 100;
        }

        public string GetImageFileName()
        {
            return "Player.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }

    class Wall : IStructure
    {
        public StructureCommand Act(int x, int y)
        {
            return new StructureCommand { };
        }

        public int GetDrawingPriority()
        {
            return 1000;
        }

        public string GetImageFileName()
        {
            return "Wall.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }

    class Empty : IStructure
    {
        public StructureCommand Act(int x, int y)
        {
            return new StructureCommand { };
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Empty";
        }

        public bool IsFreeCell()
        {
            return true;
        }
    }


}

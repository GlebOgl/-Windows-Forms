using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game.Model
{
    class Player : IStructure
    {
        public int Health = 100;
        public int Endurance = 5;
        public int Damage = 5;
        private Queue<Point> WayToCell = new Queue<Point>();

        public StructureCommand Act(int x, int y)
        {
            var newX = MapModel.PointClick.X / GameState.ElementSize;
            var newY = MapModel.PointClick.Y / GameState.ElementSize;
            if (WayToCell.Count != 0)
            {
                var point = WayToCell.Dequeue();
                return new StructureCommand { DeltaX = point.X - x, DeltaY = point.Y - y };
            }
            if (MapModel.PointClick != new Point(-1,-1))
            {
                MapModel.PointClick = new Point(-1, -1);
                if (MapModel.Map[newX, newY].Structure != null && 
                    MapModel.Map[newX, newY].Structure.GetImageFileName() == "Enemy.png")
                {
                    MapModel.Map[newX, newY].Structure.GetAttaced(Damage);
                    return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                }
                else if(MapModel.Map[newX, newY].Structure == null ||
                    MapModel.Map[newX, newY].Structure.IsFreeCell())
                {
                    WayToCell = FindPaths(new Point(x, y), new Point[] { new Point(newX, newY) });
                    if (WayToCell.Count == 0)
                        return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                    var point = WayToCell.Dequeue();
                    return new StructureCommand { DeltaX = point.X - x, DeltaY = point.Y - y };
                }
                
            }
            newX = x;
            newY = y;
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

        private static bool PointIsCorrect(Point point)
        {
            return !(point.X < 0 || point.X >= MapModel.MapWidth
                || point.Y < 0 || point.Y >= MapModel.MapHeight
                || MapModel.Map[point.X, point.Y].Structure==null
                || MapModel.Map[point.X, point.Y].Structure.IsFreeCell());
        }

        public static Queue<Point> FindPaths(Point start, Point[] chests)
        {
            var visited = new HashSet<Point>() { start };
            var queue = new Queue<Point>();
            queue.Enqueue(start);
            var tracks = new Dictionary<Point, SinglyLinkedList<Point>>();
            tracks.Add(start, new SinglyLinkedList<Point>(start));
            while (queue.Count != 0)
            {
                var point = queue.Dequeue();
                if (!PointIsCorrect(point)) continue;
                for (var dy = -1; dy <= 1; dy++)
                    for (var dx = -1; dx <= 1; dx++)
                    {
                        if (dx != 0 && dy != 0) continue;
                        var nextPoint = new Point { X = point.X + dx, Y = point.Y + dy };
                        if (visited.Contains(nextPoint)) continue;
                        queue.Enqueue(nextPoint);
                        visited.Add(nextPoint);
                        tracks.Add(nextPoint, new SinglyLinkedList<Point>(nextPoint, tracks[point]));
                    }
            }
            var result = new Queue<Point>();
            foreach (var chest in chests)
                if (tracks.ContainsKey(chest))
                    result.Enqueue(tracks[chest].Value);
            return result;
        }

        public void GetAttaced(int damage)
        {
            throw new NotImplementedException();
        }
    }

    class Wall : IStructure
    {
        public StructureCommand Act(int x, int y)
        {
            return new StructureCommand { };
        }


        public void GetAttaced(int damage)
        {
            throw new NotImplementedException();
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

        public void GetAttaced(int damage)
        {
            throw new NotImplementedException();
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
    class Enemy : IStructure
    {
        public int Health = 15;
        public StructureCommand Act(int x, int y)
        {
            if (Health == 0)
                MapModel.Map[x, y].Structure = new Empty(); 
            return new StructureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public void GetAttaced(int damage)
        {
            Health-=damage;
        }

        public int GetDrawingPriority()
        {
            return 50;
        }

        public string GetImageFileName()
        {
            return "Enemy.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }

}

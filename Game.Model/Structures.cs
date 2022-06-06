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
        public Point Coordinates { get; set; }
        //private Func<int, int, int> DamageCalculation = new Func<int, int, int>((d, s) => s * 2);
        public int Health = 20;
        public int Endurance = 5;
        private int Damage = 3;
        private int AtackRange = 1;
        public int WalkCost = 1;
        private int AttackCost = 1;
        private string Weapon = "Fist";
        //private int MaxHealth;
        //private int Dexterity;
        //private int Strength;
        //private int MaxEndurance = 3;

        private Queue<Point> WayToCell = new Queue<Point>();

        //public Player(int dexterity, int strength, int maxHealth, int endurance)
        //{
        //    Dexterity = dexterity;
        //    Strength = strength;
        //    Endurance = endurance;
        //    MaxHealth = maxHealth;
        //    Health = MaxHealth;
        //    GameState.PlayerHealth = MaxHealth;
        //    DamageCalculation = new Func<int, int, int>((d, s) => s * 2);
        //}

        public StructureCommand Act(int x, int y)
        {
            //Damage = DamageCalculation(Dexterity, Strength);
            GameState.PlayerHealth = Health;
            GameState.PlayerDamage = Damage;
            GameState.PlayerEndurance = Endurance;
            GameState.PlayerRange = AtackRange;
            if (Health <= 0)
            { 
                MapModel.Map[x, y].Structure = new Empty();
                MapModel.Map[MapModel.CloneCoordinates.X + 1, MapModel.CloneCoordinates.Y].Structure = new Player();

            }
            var newX = MapModel.PointClick.X / GameState.ElementSize;
            var newY = MapModel.PointClick.Y / GameState.ElementSize;
            if (MapModel.Map[newX, newY].Structure is RPistol
                && Math.Abs(x - newX) <= 1 && Math.Abs(y - newY) <= 1)
            {
                Weapon = "Pistol";
                Damage = 5;
                AtackRange = 5;
            }
            if (MapModel.Map[newX, newY].Structure is Shotgun
                && Math.Abs(x - newX) <= 1 && Math.Abs(y - newY) <= 1)
            {
                Weapon = "Shotgun";
                Damage = 10;
                AtackRange = 3;
            }
            if (GameState.IsBattleModOn && GameState.IsPlayerTurn)
            {
                return BattleModAct(x, y, newX, newY);
            }
            Endurance = 5;
            GameState.IsPlayerTurn = true;
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

        public StructureCommand BattleModAct(int x, int y, int newX, int newY)
        {
            if (WayToCell.Count != 0)
            {
                var point = WayToCell.Dequeue();
                if (point.X - x == 1 || point.Y - y == 1)
                {
                    WayToCell = new Queue<Point>();
                    return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                }
                Endurance -= WalkCost;
                Coordinates = new Point(point.X, point.Y);
                MapModel.PlayerCoordinates = new Point(point.X, point.Y);
                if (Endurance <= 0)
                    GameState.IsPlayerTurn = false;
                return new StructureCommand { DeltaX = point.X - x, DeltaY = point.Y - y };
            }
            else if (MapModel.PointClick != new Point(-1, -1))
            {
                MapModel.PointClick = new Point(-1, -1);
                if (MapModel.Map[newX, newY].Structure != null &&
                    MapModel.Map[newX, newY].Structure.GetImageFileName() == "Enemy.png" &&
                    (Math.Abs(newX - x) <= AtackRange && Math.Abs(newY - y) <= AtackRange))
                {
                    MapModel.Map[newX, newY].Structure.GetAttaced(Damage);
                    //if (Weapon == "Fist")
                    //    MapModel.Map[newX, newY].Structure.GetAttaced(Damage);
                    //else if (Weapon == "Pistol")
                    //    MapModel.Map[newX, newY].Structure.GetAttaced(5);
                    //else if (Weapon == "Shotgun")
                    //    MapModel.Map[newX, newY].Structure.GetAttaced(10);
                    Endurance -= AttackCost;
                    if (Endurance <= 0)
                        GameState.IsPlayerTurn = false;
                    return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                }
                else if (MapModel.Map[newX, newY].Structure == null ||
                    MapModel.Map[newX, newY].Structure.IsFreeCell())
                {
                    WayToCell = FindPaths(new Point(x, y), new Point[] { new Point(newX, newY) });
                    if (WayToCell.Count == 0)
                        return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                    var point = WayToCell.Dequeue();
                    Endurance -= WalkCost;
                    if (Endurance <= 0)
                        GameState.IsPlayerTurn = false;
                    Coordinates = new Point(point.X, point.Y);
                    MapModel.PlayerCoordinates = new Point(point.X, point.Y);
                    return new StructureCommand { DeltaX = point.X - x, DeltaY = point.Y - y };
                }

            }
            return new StructureCommand { DeltaX = 0, DeltaY = 0 };
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
            Health -= damage;
            GameState.PlayerHealth = Health;
        }
    }

    class Wall : IStructure
    {
        public Point Coordinates { get; set; }
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
        public Point Coordinates { get; set; }
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

    public class CloneMachine : IStructure
    {
        public Point Coordinates { get; set; }

        public StructureCommand Act(int x, int y)
        {
            return new StructureCommand { };
        }

        public void GetAttaced(int damage)
        {
            
        }

        public int GetDrawingPriority()
        {
            return 1000;
        }

        public string GetImageFileName()
        {
            return "Clone.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }

    public class Enemy : IStructure
    {
        public Point Coordinates { get; set; }
        public int Health = 15;
        public int Endurance = 3;
        public int Damage = 5;
        public int WalkCost = 1;
        public int AttackCost = 1;
        int xPlayer;
        int yPlayer;

        public bool FindPlayer()
        {
            for (var i = 0; i < MapModel.MapHeight; i++)
            {
                for (var j = 0; j < MapModel.MapWidth; j++)
                {
                    if (MapModel.Map[j, i].Structure is Player)
                    {
                        xPlayer = j;
                        yPlayer = i;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool FindEnemy()
        {
            for (var i = 0; i < MapModel.MapHeight; i++)
            {
                for (var j = 0; j < MapModel.MapWidth; j++)
                {
                    if (MapModel.Map[j, i].Structure is Enemy)
                        return true;
                }
            }
            return false;
        }

        public bool CanMove(int x, int y)
        {
            if (MapModel.Map[x, y].Structure == null)
                return true;
            var map = MapModel.Map[x, y];
            return !(x >= MapModel.MapWidth || x < 0 || y >= MapModel.MapHeight || y < 0 ||
                !map.Structure.IsFreeCell());
        }

        public StructureCommand Act(int x, int y)
        {
            if (Health <= 0)
            {
                MapModel.Map[x, y].Structure = new Empty();
                if (!FindEnemy())
                    GameState.IsPlayerWon = true;
                return new StructureCommand();
            }
            if (Math.Abs(MapModel.PlayerCoordinates.X - x) > 5 ||
                Math.Abs(MapModel.PlayerCoordinates.Y - y) > 5)
                return new StructureCommand { };
            if (Endurance <=0)
            {
                Endurance = 3;
                GameState.IsPlayerTurn = true;
                return new StructureCommand() { };
            }
            if (!FindPlayer())
                return new StructureCommand() { };
            if (GameState.IsBattleModOn )
            {
                if (GameState.IsPlayerTurn)
                    return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                var newX = 0;
                var newY = 0;
                //var xPlayer = MapModel.PlayerCoordinates.X;
                //var yPlayer = MapModel.PlayerCoordinates.Y;
                if (Math.Abs(x - xPlayer) <= 1 && Math.Abs(y - yPlayer) <= 1 )
                {
                    MapModel.Map[xPlayer, yPlayer].Structure.GetAttaced(Damage);
                    Endurance -= AttackCost;
                    return new StructureCommand { DeltaX = 0, DeltaY = 0 };
                }
                if (x < xPlayer && CanMove(x + 1, y))
                    newX = 1;
                else if (x > xPlayer && CanMove(x - 1, y))
                    newX = -1;
                else if (y < yPlayer && CanMove(x, y + 1))
                    newY = 1;
                else if (y > yPlayer && CanMove(x, y - 1))
                    newY = -1;
                Endurance -= WalkCost;
                return new StructureCommand { DeltaX = newX, DeltaY = newY };
            }
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

    public class RPistol : IStructure
    {
        public Point Coordinates { get ; set ; }

        public StructureCommand Act(int x, int y)
        {
            Coordinates = new Point(x, y);
            return new StructureCommand();
        }

        public void GetAttaced(int damage)
        {
            
        }

        public int GetDrawingPriority()
        {
            return 20;
        }

        public string GetImageFileName()
        {
            return "Pistol.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }

    public class Shotgun : IStructure
    {
        public Point Coordinates { get; set; }

        public StructureCommand Act(int x, int y)
        {
            Coordinates = new Point(x, y);
            return new StructureCommand();
        }

        public void GetAttaced(int damage)
        {

        }

        public int GetDrawingPriority()
        {
            return 20;
        }

        public string GetImageFileName()
        {
            return "Shotgun.png";
        }

        public bool IsFreeCell()
        {
            return false;
        }
    }
}

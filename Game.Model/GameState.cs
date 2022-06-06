using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public class GameState
    {
        public const int ElementSize = 32;
        public List<StructureAnimation> Animations = new List<StructureAnimation>();
        public static bool IsBattleModOn = true;
        public static bool IsPlayerAlive = false;
        public static bool IsPlayerTurn = true;
        public static bool IsPlayerWon = false;
        public static int PlayerHealth;
        public static int PlayerEndurance;
        public static int PlayerRange;
        public static int PlayerDamage;
        public static List<Enemy> EnemiesMoves = new List<Enemy>(); 
        public void BeginAct()
        {
            Animations.Clear();
            //if (IsBattleModOn)
            //{
            //    if (IsPlayerTurn)
            //    {
            //        var x = MapModel.PlayerCoordinates.X;
            //        var y = MapModel.PlayerCoordinates.Y;
            //        var player = (Player)MapModel.Map[x, y].Structure;
            //        if (player.Endurance <= 0)
            //        {
            //            player.Endurance = 5;
            //            IsPlayerTurn = false;
            //        }
            //        else
            //        {
            //            var cell = MapModel.Map[x, y];
            //            var command = player.Act(x, y);
            //            Animations.Add(
            //                new StructureAnimation
            //                {
            //                    Command = command,
            //                    Structure = cell.Structure,
            //                    Location = new Point(x * ElementSize, y * ElementSize),
            //                    TargetLogicalLocation = new Point(x + command.DeltaX, y + command.DeltaY)
            //                });
            //        }
            //    }
            //    else
            //    {
            //        if (EnemiesMoves.Count == 0)
            //            EnemiesMoves = FindNearestEnemies(4);
            //        else
            //        {
            //            foreach (var e in EnemiesMoves)
            //            {
            //                while(e.Endurance > 0)
            //                {
            //                    var x = e.Coordinates.X;
            //                    var y = e.Coordinates.Y;
            //                    var cell = MapModel.Map[x, y];
            //                    var command = e.Act(x, y);
            //                    Animations.Add(
            //                    new StructureAnimation
            //                    {
            //                        Command = command,
            //                        Structure = cell.Structure,
            //                        Location = new Point(x * ElementSize, y * ElementSize),
            //                        TargetLogicalLocation = new Point(x + command.DeltaX, y + command.DeltaY)
            //                    });
            //                }
            //                e.Endurance = 3;
            //            }
            //            EnemiesMoves = new List<Enemy>();
            //            IsPlayerTurn = true;
            //        }
            //    }
            //}
            //else
            //{
                for (var x = 0; x < MapModel.MapWidth; x++)
                    for (var y = 0; y < MapModel.MapHeight; y++)
                    {
                        var cell = MapModel.Map[x, y];
                        if (cell == null || cell.Structure == null) continue;
                        var command = cell.Structure.Act(x, y);

                        if (x + command.DeltaX < 0 || x + command.DeltaX >= MapModel.MapWidth || y + command.DeltaY < 0 ||
                            y + command.DeltaY >= MapModel.MapHeight)
                            throw new Exception($"The object {cell.GetType()} falls out of the game field");

                        Animations.Add(
                            new StructureAnimation
                            {
                                Command = command,
                                Structure = cell.Structure,
                                Location = new Point(x * ElementSize, y * ElementSize),
                                TargetLogicalLocation = new Point(x + command.DeltaX, y + command.DeltaY)
                            });
                    }
            //}

            Animations = Animations.OrderByDescending(z => z.Structure.GetDrawingPriority()).ToList();
        }

        public List<Enemy> FindNearestEnemies(int n)
        {
            var x = MapModel.PlayerCoordinates.X;
            var y = MapModel.PlayerCoordinates.Y;
            var result = new List<Enemy>();
            for (var i = -n; i<n;i++)
                for (var j = -n; j<n;j++)
                {
                    if (i==0 && j==0 || x + i < 0 || x + i >= MapModel.MapWidth
                        || y + j < 0 || y + j >= MapModel.MapHeight)
                        continue;
                    if (MapModel.Map[x + i, y + j].Structure != null && 
                        MapModel.Map[x + i, y + j].Structure.GetImageFileName() == "Enemy.png")
                        result.Add((Enemy)MapModel.Map[x + i, y + j].Structure);
                }
            return result;
        }

        public void EndAct()
        {
            var structuresPerLocation = GetCandidatesPerLocation();
            for (var x = 0; x < MapModel.MapWidth; x++)
                for (var y = 0; y < MapModel.MapHeight; y++)
                {
                    //var structure = structuresPerLocation[x, y].FirstOrDefault();
                    //if (MapModel.Map[x, y] == null && structure != null)
                    //    MapModel.Map[x, y] = new Cell(structure, new Point(x, y), structure.GetImageFileName());
                    //else if (structure != null)
                        MapModel.Map[x, y].Structure = structuresPerLocation[x, y].FirstOrDefault();
                }
        }

        private List<IStructure>[,] GetCandidatesPerLocation()
        {
            var creatures = new List<IStructure>[MapModel.MapWidth, MapModel.MapHeight];
            for (var x = 0; x < MapModel.MapWidth; x++)
                for (var y = 0; y < MapModel.MapHeight; y++)
                    creatures[x, y] = new List<IStructure>();
            foreach (var e in Animations)
            {
                var x = e.TargetLogicalLocation.X;
                var y = e.TargetLogicalLocation.Y;
                var nextCreature = e.Command.TransformTo ?? e.Structure;
                creatures[x, y].Add(nextCreature);
            }

            return creatures;
        }
    }

    public class SinglyLinkedList<T> : IEnumerable<T>
    {
        public readonly T Value;
        public readonly SinglyLinkedList<T> Previous;
        public readonly int Length;

        public SinglyLinkedList(T value, SinglyLinkedList<T> previous = null)
        {
            Value = value;
            Previous = previous;
            Length = previous?.Length + 1 ?? 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            yield return Value;
            var pathItem = Previous;
            while (pathItem != null)
            {
                yield return pathItem.Value;
                pathItem = pathItem.Previous;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

﻿using System;
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

        public void BeginAct()
        {
            Animations.Clear();
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

            Animations = Animations.OrderByDescending(z => z.Structure.GetDrawingPriority()).ToList();
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

        //private static IStructure SelectWinnerCandidatePerLocation(List<IStructure>[,] creatures, int x, int y)
        //{
        //    var candidates = creatures[x, y];
        //    var aliveCandidates = candidates.ToList();
        //    foreach (var candidate in candidates)
        //        foreach (var rival in candidates)
        //            if (rival != candidate && candidate.DeadInConflict(rival))
        //                aliveCandidates.Remove(candidate);
        //    if (aliveCandidates.Count > 1)
        //        throw new Exception(
        //            $"Creatures {aliveCandidates[0].GetType().Name} and {aliveCandidates[1].GetType().Name} claimed the same map cell");

        //    return aliveCandidates.FirstOrDefault();
        //}

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
}
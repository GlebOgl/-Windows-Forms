using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    public static class MapCreator
    {
        private static readonly Dictionary<string, Func<IStructure>> factory = new Dictionary<string, Func<IStructure>>();

        public static IStructure[,] CreateMap(string map, string separator = "\r\n")
        {
            var rows = map.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Select(z => z.Length).Distinct().Count() != 1)
                throw new Exception($"Wrong test map '{map}'");
            var result = new IStructure[rows[0].Length, rows.Length];
            for (var x = 0; x < rows[0].Length; x++)
                for (var y = 0; y < rows.Length; y++)
                    result[x, y] = CreateStructureBySymbol(rows[y][x]);
            return result;
        }

        private static IStructure CreateStructureByTypeName(string name)
        {
            if (!factory.ContainsKey(name))
            {
                var type = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(z => z.Name == name);
                if (type == null)
                    throw new Exception($"Can't find type '{name}'");
                factory[name] = () => (IStructure)Activator.CreateInstance(type);
            }

            return factory[name]();
        }


        private static IStructure CreateStructureBySymbol(char c)
        {
            switch (c)
            {
                case 'P':
                    return CreateStructureByTypeName("Player");
                case 'W':
                    return CreateStructureByTypeName("Wall");
                case ' ':
                    return CreateStructureByTypeName("Empty");
                default:
                    throw new Exception($"wrong character for ICreature {c}");
            }
        }
    }
}

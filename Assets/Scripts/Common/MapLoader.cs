using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Common
{
    public enum ItemType
    {
        Ground,
        Wind,
        P1,
        P2,
        Bridge,
        Water
    }

    public class MapLoader : UnitySingleton<MapLoader>
    {
        private static string Path(string map) =>
            System.IO.Path.Combine(Application.dataPath, $"Resources/Map/{map}.csv");

        private readonly Dictionary<string, ItemType> _string2Type = new Dictionary<string, ItemType>
        {
            {"Bridge", ItemType.Bridge},
            {"P1", ItemType.P1},
            {"P2", ItemType.P2},
            {"Wind", ItemType.Wind},
            {"Ground", ItemType.Ground},
            {"", ItemType.Water}
        };

        // private void Start()
        // {
        //     Read("map1");
        // }

        public Dictionary<Vector2, ItemType> Read(string map)
        {
            Dictionary<Vector2, ItemType> pos2Grid = new Dictionary<Vector2, ItemType>();
            var sr = new StreamReader(Path(map), Encoding.UTF8);
            string line;
            int row = 0;
            
            while ((line = sr.ReadLine()) != null)
            {
                Debug.Log("line");
                Debug.Log(line);
                int col = 0;
                foreach (var s in line.Split(','))
                {
                    Debug.Log(s);
                    if (!_string2Type.ContainsKey(s))
                    {
                        throw new Exception("错误的字符串");
                    }

                    var test = _string2Type[s];
                    pos2Grid.Add(new Vector2(col, row), test);
                    col += 1;
                }

                row += 1;
            }

            return pos2Grid;
        }
    }
}
using System;
using System.Collections.Generic;
using Common;
using Creator;
using UnityEngine;

namespace Controller
{
    public class MapSynchronizer : UnitySingleton<MapSynchronizer>
    {
        private static int Height => MapCreator.Instance.height;
        private static int Width => MapCreator.Instance.width;

        public static void Synchronize(Action<Vector2> sync)
        {
            List<Vector2> offset = new List<Vector2>
            {
                new Vector2(-Width, 0),
                new Vector2(Width, 0),
                new Vector2(0, Height),
                new Vector2(0, -Height),
                new Vector2(-Width, Height),
                new Vector2(Width, Height),
                new Vector2(-Width, -Height),
                new Vector2(Width, -Height)
            };

            offset.ForEach(sync);
        }

        public static Vector2 Normalize(Vector2 pos) =>
            new Vector2(((int) pos.x + Width) % Width, ((int) pos.y + Height) % Height);


        public static bool IsOutsize(Vector2 p) => p.x < 0 || p.y < 0 || p.x >= Width || p.y >= Height;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class GridModel
    {
        public Dictionary<Vector2, GameObject> Grid { get; set; } = new Dictionary<Vector2, GameObject>();

        public GameObject Get(Vector2 pos) => Grid.ContainsKey(pos) ? Grid[pos] : null;

        public void Put(Vector2 pos, GameObject go)
        {
            if (Contains(pos))
            {
                throw new Exception("重复的位置");
            }
            Grid.Add(pos, go);
        }

        public bool Contains(Vector2 pos) => Get(pos) != null;

        public void Remove(Vector2 pos) => Grid.Remove(pos);
    }
}
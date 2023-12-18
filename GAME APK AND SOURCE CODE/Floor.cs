using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;

public class Floor : Environment
{
    private GameObject originalTile;
    private GameObject currentTile;

    public Floor(Vector3 position, float cellSize, int x, int y, string printName) : base(position, cellSize, x, y, printName)
    {
        walkable = true;
        originalTile = GameObject.Find("FloorObject");
        currentTile = GameObject.Instantiate(originalTile, position, Quaternion.identity);
    }
}
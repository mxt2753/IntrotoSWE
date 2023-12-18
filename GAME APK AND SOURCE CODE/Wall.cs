using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Environment
{
    private GameObject originalWall;
    private GameObject currentWall;

    public bool walkable;

    public Wall(Vector3 position, float cellSize, int x, int y, string printName) : base(position, cellSize, x, y, printName)
    {
        walkable = false;
        originalWall = GameObject.Find("WallObject");

        currentWall = GameObject.Instantiate(originalWall, position, Quaternion.identity);
        currentWall.transform.position = position + new Vector3(0, 0, -1);
    }
}

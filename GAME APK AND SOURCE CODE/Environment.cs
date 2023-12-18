using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : GridObject
{
    public bool walkable;

    public Environment(Vector3 position, float cellSize, int x, int y, string objectName) : base(position, cellSize, x, y, objectName)
    {

    }
}
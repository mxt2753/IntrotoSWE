using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : GridObject
{
    public static LevelExit Instance {get; private set;}

    GameObject originalExit;

    private bool unlocked = false;

    public LevelExit(Vector3 position, float cellSize, int x, int y, string printName) : base(position, cellSize, x, y, printName)
    {
        Instance = this;
        originalExit = GameObject.Find("LevelExit");
        SetPosition(position + new Vector3(0, 0, -1));
    }

    public void UnlockExit()
    {
        unlocked = true;
        GridMap<GridSpace>.SetUnlockedEnd(true);
        originalExit.transform.position = GetPosition();
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }
}

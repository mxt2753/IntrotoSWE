using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private Vector3 position;
    private float cellSize;
    private int x;
    private int y;
    private string printName;

    public GridObject(Vector3 position, float cellSize, int x, int y, string printName)
    {
        this.position = position;
        this.cellSize = cellSize;
        this.x = x;
        this.y = y;
        this.printName = printName;
    }

    public string GetPrintName()
    {
        return printName;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetX(int newX)
    {
        x = newX;
    }

    public void SetY(int newY)
    {
        y = newY;
    }

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

    public Vector3 GetPosition()
    {
        return position;
    }
}

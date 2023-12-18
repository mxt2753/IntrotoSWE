using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System.Linq;

public class GridSpace
{
    private GridMap<GridSpace> grid;
    private int x;
    private int y;

    public int gCost;
    public int hCost;
    public int fCost;

    private bool highlighted;

    private GridObject layer0;
    private GridObject layer1;
    private Vector3 position;

    public GridSpace cameFromNode;

    public GridSpace(GridMap<GridSpace> grid, int x, int y, List<GridObject> objects, Vector3 position)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.position = position;
        highlighted = false;
        layer0 = objects.ElementAt(0);
        layer1 = objects.Count > 1 ? objects.ElementAt(1) : null;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public GridObject GetLayer0()
    {
        return layer0;
    }

    public GridObject GetLayer1()
    {
        return layer1;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void SetLayer1(GridObject newLayer1)
    {
        this.layer1 = newLayer1;
    }

    public bool IsWalkable()
    {
        if(layer1 == null || (layer1 is Environment && (layer1 as Environment).walkable) || (layer1 is Item) || (layer1 is Enemy) || (layer1 is LevelExit))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsHighlighted()
    {
        return highlighted;
    }

    public void SetHighlight(bool newHighlight)
    {
        highlighted = newHighlight;
    }

    public override string ToString()
    {
        return layer1?.ToString() + "\n" + layer0.ToString() + " X: " + x + " Y: " + y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public List<string> GetObjectNames()
    {
        List<string> names = new List<string>();
        names.Add(layer0.GetPrintName());
        if(layer1 != null)
        {
            names.Add(layer1.GetPrintName());
        }
        return names;
    }
}
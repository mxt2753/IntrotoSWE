using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Linq;
using System.IO;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance {get; private set;}

    private GridMap<GridSpace> grid;

    private List<GridSpace> openList;
    private List<GridSpace> closedList;

    public Pathfinding()
    {
        Instance = this;
        grid = new GridMap<GridSpace>(10f, Vector3.zero, (GridMap<GridSpace> g, int x, int y, List<GridObject> o, Vector3 p) => new GridSpace(g, x, y, o, p));
    }

    public void SaveLevel()
    {
        GridSpace[,] gridArray = grid.GetGridArray();
        // Debug.Log("IS THIS LEVEL SAVING!!!??");
        // using(var writer = new StreamWriter(Application.dataPath + "/Resources/level_save.txt", false))
        // {

        List<List<List<string>>> parsedFile = new List<List<List<string>>>();
        // for(int row = gridArray.GetLength(0) - 1; row >= 0; row--)
        for(int row = 0; row < gridArray.GetLength(0); row++)
        {
            List<List<string>> rowsList = new List<List<string>>();
            parsedFile.Add(rowsList);
            for(int col = 0; col < gridArray.GetLength(1); col++)
            {   
                GridSpace current_grid = gridArray[row,col];
                List<string> colsList = current_grid.GetObjectNames();
                rowsList.Add(colsList);
            }
        }
        grid.SetParsedFile(parsedFile);
        // grid.SetSavedGrid(grid.GetGridArray());
        // grid.SetSavedGridMap(grid);
    }

    public void ResetLevelSave()
    {
        using(var writer = new StreamWriter("level_save", false))
        {
            writer.WriteLine("none");
        }
    }

    public GridMap<GridSpace> GetGrid()
    {
        return grid;
    }

    public GridSpace GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<GridSpace> path = FindPath(startX, startY, endX, endY);

        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(GridSpace pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.GetX(), pathNode.GetY(), -1) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<GridSpace> PossibleMoves()
    {
        int radiusExplored = 0;
        GridSpace startNode = grid.GetGridObject(Player.Instance.GetX(), Player.Instance.GetY());

        List<GridSpace> possibleMoves = new List<GridSpace>();
        List<GridSpace> currentNodes = new List<GridSpace>{ startNode };
        List<GridSpace> visitedNodes = new List<GridSpace>();

        while(radiusExplored < Player.Instance.GetMovesLeft() && currentNodes.Count > 0)
        {
            List<GridSpace> nextCurrentNodes = new List<GridSpace>();
            foreach(GridSpace currentNode in currentNodes)
            {
                foreach(GridSpace neighbourNode in GetNeighbourList(currentNode))
                {
                   if(visitedNodes.Contains(neighbourNode))
                    {
                        continue;
                    }

                    if(neighbourNode.GetLayer1() is Enemy)
                    {
                        possibleMoves.Add(neighbourNode);
                        visitedNodes.Add(neighbourNode);
                        continue;
                    }

                    if(!neighbourNode.IsWalkable())
                    {
                        visitedNodes.Add(neighbourNode);
                        continue;
                    }

                    nextCurrentNodes.Add(neighbourNode);
                    visitedNodes.Add(neighbourNode);
                }
            }

            currentNodes.Clear();
            for (int i = 0; i < nextCurrentNodes.Count; i++)
            {
                currentNodes.Add(nextCurrentNodes[i]);
                possibleMoves.Add(nextCurrentNodes[i]);
            }
            radiusExplored += 1;
        }
        return possibleMoves;
    }

    public List<GridSpace> FindPath(int startX, int startY, int endX, int endY)
    {
        GridSpace startNode = grid.GetGridObject(startX, startY);
        GridSpace endNode = grid.GetGridObject(endX, endY);

        openList = new List<GridSpace>{ startNode };
        closedList = new List<GridSpace>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                GridSpace pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            GridSpace currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(GridSpace neighbourNode in GetNeighbourList(currentNode))
            {
                if(!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                if(closedList.Contains(neighbourNode))
                {
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private List<GridSpace> GetNeighbourList(GridSpace currentNode)
    {
        List<GridSpace> neighbourList = new List<GridSpace>();

        if(currentNode.GetX() - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
        }

        if(currentNode.GetX() + 1 < grid.GetWidth())
        {
            neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
        }

        if(currentNode.GetY() - 1 >= 0)
        {
            neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));
        }

        if(currentNode.GetY() + 1 < grid.GetHeight())
        {
            neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));
        }

        return neighbourList;
    }

    private int CalculateDistanceCost(GridSpace a, GridSpace b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private List<GridSpace> CalculatePath(GridSpace endNode)
    {
        List<GridSpace> path = new List<GridSpace>();
        path.Add(endNode);

        GridSpace currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private GridSpace GetLowestFCostNode(List<GridSpace> pathNodeList)
    {
        GridSpace lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}

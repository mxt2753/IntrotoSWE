using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class Enemy : GridObject
{
    public static List<Enemy> AllEnemies = new List<Enemy>();

    private int MAX_HEALTH;
    private int health;
    private int MAX_NUMBER_OF_MOVES;
    private int movesLeft;
    private int currentPathIndex;

    private float speed = 40f;

    private bool enemyTurn = false;

    private char patrolDirection;
    private char facingDirection;

    private GameObject enemyObject;
    private GameObject enemyHandler;
    private GridSpace beforeLastGrid;

    private List<Vector3> nextMoves = null;

    public Enemy(Vector3 position, float cellSize, int x, int y, int maxHealth, int maxMoves, char path, char direction, string printName) : base(position, cellSize, x, y, printName)
    {
        MAX_HEALTH = maxHealth;
        health = maxHealth;
        MAX_NUMBER_OF_MOVES = maxMoves;
        movesLeft = 0;

        facingDirection = direction;
        patrolDirection = path;

        GameObject originalEnemy = GameObject.Find("EnemyObject");
        GameObject originalHandler = GameObject.Find("EnemyMovementHandler");
        enemyObject = GameObject.Instantiate(originalEnemy, position + new Vector3(0, 0, -1), Quaternion.identity);
        enemyObject.GetComponent<EnemyMovementHandler>().SetEnemyObject(this);
        AllEnemies.Add(this);
    }

    public void GetEnemyPosition(out int x, out int y)
    {
        x = this.GetX();
        y = this.GetY();
    }

    public bool GetEnemyTurn()
    {
        return enemyTurn;
    }

    public void MoveEnemy(int x, int y, Vector3 newPosition, GridSpace currentGrid, GridSpace nextGrid)
    {
        this.SetX(x);
        this.SetY(y);
        this.SetPosition(newPosition);
        currentGrid.SetLayer1(null);
        nextGrid.SetLayer1(this);
    }

    public void DecreaseMovesLeft(int decrementValue)
    {
        movesLeft -= decrementValue;
    }

    public void ResetMovesLeft()
    {
        movesLeft = MAX_NUMBER_OF_MOVES;
    }

    public int GetMovesLeft()
    {
        return movesLeft;
    }

    public static bool AllEnemiesOutOfMoves()
    {
        bool allEnemiesDone = true;

        foreach(Enemy currentEnemy in AllEnemies)
        {
            if(currentEnemy.movesLeft > 0)
            {
                allEnemiesDone = false;
            }
        }
        return allEnemiesDone;
    }

    public static void ResetEnemiesMoves()
    {
        foreach(Enemy currentEnemy in AllEnemies)
        {
            currentEnemy.ResetMovesLeft();
        }
    }

    public static List<Enemy> GetEnemiesList()
    {
        return AllEnemies;
    }

    public void PlayTurn()
    {
        enemyTurn = true;
    }

    public void HandleEnemyMovement()
    {
        if (nextMoves != null) 
        {
            Vector3 targetPosition = nextMoves[currentPathIndex];
            if (Vector3.Distance(enemyObject.transform.position, targetPosition) > 1f) {
                Vector3 moveDir = (targetPosition - enemyObject.transform.position).normalized;

                float distanceBefore = Vector3.Distance(enemyObject.transform.position, targetPosition);
                enemyObject.transform.position = enemyObject.transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= nextMoves.Count) 
                {
                    GridSpace targetGrid = Pathfinding.Instance.GetGrid().GetGridObject(targetPosition);

                    if(targetGrid.GetLayer1() is Player)
                    {
                        MoveEnemy(beforeLastGrid.GetX(), beforeLastGrid.GetY(), beforeLastGrid.GetPosition(), Pathfinding.Instance.GetGrid().GetGridObject(GetX(), GetY()), beforeLastGrid);
                        BattleSystem.SetEnemyPosition(beforeLastGrid);

                        Pathfinding.Instance.SaveLevel();
                        SceneManager.LoadScene(6);
                    }
                    else
                    {
                        MoveEnemy(targetGrid.GetX(), targetGrid.GetY(), targetGrid.GetPosition(), Pathfinding.Instance.GetGrid().GetGridObject(GetX(), GetY()), targetGrid);
                    }
                    nextMoves = null;
                }
            }
        }
    }

    public void GetAllMoves()
    {
        nextMoves = new List<Vector3>();
        int x = GetX();
        int y = GetY();
        int curX = x;
        int curY = y;
        currentPathIndex = 0;
        Vector3 nextMove = new Vector3(0, 0, 0);

        while(movesLeft > 0)
        {
            int nextX;
            int nextY;

            nextMove = GetNextMove(curX, curY, out nextX, out nextY);
            nextMoves.Add(nextMove);
            movesLeft -= 1;

            curX = nextX;
            curY = nextY;
        }   
        
        int beforeLast;
        if(nextMoves.Count >= 2)
        {
            beforeLast = nextMoves.Count - 2;
            beforeLastGrid = Pathfinding.Instance.GetGrid().GetGridObject(nextMoves[beforeLast]);
        }
        else
        {
            beforeLastGrid = Pathfinding.Instance.GetGrid().GetGridObject(GetX(), GetY());
        }
    }

    public Vector3 GetNextMove(int x, int y, out int nextX, out int nextY)
    {
        Vector3 nextMove = new Vector3(0, 0, 0);
        GridSpace nextGrid = null;
        GridMap<GridSpace> grid = Pathfinding.Instance.GetGrid();
        GridSpace selfGrid = grid.GetGridObject(GetX(), GetY());

        if(patrolDirection == 'v')
        {
            if(facingDirection == 's')
            {
                nextGrid = y - 1 >= 0 ? grid.GetGridObject(x, y - 1) : null;
                // Debug.Log("NEXTGRID GOING SOUTH: " + nextGrid);
                if(nextGrid != null && (nextGrid.IsWalkable() || nextGrid == selfGrid))
                {
                    nextX = x;
                    nextY = y - 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    return nextMove;
                }
                else if(nextGrid.GetLayer1() is Player)
                {
                    nextX = x;
                    nextY = y - 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    movesLeft = 1;
                    return nextMove;
                }
                else
                {
                    facingDirection = 'n';
                    return GetNextMove(x, y, out nextX, out nextY);
                }
            }
            else if(facingDirection == 'n')
            {
                nextGrid = y + 1 < grid.GetHeight() ? grid.GetGridObject(x, y + 1) : null;
                if(nextGrid != null && (nextGrid.IsWalkable() || nextGrid == selfGrid))
                {
                    nextX = x;
                    nextY = y + 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    return nextMove;
                }
                else if(nextGrid is Player)
                {
                    nextX = x;
                    nextY = y - 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    movesLeft = 1;
                    return nextMove;
                }
                else
                {
                    facingDirection = 's';
                    return GetNextMove(x, y, out nextX, out nextY);
                }
            }
        }
        else if(patrolDirection == 'h')
        {
            if(facingDirection == 'e')
            {
                nextGrid = x + 1 < grid.GetWidth() ? grid.GetGridObject(x + 1, y) : null;
                if(nextGrid != null && (nextGrid.IsWalkable() || nextGrid == selfGrid))
                {
                    nextX = x + 1;
                    nextY = y;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    return nextMove;
                }
                else if(nextGrid is Player)
                {
                    nextX = x;
                    nextY = y - 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    movesLeft = 1;
                    return nextMove;
                }
                else
                {
                    facingDirection = 'w';
                    return GetNextMove(x, y, out nextX, out nextY);
                }
            }
            else if(facingDirection == 'w')
            {
                nextGrid = x - 1 >= 0 ? grid.GetGridObject(x - 1, y) : null;
                if(nextGrid != null && (nextGrid.IsWalkable() || nextGrid == selfGrid))
                {
                    nextX = x - 1;
                    nextY = y;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    return nextMove;
                }
                else if(nextGrid is Player)
                {
                    nextX = x;
                    nextY = y - 1;
                    nextMove = nextGrid.GetPosition();
                    nextMove.z = -1;
                    movesLeft = 1;
                    return nextMove;
                }
                else
                {
                    facingDirection = 'e';
                    return GetNextMove(x, y, out nextX, out nextY);
                }
            }
        }

        nextX = x;
        nextY = y;
        return nextMove;
    }

    public void DefeatEnemy(int x, int y)
    {
        List<List<List<string>>> parsedLevel = GridMap<GridSpace>.GetParsedFile();
        
        // int rows = 0;
        // foreach(List<List<string>> thingy in parsedLevel)
        // {
        //     int cols = 0;
        //     foreach(List<string> nextthing in thingy)
        //     {
        //         foreach(string FINAL in nextthing)
        //         {
        //             Debug.Log("ROW: " + rows + " COL: " + cols + " THINGY " +  FINAL);
        //         }
        //         cols += 1;
        //     }
        //     rows += 1;
        // }
        // Debug.Log("INPUT X: " + x + " INPUT Y: " + y);
        parsedLevel.ElementAt(y).ElementAt(x).RemoveAt(1);
    }

    public override string ToString()
    {
        return "ENEMY X: " + GetX() + " ENEMY Y: " + GetY() + " FACINGDIRECTION: " + facingDirection;
    }
}

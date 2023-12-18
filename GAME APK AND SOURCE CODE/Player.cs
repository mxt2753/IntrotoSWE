using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : GridObject
{
    private const int MAX_HEALTH = 10;

    public static Player Instance {get; private set;}

    private GameObject playerObject;

    private bool selected = false;
    private int MAX_PLAYER_MOVES = 10;
    private int movesLeft;
    private int health;

    private List<GameObject> possibleMoveOjects;
    private List<GridSpace> possibleMoveSelections;
    private List<SatchelItem> inventory;

    public Player(Vector3 position, float cellSize, int x, int y, string printName) : base(position, cellSize, x, y, printName)
    {
        Instance = this;
        health = MAX_HEALTH;
        playerObject = GameObject.Find("PlayerObject");
        playerObject.transform.position = position + new Vector3(0, 0, -1);
        movesLeft = MAX_PLAYER_MOVES;
        inventory = new List<SatchelItem>();
    }

    public void GetPlayerPosition(out int x, out int y)
    {
        x = this.GetX();
        y = this.GetY();
    }

    public void MovePlayer(int x, int y, Vector3 newPosition, GridSpace currentGrid, GridSpace nextGrid)
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
        movesLeft = MAX_PLAYER_MOVES;
    }

    public int GetMovesLeft()
    {
        return movesLeft;
    }

    public void SelectPlayer(GameObject moveHighlight, GameObject itemHighlight, GameObject enemyHighlight, GameObject goalHighlight)
    {
        possibleMoveSelections = Pathfinding.Instance.PossibleMoves();
        possibleMoveOjects = new List<GameObject>();

        foreach(GridSpace possibleMove in possibleMoveSelections)
        {
            Vector3 setPosition = possibleMove.GetPosition();
            GameObject newHighlight = null;
            setPosition.z = -2;
            possibleMove.SetHighlight(true);
            if(possibleMove.GetLayer1() == null)
            {
                newHighlight = GameObject.Instantiate(moveHighlight, setPosition, Quaternion.identity);
            }
            else if(possibleMove.GetLayer1() is SatchelItem)
            {
                newHighlight = GameObject.Instantiate(itemHighlight, setPosition, Quaternion.identity);
            }
            else if(possibleMove.GetLayer1() is Enemy)
            {
                newHighlight = GameObject.Instantiate(enemyHighlight, setPosition, Quaternion.identity);
            }
            else if((possibleMove.GetLayer1() is GoalItem) || ((possibleMove.GetLayer1() is LevelExit) && LevelExit.Instance.IsUnlocked())) 
            {
                newHighlight = GameObject.Instantiate(goalHighlight, setPosition, Quaternion.identity);
            }
            possibleMoveOjects.Add(newHighlight);
        }
        selected = true;
    }

    public List<SatchelItem> GetInventory()
    {
        return inventory;
    }

    public void AddItemToInventory(SatchelItem newItem)
    {
        inventory.Add(newItem);
    }

    public void DeselectPlayer()
    {
        foreach(GameObject currentObject in possibleMoveOjects)
        {
            GameObject.Destroy(currentObject);
        }

        foreach(GridSpace currentHighlight in possibleMoveSelections)
        {
            currentHighlight.SetHighlight(false);
        }

        possibleMoveSelections.Clear();
        possibleMoveOjects.Clear();

        possibleMoveOjects = null;
        possibleMoveSelections = null;
        selected = false;
    }

    public bool IsSelected()
    {
        return selected;
    }

    public override string ToString()
    {
        return "PLAYER X: " + this.GetX() + " PLAYER Y: " + this.GetY();
    }
}

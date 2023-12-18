using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using UnityEngine.SceneManagement;

public class CharacterPathfindingMovementHandler : MonoBehaviour 
{
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject moveHighlight;
    [SerializeField] GameObject enemyHighlight;
    [SerializeField] GameObject objectHighlight;
    [SerializeField] GameObject goalHighlight;

    private const float speed = 40f;

    private int currentPathIndex;

    private List<Vector3> pathVectorList;

    private GridSpace startingGrid;

    private GridSpace beforeLastGrid;

    private void Update() 
    {
        HandleMovement();

        if(Input.GetMouseButtonDown(0)) 
        {
            GridSpace clickedGrid = Pathfinding.Instance.GetGrid().GetGridObject(UtilsClass.GetMouseWorldPosition());

            // Debug.Log(clickedGrid + "Player info X: " + Player.Instance.GetX() + " PLAYER Y: " + Player.Instance.GetY());
            // Debug.Log("clicked grid X: " + clickedGrid.GetX() + "clicked grid Y: " + clickedGrid.GetY());
            // Debug.Log("player selected: " + Player.Instance.IsSelected());

            if(clickedGrid.GetX() == Player.Instance.GetX() && clickedGrid.GetY() == Player.Instance.GetY() && !Player.Instance.IsSelected())
            {
                // Debug.Log("PLAYER SELECTED");
                if(Enemy.AllEnemiesOutOfMoves())
                {
                    Player.Instance.SelectPlayer(moveHighlight, objectHighlight, enemyHighlight, goalHighlight);
                }
            }
            else if(clickedGrid.IsHighlighted())
            {
                GridSpace targetGrid = Pathfinding.Instance.GetGrid().GetGridObject(UtilsClass.GetMouseWorldPosition());
                GridSpace currentGrid = Pathfinding.Instance.GetGrid().GetGridObject(Player.Instance.GetX(), Player.Instance.GetY());
                SetTargetPosition(UtilsClass.GetMouseWorldPosition());
                Player.Instance.DeselectPlayer();
            }
            else if(Player.Instance.IsSelected() && (!clickedGrid.IsHighlighted() || (clickedGrid.GetX() == Player.Instance.GetX() && clickedGrid.GetY() == Player.Instance.GetY())))
            {
                Player.Instance.DeselectPlayer();
            }
        }
    }
    
    private void HandleMovement() {
        if (pathVectorList != null) 
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f) 
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            } 
            else 
            {
                Player.Instance.DecreaseMovesLeft(1);
                currentPathIndex++;

                if (currentPathIndex >= pathVectorList.Count) 
                {
                    Vector3 lastVector3 = pathVectorList[pathVectorList.Count - 1];
                    Pathfinding.Instance.GetGrid().GetXY(lastVector3, out int x, out int y);
                    StopMoving(pathVectorList[pathVectorList.Count - 1], x, y);
                }
            }
        }
    }

    private void StopMoving(Vector3 lastPosition, int x, int y) {
        GridSpace nextGrid = Pathfinding.Instance.GetGrid().GetGridObject(lastPosition);

        if(nextGrid.GetLayer1() is Item)
        {
            Item newItem = (nextGrid.GetLayer1() as Item);
            if(newItem is SatchelItem)
            {
                Player.Instance.AddItemToInventory(newItem as SatchelItem);
            }
            else if(newItem is GoalItem)
            {
                LevelExit.Instance.UnlockExit();
            }

            newItem.PickUp();
            Player.Instance.MovePlayer(nextGrid.GetX(), nextGrid.GetY(), nextGrid.GetPosition(), Pathfinding.Instance.GetGrid().GetGridObject(Player.Instance.GetX(), Player.Instance.GetY()), nextGrid);
        }
        else if(nextGrid.GetLayer1() is Enemy)
        {
            Debug.Log(beforeLastGrid);
            Player.Instance.MovePlayer(beforeLastGrid.GetX(), beforeLastGrid.GetY(), beforeLastGrid.GetPosition(), Pathfinding.Instance.GetGrid().GetGridObject(Player.Instance.GetX(), Player.Instance.GetY()), beforeLastGrid);
            Pathfinding.Instance.SaveLevel();
            BattleSystem.SetEnemyPosition(nextGrid);

            SceneManager.LoadScene(6); //1
        }
        else if(nextGrid.GetLayer1() is LevelExit && LevelExit.Instance.IsUnlocked())
        {
            levels.UnlockNextLevel();
            SceneManager.LoadScene(2);//1
        }
        else
        {
            Player.Instance.MovePlayer(nextGrid.GetX(), nextGrid.GetY(), nextGrid.GetPosition(), Pathfinding.Instance.GetGrid().GetGridObject(Player.Instance.GetX(), Player.Instance.GetY()), nextGrid);
        }

        pathVectorList = null;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        int beforeLast;
        if(pathVectorList.Count >= 2)
        {
            beforeLast = pathVectorList.Count - 2;
            beforeLastGrid = Pathfinding.Instance.GetGrid().GetGridObject(pathVectorList[beforeLast]);
        }
        else
        {
            beforeLastGrid = Pathfinding.Instance.GetGrid().GetGridObject(Player.Instance.GetX(), Player.Instance.GetY());
        }
        Debug.Log("BEFORE LAST: " + beforeLastGrid);
        if (pathVectorList != null && pathVectorList.Count > 1) {
            pathVectorList.RemoveAt(0);
        }
    }

}
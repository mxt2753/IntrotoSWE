using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class GameState : MonoBehaviour
{
    private bool playerTurn;
    private static bool enemiesTurn;

    private void Start()
    {
        playerTurn = true;
        enemiesTurn = false;
    }

    private void Update()
    {   
        UpdateTurns();

        if(enemiesTurn && !AllEnemiesOutOfMoves())
        {
            DoEnemyTurn();
        }
    }

    private void UpdateTurns()
    {
        if(Player.Instance.GetMovesLeft() == 0 && playerTurn)
        {
            playerTurn = false;
            enemiesTurn = true;
            ResetEnemiesMoves();
        }
        else if(enemiesTurn && AllEnemiesOutOfMoves())
        {
            enemiesTurn = false;
            playerTurn = true;
            Player.Instance.ResetMovesLeft();
        }
    }

    private void DoEnemyTurn()
    {
        foreach(Enemy currentEnemy in GetEnemiesList())
        {
            currentEnemy.GetAllMoves();
            currentEnemy.PlayTurn();
        }
    }

    public static bool IsEnemiesTurn()
    {
        return enemiesTurn;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementHandler : MonoBehaviour
{
    private Enemy enemyObject = null;

    private void Update()
    {
        if(enemyObject != null && enemyObject.GetEnemyTurn())
        {
            enemyObject.HandleEnemyMovement();
        }
    }

    public void SetEnemyObject(Enemy enemyObject)
    {
        this.enemyObject = enemyObject;
    }
}

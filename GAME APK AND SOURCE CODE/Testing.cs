using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System.Linq;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    private void Awake()
    {
        pathfinding = new Pathfinding();
    }
}

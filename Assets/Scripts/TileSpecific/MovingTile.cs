using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTile : Tile
{
    private int _count=0;
    private void Awake()
    {
        //TODO
        //Take into consideration the Direction x or y to rotate model correctly (might need a container to not mess up
        //the transform+ direction
    }
    private void OnEnable()
    {
        GameManager.onBotMove += UpdateTurn;
    }

    private void UpdateTurn()
    {
        int distance = Distance;
        while (distance>0)
        {
            //TODO 
            //Add either count and due rest from % to know ehter it goes forwards or backwards
            //or bool returning functions with the same count > 0 || < 0
            transform.position = transform.position + new Vector3(Direction.x, 0, Direction.y);
            distance--;
        }
    }

    private void OnDisable()
    {
        GameManager.onBotMove -= UpdateTurn;
    }

}

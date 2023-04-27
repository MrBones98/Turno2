using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTile : Tile
{
    private int _count=0;
    private MovingTileAnimation _animation;
    private void Awake()
    {
        //TODO
        //Take into consideration the Direction x or y to rotate model correctly (might need a container to not mess up
        //the transform+ direction
        _animation = GetComponent<MovingTileAnimation>();
    }
    private void Start()
    {
        _animation.LightForward();        
    }
    private void OnEnable()
    {
        GameManager.onBotMove += UpdateTurn;
    }

    private void UpdateTurn()
    {
        StartCoroutine(nameof(MovingDelay));
    }
    private IEnumerator MovingDelay()
    {
        yield return new WaitForSeconds(0.5f);
        int distance = Distance;
        while (distance>0)
        {
            if (_count % 2 == 0)
            {
                transform.position = transform.position + new Vector3(Direction.x, 0, Direction.y);
                _animation.LightBack(); 
            }
            else
            {
                transform.position = transform.position + new Vector3(-Direction.x, 0, -Direction.y);
                _animation.LightForward();
            }
            distance--;
        }
        _count++;
    }

    private void OnDisable()
    {
        GameManager.onBotMove -= UpdateTurn;
    }

}

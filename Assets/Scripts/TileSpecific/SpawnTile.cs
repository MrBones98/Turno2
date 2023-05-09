using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : Tile
{
    [SerializeField] private GameObject _bot;
    [SerializeField] private float _offset;

    private void OnEnable()
    {
        GameManager.onGameStarted += SpawnBot;   
    }
    private void SpawnBot()
    {
       GameObject bot = Instantiate(_bot,this.transform.position + new Vector3(0, _offset, 0),Quaternion.identity);
       GameManager.SpawnedObjects.Add(bot);
    }

    private void OnDisable()
    {
        GameManager.onGameStarted -= SpawnBot;
    }
}

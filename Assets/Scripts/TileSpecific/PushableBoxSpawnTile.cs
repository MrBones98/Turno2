using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBoxSpawnTile : Tile
{
    [SerializeField] private GameObject _box;
    [SerializeField] private float _offset;
    private void OnEnable()
    {
        GameManager.onGameStarted += SpawnBox;
    }

    private void SpawnBox()
    {
        GameObject box = Instantiate(_box, transform.position + new Vector3(0,_offset,0), Quaternion.identity);
        box.transform.SetParent(transform, true);
    }

    private void Awake()
    {

        //box.transform.parent = transform;
    }
    private void OnDisable()
    {
        GameManager.onGameStarted -= SpawnBox;
    }
}

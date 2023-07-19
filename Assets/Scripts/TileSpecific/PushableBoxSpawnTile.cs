using Assets.Scripts.Interactables;
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
        GameManager.onObjectsInstantiated += SpawnBox;
    }

    private void SpawnBox()
    {
        GameObject box = Instantiate(_box, transform.position + new Vector3(0,_offset,0), Quaternion.identity);
        //box.transform.SetParent(transform, true);
        GameManager.SpawnedObjects.Add(box);
        GameManager.Interactables.Add(new Vector3(transform.position.x, 0, transform.position.z), box.GetComponent<InteractableObject>());
    }

    private void Awake()
    {

        //box.transform.parent = transform;
    }
    private void OnDisable()
    {
        GameManager.onObjectsInstantiated -= SpawnBox;
    }
}

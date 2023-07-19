using Assets.Scripts.Interactables;
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
        GameManager.onObjectsInstantiated += SpawnBot;   
    }
    private void SpawnBot()
    {
       GameObject bot = Instantiate(_bot,this.transform.position + new Vector3(0, _offset, 0), Quaternion.Euler(0,180,0));
       GameManager.SpawnedObjects.Add(bot);
       GameManager.Interactables.Add(new Vector3(transform.position.x, 0, transform.position.z), bot.GetComponentInChildren<InteractableObject>());
    }

    private void OnDisable()
    {
        GameManager.onObjectsInstantiated -= SpawnBot;
    }
}

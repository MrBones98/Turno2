using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBoxSpawnTile : Tile
{
    [SerializeField] private GameObject _box;
    [SerializeField] private float _offset;
    private void Awake()
    {
        GameObject box = Instantiate(_box, transform.position + new Vector3(0,_offset,0), Quaternion.identity);
        box.transform.parent = transform;
    }
}

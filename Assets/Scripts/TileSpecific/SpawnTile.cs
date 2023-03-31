using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTile : Tile
{
    public static SpawnTile Instance;
    private void Awake()
    {
        if(Instance!= null && Instance != this)
        {
            LevelEditor.Instance.RemoveTileAt(this.gameObject.transform.position);
            Destroy(this.gameObject);
            //Callback to Level editor as to not have null reference exceptions
            //or saving into the lists/dictionary a tile that isn't there.
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

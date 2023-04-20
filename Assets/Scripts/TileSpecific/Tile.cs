using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //SavedLevelObject
    public TileType type;
    public int InteractableID;

    private void Awake()
    {
        //SaveSystem.tiles.Add(this);

        LevelEditor.TilesToSave.Add(this);
    }
}
public enum TypeOfTile
{
    Platform,
    Button,
    Wall,
    SpawnTile,
    Switch,
    Gate
}

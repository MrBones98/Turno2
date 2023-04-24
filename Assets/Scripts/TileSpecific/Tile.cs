using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //SavedLevelObject
    public TileType type;
    public int InteractableID =0;
    public Vector2 Direction;
    public int Distance;

    private void Awake()
    {
        //SaveSystem.tiles.Add(this);

        LevelEditor.TilesToSave.Add(this);
        Direction.Normalize();
    }
}
public enum TypeOfTile
{
    Platform,
    Button,
    Wall,
    SpawnTile,
    LatchMomentary,
    Gate,
    MomentarySwitch,
    Moving
}

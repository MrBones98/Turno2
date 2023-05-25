using Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType type;
    public int InteractableID =0;
    public Vector2 Direction;
    public int Distance;
    public bool StartsActivated = false;
    public bool IsHighlighted = false;
    public bool IsVoidHighlightTile = false;

    private void Awake()
    {
        if(IsVoidHighlightTile == false)
        {
            LevelEditor.TilesToSave.Add(this);
        }
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
    Moving,
    BoxSpawnTile,
    PushableBotSpawnTile
}

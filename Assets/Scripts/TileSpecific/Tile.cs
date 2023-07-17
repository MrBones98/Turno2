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
    public void ReferenceToDictionary()
    {
        GameManager.Instance.AddToTileToDictionary(new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z), this);
        print($"{gameObject.name} + {gameObject.transform.position.x} ");
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

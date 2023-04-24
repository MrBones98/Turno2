using Editor;
using UnityEngine;
[System.Serializable]
public class TileObject
{
    public TileType Type;
    public float[] Position;
    public int InteractableID;
    public float[] Direction;
    public int Distance;
    //TODO
    //add id for referencing
    //bool for Interactable/switcheable/activation blah (check interface pls)
    public TileObject(Tile tile)
    {
        this.Type = tile.type;
        InteractableID = tile.InteractableID;

        Vector3 tilePos = tile.transform.position;
        Position = new float[]
        {
            tilePos.x,
            tilePos.y,
            tilePos.z
        };

        Vector2 movementDirection = tile.Direction;
        Direction = new float[]
        {
            movementDirection.x,
            movementDirection.y,
        };

        Distance = tile.Distance;

    }
}
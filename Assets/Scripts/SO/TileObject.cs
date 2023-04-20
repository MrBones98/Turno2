using Editor;
using UnityEngine;
[System.Serializable]
public class TileObject
{
    public TileType Type;
    public float[] position;
    public int InteractableID;
    //TODO
    //add id for referencing
    //bool for Interactable/switcheable/activation blah (check interface pls)
    public TileObject(Tile tile)
    {
        this.Type = tile.type;
        InteractableID = tile.InteractableID;

        Vector3 tilePos = tile.transform.position;
        position = new float[]
        {
            tilePos.x,
            tilePos.y,
            tilePos.z
        };

    }
}
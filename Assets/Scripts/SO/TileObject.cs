using Editor;
using UnityEngine;
[System.Serializable]
public class TileObject
{
    public TileType Type;
    public float[] position;
    //TODO 
    public TileObject(Tile tile)
    {
        this.Type = tile.type;
        Vector3 tilePos = tile.transform.position;

        position = new float[]
        {
            tilePos.x,
            tilePos.y,
            tilePos.z
        };

    }
}
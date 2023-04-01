using Editor;
using UnityEngine;
[System.Serializable]
public class TileData
{
    public TileType Type;
    public float[] position;
    //TODO 
    public TileData(Tile tile)
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

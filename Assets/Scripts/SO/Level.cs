using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewLevel", menuName = "New Level")]
public class Level : ScriptableObject
{
   [SerializeField] public List<TileObject> tileObjects;
   [SerializeField] public string Name;
   [SerializeField] public int MoveOne, MoveTwo, MoveThree, MoveFour;

    public void ClearTileObjectList()
    {
        tileObjects.Clear();
    }
    public void AddTileObjectType(Tile tileObject)
    {
        var tileObjectInfo = new TileObject(tileObject); 
        tileObjects.Add(tileObjectInfo);
    }
}

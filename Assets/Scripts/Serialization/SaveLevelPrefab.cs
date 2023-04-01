using Editor;
using Sirenix.OdinInspector;
using UnityEngine;
[System.Serializable]
public class SaveLevelPrefab
{

    public TileType Type;
    [LabelText("$Type")]
    public GameObject Prefab;
    public SaveLevelPrefab(TileType tileType)
    {
        this.Type = tileType;
    }
}

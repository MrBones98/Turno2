using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Editor;

public class ScriptableObjectLoader : MonoBehaviour
{
    [SerializeField] private Level _levelToLoad;
    [SerializeField] private List<SaveLevelPrefab> _prefabList = new();
    [SerializeField] private GameObject _levelContainer;
    
    public delegate void LevelLoaded();
    public static event LevelLoaded onLevelLoaded;
    void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        if(_levelToLoad == null)
        {
            Debug.LogError("No Level Object");
            return;
        }

        foreach (TileObject tileObject in _levelToLoad.tileObjects)
        {
            GameObject prefab = null;
            foreach(SaveLevelPrefab levelPrefab in _prefabList)
            {
                if(tileObject.Type == levelPrefab.Type)
                {
                    prefab = levelPrefab.Prefab;
                    break;
                }
            }
            if (prefab == null)
            {
                Debug.LogError("Couldn't find prefab of type: " + tileObject.Type.ToString());
                continue;
            }

            GameObject newTileInstance = Instantiate(prefab, _levelContainer.transform);
            newTileInstance.name = $"X: {newTileInstance.transform.position.x} | Z: {newTileInstance.transform.position.z}";


            newTileInstance.transform.position = new Vector3(tileObject.position[0], tileObject.position[1], tileObject.position[2]);

            foreach (Transform child in _levelContainer.transform)
            {
                GameManager.TileGameObjects.Add(child.gameObject);
            }
        }
        onLevelLoaded();
    }
}

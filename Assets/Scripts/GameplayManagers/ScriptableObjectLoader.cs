using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Editor;
using System.Threading.Tasks;

public class ScriptableObjectLoader : MonoBehaviour
{
    public static ScriptableObjectLoader Instance;

    [SerializeField] private Level _levelToLoad;
    [SerializeField] private GameObject _levelContainer;
    [SerializeField] private List<SaveLevelPrefab> _prefabList = new();
    [SerializeField] private List<GameObject> _cardPrefabs = new();
    [SerializeField] private List<Level> _levels = new();

    private int _index = 0;
    public int Index { get { return _index; }}
    public Level LevelToLoad { get { return _levelToLoad; }}
    public List<Level> Levels { get { return _levels; }}
    public delegate void LevelLoaded();
    public static event LevelLoaded onLevelLoaded;
    public delegate void OnLevelQeued();
    public static event OnLevelQeued onLevelQeued;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        if(_levelToLoad == null)
        {
            LoadNextLevel(true);
        }
    }
    public async void LoadNextLevel(bool increaseIndex)
    {
        if (!increaseIndex)
        {
            _index--;
        }
        else
        {
            _index++;
        }
        await ClearLevelAsync();
        await GameManager.Instance.ClearLevel();
        LoadLevelWithIndex(_index);
    }
    public async Task ClearLevelAsync()
    {
        await Task.Yield();
        onLevelQeued?.Invoke();
        
    }
    public void LoadLevelWithIndex(int index)
    {
        if (index >= 0)
        {
            _levelToLoad = _levels[index];
            if(_levelToLoad == null)
            {
                Debug.LogError("No Level Object");
                return;
            }


            foreach (TileObject tileObject in _levelToLoad.tileObjects)
            {
                GameObject prefab = null;
                foreach (SaveLevelPrefab levelPrefab in _prefabList)
                {
                    if (tileObject.Type == levelPrefab.Type)
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

                newTileInstance.transform.position = new Vector3(tileObject.Position[0], tileObject.Position[1], tileObject.Position[2]);
                newTileInstance.name = $"X: {newTileInstance.transform.position.x} | Z: {newTileInstance.transform.position.z}";
                newTileInstance.GetComponent<Tile>().StartsActivated = tileObject.StartsActivated;
                newTileInstance.GetComponent<Tile>().InteractableID = tileObject.InteractableID;
                newTileInstance.GetComponent<Tile>().Direction = new Vector2(tileObject.Direction[0], tileObject.Direction[1]);
                newTileInstance.GetComponent<Tile>().Distance = tileObject.Distance;

                foreach (Transform child in _levelContainer.transform)
                {
                    GameManager.TileGameObjects.Add(child.gameObject);
                }
            }
            onLevelLoaded();
        }
        else
        {
            Debug.LogError($"Accessing collection at {index}. It must be positive: resetting to 0");
            _index = 0;
        }
    }
}

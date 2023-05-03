using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{ 
    public enum TileType
    {
        Platform,
        Button,
        Wall,
        SpawnTile,
        LatchSwitch,
        Gate,
        MomentarySwitch,
        Moving,
        BoxSpawnTile
    }
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private GridHelper _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _levelContainer;
        [SerializeField] private GameObject[] _tilePrefab;
        [SerializeField] private LayerMask _layerMask;

        [SerializeField]
        [OnInspectorGUI("UpdatePrefabList")]
        private List<SaveLevelPrefab> _prefabList = new();

        [SerializeField]
        [Required]
        [OnValueChanged("LoadLevel")]
        private Level _currentLevel;

        private int _tileIndex = 0;
        private bool _startTest = false;
        private List<GameObject> _runtimeTileObjects = new();


        public static LevelEditor Instance;
        public List<Vector3> RuntimeTilePositions = new();

        public static List<Tile> TilesToSave = new();

        public delegate void StartTest();
        public static event StartTest startTest;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {

                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            Debug.LogWarning("Assign and remove Level SO manually");

        }
        [DisableInEditorMode]
        [ButtonGroup("LevelButtons")]
        public void SaveLevel()
        {
            if(_currentLevel == null)
            {
                Debug.LogError("No Level Object");
            }
            
            _currentLevel.ClearTileObjectList();
            //So we actually add on click for the Data needed to save (different than
            //current transforms)
            Tile[] tiles = FindObjectsOfType<Tile>();
            foreach (var tile in tiles)
            {
                _currentLevel.AddTileObjectType(tile);
            }

            UnityEditor.EditorUtility.SetDirty(_currentLevel);

        }
        [DisableInEditorMode]
        [ButtonGroup("LevelButtons")]
        public void LoadCurrentLevel()
        {
            if(_currentLevel == null || !Application.isEditor)
            {
                Debug.LogError("No Level Object");
                return;
            }
            ClearLevel();
            ReassignRuntimeTileValues();

            foreach (TileObject tileObject in _currentLevel.tileObjects)
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

                GameObject newTileInstance = Instantiate(prefab,_levelContainer.transform);

                newTileInstance.transform.position = new Vector3(tileObject.Position[0], tileObject.Position[1], tileObject.Position[2]);
                newTileInstance.name = $"X: {newTileInstance.transform.position.x} | Z: {newTileInstance.transform.position.z}";
                newTileInstance.GetComponent<Tile>().StartsActivated = tileObject.StartsActivated;
                newTileInstance.GetComponent<Tile>().InteractableID = tileObject.InteractableID;
                newTileInstance.GetComponent<Tile>().Direction = new Vector2(tileObject.Direction[0], tileObject.Direction[1]);
                newTileInstance.GetComponent<Tile>().Distance = tileObject.Distance;

            }
            foreach (Transform child in _levelContainer.transform)
            {
                _runtimeTileObjects.Add(child.gameObject);
                RuntimeTilePositions.Add(child.position);
            }
        }
        public void LoadLevel(Level levelToLoad)
        {
            if (Application.isPlaying)
            {
                this._currentLevel = levelToLoad;
                LoadCurrentLevel();
            }
        }
        [DisableInEditorMode]
        [ButtonGroup("LevelButtons")]
        public void ClearLevel()
        {
            if (Application.isPlaying)
            {
                Tile[] tileObjects = FindObjectsOfType<Tile>();
                foreach (Tile tileObject in tileObjects)
                {
                    if (tileObject == null)
                    {
                        continue;
                    }
                    else
                    {
                        _runtimeTileObjects.Remove(tileObject.gameObject);
                        RuntimeTilePositions.Remove(tileObject.gameObject.transform.position);
                    
                        Destroy(tileObject.gameObject);
                    }
                    
                }

            }
        }

        private void ReassignRuntimeTileValues()
        {
            _runtimeTileObjects.Clear();
            RuntimeTilePositions.Clear();           
        }

        private void Update()
        {
            //yes soon input system
            //if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            if (_startTest == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                    if(Physics.Raycast(ray, out hitInfo,40f,_layerMask))
                    {   
                        PlaceTile(CheckCoordinates(hitInfo.point));
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    RaycastHit hitInfo;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hitInfo,40f, _layerMask))
                    {
                        RemoveTileAt(CheckCoordinates(hitInfo.point));
                    }
                }
            }
            else
            {
                //startTest
                //Need to subscribe from spawntile and perhaps more?
            }


            //??no input detected cool diassapear text, ui, whatever for better look with easing
        }
        /// <summary>
        /// Returns the pointer's position with rounded coordinates
        /// </summary>
        /// <param name="ClickPosition"></param>
        /// <returns></returns>
        private Vector3 CheckCoordinates(Vector3 ClickPosition)
        {
            return new Vector3(Mathf.Round(ClickPosition.x),Mathf.Round(ClickPosition.y),Mathf.Round(ClickPosition.z));
        }
        /// <summary>
        /// Instantiate, name, group and store a new Tile's information or return if the same coordinates are stored
        /// </summary>
        /// <param name="closestPoint"></param>
        private void PlaceTile(Vector3 closestPoint)
        {
            print($"This is where you clicked{closestPoint}");
            
            if (RuntimeTilePositions.Contains(closestPoint))
            {
                Debug.LogWarning($"A tile is already at X:{(int)closestPoint.x} | Z:{(int)closestPoint.z}");
                return;
            }
           
            Vector3 finalPosition = _grid.GetNearestPointOnGrid(closestPoint);

            GameObject tile = Instantiate(_tilePrefab[_tileIndex], finalPosition, Quaternion.identity);
            tile.transform.SetParent(_levelContainer.transform);

            tile.name = $"X: {finalPosition.x} | Z: {finalPosition.z}";

            RuntimeTilePositions.Add(finalPosition);
            _runtimeTileObjects.Add(tile);
            
        }
        
        public void RemoveTileAt(Vector3 tilePosition)
        {
            if (!RuntimeTilePositions.Contains(tilePosition))
            {
                if(RuntimeTilePositions.Count== 0)
                {
                    Debug.Log("Is the Tile not here? or are you not updating the RuntimeTiles from Saved Level");
                }
                Debug.LogWarning($"No tile  at X:{(int)tilePosition.x} | Z:{(int)tilePosition.z}");
                return;
            }
            else
            {
                int index = RuntimeTilePositions.IndexOf(tilePosition);
                RuntimeTilePositions.Remove(tilePosition);
                Destroy(_runtimeTileObjects[index].gameObject);
                _runtimeTileObjects.RemoveAt(index);
                
                
            }
        }
        /// <summary>
        /// Passes index to select tileType
        /// </summary>
        /// <param name="id"></param>
        public void SwitchPrefab(int index)
        {
            //Have to change to enum
            _tileIndex = index;
        }
        public void ChangeEditingState()
        {
            _startTest = !_startTest;
        }
    }
}

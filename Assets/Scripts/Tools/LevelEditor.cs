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
        SpawnTile
    }
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private GridHelper _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _levelContainer;
        [SerializeField] private GameObject[] _tilePrefab;


        private int _tileIndex = 0;
        private int _tileCountDebug;
        private bool _startTest = false;
        
        public static LevelEditor Instance;
        public static List<Vector3> _tilePositions = new();
        public static List<GameObject> _tiles = new();

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
                
                    if(Physics.Raycast(ray, out hitInfo))
                    {   
                        PlaceTile(CheckCoordinates(hitInfo.point));
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    RaycastHit hitInfo;
                    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hitInfo))
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
            //_tileCountDebug= _tilePositions.Count;
            //if(_tilepositions)
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
            
            if (_tilePositions.Contains(closestPoint))
            {
                Debug.LogWarning($"A tile is already at X:{(int)closestPoint.x} | Z:{(int)closestPoint.z}");
                return;
            }
           
            Vector3 finalPosition = _grid.GetNearestPointOnGrid(closestPoint);

            GameObject tile = Instantiate(_tilePrefab[_tileIndex], finalPosition, Quaternion.identity);
            tile.transform.SetParent(_levelContainer.transform);

            tile.name = $"X: {finalPosition.x} | Z: {finalPosition.z}";

            _tilePositions.Add(finalPosition);
            _tiles.Add(tile);
            
        }
        //Make public? Send Event? Ask Pete
        public void RemoveTileAt(Vector3 tilePosition)
        {
            if (!_tilePositions.Contains(tilePosition))
            {
                Debug.LogWarning($"No tile  at X:{(int)tilePosition.x} | Z:{(int)tilePosition.z}");
                return;
            }
            else
            {
                int index = _tilePositions.IndexOf(tilePosition);
                _tilePositions.Remove(tilePosition);
                Destroy(_tiles[index].gameObject);
                _tiles.RemoveAt(index);
                
                
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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{ 
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private GridHelper _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _levelContainer;
        [SerializeField] private GameObject _tilePrefab;

        [SerializeField] private List<Vector3> _tilePositions = new List<Vector3>();
        [SerializeField] private List<GameObject> _tiles = new List<GameObject>();
        
        private void Update()
        {
            //yes soon input system
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
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
            
            if (_tilePositions.Contains(closestPoint))
            {
                Debug.LogWarning($"A tile is already at X:{(int)closestPoint.x} | Z:{(int)closestPoint.z}");
                return;
            }
           
            Vector3 finalPosition = _grid.GetNearestPointOnGrid(closestPoint);

            GameObject tile = Instantiate(_tilePrefab, finalPosition, Quaternion.identity);
            tile.transform.SetParent(_levelContainer.transform);

            tile.name = $"X: {finalPosition.x} | Z: {finalPosition.z}";

            _tilePositions.Add(finalPosition);
            
        }
        private void RemoveTileAt(Vector3 tilePosition)
        {
            if (!_tilePositions.Contains(tilePosition))
            {
                Debug.LogWarning($"No tile  at X:{(int)tilePosition.x} | Z:{(int)tilePosition.z}");
                return;
            }
            else
            {
                //_tilePositions.Remove(tilePosition);
                //Destroy.
                print("I should sleep, deleting tiles will be implemented tomorrow jajajaa");
            }
        }
    }
}

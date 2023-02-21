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
            if (Input.GetMouseButtonDown(0))
            {
                //print('1');
                RaycastHit hitInfo;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                if(Physics.Raycast(ray, out hitInfo))
                {
                    //print('2');

                    PlaceTileNear(hitInfo.point);
                }
            }
        }

        private void PlaceTileNear(Vector3 closestPoint)
        {
            //Cache closestPointValue like look at thath vvvvv that's silly 
            print($"This is where you clicked{closestPoint}");
            
            //look into this when reeimplementing
            //Vector3 roundedCoordinates = new Vector3(Mathf.RoundToInt(closestPoint.x), Mathf.RoundToInt(closestPoint.y), Mathf.RoundToInt(closestPoint.x));
            //round up here and send that bruh
            //if (_tilePositions.Contains(roundedCoordinates))
            if (_tilePositions.Contains(closestPoint))
            {
                Debug.LogWarning($"A tile is already at X:{(int)closestPoint.x} | Z:{(int)closestPoint.z}");
                return;
            }
            //print('3');
            Vector3 finalPosition = _grid.GetNearestPointOnGrid(closestPoint);

            GameObject tile = Instantiate(_tilePrefab, finalPosition, Quaternion.identity);
            tile.transform.SetParent(_levelContainer.transform);

            tile.name = $"X: {finalPosition.x} | Z: {finalPosition.z}";

            _tilePositions.Add(finalPosition);
            _tiles.Add(tile);
            //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position= finalPosition;
        }
    }
}

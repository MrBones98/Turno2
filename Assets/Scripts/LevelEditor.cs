using System;
using UnityEngine;

namespace Editor
{ 
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] private GridHelper _grid;
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _levelContainer;
        private void Awake()
        {
        
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                print('1');
                RaycastHit hitInfo;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out hitInfo))
                {
                    print('2');
                    PlaceTileNear(hitInfo.point);
                }
            }
        }

        private void PlaceTileNear(Vector3 closestPoint)
        {
            print('3');
            Vector3 finalPosition = _grid.GetNearestPointOnGrid(closestPoint);
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position= finalPosition;
        }
    }
}

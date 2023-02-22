using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editor
{ 
    public class GridHelper : MonoBehaviour
    {
        //Size of cell
        [SerializeField] private int _width, _depth;
        [SerializeField] private float _size = 1f;
        public float Size { get { return _size; } }
        /// <summary>
        /// Returns the equivalent position on the created grid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 GetNearestPointOnGrid(Vector3 position)
        {
            position -= transform.position;

            int xCount = Mathf.RoundToInt(position.x/_size);
            int yCount = Mathf.RoundToInt(position.y/_size);
            int zCount = Mathf.RoundToInt(position.z/_size);

            Vector3 result = new Vector3((float)xCount * _size, (float)yCount * _size, (int)zCount * _size);
            result += transform.position;

            return result;
        }
        private void OnDrawGizmos()
        {
            Color color = Color.cyan;
            Gizmos.color = Color.cyan;
            for (float x = 0; x <40 ; x+=_size)
            {
                for (float z = 0; z < 40; z+=_size)
                {
                    var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                    Gizmos.DrawSphere(point, 0.1f);
                }
            }
        }
    }
}

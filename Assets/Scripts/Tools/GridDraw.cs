using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDraw : MonoBehaviour
{
    [SerializeField] private int _minX, _minY, _maxX, _maxY;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Vector3 pos0 = new Vector3();
        Vector3 pos1 = new Vector3();
        for (int i = -_minX; i < _maxX; i++)
        {
            pos0.x = i;
            pos0.z = -_minY;
            pos1.x = i;
            pos1.z = _maxY;

            Gizmos.DrawLine(pos0,pos1);

        }

        for (int i = -_minY; i < _maxY; i++)
        {
            pos0.x = -_minX;
            pos0.z = i;
            pos1.x = _maxX;
            pos1.z = i;
            Gizmos.DrawLine(pos0, pos1);
                
        }
    }
}

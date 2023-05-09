using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class DirectionalInputBot : MonoBehaviour
{
    [SerializeField] private bool _calculate;

    private DirectionIs _direction;
    private Bot _bot;
    private Camera _camera;
    Vector3 _mousePosition;
    Vector3 _screenPosition;

    private void Awake()
    {
        _bot = GetComponent<Bot>();
        _camera = Camera.main;
    }
    
    private Vector3 CalculateMyScreenSpacePosition()
    {

        return _screenPosition = _camera.WorldToScreenPoint(transform.position);
        
    }
    public Vector3 Calculate()
    {
        Vector3 direction = Input.mousePosition - CalculateMyScreenSpacePosition();
        return direction.normalized;
        
    }
    public DirectionIs CalculateQuadrants(Vector3 mouseDirection)
    {
        if(mouseDirection.x < 0)
        {
            if (mouseDirection.y > 0)
            {
                _direction = DirectionIs.PosZ;
            }
            else if(mouseDirection.y < 0)
            {
                _direction = DirectionIs.NegX;
            }
        }
        else if(mouseDirection.x > 0)
        {
            if (mouseDirection.y > 0)
            {
                _direction = DirectionIs.PosX;
            }
            else if (mouseDirection.y < 0)
            {
                _direction = DirectionIs.NegZ;

            }
        }
        //print(_direction);
        return _direction;
    }
}

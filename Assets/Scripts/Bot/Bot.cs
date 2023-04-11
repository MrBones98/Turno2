using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    private Vector3 _lookDirection;
    private Transform _parentTransform = null;
    private bool _isActive = true;
    private Rigidbody _rigidBody;
    private void OnEnable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }
    public void Move(Vector3 direction)
    {
        if(_parentTransform == null)
        {
            _parentTransform = transform.GetComponentInParent<Transform>();
        }
        if (_isActive)
        {
            _lookDirection = (_parentTransform.position +direction )- _parentTransform.position;
            _parentTransform.position += direction;
            //_rigidBody.MovePosition(_parentTransform.position+direction);
            _parentTransform.rotation = Quaternion.LookRotation(_lookDirection);
        }
        //TODO
        //today switch to rigidbody
        //falling off of platforms
        //Quit and Reset   
    }

    public void SwitchState()
    {
        _isActive = !_isActive;
    }
    private void OnDisable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
}

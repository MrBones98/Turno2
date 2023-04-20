using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] [Range(0.5f, 2f)] private float _botStepDelay;

    private int _stepCount;
    private Vector3 _lookDirection;
    private bool _isActive = true;
    private Rigidbody _rigidBody;
    
    private void OnEnable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>(); 
        //_parentTransform = transform.GetComponentInParent<Transform>(); //change to gameobject
    }
    public void Move(Vector3 direction)
    {
        if(_parentGameObject != null)
        {
            
            print(_parentGameObject.gameObject.name);
        }

        if (_isActive)
        {
            Vector3 correctedDirection = direction.normalized;
            _lookDirection = (_parentGameObject.transform.position +direction )- _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);
            for (int i = 0; i < _stepCount; i++)
            {
                RaycastHit hit;
                WallTile wallTile;

                if (Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.TransformDirection(Vector3.forward), out hit, 1))
                {
                    if (hit.transform.GetComponent<WallTile>())
                    {
                        wallTile = hit.transform.GetComponent<WallTile>();

                    }
                    else
                    {
                        wallTile = null;
                    }
                    //_rigidBody.MovePosition(_parentTransform.position+direction);
                }
                else
                {
                    wallTile = null;
                }
                if (wallTile != null)
                {
                    print("no movement, wall in front");
                }
                else
                {
                    StartCoroutine(StepDelay(_botStepDelay));
                    _parentGameObject.transform.position += correctedDirection;
                }
            }
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
    public void SetDistance(int distance)
    {
        _stepCount = distance;
    }
    private void OnDisable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private IEnumerator StepDelay(float botStepDelay)
    {
        yield return new WaitForSeconds(botStepDelay);
    }

}

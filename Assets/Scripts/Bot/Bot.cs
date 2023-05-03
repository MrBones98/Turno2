using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] [Range(0.5f, 2f)] private float _botStepDelay;
    [SerializeField] private float _goundcheckOffset;
    [SerializeField] private LayerMask _platformGroundCheckLayer;
    [SerializeField] private Rigidbody _rigidBody;

    //Can serialize/streamline later
    private string[] _layersToCheck = { "Platform", "Pushable" };
    private int _stepCount;
    private bool _grounded = true;
    private bool _isActive = true;
    private Vector3 _lookDirection;
    private Vector3 _gizmoPosition;

    public delegate void OnFinishedMove();
    public static OnFinishedMove onFinishedMove;
    private void OnEnable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private void Awake()
    {
        //_rigidBody = GetComponent<Rigidbody>(); 
        //_parentTransform = transform.GetComponentInParent<Transform>(); //change to gameobject
    }
    public void Move(Vector3 direction)
    {
        if (_isActive)
        {
            Vector3 correctedDirection = direction.normalized;
            _gizmoPosition = correctedDirection;
            _lookDirection = (_parentGameObject.transform.position + direction) - _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);



                StartCoroutine(SolveTurn(correctedDirection));
        }
        //Quit and Reset   
    }

    public IEnumerator SolveTurn(Vector3 correctedDirection)
    {
        RaycastHit facingHit;
        RaycastHit groundHit;
        WallTile wallTile = null;
        PushableBox box = null;
        int interactableLayers = LayerMask.GetMask(_layersToCheck);
        while(_stepCount > 0)
        {

            //if (Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.TransformDirection(Vector3.forward), out facingHit, 1))
            if(Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.forward, out facingHit,1 ,interactableLayers))
            {
                
                var collision = facingHit.collider.gameObject;
                if(interactableLayers ==(interactableLayers | 1 << collision.layer))
                {
                    
                    if (collision.gameObject.GetComponent<WallTile>())
                    {
                        wallTile = collision.gameObject.GetComponent<WallTile>();
                    }
                    else
                    {
                        wallTile=null;
                    }
                    if (collision.gameObject.GetComponent<PushableBox>())
                    {
                        box = collision.gameObject.GetComponent<PushableBox>();
                    }
                    else
                    {
                        box=null;
                    }
                    //else if(wall tile!= null aka for the check of raycast hit array
                }


            }
            else
            {
                wallTile = null;
                box = null;
            }
            //print(box);
            if (wallTile == null || (wallTile != null && wallTile.HasColision == false))
            {
                //if (!Physics.SphereCast(transform.position, 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                //if (!Physics.SphereCast(transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                if(!Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.forward, out facingHit,1,_platformGroundCheckLayer))
                {
                    _grounded = false;

                }
                else
                {
                    _grounded = true;
                }

            
               
                //TODO
                //EXTRACT THIS INTO SEPARATE FUNCTION
                if (box != null)
                {

                    box.CheckMovementDirection(correctedDirection);
                    
                    if (!box.IsPushable)
                    {
                        print("cant push box");
                    }
                    else
                    {
                        _parentGameObject.transform.position += correctedDirection;
                    }
                }
                else
                {
                    _parentGameObject.transform.position += correctedDirection;
                }
                

                yield return new WaitForSeconds(_botStepDelay); 
                //_rigidBody.MovePosition(transform.position+direction);


                if (!_grounded)
                {
                    //check if it's only becaus ethe first collider was from a box taking the object
                    //ADD Collider aray infor, just loop through the important logic here and in pushableBox
                    print("Dead animation");
                }
            }
            else
            {
                print("no movement, wall in front");
            }
            _stepCount--;
        }
        onFinishedMove();
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

    private IEnumerator StepDelay(float botStepDelay, Vector3 direction)
    {
        yield return new WaitForSeconds(botStepDelay);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position+new Vector3(_gizmoPosition.x, _goundcheckOffset, _gizmoPosition.z), 0.3f);
    }

}

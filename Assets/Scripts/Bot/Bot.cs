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

    private int _stepCount;
    private bool _willIBeGrounded = true;
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
            _lookDirection = (_parentGameObject.transform.position +direction )- _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);
            for (int i = 0; i < _stepCount; i++)
            {


                //while(_stepCount > 0)
                
                    //print(_stepCount);
                    RaycastHit facingHit;
                    RaycastHit groundHit;
                    WallTile wallTile;
                    PushableBox box;

                    if (Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.TransformDirection(Vector3.forward), out facingHit, 1))
                    {
                        //print(facingHit.collider.gameObject.name);
                        if (facingHit.transform.GetComponent<WallTile>())
                        {
                            wallTile = facingHit.transform.GetComponent<WallTile>();

                        }
                        else
                        {
                            wallTile = null;
                        }
                        if (facingHit.transform.GetComponent<PushableBox>())
                        {
                            box = facingHit.transform.GetComponent<PushableBox>();
                        }
                        else
                        {
                            box = null;
                        }

                    }
                    else
                    {
                        wallTile = null;
                        box = null;
                    }
                    print(box);
                    if (wallTile == null || (wallTile != null && wallTile.HasColision == false))
                    {
                        //if (!Physics.SphereCast(transform.position, 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                        if (!Physics.SphereCast(transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                        {
                            _willIBeGrounded = false;

                        }
                        else
                        {
                            _willIBeGrounded = true;
                        }

                        StartCoroutine(StepDelay(_botStepDelay, correctedDirection));
                        //_rigidBody.MovePosition(transform.position+direction);


                        if (box != null)
                        {
                        
                            box.CheckMovementDirection(correctedDirection);
                        }
                        if (!_willIBeGrounded)
                        {
                            print("Bot will fall");
                        }
                    }
                    else
                    {
                        print("no movement, wall in front");
                    }

                
            }
                _stepCount = 0;
                onFinishedMove();
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
    private IEnumerator StepDelay(float botStepDelay, Vector3 direction)
    {
            yield return new WaitForSeconds(botStepDelay);
            _parentGameObject.transform.position += direction;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position+new Vector3(_gizmoPosition.x, _goundcheckOffset, _gizmoPosition.z), 0.3f);
    }

}

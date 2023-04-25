using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] [Range(0.5f, 2f)] private float _botStepDelay;
    [SerializeField] private float _goundcheckOffset;
    [SerializeField] private LayerMask _platformGroundCheckLayer;

    private int _stepCount;
    private Vector3 _lookDirection;
    private bool _willIBeGrounded = true;
    private bool _isActive = true;
    private Rigidbody _rigidBody;
    private Vector3 _gizmoPosition;

    public delegate void OnFinishedMove();
    public static OnFinishedMove onFinishedMove;
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
            //print(_parentGameObject.gameObject.name);
        }

        if (_isActive)
        {
            Vector3 correctedDirection = direction.normalized;
            _gizmoPosition = correctedDirection;
            _lookDirection = (_parentGameObject.transform.position +direction )- _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);
            for (int i = 0; i < _stepCount; i++)
            {
                RaycastHit facingHit;
                RaycastHit groundHit;
                WallTile wallTile;
                PushableBox box;

                if (Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.TransformDirection(Vector3.forward), out facingHit, 1,5))
                {
                    //print(facingHit.collider.gameObject.name);
                    if (facingHit.transform.GetComponentInParent<WallTile>())
                    {
                        wallTile = facingHit.transform.GetComponentInParent<WallTile>();

                    }
                    else
                    {
                        wallTile = null;
                    }
                    if (facingHit.transform.GetComponent<PushableBox>())
                    {
                        box =facingHit.transform.GetComponent<PushableBox>();
                    }
                    else
                    {
                        box =null;
                    }
                    //_rigidBody.MovePosition(_parentTransform.position+direction);
                }
                else
                {
                    wallTile = null;
                    box =null;
                }
                if (wallTile == null || (wallTile!=null && wallTile.HasColision==false))
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
                    //Collider[] platformColliders;
                    //platformColliders =Physics.OverlapSphere(transform.position + new Vector3(_gizmoPosition.x, _goundcheckOffset, _gizmoPosition.z), 0.3f, _platformGroundCheckLayer);

                    //if (platformColliders==null)
                    //{
                    //    _willIBeGrounded = false;

                    //}

                    StartCoroutine(StepDelay(_botStepDelay));
                        _parentGameObject.transform.position += correctedDirection;


                    if (box != null)
                    {
                        box.Move(correctedDirection);
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
    private IEnumerator StepDelay(float botStepDelay)
    {
        yield return new WaitForSeconds(botStepDelay);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position+new Vector3(_gizmoPosition.x, _goundcheckOffset, _gizmoPosition.z), 0.3f);
    }

}

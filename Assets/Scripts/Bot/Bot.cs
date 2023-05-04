using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Bot : MonoBehaviour
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] [Range(0.5f, 2f)] private float _botStepDelay;
    [SerializeField] private float _goundcheckOffset;
    [SerializeField] private LayerMask _platformGroundCheckLayer;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _raycastOrigin;

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
        SolveTurnAsync();
    }
    public void CheckMove(Vector3 direction)
    {
        if (_isActive)
        {
            Vector3 correctedDirection = direction.normalized;
            _gizmoPosition = correctedDirection;
            _lookDirection = (_parentGameObject.transform.position + direction) - _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);



            //SIMPLIFIED GAME LOOP
            //Add WallTile & Pushable Box private references (don't create unless you do var assignment and then return with Task<value>)

            //GameManager ondirection Given to bot "*"

            //raycast // and await for passing the direction to make sure the ray direction is 100% right(?)

            //loop through hits
        
            //Assign null or object value to Pushable/Moveable
        
            //if Pushable/Moveable !=null
            //AWAIT Box => raycast=> loop through hits => if != null => AWAIT for next object (recursive check TODO check out how many task is okay to handle and when to stop them all)
            //else
            //solved Turn Interactions
        
            //Move
        
            //Await for Dottween move call (could be async, first get hits properly recognizing)
        
            //Movement Finished
        
            //MovingPlatform (has been waiting since "*")
        
            //Await moving platform
        
            //set card input back to true
                StartCoroutine(SolveTurn(correctedDirection));
        }
    }
    async void SolveTurnAsync()
    {
        print(Time.captureFramerate);
        await Task.Yield();
        print(Time.captureFramerate);
        //allrighty

    }
    public IEnumerator SolveTurn(Vector3 correctedDirection)
    {
        //RaycastHit[] facingHit;
        RaycastHit groundHit;
        
        WallTile wallTile = null;
        PushableBox box = null;
        bool platformCheck = false;
        int interactableLayers = LayerMask.GetMask(_layersToCheck);
        while(_stepCount > 0)
        {

            ////if (Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.TransformDirection(Vector3.forward), out facingHit, 1))
            //if(Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.forward, out facingHit,1 ,interactableLayers))
                //var collision = facingHit.collider.gameObject;
                //if(interactableLayers ==(interactableLayers | 1 << collision.layer))
                //{
                    
                //    if (collision.gameObject.GetComponent<WallTile>())
                //    {
                //        wallTile = collision.gameObject.GetComponent<WallTile>();
                //    }
                //    else
                //    {
                //        wallTile=null;
                //    }
                //    if (collision.gameObject.GetComponent<PushableBox>())
                //    {
                //        box = collision.gameObject.GetComponent<PushableBox>();
                //    }
                //    else
                //    {
                //        box=null;
                //    }
                    //else if(wall tile!= null aka for the check of raycast hit array
                //}
            //else
            //{
            //    wallTile = null;
            //    box = null;
            //}
            //print(box);
            //RaycastHit[] facingHit =Physics.RaycastAll(_parentGameObject.transform.position,_parentGameObject.transform.forward,1.2f);
            RaycastHit[] facingHit =Physics.RaycastAll(_raycastOrigin.position,_raycastOrigin.forward,0.8f);
            //Physics.OverlapSphere
            yield return new WaitForSeconds(_botStepDelay);
            print($"There are {facingHit.Length} colliders on this step");
            for(int i = 0; i < facingHit.Length; i++)
            {
                if (platformCheck== false && facingHit[i].collider.gameObject.layer ==7)
                {
                    print(facingHit[i].collider.gameObject.layer.ToString());
                    platformCheck = true;
                }
                
                //print(i);
                if (facingHit[i].collider.gameObject.GetComponent<WallTile>())
                {
                    wallTile = facingHit[i].collider.gameObject.GetComponent<WallTile>();
                }
                else if(wallTile == null)
                {
                    wallTile = null;
                }
                if(box == null || facingHit[i].collider.gameObject.GetComponent<PushableBox>())
                {
                    box = facingHit[i].collider.gameObject.GetComponent<PushableBox>();
                }
                  print(facingHit[i].collider.gameObject.name); 
            }
            if (wallTile == null || (wallTile != null && wallTile.HasColision == false))
            {
                //if (!Physics.SphereCast(transform.position, 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                //if (!Physics.SphereCast(transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), 0.3f, transform.position + new Vector3(correctedDirection.x, _goundcheckOffset, correctedDirection.z), out groundHit, _platformGroundCheckLayer))
                //if(!Physics.Raycast(_parentGameObject.transform.position, _parentGameObject.transform.forward, out facingHit,1,_platformGroundCheckLayer))
                //{
                //    _grounded = false;

                //}
                //else
                //{
                //    _grounded = true;
                //}
                if(platformCheck == false)
                {
                    _grounded = false;
                }
                else
                {
                    _grounded = true;
                }
                
                print(_grounded);
               
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
                if (_grounded == false)
                {
                    print("Dead animation");
                }
                yield return new WaitForSeconds(_botStepDelay);
            }
            else
            {
                print("no movement, wall in front");
            }
            platformCheck = false;
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
        Gizmos.DrawRay(_raycastOrigin.position, _raycastOrigin.forward*0.8f);
    }

}

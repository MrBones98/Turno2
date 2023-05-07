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
    private string[] _layersToCheck = { "Platform", "Pushable", "Wall"};
    int _collidableLayers;

    private int _stepCount;
    private bool _grounded = true;
    private bool _isActive = true;
    private bool _platformCached = false;
    private Vector3 _lookDirection;
    private Vector3 _movementDirection;
    private Vector3 _gizmoPosition;
    private WallTile _wallTile= null;
    private PushableBox _pushableBox = null;

    public delegate void OnFinishedMove();
    public static OnFinishedMove onFinishedMove;
    private void OnEnable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private void Awake()
    {
        //SolveTurnAsync();
        _collidableLayers = LayerMask.GetMask(_layersToCheck);
    }
    //Could make this async and wait for it on Game Manager
    public void CheckMove(Vector3 direction)
    {
        if (_isActive)
        {
            _movementDirection = direction;
            Vector3 correctedDirection = direction.normalized;
            _gizmoPosition = correctedDirection;
            _lookDirection = (_parentGameObject.transform.position + direction) - _parentGameObject.transform.position;
            _parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);

            SolveTurnAsync(direction);  
        }
    }
    //Can remove direction parameter here
    async Task SolveCollisionsAsync(Vector3 direction)
    {
        await Task.Delay((int)(_botStepDelay)*1000);

        RaycastHit[] facingHit = Physics.SphereCastAll(_raycastOrigin.position, 0.45f, _raycastOrigin.up,1.5f, _collidableLayers); 
        //RaycastHit[] facingHit = Physics.RaycastAll(_raycastOrigin.position,-_raycastOrigin.transform.up, 2f,_collidableLayers);
        //Collider[] facingHit = Physics.OverlapSphere(_raycastOrigin.position, 0.51f, _collidableLayers);
        for (int i = 0; i < facingHit.Length; i++)
        {
            if (_platformCached == false && facingHit[i].collider.GetComponent<Collider>().gameObject.layer == 7)
            {
                _platformCached = true;
            }
            if (facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<WallTile>())
            {
                _wallTile = facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<WallTile>();
            }
            else if (_wallTile == null)
            {
                _wallTile = null;
            }
            if (_pushableBox == null || facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<PushableBox>())
            {
                _pushableBox = facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<PushableBox>();
            }
        }
    }
    async Task SolveMovementAsync(WallTile walltile,bool platformCached, PushableBox pushablebox, Vector3 direction)
    {
        //print($"In the {direction} direction there are: WallTile = {walltile}, Box = {pushablebox}, Platform in front = {platformCached}");
        await Task.Delay((int)_botStepDelay*1000);
        if(walltile == null ||( walltile!= null && !walltile.HasColision))
        {
            if(platformCached)
            {
                _grounded = true;
            }
            else 
            {
                _grounded=false;
            }
            if(_wallTile!=null && !walltile.HasColision)
            {
                _grounded = true;
            }
            if(pushablebox != null)
            {
                //Move Bot with direction
                //pushablebox.CheckMovementDirection(direction);
                await pushablebox.SolveTurnAsync(direction);
                if (!pushablebox.IsPushable)
                {
                    print("can't move or push box");
                }
                else
                {
                    _parentGameObject.transform.position += direction;

                }
            }
            else
            {
                //Move Bot with direction
                _parentGameObject.transform.position += direction;

            }
            if (_grounded == false)
            {
                //do another await for if (box=> will be platform/became platform)
                //Dead Anim
                
                print("No platform underneath Bot => Death after movement");
            }
        }
        else
        {
            print($"No movement in {direction}, a wall blocks the path!");
        }
    }
    async void SolveTurnAsync(Vector3 direction)
    {

        while(_stepCount > 0)
        {
            var solveCollisionsTask = SolveCollisionsAsync(direction);
            await solveCollisionsTask;
            var solveMovementTask = SolveMovementAsync(_wallTile,_platformCached, _pushableBox, direction);
            await solveMovementTask;
            _stepCount--;
            _wallTile = null;
            _pushableBox = null;
            _platformCached = false;
        }
        //Move await=>loop through hits
        onFinishedMove();
        
        //so 2 functions for resolution
        //1 for check move (the one that has both calls for them and awaits move result before going through the next bot._stepCount
        
        //allrighty

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
        //Gizmos.DrawSphere(_raycastOrigin.position, 0.40f);
        Gizmos.DrawRay(_raycastOrigin.position, -_raycastOrigin.transform.up);
    }

}

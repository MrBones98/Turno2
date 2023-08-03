using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using Utils;
using Editor;
using Assets.Scripts.Utils;
using Assets.Scripts.Interactables;

public class Bot : MonoBehaviour, IMovable,IPushable
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] private float _goundcheckOffset;
    [SerializeField] private LayerMask _platformGroundCheckLayer;
    [SerializeField] private LayerMask _highlightPathLayer;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private bool _isPushable;
    [SerializeField] private float _highlightHeight;

    public bool IsPushableBot { get { return _isPushable; } }
    public bool CanBePushed { get { return _canBePushed; } }
    public bool IsFocused { get { return _isFocused; } set { _isFocused = value; } }
    public GameObject ParentGameObject { get { return _parentGameObject; } }
    public int StepCount { get { return _stepCount; } set { _stepCount = value; } }
    public bool IsMoving { get { return _isMoving; } }

    private string[] _layersToCheck = { "Platform", "Pushable", "Wall", "Player" };
    private float _originalHeightBotHighlight;
    private float _botStepDelay;
    private float _botStepSpeed;
    private float _rotationSpeed;
    private float _fallSpeed;
    int _collidableLayers;
    private int _stepCount;
    private bool _jumpInput;
    private bool _isFocused;
    private bool _canBePushed;
    private bool _isMoving;
    private bool _grounded = true;
    private bool _isActive = true;
    private bool _platformCached = false;
    private Vector3 _movementDirection;
    private WallTile _wallTile = null;
    private PushableBox _pushableBox = null;
    private Bot _pushableBot = null;

    public delegate void OnFinishedMove();
    public static event OnFinishedMove onFinishedMove;
    public delegate void OnStartedMove();
    public static event OnStartedMove onStartedMove;
    public delegate void OnBotStep();
    public static event OnBotStep onBotStep;
    public delegate void OnBotLanded();
    public static event OnBotLanded onBotLanded;
    public delegate void OnBotDeath();
    public static event OnBotDeath onBotDeath;
    private void OnEnable()
    {
        WinTile.onButtonPressed += SwitchState;

    }
    private void Awake()
    {
        _collidableLayers = LayerMask.GetMask(_layersToCheck);
        _botStepDelay = Tweener.Instance.BotStepDelay;
        _rotationSpeed = Tweener.Instance.BotRotationSpeed;
        _botStepSpeed = Tweener.Instance.BotStepSpeed;
        _originalHeightBotHighlight = transform.position.y;
        _highlightHeight = Tweener.Instance.BotHighlightHeight;
        _fallSpeed = Tweener.Instance.BotFallSpeed;
        //_directionalInputBot = GetComponent<DirectionalInputBot>();
    }
    private void Update()
    {
        //print(IsMoving);
    }
    //Could make this async and wait for it on Game Manager
    public async void CheckMove(Vector3 direction)
    {
        //Change Direction into EnumDirection is here, change parameter for SolveCollision/SolveMovement *vector3 direction*
        if (_isActive)
        {
           onStartedMove?.Invoke();
            _movementDirection = direction;
            //_parentGameObject.transform.rotation = Quaternion.LookRotation(_lookDirection);
            var solveRotationTask = SolveRotationOrientation(direction);
            //_parentGameObject.transform.DOLookAt(_lookDirection, _rotationSpeed, AxisConstraint.None);
            await solveRotationTask;
            SolveTurnAsync(direction);
        }
    }
    public async void CheckJump(Vector3 direction)
    {
        if (_isActive)
        {
            onStartedMove?.Invoke();
            _movementDirection = direction * _stepCount;
            var solveRotationTask = SolveRotationOrientation(direction);

            await solveRotationTask;
        }
    }
    private async Task SolveRotationOrientation(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation((_parentGameObject.transform.position + direction) - _parentGameObject.transform.position);
        _parentGameObject.transform.DORotateQuaternion(targetRotation, _rotationSpeed);
        await Task.Delay(500);
    }

    public RaycastHit[] PlatformsToRaise(Vector3 orientation)
    {
        return Physics.RaycastAll(_parentGameObject.transform.position, orientation, _stepCount, _highlightPathLayer);
    }
    async Task SolvePushCollisionsAsync(Vector3 direction)
    {
        //if (IsFocused) //Debug purposes, delete
        await Task.Delay((int)(_botStepDelay) * 1000);
        FindInDictionaries(direction);

        
        //GameManager.Interactables.Remove(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z));
        //Old Raycast System
        //Vector3 correctedPushDirection = new Vector3(_parentGameObject.transform.position.x, -1, _parentGameObject.transform.position.z) + direction*1.35f;
        //Collider[] hits = Physics.OverlapSphere(transform.position + direction, 0.44f, _collidableLayers);
        ////RaycastHit[] hits = Physics.SphereCastAll(correctedPushDirection, 0.44f, Vector3.up, 1.5f, _collidableLayers);
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    if (_platformCached == false && hits[i].GetComponent<Collider>().GetComponent<Collider>().gameObject.layer == 7)
        //    {
        //        _platformCached = true;
        //    }
        //    else if(!_platformCached)
        //    {
        //        _platformCached = false;
        //    }
        //    if (hits[i].GetComponent<Collider>().GetComponent<Collider>().gameObject.GetComponent<WallTile>())
        //    {
        //        _wallTile = hits[i].GetComponent<Collider>().GetComponent<Collider>().gameObject.GetComponent<WallTile>();
        //    }
        //    else if (_wallTile == null)
        //    {
        //        _wallTile = null;
        //    }
        //    if (_pushableBox == null || hits[i].GetComponent<Collider>().GetComponent<Collider>().gameObject.GetComponent<PushableBox>())
        //    {
        //        _pushableBox = hits[i].GetComponent<Collider>().GetComponent<Collider>().gameObject.GetComponent<PushableBox>();
        //    }

        //    if (hits[i].GetComponent<Collider>().GetComponent<Bot>())
        //    {
        //        _pushableBot = hits[i].GetComponent<Collider>().GetComponent<Bot>();
        //        //if (!_pushableBot.IsPushableBot)
        //        //{
        //        //    _pushableBot = null;
        //        //}
        //    }
        //    print(hits[i].GetComponent<Collider>().gameObject.name);
        //}
        //    print($"There are {hits.Length} colliders on this step");
    }
    async Task SolveCollisionsAsync(Vector3 direction, bool jump)
    {
        await Task.Delay((int)(_botStepDelay * 1000));
        //Dictionary
        FindInDictionaries(direction);
        print($"In the {direction} direction there are: WallTile = {_wallTile}, Box = {_pushableBox}, Platform in front = {_platformCached}, PushableBot = {_pushableBot}");
        //Old Raycast system

        //RaycastHit[] facingHit;
        //if (jump == false)
        //{
        //     facingHit= Physics.SphereCastAll(_raycastOrigin.position, 0.44f, _raycastOrigin.up, 1.5f, _collidableLayers);
        //}
        //else
        //{
        //    Vector3 jumpRayPosition = _parentGameObject.transform.position+ (direction*_stepCount);
        //    facingHit = Physics.SphereCastAll(jumpRayPosition,0.44f,Vector3.up, 1.5f, _collidableLayers);
        //}
        //for (int i = 0; i < facingHit.Length; i++)
        //{
        //    if (_platformCached == false && facingHit[i].collider.GetComponent<Collider>().gameObject.layer == 7)
        //    {
        //        _platformCached = true;
        //    }
        //    else if (!_platformCached)
        //    {
        //        _platformCached = false;
        //    }
        //    if (facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<WallTile>())
        //    {
        //        _wallTile = facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<WallTile>();
        //        print(_wallTile);
        //    }
        //    else if (_wallTile == null)
        //    {
        //        _wallTile = null;
        //    }
        //    if (_pushableBox == null || facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<PushableBox>())
        //    {
        //        _pushableBox = facingHit[i].collider.GetComponent<Collider>().gameObject.GetComponent<PushableBox>();
        //    }

        //    if (facingHit[i].collider.GetComponent<Bot>())
        //    {
        //        _pushableBot = facingHit[i].collider.GetComponent<Bot>();
        //        //print($"Bot in front pushable: {_pushableBot.IsPushableBot}");

        //    }
        //    //print($"Collisions on Step #{_stepCount} , collider of: {facingHit[i].collider.gameObject.name}, at Pos: {facingHit[i].collider.gameObject.transform.position}");
        //}

        //if (facingHit.Length == 0)
        //{
        //    _platformCached = false;
        //}
    }

    private void FindInDictionaries(Vector3 direction)
    {
        Tile tile = GameManager.Instance.FindTile(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z) + direction);

        InteractableObject interactable = GameManager.Instance.FindInteractable(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z) + direction);

        if (tile == null)
        {
            print("Tile is null");
            _platformCached = false;
            _wallTile = null;
        }
        else
        {
            TileType type = tile.type;
            switch (type)
            {
                case TileType.Platform:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Button:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Wall:
                    _wallTile = tile.GetComponent<WallTile>();
                    break;
                case TileType.SpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.LatchSwitch:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Gate:
                    break;
                case TileType.MomentarySwitch:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Moving:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.BoxSpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.PushableBotSpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                default:
                    break;
            }

        }
        if (interactable == null)
        {
            _pushableBot = null;
            _pushableBox = null;
        }
        else
        {
            TypeOfInteractableObject type = interactable.Type;
            switch (type)
            {
                case TypeOfInteractableObject.PushableBot:
                    _pushableBot = interactable.GetComponent<Bot>();
                    _pushableBox = null;
                    break;
                case TypeOfInteractableObject.PushableBox:
                    _pushableBox = interactable.GetComponent<PushableBox>();
                    _pushableBot = null;
                    break;
                case TypeOfInteractableObject.Bot:
                    _pushableBot = interactable.GetComponent<Bot>();
                    _pushableBox = null;
                    break;
            }
        }
    }

    async Task SolveMovementAsync(WallTile walltile, bool platformCached, PushableBox pushablebox, Vector3 direction, Bot pushableBot)
    {
        //print($"In the {direction} direction there are: WallTile = {walltile}, Box = {pushablebox}, Platform in front = {platformCached}, PushableBot = {pushableBot}");
        await Task.Delay((int)_botStepDelay * 1000);
        if (walltile == null || (walltile != null && !walltile.HasColision))
        {
            //do null check for pushable bot and then for is pushable INSIDE of the wall tile chek for blocking movement
            if (platformCached)
            {
                _grounded = true;
            }
            else 
            {
                _grounded=false;
            }
            //Debug purposes, delete
                //print(platformCached);
            if(_wallTile!=null && !walltile.HasColision)
            {
                _grounded = true;
            }
            if (pushablebox != null)
            {
                //Move Bot with direction
                //pushablebox.CheckMovementDirection(direction);
                await pushablebox.SolveTurnAsync(direction);
                if (!pushablebox.IsPushable)
                {
                    _canBePushed = false;
                        //print("can't move or push box");
                }
                else
                {
                    _canBePushed = true;
                    _grounded = true;
                    //_parentGameObject.transform.position += direction;
                    if (_jumpInput == false)
                    {
                        Move(direction);
                    }
                    else
                    {
                        await Jump(direction);
                    }
                }
            }
            else if (pushableBot != null)
            {
                if (pushableBot.IsPushableBot)
                {
                    await pushableBot.SolvePushAsync(direction);
                    if (!pushableBot.CanBePushed)
                    {
                        _canBePushed = false;
                            print("Can't push Bot");
                    }
                    else
                    {
                        //print("bot can be pushed");
                        _canBePushed = true;
                        //_parentGameObject.transform.position += direction;
                        if(_jumpInput == false)
                        {
                            Move(direction);
                            print("moving while pushing bot");
                        }
                        else
                        {
                            await Jump(direction);
                        }
                    }

                }
                else
                {
                    //print("can't push not pushable bot");
                    return;
                }

            }
            else
            {
                //Move Bot with direction
                if (IsFocused)
                    //print("box and bot are null");
                _canBePushed = true;
                if(_jumpInput == false)
                {
                    Move(direction);   
                }
                else
                {
                    await Jump(direction);
                }
            }
            
            if (_grounded == false)
            {
                //do another await for if (box=> will be platform/became platform)
                //Dead Anim
                //print("No platform underneath Bot => Death after movement");
                transform.gameObject.GetComponent<BoxCollider>().enabled = false;
                onBotDeath?.Invoke();
                _parentGameObject.transform.DOMoveY(-10f, _fallSpeed).SetEase(Ease.InBack);
                //Destroy?
                _stepCount = 0;
                
            }
        }
        else
        {
            _canBePushed = false;
            //print($"No movement in {direction}, a wall blocks the path!");
        }
        await Task.Yield();
    }
    private void Move(Vector3 direction)
    {
        onBotStep?.Invoke();
        GameManager.Interactables.Remove(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z));
        if (IsPushableBot)
        {
            _canBePushed = true;
        }
        _parentGameObject.transform.DOMove(_parentGameObject.transform.position + direction, _botStepSpeed);
        GameManager.Interactables.TryAdd(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z)+ direction, gameObject.GetComponent<InteractableObject>());

        //print("Bot Moved!");
    }
    private async Task Jump(Vector3 direction)
    {
        //HOW PARABLE
        //_parentGameObject.transform.DOMove()
        transform.DOMoveY(_highlightHeight * 2.8f, 0.3f, false);
        await Task.Delay(100);
        _parentGameObject.transform.DOMove(_parentGameObject.transform.position + direction * _stepCount, _botStepSpeed);
        //print("Bot Jumped");
    }
    async void SolveTurnAsync(Vector3 direction)
    {
        _isFocused = false;
        _isMoving = true;
        if(_jumpInput== false)
        {
            while (_stepCount > 0)
            {
                var solveCollisionsTask = SolveCollisionsAsync(direction,_jumpInput);
                await solveCollisionsTask;
                var solveMovementTask = SolveMovementAsync(_wallTile,_platformCached, _pushableBox, direction, _pushableBot);
                await solveMovementTask;
                //print(_stepCount);
                _stepCount--;
                _wallTile = null;
                _pushableBox = null;
                _pushableBot = null;
                _platformCached = false;
            }
            //print(GameManager.Interactables.TryGetValue(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z), out InteractableObject interactable));
        }
        else
        {
            var solveCollisionsTask = SolveCollisionsAsync(direction, _jumpInput);
            await solveCollisionsTask;
            var solveMovementTask = SolveMovementAsync(_wallTile,_platformCached,_pushableBox,direction, _pushableBot);
            await solveMovementTask;
            _stepCount = 0;
            _wallTile = null;
            _pushableBox = null;
            _pushableBot = null;
            _platformCached = false;
        }
        //transform.position -= new Vector3(0, 0.2f, 0);

        //Expose HeighleightHeight
        transform.DOMoveY(0.45f, 0.3f, false);

        onFinishedMove();
        //print(GameManager.Interactables.ContainsKey(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z)));
        _isMoving = false;
        
    }
    public async Task CheckForLanding()
    {
        while(_parentGameObject.transform.position.y> 0.5f)
        {
            await Task.Yield();
        }
        onBotLanded?.Invoke();
    }
   
    public async Task SolvePushAsync(Vector3 direction)
    {
        _isMoving = true;
        var solvePushCollisionsTask = SolvePushCollisionsAsync(direction);
        await solvePushCollisionsTask;
        var solveMovementTask = SolveMovementAsync(_wallTile, _platformCached, _pushableBox, direction, _pushableBot);
        await solveMovementTask;
        //GameManager.Instance.AddInteractableToDictionary(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z),gameObject.GetComponent<InteractableObject>());
        print(new Vector3(_parentGameObject.transform.position.x, 0, _parentGameObject.transform.position.z));
        _wallTile = null;
        _pushableBot=null;
        _pushableBox=null;
        _platformCached=false;
        _isMoving = false;
    }
    public void SwitchState()
    {
        //bruh
        _isActive = !_isActive;
    }
    public void SetDistance(int distance)
    {
        _stepCount = distance;
        //transform.position += new Vector3(0, 0.2f, 0);
        transform.DOMoveY(_highlightHeight, 0.3f,false);
        _jumpInput = false;
        _isFocused = true;
    }
    public void SetJumpDistance(int distance)
    {
        _stepCount = distance;
        transform.DOMoveY(_highlightHeight, 0.3f,false);
        _jumpInput = true;
        _isFocused = true;

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
        //Gizmos.DrawRay(_raycastOrigin.position + new Vector3(0, 0, 1),_raycastOrigin.position + new Vector3(0, 5, 1));
        //Gizmos.DrawSphere(_raycastOrigin.position, 0.40f);
        //Gizmos.DrawRay(_raycastOrigin.position, -_raycastOrigin.transform.up);
        //Gizmos.DrawRay(_parentGameObject.transform.position, MoveUtils.SetDirection(_raisingPathDirection));
    }

    public async void CheckMovement(Vector3 direction)
    {
        //interface
        //throw new System.NotImplementedException();
        if (_isActive)
        {
            onStartedMove?.Invoke();
            _movementDirection = direction;
            var solveRotationTask = SolveRotationOrientation(direction);
            await solveRotationTask;
            SolveDictionaryTurnAsync(direction);
        }
    }
    async void SolveDictionaryTurnAsync(Vector3 direction)
    {
        _isFocused = false;
        _isMoving = true;
        if (_jumpInput == false)
        {
            while (_stepCount > 0)
            {
                //        var solveCollisionsTask = SolveCollisionsAsync(direction, _jumpInput);
                //        await solveCollisionsTask;
                //        var solveMovementTask = SolveMovementAsync(_wallTile, _platformCached, _pushableBox, direction, _pushableBot);
                //        await solveMovementTask;
                //        //print(_stepCount);
                //        _stepCount--;
                //        _wallTile = null;
                //        _pushableBox = null;
                //        _pushableBot = null;
                //        _platformCached = false;
            }
        }
        else
        {
        //    var solveCollisionsTask = SolveCollisionsAsync(direction, _jumpInput);
        //    await solveCollisionsTask;
        //    var solveMovementTask = SolveMovementAsync(_wallTile, _platformCached, _pushableBox, direction, _pushableBot);
        //    await solveMovementTask;
        //    _stepCount = 0;
        //    _wallTile = null;
        //    _pushableBox = null;
        //    _pushableBot = null;
        //    _platformCached = false;
        }
        ////transform.position -= new Vector3(0, 0.2f, 0);

        ////Expose HeighleightHeight
        //transform.DOMoveY(0.45f, 0.3f, false);

        //onFinishedMove();
        //_isMoving = false;

    }

    public void CheckJumping(Vector3 direction)
    {
        //interface
        throw new System.NotImplementedException();
    }

    public void CheckPushing(Vector3 direction)
    {
        //interface
        throw new System.NotImplementedException();
    }
}

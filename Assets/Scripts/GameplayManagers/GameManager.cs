using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<GameObject> TileGameObjects = new();
    public static List<GameObject> Cards = new();
    public static List<GameObject> SpawnedObjects = new();

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private float _highlightHeight;
    [SerializeField] private GameObject _deathPlatformVisual;
    [SerializeField] [Range (0.5f, 3.0f)] private float _rainInDuration;
    [SerializeField] private LayerMask _highlightPathLayer;
    [SerializeField] private GameObject _botParentGameObject;


    //[OnValueChanged("AssignPlayer")]
    private GameObject _bot;
    private GameObject _voidHighlightPlatformReference = null;
    private Camera _camera;
    private DirectionalInputBot _directionalInputBot;
    private List<Transform> _highlightedPath = new();
    private List<Transform> _higlightedInteractables = new();
    private RaycastHit _hit;
    private Ray _interactableRay;
    private DirectionIs _raisingPathDirection;
    private int _currentBotStepCount;
    private int _voidDistance;
    private int _idReference;
    private bool _selectCheck = false;
    private bool _raycastCheck =false;
    //public WinTile WinTile;

    //ON THE LEVEL SO ADD COUNT OF BUTTONS FOR WINNING FOR DIFFERENT NEEED AMOUNTS

    public delegate void OnObjectsInstantiated();
    public static event OnObjectsInstantiated onObjectsInstantiated;
    public delegate void OnGameStart();
    public static event OnGameStart onGameStarted;
    public delegate void OnBotMove();
    public static event OnBotMove onBotMove;
    public delegate void OnUndoButtonPressed();
    public static event OnUndoButtonPressed onUndoButtonPressed;
    public delegate void OnBotDirectionSelected();
    public static event OnBotDirectionSelected onBotDirectionSelected;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadLevel;
        ScriptableObjectLoader.onLevelQeued +=async ()=>await  ClearLevel();
        WinTile.onButtonPressed += FinishLevel;
        SwitchTile.onSwitchPressed += Activate;
        SwitchTile.onSwitchReleased += DeActivate;
        Bot.onFinishedMove += UpdateTurn;
        SwitchTile.onSwitchHighlighted += HighlightInteractable;
        WallTile.onWallHighlighted += HighlightInteractable;
        Bot.onStartedMove += CleanVisualOnBotMove;
    }

    private async void CleanVisualOnBotMove()
    {
        await ClearPath();
    }

    private void HighlightInteractable(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if (tiles.GetComponent<Tile>().InteractableID == id && tiles.GetComponent<Tile>().IsHighlighted == false)
            {
                //print(tiles.name);
               
                    //tiles.GetComponent<ISwitchActivatable>().HighlightInteractable(_highlightHeight);
                    _higlightedInteractables.Add(tiles.transform);
                    tiles.GetComponent<Tile>().IsHighlighted = true;
                
            }
        }
        foreach (Transform interactable in _higlightedInteractables)
        {

            //await PrettyHighlightAsync(interactable,true,_highlightHeight);
            interactable.transform.DOMoveY(_highlightHeight, 0.3f, false);

        }
        //_selectCheck = !_selectCheck;
    }
    private async Task PrettyHighlightAsync(Transform interactableTransform,bool up,float offset)
    {
        float height = offset;
        if (up==false)
        {
            height = 0;
        }

        interactableTransform.DOMoveY(height, 0.3f, false);
        await Task.Delay(100);
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else 
        { 
            Destroy(gameObject);
        }
    }
    public async Task ClearLevel()
    {
        if(SceneLoader.Instance.GetCurrentSceneIndex() == 1)
        {
            await UnloadLevel();
        }
        if (TileGameObjects.Count != 0)
        {
            for (int i = TileGameObjects.Count - 1; i >= 0; i--)
            {
                Destroy(TileGameObjects[i]);
            }
            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                Destroy(Cards[i]);
            }
            for (int i = SpawnedObjects.Count - 1; i >= 0; i--)
            {
                Destroy(SpawnedObjects[i]);
            }
            TileGameObjects.Clear();
            Cards.Clear();
            SpawnedObjects.Clear();
            Destroy(_voidHighlightPlatformReference);
        }
        await Task.Yield();
    }

    private async Task UnloadLevel()
    {
        if (TileGameObjects.Count > 0)
        {
            await RainOutAnimation();
        }
    }

    private void UpdateTurn()
    {

        Invoke(nameof(BotMoved), 0.3f);
    }
    private void BotMoved()
    {
        onBotMove?.Invoke();
    }

    private void DeActivate(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if (tiles.GetComponent<Tile>().InteractableID == id)
            {
                tiles.GetComponent<ISwitchActivatable>().Deactivate();
            }
        }
    }

    private void Activate(int id)
    {
        foreach (GameObject tiles in TileGameObjects)
        {
            if(tiles.GetComponent<Tile>().InteractableID == id)
            {
                tiles.GetComponent<ISwitchActivatable>().Activate();
            }
        }
    }
    public async Task Resetlevel()
    {
        await ClearLevel();
        await Task.Yield();
    }
    public async Task UndoPressed()
    {
        onUndoButtonPressed?.Invoke();
        await Task.Yield();
        int countReference = _bot.GetComponent<Bot>().StepCount;
        if (_bot != null && countReference>0)
        {
            //remove focused
            _bot.GetComponent<Bot>().IsFocused = false;
            //Check based ond StepCount or reference to given Card (Set !active when given Distance to player, it destroys itself after the movement?)
            CardHandManager.Instance.DebugGiveMoveCard(countReference);
            //set step count to 0
            _bot.GetComponent<Bot>().StepCount = 0;

        }
        if(_voidHighlightPlatformReference!= null)
        {
            Destroy(_voidHighlightPlatformReference);
        }
    }
    private void FinishLevel()
    {
        //event for the gamespaceuicontroller to load WinScreen
        //_winScreen.SetActive(true);
        //_gameplayUI.SetActive(false);
    }
    private void UpdateInteractableRayCast()
    {
        _interactableRay = _camera.ScreenPointToRay(Input.mousePosition);

    }

    public  async void GiveChosenBotDirection(DirectionIs directionIs)
    {
        Vector3 moveVector;

        if(directionIs == DirectionIs.PosX)
        {
            moveVector = Vector3.right;
        }
        else if(directionIs==DirectionIs.NegZ)
        {
            moveVector = -Vector3.forward;
        }
        else if(directionIs ==DirectionIs.NegX)
        {
            moveVector = -Vector3.right;
        }
        else if (directionIs == DirectionIs.PosZ)
        {
            moveVector = Vector3.forward;
        }
        else
        {
            return;
        }

        var clearPathTask = ClearPath();
        await clearPathTask;
        _bot.GetComponent<Bot>().CheckMove(moveVector);
    }
    public  async void GiveChosenBotJumpDirection(DirectionIs directionIs)
    {
        Vector3 moveVector;

        if(directionIs == DirectionIs.PosX)
        {
            moveVector = Vector3.right;
        }
        else if(directionIs==DirectionIs.NegZ)
        {
            moveVector = -Vector3.forward;
        }
        else if(directionIs ==DirectionIs.NegX)
        {
            moveVector = -Vector3.right;
        }
        else if (directionIs == DirectionIs.PosZ)
        {
            moveVector = Vector3.forward;
        }
        else
        {
            return;
        }
        onBotDirectionSelected?.Invoke();
        var clearPathTask = ClearPath();
        await clearPathTask;
        _bot.GetComponent<Bot>().CheckMove(moveVector);
    }

    void Update()
    {

        if (_bot != null)
        {
            if (_bot.GetComponent<Bot>().IsFocused)
            {
                ShowDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                if (Input.GetMouseButtonDown(0))
                {
                    GiveChosenBotDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                    if(_voidHighlightPlatformReference != null)
                    {
                        Destroy(_voidHighlightPlatformReference);
                    }
                }
            }
        }
        //TODO Check for Scene
        if (_bot == null ||(_bot!= null && ! _bot.GetComponent<Bot>().IsMoving))
        {
            HighlightInteractables();
        }
    }

    private void HighlightInteractables()
    {
        if(SceneLoader.Instance.GetCurrentSceneIndex() == 1) //Gameplay Scene 
        { 
            if (Input.GetMouseButtonDown(0))
            {
                if (!_raycastCheck)
                {
                    _raycastCheck = true;
                    UpdateInteractableRayCast();
                    if (Physics.Raycast(_interactableRay, out _hit, 100, _highlightPathLayer))
                    {
                        _idReference = _hit.collider.transform.parent.GetComponent<Tile>().InteractableID;
                        if (_idReference != 0)
                        {
                            HighlightInteractable(_idReference);
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _raycastCheck = false;
                if (_idReference != 0)
                {
                    foreach (Transform interactable in _higlightedInteractables)
                    {
                        interactable.transform.DOMoveY(0, 0.3f, false);
                        interactable.GetComponent<Tile>().IsHighlighted = false;
                    }
                    _higlightedInteractables.Clear();
                }
            }

        } 
    }

    //async
    public async void ShowDirection(DirectionIs direction)
    {
        Vector3 rayOrientation;

        rayOrientation = MoveUtils.SetDirection(direction);
        
        RaycastHit[] platformsToRaise = _bot.GetComponent<Bot>().PlatformsToRaise(rayOrientation);
        
        //else if(_bot.GetComponent<Bot>().StepCount > platformsToRaise.Length)
        //print(platformsToRaise.Length);
        //print(platformsToRaise[1].collider.gameObject.name);
        if (_bot.GetComponent<Bot>().StepCount > platformsToRaise.Length)
        {
            if(platformsToRaise.Length > 0)
            {
                if (platformsToRaise.Length > 1)
                {
                    int distance = (int)Vector3.Distance(platformsToRaise[0].collider.gameObject.transform.position,_bot.transform.position);
                    if(distance > 1)
                    {
                        //_voidDistance = _bot.GetComponent<Bot>().StepCount - distance;
                        _voidDistance = 1;
                        print(distance);
                    }
                    else if (distance == 1)
                    {
                        
                        distance = (int)Vector3.Distance(platformsToRaise[1].collider.gameObject.transform.position, _bot.transform.position);
                        if(distance > 2)
                        {
                            _voidDistance =distance-1;
                        }
                        else
                        {
                            _voidDistance = 0;
                            Destroy(_voidHighlightPlatformReference);

                        }
                        print(_voidDistance);
                        
                    }
                    else
                    {
                        _voidDistance = _bot.GetComponent<Bot>().StepCount;
                        Destroy(_voidHighlightPlatformReference);
                    }
                }
            }
            else
            {
                _voidDistance = 1;
            }
           
        }
        else if(platformsToRaise.Length==0)
        {
            _voidDistance = 1;
        }
        else
        {
            _voidDistance = 0;
            Destroy(_voidHighlightPlatformReference);
        }
        
        if (MoveUtils.SetDirection(_raisingPathDirection) == Vector3.zero || _raisingPathDirection != direction)
        {
            //RaycastHit[] platformsToRaise = Physics.RaycastAll(_botParentGameObject.transform.position, rayOrientation, _currentBotStepCount, _highlightPathLayer);
            //print($"platforms on the path {platformsToRaise.Length} ");
            await ClearPath();
            foreach (var item in platformsToRaise)
            {
                //await Task.Delay(100);
                //item.collider.transform.DOMoveY(0.3f, 2);
                Transform platformToShow = item.collider.transform.parent.transform;
                float yValue = platformToShow.position.y;
                //if (!(Vector3.Distance(platformToShow.position, _botParentGameObject.transform.position) > 1.40f && (Vector3.Distance(platformToShow.position, _botParentGameObject.transform.position) < 1.5f)))
                //{
                _highlightedPath.Add(platformToShow);
                //await PrettyHighlightAsync(platformToShow, true, _highlightHeight);

                //platformToShow.transform.DOMoveY(yValue + _highlightHeight, 0.3f, false);
                //}

                //platformToShow.position += new Vector3(0, _highlightHeight, 0);
                //platformToShow.DOMoveY(_highlightHeight, 0.3f);
            }
            foreach (Transform transform in _highlightedPath)
            {
                transform.DOMoveY(transform.position.y + _highlightHeight, 0.3f, false);
            }

        }
        if (_voidDistance > 0)
        {
            if (_voidHighlightPlatformReference == null)
            {
            }
            else
            {
                Destroy(_voidHighlightPlatformReference);
            }
            _voidHighlightPlatformReference = Instantiate(_deathPlatformVisual,new Vector3(_bot.transform.position.x, 0, _bot.transform.position.z) 
                                                                                + (MoveUtils.SetDirection(direction)*_voidDistance), Quaternion.identity);
        }
        

        _raisingPathDirection = direction;
    }
    //async

    private async Task ClearPath()
    {
        if (_highlightedPath.Count > 0)
        {
            //if (_voidHighlightPlatformReference != null)
            //{
            //    Destroy(_voidHighlightPlatformReference);
            //}
            foreach (Transform transform in _highlightedPath)
            {
                if((transform.position.y > _highlightHeight && transform.position.y < 0.4f)|| ((transform.position.y - _highlightHeight) > 0.44 ))
                {
                    transform.DOMoveY(transform.position.y - _highlightHeight, 0.3f, false);
                }
                else
                {
                    transform.DOMoveY(0, 0.3f, false);
                }
            }
        }
            _highlightedPath.Clear();
        await Task.Yield();
    }

    public void AssignPlayer(GameObject selectedBot)
    {
        _bot = selectedBot;
        _directionalInputBot = _bot.GetComponent<DirectionalInputBot>();
        _botParentGameObject = _bot.transform.parent.transform.parent.gameObject;
        //_bot.GetComponent<Bot>().ShowDirection();
    }
    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= LoadLevel;
        ScriptableObjectLoader.onLevelQeued -= async () => await ClearLevel();
        WinTile.onButtonPressed -= FinishLevel;
        SwitchTile.onSwitchPressed -= Activate;
        SwitchTile.onSwitchReleased += DeActivate;
        Bot.onFinishedMove -= UpdateTurn;
        SwitchTile.onSwitchHighlighted -= HighlightInteractable;
        WallTile.onWallHighlighted -= HighlightInteractable;
        Bot.onStartedMove -= CleanVisualOnBotMove;
    }

    private async void LoadLevel()
    {
        onObjectsInstantiated?.Invoke();
        Level levelToLoad = ScriptableObjectLoader.Instance.LevelToLoad;
        if (levelToLoad != null)
        {
            GameSpaceUIController uIController = FindObjectOfType<GameSpaceUIController>();
            uIController.SetLevelNameText(levelToLoad.Name);
        }
        await RainInAnimation();
        //This after the raining in animation
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        await Task.Yield();
    }
    private async Task RainInAnimation()
    {
        foreach (GameObject tile in TileGameObjects)
        {
             RainInTween(tile.transform);
        }
        foreach (GameObject interactable in SpawnedObjects)
        {
            float randomSpeed = 1f;
            interactable.transform.DOMoveY(0.45f, randomSpeed, false).SetEase(Ease.InQuint);
            if (interactable.GetComponentInChildren<Bot>())
            {
                await interactable.GetComponentInChildren<Bot>().CheckForLanding();
            }

        }
        await Task.Yield();
        onGameStarted?.Invoke();

    }
    private async Task RainOutAnimation()
    {
        foreach (GameObject tile in TileGameObjects)
        {
              RainOutTween(tile.transform);
        }
        foreach (GameObject interactable in SpawnedObjects)
        {
             RainOutTween(interactable.transform);

        }
        await Task.Delay(300);
    }
    private async void RainOutTween(Transform transform)
    {
        transform.DOMoveY(-6f,_rainInDuration*0.25f,false).SetEase(Ease.InQuart);
        await Task.Delay(100);
    }
    private async void RainInTween(Transform transform) 
    {
        transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.OutBack);
        await Task.Delay(100);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
    }
}

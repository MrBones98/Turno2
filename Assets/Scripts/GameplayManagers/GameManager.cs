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
    [SerializeField] [Range (0.5f, 3.0f)] private float _rainInDuration;
    [SerializeField] private LayerMask _highlightPathLayer;
    [SerializeField] private GameObject _botParentGameObject;


    //[OnValueChanged("AssignPlayer")]
    private GameObject _bot;
    private Camera _camera;
    private DirectionalInputBot _directionalInputBot;
    private List<Transform> _highlightedPath = new();
    private List<Transform> _higlightedInteractables = new();
    private RaycastHit _hit;
    private Ray _interactableRay;
    private DirectionIs _raisingPathDirection;
    private int _currentBotStepCount;
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
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadLevel;
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
                print(tiles.name);
               
                    //tiles.GetComponent<ISwitchActivatable>().HighlightInteractable(_highlightHeight);
                    _higlightedInteractables.Add(tiles.transform);
                    tiles.GetComponent<Tile>().IsHighlighted = true;
                
            }
        }
        foreach (Transform interactable in _higlightedInteractables)
        {

            interactable.transform.DOMoveY(_highlightHeight, 0.3f, false);
            
        }
        //_selectCheck = !_selectCheck;
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
        for (int i = TileGameObjects.Count-1; i >= 0; i--)
        {
            Destroy(TileGameObjects[i]);
        }
        for (int i = Cards.Count-1; i >= 0; i--)
        {
            Destroy(Cards[i]);
        }
        for (int i = SpawnedObjects.Count -1; i >= 0; i--)
        {
            Destroy(SpawnedObjects[i]);
        }
        TileGameObjects.Clear();
        Cards.Clear();
        SpawnedObjects.Clear();
        await Task.Yield();
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
                }
            }
        }
        //Just Raise Event and subscribe from Intwractables
        //if(_bot != null)
        //{
        //    if(!_bot.GetComponent<Bot>().IsMoving)
        //    HighlightInteractables();
        //}
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
                        print(_idReference);
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
        
        if (MoveUtils.SetDirection(_raisingPathDirection) == Vector3.zero || _raisingPathDirection != direction)
        {
            //RaycastHit[] platformsToRaise = Physics.RaycastAll(_botParentGameObject.transform.position, rayOrientation, _currentBotStepCount, _highlightPathLayer);
            RaycastHit[] platformsToRaise = _bot.GetComponent<Bot>().PlatformsToRaise(rayOrientation);
            //print($"platforms on the path {platformsToRaise.Length} ");
            await ClearPath();
            foreach (var item in platformsToRaise)
            {
                //await Task.Delay(100);
                //item.collider.transform.DOMoveY(0.3f, 2);
                Transform platformToShow = item.collider.transform.parent.transform;
                //if (!(Vector3.Distance(platformToShow.position, _botParentGameObject.transform.position) > 1.40f && (Vector3.Distance(platformToShow.position, _botParentGameObject.transform.position) < 1.5f)))
                //{
                _highlightedPath.Add(platformToShow);
                //}

                //platformToShow.position += new Vector3(0, _highlightHeight, 0);
                platformToShow.DOMoveY(_highlightHeight, 0.3f);
            }

        }

        _raisingPathDirection = direction;
    }
    //async

    private async Task ClearPath()
    {
        if (_highlightedPath.Count > 0)
        {
            foreach (Transform transform in _highlightedPath)
            {
                //transform.position -= new Vector3(0, _highlightHeight, 0);
                //Check here for bot man
                transform.DOMoveY(0, 0.3f);
            }
            _highlightedPath.Clear();
        }
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
        //ShuffleList(TileGameObjects)
        foreach (GameObject tile in TileGameObjects)
        {
           
            await Task.Delay(50);

            RainInTween(tile.transform);
            
            //await tile.transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.OutBack).AsyncWaitForPosition(7f);
            //tile.transform.DOMoveY(0, randomSpeed, false).SetEase(Ease.InOutQuad);
            //tile.transform.DOMoveY(0, randomSpeed, false).SetEase(Ease.InOutSine);
        }
        foreach (GameObject interactable in SpawnedObjects)
        {
            //0.45~0.5
            //float randomSpeed = Random.Range(14f, 16f);
            float randomSpeed = 2.3f;
            //await Task.Delay(150);

            //interactable.transform.DOMoveY(0.45f, randomSpeed, false).SetEase(Ease.InQuad);
            interactable.transform.DOMoveY(0.45f, randomSpeed, false).SetEase(Ease.InQuint);
            //interactable.transform.DOMoveY(0.45f, randomSpeed, false).SetEase(Ease.InCubic);

        }
        await Task.Yield();
        onGameStarted?.Invoke();

    }
    private void RainInTween(Transform transform) 
    {
        //transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.InOutElastic);
        //transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.OutBounce);
        transform.DOMoveY(0, _rainInDuration, false).SetEase(Ease.OutBack);
    }
    private void ShuffleList(List<GameObject> tileGameObjects)
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //Gizmos.DrawSphere(_raycastOrigin.position, 0.40f);
        //Gizmos.DrawRay(_raycastOrigin.position, -_raycastOrigin.transform.up);
    }
}

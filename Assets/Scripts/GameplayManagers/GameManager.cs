using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using System.Threading.Tasks;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<GameObject> TileGameObjects = new();
    public static List<GameObject> Cards = new();
    public static List<GameObject> SpawnedObjects = new();

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _gameplayUI;
    [SerializeField] private float _highlightHeight;
    [SerializeField] private LayerMask _highlightPathLayer;


    //[OnValueChanged("AssignPlayer")]
    private GameObject _bot;
    [SerializeField] private GameObject _botParentGameObject;
    private DirectionalInputBot _directionalInputBot;
    private DirectionIs _raisingPathDirection;
    private List<Transform> _highlightedPath = new();
    private List<Transform> _higlightedInteractables = new();
    private int _currentBotStepCount;
    private bool _selectCheck = false;
    private bool _raycastCheck =false;
    private Camera _camera;
    private Ray _interactableRay;
    private RaycastHit _hit;
    private int _idReference;
    //public WinTile WinTile;

    //ON THE LEVEL SO ADD COUNT OF BUTTONS FOR WINNING FOR DIFFERENT NEEED AMOUNTS

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
        print("Reset level");
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

    //async
    public void ShowDirection(DirectionIs direction)
    {
        Vector3 rayOrientation;

        rayOrientation = MoveUtils.SetDirection(direction);
        
        if (MoveUtils.SetDirection(_raisingPathDirection) == Vector3.zero || _raisingPathDirection != direction)
        {
            //RaycastHit[] platformsToRaise = Physics.RaycastAll(_botParentGameObject.transform.position, rayOrientation, _currentBotStepCount, _highlightPathLayer);
            RaycastHit[] platformsToRaise = _bot.GetComponent<Bot>().PlatformsToRaise(rayOrientation);
            //print($"platforms on the path {platformsToRaise.Length} ");
            ClearPath();
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
    }

    private void LoadLevel()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        onGameStarted?.Invoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //Gizmos.DrawSphere(_raycastOrigin.position, 0.40f);
        //Gizmos.DrawRay(_raycastOrigin.position, -_raycastOrigin.transform.up);
    }
}

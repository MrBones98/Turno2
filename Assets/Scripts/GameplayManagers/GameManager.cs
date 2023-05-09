using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Utils;
using System.Threading.Tasks;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<GameObject> TileGameObjects = new();
    public static List<GameObject> Cards = new();

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
    private int _currentBotStepCount;
    //public WinTile WinTile;

    //ON THE LEVEL SO ADD COUNT OF BUTTONS FOR WINNING FOR DIFFERENT NEEED AMOUNTS

    public delegate void OnGameStart();
    public static event OnGameStart onGameStarted;
    public delegate void OnBotMove();
    public static event OnBotMove onBotMove;
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += LoadLevel;
        WinTile.onButtonPressed += FinishLevel;
        SwitchTile.onSwitchPressed += Activate;
        SwitchTile.onSwitchReleased += DeActivate;
        Bot.onFinishedMove += UpdateTurn;
    }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else 
        { 
            Destroy(this.gameObject);
        }
    }

    private void UpdateTurn()
    {
        onBotMove?.Invoke();
        //AssignPlayer(null);
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

    private void FinishLevel()
    {
        _winScreen.SetActive(true);
        _gameplayUI.SetActive(false);
    }

    public  void GiveChosenBotDirection(DirectionIs directionIs)
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
        _bot.GetComponent<Bot>().CheckMove(moveVector);
        ClearPath();
    }

    void Update()
    {
        
        if (_bot != null)
        {
            if (_bot.GetComponent<Bot>().IsFocused)
            {
                ShowDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                print(_botParentGameObject.transform.position);
                if (Input.GetMouseButtonDown(0))
                {
                    GiveChosenBotDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                }
            }
        }

    }
    public void ShowDirection(DirectionIs direction)
    {
        Vector3 rayOrientation;

        rayOrientation = MoveUtils.SetDirection(direction);
        print(rayOrientation);

        if (MoveUtils.SetDirection(_raisingPathDirection) == Vector3.zero || _raisingPathDirection != direction)
        {
            //RaycastHit[] platformsToRaise = Physics.RaycastAll(_botParentGameObject.transform.position, rayOrientation, _currentBotStepCount, _highlightPathLayer);
            RaycastHit[] platformsToRaise = _bot.GetComponent<Bot>().PlatformsToRaise(rayOrientation);
            print($"platforms on the path {platformsToRaise.Length} ");
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

                platformToShow.position += new Vector3(0, _highlightHeight, 0);
            }

        }

        _raisingPathDirection = direction;

        //await Task.Delay(1000);
        //foreach (var item in platformsToRaise)
        //{
        //    await Task.Delay(100);
        //    item.collider.gameObject.GetComponentInParent<Transform>().DOMoveY(-0.3f, 2);
        //}
    }

    private void ClearPath()
    {
        if (_highlightedPath.Count > 0)
        {
            foreach (Transform transform in _highlightedPath)
            {
                transform.position -= new Vector3(0, _highlightHeight, 0);
            }
            _highlightedPath.Clear();
        }
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
        Bot.onFinishedMove -= UpdateTurn;
    }

    private void LoadLevel()
    {
        onGameStarted();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //Gizmos.DrawSphere(_raycastOrigin.position, 0.40f);
        //Gizmos.DrawRay(_raycastOrigin.position, -_raycastOrigin.transform.up);
    }
}

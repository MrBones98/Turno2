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

    //[OnValueChanged("AssignPlayer")]
    private GameObject _bot;
    private DirectionalInputBot _directionalInputBot;


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
    public  void GiveChosenBotDirection(DirectionIs directionIs)
    {
        Vector3 moveVector; //perhaps pass it to bot
        

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
    }

    void Update()
    {
        
        if (_bot != null)
        {
            if (_bot.GetComponent<Bot>().IsFocused)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GiveChosenBotDirection(_directionalInputBot.CalculateQuadrants(_directionalInputBot.Calculate()));
                }
            }
        }

    }

    public void AssignPlayer(GameObject selectedBot)
    {
        _bot = selectedBot;
        _directionalInputBot = _bot.GetComponent<DirectionalInputBot>();
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
}

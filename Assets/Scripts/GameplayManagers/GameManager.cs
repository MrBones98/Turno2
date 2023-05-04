using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static List<GameObject> TileGameObjects = new();
    public static List<GameObject> Cards = new();

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _gameplayUI;

    //[OnValueChanged("AssignPlayer")]
    private GameObject _bot;


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
        onBotMove();
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
    public void GiveChosenBotDirection(int direction)
    {
        Vector3 moveVector; //perhaps pass it to bot
        
        //0 right
        //1 down
        //2 left
        //3 up

        if(direction == 0)
        {
            moveVector = Vector3.right;
        }
        else if(direction == 1)
        {
            moveVector = -Vector3.forward;
        }
        else if(direction == 2)
        {
            moveVector = -Vector3.right;
        }
        else if (direction == 3)
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
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.GetComponent<Bot>())
                {
                    print("Hit a bot");
                    _bot = hitInfo.collider.gameObject;
                    //Click on him better to simulate the card grabbing for now,
                    //raise event from Bot Component, activate UI
                    //that caches bot
                }
            }
        }
    }

    public void AssignPlayer(GameObject selectedBot)
    {
        _bot = selectedBot;
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

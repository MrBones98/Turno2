using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpaceUIController : MonoBehaviour
{
    [SerializeField]
    private GameSpaceUIHandler _handler;
    private ScriptableObjectLoader _instance;

    public delegate void OnAnyButtonClicked();
    public static event OnAnyButtonClicked OnAnyGameSpaceButtonClickedEvent;

    public delegate void OnCardSelected(ActionCardData cardData);
    public static event OnCardSelected onCardButtonClicked;

    [SerializeField] public ActionCardData Move1_Card;
    [SerializeField] public ActionCardData Move2_Card;
    [SerializeField] public ActionCardData Move3_Card;
    [SerializeField] public ActionCardData Move4_Card;
    [SerializeField] public ActionCardData Jump2_Card;
    [SerializeField] public ActionCardData Jump3_Card;
    [SerializeField] public ActionCardData Jump4_Card;

    private static CardSlot _activeCard;

    private Level _level;

    private List<ActionCardData> cardsToLoad = new List<ActionCardData>();

    private void Awake()
    {

    }
    private void OnEnable()
    {
        WinTile.onButtonPressed += ShowWinScreen;
        GameManager.onGameStarted += LoadCards;
        Bot.onStartedMove += RemoveCard;
        ScriptableObjectLoader.onLevelQeued += ClearAllCards;
    }


    private void OnDisable()
    {
        WinTile.onButtonPressed -= ShowWinScreen;
        GameManager.onGameStarted -= LoadCards;
        ScriptableObjectLoader.onLevelQeued -= ClearAllCards;
    }
    private void Start()
    {
        InitGameUI();
        InitPauseMenu();
        InitWinMenu();
        SetLevelNameText(" ");
        _handler.ClearMenus();
        _instance = ScriptableObjectLoader.Instance;

        //DebugDrawCards();
    }
    #region public methods
    public void ShowWinScreen()
    {
        print("Showing Win Screen");
        _handler.DrawWinMenu();
    }
    #endregion

    #region debug methods

    private void DebugDrawCards()
    {
        
        cardsToLoad.Add(Move1_Card);
        cardsToLoad.Add(Move1_Card);
        cardsToLoad.Add(Move2_Card);
        cardsToLoad.Add(Move2_Card);
        cardsToLoad.Add(Move3_Card);
        cardsToLoad.Add(Jump4_Card);

        foreach (var card in cardsToLoad)
        {
            CardSlot newSlot = new CardSlot(card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }
    }

    public static void OnDebugCardClicked(CardSlot card, int distance, bool isJump)
    {
        if (isJump)
        {
            print($"{distance} jump card clicked");
        }
        else
        {
            print($"{distance} move card clicked");
        }

        _activeCard = card;
        //print(card);
    }

    [Button("ClearCard"), DisableInEditorMode()]
    public void OnRemoveCardRequest(CardSlot target)
    {
        target = _activeCard;
        print(_activeCard);
        _handler.CardDisplay.Remove(target.button);
    }
    private void RemoveCard()
    {
        CardSlot target = _activeCard;
        _handler.CardDisplay.Remove(target.button);
        _activeCard=null;
    }
    private void DebugLoadLevel(int direction)
    {
        if (direction > 0)
        {
            print("Load Next Level Clicked");
            LoadNextLevel(true);
        }
        else
        {
            print("Load Prev Level Clicked");
            LoadNextLevel(false);
        }
    }

    private void GiveMoveCard(int amount)
    {
        CardHandManager.Instance.DebugGiveMoveCard(amount);
    }

    public void DebugSliderValue(string name, float val)
    {
        print($"{name} slider has {val} value");
    }

    #endregion

    #region Card Methods

    private void LoadCards()
    {
        _level = ScriptableObjectLoader.Instance.LevelToLoad;
 
        
        for (int i = 0; i < _level.MoveOne; i++)
        {
            CardSlot newSlot = new CardSlot(Move1_Card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }
        for (int i = 0; i < _level.MoveTwo; i++)
        {
            CardSlot newSlot = new CardSlot(Move2_Card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }
        for (int i = 0; i < _level.MoveThree; i++)
        {
            CardSlot newSlot = new CardSlot(Move3_Card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }
        for (int i = 0; i < _level.MoveFour; i++)
        {
            CardSlot newSlot = new CardSlot(Move4_Card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }
        for (int i = 0; i < _level.JumpCardTwo; i++)
        {
            CardSlot newSlot = new CardSlot(Jump2_Card, _handler.ActionCardTemplate);

            _handler.CardDisplay.Add(newSlot.button);
        }

        print(_handler.CardDisplay.childCount);
        
    }

    private void ClearAllCards()
    {
        print("i was told to clear the cards but i didnt because i am a naught little method");
        _handler.CardDisplay.Clear();

    }

    private static void OnCardClicked(CardSlot cardSlot, ActionCardData data)
    {
        _activeCard = cardSlot;
        onCardButtonClicked?.Invoke(data);

    }

    #endregion

    #region Element interaction Methods

    private async void OnResetClicked()
    {
        if (_instance.IsLoaded)
        {
            _handler.ClearMenus();
            await GameManager.Instance.Resetlevel();
            await _instance.ReloadLevel();
            print("Reset Clicked");
        }
    }

    private async void OnUndoClicked()
    {
        print("Undo Clicked");
        await GameManager.Instance.UndoPressed();
    }

    private void OnPauseMenuClicked()
    {
        print("Pause Clicked");
        _handler.ClearMenus();
        _handler.DrawPauseMenu();
    }

    public void SetLevelNameText(string target)
    {
        if (_handler.LvlNameDisplay == null)
        {
            print("No Lvl Display Panel Found");
            return;
        }
        _handler.LvlNameDisplay.text = target;
    }

    // win menu
    private void LoadNextLevel(bool isNext)
    {
        if (_instance.IsLoaded)
        {
            _handler.ClearMenus();
           _instance.LoadNextLevel(isNext);
        }
    }

    // pause menu
    private void ClosePauseMenu()
    {
        _handler.ClearMenus();
    }

    private async void GoToMainMenu()
    {
        _handler.ClearMenus();
        await GameManager.Instance.ClearLevel();
        SceneLoader.Instance.GoToMainMenu();
    }

    // audio options
    private void SetBGMVol(float target)
    {
        AudioManager.instance.SetBGMSound(target);
    }

    private void SetSFXVol(float target)
    {
        AudioManager.instance.SetSFXSound(target);
    }

    private void RaiseAnyButtonClickedEvent()
    {
        OnAnyGameSpaceButtonClickedEvent?.Invoke();
    }

    #endregion

    #region init methods

    private void InitGameUI()
    {
        _handler._root = _handler.UIDoc.rootVisualElement;

        //_handler.WinMenu = _handler.WinMenuDoc.CloneTree();

        _handler.CentralPanel = _handler._root.Q<VisualElement>(GameSpaceUIHandler.k_CentralPanel);

        _handler.CardDisplay = _handler._root.Q<VisualElement>(GameSpaceUIHandler.k_CardDisplay);

        _handler.LvlNameDisplay = _handler._root.Q<Label>(GameSpaceUIHandler.k_LvlNameDisplay);

        _handler.UndoButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_UndoButtonName);
        _handler.UndoButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.UndoButton.clicked += () => OnUndoClicked();

        _handler.ResetButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_ResetButtonName);
        _handler.ResetButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.ResetButton.clicked += () => OnResetClicked();

        _handler.PauseButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_PauseButton);
        _handler.PauseButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.PauseButton.clicked += () => OnPauseMenuClicked();


        // debug methods
        _handler.DebugGiveMove1Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give1MoveButtonName);
        _handler.DebugGiveMove1Button.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugGiveMove1Button.clicked += () => GiveMoveCard(1);

        _handler.DebugGiveMove2Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give2MoveButtonName);
        _handler.DebugGiveMove2Button.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugGiveMove2Button.clicked += () => GiveMoveCard(2);

        _handler.DebugGiveMove3Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give3MoveButtonName);
        _handler.DebugGiveMove3Button.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugGiveMove3Button.clicked += () => GiveMoveCard(3);

        _handler.DebugGiveMove4Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give4MoveButtonName);
        _handler.DebugGiveMove4Button.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugGiveMove4Button.clicked += () => GiveMoveCard(4);


        _handler.DebugNextLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_DebugNextLvlButtonName);
        _handler.DebugNextLvlButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugNextLvlButton.clicked += () => DebugLoadLevel(1);

        _handler.DebugPrevLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_DebugPrevLvlMoveButtonName);
        _handler.DebugPrevLvlButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.DebugPrevLvlButton.clicked += () => DebugLoadLevel(0);
    }

    private void InitPauseMenu()
    {
        _handler.PauseMenu = _handler.PauseMenuDoc.CloneTree();

        _handler.MainMenuFromPauseButton = _handler.PauseMenu.Q<Button>(GameSpaceUIHandler.k_MainMenuFromPauseButtonName);
        _handler.MainMenuFromPauseButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.MainMenuFromPauseButton.clicked += () => GoToMainMenu();

        _handler.ContinueButton = _handler.PauseMenu.Q<Button>(GameSpaceUIHandler.k_ContinueButtonName);
        _handler.ContinueButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.ContinueButton.clicked += () => ClosePauseMenu();

        _handler.BGMVolSlider = _handler.PauseMenu.Q<Slider>(GameSpaceUIHandler.k_BGMSliderName);
        _handler.BGMVolSlider.RegisterValueChangedCallback(evt =>
        {
            //DebugSliderValue(GameSpaceUIHandler.k_BGMSliderName, evt.newValue);
            SetBGMVol(evt.newValue);
        });

        _handler.SFXVolSlider = _handler.PauseMenu.Q<Slider>(GameSpaceUIHandler.k_SFXSliderName);
        _handler.SFXVolSlider.RegisterValueChangedCallback(evt =>
        {
            //DebugSliderValue(GameSpaceUIHandler.k_SFXSliderName, evt.newValue);
            SetSFXVol(evt.newValue);
        });
    }

    private void InitWinMenu()
    {
        _handler.WinMenu = _handler.WinMenuDoc.CloneTree();

        _handler.MainMenuFromWinButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_MainMenuFromWinButtonName);
        _handler.MainMenuFromWinButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.MainMenuFromWinButton.clicked += () => GoToMainMenu();

        _handler.NextLevelFromWinButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_NextLevelButtonName);
        _handler.NextLevelFromWinButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.NextLevelFromWinButton.clicked += () => LoadNextLevel(true);

        _handler.RedoButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_RedoButtonName);
        _handler.RedoButton.clicked += () => RaiseAnyButtonClickedEvent();
        _handler.RedoButton.clicked += () => OnResetClicked();
    }


    #endregion

    public class CardSlot
    {
        internal Button button;

        public CardSlot(ActionCardData data, VisualTreeAsset template)
        {
            TemplateContainer container = template.Instantiate();

            button = container.Q<Button>();
            button.style.backgroundImage = new StyleBackground(data.image);

            //button.RegisterCallback<ClickEvent>((evt) => GameSpaceUIController.OnDebugCardClicked(this, data.distance, data.isJump));
            button.RegisterCallback<ClickEvent>((evt) => GameSpaceUIController.OnCardClicked(this, data));


        }
    }
}

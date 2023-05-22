using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private MainMenuUIHandler _menuHandler;
    private SceneLoader _sceneLoader;
    private ScriptableObjectLoader _objectLoader;

    public delegate void OnAnyButtonClicked();
    public static event OnAnyButtonClicked OnAnyButtonClickedEvent;

    private void Awake()
    {
        InitContainerPanels();

        InitStartPanel();
        InitOptionsPanels();
        InitLevelSelectPanels();

        _menuHandler.DrawStartMenu();

        _sceneLoader = SceneLoader.Instance;
        _objectLoader = ScriptableObjectLoader.Instance;
    }

    #region Debug Methods
    public void DebugButtonPress(string name)
    {
        //print($"{name} button pressed");
    }

    public void DebugSliderValue(string name, float val)
    {
        print($"{name} slider has {val} value");
    }
    #endregion

    #region Element interaction Methods

    // menu navigation

    private void ShowStartMenu()
    {
        _menuHandler.DrawStartMenu();
    }

    private void ShowOptionsMenu()
    {
        _menuHandler.DrawOptionsMenu();
    }

    private void ShowLevelSelect()
    {
        _menuHandler.DrawLevelSelectMenu();
    }

    // game Utils
    private void QuitGame()
    {
        print("Quit game button pressed");
        Application.Quit();
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

    // level Loading
    private void StartGame()
    {
        _sceneLoader.GoToGameScene();
    }

    private void OnLvlSelectButtonClicked(int index)
    {
        //print($"Level {index} Selected");
    }

    public static void LoadSelectedLvl(int index)
    {
        //print($"Level {index} Selected");
        SceneLoader.Instance.GoToGameSceneWithLevelIndex(index-1);
    }

    private void RaiseAnyButtonClickedEvent()
    {
        OnAnyButtonClickedEvent?.Invoke();
    }

    #endregion

    #region Init Panel Methods

    private void InitContainerPanels()
    {
        _menuHandler._root = _menuHandler._mainMenuDoc.rootVisualElement;
        _menuHandler._leftPanel = _menuHandler._root.Q<VisualElement>(MainMenuUIHandler.k_leftPanel);
        _menuHandler._rightPanel = _menuHandler._root.Q<VisualElement>(MainMenuUIHandler.k_rightPanel);
    }

    private void InitStartPanel()
    {
        // set up start panel
        _menuHandler._startPanel = _menuHandler._startPanelDoc.CloneTree();

        _menuHandler._startButton = _menuHandler._startPanel.Q<Button>(MainMenuUIHandler.k_StartButtonName);
        _menuHandler._startButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_StartButtonName);
        _menuHandler._startButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._startButton.clicked += () => StartGame();



        _menuHandler._levelSelectButton = _menuHandler._startPanel.Q<Button>(MainMenuUIHandler.k_LevelSelectButtonName);
        //_menuHandler._levelSelectButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_LevelSelectButtonName);
        _menuHandler._levelSelectButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._levelSelectButton.clicked += () => ShowLevelSelect();


        _menuHandler._optionsButton = _menuHandler._startPanel.Q<Button>(MainMenuUIHandler.k_OptionsButtonName);
        //_menuHandler._optionsButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_OptionsButtonName);
        _menuHandler._optionsButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._optionsButton.clicked += () => ShowOptionsMenu();

        _menuHandler._quitButton = _menuHandler._startPanel.Q<Button>(MainMenuUIHandler.k_QuitButtonName);
        _menuHandler._quitButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_QuitButtonName);
        _menuHandler._quitButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._quitButton.clicked += () => QuitGame();
    }

    private void InitOptionsPanels()
    {
        // set up options panel
        _menuHandler._optionsPanel = _menuHandler._OptionsPanelDoc.CloneTree();

        _menuHandler._BGMSlider = _menuHandler._optionsPanel.Q<Slider>(MainMenuUIHandler.k_BGMSliderName);
        _menuHandler._BGMSlider.RegisterValueChangedCallback(evt =>
        {
            DebugSliderValue(MainMenuUIHandler.k_BGMSliderName, evt.newValue);
            SetBGMVol(evt.newValue);
        });

        _menuHandler._SFXSlider = _menuHandler._optionsPanel.Q<Slider>(MainMenuUIHandler.k_SFXSliderName);
        _menuHandler._SFXSlider.RegisterValueChangedCallback(evt =>
        {
            DebugSliderValue(MainMenuUIHandler.k_SFXSliderName, evt.newValue);
            SetSFXVol(evt.newValue);
        });

        _menuHandler._OptionsBackButton = _menuHandler._optionsPanel.Q<Button>(MainMenuUIHandler.k_OptionsBackButtonName);
        //_menuHandler._OptionsBackButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_OptionsBackButtonName);
        _menuHandler._OptionsBackButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._OptionsBackButton.clicked += () => ShowStartMenu();

    }

    private void InitLevelSelectPanels()
    {
        // set up lvl select left panel
        _menuHandler._lvlSelectLeftPanel = _menuHandler._LevelSelectLeftPanelDoc.CloneTree();

        _menuHandler._lvlSelectBackButton = _menuHandler._lvlSelectLeftPanel.Q<Button>(MainMenuUIHandler.k_LvlSelectBackButtonName);
        //_menuHandler._lvlSelectBackButton.clicked += () => DebugButtonPress(MainMenuUIHandler.k_LvlSelectBackButtonName);
        _menuHandler._lvlSelectBackButton.clicked += () => RaiseAnyButtonClickedEvent();
        _menuHandler._lvlSelectBackButton.clicked += () => ShowStartMenu();


        // set up lvl select right panel
        _menuHandler._lvlSelectRightPanel = _menuHandler._LevelSelectRightPanelDoc.CloneTree();

        _menuHandler._lvlSelectScroll = _menuHandler._lvlSelectRightPanel.Q<ScrollView>(MainMenuUIHandler.k_LvlSelectScrollName);

        PopulateLevelSelect(ScriptableObjectLoader.Instance.Levels);
    }

    private void PopulateLevelSelect(List<Level> levels)
    {
        if (levels.Count < 1)
        {
            Debug.LogWarning("No Levels to load");
            return;
        }

        int index = 1;
        foreach (var lvl in levels)
        {
            LevelSelectButton newButton = new LevelSelectButton(lvl, index, _menuHandler.LevelSelectButtonTemplate);

            _menuHandler._lvlSelectScroll.Add(newButton.Button);
            newButton.Button.clicked += () => RaiseAnyButtonClickedEvent();

            index++;           
        }      
    }
    #endregion
}

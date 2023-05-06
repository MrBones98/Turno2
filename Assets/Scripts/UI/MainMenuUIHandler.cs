using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIHandler : MonoBehaviour
{
    // menu panel UI docs
    [SerializeField]
    private UIDocument _mainMenuDoc;
    [SerializeField]
    private VisualTreeAsset _startPanelDoc;
    [SerializeField]
    private VisualTreeAsset _LevelSelectLeftPanelDoc;
    [SerializeField]
    private VisualTreeAsset _LevelSelectRightPanelDoc;
    [SerializeField]
    private VisualTreeAsset _OptionsPanelDoc;

    // controller
    [SerializeField]
    private MainMenuController _controller;

    #region Element IDs
    // main menu panel IDs
    const string k_leftPanel = "LeftPanel";
    const string k_rightPanel = "RightPanel";

    // start panel IDs
    const string k_StartButtonName = "StartGame";
    const string k_LevelSelectButtonName = "LevelSelect";
    const string k_OptionsButtonName = "Options";
    const string k_QuitButtonName = "QuitGame";

    // options panel IDs
    const string k_BGMSliderName = "BGMVolSlider";
    const string k_SFXSliderName = "SFXVolSlider";
    const string k_OptionsBackButtonName = "BackButton";

    // lvl select left panel IDs
    const string k_LvlSelectBackButtonName = "BackButton";

    // lvl select right panel IDs
    const string k_LvlSelectScrollName = "LevelScrollView";
    #endregion

    #region element caches
    // root menu containers
    private VisualElement _root;
    private VisualElement _leftPanel;
    private VisualElement _rightPanel;

    // menu panel caches
    private VisualElement _startPanel;
    private VisualElement _optionsPanel;
    private VisualElement _lvlSelectLeftPanel;
    private VisualElement _lvlSelectRightPanel;

    // start panel elements
    private Button _startButton;
    private Button _levelSelectButton;
    private Button _optionsButton;
    private Button _quitButton;

    // options panel elements
    private Slider _BGMSlider;
    private Slider _SFXSlider;
    private Button _OptionsBackButton;

    // lvl select left panel elements
    private Button _lvlSelectBackButton;

    // lvl select right panel elements
    private ScrollView _lvlSelectScroll;
    #endregion

    private void OnEnable()
    {
        InitContainerPanels();

        InitStartPanel();
        InitOptionsPanels();
        InitLevelSelectPanels();

        DrawStartMenu();
    }

    #region Draw Methods
    private void DrawStartMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_startPanel);
    }

    private void DrawOptionsMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_startPanel);
    }

    private void DrawLevelSelectMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_startPanel);
    }

    private void ClearContainerPanels()
    {
        _leftPanel.Clear();
        _rightPanel.Clear();
    }
    #endregion

    #region Init Panel Methods
    private void InitContainerPanels()
    {
        _root = _mainMenuDoc.rootVisualElement;
        _leftPanel = _root.Q<VisualElement>(k_leftPanel);
        _rightPanel = _root.Q<VisualElement>(k_rightPanel);
    }

    private void InitStartPanel()
    {
        // set up start panel
        _startPanel = _startPanelDoc.CloneTree();

        _startButton = _startPanel.Q<Button>(k_StartButtonName);
        _startButton.clicked += () => _controller.DebugButtonPress(k_StartButtonName);

        _levelSelectButton = _startPanel.Q<Button>(k_LevelSelectButtonName);
        _levelSelectButton.clicked += () => _controller.DebugButtonPress(k_LevelSelectButtonName);

        _optionsButton = _startPanel.Q<Button>(k_OptionsButtonName);
        _optionsButton.clicked += () => _controller.DebugButtonPress(k_OptionsButtonName);

        _quitButton = _startPanel.Q<Button>(k_QuitButtonName);
        _quitButton.clicked += () => _controller.DebugButtonPress(k_QuitButtonName);
    }

    private void InitOptionsPanels()
    {
        // set up options panel
        _optionsPanel = _OptionsPanelDoc.CloneTree();

        _BGMSlider = _optionsPanel.Q<Slider>(k_BGMSliderName);
        _BGMSlider.RegisterValueChangedCallback(evt =>
        {
            _controller.DebugSliderValue(k_BGMSliderName, evt.newValue);
        });

        _SFXSlider = _optionsPanel.Q<Slider>(k_SFXSliderName);
        _SFXSlider.RegisterValueChangedCallback(evt =>
        {
            _controller.DebugSliderValue(k_SFXSliderName, evt.newValue);
        });

        _OptionsBackButton = _optionsPanel.Q<Button>(k_OptionsBackButtonName);
        _OptionsBackButton.clicked += () => _controller.DebugButtonPress(k_OptionsBackButtonName);
    }

    private void InitLevelSelectPanels()
    {
        // set up lvl select left panel
        _lvlSelectLeftPanel = _LevelSelectLeftPanelDoc.CloneTree();

        _lvlSelectBackButton = _lvlSelectLeftPanel.Q<Button>(k_LvlSelectBackButtonName);
        _lvlSelectBackButton.clicked += () => _controller.DebugButtonPress(k_LvlSelectBackButtonName);

        // set up lvl select right panel
        _lvlSelectRightPanel = _LevelSelectRightPanelDoc.CloneTree();
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIHandler : MonoBehaviour
{
    // menu panel UI docs
    public UIDocument _mainMenuDoc;
    public VisualTreeAsset _startPanelDoc;
    public VisualTreeAsset _LevelSelectLeftPanelDoc;
    public VisualTreeAsset _LevelSelectRightPanelDoc;
    public VisualTreeAsset _OptionsPanelDoc;

    // menu item templates
    public VisualTreeAsset LevelSelectButtonTemplate;

    #region Element IDs
    // main menu panel IDs
    
    public const string k_leftPanel = "LeftPanel";
    public const string k_rightPanel = "RightPanel";

    // start panel IDs
    public const string k_StartButtonName = "StartGame";
    public const string k_LevelSelectButtonName = "LevelSelect";
    public const string k_OptionsButtonName = "Options";
    public const string k_QuitButtonName = "QuitGame";

    // options panel IDs
    public const string k_BGMSliderName = "BGMVolSlider";
    public const string k_SFXSliderName = "SFXVolSlider";
    public const string k_OptionsBackButtonName = "BackButton";

    // lvl select left panel IDs
    public const string k_LvlSelectBackButtonName = "BackButton";

    // lvl select right panel IDs
    public const string k_LvlSelectScrollName = "LevelScrollView";
    #endregion

    #region element caches
    // root menu containers
    [HideInInspector] public VisualElement _root;
    [HideInInspector] public VisualElement _leftPanel;
    [HideInInspector] public VisualElement _rightPanel;

    // menu panel caches
    [HideInInspector] public VisualElement _startPanel;
    [HideInInspector] public VisualElement _optionsPanel;
    [HideInInspector] public VisualElement _lvlSelectLeftPanel;
    [HideInInspector] public VisualElement _lvlSelectRightPanel;

    // start panel elements
    [HideInInspector] public Button _startButton;
    [HideInInspector] public Button _levelSelectButton;
    [HideInInspector] public Button _optionsButton;
    [HideInInspector] public Button _quitButton;

    // options panel elements
    [HideInInspector] public Slider _BGMSlider;
    [HideInInspector] public Slider _SFXSlider;
    [HideInInspector] public Button _OptionsBackButton;

    // lvl select left panel elements
    [HideInInspector] public Button _lvlSelectBackButton;

    // lvl select right panel elements
    [HideInInspector] public ScrollView _lvlSelectScroll;
    #endregion

    #region Draw Methods
    public void DrawStartMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_startPanel);
    }

    public void DrawOptionsMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_optionsPanel);
    }

    public void DrawLevelSelectMenu()
    {
        ClearContainerPanels();

        _leftPanel.Add(_lvlSelectLeftPanel);
        _rightPanel.Add(_lvlSelectRightPanel);
    }

    public void ClearContainerPanels()
    {
        _leftPanel.Clear();
        _rightPanel.Clear();
    }
    #endregion
}

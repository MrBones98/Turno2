using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpaceUIHandler : MonoBehaviour
{
    public UIDocument UIDoc;

    [SerializeField] public VisualTreeAsset PauseMenuDoc;
    [SerializeField] public VisualTreeAsset WinMenuDoc;
    //public VisualTreeAsset LossScreen;

    #region element IDs

    public const string k_ResetButtonName = "ResetButton";
    public const string k_UndoButtonName = "UndoButton";
    public const string k_PauseButton = "PauseButton";

    public const string k_CentralPanel = "CentralPanel";

    // debug menu
    public const string k_Give1MoveButtonName = "GiveMove1";
    public const string k_Give2MoveButtonName = "GiveMove2";
    public const string k_Give3MoveButtonName = "GiveMove3";
    public const string k_Give4MoveButtonName = "GiveMove4";

    public const string k_NextLvlButtonName = "NextLvl";
    public const string k_PrevLvlMoveButtonName = "PrevLvl";

    // pause menu
    public const string k_MainMenuButtonName = "MainMenuButton";
    public const string k_ContinueButtonName = "ContinueButton";
    public const string k_BGMSliderName = "BGMVolSlider";
    public const string k_SFXSliderName = "SFXVolSlider";

    #endregion

    #region element caches

    [HideInInspector] public VisualElement _root;

    // displays pause and win menus
    [HideInInspector] public VisualElement CentralPanel;

    [HideInInspector] public VisualElement PauseMenu;
    [HideInInspector] public VisualElement WinMenu;
    //[HideInInspector] public VisualElement LossMenu;


    [HideInInspector] public Button ResetButton;
    [HideInInspector] public Button UndoButton;
    [HideInInspector] public Button PauseButton;

    // pause menu
    [HideInInspector] public Button MainMenuButton;
    [HideInInspector] public Button ContinueButton;
    [HideInInspector] public Slider BGMVolSlider;
    [HideInInspector] public Slider SFXVolSlider;



    // debug menu
    [HideInInspector] public Button GiveMove1Button;
    [HideInInspector] public Button GiveMove2Button;
    [HideInInspector] public Button GiveMove3Button;
    [HideInInspector] public Button GiveMove4Button;

    [HideInInspector] public Button NextLvlButton;
    [HideInInspector] public Button PrevLvlButton;

    #endregion

    #region draw methods

    [Button, DisableInEditorMode]
    public void ClearMenus()
    {
        CentralPanel.Clear();
    }

    public void DrawPauseMenu()
    {
        CentralPanel.Clear();
        CentralPanel.Add(PauseMenu);
    }

    [Button, DisableInEditorMode]
    public void DrawWinMenu()
    {
        CentralPanel.Clear();
        CentralPanel.Add(WinMenu);
    }

    //internal void DrawLossMenu()
    //{
    //    CentralPanel.Clear();
    //    CentralPanel.Add(LossMenu);
    //}

    #endregion
}

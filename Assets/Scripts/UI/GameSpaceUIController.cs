using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpaceUIController : MonoBehaviour
{
    [SerializeField]
    private GameSpaceUIHandler _handler;

    private void Awake()
    {
        InitGameUI();
        InitPauseMenu();
        InitWinMenu();
        _handler.ClearMenus();
    }

    #region public methods
    public void ShowWinScreen()
    {
        print("Showing Win Screen");
        _handler.DrawWinMenu();
    }
    #endregion

    #region debug methods

    private void DebugLoadLevel(int direction)
    {
        if (direction > 0)
        {
            print("Load Next Level Clicked");
        }
        else
        {
            print("Load Prev Level Clicked");
        }
    }

    public void DebugSliderValue(string name, float val)
    {
        print($"{name} slider has {val} value");
    }

    #endregion

    #region Element interaction Methods

    private void GiveMoveCard(int amount)
    {
        print($"Give {amount} move card(s)");
    }

    private void OnResetClicked()
    {
        _handler.ClearMenus();
        print("Reset Clicked");
    }

    private void OnUndoClicked()
    {
        print("Undo Clicked");
    }

    private void OnPauseMenuClicked()
    {
        print("Pause Clicked");
        _handler.ClearMenus();
        _handler.DrawPauseMenu();
    }

    // win menu
    private void LoadNextLevel()
    {
        _handler.ClearMenus();
        print("Load Next Level");
    }

    // pause menu
    private void ClosePauseMenu()
    {
        _handler.ClearMenus();
    }

    private void GoToMainMenu()
    {
        _handler.ClearMenus();
        print("Go To Main Menu Clicked");
    }

    // audio options
    private void SetBGMVol(float target)
    {

    }

    private void SetSFXVol(float target)
    {

    }

    #endregion

    #region init methods

    private void InitGameUI()
    {
        _handler._root = _handler.UIDoc.rootVisualElement;

        //_handler.WinMenu = _handler.WinMenuDoc.CloneTree();

        _handler.CentralPanel = _handler._root.Q<VisualElement>(GameSpaceUIHandler.k_CentralPanel);

        _handler.UndoButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_UndoButtonName);
        _handler.UndoButton.clicked += () => OnUndoClicked();
        
        _handler.ResetButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_ResetButtonName);
        _handler.ResetButton.clicked += () => OnResetClicked();

        _handler.PauseButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_PauseButton);
        _handler.PauseButton.clicked += () => OnPauseMenuClicked();


        // debug methods
        _handler.DebugGiveMove1Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give1MoveButtonName);
        _handler.DebugGiveMove1Button.clicked += () => GiveMoveCard(1);

        _handler.DebugGiveMove2Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give2MoveButtonName);
        _handler.DebugGiveMove2Button.clicked += () => GiveMoveCard(2);

        _handler.DebugGiveMove3Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give3MoveButtonName);
        _handler.DebugGiveMove3Button.clicked += () => GiveMoveCard(3);

        _handler.DebugGiveMove4Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give4MoveButtonName);
        _handler.DebugGiveMove4Button.clicked += () => GiveMoveCard(4);


        _handler.DebugNextLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_DebugNextLvlButtonName);
        _handler.DebugNextLvlButton.clicked += () => DebugLoadLevel(1);

        _handler.DebugPrevLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_DebugPrevLvlMoveButtonName);
        _handler.DebugPrevLvlButton.clicked += () => DebugLoadLevel(0);
    }

    private void InitPauseMenu()
    {
        _handler.PauseMenu = _handler.PauseMenuDoc.CloneTree();

        _handler.MainMenuFromPauseButton = _handler.PauseMenu.Q<Button>(GameSpaceUIHandler.k_MainMenuFromPauseButtonName);
        _handler.MainMenuFromPauseButton.clicked += () => GoToMainMenu();

        _handler.ContinueButton = _handler.PauseMenu.Q<Button>(GameSpaceUIHandler.k_ContinueButtonName);
        _handler.ContinueButton.clicked += () => ClosePauseMenu();

        _handler.BGMVolSlider = _handler.PauseMenu.Q<Slider>(GameSpaceUIHandler.k_BGMSliderName);
        _handler.BGMVolSlider.RegisterValueChangedCallback(evt =>
        {
            DebugSliderValue(GameSpaceUIHandler.k_BGMSliderName, evt.newValue);
            SetBGMVol(evt.newValue);
        });

        _handler.SFXVolSlider = _handler.PauseMenu.Q<Slider>(GameSpaceUIHandler.k_SFXSliderName);
        _handler.SFXVolSlider.RegisterValueChangedCallback(evt =>
        {
            DebugSliderValue(GameSpaceUIHandler.k_SFXSliderName, evt.newValue);
            SetBGMVol(evt.newValue);
        });
    }

    private void InitWinMenu()
    {
        _handler.WinMenu = _handler.WinMenuDoc.CloneTree();

        _handler.MainMenuFromWinButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_MainMenuFromWinButtonName);
        _handler.MainMenuFromWinButton.clicked += () => GoToMainMenu();

        _handler.NextLevelFromWinButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_NextLevelButtonName);
        _handler.NextLevelFromWinButton.clicked += () => LoadNextLevel();

        _handler.RedoButton = _handler.WinMenu.Q<Button>(GameSpaceUIHandler.k_RedoButtonName);
        _handler.RedoButton.clicked += () => OnResetClicked();
    }


    #endregion
}

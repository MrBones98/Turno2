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
        print("Reset Clicked");
    }

    private void OnUndoClicked()
    {
        print("Undo Clicked");
    }

    private void OnPauseMenuClicked()
    {
        print("Pause Clicked");
        _handler.DrawPauseMenu();
    }

    public void ShowWinScreen()
    {
        print("Showing Win Screen");
        _handler.DrawWinMenu();
    }

    // pause menu
    private void ClosePauseMenu()
    {
        _handler.ClearMenus();
    }

    private void GoToMainMenu()
    {
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
        _handler.GiveMove1Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give1MoveButtonName);
        _handler.GiveMove1Button.clicked += () => GiveMoveCard(1);

        _handler.GiveMove2Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give2MoveButtonName);
        _handler.GiveMove2Button.clicked += () => GiveMoveCard(2);

        _handler.GiveMove3Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give3MoveButtonName);
        _handler.GiveMove3Button.clicked += () => GiveMoveCard(3);

        _handler.GiveMove4Button = _handler._root.Q<Button>(GameSpaceUIHandler.k_Give4MoveButtonName);
        _handler.GiveMove4Button.clicked += () => GiveMoveCard(4);


        _handler.NextLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_NextLvlButtonName);
        _handler.NextLvlButton.clicked += () => DebugLoadLevel(1);

        _handler.PrevLvlButton = _handler._root.Q<Button>(GameSpaceUIHandler.k_PrevLvlMoveButtonName);
        _handler.PrevLvlButton.clicked += () => DebugLoadLevel(0);
    }

    private void InitPauseMenu()
    {
        _handler.PauseMenu = _handler.PauseMenuDoc.CloneTree();

        _handler.MainMenuButton = _handler.PauseMenu.Q<Button>(GameSpaceUIHandler.k_MainMenuButtonName);
        _handler.MainMenuButton.clicked += () => GoToMainMenu();

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
    }

    #endregion
}

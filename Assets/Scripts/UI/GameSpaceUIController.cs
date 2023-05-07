using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpaceUIController : MonoBehaviour
{
    [SerializeField]
    private GameSpaceUIHandler _menuHandler;

    private void Awake()
    {
        Init();
    }

    #region Element interaction Methods
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
    #endregion

    #region init methods

    private void Init()
    {
        _menuHandler._root = _menuHandler.UIDoc.rootVisualElement;

        _menuHandler.UndoButton = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_UndoButtonName);
        _menuHandler.UndoButton.clicked += () => OnUndoClicked();

        _menuHandler.ResetButton = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_ResetButtonName);
        _menuHandler.ResetButton.clicked += () => OnResetClicked();


        // debug methods
        _menuHandler.GiveMove1Button = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_Give1MoveButtonName);
        _menuHandler.GiveMove1Button.clicked += () => GiveMoveCard(1);

        _menuHandler.GiveMove2Button = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_Give2MoveButtonName);
        _menuHandler.GiveMove2Button.clicked += () => GiveMoveCard(2);

        _menuHandler.GiveMove3Button = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_Give3MoveButtonName);
        _menuHandler.GiveMove3Button.clicked += () => GiveMoveCard(3);

        _menuHandler.GiveMove4Button = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_Give4MoveButtonName);
        _menuHandler.GiveMove4Button.clicked += () => GiveMoveCard(4);


        _menuHandler.NextLvlButton = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_NextLvlButtonName);
        _menuHandler.NextLvlButton.clicked += () => DebugLoadLevel(1);

        _menuHandler.PrevLvlButton = _menuHandler._root.Q<Button>(GameSpaceUIHandler.k_PrevLvlMoveButtonName);
        _menuHandler.PrevLvlButton.clicked += () => DebugLoadLevel(0);

    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameSpaceUIHandler : MonoBehaviour
{
    public UIDocument UIDoc;

    #region element IDs

    public const string k_ResetButtonName = "ResetButton";
    public const string k_UndoButtonName = "UndoButton";

    // debug menu
    public const string k_Give1MoveButtonName = "GiveMove1";
    public const string k_Give2MoveButtonName = "GiveMove2";
    public const string k_Give3MoveButtonName = "GiveMove3";
    public const string k_Give4MoveButtonName = "GiveMove4";

    public const string k_NextLvlButtonName = "NextLvl";
    public const string k_PrevLvlMoveButtonName = "PrevLvl";

    #endregion

    #region element caches

    [HideInInspector] public VisualElement _root;

    [HideInInspector] public Button ResetButton;
    [HideInInspector] public Button UndoButton;

    // debug menu
    [HideInInspector] public Button GiveMove1Button;
    [HideInInspector] public Button GiveMove2Button;
    [HideInInspector] public Button GiveMove3Button;
    [HideInInspector] public Button GiveMove4Button;

    [HideInInspector] public Button NextLvlButton;
    [HideInInspector] public Button PrevLvlButton;

    #endregion
}

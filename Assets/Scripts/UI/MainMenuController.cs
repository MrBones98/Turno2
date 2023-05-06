using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private List<Level> levels;

    #region Debug Methods
    public void DebugButtonPress(string name)
    {
        print($"{name} button pressed");
    }

    public void DebugSliderValue(string name, float val)
    {
        print($"{name} slider has {val} value");
    }
    #endregion
}

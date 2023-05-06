using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using System;

public class LvlSelectButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtField;
    [SerializeField]
    private string _lvlName = "default";
    [SerializeField]
    private int _lvlNumber = -666;
    [SerializeField]
    private Button _button;

    public void Init(LvlData data, int lvlNumber)
    {
        _lvlName = data.LvlName;
        _lvlNumber = lvlNumber;
        _txtField.SetText($"{_lvlNumber} / {_lvlName}");

    }

}

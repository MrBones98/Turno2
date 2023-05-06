using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class LvlSelectScrollHandler : MonoBehaviour
{
    [TableList]
    public List<LvlData> _lvlData = new List<LvlData>();
    [SerializeField]
    private GameObject _container;
    [SerializeField]
    private GameObject _buttonPrefab;

    private void Awake()
    {
        if (_lvlData.Count < 1)
            return;

        int count = 1;
        foreach (var lvl in _lvlData)
        {            
            var button = Instantiate(_buttonPrefab, _container.transform);
            button.GetComponent<LvlSelectButton>().Init(lvl, count);
            button.name = count+"_LvlButton";
            count++;
        }
    }
}
[Serializable]
public class LvlData
{
    public string LvlName = "Default Name";
}

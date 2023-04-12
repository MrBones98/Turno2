using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRedButtonAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonObject;
    private Material _buttonMaterialInstance;
    [SerializeField]
    private Color _buttonColor;

    private void Awake() 
    {
        _buttonMaterialInstance = _buttonObject.GetComponent<MeshRenderer>().material;
        _buttonMaterialInstance.color = _buttonColor;
    }   
}

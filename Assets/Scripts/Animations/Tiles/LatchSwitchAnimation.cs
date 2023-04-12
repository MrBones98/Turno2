using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatchSwitchAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject _buttonObject, _lightObject, _pilonObject;
    private Material _buttonMaterialInstance, _lightMaterialInstance, _pilonMaterialInstance;
    [SerializeField]
    private Color _latchSwitchColor, _deactivatedLightColor, _pilonColor;

    private void Awake() 
    {
        _buttonMaterialInstance = _buttonObject.GetComponent<MeshRenderer>().material;
        _lightMaterialInstance = _lightObject.GetComponent<MeshRenderer>().material;
        _pilonMaterialInstance = _pilonObject.GetComponent<MeshRenderer>().material;
        _buttonMaterialInstance.color = _latchSwitchColor;
        _lightMaterialInstance.color = _deactivatedLightColor;
        _pilonMaterialInstance.color = _pilonColor;
    }
}

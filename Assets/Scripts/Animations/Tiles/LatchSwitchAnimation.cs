using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LatchSwitchAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _buttonObject, _lightObject, _pilonObject;

    private Material _buttonMaterialInstance, _lightMaterialInstance, _pilonMaterialInstance;

    [SerializeField]
    private Color _latchSwitchColor, _deactivatedLightColor, _pilonColor;

    [SerializeField]
    private float _pressDistance = -0.02f;
    private Vector3 _buttonStartPosition;
    [SerializeField]
    private float _pressSpeed = 1f;

    private void OnEnable()
    {
        GameManager.onGameStarted += OnLevelLoaded;
    }
    private void OnDisable()
    {
        GameManager.onGameStarted -= OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        CacheButtonPosition();
    }

    private void Awake()
    {
        CacheEverything();
        SetColours();
    }

    [Button, DisableInEditorMode]
    public void PressButton()
    {
        _buttonObject.transform.DOMove(_buttonStartPosition + new Vector3(0,_pressDistance,0), _pressSpeed);
        SetPilonLightState(true);
    }

    [Button, DisableInEditorMode]
    public void RaiseButton()
    {
        _buttonObject.transform.DOMove(_buttonStartPosition, _pressSpeed);
        SetPilonLightState(false);
    }

    private void SetPilonLightState(bool isOn)
    {
        if (isOn)
        {
            _lightMaterialInstance.color = _latchSwitchColor;
        }
        else
        {
            _lightMaterialInstance.color = _deactivatedLightColor;
        }
    }

    private void SetColours()
    {
        _buttonMaterialInstance.color = _latchSwitchColor;
        _lightMaterialInstance.color = _deactivatedLightColor;
        _pilonMaterialInstance.color = _pilonColor;
    }

    private void CacheEverything()
    {
        _buttonMaterialInstance = _buttonObject.GetComponent<MeshRenderer>().material;
        _lightMaterialInstance = _lightObject.GetComponent<MeshRenderer>().material;
        _pilonMaterialInstance = _pilonObject.GetComponent<MeshRenderer>().material;

    }


    private void CacheButtonPosition()
    {
        _buttonStartPosition = _buttonObject.transform.position;
    }
}

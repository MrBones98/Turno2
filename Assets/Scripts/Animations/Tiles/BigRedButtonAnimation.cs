using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class BigRedButtonAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _buttonObject;
    private Material _buttonMaterialInstance;
    [SerializeField]
    private Color _buttonColor;

    [SerializeField]
    private float _pressDistance = -0.02f;
    private Vector3 _buttonStartPosition;
    [SerializeField]
    private float _pressSpeed = .3f;


    private void Awake()
    {
        CacheEverything();
        SetColors();
    }

    [Button, DisableInEditorMode]
    public void PressButton()
    {
        _buttonObject.transform.DOMove(_buttonStartPosition + new Vector3(0, _pressDistance, 0), _pressSpeed);
    }

    [Button, DisableInEditorMode]
    public void RaiseButton()
    {
        _buttonObject.transform.DOMove(_buttonStartPosition, _pressSpeed);
    }

    private void SetColors()
    {
        _buttonMaterialInstance.color = _buttonColor;
    }

    private void CacheEverything()
    {
        _buttonMaterialInstance = _buttonObject.GetComponent<MeshRenderer>().material;
        //CacheBigRedButtonPosition();
    }

    private void CacheBigRedButtonPosition()
    {
        _buttonStartPosition = _buttonObject.transform.position;
    }
    private void OnEnable()
    {
        ScriptableObjectLoader.onLevelLoaded += OnLevelLoaded;
    }
    private void OnDisable()
    {
        ScriptableObjectLoader.onLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        CacheBigRedButtonPosition();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MovingTileAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _lightForward, _lightBack;
    private Material _lForwardMaterial, _lBackMaterial;
    [SerializeField]
    private Color _lightColor, _disabledColor;

    private void Awake()
    {
        CacheMaterials();

        SetColors();
    }

    [Button, DisableInEditorMode]
    public void LightForward()
    {
        _lForwardMaterial.color = _lightColor;
        _lBackMaterial.color = _disabledColor;
    }

    [Button, DisableInEditorMode]
    public void LightBack()
    {
        _lForwardMaterial.color = _disabledColor;
        _lBackMaterial.color = _lightColor;
    }

    [Button, DisableInEditorMode]
    public void LightsOff()
    {
        _lForwardMaterial.color = _disabledColor;
        _lBackMaterial.color = _disabledColor;
    }

    private void SetColors()
    {
        LightsOff();
    }

    private void CacheMaterials()
    {
        _lForwardMaterial = _lightForward.GetComponent<Renderer>().material;
        _lBackMaterial = _lightBack.GetComponent<Renderer>().material;
    }
}

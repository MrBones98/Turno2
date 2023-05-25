using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BasicTileAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _baseObject, _lightObject;
    private Material _baseMaterialInstance, _lightMaterialInstance;
    [SerializeField]
    private Color _baseColor, _lightColor;


    private void Awake()
    {
        CacheEverthing();
        SetColors();
    }

    private void SetColors()
    {
        //if(gameObject.GetComponent<Tile>().IsVoidHighlightTile
        _baseMaterialInstance.color = _baseColor;
        _lightMaterialInstance.color = _lightColor;
        
    }

    private void CacheEverthing()
    {
        _baseMaterialInstance = _baseObject.GetComponent<MeshRenderer>().material;
        _lightMaterialInstance = _lightObject.GetComponent<MeshRenderer>().material;

    }
}

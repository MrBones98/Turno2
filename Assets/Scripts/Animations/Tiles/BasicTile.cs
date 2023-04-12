using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTileAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject _baseObject, _lightObject;
    private Material _baseMaterialInstance, _lightMaterialInstance;
    [SerializeField]
    private Color _baseColor, _lightColor;

    // // Start is called before the first frame update
    // private void Awake() 
    // {
    //     _baseMaterialInstance = _baseObject.GetComponent<MeshRenderer>().material;
    //     _lightMaterialInstance = _lightObject.GetComponent<MeshRenderer>().material;
    // }

    private void Awake() 
    {
        _baseMaterialInstance = _baseObject.GetComponent<MeshRenderer>().material;
        _lightMaterialInstance = _lightObject.GetComponent<MeshRenderer>().material;
        _baseMaterialInstance.color = _baseColor;
        _lightMaterialInstance.color = _lightColor;
    }
}

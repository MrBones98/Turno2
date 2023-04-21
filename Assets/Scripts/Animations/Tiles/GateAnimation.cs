using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class GateAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _gateCentre, _gateN, _gateS, _gateE, _gateW;
    private Material _gMC, _gMN, _gMS, _gME, _gMW;
    [SerializeField]
    private Color GateColor;

    private Vector3 _gateClosedPos_C,_gateClosedPos_N,_gateClosedPos_S,_gateClosedPos_E,_gateClosedPos_W;
    [SerializeField]
    private float OpenDistance = -.5f;
    [SerializeField]
    private float _gateSpeed = 0.1f;

    public bool isGateOpen { get => _isGateOpen; }
    private bool _isGateOpen = false;

    private void Awake()
    {
        CacheEverything();

        SetColors();
    }



    [Button,DisableInEditorMode]
    public void OpenGate()
    {
        _gateCentre.transform.DOMove(_gateClosedPos_C + new Vector3(0, OpenDistance - .11f, 0), _gateSpeed, false); 
        _gateN.transform.DOMove(_gateClosedPos_N + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateS.transform.DOMove(_gateClosedPos_S + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateE.transform.DOMove(_gateClosedPos_E + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateW.transform.DOMove(_gateClosedPos_W + new Vector3(0, OpenDistance, 0), _gateSpeed);
  
    }

    [Button, DisableInEditorMode]
    public void CloseGate()
    {
        _gateCentre.transform.DOMove(_gateClosedPos_C, _gateSpeed);
        _gateN.transform.DOMove(_gateClosedPos_N, _gateSpeed);
        _gateS.transform.DOMove(_gateClosedPos_S,_gateSpeed);
        _gateE.transform.DOMove(_gateClosedPos_E, _gateSpeed);
        _gateW.transform.DOMove(_gateClosedPos_W, _gateSpeed);
    }

    private void CacheEverything()
    {
        _gMC = _gateCentre.GetComponent<MeshRenderer>().material;
        _gMN = _gateN.GetComponent<MeshRenderer>().material;
        _gMS = _gateS.GetComponent<MeshRenderer>().material;
        _gME = _gateE.GetComponent<MeshRenderer>().material;
        _gMW = _gateW.GetComponent<MeshRenderer>().material;

        // cache closed gate positions
        _gateClosedPos_C = _gateCentre.transform.position;
        _gateClosedPos_N = _gateN.transform.position;
        _gateClosedPos_S = _gateS.transform.position;
        _gateClosedPos_E = _gateE.transform.position;
        _gateClosedPos_W = _gateW.transform.position;
    }

    private void SetColors()
    {
        _gMC.color = GateColor;
        _gMN.color = GateColor;
        _gMS.color = GateColor;
        _gME.color = GateColor;
        _gMW.color = GateColor;
    }
}

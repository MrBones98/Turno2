using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;
using System.Threading.Tasks;

public class GateAnimation : MonoBehaviour
{
    [SerializeField]
    private bool _showObjects;

    [ShowIf("_showObjects"), SerializeField]
    private GameObject _gateCentre, _gateN, _gateS, _gateE, _gateW;
    private Material _gMC, _gMN, _gMS, _gME, _gMW;
    [SerializeField]
    private Color GateColor;

    [SerializeField]private Vector3 _gateClosedPos_C,_gateClosedPos_N,_gateClosedPos_S,_gateClosedPos_E,_gateClosedPos_W;
    [SerializeField]
    private float OpenDistance = -.5f;
    [SerializeField]
    private float _gateSpeed = 0.1f;

    public bool isGateOpen { get => _isGateOpen; }
    private bool _isGateOpen = false;

    private void Awake()
    {
        CacheMaterials();

        SetColors();
    }



    [Button,DisableInEditorMode]
    public void OpenGate()
    {
        _gateCentre.transform.DOMove(new Vector3(_gateClosedPos_C.x, 0, _gateClosedPos_C.z) + new Vector3(0, OpenDistance - .11f, 0), _gateSpeed, false); 
        _gateN.transform.DOMove(new Vector3(_gateClosedPos_N.x,0, _gateClosedPos_N.z) + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateS.transform.DOMove(new Vector3(_gateClosedPos_S.x, 0, _gateClosedPos_S.z) + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateE.transform.DOMove(new Vector3(_gateClosedPos_E.x, 0, _gateClosedPos_E.z) + new Vector3(0, OpenDistance, 0), _gateSpeed); 
        _gateW.transform.DOMove(new Vector3(_gateClosedPos_W.x, 0, _gateClosedPos_W.z) + new Vector3(0, OpenDistance, 0), _gateSpeed);
  
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

    private void CacheMaterials()
    {
        _gMC = _gateCentre.GetComponent<MeshRenderer>().material;
        _gMN = _gateN.GetComponent<MeshRenderer>().material;
        _gMS = _gateS.GetComponent<MeshRenderer>().material;
        _gME = _gateE.GetComponent<MeshRenderer>().material;
        _gMW = _gateW.GetComponent<MeshRenderer>().material;

        // cache closed gate positions
        //CacheGatePosition();
    }

    private void CacheGatePosition()
    {
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
    private void OnEnable()
    {
        GameManager.onGameStarted += OnLevelLoaded;
    }

    private async void OnLevelLoaded()
    {
        await Task.Delay(300);
        CacheGatePosition();
    }

    private void OnDisable()
    {
        GameManager.onGameStarted += OnLevelLoaded;
    }
}

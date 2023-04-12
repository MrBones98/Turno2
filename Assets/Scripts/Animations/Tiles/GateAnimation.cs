using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject _gateCentre, _gateN, _gateS, _gateE, _gateW;
    private Material _gMC, _gMN, _gMS, _gME, _gMW;
    [SerializeField]
    private Color GateColor;

    private Vector3 _gateClosedPos_C,_gateClosedPos_N,_gateClosedPos_S,_gateClosedPos_E,_gateClosedPos_W;
    [SerializeField]
    private float OpenDistance = -.5f;

    private void Awake() 
    {
        _gMC = _gateCentre.GetComponent<MeshRenderer>().material;
        _gMN = _gateN.GetComponent<MeshRenderer>().material;
        _gMS = _gateS.GetComponent<MeshRenderer>().material;
        _gME = _gateE.GetComponent<MeshRenderer>().material;
        _gMW = _gateW.GetComponent<MeshRenderer>().material;

        _gMC.color = GateColor;
        _gMN.color = GateColor;
        _gMS.color = GateColor;
        _gME.color = GateColor;
        _gMW.color = GateColor;

        // cache closed gate positions
        _gateClosedPos_C = _gateCentre.position; 
        _gateClosedPos_N = _gateN.position; 
        _gateClosedPos_S = _gateS.position; 
        _gateClosedPos_E = _gateE.position; 
        _gateClosedPos_W = _gateW.position; 

        //OpenGate();
    }

    // public void OpenGate()
    // {
    //     _gateCentre.transform.position = transform.position + new Vector3(0, OpenDistance, 0); 
    //     _gateN.transform.position = transform.position + new Vector3(0, OpenDistance, 0); 
    //     _gateS.transform.position = transform.position + new Vector3(0, OpenDistance, 0); 
    //     _gateE.transform.position = transform.position + new Vector3(0, OpenDistance, 0); 
    //     _gateW.transform.position = transform.position + new Vector3(0, OpenDistance, 0);   
    // }
}

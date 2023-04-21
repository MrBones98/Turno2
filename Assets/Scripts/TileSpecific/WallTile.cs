using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Tile, ISwitchActivatable
{
    private GateAnimation _animation;
    public void Activate()
    {
        _animation.OpenGate();
    }

    public void Deactivate()
    {
        _animation.CloseGate();
    }

    private void Awake()
    {
        _animation = gameObject.GetComponent<GateAnimation>();
    }
}

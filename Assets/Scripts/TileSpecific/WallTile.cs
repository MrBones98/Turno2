using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Tile, ISwitchActivatable
{
    public bool HasColision;

    private GateAnimation _animation;
    public void Activate()
    {
        _animation.OpenGate();
        HasColision = false;
    }

    public void Deactivate()
    {
        _animation.CloseGate();
        HasColision = true;
    }

    private void Awake()
    {
        _animation = gameObject.GetComponent<GateAnimation>();
        HasColision = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallTile : Tile, ISwitchActivatable
{
    public bool HasColision;
    public delegate void OnWallHighlighted(int id);
    public static event OnWallHighlighted onWallHighlighted;

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


    public void OnPointerEnter(PointerEventData eventData)
    {
        print("enter");
        onWallHighlighted(InteractableID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit");
        onWallHighlighted(InteractableID);
    }

    private void Awake()
    {
        _animation = gameObject.GetComponent<GateAnimation>();
        if (StartsActivated)
        {
            Activate();
        }
        else
        {
            HasColision = true;
        }
    }

    public void HighlightInteractable(float height)
    {
        transform.parent.position+= new Vector3(0, height, 0);
    }
}

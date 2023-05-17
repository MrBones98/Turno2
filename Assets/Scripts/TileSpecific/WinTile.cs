using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinTile : Tile, ISwitchActivatable
{
    [SerializeField] private Collider _buttonCollider;

    public delegate void OnButtonPressed();
    public static event OnButtonPressed onButtonPressed;

    private BigRedButtonAnimation _buttonAnimation;
    private void Awake()
    {
      _buttonAnimation = gameObject.GetComponent<BigRedButtonAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bot>())
        {
            if (!other.GetComponent<Bot>().IsMoving)
            {
                onButtonPressed();
                print("Wiiiiiiiiiiiiiii");
            }
        }
    }

    public void Activate()
    {
        _buttonAnimation.PressButton();
    }

    public void Deactivate()
    {
        //
    }

    public void HighlightInteractable(float height)
    {
       // throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}

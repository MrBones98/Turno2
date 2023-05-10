using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class SwitchTile : Tile, ISwitchActivatable, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void OnSwitchPressed(int id);
    public static event OnSwitchPressed onSwitchPressed;
    public delegate void OnSwitchReleased(int id);
    public static event OnSwitchReleased onSwitchReleased;
    public delegate void OnSwitchHighlighted(int id);
    public static event OnSwitchHighlighted onSwitchHighlighted;



    [SerializeField] private bool _isLatchSwitch = false;

    private LatchSwitchAnimation _animation;

    public void Awake()
    {
        _animation = gameObject.GetComponent<LatchSwitchAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (other.GetComponent<Bot>())
        {
            onSwitchPressed(InteractableID);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!_isLatchSwitch)
        {
            onSwitchReleased(InteractableID);
        }
    }

    public void Activate()
    {
        _animation.PressButton();
    }

    public void Deactivate()
    {
        _animation.RaiseButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("enter");
        onSwitchHighlighted(InteractableID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("exit");
        onSwitchHighlighted(InteractableID);
    }

    public void HighlightInteractable(float height)
    {
        print(height);
        //print(transform.parent.gameObject.name);
        transform.position += new Vector3(0, height, 0);
    }
}

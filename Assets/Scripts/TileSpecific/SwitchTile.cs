using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchTile : Tile, ISwitchActivatable
{
    public delegate void OnSwitchPressed(int id);
    public static event OnSwitchPressed onSwitchPressed;
    public delegate void OnSwitchReleased(int id);
    public static event OnSwitchReleased onSwitchReleased;


    [SerializeField] private bool _isLatchSwitch = false;

    private LatchSwitchAnimation _animation;

    public void Awake()
    {
        _animation = gameObject.GetComponent<LatchSwitchAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name + " / " + gameObject.name);
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
}

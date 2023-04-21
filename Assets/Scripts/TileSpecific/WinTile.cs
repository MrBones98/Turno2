using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        onButtonPressed();
        print("Wiiiiiiiiiiiiiii");
    }

    public void Activate()
    {
        _buttonAnimation.PressButton();
    }

    public void Deactivate()
    {
        //
    }
}

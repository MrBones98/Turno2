using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwitchTile : Tile
{
    public delegate void OnSwitchPressed(int id);
    public static event OnSwitchPressed onSwitchPressed;

    [SerializeField] private bool _isLatchSwitch = false;

    private void OnTriggerEnter(Collider other)
    {
        //onSwitchPressed(1);
    }
}

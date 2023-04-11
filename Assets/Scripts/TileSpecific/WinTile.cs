using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTile : Tile
{
    [SerializeField] private Collider _buttonCollider;

    public delegate void OnButtonPressed();
    public static event OnButtonPressed onButtonPressed;
    private void Awake()
    {
      //GameManager.Instance.WinTile = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        onButtonPressed();
        print("Wiiiiiiiiiiiiiii");
    }
}

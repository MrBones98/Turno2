using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinTile : Tile, ISwitchActivatable
{
    [SerializeField] private Collider _buttonCollider;
    [SerializeField] private Transform[] _particleEmitterPositions;
    [SerializeField] private GameObject _winParticle;

    public delegate void OnButtonPressed();
    public static event OnButtonPressed onButtonPressed;

    private BigRedButtonAnimation _buttonAnimation;
    private bool _buttonPressed=false;
    private void Awake()
    {
      _buttonAnimation = gameObject.GetComponent<BigRedButtonAnimation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bot>())
        {
            bool isMoving = other.GetComponent<Bot>().IsMoving;

            if (!other.GetComponent<Bot>().IsMoving && !_buttonPressed)
            {
                _buttonPressed = true;
                onButtonPressed();
                foreach (Transform transform in _particleEmitterPositions)
                {
                    Instantiate(_winParticle, transform);
                }
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

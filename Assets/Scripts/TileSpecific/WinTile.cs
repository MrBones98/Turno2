using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinTile : Tile, ISwitchActivatable
{
    [SerializeField] private Collider _buttonCollider;
    [SerializeField] private Transform[] _particleEmitterPositions;
    [SerializeField] private GameObject _winParticle;

    public delegate void OnButtonPressed();
    public static event OnButtonPressed onButtonPressed;

    private string[] _layersToCheck = { "Platform", "Pushable", "Wall", "Player" };
    int _collidableLayers;

    private BigRedButtonAnimation _buttonAnimation;
    private bool _buttonPressed=false;
    private void Awake()
    {
      _buttonAnimation = gameObject.GetComponent<BigRedButtonAnimation>();
    }
    private void Start()
    {
        _collidableLayers = LayerMask.GetMask(_layersToCheck);
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
                    //WinTween();
                }
                print("Wiiiiiiiiiiiiiii");
            }
        }
    }
    private async Task WinTween()
    {
        RaycastHit[] raycastHits = Physics.SphereCastAll(gameObject.transform.position, 8,Vector3.zero,  10);
     
        await Task.Yield();
        System.Array.Sort(raycastHits, (x,z)=> x.distance.CompareTo(z.distance));
        Transform transformToLift;
        foreach (RaycastHit hit in raycastHits)
        {
            transformToLift = hit.collider.transform.parent.transform;
            //hit.collider.transform.parent.transform.DOMoveY(0.3f, 0.3f, false).onComplete()=> { hit.collider.transform.parent.transform.DOMoveY(0.3f, 0.3f, false); };
            transformToLift.DOMoveY(transformToLift.position.y+0.3f, 0.3f, false).OnComplete(()=> LowerTilesTween(hit.collider.transform.parent.transform));
            await Task.Delay(50);
        }

    }
    private async void LowerTilesTween(Transform transformToLower)
    {
        transformToLower.DOMoveY(transformToLower.position.y - 0.3f, 0.3f, false);
        await Task.Delay(50);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class MovingTile : Tile,ISwitchActivatable
{
    [SerializeField] private GameObject _ghostTile;
    private int _count=0;
    private MovingTileAnimation _animation;
    private GameObject _carriedObject;
    private bool _active;
    private bool _ghostDrawn = false;
    private float _moveDuration;
    private float _speed;
    private void Awake()
    {
        //TODO
        //Take into consideration the Direction x or y to rotate model correctly (might need a container to not mess up
        //the transform+ direction
        _animation = GetComponent<MovingTileAnimation>();
        _moveDuration = Tweener.Instance.MovinPlatformMoveDuration;
        _ghostTile.SetActive(false);
    }
    private void OnEnable()
    {
        GameManager.onBotMove += UpdateTurn;
    }
    private void Start()
    {
        _active = StartsActivated;
        if (_active)
        {
            _animation.LightForward();
        }
        else
        {
            _animation.LightsOff();
        }

        if(Direction.y < 0)
        {
           
            print("Movving Platform should rotate");
            transform.Rotate(0, 90, 0);
        }
        else if(Direction.y > 0)
        {
            transform.Rotate(0, 270, 0);
        }
    }

    //change to async
    private void UpdateTurn()
    {
        print($"Is mving platf active: {_active}");
        if(_active)
        StartCoroutine(nameof(MovingDelay));
    }
    //change to async
    private IEnumerator MovingDelay()
    {
        //print("Platform went into Move Function!");
        yield return new WaitForSeconds(0.5f);
        Vector3 endPos = new Vector3(Direction.x, 0, Direction.y)*Distance;
        if(Physics.Raycast(transform.position,endPos,Distance, 7))
        {
            Distance--;
        }
        int distance = Distance;
        while (distance>0)
        {
            if (_count % 2 == 0)
            {
                //transform.position = transform.position + new Vector3(Direction.x, 0, Direction.y);
                transform.DOMove(transform.position + endPos, _moveDuration);
                //print("moveMove");
                _animation.LightBack(); 
            }
            else
            {
                //transform.position = transform.position + new Vector3(-Direction.x, 0, -Direction.y);
                transform.DOMove(transform.position -endPos, _moveDuration);
                //print("moveMove");
                _animation.LightForward();
            }
            distance--;
        }
        _count++;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsMovable(other))
            _carriedObject.transform.SetParent(transform, true);
            //_carriedObject.transform.parent.parent.position += new Vector3(0, 0.45f, 0);


    }
    private void Update()
    {
        //print(IsHighlighted);
        GhostDisplay();
    }

    private void GhostDisplay()
    {
        if (IsHighlighted && _active && !_ghostDrawn) //perhaps not with _active?
        {
            _ghostTile.SetActive(true);
            _ghostDrawn = true;
            Vector3 endPos = new Vector3(Direction.x, 0, Direction.y) * Distance;
            if (_count % 2 == 0)
            {
                _ghostTile.transform.position += endPos;
            }
            else
            {
                _ghostTile.transform.position -= endPos;
            }
        }
        else if (!IsHighlighted)
        {
            _ghostDrawn = false;
            _ghostTile.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsMovable(other))
            _carriedObject.transform.SetParent(null, true);
            _carriedObject=null;
            //_carriedObject.transform.SetParent(_carriedObject.transform.parent.parent);
    }
    private bool IsMovable(Collider collider)
    {
        //TODO
        //Change it after iplementing interface!!
        if (collider.transform.GetComponent<Bot>())
        {
            _carriedObject = collider.transform.parent.parent.gameObject;
            print($"Triggered by: {collider.gameObject.name}");
            return true;

        }
        else if (collider.transform.GetComponent<PushableBox>())
        {
            _carriedObject = collider.gameObject;
            print($"Triggered by: {collider.gameObject.name}");
            return true;
        }
        else return false;
    }

    private void OnDisable()
    {
        GameManager.onBotMove -= UpdateTurn;
    }

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active=false;
        _animation.LightsOff();
    }

    public void HighlightInteractable()
    {
        //throw new NotImplementedException();
    }

    public void HighlightInteractable(float height)
    {
        throw new NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}

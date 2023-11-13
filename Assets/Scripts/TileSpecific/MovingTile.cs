using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Threading.Tasks;
using Assets.Scripts.Interactables;

public class MovingTile : Tile,ISwitchActivatable
{
    [SerializeField] private GameObject _ghostTile;

    private Vector3 _moveDireciton;
    
    private int _count=0;
    private InteractableObject _interactable;
    private MovingTileAnimation _animation;
    private GameObject _carriedObject;
    private bool _active;
    private bool _ghostDrawn = false;
    private bool _isOccupied = false;
    private bool _adjustedDistance = false;
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
    //returns the int by which the direction should be applied, depending on wheter a tile was found or not. 
    private int FindDistanceScaleValue(Vector3 direction)
    {
        //Think about how to do multiple moving platforms together
        //TODO Implement iteration along the path not just final set destination
        //TODO add direction *length instead of calculating inside of the call, also rework stopping at nwely found til;es while caching
        //original path!!!!!!!!!!!
        int adjustedValue = Distance;
        Tile tile;
        InteractableObject interactable;
        for (int i = 1; i < Distance+1; i++)
        {
            Vector3 pathCheckValue;
            if (_count % 2 == 0)
            {
                pathCheckValue = new Vector3(direction.x,0,direction.y) * i;

            }
            else
            {
                pathCheckValue = new Vector3(direction.x,0,direction.y) *-i;
    }
            print($"on the {i} check the vector is {pathCheckValue}");
            tile = GameManager.Instance.FindTile(new Vector3(transform.position.x, 0, transform.position.z) + pathCheckValue);
            interactable = GameManager.Instance.FindInteractable(new Vector3(transform.position.x, 0, transform.position.z) + pathCheckValue);
            print(new Vector3(transform.position.x, 0, transform.position.z) + pathCheckValue);
            
            if((tile != null && tile != gameObject.GetComponent<MovingTile>()) || interactable!=null)
            {
                print($"found {tile} or {interactable}");
                _adjustedDistance = true;
                adjustedValue =i-1;
                break;
            }
            else
            {
                _adjustedDistance=false;
            }
        }
        return adjustedValue;
    }

    private async void UpdateTurn()
    {
        //print($"Is mving platf active: {_active}");
        if(_active)
        await MovingDelay();
    }

    private async Task MovingDelay()
    {
        //await Task.Delay(500);
        await Task.Yield();
        print($" passing {Direction}");
        int distance = FindDistanceScaleValue(Direction);
//        //Collider[] hits = Physics.OverlapSphere(transform.position + endPos, 0.44f, 7);
//        Vector3 endPos = new Vector3(Direction.x, 0, Direction.y)*Distance;
//        //_moveDireciton = new Vector3(Direction.x, 0, Direction.y) * Distance;
//        RaycastHit[] raycastHits = Physics.RaycastAll(transform.position,endPos);
//        float hitDistance = 0;
//        if(raycastHits.Length>0 && !_adjustedDistance)
//        {
//            _adjustedDistance = true;
//           //print(raycastHits[0].collider.name);
//           //print(raycastHits[0].collider.transform.position);
//           hitDistance = Vector3.Distance(transform.position, raycastHits[0].collider.transform.position);
//            distance = (int)hitDistance -1;
//            Distance = distance;
////            print(Distance);
//           await Task.Yield();
//        }

        print($"Current distance is{distance}");
        //GameManager.Interactables.Remove(transform.position);
        //print($"removing from{transform.position}");
        _moveDireciton = new Vector3(Direction.x, 0, Direction.y) * distance;
        {

        }
        Vector3 positiveDictionaryKey = new Vector3((int)transform.position.x+_moveDireciton.x,0,(int)(transform.position.z + _moveDireciton.z));
        print (positiveDictionaryKey);
        if (_carriedObject != null)
        {
            print($"Removing interactable at{transform.position}");
            GameManager.Interactables.Remove(transform.position);
            if(_count%2 == 0)
            {
                RoundToUpdateDictionary(positiveDictionaryKey);
            }
            else
            {
                RoundToUpdateDictionary(transform.position - _moveDireciton);
            }
        }
        for (int i = 0; i <= distance; i++)
        {
            if (_count % 2 == 0)
            {
                //transform.position = transform.position + new Vector3(Direction.x, 0, Direction.y);
                transform.DOMove(positiveDictionaryKey, _moveDuration);
                //print("moveMove");
                _animation.LightBack(); 
            }
            else
            {
                //transform.position = transform.position + new Vector3(-Direction.x, 0, -Direction.y);
                
                transform.DOMove(transform.position -_moveDireciton, _moveDuration);
                //print("moveMove");
                _animation.LightForward();
            }
            distance--;
        }
        GameManager.Instance.AddToTileToDictionary(transform.position+_moveDireciton, gameObject.GetComponent<Tile>());
        
        _count++;
    }
    private void RoundToUpdateDictionary(Vector3 key)
    {
        if (_carriedObject != null)
        {
            GameManager.Instance.AddInteractableToDictionary(key, _interactable);
            foreach (var interact in GameManager.Interactables)
            {
                print($"{interact.Key} + {interact.Value.Type}");
            }
            print($"Interactables count: {GameManager.Interactables.Count}");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsMovable(other))
        {
            _isOccupied = true;
            _interactable = other.gameObject.GetComponent<InteractableObject>();
            _carriedObject.transform.SetParent(transform, true);
            print(_carriedObject.name);
            //RECACHE INTERACTABLE DICTIONARY
            
            //_carriedObject.transform.parent.parent.position += new Vector3(0, 0.45f, 0);

        }


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
        {
            _carriedObject.transform.SetParent(null, true);
            _carriedObject = null;
            _interactable = null;
        }
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

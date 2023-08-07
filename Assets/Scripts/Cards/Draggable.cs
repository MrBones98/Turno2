using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler,IPointerMoveHandler
{
    public int MoveCount { get { return _moveCount; } }
    public bool IsJumpCard { get { return _isJumpCard; } }
    public bool IsPickedUp { get { return _isPickedUp; } }

    [SerializeField] [Range(1,4)]private int _moveCount;
    [SerializeField] private TextMeshProUGUI _cardValueText;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private bool _isJumpCard;

    private Transform _originalHandParent = null;
    private GameObject _bot;
    private DirectionalInputBot _directionalInputBot;
    private GameManager _gameManager;
    private Vector3 _scaleAdditionVector;
    private float _hoverHeight;
    private float _originalHeight;
    private float _hoverDuration;
    private bool _isPickedUp = false;

    private Vector3 _startScale;

    public delegate void OnCardPickedUp();
    public static event OnCardPickedUp onCardPickedUp;
    public delegate void OnCardSelected(GameObject gameObject, Draggable draggable);
    public static event OnCardSelected onCardSelected;
    public delegate void OnCardDropped();
    public static event OnCardDropped onCardDropped;
    public delegate void OnCardGiven();
    public static event OnCardGiven onCardGiven;
    private void Awake()
    {
        
        _cardValueText.text = _moveCount.ToString();
        _gameManager = GameManager.Instance;
        _hoverHeight = Tweener.Instance.CardOnHoverHeight;
        _hoverDuration = Tweener.Instance.CardHoverDuration;
        _originalHeight = transform.position.y;
        
        //transform.DOMoveY(_originalHeight, 3);

    }
    private void Start()
    {
        _originalHandParent = transform.parent;
        //print(_originalHandParent.name);
        _scaleAdditionVector = new Vector3(transform.localScale.x, transform.localScale.y, 0) * 0.15f;
        //transform.DOMoveY(_originalHeight, _hoverDuration);
        _startScale = transform.localScale;

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //keep track of mouse position vs anchor point to have card movement relative to the grabbing point
        //AKA click on a corner and move it, without it jumping back to it
        //print("begin");
        //onCardPickedUp?.Invoke();
        //transform.DOScale(_startScale * .5f, .1f);
        //transform.SetParent(transform.parent.parent);
    }
    public void OnPointerDown()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isPickedUp == false)
        {
            onCardPickedUp?.Invoke();
            onCardSelected?.Invoke(gameObject,this);
            transform.DOScale(_startScale * .5f, .1f);
            transform.SetParent(transform.parent.parent);
            _isPickedUp = true;
        }
        else
        {
            //raise this from gameManager bc pointerclick does not work for an object you're not carrying around lmao
            onCardGiven?.Invoke();
            //BotCaching();
            onCardDropped?.Invoke();
        }
    }

    private void BotCaching()
    {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 40f, _layerMask))
        {

            if (hitInfo.collider.GetComponent<Bot>())
            {
                _bot = hitInfo.collider.gameObject;
                //transform.localScale-= _scaleAdditionVector;
            }
            else
            {
                //transform.localScale+= _scaleAdditionVector;
                _bot = null;
            }
        }
        //transform.SetParent(_originalHandParent, false);
        //PopDown();
        if (_bot != null)
        {
            onCardDropped?.Invoke();
            DropScaling();
            //TODO
            if (_isJumpCard == false)
            {
                _bot.GetComponent<Bot>().SetDistance(_moveCount);
            }
            else
            {
                _bot.GetComponent<Bot>().SetJumpDistance(_moveCount);
            }
            GameManager.Instance.AssignPlayer(_bot);
            //Don' Destroy, pass object to gamanager list as well as previous bot position c:
            //Destroy(gameObject, 0.1f);
        }
        else
        {
            ResetCard();
        }
    }

    public void DropScaling()
    {
        transform.DOScale(_startScale, .1f);
    }

    public void ResetCard()
    {
        _isPickedUp = false;
        transform.SetParent(_originalHandParent,false);
        transform.DOScale(_startScale, 0.1f);
        PopDown();
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        if (_isPickedUp)
        {
            //transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    { 
        //transform.position = eventData.position;
        //RaycastHit hitInfo;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if(Physics.Raycast(ray, out hitInfo, 40f, _layerMask))
        //{
            
        //    if (hitInfo.collider.GetComponent<Bot>())
        //    {
        //        _bot= hitInfo.collider.gameObject;
        //        transform.localScale-= _scaleAdditionVector;
        //    }
        //    else
        //    {
        //        transform.localScale+= _scaleAdditionVector;
        //        _bot= null;
        //    }
        //}
    }
    public void PopDown()
    {
        this.transform.position = new Vector3(_originalHandParent.position.x, _originalHandParent.position.y, _originalHandParent.position.z); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print("end");
        //onCardDropped?.Invoke();
        //transform.SetParent(_originalHandParent,false);
        //transform.DOScale(_startScale, .1f);
        //PopDown();
        //if (_bot != null)
        //{
        //    //TODO
        //    if(_isJumpCard == false)
        //    {
        //        _bot.GetComponent<Bot>().SetDistance(_moveCount);
        //    }
        //    else
        //    {
        //        _bot.GetComponent<Bot>().SetJumpDistance(_moveCount);
        //    }
        //    GameManager.Instance.AssignPlayer(_bot);
        //    //Don' Destroy, pass object to gamanager list as well as previous bot position c:
        //    Destroy(gameObject,0.3f);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_isPickedUp)
        transform.DOMoveY(0, _hoverDuration);
        //print("hover card down");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_isPickedUp)
        transform.DOMoveY(_hoverHeight, _hoverDuration);
        //print($"{this.gameObject.name} hover card up");
    }

}

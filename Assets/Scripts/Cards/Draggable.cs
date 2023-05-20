using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] [Range(1,4)]private int _moveCount;
    [SerializeField] private TextMeshProUGUI _cardValueText;
    [SerializeField] private LayerMask _layerMask;

    private Transform _originalHandParent = null;
    private GameObject _bot;
    private DirectionalInputBot _directionalInputBot;
    private GameManager _gameManager;
    private Vector3 _scaleAdditionVector;
    private float _hoverHeight;
    private float _originalHeight;
    private float _hoverDuration;
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

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //keep track of mouse position vs anchor point to have card movement relative to the grabbing point
        //AKA click on a corner and move it, without it jumping back to it
        //print("begin");
        transform.SetParent(transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    { 
        transform.position = eventData.position;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo, 40f, _layerMask))
        {
            
            if (hitInfo.collider.GetComponent<Bot>())
            {
                _bot= hitInfo.collider.gameObject;
                //transform.localScale-= _scaleAdditionVector;
            }
            else
            {
                //transform.localScale+= _scaleAdditionVector;
                _bot= null;
            }
        }
    }
    public void PopDown()
    {
        this.transform.position = new Vector3(_originalHandParent.position.x, _originalHandParent.position.y, _originalHandParent.position.z); 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //print("end");
        transform.SetParent(_originalHandParent,false);
        PopDown();
        if (_bot != null)
        {
            //TODO
            _bot.GetComponent<Bot>().SetDistance(_moveCount);
            GameManager.Instance.AssignPlayer(_bot);
            //Don' Destroy, pass object to gamanager list as well as previous bot position c:
            Destroy(gameObject,0.3f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOMoveY(0, _hoverDuration);
        //print("hover card down");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOMoveY(_hoverHeight, _hoverDuration);
        //print($"{this.gameObject.name} hover card up");
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] [Range(1,4)]private int _moveCount;
    [SerializeField] private TextMeshProUGUI _cardValueText;

    private Transform _originalHandParent = null;
    private GameObject _bot;


    private void Awake()
    {
        _cardValueText.text = _moveCount.ToString();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //keep track of mouse position vs anchor point to have card movement relative to the grabbing point
        //AKA click on a corner and move it, without it jumping back to it
        print("begin");
        _originalHandParent = transform.parent;
        transform.SetParent(transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("dragging");
        transform.position = eventData.position;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.GetComponent<Bot>())
            {
                _bot= hitInfo.collider.gameObject;
                _bot.GetComponent<Bot>().SetDistance(_moveCount);
                GameManager.Instance.AssignPlayer(_bot);

            }
            else
            {
                _bot= null;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        print("end");
        transform.SetParent(_originalHandParent);
        if (_bot != null)
        {
            Destroy(gameObject);
            //_bot.GetComponent<Bot>().Move(Vector3.forward);
        }
    }
}

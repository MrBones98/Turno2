using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using System;
using Assets.Scripts.Interactables;
using Editor;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private string[] _layersToCheck = { "Platform", "Pushable", "Wall", "Player" };
    int _collidableLayers;

    private bool _isPushable = true;
    public bool IsPushable { get { return _isPushable; } set { } }

    private Vector3 _direction;
    private bool _willBePlatform = false;
    private bool _platformCached = false;
    private bool _spawned = false;
    private WallTile _wallTile = null;
    private PushableBox _pushableBox = null; 
    private Bot _pushableBot = null;
    private float _originalHeight;
    private float _stepSpeed;
    private GameObject _mesh;

    public delegate void OnBoxPushed();
    public static event OnBoxPushed onBoxPushed;
    private void Awake()
    {
        _collidableLayers = LayerMask.GetMask(_layersToCheck);
        _stepSpeed = Tweener.Instance.PushableBoxStepSpeed;
        _originalHeight = transform.position.y;
        _mesh = transform.gameObject.GetComponentInChildren<MeshFilter>().gameObject;
    }
    public async Task SolveTurnAsync(Vector3 direction)
    {
        if (!_spawned)
        {
            _direction = direction;
            var solveCollisionsTask = SolveCollisionAsync(_direction);
            await solveCollisionsTask;
            var solveMovementTask = SolveMovementAsync(_wallTile, _platformCached, _pushableBox, _direction, _pushableBot);
            await solveMovementTask;

            _wallTile = null;
            _pushableBox = null;
            _pushableBot = null;
            _platformCached = false;
        }

    }
    async Task SolveCollisionAsync(Vector3 direction)
    {
        //Dictionaries
        await Task.Yield();
        FindInDictionaries(direction);

        //Old Raycast System
        //Vector3 raySpawnPos = new Vector3(transform.position.x, -1, transform.position.z);
        //RaycastHit[] hits = Physics.SphereCastAll(raySpawnPos + direction, 0.44f, transform.position + direction, _collidableLayers);
        //Collider[] hits = Physics.OverlapSphere(transform.position+ direction,0.44f, _collidableLayers);
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    if (_platformCached == false && hits[i].GetComponent<Collider>().gameObject.layer == 7)
        //    {
        //        _platformCached = true;
        //    }
        //    else if (!_platformCached)
        //    {
        //        _platformCached = false;
        //    }
        //    if (hits[i].gameObject.GetComponent<WallTile>())
        //    {
        //        _wallTile = hits[i].gameObject.GetComponent<WallTile>();
        //    }
        //    else if (_wallTile == null)
        //    {
        //        _wallTile = null;
        //    }
        //    if (_pushableBox == null || hits[i].GetComponent<Collider>().gameObject.GetComponent<PushableBox>())
        //    {
        //        _pushableBox = hits[i].GetComponent<Collider>().gameObject.GetComponent<PushableBox>();
        //    }
        //    if (hits[i].GetComponent<Bot>())
        //    {
        //        _pushableBot = hits[i].GetComponent<Bot>();
        //        //print($"Bot in front pushable: {_pushableBot.IsPushableBot}");
        //    }
        //    print(hits[i].GetComponent<Collider>().gameObject.name);
        //}
        //if (hits.Length == 0)
        //{
        //    _platformCached = false;
        //}
        //print($"There are {hits.Length} colliders on this step");
    }

    private void FindInDictionaries(Vector3 direction)
    {
        Tile tile = GameManager.Instance.FindTile(new Vector3(transform.position.x,0,transform.position.z)+direction);
        InteractableObject interactable = GameManager.Instance.FindInteractable(new Vector3(transform.position.x,0,transform.position.z)+direction);

        if (tile == null)
        {
            //print("Tile is null");
            _platformCached = false;
            _wallTile = null;
        }
        else
        {
            TileType type = tile.type;
            switch (type)
            {
                case TileType.Platform:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Button:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Wall:
                    _wallTile = tile.GetComponent<WallTile>();
                    break;
                case TileType.SpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.LatchSwitch:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Gate:
                    break;
                case TileType.MomentarySwitch:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.Moving:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.BoxSpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                case TileType.PushableBotSpawnTile:
                    _platformCached = true;
                    _wallTile = null;
                    break;
                default:
                    break;
            }

        }
        if (interactable == null)
        {
            _pushableBot = null;
            _pushableBox = null;
        }
        else
        {
            TypeOfInteractableObject type = interactable.Type;
            switch (type)
            {
                case TypeOfInteractableObject.PushableBot:
                    _pushableBot = interactable.GetComponent<Bot>();
                    _pushableBox = null;
                    break;
                case TypeOfInteractableObject.PushableBox:
                    _pushableBox = interactable.GetComponent<PushableBox>();
                    _pushableBot = null;
                    break;
                case TypeOfInteractableObject.Bot:
                    _pushableBot = null;
                    _pushableBox = null;
                    break;
            }
        }
    }

    async Task SolveMovementAsync(WallTile wallTile, bool platformCached, PushableBox pushableBox, Vector3 direction, Bot bot)
    {
        //print($"In the {direction} direction there are: WallTile = {wallTile}, Box = {pushableBox}, Platform in front = {platformCached}");
        await Task.Yield();
        if (wallTile == null || (wallTile != null && !wallTile.HasColision))
        {
            if (platformCached)
            {
                _willBePlatform = false;
            }
            else if(_wallTile != null && !wallTile.HasColision)
            {
                _willBePlatform = false;
            }
            else
            {
                _willBePlatform = true;
            }
            
            if (pushableBox != null)
            {
                //Move Bot with direction
                await pushableBox.SolveTurnAsync(direction);
                if (!pushableBox.IsPushable)
                {
                    _isPushable = false;
                }
                else
                {

                    //transform.position += direction;
                    _isPushable = true;
                    Move(direction);
                    //_willBePlatform = false;
                }
                _willBePlatform = false;
            }
            else if (bot!= null)
            {
                if (bot.IsPushableBot)
                {
                    await bot.SolvePushAsync(direction);
                    if (!bot.CanBePushed)
                    {
                        _isPushable = false;
                    }
                    else
                    {
                        _isPushable = true;
                        Move(direction);
                    }
                }
                else
                {
                    _isPushable = false;
                    return;
                }
                    _willBePlatform = false;
            }
            else
            {
                Move(direction);
            }
        }
        else
        {
            _isPushable = false;
            print("wall in front");
        }
        if (_willBePlatform == true)
        {
            await PlatformSetupAsync();
            await SpawnPlatform();
        }
    }

    private void Move(Vector3 direction)
    {
        GameManager.Interactables.Remove(new Vector3(transform.position.x, 0, transform.position.z));
        Vector3 newKey = new Vector3 (transform.position.x,0,transform.position.z) + direction;
        transform.DOMove(newKey, _stepSpeed);
        GameManager.Instance.AddInteractableToDictionary(newKey, gameObject.GetComponent<InteractableObject>());
        onBoxPushed?.Invoke();
    }

    private async Task PlatformSetupAsync()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        _spawned = true;
        _mesh.transform.DOScale(1, 0.2f);
        await Task.Delay(200);
        GameManager.SpawnedObjects.Remove(gameObject);
    }

    private async Task SpawnPlatform()
    {
        await Task.Yield();
        GameObject tile = Instantiate(_platform, new Vector3(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z)), Quaternion.identity);
        //GameObject tile = Instantiate(_platform, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        //print(tile.transform.position);
        GameManager.Interactables.Remove(transform.position);
        GameManager.TileGameObjects.Add(tile);
        GameManager.Instance.AddToTileToDictionary(new Vector3(MathF.Round( tile.transform.position.x),0, (int)tile.transform.position.z), tile.GetComponent<Tile>());
        print(transform.position);
        await Task.Yield();
        Destroy(gameObject, 0.4f);
    }

    private void Update()
    {
        if(transform.position.y <0 && !_willBePlatform)
        {
            //transform.position = new Vector3(transform.position.x,_originalHeight, transform.position.z);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + _direction, 0.44f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private string[] _layersToCheck = { "Platform", "Pushable", "Wall" };
    int _collidableLayers;

    private bool _isPushable = true;
    public bool IsPushable { get { return _isPushable; } set { } }


    private Vector3 _debugDirection;
    private Vector3 _direction;
    private bool _willBePlatform = false;
    private WallTile _wallTile = null;
    private PushableBox _pushableBox = null;
    private bool _platformCached = false;
   
    private void Awake()
    {
        _collidableLayers = LayerMask.GetMask(_layersToCheck);
        
    }
    public async Task SolveTurnAsync(Vector3 direction)
    {
        _direction = direction;
        var solveCollisionsTask = SolveCollisionAsync(_direction);
        await solveCollisionsTask;
        var solveMovementTask = SolveMovementAsync(_wallTile, _platformCached, _pushableBox, _direction);
        await solveMovementTask;

        _wallTile = null;
        _pushableBox = null;
        _platformCached = false;

    }
    async Task SolveCollisionAsync(Vector3 direction)
    {
        Vector3 raySpawnPos = new Vector3(transform.position.x, -1, transform.position.z);
         await Task.Yield();
        //RaycastHit[] hits = Physics.SphereCastAll(raySpawnPos + direction, 0.44f, transform.position + direction, _collidableLayers);
        Collider[] hits = Physics.OverlapSphere(transform.position+ direction,0.44f, _collidableLayers);
        for (int i = 0; i < hits.Length; i++)
        {
            if (_platformCached == false && hits[i].GetComponent<Collider>().gameObject.layer == 7)
            {
                _platformCached = true;
            }
            if (hits[i].GetComponent<Collider>().gameObject.GetComponent<WallTile>())
            {
                _wallTile = hits[i].GetComponent<Collider>().gameObject.GetComponent<WallTile>();
            }
            else if (_wallTile == null)
            {
                _wallTile = null;
            }
            if (_pushableBox == null || hits[i].GetComponent<Collider>().gameObject.GetComponent<PushableBox>())
            {
                _pushableBox = hits[i].GetComponent<Collider>().gameObject.GetComponent<PushableBox>();
            }
            print(hits[i].GetComponent<Collider>().gameObject.name);
        }
        
        print($"There are {hits.Length} colliders on this step");
    }
    async Task SolveMovementAsync(WallTile wallTile, bool platformCached, PushableBox pushableBox, Vector3 direction)
    {
        print($"In the {direction} direction there are: WallTile = {wallTile}, Box = {pushableBox}, Platform in front = {platformCached}");
        await Task.Yield();
        if (wallTile == null || (wallTile != null && !wallTile.HasColision))
        {
            if (platformCached)
            {
                _willBePlatform = false;
            }
            else
            {
                _willBePlatform = true;
            }
            if (_wallTile != null && !wallTile.HasColision)
            {
                _willBePlatform = false;
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
                    
                    transform.position += direction;

                }
            }
            else
            {
                //Move Bot with direction
                transform.position += direction;

            }
            if (_willBePlatform == true)
            {
                SpawnPlatform();  

                
            }
        }
        else
        {
            _isPushable = false;
            print("wall in front");
        }
    }
    private void SpawnPlatform()
    {
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        Instantiate(_platform, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        Destroy(gameObject, 1f);
    }

    public void TransformIntoPlatform()
    {
        _willBePlatform = true;
        print($"Box will be platform: {_willBePlatform}");
    }
    private IEnumerator SphereCastDelay()
    {
        yield return new WaitForSeconds(0.5f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.DrawSphere(transform.position + new Vector3(_debugDirection.x, -0.3f, _debugDirection.z), 0.3f);
        //Gizmos.DrawRay(transform.position, _debugDirection);
        Gizmos.DrawSphere(transform.position + _direction, 0.44f);
    }

}

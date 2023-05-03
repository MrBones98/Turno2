using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private string[] _layersToCheck = { "Platform", "Pushable" };

    private bool _isPushable = true;
    public bool IsPushable { get { return _isPushable; } set { } }


    private Vector3 _debugDirection;
    private bool _willBePlatform = false;
   public void CheckMovementDirection(Vector3 direction)
   {
        RaycastHit groundHit;
        _debugDirection =direction;
        WallTile wallTile = null;
        PushableBox box = null;
        //int interactableLayers = LayerMask.GetMask(_layersToCheck);
        //StartCoroutine(nameof(SphereCastDelay));
        //if (Physics.SphereCast(transform.position + new Vector3(direction.x, -0.3f, direction.z), 0.4f, transform.position + new Vector3(direction.x, -0.3f, direction.z),out groundHit) )       
        if (Physics.Raycast(transform.position, _debugDirection,out groundHit,1))
        {
            var collision = groundHit.collider.gameObject;

            if (collision.gameObject.GetComponent<WallTile>())
            {
                wallTile = collision.gameObject.GetComponent<WallTile>();
                _isPushable = false;
            }
            else
            {
                wallTile = null;
                _isPushable = true;
            }
            if (collision.gameObject.GetComponent<PushableBox>())
            {
                box = collision.gameObject.GetComponent<PushableBox>();
                box.CheckMovementDirection(direction);
                _isPushable = true;
            }
            else
            {
                box=null;
                _isPushable = true;
            }

            //TODO, array of Raycasthits
            //print(groundHit.collider.name);
            //this do separate wall check v
            if (!collision.GetComponentInParent<Tile>())
            {
                TransformIntoPlatform();                
            }
            //if(groundHit.collider.transform.gameObject.layer == _mask)
            //{
            //TransfromIntoPlatform();
            //}
        }
        else
        {
            TransformIntoPlatform();
        }
        StartCoroutine(nameof(SphereCastDelay));

        //TODO
        //EXTRACT THIS INTO SEPARATE FUNCTION (important for box.IsPushable check in Bot movement
        //print($"will it in this direction next turn be a platform:{_willBePlatform}");

        Move(direction);

   }
    private void Move(Vector3 direction)
    {
        if (!_willBePlatform & _isPushable)
        {
            transform.position += direction;
        }
        else if (!_isPushable)
        {
            print("no movement, wall in front");
        }
        else
        {
            print("Spawning Platform");
            transform.position += direction;
            SpawnPlatform();
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
        Gizmos.DrawRay(transform.position, _debugDirection);
    }

}

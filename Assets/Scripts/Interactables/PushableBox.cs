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
            }
            else
            {
                wallTile = null;
            }
            if (collision.gameObject.GetComponent<PushableBox>())
            {
                box = collision.gameObject.GetComponent<PushableBox>();
            }
            else
            {
                box=null;
            }

            //yeo, array
            //print(groundHit.collider.name);
            if (!collision.GetComponentInParent<Tile>())
            {
                TransfromIntoPlatform();                
            }
            //if(groundHit.collider.transform.gameObject.layer == _mask)
            //{
            //TransfromIntoPlatform();
            //}
        }
        StartCoroutine(nameof(SphereCastDelay));
        if (!_willBePlatform)
        {
            transform.position += direction;
        }
        else
        {
            print("Spawning Platform");
            transform.position += direction;
            SpawnPlatform();
        }
        //print($"will it in this direction next turn be a platform:{_willBePlatform}");
        

   }

    private void SpawnPlatform()
    {
        gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        Instantiate(_platform, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        Destroy(gameObject, 1f);
    }

    public void TransfromIntoPlatform()
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

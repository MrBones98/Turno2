using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private Vector3 _debugDirection;
    private bool _willBePlatform = false;
   public void CheckMovementDirection(Vector3 direction)
   {
        RaycastHit groundHit;
        _debugDirection = direction;
        //StartCoroutine(nameof(SphereCastDelay));
        //if (Physics.SphereCast(transform.position + new Vector3(direction.x, -0.3f, direction.z), 0.4f, transform.position + new Vector3(direction.x, -0.3f, direction.z),out groundHit) )       
        if(!Physics.Raycast(transform.position, _debugDirection,1,_mask))
        {
            print(direction);
            //print(groundHit.collider.name);
            //if(groundHit.collider.transform.gameObject.layer == _mask)
            //{
            TransfromIntoPlatform();
            //}
        }
        StartCoroutine(nameof(SphereCastDelay));
        if (!_willBePlatform)
        {
            transform.position += direction;
        }
        else
        {
            print('f');
            transform.position += direction;
            SpawnPlatform();
        }
        //print($"will it in this direction next turn be a platform:{_willBePlatform}");
        

   }

    private void SpawnPlatform()
    {
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
        yield return new WaitForSeconds(4);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.DrawSphere(transform.position + new Vector3(_debugDirection.x, -0.3f, _debugDirection.z), 0.3f);
        Gizmos.DrawRay(transform.position, _debugDirection);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private Vector3 _debugDirection;
    private bool _willBePlatform = false;
   public void Move(Vector3 direction)
   {
        RaycastHit groundHit;
        _debugDirection = direction;
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
        StartCoroutine(nameof(SphereCastDelay));
        if (!Physics.SphereCast(transform.position + new Vector3(direction.x, -0.3f, direction.z), 0.3f, transform.position + new Vector3(direction.x, -0.3f, direction.z),out groundHit, _mask) )       
        {
            TransfromIntoPlatform();
        }
        print($"will it in this direction next turn be a platform:{_willBePlatform}");
        

   }

    private void SpawnPlatform()
    {
        Instantiate(_platform, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        Destroy(gameObject, 1f);
    }

    public void TransfromIntoPlatform()
   {
        _willBePlatform = true;
   }
    private IEnumerator SphereCastDelay()
    {
        yield return new WaitForEndOfFrame();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position + new Vector3(_debugDirection.x, -0.3f, _debugDirection.z), 0.3f);
    }

}

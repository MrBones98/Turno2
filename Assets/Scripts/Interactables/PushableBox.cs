using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
   [SerializeField] private GameObject _platform;
   [SerializeField] private LayerMask _mask;

    private bool _isPlatform = false;
   public void Move(Vector3 direction)
   {
        RaycastHit groundHit;

        if (!_isPlatform)
        {
            transform.position += direction;
        }
        if (!Physics.SphereCast(transform.position + new Vector3(direction.x, -0.3f, direction.z), 0.3f, transform.position + new Vector3(direction.x, -0.3f, direction.z),out groundHit, _mask) )
        {
            TransfromIntoPlatform();
        }
        

   }
   public void TransfromIntoPlatform()
   {
        _isPlatform = true;
        //spawn proper GameObject later, for now regular platform
        Instantiate(_platform,new Vector3(transform.position.x,0,transform.position.z),Quaternion.identity);
        Destroy(gameObject);
   }

}

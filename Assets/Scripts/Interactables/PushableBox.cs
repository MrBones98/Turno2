using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
   public void Move(Vector3 direction)
   {
        print($"moving box with {direction}");
         transform.position += direction;
   }
   public void TransfromIntoPlatform()
   {

   }

}

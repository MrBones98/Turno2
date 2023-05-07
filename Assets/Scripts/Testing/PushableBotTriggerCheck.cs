using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBotTriggerCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print($"There's a {other.gameObject.name} on top of me!");
        
    }
    private void OnTriggerStay(Collider other)
    {
        print($"There's a {other.gameObject.name} on top of me!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public void Move(Vector3 direction)
    {
        transform.LookAt(direction);
        transform.Translate(direction);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Sirenix.OdinInspector;
using static UnityEngine.Mathf;

public class TweenedMoveTest : MonoBehaviour
{
    [Range(0.01f, 5f)]
    public float duration;
    public int distance = 1;

    [ButtonGroup]
    public void PosX()
    {
        MoveUtils.MoveWithTween(this.gameObject, DirectionIs.PosX, distance, duration);
    }
    [ButtonGroup]
    public void NegX()
    {
        MoveUtils.MoveWithTween(this.gameObject, DirectionIs.NegX, distance, duration);
    }
    [ButtonGroup]
    public void PosZ()
    {
        MoveUtils.MoveWithTween(this.gameObject, DirectionIs.PosZ, distance, duration);
    }
    [ButtonGroup]
    public void NegZ()
    {
        MoveUtils.MoveWithTween(this.gameObject, DirectionIs.NegZ, distance, duration);
    }
}

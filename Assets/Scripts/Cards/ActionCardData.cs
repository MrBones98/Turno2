using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDataSO", menuName = "Card")]
public class ActionCardData : ScriptableObject
{
    public int distance;
    public Sprite image;
    public bool isJump;
}

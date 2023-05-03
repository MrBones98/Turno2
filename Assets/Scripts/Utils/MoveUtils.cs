using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Utils;

namespace Utils
{
    public static class MoveUtils
    {
        public static void MoveWithTween(GameObject target, DirectionIs direction, float duration)
        {
            target.transform.DOMove(target.transform.position + SetDirection(direction), duration, false);
        }

        public static void MoveWithTween(GameObject target, DirectionIs direction, int distance, float duration)
        {
            target.transform.DOMove(target.transform.position + SetDirection(direction, distance), duration, false);
        }

        public static Vector3 SetDirection(DirectionIs direction)
        {

            switch (direction)
            {
                case DirectionIs.PosX:
                    return new Vector3(1f, 0, 0);
                case DirectionIs.NegX:
                    return new Vector3(-1f, 0, 0);
                case DirectionIs.PosZ:
                    return new Vector3(0, 0, 1f);
                case DirectionIs.NegZ:
                    return new Vector3(0, 0, -1f);
                default:
                    break;
            }
            return Vector3.zero;
        }

        public static Vector3 SetDirection(DirectionIs direction, int distance)
        {

            switch (direction)
            {
                case DirectionIs.PosX:
                    return new Vector3(distance, 0, 0);
                case DirectionIs.NegX:
                    return new Vector3(-distance, 0, 0);
                case DirectionIs.PosZ:
                    return new Vector3(0, 0, distance);
                case DirectionIs.NegZ:
                    return new Vector3(0, 0, -distance);
                default:
                    break;
            }
            return Vector3.zero;
        }
    }
}

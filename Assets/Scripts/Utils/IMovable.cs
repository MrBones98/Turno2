using UnityEngine;

namespace Assets.Scripts.Utils
{
    public interface IMovable
    {
        public void CheckMovement(Vector3 direction);
        public void CheckJumping(Vector3 direction);
    }
}

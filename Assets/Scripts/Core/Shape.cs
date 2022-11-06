using UnityEngine;

namespace Core
{
    public class Shape : MonoBehaviour
    {
        [SerializeField] private bool _canRotate;
        public Vector3 _queueOffset;
        
        // ----

        private void Move(Vector3 direction)
        {
            transform.position += direction;
        }

        public void MoveLeft()
        {
            Move(Vector3.left);
        }

        public void MoveRight()
        {
            Move(Vector3.right);
        }

        public void MoveDown()
        {
            Move(Vector3.down);
        }

        public void MoveUp()
        {
            Move(Vector3.up);
        }
        
        private void Rotate(Vector3 rotation)
        {
            if (_canRotate) transform.Rotate(rotation);
        }

        private void RotateLeft()
        {
            Rotate(new Vector3(0, 0, -90));
        }

        private void RotateRight()
        {
            Rotate(new Vector3(0, 0, 90));
        }

        public void RotateClockwise(bool isClockwise)
        {
            if (isClockwise) RotateRight();
            else RotateLeft();
        }
    }
}
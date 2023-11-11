using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;

        [SerializeField] private float _speed = 1;
        [SerializeField] private float _speedRotation = 3;

        void Update()
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * _speedRotation, 0);

            var forward = transform.TransformDirection(Vector3.forward);
            var curSpeed = _speed * Input.GetAxis("Vertical");

            _characterController.SimpleMove(forward * curSpeed);
        }
    }
}
using Input;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private float _movementSpeed = 6f;
        [SerializeField] private float _gravity = -10f;
        [SerializeField] private float _jumpHeight = 3f;
        
        private void FixedUpdate()
        {
            Gravity();
            Movement();
        }

        private void Gravity()
        {
            var y = _playerRigidbody.linearVelocity.y - _gravity * Time.fixedDeltaTime;
            _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, y, _playerRigidbody.linearVelocity.z);
        }

        private void Movement()
        {
            var movement = InputSystem.InputControls.GameActionMap.Movement.ReadValue<Vector2>();
            var jump = InputSystem.InputControls.GameActionMap.Jump.IsPressed();
            var crouch = InputSystem.InputControls.GameActionMap.Crouch.IsPressed();
            var sprint = InputSystem.InputControls.GameActionMap.Sprint.IsPressed();

            if (jump)
            {
                var jumpVector = new Vector3(0, _jumpHeight, 0);
                _playerRigidbody.AddForce(jumpVector, ForceMode.VelocityChange);
            }
                
            var speedX = movement.x * _movementSpeed;
            var speedY = movement.y * _movementSpeed;
            
            _playerRigidbody.linearVelocity = new Vector3(speedX, _playerRigidbody.linearVelocity.y, speedY);
        }
    }
}
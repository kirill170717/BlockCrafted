using Input;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody _playerRigidbody;
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector2 _mouseSensitivity = new(1f, 1f);
        [SerializeField] private float _movementSpeed = 6f;
        [SerializeField] private float _jumpForce = 10.15f;
        [SerializeField] private float _crouchSpeed = 1.31f;
        [SerializeField] private float _sprintSpeed = 5.612f;
        [SerializeField] private float _flySpeed = 10.92f;
        [SerializeField] private float _flySprintSpeed = 21.6f;
        [SerializeField] private float _gravity = -10f;

        private Quaternion _cameraOriginalRotation;
        private Quaternion _transformOriginalRotation;
        private float _rotationX;
        private float _rotationY;

        private float _horizontal;
        private float _vertical;
        private float _verticalMomentum;
        private Vector3 _velocity;
        private Vector3 _direction;

        private bool _isGrounded;
        private bool _isJumping;
        private bool _isFlying;
        private bool _isSprinting;
        private bool _isCrouching;

        private void Start()
        {
            _cameraOriginalRotation = _camera.transform.localRotation;
            _transformOriginalRotation = transform.rotation;
        }

        private void Update()
        {
            var cameraMovement = InputSystem.InputControls.GameActionMap.CameraMovement.ReadValue<Vector2>();

            HandleMouseLook(cameraMovement);
        }

        private void FixedUpdate()
        {
            _isJumping = InputSystem.InputControls.GameActionMap.Jump.IsPressed();
            _isCrouching = InputSystem.InputControls.GameActionMap.Crouch.IsPressed();
            _isSprinting = InputSystem.InputControls.GameActionMap.Sprint.IsPressed();

            if (Mathf.Abs(_velocity.x) <= 0 && Mathf.Abs(_velocity.z) <= 0)
            {
                _isSprinting = false;
            }

            Gravity();
            Movement();

            _playerRigidbody.linearVelocity = new Vector3(_velocity.x, _playerRigidbody.linearVelocity.y, _velocity.z);
        }

        private void HandleMouseLook(Vector2 cameraMovement)
        {
            _rotationX += cameraMovement.x * _mouseSensitivity.x;
            _rotationY += cameraMovement.y * _mouseSensitivity.y;
            _rotationY = ClampAngle(_rotationY, Constants.MouseLook.MINIMUM_ROTATION_Y,
                Constants.MouseLook.MAXIMUM_ROTATION_Y);

            var xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
            var yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);

            _camera.transform.localRotation = _cameraOriginalRotation * yQuaternion;
            transform.rotation = _transformOriginalRotation * xQuaternion;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;

            if (angle > 360f) angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }

        private void Gravity()
        {
            _velocity = new Vector3
            (
                _velocity.x,
                _velocity.y - _gravity * Time.fixedDeltaTime,
                _velocity.z
            );
        }

        private void Movement()
        {
            var movement = InputSystem.InputControls.GameActionMap.Movement.ReadValue<Vector2>();

            _direction = (transform.forward * movement.y + transform.right * movement.x) * (_isFlying ? _flySpeed : _movementSpeed);
            _velocity = new Vector3(_direction.x, _velocity.y, _direction.z) ;

            if (_isJumping) Jump();

            if (_isCrouching) Crouch();
            else if (_isSprinting) Sprint();
        }

        private void Jump()
        {
            _velocity += Vector3.up * _jumpForce;
        }

        private void Sprint() => _velocity = _direction * (_isFlying ? _flySprintSpeed : _sprintSpeed);

        private void Crouch() => _velocity = _direction * _crouchSpeed;
    }
}
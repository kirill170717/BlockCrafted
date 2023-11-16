using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _gravity = -10f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 20f;
        [SerializeField] private float _jumpHeight = 3f;
        
        private Vector2 _moveVector;
        private float _maxMovementSpeed;
        private bool _isGrounded;
        private bool _isJumping;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            _maxMovementSpeed = _speed;
            HandleInput();
        }
        
        private void FixedUpdate()
        {
            HandleWalking();
            
            // var y = _rigidbody.velocity.y + _gravity * Time.fixedDeltaTime;
            // _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, y, _rigidbody.velocity.z);
        }

        private void HandleInput()  
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJumping = true;
            }

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            _moveVector = new Vector2(horizontal, vertical).normalized;

            var isAccelerating = _moveVector.sqrMagnitude > 0;
            var targetSpeed = isAccelerating ? _maxMovementSpeed : 0;
            var accelerationSpeed = Time.deltaTime * (targetSpeed > _speed ? _acceleration : _deceleration);

            _speed = Mathf.MoveTowards(_speed, targetSpeed, accelerationSpeed);
            _moveVector *= _speed;
        }

        private void HandleWalking()
        {
            var nPos = transform.forward * _moveVector.y + transform.right * _moveVector.x;
            _rigidbody.velocity = new Vector3(nPos.x, _rigidbody.velocity.y, nPos.z);

            if (_isJumping)
            {
                _rigidbody.AddForce(new Vector3(0, Mathf.Sqrt(-2 * _gravity * _jumpHeight), 0), ForceMode.VelocityChange);
                _isJumping = false;
            }
        }
    }
}
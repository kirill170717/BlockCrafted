using UnityEngine;
using static Consts.MouseLook;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] private Vector2 _mouseSensitivity = new(10f, 10f);

        private Transform _cameraTransform;

        private Quaternion _cameraOriginalRotation;
        private Quaternion _transformOriginalRotation;
        
        private float _rotationY;
        private float _rotationX;

        private void Start()
        {
            _cameraTransform = PlayerController.Instance.MainCamera.transform;
            _cameraOriginalRotation = _cameraTransform.localRotation;
            _transformOriginalRotation = transform.rotation;
        }

        private void Update()
        {
            HandleMouseLook();
        }

        private void HandleMouseLook()
        {
            _rotationX += Input.GetAxis("Mouse X") * _mouseSensitivity.x;
            _rotationY += Input.GetAxis("Mouse Y") * _mouseSensitivity.y;
            _rotationY = ClampAngle(_rotationY, MINIMUM_ROTATION_Y, MAXIMUM_ROTATION_Y);

            var xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);
            var yQuaternion = Quaternion.AngleAxis(_rotationY, -Vector3.right);

            _cameraTransform.localRotation = _cameraOriginalRotation * yQuaternion;
            transform.rotation = _transformOriginalRotation * xQuaternion;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }

            if (angle > 360F)
            {
                angle -= 360F;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}
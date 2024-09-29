using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Input
{
    public class InputSystem : MonoBehaviour
    {
        public static InputControls InputControls { get; private set; }
        
        private void OnEnable() => InputControls.Enable();

        private void OnDisable() => InputControls.Disable();
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            InputControls = new InputControls();
            EnhancedTouchSupport.Enable();
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WannaBe
{
    public class DefaultCameraInput : ICameraInput, IDisposable
    {
        private InputAction _rightStick;

        public DefaultCameraInput()
        {
            // якщо використовуЇте PS4-контролер, можна зам≥нити шл€х на "<DualShockGamepad>/rightStick"
            _rightStick = new InputAction("RightStick", InputActionType.Value, "<Gamepad>/rightStick");
            _rightStick.Enable();
        }

        public Vector2 GetRightStickValue()
        {
            return _rightStick.ReadValue<Vector2>();
        }

        public void Dispose()
        {
            _rightStick.Dispose();
        }
    }
}
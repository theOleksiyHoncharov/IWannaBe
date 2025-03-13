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
            // ���� ������������� PS4-���������, ����� ������� ���� �� "<DualShockGamepad>/rightStick"
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
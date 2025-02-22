using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DefaultSlimeInput : ISlimeInput, IDisposable
{
    private InputAction _move;

    public DefaultSlimeInput()
    {
        // Дія для руху: лівий стік із додатковими клавіатурними биндами
        _move = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
        _move.AddCompositeBinding("2DVector")
             .With("Up", "<Keyboard>/w")
             .With("Down", "<Keyboard>/s")
             .With("Left", "<Keyboard>/a")
             .With("Right", "<Keyboard>/d");
        _move.Enable();

    }

    public Vector2 GetMoveValue()
    {
        return _move.ReadValue<Vector2>();
    }

    public void Dispose()
    {
        _move.Dispose();
    }
}

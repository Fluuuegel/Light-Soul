using UnityEngine;
using UnityEngine.InputSystem;
using Gadgets.Singleton;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager>
{
    private CharacterInput _inputAction;
    public Vector2 Move => _inputAction.Player.Move.ReadValue<Vector2>();
    public Vector2 Look => _inputAction.Player.Look.ReadValue<Vector2>();
    public float BufferTime = 0.2f;
    public InputBuffer LAttackBuffer;
    public InputBuffer HAttackBuffer;
    private List<InputBuffer> _inputBufferList = new List<InputBuffer>();
    public bool Denfense => _inputAction.Player.Denfense.phase == InputActionPhase.Performed;
    public bool Sprint => _inputAction.Player.Sprint.phase == InputActionPhase.Performed;
    public bool Dodge => _inputAction.Player.Dodge.triggered;
    public bool Crouch => _inputAction.Player.Crouch.triggered;
    public bool Interact => _inputAction.Player.Interact.triggered;
    public bool Jump => _inputAction.Player.Jump.triggered;
    public bool LAttack => _inputAction.Player.LAttack.triggered;
    public bool HAttack => _inputAction.Player.HAttack.triggered;
    public bool Equip1 => _inputAction.Player.Equip1.triggered;
    public bool Lock => _inputAction.Player.Lock.triggered;

    private void Awake()
    {
        _inputAction = new CharacterInput();
        LAttackBuffer = new InputBuffer(_inputAction.Player.LAttack);
        HAttackBuffer = new InputBuffer(_inputAction.Player.HAttack);
        _inputBufferList.Add(LAttackBuffer);
        _inputBufferList.Add(HAttackBuffer);
    }

    private void Update()
    {
        foreach (var inputBuffer in _inputBufferList)
        {
            inputBuffer.Update();
        }
    }

    private void OnEnable()
    {
        _inputAction.Enable();
    }

    private void OnDisable()
    {
        _inputAction.Disable();
    }

    public void ResetTriggers()
    {
        LAttackBuffer.ResetTrigger();
        HAttackBuffer.ResetTrigger();
    }
}

public class InputBuffer
{
    private InputAction _inputAction;
    private float timer = 0f;
    private bool _triggered = false;

    public InputBuffer(InputAction inputAction)
    {
        _inputAction = inputAction;
    }

    public void ResetTrigger()
    {
        _triggered = false;
        timer = 0f;
    }

    public void Update()
    {
        if (_inputAction.triggered)
        {
            _triggered = true;
        }
        if (_triggered)
        {
            if (timer < InputManager.Instance.BufferTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                _triggered = false;
            }
        }
    }

    public bool IsTriggered()
    {
        return _triggered;
    }

}

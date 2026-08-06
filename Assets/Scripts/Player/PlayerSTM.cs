using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerSTM : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Attack
    }
    [SerializeField] private PlayerStates _currentState = PlayerStates.Idle;

    private PlayerController _controller;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void OnEnterState()
    {
        //print("Enter State: " + _currentState.ToString());
        switch (_currentState)
        {
            case PlayerStates.Idle:
                _controller.SetGroundGravity();
                _controller.Stop();
                break;
            case PlayerStates.Walk:
                _controller.SetGroundGravity();
                break;
            case PlayerStates.Jump:
                _controller.SetGroundGravity();
                _controller.Jump();
                break;
            case PlayerStates.Fall:
                _controller.SetFallGravity();
                break;
        }
    }

    public void OnUpdateState()
    {
        switch (_currentState)
        {
            case PlayerStates.Idle:
                if (_controller.IsMoving())
                    SetState(PlayerStates.Walk);

                if (_controller.CanJump())
                    SetState(PlayerStates.Jump);

                if (_controller.IsFalling())
                    SetState(PlayerStates.Fall);

                if (_controller.CanAttack())
                    SetState(PlayerStates.Attack);
                break;

            case PlayerStates.Walk:
                if (_controller.IsStanding())
                    SetState(PlayerStates.Idle);

                if (_controller.CanJump())
                    SetState(PlayerStates.Jump);

                if (_controller.IsFalling())
                    SetState(PlayerStates.Fall);

                if (_controller.CanAttack())
                    SetState(PlayerStates.Attack);
                break;

            case PlayerStates.Jump:
                if (_controller.IsJumpReleased())
                    _controller.VariableJump();

                if (_controller.IsFalling())
                    SetState(PlayerStates.Fall);
                break;

            case PlayerStates.Fall:
                if (_controller.CanJump())
                {
                    SetState(PlayerStates.Jump);
                }
                else if (_controller.IsGrounded())
                {
                    if (_controller.IsMoving())
                        SetState(PlayerStates.Walk);
                    else
                        SetState(PlayerStates.Idle);
                }
                break;
        }
    }

    public void OnFixedUpdateState()
    {
        switch (_currentState)
        {
            case PlayerStates.Walk:
            case PlayerStates.Jump:
            case PlayerStates.Fall:
                _controller.Move();
                break;
        }
    }

    public void SetState(PlayerStates newState)
    {
        if (_currentState != newState)
        {
            _currentState = newState;
            _controller.SetStateText(newState.ToString());

            OnEnterState();
        }
    }
}

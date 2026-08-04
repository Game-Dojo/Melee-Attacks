using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public enum PlayerStates
	{
		Idle,
		Walk,
		Jump,
		Fall,
		Attack
	}

	[Header("Properties")]
	[SerializeField] private float speed = 8.0f;
	[SerializeField] private float jumpForce = 2.0f;
	[SerializeField] private LayerMask groundMask;

	[Header("UI")]
	[SerializeField] private TMP_Text stateText;

	[Header("Input")]
	[SerializeField] private InputActionReference _moveAction;
	[SerializeField] private InputActionReference _jumpAction;
	[SerializeField] private InputActionReference _attackAction;
	[SerializeField] private InputActionReference _pauseAction;

	#region Private members
	private Rigidbody2D _rb;
	private SpriteRenderer _renderer;
	private Animator _animator;
	private PlayerStates _currentState = PlayerStates.Idle;

	private float _horizontalMovement;

	private bool _canJump = false;
	private bool _canAttack = false;
	private bool _isGrounded = false;
	private bool _isDead = false;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_renderer = GetComponentInChildren<SpriteRenderer>();
		_animator = GetComponent<Animator>();

		_moveAction.action.performed += OnMove;
		_moveAction.action.canceled += OnMovementStop;

		_jumpAction.action.performed += OnJump;
		_attackAction.action.performed += OnAttack;
		_pauseAction.action.performed += OnPause;
	}

	private void Update()
	{
		if (_isDead) return;

		CheckGround();

		switch (_currentState)
		{
			case PlayerStates.Idle:
				if (IsMoving())
					SetState(PlayerStates.Walk);

				if (IsOnGround())
					SetState(PlayerStates.Jump);

				if (IsFalling())
					SetState(PlayerStates.Fall);

				if (CanAttack())
					SetState(PlayerStates.Attack);
				break;

			case PlayerStates.Walk:
				if (IsStanding())
					SetState(PlayerStates.Idle);

				if (IsOnGround())
					SetState(PlayerStates.Jump);

				if (IsFalling())
					SetState(PlayerStates.Fall);

				if (CanAttack())
					SetState(PlayerStates.Attack);
				break;

			case PlayerStates.Jump:
				if (IsFalling())
					SetState(PlayerStates.Fall);

				if (IsMoving())
					SetState(PlayerStates.Walk);
				break;

			case PlayerStates.Fall:
				if (IsStanding())
					SetState(PlayerStates.Idle);

				if (IsMoving())
					SetState(PlayerStates.Walk);
				break;

			case PlayerStates.Attack:
				if (IsMoving())
					SetState(PlayerStates.Walk);

				if (IsStanding())
					SetState(PlayerStates.Idle);

				if (IsFalling())
					SetState(PlayerStates.Fall);
				break;
		}
	}

	private void FixedUpdate()
	{
		switch (_currentState)
		{
			case PlayerStates.Idle:
				_rb.linearVelocityX = 0;
				break;
			case PlayerStates.Walk:
				_rb.linearVelocityX = _horizontalMovement * speed;
				break;
			case PlayerStates.Jump:
				if (_canJump)
				{
					_rb.linearVelocityY = 0;
					_rb.linearVelocityY = jumpForce;
					_canJump = false;
				}
				break;
		}
	}

	private void OnDrawGizmos()
	{
		if (_isGrounded == true)
		{
			Gizmos.color = Color.yellow;
		}
		else
		{
			Gizmos.color = Color.purple;
		}

		//Gizmos.color = (_isGrounded) ? Color.yellow : Color.purple;
		Gizmos.DrawLine(transform.position - new Vector3(0, -0.1f, 0), (transform.position) + (Vector3.down * 0.2f));
	}

	private void OnDestroy()
	{
		_pauseAction.action.performed -= OnPause;
		_moveAction.action.performed -= OnMove;
		_moveAction.action.canceled -= OnMovementStop;
		_jumpAction.action.performed -= OnJump;
		_attackAction.action.performed -= OnAttack;
	}
	#endregion

	#region Input Management
	private void OnPause(InputAction.CallbackContext context)
	{
		print("Pausa el juego");
	}

	private void OnAttack(InputAction.CallbackContext context)
	{
		//if (_animator) _animator.SetTrigger("Attack");
		_canAttack = true;
	}

	private void OnJump(InputAction.CallbackContext context)
	{
		_canJump = true;
		//if (_animator) _animator.SetTrigger("Jump");
		/*if (_isGrounded == true)
		{
			_canJump = true;
			_isJumping = true;
			if (_animator) _animator.SetTrigger("Jump");

			SetState(PlayerStates.Jump);
		}*/
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_horizontalMovement = context.ReadValue<Vector2>().x;

		var flip = (_horizontalMovement > 0) ? 1 : -1;
		transform.localScale = new Vector3(flip, 1, 1);
	}

	private void OnMovementStop(InputAction.CallbackContext context)
	{
		_horizontalMovement = 0;
		//_isRunning = false;

		//SetState(PlayerStates.Idle);
	}
	#endregion

	#region Finite State Machine
	private void SetState(PlayerStates newState)
	{
		if (_currentState != newState)
		{
			_currentState = newState;
			stateText.text = newState.ToString();
		}
	}

	private bool IsStanding() => _horizontalMovement == 0 && _isGrounded;
	private bool IsMoving() => _horizontalMovement != 0;
	private bool IsOnGround() => _canJump && _isGrounded;
	private bool IsFalling() => !_isGrounded && _rb.linearVelocityY < 0;
	private bool CanAttack() => _canAttack;

	#endregion

	private void CheckGround()
	{
		var origin = transform.position - new Vector3(0, -0.1f, 0);
		_isGrounded = Physics2D.Raycast(origin, Vector3.down, 0.2f, groundMask);
	}
}

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

	private Rigidbody2D _rb;
	private SpriteRenderer _renderer;
	private Animator _animator;

	private PlayerStates _currentState = PlayerStates.Idle;

	#region Private members
	private float _horizontalMovement;

	private bool _canJump = false;
	private bool _isGrounded = false;
	private bool _isJumping = false;
	private bool _isFalling = false;
	private bool _isRunning = false;
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
		if (!_isDead)
		{
			CheckGround();

			// Resetting states
			if (!_isJumping && _isGrounded)
			{
				_isJumping = false;
				_isFalling = false;
				_rb.gravityScale = 1;

				SetState(PlayerStates.Idle);
			}

			// Falling
			if (_isJumping && _rb.linearVelocityY < 0)
			{
				_isJumping = false;
				_isFalling = true;
				_rb.gravityScale = 2.5f;

				SetState(PlayerStates.Fall);
			}
		}
	}

	private void FixedUpdate()
	{
		_rb.linearVelocityX = _horizontalMovement * speed;

		if (_canJump)
		{
			_rb.linearVelocityY = 0;
			_rb.linearVelocityY = jumpForce;
			_canJump = false;
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

	private void SetState(PlayerStates newState)
	{
		if (_currentState != newState)
		{
			_currentState = newState;
			stateText.text = newState.ToString();
		}
	}

	#region Input Management
	private void OnPause(InputAction.CallbackContext context)
	{
		print("Pausa el juego");
	}

	private void OnAttack(InputAction.CallbackContext context)
	{
		if (_animator) _animator.SetTrigger("Attack");
		SetState(PlayerStates.Attack);
	}

	private void OnJump(InputAction.CallbackContext context)
	{
		if (_isGrounded == true)
		{
			_canJump = true;
			_isJumping = true;
			if (_animator) _animator.SetTrigger("Jump");

			SetState(PlayerStates.Jump);
		}
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_horizontalMovement = context.ReadValue<Vector2>().x;
		var flip = (_horizontalMovement > 0) ? 1 : -1;
		transform.localScale = new Vector3(flip, 1, 1);
		_isRunning = true;

		SetState(PlayerStates.Walk);
	}

	private void OnMovementStop(InputAction.CallbackContext context)
	{
		_horizontalMovement = 0;
		_isRunning = false;

		SetState(PlayerStates.Idle);
	}
	#endregion

	private void CheckGround()
	{
		var origin = transform.position - new Vector3(0, -0.1f, 0);
		_isGrounded = Physics2D.Raycast(origin, Vector3.down, 0.2f, groundMask);
	}
}

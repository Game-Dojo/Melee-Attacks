using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] private float speed = 8.0f;
	[SerializeField] private float jumpForce = 2.0f;
	[SerializeField] private LayerMask groundMask;

	[Header("Input")]
	[SerializeField] private InputActionReference _moveAction;
	[SerializeField] private InputActionReference _jumpAction;
	[SerializeField] private InputActionReference _attackAction;

	private Rigidbody2D _rb;
	private SpriteRenderer _renderer;
	private Animator _animator;

	#region Private members
	private float _horizontalMovement;

	private bool _canJump = false;
	private bool _isGrounded = false;
	private bool _isJumping = false;
	private bool _isFalling = true;
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
	}

	private void Update()
	{
		if (!_isDead)
		{
			CheckGround();
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
		_moveAction.action.performed -= OnMove;
		_moveAction.action.canceled -= OnMovementStop;
		_jumpAction.action.performed -= OnJump;
		_attackAction.action.performed -= OnAttack;
	}
	#endregion

	private void OnAttack(InputAction.CallbackContext context)
	{
		if (_animator) _animator.SetTrigger("Attack");
	}

	private void OnJump(InputAction.CallbackContext context)
	{
		if (_isGrounded == false)
		{
			return;
		}

		_canJump = true;
		if (_animator) _animator.SetTrigger("Jump");
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_horizontalMovement = context.ReadValue<Vector2>().x;
		transform.localScale = new Vector3(_horizontalMovement, 1, 1);
		_isRunning = true;
	}

	private void OnMovementStop(InputAction.CallbackContext context)
	{
		_horizontalMovement = 0;
		_isRunning = false;
	}

	private void CheckGround()
	{
		var origin = transform.position - new Vector3(0, -0.1f, 0);
		_isGrounded = Physics2D.Raycast(origin, Vector3.down, 0.2f, groundMask);
	}
	public void OnLifeLost()
	{
		_isDead = true;
	}
}

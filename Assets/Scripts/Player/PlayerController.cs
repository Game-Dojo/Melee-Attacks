using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Properties")]
	[SerializeField] private float speed = 8.0f;
	[SerializeField] private float jumpForce = 2.0f;
	[SerializeField] private float raycastLength = 0.3f;
	[SerializeField] private Rigidbody2D.SlideMovement slideMovement = new Rigidbody2D.SlideMovement();
	[SerializeField] private Rigidbody2D.SlideResults slideResults;

	[SerializeField] private EnemyDialogs enemyDialogs;

	[Header("Game Feel")]
	[SerializeField] private float fallGravity = 2f;
	[SerializeField] private float coyoteTimeCount = 0.2f;
	[SerializeField] private float jumpBufferCount = 0.2f;
	[SerializeField] private float jumpReleasedForce = 0.5f;

	[Header("Collisions")]
	[SerializeField] private LayerMask groundMask;

	[Header("UI")]
	[SerializeField] private TMP_Text stateText;

	[Header("Input")]
	[SerializeField] private InputActionReference _moveAction;
	[SerializeField] private InputActionReference _jumpAction;
	[SerializeField] private InputActionReference _attackAction;
	[SerializeField] private InputActionReference _pauseAction;
	[SerializeField] private InputActionReference _interactAction;

	#region Private members
	private Rigidbody2D _rb;
	private Animator _animator;
	private PlayerSTM _stm;

	private float _horizontalMovement;
	private Vector2 _velocity = Vector2.zero;

	private bool _canAttack = false;
	private bool _isGrounded = false;
	private bool _isDead = false;
	private bool _isJumpReleased = false;

	private float _coyoteCounter = 0;
	private float _jumpBufferCounter = 0;

	private bool _isInsideArea = false;

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponent<Animator>();

		_stm = GetComponent<PlayerSTM>();

		_moveAction.action.performed += OnMove;
		_moveAction.action.canceled += OnMovementStop;

		_jumpAction.action.performed += OnJump;
		_jumpAction.action.canceled += OnReleaseJump;

		_attackAction.action.performed += OnAttack;
		_pauseAction.action.performed += OnPause;
		_interactAction.action.performed += OnInteract;
	}

	private void Update()
	{
		if (_isDead) return;

		CheckGround();

		if (_isGrounded)
			_coyoteCounter = coyoteTimeCount;
		else
			_coyoteCounter -= Time.deltaTime;

		_jumpBufferCounter -= Time.deltaTime;

		_stm.OnUpdateState();
	}

	private void FixedUpdate()
	{
		_stm.OnFixedUpdateState();
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
		//Gizmos.DrawLine(transform.position - new Vector3(0, -0.1f, 0), (transform.position) + (Vector3.down * raycastLength));
		Gizmos.DrawWireSphere(transform.position - new Vector3(0, raycastLength, 0), raycastLength);
	}

	private void OnDestroy()
	{
		_pauseAction.action.performed -= OnPause;
		_moveAction.action.performed -= OnMove;
		_moveAction.action.canceled -= OnMovementStop;
		_jumpAction.action.performed -= OnJump;
		_jumpAction.action.canceled -= OnReleaseJump;
		_attackAction.action.performed -= OnAttack;
		_interactAction.action.performed -= OnInteract;
	}
	#endregion

	#region Input Management

	private void OnPause(InputAction.CallbackContext context)
	{
		print("Pausa el juego");
	}

	private void OnAttack(InputAction.CallbackContext context)
	{
		if (_animator) _animator.SetTrigger("Attack");
	}

	private void OnJump(InputAction.CallbackContext context)
	{
		_jumpBufferCounter = jumpBufferCount;
		_isJumpReleased = false;
	}

	private void OnReleaseJump(InputAction.CallbackContext context)
	{
		_isJumpReleased = true;
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_horizontalMovement = context.ReadValue<Vector2>().x;

		var flip = (_horizontalMovement > 0) ? 1 : -1;
		transform.localScale = new Vector3(flip, 1, 1);
		stateText.rectTransform.localScale = new Vector2(flip, 1);

		_velocity.x = _horizontalMovement * speed;
	}

	private void OnMovementStop(InputAction.CallbackContext context)
	{
		_horizontalMovement = 0;
	}

	private void OnInteract(InputAction.CallbackContext context)
	{
		if (_isInsideArea)
		{
			print("INTERACT");
			enemyDialogs.NextDialog();
		}
	}
	#endregion

	#region Conditions
	public bool IsStanding() => _horizontalMovement == 0 && _isGrounded;
	public bool IsMoving() => _horizontalMovement != 0;
	public bool IsGrounded() => _isGrounded;
	public bool IsFalling() => !_isGrounded && _rb.linearVelocityY < 0;
	public bool CanJump() => _jumpBufferCounter > 0f && _coyoteCounter > 0f;
	public bool CanAttack() => _canAttack;
	public bool IsDead() => _isDead;

	public bool IsJumpReleased() => _isJumpReleased;
	#endregion

	#region Actions
	public void Stop()
	{
		_velocity.x = 0;
		_rb.linearVelocityX = _velocity.x;
	}

	public void Jump()
	{
		_velocity.y = jumpForce;
		_rb.linearVelocityY = _velocity.y;

		_jumpBufferCounter = 0;
		_coyoteCounter = 0;
		_isJumpReleased = false;
	}

	public void VariableJump()
	{
		if (_rb.linearVelocityY > 0)
		{
			_rb.linearVelocityY *= jumpReleasedForce;
		}
		_isJumpReleased = false;
	}

	public void Move()
	{
		_rb.linearVelocityX = _velocity.x;
	}

	public void SetFallGravity() => _rb.gravityScale *= fallGravity;
	public void SetGroundGravity() => _rb.gravityScale = 1;
	#endregion

	private void CheckGround()
	{
		var origin = transform.position - new Vector3(0, raycastLength - 0.1f, 0);
		//_isGrounded = Physics2D.Raycast(origin, Vector3.down, raycastLength, groundMask);

		_isGrounded = Physics2D.OverlapCircle(origin, raycastLength, groundMask);
	}

	public void SetStateText(string newState)
	{
		stateText.text = newState;
	}

	public void SetInArea()
	{
		_isInsideArea = true;
	}

	public void SetOutArea()
	{
		_isInsideArea = false;
	}

	public Rigidbody2D GetBody() => _rb;
	public Animator GetAnimator() => _animator;

}

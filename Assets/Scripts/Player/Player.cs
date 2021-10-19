using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{

	private DreadantInput inputMaster;
	private InputAction movementAction;
	private InputAction camAction;

	private new Rigidbody rigidbody;

	[Header("Cam")]
	[SerializeField] private Camera playerCam;
	[SerializeField] private float camSpeed;

	[Header("Stats")]
	[SerializeField] [SyncVar] private float health;
	[SerializeField] private float speed;
	[SerializeField] private float jumpStrength;

	[SerializeField] private Weapon weapon;

	// Start is called before the first frame update
	void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		if (!isLocalPlayer)
		{
			playerCam.enabled = false;
			playerCam.GetComponent<AudioListener>().enabled = false;
			OnDisable();
			return;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!isLocalPlayer) return;

		CmdUpdateRotation(camAction.ReadValue<Vector2>());


	}

	private void FixedUpdate()
	{

		if (!isLocalPlayer) return;

		CmdMove(movementAction.ReadValue<Vector2>());
	}

	[Server]
	private void CheckHealth()
	{
		if (health <= 0)
		{
			NetworkServer.Destroy(gameObject);
		}
	}

	private bool IsGrounded()
	{
		return Physics.Raycast(transform.position + transform.up * 0.5f, -Vector3.up, 0.5f + 0.1f);
	}

	#region Input
	[Command]
	private void CmdMove(Vector2 input)
	{
		if (IsGrounded())
		{
			Vector3 force = (GetMovementVector(input) * Time.fixedDeltaTime * speed) * 100;
			rigidbody.AddForce(force);
		}
	}

	[Command]
	private void CmdUpdateRotation(Vector2 input)
	{
		transform.Rotate(0, input.x * Time.deltaTime * camSpeed, 0);
	}

	[Client]
	private void GetJump()
	{
		CmdJump();
	}

	[Command]
	private void CmdJump()
	{
		if (IsGrounded())
		{
			rigidbody.AddForce(new Vector3(0, jumpStrength, 0), ForceMode.Acceleration);
		}
	}

	[Client]
	private void GetFireShot()
	{
		CmdFireShot();
	}

	[Command]
	private void CmdFireShot()
	{
		weapon.Fire();
	}

	[Server]
	public void TakeDamage(float toTake)
	{
		health -= toTake;

		CheckHealth();
	}

	/// <summary>
	/// Get Vector3 based on transform.rotation
	/// </summary>
	/// <param name="input">Input from new Unity Input System</param>
	/// <returns>Vector 3 based on transform rotation and input</returns>
	private Vector3 GetMovementVector(Vector2 input)
	{
		Vector3 movement = Vector3.zero;

		if (input.x > 0) { movement += transform.right; }
		else if (input.x < 0) { movement -= transform.right; }

		if (input.y > 0) { movement += transform.forward; }
		else if (input.y < 0) { movement -= transform.forward; }
		else { }
		return movement;
	}

	#endregion

	private void OnEnable()
	{
		inputMaster = new DreadantInput();

		inputMaster.Player.Fire.performed += ctx => GetFireShot();
		inputMaster.Player.Jump.performed += ctx => GetJump();

		movementAction = inputMaster.Player.Move;
		camAction = inputMaster.Player.Look;

		inputMaster.Enable();
	}

	private void OnDisable()
	{
		movementAction.Disable();
		inputMaster.Disable();
	}
}

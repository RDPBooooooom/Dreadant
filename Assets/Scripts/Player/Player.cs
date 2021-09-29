using Mirror;
using System.Collections;
using System.Collections.Generic;
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
	[SerializeField] private float camSpeed = 10;

	[Header("Stats")]
	[SerializeField] private float speed = 100;

	// Start is called before the first frame update
	void Start()
	{
		if (!isLocalPlayer)
		{
			playerCam.enabled = false;
			OnDisable();
			return;
		}

		rigidbody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		Vector2 input = camAction.ReadValue<Vector2>();
		playerCam.transform.Rotate(0, input.x * Time.deltaTime * camSpeed, 0);
		Debug.Log("Cam stuff: " + camAction.ReadValue<Vector2>());
	}

	private void FixedUpdate()
	{
		CmdMove(movementAction.ReadValue<Vector2>());	
	}

	#region Input
	[Command]
	private void CmdMove(Vector2 input) {
		Vector3 newPos = rigidbody.position + new Vector3(speed * Time.deltaTime * input.x, 0, speed * Time.deltaTime * input.y);
		rigidbody.MovePosition(newPos);
	}

	[Client]
	public void GetFireShot()
	{
		CmdFireShot();
	}

	[Command]
	private void CmdFireShot()
	{
		Debug.Log("Shot Fired");
	}
	#endregion

	private void OnEnable()
	{
		inputMaster = new DreadantInput();

		inputMaster.Player.Fire.performed += ctx => GetFireShot();

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

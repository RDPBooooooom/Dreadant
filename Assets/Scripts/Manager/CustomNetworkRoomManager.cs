using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;

public class CustomNetworkRoomManager : NetworkRoomManager
{

	[Header("Custom Room Settings")]
	[SerializeField]
	private int minPlayersToStart;

	public int MinPlayersToStart { get => minPlayersToStart; set => minPlayersToStart = value; }

	public delegate void ClientStoped(string currentSceneName);
	public event ClientStoped OnClientStoped;

	public override void OnStopClient()
	{
		base.OnStopClient();
		
		string activeScene = SceneManager.GetActiveScene().name;

		OnClientStoped.Invoke(activeScene);

		if (GameplayScene.Equals(activeScene))
		{
			Cursor.lockState = CursorLockMode.Confined;
		}
	}

	public override void OnRoomClientExit()
	{
		Cursor.lockState = CursorLockMode.Confined;
	}

	public override void OnRoomClientDisconnect(NetworkConnection conn)
	{
		SceneManager.LoadScene(RoomScene);
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		GameManager manager = GameManager.Instance;

		if (manager != null)
		{
			Player p = conn.identity.gameObject.GetComponent<Player>();
			if (p.isClient)
			{
				manager.OnPlayerDisconnected(p);
			}
		}
		base.OnServerDisconnect(conn);
	}

	public void Leave()
	{
		StopHost();
		SceneManager.LoadScene(RoomScene);
	}
}

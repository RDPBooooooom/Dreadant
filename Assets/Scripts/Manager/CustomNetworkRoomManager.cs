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

		if (GameplayScene.Equals(activeScene)) {
			// TODO: Inform gameplaymanger
			Debug.Log("Client Stopped in Game Scene");
		}
	}
}

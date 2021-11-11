using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkHUD : NetworkBehaviour
{

	private CustomNetworkRoomManager manager;

	[SerializeField]
	private GameObject mainMenu;

	[SerializeField]
	private GameObject readyCheckMenu;

	[SerializeField]
	private Button readyButton;

	[SerializeField]
	private List<ReadyCheckUI> readyCheckMenus;

	private bool isHosting;

	private void Awake()
	{
		isHosting = false;
	}

	public void StartHost()
	{
		CheckManager();
		manager.StartHost();

		Debug.Log("Started host");

		ActivateReadyCheck();
		readyButton.gameObject.SetActive(true);

		isHosting = true;
	}
	public void StopHost()
	{
		manager.StopHost();

		ActivateMain();
		isHosting = false;
	}

	public void StartClient()
	{
		CheckManager();
		manager.StartClient();

		ActivateReadyCheck();
		readyButton.gameObject.SetActive(false);
	}

	public void StopClient()
	{
		manager.StopClient();
	}

	public void OnClientStoped(string activeScene)
	{
		if (manager.GameplayScene.Equals(activeScene))
		{
			SceneManager.LoadScene(manager.RoomScene);
		}

		ActivateMain();
	}

	private void ActivateMain()
	{
		mainMenu.SetActive(true);
		readyCheckMenu.SetActive(false);
	}

	private void ActivateReadyCheck()
	{
		mainMenu.SetActive(false);
		readyCheckMenu.SetActive(true);
	}

	public void Leave()
	{
		if (isHosting)
		{
			StopHost();
		}
		else
		{
			StopClient();
		}
	}

	public void Update()
	{
		if (isHosting || readyCheckMenu.activeSelf)
		{
			readyButton.interactable = CanStart();
		}

		if (isServer)
		{
			List<NetworkRoomPlayer> connectedPlayers = manager.roomSlots;

			Debug.Log("Number of connected Players" + connectedPlayers.Count);

			foreach (NetworkRoomPlayer player in connectedPlayers)
			{
				Debug.Log(player.index);
				UpdatePlayerUI(player);
			}
		}
	}

	[ClientRpc]
	void UpdatePlayerUI(NetworkRoomPlayer player)
	{
		readyCheckMenus[player.index].Player = player;
	}

	private void CheckManager()
	{
		if (manager == null)
		{
			manager = (CustomNetworkRoomManager)NetworkManager.singleton;
			manager.OnClientStoped += OnClientStoped;
		}
	}

	public void ReadyUnready()
	{
		List<NetworkRoomPlayer> connectedPlayers = manager.roomSlots;

		foreach (NetworkRoomPlayer player in connectedPlayers)
		{
			if (player.isLocalPlayer)
			{
				player.CmdChangeReadyState(!player.readyToBegin);
			}
		}
	}

	public void StartGame()
	{
		if (CanStart())
		{
			manager.OnRoomServerPlayersReady();
		}
	}

	public bool CanStart()
	{
		CheckManager();
		List<NetworkRoomPlayer> connectedPlayers = manager.roomSlots;
		bool allReady = true;
		foreach (NetworkRoomPlayer player in connectedPlayers)
		{
			if (!player.readyToBegin)
			{
				allReady = false;
			}
		}

		return allReady && connectedPlayers.Count >= manager.MinPlayersToStart;
	}
}

using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{

	public static GameManager Instance { get; private set; }

	private CustomNetworkRoomManager manager;

	private bool isSetupDone;

	[SerializeField]
	private List<Player> teamOne;
	[SerializeField]
	private List<Player> teamTwo;

	[SerializeField]
	private List<GameObject> teamOneSpawns;

	[SerializeField]
	private List<GameObject> teamTwoSpawns;

	[SerializeField]
	private GameUI playerUI;

	[SerializeField]
	private int teamOneWinCount;
	[SerializeField]
	private int teamTwoWinCount;

	[SerializeField]
	private GameObject deathRoomSpawn;

	public delegate void SetupDoneDelegate();
	public delegate void RoundDoneDelegate();

	public event SetupDoneDelegate SetupDoneEvent;
	public event RoundDoneDelegate RoundDoneEvent;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this.gameObject);
			return;
		}
		Instance = this;

		isSetupDone = false;

		teamOne = new List<Player>();
		teamTwo = new List<Player>();
	}
	// Start is called before the first frame update
	void Start()
	{
		SetupDoneEvent += OnSetupDone;
		manager = (CustomNetworkRoomManager)NetworkManager.singleton;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isSetupDone)
		{
			UpdateTeam();
		}
	}

	private void UpdateTeam()
	{
		foreach (NetworkRoomPlayer roomPlayer in manager.roomSlots)
		{
			Debug.Log("Room player:" + roomPlayer.connectionToClient.identity.gameObject.name);
			Player player = roomPlayer.connectionToClient.identity.gameObject.GetComponent<Player>();
			AddToTeam(player);
		}

		if (teamOne.Count + teamTwo.Count == manager.roomSlots.Count)
		{
			isSetupDone = true;
			SetupDoneEvent.Invoke();
		}
	}

	private void AddToTeam(Player player)
	{
		if (player == null) return;

		if (!teamOne.Contains(player) && !teamTwo.Contains(player))
		{
			if (teamOne.Count <= teamTwo.Count)
			{
				teamOne.Add(player);
				player.RPCSetTeam(Teams.TeamOne);
			}
			else
			{
				teamTwo.Add(player);
				player.RPCSetTeam(Teams.TeamTwo);
			}
		}
	}

	private void OnSetupDone()
	{
		SpawnTeams();
		SetupPlayerDiedEvent(teamOne);
		SetupPlayerDiedEvent(teamTwo);

		playerUI.UpdateUI(teamOneWinCount, teamTwoWinCount);
	}

	private void SetupPlayerDiedEvent(List<Player> players)
	{
		foreach (Player p in players)
		{
			p.PlayerDiedEvent += OnPlayerDied;
		}
	}

	private void SpawnTeams()
	{
		foreach (Player player in teamOne)
		{
			player.transform.position = GetRandomSpawnPosition(Teams.TeamOne);
		}
		foreach (Player player in teamTwo)
		{
			player.transform.position = GetRandomSpawnPosition(Teams.TeamTwo);
		}
	}

	private Vector3 GetRandomSpawnPosition(Teams team)
	{
		switch (team)
		{
			case Teams.TeamOne:
				return teamOneSpawns[Random.Range(0, teamOneSpawns.Count)].transform.position;
			case Teams.TeamTwo:
				return teamTwoSpawns[Random.Range(0, teamTwoSpawns.Count)].transform.position;
			default:
				return Vector3.zero;
		}
	}

	private void OnPlayerDied(Player player)
	{
		player.transform.position = deathRoomSpawn.transform.position;
		player.ResetPlayer();

		CheckTeamLose();
	}

	private void CheckTeamLose()
	{

		if (IsTeamDead(teamOne))
		{
			teamTwoWinCount++;
			Reset();
			playerUI.UpdateUI(teamOneWinCount, teamTwoWinCount);
		}
		else if (IsTeamDead(teamTwo))
		{
			teamOneWinCount++;
			Reset();
			playerUI.UpdateUI(teamOneWinCount, teamTwoWinCount);
		}
	}

	private void Reset()
	{
		foreach (Player p in teamOne)
		{
			p.FullResetPlayer();
		}
		foreach (Player p in teamTwo)
		{
			p.FullResetPlayer();
		}
		SpawnTeams();
	}

	private bool IsTeamDead(List<Player> playerList)
	{
		bool allPlayersDead = true;
		foreach (Player p in playerList)
		{
			if (p.IsAlive)
			{
				allPlayersDead = false;
				break;
			}
		}

		return allPlayersDead;
	}

	public enum Teams
	{
		TeamOne,
		TeamTwo,
		None
	}
}

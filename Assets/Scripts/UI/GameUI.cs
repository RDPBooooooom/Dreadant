using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameUI : NetworkBehaviour
{

	[SerializeField]
	private Image t1BgPanel;
	[SerializeField]
	private Text t1Text;

	[SerializeField]
	private Image t2BgPanel;
	[SerializeField]
	private Text t2Text;


	private GameManager.Teams playerTeam;
	public GameManager.Teams PlayerTeam
	{
		get => playerTeam;
		set
		{
			playerTeam = value;
			UpdateUIColor();
		}
	}

	[ClientRpc]
	public void UpdateUI(int teamOneScore, int teamTwoScore)
	{
		t1Text.text = "Team 1 Score: " + teamOneScore;
		t2Text.text = "Team 2 Score: " + teamTwoScore;
	}

	private void UpdateUIColor()
	{
		switch (playerTeam)
		{
			case GameManager.Teams.TeamOne:
				t1BgPanel.color = Color.green;
				break;
			case GameManager.Teams.TeamTwo:
				t2BgPanel.color = Color.green;
				break;
			default:
				break;
		}
	}

}

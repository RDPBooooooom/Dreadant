using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ReadyCheckUI : MonoBehaviour
{

	[SerializeField]
	private Text readyText;

	[SerializeField]
	private Text connectedText;

	public NetworkRoomPlayer Player { get; set; }

	private void Update()
	{
		if (Player != null)
		{
			connectedText.text = "Connected";
			if (Player.readyToBegin)
			{
				readyText.text = "Ready";
			}
			else
			{
				readyText.text = "Not Ready";
			}
		}
		else
		{
			connectedText.text = "Not connected";
			readyText.text = "Not ready";
		}
	}
}

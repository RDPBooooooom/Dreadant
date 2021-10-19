using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{

	[SerializeField] private float damageMultiplier;

	public float DamageTaken { get; private set; }

	private Player player;

	// Start is called before the first frame update
	void Start()
	{
		player = GetComponentInParent<Player>();
		DamageTaken = 0;
	}

	public void TakeDamage(float bulletDamage)
	{
		float toTake = damageMultiplier * bulletDamage;

		player.TakeDamage(toTake);
	}

}

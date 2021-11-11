using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{

	public float Damage { get; set; }
	public float TravelSpeed { get; set; }

	public float DmgReduction { get; set; }

	private Vector3 originalPos;

	private float travelTime;

	// Start is called before the first frame update
	void Start()
	{
		originalPos = transform.position;

		travelTime = 0;
	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(-transform.forward * TravelSpeed * Time.deltaTime, Space.World);

		travelTime += Time.deltaTime;

		if (travelTime > 10)
		{
			NetworkServer.Destroy(gameObject);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (!isServer) return;

		BodyPart playerHit = collision.collider.gameObject.GetComponent<BodyPart>();

		if (playerHit != null)
		{
			playerHit.TakeDamage(GetBulletDamage());
		}
		NetworkServer.Destroy(gameObject);
	}

	private float GetBulletDamage()
	{
		float distance = Vector3.Distance(originalPos, transform.position);

		return Mathf.Max(1, Damage - (Damage * distance * DmgReduction));
	}
}

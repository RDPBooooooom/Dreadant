using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

	[Header("Weapon Stats")]
	[SerializeField] private float damage;
	[SerializeField] private float speedModifier;
	[SerializeField] private float fireSpeed;
	[SerializeField] private float dmgReductionOverRange;
	[SerializeField] private float projectileTravelSpeed;
	[SerializeField] private WeaponType WeaponType;

	[Header("Projectile")]
	[SerializeField] private GameObject projectileMount;
	[SerializeField] private GameObject projectile;

	public float Damage { get => damage; private set => damage = value; }

	public void Fire()
	{

		GameObject instantiated = Instantiate(projectile, projectileMount.transform.position, transform.rotation);

		Projectile tmpProjectile = instantiated.GetComponent<Projectile>();
		tmpProjectile.DmgReduction = dmgReductionOverRange;
		tmpProjectile.Damage = damage;
		tmpProjectile.TravelSpeed = projectileTravelSpeed;

		NetworkServer.Spawn(instantiated);
	}
}

public enum WeaponType
{
	Main,
	Secondary,
	Melee
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Rebound : Cube
{
	[SerializeField] private float reboundForce;
	private Rigidbody2D rigidbody2d;

	private void Awake()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		if (collision.gameObject.layer == 30)
		{
			if (isConnnectedToEntity && leader)
			{
				if (!leader.isPlayerControlling)
					return;
				else
					rigidbody2d.AddForce(new Vector2(-1, 1) * reboundForce, ForceMode2D.Impulse);
			}
		}
	}
}

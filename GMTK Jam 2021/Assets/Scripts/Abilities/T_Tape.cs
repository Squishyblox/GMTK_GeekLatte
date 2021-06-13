using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T_Tape : Cube
{
    protected override void Update()
    {
        base.Update();
        if (isConnnectedToOff)
            StickTime = 5f;
    }

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
        if (isConnnectedToEntity)
            collision.gameObject.GetComponent<Cube>().StickTime = Mathf.Infinity;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
        collision.gameObject.GetComponent<Cube>().StickTime = 5f;
    }
}
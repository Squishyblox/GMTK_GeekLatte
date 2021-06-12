using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
	[SerializeField] private float speed = 10;

    void Update()
    {
        //super dirty. Will delete it later
		float hori = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");
		GetComponent<Rigidbody2D>().AddForce(new Vector2(hori, ver)* speed * Time.deltaTime, ForceMode2D.Impulse);
    }
}

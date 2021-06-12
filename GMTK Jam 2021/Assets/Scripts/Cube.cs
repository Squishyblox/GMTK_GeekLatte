using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cube : MonoBehaviour
{
    [SerializeField] private float stickTimer;

    protected void PartialStick(float stickTime) {}
    protected void ActivateAbility() {}

	private void OnCollisionEnter(Collision collision)
	{
		PartialStick(stickTimer);
	}
}
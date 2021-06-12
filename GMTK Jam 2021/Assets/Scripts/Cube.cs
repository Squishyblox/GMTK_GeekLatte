using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float stickTime = 5f;
	private Timer stickTimer;

	private void Awake()
	{
		stickTimer = Timer.CreateComponent(gameObject, "Stick Time", stickTime);
	}

	private void Update()
	{
		if (stickTimer.TimerFinished)
		{
			Debug.Log("Unstick");
		}
	}

	protected virtual void PartialStick(float stickTime) {
		Debug.Log("Stick!");
		stickTimer.StartTimer();
	}
    protected virtual void ActivateAbility() {}

	private void OnCollisionEnter(Collision collision)
	{
		PartialStick(stickTime);
	}
}
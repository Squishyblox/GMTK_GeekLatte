using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float stickTime = 5f;
    [SerializeField] private float unstickTime = 2f;
	
	private Timer stickTimer;
	private Timer unstickTimer;
	private bool canStick;

	private void Awake()
	{
		unstickTimer = Timer.CreateComponent(gameObject, "Unstick Time", unstickTime);
		stickTimer = Timer.CreateComponent(gameObject, "Stick Time", stickTime);
	}

	private void Update()
	{
		if (unstickTimer.TimerFinished)
		{
			canStick = true;
			Debug.Log("Unstick");
		}

		if (stickTimer.TimerFinished)
			unstickTimer.StartTimer();
	}

	protected virtual void PartialStick(float stickTime) {
		if (canStick)
		{
			Debug.Log("Stick!");
			stickTimer.StartTimer();
			canStick = false;
		}
	}

    protected virtual void ActivateAbility() {}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag== "Cube")
			PartialStick(stickTime);
	}
}
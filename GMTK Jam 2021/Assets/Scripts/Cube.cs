using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float stickTime = 5f;
    [SerializeField] private float unstickTime = 2f;
	
	private Timer stickTimer;
	private Timer unstickTimer;
	private bool canStick = true;
	private float timePassed;

	private void Awake()
	{
		unstickTimer = Timer.CreateComponent(gameObject, "Unstick Time", unstickTime);
		stickTimer = Timer.CreateComponent(gameObject, "Stick Time", stickTime);
	}

	private void Start()
	{
		canStick = true;
	}

	protected virtual IEnumerator PartialStick(Rigidbody rb) {
		Stick(rb);
		yield return new WaitForSeconds(stickTime);
		Unstick(rb);
		yield return new WaitForSeconds(unstickTime);
		yield return 0;
	}

    protected virtual void ActivateAbility() {}

	private void OnCollisionEnter(Collision collision)
	{
		if (canStick && collision.gameObject.tag == "Cube") {
			Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
			StartCoroutine(PartialStick(rb));
		}
	}

	private void Stick(Rigidbody rb)
	{
		Debug.Log("Stick!");
		rb.transform.parent = Player.Instance.transform;
		Debug.Log(rb.transform.parent.name);
		rb.constraints = RigidbodyConstraints.FreezeAll;
		canStick = false;
	}

	private void Unstick(Rigidbody rb)
	{
		Debug.Log("Unstick");
		rb.transform.parent = null;
		rb.constraints = RigidbodyConstraints.None;
		canStick = true;
	}
}
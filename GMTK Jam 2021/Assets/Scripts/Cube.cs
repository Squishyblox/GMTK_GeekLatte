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

	protected virtual IEnumerator PartialStick(Rigidbody rb, Collision collision) {
		Stick(rb, collision);
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
			StartCoroutine(PartialStick(rb, collision));
		}
	}

	private void Stick(Rigidbody rb, Collision collision)
	{
		Debug.Log("Stick!");

		// creates joint
		FixedJoint joint = gameObject.AddComponent<FixedJoint>();
		// sets joint position to point of contact
		joint.anchor = collision.contacts[0].point;
		// conects the joint to the other object
		joint.connectedBody = collision.contacts[0].otherCollider.transform.GetComponentInParent<Rigidbody>();
		// Stops objects from continuing to collide and creating more joints
		joint.enableCollision = false;

		//rb.transform.parent = Player.Instance.transform;
		//Debug.Log(rb.transform.parent.name);
		//rb.constraints = RigidbodyConstraints.FreezeAll;
		canStick = false;
	}

	private void Unstick(Rigidbody rb)
	{
		Debug.Log("Unstick");
		//rb.transform.parent = null;
		//rb.constraints = RigidbodyConstraints.None;
		canStick = true;
	}
}
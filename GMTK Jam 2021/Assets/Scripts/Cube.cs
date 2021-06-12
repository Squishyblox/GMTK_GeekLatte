using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float stickTime = 5f;
    [SerializeField] private float unstickTime = 2f;

    private bool canStick = true;
	private float speed = 10;

    private void Start()
    {
        canStick = true;
    }

	void Update() 
	{
		//super dirty. Will delete it later
		float hori = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");
		GetComponent<Rigidbody2D>().AddForce(new Vector2(hori, ver)* speed * Time.deltaTime, ForceMode2D.Impulse);	
	}

    protected virtual IEnumerator PartialStick(Rigidbody2D rb, Collision2D collision)
    {
        Stick(rb, collision);
        yield return new WaitForSeconds(stickTime);
        Unstick(rb);
        yield return new WaitForSeconds(unstickTime);
        yield return 0;
    }

    protected virtual void ActivateAbility() { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canStick && collision.gameObject.tag == "Cube")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            StartCoroutine(PartialStick(rb, collision));
        }
    }

    private void Stick(Rigidbody2D rb, Collision2D collision)
    {
        Debug.Log("Stick!");
        // creates joint
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        // sets joint position to point of contact
        joint.anchor = collision.contacts[0].point;
        // conects the joint to the other object
        joint.connectedBody = collision.transform.GetComponentInParent<Rigidbody2D>();
        // Stops objects from continuing to collide and creating more joints
        joint.enableCollision = false;
        canStick = false;
    }

    private void Unstick(Rigidbody2D rb)
    {
        Debug.Log("Unstick");
        //rb.transform.parent = null;
        //rb.constraints = RigidbodyConstraints.None;
        canStick = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [Header("Stick")]
    [SerializeField] private float stickTime = 5f;
    [SerializeField] private float unstickTime = 2f;

    [Header("ConnectedCubes")]
    public List<CubeAndJoint> connectedCubes = new List<CubeAndJoint>();//only neighbor
    public List<Cube> allConnectedCubes = new List<Cube>();
    //a list to cache all connected cubes(neighbor cubes), 
    //make it easier when applying ability effect onto each other. (for T, O etc.)
    [SerializeField] protected bool isConnnectedToEntity = false;
    //a flag that set to true when connected with E.
    [SerializeField] protected bool isConnnectedToOff = false;
    //a flag that set to true when connected with O.
    [SerializeField] protected bool isConnnectedToGravity = false;
    //a flag that set to true when connected with T.
    [SerializeField] protected bool isConnnectedToTape = false;
    //a flag that set to true when connected with G. If it's connected to G but G is disable this will be false
    [SerializeField] protected E_Entity leader;
    //cache the leader to easily check if the leader is selected by the player or not.
    //check if leader.isPlayerControlling == true;

    [Header("GroundCheck")]
    //a bool for entity to check if can jump
    public bool IsGrounded = false;
    [SerializeField] protected LayerMask groundLayer;
    public bool isCollidingWithGround = false;
    public Vector2 groundCollidePoint;

    [SerializeField]private bool canStick = true;
    private Rigidbody2D connectedRb;

	public bool IsConnnectedToTape
	{
        get { return isConnnectedToTape; }
        set
		{
            isConnnectedToTape = value;
			if (!isConnnectedToTape)
			{
                StartCoroutine(UnstickWithTime());
			}
		}
	}

	protected virtual void Start()
    {
        //GameManager.instance.onCubeEvent += UpdateAllConnection;
        canStick = true;
    }

    protected virtual void Update()
    {
        if (!isConnnectedToGravity)
            GetComponent<Rigidbody2D>().gravityScale = 1;
        else
            GetComponent<Rigidbody2D>().gravityScale = -1;
    }

    protected virtual void FixedUpdate()
    {
        UpdateAllConnection();
        CheckIfConnectedToOff();//always check first since off will turn off any other blocks
        CheckIfConnectedToEntity();
        CheckIfConnectedToGravity();
        CheckIfConnectedToTape();

        UpdateIsGrounded();
    }

    protected virtual IEnumerator PartialStick(Rigidbody2D rb, Collision2D collision)
    {
        Stick(collision);
        yield return new WaitForSeconds(stickTime);
        //canStick = false;

        if (!isConnnectedToTape)
        {
            Unstick();
            yield return new WaitForSeconds(unstickTime);
            canStick = true;
            yield return 0;
        }
    }

    protected virtual void ActivateAbility() { }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Cube")
        {
            if (cubeAlreadyConnected(collision.gameObject))//if it's already stick, do not stick it again
                return;

            connectedRb = collision.gameObject.GetComponent<Rigidbody2D>();

            StartCoroutine(PartialStick(connectedRb, collision));
        }

        if(collision.gameObject.tag == "Ground")
        {
            isCollidingWithGround = true;
            groundCollidePoint = collision.GetContact(0).point;
        }
    }

    private void Stick(Collision2D collision)
    {
        if (!canStick || !collision.gameObject.GetComponent<Cube>().canStick)
            return;
        
        //Debug.Log($"Stick to {collision.gameObject.name}!");
        // creates joint
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        // sets joint position to point of contact
        //joint.anchor = collision.GetContact(0).point;
        // conects the joint to the other object
        joint.connectedBody = collision.transform.GetComponent<Rigidbody2D>();
        // Stops objects from continuing to collide and creating more joints
        joint.enableCollision = false;

        connectedCubes.Add(new CubeAndJoint(connectedRb.GetComponent<Cube>(), joint));
        
        GameManager.instance.CubeEvent();//call this whenever cube sticks or unsticks
    }

    private IEnumerator UnstickWithTime()
	{
        Unstick();
        yield return new WaitForSeconds(unstickTime);
        canStick = true;
        yield return 0;
    }

    public void Unstick()
    {
        foreach (var c in connectedCubes)//find the connected cube in order to detory the joint
        {
            if (c.joint != null)
			{
                Destroy(c.joint);//destory joint
            }
            connectedCubes.Remove(c);
            break;
        }

        GameManager.instance.CubeEvent();//call this whenever cube sticks or unsticks
    }

    public bool cubeAlreadyConnected(GameObject gameObject)
    {
        foreach (var c in connectedCubes)
        {
            if (c.cube == gameObject.GetComponent<Cube>())
            {
                print("already Stick");
                return true;
            }
        }

        return false;
    }

    ///Connection
    private void CheckIfConnectedToEntity()//check if it's connected to E in any way
    {
        if (this is E_Entity)//self is entity
        {
            //if (!isConnnectedToOff)
            {
                isConnnectedToEntity = true;
                leader = (E_Entity)this;
                return;
            }
        }

        if (connectedCubes.Count == 0)//of course not connected to anything
        {
            isConnnectedToEntity = false;
            return;
        }

        isConnnectedToEntity = false;
        foreach (var c in allConnectedCubes)
        {
            if (c is E_Entity)
            {
                //found entity
                leader = (E_Entity)c;
                isConnnectedToEntity = true;
                break;
            }
        }
    }

    private void CheckIfConnectedToGravity()//check if it's connected to G in any way
    {
        if (this is G_Gravity)//self is entity
        {
            if (isConnnectedToEntity/* && !isConnnectedToOff*/)
            {
                isConnnectedToGravity = true;
                return;
            }
        }

        if (connectedCubes.Count == 0)//of course not connected to anything
        {
            isConnnectedToGravity = false;
            return;
        }

        isConnnectedToGravity = false;//function below will set to true if there's gravity connected.
        foreach (var c in allConnectedCubes)
        {
            if (c is G_Gravity)
            {
                if (isConnnectedToEntity/* && !isConnnectedToOff*/)
                {
                    isConnnectedToGravity = true;
                    break;//break this foreach loop
                }
            }
        }
    }

    private void CheckIfConnectedToOff()//check if it's connected to G in any way
    {
        if (this is O_Off)//self is entity
        {
            if (isConnnectedToEntity/* && !isConnnectedToOff*/)
            {
                isConnnectedToOff = true;
                return;
            }
        }

        if (connectedCubes.Count == 0)//of course not connected to anything
        {
            isConnnectedToOff = false;
            return;
        }

        isConnnectedToOff = false;//function below will set to true if there's gravity connected.
        foreach (var c in allConnectedCubes)
        {
            if (c is O_Off)
            {
                if (isConnnectedToEntity/* && !isConnnectedToOff*/)
                {
                    isConnnectedToOff = true;
                    break;//break this foreach loop
                }
            }
        }
    }

    private void CheckIfConnectedToTape()//check if it's connected to T in any way
    {
        if (connectedCubes.Count == 0 || isConnnectedToOff)//of course not connected to anything
        {
            if (IsConnnectedToTape)
                IsConnnectedToTape = false;
            return;
        }

        isConnnectedToTape = false;//function below will set to true if there's tape connected.
        foreach (var c in allConnectedCubes)
        {
            if (c is T_Tape)
            {
                if (isConnnectedToEntity && leader)
                {
                    if (!leader.isPlayerControlling)
                        return;
                    isConnnectedToTape = true;
                    break;//break this foreach loop
                }
            }
        }
    }

    public void UpdateAllConnection()
    {
        allConnectedCubes = new List<Cube>();

        if (connectedCubes.Count == 0)//of course not connected to anything
        {
            return;
        }

        Queue<Cube> queueingCubes = new Queue<Cube>();//cubes waiting to be add;

        foreach (var c in connectedCubes)//add connected cubes to the queue
        {
            queueingCubes.Enqueue(c.cube);
        }

        while (queueingCubes.Count > 0)
        {
            Cube c = queueingCubes.Dequeue();
            allConnectedCubes.Add(c);//mark as checked
            foreach (var cNeighbor in c.connectedCubes)//Add all neighbors to the queue, except the checked one.
            {
                bool alreadyHave = false;
                foreach (var oldc in allConnectedCubes)
                {
                    if (oldc == cNeighbor.cube)
                    {
                        alreadyHave = true;
                    }
                }

                if (!alreadyHave)
                {
                    queueingCubes.Enqueue(cNeighbor.cube);
                }
            }
        }

        //CheckIfConnectedToOff();//always check first since off will turn off any other blocks
        //CheckIfConnectedToEntity();
        //CheckIfConnectedToGravity();
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (canStick && collision.gameObject.tag == "Cube")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (cubeAlreadyConnected(rb.gameObject))//if it's already stick, do not stick it again
                return;

            StartCoroutine(PartialStick(rb, collision));
        }
        
        if(collision.gameObject.tag == "Ground")
        {
            isCollidingWithGround = true;
            groundCollidePoint = collision.GetContact(0).point;
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Ground")
        {
            isCollidingWithGround = false;
        }
    }

    private void UpdateIsGrounded()
    {
        if(!isCollidingWithGround)
        {
            IsGrounded = false;
            return;
        }
        
        float raycastDistance = 0.5f;
        Vector2 castDirection = new Vector2(0, GetComponent<Rigidbody2D>().gravityScale * -1f);
        RaycastHit2D raycastHit = Physics2D.Raycast(groundCollidePoint, castDirection, raycastDistance, groundLayer);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
            IsGrounded = true;
        }
        else
        {
            rayColor = Color.red;
            IsGrounded = false;
        }

        Debug.DrawRay(groundCollidePoint, castDirection * raycastDistance, rayColor);
    }
}

[System.Serializable]
public class CubeAndJoint//simply store the joint and cube together, so it's easier to find the joint and destory
{
    public Cube cube;
    public FixedJoint2D joint;

    public CubeAndJoint(Cube cube, FixedJoint2D joint)
    {
        this.cube = cube;
        this.joint = joint;
    }
}
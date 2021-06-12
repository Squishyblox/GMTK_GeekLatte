using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float stickTime = 5f;
    [SerializeField] private float unstickTime = 2f;

    public List<CubeAndJoint> connectedCubes = new List<CubeAndJoint>();//only neighbor
    public List<Cube> allConnectedCubes = new List<Cube>();
    //a list to cache all connected cubes(neighbor cubes), 
    //make it easier when applying ability effect onto each other. (for T, O etc.)
    [SerializeField] protected bool isConnnectedToEntity = false;
    //a flag that set to true when connected with E.
    [SerializeField] protected bool isConnnectedToOff = false;
    //a flag that set to true when connected with O.
    [SerializeField] protected bool isConnnectedToGravity = false;
    //a flag that set to true when connected with G. If it's connected to G but G is disable this will be false
    [SerializeField] protected E_Entity leader;
    //cache the leader to easily check if the leader is selected by the player or not.
    //check if leader.isPlayerControlling == true;

    private bool canStick = true;

    protected virtual void Start()
    {
        GameManager.instance.onCubeEvent += UpdateAllConnection;
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
        CheckIfConnectedToOff();//always check first since off will turn off any other blocks
        CheckIfConnectedToEntity();
        CheckIfConnectedToGravity();
    }

    protected virtual IEnumerator PartialStick(Rigidbody2D rb, Collision2D collision)
    {
        Stick(rb, collision);
        yield return new WaitForSeconds(stickTime);
        Unstick(rb, collision);
        canStick = false;
        yield return new WaitForSeconds(unstickTime);
        canStick = true;
        yield return 0;
    }

    protected virtual void ActivateAbility() { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canStick && collision.gameObject.tag == "Cube")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (cubeAlreadyConnected(rb.gameObject))//if it's already stick, do not stick it again
                return;

            StartCoroutine(PartialStick(rb, collision));
        }
    }

    private void Stick(Rigidbody2D rb, Collision2D collision)
    {
        //Debug.Log("Stick!");
        // creates joint
        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        // sets joint position to point of contact
        joint.anchor = collision.contacts[0].point;
        // conects the joint to the other object
        joint.connectedBody = collision.transform.GetComponent<Rigidbody2D>();
        // Stops objects from continuing to collide and creating more joints
        joint.enableCollision = false;

        connectedCubes.Add(new CubeAndJoint(rb.GetComponent<Cube>(), joint));

        GameManager.instance.CubeEvent();//call this whenever cube sticks or unsticks
    }

    private void Unstick(Rigidbody2D rb, Collision2D collision)
    {
        //Debug.Log("Unstick");
        //CubeAndJoint cubeAndJoint = new CubeAndJoint();
        foreach (var c in connectedCubes)//find the connected cube in order to detory the joint
        {
            if (c.cube == rb.GetComponent<Cube>())
            {
                if (c.joint != null)
                    Destroy(c.joint);//destory joint
                connectedCubes.Remove(c);
                break;
            }
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
                if (!c.isConnnectedToOff)
                {
                    //found entity
                    leader = (E_Entity)c;
                    isConnnectedToEntity = true;
                    break;
                }
            }
        }
    }

    private void CheckIfConnectedToGravity()//check if it's connected to G in any way
    {

        if (this is G_Gravity)//self is entity
        {
            if (isConnnectedToEntity && !isConnnectedToOff)
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
                if (isConnnectedToEntity && !isConnnectedToOff)
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
            isConnnectedToOff = true;
            return;
        }

        if (connectedCubes.Count == 0)//of course not connected to anything
        {
            isConnnectedToOff = false;
            return;
        }

        isConnnectedToOff = false;//function below will set to true if there's gravity connected.
        foreach (var c in connectedCubes)
        {
            if (c.cube is O_Off)
            {
                isConnnectedToOff = true;
                break;//break this foreach loop
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
                if (!allConnectedCubes.Contains(c))//do not add the checked one.
                    queueingCubes.Enqueue(cNeighbor.cube);
            }
        }

        //CheckIfConnectedToOff();//always check first since off will turn off any other blocks
        //CheckIfConnectedToEntity();
        //CheckIfConnectedToGravity();
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
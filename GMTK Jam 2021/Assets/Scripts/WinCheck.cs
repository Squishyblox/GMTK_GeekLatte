using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCheck : MonoBehaviour
{
    [SerializeField] private Transform[] letterList = new Transform[8];
    [SerializeField] private Cube[] cubeList = new Cube[8];
    int i = 0;

    public GameObject winText;

    private void Update()
    {
        foreach (var c in cubeList)
        {
            
            if (c)
            {
                
                if (c is H_Hook)
                {
                    i = 5;
                    c.GetComponent<LineRenderer>().enabled = false;
                }
                else if (c is O_Off)
                    i = 1;
                else if (c is R_Rebound)
                    i = 7;
                else if (c is G_Gravity)
                    i = 2;
                else if (c is T_Tape)
                {
                    i = 0;
                    if (cubeList[i] != null)
                        i = 4;
                }
                else if (c is E_Entity)
                {
                    i = 3;
                    if (cubeList[i] != null)
                        i = 6;
                }
                
                c.transform.position = letterList[i].position;
                c.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                c.GetComponent<Rigidbody2D>().gravityScale = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cube cube = collision.gameObject.GetComponent<Cube>();
        if (!cube)
            return;

        cube.Unstick();

        if (cube is H_Hook)
        {
            i = 5;
            cube.GetComponent<LineRenderer>().enabled = false;
        }
        else if (cube is O_Off)
            i = 1;
        else if (cube is R_Rebound)
            i = 7;
        else if (cube is G_Gravity)
            i = 2;
        else if (cube is T_Tape)
        {
            i = 0;
            if (cubeList[i] != null)
                i = 4;
        }
        else if (cube is E_Entity)
        {
            i = 3;
            if (cubeList[i] != null)
                i = 6;
        }

        cube.transform.eulerAngles = Vector3.zero;
        cube.transform.position = letterList[i].position;
        cube.GetComponent<Rigidbody2D>().freezeRotation = true;
        cube.GetComponent<Collider2D>().enabled = false;

        foreach (var c in cube.GetComponent<Cube>().allConnectedCubes)
        {
            if (c)
            {
                Destroy(c.GetComponent<FixedJoint2D>());
            }
        }

        cube.GetComponent<Cube>().enabled = false;

        cubeList[i] = cube;

        if (IsGameOver())
        {
            winText.SetActive(true);
            Debug.Log("You win");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cubeList[i] = null;
    }

    private bool IsGameOver()
    {
        foreach (var c in cubeList)
        {
            if (c == null)
                return false;
        }

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public event Action onCubeEvent;
    //call this whenever cubes stick or unstick,
    //so it will cause all the cube to update the connectedCube and isActive
    public void CubeEvent()
    {
        if(onCubeEvent != null)
            onCubeEvent();
    }
}

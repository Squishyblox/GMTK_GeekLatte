using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<E_Entity> entities;
    public E_Entity currentEntity;

    public E_Entity startEntity;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }

        entities = new List<E_Entity>();//empty for entity to add themselves
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Tab))
            SwitchEntity();
    }

    private void Start() 
    {
        currentEntity = startEntity;
        currentEntity.isPlayerControlling = true;
    }

    public void SwitchEntity()
    {
        foreach (var e in entities)
        {
            e.isPlayerControlling = false;
        }
        
        foreach (var e in entities)
        {
            if(e != currentEntity)
            {
                currentEntity = e;
                e.isPlayerControlling = true;
                return;
            }
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

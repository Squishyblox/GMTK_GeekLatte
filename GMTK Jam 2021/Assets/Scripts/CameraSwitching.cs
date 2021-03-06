using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    //public Transform originalEntity;
    //public Transform otherEntity;
    public Vector3 cameraOffset;
    public Vector3 cameraLookAtOffset;
    private Vector3 targetPosition;
    private Vector3 cameraTargetPosition;
    private bool lookAtOriginal;
    //[Range(0, 1)]
    public float speed = 10;

    private void Start() 
    {
        /*
        if (originalEntity || otherEntity == null)
        {
            Debug.LogError("Camera target entities not set.");
        }
        lookAtOriginal = true;
        */
    }

    private void FixedUpdate()
    {
        if(GameManager.instance.currentEntity == null)
            return;

        E_Entity e = GameManager.instance.currentEntity;
        
        targetPosition = e.transform.position + cameraLookAtOffset;
        cameraTargetPosition = e.transform.position + cameraOffset;

        this.transform.LookAt(Vector3.LerpUnclamped(this.transform.position - cameraOffset + cameraLookAtOffset, targetPosition, speed * Time.deltaTime));
        this.transform.position = Vector3.LerpUnclamped(this.transform.position, cameraTargetPosition, speed * Time.deltaTime);
        
        /*
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            lookAtOriginal = !lookAtOriginal;
        }
        if (lookAtOriginal)
        {
            targetPosition = originalEntity.position + cameraLookAtOffset;
            cameraTargetPosition = originalEntity.position + cameraOffset;
        }
        else
        {
            targetPosition = otherEntity.position + cameraLookAtOffset;
            cameraTargetPosition = otherEntity.position + cameraOffset;
        }*/
    }
}

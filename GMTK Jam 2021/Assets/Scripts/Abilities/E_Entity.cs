using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Entity : Cube
{
    [Header("Move")]
    [SerializeField] private float moveSpd = 10;
    private float maxVelocity = 1000f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 50;
    [SerializeField] private float jumpCD = 0.5f;

    public bool isPlayerControlling = true;// a flag to check if it's selected by player

    protected override void Start()
    {
        base.Start();
        isConnnectedToEntity = true;//default true on entity
        StartCoroutine(PlayerJump());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isPlayerControlling && isConnnectedToEntity)
        {
            PlayerMove();
        }
    }

    private void PlayerMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = 0; //Input.GetAxisRaw("Vertical");
        GetComponent<Rigidbody2D>().AddForce(new Vector2(horizontal, vertical) * moveSpd * Time.deltaTime * (1+ allConnectedCubes.Count), ForceMode2D.Impulse);
        //GetComponent<Rigidbody2D>().velocity = new Vector2(horizontal* moveSpd, GetComponent<Rigidbody2D>().velocity.y);
    }

    private IEnumerator PlayerJump()
    {
        while (true)
        {
            if (isPlayerControlling && isConnnectedToEntity)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    GetComponent<Rigidbody2D>().velocity += Vector2.up * jumpForce * (1+ allConnectedCubes.Count);
                    yield return new WaitForSeconds(jumpCD);
                }
            }
            yield return 0;
        }
    }
}

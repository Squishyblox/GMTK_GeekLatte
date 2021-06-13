using UnityEngine;

/// <summary>
/// Ability for the letter H: The grappling hook.
/// </summary>
public class H_Hook : Cube
{
    [Tooltip("Which layer(s) of objects the grappling hook attach to")]
    [SerializeField] private LayerMask grappableLayerMask;

    [Tooltip("How quickly the grappling hook pulls the player towards it")]
    [SerializeField] private float retractTime = 1f;

    private LineRenderer lineRenderer;
    private Vector3 grapplePosition;
    private State currentState;
    private Rigidbody2D rb;
    private float currentLerpTime;
    private bool isLerping;

    /// <summary>
    /// Statemachine for the grappling hook.
    /// </summary>
    private enum State
	{
        Normal,
        GrappleStart,
        IsGrappling,
        Grappled
	}

    /// <summary>
    /// Get attached components from the game object.
    /// </summary>
    private void Awake()
	{
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Initialize state.
    /// </summary>
	protected override void Start()
	{
        base.Start();
        currentState = State.Normal;
    }

    /// <summary>
    /// Handles the retraction of the grappling hook.
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (currentState == State.IsGrappling)
		{
            currentState = State.Grappled;
            currentLerpTime = 0f;
            isLerping = true;
		}

        if (isLerping)
		{
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > retractTime)
                StopGrapple();

            float percentComplete = currentLerpTime / retractTime;
            rb.MovePosition(Vector2.Lerp(transform.position, grapplePosition, percentComplete));
        }
    }

    /// <summary>
    /// Uses the statemachine to check player's input.
    /// </summary>
    protected override void Update()
	{
        base.Update();

        if (!leader)
            return;

        switch (currentState)
		{
			case State.Normal:
                if (Input.GetMouseButtonDown(0))
                    StartGrapple();
                break;
            case State.GrappleStart:
                if (Input.GetMouseButtonUp(0))
                    currentState = State.IsGrappling;
                else if (Input.GetMouseButtonDown(1))
                    StopGrapple();
                break;
			default:
				break;
		}
    }

    /// <summary>
    /// Cast a ray to detect a grappling point for to pull the player.
    /// </summary>
	private void StartGrapple()
	{
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, grappableLayerMask))
		{
            grapplePosition = hit.point;

            currentState = State.GrappleStart;
        }
    }

    /// <summary>
    /// Resets states and variables.
    /// </summary>
    private void StopGrapple()
	{
        currentLerpTime = retractTime;
        isLerping = false;
        lineRenderer.enabled = false;
        currentState = State.Normal;
    }

    private void LateUpdate()
	{
        DrawRope();
    }

    /// <summary>
    /// Feed position data to the line renderer while grappling.
    /// </summary>
	private void DrawRope()
	{
        if (currentState != State.Normal)
		{
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePosition);
        }
	}

    /// <summary>
    /// If collided with the ceiling, stop grappling.
    /// </summary>
    /// <param name="collision">The object collided with.</param>
	protected override void OnCollisionEnter2D(Collision2D collision)
	{
        base.OnCollisionEnter2D(collision);
		if (currentState == State.Grappled && collision.gameObject.layer == 31)
            StopGrapple();
	}
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Grabling : MonoBehaviour
{
    [Header("References")]
    private NewPlayerController P_controller;
    public Transform cam;
    public Transform orientation;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    LineRenderer lr;

    [Header("Grappling")]
    public float maxGrapDistance;
    public float grapDelay;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    bool grappling;

    bool perform = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        P_controller = GetComponent<NewPlayerController>();
        lr = GetComponentInChildren<LineRenderer>();


    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        P_controller.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(orientation.position, cam.forward, out hit, maxGrapDistance, whatIsGrappleable))
        {
            Debug.Log("Grapple hit: " + hit.collider.name);

            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grapDelay);
        }
        else
        {
            Debug.Log("Grapple missed.");

            grapplePoint = cam.position + cam.forward * maxGrapDistance;

            Invoke(nameof(StopGrapple), grapDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        P_controller.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPoinOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPoinOnArc = overshootYAxis;

        P_controller.hookToPosition(grapplePoint, highestPoinOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        P_controller.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

        if (perform)
        {
            StartGrapple();
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    public void Ongrap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Performed");
            perform = true;
        }
        else if (context.canceled)
        {
            Debug.Log("Cancel");
            perform = false;
        }
    }

}

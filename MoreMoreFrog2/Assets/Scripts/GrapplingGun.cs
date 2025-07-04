using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour
{
    [Header("References")]
    private NewPlayerController P_controller;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    LineRenderer lr;

    [Header("Input")]
    public InputActionReference grappleAction; // ลิงก์ไปยัง Input Action ที่ตั้งไว้ใน Input Asset

    [Header("Grappling")]
    public float maxGrapDistance;
    public float grapDelay;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    bool grappling;

    void Start()
    {
        P_controller = GetComponent<NewPlayerController>();

        // Subscribe action
        grappleAction.action.performed += ctx => StartGrapple();
        grappleAction.action.Enable();
    }

    private void OnDestroy()
    {
        //grappleAction.action.performed -= ctx => StartGrapple();
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrapDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grapDelay);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrapDistance;

            Invoke(nameof(StopGrapple), grapDelay);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        // TODO: Add rope physics or movement here
    }

    private void StopGrapple()
    {
        grappling = false;
        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    void Update()
    {
        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0,gunTip.position);
        }
    }
}

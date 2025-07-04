using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

public class GrappleHookV5 : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform cameraTransform;
    public Transform gunTip;
    public LayerMask grappleLayer;

    public float maxDistance = 30f;
    public float grappleSpeed = 10f;

    public InputActionReference grappleAction;      // คลิกซ้าย - เหวี่ยง
    public InputActionReference grappleDashAction;  // คลิกขวา - พุ่ง

    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private Rigidbody rb;
    private SpringJoint currentJoint;

    private NewPlayerController playerController;

    void OnEnable()
    {
        grappleAction.action.performed += StartGrapple;
        grappleAction.action.canceled += StopGrapple;
        grappleAction.action.Enable();

        grappleDashAction.action.performed += DashToGrapple;
        grappleDashAction.action.Enable();
    }

    void OnDisable()
    {
        grappleAction.action.performed -= StartGrapple;
        grappleAction.action.canceled -= StopGrapple;
        grappleAction.action.Disable();

        grappleDashAction.action.performed -= DashToGrapple;
        grappleDashAction.action.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<NewPlayerController>();
    }

    void Update()
    {
        if (isGrappling)
        {
            DrawRope();
        }
    }

    void StartGrapple(InputAction.CallbackContext ctx)
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxDistance, grappleLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (hit.collider.TryGetComponent<Rigidbody>(out Rigidbody enemyRb))
                {
                    StartCoroutine(PullPlayerAndEnemyTogether(enemyRb));
                }

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, gunTip.position);
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                grapplePoint = hit.point;
                isGrappling = true;

                SpringJoint joint = gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);
                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;

                joint.spring = 200f;
                joint.damper = 10f;
                joint.massScale = 3.5f;

                currentJoint = joint;

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, gunTip.position);
                lineRenderer.SetPosition(1, grapplePoint);
            }
        }
        else
        {
            Vector3 kickDirection = -cameraTransform.forward;
            float baseKickStrength = grappleSpeed * 0.1f;

            Vector3 finalKickForce = kickDirection * baseKickStrength;
            finalKickForce.y *= 0.01f;

            if (playerController != null && playerController.IsGrounded())
            {
                rb.AddForce(kickDirection * baseKickStrength, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(kickDirection * (baseKickStrength * 0.05f), ForceMode.Impulse);
            }

            Vector3 fakePoint = cameraTransform.position + cameraTransform.forward * maxDistance;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, fakePoint);

            StartCoroutine(ClearRopeAfterDelay(0.2f));
        }
    }

    void DashToGrapple(InputAction.CallbackContext ctx)
    {
        if (!isGrappling) return;

        Vector3 direction = (grapplePoint - transform.position).normalized;
        float dashForce = grappleSpeed * 50f;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * dashForce, ForceMode.Impulse);

        isGrappling = false;

        if (currentJoint != null)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }

        lineRenderer.positionCount = 0;
    }

    void StopGrapple(InputAction.CallbackContext ctx)
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;

        if (currentJoint != null)
        {
            Destroy(currentJoint);
            currentJoint = null;
        }
    }

    void DrawRope()
    {
        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    IEnumerator ClearRopeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.positionCount = 0;
    }

    IEnumerator PullPlayerAndEnemyTogether(Rigidbody enemyRb)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        Vector3 startPlayer = transform.position;
        Vector3 startEnemy = enemyRb.position;
        Vector3 midPoint = (startPlayer + startEnemy) / 2f;

        if (enemyRb.TryGetComponent<EnemyFloat>(out EnemyFloat enemyFloat))
        {
            enemyFloat.DisableFloat();
        }

        lineRenderer.positionCount = 2;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 newPlayerPos = Vector3.Lerp(startPlayer, midPoint, t);
            Vector3 newEnemyPos = Vector3.Lerp(startEnemy, midPoint, t);

            rb.MovePosition(newPlayerPos);
            enemyRb.MovePosition(newEnemyPos);

            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, enemyRb.position);

            yield return null;
        }

        rb.linearVelocity = Vector3.zero;
        enemyRb.linearVelocity = Vector3.zero;

        lineRenderer.positionCount = 0;

        if (enemyFloat != null)
            enemyFloat.EnableFloat();
    }
}

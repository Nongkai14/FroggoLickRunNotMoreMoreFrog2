using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;

public class GrappleHookV4 : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform cameraTransform;
    public Transform gunTip;
    public LayerMask grappleLayer;

    public float maxDistance = 30f;
    public float grappleSpeed = 10f;

    public InputActionReference grappleAction;

    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private Rigidbody rb;
    private SpringJoint currentJoint;

    private NewPlayerController playerController;

    [Header("Grapple Accuracy")]
    public float enemyHitAngleThreshold = 10f;

    void OnEnable()
    {
        grappleAction.action.performed += StartGrapple;
        grappleAction.action.canceled += StopGrapple;
        grappleAction.action.Enable();
    }

    void OnDisable()
    {
        grappleAction.action.performed -= StartGrapple;
        grappleAction.action.canceled -= StopGrapple;
        grappleAction.action.Disable();
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
        Vector3 ReduceY(Vector3 force, float yScale)
        {
            force.y *= yScale;
            return force;
        }

        RaycastHit hit;
        Vector3 targetPoint;


        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxDistance, grappleLayer))
        {
            targetPoint = hit.point;
        }
        else
        {
            // ถ้าไม่เจออะไร ก็ใช้จุดไกลสุดตามแนวกล้อง
            targetPoint = cameraTransform.position + cameraTransform.forward * maxDistance;
        }

        Vector3 shootDir = (targetPoint - gunTip.position).normalized;

        if (Physics.Raycast(gunTip.position, shootDir, out hit, maxDistance, grappleLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // คำนวณมุมระหว่างทิศทางกล้องกับทิศไปยังศัตรู
                Vector3 dirToEnemy = (hit.point - cameraTransform.position).normalized;
                float angle = Vector3.Angle(cameraTransform.forward, dirToEnemy);

                if (angle <= enemyHitAngleThreshold)
                {
                    // อยู่ในระยะเล็ง → ติดศัตรู
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
                    // หลุดจากขอบ crosshair → ยิงผ่าน
                    Debug.Log("Grapple miss: enemy is outside aim threshold");
                    StartCoroutine(ClearRopeAfterDelay(0.2f));
                }
            }
            else
            {
                grapplePoint = hit.point;
                isGrappling = true;

                SpringJoint joint = gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = grapplePoint;

                float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint);
                joint.maxDistance = distanceFromPoint * 0.5f;
                joint.minDistance = distanceFromPoint * 0.25f;

                joint.spring = 100f;
                joint.damper = 30f;
                joint.massScale = 1f;

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
            finalKickForce = ReduceY(finalKickForce, 0.01f); // ลดแรง Y


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
            if (enemyRb == null || lineRenderer == null)
                break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 newPlayerPos = Vector3.Lerp(startPlayer, midPoint, t);
            Vector3 newEnemyPos = Vector3.Lerp(startEnemy, midPoint, t);

            if (rb != null)
                rb.MovePosition(newPlayerPos);

            if (enemyRb != null)
                enemyRb.MovePosition(newEnemyPos);

            // ✅ ป้องกัน SetPosition error
            if (lineRenderer.positionCount < 2)
                lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, enemyRb.position);

            yield return null;
        }

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        if (enemyRb != null)
            enemyRb.linearVelocity = Vector3.zero;

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;

        if (enemyFloat != null)
            enemyFloat.EnableFloat();
    }
}

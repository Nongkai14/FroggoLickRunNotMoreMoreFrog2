using UnityEngine;

public class EnemyFloat : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 0.5f;

    [HideInInspector] public bool allowFloat = true;

    private Vector3 startPos;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("EnemyFloat: Rigidbody not found!");
            enabled = false;
            return;
        }

        startPos = transform.position;

        rb.useGravity = false;
        // rb.isKinematic = true; // ปิดถ้าอยากให้ศัตรูมีการชน
    }

    void FixedUpdate()
    {
        if (!allowFloat) return;

        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        Vector3 targetPos = startPos + new Vector3(0, newY, 0);
        rb.MovePosition(targetPos);
    }

    public void DisableFloat()
    {
        allowFloat = false;
    }

    public void EnableFloat()
    {
        startPos = transform.position; // รีเซ็ตตำแหน่งเริ่ม
        allowFloat = true;
    }
}

using UnityEngine;

public class SpawnMaker : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 2.5f);
        }
    }
}

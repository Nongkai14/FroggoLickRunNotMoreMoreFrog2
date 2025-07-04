using UnityEngine;
using TMPro;

public class DistanceScore : MonoBehaviour
{
    public Transform player;
    public TMP_Text scoreText;

    private float score = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        if (player != null)
            lastPosition = player.position;
    }

    void Update()
    {
        if (player == null) return;

        float deltaDistance = Vector3.Distance(player.position, lastPosition);
        score += deltaDistance;
        lastPosition = player.position;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    public float GetScore()
    {
        return score;
    }
}

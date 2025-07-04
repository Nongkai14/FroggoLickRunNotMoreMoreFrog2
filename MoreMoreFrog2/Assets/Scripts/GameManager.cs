// GameManager.cs
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int eliminateCount = 0;

    public HealthBarScript healthBar;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddEliminate()
    {
        eliminateCount++;
        Debug.Log("Eliminated: " + eliminateCount);

        if (healthBar != null)
        {
            healthBar.Heal(5);
        }
    }


}

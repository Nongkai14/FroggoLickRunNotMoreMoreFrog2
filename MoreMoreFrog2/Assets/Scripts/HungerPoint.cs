using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider HungerBarSlider;
    public Slider easeHungerSlider;
    public float maxHunger;
    public float hunger;
    private float lerpspeed = 5f;
    private NewPlayerController player;
    public GameObject gameOverPanel;
    private GameOverController gameOverController;

    void Start()
    {
        player = GetComponent<NewPlayerController>();
        gameOverController = FindAnyObjectByType<GameOverController>();
        maxHunger = 100f;
        hunger = maxHunger;

        InvokeRepeating(nameof(ReduceHunger), 1f, 2f);
    }

    void Update()
    {
        float lerpAmount = lerpspeed * Time.deltaTime;

        HungerBarSlider.value = hunger;

        if (Mathf.Abs(easeHungerSlider.value - hunger) > 0.01f)
        {
            easeHungerSlider.value = Mathf.Lerp(easeHungerSlider.value, hunger, lerpAmount);
        }
        else
        {
            easeHungerSlider.value = hunger;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Heal(10);
        }
        if (hunger <= 0 && gameOverPanel != null)
        {
            gameOverController.ShowGameOverScreen(gameOverPanel);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void TakeDamage(float damage)
    {
        hunger -= damage;
        hunger = math.max(hunger, 0);
    }

    public void Heal(float amount)
    {
        hunger += amount;
        hunger = math.min(hunger, maxHunger);
    }

    void ReduceHunger()
    {
        if (hunger <= 0)
        {
            CancelInvoke(nameof(ReduceHunger));
            return;
        }

        TakeDamage(3f);
    }


}

using TMPro;
using UnityEngine;

// Shows the boss fight countdown on screen.

public class BossTimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    
    [SerializeField] private Boss boss;

    [Header("Low Time Warning")]
  
    [SerializeField] private float lowTimeThreshold = 5f;
    [SerializeField] private Color lowTimeColor = Color.red;

    private Color normalColor;
    private int lastShownSeconds = -1; 

    private void Start()
    {
        if (timerText == null)
        {
            Debug.LogWarning("BossTimerDisplay: Timer Text is not assigned in the Inspector.");
            enabled = false;
            return;
        }

        normalColor = timerText.color;

        
        if (boss == null)
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null && gameManager.isBossLevel)
            {
                boss = gameManager.currentBoss;
            }
        }

        if (boss == null)
        {
            timerText.enabled = false;
            enabled = false;
        }
    }

    
    private void LateUpdate()
    {
        float remaining = Mathf.Max(0f, boss.timeRemaining);

        // needs to round up so it reads 0:01 until the time is really gone not 0:00 early. otherwise players can lose before time is up
        int seconds = Mathf.CeilToInt(remaining);

        if (seconds != lastShownSeconds)
        {
            lastShownSeconds = seconds;
            timerText.text = (seconds / 60) + ":" + (seconds % 60).ToString("00");
        }

        timerText.color = remaining <= lowTimeThreshold ? lowTimeColor : normalColor;
    }
}

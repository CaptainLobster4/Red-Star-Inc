using TMPro;
using UnityEngine;

// Shows the boss fight countdown on screen.
//
// Goes on any object in the boss level (the Canvas is fine). It only READS
// Boss.timeRemaining, which GameManager already ticks down each frame, so it
// never touches GameManager's logic. When the player wins or loses, GameManager
// stops ticking and this display freezes on the final time automatically.
//
// On levels with no boss (Is Boss Level unticked, or no Current Boss assigned)
// the text hides itself, so the same HUD prefab can be reused everywhere.
public class BossTimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    [Tooltip("Leave empty to use the Current Boss assigned on GameManager automatically.")]
    [SerializeField] private Boss boss;

    [Header("Low Time Warning")]
    [Tooltip("Timer turns the warning color when this many seconds (or fewer) are left.")]
    [SerializeField] private float lowTimeThreshold = 5f;
    [SerializeField] private Color lowTimeColor = Color.red;

    private Color normalColor;
    private int lastShownSeconds = -1; // only rebuild the string when the shown second changes

    private void Start()
    {
        if (timerText == null)
        {
            Debug.LogWarning("BossTimerDisplay: Timer Text is not assigned in the Inspector.");
            enabled = false;
            return;
        }

        normalColor = timerText.color;

        // No boss assigned here? Borrow the one GameManager is running the fight with.
        if (boss == null)
        {
            GameManager gameManager = FindAnyObjectByType<GameManager>();
            if (gameManager != null && gameManager.isBossLevel)
            {
                boss = gameManager.currentBoss;
            }
        }

        // Not a boss level: hide the text and stop updating
        if (boss == null)
        {
            timerText.enabled = false;
            enabled = false;
        }
    }

    // LateUpdate so GameManager.Update has already ticked the timer this frame
    private void LateUpdate()
    {
        float remaining = Mathf.Max(0f, boss.timeRemaining);

        // Round up so it reads 0:01 until the time is really gone, not 0:00 early
        int seconds = Mathf.CeilToInt(remaining);

        if (seconds != lastShownSeconds)
        {
            lastShownSeconds = seconds;
            timerText.text = (seconds / 60) + ":" + (seconds % 60).ToString("00");
        }

        timerText.color = remaining <= lowTimeThreshold ? lowTimeColor : normalColor;
    }
}

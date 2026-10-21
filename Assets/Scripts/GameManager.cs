using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// keep track of bullet count
// instantiate gun and target
// UI tracking (tickets, target progress, current equipped gun, current level, and collectibles)
public class GameManager : MonoBehaviour
{
    // progress bar fill
    public Image fillImage;

    // UI text assignments
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI BulletText;

    // amount of bullets
    public int bulletCount;

    // amount of bullets needed to cut out entire shape
    public float shapeAmount = 30f;

    [Header("Boss Fight")]
    public bool isBossLevel;
    public Boss currentBoss; // assign in Inspector on boss levels

    [Tooltip("Optional. Text shown on a boss win when a collectible is unlocked for the first time.")]
    public TextMeshProUGUI collectibleUnlockedText;

    [Header("Bullets")]
    [Tooltip("How many shots the player gets this level. 0 = unlimited. Running out before the shape is done loses the level.")]
    public int maxBullets = 0;

    [Tooltip("After the last bullet is fired, seconds to wait for it to land before the level is lost.")]
    public float outOfAmmoDelay = 0.75f;

    [Header("Win Reward")]
    [Tooltip("Tickets awarded for beating this level.")]
    public int winTicketsBase = 20;

    [Tooltip("Bonus tickets for every unused bullet when the level is won (only when Max Bullets is above 0).")]
    public int ticketsPerBulletLeft = 2;

    [Tooltip("Optional. Shows e.g. '+34 tickets' on a win.")]
    public TextMeshProUGUI ticketRewardText;

    [Header("Level Transition")]
    [Tooltip("Exact scene name (as it appears in Build Settings) to load when this level is won. Set per-level in the Inspector - not tied to build order, so it stays correct once a level-select menu lets players jump around.")]
    public string nextSceneName;

    [Tooltip("Seconds to show the Win text before moving on to the next scene.")]
    public float winDelay = 1.5f;

    [Tooltip("Exact scene name of the Lose screen scene (as it appears in Build Settings). Loaded when the player loses.")]
    public string loseSceneName;

    [Tooltip("Seconds to show the Lose text (if assigned) before moving to the Lose screen.")]
    public float loseDelay = 1.5f;

    private PlayerCharacter playerCharacter;
    private bool levelOver = false;
    private bool loseTriggered = false;
    private int bulletsLeft;
    private float outOfAmmoTimer = 0f;

    public int BulletsLeft => bulletsLeft;
    public int ShotsFired { get; private set; }


    public void UpdateProgress(float newProgress)
    {
        if (fillImage.fillAmount + newProgress > 1f)
        {
            fillImage.fillAmount = 1f;
            return;
        }
        else
        {
            fillImage.fillAmount += newProgress;
        }

    }

    public float DecreaseSideAmount(float amount)
    {
        amount -= 1f;
        return amount;
    }


    void Start()
    {
        // limited bullets (0 = unlimited)
        bulletsLeft = maxBullets;
        ShotsFired = 0;
        UpdateBulletText();

        // current level
        // is boss fight?
        // equipped gun
        // Starting Health


        GameObject targetObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (targetObj != null)
        {
            playerCharacter = targetObj.GetComponent<PlayerCharacter>();
        }

        if (isBossLevel && currentBoss != null)
        {
            currentBoss.ResetFight();
        }

        //set progress to zero when new level starts eventually

    }

    void Update()
    {
        if (levelOver)
        {
            return;
        }

        // this would be where progress is tracked
        if (fillImage.fillAmount >= 1f)
        {
            levelOver = true;
            winText.gameObject.SetActive(true);
            playerCharacter.win = true;

            AwardBossCollectible();

            AwardWinTickets();

            StartCoroutine(LoadNextSceneAfterDelay());
            return;
        }

        // out of bullets with the shape unfinished = lose (after a short wait so the last shot can land)
        if (maxBullets > 0 && bulletsLeft <= 0)
        {
            outOfAmmoTimer += Time.deltaTime;
            if (outOfAmmoTimer >= outOfAmmoDelay)
            {
                levelOver = true;
                TriggerLose();
                return;
            }
        }

        // boss fights run on a timer instead of (or alongside) shape progress -
        // running out of time means the player succumbs before finishing the shape
        if (isBossLevel && currentBoss != null)
        {
            currentBoss.TickTimer(Time.deltaTime);

            if (currentBoss.TimeExpired())
            {
                levelOver = true;
                TriggerLose();
            }
        }
    }

    // Called by every new BulletHole. Returns false if the player is out of bullets.
    public bool TryUseBullet()
    {
        if (levelOver)
        {
            return false;
        }

        if (maxBullets > 0)
        {
            if (bulletsLeft <= 0)
            {
                return false;
            }
            bulletsLeft--;
        }

        ShotsFired++;
        UpdateBulletText();
        return true;
    }

    private void UpdateBulletText()
    {
        if (BulletText == null)
        {
            return;
        }

        BulletText.text = maxBullets > 0 ? "Bullets: " + bulletsLeft : "";
    }

    // Winning pays tickets: a base amount plus a bonus for every unused bullet
    private void AwardWinTickets()
    {
        int reward = winTicketsBase;
        if (maxBullets > 0)
        {
            reward += ticketsPerBulletLeft * Mathf.Max(0, bulletsLeft);
        }

        if (reward <= 0)
        {
            return;
        }

        PlayerInventory.AddTickets(reward);

        if (ticketRewardText != null)
        {
            ticketRewardText.text = "+" + reward + " tickets";
            ticketRewardText.gameObject.SetActive(true);
        }
    }

    // Beating a boss level unlocks that boss's collectible (Boss.maskReward).
    // Losing, or a non-boss level, awards nothing.
    private void AwardBossCollectible()
    {
        if (!isBossLevel || currentBoss == null || currentBoss.maskReward == null)
        {
            return;
        }

        if (CollectibleManager.Instance == null)
        {
            Debug.LogWarning("GameManager: no CollectibleManager found, so the collectible wasn't saved. Start from the main menu, or add a CollectibleManager object to this scene.");
            return;
        }

        bool firstTime = !CollectibleManager.Instance.IsUnlocked(currentBoss.maskReward);
        CollectibleManager.Instance.Unlock(currentBoss.maskReward);

        if (firstTime && collectibleUnlockedText != null)
        {
            collectibleUnlockedText.text = "New collectible: " + currentBoss.maskReward.collectibleName;
            collectibleUnlockedText.gameObject.SetActive(true);
        }
    }

    public void TriggerLose()
    {
        // guard so a second call (e.g. from another script) can't start a second scene load
        if (loseTriggered)
        {
            return;
        }
        loseTriggered = true;
        levelOver = true;

        // the on-screen "You lost!" text is now optional - the Lose screen scene takes over
        if (loseText != null)
        {
            loseText.gameObject.SetActive(true);
        }
        if (playerCharacter != null)
        {
            playerCharacter.lose = true;
        }

        StartCoroutine(LoadLoseSceneAfterDelay());
    }

    private IEnumerator LoadLoseSceneAfterDelay()
    {
        yield return new WaitForSeconds(loseDelay);

        if (string.IsNullOrEmpty(loseSceneName))
        {
            Debug.LogWarning("GameManager: loseSceneName is not set on this level's GameManager - staying on this scene.");
            yield break;
        }

        SceneManager.LoadSceneAsync(loseSceneName);
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(winDelay);

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("GameManager: nextSceneName is not set on this level's GameManager - staying on this scene.");
            yield break;
        }

        SceneManager.LoadSceneAsync(nextSceneName);
    }
}
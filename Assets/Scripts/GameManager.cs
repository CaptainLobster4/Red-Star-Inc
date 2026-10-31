using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // progress bar fill
    public Image fillImage;

    // UI texts
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI BulletText;

    // amount of bullets
    public int bulletCount;

    // max bullets
    public float shapeAmount = 30f;

    [Header("Boss Fight")]
    public bool isBossLevel;
    public Boss currentBoss;


    public TextMeshProUGUI collectibleUnlockedText;

    [Header("Win Reward")]

    public int winTicketsBase = 20;


    public int ticketsPerBulletLeft = 2;


    public TextMeshProUGUI ticketRewardText;

    [Header("Level Transition")]

    public string nextSceneName;


    public float winDelay = 1.5f;


    public string loseSceneName;


    public float loseDelay = 1.5f;

    private PlayerCharacter playerCharacter;
    private bool levelOver = false;
    private bool loseTriggered = false;


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

        // progress track
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

    // Winning earns more tickets per bullet remaining
    private void AwardWinTickets()
    {
        int bulletsLeft = playerCharacter != null ? Mathf.Max(0, Mathf.FloorToInt(playerCharacter.bulletCount)) : 0;
        int reward = winTicketsBase + ticketsPerBulletLeft * bulletsLeft;

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

        if (loseTriggered)
        {
            return;
        }


        if (playerCharacter != null && playerCharacter.win)
        {
            return;
        }
        loseTriggered = true;
        levelOver = true;


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
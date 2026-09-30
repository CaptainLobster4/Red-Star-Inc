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
    public GameObject winMenu;
    public GameObject loseMenu;
    public TextMeshProUGUI BulletText;
    public GameObject DimScreen;

    // amount of bullets
    public int bulletCount;

    // amount of bullets needed to cut out entire shape
    public float shapeAmount = 30f;

    [Header("Boss Fight")]
    public bool isBossLevel;
    public Boss currentBoss; // assign in Inspector on boss levels

    [Header("Level Transition")]
    [Tooltip("Exact scene name (as it appears in Build Settings) to load when this level is won. Set per-level in the Inspector - not tied to build order, so it stays correct once a level-select menu lets players jump around.")]
    public string nextSceneName;

    [Tooltip("Seconds to show the Win text before moving on to the next scene.")]
    public float winDelay = 1.5f;

    private PlayerCharacter playerCharacter;
    private bool levelOver = false;

    public bool temp = false;
    public bool temp2 = false;



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

        DimScreen.gameObject.SetActive(false);
        loseMenu.gameObject.SetActive(false);
        winMenu.gameObject.SetActive(false);



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
            winMenu.gameObject.SetActive(true);
            playerCharacter.win = true;

            StartCoroutine(LoadNextSceneAfterDelay());
            return;
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

        if (temp == true)
        {
            TriggerLose();
        }

        if (temp2 == true)
        {
            winMenu.gameObject.SetActive(true);
            DimScreen.gameObject.SetActive(true);

        }

    }

    public void TriggerLose()
    {
        loseMenu.gameObject.SetActive(true);
        DimScreen.gameObject.SetActive(true);
        playerCharacter.lose = true;
    }

    public void ReloadScene()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Menu()
    {
        SceneManager.LoadSceneAsync(0);

    }

    public void GoToShop()
    {
        SceneManager.LoadSceneAsync(3);

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
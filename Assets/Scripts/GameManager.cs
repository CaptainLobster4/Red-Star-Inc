using TMPro;
using UnityEngine;
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


    private PlayerCharacter playerCharacter;




    public void UpdateProgress(float newProgress)
    {
        if (fillImage.fillAmount + newProgress > 1f)
        {
            fillImage.fillAmount = 1f;
            return;
        }
        else {
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


        //set progress to zero when new level starts eventually

    }

    void Update()
    {
        // this would be where progress is tracked
        // 

        if (fillImage.fillAmount >= 1f)
        {
            winText.gameObject.SetActive(true);
            playerCharacter.win = true;
        }


    }
}

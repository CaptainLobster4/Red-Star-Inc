using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class QuickDrawManager : MonoBehaviour
{
    private enum State { Intro, Red, Yellow, Live, Between, Over }

    [Header("Input (same asset as PlayerCharacter)")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Targets")]
    [Tooltip("Each prefab needs a Collider2D. One is picked at random every round.")]
    [SerializeField] private GameObject[] targetPrefabs;
    [Tooltip("Where the target appears on early levels (before random spawning starts).")]
    [SerializeField] private Vector2 centerSpawn = new Vector2(0f, 1f);
    [Tooltip("Random spawn / bounce area in world units (bottom-left and top-right corners). Yellow box in the Scene view.")]
    [SerializeField] private Vector2 areaMin = new Vector2(-2f, -1f);
    [SerializeField] private Vector2 areaMax = new Vector2(2f, 3.5f);

    [Header("Bullet Hole")]
    [Tooltip("A prefab with a SpriteRenderer (your bullet hole sprite). Spawned where the player taps. Give it a Sorting Order higher than the targets so it draws on top. No collider needed.")]
    [SerializeField] private GameObject bulletHolePrefab;

    [Header("Traffic Light (UI Images)")]
    [SerializeField] private Image redLight;
    [SerializeField] private Image yellowLight;
    [SerializeField] private Image greenLight;
    [SerializeField] private Color redOn = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color yellowOn = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private Color greenOn = new Color(0.2f, 1f, 0.4f);
    [Range(0f, 1f)]
    [SerializeField] private float unlitBrightness = 0.25f;

    [Header("Light Timing (seconds, random between min and max)")]
    [SerializeField] private Vector2 redTime = new Vector2(0.8f, 2.2f);
    [SerializeField] private Vector2 yellowTime = new Vector2(0.5f, 1.6f);
    [SerializeField] private float timeBetweenRounds = 1f;
    [SerializeField] private float startDelay = 1.2f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [Tooltip("Health bar pieces, left to right (your sketch has 3). Each is an Image; the script makes them fill left to right.")]
    [SerializeField] private Image[] healthSegments;
    [Tooltip("Tick this if your segment images are rotated 90 degrees (like -90 on Z). Leave unticked if they're not rotated.")]
    [SerializeField] private bool segmentsAreRotated = true;
    [SerializeField] private float healthGainOnHit = 6f;
    [SerializeField] private float missPenalty = 18f;
    [Tooltip("Penalty for tapping before the light turns green. 0 = ignore early taps.")]
    [SerializeField] private float falseStartPenalty = 8f;

    [Header("Difficulty")]
    [SerializeField] private int hitsPerLevel = 4;
    [Tooltip("Health lost per second while the target is on screen, at level 1.")]
    [SerializeField] private float baseDrainPerSecond = 8f;
    [Tooltip("Added to the drain every level.")]
    [SerializeField] private float drainIncreasePerLevel = 3.5f;
    [Tooltip("From this level on the target spawns at a random position.")]
    [SerializeField] private int randomPositionLevel = 3;
    [Tooltip("From this level on the target also moves after spawning.")]
    [SerializeField] private int movingLevel = 5;
    [SerializeField] private float moveSpeedAtFirstMovingLevel = 1.5f;
    [SerializeField] private float moveSpeedIncreasePerLevel = 0.4f;

    [Header("Target Size")]
    [Tooltip("Random size multiplier picked each spawn (x = smallest, y = biggest), multiplied on top of the prefab's own scale.")]
    [SerializeField] private Vector2 randomSizeRange = new Vector2(0.7f, 1.4f);
    [Tooltip("Every level, the target gets this much smaller (0.05 = 5% per level).")]
    [SerializeField] private float shrinkPerLevel = 0.05f;
    [Tooltip("The size multiplier will never go below this, so the target stays hittable.")]
    [SerializeField] private float minimumSizeMultiplier = 0.35f;

    [Header("Rewards")]
    [Tooltip("Tickets (shared PlayerInventory) per target hit. 0 = none.")]
    [SerializeField] private int ticketsPerHit = 1;

    [Header("UI Text (all optional)")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over")]
    [Tooltip("Seconds after game over before the game restarts by itself. 0 = wait for a button wired to Restart().")]
    [SerializeField] private float autoRestartDelay = 0f;

    private InputAction tapAction;
    private InputAction positionAction;

    private State state = State.Intro;
    private float timer;
    private float health;
    private int hits;
    private int misses;
    private int score;
    private GameObject currentTarget;
    private GameObject lastTarget;   
    private GameObject currentHole;
    private float spawnTime;

    private int Level => 1 + hits / Mathf.Max(1, hitsPerLevel);

    private void Awake()
    {
        tapAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");
        tapAction.Enable();
        positionAction.Enable();

        foreach (Image seg in healthSegments)
        {
            if (seg == null) continue;
            seg.type = Image.Type.Filled;
            if (segmentsAreRotated)
            {
                seg.fillMethod = Image.FillMethod.Vertical;
                seg.fillOrigin = (int)Image.OriginVertical.Bottom;
            }
            else
            {
                seg.fillMethod = Image.FillMethod.Horizontal;
                seg.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
        }
    }

    private void Start()
    {
        Restart();
    }

    //Resets everything in this scene. Hook  Game Over panel button to this.
    public void Restart()
    {
        CancelInvoke();

        if (currentTarget != null) Destroy(currentTarget);
        currentTarget = null;
        ClearLeftovers();

        health = maxHealth;
        hits = 0;
        misses = 0;
        score = 0;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        SetLights(false, false, false);
        Say("Get ready...");
        state = State.Intro;
        timer = startDelay;
        UpdateUI();
    }

    private void Update()
    {
        if (state == State.Over) return;

        bool tapped = tapAction.triggered;
        Vector2 tapWorld = Vector2.zero;
        if (tapped)
        {
            tapWorld = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());
        }

        switch (state)
        {
            case State.Intro:
            case State.Between:
                timer -= Time.deltaTime;
                if (timer <= 0f) StartRound();
                break;

            case State.Red:
                if (tapped && FalseStart(tapWorld)) break;
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Yellow;
                    timer = Random.Range(yellowTime.x, yellowTime.y);
                    SetLights(false, true, false);
                    Say("Steady...");
                }
                break;

            case State.Yellow:
                if (tapped && FalseStart(tapWorld)) break;
                timer -= Time.deltaTime;
                if (timer <= 0f) GoGreen();
                break;

            case State.Live:
                //the target is on screen health drains faster each level
                health -= (baseDrainPerSecond + drainIncreasePerLevel * (Level - 1)) * Time.deltaTime;
                UpdateUI();
                if (health <= 0f) { GameOver(); break; }

                if (tapped) ResolveShot(tapWorld);
                break;
        }
    }

    //Rounds

    private void StartRound()
    {
        ClearLeftovers();
        state = State.Red;
        timer = Random.Range(redTime.x, redTime.y);
        SetLights(true, false, false);
        Say("Wait for green...");
    }

    private void GoGreen()
    {
        if (targetPrefabs == null || targetPrefabs.Length == 0)
        {
            Debug.LogWarning("QuickDrawManager: no target prefabs assigned.");
            state = State.Over;
            return;
        }

        state = State.Live;
        SetLights(false, false, true);
        Say("SHOOT!");

        int level = Level;
        Vector2 pos = level >= randomPositionLevel
            ? new Vector2(Random.Range(areaMin.x, areaMax.x), Random.Range(areaMin.y, areaMax.y))
            : centerSpawn;

        //random shape from the array
        GameObject prefab = targetPrefabs[Random.Range(0, targetPrefabs.Length)];
        currentTarget = Instantiate(prefab, pos, Quaternion.identity);

        //random size
        float levelShrink = Mathf.Max(0f, 1f - shrinkPerLevel * (level - 1));
        float sizeMultiplier = Random.Range(randomSizeRange.x, randomSizeRange.y) * levelShrink;
        sizeMultiplier = Mathf.Max(minimumSizeMultiplier, sizeMultiplier);
        currentTarget.transform.localScale *= sizeMultiplier;

        QuickDrawTarget mover = currentTarget.GetComponent<QuickDrawTarget>();
        if (mover == null) mover = currentTarget.AddComponent<QuickDrawTarget>();

        float speed = level >= movingLevel
            ? moveSpeedAtFirstMovingLevel + moveSpeedIncreasePerLevel * (level - movingLevel)
            : 0f;
        mover.Init(areaMin, areaMax, speed);

        spawnTime = Time.time;
    }


    private void SpawnHole(Vector2 position)
    {
        if (bulletHolePrefab == null) return;
        currentHole = Instantiate(bulletHolePrefab, position, Quaternion.identity);
    }

   
    private void ClearLeftovers()
    {
        if (lastTarget != null) Destroy(lastTarget);
        if (currentHole != null) Destroy(currentHole);
        lastTarget = null;
        currentHole = null;
    }

    private bool FalseStart(Vector2 tapWorld)
    {
        if (falseStartPenalty <= 0f) return false; 
        //early taps ignored

        SpawnHole(tapWorld);
        health -= falseStartPenalty;
        EndRound("Too early!");
        return true;
    }

    private void ResolveShot(Vector2 worldPoint)
    {
        Physics2D.SyncTransforms(); 
        //moving targets
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);

        bool hitTarget = hit != null && currentTarget != null && hit.transform.IsChildOf(currentTarget.transform);

        SpawnHole(worldPoint); 
        

        if (hitTarget)
        {
            int ms = Mathf.RoundToInt((Time.time - spawnTime) * 1000f);
            hits++;
            score += Mathf.Max(100, 1500 - ms);
            health = Mathf.Min(maxHealth, health + healthGainOnHit);

            if (ticketsPerHit > 0) PlayerInventory.AddTickets(ticketsPerHit);

            EndRound("Hit! " + ms + " ms");
        }
        else
        {
            misses++;
            health -= missPenalty;
            EndRound("Miss!");
        }
    }

    private void EndRound(string message)
    {
        //freeze the target where it is so the bullet hole sits on it
        if (currentTarget != null)
        {
            QuickDrawTarget mover = currentTarget.GetComponent<QuickDrawTarget>();
            if (mover != null) mover.enabled = false;
            lastTarget = currentTarget;
        }
        currentTarget = null;

        SetLights(false, false, false);
        Say(message);
        UpdateUI();

        if (health <= 0f)
        {
            GameOver();
            return;
        }

        state = State.Between;
        timer = timeBetweenRounds;
    }

    private void GameOver()
    {
        state = State.Over;
        health = 0f;

        if (currentTarget != null) Destroy(currentTarget);
        SetLights(false, false, false);
        Say("Out of health!");
        UpdateUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (autoRestartDelay > 0f) Invoke(nameof(Restart), autoRestartDelay);
    }

    //UI

    private void SetLights(bool red, bool yellow, bool green)
    {
        SetLight(redLight, redOn, red);
        SetLight(yellowLight, yellowOn, yellow);
        SetLight(greenLight, greenOn, green);
    }

    private void SetLight(Image img, Color onColor, bool lit)
    {
        if (img == null) return;
        img.color = lit ? onColor : onColor * unlitBrightness;
    }

    private void UpdateUI()
    {
        if (healthSegments != null && healthSegments.Length > 0)
        {
            float total = Mathf.Clamp01(health / maxHealth) * healthSegments.Length;
            for (int i = 0; i < healthSegments.Length; i++)
            {
                if (healthSegments[i] != null)
                {
                    healthSegments[i].fillAmount = Mathf.Clamp01(total - i);
                }
            }
        }

        if (levelText != null) levelText.text = "Lvl. " + Level;
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    private void Say(string text)
    {
        if (messageText != null) messageText.text = text;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 size = areaMax - areaMin;
        Gizmos.DrawWireCube((areaMin + areaMax) * 0.5f, size);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerSpawn, 0.2f);
    }
}
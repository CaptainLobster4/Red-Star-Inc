using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// TEMPORARY. Drop this on any object in a level (Canvas is fine). It only prints to the Console
// and changes nothing in your game. Delete it once the bug is found.
// Every line starts with [Diag] so you can type that in the Console search box.
public class ShotDiagnostics : MonoBehaviour
{
    private PlayerCharacter player;
    private GameManager gm;
    private Collider2D paperCol;

    private bool lastWin, lastLose, lastPaperOn;
    private int lastHoles;
    private int lastFillPercent = -1;
    private bool tappedThisFrame;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerCharacter>();
        gm = FindAnyObjectByType<GameManager>();
        if (player != null && player.Paper != null) paperCol = player.Paper.GetComponent<Collider2D>();
        if (paperCol != null) lastPaperOn = paperCol.enabled;

        Log("Started. PlayerCharacter found=" + (player != null)
            + " | on MainCamera-tagged object=" + (player != null && player.CompareTag("MainCamera"))
            + " | GameManager found=" + (gm != null)
            + " | paper collider found=" + (paperCol != null)
            + " | ammo pool=" + PlayerInventory.Ammo);
    }

    // ---------- Cheats (hotkeys, or right-click this component's header in Play Mode) ----------
    // F1 = refill ammo   F2 = +100 tickets   F3 = wipe all save data   F4 = reload this level

    [ContextMenu("Cheat: Refill Ammo")]
    private void RefillAmmo()
    {
        PlayerInventory.SetAmmo(PlayerInventory.MaxAmmo);
        if (player != null) player.bulletCount = PlayerInventory.MaxAmmo; // the level keeps its own copy
        Log("Ammo refilled to " + PlayerInventory.MaxAmmo);
    }

    [ContextMenu("Cheat: Add 100 Tickets")]
    private void AddTickets()
    {
        PlayerInventory.AddTickets(100);
        Log("Added 100 tickets, now " + PlayerInventory.Tickets);
    }

    [ContextMenu("Cheat: Reset All Save Data")]
    private void ResetSave()
    {
        PlayerInventory.ResetAll(); // tickets, ammo and abilities
        Log("Save data wiped");
    }

    [ContextMenu("Cheat: Reload Level")]
    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CheckHotkeys()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.f1Key.wasPressedThisFrame) RefillAmmo();
        if (Keyboard.current.f2Key.wasPressedThisFrame) AddTickets();
        if (Keyboard.current.f3Key.wasPressedThisFrame) ResetSave();
        if (Keyboard.current.f4Key.wasPressedThisFrame) ReloadLevel();
    }

    private void Update()
    {
        CheckHotkeys();
        if (player == null) return;

        if (player.win != lastWin) { lastWin = player.win; Log("player.win became " + lastWin + " | " + State()); }
        if (player.lose != lastLose) { lastLose = player.lose; Log("player.lose became " + lastLose + " | " + State()); }

        if (paperCol != null && paperCol.enabled != lastPaperOn)
        {
            lastPaperOn = paperCol.enabled;
            Log("paper collider enabled became " + lastPaperOn + " | " + State());
        }

        if (gm != null && gm.fillImage != null)
        {
            int pct = Mathf.RoundToInt(gm.fillImage.fillAmount * 100f);
            if (pct != lastFillPercent)
            {
                lastFillPercent = pct;
                Log("progress bar " + pct + "%");
            }
        }

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) tappedThisFrame = true;
    }

    // LateUpdate so PlayerCharacter has already had its chance to spawn the hole this frame
    private void LateUpdate()
    {
        int holes = FindObjectsByType<BulletHole>(FindObjectsSortMode.None).Length;

        if (tappedThisFrame)
        {
            tappedThisFrame = false;
            Log("TAP -> " + (holes > lastHoles ? "hole spawned" : "NO NEW HOLE") + " | " + State());
        }

        lastHoles = holes;
    }

    private string State()
    {
        if (player == null) return "no player";

        string s = "win=" + player.win + " lose=" + player.lose
            + " bullets=" + player.bulletCount + " pool=" + PlayerInventory.Ammo
            + " paperOn=" + (paperCol != null && paperCol.enabled)
            + " TriangleArea(Find)=" + (GameObject.Find("TriangleArea") != null);

        if (gm != null && gm.isBossLevel && gm.currentBoss != null)
        {
            s += " bossTime=" + gm.currentBoss.timeRemaining.ToString("F1");
        }
        return s;
    }

    private void Log(string message)
    {
        Debug.Log("[Diag t=" + Time.time.ToString("F2") + "] " + message);
    }
}

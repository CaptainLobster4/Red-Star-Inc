using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Goes on an object in the Collection scene.
// Builds one slot per CollectibleData. Unlocked slots show the mask and can be tapped
// to read its story; locked slots show a silhouette (or lockedSprite) and "???".
//
// Unlock state comes from the CollectibleManager, so make sure a CollectibleManager
// object exists in this scene too (extras destroy themselves, so it's safe to add one
// here as well as in the first scene).
public class CollectionScreen : MonoBehaviour
{
    [Header("Collectibles (one entry per boss mask)")]
    [SerializeField] private CollectibleData[] collectibles;

    [Header("Grid")]
    [Tooltip("Empty object with a Grid Layout Group. Slots are created inside it.")]
    [SerializeField] private Transform slotContainer;
    [Tooltip("Prefab with a Button + Image on its root. A TMP text child is used for the name if present.")]
    [SerializeField] private Button slotPrefab;
    [Tooltip("Optional image for locked slots. If empty, locked slots show the icon as a black silhouette.")]
    [SerializeField] private Sprite lockedSprite;

    [Header("Story Panel")]
    [Tooltip("Starts hidden. Opens when an unlocked collectible is tapped.")]
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private Image storyIcon;
    [SerializeField] private TextMeshProUGUI storyTitle;
    [SerializeField] private TextMeshProUGUI storyBody;

    private void Start()
    {
        if (CollectibleManager.Instance == null)
        {
            Debug.LogWarning("CollectionScreen: no CollectibleManager found, so everything will show as locked. Add a CollectibleManager object to this scene.");
        }

        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }

        BuildSlots();
    }

    private void BuildSlots()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogWarning("CollectionScreen: Slot Container and Slot Prefab must be assigned in the Inspector.");
            return;
        }

        // Clear any placeholder slots left in the container from the editor
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(slotContainer.GetChild(i).gameObject);
        }

        if (collectibles == null) return;

        foreach (CollectibleData collectible in collectibles)
        {
            if (collectible == null) continue;

            CollectibleData captured = collectible;
            Button slot = Instantiate(slotPrefab, slotContainer);
            Image image = slot.GetComponent<Image>();
            TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            if (IsUnlocked(captured))
            {
                if (image != null)
                {
                    image.sprite = captured.icon;
                    image.color = Color.white;
                }
                if (label != null) label.text = captured.collectibleName;

                slot.interactable = true;
                slot.onClick.AddListener(() => ShowStory(captured));
            }
            else
            {
                if (image != null)
                {
                    if (lockedSprite != null)
                    {
                        image.sprite = lockedSprite;
                        image.color = Color.white;
                    }
                    else
                    {
                        image.sprite = captured.icon;
                        image.color = Color.black; // silhouette
                    }
                }
                if (label != null) label.text = "???";

                slot.interactable = false;
            }
        }
    }

    private bool IsUnlocked(CollectibleData collectible)
    {
        return CollectibleManager.Instance != null && CollectibleManager.Instance.IsUnlocked(collectible);
    }

    private void ShowStory(CollectibleData collectible)
    {
        if (storyPanel == null) return;

        storyPanel.SetActive(true);
        if (storyIcon != null) storyIcon.sprite = collectible.icon;
        if (storyTitle != null) storyTitle.text = collectible.collectibleName;
        if (storyBody != null) storyBody.text = collectible.storyText;
    }

    // Hook the story panel's Close button (or a full-screen button behind the text) to this
    public void CloseStory()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }
    }

    // Right-click the component header in Play Mode to test the unlocked look
    [ContextMenu("Debug: Unlock All")]
    private void DebugUnlockAll()
    {
        if (CollectibleManager.Instance == null || collectibles == null) return;

        foreach (CollectibleData collectible in collectibles)
        {
            CollectibleManager.Instance.Unlock(collectible);
        }
        BuildSlots();
    }
}
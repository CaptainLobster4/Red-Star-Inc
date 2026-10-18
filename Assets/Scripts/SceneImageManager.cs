using UnityEngine;
using UnityEngine.SceneManagement; // Required for changing scenes
using UnityEngine.UI;              // Required for handling UI elements

public class SceneImageManager : MonoBehaviour
{
    // A static variable retains its value across different scene changes
    public static bool shouldShowImage = false;

    [Header("Scene 1 References")]
    public GameObject imageToActivate;

    private void Start()
    {
        // When Scene 1 loads, check if the static flag is true
        if (imageToActivate != null)
        {
            imageToActivate.SetActive(shouldShowImage);
        }
    }

    // Call this function from your Button in Scene 2
    public void ClickButtonInScene2()
    {
        // Set the flag to true so Scene 1 knows to show the image
        shouldShowImage = true;

        // Load Scene 1 (Make sure the scene name matches exactly)
        SceneManager.LoadScene("Scene 1");
    }
}
using UnityEngine;

public class NewRun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Removes all saved preferences for a new campaign run
        PlayerPrefs.DeleteAll();

        // save new empty state
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

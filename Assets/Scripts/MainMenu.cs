using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ShopMenu()
    {
        SceneManager.LoadSceneAsync(3);
    }
    
    public void CollectionMenu()
    {
        SceneManager.LoadSceneAsync(4);
    }
    
}

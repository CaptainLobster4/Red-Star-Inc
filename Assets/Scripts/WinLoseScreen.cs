using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    public void PlayAgainButton() 
    {
        SceneManager.LoadSceneAsync(1);
    }
    
    public void CollectionsButton() 
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void ShopMenu()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
    
}

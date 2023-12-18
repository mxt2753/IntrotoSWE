using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject inventoryButton;
    [SerializeField] GameObject pauseButton;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    //THIS IS FOR THE PAUSE MENU FUCTIONALITY
    public void Pause()
    {
        if(pauseButton.activeSelf)
        {
            pauseMenu.SetActive(true);
            inventoryButton.SetActive(false);
            Time.timeScale=0f;  
        }
    }

    public void Resume()
    {
        if(pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            pauseButton.SetActive(true);
            inventoryButton.SetActive(true);
            Time.timeScale=1f; 
        }
    }

    public void Menu()
    {
        if(pauseMenu.activeSelf)
        {
            Time.timeScale=1f;
            SceneManager.LoadScene(0); //GOES BACK TO THE WORLD LEVEL MENU
        }
    }    

}

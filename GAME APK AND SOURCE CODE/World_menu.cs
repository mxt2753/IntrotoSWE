using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class WorldMenu : MonoBehaviour
{
    public Button world1Button;
    public Button world2Button;
    public Button world3Button;

    public Button backButton;

    void Start()
    {
        if(world1Button!=null)
            world1Button.onClick.AddListener(()=>LoadNextScene(1));
        if(world2Button!=null)
            world2Button.onClick.AddListener(()=>LoadNextScene(2));
        if(world3Button!=null)
            world3Button.onClick.AddListener(()=>LoadNextScene(3));
        if(backButton!=null)
            backButton.onClick.AddListener(()=>LoadMainMenu(0));
    }
    void LoadNextScene(int worldIndex)
    {
        string sceneName="world"+worldIndex;
        SceneManager.LoadScene("world"+worldIndex);
    }
    void LoadMainMenu(int startIndex)
    {
        SceneManager.LoadScene("MainStart");
    }
   

}

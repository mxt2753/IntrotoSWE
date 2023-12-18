using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class levels : MonoBehaviour
{
    private static List<Button> levelButtons;

    private static int unlockedLevel = 1;
    private static int currentLevel;
    private static string chosenLevel = "level1";

    public Button backButton;
    public Transform transitionPoint;

    private void Start()
    {
        levelButtons = new List<Button>();
        StoreButtons();

        foreach (Button button in levelButtons)
        {
            int level = int.Parse(button.name.Substring(button.name.Length - 1));
            button.onClick.AddListener(() => LoadScene(level));
            button.interactable = level <= unlockedLevel;
        }

        backButton.onClick.AddListener(() => LoadScene(0));
    }

    public void StoreButtons()
    {
        levelButtons.Add(GameObject.Find("L1").GetComponent<Button>());
        levelButtons.Add(GameObject.Find("L2").GetComponent<Button>());
        levelButtons.Add(GameObject.Find("L3").GetComponent<Button>());
    }

    public void LoadScene(int level) {
        if (level == 0)
        {
            SceneManager.LoadScene("WorldMenu");
        }
        else
        {
            if (level > unlockedLevel)
            {
                return;
            }
            currentLevel = level;
            StartCoroutine(LoadSceneWithTransition(level));
        }
    }

    public static IEnumerator LoadSceneWithTransition(int level)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Level1");
        operation.allowSceneActivation = false;
        chosenLevel = "level" + level;

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    public static int GetCurrentLevel()
    {
        return currentLevel;
    }

    public static void UnlockNextLevel()
    {
        if(unlockedLevel < 3)
        {
            // levelButtons.ElementAt(unlockedLevel).interactable = true;
            unlockedLevel += 1;
        }
    }

    public static string GetLevelName()
    {
        return chosenLevel;
    }
}

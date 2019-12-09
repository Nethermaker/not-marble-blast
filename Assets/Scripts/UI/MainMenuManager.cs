using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Controls the main menu
//Most UI elements are set and controlled in the inspector. You can set events to call functions, 
//  which is why most of these functions aren't called by the script itself
public class MainMenuManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject credits;
    public GameObject creditsText;

    private GameObject[] levelList;

    private GameObject selectedLevel;
    public Text levelDescription;

    private void Start()
    {
        levelList = GameObject.FindGameObjectsWithTag("Level");
        GetSelectedLevel();
        levelSelect.SetActive(false);
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(selectedLevel.GetComponent<LevelInfo>().levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActivateLevelSelect()
    {
        levelSelect.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ActivateMainMenu()
    {
        levelSelect.SetActive(false);
        creditsText.SetActive(false);
        credits.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ActivateCredits()
    {
        credits.SetActive(true);
        creditsText.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void GetSelectedLevel()
    {
        foreach (GameObject level in levelList)
        {
            if (level.GetComponent<Toggle>().isOn)
            {
                selectedLevel = level;
            }
        }

        levelDescription.text = selectedLevel.GetComponent<LevelInfo>().description;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Controls the in-game pause menu and end-of-level menu
public class LevelMenuManager : MonoBehaviour {

    public GameObject pauseMenu;
    public GameObject endMenu;

	public void ReturnToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(GameObject.FindGameObjectWithTag("Finish").GetComponent<Finish>().nextLevel);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LevelEnd()
    {
        endMenu.SetActive(true);
        Text finalTime = GameObject.Find("Final Time Text").GetComponent<Text>();
        finalTime.text = GameObject.Find("Timer").GetComponent<Text>().text;
    }
}

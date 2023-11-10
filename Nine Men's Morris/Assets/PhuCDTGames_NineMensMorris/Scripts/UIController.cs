using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    //Game Move Selection Parameters
    public GameObject gamemodePanel;

    //Endgame Parameters
    public GameObject endgamePanel;
    public TextMeshProUGUI endgameText;

    private void Start()
    {
        gamemodePanel.SetActive(true);
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnClick_SelectGameModePlayer()
    {
        Table.Instance.isBotPlaying = false;
        gamemodePanel.SetActive(false);
    }    
    public void OnClick_SelectGameModeBot()
    {
        Table.Instance.isBotPlaying = true;
        gamemodePanel.SetActive(false);
    }

    public void SetEndgameText(string text)
    {
        endgamePanel.SetActive(true);
        endgameText.text = text;
    }

    public void OnClick_Restart()
    {
        endgamePanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
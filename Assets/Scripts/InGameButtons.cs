using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameButtons : MonoBehaviour
{
    [SerializeField] GameObject PausePanel;
    [SerializeField] GameObject SummaryPanel;
    [SerializeField] AdManagerInterstitial AdManager;
    enum SelectedButton
    {
        QuitToMenu,
        NextLevel,
        RestartLevel
    }
    SelectedButton selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPauseMenu()
    {
        PausePanel.SetActive(true);
    }

    public void Resume()
    {
        PausePanel.SetActive(false);
    }

    public void Restart()
    {
        selectedButton = SelectedButton.RestartLevel;
        if (AdManager.ShowAd())
        {
            return;
        }
        
        SceneManager.LoadScene("InGame");
    }

    public void Quit()
    {
        selectedButton = SelectedButton.QuitToMenu;
        if (AdManager.ShowAd())
        {
            return;
        }

        SceneManager.LoadScene("Menu");
    }

    public void OnAdClosedEvent() {
        switch (selectedButton)
        {
            case SelectedButton.RestartLevel:
                SceneManager.LoadScene("InGame");
                break;
            case SelectedButton.QuitToMenu:
                SceneManager.LoadScene("Menu");
                break;
        }
    }
}

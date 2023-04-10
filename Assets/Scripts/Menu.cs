using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] MessageDialog messageDialog;
    [SerializeField] QuestionDialog questionDialog;
    [SerializeField] LanguageManager languageManager;
    [SerializeField] GameObject difficultButtonPack;

    [SerializeField] Player player;

    [SerializeField] InputDialog inputDialog;
    [SerializeField] Button rewardBtn;
    [SerializeField] Tutorial tutorial;

    GameObject actualMenu;
    // Start is called before the first frame update
    void Start()
    {
        actualMenu = GameObject.Find("Canvas/EntryMenu");
        Button[] difficultButtons = difficultButtonPack.GetComponentsInChildren<Button>();
        
        for (int i = 1; i < player.DifficultAvaiable(); i++)
        {
            difficultButtons[i].interactable = true;
        }

        if ((DateTime.Now - player.LastRewarded).TotalHours < 2)
        {
            DisableRewardButton();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!rewardBtn.interactable && (int)(DateTime.Now - player.LastRewarded).TotalHours >= 2)
        {
            rewardBtn.interactable = true;
        }
    }

    public void StartGame(int difficult)
    {
        Global.difficult = (Difficult)difficult;
        SceneManager.LoadScene("InGame");
    }

    public void StartLocalMultiGame()
    {
        Global.difficult = Difficult.Human;
        if (player.PlayerNames[1] == "")
        {
            InputAddName2AndStartGame();
            return;
        }

        SceneManager.LoadScene("InGame");
    }

    public void ShowMenu(GameObject menu)
    {
        actualMenu.SetActive(false);
        menu.SetActive(true);
        actualMenu = menu;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RewardXP()
    {
        player.XP += 50;
        if (player.IsNextLevelReached())
        {
            player.Level++;
        }

        player.LastRewarded = DateTime.Now;
        player.Save();

        rewardBtn.interactable = false;
        messageDialog.Popup(string.Format(languageManager.GetTextByValue("Congratulations"), (int)(50 * (1+ (0.25f) * player.Level))));
        //messageDialog.Popup($"Congratualtions! You rewarded {(int)(50 * (1+ (0.25f) * player.Level))} XP");
    }

    public void DisableRewardButton()
    {
        rewardBtn.interactable = false;
    }

    public void MessageUnimplemented()
    {
        messageDialog.Popup(languageManager.GetTextByValue("Unimplemented"));
    }

    public void RequestForFirstRun()
    {
        if (player.PlayerNames[0] == "")
        {
            InputAddName1();
        }
        else if (player.IsFirstPlay)
        {
            questionDialog.Popup(languageManager.GetTextByValue("_TutorialRequest"), tutorial.StartVideo, null);
            player.IsFirstPlay = false;
        }
    }

    public void InputAddName1()
    {
        inputDialog.Popup(languageManager.GetTextByValue("GetName1"), (string name) => { SetName1(name); ShowTutorialQuestion(); }, player.PlayerNames[0]);
    }

    public void InputAddName2()
    {
        inputDialog.Popup(languageManager.GetTextByValue("GetName2"), SetName2, player.PlayerNames[1]);
    }

    public void InputAddName2AndStartGame()
    {
        inputDialog.Popup(languageManager.GetTextByValue("GetName2"), SetName2AndStartGame);
    }

    private void SetName1(string name)
    {
        player.SetPlayerName(0, name);
    }

    private void SetName2(string name)
    {
        player.SetPlayerName(1, name);
    }

    private void SetName2AndStartGame(string name)
    {
        player.SetPlayerName(1, name);
        StartLocalMultiGame();
    }

    private void ShowTutorialQuestion()
    {
        if (player.IsFirstPlay)
        {
            questionDialog.Popup(languageManager.GetTextByValue("_TutorialRequest"), tutorial.StartVideo, null);
        }
    }
}

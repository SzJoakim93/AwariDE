using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public enum TurnBehaviour
    {
        Neutral = 0, Bad = 1, Good = 2
    }
    string[] waitingMessages;
    string[] turnOnGoodMessages;
    string[] turnOnBadMessages;
    string[] loseMessages;
    string[] turnAgainMessages;
    string[] turnMultiMessages;
    readonly Color[] playerColor = new Color[] { new Color(0.8588f, 0.4509f, 0.0235f),  new Color(0.1509f, 0.8098f, 0.1588f) };
    [SerializeField] LanguageManager languageManager;
    [SerializeField] Text messageTxt;
    [SerializeField] Player player;

    void Start()
    {
        waitingMessages = languageManager.GetTextByValueRange("WaitingMessages").ToArray();
        turnOnGoodMessages = languageManager.GetTextByValueRange("TurnOnGoodMessages").ToArray();
        turnOnBadMessages = languageManager.GetTextByValueRange("TurnOnBadMessages").ToArray();
        loseMessages = languageManager.GetTextByValueRange("LoseMessages").ToArray();
        turnAgainMessages = languageManager.GetTextByValueRange("TurnAgainMessages").ToArray();
        turnMultiMessages = languageManager.GetTextByValueRange("TurnMessages").ToArray();
    }

    public void showOnTurnMulti(int playerIndex)
    {
        int x = Random.Range(0, 6);
        messageTxt.text = string.Format(turnMultiMessages[x], player.PlayerNames[playerIndex]);
        messageTxt.color = playerColor[playerIndex];

    }

    public void showOnTurn(GameEvents.PlayerMode playMode, TurnBehaviour turnBehaviour)
    {
        if (playMode == GameEvents.PlayerMode.AI)
        {
            showOnTurnAI(turnBehaviour);
        }
        else
        {
            messageTxt.text = string.Format(languageManager.GetTextByValue("TurnMessages1"), player.PlayerNames[0]);
        }
    }

    public void showOnTurnAgain(GameEvents.PlayerMode playMode)
    {
        if (playMode == GameEvents.PlayerMode.AI)
        {
            messageTxt.text = languageManager.GetTextByValue("TurnAgainAIMessages");
        }
        else
        {
            messageTxt.text = turnAgainMessages[Random.Range(0, turnAgainMessages.Length)];
        }
    }

    void showOnTurnAI(TurnBehaviour turnBehaviour)
    {
        switch (turnBehaviour)
        {
            case TurnBehaviour.Neutral:
                messageTxt.text = languageManager.GetTextByValue("TurnOnNeutralMessages");
                break;
            case TurnBehaviour.Bad:
                messageTxt.text = turnOnBadMessages[Random.Range(0, turnOnBadMessages.Length)];
                break;
            case TurnBehaviour.Good:
                messageTxt.text = turnOnGoodMessages[Random.Range(0, turnOnGoodMessages.Length)];
                break;
        }
    }

    public void showOnWaiting()
    {
        messageTxt.text = waitingMessages[Random.Range(0, waitingMessages.Length)];
    }

    public void showOnGameOverMulti(int winner)
    {
        if (winner == -1)
        {
            messageTxt.text = languageManager.GetTextByValue("DrawMulti");
        }
        else
        {
            messageTxt.text = string.Format(languageManager.GetTextByValue("winMulti"), player.PlayerNames[winner]);
            messageTxt.color = playerColor[winner];
        }
    }

    public void showOnGameOver(int winner, GameEvents.PlayerMode playMode)
    {
        if (winner == -1)
        {
            messageTxt.text = languageManager.GetTextByValue("Draw");
        }
        else
        {
            if (playMode == GameEvents.PlayerMode.AI)
            {
                messageTxt.text = loseMessages[Random.Range(0, loseMessages.Length)];
            }
            else
            {
                messageTxt.text = languageManager.GetTextByValue("Win");
            }
        }
    }

    public void ShowOnBegin(int turningPlayer, GameEvents.PlayerMode playerMode)
    {
        if (playerMode != GameEvents.PlayerMode.AI)
        {
            messageTxt.text = string.Format(languageManager.GetTextByValue("Begin"), player.PlayerNames[turningPlayer]);
        }
        else
        {
            messageTxt.text = languageManager.GetTextByValue("BeginAI");
        }
    }
}

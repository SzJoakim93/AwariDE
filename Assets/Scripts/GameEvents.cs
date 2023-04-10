using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour

{
    public enum PlayerMode
    {
        Human = 0, AI = 1, Remote = 2
    };

    public enum PickingType
    {
        PickingByStep = 0, PickingAll = 1
    };

    public enum PickingState
    {
        Idle = 0, Pushing = 1, Poping = 2, Ending = 3
    };

    [SerializeField] PickingAnimation pickingAnimation;
    [SerializeField] SummaryAnimation summaryAnimation;
    [SerializeField] MessageManager messageManager;
    [SerializeField] GameObject [] playerLights;
    [SerializeField] Player player;
    [SerializeField] QuestionDialog questionDialog;
    [SerializeField] InGameButtons ingameButtons;
    int [,] pointsInTrays;
    PlayerMode [] playerModes;

    int turningPlayer = 0;

    int pointOutSide;
    int playerIndex;
    int trayIndex;
    int lastPlayer;
    int lastTray;
    bool isBonusTurnAllow = true;
    bool isGameOver = false;
    float waitingTime = 0.0f;
    float waitingForSummaryTime = 0.0f;
    float waitiongForQuestionDialog = 0.0f;
    bool isWin = false;
    int pickedDotsOnPickMany = 0;

    PickingState pickingState = PickingState.Idle;
    PickingType pickingType = PickingType.PickingByStep;
    AIManager aIManager;

    // Start is called before the first frame update
    void Start()
    {
        pointsInTrays = new int[2,7]{{6, 6, 6, 6, 6, 6, 0}, {6, 6, 6, 6, 6, 6, 0}};
        pointOutSide = 0;
        if (Global.difficult != Difficult.Human)
        {
            playerModes = new PlayerMode[2]{PlayerMode.Human, PlayerMode.AI};
            aIManager = new AIManager(pointsInTrays);
        }
        else
        {
            playerModes = new PlayerMode[2]{PlayerMode.Human, PlayerMode.Human};
        }
        
        if (player.IsBeginAtLastRound)
        {
            turningPlayer = 1;
        }

        player.IsBeginAtLastRound = !player.IsBeginAtLastRound;
        messageManager.ShowOnBegin(turningPlayer, playerModes[turningPlayer]);
        
        //summaryAnimation.StartAnimation(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (pickingState == PickingState.Ending  && pickingAnimation.IsPushingFinished)
        {
            if (trayIndex != 6 && trayIndex != 13)
            {
                if (pointsInTrays[playerIndex, trayIndex] == 1)
                {
                    popFromPlate(playerIndex, trayIndex);
                    pickingAnimation.PopFromPlate(playerIndex*7+trayIndex);
                    pickingState = PickingState.Poping;
                    pickingType = PickingType.PickingAll;
                    pickedDotsOnPickMany = pointsInTrays[playerIndex == 0 ? 1 : 0, 5 - trayIndex];
                }
                else if (pickingType == PickingType.PickingAll && pointsInTrays[playerIndex == 0 ? 1 : 0, 5 - trayIndex] > 0)
                {
                    popFromPlate(playerIndex == 0 ? 1 : 0, 5- trayIndex);
                    pickingAnimation.PopFromPlate((playerIndex == 0 ? 1 : 0)*7+(5-trayIndex));
                    pickingState = PickingState.Poping;
                }
                else
                {   
                    pickingState = PickingState.Idle;
                    switchPlayer();
                }
            }
            else
            {
                if (!isBonusTurnAllow)
                {
                    switchPlayer();
                }
                else
                {
                    isBonusTurnAllow = false;
                    messageManager.showOnTurnAgain(playerModes[turningPlayer]);
                    waitingTime = Time.timeSinceLevelLoad;
                }
                
                pickingState = PickingState.Idle;
            }
            
        }
        else if (pickingState == PickingState.Poping)
        {
            if (pickingAnimation.IsPopingFinished)
            {
                pickingState = PickingState.Pushing;
            }
        }
        else if (pickingState == PickingState.Pushing)
        {
            if (pickingAnimation.IsPushingFinished)
            {
                if (pickingType == PickingType.PickingByStep)
                {
                    pushToPlate(playerIndex, trayIndex);
                    pickingAnimation.PushToPlate(playerIndex*7+trayIndex);

                    if (pointOutSide <= 0)
                    {
                        pickingState = PickingState.Ending;
                        return;
                    }

                    trayIndex++;

                    if (trayIndex > 6)
                    {
                        playerIndex = playerIndex == 0 ? 1 : 0;
                        trayIndex = 0;
                    }
                }
                else
                {
                    pushToPlate(turningPlayer, 6);
                    pickingAnimation.PushToPlate(turningPlayer*7+6);

                    if (pointOutSide <= 0)
                    {
                        pickingState = PickingState.Ending;
                        return;
                    }
                }
            }
        }
        else if (pickingState == PickingState.Idle && !isGameOver)
        {
            if (pointsInTrays[0, 6] > 36 || pointsInTrays[1, 6] > 36 || (pointsInTrays[0, 6] == 36 && pointsInTrays[1, 6] == 36) || isAllPlateEmpty())
            {
                isGameOver = true;
                onGameOver();
            }
            else if (playerModes[turningPlayer] == PlayerMode.AI && Time.timeSinceLevelLoad - waitingTime > 2.0f)
            {
                int x = aIManager.DetermineSelectedPlate(turningPlayer);
                SelectPlate(x+6);
            }

            if (Time.timeSinceLevelLoad - waitingTime > 15.0f)
            {
                messageManager.showOnWaiting();
                waitingTime = Time.timeSinceLevelLoad;
            }
        }

        if (waitingForSummaryTime > 0.0f && Time.timeSinceLevelLoad - waitingForSummaryTime > 5.0f)
        {
            summaryAnimation.StartAnimation(calculateCollectedXP());
            waitingForSummaryTime = 0.0f;
        }

        if (waitiongForQuestionDialog > 0.0f && Time.timeSinceLevelLoad - waitiongForQuestionDialog > 5.0f)
        {
            questionDialog.Popup("Do you want to play again?", ingameButtons.Restart, ingameButtons.Quit);
        }
    }
    public void SelectPlate(int i)
    {
        if (i > 5 && turningPlayer == 0 || i < 6 && turningPlayer == 1)
        {
            return;
        }

        if (i > 5) //player2
        {
            i -= 6;
        }

        if (pointsInTrays[turningPlayer, i] == 0
            || pickingState != PickingState.Idle
            || isGameOver
            /*|| playerModes[turningPlayer] != PlayerMode.Human*/)
        {
            return;
        }

        popFromPlate(turningPlayer, i);
        pickingAnimation.PopFromPlate(turningPlayer*7+i);
        pickingState = PickingState.Poping;
        pickingType = PickingType.PickingByStep;
        trayIndex = i + 1;
        playerIndex = turningPlayer;
    }

    void pushToPlate(int i, int j)
    {
        pointsInTrays[i, j]++;
        pointOutSide--;
    }

    void popFromPlate(int i, int j)
    {
        pointOutSide = pointsInTrays[i, j];
        pointsInTrays[i, j] = 0;
    }

    void switchPlayer()
    {
        turningPlayer = turningPlayer == 0 ? 1 : 0;
        isBonusTurnAllow = true;
        waitingTime = Time.timeSinceLevelLoad;
        playerLights[turningPlayer].SetActive(true);
        playerLights[turningPlayer == 0 ? 1 : 0].SetActive(false);
        if (Global.difficult == Difficult.Human)
        {
            messageManager.showOnTurnMulti(turningPlayer);
        }
        else
        {
            messageManager.showOnTurn(playerModes[turningPlayer], DetermineTurnBehaviour());
        }
        pickedDotsOnPickMany = 0;
    }

    private MessageManager.TurnBehaviour DetermineTurnBehaviour()
    {
        if (pickedDotsOnPickMany > 5)
        {
            return MessageManager.TurnBehaviour.Good;
        }
        else if (pointsInTrays[turningPlayer, 6] > pointsInTrays[turningPlayer == 0 ? 1: 0, 6])
        {
            return MessageManager.TurnBehaviour.Bad;
        }

        return MessageManager.TurnBehaviour.Neutral;
    }

    void onGameOver()
    {
        int winner = pointsInTrays[0, 6] > 36 ? 0 : pointsInTrays[1, 6] > 36 ? 1 : -1;
        isWin = winner == 0;
        if (Global.difficult == Difficult.Human)
        {
            messageManager.showOnGameOver(winner, playerModes[winner != -1 ? winner : 0]);
            waitiongForQuestionDialog = Time.timeSinceLevelLoad;
        }
        else
        {
            messageManager.showOnGameOver(winner, playerModes[winner != -1 ? winner : 0]);
            waitingForSummaryTime = Time.timeSinceLevelLoad;

            if (isWin && (int)Global.difficult > player.DifficultCompleted)
            {
                player.DifficultCompleted = (int)Global.difficult;
            }
        }
        
    }

    bool isAllPlateEmpty()
    {
        for (int i = 0; i < 6; i++)
        {
            if (pointsInTrays[turningPlayer, i] != 0)
            {
                return false;
            }
        }
        return true;
    }

    int calculateCollectedXP()
    {
        int pointsReward = pointsInTrays[0, 6];
        int winReward = isWin ? 100 : 0;
        float difficultMultipler = 1 + 0.25f * ((int)Global.difficult);
        return (int)((winReward + pointsReward)*difficultMultipler*Global.dailyXpMultipler);
    }
}

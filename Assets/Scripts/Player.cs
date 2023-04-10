using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region SerializeFields
    [SerializeField] ProgressBar progressBar;
    [SerializeField] Text levelTitle;
    [SerializeField] Text xpTitle;
    #endregion

    #region Properties
    private int xP;
    public int XP
    {
        get { return xP; }
        set
        {
            xP = value;
            progressBar.OnVariableChange((float)(value-levelTressHolds[level-1]) / (float)(levelTressHolds[level]-levelTressHolds[level-1]));
            xpTitle.text = value.ToString() + " XP";
        }
    }

    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            levelTitle.text = value.ToString();
        }
    }

    private int difficultCompleted;
    public int DifficultCompleted
    {
        get { return difficultCompleted; }
        set
        {
            difficultCompleted = value;
            PlayerPrefs.SetInt("PlayerDifficultCompleted", value);
        }
    }

    private DateTime lastRewarded;
    public DateTime LastRewarded
    {
        get { return lastRewarded; }
        set
        {
            lastRewarded = value;
            SetDateTimeToPlayerPref("LastRewarded", lastRewarded);
        }
    }
    

    private string[] playerNames = new string[] { "", "" };
    public string[] PlayerNames
    {
        get { return playerNames; }
    }

    private bool isBeginAtLastRound;
    public bool IsBeginAtLastRound
    {
        get { return isBeginAtLastRound; }
        set
        {
            isBeginAtLastRound = value;
            PlayerPrefs.SetInt("IsBeginAtLastRound", isBeginAtLastRound ? 1 : 0);
        }
    }

    private bool isFirstPlay;
    public bool IsFirstPlay
    {
        get { return isFirstPlay; }
        set
        {
            isFirstPlay = value;
            PlayerPrefs.SetInt("IsFirstPlay", isFirstPlay ? 1 : 0);
        }
    }
    
    #endregion

    #region Init
    void Awake()
    {
        Level = PlayerPrefs.GetInt("PlayerLevel", 1);
        XP = PlayerPrefs.GetInt("PlayerXP", 0);
        difficultCompleted = PlayerPrefs.GetInt("PlayerDifficultCompleted", -1);
        playerNames[0] = PlayerPrefs.GetString("Player1Name", "");
        playerNames[1] = PlayerPrefs.GetString("Player2Name", "");
        lastRewarded = GetDateTimeFromPlayerPref("LastRewarded");
        isBeginAtLastRound = PlayerPrefs.GetInt("IsBeginAtLastRound", 0) == 1;
        isFirstPlay = PlayerPrefs.GetInt("IsFirstPlay", 1) == 1;
    }
    #endregion

    #region PublicMethods

    public void SetPlayerName(int i, string value)
    {
        playerNames[i] = value;
        PlayerPrefs.SetString($"Player{i+1}Name", value);
    }

    public bool IsNextLevelReached()
    {
        return xP > levelTressHolds[level];
    }

    public int DifficultAvaiable()
    {
        return difficultCompleted + 2 > level ? level : (difficultCompleted + 2);
    }

    public bool IsDifficultLevelRequirementsMeeted()
    {
        int diffAvaiable = DifficultAvaiable();
        return diffAvaiable == 7 || difficultCompleted >= diffAvaiable;
    }

    public bool IsLevelRequirementsMeeted()
    {
        int diffAvaiable = DifficultAvaiable();
        return diffAvaiable == 7 || level > diffAvaiable;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerXP", xP);
    }
    #endregion

    #region Private
    private readonly int[] levelTressHolds = {0, 100, 500, 1500, 3000, 6000, 15000 };

    private DateTime GetDateTimeFromPlayerPref(string key)
    {
        return DateTime.ParseExact(PlayerPrefs.GetString(key, "2023-01-01 00:00:00,000") , "yyyy-MM-dd HH:mm:ss,fff",
                                              System.Globalization.CultureInfo.InvariantCulture);
    }

    private void SetDateTimeToPlayerPref(string key, DateTime value)
    {
        PlayerPrefs.SetString(key, value.ToString("yyyy-MM-dd HH:mm:ss,fff"));
    }

    #endregion
}
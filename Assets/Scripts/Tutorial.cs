using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Video;

public class Tutorial : MonoBehaviour {
    [SerializeField] Text tutorialText;
    [SerializeField] VideoPlayer[] videos;
    [SerializeField] GameObject picture;
    [SerializeField] LanguageManager languageManager;
    [SerializeField] GameObject questionDialog;
    List<string> messages = null;
    float startTime = 0.0f;
    int currentVideo = 0;
    int currentMessage = 0;
    readonly int [] startingMessages = { 0, 6, 12, 19, 26 };
    readonly float [,] frameSwitchingTimes =
    {
        { 4.0f, 11.0f, 16.0f, 21.0f, 26.0f, 31.0f , -1.0f, -1.0f },
        { 10.0f, 14.0f, 22.0f, 32.0f, 44.0f, 55.0f, -1.0f, -1.0f },
        { 5.0f, 12.0f, 22.0f, 38.0f, 44.0f, 50.0f , 60.0f, -1.0f },
        { 4.0f, 8.0f, 15.0f, 22.0f, 29.0f , 36.0f, 41.0f, 46.0f },
        { 5.0f, 10.0f, 15.0f, 20.0f, -1.0f , -1.0f, -1.0f, -1.0f },
    };
    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
        if (Time.timeSinceLevelLoad - startTime > frameSwitchingTimes[currentVideo, currentMessage] && frameSwitchingTimes[currentVideo, currentMessage] != -1)
        {
            currentMessage++;
            if (currentMessage >= frameSwitchingTimes.GetLength(1) - 1 || frameSwitchingTimes[currentVideo, currentMessage] == -1)
            {
                if (currentVideo < videos.Length)
                {
                    videos[currentVideo].Stop();
                }
                tutorialText.gameObject.SetActive(false);

                if (currentVideo >= frameSwitchingTimes.GetLength(0) - 1)
                {
                    gameObject.SetActive(false);
                    currentVideo = 0;
                }
                else
                {
                    questionDialog.SetActive(true);
                }
            }
            else
            {
                tutorialText.text = messages[startingMessages[currentVideo]+currentMessage];
            }
            
        }
    }

    public void StartVideo()
    {
        if (messages == null)
        {
            messages = languageManager.GetTextByValueRange("Tutorial").ToList();
        }
        startTime = Time.timeSinceLevelLoad;
        currentMessage = 0;
        ActivateVideo(currentVideo);
        if (currentVideo < videos.Length)
        {
            videos[currentVideo].Play();
        }
        tutorialText.gameObject.SetActive(true);
        tutorialText.text = messages[startingMessages[currentVideo]+currentMessage];
        questionDialog.SetActive(false);
        gameObject.SetActive(true);
    }

    public void StartNextVideo()
    {
        currentVideo++;
        StartVideo();
    }

    void ActivateVideo(int x)
    {
        if (x >= videos.Length)
        {
            picture.SetActive(true);
        }

        for (int i = 0; i < videos.Length; i++)
        {
            videos[i].gameObject.SetActive(i == x);
        }
    }
}
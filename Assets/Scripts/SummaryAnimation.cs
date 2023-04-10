using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryAnimation : MonoBehaviour
{
    /*enum AnimationState
    {
        Start = 0, Counting = 1, End = 2
    }*/

    [SerializeField] Player player;
    [SerializeField] GameObject buttons;
    [SerializeField] AudioSource pointCountSound;
    [SerializeField] Button rewardBtn;
    [SerializeField] GameObject spark;

    bool isStarted = false;
    float timeOnstart;
    float delay = 0.0f;
    float delayDuration = 0.0f;
    int collectedXp;
    int collectedXpPrev;
    // Start is called before the first frame update
    void Start()
    {
        timeOnstart = 0.0f;
        pointCountSound.volume = Settings.SoundVolume;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            if (Time.timeSinceLevelLoad - timeOnstart > 1.0f && collectedXp > 0)
            {
                if (Time.timeSinceLevelLoad - delay > delayDuration)
                {
                    if (player.IsNextLevelReached())
                    {
                        player.Level++;
                        spark.SetActive(true);
                        timeOnstart = Time.timeSinceLevelLoad;
                        pointCountSound.Stop();
                    }

                    player.XP++;
                    collectedXp--;

                    delay = Time.timeSinceLevelLoad;

                    if (!pointCountSound.isPlaying)
                    {
                        pointCountSound.Play();
                    }
                }
            }
            else if (collectedXp <= 0)
            {
                isStarted = false;
                buttons.SetActive(true);
                pointCountSound.Stop();
                player.Save();
            }
        }
    }

    public void StartAnimation(int collectedXp)
    {
        gameObject.SetActive(true);
        isStarted = true;
        timeOnstart = Time.timeSinceLevelLoad;
        this.collectedXp = this.collectedXpPrev = collectedXp;
        delayDuration = 2.5f / (float)collectedXp;
    }

    public void DoubleXP()
    {
        StartAnimation(collectedXpPrev);
        DisableRewardBtn();
    }

    public void DisableRewardBtn()
    {
        rewardBtn.interactable = false;
    }
}

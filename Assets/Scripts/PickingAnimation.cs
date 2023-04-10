using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickingAnimation : MonoBehaviour
{
    [SerializeField] Transform dot;
    [SerializeField] List<Transform> plates;
    [SerializeField] List<Text> dotsQtyTexts;
    [SerializeField] AudioSource dingSound;
    [SerializeField] AudioSource pickingSound;
    private bool isPushingFinished = true;
    public bool IsPushingFinished
    {
        get { return isPushingFinished; }
    }
    private bool isPopingFinished = true;
    public bool IsPopingFinished
    {
        get { return isPopingFinished; }
    }
    
    List<Transform> dots;
    List<Transform> dotsOutSide;
    Transform dotToPush;
    Vector2 dotDestinationCoord;
    float animationstart = 0.0f;
    int dotsCountInPlate = 0;
    float pushSpeed;

    const float REFERENCE_SCREEN_WIDTH = 1280.0f;
    const float REFERENCE_SCREEN_HEIGHT = 720.0f;
    const float REFERENCE_PUSH_SPEED = -150.0f;

    public enum PickingState
    {
        Idle = 0, Pushing = 1, Poping = 2
    };

    PickingState pickingState = PickingState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        float x = Screen.currentResolution.width / REFERENCE_SCREEN_WIDTH;
        float y = Screen.currentResolution.height / REFERENCE_SCREEN_HEIGHT;
        dot.localScale = new Vector3(x ,y, dot.localScale.z);
        pushSpeed = (Screen.currentResolution.width / REFERENCE_SCREEN_WIDTH) * (REFERENCE_PUSH_SPEED);

        dingSound.volume = Settings.SoundVolume;
        pickingSound.volume = Settings.SoundVolume;

        dots = new List<Transform>();
        dotsOutSide = new List<Transform>();

        for (int i = 0; i < 72; i++)
        {
            Transform newDot = Instantiate<Transform>(dot, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            dots.Add(newDot);
        }
        
        int k = 0;
        for (int i = 0; i < 14; i++)
        {
            if (i == 6 || i == 13)
            {
                continue;
            }

            for (int j = 0; j < 6; j++)
            {
                dots[k].gameObject.SetActive(true);
                dots[k].SetParent(plates[i]);
                dots[k].localPosition = new Vector2(-42.0f + (j%6)*15.5f, 85.0f - (j/6)*15.5f);
                k++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (pickingState)
        {
            case PickingState.Poping:
                popAnimation();
                break;
            case PickingState.Pushing:
                pushAnimation();
                break;
            default:
                break;
        }
    }

    void popAnimation()
    {
        for (int i = 0; i < dotsOutSide.Count; i++)
        {
            dotsOutSide[i].Translate(0.0f, 400.0f * Time.deltaTime, 0.0f);
        }

        if (Time.timeSinceLevelLoad - animationstart > 0.25f)
        {
            popingEnd();
        }
    }

    void pushAnimation()
    {
        dotToPush.Translate(0.0f, pushSpeed * Time.deltaTime, 0.0f);

        if (dotToPush.localPosition.y < dotDestinationCoord.y)
        {
            dotToPush.localPosition = new Vector2(dotDestinationCoord.x, dotDestinationCoord.y);
            pickingState = PickingState.Idle;
            isPushingFinished = true;
            dingSound.Play();
        }
    }

    void popingEnd()
    {
        pickingState = PickingState.Idle;
        isPopingFinished = true;

        for (int j = 0; j < dotsOutSide.Count; j++)
        {
            dotsOutSide[j].gameObject.SetActive(false);
        }
    }

    public void PushToPlate(int actualPlate)
    {
        pickingState = PickingState.Pushing;
        isPushingFinished = false;
        dotToPush = dotsOutSide[dotsOutSide.Count-1];
        dotsOutSide.RemoveAt(dotsOutSide.Count-1);
        dotToPush.gameObject.SetActive(true);
        dotsCountInPlate = plates[actualPlate].GetComponentsInChildren<Transform>().Length-1;
        dotToPush.SetParent(plates[actualPlate]);
        dotDestinationCoord = getDestinationCoord(actualPlate, dotsCountInPlate);
        dotToPush.localPosition = new Vector2(dotDestinationCoord.x, dotDestinationCoord.y + 50.0f);
        dotsQtyTexts[actualPlate].text = (dotsCountInPlate+1).ToString();
    }

    public void PopFromPlate(int actualPlate)
    {
        pickingState = PickingState.Poping;
        isPopingFinished = false;
        animationstart = Time.timeSinceLevelLoad;
        Transform[] dots = plates[actualPlate].GetComponentsInChildren<Transform>();
        dotsOutSide.Clear();
        for (int k = 1; k < dots.Length; k++)
        {
            dotsOutSide.Add(dots[k]);
        }

        dotsQtyTexts[actualPlate].text = "0";
        pickingSound.Play();
    }

    Vector2 getDestinationCoord(int actualPlate, int dotsCountInPlate)
    {
        if (actualPlate == 6 || actualPlate == 13)
        {
            return new Vector2(-49.0f + (dotsCountInPlate%6)*15.5f, 200.0f - (dotsCountInPlate/6)*15.5f);
        }
        else
        {
            return new Vector2(-42.0f + (dotsCountInPlate%6)*15.5f, 85.0f - (dotsCountInPlate/6)*15.5f);
        }
    }
}

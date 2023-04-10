using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestionDialog : MonoBehaviour
{
    [SerializeField]
    Text messageTxt;

    Action callBackYes;
    Action callBackNo;

    public void Popup(string message, Action callBackYes, Action callBackNo)
    {
        gameObject.SetActive(true);
        this.messageTxt.text = message;
        this.callBackYes = callBackYes;
        this.callBackNo = callBackNo;
    }

    public void Yes()
    {
        callBackYes();
        gameObject.SetActive(false);
    }

    public void No()
    {
        if (callBackNo != null)
        {
            callBackNo();
        }
        gameObject.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessageDialog : MonoBehaviour
{
    [SerializeField]
    Text messageTxt;

    public void Popup(string message)
    {
        gameObject.SetActive(true);
        this.messageTxt.text = message;
    }

    public void Ok()
    {
        gameObject.SetActive(false);
    }
}
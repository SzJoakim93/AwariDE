using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputDialog : MonoBehaviour
{
    [SerializeField]
    EventSystem EventSystem;

    [SerializeField]
    Text messageTxt;
    [SerializeField]
    InputField inputField;
    Action<string> submit;

    public void Popup(string message, Action<string> submit, string defaultVaue = "")
    {
        gameObject.SetActive(true);
        this.messageTxt.text = message;
        EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        this.submit = submit;
        inputField.text = defaultVaue;
    }

    public void Submit()
    {
        if (inputField.text == "")
        {
            return;
        }

        submit(inputField.text);
        gameObject.SetActive(false);
    }

}

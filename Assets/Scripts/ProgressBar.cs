using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    RectTransform statusBar = null;
    float maxWidth;
    // Start is called before the first frame update
    void Start()
    {
        if (statusBar == null)
        {
            statusBar = gameObject.GetComponentsInChildren<RectTransform>(true)[1];
            maxWidth = statusBar.sizeDelta.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnVariableChange(float newVal)
    {
        //do whatever
        Start();
        statusBar.sizeDelta = new Vector2(maxWidth * newVal, statusBar.sizeDelta.y);
    }
}

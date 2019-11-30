using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZemeBar : MonoBehaviour
{
    public GameObject Bar;
    private Image FullZemeBar;
    private Image EmptyZemeBar;

    void Start()
    {
        FullZemeBar = Bar.GetComponent<Image>();
        EmptyZemeBar = this.GetComponent<Image>();
    }

    public void SetValue(float NewValue) {
        if (Bar) {
            if (NewValue < 0f) {
                NewValue = 0f;
            }
            FullZemeBar.fillAmount = NewValue;
        }
    }
    //public void SetValue(float NewValue) {
    //    if (Bar) {
    //        if (NewValue < 0f) {
    //            NewValue = 0f;
    //        }
    //        Vector2 localScale;
    //        localScale.x = NewValue;
    //        Transform bar = Bar.transform;
    //        bar.localScale = new Vector3(NewValue, 1f);
    //    }
    //}
}

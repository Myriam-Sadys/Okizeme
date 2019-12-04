using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    public GameObject Bar;
    private Image FullHealthBar;
    private Image EmptyHealthBar;

    void Start()
    {
        FullHealthBar = Bar.GetComponent<Image>();
        EmptyHealthBar = this.GetComponent<Image>();
    }

    public void SetValue(float NewValue) {
        if (Bar) {
            if (NewValue < 0f)
                NewValue = 0f;
            FullHealthBar.fillAmount = NewValue;
        }
    }

    public float GetValue() {
        float value_bar = 0f;
        if (Bar)
        {
            value_bar = FullHealthBar.fillAmount;
        }
        return value_bar;
    }
}

//public void SetValue(float NewValue)
//{
//    if (Bar)
//    {
//        if (Bar.transform.localScale.x <= 0f)
//        {
//            if (NewValue < 0f)
//                NewValue = 0f;
//            Vector2 localScale;
//            localScale.x = -NewValue;
//            Transform bar = Bar.transform;
//            bar.localScale = new Vector3(-NewValue, 1f);
//        }
//        else
//        {
//            Vector2 localScale;
//            localScale.x = NewValue;
//            Transform bar = Bar.transform;
//            bar.localScale = new Vector3(NewValue, 1f);
//        }
//    }
//}

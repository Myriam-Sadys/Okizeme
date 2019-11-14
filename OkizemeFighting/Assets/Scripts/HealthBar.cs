using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    public GameObject Bar;
    public Image FullBar;
    public Image EmptyBar;
    bool updateBar;
    float barValue;

    private void Start()
    {
        updateBar = false;
        barValue = 0;
    }

    void Update()
    {
        if (updateBar)
        {
            FullBar.fillAmount = barValue;
            updateBar = false;
        }
    }

    public void SetValue(float NewValue) {
        if (Bar) {
            if (NewValue < 0f) {
                NewValue = 0f;
            }
            barValue = NewValue;
            Vector2 localScale;
            localScale.x = NewValue;
            updateBar = true;
            Transform bar = Bar.transform;
            bar.localScale = new Vector3(NewValue, 1f);
        }
    }
}

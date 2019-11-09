using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
    public GameObject Bar;
	
    public void SetValue(float NewValue) {
        if (Bar) {
            if (Bar.transform.localScale.x <= 0f)
            {
                if (NewValue < 0f)
                    NewValue = 0f;
                Vector2 localScale;
                localScale.x = -NewValue;
                Transform bar = Bar.transform;
                bar.localScale = new Vector3(-NewValue, 1f);
            }
            else
            {
                Vector2 localScale;
                localScale.x = NewValue;
                Transform bar = Bar.transform;
                bar.localScale = new Vector3(NewValue, 1f);
            }
        }
    }

    public float GetValue() {
        float value_bar = 0f;
        if (Bar)
        {
            if (Bar.transform.localScale.x <= 0f)
            {
                value_bar = -Bar.transform.localScale.x * 1000;
            }
            else
            {
                value_bar = Bar.transform.localScale.x * 1000;
            }
        }
        return value_bar;
    }
}

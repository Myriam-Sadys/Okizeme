using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Okizeme.Fight
{
    public class Timer : MonoBehaviour
    {
        public float StartValue = 60.0f;
        private float Value;
        public Text text;

        private void Start()
        {
            Value = StartValue;
        }

        private void Update()
        {
            if (Value > 0.1)
            {
                Value -= Time.deltaTime;
                text.text = Value.ToString("0");
            }
            else
            {
                text.fontSize = 30;
                text.text = "EXTRA TIME";
            }
        }

        public float TimeRemaining()
        {
            return this.Value;
        }
    }
}

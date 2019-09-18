using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class AtkName : MonoBehaviour
    {
        public Text Atk;

        public void Start()
        {
            Atk.text = StaticClass.AtkName;
        }
    }
}

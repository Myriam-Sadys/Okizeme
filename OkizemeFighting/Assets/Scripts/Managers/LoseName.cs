using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class LoseName : MonoBehaviour
    {
        public Text Looser;

        public void Start()
        {
            Looser.text = StaticClass2.Loser;
        }
    }
}

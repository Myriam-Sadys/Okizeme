using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class DefName : MonoBehaviour
    {
        public Text Def;

        public void Start()
        {
            Def.text = StaticClass.DefName;
        }
    }
}

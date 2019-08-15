using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA
{
    public class SetInvisible : MonoBehaviour
    {
        void Update()
        {
            if (Fight.IsFight)
                this.gameObject.SetActive(false);
        }
    }
}
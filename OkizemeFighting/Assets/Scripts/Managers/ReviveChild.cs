using UnityEngine;
using UnityEditor;

namespace SA
{
    public class ReviveChild : MonoBehaviour
    {
        void Update()
        {
            if (Fight.IsFight == false)
            {
                foreach (Transform child in this.gameObject.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
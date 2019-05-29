using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{  
    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject
    {
        public CardType cardType;
        public int cost;
        public CardProperties[] properties;

        public CardProperties GetProperty(Element e)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if(properties[i].element == e)
                {
                    return properties[i];
                }
            }

            return null;
        }
    }

}

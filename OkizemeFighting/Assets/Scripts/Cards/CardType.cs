using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public abstract class CardType : ScriptableObject
    {

        public string typeName;
        public bool canAttack;

        public virtual void OnSetType(CardViz viz)
        {
            Element t = Settings.GetResourcesManager().typeElement;
            CardVizProperties type = viz.GetProperty(t);
            type.text.text = typeName;
        }

        public bool TypeAllowForAttack(CardInstance inst)
        {
            if (canAttack)
            {
                return true;
            }
            else
                return false;
        }
    }
}

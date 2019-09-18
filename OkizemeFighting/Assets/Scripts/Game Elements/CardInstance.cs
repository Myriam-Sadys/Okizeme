using UnityEngine;
using System.Collections;

namespace SA
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public PlayerHolder owner;
        public CardViz viz;
        public SA.GameElements.GE_Logic currentLogic;
        public bool isFlatFooted;

        public void SetFlatfooted(bool isFlat)
        {
            isFlatFooted = isFlat;
            if (isFlatFooted)
                transform.localEulerAngles = new Vector3(0, 0, 90);
            else
                transform.localEulerAngles = Vector3.zero;
        }

        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public void CardInstanceToGraveyard()
        {
            Settings.gameManager.PutCardToGraveyard(this);
        }

        public bool CanBeBlocked(CardInstance block, ref int count)
        {
            bool result = false;
            if (viz.card.cardType.canAttack)
            {
                result = true;
                if (result)
                {
                    Settings.gameManager.AddBlockInstance(this, block, ref count);
                }
                return result;
            }
            else
                return false;
            
        }

        public bool CanAttack()
        {
            bool result = true;

            if (isFlatFooted)
                result = false;

            if (viz.card.cardType.TypeAllowForAttack(this))
            {
                result = true;
            }
            return result;
        }

        public void OnClick()
        {
            if (currentLogic == null)
                return;
                
            currentLogic.OnClick(this);
        }

        public void OnHighlight()
        {
            if (currentLogic == null)
                return;

            currentLogic.OnHighlight(this);
        }
    }
}
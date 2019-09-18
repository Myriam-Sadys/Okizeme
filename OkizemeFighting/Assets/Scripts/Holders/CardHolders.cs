using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Holders/Card Holder")]
    public class CardHolders : ScriptableObject
    {
        public SO.TransformVariable handGrid;
        public SO.TransformVariable resourcesGrid;
        public SO.TransformVariable downGrid;
        public SO.TransformVariable battleLine;


       // [System.NonSerialized]
        public PlayerHolder playerHolder;

        public void SetCardOnBattleLine(CardInstance card)
        {
            Vector3 position = card.viz.gameObject.transform.position;

            Settings.SetParentForCard(card.viz.gameObject.transform, battleLine.value.transform);
            position.z = card.viz.gameObject.transform.position.z;
            position.y = card.viz.gameObject.transform.position.y;
            card.viz.gameObject.transform.position = position;
        }

        public void SetCardDown(CardInstance card)
        {
            Settings.SetParentForCard(card.viz.gameObject.transform, downGrid.value.transform);

        }

        public void LoadPlayer(PlayerHolder p, PlayerStatsUI statsUI)
        {
            if (p == null)
                return;
            playerHolder = p;
            p.currentHolder = this;

        foreach (CardInstance c in p.cardsDown)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, downGrid.value.transform);
            }

            foreach (CardInstance c in p.handCards)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, handGrid.value.transform);
            }


            foreach (CardInstance c in p.attackingCards)
            {
                SetCardOnBattleLine(c);
            }

            p.statsUI = statsUI;
            p.LoadPlayerOnStatsUI();
        }
    }
}
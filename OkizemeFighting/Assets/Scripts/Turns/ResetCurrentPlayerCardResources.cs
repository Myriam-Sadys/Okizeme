using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/ResetCurrentPlayerCardResources")]
    public class ResetCurrentPlayerCardResources : Phase
    {
        public override bool IsComplete()
        {
            Settings.gameManager.currentPlayer.MakeAllResourcesCardsUsable();
            return true;
        }

        public override void OnEndPhase()
        {
        }

        public override void OnStartPhase()
        {
        }
    }
}

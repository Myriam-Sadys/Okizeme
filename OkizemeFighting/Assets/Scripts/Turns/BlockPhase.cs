using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/BlockPhase")]
    public class BlockPhase : Phase
    {
        public PlayerAction changeActivePlayer;
        public GameStates.State playerControlState;

        public override bool IsComplete()
        {
            if (forceExit)
            {
                forceExit = false;
                return true;
            }
            return false;
        }

        public override void OnEndPhase()
        {
            if (isInit)
            {
                Settings.gameManager.SetState(null);
                isInit = false;
            }
        }

        public override void OnStartPhase()
        {
            if (!isInit)
            {
                GameManager gm = Settings.gameManager;
                gm.SetState(playerControlState);
                gm.onPhaseChanged.Raise();
                isInit = true;


                if (gm.currentPlayer.attackingCards.Count == 0)
                {
                    forceExit = true;
                    return;
                }

                if (gm.otherPlayersHolder.playerHolder.isHumanPlayer)
                {
                    gm.LoadPlayerOnActive(gm.otherPlayersHolder.playerHolder);
                }
                else
                {

                }
            }
        }

    }
}

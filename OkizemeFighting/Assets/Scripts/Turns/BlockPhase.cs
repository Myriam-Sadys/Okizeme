using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/BlockPhase ")]
    public class BlockPhase : Phase
    {
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
            Debug.Log("<color=green>block phase</color>");
            //  if (!isInit)
            //{
            GameManager gm = Settings.gameManager;
            gm.SetState(playerControlState);
            gm.onPhaseChanged.Raise();
            isInit = true;

            PlayerHolder e = gm.GetEnemyOf(gm.currentPlayer);
            Debug.Log(e.name + "attaque avec : " + e.attackingCards.Count + " cartes");
            if (e.attackingCards.Count == 0)
            {
                forceExit = true;
                return;
            }


            else if (!Fight.HaveFight)
            {
                Fight.IsFight = true;
                Debug.Log("combat");
                SceneManager.LoadScene("FightScene", LoadSceneMode.Additive);
                Fight.HaveFight = true;
                //Scene currentScene = SceneManager.GetActiveScene();
            }

            if (Fight.HaveFight && !Fight.IsFight)
            {
                Debug.Log("<color=yellow>ici</color>");
                forceExit = true;
                return;
            }

            //int availableCards = 0;
            //foreach (CardInstance c in gm.currentPlayer.cardsDown)
            //{
            //	if (!c.isFlatfooted)
            //	{
            //		availableCards++;
            //	}
            //}

            //Debug.Log(availableCards);

            //if (availableCards <= 0)
            //{
            //	forceExit = true;
            //	return;
            //}

            //if (gm.currentPlayer.attackingCards.Count == 0)
            //{
            //	forceExit = true;
            //	return;
            //}

            /*			}
                        else
                        {
                            Debug.Log("<color=pink>ça coince ici</color>");
                        }*/
        }
    }
}

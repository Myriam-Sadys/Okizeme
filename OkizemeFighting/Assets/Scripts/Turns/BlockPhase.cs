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
			//Debug.Log("block phase");	
		
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

				PlayerHolder e = gm.GetEnemyOf(gm.currentPlayer);
				if (e.attackingCards.Count == 0)
				{                    
                    forceExit = true;
					return;
				}

                else
                {
                    SceneManager.LoadScene(3, LoadSceneMode.Single);
                    Scene currentScene = SceneManager.GetActiveScene();
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

			}
		}
	}
}

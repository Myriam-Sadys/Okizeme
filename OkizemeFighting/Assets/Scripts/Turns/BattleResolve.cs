using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
	[CreateAssetMenu(menuName = "Turns/Battle Resolve ")]
	public class BattleResolve : Phase
	{
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
			isInit = false;
		}

		public override void OnStartPhase()
		{
			if (!isInit)
			{
                Debug.Log("battle resolve");
				isInit = true;
				MultiplayerManager.singleton.SetBattleResolvePhase();
			}
		}
	}
}


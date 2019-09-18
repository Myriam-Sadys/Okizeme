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

		BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
		{
			BlockInstance r = null;
			blockInstances.TryGetValue(attacker, out r);
			return r;
		}


		public override void OnEndPhase()
		{
			isInit = false;
		}

		public override void OnStartPhase()
		{
			if (!isInit)
			{
				isInit = true;
				MultiplayerManager.singleton.SetBattleResolvePhase();
			}
		}
	}
}


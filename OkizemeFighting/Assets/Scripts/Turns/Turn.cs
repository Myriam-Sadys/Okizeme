using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/Turn")]
    public class Turn : ScriptableObject
    {
        public bool tmp;

        public PlayerHolder player;
        [System.NonSerialized]
        public int index = 0;
        public PhaseVariable currentPhase;
        public Phase[] phases;

        public PlayerAction[] turnStartActions;

        public void OnTurnStart()
        {
            if (turnStartActions == null)
                return;
            for (int i = 0; i < turnStartActions.Length; i++)
            {
                Debug.Log(turnStartActions[i].name);
                turnStartActions[i].Execute(player);
            }
        }

        public bool Execute()
        {
            bool result = false;
            currentPhase.value = phases[index];
            phases[index].OnStartPhase();
          //  Debug.Log("debut de la phase " + phases[index].name + " de " + player.name);

            bool phaseIsComplete = phases[index].IsComplete();

            if (phaseIsComplete)
            {

                Debug.Log("fin de la phase " + phases[index].name + " de " + player.name);
                phases[index].OnEndPhase();
                index++;
                if (index > phases.Length - 1)
                {
                    index = 0;
                    result = true;
                }
                tmp = false;



            }
            else if (tmp == false)
            {
                Debug.Log("<color=blue>"+phases[index].name+"</color>" );
                tmp = true;
            }

            return result;
        }
        public void EndCurrentPhase()
        {
            phases[index].forceExit = true;
        }
    }
}
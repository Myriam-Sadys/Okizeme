using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/Battle Resolve")]
    public class BattleResolve : Phase
    {
        public Element attackElement;
        public Element defenceElement;

        public override bool IsComplete()
        {
            PlayerHolder p = Settings.gameManager.currentPlayer;
            PlayerHolder e = Settings.gameManager.GetEnemyOf(p);
            StaticClass.Player = Settings.gameManager.currentPlayer; ;
            StaticClass.Ennemy = Settings.gameManager.GetEnemyOf(p);

            if (p.attackingCards.Count == 0)
            {
                return true;
            }

            Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            for (int i = 0; i < p.attackingCards.Count; i++)
            {
                CardInstance inst = p.attackingCards[i];
                Card c = inst.viz.card;
                CardProperties attack = c.GetProperty(attackElement);

                if (attack == null)
                {
                    Debug.LogError("you are attacking with a card that can't attack !");
                    continue;
                }

                int attackValue = attack.intValue;
                StaticClass.AttackValue = attackValue;
                BlockInstance bi = GetBlockInstanceOfAttacker(inst, blockDict);
                if (bi != null)
                {
                    for (int b = 0; b < bi.blocker.Count; b++)
                    {
                        CardProperties def = c.GetProperty(defenceElement);
                        if (def == null)
                        {
                            Debug.LogWarning("you are trying to block with a card with no defence element");
                            continue;
                        }

                        StaticClass.Attacker = inst;
                        StaticClass.Blocker = bi.blocker[i];
                        StaticClass.AtkName = inst.viz.card.properties[i].strinValue;
                        StaticClass.DefName = bi.blocker[i].viz.card.properties[i].strinValue;
                        SceneManager.LoadScene(2, LoadSceneMode.Additive);
                        Scene currentScene = SceneManager.GetActiveScene();
                        string sceneName = currentScene.name;
                        Fight.IsFight = true;
                    }
                }
                else
                {
                    StaticClass.Ennemy.DoDamage(StaticClass.AttackValue);
                }
          

                /*   if (attackValue <= 0)
                   {
                       attackValue = 0;
                       inst.CardInstanceToGraveyard(); // carte attaquante
                   }*/

              /*  p.DropCard(inst, false);
                p.currentHolder.SetCardDown(inst);
                inst.SetFlatfooted(true);*/
                
            }

             Settings.gameManager.ClearBlockInstances();
             p.attackingCards.Clear();
           
            return true;
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);
            return r;
        }

        public override void OnEndPhase()
        {

        }

        public override void OnStartPhase()
        {

        }
    }
}
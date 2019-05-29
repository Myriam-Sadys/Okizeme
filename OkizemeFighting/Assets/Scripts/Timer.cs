using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SA
{
    public class Timer : MonoBehaviour
    {
        public int StartValue = 99;
        private int Value;
        private float Delta = 0.0f;
        public Text text;
        public Player Player1;
        public Player Player2;
        public Element attackElement;

        void Start()
        {
            Debug.Log("Fight");
            Value = StartValue;
        }
        void Update()
        {
            if (Player1.IsAlive() && Player2.IsAlive())
            {
                Delta += Time.fixedDeltaTime;
                Value = (int)((float)StartValue - (Delta % 60.0f));
                text.text = Value.ToString();

                //DEBUG
                if (Input.GetKeyDown("p"))
                {
                    Value -= 10;
                }
            }
            else
            {
                if (Player1.IsAlive())
                {
                    StaticClass.Blocker.CardInstanceToGraveyard();
                    StaticClass.Ennemy.DoDamage(StaticClass.AttackValue);
                }
                else
                { 
                    StaticClass.Attacker.CardInstanceToGraveyard();
                    StaticClass.Player.DoDamage(StaticClass.AttackValue);
                }
                Fight.IsFight = false;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName("Test"));
                SceneManager.UnloadScene("MainScene2");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SA
{
    public class Timer : MonoBehaviour
    {
        public float StartValue = 99.0f;
        private float Value;
        public Text text;
        //public PlayerMoving player1;
        //public PlayerMoving player2;
        //public Element attackElement;


        void Start()
        {
            Debug.Log("Fight");
            Value = StartValue;
        }

        void Update()
        {
            //if (player1 && player2)
            //{
                if (/*player1.IsAlive() && player2.IsAlive() &&*/ Value > 0.1)
                {
                    Value -= Time.deltaTime;
                    text.text = Value.ToString("0");
                }
                else
                {
                    if (Value <= 0.1)
                    {
                        Fight.IsFight = false;
                    }
                    //else if (Player1.IsAlive())
                    //{
                    //    StaticClass.Blocker.CardInstanceToGraveyard();
                    //    StaticClass.Ennemy.DoDamage(StaticClass.AttackValue);
                    //}
                    //else
                    //{ 
                    //    StaticClass.Attacker.CardInstanceToGraveyard();
                    //    StaticClass.Player.DoDamage(StaticClass.AttackValue);
                    //}
                    Fight.IsFight = false;
                    //SceneManager.SetActiveScene(SceneManager.GetSceneByName("Test"));
                    //SceneManager.UnloadScene("MainScene2");
                }
            //}
        }
    }
}

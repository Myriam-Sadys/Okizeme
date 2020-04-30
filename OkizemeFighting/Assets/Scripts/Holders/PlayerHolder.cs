using SA.GameElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
	[CreateAssetMenu(menuName = "Holders/Player Holder")]
	public class PlayerHolder : ScriptableObject
	{
		public string username;
		public Sprite potrait;

		[System.NonSerialized]
		public int photonId = -1;

        private int _health = 20;
        [System.NonSerialized]
        public int health;


        public int RessourcesAvailable;
        public PlayerStatsUI statsUI;

		//public string[] startingCards;
		public List<string> startingDeck = new List<string>();
		[System.NonSerialized]
		public List<string> all_cards = new List<string>();
		
		public int resourcesPerTurn = 1;
		[System.NonSerialized]
		public int resourcesDroppedThisTurn;

		public bool isHumanPlayer;

		public GE_Logic handLogic;
		public GE_Logic downLogic;

		[System.NonSerialized]
		public CardHolders currentHolder;

		[System.NonSerialized]
		public List<CardInstance> handCards = new List<CardInstance>();
		[System.NonSerialized]
		public List<CardInstance> cardsDown = new List<CardInstance>();
		[System.NonSerialized]
		public List<CardInstance> attackingCards = new List<CardInstance>();
        [System.NonSerialized]
        public List<int> cardInstIds = new List<int>();
        [System.NonSerialized]
        bool isInit = false;

        public List<CardInstance> allCardInstances = new List<CardInstance>();

        public int RessourcesTurn;

        public void Init()
		{
            Fight.Game = true;
            RessourcesAvailable = 1;
            RessourcesTurn = 1;
            health = 20;
            if (!isInit)
            {
                all_cards.AddRange(startingDeck);
                isInit = true;
            }
        }

		public int resourcesCount
		{
			get { return currentHolder.resourcesGrid.value.GetComponentsInChildren<CardViz>().Length; }
		}

		public void CardToGraveyard(CardInstance c)
		{
			if (attackingCards.Contains(c))
				attackingCards.Remove(c);
			if (handCards.Contains(c))
				handCards.Remove(c);
			if (cardsDown.Contains(c))
				cardsDown.Remove(c);

		}

		/*public void AddResourceCard(GameObject cardObj)
		{
			ResourceHolder resourceHolder = new ResourceHolder
			{
				cardObj = cardObj
			};

			resourcesList.Add(resourceHolder);
			resourcesDroppedThisTurn++;
		}

		public int NonUsedCards()
		{
			int result = 0;

			for (int i = 0; i < resourcesList.Count; i++)
			{
				if (!resourcesList[i].isUsed)
				{
					result++;
				}
			}

			return result;
		}*/

		public bool CanUseCard(Card c)
		{
            bool result = false;
            if (c.cardType is CreatureCard || c.cardType is SpellCard)
            {
                if (c.cost <= RessourcesAvailable)
                    result = true;
                statsUI.UpdatePZ();


            }
            /* else
             {
                 if (c.cardType is ResourceCard)
                 {
                     if (resourcesPerTurn - resourcesDroppedThisTurn > 0)
                         result = true;
                 }
             }*/
            return result;
		}

		public void DropCard(CardInstance inst, bool registerEvent = true)
		{
			if (handCards.Contains(inst))
				handCards.Remove(inst);

			cardsDown.Add(inst);
		}
	
		/*public List<ResourceHolder> GetUnusedResources()
		{
			List<ResourceHolder> result = new List<ResourceHolder>();

			for (int i = 0; i < resourcesList.Count; i++)
			{
				if (!resourcesList[i].isUsed)
				{
					result.Add(resourcesList[i]);
				}
			}

			return result;
		}*/

		public void MakeAllResourcesCardsUsable()
		{
            RessourcesAvailable = RessourcesTurn;
            if (RessourcesTurn < 10)
                RessourcesTurn++;
            statsUI.UpdatePZ();
        }

		public void UseResourceCards(int amount)
		{
            RessourcesAvailable -= amount;
            statsUI.UpdatePZ();
        }

		public void LoadPlayerOnStatsUI()
		{
			if (statsUI != null)
			{
				statsUI.player = this;
				statsUI.UpdateAll();
			}
		}

		public void DoDamage(int v)
		{
            health -= v;
            if (statsUI != null)
                statsUI.UpdateHealth();
            if (health < 1)
            {
                StaticClass2.Loser = username;
                SceneManager.LoadScene("YouLost");
            }
        }
		
	}
}

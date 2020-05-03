using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
    public class MultiplayerManager : Photon.MonoBehaviour
    {
        #region Variables
        public static MultiplayerManager singleton;
        Transform multiplayerReferences;
        public MainDataHolder dataHolder;
        //public PlayerHolder localPlayerHolder;
        //public PlayerHolder clientPlayerHolder;

        bool gameStarted;
        public bool countPlayers;
        GameManager gm
        {
            get
            {
                return GameManager.singleton;
            }
        }


        #endregion

        #region Player Management
        List<NetworkPrint> players = new List<NetworkPrint>();
        NetworkPrint localPlayer;
        NetworkPrint GetPlayer(int photonId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].photonId == photonId)
                    return players[i];
            }

            return null;
        }
        #endregion

        #region Init
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            multiplayerReferences = new GameObject("references").transform;
            DontDestroyOnLoad(multiplayerReferences.gameObject);

            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            InstantiateNetworkPrint();
            NetworkManager.singleton.LoadGameScene();
        }

        void InstantiateNetworkPrint()
        {
            PlayerProfile profile = Resources.Load("PlayerProfile") as PlayerProfile;
            object[] data = new object[1];
            data[0] = profile.cardIds;

            PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data);
        }
        #endregion

        #region Tick
        private void Update()
        {
            if (!gameStarted && countPlayers)
            {
                if (players.Count > 1)
                {
                    gameStarted = true;
                    StartMatch();
                }
            }
        }
        #endregion

        #region Starting the Match
        public void StartMatch()
        {
            ResourcesManager rm = gm.resourcesManager;

            if (NetworkManager.isMaster)
            {
                List<int> playerId = new List<int>();
                List<int> cardInstId = new List<int>();
                List<string> cardName = new List<string>();

                foreach (NetworkPrint p in players)
                {
                    foreach (string id in p.GetStartingCardIds())
                    {
                        Card card = rm.GetCardInstance(id);
                        playerId.Add(p.photonId);
                        cardInstId.Add(card.instId);
                        cardName.Add(id);

                        if (p.isLocal)
                        {
                            p.playerHolder = gm.localPlayer;
                            p.playerHolder.photonId = p.photonId;
                        }
                        else
                        {
                            p.playerHolder = gm.clientPlayer;
                            p.playerHolder.photonId = p.photonId;
                        }
                    }
                }

                for (int i = 0; i < playerId.Count; i++)
                {
                    photonView.RPC("RPC_PlayerCreatesCard", PhotonTargets.All, playerId[i], cardInstId[i], cardName[i]);
                }

                photonView.RPC("RPC_InitGame", PhotonTargets.All, 1);
            }
            else
            {
                foreach (NetworkPrint p in players)
                {
                    if (p.isLocal)
                    {
                        p.playerHolder = gm.localPlayer;
                        p.playerHolder.photonId = p.photonId;
                    }
                    else
                    {
                        p.playerHolder = gm.clientPlayer;
                        p.playerHolder.photonId = p.photonId;
                    }
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerCreatesCard(int photonId, int instId, string cardName)
        {
            Card c = gm.resourcesManager.GetCardInstance(cardName);
            c.instId = instId;

            NetworkPrint p = GetPlayer(photonId);
            p.AddCard(c);
        }

        [PunRPC]
        public void RPC_InitGame(int startingPlayer)
        {
            gm.isMultiplayer = true;
            gm.InitGame(startingPlayer);
        }

        public void AddPlayer(NetworkPrint n_print)
        {
            if (n_print.isLocal)
                localPlayer = n_print;

            players.Add(n_print);
            n_print.transform.parent = multiplayerReferences;
        }


        #endregion

        #region End Turn
        public void PlayerEndsTurn(int photonId)
        {
            photonView.RPC("RPC_PlayerEndsTurn", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndsTurn(int photonId)
        {
            if (photonId == gm.currentPlayer.photonId)
            {
                Debug.Log("Joueur " + photonId + " a finis son tour");
                if (NetworkManager.isMaster)
                {
                    int targetId = gm.GetNextPlayerID();
                    photonView.RPC("RPC_PlayerStartsTurn", PhotonTargets.All, targetId);
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerStartsTurn(int photonId)
        {
            gm.ChangeCurrentTurn(photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndPhase(int photonId)
        {
            if (photonId == gm.currentPlayer.photonId)
            {
                gm.EndCurrentPhase();
            }
        }
        #endregion

        #region Card Checks
        public void PlayerPicksCardFromDeck(PlayerHolder playerHolder)
        {
            Debug.Log(playerHolder.name + " pioche une carte");
            NetworkPrint p = GetPlayer(playerHolder.photonId);

            Card c = p.deckCards[0];
            p.deckCards.RemoveAt(0);

            PlayerWantsToUseCard(c.instId, p.photonId, CardOperation.pickCardFromDeck);
        }

        public void PlayerWantsToUseCard(int cardInst, int photonId, CardOperation operation)
        {
            photonView.RPC("RPC_PlayerWantsToUseCard", PhotonTargets.MasterClient, cardInst, photonId, operation);
        }

        [PunRPC]
        public void RPC_PlayerWantsToUseCard(int cardInst, int photonId, CardOperation operation)
        {
            if (!NetworkManager.isMaster)
                return;

            bool hasCard = PlayerHasCard(cardInst, photonId);

            if (hasCard)
            {
                photonView.RPC("RPC_PlayerUsesCard", PhotonTargets.All, cardInst, photonId, operation);
            }

        }

        bool PlayerHasCard(int cardInst, int photonId)
        {
            NetworkPrint player = GetPlayer(photonId);
            Card c = player.GetCard(cardInst);
            return (c != null);
        }
        #endregion

        #region Card Operations
        public enum CardOperation
        {
            dropResourcesCard, pickCardFromDeck, dropCreatureCard, setCardForBattle, cardToGraveyard, blockCard
        }

        [PunRPC]
        public void RPC_PlayerUsesCard(int instId, int photonId, CardOperation operation)
        {
            NetworkPrint p = GetPlayer(photonId);
            Card card = p.GetCard(instId);

            switch (operation)
            {
                case CardOperation.pickCardFromDeck:
                    GameObject go = Instantiate(dataHolder.cardPrefab) as GameObject;
                    CardViz v = go.GetComponent<CardViz>();
                    v.LoadCard(card);
                    card.cardPhysicalInst = go.GetComponent<CardInstance>();
                    card.cardPhysicalInst.currentLogic = dataHolder.handCard;
                    card.cardPhysicalInst.owner = p.playerHolder;
                    Settings.SetParentForCard(go.transform, p.playerHolder.currentHolder.handGrid.value);
                    p.playerHolder.handCards.Add(card.cardPhysicalInst);
                    break;
                case CardOperation.dropCreatureCard:
                    bool canUse = p.playerHolder.CanUseCard(card);
                    //bool canUse = Settings.gameManager.currentPlayer.CanUseCard(c);
                    if (canUse)
                    {
                        Settings.DropCreatureCard(card.cardPhysicalInst.transform, p.playerHolder.currentHolder.downGrid.value, card.cardPhysicalInst);
                        card.cardPhysicalInst.currentLogic = dataHolder.cardDownLogic;
                    }
                    else
                    {
                        //	Settings.RegisterEvent("Not enough resources to use card", Color.red);
                    }

                    card.cardPhysicalInst.gameObject.SetActive(true);
                    break;
                case CardOperation.setCardForBattle:
                    if (p.playerHolder.attackingCards.Contains(card.cardPhysicalInst))
                    {
                        p.playerHolder.attackingCards.Remove(card.cardPhysicalInst);
                        p.playerHolder.currentHolder.SetCardDown(card.cardPhysicalInst);
                    }
                    else
                    {
                        if (card.cardPhysicalInst.CanAttack())
                        {
                            p.playerHolder.attackingCards.Add(card.cardPhysicalInst);
                            p.playerHolder.currentHolder.SetCardOnBattleLine(card.cardPhysicalInst);
                        }
                    }
                    break;
                case CardOperation.cardToGraveyard:
                    card.cardPhysicalInst.CardInstanceToGraveyard();
                    break;
                case CardOperation.blockCard:
                    break;
                default:
                    break;
            }


        }


        #endregion

        #region Battle Resolve
        public void SetBattleResolvePhase()
        {
            photonView.RPC("RPC_BattleResolve", PhotonTargets.MasterClient);
        }

        [PunRPC]
        public void RPC_BattleResolve()
        {
            if (!NetworkManager.isMaster)
                return;

            BattleResolveForPlayers();
        }

        void BattleResolveForPlayers()
        {
            Debug.Log("reolution du combat");
            PlayerHolder player = Settings.gameManager.currentPlayer;
            PlayerHolder enemy = Settings.gameManager.GetEnemyOf(player);

            if (enemy.attackingCards.Count == 0)
            {
                //Debug.Log("enemy.attackingCards.Count == 0");
                photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
                //	photonView.RPC("RPC_PlayerEndPhase", PhotonTargets.All, player.photonId);
                return;
            }

            Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            for (int i = 0; i < enemy.attackingCards.Count; i++)
            {
                Debug.Log("on est la");
                CardInstance inst = enemy.attackingCards[i];
                Card c = inst.viz.card;
                CardProperties attack = c.GetProperty(dataHolder.attackElement);
                if (attack == null)
                {
                    Debug.LogError("you are attacking with a card that can't attack!");
                    continue;
                }

                int attackValue = attack.intValue;

                BlockInstance bi = GetBlockInstanceOfAttacker(inst, gm.GetBlockInstances());
                if (bi != null)
                {


                    for (int b = 0; b < bi.blocker.Count; b++)
                    {
                        CardProperties def = c.GetProperty(gm.defenceProperty);
                        if (def == null)
                        {
                            Debug.LogWarning("you are trying to block with a card with no defence element!");
                            continue;
                        }

                        attackValue -= def.intValue;

                        if (def.intValue <= attackValue)
                        {
                            bi.blocker[b].CardInstanceToGraveyard();
                        }
                    }
                }

                if (attackValue <= 0)
                {
                    attackValue = 0;
                    PlayerWantsToUseCard(inst.viz.card.instId, enemy.photonId, CardOperation.cardToGraveyard);
                }

                enemy.DropCard(inst, false);

                player.DoDamage(attackValue);
                photonView.RPC("RPC_SyncPlayerHealth", PhotonTargets.All, player.photonId, player.health);
                if (enemy.health < 0)
                {
                    SceneManager.LoadScene("YouWon");
                }
            }

            photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
            //	photonView.RPC("RPC_PlayerEndPhase", PhotonTargets.All, player.photonId);
            return;
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);
            return r;
        }

        [PunRPC]
        public void RPC_SyncPlayerHealth(int photonId, int health)
        {
            NetworkPrint p = GetPlayer(photonId);
            p.playerHolder.health = health;
            p.playerHolder.statsUI.UpdateHealth();
        }

        [PunRPC]
        public void RPC_BattleResolveCallback(int photonId)
        {
            //Debug.Log("RPC_BattleResolveCallback");
            foreach (NetworkPrint p in players)
            {
                //bool isAttacker = false;
                foreach (CardInstance c in p.playerHolder.attackingCards)
                {
                    p.playerHolder.currentHolder.SetCardDown(c);
                    c.SetFlatfooted(true);
                }

                if (p.photonId == photonId)
                {
                    if (p == localPlayer)
                    {
                        //isAttacker = true;
                        //	Settings.gameManager.EndCurrentPhase();
                    }
                }

                p.playerHolder.attackingCards.Clear();
            }

            foreach (BlockInstance bi in Settings.gameManager.GetBlockInstances().Values)
            {
                foreach (CardInstance c in bi.blocker)
                {
                    c.owner.currentHolder.SetCardDown(c);
                }
            }

            Settings.gameManager.ClearBlockInstances();
            //p.attackingCards.Clear();

        }

        #endregion

        #region Blocking
        public void PlayerBlocksTargetCard(int cardInst, int photonId, int targetInst, int blocked)
        {
            photonView.RPC("RPC_PlayerBlocksTargetCard_Master", PhotonTargets.MasterClient, cardInst, photonId, targetInst, blocked);
        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Master(int cardInst, int photonId, int targetInst, int blocked)
        {
            NetworkPrint playerBlocker = GetPlayer(photonId);
            Card blockerCard = playerBlocker.GetCard(cardInst);
            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = blockedPlayer.GetCard(targetInst);

            int count = 0;
            Settings.gameManager.AddBlockInstance(blockedCard.cardPhysicalInst, blockerCard.cardPhysicalInst, ref count);
            photonView.RPC("RPC_PlayerBlocksTargetCard_Client", PhotonTargets.All, cardInst, photonId, targetInst, blocked, count);
        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Client(int cardInst, int photonId, int targetInst, int blocked, int count)
        {
            NetworkPrint playerBlocker = GetPlayer(photonId);
            Card blockerCard = playerBlocker.GetCard(cardInst);
            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = blockedPlayer.GetCard(targetInst);

            Settings.SetCardForBlock(blockerCard.cardPhysicalInst.transform, blockedCard.cardPhysicalInst.transform, count);
        }
        #endregion

        #region Multiple Card Operations

        #region Flatfooted Cards
        public void PlayerWantsToResetFlatfootedCards(int photonId)
        {
            photonView.RPC("RPC_ResetFlatfootedCardsForPlayer_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.turns[gm.turnIndex].player == p.playerHolder)
            {
                photonView.RPC("RPC_ResetFlatfootedCardsForPlayer", PhotonTargets.All, photonId);
            }
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            foreach (CardInstance c in p.playerHolder.cardsDown)
            {
                if (c.isFlatFooted)
                {
                    c.SetFlatfooted(false);
                }
            }
        }
        #endregion

        #region Resources Cards
        public void PlayerWantsToResetResourcesCards(int photonId)
        {
            photonView.RPC("RPC_PlayerWantsToResetResourcesCards_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerWantsToResetResourcesCards_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.turns[gm.turnIndex].player == p.playerHolder)
            {
                photonView.RPC("RPC_ResetResourcesCardsForPlayer", PhotonTargets.All, photonId);
            }
        }

        [PunRPC]
        public void RPC_ResetResourcesCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            p.playerHolder.MakeAllResourcesCardsUsable();
        }
        #endregion

        #region Management
        public void SendPhase(string holder, string phase)
        {
            //photonView.RPC("RPC_MessagePhase", PhotonTargets.All, phase, holder);
        }

        [PunRPC]
        public void RPC_MessagePhase(string phase, string holder)
        {
            //	Debug.Log(phase + " " + holder);
        }

        #endregion

        #endregion
    }
}

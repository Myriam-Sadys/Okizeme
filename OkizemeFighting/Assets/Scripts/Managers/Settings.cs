using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SA
{
    public static class Settings
    {
        public static GameManager gameManager;

        private static ResourcesManager _ressourcesManager;

        public static ResourcesManager GetResourcesManager()
        {
            if (_ressourcesManager == null)
            {
                _ressourcesManager = Resources.Load("ResourcesManager") as ResourcesManager;
                _ressourcesManager.Init();
            }

            return _ressourcesManager;
        }

        public static List<RaycastResult> GetUIObjs()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            return results;
        }

        public static void DropCreatureCard(Transform c, Transform p, CardInstance cardInst)
        {
            cardInst.isFlatFooted = true;
            //execute any special card abilitie ond drop
            SetParentForCard(c, p);
            cardInst.SetFlatfooted(true);
            gameManager.currentPlayer.UseResourceCards(cardInst.viz.card.cost);
            gameManager.currentPlayer.DropCard(cardInst);

        }

        public static void SetParentForCard(Transform c, Transform p)
        {
            c.SetParent(p);
            c.localPosition = Vector3.zero;
            c.localEulerAngles = Vector3.zero;
            c.localScale = Vector3.one * 2;
        }

        public static void SetCardForBlock(Transform c, Transform p, int count)
        {
            Vector3 blockPosition = Vector3.zero;
            blockPosition.x += 150 * count;
            blockPosition.y -= 150 * count;
            SetParentForCardBlock(c, p, blockPosition, Vector3.zero);
        }

        public static void SetParentForCardBlock(Transform c, Transform p, Vector3 localPosition, Vector3 euler)
        {
            c.SetParent(p);
            c.localPosition = localPosition;
            c.localEulerAngles = euler;
            c.localScale = Vector3.one;
        }

        public static void SetParentForCard(Transform c, Transform p, Vector3 localPosition, Vector3 euler)
        {
            c.SetParent(p);
            c.localPosition = localPosition;
            c.localEulerAngles = euler;
            c.localScale = Vector3.one * 2;
        }
    }
}
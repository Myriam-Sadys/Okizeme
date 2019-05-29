using UnityEngine;
using System.Collections;

namespace SA
{
   [CreateAssetMenu(menuName ="Actions/Player Actions/Reset Flat Foot")]
    public class ResetFlatFlootedCards : PlayerAction
    {
        public override void Execute(PlayerHolder player)
        {
            foreach (CardInstance c in player.cardsDown)
            {
                if(c.isFlatFooted)
                {
                    c.SetFlatfooted(false);
                }
            }
        }
    }
}

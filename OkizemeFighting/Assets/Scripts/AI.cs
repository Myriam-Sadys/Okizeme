using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private float Spacing = 75.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDecision(Player player)
    {
        player.anim.SetTrigger("Idle");
        if (Mathf.Abs(player.transform.position.x - player.Enemy.transform.position.x) > 400.0f && !player.ProjectileLaunched)
        { // Joueur trop loin -> Projectile
            player.anim.SetTrigger("AttackB");
            player.SpawnProjectile();
        }
        else if (Mathf.Abs(player.transform.position.x - player.Enemy.transform.position.x) < 100.0f && !player.GetIsAttacking())
        { // Joueur proche -> Coup de poing
            player.anim.SetTrigger("AttackA");
            player.SetIsAttacking(true);
            if (Vector2.Distance(player.transform.position, player.Enemy.transform.position) < 100.0f)
            {
                player.Enemy.TakeDamage(50);
                player.GainZemePoints(7);
            }
        }
        else
        { // Sinon, approchez le joueur
            MoveTowardsPlayer(player);
        }
    }

    void MoveTowardsPlayer(Player player)
    {
        float Direction = 0.0f;
        if (player.transform.position.x + Spacing > player.Enemy.transform.position.x)
        {
            //Debug.Log("AI GO LEFT");
            Direction = -1.0f;
        }
        if (player.transform.position.x - Spacing < player.Enemy.transform.position.x)
        {
            //Debug.Log("AI GO RIGHT");
            Direction = 1.0f;
        }
        //Debug.Log("AI MOVE");
        Vector3 movement = new Vector3(Direction, 0.0f, 0.0f);
        transform.Translate(movement * player.PlayerSpeed * Time.fixedDeltaTime);
    }
}

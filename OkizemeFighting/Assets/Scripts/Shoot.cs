using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;


    void Update()
    {
        if (Input.GetButtonDown("AttackB"))
        {
            ShootSpell();
        }
    }

    private void ShootSpell()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}

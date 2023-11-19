using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxInfo : MonoBehaviour
{
    private int m_HitboxDamage = 0;
    public int HitboxDamage
    {
       get { return m_HitboxDamage; }
    }

    public void SetDamage(int damage)
    {
        m_HitboxDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);
        }
    }
}

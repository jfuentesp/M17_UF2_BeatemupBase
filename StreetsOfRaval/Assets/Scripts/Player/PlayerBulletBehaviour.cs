using streetsofraval;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehaviour : MonoBehaviour
{
    [SerializeField]
    private int m_BulletDamage;
    public int BulletDamage => m_BulletDamage;
    [SerializeField]
    private float m_BulletSpeed;
    [SerializeField]
    private float m_BulletLifetime;

    private Rigidbody2D m_RigidBody;
    private bool m_Destroyable;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }

    public void InitBullet(/*float speed,*/ int damage, bool destroyable, Vector2 direction)
    {
        /*m_BulletSpeed = speed;*/
        //It will set the bullet damage
        m_BulletDamage = damage;
        m_Destroyable = destroyable;
        //It will initiate the bullet direction and give it a speed
        m_RigidBody.velocity = direction * m_BulletSpeed;
        //If the direction is negative it will invert the sprite. If not, it will let it as default.
        if(direction.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0); 
        } else
        {
            transform.eulerAngles = Vector3.zero;
        }
        StartCoroutine(BulletAlive()); //Starting the lifetime coroutine
    }

    private IEnumerator BulletAlive()
    {
        yield return new WaitForSeconds(m_BulletLifetime);
        if (m_Destroyable)
        {
            Destroy(this.gameObject);
        } else
        {
            this.gameObject.SetActive(false);
        }    
    }
}

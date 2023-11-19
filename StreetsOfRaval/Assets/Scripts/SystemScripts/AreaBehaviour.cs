using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBehaviour : MonoBehaviour
{
    private bool m_PlayerDetected;
    public bool PlayerDetected => m_PlayerDetected;

    private void OnEnable()
    {
        m_PlayerDetected = false;
    }

    private void OnDisable()
    {
        m_PlayerDetected = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            m_PlayerDetected = true;
        } else
        {
            m_PlayerDetected = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_PlayerDetected = false;
        }
        else
        {
            m_PlayerDetected = true;
        }
    }
}

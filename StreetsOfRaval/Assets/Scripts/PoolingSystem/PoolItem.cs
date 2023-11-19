using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItem : MonoBehaviour
{
    //Private variable to set which Pool will manage the PoolObject
    private Pool m_Owner;
    //private bool m_Created;

    private void Awake()
    {
        //m_Created = false;
    }

    //Function that sets the Pool to the PoolObject
    public void SetPool(Pool owner)
    {
        m_Owner = owner;
    }


    private void OnDisable()
    {
        //Enable if you want to control who is returning the element.
        /*if (!m_Owner.ReturnElement(gameObject))
            Debug.LogError(gameObject + "Pool Return error");

        */
    }
}

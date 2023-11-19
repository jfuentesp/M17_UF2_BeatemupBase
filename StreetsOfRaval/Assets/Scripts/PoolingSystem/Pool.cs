using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public static Pool instance;
    //We set a list. This list will be the Pool. 
    public List<GameObject> m_Pool;

    [Header("Pool Settings => Item to Pool and Size.")]
    //A reference to the GameObject that the Pool will reserve the size in the memory
    [SerializeField]
    public GameObject m_PoolObject;
    //Number of items the Pool will held. The bigger the number, the more items will be able to spawn, and more memory will need to alocate.
    [SerializeField]
    public int m_Capacity = 50;
    [Header("Reference to the parent item.")]
    [SerializeField]
    public GameObject m_TransformReference;


    private void Awake()
    {
        instance = this;
        //Checking if Pool Item has its component attached
        if (!m_PoolObject.GetComponent<PoolItem>())
        {
            Debug.Log("Pool Item has no PoolItem script associated or can't find the script.");
            return;
        }

        //Creating new List of GameObjects on Awake. This will be the Pool.
        m_Pool = new List<GameObject>();
        for (int i = 0; i < m_Capacity; i++)
        {
            //Instantiating and disabling a new element for each iteration in List capacity (pool capacity). And add to the queue.
            //GameObject element = Instantiate(m_PoolObject, m_TransformReference.transform.position, m_TransformReference.transform.rotation); 
            // Note about transform.pos/rot or this.transform: if item needs to be spawned in a certain position, it should be set after activating it in the Behaviour script.
            //this.transform sets the parent reference, so every item will be saved as a child from parent instead of replenishing all the scene with free gameobjects
            GameObject element = Instantiate(m_PoolObject, this.transform);
            element.GetComponent<PoolItem>().SetPool(this);
            m_Pool.Add(element);
            element.SetActive(false);
        }
    }

    public GameObject GetElement()
    {
        foreach (GameObject element in m_Pool)
        {
            //For each iteration in the Pool, we enable and return the first item that is available (disabled).
            if (element.activeInHierarchy == false)
            {
                //Had to add the rotation inside here because there was a problem setting it into the shooting action in PlayerBehaviour. Called first this function than the properties given.
                element.SetActive(true);
                return element;
            }
        }
        //If returns null, means that all the items are active so we need a higher capacity.
        Debug.Log("Error: there are no more Items available to Pool. Set a higher Capacity.");
        return null;
    }

    public bool ReturnElement(GameObject element)
    {

        if (element.activeInHierarchy == true)
        {
            //Checking if the element is active. If it is, it will be disabled and return a true status (success).
            element.SetActive(false);
            return true;
        }
        return false;
    }
}

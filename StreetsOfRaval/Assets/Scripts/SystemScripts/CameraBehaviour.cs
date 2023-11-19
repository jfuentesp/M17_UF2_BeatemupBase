using System.Collections;
using System.Collections.Generic;
using streetsofraval;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    PlayerBehaviour m_Player;
    // Start is called before the first frame update
    void Start()
    {
        m_Player = PlayerBehaviour.PlayerInstance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = new Vector3(m_Player.transform.position.x, transform.position.y, transform.position.z);
        transform.position = playerPosition;
    }
}

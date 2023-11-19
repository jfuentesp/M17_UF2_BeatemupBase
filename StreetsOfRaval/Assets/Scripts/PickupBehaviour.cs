using System.Collections;
using System.Collections.Generic;
using streetsofraval;
using UnityEngine;

/* LIST OF PICKUPS REFERENCED */
/*
 * 0 = Coins + 100 score
 * 1 = Diamond + 500 score
 * 2 = Life + 1 Life
 * 3 = Mana + 20 mana
 * 4 = Potion + 20 hp
 * 5 = Treasure + 1000 score
 */

public class PickupBehaviour : MonoBehaviour
{
    [Header("Scriptable List References")]
    [SerializeField]
    List<PickupScriptableObject> m_ScriptablePickups;

    private int m_PickupID;
    private string m_PickupName;
    private int m_PickupValue;
    private float m_PickupEffectDuration;
    private Color m_PickupColor;
    private Sprite m_PickupSprite;

    private SpriteRenderer m_SpriteRenderer;

    [Header("Time until the pickup disappears")]
    [SerializeField]
    private float m_PickupDuration;

    [Header("GameEvent references")]
    [SerializeField]
    private GameEventInt m_OnPickupScore;
    [SerializeField]
    private GameEventInt m_OnPickupHealth;
    [SerializeField]
    private GameEventInt m_OnPickupMana;
    [SerializeField]
    private GameEventInt m_OnPickupLife;

    private Vector2 m_SpawnPoint;

    private void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PickupScriptableObject randomPickup = m_ScriptablePickups[Random.Range(0, m_ScriptablePickups.Count)];
        InitPickup(randomPickup);
        StartCoroutine(AliveCoroutine());
    }

    public void InitPickup(PickupScriptableObject pickupInfo)
    {
        m_PickupID = pickupInfo.PickupID;
        m_PickupName = pickupInfo.PickupName;
        m_PickupValue = pickupInfo.PickupValue;
        m_PickupEffectDuration = pickupInfo.PickupEffectDuration;
        m_PickupColor = pickupInfo.PickupColor;
        m_PickupSprite = pickupInfo.PickupSprite;
        m_SpriteRenderer.sprite = m_PickupSprite;
    }

    private IEnumerator AliveCoroutine()
    {
        yield return new WaitForSeconds(m_PickupDuration);
        Destroy(this.gameObject);
    }

    public void GetPickup()
    {
        switch (m_PickupID)
        {
            case 0:
                m_OnPickupScore.Raise(100);
                break;
            case 1:
                m_OnPickupScore.Raise(500);
                break;
            case 2:
                m_OnPickupLife.Raise(1);
                break;
            case 3:
                m_OnPickupMana.Raise(20);
                break;
            case 4:
                m_OnPickupHealth.Raise(20);
                break;
            case 5:
                m_OnPickupScore.Raise(1000);
                break;
            default:
                break;
        }
    }
}

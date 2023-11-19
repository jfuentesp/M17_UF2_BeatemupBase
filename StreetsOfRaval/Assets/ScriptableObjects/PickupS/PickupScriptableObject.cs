using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace streetsofraval
{
    [CreateAssetMenu(fileName = "PickupScriptableObject", menuName = "Scriptable Objects/Scriptable Pickup")]
    public class PickupScriptableObject : ScriptableObject
    {
        [SerializeField]
        private int m_PickupID;
        [SerializeField]
        private string m_PickupName;
        [SerializeField]
        private int m_PickupValue;
        [SerializeField]
        private float m_PickupEffectDuration;
        [SerializeField]
        private Color m_PickupColor;
        [SerializeField]
        private Sprite m_PickupSprite;

        public int PickupID => m_PickupID;
        public string PickupName => m_PickupName;
        public int PickupValue => m_PickupValue;
        public float PickupEffectDuration => m_PickupEffectDuration;
        public Color PickupColor => m_PickupColor;
        public Sprite PickupSprite => m_PickupSprite;
    }
}

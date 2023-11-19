using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

//Order of difficulty: Common < Elite < Veteran

namespace streetsofraval
{
    [CreateAssetMenu(fileName = "EnemyInfoScriptableObject", menuName = "Scriptable Objects/Scriptable EnemyInfo")]
    public class EnemyScriptableObject : ScriptableObject
    {
        [SerializeField]
        private float m_EnemyMaxHP;
        [SerializeField]
        private float m_EnemyDamage;
        [SerializeField]
        private float m_EnemySpeed;
        [SerializeField]
        private Color m_SpriteColor;
        [SerializeField]
        private int m_ScoreValue;


        public float EnemyMaxHP => m_EnemyMaxHP;
        public float EnemyDamage => m_EnemyDamage;
        public float EnemySpeed => m_EnemySpeed;
        public Color SpriteColor => m_SpriteColor;
        public int ScoreValue => m_ScoreValue;
    }
}


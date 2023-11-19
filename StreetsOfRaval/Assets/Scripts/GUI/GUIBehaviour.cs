using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace streetsofraval
{
    public class GUIBehaviour : MonoBehaviour
    {
        private GameManager m_GameManager;
        private PlayerBehaviour m_Player;
        private SpawnerBehaviour m_Spawner;

        [Header("GUI References")]
        [SerializeField]
        private TextMeshProUGUI m_EnemiesText;
        [SerializeField]
        private TextMeshProUGUI m_ScoreText;
        [SerializeField]
        private TextMeshProUGUI m_WaveText;
        [SerializeField]
        private TextMeshProUGUI m_Lives;
        [SerializeField]
        private Image m_HPBar;
        [SerializeField]
        private Image m_EnergyBar;
      

        //This function updates every GUI Information. It's called from different objects.
        public void UpdateGUI()
        {
            //If I set the Instances on the Start() function, it throws an error. I must instance them in the UpdateGUI function, then works.
            m_GameManager = GameManager.GameManagerInstance;
            m_Player = PlayerBehaviour.PlayerInstance;
            m_Spawner = SpawnerBehaviour.SpawnerInstance;
            m_EnemiesText.text = m_Spawner.Enemies + "/" + m_Spawner.TotalEnemies;
            m_WaveText.text = "Wave: " + m_GameManager.Wave;
            m_ScoreText.text = "Score: " + m_GameManager.Score;
            m_Lives.text = "Player lives: " + m_GameManager.Lives;
            m_HPBar.fillAmount = m_Player.Hitpoints / m_Player.MaxHitpoints;
            m_EnergyBar.fillAmount = m_Player.Energy / m_Player.MaxEnergy;
        }

        //This function activates once every enemy has been slain, it's a wait-time between waves.
        public void LoadingWaveGUI()
        {
            m_WaveText.text = "Awaiting for next wave...";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace streetsofraval { 
    public class GameOverGUI : MonoBehaviour
    {
        private GameManager m_GameManager;

        [Header("UI Elements references")]
        [SerializeField]
        private TextMeshProUGUI m_Score;
        [SerializeField]
        private TextMeshProUGUI m_Wave;
        [SerializeField]
        private Button m_PlayAgainButton;
        [SerializeField]
        private Button m_MainMenuButton;

        // Start is called before the first frame update
        void Start()
        {
            m_GameManager = GameManager.GameManagerInstance;
            m_Score.text = "Your Score: " + m_GameManager.Score;
            m_Wave.text = "Your Wave: " + m_GameManager.Wave;
            m_PlayAgainButton.onClick.AddListener(m_GameManager.PlayAgain);
            m_MainMenuButton.onClick.AddListener(m_GameManager.MainMenuScene);
        }
    }
}
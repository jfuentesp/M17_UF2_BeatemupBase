using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace streetsofraval
{
    public class GameManager : MonoBehaviour
    {
        //Instance of the GameManager. Refers to this own gameobject.
        private static GameManager m_Instance;
        public static GameManager GameManagerInstance => m_Instance; //A getter for the instance of the game manager. Similar to get { return m_Instance }. (Accessor)

        private const string m_MainTitleScene = "MainTitleScene";
        private const string m_GameScene = "GameScene";
        private const string m_GameOverScene = "GameOverScene";

        [Header("Game Parameters")]
        [SerializeField]
        private int m_Wave = 0;
        [SerializeField]
        private int m_Score = 0;
        [SerializeField]
        private int m_Lives = 0;
        [SerializeField]
        List<int> m_NumberOfEnemiesByWave;
        private int m_EnemiesSpawned;

        private int m_RemainingEnemies;

        public int Wave => m_Wave;
        public int Score => m_Score;
        public int Lives => m_Lives;
        public List<int> NumberOfEnemiesByWave => m_NumberOfEnemiesByWave;
        public int NumberOfEnemies => m_EnemiesSpawned;

        [Header("GameEvents for the Game Mechanics")]
        [SerializeField]
        GameEvent m_OnNextWave;
        //[SerializeField]
        //GameEventVoid m_OnPlayerRespawn;
        [SerializeField]
        GameEvent m_OnGUIUpdate;

        Vector3 m_PlayerSpawnPoint;

        SpawnerBehaviour m_Spawner;
        PlayerBehaviour m_Player;

        private void Awake()
        {
            //First, we initialize an instance of GameManager. If there is already an instance, it destroys the element and returns.
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            m_Spawner = SpawnerBehaviour.SpawnerInstance;
            m_Player = PlayerBehaviour.PlayerInstance;
            m_PlayerSpawnPoint = m_Player.transform.position;
            InitializeGame();
        }

        public void InitializeGame()
        {
            m_Score = 0;
            m_Lives = 2;
            m_Wave = 0;
        }

        //Substracts a Live and checks if the lives are more than 0. If not, loads the GameOver scene.
        public void OnPlayerDeath()
        {
            SubstractLives(1);
            if (m_Lives > 0)
            {
                StartCoroutine(PlayerDeathCoroutine());
            }
            else
            {
                SceneManager.LoadScene(m_GameOverScene);
            }
        }

        public void PlayAgain()
        {
            InitializeGame();
            SceneManager.LoadScene(m_GameScene);
        }

        public void MainMenuScene()
        {
            InitializeGame();
            SceneManager.LoadScene(m_MainTitleScene);
        }

        public void AddScore(int score)
        {
            m_Score += score;
            m_OnGUIUpdate.Raise();
        }

        private void AddWave(int wave)
        {
            m_Wave += wave;
        }

        public void AddLives(int lives)
        {
            m_Lives += lives;
            m_OnGUIUpdate.Raise();
        }

        private void SubstractLives(int lives)
        {
            m_Lives -= lives;
            m_OnGUIUpdate.Raise();
        }

        public void OnWaveCleared()
        {
            StartCoroutine(NextWaveCoroutine());
        }

        private IEnumerator NextWaveCoroutine()
        {
            yield return new WaitForSeconds(5f);
            AddWave(1);
            m_OnNextWave.Raise(); //For Spawner
            m_OnGUIUpdate.Raise(); //For GUI update
        }

        private IEnumerator PlayerDeathCoroutine()
        {
            yield return new WaitForSeconds(2f);
            m_Player.transform.position = m_PlayerSpawnPoint;
            m_Player.gameObject.SetActive(true);
        }
    }
}

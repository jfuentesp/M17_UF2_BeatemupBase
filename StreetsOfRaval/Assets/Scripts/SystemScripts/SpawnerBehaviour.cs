using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace streetsofraval 
{ 
    public class SpawnerBehaviour : MonoBehaviour
    {
        //Instance of the Spawner. Refers to this own gameobject.
        private static SpawnerBehaviour m_Instance;
        public static SpawnerBehaviour SpawnerInstance => m_Instance; //A getter for the instance of the spawner. Similar to get { return m_Instance }. (Accessor)

        [Header("Spawner settings")]
        [SerializeField]
        private float m_SpawnTime;
        private int m_Wave;
        [SerializeField]
        private int m_EnemiesToSpawn;
        [SerializeField]
        private int m_EnemiesSpawned;
        private bool m_IsSpawning;
        private int m_LowerDifficultyLevel;
        private int m_HigherDifficultyLevel;

        public int TotalEnemies => m_EnemiesToSpawn;
        public int Enemies => m_EnemiesSpawned;
        public bool IsSpawning => m_IsSpawning;

        [Header("Spawnpoints references")]
        [SerializeField]
        private Transform m_LeftSpawnpoint;
        [SerializeField]
        private Transform m_RightSpawnpoint;
        [Header("Pool references")]
        [SerializeField]
        private Pool m_RobberPool;
        [SerializeField]
        private Pool m_RangedPool;
        [SerializeField]
        private Pool m_ThiefPool;
        [SerializeField]
        private Pool m_GangPool;


        [Header("Game Events")] 
        [SerializeField]
        GameEvent m_OnWaveCleared;
        [SerializeField]
        GameEvent m_OnGUIUpdate;


        GameManager m_GameManager;

        

        [Header("List of all enemy Scriptable Objects set in order from lower to higher stats")]
        [SerializeField]
        List<EnemyScriptableObject> m_EnemyInfoList;
        //[SerializeField]
        //GameEventIntInt m_OnEnemySpawned;
        //[SerializeField]
        //GameEventIntInt m_OnEnemyDeathUpdate;



        private void Awake()
        {
            //First, we initialize an instance of Spawner. If there is already an instance, it destroys the element and returns.
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            //Giving all the variables a default value
            m_LowerDifficultyLevel = 0;
            m_HigherDifficultyLevel = 0;
            m_EnemiesSpawned = 0;
            m_EnemiesToSpawn = 0;
            m_IsSpawning = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_GameManager = GameManager.GameManagerInstance;
            m_Wave = m_GameManager.Wave;
            OnWaveStart();       
        }


        public void OnWaveStart()
        {
            m_Wave = m_GameManager.Wave;
            m_EnemiesSpawned = 0;
            m_IsSpawning = true;
            switch (m_Wave)
            {
                case 0:
                    m_LowerDifficultyLevel = 0;
                    m_HigherDifficultyLevel = 0;
                    m_EnemiesToSpawn = 12;
                    break;

                case 1:
                    m_LowerDifficultyLevel = 1;
                    m_HigherDifficultyLevel = 1;
                    m_EnemiesToSpawn = 12;
                    break;

                case 2:
                    m_LowerDifficultyLevel = 0;
                    m_HigherDifficultyLevel = 2;
                    m_EnemiesToSpawn = 18;
                    break;

                case 3:
                    m_LowerDifficultyLevel = 0;
                    m_HigherDifficultyLevel = 4;
                    m_EnemiesToSpawn = 25;
                    break;

                case 4:
                    m_LowerDifficultyLevel = 2;
                    m_HigherDifficultyLevel = 6;
                    m_EnemiesToSpawn = 42;
                    break;

                default:
                    m_LowerDifficultyLevel = 3;
                    m_HigherDifficultyLevel = 9;
                    m_EnemiesToSpawn = 52;
                    break;
            }
            m_OnGUIUpdate.Raise();
            StartCoroutine(SpawnCoroutine());
        }

        public void OnEnemyDeath(int score)
        {
            m_EnemiesSpawned--;
            m_OnGUIUpdate.Raise();
            if(m_EnemiesSpawned == 0 && !m_IsSpawning)
                m_OnWaveCleared.Raise();
        }

        private IEnumerator SpawnCoroutine()
        {
            Debug.Log("Llego al interior de la corrutina");
            int counter = 0;
            while (m_IsSpawning)
            {
                int spawnpoint = Random.Range(0, 2);
                int difficultylevel = Random.Range(m_LowerDifficultyLevel, m_HigherDifficultyLevel);
                Debug.Log(string.Format("Spawnpoint is: {0} And the difficulty level is: {1}", spawnpoint, difficultylevel));
                SpawnEnemy(spawnpoint, difficultylevel);
                counter++;
                m_EnemiesSpawned++;
                m_OnGUIUpdate.Raise();
                if (counter == m_EnemiesToSpawn - 1)
                {
                    m_IsSpawning = false;
                }
                yield return new WaitForSeconds(m_SpawnTime);
            }
        }

        private void SpawnEnemy(int spawnpoint, int difficultylevel)
        {
            EnemyScriptableObject enemyInfo;
            GameObject enemy;
            //Enemies will spawn in a random location on X axis between the two spawnpoints
            float spawnX = Random.Range(m_LeftSpawnpoint.position.x, m_RightSpawnpoint.position.x); //spawnpoint == 0 ? m_LeftSpawnpoint.position : m_RightSpawnpoint.position; if we want to spawn right in the spawnpoints
            Vector2 spawnposition = new Vector2(spawnX, m_RightSpawnpoint.position.y);
            switch (difficultylevel % 4)
            {
                case 0:
                    enemyInfo = m_EnemyInfoList[difficultylevel];
                    Pool RobberPool = m_RobberPool.GetComponent<Pool>();
                    enemy = RobberPool.GetComponent<Pool>().GetElement();              
                    enemy.GetComponent<EnemyRobberBehaviour>().InitEnemy(enemyInfo, spawnpoint);
                    enemy.transform.position = spawnposition;
                    break;
                case 1:
                    enemyInfo = m_EnemyInfoList[difficultylevel];
                    Pool RangedPool = m_RangedPool.GetComponent<Pool>();
                    enemy = RangedPool.GetComponent<Pool>().GetElement();
                    enemy.GetComponent<EnemyRangedBehaviour>().InitEnemy(enemyInfo, spawnpoint);
                    enemy.transform.position = spawnposition;
                    break;
                case 2:
                    enemyInfo = m_EnemyInfoList[difficultylevel];
                    Pool ThiefPool = m_ThiefPool.GetComponent<Pool>();
                    enemy = ThiefPool.GetComponent<Pool>().GetElement();
                    enemy.GetComponent<EnemyThiefBehaviour>().InitEnemy(enemyInfo, spawnpoint);
                    enemy.transform.position = spawnposition;
                    break;
                default:
                    enemyInfo = m_EnemyInfoList[difficultylevel];
                    Pool GangPool = m_GangPool.GetComponent<Pool>();
                    enemy = GangPool.GetComponent<Pool>().GetElement();
                    enemy.GetComponent<EnemyGangBehaviour>().InitEnemy(enemyInfo, spawnpoint);
                    enemy.transform.position = spawnposition;
                    break;
            }
       
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace streetsofraval
{
    public class PlayerBehaviour : MonoBehaviour
    {
        //Instance of the Player. Refers to this own gameobject. It needs to be an instance if the prefabs should refer to this object. (As enemies, for example)
        private static PlayerBehaviour m_Instance;
        public static PlayerBehaviour PlayerInstance => m_Instance; //A getter for the instance of the player. Similar to get { return m_Instance }. (Accessor)

        //Reference to the InputSystem
        [Header("Reference to the Input System")]
        [SerializeField]
        private InputActionAsset m_InputAsset;
        private InputActionAsset m_Input;
        public InputActionAsset Input => m_Input;
        private InputAction m_MovementAction;
        public InputAction MovementAction => m_MovementAction;

        

        //Player rigidbody
        private Rigidbody2D m_RigidBody;
        //Player animator
        private Animator m_Animator;


        //Animation names
        private const string m_IdleAnimationName = "idle";
        private const string m_WalkAnimationName = "walk";
        private const string m_JumpAnimationName = "jump";
        private const string m_Attack1AnimationName = "attack1";
        private const string m_Attack2AnimationName = "attack2";
        private const string m_Combo1AnimationName = "combo1";
        private const string m_Combo2AnimationName = "combo2";
        private const string m_SuperAnimationName = "super";
        private const string m_CrouchAnimationName = "crouch";
        private const string m_HitAnimationName = "hit";
        private const string m_CrouchAttack1AnimationName = "crouchattack1";
        private const string m_CrouchAttack2AnimationName = "crouchattack2";


        //Variables for the current state and an Enum for setting the Player States
        private enum PlayerMachineStates { NONE, IDLE, WALK, ATTACK1, ATTACK2, COMBO1, COMBO2, SUPER, JUMP, CROUCHATTACK1, CROUCHATTACK2, CROUCH, HIT }
        private PlayerMachineStates m_CurrentState;

        [Header("Player parameters")]
        [SerializeField]
        private float m_MaxHitpoints;
        private float m_Hitpoints;
        public float Hitpoints => m_Hitpoints;
        public float MaxHitpoints => m_MaxHitpoints;
        [SerializeField]
        private float m_MaxEnergy;
        private float m_Energy;
        public float Energy => m_Energy;
        public float MaxEnergy => m_MaxEnergy;
        [SerializeField]
        private float m_Speed;
        [SerializeField]
        private int m_LightDamage;
        public int LightDamage => m_LightDamage;
        [SerializeField]
        private int m_StrongDamage;
        public int StrongDamage => m_StrongDamage;
        [SerializeField]
        private int m_ComboDamage;
        public int ComboDamage => m_ComboDamage;
        [SerializeField]
        private float m_JumpForce;
        [SerializeField]
        private bool m_IsFlipped;
        public bool IsFlipped
        {
            get { return m_IsFlipped;}
        }
        [SerializeField]
        private int m_Combo1EnergyCost;

        [SerializeField]
        private bool m_ComboAvailable;

        [Header("References to another attached objects")]
        [SerializeField]
        private Pool m_BulletPool;

        GameObject m_PlayerHitbox;
        HitboxInfo m_Hitbox;

        [Header("References to GameEvents")]
        [SerializeField]
        private GameEvent m_OnPlayerDamage;
        [SerializeField]
        private GameEvent m_OnEnergyUsed;
        [SerializeField]
        private GameEvent m_OnPlayerDeath;

        //LayerMask of the Pickups for casting it through Physics2D.CircleCast instead of OnTriggerStay
        [SerializeField]
        LayerMask m_PickupLayerMask;

        private void Awake()
        {
            //First, we initialize an instance of Player. If there is already an instance, it destroys the element and returns.
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }          

            //We set the player gameobject rigid body
            m_RigidBody = GetComponent<Rigidbody2D>();
            //We set the player gameobject animator
            m_Animator = GetComponent<Animator>();
            //We can set the Hitbox in a variable to instantiate projectiles from there
            m_PlayerHitbox = this.transform.GetChild(0).gameObject;
            m_Hitbox = m_PlayerHitbox.GetComponent<HitboxInfo>();
            //We set the boolean that will control if the character is flipped as false
            m_IsFlipped = false;
            m_Hitpoints = m_MaxHitpoints;
            m_Energy = m_MaxEnergy;
        }

        private void OnEnable()
        {
            //Setting the input variables. Don't forget to enable.
            Assert.IsNotNull(m_InputAsset);
            m_Input = Instantiate(m_InputAsset);
            m_MovementAction = m_Input.FindActionMap("PlayerActions").FindAction("Movement");
            m_Input.FindActionMap("PlayerActions").FindAction("Attack1").performed += Attack1;
            m_Input.FindActionMap("PlayerActions").FindAction("Attack2").performed += Attack2;
            m_Input.FindActionMap("PlayerActions").FindAction("Jump").performed += Jump;
            m_Input.FindActionMap("PlayerActions").FindAction("Crouch").started += Crouch;
            m_Input.FindActionMap("PlayerActions").FindAction("Crouch").canceled += ReturnToIdleState;
            m_Input.FindActionMap("PlayerActions").Enable();
            InitState(PlayerMachineStates.IDLE);
        }

        private void OnDisable()
        {
            //Disabling the inputs variables and delegates.
            Assert.IsNotNull(m_InputAsset);
            m_Input = Instantiate(m_InputAsset);
            m_MovementAction = m_Input.FindActionMap("PlayerActions").FindAction("Movement");
            m_Input.FindActionMap("PlayerActions").FindAction("Attack1").performed -= Attack1;
            m_Input.FindActionMap("PlayerActions").FindAction("Attack2").performed -= Attack2;
            m_Input.FindActionMap("PlayerActions").FindAction("Jump").performed -= Jump;
            m_Input.FindActionMap("PlayerActions").FindAction("Crouch").started -= Crouch;
            m_Input.FindActionMap("PlayerActions").FindAction("Crouch").canceled -= ReturnToIdleState;
            m_Input.FindActionMap("PlayerActions").Disable();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("EnemyHitbox"))
            {
                PlayerIsDamaged(collision.GetComponent<HitboxInfo>().HitboxDamage);
            }

            if (collision.CompareTag("EnemyProjectile"))
            {
                PlayerIsDamaged(collision.GetComponent<PlayerBulletBehaviour>().BulletDamage);
                Destroy(collision.gameObject);
            }         
        }

        // Start is called before the first frame update
        void Start()
        {
            //In this case, we can use InitState directly instead of ChangeState as it doesn't have to Exit any state previously. 
            InitState(PlayerMachineStates.IDLE);
        }

        // Update is called once per frame
        void Update()
        {
            //Each frame, player behaviour will be listening 
            UpdateState();
        }

        //Combo implementates with a boolean and few functions
        //This public functions can be triggered from the clip events to trigger the begin and end of the combo frame and the end of the hit animation
        public void InitComboWindow() 
        {
            m_ComboAvailable = true;
        }

        public void EndComboWindow() 
        {
            m_ComboAvailable = false;
        }

        public void EndHit()
        {
            ChangeState(PlayerMachineStates.IDLE);
        }

        //Function used to go back to idle state after performing an inputname.canceled action
        private void ReturnToIdleState(InputAction.CallbackContext context)
        {
            ChangeState(PlayerMachineStates.IDLE);
        }

        //Simple function that calls for damage that the player receives
        public void PlayerIsDamaged(int damage)
        {         
            m_Hitpoints -= damage;
            ChangeState(PlayerMachineStates.HIT);
            m_OnPlayerDamage.Raise();
            if (m_Hitpoints <= 0)
                OnPlayerDeath();
        }

        public void PlayerIsHealed(int healedHP)
        {
            m_Hitpoints += healedHP;
            if (m_Hitpoints > m_MaxHitpoints)
                m_Hitpoints = m_MaxHitpoints;
            m_OnPlayerDamage.Raise();
        }

        public void PlayerRecoversMana(int recoveredMana)
        {
            m_Energy += recoveredMana;
            if (m_Energy > m_MaxEnergy)
                m_Energy = m_MaxEnergy;
            m_OnPlayerDamage.Raise();
        }

        private void OnPlayerDeath()
        {
            gameObject.SetActive(false);
            ResetStats();
            m_OnPlayerDeath.Raise();
        }

        private void ResetStats()
        {
            m_Hitpoints = m_MaxHitpoints;
            m_Energy = m_MaxEnergy;
        }

        private void Attack1(InputAction.CallbackContext context)
        {
            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:
                    ChangeState(PlayerMachineStates.ATTACK1);

                    break;

                case PlayerMachineStates.WALK:
                    ChangeState(PlayerMachineStates.ATTACK1);

                    break;

                case PlayerMachineStates.ATTACK1:

                    if (m_ComboAvailable && m_Energy >= m_Combo1EnergyCost)
                        ChangeState(PlayerMachineStates.COMBO1);
                    else
                        ChangeState(PlayerMachineStates.ATTACK1);

                    break;

                case PlayerMachineStates.ATTACK2:

                    if (m_ComboAvailable)
                        ChangeState(PlayerMachineStates.ATTACK1);
                    else
                        ChangeState(PlayerMachineStates.ATTACK2);
                    break;

                case PlayerMachineStates.CROUCH:

                    if (m_ComboAvailable)
                        ChangeState(PlayerMachineStates.CROUCHATTACK2);
                    else
                        ChangeState(PlayerMachineStates.CROUCHATTACK1);

                    break;

                default:
                    break;
            }

        }

        private void Attack2(InputAction.CallbackContext context)
        {
            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:
                    ChangeState(PlayerMachineStates.ATTACK2);

                    break;

                case PlayerMachineStates.WALK:
                    ChangeState(PlayerMachineStates.ATTACK2);

                    break;

                case PlayerMachineStates.ATTACK1:

                    if (m_ComboAvailable)
                        ChangeState(PlayerMachineStates.ATTACK2);
                    else
                        ChangeState(PlayerMachineStates.ATTACK1);

                    break;

                case PlayerMachineStates.ATTACK2:

                    if (m_ComboAvailable)
                        ChangeState(PlayerMachineStates.COMBO2);
                    else
                        ChangeState(PlayerMachineStates.ATTACK2);

                    break;

                case PlayerMachineStates.CROUCH:

                    if (m_ComboAvailable)
                        ChangeState(PlayerMachineStates.CROUCHATTACK1);
                    else
                        ChangeState(PlayerMachineStates.CROUCHATTACK2);

                    break;

                default:
                    break;
            }

        }

        private void Jump(InputAction.CallbackContext context)
        {
            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:
                    if(m_RigidBody.velocity.y == 0)
                        ChangeState(PlayerMachineStates.JUMP);

                    break;

                case PlayerMachineStates.WALK:
                    if (m_RigidBody.velocity.y == 0)
                        ChangeState(PlayerMachineStates.JUMP);

                    break;

                case PlayerMachineStates.ATTACK1:

                    break;

                case PlayerMachineStates.ATTACK2:

                    break;

                default:
                    break;
            }
        }

        private void Crouch(InputAction.CallbackContext context)
        {
            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:
                    ChangeState(PlayerMachineStates.CROUCH);

                    break;

                case PlayerMachineStates.WALK:
                    ChangeState(PlayerMachineStates.CROUCH);

                    break;

                case PlayerMachineStates.ATTACK1:
                    ChangeState(PlayerMachineStates.CROUCH);

                    break;

                case PlayerMachineStates.ATTACK2:
                    ChangeState(PlayerMachineStates.CROUCH);

                    break;

                case PlayerMachineStates.CROUCH:

                    break;

                default:
                    break;
            }
        }



        /* !!! BUILDING UP STATE MACHINE !!! Always change state with the function ChangeState */
        private void ChangeState(PlayerMachineStates newState)
        {
            //if the actual state is the same as the state we are trying to set, it exits the function
            if (newState == m_CurrentState)
                return;
            //First, it will do the actions to exit the current state, then will initiate the new state.
            ExitState();
            InitState(newState);
        }

        /* InitState will run every instruction that has to be started ONLY when enters a state */
        private void InitState(PlayerMachineStates currentState)
        {
            //We declare that the current state of the object is the new state we declare on the function
            m_CurrentState = currentState;
                        
            
            //Then it will compare the current state to run the state actions
            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:

                    m_RigidBody.velocity = Vector3.zero;
                    
                    m_Animator.Play(m_IdleAnimationName);

                    break;

                case PlayerMachineStates.WALK:

                    m_Animator.Play(m_WalkAnimationName);

                    break;

                case PlayerMachineStates.JUMP:

                    m_Animator.Play(m_JumpAnimationName);
                    m_RigidBody.AddForce(Vector2.up * m_JumpForce);

                    break;

                case PlayerMachineStates.ATTACK1:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_Attack1AnimationName);
                    m_Hitbox.SetDamage(m_LightDamage);

                    break;

                case PlayerMachineStates.ATTACK2:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_Attack2AnimationName);
                    m_Hitbox.SetDamage(m_StrongDamage);

                    break;

                case PlayerMachineStates.COMBO1:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_Combo1AnimationName);
                    //It will instance a projectile that will start at the Hitbox position
                    GameObject m_Bullet = m_BulletPool.GetElement();
                    m_Bullet.transform.position = m_PlayerHitbox.transform.position;
                    //It will run the InitBullet function and give the Vector2 direction considering if its flipped or not
                    m_Bullet.GetComponent<PlayerBulletBehaviour>().InitBullet(/* speed,*/m_LightDamage, false, IsFlipped ? Vector2.left: Vector2.right);
                    m_Energy -= m_Combo1EnergyCost;
                    m_OnEnergyUsed.Raise();

                    break;

                case PlayerMachineStates.COMBO2:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_Combo2AnimationName);
                    m_Hitbox.SetDamage(m_ComboDamage);

                    break;

                case PlayerMachineStates.HIT:
                    //Will play the animation and then set the state to Idle
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_HitAnimationName);

                    break;

                case PlayerMachineStates.CROUCH:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_CrouchAnimationName);

                    break;

                case PlayerMachineStates.CROUCHATTACK1:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_CrouchAttack1AnimationName);

                    break;

                case PlayerMachineStates.CROUCHATTACK2:
                    //Attack will set the velocity to zero, so it cant move while attacking
                    m_RigidBody.velocity = Vector3.zero;
                    m_Animator.Play(m_CrouchAttack2AnimationName);

                    break;

                default:
                    break;
            }
        }

        /* ExitState will run every instruction that has to be started ONLY when exits a state */
        private void ExitState()
        {
            switch(m_CurrentState)
            {
                case PlayerMachineStates.IDLE:

                    break;

                case PlayerMachineStates.WALK:

                    break;

                case PlayerMachineStates.JUMP:

                    break;

                case PlayerMachineStates.ATTACK1:

                    break;

                case PlayerMachineStates.ATTACK2:

                    break;

                case PlayerMachineStates.CROUCH:

                    break;

                default:
                    break;
            }
        }

        /* UpdateState will control every frame since it will be called from Update() and will control when it changes the state */
        private void UpdateState()
        {
            m_RigidBody.transform.eulerAngles = m_IsFlipped ? Vector3.up * 180 : Vector3.zero;

            switch (m_CurrentState)
            {
                case PlayerMachineStates.IDLE:

                    if (m_MovementAction.ReadValue<Vector2>().x != 0) { 
                        if(m_MovementAction.ReadValue<Vector2>().x < 0)
                          m_IsFlipped = true;
                        if (m_MovementAction.ReadValue<Vector2>().x > 0)
                          m_IsFlipped = false;
                        ChangeState(PlayerMachineStates.WALK);
                    }

                    break;

                case PlayerMachineStates.WALK:

                    m_RigidBody.velocity = new Vector2(m_MovementAction.ReadValue<Vector2>().x * m_Speed, m_RigidBody.velocity.y); 

                    if (m_RigidBody.velocity == Vector2.zero)
                        ChangeState(PlayerMachineStates.IDLE);

                    break;

                case PlayerMachineStates.JUMP:

                    if (m_RigidBody.velocity == Vector2.zero)
                        ChangeState(PlayerMachineStates.IDLE);

                    break;

                case PlayerMachineStates.CROUCH:

                    if (m_MovementAction.ReadValue<Vector2>().x < 0)
                        m_IsFlipped = true;
                    if (m_MovementAction.ReadValue<Vector2>().x > 0)
                        m_IsFlipped = false;

                    //This gets the gameobject of the pickup, just as it would do in OnTriggerEnter/Stay, but with less load since it's a "Raycast"
                    if (Physics2D.CircleCast(transform.position, 0.5f, Vector2.up, 0.5f, m_PickupLayerMask)) {
                        GameObject pickup = Physics2D.CircleCast(transform.position, 0.5f, Vector2.up, 0.5f, m_PickupLayerMask).collider.gameObject;
                        pickup.GetComponent<PickupBehaviour>().GetPickup();
                        Destroy(pickup.gameObject);
                    }
                    break;

                default:
                    break;
            }
        }

        /* !!! FINISHING THE BUILD OF THE STATE MACHINE !!! */

    }
}


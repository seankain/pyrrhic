using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PyrrhicPlayerMovement : NetworkBehaviour
{

    [Serializable]
    public class MovementSettings
    {
        public float ForwardSpeed = 8.0f;   // Speed when walking forward
        public float BackwardSpeed = 4.0f;  // Speed when walking backwards
        public float StrafeSpeed = 4.0f;    // Speed when walking sideways
        public float RunMultiplier = 2.0f;   // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce = 30f;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        [HideInInspector] public float CurrentTargetSpeed = 8f;
#if !MOBILE_INPUT
        private bool m_Running;
#endif

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //strafe
                CurrentTargetSpeed = StrafeSpeed;
            }
            if (input.y < 0)
            {
                //backwards
                CurrentTargetSpeed = BackwardSpeed;
            }
            if (input.y > 0)
            {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                CurrentTargetSpeed = ForwardSpeed;
            }
#if !MOBILE_INPUT
            if (Input.GetKey(RunKey))
            {
                CurrentTargetSpeed *= RunMultiplier;
                m_Running = true;
            }
            else
            {
                m_Running = false;
            }
#endif
        }

#if !MOBILE_INPUT
        public bool Running
        {
            get { return m_Running; }
        }
#endif
    }


    [Serializable]
    public class AdvancedSettings
    {
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f; // stops the character
        public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        public bool airControl; // can the user control the direction that is being moved in the air
        [Tooltip("set it to 0.1 or more if you get stuck in wall")]
        public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
    }


    public Camera cam;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();


    private Rigidbody m_RigidBody;
    private CapsuleCollider m_Capsule;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal;
    private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
    private Animator anim;

    public Vector3 Velocity
    {
        get { return m_RigidBody.velocity; }
    }

    public bool Grounded
    {
        get { return m_IsGrounded; }
    }

    public bool Jumping
    {
        get { return m_Jumping; }
    }

    public bool Running
    {
        get
        {
#if !MOBILE_INPUT
            return movementSettings.Running;
#else
	            return false;
#endif
        }
    }

    private void Start()
    {
        if (!isLocalPlayer) { return; }
        m_RigidBody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        mouseLook.Init(transform, cam.transform);
        anim = GetComponentInChildren<Animator>();
    }


    private void Update()
    {
        if (!isLocalPlayer) { return; }
        //RotateView();
        anim.SetFloat("Speed", this.m_RigidBody.velocity.sqrMagnitude);
        anim.SetFloat("Direction", Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Jump") && !m_Jump)
        {
            m_Jump = true;
        }
    }


    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }
        GroundCheck();
        Vector2 input = GetInput();

        if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
        {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

            desiredMove.x = desiredMove.x * movementSettings.CurrentTargetSpeed;
            desiredMove.z = desiredMove.z * movementSettings.CurrentTargetSpeed;
            desiredMove.y = desiredMove.y * movementSettings.CurrentTargetSpeed;
            if (m_RigidBody.velocity.sqrMagnitude <
                (movementSettings.CurrentTargetSpeed * movementSettings.CurrentTargetSpeed))
            {
                m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
            }
        }

        if (m_IsGrounded)
        {
            m_RigidBody.drag = 5f;

            if (m_Jump)
            {
                m_RigidBody.drag = 0f;
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                m_Jumping = true;
            }

            if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
            {
                m_RigidBody.Sleep();
            }
        }
        else
        {
            m_RigidBody.drag = 0f;
            if (m_PreviouslyGrounded && !m_Jumping)
            {
                StickToGroundHelper();
            }
        }
        m_Jump = false;
    }


    private float SlopeMultiplier()
    {
        float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
        return movementSettings.SlopeCurveModifier.Evaluate(angle);
    }


    private void StickToGroundHelper()
    {
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) +
                               advancedSettings.stickToGroundHelperDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
            {
                m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
            }
        }
    }


    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }


    private void RotateView()
    {
        //avoids the mouse looking if the game is effectively paused
        if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

        // get the rotation before it's changed
        float oldYRotation = transform.eulerAngles.y;

        mouseLook.LookRotation(transform, cam.transform);

        if (m_IsGrounded || advancedSettings.airControl)
        {
            // Rotate the rigidbody velocity to match the new direction that the character is looking
            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
        }
    }

    /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
    private void GroundCheck()
    {
        m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                               ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            m_IsGrounded = true;
            m_GroundContactNormal = hitInfo.normal;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
        {
            m_Jumping = false;
        }
    }
    #region bullshit
    //   public float MaxSpeed = 3.0f;
    //   public float MoveSpeed = 3.0f;
    //   public float StrafeSpeed = 3.0f;
    //   float deltaTime = 0.0f;

    //   public Transform playerViewCamera;

    //   [Header("Z Rotation Camera")]
    //   [HideInInspector] public float timer;
    //   [HideInInspector] public int int_timer;
    //   [HideInInspector] public float zRotation;
    //   [HideInInspector] public float wantedZ;
    //   [HideInInspector] public float timeSpeed = 2;
    //   [HideInInspector] public float timerToRotateZ;
    //   [Tooltip("Current mouse sensivity, changes in the weapon properties")]
    //   public float mouseSensitvity = 0;
    //   [HideInInspector]
    //   public float mouseSensitvity_notAiming = 300;
    //   [HideInInspector]
    //   public float mouseSensitvity_aiming = 50;
    //   private float rotationYVelocity, cameraXVelocity;
    //   [Tooltip("Speed that determines how much camera rotation will lag behind mouse movement.")]
    //   public float yRotationSpeed, xCameraSpeed;
    //   [HideInInspector]
    //   public float wantedYRotation;
    //   [HideInInspector]
    //   public float currentYRotation;
    //   [HideInInspector]
    //   public float wantedCameraXRotation;
    //   [HideInInspector]
    //   public float currentCameraXRotation;
    //   [Tooltip("Top camera angle.")]
    //   public float topAngleView = 60;
    //   [Tooltip("Minimum camera angle.")]
    //   public float bottomAngleView = -45;

    //   //Movement fields
    //   Rigidbody rb;

    //   [Tooltip("Current players speed")]
    //   public float currentSpeed;
    //   [Tooltip("Assign players camera here")]
    //   [HideInInspector] public Transform cameraMain;
    //   [Tooltip("Force that moves player into jump")]
    //   public float jumpForce = 100;
    //   [Tooltip("Position of the camera inside the player")]
    //   [HideInInspector] public Vector3 cameraPosition;
    //   private Vector3 slowdownV;
    //   private Vector2 horizontalMovement;
    //   [Tooltip("The maximum speed you want to achieve")]
    //   public int maxSpeed = 5;
    //   [Tooltip("The higher the number the faster it will stop")]
    //   public float deaccelerationSpeed = 15.0f;
    //   [Tooltip("Force that is applied when moving forward or backward")]
    //   public float accelerationSpeed = 50000.0f;
    //   [Tooltip("Tells us weather the player is grounded or not.")]
    //   public bool grounded;
    //   RaycastHit hitInfo;
    //   private float meleeAttack_cooldown;
    //   private string currentWeapo;
    //   [Tooltip("Put 'Player' layer here")]
    //   [Header("Shooting Properties")]
    //   private LayerMask ignoreLayer;//to ignore player layer
    //   Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
    //   private float rayDetectorMeeleSpace = 0.15f;
    //   private float offsetStart = 0.05f;
    //   [Tooltip("Put BulletSpawn gameobject here, palce from where bullets are created.")]
    //   [HideInInspector]
    //   public Transform bulletSpawn; 

    //   [Header("Player SOUNDS")]
    //   [Tooltip("Jump sound when player jumps.")]
    //   public AudioSource _jumpSound;
    //   [Tooltip("Sound while player makes when successfully reloads weapon.")]

    //   public AudioSource _hitSound;
    //   [Tooltip("Walk sound player makes.")]
    //   public AudioSource _walkSound;
    //   [Tooltip("Run Sound player makes.")]
    //   public AudioSource _runSound;



    //   /*
    // * Hiding the cursor.
    // */
    //   void Awake()
    //   {
    //       Cursor.lockState = CursorLockMode.Locked;
    //       //myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    //       playerViewCamera = gameObject.GetComponentInChildren<Camera>().transform;
    //       rb = GetComponent<Rigidbody>();
    //   }

    //   /*
    //* Locking the mouse if pressing L.
    //* Triggering the headbob camera omvement if player is faster than 1 of speed
    //*/
    //   void Update()
    //   {
    //       if (!isLocalPlayer) { return; }

    //       MouseInputMovement();

    //       if (Input.GetKeyDown(KeyCode.L))
    //       {
    //           Cursor.lockState = CursorLockMode.Locked;

    //       }
    //       deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

    //       if (currentSpeed > 1)
    //       {
    //           HeadMovement();
    //       }


    //       Jumping();

    //       Crouching();

    //       WalkingSound();

    //   }

    //   /*
    //* Switching Z rotation and applying to camera in camera Rotation().
    //*/
    //   void HeadMovement()
    //   {
    //       timer += timeSpeed * Time.deltaTime;
    //       int_timer = Mathf.RoundToInt(timer);
    //       if (int_timer % 2 == 0)
    //       {
    //           wantedZ = -1;
    //       }
    //       else
    //       {
    //           wantedZ = 1;
    //       }

    //       zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
    //   }

    //   /*
    //   * FixedUpdate()
    //   * If aiming set the mouse sensitvity from our variables and vice versa.
    //   */
    //   void FixedUpdate()
    //   {
    //       if (!isLocalPlayer) { return; }
    //       //TODO bring back when refactoring melee attacks
    //       //RaycastForMeleeAttacks();

    //       PlayerMovementLogic();

    //       /*
    //        * Reduxing mouse sensitvity if we are aiming.
    //        */
    //       if (Input.GetAxis("Fire2") != 0)
    //       {
    //           mouseSensitvity = mouseSensitvity_aiming;
    //       }
    //       else if (MaxSpeed > 5)
    //       {
    //           mouseSensitvity = mouseSensitvity_notAiming;
    //       }
    //       else
    //       {
    //           mouseSensitvity = mouseSensitvity_notAiming;
    //       }

    //       ApplySmoothing();
    //   }

    //   /*
    //    * Upon mouse movenet it increases/decreased wanted value. (not actually moving yet)
    //    * Clamping the camera rotation X to top and bottom angles.
    //    */
    //   void MouseInputMovement()
    //   {
    //       wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;
    //       wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;
    //       wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
    //   }

    //   /*
    //* Accordingly to input adds force and if magnitude is bigger it will clamp it.
    //* If player leaves keys it will deaccelerate
    //*/
    //   void PlayerMovementLogic()
    //   {
    //       currentSpeed = rb.velocity.magnitude;
    //       horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);
    //       if (horizontalMovement.magnitude > maxSpeed)
    //       {
    //           horizontalMovement = horizontalMovement.normalized;
    //           horizontalMovement *= maxSpeed;
    //       }
    //       rb.velocity = new Vector3(
    //           horizontalMovement.x,
    //           rb.velocity.y,
    //           horizontalMovement.y
    //       );
    //       if (grounded)
    //       {
    //           rb.velocity = Vector3.SmoothDamp(rb.velocity,
    //               new Vector3(0, rb.velocity.y, 0),
    //               ref slowdownV,
    //               deaccelerationSpeed);
    //       }

    //       if (grounded)
    //       {
    //           rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed * Time.deltaTime);
    //       }
    //       else
    //       {
    //           rb.AddRelativeForce(Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime, 0, Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime);

    //       }
    //       /*
    //	 * Slippery issues fixed here
    //	 */
    //       if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
    //       {
    //           deaccelerationSpeed = 0.5f;
    //       }
    //       else
    //       {
    //           deaccelerationSpeed = 0.1f;
    //       }
    //   }
    //   /*
    //* Handles jumping and ads the force and sounds.
    //*/
    //   void Jumping()
    //   {
    //       if (Input.GetKeyDown(KeyCode.Space) && grounded)
    //       {
    //           rb.AddRelativeForce(Vector3.up * jumpForce);
    //           if (_jumpSound)
    //               _jumpSound.Play();
    //           else
    //               print("Missig jump sound.");
    //           _walkSound.Stop();
    //           _runSound.Stop();
    //       }
    //   }

    //   /*
    //* Checks if player is grounded and plays the sound accorindlgy to his speed
    //*/
    //   void WalkingSound()
    //   {
    //       if (_walkSound && _runSound)
    //       {
    //           if (RayCastGrounded())
    //           { //for walk sounsd using this because suraface is not straigh			
    //               if (currentSpeed > 1)
    //               {
    //                   //				print ("unutra sam");
    //                   if (maxSpeed == 3)
    //                   {
    //                       //	print ("tu sem");
    //                       if (!_walkSound.isPlaying)
    //                       {
    //                           //	print ("playam hod");
    //                           _walkSound.Play();
    //                           _runSound.Stop();
    //                       }
    //                   }
    //                   else if (maxSpeed == 5)
    //                   {
    //                       //	print ("NE tu sem");

    //                       if (!_runSound.isPlaying)
    //                       {
    //                           _walkSound.Stop();
    //                           _runSound.Play();
    //                       }
    //                   }
    //               }
    //               else
    //               {
    //                   _walkSound.Stop();
    //                   _runSound.Stop();
    //               }
    //           }
    //           else
    //           {
    //               _walkSound.Stop();
    //               _runSound.Stop();
    //           }
    //       }
    //       else
    //       {
    //           print("Missing walk and running sounds.");
    //       }

    //   }
    //   /*
    //* Raycasts down to check if we are grounded along the gorunded method() because if the
    //* floor is curvy it will go ON/OFF constatly this assures us if we are really grounded
    //*/
    //   private bool RayCastGrounded()
    //   {
    //       RaycastHit groundedInfo;
    //       if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
    //       {
    //           Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);
    //           if (groundedInfo.transform != null)
    //           {
    //               //print ("vracam true");
    //               return true;
    //           }
    //           else
    //           {
    //               //print ("vracam false");
    //               return false;
    //           }
    //       }
    //       //print ("nisam if dosao");

    //       return false;
    //   }

    //   /*
    //* If player toggle the crouch it will scale the player to appear that is crouching
    //*/
    //   void Crouching()
    //   {
    //       if (Input.GetKey(KeyCode.C))
    //       {
    //           transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
    //       }
    //       else
    //       {
    //           transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);

    //       }
    //   }

    //   /*
    //* checks if our player is contacting the ground in the angle less than 60 degrees
    //*	if it is, set groudede to true
    //*/
    //   void OnCollisionStay(Collision other)
    //   {
    //       foreach (ContactPoint contact in other.contacts)
    //       {
    //           if (Vector2.Angle(contact.normal, Vector3.up) < 60)
    //           {
    //               grounded = true;
    //           }
    //       }
    //   }
    //   /*
    //* On collision exit set grounded to false
    //*/
    //   void OnCollisionExit()
    //   {
    //       grounded = false;
    //   }


    //   /*
    //    * Smoothing the wanted movement.
    //    * Calling the weaponRotation from here, we are rotating the weapon from this script.
    //    * Applying the camera wanted rotation to its transform.
    //    */
    //   void ApplySmoothing()
    //   {

    //       currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
    //       currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);
    //       //TODO bring back when refacforing weapon scripts
    //       //WeaponRotation();

    //       transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    //       playerViewCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);

    //   }

    //   //void WeaponRotation()
    //   //{
    //   //    if (!weapon)
    //   //    {
    //   //        weapon = GameObject.FindGameObjectWithTag("Weapon");
    //   //        if (weapon)
    //   //        {
    //   //            if (weapon.GetComponent<GunScript>())
    //   //            {
    //   //                try
    //   //                {
    //   //                    gun = GameObject.FindGameObjectWithTag("Weapon").GetComponent<GunScript>();
    //   //                }
    //   //                catch (System.Exception ex)
    //   //                {
    //   //                    print("gun not found->" + ex.StackTrace.ToString());
    //   //                }
    //   //            }
    //   //        }
    //   //    }

    //   //}
    #endregion bullshit
}

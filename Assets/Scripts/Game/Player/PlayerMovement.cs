using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour // old code
{

    private CharacterController Controller;

    public Camera CharacterCamera;

    public AudioSource SoundSource;

    public float Gravity = 1f;

    public float MoveSpeed = 6;

    public FixedJoystick MoveInput;

    public float JumpSpeed = 10;

    public string JumpInput = "Jump";

    public float JumpReuse = 0.1f;

    public ActionSoundController FootstepController;

    private float InputVert;

    private float InputHor;

    private float tempJumpSpeed;

    private float jt;

    private float TheX;

    private float TheZ;

    private float TheY;

    private Vector3 TheVector;

    private Vector3 _velocity;

    private bool _jump;

    private bool JumpBlock;

    private float ContrH;

    private float _timeOnFly;

    private bool _isMobile = true;

    private bool _isLadder;

    private bool _isFlying;

    // autojump
    private bool _isReadyAutoJump;

    // footsteps
    private World _world;

    public static PlayerMovement Instance;

    public void MobileJump()
    {
        _jump = true;
    }
    public void StopMobileJump()
    {
        _jump = false;
    }
    void Start()
    {
        Instance = this;
        // Check if the mobile device

        if (Application.isEditor == false && Application.platform == RuntimePlatform.WindowsPlayer)
        {
            _isMobile = false;
        }
        else
        {
            _isMobile = true;
        }


        if (GetComponent<CharacterController>() == null)
        {
            Debug.LogError("Component CharacterController not found!");
        }
        else
        {
            Controller = GetComponent<CharacterController>();
        }
        _world = World.Instance;
    }


    void Update()
    {
        RaycastHit ForwardBlock;
        RaycastHit DownBlock;
        // RaycastHit UpBlock; for ivy
        bool isDownBlock = Physics.Raycast(Controller.transform.position + new Vector3(0, 0.84f, 0), -Controller.transform.up, out DownBlock, 1f);

        _isReadyAutoJump = Physics.Raycast(Controller.transform.position + new Vector3(0, -0.75f, 0), Controller.transform.forward, out ForwardBlock, 1.2f);

        if (isDownBlock)
        {
            //  _player.isWaitToBuild = true;
            JumpBlock = true;
            tempJumpSpeed = 7;
        }
        else
        {
            // _player.isWaitToBuild = false;
        }

        if (_isReadyAutoJump && Controller.isGrounded && InputVert != 0)
        {
            JumpBlock = true;
            tempJumpSpeed = JumpSpeed;
        }

        //Check block
        _isReadyAutoJump = _isReadyAutoJump = Physics.Raycast(Controller.transform.position + new Vector3(0, -0.75f, 0), Controller.transform.forward, out ForwardBlock, 0.5f);
        if (_isReadyAutoJump == true)
        {
            Vector3 final = ForwardBlock.point - (ForwardBlock.normal * 0.5f);
            int x = Mathf.FloorToInt(final.x);
            int y = Mathf.FloorToInt(final.y);
            int z = Mathf.FloorToInt(final.z);

            if (World.Instance.GetBlock(x, y, z).BlockType == BlockType.Ladder || World.Instance.GetBlock(x, y, z).BlockType == BlockType.Ivy) // Ladder logic
            {
                _isLadder = true;
            }

        }
        else
        {
            if (InputVert != 0)
            {
                _isLadder = false;
            }
        }

        if (!_isLadder && !isDownBlock)
        {
            Gravity = 0.6f;
        }

        if (_isLadder || isDownBlock)
        {
            Gravity = 0.0f;
            if (_isLadder)
            {
                JumpBlock = true;
                tempJumpSpeed = Mathf.Lerp(tempJumpSpeed, InputVert * 5, 0.2f);
            }
        }

        if (Controller.isGrounded && JumpBlock == false)
        {
            tempJumpSpeed = 0;
        }
        if (_isMobile == true)
        {
            if (!Controller.isGrounded)
            {
                if (!_isLadder)
                {
                    _timeOnFly = Mathf.MoveTowards(_timeOnFly, 2, 0.02f);
                }
                else
                {
                    _timeOnFly = Mathf.MoveTowards(_timeOnFly, 0.5f, 0.04f);
                }
                InputVert = MoveInput.Direction.y * _timeOnFly;
                if (!_isLadder)
                {
                    InputHor = MoveInput.Direction.x * _timeOnFly;
                }
                else
                {
                    InputHor = MoveInput.Direction.x * _timeOnFly / 2;
                }
            }
            else
            {
                _timeOnFly = Mathf.MoveTowards(_timeOnFly, 0.5f, 0.04f);
                InputVert = MoveInput.Direction.y;
                InputHor = MoveInput.Direction.x;
            }
        }
        else
        {
            InputVert = Input.GetAxis("Vertical");
            InputHor = Input.GetAxis("Horizontal");
        }

        if (Input.GetButtonDown(JumpInput) || _jump && JumpBlock == false)
        {
            if (Controller.isGrounded)
            {
                _jump = false;
                JumpBlock = true;
                tempJumpSpeed = JumpSpeed;
            }
        }
        _jump = false;


        if (JumpBlock == true)
        {
            jt += Time.deltaTime;
            if (jt > JumpReuse)
            {
                JumpBlock = false;
                jt = 0;
            }
        }

        if (Controller.isGrounded && TheX != 0 && TheZ != 0) // footsteps
        {
            if (Time.frameCount % 30 == 0)
            {
                PlayFootstep((int)Controller.transform.position.x, (int)Controller.transform.position.y, (int)Controller.transform.position.z);
            }
        }

        if (transform.position.y < -5) // If the player fell under the world
        {
            Controller.enabled = false;
            transform.position = new Vector3(transform.position.x, 60, transform.position.z);
        }
        else
        {
            Controller.enabled = true;
        }
    }

    public void PlayFootstep(int x, int y, int z)
    {
        if (_world.GetBlock(x, y, z) != null && _world.GetBlock(x, y, z).BlockType != BlockType.Air)
        {
            BlockSet.BlockSettings settings;
            BlockSet.Blocks.TryGetValue(_world.GetBlock(x, y, z).BlockType, out settings);
            if (settings != null)
            {
                FootstepController.PlayRandomSound(settings.BlockSound, Constants.Action.Footstep);
            }
        }
    }

    public void Explode(Vector3 Direction, float Power)
    {
        PlayerHealth.Instance.PlayDamageEffect();
        // _velocity = Direction * Power;
        _jump = false;
        JumpBlock = true;
        tempJumpSpeed = 20;
    }

    public void Teleport(Vector3 pos)
    {
        Controller.enabled = false;
        transform.position = pos;
        Invoke("EnableController", 0.5f);
    }

    void EnableController()
    {
        Controller.enabled = true;
    }

    void FixedUpdate()
    {
        if (Application.isEditor && Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            InputVert = Input.GetAxis("Vertical");
            InputHor = Input.GetAxis("Horizontal");
        }
        TheVector = Vector3.zero;

        TheX = MoveSpeed * Time.fixedDeltaTime * InputHor;
        if (!_isLadder)
        {
            TheZ = MoveSpeed * Time.fixedDeltaTime * InputVert;
        }
        else
        {
            TheZ = 0;
        }

        if (Physics.Raycast(Controller.transform.position, Controller.transform.up, Controller.height * 0.55f))
        {
            tempJumpSpeed = 0;
        }

        tempJumpSpeed -= Gravity;
        TheY = Time.fixedDeltaTime * tempJumpSpeed;
        _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, 0.2f);
        TheVector = new Vector3(TheX, TheY, TheZ);
        TheVector = Controller.transform.TransformDirection(TheVector);
        Controller.Move(TheVector + _velocity);
    }
}

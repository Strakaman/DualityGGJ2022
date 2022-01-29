using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Move the player charactercontroller based on horizontal and vertical axis input
    /// </summary>

    [Range(5f,25f)]
    public float gravity = -9.81f;
    //the speed of the player movement
    [Range(5f,15f)]
    public float movementSpeed = 5f;
    public float gamepadDeadzone = 0.19f;
    public float gamepadRotateSmooting = 1000f;

    //the charachtercompononet for moving us
    CharacterController cc;
    DigiPlayerAnimation animatorScript;

    public Vector3 velocity;


    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animatorScript = GetComponent<DigiPlayerAnimation>();
        if (!photonView.IsMine)
        {
            GetComponentInChildren<AudioListener>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Look();
            Move();
        }
    }

    void Look()
    {
        if (InputManager.instance.isGamepad)
        {
            if (Mathf.Abs(InputManager.instance.horizontalLookAxis) > gamepadDeadzone || Mathf.Abs(InputManager.instance.verticalLookAxis) > gamepadDeadzone)
            {
                Vector3 playerDirection = Vector3.right * InputManager.instance.horizontalLookAxis + Vector3.right * InputManager.instance.verticalLookAxis;
                if (playerDirection.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, gamepadRotateSmooting * Time.deltaTime);
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(InputManager.instance.horizontalLookAxis, InputManager.instance.verticalLookAxis));
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    void Move()
    {
        float xMovement = InputManager.instance.horizontalMovement;
        float zMovement = InputManager.instance.verticalMovement;
        Vector3 move = new Vector3(xMovement, 0, zMovement);
        cc.Move(move * Time.deltaTime * movementSpeed);

        velocity.y -= gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
        if (Mathf.Abs(xMovement) > Mathf.Epsilon || Mathf.Abs(zMovement) > Mathf.Epsilon)
        {
            animatorScript.isWalking = true;
        }
        else
        {
            animatorScript.isWalking = false;
        }
    }

    void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }
}

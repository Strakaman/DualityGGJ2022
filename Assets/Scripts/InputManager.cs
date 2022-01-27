using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // A global instance for scripts to reference
    public static InputManager instance;

    public float horizontalMovement;
    public float verticalMovement;
    public float horizontalLookAxis;
    public float verticalLookAxis;

    public bool fireStarted = false;
    public bool switchTeamsStarted = false;
    public bool pauseStarted = false;
    public bool statsStarted = false;

    public bool isGamepad;

    private void Awake()
    {
        ResetValuesToDefault();
        // Set up the instance of this
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Sets all the input variables to their default values so that nothing weird happens in the game if you accidentally
    /// set them in the editor
    /// </summary>
    void ResetValuesToDefault()
    {
        horizontalMovement = default;
        verticalMovement = default;

        horizontalLookAxis = default;
        verticalLookAxis = default;

        fireStarted = default;
        switchTeamsStarted = default;
        pauseStarted = default;
        statsStarted = default;
    }


    public void GetMovementInput(InputAction.CallbackContext callbackContext)
    {
        Vector2 movementVector = callbackContext.ReadValue<Vector2>();
        horizontalMovement = movementVector.x;
        verticalMovement = movementVector.y;
    }


    public void GetMouseLookInput(InputAction.CallbackContext callbackContext)
    {
        Vector2 mouseLookVector = callbackContext.ReadValue<Vector2>();
        horizontalLookAxis = mouseLookVector.x;
        verticalLookAxis = mouseLookVector.y;
    }


    public void GetFireInput(InputAction.CallbackContext callbackContext)
    {
        fireStarted = !callbackContext.canceled;
        if (InputManager.instance.isActiveAndEnabled)
        {
            StartCoroutine("ResetFireStart");
        }
    }

    private IEnumerator ResetFireStart()
    {
        yield return new WaitForEndOfFrame();
        fireStarted = false;
    }


    public void GetSwitchTeamsInput(InputAction.CallbackContext callbackContext)
    {
        switchTeamsStarted = !callbackContext.canceled;
        if (InputManager.instance.isActiveAndEnabled)
        {
            StartCoroutine("ResetSwitchTeamsStart");
        }
    }

    private IEnumerator ResetSwitchTeamsStart()
    {
        yield return new WaitForEndOfFrame();
        switchTeamsStarted = false;
    }


    public void GetPauseInput(InputAction.CallbackContext callbackContext)
    {
        pauseStarted = !callbackContext.canceled;
        if (InputManager.instance.isActiveAndEnabled)
        {
            StartCoroutine("ResetPauseStart");
        }
    }

    private IEnumerator ResetPauseStart()
    {
        yield return new WaitForEndOfFrame();
        pauseStarted = false;
    }

    public void GetStatsInput(InputAction.CallbackContext callbackContext)
    {
        statsStarted = !callbackContext.canceled;
        if (InputManager.instance.isActiveAndEnabled)
        {
            StartCoroutine("ResetStatsStart");
        }
    }

    private IEnumerator ResetStatsStart()
    {
        yield return new WaitForEndOfFrame();
        statsStarted = false;
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        isGamepad = pi.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}

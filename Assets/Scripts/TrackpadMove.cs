using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(FindController))]
public class TrackpadMove : MonoBehaviour
{
    SteamVR_Controller.Device controller;
    CharacterMotor motor;

    private float forwardInputDelay = 10.0f;
    private bool running = false;

    // Use this for initialization
    void Start()
    {
        motor = GetComponent<CharacterMotor>();

        FindController find = GetComponent<FindController>();
        find.OnControllerFound += GetController;
    }

    void GetController()
    {
        controller = GetComponent<FindController>().Controller;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            return;
        }

        Vector2 axis = controller.GetAxis();
        Vector3 direction = new Vector3(axis.x, 0.0f, axis.y);

        if (controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (forwardInputDelay < 0.4) // TODO: && !aimscript.isaiming
            {
                motor.running = Mathf.Clamp((0.4f - forwardInputDelay) / 0.2f, 0.01f, 1.0f);
                running = true;
            }
            forwardInputDelay = 0.0f;
        }
        forwardInputDelay += Time.deltaTime;
        if (forwardInputDelay > 0.4f) // TODO: || aimscript.isaiming
        {
            motor.running = 0.0f;
            running = false;
        }
        if (running)
        {
            direction.z = 1.0f;
        }

        if (direction != Vector3.zero)
        {
            float magnitude = direction.magnitude;
            direction /= magnitude;
            magnitude = Mathf.Min(1, magnitude);
            magnitude *= magnitude;
            direction *= magnitude;
        }

        Vector3 relativeDirection = controller.transform.rot * direction;
        motor.inputMoveDirection = transform.rotation * relativeDirection;
        motor.inputJump = controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }
}
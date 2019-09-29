using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        Vector3 movement = transform.TransformDirection(direction);
        controller.Move(movement * speed * Time.deltaTime);
    }
}
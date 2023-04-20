using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomInputManager : MonoBehaviour
{
    public static CustomInputManager Instance;
    [SerializeField] PlayerInput AdminInput;
    [SerializeField] PlayerInput PlayerInput;

    public InputAction KeyboardC {get; private set;}
    public InputAction MoveKeys {get; private set;}
    public InputAction KeyboardDot {get; private set;}
    public InputAction LeftShift {get; private set;}
    public InputAction LeftControl {get; private set;}
    public InputAction Hotbar1 {get; private set;}
    public InputAction Hotbar2 {get; private set;}
    public InputAction Hotbar3 {get; private set;}
    public InputAction Hotbar4 {get; private set;}
    public InputAction Hotbar5 {get; private set;}
    public InputAction Hotbar6 {get; private set;}
    public InputAction Hotbar7 {get; private set;}
    public InputAction Hotbar8 {get; private set;}
    public InputAction Interact {get; private set;}
    public InputAction Tab {get; private set;}

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 instance of Input manager found");
            Destroy(this); ;
            return;
        }
        Instance = this;
        KeyboardC = AdminInput.actions["KeyboardC"];
        KeyboardDot = AdminInput.actions["KeyboardDot"];
        LeftShift = PlayerInput.actions["Sprint"];
        LeftControl = PlayerInput.actions["LeftControl"];
        MoveKeys = PlayerInput.actions["Move"];
        Tab = PlayerInput.actions["ToggleInventory"];
        Hotbar1= PlayerInput.actions["Hotbar1"];
        Hotbar2= PlayerInput.actions["Hotbar2"];
        Hotbar3= PlayerInput.actions["Hotbar3"];
        Hotbar4= PlayerInput.actions["Hotbar4"];
        Hotbar5= PlayerInput.actions["Hotbar5"];
        Hotbar6= PlayerInput.actions["Hotbar6"];
        Hotbar7= PlayerInput.actions["Hotbar7"];
        Hotbar8= PlayerInput.actions["Hotbar8"];
        Interact = PlayerInput.actions["Interact"];

    }



}

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;

public class TypingController : UdonSharpBehaviour
{
    public KeyboardManager2 manager;
    public ChatController chatController;
    private InputField input;
    private bool focused;

    public void Start()
    {
        input = GetComponent<InputField>();
    }

    public void Update()
    {
        if (focused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            chatController.SendMessage();
            input.DeactivateInputField();
        }

        if (input.isFocused)
        {
            if (!focused)
            {
                focused = true;
                manager.Immobilize();
            }
        }
        else if (!input.isFocused && focused)
        {
            focused = false;
            manager.UnImmobilize();
        }
    }

}
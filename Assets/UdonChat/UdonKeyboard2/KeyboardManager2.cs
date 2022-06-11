using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEngine.UI;

///<Summary>The manager for a keyboard.</Summary>
public class KeyboardManager2 : UdonSharpBehaviour
{
    [Tooltip("True: UdonChat is placed in a static location in the world. False: UdonChat follows the player.")]
    public bool isStaticScreen;

    /// <summary>
    /// The anchor on this keyboard for putting a Logger on it.
    /// </summary>
    [Tooltip("The anchor on this keyboard for putting a Logger on it.")]
    public GameObject keyboardAnchor;

    /// <summary>
    /// The root of a Logger, used for anchoring it to the keyboard.
    /// </summary>
    [Tooltip("The root of a Logger, used for anchoring it to the keyboard.")]
    public UdonLogger logScreen;

    /// <summary>
    /// The input field for this keyboard.
    /// </summary>
    [Tooltip("The input field for this keyboard.")]
    public InputField input;

    /// <summary>
    /// All the keyboard keys this keyboard has.
    /// </summary>
    [Tooltip("All the keyboard keys this keyboard has.")]
    public Button[] keys;

    /// <summary>
    /// The field that drives text updates.
    /// </summary>
    [Tooltip("The field that drives text updates.")]
    public InputField field;

    public Image immobilizedButtonImage;

    public Image capsButtonImage;

    public Image shiftButtonImage;

    private bool caps;
    private bool shift;
    private bool visible;
    private bool immobilized;

    public void Start()
    {
        if (!isStaticScreen)
        {
            gameObject.SetActive(false);
        }

        foreach(var button in keys)
        {
            button.GetComponentInChildren<Text>().text = button.name;
        }
    }

    void LateUpdate()
    {
        // If the keyboard is active, the log screen should sit above the keyboard.
        if (gameObject.activeSelf)
        {
            logScreen.transform.position = keyboardAnchor.transform.position;
            logScreen.transform.rotation = keyboardAnchor.transform.rotation;
            logScreen.transform.localScale = new Vector3(2, 2, 2);
        }

        if (Networking.LocalPlayer == null)
        {
            return;
        }
        
        if (!isStaticScreen)
        {
            var lowestPossibleYPos = Vector3.Lerp(
                    Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftFoot),
                    Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightFoot), 0.5f).y - 1f;
            var newPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position.y - 1.7F;

            if (lowestPossibleYPos > newPos)
            {
                newPos = lowestPossibleYPos;
            }

            var pos = Networking.LocalPlayer.GetPosition();
            pos.y = newPos;
            transform.position = pos;
        }
    }

    ///<Summary>Send a key's character to the keyboard. Deactivate shift if it's on.</Summary>
    public void SendKey()
    {
        if ((shift || caps) && field.text.Length > 0)
        {
            input.text += ToUpperCase(field.text.ToCharArray()[0]);
        } else
        {
            input.text += field.text;
        }

        field.text = "";

        if (shift)
        {
            ShiftOff();
        }
    }

    private string ToUpperCase(char input)
    {
        string output = "";

        switch (input)
        {
            case '1':
                output = "!";
                break;
            case '2':
                output = "\"";
                break;
            case '3':
                output = "GBP";
                break;
            case '4':
                output = "$";
                break;
            case '5':
                output = "%";
                break;
            case '6':
                output = "^";
                break;
            case '7':
                output = "&";
                break;
            case '8':
                output = "*";
                break;
            case '9':
                output = "(";
                break;
            case '0':
                output = ")";
                break;
            case '[':
                output = "{";
                break;
            case ']':
                output = "}";
                break;
            case '-':
                output = "_";
                break;
            case '=':
                output = "+";
                break;
            case ';':
                output = ":";
                break;
            case '\'':
                output = "@";
                break;
            case '#':
                output = "~";
                break;
            case '`':
                output = "¬";
                break;
            case '\\':
                output = "|";
                break;
            case ',':
                output = "<";
                break;
            case '.':
                output = ">";
                break;
            case '/':
                output = "?";
                break;
            default:
                output = input.ToString().ToUpper();
                break;
        }

        return output;
    }

    public void CapsHit()
    {
        if (caps)
        {
            CapsOff();
        } else
        {
            CapsOn();
        }
    }

    ///<Summary>Set caps to ON. Shift the keys to upper case if shift isn't on.</Summary>
    private void CapsOn()
    {
        caps = true;
        SetKeysUpper();
        capsButtonImage.color = Color.blue;
    }

    ///<Summary>Set caps to off. Shift the keys to lower case if shift isn't on.</Summary>
    private void CapsOff()
    {
        caps = false;
        if (!shift)
        {
            SetKeysLower();
        }
        capsButtonImage.color = Color.white;
    }

    public void ShiftHit()
    {
        if (shift)
        {
            ShiftOff();
        }
        else
        {
            ShiftOn();
        }
    }

    ///<Summary>Set shift to ON. Shift the keys to upper case if caps isn't on.</Summary>
    public void ShiftOn()
    {
        shift = true;
        SetKeysUpper();
        shiftButtonImage.color = Color.blue;
    }

    ///<Summary>Set shift to off. Shift the keys to lower case if caps isn't on.</Summary>
    public void ShiftOff()
    {
        shift = false;
        if (!caps)
        {
            SetKeysLower();
        }
        shiftButtonImage.color = Color.white;
    }

    ///<Summary>Delete the last character in the input field.</Summary>
    public void BackspaceHit()
    {
        if (input.text.Length > 0)
        {
            input.text = input.text.Remove(input.text.Length - 1);
        }
    }

    ///<Summary>Toggle the keyboard on or off.</Summary>
    public void Toggle()
    {
        if (input.isFocused)
        {
            return;
        }

        if (Networking.LocalPlayer != null && !Networking.LocalPlayer.IsUserInVR())
        {
            logScreen.Toggle();
        }

        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            if (Networking.LocalPlayer != null)
            {
                transform.rotation = Networking.LocalPlayer.GetRotation();
            }

            logScreen.Anchor();
            return;
        }

        logScreen.Unanchor();
    }

    private void SetKeysUpper()
    {
        foreach(var button in keys)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text.text == "")
            {
                Debug.LogError($"KeyboardManager: Text named {button.name} has no value associated with it.");
            }

            var character = text.text.ToCharArray()[0];
            switch(character)
            {
                case '1':
                    text.text = "!";
                    break;
                case '2':
                    text.text = "\"";
                    break;
                case '3':
                    text.text = "GBP";
                    break;
                case '4':
                    text.text = "$";
                    break;
                case '5':
                    text.text = "%";
                    break;
                case '6':
                    text.text = "^";
                    break;
                case '7':
                    text.text = "&";
                    break;
                case '8':
                    text.text = "*";
                    break;
                case '9':
                    text.text = "(";
                    break;
                case '0':
                    text.text = ")";
                    break;
                case '[':
                    text.text = "{";
                    break;
                case ']':
                    text.text = "}";
                    break;
                case '-':
                    text.text = "_";
                    break;
                case '=':
                    text.text = "+";
                    break;
                case ';':
                    text.text = ":";
                    break;
                case '\'':
                    text.text = "@";
                    break;
                case '#':
                    text.text = "~";
                    break;
                case '`':
                    text.text = "¬";
                    break;
                case '\\':
                    text.text = "|";
                    break;
                case ',':
                    text.text = "<";
                    break;
                case '.':
                    text.text = ">";
                    break;
                case '/':
                    text.text = "?";
                    break;
                default:
                    text.text = text.text.ToUpper();
                    break;
            }
        }
    }

    private void SetKeysLower()
    {
        foreach (var button in keys)
        {
            var text = button.GetComponentInChildren<Text>();
            if (text.text == "")
            {
                Debug.LogError($"KeyboardManager: Text named {button.name} has no value associated with it.");
            }

            var character = text.text.ToCharArray()[0];
            switch (character)
            {
                case '!':
                    text.text = "1";
                    break;
                case '"':
                    text.text = "2";
                    break;
                case '£':
                    text.text = "3";
                    break;
                case '$':
                    text.text = "4";
                    break;
                case '%':
                    text.text = "5";
                    break;
                case '^':
                    text.text = "6";
                    break;
                case '&':
                    text.text = "7";
                    break;
                case '*':
                    text.text = "8";
                    break;
                case '(':
                    text.text = "9";
                    break;
                case ')':
                    text.text = "0";
                    break;
                case '_':
                    text.text = "-";
                    break;
                case '+':
                    text.text = "=";
                    break;
                case '¬':
                    text.text = "`";
                    break;
                case '|':
                    text.text = "\\";
                    break;
                case '<':
                    text.text = ",";
                    break;
                case '>':
                    text.text = ".";
                    break;
                case '?':
                    text.text = "/";
                    break;
                case '{':
                    text.text = "/";
                    break;
                case '}':
                    text.text = "]";
                    break;
                case ':':
                    text.text = ";";
                    break;
                case '@':
                    text.text = "'";
                    break;
                case '~':
                    text.text = "#";
                    break;
                default:
                    if (text.text == "GBP")
                    {
                        text.text = "3";
                    }
                    else
                    {
                        text.text = text.text.ToLower();
                    }
                    
                    break;
            }
        }
    }

    public void ImmobilizeHit()
    {
        if (immobilized)
        {
            UnImmobilize();
            return;
        }

        Immobilize();
    }

    public void Immobilize()
    {
        if (Networking.LocalPlayer != null)
        {
            Networking.LocalPlayer.Immobilize(true);
        }

        immobilizedButtonImage.color = Color.blue;
        immobilized = true;
    }

    public void UnImmobilize()
    {
        if (Networking.LocalPlayer != null)
        {
            Networking.LocalPlayer.Immobilize(false);
        }

        immobilizedButtonImage.color = Color.white;
        immobilized = false;
    }
}
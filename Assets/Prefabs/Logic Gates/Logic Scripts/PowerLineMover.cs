using UdonSharp;
using UnityEngine;

public class PowerLineMover : UdonSharpBehaviour
{
    public LineRenderer powerLine;
    public GameObject gateObjects;
    public GameObject on;
    public float timeDelayToActivate = 0.2f;
    InputNOT inputNot;
    InputsOR inputOr;
    float countDownTimer;
    bool startedTimer = false;
    string inputType = "";
    bool usingInputA = false;

    private void Start()
    {
        // TODO find all the inputs
        // manually adding them
        // will likely need to update the list if I add more in the world dynamically.
        // if I can somehow use the start() for the InputLines to add them, that should work better.
        // I think if I had an array for all the powerlines and all the inputs I should be able
        // to make them update dynamically.
        //inputs[0] = GameObject.Find("Input Line");
        countDownTimer = timeDelayToActivate;
    }

    private void Update()
    {
        // set powerline positions according to the parent and mover(self) positions
        // could do a check to see if it not connected to move these, might save a frame... or not
        powerLine.SetPosition(0, gateObjects.transform.position - transform.parent.GetComponent<Transform>().position);
        powerLine.SetPosition(1, transform.position - transform.parent.GetComponent<Transform>().position);
        // input delay
        if (startedTimer)
        {
            countDownTimer -= Time.deltaTime;
            if (countDownTimer <= 0)
            {
                countDownTimer = timeDelayToActivate;
                startedTimer = false;
                SendSignalUpdate();
            }
        }
    }

    // If player moves the powerline, disconnect
    public override void OnPickup()
    {
        startedTimer = false;
        countDownTimer = timeDelayToActivate;
        switch (inputType)
        {
            case "Input Line NOT":
                if (inputNot)
                {
                    inputNot.SetInputSignal(false);
                    inputNot.SetInUse(false);
                    inputNot.ForceUpdateGate();
                    inputNot = null;
                }
                break;
            case "Input Lines OR":
                if (inputOr)
                {
                    if (usingInputA)
                    {
                        inputOr.inputA = false;
                        inputOr.aInUse = false;
                    }
                    else
                    {
                        inputOr.inputB = false;
                        inputOr.bInUse = false;
                    }
                    inputOr.ForceUpdateGate();
                    inputOr = null;
                }
                break;
            default:
                break;
        }
    }

    public override void OnDrop()
    {
        ConnectToGate();
    }
    
    public void ConnectToGate()
    {
        GameObject[] inputs = GetComponentInParent<Transform>().parent.GetComponentInParent<GateSpawner>().inputs;
        // try to connect to an imput.
        for (int i = 0; i < inputs.Length; i++)
        {
            // check what the object is using something specific to each gate before assigning it.            
            inputType = inputs[i].name;
            switch (inputType)
            {
                case "Input Line NOT":
                    InputNOT inputLineNOT = inputs[i].GetComponent<InputNOT>();
                    Debug.Log(inputLineNOT.ToString());
                    // if input is not in use and less than 0.2 units away connect
                    if (!inputLineNOT.GetInUse() && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
                    {
                        inputNot = inputLineNOT;
                        inputNot.SetInUse(true);
                        transform.position = inputs[i].transform.position;
                        transform.rotation = inputs[i].transform.rotation;
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines OR":
                    InputsOR inputLine = inputs[i].GetComponent<InputsOR>();
                    Debug.Log(inputLine.ToString());
                    // if input is not in use and less than 0.2 units away connect
                    if (!inputLine.aInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position) < 0.2f)
                    {
                        usingInputA = true;
                        inputOr = inputLine;
                        inputOr.aInUse = true;
                        transform.position = inputs[i].transform.GetChild(0).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                        startedTimer = true;
                        return;
                    }
                    if (!inputLine.bInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position) < 0.2f)
                    {
                        usingInputA = false;
                        inputOr = inputLine;
                        inputOr.bInUse = true;
                        transform.position = inputs[i].transform.GetChild(1).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                        startedTimer = true;
                        return;
                    }
                    break;
                default:
                    break;
            }
        }
    }


    public void SendSignalUpdate()
    {
        switch (inputType)
        {
            case "Input Line NOT":
                if (inputNot)
                {// null check should be needed if input line was removed(pickedUp) before timer ended. and maybe for late jioners
                    inputNot.SetInputSignal(on.activeSelf);
                    inputNot.UpdateGate();
                }
                break;
            case "Input Lines OR":
                if (inputOr)
                {
                    if (usingInputA)
                    {
                        inputOr.inputA = on.activeSelf;
                    }
                    else
                    {
                        inputOr.inputB = on.activeSelf;
                    }
                    inputOr.UpdateGate();
                }
                break;
            default:
                break;
        }
    }

    public InputNOT GetConnectedNOTInput()
    {
        return inputNot;
    }
    public InputsOR GetConnectedORInput()
    {
        return inputOr;
    }
}
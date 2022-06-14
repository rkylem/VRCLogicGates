using UdonSharp;
using UnityEngine;

public class PowerLineMover : UdonSharpBehaviour
{
    public GameObject on;
    public LineRenderer powerLine;
    public GameObject gateObjects;
    public float timeDelayToActivate = 0.2f;
    bool startedTimer = false;
    float countDownTimer;

    InputNOT inputNot;
    InputsOR inputOr;
    InputLineSplitter inputSplitter;
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
                // this might cause problems... but it should settle to the right place.
                SendSignalUpdate(on.activeSelf);
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
            case "Input Line Splitter":
                if (inputSplitter)
                {
                    inputSplitter.input = false;
                    inputSplitter.inUse = false;
                    inputSplitter.ForceUpdateGate();
                    inputSplitter = null;
                }
                break;
            default:
                break;
        }
    }
    public void pick()
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
            case "Input Line Splitter":
                if (inputSplitter)
                {
                    inputSplitter.input = false;
                    inputSplitter.inUse = false;
                    inputSplitter.ForceUpdateGate();
                    inputSplitter = null;
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
                case "Input Line Splitter":
                    InputLineSplitter inputLineSplitter = inputs[i].GetComponent<InputLineSplitter>();
                    // if input is not in use and less than 0.2 units away connect
                    if (!inputLineSplitter.inUse && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
                    {
                        inputSplitter = inputLineSplitter;
                        inputSplitter.inUse = true;
                        transform.position = inputs[i].transform.position;
                        transform.rotation = inputs[i].transform.rotation;
                        startedTimer = true;
                        return;
                    }
                    break;
                default:
                    break;
            }
        }
    }


    public void SendSignalUpdate(bool updateState)
    {
        switch (inputType)
        {
            case "Input Line NOT":
                if (inputNot)
                {// null check should be needed if input line was removed(pickedUp) before timer ended. and maybe for late jioners
                    inputNot.SetInputSignal(updateState);
                    inputNot.UpdateGate();
                }
                break;
            case "Input Lines OR":
                if (inputOr)
                {
                    // right here if the input is the same input as the powerline connected is the problem...
                    // because I'm not using a parameter, I get the value from the gates state. This means when
                    // I disconnect the powerline I'm trying to update the gate with it's own gate state.
                    if (usingInputA)
                    {// Got a cheaky workaround... if the inputs aren't in use then they can't be true
                        if (inputOr.aInUse)
                        {
                            inputOr.inputA = updateState;
                        }
                        else
                        {
                            inputOr.inputA = false;
                        }
                        //inputOr.inputA = on.activeSelf;
                    }
                    else
                    {
                        if (inputOr.bInUse)
                        {
                            inputOr.inputB = updateState;
                        }
                        else
                        {
                            inputOr.inputB = false;
                        }
                        //inputOr.inputB = on.activeSelf;
                    }
                    inputOr.UpdateGate();
                }
                break;
            case "Input Line Splitter":
                if (inputSplitter)
                {
                    inputSplitter.input = updateState;
                    inputSplitter.UpdateGate();
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
    public InputLineSplitter GetConnectedSplitterInput()
    {
        return inputSplitter;
    }
}
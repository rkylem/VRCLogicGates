using UdonSharp;
using UnityEngine;

public class LocalPowerlineMover : UdonSharpBehaviour
{
    public GameObject on;
    public LineRenderer powerLine;
    public GameObject gateObjects;
    public float timeDelayToActivate = 0.2f;
    bool startedTimer = false;
    float countDownTimer;

    LocalInputNot inputNot;
    LocalInputsOR inputOr;
    LocalInputSplitter inputSplitter;
    string inputType = "";
    bool usingInputA = false;
    public bool holding = false;

    public bool pickedUp = false;

    private void Start()
    {
        countDownTimer = timeDelayToActivate;
    }

    private void Update()
    {
        if (holding && pickedUp)
        {
            powerLine.SetPosition(0, gateObjects.transform.position - transform.parent.GetComponent<Transform>().position);
            powerLine.SetPosition(1, transform.position - transform.parent.GetComponent<Transform>().position);
        }
        
        if (startedTimer)
        {
            countDownTimer -= Time.deltaTime;
            if (countDownTimer <= 0)
            {
                startedTimer = false;
                countDownTimer = timeDelayToActivate;
                SendSignalUpdate(on.activeSelf);
            }
        }
    }

    public override void OnPickup()
    {
        if (!pickedUp)
        {
            transform.parent = transform.parent.parent;
            pickedUp = true;
        }
        holding = true;
        startedTimer = false;
        countDownTimer = timeDelayToActivate;
        switch (inputType)
        {
            case "Input Line NOT":
                if (inputNot)
                {
                    inputNot.inputSignal = false;
                    inputNot.inUse = false;
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
        holding = false;
        GameObject[] inputs = GetComponentInParent<Transform>().parent.GetComponentInParent<GateSpawner>().inputs;
        for (int i = 0; i < inputs.Length; i++)
        {         
            inputType = inputs[i].name;
            switch (inputType)
            {
                case "Input Line NOT":
                    LocalInputNot inputLineNOT = inputs[i].GetComponent<LocalInputNot>();
                    if (!inputLineNOT.inUse && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
                    {
                        inputNot = inputLineNOT;
                        inputNot.inUse = true;
                        transform.position = inputs[i].transform.position;
                        transform.rotation = inputs[i].transform.rotation;
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines OR":
                    LocalInputsOR inputLine = inputs[i].GetComponent<LocalInputsOR>();
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
                    LocalInputSplitter inputLineSplitter = inputs[i].GetComponent<LocalInputSplitter>();
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
        powerLine.SetPosition(1, transform.position - transform.parent.GetComponent<Transform>().position);
    }

    public void SendSignalUpdate(bool updateState)
    {
        switch (inputType)
        {
            case "Input Line NOT":
                if (inputNot)
                {
                    if (inputNot.inUse)
                    {
                        inputNot.inputSignal = updateState;
                    }
                    else
                    {
                        inputNot.inputSignal = false;
                    }
                    inputNot.UpdateGate();
                }
                break;
            case "Input Lines OR":
                if (inputOr)
                {
                    if (usingInputA)
                    {
                        if (inputOr.aInUse)
                        {
                            inputOr.inputA = updateState;
                        }
                        else
                        {
                            inputOr.inputA = false;
                        }
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
                    }
                    inputOr.UpdateGate();
                }
                break;
            case "Input Line Splitter":
                if (inputSplitter)
                {
                    if (inputSplitter.inUse)
                    {
                        inputSplitter.input = updateState;
                    }
                    else
                    {
                        inputSplitter.input = false;
                    }
                    inputSplitter.UpdateGate();
                }
                break;
            default:
                break;
        }
    }

    public LocalInputNot GetConnectedNOTInput()
    {
        return inputNot;
    }
    public LocalInputsOR GetConnectedORInput()
    {
        return inputOr;
    }
    public LocalInputSplitter GetConnectedSplitterInput()
    {
        return inputSplitter;
    }
    public void SetSplitterInputNull()
    {
        inputSplitter = null;
    }
}
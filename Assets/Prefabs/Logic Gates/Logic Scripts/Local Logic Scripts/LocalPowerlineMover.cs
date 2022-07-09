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
    LocalInputsAnd inputAnd;
    LocalInputsXor inputXor;
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
            powerLine.SetPosition(0, gateObjects.transform.position - transform.parent.transform.position);
            powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
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
            case "Input Lines AND":
                if (inputAnd)
                {
                    if (usingInputA)
                    {
                        inputAnd.inputA = false;
                        inputAnd.aInUse = false;
                    }
                    else
                    {
                        inputAnd.inputB = false;
                        inputAnd.bInUse = false;
                    }
                    inputAnd.ForceUpdateGate();
                    inputAnd = null;
                }
                break;
            case "Input Lines XOR":
                if (inputXor)
                {
                    if (usingInputA)
                    {
                        inputXor.inputA = false;
                        inputXor.aInUse = false;
                    }
                    else
                    {
                        inputXor.inputB = false;
                        inputXor.bInUse = false;
                    }
                    inputXor.ForceUpdateGate();
                    inputXor = null;
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
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
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
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
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
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
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
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines AND":
                    LocalInputsAnd inputLineAnd = inputs[i].GetComponent<LocalInputsAnd>();
                    if (!inputLineAnd.aInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position) < 0.2f)
                    {
                        usingInputA = true;
                        inputAnd = inputLineAnd;
                        inputAnd.aInUse = true;
                        transform.position = inputs[i].transform.GetChild(0).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    if (!inputLineAnd.bInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position) < 0.2f)
                    {
                        usingInputA = false;
                        inputAnd = inputLineAnd;
                        inputAnd.bInUse = true;
                        transform.position = inputs[i].transform.GetChild(1).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines XOR":
                    LocalInputsXor inputLineXor = inputs[i].GetComponent<LocalInputsXor>();
                    if (!inputLineXor.aInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position) < 0.2f)
                    {
                        usingInputA = true;
                        inputXor = inputLineXor;
                        inputXor.aInUse = true;
                        transform.position = inputs[i].transform.GetChild(0).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    if (!inputLineXor.bInUse && Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position) < 0.2f)
                    {
                        usingInputA = false;
                        inputXor = inputLineXor;
                        inputXor.bInUse = true;
                        transform.position = inputs[i].transform.GetChild(1).transform.position;
                        transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
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
            case "Input Lines AND":
                if (inputAnd)
                {
                    if (usingInputA)
                    {
                        if (inputAnd.aInUse)
                        {
                            inputAnd.inputA = updateState;
                        }
                        else
                        {
                            inputAnd.inputA = false;
                        }
                    }
                    else
                    {
                        if (inputAnd.bInUse)
                        {
                            inputAnd.inputB = updateState;
                        }
                        else
                        {
                            inputAnd.inputB = false;
                        }
                    }
                    inputAnd.UpdateGate();
                }
                break;
            case "Input Lines XOR":
                if (inputXor)
                {
                    if (usingInputA)
                    {
                        if (inputXor.aInUse)
                        {
                            inputXor.inputA = updateState;
                        }
                        else
                        {
                            inputXor.inputA = false;
                        }
                    }
                    else
                    {
                        if (inputXor.bInUse)
                        {
                            inputXor.inputB = updateState;
                        }
                        else
                        {
                            inputXor.inputB = false;
                        }
                    }
                    inputXor.UpdateGate();
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
    public LocalInputsAnd GetConnectedAndInput()
    {
        return inputAnd;
    }
    public LocalInputsXor GetConnectedXorInput()
    {
        return inputXor;
    }
    public void SetSplitterInputNull()
    {
        inputSplitter = null;
    }
}
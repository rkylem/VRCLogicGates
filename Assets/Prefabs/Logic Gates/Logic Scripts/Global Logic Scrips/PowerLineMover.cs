using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class PowerLineMover : UdonSharpBehaviour
{
    public GameObject on;
    public LineRenderer powerLine;
    public GameObject gateObjects;
    public float timeDelayToActivate = 0.2f;
    bool startedTimer = false;
    float countDownTimer;

    bool startedTimerForJoined = false;
    float joinedPlayerTimer = 2;

    InputNOT inputNot;
    InputsOR inputOr;
    InputSplitter inputSplitter;
    InputsAnd inputAnd;
    string inputType = "";
    bool usingInputA = false;

    //public bool holding = false;

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
        //new thought to fix the lines not reconnecting right... it seems like they are not connecting correctly except the not gate for some reason...
        // maybe if I used a timmer and reconnected them after some time... maybe then?
        if (startedTimerForJoined)
        {
            joinedPlayerTimer -= Time.deltaTime;
            if (joinedPlayerTimer <= 0)
            {
                startedTimerForJoined = false;
                joinedPlayerTimer = 2;
                FixJoinPlayer();
            }
        }
        // set powerline positions according to the parent and mover(self) positions
        // could do a check to see if it not connected to move these, might save a frame... or not
        //if (holding) // not worth the networking calls, prob laggier with them
        //{
        powerLine.SetPosition(0, gateObjects.transform.position - transform.parent.transform.position);
        powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
        //}
        // input delay
        if (startedTimer)
        {
            countDownTimer -= Time.deltaTime;
            if (countDownTimer <= 0)
            {
                startedTimer = false;
                countDownTimer = timeDelayToActivate;
                // this might cause problems... but it should settle to the right place.
                SendSignalUpdate(on.activeSelf);
            }
        }
    }

    // If player moves the powerline, disconnect
    public override void OnPickup()
    {// might need to network this function
        //pick();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "pick");
    }
    public void pick()
    {
        startedTimer = false;
        //holding = true;
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
                    default:
                break;
        }
    }

    public override void OnDrop()
    {// might need to network this function
        //ConnectToGate();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ConnectToGate");
    }
    // when they join everyone needs to be on the same page so the lines should be connected
    public override void OnPlayerJoined(VRCPlayerApi player)
    {// might need to network this.
        //reset everything
        //if (Networking.LocalPlayer == player)
        //{
        //    Debug.Log("Networking.LocalPlayer FixJoinPlayer");
        //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "FixJoinPlayer");
        //}

        //if (Networking.IsMaster)
        //{//master has differnet results than joined palyer how can I sync.
        //    Debug.Log("Networking.IsMaster FixJoinPlayer");
        //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "FixJoinPlayer");
        //}
        //FixJoinPlayer();
        startedTimerForJoined = true;
    }
    public void FixJoinPlayer()
    {// reset everything
        startedTimer = false;
        countDownTimer = timeDelayToActivate;
        if (inputNot)
        {//maybe just call pick in this class
            inputNot.ResetInput();
        }
        if (inputOr)
        {
            usingInputA = false;
            inputOr.ResetInputs();
        }
        if (inputSplitter)
        {
            inputSplitter.ResetInput();
        }
        if (inputAnd)
        {
            inputAnd.ResetInputs();
        }
        pick();
        // and reconnect back
        ConnectToGate();
    }
    //public void ConnectAllGates()
    //{
    //    inputNot = null;
    //    inputOr = null;
    //    inputSplitter = null;
    //    GameObject[] inputs = GetComponentInParent<Transform>().parent.GetComponentInParent<GateSpawner>().inputs;
    //    for (int i = 0; i < inputs.Length; i++)
    //    {            
    //        inputType = inputs[i].name;
    //        switch (inputType)
    //        {
    //            case "Input Line NOT":
    //                InputNOT inputLineNOT = inputs[i].GetComponent<InputNOT>();                    
    //                // if input is not in use and less than 0.2 units away connect
    //                if (!inputLineNOT.GetInUse() && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
    //                {
    //                    Debug.Log("connected to not " + i);
    //                    inputNot = inputLineNOT;
    //                    inputNot.notGate.connectedPowerLineScript = this;
    //                    inputNot.SetInUse(true);
    //                    transform.position = inputs[i].transform.position;
    //                    transform.rotation = inputs[i].transform.rotation;
    //                    //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                    startedTimer = true;
    //                    return;
    //                }
    //                break;
    //            case "Input Lines OR":
    //                InputsOR inputLine = inputs[i].GetComponent<InputsOR>();
    //                // calculate the distances first and find the one the mover is closer to and try to connect to that one first
    //                float distanceToInputA = Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position);
    //                float distanceToInputB = Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position);
    //                if (distanceToInputA < distanceToInputB)
    //                {// if input is not in use and less than 0.2 units away connect
    //                    if (!inputLine.aInUse && distanceToInputA < 0.2f)
    //                    {
    //                        Debug.Log("connected to or a " + i);
    //                        usingInputA = true;
    //                        inputOr = inputLine;
    //                        inputOr.orGate.connectedPowerLineScriptA = this;
    //                        inputOr.aInUse = true;
    //                        transform.position = inputs[i].transform.GetChild(0).transform.position;
    //                        transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
    //                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                        startedTimer = true;
    //                        // break is needed to only connect to 1 input as it still tries both even after it connected
    //                        /*break;*/
    //                        return;
    //                    }
    //                    if (!inputLine.bInUse && distanceToInputB < 0.2f)
    //                    {
    //                        Debug.Log("connected to or b " + i);
    //                        usingInputA = false;
    //                        inputOr = inputLine;
    //                        inputOr.orGate.connectedPowerLineScriptB = this;
    //                        inputOr.bInUse = true;
    //                        transform.position = inputs[i].transform.GetChild(1).transform.position;
    //                        transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
    //                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                        startedTimer = true;
    //                        return;
    //                    }
    //                }
    //                else
    //                {// if input is not in use and less than 0.2 units away connect
    //                    if (!inputLine.bInUse && distanceToInputB < 0.2f)
    //                    {
    //                        Debug.Log("connected to or b " + i);
    //                        usingInputA = false;
    //                        inputOr = inputLine;
    //                        inputOr.orGate.connectedPowerLineScriptB = this;
    //                        inputOr.bInUse = true;
    //                        transform.position = inputs[i].transform.GetChild(1).transform.position;
    //                        transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
    //                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                        startedTimer = true;
    //                        // break is needed to only connect to 1 input as it still tries both even after it connected
    //                        /*break;*/
    //                        return;
    //                    }
    //                    if (!inputLine.aInUse && distanceToInputA < 0.2f)
    //                    {
    //                        Debug.Log("connected to or a " + i);
    //                        usingInputA = true;
    //                        inputOr = inputLine;
    //                        inputOr.orGate.connectedPowerLineScriptA = this;
    //                        inputOr.aInUse = true;
    //                        transform.position = inputs[i].transform.GetChild(0).transform.position;
    //                        transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
    //                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                        startedTimer = true;
    //                        return;
    //                    }
    //                }
    //                break;
    //            case "Input Line Splitter":
    //                InputSplitter inputLineSplitter = inputs[i].GetComponent<InputSplitter>();
    //                // if input is not in use and less than 0.2 units away connect
    //                if (!inputLineSplitter.inUse && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
    //                {
    //                    Debug.Log("connected to spliter " + i);
    //                    inputSplitter = inputLineSplitter;
    //                    inputSplitter.lineSplitter.connectedPowerLineScript = this;
    //                    inputSplitter.inUse = true;
    //                    transform.position = inputs[i].transform.position;
    //                    transform.rotation = inputs[i].transform.rotation;
    //                    //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
    //                    startedTimer = true;
    //                    return;
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
    public void ConnectToGate()
    {
        inputNot = null;
        inputOr = null;
        inputSplitter = null;
        //holding = false;
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
                    //Debug.Log("Input line not in use: " + inputLineNOT.inUse);
                    //Debug.Log("Input line not distance: " + Vector3.Distance(transform.position, inputs[i].transform.position));
                    // if input is not in use and less than 0.2 units away connect
                    if (!inputLineNOT.GetInUse() && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
                    {
                        inputNot = inputLineNOT;
                        inputNot.notGate.connectedPowerLineScript = this;
                        inputNot.SetInUse(true);
                        transform.position = inputs[i].transform.position;
                        transform.rotation = inputs[i].transform.rotation;
                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines OR":
                    InputsOR inputLine = inputs[i].GetComponent<InputsOR>();
                    //Debug.Log("Input lines or a in use: " + inputLine.aInUse);
                    //Debug.Log("Input lines or b in use: " + inputLine.bInUse);
                    // calculate the distances first and find the one the mover is closer to and try to connect to that one first
                    float distanceToOrA = Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position);
                    float distanceToOrB = Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position);
                    //Debug.Log("Input line or a distance: " + distanceToInputA);
                    //Debug.Log("Input line or b distance: " + distanceToInputB);
                    // if I moved things around and put if(distanceToInputB > distanceToInputA) it would connect to A if in middle
                    if (distanceToOrA < distanceToOrB)
                    {// if input is not in use and less than 0.2 units away connect
                        if (!inputLine.aInUse && distanceToOrA < 0.2f)
                        {
                            usingInputA = true;
                            inputOr = inputLine;
                            inputOr.orGate.connectedPowerLineScriptA = this;
                            inputOr.aInUse = true;
                            transform.position = inputs[i].transform.GetChild(0).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                        if (!inputLine.bInUse && distanceToOrB < 0.2f)
                        {
                            usingInputA = false;
                            inputOr = inputLine;
                            inputOr.orGate.connectedPowerLineScriptB = this;
                            inputOr.bInUse = true;
                            transform.position = inputs[i].transform.GetChild(1).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                    }
                    else
                    {
                        if (!inputLine.bInUse && distanceToOrB < 0.2f)
                        {
                            usingInputA = false;
                            inputOr = inputLine;
                            inputOr.orGate.connectedPowerLineScriptB = this;
                            inputOr.bInUse = true;
                            transform.position = inputs[i].transform.GetChild(1).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                        if (!inputLine.aInUse && distanceToOrA < 0.2f)
                        {
                            usingInputA = true;
                            inputOr = inputLine;
                            inputOr.orGate.connectedPowerLineScriptA = this;
                            inputOr.aInUse = true;
                            transform.position = inputs[i].transform.GetChild(0).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                    }
                    break;
                case "Input Line Splitter":
                    InputSplitter inputLineSplitter = inputs[i].GetComponent<InputSplitter>();
                    //Debug.Log("Input line splitter in use: " + inputLineSplitter.inUse);
                    //Debug.Log("Input line not distance: " + Vector3.Distance(transform.position, inputs[i].transform.position));
                    // if input is not in use and less than 0.2 units away connect
                    if (!inputLineSplitter.inUse && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
                    {
                        inputSplitter = inputLineSplitter;
                        inputSplitter.lineSplitter.connectedPowerLineScript = this;
                        inputSplitter.inUse = true;
                        transform.position = inputs[i].transform.position;
                        transform.rotation = inputs[i].transform.rotation;
                        //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                        startedTimer = true;
                        return;
                    }
                    break;
                case "Input Lines AND":
                    InputsAnd inputLineAnd = inputs[i].GetComponent<InputsAnd>();
                    float distanceToAndA = Vector3.Distance(transform.position, inputs[i].transform.GetChild(0).transform.position);
                    float distanceToAndB = Vector3.Distance(transform.position, inputs[i].transform.GetChild(1).transform.position);
                    if (distanceToAndA < distanceToAndB)
                    {// if input is not in use and less than 0.2 units away connect
                        if (!inputLineAnd.aInUse && distanceToAndA < 0.2f)
                        {
                            usingInputA = true;
                            inputAnd = inputLineAnd;
                            inputAnd.andGate.connectedPowerLineScriptA = this;
                            inputAnd.aInUse = true;
                            transform.position = inputs[i].transform.GetChild(0).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                        if (!inputLineAnd.bInUse && distanceToAndB < 0.2f)
                        {
                            usingInputA = false;
                            inputAnd = inputLineAnd;
                            inputAnd.andGate.connectedPowerLineScriptB = this;
                            inputAnd.bInUse = true;
                            transform.position = inputs[i].transform.GetChild(1).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                    }
                    else
                    {
                        if (!inputLineAnd.bInUse && distanceToAndB < 0.2f)
                        {
                            usingInputA = false;
                            inputAnd = inputLineAnd;
                            inputAnd.andGate.connectedPowerLineScriptB = this;
                            inputAnd.bInUse = true;
                            transform.position = inputs[i].transform.GetChild(1).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(1).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
                        if (!inputLineAnd.aInUse && distanceToAndA < 0.2f)
                        {
                            usingInputA = true;
                            inputAnd = inputLineAnd;
                            inputAnd.andGate.connectedPowerLineScriptA = this;
                            inputAnd.aInUse = true;
                            transform.position = inputs[i].transform.GetChild(0).transform.position;
                            transform.rotation = inputs[i].transform.GetChild(0).transform.rotation;
                            //powerLine.SetPosition(1, transform.position - transform.parent.transform.position);
                            startedTimer = true;
                            return;
                        }
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
                    if (inputNot.inUse)
                    {
                        inputNot.SetInputSignal(updateState);
                    }
                    else
                    {
                        inputNot.SetInputSignal(false);
                    }
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
                    {// Got a cheaky workaround... if the inputs aren't in use then they can't be true
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
    public InputSplitter GetConnectedSplitterInput()
    {
        return inputSplitter;
    }

    public void SetNotNull()
    {
        inputNot = null;
    }
    public void SetOrNull()
    {
        inputOr = null;
    }
    public void SetSplitterNull()
    {
        inputSplitter = null;
    }
    public void SetInputsNull()
    {
        inputNot = null;
        inputOr = null;
        inputSplitter = null;
    }
}
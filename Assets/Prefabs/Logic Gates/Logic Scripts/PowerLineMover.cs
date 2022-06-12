using UdonSharp;
using UnityEngine;

public class PowerLineMover : UdonSharpBehaviour
{
    public LineRenderer powerLine;
    public GameObject notGateObjects;
    public GameObject on;
    public float timeDelayToActivate = 0.3f;
    InputLineNot connectedInput;
    float countDownTimer;
    bool startedTimer = false;

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
        powerLine.SetPosition(0, notGateObjects.transform.position - transform.parent.GetComponent<Transform>().position);
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
        //startedTimer = false;
        //countDownTimer = timeDelayToActivate;
        //if (connectedInput)
        //{
        //    connectedInput.SetInputSignal(false);
        //    connectedInput.SetInUse(false);
        //    connectedInput.ForceUpdateGate();
        //    connectedInput = null;
        //}
        pick();
    }
    public void pick()
    {
        startedTimer = false;
        countDownTimer = timeDelayToActivate;
        if (connectedInput)
        {
            connectedInput.SetInputSignal(false);
            connectedInput.SetInUse(false);
            connectedInput.ForceUpdateGate();
            connectedInput = null;
        }
    }

    public override void OnDrop()
    {
        //GameObject[] inputs = GetComponentInParent<Transform>().parent.GetComponentInParent<SpawnNotGate>().inputs;
        //// try to connect to an imput.
        //for (int i = 0; i < inputs.Length; i++)
        //{
        //    // if input is not in use and less than 0.2 units away connect
        //    if (!inputs[i].GetComponent<InputLineNot>().GetInUse() &&
        //        Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
        //    {
        //        startedTimer = true;
        //        connectedInput = inputs[i].GetComponent<InputLineNot>();
        //        connectedInput.SetInUse(true);
        //        transform.position = inputs[i].transform.position;
        //        transform.rotation = inputs[i].transform.rotation;
        //        break;
        //    }
        //}
        connect();
    }
    public void connect()
    {
        GameObject[] inputs = GetComponentInParent<Transform>().parent.GetComponentInParent<SpawnNotGate>().inputs;
        // try to connect to an imput.
        for (int i = 0; i < inputs.Length; i++)
        {
            InputLineNot inputLine = inputs[i].GetComponent<InputLineNot>();
            Debug.Log(inputLine.ToString());
            // if input is not in use and less than 0.2 units away connect
            if (!inputLine.GetInUse() && Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
            {
                connectedInput = inputLine;
                connectedInput.SetInUse(true);
                transform.position = inputs[i].transform.position;
                transform.rotation = inputs[i].transform.rotation;
                startedTimer = true;
                break;
            }
        }
    }

    public void SendSignalUpdate()
    {
        // null check should be needed if input line was removed(pickedUp) before timer ended. and maybe for late jioners
        if (connectedInput)
        {
            connectedInput.SetInputSignal(on.activeSelf);
            connectedInput.UpdateGate();
        }
    }

    public InputLineNot GetConnectedInput()
    {
        return connectedInput;
    }
}    

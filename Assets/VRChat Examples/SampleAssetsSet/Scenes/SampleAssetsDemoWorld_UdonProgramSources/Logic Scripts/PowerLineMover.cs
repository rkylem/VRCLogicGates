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

    public GameObject[] inputs;
    //public GameObject[] inputs = new GameObject[1];
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
    //public void TempPickUp()
    //{
    //    countDownTimer = timeDelayToActivate;
    //    startedTimer = false;
    //    if (connectedInput)
    //    {
    //        connectedInput.SetInUse(false);
    //        connectedInput.SetInputSignal(false);
    //        SendSignalUpdate();
    //        connectedInput = null;
    //    }
    //}
    public override void OnDrop()
    {
        // try to connect to an imput.
        for (int i = 0; i < inputs.Length; i++)
        {
            // if input is not in use and less than 0.2 units away connect
            if (!inputs[i].GetComponent<InputLineNot>().GetInUse() &&
                Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
            {
                startedTimer = true;
                connectedInput = inputs[i].GetComponent<InputLineNot>();
                connectedInput.SetInUse(true);
                transform.position = inputs[i].transform.position;
                transform.rotation = inputs[i].transform.rotation;
                break;
            }
        }
    }

    //public void ConnectToInput()
    //{
    //    for (int i = 0; i < inputs.Length; i++)
    //    {
    //        // if input is not in use and less than 0.2 units away connect
    //        if (!inputs[i].GetComponent<InputLineNot>().GetInUse() &&
    //            Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
    //        {
    //            startedTimer = true;
    //            connectedInput = inputs[i].GetComponent<InputLineNot>();
    //            connectedInput.SetInUse(true);
    //            transform.position = inputs[i].transform.position;
    //            transform.rotation = inputs[i].transform.rotation;
    //            break;
    //        }
    //    }
    //}

    public void SendSignalUpdate()
    {
        // null check should be needed if input line was removed(pickedUp) before timer ended. and maybe for late jioners
        if (connectedInput)
        {
            connectedInput.SetInputSignal(on.activeSelf);
            connectedInput.UpdateGate();
        }
    }

    //GameObject[] FindGameObjectsWithName(string name)
    //{
    //    int a = GameObject.FindObjectsOfType<GameObject>().Length;
    //    GameObject[] arr = new GameObject[a];
    //    int FluentNumber = 0;
    //    for (int i = 0; i < a; i++)
    //    {
    //        if (GameObject.FindObjectsOfType<GameObject>()[i].name == name)
    //        {
    //            arr[FluentNumber] = GameObject.FindObjectsOfType<GameObject>()[i];
    //            FluentNumber++;
    //        }
    //    }
    //    System.Array.Resize(ref arr, FluentNumber);
    //    return arr;
    //}
}    

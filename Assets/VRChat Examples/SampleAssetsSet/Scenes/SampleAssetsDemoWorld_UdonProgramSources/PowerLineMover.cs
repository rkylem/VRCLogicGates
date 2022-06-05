using UdonSharp;
using UnityEngine;

public class PowerLineMover : UdonSharpBehaviour
{
    public LineRenderer powerLine;
    public GameObject notGateObjects;
    public float timeDelayToActivate = 0.1f;
    InputLineNot connectedInput;
    float countDownTimer;
    bool startedTimer = false;

    //GameObject[] inputs;
    GameObject[] inputs;
    private void Start()
    {
        //GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        //inputs = new GameObject[allObjects.Length];
        //int inputCount = 0;
        //for (int i = 0; i < allObjects.Length; ++i)
        //{
        //    if (allObjects[i].name == "Input Line")
        //    {
        //        inputs[i] = allObjects[i];
        //        inputCount++;
        //    }
        //}
        //System.Array.Resize(ref inputs, inputCount);
        //inputs = GameObject.FindGameObjectsWithTag("Input Line");

        inputs[0] = GameObject.Find("Input Line");
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
        countDownTimer = timeDelayToActivate;
        startedTimer = false;
        if (connectedInput)
        {
            connectedInput.SetInUse(false);
            connectedInput.SetInputSignal(false);
            connectedInput = null;
        }
    }
    public override void OnDrop()
    {
        ConnectToInput();
        // can't seem to use threads
        //StartCoroutine("ConnectToInput");
    }

    public void ConnectToInput()
    {
        startedTimer = true;
        for (int i = 0; i < inputs.Length; i++)
        {
            // if input is not in use and less than 0.2 units away connect
            if (!inputs[i].GetComponent<InputLineNot>().GetInUse() &&
                Vector3.Distance(transform.position, inputs[i].transform.position) < 0.2f)
            {
                connectedInput = inputs[i].GetComponent<InputLineNot>();
                connectedInput.SetInUse(true);
                transform.position = inputs[i].transform.position;
                transform.rotation = inputs[i].transform.rotation;
                break;
            }
        }
    }

    public void SendSignalUpdate()
    {
        connectedInput.UpdateGate();
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

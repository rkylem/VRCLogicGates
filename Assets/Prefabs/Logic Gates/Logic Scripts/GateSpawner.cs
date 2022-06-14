using UdonSharp;
using UnityEngine;

public class GateSpawner : UdonSharpBehaviour
{
    public GameObject inputSwitch;
    public GameObject notGate;
    public GameObject orGate;
    public GameObject inputSplitter;

    public GameObject[] inputs;
    void Start()
    {
        
    }

    public override void Interact()
    {
        // maybe a switch statement here for the different gates
        // there should only be or, not, and, xor, and switch
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CreateSwitch");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CreateNOTGate");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CreateORGate");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CreateInputSplitter");
        // not sure I can or should be calling networked events inside other networked events...
    }

    public void CreateSwitch()
    {
        GameObject newSwitch = VRCInstantiate(inputSwitch);
        newSwitch.transform.parent = transform;
        newSwitch.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y);
    }

    public void CreateNOTGate()
    {
        GameObject newNotGate = VRCInstantiate(notGate);
        newNotGate.transform.parent = transform;
        newNotGate.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];

        //System.Array.Copy(inputs, tempArray, inputs.Length + 1);

        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        // we want the input line object specifically.
        tempArray[inputs.Length] = newNotGate.transform.GetChild(1).GetChild(2).gameObject;

        inputs = tempArray;
    }

    public void CreateORGate()
    {
        GameObject newOrGate = VRCInstantiate(orGate);
        newOrGate.transform.parent = transform;
        newOrGate.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];
        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        tempArray[inputs.Length] = newOrGate.transform.GetChild(1).GetChild(2).gameObject;

        inputs = tempArray;
    }

    public void CreateInputSplitter()
    {
        GameObject newInputSplitter = VRCInstantiate(inputSplitter);
        newInputSplitter.transform.parent = transform;
        newInputSplitter.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];
        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        tempArray[inputs.Length] = newInputSplitter.transform.GetChild(3).GetChild(2).gameObject;

        inputs = tempArray;
    }
}
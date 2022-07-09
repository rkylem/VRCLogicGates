using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class GateSpawner : UdonSharpBehaviour
{
    public GameObject inputSwitch;
    public GameObject notGate;
    public GameObject orGate;
    public GameObject inputSplitter;
    public GameObject andGate;
    public GameObject xorGate;

    public GameObject[] inputs;

    //public override void OnPlayerJoined(VRCPlayerApi player)
    //{
    //    if (Networking.IsMaster)
    //    {
    //        for (int i = 0; i < inputs.Length; i++)
    //        {
    //            Debug.Log(inputs[i].name);
    //        }
    //        CreateSwitch();
    //    }
    //}
    //public void CreateNetworkedGate(string functionName)
    //{
    //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, functionName);
    //}

    public void CreateGate(string functionName)
    {
        switch (functionName)
        {
            case "CreateSwitch":
                CreateSwitch();
                break;
            case "CreateNOTGate":
                CreateNOTGate();
                break;
            case "CreateORGate":
                CreateORGate();
                break;
            case "CreateInputSplitter":
                CreateInputSplitter();
                break;
            case "CreateANDGate":
                CreateANDGate();
                break;
            case "CreateXORGate":
                CreateXORGate();
                break;
            default:
                break;
        }
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
        tempArray[inputs.Length] = newNotGate.transform.GetChild(0).GetChild(0).gameObject;

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
        tempArray[inputs.Length] = newOrGate.transform.GetChild(0).GetChild(0).gameObject;

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
        tempArray[inputs.Length] = newInputSplitter.transform.GetChild(0).GetChild(0).gameObject;

        inputs = tempArray;
    }

    public void CreateANDGate()
    {
        GameObject newAndGate = VRCInstantiate(andGate);
        newAndGate.transform.parent = transform;
        newAndGate.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z + 1);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];
        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        tempArray[inputs.Length] = newAndGate.transform.GetChild(0).GetChild(0).gameObject;

        inputs = tempArray;
    }

    public void CreateXORGate()
    {
        GameObject newXorGate = VRCInstantiate(xorGate);
        newXorGate.transform.parent = transform;
        newXorGate.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];
        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        tempArray[inputs.Length] = newXorGate.transform.GetChild(0).GetChild(0).gameObject;

        inputs = tempArray;
    }
}
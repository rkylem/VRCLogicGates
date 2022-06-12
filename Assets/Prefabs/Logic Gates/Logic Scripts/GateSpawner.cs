using UdonSharp;
using UnityEngine;

public class GateSpawner : UdonSharpBehaviour
{
    public GameObject inputSwitch;
    public GameObject notGate;
    
    public GameObject[] inputs;
    void Start()
    {
        
    }

    public override void Interact()
    {
        // maybe a switch statement here for the different gates
        // there should only be or, not, and, xor, and switch
        CreateSwitch();
        CreateNotGate();
    }

    void CreateSwitch()
    {
        GameObject newSwitch = VRCInstantiate(inputSwitch);
        newSwitch.transform.parent = transform;
        newSwitch.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y);
    }

    void CreateNotGate()
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
}
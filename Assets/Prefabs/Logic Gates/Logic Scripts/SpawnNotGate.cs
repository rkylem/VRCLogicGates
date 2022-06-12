using UdonSharp;
using UnityEngine;

public class SpawnNotGate : UdonSharpBehaviour
{
    public GameObject notGate;
    public GameObject[] inputs;
    void Start()
    {
        
    }

    public override void Interact()
    {
        GameObject newNotGate = VRCInstantiate(notGate);
        newNotGate.transform.parent = transform;
        newNotGate.transform.position = new Vector3(this.transform.position.x + 0.5f, this.transform.position.y + 0.5f);

        GameObject[] tempArray = new GameObject[inputs.Length + 1];

        //System.Array.Copy(inputs, tempArray, inputs.Length + 1);

        for (int i = 0; i < inputs.Length; i++)
        {
            tempArray[i] = inputs[i];
        }
        tempArray[inputs.Length] = newNotGate.transform.GetChild(1).transform.GetChild(2).gameObject;

        inputs = tempArray;
    }
}

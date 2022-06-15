using UdonSharp;

public class CallFunctionOfGateSpawner : UdonSharpBehaviour
{
    public GateSpawner spawner;
    public string functionToCall;

    public override void Interact()
    {
        spawner.CreateNetworkedGate(functionToCall);
    }
}

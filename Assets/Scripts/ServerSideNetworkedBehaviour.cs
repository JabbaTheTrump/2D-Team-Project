using Unity.Netcode;

public class ServerSideNetworkedBehaviour : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsServer;
    }
}

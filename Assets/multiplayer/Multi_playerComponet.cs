using UnityEngine;
using PurrNet;

public class Multi_playerComponet: NetworkIdentity
{
    protected override void OnDespawned()
    {
        base.OnDespawned();
        enabled = isOwner;


    }
}

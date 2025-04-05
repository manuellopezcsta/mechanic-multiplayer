using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleHolder : BaseCounter
{
    public override void Interact(Player player)
    {
            // There is a car obj here already.
            if(player.HasCarObject()) {
                // Player is carrying something
            } else {
                // Player is not carrying anything.
                GetCarObject().SetCarObjectParent(player);
                //Borramos el objeto invisible.
                Destroy(gameObject);
            }
    }

    public void FixColliderSize() {
        BoxCollider collider = GetComponent<BoxCollider>();
        Tools.CopyColliderValues(GetCarObject().GetComponent<BoxCollider>(), collider);

        collider.enabled = true;

    }
}

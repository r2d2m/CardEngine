using System;
using UnityEngine;

namespace SadSapphicGames.CardEngine {
    public abstract class ResourceSO : ScriptableObject {
        public abstract PayResourceCommand CreatePayResourceCommand(AbstractActor actorToPay, int paymentMagnitude);
    }
}
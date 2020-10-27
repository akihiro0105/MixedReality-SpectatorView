using UnityEngine;

namespace Microsoft.MixedReality.SpectatorView
{
    internal class TrailRendererService : ComponentBroadcasterService<TrailRendererService, TrailRendererObserver>
    {
        public static readonly ShortID ID = new ShortID("TLR");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<TrailRendererBroadcaster>(typeof(TrailRenderer)));
        }
    }
}

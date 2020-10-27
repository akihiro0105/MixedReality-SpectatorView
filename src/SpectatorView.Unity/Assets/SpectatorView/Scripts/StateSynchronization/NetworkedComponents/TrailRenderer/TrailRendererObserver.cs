using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.SpectatorView
{
    internal class TrailRendererObserver : RendererObserver<TrailRenderer, TrailRendererService>
    {
        protected override void Read(INetworkConnection connection, BinaryReader message, byte changeType)
        {
            base.Read(connection, message, changeType);

            if (TrailRendererBroadcaster.HasFlag(changeType, TrailRendererBroadcaster.TrailRendererChangeType.StaticProperties))
            {
                Renderer.generateLightingData = message.ReadBoolean();
                Renderer.numCapVertices = message.ReadInt32();
                Renderer.textureMode = (LineTextureMode)message.ReadByte();
                Renderer.alignment = (LineAlignment)message.ReadByte();
                Renderer.colorGradient = ReadColorGradient(message);
                Renderer.widthCurve = ReadAnimationCurve(message);
                Renderer.time = message.ReadSingle();
            }
            if (TrailRendererBroadcaster.HasFlag(changeType, TrailRendererBroadcaster.TrailRendererChangeType.DynamicProperties))
            {
                Renderer.endColor = message.ReadColor();
                Renderer.startColor = message.ReadColor();
                Renderer.widthMultiplier = message.ReadSingle();
                Renderer.endWidth = message.ReadSingle();
                Renderer.startWidth = message.ReadSingle();
            }
        }

        private Gradient ReadColorGradient(BinaryReader message)
        {
            Gradient colorGradient = new Gradient();
            colorGradient.mode = (GradientMode)message.ReadByte();

            GradientColorKey[] colorKeys = message.ReadArray((BinaryReader msg) =>
            {
                return new GradientColorKey(msg.ReadColor(), msg.ReadSingle());
            });

            GradientAlphaKey[] alphaKeys = message.ReadArray((BinaryReader msg) =>
            {
                return new GradientAlphaKey(msg.ReadSingle(), msg.ReadSingle());
            });

            colorGradient.SetKeys(colorKeys, alphaKeys);
            return colorGradient;
        }

        private AnimationCurve ReadAnimationCurve(BinaryReader message)
        {
            AnimationCurve animationCurve = new AnimationCurve();
            animationCurve.preWrapMode = (WrapMode)message.ReadByte();
            animationCurve.postWrapMode = (WrapMode)message.ReadByte();

            animationCurve.keys = message.ReadArray((BinaryReader msg) =>
            {
                Keyframe elem = new Keyframe();
                elem.time = msg.ReadSingle();
                elem.value = msg.ReadSingle();
                elem.inTangent = msg.ReadSingle();
                elem.outTangent = msg.ReadSingle();
#if UNITY_2018_1_OR_NEWER
                elem.inWeight = msg.ReadSingle();
                elem.outWeight = msg.ReadSingle();
                elem.weightedMode = (WeightedMode)msg.ReadByte();
#else
                elem.tangentMode = msg.ReadInt32();
#endif
                return elem;
            });

            return animationCurve;
        }
    }
}

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.SpectatorView
{
    internal class TrailRendererBroadcaster : RendererBroadcaster<TrailRenderer, TrailRendererService>
    {
        public static class TrailRendererChangeType
        {
            public const byte StaticProperties = 0x8;
            public const byte DynamicProperties = 0x10;
        }

        private TrailRendererDynamicData previousData = new TrailRendererDynamicData();

        protected override byte InitialChangeType
        {
            get
            {
                return ChangeType.Enabled | TrailRendererChangeType.StaticProperties | TrailRendererChangeType.DynamicProperties | ChangeType.Materials;
            }
        }

        protected override byte CalculateDeltaChanges()
        {
            byte changeType = base.CalculateDeltaChanges();

            if (!previousData.Equals(Renderer))
            {
                previousData.Copy(Renderer);
                changeType |= TrailRendererChangeType.DynamicProperties;
            }

            return changeType;
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            base.WriteRenderer(message, changeType);

            if (HasFlag(changeType, TrailRendererChangeType.StaticProperties))
            {
                message.Write(Renderer.generateLightingData);
                message.Write(Renderer.numCapVertices);
                message.Write((byte)Renderer.textureMode);
                message.Write((byte)Renderer.alignment);
                WriteColorGradient(message, Renderer.colorGradient);
                WriteAnimationCurve(message, Renderer.widthCurve);
                message.Write(Renderer.time);
            }
            if (HasFlag(changeType, TrailRendererChangeType.DynamicProperties))
            {
                message.Write(Renderer.endColor);
                message.Write(Renderer.startColor);
                message.Write(Renderer.widthMultiplier);
                message.Write(Renderer.endWidth);
                message.Write(Renderer.startWidth);
            }
        }

        private void WriteColorGradient(BinaryWriter message, Gradient colorGradient)
        {
            message.Write((byte)colorGradient.mode);

            message.WriteArray(colorGradient.colorKeys, (msg, elem) =>
            {
                msg.Write(elem.color);
                msg.Write(elem.time);
            });

            message.WriteArray(colorGradient.alphaKeys, (msg, elem) =>
            {
                msg.Write(elem.alpha);
                msg.Write(elem.time);
            });
        }

        private void WriteAnimationCurve(BinaryWriter message, AnimationCurve animationCurve)
        {
            message.Write((byte)animationCurve.preWrapMode);
            message.Write((byte)animationCurve.postWrapMode);

            message.WriteArray(animationCurve.keys, (msg, elem) =>
            {
                msg.Write(elem.time);
                msg.Write(elem.value);
                msg.Write(elem.inTangent);
                msg.Write(elem.outTangent);
#if UNITY_2018_1_OR_NEWER
                msg.Write(elem.inWeight);
                msg.Write(elem.outWeight);
                msg.Write((byte)elem.weightedMode);
#else
                msg.Write(elem.tangentMode);

#endif
            });
        }

        class TrailRendererDynamicData
        {
            public Color endColor;
            public Color startColor;
            public float widthMultiplier;
            public float endWidth;
            public float startWidth;

            public bool Equals(TrailRenderer trailRenderer)
            {
                if (trailRenderer.endColor != endColor ||
                    trailRenderer.startColor != startColor ||
                    trailRenderer.widthMultiplier != widthMultiplier ||
                    trailRenderer.endWidth != endWidth ||
                    trailRenderer.startWidth != startWidth)
                    return false;

                return true;
            }

            public void Copy(TrailRenderer trailRenderer)
            {
                endColor = trailRenderer.endColor;
                startColor = trailRenderer.startColor;
                widthMultiplier = trailRenderer.widthMultiplier;
                endWidth = trailRenderer.endWidth;
                startWidth = trailRenderer.startWidth;
            }
        }
    }
}

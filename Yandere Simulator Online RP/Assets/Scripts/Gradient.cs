using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI {
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect {
        [SerializeField]
        private Type _gradientType;

        [SerializeField]
        private Blend _blendMode = Blend.Multiply;

        [SerializeField, Range(-1, 1)]
        private float _offset = 0f;

        [SerializeField]
        private UnityEngine.Gradient _effectGradient = new UnityEngine.Gradient {
            colorKeys = new GradientColorKey[] {
                new GradientColorKey(Color.black, 0),
                new GradientColorKey(Color.white, 1)
            }
        };

        #region Properties
        public Blend BlendMode {
            get => _blendMode;
            set => _blendMode = value;
        }

        public UnityEngine.Gradient EffectGradient {
            get => _effectGradient;
            set => _effectGradient = value;
        }

        public Type GradientType {
            get => _gradientType;
            set => _gradientType = value;
        }

        public float Offset {
            get => _offset;
            set => _offset = value;
        }
        #endregion

        public override void ModifyMesh(VertexHelper helper) {
            if (!IsActive() || helper.currentVertCount == 0)
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            helper.GetUIVertexStream(vertexList);

            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var vert in vertexList) {
                minY = Mathf.Min(minY, vert.position.y);
                maxY = Mathf.Max(maxY, vert.position.y);
            }

            float rangeY = maxY - minY;
            if (rangeY == 0) return;

            for (int i = 0; i < vertexList.Count; i++) {
                UIVertex vertex = vertexList[i];
                float normalizedY = (vertex.position.y - minY) / rangeY;
                vertex.color = BlendColor(vertex.color, EffectGradient.Evaluate(normalizedY - Offset));
                vertexList[i] = vertex;
            }

            helper.Clear();
            helper.AddUIVertexTriangleStream(vertexList);
        }

        private Color BlendColor(Color colorA, Color colorB) => BlendMode switch {
            Blend.Add => colorA + colorB,
            Blend.Multiply => colorA * colorB,
            _ => colorB
        };

        public enum Type {
            Horizontal,
            Vertical
        }

        public enum Blend {
            Override,
            Add,
            Multiply
        }
    }
}
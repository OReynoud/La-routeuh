using System.Collections;
using UnityEngine;

public class RippleBehavior : MonoBehaviour
{
    // Material properties
    [SerializeField] private Texture2D mainTex;
    [SerializeField] [ColorUsage(true, true)] private Color gradientColor;
    [SerializeField] private float gradientEdge;
    [SerializeField] private Vector3 rippleOrigin;
    [SerializeField] private float amplitude;
    [SerializeField] private float frequency;
    [SerializeField] [Range(0f, 10f)] private float rippleDensity;
    [SerializeField] [Range(0.1f, 20f)] private float effectRadius;
    [SerializeField] [Range(0.01f, 0.9f)] private float edgeBlend;
    private readonly float[] _activations = new float[10];

    // Mesh renderer
    [SerializeField] private MeshRenderer meshRenderer;
    
    // Time values
    [SerializeField] private float rippleDuration;
    private readonly Coroutine[] _rippleCoroutines = new Coroutine[10];
    
    // Shader property IDs
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int GradientColor = Shader.PropertyToID("_GradientColor");
    private static readonly int GradientEdge = Shader.PropertyToID("_GradientEdge");
    private static readonly int RippleOrigin = Shader.PropertyToID("_RippleOrigin");
    private static readonly int Amplitude = Shader.PropertyToID("_Amplitude");
    private static readonly int Frequency = Shader.PropertyToID("_Frequency");
    private static readonly int RippleDensity = Shader.PropertyToID("_RippleDensity");
    private static readonly int EffectRadius = Shader.PropertyToID("_EffectRadius");
    private static readonly int EdgeBlend = Shader.PropertyToID("_EdgeBlend");
    private static readonly int Activation = Shader.PropertyToID("_Activation");

    private void Awake()
    {
        foreach (var material in meshRenderer.materials)
        {
            material.SetTexture(MainTex, mainTex);
            material.SetColor(GradientColor, gradientColor);
            material.SetFloat(GradientEdge, gradientEdge);
            material.SetVector(RippleOrigin, rippleOrigin);
            material.SetFloat(Amplitude, amplitude);
            material.SetFloat(Frequency, frequency);
            material.SetFloat(RippleDensity, rippleDensity);
            material.SetFloat(EffectRadius, effectRadius);
            material.SetFloat(EdgeBlend, edgeBlend);
            material.SetFloat(Activation, 0f);
        }
    }

    private void Update()
    {
        for (var i = 0; i < _activations.Length; i++)
        {
            if (_activations[i] > 0f)
            {
                meshRenderer.materials[i].SetFloat(Activation, _activations[i]);
                meshRenderer.materials[i].SetFloat(Amplitude, amplitude);
                meshRenderer.materials[i].SetFloat(GradientEdge, gradientEdge);
            }
            else
            {
                meshRenderer.materials[i].SetFloat(Activation, 0f);
                meshRenderer.materials[i].SetFloat(Amplitude, 0f);
                meshRenderer.materials[i].SetFloat(GradientEdge, 0f);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var position = transform.position;
        for (var i = 0; i < _rippleCoroutines.Length; i++)
        {
            if (_rippleCoroutines[i] != null) continue;
            _rippleCoroutines[i] =
                StartCoroutine(RippleCoroutine(new Vector2(other.ClosestPointOnBounds(position).x,
                    other.ClosestPointOnBounds(position).z), i));
            break;
        }
    }
    
    private IEnumerator RippleCoroutine(Vector2 collisionPoint, int arrayIndex)
    {
        _activations[arrayIndex] = 1f;
        meshRenderer.materials[arrayIndex].SetVector(RippleOrigin, new Vector3(collisionPoint.x, 0f, collisionPoint.y) - transform.position);
        yield return new WaitForSeconds(rippleDuration);
        _activations[arrayIndex] = 0f;
        _rippleCoroutines[arrayIndex] = null;
    }

    // Amplitude and gradient edge to 0 when activation 0
}

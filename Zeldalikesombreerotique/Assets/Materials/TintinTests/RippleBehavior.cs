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
    [SerializeField] private float activation;
    
    // Mesh renderer
    [SerializeField] private MeshRenderer meshRenderer;
    
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
        meshRenderer.material.SetTexture(MainTex, mainTex);
        meshRenderer.material.SetColor(GradientColor, gradientColor);
        meshRenderer.material.SetFloat(GradientEdge, gradientEdge);
        meshRenderer.material.SetVector(RippleOrigin, rippleOrigin);
        meshRenderer.material.SetFloat(Amplitude, amplitude);
        meshRenderer.material.SetFloat(Frequency, frequency);
        meshRenderer.material.SetFloat(RippleDensity, rippleDensity);
        meshRenderer.material.SetFloat(EffectRadius, effectRadius);
        meshRenderer.material.SetFloat(EdgeBlend, edgeBlend);
        meshRenderer.material.SetFloat(Activation, activation);
    }
    
    // Amplitude and gradient edge to 0 when activation 0
}

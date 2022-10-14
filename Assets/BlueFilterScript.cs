using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlueFilterScript : MonoBehaviour
{
    public bool active;
    public Color color;
    public float satpeak;
    public float satmult;
    private Material material;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Custom/WhiteOrColorShader"));

        material.SetColor("_Color", color);
        material.SetFloat("SatPeak", satpeak);
        material.SetFloat("SatMult", satmult);
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (PlayerController.instance.power != PlayerController.Power.Immune)
        {
            Graphics.Blit(source, destination);
            return;
        }


        Graphics.Blit(source, destination, material);
    }
}

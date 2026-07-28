using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DayNightCameraBackground : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currentTime = 6f;

    [Tooltip("How many real seconds = one game day")]
    public float dayLength = 300f;

    public bool autoCycle = true;

    [Header("Sky Colors")]
    public Gradient skyGradient;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Create default gradient if none exists
        if (skyGradient.colorKeys.Length == 0)
            CreateDefaultGradient();
    }

    private void Update()
    {
        if (autoCycle)
        {
            currentTime += (24f / dayLength) * Time.deltaTime;

            if (currentTime >= 24f)
                currentTime -= 24f;
        }

        UpdateBackground();
    }

    void UpdateBackground()
    {
        float t = currentTime / 24f;
        cam.backgroundColor = skyGradient.Evaluate(t);
    }

    void CreateDefaultGradient()
    {
        GradientColorKey[] colors = new GradientColorKey[]
        {
            // Midnight
            new GradientColorKey(new Color32(8,10,30,255),0.00f),

            // Sunrise
            new GradientColorKey(new Color32(255,120,70,255),0.20f),

            // Morning
            new GradientColorKey(new Color32(135,206,255,255),0.30f),

            // Noon
            new GradientColorKey(new Color32(90,180,255,255),0.50f),

            // Afternoon
            new GradientColorKey(new Color32(120,200,255,255),0.65f),

            // Sunset
            new GradientColorKey(new Color32(255,130,80,255),0.78f),

            // Dusk
            new GradientColorKey(new Color32(60,60,120,255),0.88f),

            // Night
            new GradientColorKey(new Color32(10,15,45,255),1.00f)
        };

        GradientAlphaKey[] alpha = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1,0),
            new GradientAlphaKey(1,1)
        };

        skyGradient = new Gradient();
        skyGradient.SetKeys(colors, alpha);
    }

    public void SetTime(float hour)
    {
        currentTime = Mathf.Clamp(hour, 0, 24);
        UpdateBackground();
    }
}
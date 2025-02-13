using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Playables;

public class P_CTR_Control : MonoBehaviour
{
    [SerializeField] private Volume volume;
    ChromaticAberration chromaticAberration; 
    Vignette vignette;

    [SerializeField] float minIntensity = 0.3f; // Intensidad mínima cuando la vida está alta
    [SerializeField] float maxIntensity = 0.55f; // Intensidad máxima cuando la vida está baja
    [SerializeField] Color minColor = Color.black; // Color cuando la vida está alta
    [SerializeField] Color maxColor = Color.red; // Color cuando la vida está baja

    [SerializeField] BatterySystem batterySystem; // Referencia al sistema de baterías (o salud)
    float currentIntensity;
    Color currentColor;

    [SerializeField] float aberrationMin = 0.1f;
    [SerializeField] float aberrationMax = 0.7f;
    [SerializeField] float aberrationSpeed = 0.25f;

    bool isLerping = false;
    Coroutine lerpRoutine;

    private void Start()
    {
        if (volume != null && volume.profile.TryGet(out chromaticAberration))
        {
            chromaticAberration.intensity.value = aberrationMin;
            StartOscillation();
        }

        batterySystem = FindAnyObjectByType<BatterySystem>();

        if (volume != null && volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = minIntensity;
            vignette.color.value = minColor;
        }
    }

    private void Update()
    {
        if (batterySystem != null)
        {
            float lerpFactor = Mathf.InverseLerp(3f, 0f, batterySystem.currentPiles);

            currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, lerpFactor);

            currentColor = Color.Lerp(minColor, maxColor, lerpFactor);

            vignette.intensity.value = currentIntensity;
            vignette.color.value = currentColor;
        }
    }

    public void StartOscillation()
    {
        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        lerpRoutine = StartCoroutine(LerpChromaticAberration());
    }

    public void StopOscillation()
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
            lerpRoutine = null;
        }
    }

    public void SetAberration(float target)
    {
        if (!gameObject.activeInHierarchy) return;
        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        lerpRoutine = StartCoroutine(SmoothSetChromaticAberration(target));
    }

    private IEnumerator LerpChromaticAberration()
    {
        isLerping = true;
        while (true)
        {
            float time = 0;
            while (time < 1f)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(aberrationMin, aberrationMax, time);
                time += Time.deltaTime * aberrationSpeed;
                yield return null;
            }

            time = 0;
            while (time < 1f)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(aberrationMax, aberrationMin, time);
                time += Time.deltaTime * aberrationSpeed;
                yield return null;
            }
        }
    }

    private IEnumerator SmoothSetChromaticAberration(float target)
    {
        isLerping = true;
        float startValue = chromaticAberration.intensity.value;
        float time = 0;

        while (time < 1f)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(startValue, target, time);
            time += Time.deltaTime * 2f; // Velocidad de ajuste manual
            yield return null;
        }

        chromaticAberration.intensity.value = target;
        isLerping = false;
    }
}
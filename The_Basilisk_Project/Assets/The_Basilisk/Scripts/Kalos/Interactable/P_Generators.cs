using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_Generators : MonoBehaviour
{
    [Header("=== Generator Settings ===")]
    [SerializeField] float actualPower;
    [SerializeField] float maxPower = 30;
    [SerializeField] float actualFuel;
    [SerializeField] float maxFuel = 30;
    [SerializeField] bool isCharging;
    [SerializeField] bool powered;

    [Header("=== UI Settings ===")]
    public Slider progressBar;
    public Slider fuelBar;

    [Header("=== Effect Settings ===")]

    P_GameManager gameManager;
    P_Character_HookGrab playerGrab;

    void Start()
    {
        gameManager = FindAnyObjectByType<P_GameManager>();
        playerGrab = FindAnyObjectByType<P_Character_HookGrab>();

        actualPower = 0;
        actualFuel = 0;
        powered = false;
        progressBar.maxValue = maxPower;
        progressBar.value = 0;
        fuelBar.maxValue = maxFuel;
        fuelBar.value = actualFuel;
        progressBar.gameObject.SetActive(false);
        fuelBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (actualPower > maxPower)
        {
            actualPower = maxPower;
        }

        if (progressBar != null)
        {
            progressBar.value = actualPower;
        }

        if (fuelBar != null)
        {
            fuelBar.value = actualFuel;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuel") && playerGrab.grabbed)
        {
            StartCoroutine(AddFuel(10));
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            progressBar.gameObject.SetActive(true);
            fuelBar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            progressBar.gameObject.SetActive(false);
            fuelBar.gameObject.SetActive(false);
        }
    }

    /*
    void AddFuel()
    {
        actualFuel++;
        Debug.Log($"Combustible entregaddo: {actualFuel}/{maxFuel}");

        if (actualFuel >= maxFuel)
        {
            StartCoroutine(nameof(ChargeGenerator));
        }
    }
    */
    private IEnumerator AddFuel(float fuelAmount)
    {
        float targetFuel = actualFuel + fuelAmount;

        while (actualFuel < targetFuel)
        {
            actualFuel += Time.deltaTime;
            yield return null;
        }

        if(actualFuel >= maxFuel && !powered)
            StartCoroutine(nameof(ChargeGenerator));
    }


    private IEnumerator ChargeGenerator()
    {
        while (actualPower < maxPower)
        {
            actualPower += Time.deltaTime;
            yield return null;
        }

        ActivateGenerator();
    }

    void ActivateGenerator()
    {
        powered = true;
        StopAllCoroutines();
        gameManager.generatorsPowered++;
        Debug.Log("Generador activado");
    }
}

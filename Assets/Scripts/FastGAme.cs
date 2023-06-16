using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FastGAme : MonoBehaviour, IPointerDownHandler
{
    float time;
    bool countDown;
    private bool corutineCounter;
    [SerializeField] private GameObject tapToSpeedUpObject;
    [SerializeField] private float showTapToSpeedUpTime;
    [SerializeField] private float fastGameTime;
    public void OnPointerDown(PointerEventData eventData)
    {
        time = fastGameTime;
        MoneySignals.Signal_IsFast(true);
        Time.timeScale = 2;
        countDown = true;
        tapToSpeedUpObject.SetActive(false);
    }

    private void Update()
    {
        if (!countDown) return;
        time -= Time.deltaTime;
        if(time <= 0)
        {
            MoneySignals.Signal_IsFast(false);
            Time.timeScale = 1;
            countDown = false;
            if (!corutineCounter) StartCoroutine(ShowSpeedUpObject());
        }
    }

    IEnumerator ShowSpeedUpObject()
    {
        corutineCounter = true;
        yield return new WaitForSeconds(showTapToSpeedUpTime);
        tapToSpeedUpObject.SetActive(true);
        corutineCounter = false;
    }
}

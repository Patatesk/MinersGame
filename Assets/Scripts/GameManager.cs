using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Save;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour, ISaveble
{
    public int ActiveMountain
    {
        get { return activeMountain; }
        set
        {
            activeMountain = value;
            ChangeMountain();
        }
    }
    public GameObject[] allMountains;

    [SerializeField] private CinemachineVirtualCamera[] vCams;
    [SerializeField] private CinemachineBrain vCamBrain;

    [SerializeField] private GameObject gameCamera;
    [SerializeField] private int[] mountainCosts;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject SpawnButton;
    [SerializeField] private GameObject openWorkButton;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private GameObject[] incomeTexts;
    private bool firstLoad = true;



    [SerializeField] private int activeMountain = 0;
    private int beforeChangeMountain = 0;
    private int totalActiveMountain = 0;
    
    private void OnEnable()
    {
        GameManagerSignals.SetButtons += SetButtons;
        GameManagerSignals.CloseMountains += CloseMountains;
    }
    private void Awake()
    {
        vCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        CloseMountains();
        ChangeMountain();
    }

    private void Start()
    {
        CheckForBuyButton();
        if (mountainCosts.Length -1 >= activeMountain +1)
        SetButtonText(buyButton, mountainCosts[activeMountain]);
        if (firstLoad)
        {
            allMountains[0].GetComponent<Mountain>().OpenNewWorkPoint();
            allMountains[0].GetComponent<MinerController>().SpawnLevel1Miner();
            firstLoad = false;
        }

    }

    private void OnDisable()
    {
        GameManagerSignals.SetButtons -= SetButtons;
        GameManagerSignals.CloseMountains -= CloseMountains;

    }

    private void SetButtons(Action<GameObject, GameObject, GameObject> callBack)
    {
        callBack?.Invoke(upgradeButton, SpawnButton, openWorkButton);
    }

    public void ChangeMountainIndex(int change)
    {
        beforeChangeMountain = ActiveMountain;
        int activeNumber = activeMountain + change;
        ActiveMountain = activeNumber <= 0 ? 0 : activeNumber > allMountains.Length - 1 ? allMountains.Length - 1 : activeNumber;

        if (ActiveMountain == 0)
        {
            backButton.SetActive(false);
            forwardButton.SetActive(true);

        }
        else if (ActiveMountain == allMountains.Length - 1)
        {
            forwardButton.SetActive(false);
        }
        else
        {
            if (ActiveMountain == totalActiveMountain) return;
            forwardButton.SetActive(true);
            backButton.SetActive(true);
        }
    }

    private void ChangeMountain()
    {
        Mountain mountain = allMountains[beforeChangeMountain].transform.GetComponent<Mountain>();
        foreach (CinemachineVirtualCamera vCam in vCams)
        {
            vCam.gameObject.SetActive(false);
        }
        vCams[activeMountain].gameObject.SetActive(true);
        allMountains[beforeChangeMountain].SetActive(false);
        allMountains[activeMountain].SetActive(true);
        CheckForBuyButton();
        if (ActiveMountain == 0)
        {
            backButton.SetActive(false);
            forwardButton.SetActive(true);

        }
        else if (ActiveMountain == allMountains.Length - 1)
        {
            forwardButton.SetActive(false);
        }
        else
        {
            if (ActiveMountain == totalActiveMountain) return;
            forwardButton.SetActive(true);
            backButton.SetActive(true);
        }
    }

    private void CheckForBuyButton()
    {
        if (ActiveMountain == totalActiveMountain)
        {
            forwardButton.SetActive(false);
            if(ActiveMountain != 0) backButton.SetActive(true);

            buyButton.SetActive(true);
        }
        if(totalActiveMountain == allMountains.Length -1)
        {
            buyButton.SetActive(false);
        }
    }
    
    private void SetButtonText(GameObject button , int text)
    {
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = StringFormatter.StringIntConverter(text);
    }
    public void BuyMountainButton()
    {
        if (mountainCosts.Length >= activeMountain +1) MoneySignals.Signal_UseMoney(mountainCosts[activeMountain], BuyMountain);
        
    }
    private void BuyMountain(bool transaction)
    {
        if (transaction)
        {
            totalActiveMountain++;
            forwardButton.SetActive(true);
            if (mountainCosts.Length -1 >= activeMountain + 1) SetButtonText(buyButton, mountainCosts[activeMountain + 1]);
            incomeTexts[totalActiveMountain].SetActive(true);
            CheckForBuyButton();
        }
    }

    private void CloseMountains()
    {
        for (int i = 0; i < allMountains.Length; i++)
        {
            allMountains[i].SetActive(false);
        }
        allMountains[activeMountain].SetActive(true);
    }

    private IEnumerator BackGroundWorker(int money)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            MoneySignals.Signal_EarnMoney(money);
        }
    }

    public object SaveState()
    {
       return new SaveDataGM
        {
            BuyedMountain = totalActiveMountain,
            firstLoad = firstLoad,
        };
    }

    public void LoadState(object state)
    {
        var loadedData = (SaveDataGM)state;
        firstLoad = loadedData.firstLoad;
        for (int i = 0; i < loadedData.BuyedMountain; i++)
        {
            BuyMountain(true);
            ActiveMountain += 1;
        }
        CloseMountains();
    }
}
[Serializable]
public struct SaveDataGM
{
    public int BuyedMountain;
    public bool firstLoad;
}
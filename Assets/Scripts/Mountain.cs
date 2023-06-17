using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Save;
using Doozy.Runtime.UIManager.Components;
using MoreMountains.Feedbacks;



public class Mountain : MonoBehaviour, ISaveble
{
    public List<GameObject> WorkPoints = new List<GameObject>();
    public HashSet<SpawnPointHolder> spawnPoints = new HashSet<SpawnPointHolder>();
    public int MountainIndex;
    public int baseIncome;
    public int levelOneMinerStartCost;
    public int upgradeMinerStartCost = 10;
    public GameObject button;
    public int totalIncome;
    public int totalWorkPoint;
    [SerializeField] private TextMeshProUGUI workPointCostText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private MinerController MinerController;
    [SerializeField] private GameObject openParticle;
    [SerializeField] public GameObject workPointButton;
    [SerializeField] public int workPointCost;
    private Coroutine backGroundWorker;


    private void Start()
    {
        GameManagerSignals.Signal_SetButtons(DiscardButtons);
        GameManagerSignals.Signal_SetButtons(SetButtons);
    }
    public int IncomeSetter
    {
        get { return totalIncome; }
        set
        {
            totalIncome += value;
            UpdateIncomeText(totalIncome);
        }
    }

    public void OpenNewWorkPointButton()
    {
        MoneySignals.Signal_UseMoney(workPointCost, CheckPaymantForNewWorkPoint);
    }

    private void CheckPaymantForNewWorkPoint(bool complete)
    {
        if (complete) OpenNewWorkPoint();
        else return;
    }
    public void OpenNewWorkPoint()
    {
        if (WorkPoints.Count <= 0) return;
        for (int i = 0; i < WorkPoints[0].transform.GetChild(0).childCount; i++)
        {
            Instantiate(openParticle, WorkPoints[0].transform.GetChild(0).GetChild(i).position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        WorkPoints[0].transform.GetChild(0).gameObject.SetActive(false);

        for (int i = 1; i < WorkPoints[0].transform.childCount-1; i++)
        {
            MMF_Player feedback = null;
            if (i is 1 || i is 2 || i is 3) feedback = WorkPoints[0].transform.GetChild(7).GetChild(0).GetComponent<MMF_Player>();
            else feedback = WorkPoints[0].transform.GetChild(7).GetChild(1).GetComponent<MMF_Player>();
            spawnPoints.Add(new SpawnPointHolder
            {
                spawnPoint = WorkPoints[0].transform.GetChild(i),
                feedback = feedback,
            });
        }

        WorkPoints.RemoveAt(0);
        button.GetComponent<UIButton>().interactable = true;
        SaveLoadSignals.Signal_Save();
        workPointCost += workPointCost;
        totalWorkPoint++;
        if(totalWorkPoint is 3) MinerController.SetButtonText(workPointButton, 3);
        else MinerController.SetButtonText(workPointButton, workPointCost);



    }


    private void OnEnable()
    {
        incomeText.transform.parent.GetChild(0).GetComponent<Image>().enabled = true;
        GameManagerSignals.Signal_SetButtons(SetButtons);
        MoneySignals.Signal_SetWorker(MountainIndex, false, totalIncome);
    }
   
    private void OnDisable()
    {
        if (incomeText != null) incomeText.transform.parent.GetChild(0).GetComponent<Image>().enabled = false;
        GameManagerSignals.Signal_SetButtons(DiscardButtons);
        MoneySignals.Signal_SetWorker(MountainIndex, true, totalIncome);
    }

    private void Update()
    {
        if (Time.timeScale == 2)
        {
            UpdateIncomeText((totalIncome * 2));
        }
        else UpdateIncomeText((totalIncome));
    }
    private void SetButtons(GameObject upgrade, GameObject spawn, GameObject openNewWork)
    {
        openNewWork.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.AddListener(OpenNewWorkPointButton);
        upgrade.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.AddListener(MinerController.UpgradeMinerButton);
        spawn.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.AddListener(MinerController.spawnLevel1Button);
    }

    private void DiscardButtons(GameObject upgrade, GameObject spawn, GameObject openNewWork)
    {
        if (openNewWork != null) openNewWork.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.RemoveAllListeners();
        if (upgrade != null) upgrade.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.RemoveAllListeners();
        if (spawn != null) spawn.GetComponent<UIButton>().behaviours.GetBehaviour(Doozy.Runtime.UIManager.UIBehaviour.Name.PointerClick).Event.RemoveAllListeners();
    }

    public SpawnPointHolder GiveSpawnPoint()
    {
        SpawnPointHolder spawnPoint = null;
        foreach (SpawnPointHolder spawn in spawnPoints)
        {
            if (spawn.miners.Count == 0)
            {
                spawnPoint = spawn;
                break;
            }
        }

        if (spawnPoint == null)
        {
            button.GetComponent<UIButton>().interactable = false;
        }

        return spawnPoint;
    }

    public void OpenButton()
    {
        button.SetActive(true);
    }

    private void UpdateIncomeText(int income)
    {
        incomeText.text = StringFormatter.StringIntConverter(income);
    }

    public object SaveState()
    {
        List<SerializableMinerList> miners = new List<SerializableMinerList>();
        foreach (MinerList list in MinerController.minerByLevel)
        {
            miners.Add(new SerializableMinerList { level = list.level, count = list.miners.Count });
        }

        return new SaveData
        {
            openedWorkPoints = spawnPoints.Count / 6,
            workPointCost = this.workPointCost,
            list = miners,
            UpgradeMinerStartCost = this.upgradeMinerStartCost,
        };
    }

    public void LoadState(object state)
    {
        var loadedData = (SaveData)state;
        upgradeMinerStartCost = loadedData.UpgradeMinerStartCost;
        for (int i = 0; i < loadedData.openedWorkPoints; i++)
        {
            OpenNewWorkPoint();
        }
        for (int i = 0; i < loadedData.list.Count; i++)
        {
            MinerController.minerByLevel.Add(new MinerList { level = loadedData.list[i].level});
        }
        for (int j = 0; j < loadedData.list.Count; j++)
        {
            if (loadedData.list[j].count is 0) continue;
            for (int i = 0; i < loadedData.list[j].count; i++)
            {
                MinerController.SpawnMiner(j + 1);
            }
            
        }
        workPointCost = loadedData.workPointCost;
        
    }
}

[Serializable]
public class SpawnPointHolder
{
    public Transform spawnPoint;
    public MMF_Player feedback;
    public List<GameObject> miners = new List<GameObject>();
}
[Serializable]
public struct SaveData
{
    public int openedWorkPoints;
    public int workPointCost;
    public List<SerializableMinerList> list;
    public int UpgradeMinerStartCost;

}

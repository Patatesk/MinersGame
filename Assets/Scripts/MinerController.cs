using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Doozy.Runtime.UIManager.Components;
public class MinerController : MonoBehaviour
{
    public List<MinerList> minerByLevel = new List<MinerList>();
    private List<int> minersIncome = new List<int>();

    [SerializeField] GameObject[] miners;
    [SerializeField] private Mountain mountain;
    [SerializeField] GameObject button;


    private void Awake()
    {
        GetIncomes();
    }

    private void GetIncomes()
    {
        minersIncome.Add(mountain.baseIncome);
        minersIncome.Add(minersIncome[0] * 6);
        minersIncome.Add(minersIncome[1] * 6);
        minersIncome.Add(minersIncome[2] * 6);
        minersIncome.Add(minersIncome[3] * 6);
        minersIncome.Add(minersIncome[4] * 6);
        minersIncome.Add(minersIncome[5] * 6);
        minersIncome.Add(minersIncome[6] * 6);
    }
    private void OnEnable()
    {
        SetButtonText(mountain.button, mountain.levelOneMinerStartCost);
        SetButtonText(button, mountain.upgradeMinerStartCost);
        if (mountain.totalWorkPoint is 3) SetButtonText(mountain.workPointButton, 3);
        else SetButtonText(mountain.workPointButton, mountain.workPointCost);
        CheckUpgradebleMiner();
        CheckIfMinerSpawnable(); 
        MoneySignals.Signal_CheckEnoughMoney(CheckAndApply2, mountain.upgradeMinerStartCost);
    }
    public void spawnLevel1Button()
    {
        MoneySignals.Signal_UseMoney(mountain.levelOneMinerStartCost, CheckPaymentCompleteForSpawn);
    }

    public void UpgradeMinerButton()
    {
        MoneySignals.Signal_UseMoney(mountain.upgradeMinerStartCost, CheckPaymantForUpgrade);
    }

    private void CheckPaymentCompleteForSpawn(bool complete)
    {
        if (complete) SpawnLevel1Miner();
        else return;
    }

    private void CheckPaymantForUpgrade(bool complete)
    {
        if (complete) UpgradeMiner();
        else return;
    }
    private void CheckIfMinerSpawnable()
    {
        SpawnPointHolder spawnPoint = mountain.GiveSpawnPoint();
        if (spawnPoint is null) button.GetComponent<UIButton>().interactable = false;
        else button.GetComponent<UIButton>().interactable = true;
    }
    public void SpawnLevel1Miner(int level = 0)
    {
        mountain.levelOneMinerStartCost = Mathf.RoundToInt(mountain.levelOneMinerStartCost * 1.2f);
        SpawnPointHolder spawnPoint = mountain.GiveSpawnPoint();
        if (spawnPoint is null) return;
        GameObject miner = Instantiate(miners[level], spawnPoint.spawnPoint.position, spawnPoint.spawnPoint.rotation);
        miner.transform.parent = spawnPoint.spawnPoint;
        miner.GetComponent<Miner>().parent = spawnPoint;
        miner.GetComponent<Miner>().income = minersIncome[0];
        mountain.IncomeSetter = miner.GetComponent<Miner>().income;
        if (minerByLevel.Count < level + 1) minerByLevel.Add(new MinerList { level = level + 1 });
        minerByLevel[level].miners.Add(miner);
        spawnPoint.miners.Add(miner);
        CheckUpgradebleMiner();
        ChechEnoughMoneyAndApplyButton(mountain.button, mountain.levelOneMinerStartCost);
        SetButtonText(mountain.button, mountain.levelOneMinerStartCost);
        SaveLoadSignals.Signal_Save();
    }

    private void ChechEnoughMoneyAndApplyButton(GameObject button, int cost)
    {
        MoneySignals.Signal_CheckEnoughMoney(CheckAndApply, cost);
    }

    private void CheckAndApply(bool enough)
    {
        if (!enough) mountain.button.GetComponent<UIButton>().interactable = false;
        else mountain.button.GetComponent<UIButton>().interactable = true;
        if(mountain.GiveSpawnPoint() == null) mountain.button.GetComponent<UIButton>().interactable = false;
    }
    private void CheckAndApply2(bool enough)
    {
        if (!enough) button.GetComponent<UIButton>().interactable = false;
        else button.GetComponent<UIButton>().interactable = true;

        if(enough) CheckUpgradebleMiner();
    }


    private void CheckUpgradebleMiner()
    {
        UIButton button1 = button.GetComponent<UIButton>();
        bool noUpgrade = true;
        for (int i = 0; i < minerByLevel.Count; i++)
        {
            if (minerByLevel[i].miners.Count >= 3)
            {
                button1.interactable = true;
                noUpgrade = false;
                break;
            }
        }
        if(noUpgrade && button1.interactable) button1.interactable = false;
    }

    public void SetButtonText(GameObject button, int text)
    {
        if (text is 3) button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "MAX";
        else button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = StringFormatter.StringIntConverter(text);
    }

    public void UpgradeMiner()
    {
        int b = 0;
        mountain.upgradeMinerStartCost = Mathf.RoundToInt(mountain.upgradeMinerStartCost * 1.3f);
        bool merged = false;
        for (int i = 0; i < minerByLevel.Count; i++)
        {
            if (minerByLevel[i].miners.Count >= 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    GameObject miner = minerByLevel[i].miners[0];
                    miner.GetComponent<Miner>().parent.miners.Remove(miner);
                    b = miner.GetComponent<Miner>().level;
                    mountain.totalIncome -= miner.GetComponent<Miner>().income;
                    minerByLevel[i].miners.Remove(miner);
                    Destroy(miner);
                }
                if (b == 0) return;
                SpawnMiner(b + 1);
                mountain.OpenButton();
                CheckUpgradebleMiner();
                merged = true;
            }
            if (merged) break;
        }

        if (b == 0)
        {
            button.GetComponent<UIButton>().interactable = false;
        }
        SetButtonText(button, mountain.upgradeMinerStartCost);
        ChechEnoughMoneyAndApplyButton(mountain.button, mountain.levelOneMinerStartCost);
        MoneySignals.Signal_CheckEnoughMoney(CheckAndApply2, mountain.upgradeMinerStartCost);

    }

    public void SpawnMiner(int minerLevel)
    {
        SpawnPointHolder spawnPoint = mountain.GiveSpawnPoint();
        GameObject miner = Instantiate(miners[minerLevel - 1], spawnPoint.spawnPoint);
        miner.GetComponent<Miner>().parent = spawnPoint;
        if (minersIncome.Count == 0) GetIncomes();
        miner.GetComponent<Miner>().income = minersIncome[minerLevel - 1];
        mountain.IncomeSetter = miner.GetComponent<Miner>().income;
        spawnPoint.miners.Add(miner);
        if (minerByLevel.Count < minerLevel)
        {
            minerByLevel.Add(new MinerList { level = minerByLevel.Count });
        }
        minerByLevel[minerLevel - 1].miners.Add(miner);
        CheckUpgradebleMiner();
        SaveLoadSignals.Signal_Save();
        ChechEnoughMoneyAndApplyButton(mountain.button, mountain.levelOneMinerStartCost);
        MoneySignals.Signal_CheckEnoughMoney(CheckAndApply2, mountain.upgradeMinerStartCost);
    }

    private void Update()
    {
      
    }

}

[Serializable]
public class MinerList
{
    public int level;
    public List<GameObject> miners = new List<GameObject>();
}

[Serializable]
public struct SerializableMinerList
{
    public int level;
    public int count;
}
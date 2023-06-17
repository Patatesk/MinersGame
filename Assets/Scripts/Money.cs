using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using MoreMountains.Feedbacks;
using Save;


public class Money : MonoBehaviour ,ISaveble
{
    private int money = 300000000;
    private int income;
    bool isFast = false;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private MMF_Player moneyFeedBack;

    private Coroutine[] backgroundWorkers = new Coroutine[4];
    private bool[] workerPermison = new bool[4];
    private int[] mountaintIncome = new int[4];


    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            workerPermison[i] = true;
            mountaintIncome[i] = 0;
            backgroundWorkers[i] = StartCoroutine(Worker(i));
        }
    }
    IEnumerator Worker(int mountainIndex)
    {
        while (workerPermison[mountainIndex])
        {
            yield return new WaitForSecondsRealtime(1);
            AddMoney(mountaintIncome[mountainIndex]);
        }
    }

    private void SetWorker(int Index, bool  permision, int income)
    {
        workerPermison[Index] = permision;
        mountaintIncome[Index] = income;
        backgroundWorkers[Index] = StartCoroutine(Worker(Index));
    }
    private void OnEnable()
    {
        MoneySignals.UseMoney += UseMoney;
        MoneySignals.EarnMoney += AddMoney;
        MoneySignals.RemoveIncome += RemoveIncome;
        MoneySignals.IsFast += ChangeIsFast;
        MoneySignals.CheckEnoughMoney += CheckMoneyEnough;
        MoneySignals.SetWorker += SetWorker;
    }
    private void OnDisable()
    {
        MoneySignals.UseMoney -= UseMoney;
        MoneySignals.EarnMoney -= AddMoney;
        MoneySignals.RemoveIncome -= RemoveIncome;
        MoneySignals.IsFast -= ChangeIsFast;
        MoneySignals.CheckEnoughMoney -= CheckMoneyEnough;
        MoneySignals.SetWorker -= SetWorker;

    }
    private void ChangeIsFast(bool fast)
    {
        isFast = fast;
    }
    private void CheckMoneyEnough(Action<bool> callBack, int cost)
    {
        if (money < cost) callBack?.Invoke(false);
        else callBack?.Invoke(true);
    }
    private void UseMoney(int prize, Action<bool> transaction)
    {
        if (money >= prize)
        {
            money -= prize;
            transaction?.Invoke(true);
            moneyText.text = StringFormatter.StringIntConverter(money);
        }

        else
        {
            transaction?.Invoke(false);
            Debug.Log("Not Enough Money");
        }
    }

    private void AddMoney(int earnings)
    {
        money += earnings;
        moneyText.text = StringFormatter.StringIntConverter(money);
        if(earnings > 0) moneyFeedBack.PlayFeedbacks();
    }
   
    private void RemoveIncome(int _income)
    {
        income -= _income;
        if (income <0)
        {
            income = 0;
        }
    }

    public object SaveState()
    {
        return new SavedData { _money = money };
    }

    public void LoadState(object state)
    {
        var loadedData = (SavedData)state;
        money = loadedData._money;
        moneyText.text = StringFormatter.StringIntConverter(money);
    }
}
[Serializable]
public struct SavedData
{
    public int _money;
}

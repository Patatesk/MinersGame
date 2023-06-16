using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public static class MoneySignals
{
    public static event Action<int, Action<bool>> UseMoney;
    public static void Signal_UseMoney(int prize, Action<bool> callback)
    { UseMoney?.Invoke(prize, callback); }

    public static event Action<int> EarnMoney;
    public static void Signal_EarnMoney(int earning) 
    { EarnMoney?.Invoke(earning); }

    public static event Action<int> AddIncome;
    public static void Signal_AddIncome(int earning)
    { AddIncome?.Invoke(earning); }

    public static Action<int> RemoveIncome;
    public static void Signal_RemoveIncome(int income)
    { RemoveIncome?.Invoke(income); }
    public static Action<bool> IsFast;
    public static void Signal_IsFast(bool fast)
    { IsFast?.Invoke(fast); }

    public static event Action<TextMeshProUGUI> ChangeActiveIncomeText;
    public static void Signal_ChangeActiveIncomeText(TextMeshProUGUI text)
        { ChangeActiveIncomeText?.Invoke(text); }

    public static event Action<Action<bool>, int> CheckEnoughMoney;
    public static void Signal_CheckEnoughMoney(Action<bool> callBack, int cost)
    {
        CheckEnoughMoney?.Invoke(callBack, cost);
    }
    public static event Action<int, bool, int> SetWorker;
    public static void Signal_SetWorker(int Index, bool permison, int income)
    {
        SetWorker?.Invoke(Index, permison, income);
    }
}
   


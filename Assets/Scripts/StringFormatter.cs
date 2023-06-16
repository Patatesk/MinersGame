using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringFormatter
{
    public static string StringIntConverter(int value)
    {
        string output = "";
        if (value > 10000 && value < 100000)
        {
            value = value / 1000;
            output = value.ToString("F0") + "K";
        }
        else if (value > 100000 && value < 1000000)
        {
            value = value / 1000;
            output = string.Format("{0:0,0}", value).Replace(",", ".") + "K";
        }
        else if (value > 1000000 && value < 10000000)
        {
            value = value / 1000000;
            output = string.Format("{0:0,0}", value).Replace(",", ".") + "M";
        }

        else if (value > 10000000 && value < 100000000)
        {
            value = value / 1000000;
            output = string.Format("{0:0,0}", value).Replace(",", ".") + "M";
        }
        else if (value > 100000000 && value < 1000000000)
        {
            value = value / 100000000;
            output = string.Format("{0:0,0}", value).Replace(",", ".") + "M";
        }
        else if (value > 1000000000)
        {
            value = value / 1000000000;
            output = string.Format("{0:0,0}", value).Replace(",", ".") + "B";
        }
        else output = value.ToString("F0");
        return output;
    }
}

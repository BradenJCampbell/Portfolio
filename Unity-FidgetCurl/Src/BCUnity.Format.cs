using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BCUnity
{
    class Format
    {
        public static string Number(float number, int places, int decimal_places = 0)
        {
            if (decimal_places > 0)
            {
                int whole = Mathf.FloorToInt(number);
                int decimals = Mathf.RoundToInt((number - whole) * Mathf.Pow(10, decimal_places));
                return Number(whole, places) + "." + decimals.ToString().PadRight(decimal_places, '0');
            }
            return Number(Mathf.RoundToInt(number), places);
        }

        public static string Number(int number, int places = 0)
        {
            string ret = number.ToString();
            if (places <= ret.Length)
            {
                return ret;
            }
            return ret.PadLeft(places, '0');
        }
    }
}

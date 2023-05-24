using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    // <summary>
    // Enum 다음값 가져오기
    // </summary>
    // <typeparam name="T"></typeparam>
    // <param name="source"></param>
    // <returns></returns>

    public static T GetEnumNext<T>(this T source) where T : System.Enum
    {
        var array = System.Enum.GetValues(typeof(T));
        for (int i = 0; i < array.Length - 1; ++i)
        {
            if (source.Equals(array.GetValue(i)))
                return (T)array.GetValue(i + 1);
        }
        return (T)array.GetValue(0);
    }
    
    public static T GetEnumPrev<T>(this T source) where T : System.Enum
    {
        var array = System.Enum.GetValues(typeof(T));
        for (int i = 0; i < array.Length - 1; ++i)
        {
            if (source.Equals(array.GetValue(array.Length - i - 1)))
                return (T)array.GetValue(array.Length - i - 2);
        }
        return (T)array.GetValue(array.Length - 1);
    }
}

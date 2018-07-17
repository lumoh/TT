using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static List<T> ConcatListWithoutDuplicate<T>(IList<T> a1, IList<T> a2)
    {
        List<T> result = new List<T> ();
        T b;
        if(a1 != null)
        {
            for (int i = 0; i < a1.Count; i++) 
            {
                b = a1[i];
                //Unnecessary if a1 contains no duplicates in itself
                if(result.IndexOf(b) == -1)
                {
                    result.Add(b);
                }
            }
        }

        if(a2 != null)
        {
            for (int i = 0; i < a2.Count; i++) 
            {
                b = a2[i];
                if(result.IndexOf(b) == -1)
                {
                    result.Add(b);
                }
            }
        }
        return result;
    }
}
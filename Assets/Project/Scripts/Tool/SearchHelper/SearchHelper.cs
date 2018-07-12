using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchHelper
{
    static private SearchHelper instance;
    static public SearchHelper GetInstance()
    {
        if (null == instance)
        {
            instance = new SearchHelper();
        }
        return instance;
    }

    public T SerchChoise<T>(Dictionary<int,List<T>> dic,int currentgroup, int currentindex)
        where T :Component
    {
        T t = null;
        foreach (KeyValuePair<int, List<T>> item in dic)
        {
            int group = item.Key;
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (currentgroup == group && currentindex == i)
                {
                    t = item.Value[i];
                    return t;
                }
            }

        }
        return t;
    }

}

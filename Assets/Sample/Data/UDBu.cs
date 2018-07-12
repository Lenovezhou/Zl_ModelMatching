using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UDBu  {
    public class Bu
    {
        public string name;
        public int level;
        public List<Bu> bu_sub_list = new List<Bu>();
    }
    public List<Bu> bulist = new List<Bu>();
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData {
    private static GameData m_instance;
    public static GameData Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = new GameData();
            }
            return m_instance;
        }
    }


    public UDSkill playerSkill;

    public UDBu playerleftButtons;

    public SolutionItt playersolutions;
    /*构造*/
    private GameData() {
        //NOTE : this is Test Init Here.、
//        Debug.Log("skill_____gamedata____重载");
        playerSkill = new UDSkill();
        playerSkill.skills = new List<UDSkill.Skill>();
        for(int i=0; i < 10; i++)
        {
            UDSkill.Skill skill = new UDSkill.Skill();
			skill.name = "界面" + (i+1);
            skill.level = 1;
            skill.desc = "这是个牛逼的技能";
            playerSkill.skills.Add(skill);
        }
        playerleftButtons = new UDBu();
        playerleftButtons.bulist = new List<UDBu.Bu>();
        for (int i = 0; i < 5; i++)
        {
            UDBu.Bu b = new UDBu.Bu();
            b.level = 1;
            b.name = "<color=red>this:" + i + "</color>";
            playerleftButtons.bulist.Add(b);
            for (int j = 0; j < 2; j++)
            {
                UDBu.Bu bb = new UDBu.Bu();
                bb.level = 01;
                bb.name = "<color=green>child" + j + "</color>";
                //  Debug.Log(playerleftButtons.bu_sub_list == null);
                b.bu_sub_list.Add(bb);
            }
        }
        playersolutions = new SolutionItt();
        playersolutions.Solut_data = new List<SolutionItt.solut>();
        for (int i = 0; i < 6; i++)
        {
            SolutionItt.solut sss = new SolutionItt.solut();
            sss.name_main = "DayDayUP" + i;
            sss.tture_main = Resources.Load("card_bg_big_"+i) as Texture;
            //sss.tture_out = "out" + i;
            playersolutions.Solut_data.Add(sss);
        }
    }

    //private void CreatButtonData() 
    //{
    //    Debug.Log("buttons____GameData__-重载");
    //    playerleftButtons = new UDBu();
    //    playerleftButtons.bulist = new List<UDBu.Bu>();
    //    for (int i = 0; i < 20; i++)
    //    {
    //        UDBu.Bu b = new UDBu.Bu();
    //        b.level = 1;
    //        b.name = "<color=yellow>this:" + i + "</color>";
    //        playerleftButtons.bulist.Add(b);
    //    }
    //}


}

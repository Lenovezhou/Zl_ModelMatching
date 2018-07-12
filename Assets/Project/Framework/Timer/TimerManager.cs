using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TimerManager :Singleton<TimerManager>
{
    List<Timer> m_timer;
    public override void Awake()
    {
        base.Awake();
        if (m_timer == null)
        {
            m_timer = new List<Timer>();
        }
    }


    public Timer CreateTimer(Action call, int repeapes, float freqency) 
    {
        return Create(call,null,repeapes,freqency);
    }

    public Timer CreateTimer( Action<System.Object[]> argscall, int repeapes, float freqency, params System.Object[] args)
    {
        return Create(null, argscall, repeapes, freqency, args);
    }

    public Timer Create(Action call,Action<System.Object[]>argscall,int repeapes,float freqency,params System.Object[] args)
    {
        Timer t = new Timer(call,argscall,repeapes,freqency,args);
        m_timer.Add(t);
        return t;
    }


    public void FixedUpdate() 
    {
        if (m_timer.Count > 0)
        {
            for (int i = 0; i < m_timer.Count; i++)
            {
                Timer t = m_timer[i];
                if (t.m_lasttictime + t.m_frequency > Time.time)
                {
                    continue;
                }
                t.m_lasttictime = Time.time;
                if (t.m_reapet-- == 0)
                {
                    t.CleanUP();
                }
                else {
                    t.Notify();
                }
            }
        }
    }


    private void DestroyTimer(Timer t) 
    {
        t.CleanUP();
        m_timer.Remove(t);
    }

}
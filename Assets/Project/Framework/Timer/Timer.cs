using UnityEngine;
using System.Collections;
using System;



public class Timer 
{
    private System.Object[] m_args;
    private Action m_timerhandler;
    private Action<System.Object[]> m_arghandler;

    public int m_reapet;
    public float m_frequency;
    public float m_lasttictime;


    public Timer(Action call,Action<System.Object[]> arghandler,int reapet,float frequency,System.Object[] arges)
    {
        this.m_timerhandler = call;
        this.m_arghandler = arghandler;
        this.m_reapet = reapet == 0 ? 1 : reapet;
        this.m_frequency = frequency;
        this.m_lasttictime = Time.time;
        this.m_args = arges;
    }

    public void CleanUP() 
    {
        m_timerhandler = null;
        m_arghandler = null;
        m_reapet = 1;
        m_frequency = 0;
    }

    public void Notify() 
    {
        if (m_timerhandler != null)
        {
            m_timerhandler();
        }
        if (m_arghandler != null)
        {
            m_arghandler(m_args);
        }
    }
}

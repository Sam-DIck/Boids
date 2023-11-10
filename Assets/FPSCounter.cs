using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public List<float> frameTimes = new();
    public int frameLimit;
    public Text frameText;

    void Update()
    {
        frameTimes.Add(Time.unscaledDeltaTime);
        if (frameTimes.Count > frameLimit)
        {
            frameTimes.RemoveAt(0);
        }
        List<float> l = new List<float>(frameTimes);
        l.Sort();
        float mean = l.Sum() / l.Count;
        int p01 = 99*l.Count / 100;
        int p10 = 9*l.Count / 10;
        int p25 = 3*l.Count / 4;
        int p50 = l.Count / 2;
        int p75 = l.Count / 4;
        int p90 = l.Count / 10;
        int p99 = l.Count/ 100;
        float t01 = l[p01];
        float t10 = l[p10];
        float t25 = l[p25];
        float t50 = l[p50];
        float t75 = l[p75];
        float t90 = l[p90];
        float t99 = l[p99];

        frameText.text = $"mean:\t{(int)(1 / mean)}\n" +
            $"1%:\t{(int)(1 / t01)}\n" +
            $"10%:\t{(int)(1 / t10)}\n" +
            $"25%:\t{(int)(1 / t25)}\n" +
            $"50%:\t{(int)(1 / t50)}\n" +
            $"75%:\t{(int)(1 / t75)}\n" +
            $"90%:\t{(int)(1 / t90)}\n" +
            $"99%:\t{(int)(1 / t99)}";
    }
}

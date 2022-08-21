using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestFind : MonoBehaviour
{
    public GameObject Prefab;
    
    IEnumerator Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            var test = Instantiate(Prefab, transform, false);
            test.name = $"Test{i}";
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                var trans = transform.Find($"Test{i}");
            }
            Debug.Log(sw.ElapsedMilliseconds);
            sw.Stop();
        }
    }
}

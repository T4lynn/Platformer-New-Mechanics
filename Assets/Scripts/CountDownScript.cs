using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    public Slider countDownSlider;
    public bool dashActive;
    float maxValue = 5f;
    // Start is called before the first frame update
    void Start()
    {
        countDownSlider.maxValue = maxValue;
        countDownSlider.value = maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
             { dashActive = true; }

    if (dashActive) { countDown(); }
    }
    void countDown()
    {
        if (countDownSlider.value > 0)
        {
            countDownSlider.value = countDownSlider.value - 1 * Time.deltaTime;
        }
        else { dashActive = false; countDownSlider.value = maxValue; return; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    //makes a reference to the slider, a bool to keep track of whether the dash is active, 
    //and the max value of the slider. 
    public Slider countDownSlider;
    public bool dashActive;
    float maxValue = 5f;
   
    void Start()
    {
        //sets the slider's max value, and the current value of the slider to the max. 
        countDownSlider.maxValue = maxValue;
        countDownSlider.value = maxValue;
    }

    void Update()
    {
        //tracks when the E key is pressed. When it is, Dash active == true. 
        if (Input.GetKeyDown(KeyCode.E))
             { dashActive = true; }
        // runs count down when dashactive == true. 
    if (dashActive) { countDown(); }
    }
    //countdown checks the slider value isn't less than 0. If it isn't, it decrements the slider
    //value by time delta time per frame. 
    //When the slider's value is less than 0, it sets dashactive to false and returns the value
    //to max. 
    void countDown()
    {
        if (countDownSlider.value > 0)
        {
            countDownSlider.value = countDownSlider.value - 1 * Time.deltaTime;
        }
        else { dashActive = false; countDownSlider.value = maxValue; return; }
    }
}

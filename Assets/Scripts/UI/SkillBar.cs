using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public Gradient gradient;

    public void SetMaxSkill(int skill)
    {
        slider.maxValue = skill;
        slider.value = skill;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetSkill(int skill)
    {
        slider.value = skill;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class SliderUpdateSprite : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void Update()
    {
        UpdateSliderValue();
    }

    private void UpdateSliderValue()
    {
        fillImage.fillAmount = gameObject.GetComponent<Slider>().value;
    }
}

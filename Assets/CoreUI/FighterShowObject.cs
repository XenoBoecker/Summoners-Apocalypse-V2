

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FighterShowObject : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    public Fighter GetFighter => fighter;

    [SerializeField] GameObject panel;
    [SerializeField] Image isTargetableHighlightImage;
    [SerializeField] Image isThisFightersTurnHighlightImage;
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Slider healthSlider;

    [SerializeField] Sprite emptySprite;

    public delegate void FighterSelectedEventHandler(Fighter fighter);

    public event FighterSelectedEventHandler OnSelectFighter;

    float healthSliderMaxValue = 95;

    bool isClickable;

    private void Start()
    {
        SetTargetHighlightActive(false);
        ShowFighter();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowFighter();
        }
    }

    public void SetFighter(Fighter fighter)
    {
        this.fighter = fighter;

        ShowFighter();
    }

    private void ShowFighter()
    {
        if (fighter == null || fighter.fighterData == null)
        {
            panel.SetActive(true);
            ShowEmpty();
            return;
        }else if (fighter.isDead)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            image.sprite = fighter.fighterData.sprite;
            nameText.text = fighter.fighterName;

            // print(fighter.fighterName + " health: " + fighter.health + ", max health: " + fighter.fighterData.maxHealth);
            // print("SliderValue: " + fighter.health / fighter.fighterData.maxHealth * healthSliderMaxValue);

            healthSlider.GetComponent<RectTransform>().sizeDelta = new Vector2((float)fighter.health / fighter.fighterData.maxHealth * healthSliderMaxValue, healthSlider.GetComponent<RectTransform>().sizeDelta.y);

        }

    }

    void ShowEmpty()
    {
        image.sprite = emptySprite;
        nameText.text = "";

        healthSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(healthSliderMaxValue, healthSlider.GetComponent<RectTransform>().sizeDelta.y);

    }

    public void SetTargetHighlightActive(bool active)
    {
        isTargetableHighlightImage.enabled = active;
        SetClickable(true);
    }

    public void SetThisFightersTurnHighlightActive(bool active)
    {
        isThisFightersTurnHighlightImage.enabled = active;
        SetClickable(false);
    }

    public void SetClickable(bool clickable)
    {
        isClickable = clickable;
    }

    public void OnClickFighterShowObject()
    {
        if (isClickable) 
        {
            OnSelectFighter(fighter);
        }
    }
}

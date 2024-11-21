using UnityEngine;
using UnityEngine.UI;

public class FighterIconShowObject : MonoBehaviour
{
    [SerializeField] Image image;
    public void SetFighter(Fighter fighter)
    {
        image.sprite = fighter.fighterData.sprite;
    }
}

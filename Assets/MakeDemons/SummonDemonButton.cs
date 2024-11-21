using TMPro;
using UnityEngine;

namespace SummonersApocalypse.MakeDemons
{
    public class SummonDemonButton : MonoBehaviour
    {

        public TMP_InputField Input;

        public CreateDemonAI Dialogue;

        public void Talk()
        {
            Dialogue.Talk(Input.text);
        }
    }
}
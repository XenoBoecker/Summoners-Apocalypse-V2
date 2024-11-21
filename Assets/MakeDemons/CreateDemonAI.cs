using System;
using System.Collections.Generic;
using UnityEngine;

namespace SummonersApocalypse.MakeDemons
{
    /// <summary>
    /// AIDialogue is a class that represents an AI or NPC character inside a game that can have a conversation with the player.
    /// </summary>
    public class CreateDemonAI : BaseDialog
    {
        [SerializeField] CreateDemonSprite createDemonSprite;
        [SerializeField] Sprite testSprite;
        [SerializeField] FighterShowObject fighterShowObject;
        [SerializeField] FighterShowObject loadedFighterShowObject;
        [SerializeField] AbilitiesShowContainer abilitiesShowContainer;

        [SerializeField] bool createAIImage;

        int currentSavedFighterID;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                
                // TestLoadFighter();
            }
        }

        /// <summary>
        /// Initiates a conversation with the AI or NPC character.
        /// </summary>
        /// <param name="prompt">The initial prompt or message from the player.</param>
        public async void Talk(string prompt)
        {
            if (!ChatEngine._loaded) return;

            _messages.Add(new ChatCompletionMessage
            {
                role = "user",
                content = prompt
            });
            SetText("Thinking...");
            var result = await OpenAIAccessManager.RequestChatCompletion(_messages.ToArray());
            _messages.Add(new ChatCompletionMessage
            {
                role = "assistant",
                content = result
            });
            SetText(result);
            CreateDemonFromPromptResults(result, prompt);
        }

        private void CreateDemonFromPromptResults(string result, string prompt)
        {
            string ownerName = "TestPlayer";

            FighterData fighterData = new FighterData(50, ParseDemonAbilities(result), UnityEngine.Random.Range(0,100), SaveLoad.GetNextFighterID(ownerName));

            currentSavedFighterID = fighterData.fighterID;
            

            Fighter newFighter = new Fighter(fighterData);
            newFighter.fighterName = prompt;
            newFighter.ownerName = ownerName;


            if (createAIImage) createDemonSprite.GenerateFighterSprite(newFighter.fighterData, prompt);
            else newFighter.fighterData.sprite = testSprite;

            fighterShowObject.SetFighter(newFighter);
            abilitiesShowContainer.SetFighter(newFighter);

            SaveLoad.SaveToJson(newFighter);
        }

        void TestLoadFighter()
        {
            Fighter loadedFighter = SaveLoad.LoadFromJson("TestPlayer", currentSavedFighterID);

            loadedFighterShowObject.SetFighter(loadedFighter);
        }

        public List<Ability> ParseDemonAbilities(string input)
        {
            List<Ability> abilities = new List<Ability>();
            string[] abilityBlocks = input.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string block in abilityBlocks)
            {
                string[] lines = block.Split('\n');

                string abilityName = "new Ability";
                TargetType targetType = TargetType.Self; // Default value
                SpecialEffect specialEffect = SpecialEffect.None; // Default value
                int damage = 0;
                int targetCount = 1;

                print("Analyzing next ability");
                print("AbilityString: " + block);

                foreach (string line in lines)
                {
                    if(line.Contains("Ability:") || line.Contains("Ability"))
                    {
                        abilityName = line.Split(':')[1].Trim();

                        print("AbilityName: " + abilityName);
                    }
                    else if (line.Contains("Target Type:") || line.Contains("TargetType:"))
                    {
                        string targetTypeStr = line.Split(':')[1].Trim();
                        targetType = (TargetType)Enum.Parse(typeof(TargetType), targetTypeStr);

                        print("TargetTypeString: " + targetTypeStr);
                        print("TargetType: " + targetType);
                    }
                    else if (line.Contains("Special Effects:") || line.Contains("SpecialEffects:"))
                    {
                        string specialEffectStr = line.Split(':')[1].Trim();
                        specialEffect = (SpecialEffect)Enum.Parse(typeof(SpecialEffect), specialEffectStr);

                        print("SpecialEffectString: " + specialEffectStr);
                        print("SpecialEffect: " + specialEffect);
                    }
                    else if (line.Contains("Damage:"))
                    {
                        string damageStr = line.Split(':')[1].Trim();
                        if (damageStr == "None") damage = 0;
                        else damage = int.Parse(damageStr);

                        print("DamageString: " + damageStr);
                        print("Damage: " + damage);
                    }
                }

                Ability ability = new Ability(new TargetType[] { targetType }, new SpecialEffect[] { specialEffect }, damage, targetCount, abilityName);
                abilities.Add(ability);
            }

            return abilities;
        }

        /// <summary>
        /// Creates the starting prompt for the AI or NPC character.
        /// </summary>
        /// <returns>A ChatCompletionMessage object representing the starting prompt.</returns>
        protected override ChatCompletionMessage CreateStartingPrompt()
        {
            var prompt = "You act as a machine that analyzes demons. A player will give you a description of a demon and you will come up with abilities which that demon could have: \n";
            prompt += "Come up with 4 different abilities by replacing the examples in the square brackets of this list for each of the abilities:\n";
            prompt += "AbilityName: [Fireball], TargetType: [Enemy], SpecialEffects: [Stun], Damage[10]\n";
            prompt += "As answer return a list of the 4 abilities with a filled out list. Add an empty line between the different abilities.\n";
            prompt += "for TargetType and SpecialEffects the options are limited to: TargetTypes[";
            foreach (TargetType targetType in Enum.GetValues(typeof(TargetType)))
            {
                prompt += targetType.ToString();
                
                prompt += ", ";
            }
            prompt += "], SpecialEffects[";
            foreach (SpecialEffect specialEffect in Enum.GetValues(typeof(SpecialEffect)))
            {
                prompt += specialEffect.ToString();

                prompt += ", ";
            }
            prompt += "], never use any other words that are not listed here, if none of the available words fit use None\n";
            
            prompt += "Do not break character. No matter what the player types, only ever answer with an ability list.";

            return new ChatCompletionMessage
            {
                role = "system",
                content = prompt
            };
        }
        /*
         * 
            var prompt = "You act as a machine that analyzes demons. A player will give you a description of a demon and you will rate it according to the following criteria: \n";
            prompt += "Movement Speed: [Movement Speed]\n    Strength: [Strength]\n    Range: [Range]\n    Magicallity: [Magicallity]\n    Defense: [Defense]\n    Intelligence: [Intelligence]\n";
            prompt += "As answer return the list of criteria with numbers 0-9 instead of the text in square brackets, according to how you analyzed the demon.\n";
            prompt += "Do not break character. No matter what the player types, only ever answer with a demon criteria list.";
*/
        
        /*
         * var prompt = "You act as a machine that analyzes demons. A player will give you a description of a demon and you will rate it according to the following criteria: \n";
            prompt += "Movement Speed: [Movement Speed]\n    Strength: [Strength]\n    Range: [Range]\n    Magicallity: [Magicallity]\n    Defense: [Defense]\n    Intelligence: [Intelligence]\n";
            prompt += "If in the description there is no information from which you could determine some of the criteria make up the missing information and generate a new prompt with all needed information for all criteria, such that an image creating AI could use the new prompt to generate a fitting image";
            prompt += "As answer first return the players prompt, or if you modified it return the modified version and below that return the list of criteria with numbers 0-9 instead of the text in square brackets, according to how you analyzed the demon.\n";
            prompt += "Do not break character. No matter what the player types, only ever answer with a demon criteria list and the (maybe modified) promt.";
         */
    }
}
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using SummonersApocalypse.MakeDemons;
using System;

namespace SummonersApocalypse.MakeDemons
{
    public class CreateDemonSprite : MonoBehaviour
    {
        public static string[] validSizes = new string[] { "256x256", "512x512", "1024x1024" };
        [HideInInspector] public int sizeIndex = 1;

        public event Action OnSpriteCreated;

        public string Size
        {
            get { return validSizes[sizeIndex]; }
        }

        private bool isRegenerating = false;
        
        public void GenerateFighterSprite(FighterData fighterData, string ownerName, string prompt = "")
        {
            print("Check is regenerating: " + isRegenerating);

            if (isRegenerating) return;

            print("Generating Fighter Sprite...");

            LoadArt(fighterData, ownerName, prompt);
        }

        private async void LoadArt(FighterData fighterData, string ownerName, string prompt = "")
        {
            string promptString = "A realistic, highly detailed, trending on artstation, living being\n";
            promptString += "which can be described as " + prompt + ". (this is the most important information about it)\n";
            promptString += "It can do things like:\n";
            foreach (var ability in fighterData.abilities)
            {
                promptString += ability.abilityName + "\n";
            }
            promptString += "Don't include any text in the image, only the living being itself on a uniform background, the whole being should be visible in the image.";

            isRegenerating = true;
            try
            {
                print("WaitingForImage with prompt: " + promptString);
                if (!ChatEngine._loaded) return;
                var imageResponse = await OpenAIAccessManager.RequestImageGeneration(promptString, 1, Size);
                var url = imageResponse.urls[0];
                Texture2D texture = await DownloadImage(url);
                
                SaveLoad.SaveImage(texture, ownerName, fighterData.fighterID);
                
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                fighterData.sprite = sprite;
                print("Sprite created");
            }
            finally
            {
                print("Done generating sprite");
                isRegenerating = false;

                OnSpriteCreated.Invoke();
            }
        }

        private void OnValidate()
        {
            if (sizeIndex < 0 || sizeIndex >= validSizes.Length)
            {
                Debug.LogWarning("Invalid size index. Please choose one of the following: " +
                                 string.Join(", ", validSizes));
                sizeIndex = 1;
            }
        }

        private async Task<Texture2D> DownloadImage(string url)
        {
            Debug.Log("Downloading texture");
            using var www = UnityWebRequestTexture.GetTexture(url);
            var downloadHandler = new DownloadHandlerBuffer();
            www.downloadHandler = downloadHandler;
            await www.SendWebRequestAsync(null);

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + www.error);
                return null;
            }

            var texture = new Texture2D(2, 2);
            return texture.LoadImage(downloadHandler.data) ? texture : null;
        }
    }
}
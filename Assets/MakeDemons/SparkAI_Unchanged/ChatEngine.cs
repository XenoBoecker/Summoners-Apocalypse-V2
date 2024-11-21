using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SummonersApocalypse.MakeDemons
{
    class AuthCredentials
    {
        public string api_key;
        public float temperature = 0.2f;
    }
    
    /// <summary>
    /// The ChatEngine class is responsible for loading the OpenAI API credentials and setting the API key and temperature.
    /// </summary>
    public class ChatEngine : MonoBehaviour
    {
        public static bool _loaded;
        public TextAsset _authFile;
        
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        private void Awake()
        {
            if(_loaded) return;
            
            if (_authFile != null)
            {
                // Parse the JSON data
                var jsonData = _authFile.text;
                var creds = JsonUtility.FromJson<AuthCredentials>(jsonData);
                
                OpenAIAccessManager.SetAPIKey(creds.api_key);
                OpenAIAccessManager.Temperature = creds.temperature;
                Debug.Log("Loaded auth file for the OpenAI API");
                _loaded = true;
            }
            else
            {
                Debug.Log("Failed to load or find auth file for the OpenAI API");
            }

            Debug.Log("_loaded: " + _loaded);
        }

        void Start()
        {
            Debug.Log("start: _loaded: " + _loaded);
        }
    }
}
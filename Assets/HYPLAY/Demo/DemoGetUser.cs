using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HYPLAY.Core.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HYPLAY.Demo
{
    public class DemoGetUser : MonoBehaviour
    {
        public static DemoGetUser instance; // Static instance to hold the singleton
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private CanvasGroup signInSplash;

        private void Awake()
        {
            // If the instance already exists and it's not this, destroy the duplicate
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject); // Make this object persistent across scenes
            }
            else if (instance != this)
            {
                Destroy(gameObject); // Destroy duplicate instance
                return; // Exit to avoid further initialization
            }

            // Subscribe to the logged-in event
            HyplayBridge.LoggedIn += OnLoggedIn;

            // If already logged in, call the logged-in handler
            if (HyplayBridge.IsLoggedIn)
                OnLoggedIn();
        }

        private async void OnLoggedIn()
        {
            signInSplash.alpha = 0;
            signInSplash.blocksRaycasts = false;
            var res = await HyplayBridge.GetUserAsync();
            if (res.Success)
                text.text = $"Welcome {res.Data.Username}";
        }
    }
}

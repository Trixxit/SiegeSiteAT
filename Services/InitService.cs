#define ForceReset
//#define SkipLoading

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FPSHome.Services
{
    public partial class InitializationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;

        public bool IsInitialized { get; private set; }

        public event Action<int> OnProgressChanged;
        public event Action OnProgressCompleted;

        public InitializationService(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public async Task Initialize()
        {
#if !SkipLoading
            await _jsRuntime.InvokeVoidAsync("logMessage", "Initing");
            await Task.Delay(1000);
            if (!IsInitialized)
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", "First Load~");
                IsInitialized = true;
                bool inited = IsDataInitialized();
                await _jsRuntime.InvokeVoidAsync("logMessage", "Inited: " + inited);
                if (!inited)
                {
                    await _jsRuntime.InvokeVoidAsync("logMessage", "Initting owo");
                    await LoadStoreUserData();
                    await GenerateRandomPeople();
                    await _jsRuntime.InvokeVoidAsync("logMessage", "Finished!");
                }
                else
                {
                    await _jsRuntime.InvokeVoidAsync("logMessage", "Data already in memory");
                }
            }
#endif
            OnProgressCompleted?.Invoke();
        }

        private bool IsDataInitialized()
        {
            return false;
        }

        private async Task GenerateRandomPeople()
        {
            await _jsRuntime.InvokeVoidAsync("logMessage", $"Gwah");
            await Task.Delay(5000);
            await GenerateUserData(30);
        }

        private async Task LoadStoreUserData()
            => await ProcessUserData(await FetchAndDeserializeFile($"/JSON/user_data.json"));

        private async Task<Dictionary<string, User>> FetchAndDeserializeFile(string fileName)
        {
            await _jsRuntime.InvokeVoidAsync("logMessage", $"Loading {fileName}...");
            var jsonData = await _jsRuntime.InvokeAsync<string>("fetchData", fileName);
            await _jsRuntime.InvokeVoidAsync("logMessage", $"Finished fetch for {fileName}.");
            var userData = JsonSerializer.Deserialize<Dictionary<string, User>>(jsonData);
            await _jsRuntime.InvokeVoidAsync("logMessage", "Deserialized...");
            return userData;
        }

        private async Task<int> ProcessUserData(Dictionary<string, User> userData)
        {
            if (userData != null)
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", "Userdata NOT null, loading");
                int i = 0;
                int len = userData.Count;
                foreach (var (key, user) in userData)
                {
                    i++;
                    if (!Userdata.TryAdd(key, user))
                        Userdata[key] = user;

                    int progress = (int)(((double)i / len) * 100);
                    await _jsRuntime.InvokeVoidAsync("logMessage", $"Processing user: {key}, #{i} out of ~{len} (Progress: {progress}%)");
                    OnProgressChanged?.Invoke(progress);
                }
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", "Userdata was null");
            }

            return userData.Count;
        }
    }
}
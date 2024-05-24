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

        public bool IsInitialized { get; private set; }

        public event Action<int> OnProgressChanged;
        public event Action OnProgressCompleted;

        public InitializationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task Initialize()
        {
#if !SkipLoading
            await _jsRuntime.InvokeVoidAsync("logMessage", "Initing");
            if (!IsInitialized)
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", "First Load~");
                IsInitialized = true;
                bool inited = IsDataInitialized();
                await _jsRuntime.InvokeVoidAsync("logMessage", "Inited: " + inited);
                if (!inited)
                {
                    await _jsRuntime.InvokeVoidAsync("logMessage", "Initting owo");
                    await LoadAndStoreData();
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
            return true;
        }

        private async Task LoadAndStoreData()
        {
            int totalFiles = 3;
            int totalUsers = totalFiles * 30;
            int processedUsers = 0;

            for (int i = 0; i < totalFiles; i++)
            {
                var userData = await FetchAndDeserializeFile($"/JSON/fake_data_{i}.json");
                if (userData != null)
                {
                    processedUsers += await ProcessUserData(userData, totalUsers, processedUsers);
                }
            }
        }

        private async Task<Dictionary<string, User>> FetchAndDeserializeFile(string fileName)
        {
            await _jsRuntime.InvokeVoidAsync("logMessage", $"Loading {fileName}...");

            // Fetch the data asynchronously
            var jsonData = await _jsRuntime.InvokeAsync<string>("fetchData", fileName);
            await _jsRuntime.InvokeVoidAsync("logMessage", $"Finished fetch for {fileName}.");

            // Deserialize the data (blocking operation)
            var userData = JsonSerializer.Deserialize<Dictionary<string, User>>(jsonData);
            await _jsRuntime.InvokeVoidAsync("logMessage", "Deserialized...");

            return userData;
        }

        private async Task<int> ProcessUserData(Dictionary<string, User> userData, int totalUsers, int processedUsers)
        {
            if (userData != null)
            {
                await _jsRuntime.InvokeVoidAsync("logMessage", "Userdata NOT null, loading");

                foreach (var (key, user) in userData)
                {
                    processedUsers++;
                    Userdata[key] = user;

                    int progress = (int)(((double)processedUsers / totalUsers) * 100);
                    if (processedUsers % 10 == 0) // Report progress every 10 users to reduce overhead
                    {
                        await _jsRuntime.InvokeVoidAsync("logMessage", $"Processing user: {key}, #{processedUsers} out of ~{totalUsers} (Progress: {progress}%)");
                        OnProgressChanged?.Invoke(progress);
                    }
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

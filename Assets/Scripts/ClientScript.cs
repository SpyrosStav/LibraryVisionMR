using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;


public class ClientScript: MonoBehaviour
{
    string baseAddress = "";
    string BaseSharingUrl = "https://myasasharingservice20-7.azurewebsites.net/swagger/api/anchors";
    
    void Awake()
    {
        Uri result;
        if (!Uri.TryCreate(BaseSharingUrl, UriKind.Absolute, out result))
        {
            Debug.LogError($"{nameof(BaseSharingUrl)} is not a valid url");
            return;
        }
        else
        {
            BaseSharingUrl = $"{result.Scheme}://{result.Host}/api/anchors";
            baseAddress = BaseSharingUrl;
            Debug.Log("Created URI at " + baseAddress);
        }
    }
    

    public async Task<string> RetrieveAllAnchorKeys(string mode)
    {
        try
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(baseAddress + "/all/" + mode);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Failed to retrieve anchor keys.");
            return null;
        }
    }

    public async Task<long> StoreAnchorKey(string anchorKey, string mode)
    {
        if (string.IsNullOrWhiteSpace(anchorKey))
        {
            return -1;
        }

        try
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{baseAddress}/{anchorKey}/{mode}", new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                long ret;
                if (long.TryParse(responseBody, out ret))
                {
                    Debug.Log("Key " + ret.ToString());
                    return ret;
                }
                else
                {
                    Debug.LogError($"Failed to store the anchor key. Failed to parse the response body to a long: {responseBody}.");
                }
            }
            else
            {
                Debug.LogError($"Failed to store the anchor key: {response.StatusCode} {response.ReasonPhrase}.");
            }

            Debug.LogError($"Failed to store the anchor key: {anchorKey}.");
            return -1;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError($"Failed to store the anchor key: {anchorKey}.");
            return -1;
        }
    }

    public async Task<string> RetrieveQuiz(string mode)
    {
        try
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(baseAddress + "/quiz/" + mode);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Failed to retrieve quiz.");
            return null;
        }
    }

    public async Task<string> RetrieveContent(string keyword)
    {
        try
        {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(baseAddress + "/feedback/" + keyword);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Failed to retrieve content.");
            return null;
        }
    }

    public async Task<long> PostContent(string keyword)
    {
        try
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync($"{baseAddress}/feedback/{keyword}", new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                long ret;
                if (long.TryParse(responseBody, out ret))
                {
                    Debug.Log("Key " + ret.ToString());
                    return ret;
                }
                else
                {
                    Debug.LogError($"Failed to store the keyword. Failed to parse the response body to a long: {responseBody}.");
                }
            }
            else
            {
                Debug.LogError($"Failed to store the keyword: {response.StatusCode} {response.ReasonPhrase}.");
            }

            Debug.LogError($"Failed to store the keyword: {keyword}.");
            return -1;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError($"Failed to store the keyword: {keyword}.");
            return -1;
        }
    }

    public async void DeleteAnchorKeys(string mode)
    {
        try
        {
            HttpClient client = new HttpClient();
            var response = await client.DeleteAsync($"{baseAddress}/{mode}");
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                long ret;
                if (long.TryParse(responseBody, out ret))
                {
                    Debug.Log("Key " + ret.ToString());
                    return;
                }
                else
                {
                    Debug.LogError($"Failed to delete. Failed to parse the response body to a long: {responseBody}.");
                }
            }
            else
            {
                Debug.LogError($"Failed to delete: {response.StatusCode} {response.ReasonPhrase}.");
            }

            Debug.LogError("Failed to delete.");
            return;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            Debug.LogError("Failed to delete.");
            return;
        }
    }
}
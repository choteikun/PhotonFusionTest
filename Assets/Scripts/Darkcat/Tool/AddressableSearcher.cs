using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
public class AddressableSearcher
{
    public static async Task<T> GetAddressableAssetAsync<T>(string assetAddress)
    {
        var resultTask = Addressables.LoadAssetAsync<T>(assetAddress).Task;
        T result = await resultTask;
        return result;
    }

}

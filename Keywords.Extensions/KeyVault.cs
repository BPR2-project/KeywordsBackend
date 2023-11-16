using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Keywords.Extensions;

public class KeyVault
{
    public enum VaultSecrets
    {
        keywordsdb,
        blobcontainer,
        blobstorageuri,
        ttskey,
        ttsregion
    }
}
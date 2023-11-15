using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Keywords.Extensions;

public class KeyVault
{
    private string _keyVaultUri = "https://keywordskeyvault.vault.azure.net";
    static SecretClient? _azureKeyVaultSecretClient;

    public KeyVault(){
        var credentials = new DefaultAzureCredential();
        _azureKeyVaultSecretClient = new SecretClient(new
            Uri(_keyVaultUri), credentials);
    }

    public string GetSecret(string name)
    {
        return _azureKeyVaultSecretClient.GetSecret(name).Value.Value;
    }
    
    public enum VaultSecrets
    {
        blobcontainer,
        blobstorageuri,
        ttskey,
        ttsregion
    }
}
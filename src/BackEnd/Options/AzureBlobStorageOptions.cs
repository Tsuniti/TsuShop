﻿namespace TsuShopWebApi.Options;

public class AzureBlobStorageOptions
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}
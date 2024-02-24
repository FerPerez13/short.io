namespace Short.IO.API;

public class StorageConfiguration
{
    public string ConnectionString { get; set; }
    public AzureTableConfiguration AzureTable { get; set; }
}

public class AzureTableConfiguration
{
    /// <summary>
    /// Tabla de Azure donde se guardaran los datos de las URLs
    /// </summary>
    public string UrlTableName { get; set; }
    
    /// <summary>
    /// Nombre de la propiedad de BlobReference.Metadata que actuara como la PartitionKey
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Nombre de la propiedad de BlobReference.Metadata que actuara como la RowKey
    /// </summary>
    public string RowKey { get; set; }
}
namespace Catalog.Api.Settings
{
    public interface ICatalogDatabaseSettings
    {
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
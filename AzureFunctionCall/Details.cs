using Microsoft.Azure.Cosmos.Table;

namespace AzureFunctionCall
{
    public class Details: TableEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
    }
}

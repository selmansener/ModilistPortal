using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

namespace ModilistPortal.Infrastructure.Azure.Extensions.BlobStorage
{
    public interface IBlobClientFactory
    {
        BlobServiceClient GetClient(string connectionString);
    }

    internal class BlobClientFactory : IBlobClientFactory
    {
        private readonly Dictionary<string, BlobServiceClient> _clients = new Dictionary<string, BlobServiceClient>();

        public BlobServiceClient GetClient(string connectionString)
        {
            if (_clients.ContainsKey(connectionString))
            {
                return _clients[connectionString];
            }

            BlobServiceClient client = new BlobServiceClient(connectionString);

            _clients.Add(connectionString, client);

            return client;
        }
    }
}

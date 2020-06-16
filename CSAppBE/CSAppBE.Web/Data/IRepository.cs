namespace CSAppBE.Web.Data
{
    using CSAppBE.Web.Data.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository
    {
        void AddClient(Client client);
        bool ClientExists(int id);
        Client GetClient(int id);
        IEnumerable<Client> GetClients();
        void RemoveClient(Client client);
        Task<bool> SaveAllAsync();
        void UpdateClient(Client client);
    }
}
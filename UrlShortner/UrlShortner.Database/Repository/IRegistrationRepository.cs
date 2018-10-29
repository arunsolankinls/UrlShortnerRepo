using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database.Repository
{
    public interface IRegistrationRepository
    {
        IEnumerable<Registration> GetAllClients();
        Registration GetClientById(long Id);
        void AddClient(Registration newClient);
        bool SaveAll();        
        Registration ClientAuthentication(string UserName, string Password);
        void Edit(Registration newUrlShotner);
        bool IsEmailExist(string Email);
        Registration GetUserByEmail(string Email);
        Registration GetUserByEmailAndCode(string email, string code);
        void Update(Registration regiter);
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database.Repository
{

    public class RegistrationRepository : IRegistrationRepository
    {
        public ShortnerContext _ctx;

        public RegistrationRepository(ShortnerContext ctx)
        {
            this._ctx = ctx;
        }

        public IEnumerable<Registration> GetAllClients()
        {
            try
            {
                return _ctx.Registration.Where(t=>t.ActiveStatus!=false).OrderBy(z => z.RegistId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Registration ClientAuthentication(string UserName, string Password)
        {
            Registration objRegist = new Registration();
            try
            {
                objRegist = _ctx.Registration.Where(z => z.Email.ToLower() == UserName.ToLower() && z.Password == Password && z.ActiveStatus!=false ).FirstOrDefault();                
            }
            catch (Exception ex)
            {
                return null;
            }
            return objRegist;

        }

        public bool IsEmailExist(string Email)
        {
            Registration objRegist = new Registration();
            try
            {
                objRegist = _ctx.Registration.Where(z => z.Email.ToLower() == Email.ToLower() && z.ActiveStatus != false).FirstOrDefault();

                if (objRegist!=null)
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        public Registration GetUserByEmail(string Email)
        {
            Registration objRegist = new Registration();
            try
            {
                objRegist = _ctx.Registration.Where(z => z.Email.ToLower() == Email.ToLower() && z.ActiveStatus != false).FirstOrDefault();

                if (objRegist != null)
                    return objRegist;
            }
            catch (Exception ex)
            {
                return objRegist;
            }
            return objRegist;
        }

        public Registration GetClientById(long Id)
        {
            try
            {
                return _ctx.Registration
                      .Where(z => z.RegistId == Id && z.ActiveStatus != false).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Registration GetUserByEmailAndCode(string email,string code)
        {
            try
            {
                return _ctx.Registration
                    .Where(x => x.Email.ToLower() == email && x.Code == code && x.PasswordReset == true && x.ActiveStatus == true).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public void AddClient(Registration newClient)
        {
            _ctx.Registration.Add(newClient);
            SaveAll();
        }
        
        public bool SaveAll()
        {
            int Return = _ctx.SaveChanges();
            return Return > 0;
        }

        public void Edit(Registration updatedUrl)
        {
            _ctx.Entry(updatedUrl).State = EntityState.Modified;
            SaveAll();
        }

        public void Update(Registration regiter)
        {
            var registereduser= _ctx.Registration.Find(regiter.RegistId);

            if (registereduser == null)
            {
                return;
            }
            registereduser = regiter;
            _ctx.Registration.Attach(registereduser);
            _ctx.Entry(registereduser).State = EntityState.Modified;
            _ctx.SaveChanges();
        }
    }
}
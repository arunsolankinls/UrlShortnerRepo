using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;
using System.Data.Entity;

namespace UrlShortner.Database.Repository
{
    public class UrlShortnerRepository : IUrlShortnerRepository
    {
        public ShortnerContext _ctx;
         

        public UrlShortnerRepository(ShortnerContext ctx)
        {
            this._ctx = ctx;
        }

        public IEnumerable<Entities.UrlShortner> GetAllUrlsByClient(long UserId)
        {
            
            try
            {
                return _ctx.UrlShortner.Where(z => z.Registration.RegistId == UserId).OrderBy(z => z.UrlId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //short URL list
        public IList<string> GetAllShortUrl(string hosturl)
        {
            try
            {
                //return _ctx.UrlShortner.Select(x=>x.ShortUrlKey.Replace(hosturl, "")).ToList();
                var list = _ctx.UrlShortner.Select(x => x.ShortUrlKey).ToList();
                return list.Select(x => x.Substring(x.LastIndexOf("/")+1)).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        public Entities.UrlShortner GetUrlByShortUrlKey(string hosturl,string key)
        {
            try
            {
                string shorturl = hosturl + key;
                return _ctx.UrlShortner.Where(x=>x.ShortUrlKey==shorturl).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Entities.UrlShortner GetUrlById(long Id)
        {
            try
            {
                return _ctx.UrlShortner
                    .Include(i => i.AdManage)
                    .Include(i => i.Registration)
                      .Where(z => z.UrlId == Id).Single();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Entities.UrlShortner GetUrlById1(long Id)
        {
            try
            {
                return _ctx.UrlShortner
                    .Include(i => i.AdManage)
                    .Include(i => i.Registration)
                      .Where(z => z.UrlId == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void AddUrl(Entities.UrlShortner newUrl)
        {
            _ctx.UrlShortner.Add(newUrl);
        }

        public void Edit(Entities.UrlShortner updatedUrl)
        {
            _ctx.Entry(updatedUrl).State = EntityState.Modified;
            SaveAll();
        }
        public bool SaveAll()
        {
            int Return = _ctx.SaveChanges();
               return Return > 0;
        }

        public int CountAllShortenUrl()
        {
            return _ctx.UrlShortner.Count();
        }

        public bool Delete(long Id)
        {
            bool returnType = false;
            try
            {
                var deletUrl = _ctx.UrlShortner.Find(Id);
                if (deletUrl != null)
                {
                    _ctx.UrlShortner.Remove(deletUrl);
                    returnType = SaveAll();
                    returnType = true;
                }
            }
            catch (Exception)
            {
                returnType = false;
            }
            return returnType;
        }
    }
}

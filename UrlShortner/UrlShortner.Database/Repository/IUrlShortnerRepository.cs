using System.Collections.Generic;

namespace UrlShortner.Database.Repository
{
    public interface IUrlShortnerRepository
    {
        IEnumerable<Entities.UrlShortner> GetAllUrlsByClient(long UserId);
        Entities.UrlShortner GetUrlById(long Id);
        void AddUrl(Entities.UrlShortner newUrlShotner);
        bool SaveAll();
        void Edit(Entities.UrlShortner newUrlShotner);
        Entities.UrlShortner GetUrlById1(long Id);
        bool Delete(long Id);
        IList<string> GetAllShortUrl(string hosturl);
        Entities.UrlShortner GetUrlByShortUrlKey(string hosturl, string key);
        //Return count of all shorten urls.
        int CountAllShortenUrl();
    }
}

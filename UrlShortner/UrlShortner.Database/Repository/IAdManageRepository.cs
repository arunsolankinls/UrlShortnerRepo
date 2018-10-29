using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database.Repository
{
    public interface IAdManageRepository
    {
        IEnumerable<AdManage> GetAllAdsByClient(long UserId);
        AdManage GetAdById(long Id);
        void AddAd(AdManage newAd);
        bool SaveAll();
        void Edit(AdManage newAd);        
        bool Delete(long Id);
        long MemberAdId(long UserId);
        string GetFileName(long AdId);
    }
}

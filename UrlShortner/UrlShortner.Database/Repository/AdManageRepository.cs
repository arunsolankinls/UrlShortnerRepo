using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Database.Repository
{
    public class AdManageRepository : IAdManageRepository
    {
        public ShortnerContext _ctx;

        public AdManageRepository(ShortnerContext ctx)
        {
            this._ctx = ctx;
        }

        public IEnumerable<Entities.AdManage> GetAllAdsByClient(long UserId)
        {
            try
            {
                return _ctx.AdManage.Where(z => z.RegistAdId == UserId).OrderBy(z => z.AdId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Entities.AdManage GetAdById(long Id)
        {
            try
            {
                return _ctx.AdManage
                      .Where(z => z.AdId == Id).Single();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void AddAd(Entities.AdManage newAd)
        {
            _ctx.AdManage.Add(newAd);
        }
        public string GetFileName(long AdId)
        {
            // return "";
            return _ctx.AdManage
                     .Where(z => z.AdId == AdId).Single().UploadFile;
        }


        public void Edit(Entities.AdManage updatedAd)
        {
            _ctx.Entry(updatedAd).State = EntityState.Modified;
            SaveAll();
        }
        public bool SaveAll()
        {
            return _ctx.SaveChanges() > 0;
        }

        public bool Delete(long Id)
        {
            bool returnType = false;
            try
            {
                var deletAd = _ctx.AdManage.Find(Id);
                if (deletAd != null)
                {
                    _ctx.AdManage.Remove(deletAd);
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

        public long MemberAdId(long UserId)
        {
            long MemberType = 0;
            //MemberType = _ctx.AdManage.Where(z => z.RegistAdId == UserId).Single().AdId;
            MemberType = _ctx.AdManage.Where(z => z.RegistAdId == UserId).FirstOrDefault().AdId;
            return MemberType;
        }
    }
}

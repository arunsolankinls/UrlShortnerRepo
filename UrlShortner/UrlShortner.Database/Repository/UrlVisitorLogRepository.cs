using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortner.Database.Entities;
using System.Data.Entity;

namespace UrlShortner.Database.Repository
{
    public class UrlVisitorLogRepository : IUrlVisitorLogRepository
    {
        public ShortnerContext _ctx;
         

        public UrlVisitorLogRepository(ShortnerContext ctx)
        {
            this._ctx = ctx;
        }

        public IEnumerable<Entities.UrlVisitorLog> GetAllUrlVisitorLog(long UserId)
        {
            try
            {
                //return _ctx.UrlVisitorLog.Where(z => z.RegistId== UserId).OrderBy(z => z.UrlVisitorLogId).ToList();
                return _ctx.UrlVisitorLog.Where(z => z.RegistId == UserId).ToList();

                 //IEnumerable<Entities.UrlVisitorLog> list = _ctx.UrlVisitorLog.Where(x => x.RegistId == UserId)
                 //                          .GroupBy(x => new { x.ShortUrl, x.VisitedDate, x.OriginalUrl }).ToList();
                         
                 //                          //.Select(x=>new {Value=x.Count(),x.ur }).ToList();
                //return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public IEnumerable<Entities.UrlVisitorLog> GetAllUrlVisitorLogById(long UserId)
        //{
        //    try
        //    {
        //        var data= _ctx.UrlVisitorLog
        //            .Where(z => z.RegistId == UserId)
        //            .GroupBy(x=> new {x.VisitedDate})
        //            .Select(x => new { x.ShortUrl, x.VisitedDate});
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public void AddUrl(Entities.UrlVisitorLog urlvisitor)
        {
            _ctx.UrlVisitorLog.Add(urlvisitor);
        }
    
        public bool SaveAll()
        {
            int Return = _ctx.SaveChanges();
               return Return > 0;
        }

        public IList<UrlVisitorLog> ListByStartDateEndDate(DateTime startdate,DateTime enddate,long registid)
        {
            var visitorlist = _ctx.UrlVisitorLog.Where(x => x.VisitedDate >= enddate && x.VisitedDate <= startdate && x.RegistId== registid).ToList();

            return visitorlist;
        }
    }
}

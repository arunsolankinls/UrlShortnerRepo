using System;
using System.Collections.Generic;
using UrlShortner.Database.Entities;

namespace UrlShortner.Database.Repository
{
    public interface IUrlVisitorLogRepository
    {
        IEnumerable<UrlVisitorLog> GetAllUrlVisitorLog(long UserId);

       // IEnumerable<UrlVisitorLog> GetAllUrlVisitorLogById(long UserId);

        void AddUrl(UrlVisitorLog urlvisitor);
        bool SaveAll();
        IList<UrlVisitorLog> ListByStartDateEndDate(DateTime startdate, DateTime enddate, long registid);
    }
}

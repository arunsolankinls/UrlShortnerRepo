using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UrlShortner.Comman;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortner.Models;

namespace UrlShortner.Controllers
{
    [UserAuthenticationFilter]
    public class ClientController : BaseController
    {
        private readonly IRegistrationRepository _RegistrationRepository;
        // GET: Client

        public ClientController(IRegistrationRepository RegistrationRepository)
        {
            this._RegistrationRepository = RegistrationRepository;
        }
        public ActionResult Index()
        {
            var objRegistraion = _RegistrationRepository.GetAllClients();
            return View(Mapper.Map<IEnumerable<Registration>, IEnumerable<RegistrationViewModel>>(objRegistraion));
        }

        public ActionResult ProfileView()
        {
            var objRegistraion = _RegistrationRepository.GetClientById(Convert.ToInt64(Session["UserId"]));
            TempData["UserId"] = objRegistraion.RegistId;
            TempData.Keep();

            return View(Mapper.Map<Registration, RegistrationViewModel>(objRegistraion));
        }

        [HttpPost, ActionName("ProfileView")]
        public ActionResult ProfileView(RegistrationViewModel model)
        {
            if (model == null)
                return View(model);

            long UrlId = (long)TempData["UserId"];
            Registration objRegisterUser = _RegistrationRepository.GetClientById(UrlId);
            objRegisterUser.UpdatedDate = DateTime.Now.Date;
            objRegisterUser.UpdatedBy = Convert.ToInt64(Session["UserId"]);
            objRegisterUser.City = model.City;
            objRegisterUser.Country = model.Country;
            objRegisterUser.Email = model.Email;
            objRegisterUser.FirstName = model.FirstName;
            objRegisterUser.LastName = model.LastName;
            objRegisterUser.Phone = model.Phone;
            objRegisterUser.State = model.State;
            objRegisterUser.Zipcode = model.Zipcode;
            _RegistrationRepository.Edit(objRegisterUser);

            return RedirectToAction("ProfileView", "Client");
        }

        public ActionResult Edit(int id)
        {
            //var objRegistraion = _RegistrationRepository.GetClientById(Convert.ToInt64(Session["UserId"]));

            var objRegistraion = _RegistrationRepository.GetClientById(id);
            TempData["ClientId"] = objRegistraion.RegistId;
            TempData["UserId"] = Session["UserId"];


            TempData.Keep();
            return View(Mapper.Map<Registration, RegistrationViewModel>(objRegistraion));
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPost(RegistrationViewModel model)
        {
            try
            {
                long clientid = (long)TempData["ClientId"];
                long adminid = (long)TempData["UserId"];
                if (clientid <= 0 && adminid <= 0)
                {

                    return View(model);
                }
                else
                {
                    //long UrlId = (long)TempData["UserId"];
                    Registration objRegisterUser = _RegistrationRepository.GetClientById(clientid);
                    objRegisterUser.UpdatedDate = DateTime.Now.Date;
                    objRegisterUser.UpdatedBy = adminid;
                    objRegisterUser.ActiveStatus = model.ActiveStatus;
                    objRegisterUser.Address = model.Address;
                    objRegisterUser.City = model.City;
                    objRegisterUser.Country = model.Country;
                    objRegisterUser.Email = model.Email;
                    //objRegisterUser.ExpireDate = model.ExpireDate;
                    objRegisterUser.FirstName = model.FirstName;
                    objRegisterUser.LastName = model.LastName;
                    objRegisterUser.Phone = model.Phone;
                    objRegisterUser.State = model.State;
                    objRegisterUser.Zipcode = model.Zipcode;

                    _RegistrationRepository.Edit(objRegisterUser);
                    return RedirectToAction("Index", "Client");
                }
            }
            catch (Exception ex)
            {
            }
            return View();

        }
    }
}
using AutoMapper;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortner.Models;

namespace UrlShortnerClient
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            container.RegisterType<IRegistrationRepository, RegistrationRepository>();
            container.RegisterType<IUrlShortnerHistoryRepository, UrlShortnerHistoryRepository>();
            container.RegisterType<IUrlShortnerRepository, UrlShortnerRepository>();
            container.RegisterType<IAdManageRepository, AdManageRepository>();
            container.RegisterType<IUrlVisitorLogRepository, UrlVisitorLogRepository>();
            container.RegisterType<IPaymentRepository, PaymentRepository>();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Registration, RegistrationViewModel>();
                cfg.CreateMap<RegistrationViewModel, Registration>();
                cfg.CreateMap<UrlShortner.Database.Entities.UrlShortner, UrlShortnerViewModel>();
                cfg.CreateMap<AdManage, AdManageViewModel>();
                cfg.CreateMap<UrlVisitorLog, UrlVisitorLogViewModel>();
                cfg.CreateMap<PaymentTransaction, PaymentTransactionViewModel>();
            });
        }
    }
}
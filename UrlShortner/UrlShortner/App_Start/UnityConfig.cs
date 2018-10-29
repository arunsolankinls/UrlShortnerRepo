using AutoMapper;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using UrlShortner.Database.Entities;
using UrlShortner.Database.Repository;
using UrlShortner.Models;
namespace UrlShortner
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

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IUrlShortnerHistoryRepository, UrlShortnerHistoryRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IUrlShortnerRepository, UrlShortnerRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IAdManageRepository, AdManageRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IUrlVisitorLogRepository, UrlVisitorLogRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            container.RegisterType<IPaymentRepository, PaymentRepository>();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Registration, RegistrationViewModel>();
                cfg.CreateMap<Database.Entities.UrlShortner, UrlShortnerViewModel>();
                cfg.CreateMap<AdManage, AdManageViewModel>();
                cfg.CreateMap<UrlVisitorLog, UrlVisitorLogViewModel>();
                cfg.CreateMap<PaymentTransaction, PaymentTransactionViewModel>();
            });
        }
    }
}
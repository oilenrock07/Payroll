using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
using Payroll.Service.Caching;
using Payroll.Service.Implementations;
using Payroll.Service.Interfaces;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Payroll.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Payroll.App_Start.NinjectWebCommon), "Stop")]

namespace Payroll.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            //Infrastructure
            kernel.Bind<IDatabaseFactory>().To<DatabaseFactory>().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>));

            //Repository
            kernel.Bind<IAttendanceRepository>().To<AttendanceRepository>().InRequestScope();
            kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>().InRequestScope();
            kernel.Bind<ISettingRepository>().To<SettingRepository>().InRequestScope();
            kernel.Bind<IEmployeeInfoRepository>().To<EmployeeInfoRepository>().InRequestScope();
            kernel.Bind<IPositionRepository>().To<PositionRepository>().InRequestScope();
            kernel.Bind<IPaymentFrequencyRepository>().To<PaymentFrequencyRepository>().InRequestScope();
            kernel.Bind<IDepartmentRepository>().To<DepartmentRepository>().InRequestScope();
            kernel.Bind<IEmployeeDepartmentRepository>().To<EmployeeDepartmentRepository>().InRequestScope();
            kernel.Bind<IAttendanceLogRepository>().To<AttendanceLogRepository>().InRequestScope();
            kernel.Bind<IHolidayRepository>().To<HolidayRepository>().InRequestScope();
            kernel.Bind<ILeaveRepository>().To<LeaveRepository>().InRequestScope();
            kernel.Bind<ILoanRepository>().To<LoanRepository>().InRequestScope();
            kernel.Bind<IEmployeeLoanRepository>().To<EmployeeLoanRepository>().InRequestScope();
            kernel.Bind<IMachineRepository>().To<MachineRepository>().InRequestScope();
            kernel.Bind<IEmployeeInfoHistoryRepository>().To<EmployeeInfoHistoryRepository>().InRequestScope();
            kernel.Bind<IEmployeeLeaveRepository>().To<EmployeeLeaveRepository>().InRequestScope();
            kernel.Bind<IRoleRepository>().To<RoleRepository>().InRequestScope();
            kernel.Bind<IUserRoleRepository>().To<UserRoleRepository>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
            kernel.Bind<IEmployeeMachineRepository>().To<EmployeeMachineRepository>().InRequestScope();
             
            //Service
            kernel.Bind<IUserRoleService>().To<UserRoleService>().InRequestScope();
            kernel.Bind<IWebService>().To<WebService>().InRequestScope();
            kernel.Bind<IEmployeeMachineService>().To<EmployeeMachineService>().InRequestScope();
            //Caching
            //kernel.Bind<ISettingRepository>().To<CachedSettingService>().InRequestScope();
        }        
    }
}

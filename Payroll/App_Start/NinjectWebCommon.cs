using Payroll.Infrastructure.Implementations;
using Payroll.Infrastructure.Interfaces;
using Payroll.Repository.Interface;
using Payroll.Repository.Repositories;
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
    using CacheManager.Core;

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
            kernel.Bind(typeof(ICacheManager<>)).ToMethod((context) =>
            {
                return CacheManager.Core.CacheFactory.Build<object>(p => p.WithSystemRuntimeCacheHandle());
            }).InSingletonScope();

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
            kernel.Bind<IEmployeePayrollRepository>().To<EmployeePayrollRepository>().InRequestScope();
            kernel.Bind<ITotalEmployeeHoursRepository>().To<TotalEmployeeHoursRepository>().InRequestScope();
            kernel.Bind<IEmployeeHoursRepository>().To<EmployeeHoursRepository>().InRequestScope();
            kernel.Bind<IEmployeeWorkScheduleRepository>().To<EmployeeWorkScheduleRepository>().InRequestScope();
            kernel.Bind<IEmployeeDailyPayrollRepository>().To<EmployeeDailyPayrollRepository>().InRequestScope();
            kernel.Bind<IEmployeeDeductionRepository>().To<EmployeeDeductionRepository>().InRequestScope();
            kernel.Bind<IDeductionRepository>().To<DeductionRepository>().InRequestScope();
            kernel.Bind<IEmployeePayrollDeductionRepository>().To<EmployeePayrollDeductionRepository>().InRequestScope();
            kernel.Bind<ITaxRepository>().To<TaxRepository>().InRequestScope();
            kernel.Bind<IWorkScheduleRepository>().To<WorkScheduleRepository>().InRequestScope();
            kernel.Bind<ISchedulerLogRepository>().To<SchedulerLogRepository>().InRequestScope();
            kernel.Bind<IAdjustmentRepository>().To<AdjustmentRepository>().InRequestScope();
            kernel.Bind<IEmployeeAdjustmentRepository>().To<EmployeeAdjustmentRepository>().InRequestScope();
            kernel.Bind<IEmployeePayrollItemRepository>().To<EmployeePayrollItemRepository>().InRequestScope();
            
            //Service
            kernel.Bind<IUserRoleService>().To<UserRoleService>().InRequestScope();
            kernel.Bind<IWebService>().To<WebService>().InRequestScope();
            kernel.Bind<IEmployeeService>().To<EmployeeService>().InRequestScope();
            kernel.Bind<IEmployeeMachineService>().To<EmployeeMachineService>().InRequestScope();
            kernel.Bind<IEmployeePayrollService>().To<EmployeePayrollService>().InRequestScope();
            kernel.Bind<IEmployeeDailyPayrollService>().To<EmployeeDailyPayrollService>().InRequestScope();
            kernel.Bind<IEmployeePayrollDeductionService>().To<EmployeePayrollDeductionService>().InRequestScope();
            kernel.Bind<ISettingService>().To<SettingService>().InRequestScope();
            kernel.Bind<IEmployeeInfoService>().To<EmployeeInfoService>().InRequestScope();
            kernel.Bind<ITotalEmployeeHoursService>().To<TotalEmployeeHoursService>().InRequestScope();
            kernel.Bind<IEmployeeHoursService>().To<EmployeeHoursService>().InRequestScope();
            kernel.Bind<IEmployeeWorkScheduleService>().To<EmployeeWorkScheduleService>().InRequestScope();
            kernel.Bind<IAttendanceService>().To<AttendanceService>().InRequestScope();
            kernel.Bind<IAttendanceLogService>().To<AttendanceLogService>().InRequestScope();
            kernel.Bind<IHolidayService>().To<HolidayService>().InRequestScope();
            kernel.Bind<IEmployeeSalaryService>().To<EmployeeSalaryService>().InRequestScope();
            kernel.Bind<IEmployeeDeductionService>().To<EmployeeDeductionService>().InRequestScope();
            kernel.Bind<IDeductionService>().To<DeductionService>().InRequestScope();
            kernel.Bind<ITaxService>().To<TaxService>().InRequestScope();
            kernel.Bind<ISchedulerLogService>().To<SchedulerLogService>().InRequestScope();
            kernel.Bind<IEmployeeAdjustmentService>().To<EmployeeAdjustmentService>().InRequestScope();
            kernel.Bind<IEmployeePayrollItemService>().To<EmployeePayrollItemService>().InRequestScope();
            kernel.Bind<IEmployeePayrollAllowanceService>().To<EmployeePayrollAllowanceService>().InRequestScope();

            //Caching
            //kernel.Bind<ISettingRepository>().To<CachedSettingService>().InRequestScope();
        }
    }
}

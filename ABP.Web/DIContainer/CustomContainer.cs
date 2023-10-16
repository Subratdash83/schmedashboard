using ABP.Infrastructure;
using ABP.Repository.AdminConsole;
using ABP.Repository.BDOData;
using ABP.Repository.Block;
using ABP.Repository.BlockData;
using ABP.Repository.CollectorData;
using ABP.Repository.CollectorDataIndicator;
using ABP.Repository.Common;
using ABP.Repository.Contract.Block;
using ABP.Repository.Contract.Common;
using ABP.Repository.Contract.Contract.AdminConsole;
using ABP.Repository.Contract.Contract.BDOData;
using ABP.Repository.Contract.Contract.BlockData;
using ABP.Repository.Contract.Contract.CollectorData;
using ABP.Repository.Contract.Contract.CollectorIndicatorData;
using ABP.Repository.Contract.Contract.Dashboard;
using ABP.Repository.Contract.Contract.DataPoint;
using ABP.Repository.Contract.Contract.Department;
using ABP.Repository.Contract.Contract.District;
using ABP.Repository.Contract.Contract.DistrictData;
using ABP.Repository.Contract.Contract.Indicator;
using ABP.Repository.Contract.Contract.Login;
using ABP.Repository.Contract.Contract.Panel;
using ABP.Repository.Contract.Contract.Sector;
using ABP.Repository.Contract.Contract.SMS;
using ABP.Repository.Contract.Contract.SMSTemplate;
using ABP.Repository.Contract.DistDetailReport;
using ABP.Repository.Contract.Factory;
using ABP.Repository.Contract.Report;
using ABP.Repository.ControlPanel;
using ABP.Repository.Dashboard;
using ABP.Repository.DataPoint;
using ABP.Repository.Department;
using ABP.Repository.DistDetailReport;
using ABP.Repository.District;
using ABP.Repository.DistrictData;
using ABP.Repository.Factory;
using ABP.Repository.Indicator;
using ABP.Repository.Login;
using ABP.Repository.Panel;
using ABP.Repository.Report;
using ABP.Repository.Sector;
using ABP.Repository.SMS;
using ABP.Repository.SMSTemplate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABP.Web.DIContainer
{
    public static class CustomContainer
    {
        public static void AddCustomContainer(this IServiceCollection services, IConfiguration configuration)
        {
            IConnectionFactory connectionFactory = new ConnectionFactory(configuration.GetConnectionString("ABPConnectionString"));
            IConnectionFactory adminconnectionFactory = new ConnectionFactory(configuration.GetConnectionString("AdminConsoleConnectionString"));
            services.AddSingleton(connectionFactory);
            services.AddSingleton<IAdminConsoleRepository>(_ => new AdminConsoleRepository(adminconnectionFactory));
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<ICommonRepository, CommonRepository>();

            services.AddSingleton<ILoginRepository, LoginRepository>();
            services.AddSingleton<ISectorRepository, SectorRepository>();
            services.AddSingleton<IDistrictRepository, DistrictRepository>();
            services.AddSingleton<IBlockRepository, BlockRepository>();
            services.AddSingleton<IIndicatorRepository, IndicatorRepository>();
            services.AddSingleton<IDepartmentRepository, DepartmentRepository>();
            services.AddSingleton<IDataPointRepository, DataPointRepostitory>();
            services.AddSingleton<IBDODataRepository, BDODataRepository>();
            services.AddSingleton<ICollectorDataRepository, CollectorDataRepository>();
            services.AddSingleton<ICollectorDataIndicatorRepository, CollectorDataIndicatorRepository>();
            services.AddSingleton<IDashboardRepository, DashboardRepository>();
            services.AddSingleton<ISendSMSRepository, SendSMSRepository>();
            services.AddSingleton<ISMSRepository, SMSRepository>();
            services.AddSingleton<IPanelRepository, PanelRepository>();
            services.AddSingleton<IControlPanelRepository, ControlPanelRepository>();
            services.AddSingleton<IBlockDataRepository, BlockDataRepository>();
            services.AddSingleton<IDistrictDataRepository, DistrictDataRepository>();
            services.AddSingleton<ISMSTemplateRepository, SMSTemplateRepository>();
            services.AddSingleton<IDistDetailRepository, DistDetailRepository>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Adm.Boot.Application;
using Adm.Boot.Data.EntityFrameworkCore;
using Adm.Boot.Domain.IRepositories;
using Adm.Boot.Infrastructure;
using Adm.Boot.Infrastructure.Interceptors;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Adm.Boot.Infrastructure.Extensions;
using Adm.Boot.Api.StartupExtensions;
using Newtonsoft.Json;
using Adm.Boot.Api.Filters;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using AutoMapper;

namespace Adm.Boot.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            //��������ļ����ݻ����������ֿ��ˣ���������д
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";
            Configuration = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
            //ReloadOnChange = true ��appsettings.json���޸�ʱ���¼���
            .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
            .Build();
            AdmApp.Configuration = Configuration;
        }

        /// <summary>
        /// AutoFac����
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //ע��������
            // builder.RegisterType<TransactionInterceptor>().AsSelf();
            // builder.RegisterType<TransactionAsyncInterceptor>().AsSelf();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            //builder.RegisterType<AdminSession>().As<IAdminSession>();
            try
            {
                //Adm.Boot.Application�Ǽ̳нӿڵ�ʵ�ַ����������
                var assemblys = Assembly.Load("Adm.Boot.Application");
                //ITransientDependency ��һ���ӿڣ�����Application��Ҫʵ������ע��Ľ�ڶ�Ҫ�̳иýӿڣ�
                var baseType = typeof(ITransientDependency);
                builder.RegisterAssemblyTypes(assemblys)
                    .Where(m => baseType.IsAssignableFrom(m) && m != baseType && !m.IsAbstract)
                .AsImplementedInterfaces()
                .PropertiesAutowired()                      //֧������ע��
                .EnableInterfaceInterceptors()               //���ýӿ�����
                .InterceptedBy(typeof(TransactionInterceptor));

                var basePath = AppContext.BaseDirectory;
                var repositoryDllFile = Path.Combine(basePath, "Adm.Boot.Data.dll");
                var assemblysRepository = Assembly.LoadFrom(repositoryDllFile);
                builder.RegisterAssemblyTypes(assemblysRepository)
                    .AsImplementedInterfaces();

                builder.RegisterGeneric(typeof(AdmRepositoryBase<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            }
            catch (Exception ex)
            {
                ("Adm.Boot.Data.dll ��ʧ�����ȱ��������С�\n" + ex.Message).WriteErrorLine();
                throw;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerSetup();
            services.AddAutoMapper(Assembly.Load("Adm.Boot.Application"));
            services.AddApiVersioning(option => option.ReportApiVersions = true);
            services.AddControllers(o =>
            {
                o.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            AdmApp.ServiceProvider = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //��������ע�������м��˳���������

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                //c.IndexStream = () => Assembly.GetExecutingAssembly()
                //   .GetManifestResourceStream("Adm.Boot.Api.wwwroot.swagger.index.html");
                c.RoutePrefix = "";//����Ϊ�գ�launchSettings.json��launchUrlȥ��,localhost:8081 ���� localhost:8001/swagger               
            });
            #endregion
        }
    }
}
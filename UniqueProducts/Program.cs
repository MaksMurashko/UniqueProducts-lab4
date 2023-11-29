using UniqueProducts.Data;
using UniqueProducts.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace UniqueProducts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IServiceCollection services = builder.Services;

            string connection = builder.Configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<UniqueProductsContext>(options => options.UseSqlServer(connection));
           
            services.AddDistributedMemoryCache();
            services.AddSession();

            builder.Services.AddControllersWithViews(options => {
                options.CacheProfiles.Add("UniqueProductsCache",
                    new CacheProfile()
                    {
                        Location = ResponseCacheLocation.Any,
                        Duration = 2 * 23 + 240
                    });
            });

            //������������� MVC
            services.AddControllersWithViews();
            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // ��������� ��������� ����������� ������
            app.UseStaticFiles();

            // ��������� ��������� ������
            app.UseSession();

            // ��������� ��������� middleware �� ������������� ���� ������ � ���������� ������������� ����
            app.UseDbInitializer();

            //�������������
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.Run();

        }

    }
}
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        // Configure the application to use the protocol and client ip address forwared by the frontend load balancer
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default. Clear that restriction to enable this explicit configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Configure the application to client certificate forwarded the frontend load balancer
        services.AddCertificateForwarding(options => { options.CertificateHeader = "X-ARR-ClientCert"; });

        // Add certificate authentication so when authorization is performed the user will be created from the certificate
        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(options =>
        {
            options.AllowedCertificateTypes = CertificateTypes.All;
            options.ValidateCertificateUse = false;
            options.ValidateValidityPeriod = false;
            options.RevocationFlag = X509RevocationFlag.EndCertificateOnly;
            options.RevocationMode = X509RevocationMode.NoCheck;
        }

        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseForwardedHeaders();
        app.UseCertificateForwarding();
        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
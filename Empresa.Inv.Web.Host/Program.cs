using Empresa.Inv.Application.Shared.Entities;
using Empresa.Inv.Application;
using Empresa.Inv.Application.Shared.Entities.Dto;
using Empresa.Inv.EntityFrameworkCore;
using Empresa.Inv.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using Empresa.Inv.EntityFrameworkCore.EntityFrameworkCore;
using Empresa.Inv.Web.Host.Services;
using Empresa.Inv.Web.Host.Authorization;
using Empresa.Inv.Web.Host.Services.General;
using Empresa.Inv.Infraestructure;
using AspNetCoreRateLimit;
using Empresa.Inv.Web.Host.General;

namespace Empresa.Inv.Web.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region Configuraci�n CORS Parte 1


            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            #endregion

            #region Configuraci�n JWT Parte 1

            // Configuraci�n JWT
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettingsFile>();

            if (string.IsNullOrEmpty(jwtSettings.PrivateKeyPath) || string.IsNullOrEmpty(jwtSettings.PublicKeyPath))
            {
                throw new InvalidOperationException("Las rutas de las claves p�blicas o privadas no est�n configuradas.");
            }



            // Registro para compartir la configuracion leida del appsetting
            builder.Services.Configure<JwtSettingsFile>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.AddSingleton(jwtSettings);

            //Registro JwtServices
            builder.Services.AddScoped<JwtTokenService>();
            #endregion

            #region Configuraci�n Autorizacion personalizada
            //Registro de autorizacion personalizada
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("PermissionPolicy", policy =>
                {
                    policy.Requirements.Add(new PermissionRequirement("")); // Esto puede estar vac�o, ya que se establecer� en el atributo
                });
            });
            #endregion

            #region Configuraci�n JWT Parte 2


            // Cargar las claves desde archivos
            var privateKeyContent = ReadPemFile(jwtSettings.PrivateKeyPath);
            var publicKeyContent = ReadPemFile(jwtSettings.PublicKeyPath);

            // autenticaci�n JWT

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyContent.ToCharArray());
            var rsaSecurityKey = new RsaSecurityKey(rsa);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = rsaSecurityKey,
                    ClockSkew = TimeSpan.Zero // Opcional: Elimina el margen de 5 minutos en la expiraci�n de tokens

                };
            });

            #endregion

            #region Configuraci�n SERILOG parte 1

            Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(builder.Configuration)
           .CreateLogger();

            // Agrega un registro de prueba
            Log.Information("Aplicaci�n iniciada.");

            #endregion


            #region Configuraci�n EMAIL

            // Cargar configuraci�n de appsettings.json
            var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

            // Registrar EmailSettings
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            // Registrar TwoFactorSettings en el contenedor de servicios
            var twoFactorSettings = builder.Configuration.GetSection("TwoFactorAuthentication").Get<TwoFactorSettings>();

            builder.Services.Configure<TwoFactorSettings>(builder.Configuration.GetSection("TwoFactorAuthentication"));



            #endregion


            #region Configuraci�n RATE LIMITING - setting   parte 1

            // Agregar las configuraciones de rate limiting
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

            builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

            builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

            // Agregar los stores necesarios
            builder.Services.AddInMemoryRateLimiting();


            #endregion

            try
            {

                // Usa Serilog como el logger
                builder.Host.UseSerilog();

                builder.Services.AddControllers();


                #region Configuraci�n Contexto EF

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                       .AddInterceptors(new CustomDbCommandInterceptor())
                       .AddInterceptors(new PerformanceInterceptor()
                    ));

                #endregion

                #region Registro de Interfaces E Implementacion (para la ID)

                //Registrar repositorios
                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IProductCustomRepository, ProductCustomRepository>();

                //Envio de correo
                builder.Services.AddScoped<IEmailSender, EmailSender>();


                //Registrar servicios de negocio
                builder.Services.AddScoped<IInvAppService, InvAppService>();

                // Registrar los servicios necesarios para AspNetCoreRateLimit
                builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
                builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();



                // Registro de UnitOfWork
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


                #endregion

                #region Configuraci�n SWAGGER parte 1


                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtExample API", Version = "v1" });
                });

                #endregion

                #region Configuraci�n Automapper

                builder.Services.AddAutoMapper(typeof(MappingProfile));

                #endregion

                #region Configuraci�n del servicio de cach�
                // Configura el servicio de caching

                builder.Services.AddOptions();     // complementario a Rate Limiting
                builder.Services.AddMemoryCache(); // Configura el servicio de caching
                builder.Services.AddSingleton<CacheService>(); // Registra el servicio de cache


                #endregion

                var app = builder.Build();

                #region Configuracion RateLimiting parte 2
                //app.UseIpRateLimiting();
                app.UseMiddleware<ClientIdValidationMiddleware>();
                // Usar el middleware de ClientRateLimiting
                app.UseClientRateLimiting();

                #endregion


                #region Configuraci�n del manejo centralizado de errores
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                #endregion


                #region Configuraci�n SWAGGER parte 2

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JwtExample API v1");
                    });
                }


                #endregion

                //Utilizaci�n de routing - A�adido
                app.UseRouting();


                #region Configuraci�n CORS parte 2
                //Implementaci�n politica CORS   Todas las pol�ticas - A�adido
                app.UseCors("AllowSpecificOrigin"); // Usa la pol�tica de CORS definida
                #endregion

                // Middleware de autenticaci�n y autorizaci�n - A�adido
                app.UseAuthentication();


                // Configure the HTTP request pipeline.

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {

                Log.Fatal(ex, "La aplicaci�n fall� al iniciar.");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }

        private static string ReadPemFile(string path)
        {
            return File.ReadAllText(path);
        }

    }

}

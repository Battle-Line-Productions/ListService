namespace ListService.Api
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json.Serialization;
    using ApiModels;
    using AutoMapper;
    using AutoMapper.Configuration;
    using DataAccess;
    using Filters;
    using FluentValidation.AspNetCore;
    using Helpers;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Middleware;
    using RockLib.Logging;
    using RockLib.Logging.DependencyInjection;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using Swashbuckle.AspNetCore.Filters;
    using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly Container _container = new();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogger();

            services.AddCors(x =>
            {
                x.AddPolicy("CorsPolicy", y =>
                {
                    y.AllowAnyOrigin();
                    y.AllowAnyHeader();
                    y.AllowAnyMethod();
                });
            });

            services.AddHttpContextAccessor();
            services.AddMvc(x =>
            {
                x.EnableEndpointRouting = false;
                x.Filters.Add<ValidationFilter>();
            }).AddFluentValidation(x =>
            {
                x.RegisterValidatorsFromAssemblyContaining<Startup>();
            }).AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddControllers();
            
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo {Title = "ListService.Api", Version = "v1"});
                x.ExampleFilters();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(x =>
            {
                x.GroupNameFormat = "'v'VVV";
                x.SubstituteApiVersionInUrl = true;
            });

            var connectionString = Configuration["ConfigSettings:ListServiceDataContext"] ??
                                   Configuration["ListServiceDataContext"];

            services.AddDbContext<ListServiceDataContext>(x =>
            {
                x.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });

            services.AddHealthChecks().AddDbContextCheck<ListServiceDataContext>();

            _container.Options.DefaultLifestyle = Lifestyle.Scoped;
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSimpleInjector(_container, x =>
            {
                x.AddAspNetCore().AddControllerActivation();
                x.AddLogging();
            });

            ConfigureModelBindingExceptionHandling(services);
            InitializeContainer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(_container);
            app.UseSwagger();
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "List Service Api v1"));
            }
            else
            {
                app.UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "List Service Api v1");
                    x.SupportedSubmitMethods();
                });

                app.UseExceptionHandler("/error");
            }

            app.UseMiddleware<CorrelationTokenMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>(_container);

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(x =>
            {
                x.MapControllers();
            });

            app.UseHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        HealthChecks = report.Entries.Select(y => new HealthCheck
                        {
                            Component = y.Key,
                            Status = y.Value.Status.ToString(),
                            Description = y.Value.Description,
                            Data = y.Value.Data.Select(z => z.Value.ToString()),
                            Exception = y.Value.Exception?.Message
                        })
                    };

                    await context.Response.WriteAsJsonAsync(response);
                }
            });

            _container.Verify();
        }

        private void InitializeContainer()
        {
            _container.RegisterSingleton<AutoMapperConfig>();
            _container.RegisterSingleton(() => GetMapper(_container));

            _container.Register<IPagedResponseBuilder, PagedResponseBuilder>();
        }

        private static IMapper GetMapper(Container container)
        {
            var mapperConfigurationExpression = new MapperConfigurationExpression();
            mapperConfigurationExpression.ConstructServicesUsing(container.GetInstance);

            mapperConfigurationExpression.AddProfile(container.GetInstance<AutoMapperConfig>());

            var mapperConfig = new MapperConfiguration(mapperConfigurationExpression);
            mapperConfig.AssertConfigurationIsValid();

            IMapper mapper = new Mapper(mapperConfig, container.GetInstance);
            return mapper;
        }

        private void ConfigureModelBindingExceptionHandling(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(x =>
            {
                x.InvalidModelStateResponseFactory = actionContext =>
                {
                    var error = actionContext.ModelState.Where(e => e.Value.Errors.Count > 0)
                        .Select(ee => new ValidationProblemDetails(actionContext.ModelState)).FirstOrDefault();

                    var logger = _container.GetInstance<ILogger>();

                    logger.Error($"{string.Join("\n", error.Errors.Values.SelectMany(x => x))}");

                    return new BadRequestObjectResult(error);
                };
            });
        }
    }
}

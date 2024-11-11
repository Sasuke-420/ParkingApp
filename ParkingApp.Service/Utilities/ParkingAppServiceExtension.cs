using Lisec.Base.Configurations;
using Lisec.Base.Utilities;
using Lisec.ParkingApp.Repositories;
using Lisec.ParkingApp.Services;
using Lisec.ServiceBase.HealthCheck;
using Lisec.ServiceBase.OpenApi;
using Lisec.ServiceBase.Statistic.Controllers;
using Lisec.ServiceBase.Utilities;
using Lisec.ServiceBase.Version.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Collections.Generic;
using System.Linq;

namespace Lisec.ParkingApp.Utilities
{
    /// <summary>
    /// ParkingAppServiceExtension
    /// </summary>
    public static class ParkingAppServiceExtension
    {

        /// <summary>
        /// To add parking card management app extension.
        /// </summary>
        /// <param name="services">Specify IServiceCollection.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddParkingAppExtensions(this IServiceCollection services, string serviceName, string openApiDescription)
        {
            AddParkingAppDependency(services);
            AddParkingAppDatabaseContext(services);
            AddOpenApiDocumentation(services, serviceName, openApiDescription);
            return services;
        }

        private static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, string _serviceName, string openApiDescription)
        {
            OpenApiSettings openApiSettings = new OpenApiSettings
            {
                ServiceName = new SnakeCaseNamingStrategy().GetPropertyName(_serviceName, false),
                VersionDetails = new List<OpenApiVersionDetail> {
                    new OpenApiVersionDetail { ApiDescription = openApiDescription, MajorVersion = 1, MinorVersion = 0 },
                }
            };
            IEnumerable<ApiVersion> apiVersions = GetApiVersions(openApiSettings.VersionDetails);
            services.AddApiVersioning(delegate (ApiVersioningOptions config)
            {
                config.Conventions.Controller<VersionController>().HasApiVersions(apiVersions);
                config.Conventions.Controller<StatisticsController>().HasApiVersions(apiVersions);
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new QueryStringApiVersionReader("apiVersion"));
            });
            services.AddVersionedApiExplorer(delegate (ApiExplorerOptions options)
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            string serviceName = openApiSettings.ServiceName;
            if (serviceName == null)
            {
                serviceName = "swagger";
            }

            foreach (OpenApiVersionDetail openApiDetail in openApiSettings.VersionDetails)
            {
                services.AddOpenApiDocument(delegate (AspNetCoreOpenApiDocumentGeneratorSettings configure)
                {
                    configure.Title = _serviceName;
                    configure.Description = openApiDetail.ApiDescription;
                    string versionStr = "v" + openApiDetail.MajorVersion;
                    if (openApiDetail.MinorVersion > 0)
                    {
                        versionStr = versionStr + "." + openApiDetail.MinorVersion;
                    }

                    configure.DocumentName = serviceName + "_" + versionStr;
                    configure.ApiGroupNames = new string[1] { versionStr };
                    configure.Version = versionStr;
                    configure.AllowReferencesWithProperties = true;
                    configure.FlattenInheritanceHierarchy = true;
                    configure.PostProcess = delegate (OpenApiDocument document)
                    {
                        string text = "/" + versionStr;
                        KeyValuePair<string, OpenApiPathItem>[] array = document.Paths.ToArray();
                        for (int i = 0; i < array.Length; i++)
                        {
                            KeyValuePair<string, OpenApiPathItem> keyValuePair = array[i];
                            if (keyValuePair.Key.StartsWith(text))
                            {
                                document.Paths.Remove(keyValuePair.Key);
                                IDictionary<string, OpenApiPathItem> paths = document.Paths;
                                string key = keyValuePair.Key;
                                int length = text.Length;
                                paths[key.Substring(length, key.Length - length)] = keyValuePair.Value;
                            }
                        }
                    };
                    AddOpenApiSecurity(configure);
                });
            }
            return services;
        }

        /// <summary>
        /// Add security to open API document
        /// </summary>
        /// <param name="configure">Specify configure of open API document.</param>
        private static void AddOpenApiSecurity(AspNetCoreOpenApiDocumentGeneratorSettings configure)
        {
            var authorityUrl = Lisec.Base.Configurations.Configuration.GetValue(ServiceBaseConfigurationKeys.AuthenticationAuthorityUrl, BaseConstants.EmptyString);
            configure.AddSecurity("oauth2", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Flow = OpenApiOAuth2Flow.Password,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = authorityUrl + "/connect/token",
                        Scopes = new Dictionary<string, string>
                            {
                                { Lisec.Base.Configurations.Configuration.GetValue(ServiceBaseConfigurationKeys.SwaggerApiScope, Lisec.Base.Configurations.Configuration.GetValue(ServiceBaseConfigurationKeys.AuthenticationClientScope, "default.manage")), "API scope"}
                            },
                    }
                },
            });
            configure.OperationProcessors.Add(new OperationSecurityScopeProcessor("oauth2"));
        }

        /// <summary>
        /// To get api versions
        /// </summary>
        /// <param name="openApiDetails">Specify list of OpenApiVersionDetail</param>
        /// <returns></returns>
        private static IEnumerable<ApiVersion> GetApiVersions(List<OpenApiVersionDetail> openApiDetails)
        {
            List<ApiVersion> list = new List<ApiVersion>();
            foreach (OpenApiVersionDetail openApiDetail in openApiDetails)
            {
                list.Add(new ApiVersion(openApiDetail.MajorVersion, openApiDetail.MinorVersion));
            }

            return list;
        }

        /// <summary>
        /// To add database and it's healthcheck to DI.
        /// </summary>
        /// <param name="services">Specify IServiceCollection.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddParkingAppDatabaseContext(this IServiceCollection services)
        {
            var databaseType = CommonConfiguration.GetDatabaseType();

            if (databaseType == Base.Database.DatabaseConnectHandler.DatabaseType.Postgres)
            {
                services.AddDbContext<ParkingAppDbContext, ParkingAppPostgresDBContext>();
            }
            else if (databaseType == Base.Database.DatabaseConnectHandler.DatabaseType.SqlServer)
            {
                services.AddDbContext<ParkingAppDbContext, ParkingAppSqlServerDBContext>();
            }
            else
            {
                services.AddDbContext<ParkingAppDbContext>();
            }
            services.AddCustomDbHealthCheck<ParkingAppDbContext>(healthCheckName: "Parking card management app database", schemaComparerName: "Parking card management app database schema comparison", schemaComparerTags: new[] { "Parking card management app database" });

            return services;
        }

        /// <summary>
        /// Add dependency to configure service in startup
        /// </summary>
        /// <param name="services">Specify IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddParkingAppDependency(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<CardsRepository>();
            services.AddScoped<RestrictionsRepository>();
            services.AddScoped<MessagesRepository>();
            services.AddScoped<GroupsRepository>();
            services.AddScoped<PaidParkingsRepository>();
            services.AddScoped<CardsService>();
            services.AddScoped<PaidParkingsService>();
            services.AddScoped<RestrictionsService>();
            services.AddScoped<MessagesService>();
            services.AddScoped<GroupsService>();
            services.AddScoped<UserCarsRepository>();
            services.AddScoped<UserCardsRepository>();
            services.AddScoped<UserCarsService>();
            services.AddScoped<UserCardsService>();
            services.AddScoped<ParkingAppUtility>();
            return services;
        }
    }
}

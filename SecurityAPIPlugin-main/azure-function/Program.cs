// Copyright (c) Microsoft. All rights reserved.

using AIPlugins.AzureFunctions.Extensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Models;

const string DefaultSemanticFunctionsFolder = "Prompts";
string semanticFunctionsFolder = Environment.GetEnvironmentVariable("SEMANTIC_SKILLS_FOLDER") ?? DefaultSemanticFunctionsFolder;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {



        services
            .AddScoped<IKernel>((providers) =>
            {
                // This will be called each time a new Kernel is needed

                // Get a logger instance
                ILogger<IKernel> logger = providers
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger<IKernel>();

                // Register your AI Providers...
                var appSettings = AppSettings.LoadSettings();
                IKernel kernel = new KernelBuilder()
                    .WithChatCompletionService(appSettings.Kernel)
                    .WithLogger(logger)
                    .Build();

                // Load your semantic functions...
                kernel.ImportPromptsFromDirectory(appSettings.AIPlugin.NameForModel, semanticFunctionsFolder);

                var context = kernel.CreateNewContext();
                context["options"] = @"1.Email_API - This API is used to check email related information, 
                                       2.Security_API - This API is used to check security related information,
                                       3.User_API - This API is used to check user related information";


                return kernel;
            })
            .AddScoped<IAIPluginRunner, AIPluginRunner>();
    })
    .Build();

host.Run();

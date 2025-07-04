using SurveyInterviewer.EfCore;
using SurveyInterviewer.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(MigrationWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<SurveysDbContext>("surveysdb");

builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();

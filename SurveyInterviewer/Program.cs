using SurveyInterviewer;
using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;
using SurveyInterviewer.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<SurveysDbContext>(
    "surveysdb" /*,
    // make sure you add the secret:
    // dotnet user-secrets init
    // dotnet user-secrets set "surveysdb:ConnectionString" "<CONNECTION STRING>"
    s => s.ConnectionString = builder.Configuration["surveysdb:ConnectionString"]*/);

// Add services to the container.
builder.Services
    .AddTransient<ISurveyRepository, SurveyRepository>()
    .AddTransient<IGenericRepository<Survey>, SurveyRepository>()

    .AddTransient<IInterviewRepository, InterviewRepository>()
    .AddTransient<IGenericRepository<Interview>, InterviewRepository>()

    .AddTransient<IInterviewResultRepository, InterviewResultRepository>()
    .AddTransient<IGenericRepository<InterviewResult>, InterviewResultRepository>()

    .AddTransient<IQuestionAnswerRepository, QuestionAnswerRepository>()
    .AddTransient<IGenericRepository<QuestionAnswer>, QuestionAnswerRepository>()

    .AddTransient<ISurveyQuestionRepository, SurveyQuestionRepository>()
    .AddTransient<IGenericRepository<SurveyQuestion>, SurveyQuestionRepository>()

    .AddTransient<ITerminatedAnswerRepository, TerminatedAnswerRepository>()
    .AddTransient<IGenericRepository<TerminatedQuestionAnswer>, TerminatedAnswerRepository>()

    .AddTransient<IUnitOfWork, UnitOfWork>()
    .AddTransient<ISurveyService, SurveyService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

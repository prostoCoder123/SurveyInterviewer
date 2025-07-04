var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("db").WithPgAdmin(); //adds a container based on the docker.io/dpage/pgadmin4 image
var tasksDb = db.AddDatabase("surveysdb");

var migrations = builder.AddProject<Projects.SurveyInterviewer_MigrationWorker>("migrations")
    .WithReference(tasksDb)
    .WaitFor(tasksDb);

var taskManager = builder.AddProject<Projects.SurveyInterviewer>("surveyInterviewer")
    .WithReference(tasksDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

builder.Build().Run();

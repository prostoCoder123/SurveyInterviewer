using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using System.Diagnostics;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.MigrationService;

public class MigrationWorker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SurveysDbContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(SurveysDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    private static async Task SeedDataAsync(SurveysDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Seed the database
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var questions = new List<Question>
            {
                new Question() { Id = 1, Title = "Укажите ваш возраст:" },
                new Question() { Id = 2, Title = "Укажите ваш пол:" },
                new Question() { Id = 3, Title = "Укажите уровень вашего дохода:" },

                new Question() { Id = 4, Title = "Сколько раз в неделю вы заказываете еду на дом:" },
                new Question() { Id = 5, Title = "Какими сервисами доставки еды вы пользовались за последний месяц:" },

                new Question() { Id = 6, Title = "Как часто вы покупаете одежду в онлайн магазинах:" },
                new Question() { Id = 7, Title = "Какие бренды одежды вы предпочитаете покупать онлайн:" },
            };

            var answers = new List<Answer>()
            {
                new Answer() { Id = 1, Title = "18-25" },
                new Answer() { Id = 2, Title = "25-40" },
                new Answer() { Id = 3, Title = "40-65" },
                new Answer() { Id = 4, Title = ">65" },

                new Answer() { Id = 5, Title = "мужской" },
                new Answer() { Id = 6, Title = "женский" },

                new Answer() { Id = 7, Title = "< 50.000 руб" },
                new Answer() { Id = 8, Title = "50.000 - 70.000  руб" },
                new Answer() { Id = 9, Title = "70.000 - 165.000  руб" },
                new Answer() { Id = 10, Title = "> 165.000  руб" },

                new Answer() { Id = 11, Title = "не заказываю" },
                new Answer() { Id = 12, Title = "1-3" },
                new Answer() { Id = 13, Title = "3-7" },

                new Answer() { Id = 14, Title = "Яндекс Еда" },
                new Answer() { Id = 15, Title = "Самокат" },
                new Answer() { Id = 16, Title = "Пятерочка" },

                new Answer() { Id = 17, Title = "менее 1 раза в месяц" },
                new Answer() { Id = 18, Title = "1-2 раза в месяц" },
                new Answer() { Id = 19, Title = "более 2 раз в месяц" },

                new Answer() { Id = 20, Title = "Zara" },
                new Answer() { Id = 21, Title = "Lime" },
                new Answer() { Id = 22, Title = "Mango" },
            };

            var surveys = new List<Survey>
            {
                new Survey() { Id = 1, Name = "Сервисы доставки еды", Description = "Получить сведения о сервисах доставки еды", CreatedAt = DateTime.UtcNow },
                new Survey() { Id = 2, Name = "Шопинг одежды в онлайн магазинах", Description = "Расскажите о том, насколько вы довольны шопингом в онлайн магазинах", CreatedAt = DateTime.UtcNow }
            };

            var surveyQuestions = new List<SurveyQuestion>()
            {
                new SurveyQuestion() { Id = 1, SurveyId = 1, QuestionId = 1 },
                new SurveyQuestion() { Id = 2, SurveyId = 1, QuestionId = 2 },
                new SurveyQuestion() { Id = 3, SurveyId = 1, QuestionId = 3 },
                new SurveyQuestion() { Id = 4, SurveyId = 1, QuestionId = 4 },
                new SurveyQuestion() { Id = 5, SurveyId = 1, QuestionId = 5, IsMultipleSelection = true },

                new SurveyQuestion() { Id = 6, SurveyId = 2, QuestionId = 1 },
                new SurveyQuestion() { Id = 7, SurveyId = 2, QuestionId = 2 },
                new SurveyQuestion() { Id = 8, SurveyId = 2, QuestionId = 3 },
                new SurveyQuestion() { Id = 9, SurveyId = 2, QuestionId = 6 },
                new SurveyQuestion() { Id = 10, SurveyId = 2, QuestionId = 7, IsMultipleSelection = true },
            };

            var questionAnswers = new List<QuestionAnswer>()
            {
                new QuestionAnswer() { Id = 1, SurveyQuestionId = 1, AnswerId = 1, Order = 0 },
                new QuestionAnswer() { Id = 2, SurveyQuestionId = 1, AnswerId = 2, Order = 1 },
                new QuestionAnswer() { Id = 3, SurveyQuestionId = 1, AnswerId = 3, Order = 2 },
                new QuestionAnswer() { Id = 4, SurveyQuestionId = 1, AnswerId = 4, Order = 3 },

                new QuestionAnswer() { Id = 5, SurveyQuestionId = 2, AnswerId = 5, Order = 0 },
                new QuestionAnswer() { Id = 6, SurveyQuestionId = 2, AnswerId = 6, Order = 1 },

                new QuestionAnswer() { Id = 7, SurveyQuestionId = 3, AnswerId = 7, Order = 0 },
                new QuestionAnswer() { Id = 8, SurveyQuestionId = 3, AnswerId = 8, Order = 1 },
                new QuestionAnswer() { Id = 9, SurveyQuestionId = 3, AnswerId = 9, Order = 2 },
                new QuestionAnswer() { Id = 10, SurveyQuestionId = 3, AnswerId = 10, Order = 3 },

                new QuestionAnswer() { Id = 11, SurveyQuestionId = 4, AnswerId = 11, Order = 0 },
                new QuestionAnswer() { Id = 12, SurveyQuestionId = 4, AnswerId = 12, Order = 1 },
                new QuestionAnswer() { Id = 13, SurveyQuestionId = 4, AnswerId = 13, Order = 2 },

                new QuestionAnswer() { Id = 14, SurveyQuestionId = 5, AnswerId = 14, Order = 0 },
                new QuestionAnswer() { Id = 15, SurveyQuestionId = 5, AnswerId = 15, Order = 1 },
                new QuestionAnswer() { Id = 16, SurveyQuestionId = 5, AnswerId = 16, Order = 2 },


                new QuestionAnswer() { Id = 17, SurveyQuestionId = 6, AnswerId = 1, Order = 0 },
                new QuestionAnswer() { Id = 18, SurveyQuestionId = 6, AnswerId = 2, Order = 1 },
                new QuestionAnswer() { Id = 19, SurveyQuestionId = 6, AnswerId = 3, Order = 2 },
                new QuestionAnswer() { Id = 20, SurveyQuestionId = 6, AnswerId = 4, Order = 3 },

                new QuestionAnswer() { Id = 21, SurveyQuestionId = 7, AnswerId = 5, Order = 0 },
                new QuestionAnswer() { Id = 22, SurveyQuestionId = 7, AnswerId = 6, Order = 1 },

                new QuestionAnswer() { Id = 23, SurveyQuestionId = 8, AnswerId = 7, Order = 0 },
                new QuestionAnswer() { Id = 24, SurveyQuestionId = 8, AnswerId = 8, Order = 1 },
                new QuestionAnswer() { Id = 25, SurveyQuestionId = 8, AnswerId = 9, Order = 2 },
                new QuestionAnswer() { Id = 26, SurveyQuestionId = 8, AnswerId = 10, Order = 3 },

                new QuestionAnswer() { Id = 27, SurveyQuestionId = 9, AnswerId = 17, Order = 0 },
                new QuestionAnswer() { Id = 28, SurveyQuestionId = 9, AnswerId = 18, Order = 1 },
                new QuestionAnswer() { Id = 29, SurveyQuestionId = 9, AnswerId = 19, Order = 2 },

                new QuestionAnswer() { Id = 30, SurveyQuestionId = 10, AnswerId = 20, Order = 0 },
                new QuestionAnswer() { Id = 31, SurveyQuestionId = 10, AnswerId = 21, Order = 1 },
                new QuestionAnswer() { Id = 32, SurveyQuestionId = 10, AnswerId = 22, Order = 2 },
            };

            var terminatedAnswers = new List<TerminatedQuestionAnswer>()
            {
                new TerminatedQuestionAnswer() { QuestionAnswerId = 11, Reason = "Не заказывает еду на дом" },
                new TerminatedQuestionAnswer() { QuestionAnswerId = 19, Reason = "Опрос об онлайн шопинге для молодой аудитории" },
                new TerminatedQuestionAnswer() { QuestionAnswerId = 20, Reason = "Опрос об онлайн шопинге для молодой аудитории" }
            };

            dbContext.Set<Question>().AddRange(questions);
            dbContext.Set<Answer>().AddRange(answers);
            dbContext.Set<Survey>().AddRange(surveys);
            dbContext.Set<SurveyQuestion>().AddRange(surveyQuestions);
            dbContext.Set<QuestionAnswer>().AddRange(questionAnswers);
            dbContext.Set<TerminatedQuestionAnswer>().AddRange(terminatedAnswers);

            await dbContext.SaveChangesAsync(cancellationToken);

            surveyQuestions[0].NextQuestionId = 2;
            surveyQuestions[1].PrevQuestionId = 1;
            surveyQuestions[1].NextQuestionId = 3;
            surveyQuestions[2].PrevQuestionId = 2;
            surveyQuestions[2].NextQuestionId = 4;
            surveyQuestions[3].PrevQuestionId = 3;
            surveyQuestions[3].NextQuestionId = 5;
            surveyQuestions[4].PrevQuestionId = 4; // 1-5

            surveyQuestions[5].NextQuestionId = 7; // 6-10
            surveyQuestions[6].PrevQuestionId = 6;
            surveyQuestions[6].NextQuestionId = 8;
            surveyQuestions[7].PrevQuestionId = 7;
            surveyQuestions[7].NextQuestionId = 9;
            surveyQuestions[8].PrevQuestionId = 8;
            surveyQuestions[8].NextQuestionId = 10;
            surveyQuestions[9].PrevQuestionId = 9;

            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
}
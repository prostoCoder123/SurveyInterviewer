using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SurveyInterviewer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("answers_pk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsTerminated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("interviews_pk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("questions_pk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("surveys_pk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    SurveyId = table.Column<int>(type: "integer", nullable: false),
                    PrevQuestionId = table.Column<int>(type: "integer", nullable: true),
                    NextQuestionId = table.Column<int>(type: "integer", nullable: true),
                    IsMultipleSelection = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("surveyquestions_pk", x => x.Id);
                    table.ForeignKey(
                        name: "surveyquestions_next_fk",
                        column: x => x.NextQuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "surveyquestions_prev_fk",
                        column: x => x.PrevQuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "surveyquestions_questions_fk",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "surveyquestions_surveys_fk",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InterviewResults",
                columns: table => new
                {
                    SurveyInterviewId = table.Column<int>(type: "integer", nullable: false),
                    QuestionAnswerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("interviewresult_pk", x => new { x.SurveyInterviewId, x.QuestionAnswerId });
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SurveyQuestionId = table.Column<int>(type: "integer", nullable: false),
                    AnswerId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    InterviewResultQuestionAnswerId = table.Column<int>(type: "integer", nullable: true),
                    InterviewResultSurveyInterviewId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("questionanswers_pk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswers_InterviewResults_InterviewResultSurveyInter~",
                        columns: x => new { x.InterviewResultSurveyInterviewId, x.InterviewResultQuestionAnswerId },
                        principalTable: "InterviewResults",
                        principalColumns: new[] { "SurveyInterviewId", "QuestionAnswerId" });
                    table.ForeignKey(
                        name: "questionanswers_answers_fk",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "questionanswers_surveyquestions_fk",
                        column: x => x.SurveyQuestionId,
                        principalTable: "SurveyQuestions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SurveyInterviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SurveyId = table.Column<int>(type: "integer", nullable: false),
                    InterviewId = table.Column<int>(type: "integer", nullable: false),
                    InterviewResultQuestionAnswerId = table.Column<int>(type: "integer", nullable: true),
                    InterviewResultSurveyInterviewId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("surveyinterview_pk", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyInterviews_InterviewResults_InterviewResultSurveyInte~",
                        columns: x => new { x.InterviewResultSurveyInterviewId, x.InterviewResultQuestionAnswerId },
                        principalTable: "InterviewResults",
                        principalColumns: new[] { "SurveyInterviewId", "QuestionAnswerId" });
                    table.ForeignKey(
                        name: "questionanswers_answers_fk",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "questionanswers_surveyquestions_fk",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TerminatedQuestionAnswers",
                columns: table => new
                {
                    QuestionAnswerId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminatedQuestionAnswers", x => x.QuestionAnswerId);
                    table.ForeignKey(
                        name: "terminatedquestionanswers_questionanswers_fk",
                        column: x => x.QuestionAnswerId,
                        principalTable: "QuestionAnswers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewResults_QuestionAnswerId",
                table: "InterviewResults",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_InterviewResultSurveyInterviewId_InterviewR~",
                table: "QuestionAnswers",
                columns: new[] { "InterviewResultSurveyInterviewId", "InterviewResultQuestionAnswerId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_SurveyQuestionId",
                table: "QuestionAnswers",
                column: "SurveyQuestionId");

            migrationBuilder.CreateIndex(
                name: "questionanswers_unique",
                table: "QuestionAnswers",
                columns: new[] { "AnswerId", "SurveyQuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyInterviews_InterviewResultSurveyInterviewId_Interview~",
                table: "SurveyInterviews",
                columns: new[] { "InterviewResultSurveyInterviewId", "InterviewResultQuestionAnswerId" });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyInterviews_SurveyId",
                table: "SurveyInterviews",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "surveyinterview_unique",
                table: "SurveyInterviews",
                columns: new[] { "InterviewId", "SurveyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_NextQuestionId",
                table: "SurveyQuestions",
                column: "NextQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_PrevQuestionId",
                table: "SurveyQuestions",
                column: "PrevQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyQuestions_QuestionId",
                table: "SurveyQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "surveyquestions_unique",
                table: "SurveyQuestions",
                columns: new[] { "SurveyId", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "interviewresult_questionanswers_fk",
                table: "InterviewResults",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "interviewresult_surveyinterviews_fk",
                table: "InterviewResults",
                column: "SurveyInterviewId",
                principalTable: "SurveyInterviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "interviewresult_questionanswers_fk",
                table: "InterviewResults");

            migrationBuilder.DropForeignKey(
                name: "interviewresult_surveyinterviews_fk",
                table: "InterviewResults");

            migrationBuilder.DropTable(
                name: "TerminatedQuestionAnswers");

            migrationBuilder.DropTable(
                name: "QuestionAnswers");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "SurveyQuestions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SurveyInterviews");

            migrationBuilder.DropTable(
                name: "InterviewResults");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "Interviews");
        }
    }
}

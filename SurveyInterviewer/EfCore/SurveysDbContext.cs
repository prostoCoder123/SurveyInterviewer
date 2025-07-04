using Microsoft.EntityFrameworkCore;

namespace SurveyInterviewer.EfCore;

public partial class SurveysDbContext : DbContext
{
    public SurveysDbContext()
    {
    }

    public SurveysDbContext(DbContextOptions<SurveysDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<SurveyQuestion> SurveyQuestions { get; set; }

    public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; }

    public virtual DbSet<SurveyInterview> SurveyInterviews { get; set; }

    public virtual DbSet<InterviewResult> InterviewResults { get; set; }

    public virtual DbSet<TerminatedQuestionAnswer> TerminatedQuestionAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questions_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Title).HasMaxLength(300);
        });

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("answers_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Title).HasMaxLength(300);
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("surveys_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(p => p.Name).HasMaxLength(500);
            entity.Property(p => p.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("interviews_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<SurveyQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("surveyquestions_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => new { e.SurveyId, e.QuestionId }, "surveyquestions_unique").IsUnique();

            entity.HasOne(d => d.NextQuestion).WithMany(p => p.NextQuestions)
                .HasForeignKey(d => d.NextQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("surveyquestions_next_fk");

            entity.HasOne(d => d.PrevQuestion).WithMany(p => p.PrevQuestions)
                .HasForeignKey(d => d.PrevQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("surveyquestions_prev_fk");

            entity.HasOne(d => d.Question).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("surveyquestions_questions_fk");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyQuestions)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("surveyquestions_surveys_fk");
        });

        modelBuilder.Entity<QuestionAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questionanswers_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => new { e.AnswerId, e.SurveyQuestionId }, "questionanswers_unique").IsUnique();

            entity.HasOne(d => d.Answer).WithMany(p => p.QuestionAnswers)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("questionanswers_answers_fk");

            entity.HasOne(d => d.SurveyQuestion).WithMany(p => p.QuestionAnswers)
                .HasForeignKey(d => d.SurveyQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("questionanswers_surveyquestions_fk");
        });

        modelBuilder.Entity<SurveyInterview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("surveyinterview_pk");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasIndex(e => new { e.InterviewId, e.SurveyId }, "surveyinterview_unique").IsUnique();

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyInterviews)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("questionanswers_answers_fk");

            entity.HasOne(d => d.Interview).WithMany(p => p.SurveyInterviews)
                .HasForeignKey(d => d.InterviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("questionanswers_surveyquestions_fk");
        });

        modelBuilder.Entity<InterviewResult>(entity =>
        {
            entity.HasKey(e => new { e.SurveyInterviewId, e.QuestionAnswerId }).HasName("interviewresult_pk");

            entity.HasOne(d => d.SurveyInterview).WithMany(p => p.InterviewResults)
                .HasForeignKey(d => d.SurveyInterviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interviewresult_surveyinterviews_fk");

            entity.HasOne(d => d.QuestionAnswer).WithMany(p => p.InterviewResults)
                .HasForeignKey(d => d.QuestionAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interviewresult_questionanswers_fk");
        });

        modelBuilder.Entity<TerminatedQuestionAnswer>(entity =>
        {
            entity.HasKey(e => e.QuestionAnswerId);

            entity.Property(e => e.QuestionAnswerId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.QuestionAnswer).WithOne(p => p.TerminatedQuestionAnswer)
                .HasForeignKey<TerminatedQuestionAnswer>(d => d.QuestionAnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("terminatedquestionanswers_questionanswers_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

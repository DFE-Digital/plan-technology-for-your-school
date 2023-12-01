﻿// <auto-generated />
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Dfe.PlanTech.Infrastructure.Data.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    partial class CmsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("ContentComponentDbEntity");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.PageContentDbEntity", b =>
                {
                    b.Property<string>("BeforeTitleContentId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("BeforeTitleContentPagesId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ContentComponentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ContentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ContentPagesId")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("PageId")
                        .IsRequired()
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("BeforeTitleContentId", "BeforeTitleContentPagesId");

                    b.HasIndex("BeforeTitleContentPagesId");

                    b.HasIndex("ContentComponentId");

                    b.HasIndex("ContentId");

                    b.HasIndex("ContentPagesId");

                    b.HasIndex("PageId");

                    b.ToTable("PageContentDbEntity");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<bool>("DisplayBackButton")
                        .HasColumnType("bit");

                    b.Property<bool>("DisplayHomeButton")
                        .HasColumnType("bit");

                    b.Property<bool>("DisplayOrganisationName")
                        .HasColumnType("bit");

                    b.Property<bool>("DisplayTopicTitle")
                        .HasColumnType("bit");

                    b.Property<string>("InternalName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganisationName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RequiresAuthorisation")
                        .HasColumnType("bit");

                    b.Property<string>("SectionTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TitleId")
                        .HasColumnType("nvarchar(30)");

                    b.HasIndex("TitleId");

                    b.ToTable("Pages", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("ContentfulId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Titles", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Questionnaire.Models.AnswerDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("Maturity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NextQuestionId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("ParentQuestionId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("NextQuestionId");

                    b.HasIndex("ParentQuestionId");

                    b.ToTable("Answers", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("HelpText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Param")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Questions", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.PageContentDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithMany()
                        .HasForeignKey("BeforeTitleContentId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", null)
                        .WithMany()
                        .HasForeignKey("BeforeTitleContentPagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", "ContentComponent")
                        .WithMany()
                        .HasForeignKey("ContentComponentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithMany()
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", null)
                        .WithMany()
                        .HasForeignKey("ContentPagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", "Page")
                        .WithMany()
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentComponent");

                    b.Navigation("Page");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", "Title")
                        .WithMany("Pages")
                        .HasForeignKey("TitleId");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Questionnaire.Models.AnswerDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Questionnaire.Models.AnswerDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", "NextQuestion")
                        .WithMany("PreviousAnswers")
                        .HasForeignKey("NextQuestionId");

                    b.HasOne("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", "ParentQuestion")
                        .WithMany("Answers")
                        .HasForeignKey("ParentQuestionId");

                    b.Navigation("NextQuestion");

                    b.Navigation("ParentQuestion");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", b =>
                {
                    b.Navigation("Pages");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Questionnaire.Models.QuestionDbEntity", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("PreviousAnswers");
                });
#pragma warning restore 612, 618
        }
    }
}

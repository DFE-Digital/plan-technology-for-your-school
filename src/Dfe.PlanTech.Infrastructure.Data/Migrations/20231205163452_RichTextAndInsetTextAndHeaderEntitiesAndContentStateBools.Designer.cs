﻿// <auto-generated />
using Dfe.PlanTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Dfe.PlanTech.Infrastructure.Data.Migrations
{
    [DbContext(typeof(CmsDbContext))]
    [Migration("20231205163452_RichTextAndInsetTextAndHeaderEntitiesAndContentStateBools")]
    partial class RichTextAndInsetTextAndHeaderEntitiesAndContentStateBools
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("Archived")
                        .HasColumnType("bit");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<bool>("Published")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("ContentComponents", "Contentful");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.PageContentDbEntity", b =>
                {
                    b.Property<string>("ContentComponentId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("PageId")
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ContentComponentId", "PageId");

                    b.HasIndex("PageId");

                    b.ToTable("PageContents", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.HeaderDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<int>("Tag")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Headers", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.InsetTextDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("InsetTexts", "Contentful");
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

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("DataId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("NodeType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RichTextContentDbEntityId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("DataId");

                    b.HasIndex("RichTextContentDbEntityId");

                    b.ToTable("RichTextContents", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextDataDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("Uri")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RichTextDataDbEntity", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextMarkDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

                    b.Property<string>("RichTextContentDbEntityId")
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("RichTextContentDbEntityId");

                    b.ToTable("RichTextMarkDbEntity", "Contentful");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.TitleDbEntity", b =>
                {
                    b.HasBaseType("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity");

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
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", "ContentComponent")
                        .WithMany()
                        .HasForeignKey("ContentComponentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.PageDbEntity", "Page")
                        .WithMany()
                        .HasForeignKey("PageId")
                        .OnDelete(DeleteBehavior.Restrict)
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

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.RichTextDataDbEntity", "Data")
                        .WithMany()
                        .HasForeignKey("DataId");

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", null)
                        .WithMany("Content")
                        .HasForeignKey("RichTextContentDbEntityId");

                    b.Navigation("Data");
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextDataDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Content.Models.RichTextDataDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextMarkDbEntity", b =>
                {
                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.ContentComponentDbEntity", null)
                        .WithOne()
                        .HasForeignKey("Dfe.PlanTech.Domain.Content.Models.RichTextMarkDbEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", null)
                        .WithMany("Marks")
                        .HasForeignKey("RichTextContentDbEntityId");
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

            modelBuilder.Entity("Dfe.PlanTech.Domain.Content.Models.RichTextContentDbEntity", b =>
                {
                    b.Navigation("Content");

                    b.Navigation("Marks");
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
﻿// <auto-generated />
using System;
using BudgetBackend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BudgetBackend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BudgetBackend.Models.BudgetRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DebtBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("DebtToIncomeRatio")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Entertainment")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Groceries")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Insurance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("InvestmentBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Investments")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("LoanPayments")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("NetWorth")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("RentMortgage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Salary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SavingsBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("SavingsRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Subscriptions")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalExpenses")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalIncome")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Transportation")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Utilities")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("BudgetRecords");
                });
#pragma warning restore 612, 618
        }
    }
}

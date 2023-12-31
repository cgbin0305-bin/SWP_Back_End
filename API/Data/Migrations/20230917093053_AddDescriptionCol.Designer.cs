﻿// <auto-generated />
using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Data.Migrations
{
    [DbContext(typeof(WebContext))]
    [Migration("20230917093053_AddDescriptionCol")]
    partial class AddDescriptionCol
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.10");

            modelBuilder.Entity("API.Entities.HouseHoldChores", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ChoresName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("HouseHoldChores");
                });

            modelBuilder.Entity("API.Entities.OrderHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("GuestAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GuestEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GuestPhone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkerId");

                    b.ToTable("OrderHistory");
                });

            modelBuilder.Entity("API.Entities.Worker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Fee")
                        .HasColumnType("money");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Version")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Worker");
                });

            modelBuilder.Entity("API.Entities.Workers_Chores", b =>
                {
                    b.Property<int>("WorkerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChoreId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WorkerId", "ChoreId");

                    b.HasIndex("ChoreId");

                    b.ToTable("Workers_Chores");
                });

            modelBuilder.Entity("API.Entities.OrderHistory", b =>
                {
                    b.HasOne("API.Entities.Worker", "Worker")
                        .WithMany("orderHistories")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("API.Entities.Workers_Chores", b =>
                {
                    b.HasOne("API.Entities.HouseHoldChores", "Chore")
                        .WithMany()
                        .HasForeignKey("ChoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.Worker", "Worker")
                        .WithMany()
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chore");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("API.Entities.Worker", b =>
                {
                    b.Navigation("orderHistories");
                });
#pragma warning restore 612, 618
        }
    }
}

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
    [Migration("20231103095419_Add_TrackingWOrker_Table")]
    partial class Add_TrackingWOrker_Table
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

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
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

                    b.Property<string>("GuestName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GuestPhone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<int>("WorkerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WorkerId");

                    b.ToTable("OrderHistory");
                });

            modelBuilder.Entity("API.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Rate")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("API.Entities.TrackingWorker", b =>
                {
                    b.Property<int>("WorkerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChoreId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Fee")
                        .HasColumnType("money");

                    b.HasKey("WorkerId", "ChoreId");

                    b.HasIndex("ChoreId");

                    b.ToTable("TrackingWorker");
                });

            modelBuilder.Entity("API.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("API.Entities.Worker", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Fee")
                        .HasColumnType("money");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("PublicId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("WorkingState")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Worker");
                });

            modelBuilder.Entity("API.Entities.Workers_Chores", b =>
                {
                    b.Property<int>("WorkerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChoreId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.HasKey("WorkerId", "ChoreId");

                    b.HasIndex("ChoreId");

                    b.ToTable("Workers_Chores");
                });

            modelBuilder.Entity("API.Entities.OrderHistory", b =>
                {
                    b.HasOne("API.Entities.Worker", "Worker")
                        .WithMany("OrderHistories")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("API.Entities.Review", b =>
                {
                    b.HasOne("API.Entities.OrderHistory", "OrderHistory")
                        .WithOne("Review")
                        .HasForeignKey("API.Entities.Review", "Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("OrderHistory");
                });

            modelBuilder.Entity("API.Entities.TrackingWorker", b =>
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
                    b.HasOne("API.Entities.User", "User")
                        .WithOne("Worker")
                        .HasForeignKey("API.Entities.Worker", "Id")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Entities.Workers_Chores", b =>
                {
                    b.HasOne("API.Entities.HouseHoldChores", "Chore")
                        .WithMany()
                        .HasForeignKey("ChoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.Worker", "Worker")
                        .WithMany("Workers_Chores")
                        .HasForeignKey("WorkerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chore");

                    b.Navigation("Worker");
                });

            modelBuilder.Entity("API.Entities.OrderHistory", b =>
                {
                    b.Navigation("Review");
                });

            modelBuilder.Entity("API.Entities.User", b =>
                {
                    b.Navigation("Worker");
                });

            modelBuilder.Entity("API.Entities.Worker", b =>
                {
                    b.Navigation("OrderHistories");

                    b.Navigation("Workers_Chores");
                });
#pragma warning restore 612, 618
        }
    }
}

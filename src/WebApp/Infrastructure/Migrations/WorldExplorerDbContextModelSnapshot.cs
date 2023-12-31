﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using WebApp.Infrastructure;

#nullable disable

namespace WebApp.Infrastructure.Migrations
{
    [DbContext(typeof(WorldExplorerDbContext))]
    partial class WorldExplorerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("WebApp.Infrastructure.Entities.LocationInfoRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<Point>("Location")
                        .IsRequired()
                        .HasColumnType("POINT")
                        .HasAnnotation("Sqlite:Srid", 4326);

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("LocationInfoRequests");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Place", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<Point>("Location")
                        .IsRequired()
                        .HasColumnType("POINT")
                        .HasAnnotation("Sqlite:Srid", 4326);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Places");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PlaceId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ReviewDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.HasIndex("UserId");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Visit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PlaceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("VisitDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PlaceId");

                    b.HasIndex("UserId");

                    b.ToTable("Visits");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Place", b =>
                {
                    b.OwnsMany("WebApp.Infrastructure.Entities.Image", "Images", b1 =>
                        {
                            b1.Property<Guid>("PlaceId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Source")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("PlaceId", "Id");

                            b1.ToTable("Places");

                            b1.ToJson("Images");

                            b1.WithOwner()
                                .HasForeignKey("PlaceId");
                        });

                    b.Navigation("Images");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Review", b =>
                {
                    b.HasOne("WebApp.Infrastructure.Entities.Place", null)
                        .WithMany("Reviews")
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApp.Infrastructure.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.User", b =>
                {
                    b.OwnsOne("WebApp.Infrastructure.Entities.UserSettings", "Settings", b1 =>
                        {
                            b1.Property<string>("UserId")
                                .HasColumnType("TEXT");

                            b1.Property<bool>("TrackUserLocation")
                                .HasColumnType("INTEGER");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.ToJson("Settings");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Settings")
                        .IsRequired();
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Visit", b =>
                {
                    b.HasOne("WebApp.Infrastructure.Entities.Place", null)
                        .WithMany()
                        .HasForeignKey("PlaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApp.Infrastructure.Entities.User", null)
                        .WithMany("Visits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.Place", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("WebApp.Infrastructure.Entities.User", b =>
                {
                    b.Navigation("Visits");
                });
#pragma warning restore 612, 618
        }
    }
}

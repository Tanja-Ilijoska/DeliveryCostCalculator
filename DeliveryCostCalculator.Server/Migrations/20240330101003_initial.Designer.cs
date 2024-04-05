﻿// <auto-generated />
using DeliveryCostCalculator.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DeliveryCostCalculator.Server.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240330101003_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DeliveryCostCalculator.Entities.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("CostCorrectionPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CountryType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<int>("DeliveryServiceId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("DeliveryServiceId");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.DeliveryService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Formula")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DeliveryServices");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.DeliveryServiceProperties", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeliveryServiceId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryServiceId");

                    b.ToTable("DeliveryServiceProperties");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.Delivery", b =>
                {
                    b.HasOne("DeliveryCostCalculator.Entities.Country", "Country")
                        .WithMany("Delivery")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeliveryCostCalculator.Entities.DeliveryService", "DeliveryService")
                        .WithMany("Delivery")
                        .HasForeignKey("DeliveryServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("DeliveryService");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.DeliveryServiceProperties", b =>
                {
                    b.HasOne("DeliveryCostCalculator.Entities.DeliveryService", "DeliveryService")
                        .WithMany("DeliveryServiceProperties")
                        .HasForeignKey("DeliveryServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeliveryService");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.Country", b =>
                {
                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("DeliveryCostCalculator.Entities.DeliveryService", b =>
                {
                    b.Navigation("Delivery");

                    b.Navigation("DeliveryServiceProperties");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MultiEfCoreDbContextSample.Database;

#nullable disable

namespace MultiEfCoreDbContextSample.Migrations1
{
    [DbContext(typeof(DemoDbContext))]
    partial class DemoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.15");

            modelBuilder.Entity("MultiEfCoreDbContextSample.Database.SampleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Age")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("SampleModel");
                });
#pragma warning restore 612, 618
        }
    }
}

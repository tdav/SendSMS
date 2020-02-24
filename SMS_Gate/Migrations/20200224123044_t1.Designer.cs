﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SMS_Gate.Model;

namespace SMS_Gate.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20200224123044_t1")]
    partial class t1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("SMS_Gate.Model.Client", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("created")
                        .HasColumnType("TEXT");

                    b.Property<int>("fail_count")
                        .HasColumnType("INTEGER");

                    b.Property<string>("phone_num")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("sended")
                        .HasColumnType("TEXT");

                    b.Property<int>("status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("text")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}
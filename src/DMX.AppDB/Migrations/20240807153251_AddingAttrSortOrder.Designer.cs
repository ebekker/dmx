﻿// <auto-generated />
using System;
using DMX.AppDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DMX.AppDB.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240807153251_AddingAttrSortOrder")]
    partial class AddingAttrSortOrder
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.6.24327.4");

            modelBuilder.Entity("DMX.AppDB.Models.DmxAttribute", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DomainId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("EntityId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsPrimaryKey")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRequired")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SortOrder")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DomainId");

                    b.HasIndex("EntityId");

                    b.ToTable("att", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxDomain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dom", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PosX")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PoxY")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ZOrder")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ent", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxRelationship", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ChildEdge")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ChildEdgeOffset")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ChildId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Memo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ParentEdge")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ParentEdgeOffset")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChildId");

                    b.HasIndex("ParentId");

                    b.ToTable("rel", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxRelationshipPair", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ChildId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("RelationshipId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ChildId");

                    b.HasIndex("ParentId");

                    b.HasIndex("RelationshipId");

                    b.ToTable("rel_pair", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxShape", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Details")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DimH")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DimW")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Kind")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PosX")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PosY")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ZOrder")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("shape", (string)null);
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxAttribute", b =>
                {
                    b.HasOne("DMX.AppDB.Models.DmxDomain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId");

                    b.HasOne("DMX.AppDB.Models.DmxEntity", "Entity")
                        .WithMany("Attributes")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Domain");

                    b.Navigation("Entity");
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxRelationship", b =>
                {
                    b.HasOne("DMX.AppDB.Models.DmxEntity", "Child")
                        .WithMany("RelationshipsAsChild")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DMX.AppDB.Models.DmxEntity", "Parent")
                        .WithMany("RelationshipsAsParent")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Child");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxRelationshipPair", b =>
                {
                    b.HasOne("DMX.AppDB.Models.DmxAttribute", "Child")
                        .WithMany()
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DMX.AppDB.Models.DmxAttribute", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DMX.AppDB.Models.DmxRelationship", "Relationship")
                        .WithMany("Attributes")
                        .HasForeignKey("RelationshipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Child");

                    b.Navigation("Parent");

                    b.Navigation("Relationship");
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxEntity", b =>
                {
                    b.Navigation("Attributes");

                    b.Navigation("RelationshipsAsChild");

                    b.Navigation("RelationshipsAsParent");
                });

            modelBuilder.Entity("DMX.AppDB.Models.DmxRelationship", b =>
                {
                    b.Navigation("Attributes");
                });
#pragma warning restore 612, 618
        }
    }
}

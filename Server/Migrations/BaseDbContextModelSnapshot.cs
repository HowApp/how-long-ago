﻿// <auto-generated />
using System;
using How.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace How.Server.Migrations
{
    [DbContext(typeof(BaseDbContext))]
    partial class BaseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("How.Core.Database.Entities.Event.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Instant?>("ChangedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("changed_at");

                    b.Property<int?>("ChangedBy")
                        .HasColumnType("integer")
                        .HasColumnName("changed_by");

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("CreteById")
                        .HasColumnType("integer")
                        .HasColumnName("crete_by_id");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)")
                        .HasColumnName("name");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer")
                        .HasColumnName("owner_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<int?>("StorageImageId")
                        .HasColumnType("integer")
                        .HasColumnName("storage_image_id");

                    b.HasKey("Id")
                        .HasName("pk_events");

                    b.HasIndex("OwnerId")
                        .HasDatabaseName("ix_events_owner_id");

                    b.HasIndex("StorageImageId")
                        .HasDatabaseName("ix_events_storage_image_id");

                    b.ToTable("events", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Event.EventRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<Instant>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("CreteById")
                        .HasColumnType("integer")
                        .HasColumnName("crete_by_id");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("description");

                    b.Property<int>("EventId")
                        .HasColumnType("integer")
                        .HasColumnName("event_id");

                    b.Property<int?>("StorageImageId")
                        .HasColumnType("integer")
                        .HasColumnName("storage_image_id");

                    b.HasKey("Id")
                        .HasName("pk_event_records");

                    b.HasIndex("EventId")
                        .HasDatabaseName("ix_event_records_event_id");

                    b.HasIndex("StorageImageId")
                        .HasDatabaseName("ix_event_records_storage_image_id");

                    b.ToTable("event_records", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));
                    NpgsqlPropertyBuilderExtensions.HasIdentityOptions(b.Property<int>("Id"), 3L, null, null, null, null, null);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("role_name_index");

                    b.ToTable("roles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ConcurrencyStamp = "88d484e2-ee7a-49b8-95e6-e34qw5rqb625",
                            Name = "User",
                            NormalizedName = "USER"
                        },
                        new
                        {
                            Id = 2,
                            ConcurrencyStamp = "8b6258e2-ee7a-49b8-95e6-e34qw5rqd484",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        });
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowRoleClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claims_role_id");

                    b.ToTable("role_claims", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<int?>("StorageImageId")
                        .HasColumnType("integer")
                        .HasColumnName("storage_image_id");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("user_name_index");

                    b.HasIndex("StorageImageId")
                        .HasDatabaseName("ix_users_storage_image_id");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claims_user_id");

                    b.ToTable("user_claims", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserLogin", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_logins_user_id");

                    b.ToTable("user_logins", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_roles");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_user_roles_role_id");

                    b.ToTable("user_roles", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserToken", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_tokens");

                    b.ToTable("user_tokens", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Storage.StorageFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("content");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("extension");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("hash");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("path");

                    b.Property<long>("Size")
                        .HasColumnType("bigint")
                        .HasColumnName("size");

                    b.HasKey("Id")
                        .HasName("pk_storage_files");

                    b.HasIndex("Hash")
                        .IsUnique()
                        .HasDatabaseName("ix_storage_files_hash");

                    b.ToTable("storage_files", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Storage.StorageImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ImageHeight")
                        .HasColumnType("integer")
                        .HasColumnName("image_height");

                    b.Property<int>("ImageWidth")
                        .HasColumnType("integer")
                        .HasColumnName("image_width");

                    b.Property<int>("MainId")
                        .HasColumnType("integer")
                        .HasColumnName("main_id");

                    b.Property<int>("ThumbnailHeight")
                        .HasColumnType("integer")
                        .HasColumnName("thumbnail_height");

                    b.Property<int>("ThumbnailId")
                        .HasColumnType("integer")
                        .HasColumnName("thumbnail_id");

                    b.Property<int>("ThumbnailWidth")
                        .HasColumnType("integer")
                        .HasColumnName("thumbnail_width");

                    b.HasKey("Id")
                        .HasName("pk_storage_images");

                    b.HasIndex("MainId")
                        .HasDatabaseName("ix_storage_images_main_id");

                    b.HasIndex("ThumbnailId")
                        .HasDatabaseName("ix_storage_images_thumbnail_id");

                    b.ToTable("storage_images", (string)null);
                });

            modelBuilder.Entity("How.Core.Database.Entities.Event.Event", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_events_users_owner_id");

                    b.HasOne("How.Core.Database.Entities.Storage.StorageImage", "StorageImage")
                        .WithMany()
                        .HasForeignKey("StorageImageId")
                        .HasConstraintName("fk_events_storage_images_storage_image_id");

                    b.Navigation("Owner");

                    b.Navigation("StorageImage");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Event.EventRecord", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Event.Event", "Event")
                        .WithMany("Records")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_event_records_events_event_id");

                    b.HasOne("How.Core.Database.Entities.Storage.StorageImage", "StorageImage")
                        .WithMany()
                        .HasForeignKey("StorageImageId")
                        .HasConstraintName("fk_event_records_storage_images_storage_image_id");

                    b.Navigation("Event");

                    b.Navigation("StorageImage");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowRoleClaim", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_role_claims_roles_role_id");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUser", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Storage.StorageImage", "StorageImage")
                        .WithMany()
                        .HasForeignKey("StorageImageId")
                        .HasConstraintName("fk_users_storage_images_storage_image_id");

                    b.Navigation("StorageImage");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserClaim", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_users_user_id");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserLogin", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_logins_users_user_id");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserRole", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_roles_role_id");

                    b.HasOne("How.Core.Database.Entities.Identity.HowUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_users_user_id");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUserToken", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Identity.HowUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_tokens_users_user_id");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Storage.StorageImage", b =>
                {
                    b.HasOne("How.Core.Database.Entities.Storage.StorageFile", "Main")
                        .WithMany()
                        .HasForeignKey("MainId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_storage_images_storage_files_main_id");

                    b.HasOne("How.Core.Database.Entities.Storage.StorageFile", "Thumbnail")
                        .WithMany()
                        .HasForeignKey("ThumbnailId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_storage_images_storage_files_thumbnail_id");

                    b.Navigation("Main");

                    b.Navigation("Thumbnail");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Event.Event", b =>
                {
                    b.Navigation("Records");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowRole", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("How.Core.Database.Entities.Identity.HowUser", b =>
                {
                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}

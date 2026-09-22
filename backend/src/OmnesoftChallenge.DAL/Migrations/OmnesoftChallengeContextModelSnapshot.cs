using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OmnesoftChallenge.DAL.Context;

#nullable disable

namespace OmnesoftChallenge.DAL.Migrations
{
    [DbContext(typeof(EntityFrameworkContext))]
    partial class OmnesoftChallengeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OmnesoftChallenge.DAL.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("description");

                    b.Property<DateTime>("InsertionDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("insertion_date");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_update_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasPrecision(15, 2)
                        .HasColumnType("numeric(15,2)")
                        .HasColumnName("price");

                    b.HasKey("Id")
                        .HasName("pk_product");

                    b.ToTable("product", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "A sample product for testing.",
                            InsertionDate = new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            LastUpdateDate = new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            Name = "Sample Product",
                            Price = 10.99m
                        });
                });

            modelBuilder.Entity("OmnesoftChallenge.DAL.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<DateTime?>("RevokedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("revoked_at");

                    b.Property<int>("SystemUserId")
                        .HasColumnType("integer")
                        .HasColumnName("system_user_id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("token");

                    b.HasKey("Id")
                        .HasName("pk_refresh_token");

                    b.HasIndex("SystemUserId")
                        .HasDatabaseName("ix_refresh_token_system_user_id");

                    b.HasIndex("Token")
                        .IsUnique()
                        .HasDatabaseName("ix_refresh_token_token");

                    b.ToTable("refresh_token", (string)null);
                });

            modelBuilder.Entity("OmnesoftChallenge.DAL.Entities.SystemUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("password");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_system_user");

                    b.ToTable("system_user", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Active = true,
                            Password = "GqUOUAuIMkk/PrMHRvVxltY1BsgjTSbADpJTN+WrbdI=",
                            Role = "Admin",
                            UserName = "admin"
                        },
                        new
                        {
                            Id = 2,
                            Active = true,
                            Password = "6HaA0dxGQcaL5PHuRJm+WAkW6s+0h7KHFA6Y5zFPHsg=",
                            Role = "Guest",
                            UserName = "guest"
                        });
                });

            modelBuilder.Entity("OmnesoftChallenge.DAL.Entities.RefreshToken", b =>
                {
                    b.HasOne("OmnesoftChallenge.DAL.Entities.SystemUser", "SystemUser")
                        .WithMany("RefreshToken")
                        .HasForeignKey("SystemUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_refresh_token_system_user_system_user_id");

                    b.Navigation("SystemUser");
                });

            modelBuilder.Entity("OmnesoftChallenge.DAL.Entities.SystemUser", b =>
                {
                    b.Navigation("RefreshToken");
                });
#pragma warning restore 612, 618
        }
    }
}

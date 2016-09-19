using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using UploadFile_CoreMVC.Models;

namespace UploadFile_CoreMVC.Migrations
{
    [DbContext(typeof(UploadDbContext))]
    partial class UploadDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UploadFile_CoreMVC.Models.User", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UploadFile_CoreMVC.Models.UserFiles", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("FileName");

                    b.Property<string>("FileType");

                    b.Property<byte[]>("UserFile");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserFiles");
                });

            modelBuilder.Entity("UploadFile_CoreMVC.Models.UserFiles", b =>
                {
                    b.HasOne("UploadFile_CoreMVC.Models.User", "User")
                        .WithMany("UserFiles")
                        .HasForeignKey("UserId");
                });
        }
    }
}

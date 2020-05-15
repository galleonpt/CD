using System;
using Api.Data.Mapping;
using Api.Model;
using Api.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Context
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Mapping
            modelBuilder.Entity<UserEntity>(new UserMap().Configure); //executar a configuração do user
            modelBuilder.Entity<ChatEntity>(new ChatMap().Configure); //executar a configuração do Chat
            modelBuilder.Entity<MessageEntity>(new MessageMap().Configure); //executar a configuração do Message

            //Utilizador padrão
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity
                {
                    Id = 1,
                    Name = "admin",
                    Username = "admin",
                    Password = "admin",
                    Email = "admin@admin.com",
                    ImagePath = "default",
                    FolderPath = "default",
                    CreateAt = DateTime.Now
                }
            );
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<ChatEntity> Chats { get; set; }

        public DbSet<MessageEntity> Messages { get; set; }


    }
}
using BlueBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueBook.Data
{
    public class BookContext : IdentityDbContext
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Mensagem>()
                .HasOne(a => a.usuario)
                .WithMany(d => d.Mensagens)
                .HasForeignKey(d => d.UsuarioID);

            builder.Entity<Postagem>() /*(1,1)Postagem <> Comentário(0,N)*/
                .HasMany(a => a.Comentarios)
                .WithOne(d => d.PostagemOriginal)
                .HasForeignKey(p => p.PostagemId);

            builder.Entity<Postagem>() /*(1,1)Usuário <> Postagem(0,N)*/
                .HasOne(a => a.UsuarioPost)
                .WithMany(d => d.Postagens)
                .HasForeignKey(d => d.UsuarioIdPost);

            builder.Entity<Comentario>() /*(1,1)Usuário <> Comentário(0,N)*/
                .HasOne(a => a.UsuarioComentario)
                .WithMany(d => d.Comentarios)
                .HasForeignKey(d => d.UsuarioIdComentario);

            builder.Entity<Amizade>()
                .HasKey(tt => new { tt.OrigemId, tt.AlvoId });

            builder.Entity<Usuario>()
                .HasMany(u => u.Origem)
                .WithOne(uu => uu.Alvo)
                .HasForeignKey(uu => uu.OrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Usuario>()
                .HasMany(u => u.Alvo)
                .WithOne(uu => uu.Origem)
                .HasForeignKey(uu => uu.AlvoId);
        }

        public DbSet<Mensagem> Mensagem { get; set; }

        public DbSet<Postagem> Postagem { get; set; }

        public DbSet<Comentario> Comentario { get; set; }

        public DbSet<Amizade> Amizade { get; set; }
    }
}

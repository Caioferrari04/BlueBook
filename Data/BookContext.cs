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

            /*Uma postagem pode ter apenas um like de um determinado usuário
             Mas pode ter vários likes de outros usuários
            E um usuário só pode dar um like uma vez para uma postagem
            mas pode dar um like para várias postagens*/
            /*(1,1)Postagem <> Like(0,N)*/
            /*(1,1)Usuario <> Like(0,N)*/
            /*(0,N)PostagemLike <> UsuarioLike(0,N)*/

            builder.Entity<Likes>()
                .HasKey(tt => new { tt.PostagemId, tt.UsuarioId });

            builder.Entity<LikesComentarios>()
                .HasKey(tt => new { tt.ComentarioId, tt.UsuarioId });

            builder.Entity<ComentarioDeComentario>()
                .HasKey(pk => new { pk.ComentarioFonteId, pk.ComentarioFilhoId });

            builder.Entity<Comentario>()
                .HasMany(c => c.SubComentarios)
                .WithOne(sc => sc.ComentarioFonte)
                .HasForeignKey(fk => fk.ComentarioFonteId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Mensagem> Mensagem { get; set; }

        public DbSet<Postagem> Postagem { get; set; }

        public DbSet<Comentario> Comentario { get; set; }

        public DbSet<Amizade> Amizade { get; set; }

        public DbSet<Likes> Likes { get; set; }

        public DbSet<LikesComentarios> LikesComentarios { get; set; }
    }
}

using Alura.Filmes.App.Negocio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Alura.Filmes.App.Dados
{
    public class PessoaConfiguration<T> : IEntityTypeConfiguration<T> where T : Pessoa
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
 
            builder
                .Property(c => c.PrimeiroNome)
                .HasColumnName("first_name")
                .HasColumnType("varchar(45)")
                .IsRequired();

            builder
                .Property(c => c.UltimoNome)
                .HasColumnName("last_name")
                .HasColumnType("varchar(45)")
                .IsRequired();

            builder
             .Property(c => c.Email)
             .HasColumnName("email")
             .HasColumnType("varchar(50)");

            builder.Property(c => c.Ativo).HasColumnName("active");

            builder.Property<DateTime>("last_update")
                 .HasColumnType("datetime")
                 .HasDefaultValueSql("getdate()")
                 .IsRequired();
        }
    }
}

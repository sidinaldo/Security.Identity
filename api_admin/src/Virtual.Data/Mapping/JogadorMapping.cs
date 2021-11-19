using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtual.Domain.Models;

namespace Virtual.Data.Mapping
{
    public class JogadorMapping : IEntityTypeConfiguration<Jogador>
    {
        public void Configure(EntityTypeBuilder<Jogador> builder)
        {
            builder.ToTable("tb_Jogador");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IdJogador");

            builder.Property(p => p.Nome)
                .HasColumnType("VARCHAR(100)")
                .IsRequired();

            builder.Property(p => p.Sobrenome)
                .HasColumnType("VARCHAR(100)")
                .IsRequired();

            builder.Property(p => p.Posicao)
                .HasColumnType("VARCHAR(30)")
               .IsRequired();

            builder.Property(p => p.Foto)
              .IsRequired();

            builder.Property(p => p.Ativo)
              .IsRequired();

            builder.Property(p => p.DataCadastro);

            builder.Property(p => p.IdUsuarioCadastro)
            .IsRequired();
        }
    }
}

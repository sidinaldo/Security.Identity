using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtual.Domain.Models;

namespace Virtual.Data.Mapping
{
    public class JogadorJogoMapping : IEntityTypeConfiguration<JogadorJogo>
    {
        public void Configure(EntityTypeBuilder<JogadorJogo> builder)
        {
            builder.ToTable("tb_JogadorJogo");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IdJogadorJogo");

            builder.Property(p => p.IdJogador)
           .IsRequired();

            builder.Property(p => p.IdJogo)
                .IsRequired();

            builder.Property(p => p.Gols)
               .IsRequired()
               .HasDefaultValue(0);

            builder.Property(p => p.DataCadastro);

            builder.Property(p => p.IdUsuarioCadastro)
            .IsRequired();

            // 1 : 1
            builder.HasOne(a => a.Jogador)
                .WithMany()
                .HasForeignKey(p => p.IdJogador);

            // 1 : 1
            builder.HasOne(a => a.Jogo)
                .WithMany(a => a.Jogadores)
                .HasForeignKey(p => p.IdJogo);

        }
    }
}

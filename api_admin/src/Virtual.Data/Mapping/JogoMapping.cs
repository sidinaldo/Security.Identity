using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtual.Domain.Models;

namespace Virtual.Data.Mapping
{
    public class JogoMapping : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("tb_Jogo");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IdJogo");

            builder.Property(p => p.IdAdversario)
           .IsRequired();

            builder.Property(p => p.IdCidade)
                .IsRequired();

            builder.Property(p => p.Bairro)
                .HasColumnType("VARCHAR(200)")
               .IsRequired();

            builder.Property(p => p.Campo)
                .HasColumnType("VARCHAR(200)")
              .IsRequired();

            builder.Property(p => p.Hora)
                .HasColumnType("VARCHAR(20)")
             .IsRequired();

            builder.Property(p => p.DataJogo)
             .IsRequired();

            builder.Property(p => p.Gols)
            .IsRequired();

            builder.Property(p => p.GolsAdversario)
             .IsRequired();

            builder.Property(p => p.Resultado)
                .HasColumnType("VARCHAR(20)")
            .IsRequired();


            builder.Property(p => p.DataCadastro);

            builder.Property(p => p.IdUsuarioCadastro)
            .IsRequired();

            // 1 : 1
            builder.HasOne(a => a.Adversario)
                .WithMany()
                .HasForeignKey(p => p.IdAdversario);

            // 1 : 1
            builder.HasOne(a => a.Cidade)
                .WithMany()
                .HasForeignKey(p => p.IdCidade);

        }
    }
}

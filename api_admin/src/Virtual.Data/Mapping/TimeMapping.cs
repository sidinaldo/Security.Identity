using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Virtual.Domain.Models;

namespace Virtual.Data.Mapping
{
    public class TimeMapping : IEntityTypeConfiguration<Time>
    {
        public void Configure(EntityTypeBuilder<Time> builder)
        {
            builder.ToTable("tb_Time");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("IdTime");

            builder.Property(p => p.Nome)
                .HasColumnType("VARCHAR(100)")
                .IsRequired();

            builder.Property(p => p.Cidade)
                .HasColumnType("VARCHAR(100)")
               .IsRequired();

            builder.Property(p => p.Bairro)
                .HasColumnType("VARCHAR(100)")
              .IsRequired();

            builder.Property(p => p.DataCadastro);

            builder.Property(p => p.IdUsuarioCadastro)
            .IsRequired();
        }
    
    }
}

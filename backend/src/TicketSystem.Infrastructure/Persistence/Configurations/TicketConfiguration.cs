using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketSystem.Domain.Entities;

namespace TicketSystem.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TicketNumber)
            .IsRequired()
            .HasMaxLength(20)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("'TKT-' || lpad(nextval('ticket_number_seq')::text, 5, '0')");

        builder.HasIndex(t => t.TicketNumber).IsUnique();

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired();

        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);

        builder.Ignore(t => t.IsClosed);
        builder.Ignore(t => t.IsOverdue);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.AssignedToId);
        builder.HasIndex(t => t.CreatedById);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.CreatedAt);
    }
}

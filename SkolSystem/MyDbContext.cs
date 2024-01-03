using Microsoft.EntityFrameworkCore;
using SkolSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkolSystem;

public class MyDbContext : DbContext
{
    // properties för att arbeta mot databasen 
    public DbSet<Personal> Personal { get; set; }
    public DbSet<Student> Student { get; set; }
    public DbSet<Kurs> Kurs { get; set; }
    public DbSet<Betyg> Betyg { get; set; }
    public DbSet<Klass> Klass { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // defienerar kopplingen mellan alla tables
        // beskriver table med foreign key till annan table 
        // beskriver relationerna mellan tables 

        modelBuilder.Entity<Kurs>() 
            .HasOne(k => k.Lärare)
            .WithMany(p => p.Kurser)
            .HasForeignKey(k => k.PersonalID);

        modelBuilder.Entity<Betyg>()
            .HasOne(b => b.Kurs)
            .WithMany(k => k.Betyg)
            .HasForeignKey(b => b.KursID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Betyg>()
            .HasOne(b => b.Student)
            .WithMany(s => s.Betyg)
            .HasForeignKey(b => b.StudentID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Betyg>()
            .HasOne(b => b.Lärare)
            .WithMany(s => s.Betyg)
            .HasForeignKey(b => b.PersonalID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasOne(b => b.Klass)
            .WithMany(s => s.Studenter)
            .HasForeignKey(b => b.KlassID);
        
        // definerar composit primery key av två foreign keys 
        modelBuilder.Entity<Betyg>()
        .HasKey(b => new { b.KursID, b.StudentID });

        // definerar enkla primery keys 
        modelBuilder.Entity<Student>()
        .HasKey(s => s.StudentID);

        modelBuilder.Entity<Personal>()
        .HasKey(p => p.PersonalID);

        modelBuilder.Entity<Klass>()
        .HasKey(k => k.KlassID);

        modelBuilder.Entity<Kurs>()
        .HasKey(k => k.KursID);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // kopplar min lokala databas till min kod
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-UKFB1CL\\MSSQLSERVER01;Initial Catalog=Skolsystem2;Integrated Security=True;TrustServerCertificate=True;");
    }
}


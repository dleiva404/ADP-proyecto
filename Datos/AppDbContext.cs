using Microsoft.EntityFrameworkCore;
using SmartProcessAI.Models;

namespace SmartProcessAI.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proceso> Procesos { get; set; }
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Regla> Reglas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Hallazgo> Hallazgos { get; set; }
        public DbSet<Recomendacion> Recomendaciones { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Decision> Decisiones { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<VersionDocumento> VersionesDocumento { get; set; }
        public DbSet<Correccion> Correcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empresa>().ToTable("Empresa");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Proceso>().ToTable("Proceso");
            modelBuilder.Entity<Tarea>().ToTable("Tarea");
            modelBuilder.Entity<Regla>().ToTable("Regla");
            modelBuilder.Entity<Auditoria>().ToTable("Auditoria");
            modelBuilder.Entity<Hallazgo>().ToTable("Hallazgo");
            modelBuilder.Entity<Recomendacion>().ToTable("Recomendacion");
            modelBuilder.Entity<Evidencia>().ToTable("Evidencia");
            modelBuilder.Entity<Area>().ToTable("Area");
            modelBuilder.Entity<Decision>().ToTable("Decision");
            modelBuilder.Entity<Documento>().ToTable("Documento");
            modelBuilder.Entity<VersionDocumento>().ToTable("VersionDocumento");
            modelBuilder.Entity<Correccion>().ToTable("Correccion");

            modelBuilder.Entity<Recomendacion>()
                .HasOne(r => r.RevisadoPor)
                .WithMany(u => u.RecomendacionesRevisadas)
                .HasForeignKey(r => r.RevisadoPorUsuarioId);

            modelBuilder.Entity<Documento>()
                .HasOne(d => d.SubidoPor).WithMany()
                .HasForeignKey(d => d.SubidoPorUsuarioId);

            modelBuilder.Entity<VersionDocumento>()
                .HasOne(v => v.CreadaPor).WithMany()
                .HasForeignKey(v => v.CreadaPorUsuarioId);

            modelBuilder.Entity<Correccion>()
                .HasOne(c => c.CreadaPor).WithMany()
                .HasForeignKey(c => c.CreadaPorUsuarioId);

            modelBuilder.Entity<Correccion>()
                .HasOne(c => c.AprobadaPor).WithMany()
                .HasForeignKey(c => c.AprobadaPorUsuarioId);


        }
    }
}
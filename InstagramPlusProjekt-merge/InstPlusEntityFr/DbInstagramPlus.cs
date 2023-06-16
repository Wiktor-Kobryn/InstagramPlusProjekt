﻿using Microsoft.EntityFrameworkCore;

namespace InstPlusEntityFr
{
    public class DbInstagramPlus : DbContext
    {
        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Post> Posty { get; set; }
        public DbSet<Komentarz> Komentarze { get; set; }
        public DbSet<PolubieniePosta> PolubieniaPostow { get; set; }
        public DbSet<PolubienieKomentarza> PolubieniaKomentarzy { get; set; }
        public DbSet<Obserwowany> Obserwowani { get; set; }
        public DbSet<Obserwujacy> Obserwujacy { get; set; }
        public DbSet<TagPostu> TagiPostow { get; set; }
        public DbSet<TagUzytkownika> TagiUzytkownikow { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PolubieniePosta>().HasKey(e => new { e.PostId, e.UzytkownikId }); 
            modelBuilder.Entity<PolubienieKomentarza>().HasKey(e => new { e.KomentarzId, e.UzytkownikId });
            modelBuilder.Entity<Obserwujacy>().HasKey(e => new { e.ObserwatorId, e.ObserwowanyId }); 
            modelBuilder.Entity<Obserwowany>().HasKey(e => new { e.ObserwatorId, e.ObserwowanyId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Nie dodawać niczego na koniec
            //tylko wkleić swojego stringa


            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=InstagramPlus;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
                + ";MultipleActiveResultSets=True");

        }
    }
}

namespace система_для_хранения_pdf_файлов.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Files_DBEntities : DbContext
    {
        private static Files_DBEntities _context;
        public Files_DBEntities()
            : base("name=Files_DBEntities")
        {
        }
    
        public static Files_DBEntities GetContext()
        {
            if (_context == null)
            {
                _context = new Files_DBEntities();
            }
            return _context;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Table_Image_Files> Table_Image_Files { get; set; }
        public virtual DbSet<Table_PDF_Files> Table_PDF_Files { get; set; }
        public virtual DbSet<Table_Files> Table_Files { get; set; }
    }
}

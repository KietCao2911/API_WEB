using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using API_DSCS2_WEBBANGIAY.Models;

#nullable disable

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class ShoesEcommereContext : DbContext
    {
        public ShoesEcommereContext()
        {
        }

        public ShoesEcommereContext(DbContextOptions<ShoesEcommereContext> options)
            : base(options)
        {
        }
        public virtual DbSet<DiaChi> DiaChis { get; set; }
        public virtual DbSet<BoSuuTap> BoSuuTaps { get; set; }
        //public virtual DbSet<ChiTietSale> ChiTietSales { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<HinhAnh> HinhAnhs { get; set; }
        public virtual DbSet<ChiTietHinhAnh> ChiTietHinhAnhs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<MauSac> MauSacs { get; set; }
        public virtual DbSet<ReviewStar> ReviewStars { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<RoomMessage> RoomMessages { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<VAT> Vats { get; set; }
        public virtual DbSet<Branchs> Branchs { get; set; }
        public virtual DbSet<GenKey> Keys { get; set; }
        public virtual DbSet<ChiNhanh_SanPham> KhoHangs{ get; set; }
        public virtual DbSet<NCC> NhaCungCap{ get; set; }
        public virtual DbSet<PhieuNhapXuat> PhieuNhapXuats { get; set; }
        public virtual DbSet<ChiTietNhapXuat> ChiTietNhapXuats { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<ChiTietCoupon> ChiTietCoupons { get; set; }
        public virtual DbSet<ChiTietBST> ChiTietBSTs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-FQD5SBV;Database=ShoesEcommere;Trusted_Connection=True;MultipleActiveResultSets=true;");
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            //modelBuilder.Entity<Role>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Id).ValueGeneratedOnAdd();
            //    entity.Property(e => e.RoleName).HasColumnType("nvarchar(30)");
            //});
            modelBuilder.Entity<ChiTietBST>(entity =>
            {

                entity.HasKey(e => new{ e.IDBST,e.MaSanPham});
                entity.HasOne(e => e.SanPhamNavigation).WithMany(x => x.ChiTietBSTs).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.BSTNavigation).WithMany(x => x.ChiTietBSTs).HasForeignKey(x => x.IDBST).OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<Coupon>(entity =>
            {

                entity.HasKey(e => e.MaCoupon);
                entity.Property(x => x.MaCoupon).HasColumnType("char(15)");

            });
            modelBuilder.Entity<ChiTietCoupon>(entity =>
            {
                entity.HasKey(e => new {e.MaSanPham,e.MaCoupon});
                entity.Property(x => x.MaCoupon).HasColumnType("char(15)");
                entity.HasOne(e => e.SanPhamNavigation).WithMany(x => x.ChiTietCoupons).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.CouponNavigation).WithMany(x => x.ChiTietCoupons).HasForeignKey(x => x.MaCoupon).OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<LoaiPhieu>(entity =>
            {

                entity.HasKey(e => e.MaPhieu);
                entity.Property(x => x.MaPhieu).HasColumnType("char(10)");

            });
            modelBuilder.Entity<ChiTietNhapXuat>(entity =>
            {
                entity.HasKey(e => new { e.MaSanPham, e.IDPN});
                entity.Property(e => e.DVT).HasColumnType("char(10)");
                entity.Property(e => e.Id).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.HasOne(e => e.SanPhamNavigation).WithMany(x => x.ChiTietNhapXuats).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.PhieuNhapXuatNavigation).WithMany(x => x.ChiTietNhapXuats).HasForeignKey(x => x.IDPN).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PhieuNhapXuat>(entity =>
            {

                entity.HasKey(e => e.Id);
                entity.Property(x => x.createdAt).HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
                entity.HasOne(e => e.NhaCungCapNavigation).WithMany(x => x.PhieuNhapXuats).HasForeignKey(x => x.IDNCC).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.LoaiPhieuNavigation).WithMany(x => x.PhieuNhapXuats).HasForeignKey(x => x.LoaiPhieu).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TenTaiKhoanNavigation).WithMany(x => x.PhieuNhapXuats).HasForeignKey(x => x.idTaiKhoan).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.DiaChiNavigation).WithMany(x => x.PhieuNhapXuats).HasForeignKey(x => x.IdDiaChi).OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(e => e.KhoHangNavigation).WithMany(x => x.PhieuNhapXuats).HasForeignKey(x => x.MaChiNhanh).OnDelete(DeleteBehavior.NoAction);

            });
            modelBuilder.Entity<NCC>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnType("nvarchar(50)");
                entity.Property(e => e.Phone).HasColumnType("char(10)");
                entity.Property(e => e.Email).HasColumnType("char(50)");
                entity.HasOne(e => e.DiaChiNavigation).WithMany(e => e.NhaCungCaps).HasForeignKey(x => x.IDDiaChi).OnDelete(DeleteBehavior.Cascade); ;
            });
            modelBuilder.Entity<ChiNhanh_SanPham>(entity =>
            {
                entity.HasKey(e => new { e.MaChiNhanh ,e.MaSanPham});
                entity.Property(e => e.ID).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore); ; ;
                entity.HasOne(e => e.SanPhamNavigation).WithMany(e => e.KhoHangs).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade); ;
                entity.HasOne(e => e.BranchNavigation).WithMany(e => e.KhoHangs).HasForeignKey(x => x.MaChiNhanh).OnDelete(DeleteBehavior.Cascade); ;
            });
            modelBuilder.Entity<GenKey>(entity =>
            {
                entity.Property(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Size>(entity =>
            {
                entity.Property(e => e.Id);
                entity.Property(e => e.Id).HasColumnType("char(10)");
                entity.Property(e => e.Size1).HasColumnName("size");
            });
            modelBuilder.Entity<Branchs>(entity =>
            {
                entity.HasKey(e => e.MaChiNhanh);
                entity.Property(e => e.MaChiNhanh).HasColumnType("char(20)");
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.TenChiNhanh).HasColumnType("nvarchar(50)");
                
            });
            modelBuilder.Entity<VAT>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnType("nvarchar(50)");
            });
            modelBuilder.Entity<Type>(entity =>
            {
                entity.HasKey(e =>e.ID);
                entity.Property(e=>e.ID).ValueGeneratedOnAdd();
                entity.Property(e=>e.Name).HasColumnType("nvarchar(50)");
            });
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnType("nvarchar(50)");

            });
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd() ;
                entity.HasOne(e => e.MessageNavigation).WithMany(x => x.Messages).HasForeignKey(x => x.ParentMessageID);
                entity.HasOne(e => e.userNavigation).WithMany(x => x.Messages).HasForeignKey(x => x.creatorID);
            });
            modelBuilder.Entity<RoomMessage>(entity =>
            {
                entity.HasKey(e => new { e.MessageID, e.MaSanPham, e.UserID });
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.HasOne(e => e.SanPhamNavigation).WithMany(x => x.RoomMessages).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TaiKhoanNavigation).WithMany(x => x.RoomMessages).HasForeignKey(x => x.UserID).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MessageNavigation).WithMany(x => x.RoomMessages).HasForeignKey(x => x.MessageID);
            });

            modelBuilder.Entity<DiaChi>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Phone).HasColumnType("char(10)");
                entity.Property(e => e.Email).HasColumnType("char(254)");
                entity.Property(e => e.Name).HasColumnType("nvarchar(30)");
                entity.HasOne(e => e.KhachHangNavigation).WithMany(e => e.DiaChis).HasForeignKey(x => x.IDKH);
                entity.HasOne(e => e.TaiKhoanNavigation).WithMany(e => e.DiaChis).HasForeignKey(x => x.TenTaiKhoan);
            });
            modelBuilder.Entity<DanhMucDetails>(entity =>
            {
                entity.HasKey(e => new { e.danhMucId, e.MaSanPham });
                entity.HasOne(e => e.IdDanhMucNavigation).WithMany(e => e.DanhMucDetails).HasForeignKey(x => x.danhMucId).OnDelete(DeleteBehavior.Cascade); ;
                entity.HasOne(e => e.IdSanPhamNavigation).WithMany(e => e.DanhMucDetails).HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade); ;

            });
            modelBuilder.Entity<BoSuuTap>(entity =>
            {
                entity.ToTable("BoSuuTap");

                entity.Property(e => e.Id).HasColumnName("_id");
                entity.Property(e => e.Img).HasColumnType("text");             
               

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Slug)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("slug")
                    .IsFixedLength(true);

                entity.Property(e => e.TenBoSuuTap).HasMaxLength(30);

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });


            //modelBuilder.Entity<ChiTietSale>(entity =>
            //{
            //    entity.HasKey(e => new { e.IdSale, e.MaSanPham })
            //        .HasName("pk_cts");

            //    entity.ToTable("ChiTietSale");

            //    entity.HasIndex(e => e.MaSanPham, "IX_ChiTietSale_MaSanPham");

            //    entity.Property(e => e.IdSale).HasColumnName("_id_sale");

            //    entity.Property(e => e.MaSanPham)
            //        .HasMaxLength(10)
            //        .IsUnicode(false)
            //        .IsFixedLength(true);

            //    entity.Property(e => e.Giamgia).HasColumnName("giamgia");

            //    entity.HasOne(d => d.IdSaleNavigation)
            //        .WithMany(p => p.ChiTietSales)
            //        .HasForeignKey(d => d.IdSale)
            //        .HasConstraintName("fk_cts_Sale");

            //    entity.HasOne(d => d.MaSanPhamNavigation)
            //        .WithMany(p => p.ChiTietSales)
            //        .HasForeignKey(d => d.MaSanPham)
            //        .HasConstraintName("fk_cts_SP");
            //});

            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.ToTable("DanhMuc");

                entity.HasIndex(e => e.GioiTinhCode, "IX_DanhMuc_GioiTinh_Code");

                entity.Property(e => e.Id).HasColumnName("_id");


                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("slug")
                    .IsFixedLength(true);

                entity.Property(e => e.TenDanhMuc)
                    .IsRequired()
                    .HasMaxLength(30);
            });



            modelBuilder.Entity<HinhAnh>(entity =>
            {
                entity.ToTable("HinhAnh");

                entity.Property(e => e.Id).HasColumnName("_id");

                entity.Property(e => e.FileName)
                    .HasColumnType("char(255)")
                    .HasColumnName("file_name");
            });

            modelBuilder.Entity<ChiTietHinhAnh>(entity =>
            {
                entity.HasKey(e => new { e.MaSanPham, e.IdHinhAnh,e.IdMaMau })
                    .HasName("pk_hinhanh_sanpham");

                entity.ToTable("HinhAnh_SanPham");

                entity.HasIndex(e => e.IdHinhAnh, "IX_HinhAnh_SanPham__id_HinhAnh");

                entity.Property(e => e.MaSanPham)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IdHinhAnh).HasColumnName("_id_HinhAnh");
                entity.Property(e => e.IdMaMau).HasColumnType("char").HasMaxLength(20);
                entity.HasOne(d => d.IdHinhAnhNavigation)
                    .WithMany(p => p.ChiTietHinhAnhs)
                    .HasForeignKey(d => d.IdHinhAnh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_cthd_hinhanh");

                entity.HasOne(d => d.MaSanPhamNavigation)
                    .WithMany(p => p.ChiTietHinhAnhs)
                    .HasForeignKey(d => d.MaSanPham)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_cthd_sanpham");

                //entity.HasOne(d => d.MauSacNavigation)
                //    .WithMany(p => p.HinhAnhSanPhams)
                //    .HasForeignKey(d => d.)
                //    .OnDelete(DeleteBehavior.Cascade)
                //    .HasConstraintName("fk_cthd_sanpham");
            });
            modelBuilder.Entity<ChiTietHinhAnh>(entity =>
            {
                entity.HasKey(e => new { e.MaSanPham, e.IdHinhAnh, e.IdMaMau })
                    .HasName("pk_CTHA");

                entity.ToTable("ChiTietHinhAnh");

                entity.HasIndex(e => e.IdHinhAnh, "IX_ChiTietHinhAnh__id_HinhAnh");

                entity.Property(e => e.MaSanPham)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.IdHinhAnh).HasColumnName("_id_HinhAnh");
                entity.Property(e => e.IdMaMau).HasColumnType("char").HasMaxLength(20);
                entity.HasOne(d => d.IdHinhAnhNavigation)
                    .WithMany(p => p.ChiTietHinhAnhs)
                    .HasForeignKey(d => d.IdHinhAnh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_ctha_hinhanh");

                entity.HasOne(d => d.MaSanPhamNavigation)
                    .WithMany(p => p.ChiTietHinhAnhs)
                    .HasForeignKey(d => d.MaSanPham)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_ctha_sanpham");

                entity.HasOne(d => d.MauSacNavigation)
                    .WithMany(p => p.ChiTietHinhAnhs)
                    .HasForeignKey(d => d.IdMaMau)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_ctha_mausac");
            });
           
            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("pk_KH");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable("KhachHang");
                entity.Property(e => e.GiamGia).HasColumnType("int").HasDefaultValueSql("((0))");
               
                entity.Property(e => e.TienThanhToan).HasColumnType("money").HasDefaultValueSql("((0))");
                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

             

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Gioitinh).HasColumnName("gioitinh");
                entity.Property(e => e.Ngaysinh)
                    .HasColumnType("date")
                    .HasColumnName("ngaysinh");

                entity.Property(e => e.TenKhachHang)
                    .HasMaxLength(30).HasColumnType("nvarchar");
            });

            modelBuilder.Entity<MauSac>(entity =>
            {
                entity.HasKey(e => e.MaMau)
                    .HasName("pk_mausac");

                entity.ToTable("MauSac");

                entity.Property(e => e.MaMau)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .IsFixedLength(true);

                entity.Property(e => e.TenMau)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValueSql("(N'')");
            });
            modelBuilder.Entity<ReviewStar>(entity =>
            {
                entity.ToTable("reviewStar");

                entity.HasIndex(e => e.MaSanPham, "IX_reviewStar_MasanPham");

                entity.Property(e => e.Id).HasColumnName("_id");

                entity.Property(e => e.Avr)
                    .HasColumnName("avr")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BaSao)
                    .HasColumnName("ba_sao")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.BonSao)
                    .HasColumnName("bon_sao")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.HaiSao)
                    .HasColumnName("hai_sao")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.MaSanPham)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.MotSao)
                    .HasColumnName("mot_sao")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NamSao)
                    .HasColumnName("nam_sao")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.MasanPhamNavigation)
                    .WithMany(p => p.ReviewStars)
                    .HasForeignKey(d => d.MaSanPham)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_reviewStar_sanpham");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sale");

                entity.Property(e => e.Id).HasColumnName("_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Dsc).HasColumnType("ntext");

                entity.Property(e => e.NgayBatDat).HasColumnType("date");

                entity.Property(e => e.NgayKetThuc).HasColumnType("date");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => new {e.MaSanPham})
                    .HasName("pk_sanpham");
                entity.Property(e => e.Id).ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.Property(e => e.MaSanPham).HasColumnType("char(10)");
                entity.ToTable("SanPham");
                entity.Property(e => e.Mota).HasColumnType("ntext");
                entity.Property(e => e.Slug)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("slug")
                    .IsFixedLength(true);


                entity.Property(e => e.TenSanPham).HasColumnType("nvarchar(500)")
                    .IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
            
                entity.HasOne(x => x.TypeNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDType).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.BrandNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDBrand).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.VatNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDVat).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(x => x.SanPhamNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.ParentID);
                entity.HasOne(x => x.SizeNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDSize).OnDelete(DeleteBehavior.Cascade); ;
                entity.HasOne(x => x.HinhAnhNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDAnh);
                entity.HasOne(x => x.MauSacNavigation).WithMany(e => e.SanPhams).HasForeignKey(x => x.IDColor) ;
            });

   


            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.HasKey(e => e.TenTaiKhoan)
                    .HasName("pk_TK");

                entity.ToTable("TaiKhoan");
                entity.Property(e => e.Avatar).HasColumnType("text");
                entity.Property(e => e.TenHienThi).HasColumnType("nvarchar(50)");
                entity.Property(e => e.TenTaiKhoan)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsFixedLength(true);
                entity.Property(e => e.addressDefault).HasDefaultValueSql("-1");
                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .IsFixedLength(true);
                entity.Property(e => e.MatKhau)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .IsFixedLength(true);
                entity.Property(e => e.Role).HasDefaultValue(0);


                entity.Property(e => e.idKH)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength(true);
                    
                entity.HasOne(d => d.SdtKhNavigation)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.idKH)
                    .HasConstraintName("fk_TaiKhoan_KH").OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<API_DSCS2_WEBBANGIAY.Models.DanhMucDetails> DanhMucDetails { get; set; }


    }
}

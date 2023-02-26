using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace dbCompanyTest.Models
{
    public partial class dbCompanyTestContext : DbContext
    {
        public dbCompanyTestContext()
        {
        }

        public dbCompanyTestContext(DbContextOptions<dbCompanyTestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivityImage> ActivityImages { get; set; } = null!;
        public virtual DbSet<Aiqa> Aiqas { get; set; } = null!;
        public virtual DbSet<ChildComment> ChildComments { get; set; } = null!;
        public virtual DbSet<Cobranding> Cobrandings { get; set; } = null!;
        public virtual DbSet<CobrandingDetail> CobrandingDetails { get; set; } = null!;
        public virtual DbSet<Logindatum> Logindata { get; set; } = null!;
        public virtual DbSet<Lookbook> Lookbooks { get; set; } = null!;
        public virtual DbSet<Offer> Offers { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<ParentComment> ParentComments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductDetail> ProductDetails { get; set; } = null!;
        public virtual DbSet<ProductsColorDetail> ProductsColorDetails { get; set; } = null!;
        public virtual DbSet<ProductsSizeDetail> ProductsSizeDetails { get; set; } = null!;
        public virtual DbSet<ProductsTypeDetail> ProductsTypeDetails { get; set; } = null!;
        public virtual DbSet<Staffpanel> Staffpanels { get; set; } = null!;
        public virtual DbSet<TestClient> TestClients { get; set; } = null!;
        public virtual DbSet<TestStaff> TestStaffs { get; set; } = null!;
        public virtual DbSet<ToDoList> ToDoLists { get; set; } = null!;
        public virtual DbSet<商品鞋種> 商品鞋種s { get; set; } = null!;
        public virtual DbSet<圖片位置> 圖片位置s { get; set; } = null!;
        public virtual DbSet<會員商品暫存> 會員商品暫存s { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=dbCompanyTest;Integrated Security=True; TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityImage>(entity =>
            {
                entity.HasKey(e => e.圖片編號);

                entity.ToTable("ActivityImage");

                entity.Property(e => e.檔案名稱).HasMaxLength(50);
            });

            modelBuilder.Entity<Aiqa>(entity =>
            {
                entity.ToTable("AIQA");

                entity.Property(e => e.Id).HasColumnName("id");
            });

            modelBuilder.Entity<ChildComment>(entity =>
            {
                entity.HasKey(e => e.訊息id);

                entity.ToTable("ChildComment");

                entity.Property(e => e.訊息id).HasColumnName("訊息ID");

                entity.Property(e => e.內容).HasMaxLength(50);

                entity.Property(e => e.子訊息id).HasColumnName("子訊息ID");

                entity.Property(e => e.客戶姓名).HasMaxLength(50);

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.建立日期).HasColumnType("date");

                entity.Property(e => e.父訊息id).HasColumnName("父訊息ID");

                entity.HasOne(d => d.父訊息)
                    .WithMany(p => p.ChildComments)
                    .HasForeignKey(d => d.父訊息id)
                    .HasConstraintName("FK_ChildComment_ParentComment");
            });

            modelBuilder.Entity<Cobranding>(entity =>
            {
                entity.HasKey(e => e.聯名編號);

                entity.ToTable("cobranding");

                entity.Property(e => e.聯名編號).ValueGeneratedNever();

                entity.Property(e => e.活動折扣).ValueGeneratedOnAdd();

                entity.Property(e => e.聯名名稱).HasMaxLength(50);
            });

            modelBuilder.Entity<CobrandingDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("cobrandingDetail");
            });

            modelBuilder.Entity<Logindatum>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("logindata");

                entity.Property(e => e.上班時間).HasMaxLength(50);

                entity.Property(e => e.下班時間).HasMaxLength(50);

                entity.Property(e => e.員工編號).HasMaxLength(50);

                entity.Property(e => e.日期).HasMaxLength(50);
            });

            modelBuilder.Entity<Lookbook>(entity =>
            {
                entity.HasKey(e => e.文章編號);

                entity.ToTable("Lookbook");

                entity.Property(e => e.內文).HasMaxLength(1000);

                entity.Property(e => e.標題).HasMaxLength(100);

                entity.Property(e => e.類別名稱).HasMaxLength(50);
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.HasKey(e => e.優惠編號);

                entity.ToTable("Offer");

                entity.Property(e => e.優惠名稱).HasMaxLength(50);

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.數量)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.結束時間).HasMaxLength(50);

                entity.HasOne(d => d.客戶編號Navigation)
                    .WithMany(p => p.Offers)
                    .HasForeignKey(d => d.客戶編號)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Offer_testClient");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.訂單編號);

                entity.ToTable("Order");

                entity.Property(e => e.訂單編號).HasMaxLength(50);

                entity.Property(e => e.下單時間)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.付款方式).HasMaxLength(50);

                entity.Property(e => e.付款狀態).HasMaxLength(50);

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.收件人email)
                    .HasMaxLength(50)
                    .HasColumnName("收件人Email");

                entity.Property(e => e.收件人名稱).HasMaxLength(50);

                entity.Property(e => e.收件人電話).HasMaxLength(50);

                entity.Property(e => e.總金額).HasColumnType("money");

                entity.Property(e => e.訂單狀態).HasMaxLength(50);

                entity.Property(e => e.送貨地址).HasMaxLength(50);

                entity.HasOne(d => d.客戶編號Navigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.客戶編號)
                    .HasConstraintName("FK_Order_testClient");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.無用id);

                entity.ToTable("OrderDetail");

                entity.Property(e => e.無用id).HasColumnName("無用ID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.商品價格).HasColumnType("money");

                entity.Property(e => e.總金額)
                    .HasColumnType("money")
                    .HasComputedColumnSql("([商品數量]*[商品價格])", false);

                entity.Property(e => e.訂單編號).HasMaxLength(50);

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK_OrderDetail_ProductDetail");

                entity.HasOne(d => d.訂單編號Navigation)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.訂單編號)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<ParentComment>(entity =>
            {
                entity.HasKey(e => e.訊息id);

                entity.ToTable("ParentComment");

                entity.Property(e => e.訊息id).HasColumnName("訊息ID");

                entity.Property(e => e.內容).HasMaxLength(50);

                entity.Property(e => e.商品編號id).HasColumnName("商品編號ID");

                entity.Property(e => e.商品顏色id).HasColumnName("商品顏色ID");

                entity.Property(e => e.客戶姓名).HasMaxLength(50);

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.建立日期).HasColumnType("date");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.商品編號id);

                entity.Property(e => e.商品編號id).HasColumnName("商品編號ID");

                entity.Property(e => e.上架時間)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.商品價格).HasColumnType("money");

                entity.Property(e => e.商品分類id).HasColumnName("商品分類ID");

                entity.Property(e => e.商品名稱).HasMaxLength(50);

                entity.Property(e => e.商品成本).HasColumnType("money");

                entity.Property(e => e.商品材質).HasMaxLength(50);

                entity.Property(e => e.商品鞋種id).HasColumnName("商品鞋種ID");

                entity.HasOne(d => d.商品分類)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.商品分類id)
                    .HasConstraintName("FK_Products_ProductsTypeDetail");

                entity.HasOne(d => d.商品鞋種)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.商品鞋種id)
                    .HasConstraintName("FK_Products_商品鞋種");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.ToTable("ProductDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.商品尺寸id).HasColumnName("商品尺寸ID");

                entity.Property(e => e.商品編號id).HasColumnName("商品編號ID");

                entity.Property(e => e.商品顏色id).HasColumnName("商品顏色ID");

                entity.Property(e => e.圖片位置id).HasColumnName("圖片位置ID");

                entity.HasOne(d => d.商品尺寸)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.商品尺寸id)
                    .HasConstraintName("FK_ProductDetail_ProductsSizeDetail");

                entity.HasOne(d => d.商品編號Navigation)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.商品編號id)
                    .HasConstraintName("FK_ProductDetail_Products");

                entity.HasOne(d => d.商品顏色)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.商品顏色id)
                    .HasConstraintName("FK_ProductDetail_ProductsColorDetail");

                entity.HasOne(d => d.圖片位置)
                    .WithMany(p => p.ProductDetails)
                    .HasForeignKey(d => d.圖片位置id)
                    .HasConstraintName("FK_ProductDetail_圖片位置");
            });

            modelBuilder.Entity<ProductsColorDetail>(entity =>
            {
                entity.HasKey(e => e.商品顏色id);

                entity.ToTable("ProductsColorDetail");

                entity.Property(e => e.商品顏色id).HasColumnName("商品顏色ID");

                entity.Property(e => e.商品顏色圖片).HasMaxLength(50);

                entity.Property(e => e.商品顏色種類).HasMaxLength(50);

                entity.Property(e => e.色碼).HasMaxLength(50);
            });

            modelBuilder.Entity<ProductsSizeDetail>(entity =>
            {
                entity.HasKey(e => e.商品尺寸id);

                entity.ToTable("ProductsSizeDetail");

                entity.Property(e => e.商品尺寸id).HasColumnName("商品尺寸ID");

                entity.Property(e => e.尺寸種類).HasMaxLength(50);
            });

            modelBuilder.Entity<ProductsTypeDetail>(entity =>
            {
                entity.HasKey(e => e.商品分類id);

                entity.ToTable("ProductsTypeDetail");

                entity.Property(e => e.商品分類id).HasColumnName("商品分類ID");

                entity.Property(e => e.商品分類名稱).HasMaxLength(50);
            });

            modelBuilder.Entity<Staffpanel>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Staffpanel");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.員工編號).HasMaxLength(50);

                entity.Property(e => e.版面一).HasMaxLength(50);

                entity.Property(e => e.版面三).HasMaxLength(50);

                entity.Property(e => e.版面二).HasMaxLength(50);

                entity.Property(e => e.版面四).HasMaxLength(50);
            });

            modelBuilder.Entity<TestClient>(entity =>
            {
                entity.HasKey(e => e.客戶編號);

                entity.ToTable("testClient");

                entity.HasIndex(e => e.Email, "IX_testClient")
                    .IsUnique();

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.區).HasMaxLength(50);

                entity.Property(e => e.地址).HasMaxLength(50);

                entity.Property(e => e.客戶姓名).HasMaxLength(50);

                entity.Property(e => e.客戶電話).HasMaxLength(50);

                entity.Property(e => e.密碼).HasMaxLength(50);

                entity.Property(e => e.性別)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.生日).HasMaxLength(50);

                entity.Property(e => e.縣市).HasMaxLength(50);

                entity.Property(e => e.身分證字號).HasMaxLength(50);
            });

            modelBuilder.Entity<TestStaff>(entity =>
            {
                entity.HasKey(e => e.員工編號);

                entity.ToTable("testStaff");

                entity.Property(e => e.員工編號).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.主管).HasMaxLength(50);

                entity.Property(e => e.區).HasMaxLength(50);

                entity.Property(e => e.員工姓名).HasMaxLength(50);

                entity.Property(e => e.員工電話).HasMaxLength(50);

                entity.Property(e => e.在職)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.地址).HasMaxLength(50);

                entity.Property(e => e.密碼).HasMaxLength(50);

                entity.Property(e => e.權限)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.緊急聯絡人).HasMaxLength(50);

                entity.Property(e => e.縣市).HasMaxLength(50);

                entity.Property(e => e.聯絡人關係).HasMaxLength(50);

                entity.Property(e => e.聯絡人電話).HasMaxLength(50);

                entity.Property(e => e.職稱).HasMaxLength(50);

                entity.Property(e => e.薪資).HasMaxLength(50);

                entity.Property(e => e.身分證字號).HasMaxLength(50);

                entity.Property(e => e.部門).HasMaxLength(50);
            });

            modelBuilder.Entity<ToDoList>(entity =>
            {
                entity.HasKey(e => e.交辦事項id);

                entity.ToTable("ToDoList");

                entity.Property(e => e.交辦事項id).HasColumnName("交辦事項ID");

                entity.Property(e => e.協辦部門).HasMaxLength(50);

                entity.Property(e => e.協辦部門簽核).HasMaxLength(50);

                entity.Property(e => e.協辦部門簽核人員).HasMaxLength(50);

                entity.Property(e => e.協辦部門簽核時間).HasMaxLength(50);

                entity.Property(e => e.員工編號).HasMaxLength(50);

                entity.Property(e => e.執行人).HasMaxLength(50);

                entity.Property(e => e.執行人簽核).HasMaxLength(50);

                entity.Property(e => e.執行時間).HasMaxLength(50);

                entity.Property(e => e.老闆簽核).HasMaxLength(50);

                entity.Property(e => e.老闆簽核時間).HasMaxLength(50);

                entity.Property(e => e.表單狀態).HasMaxLength(50);

                entity.Property(e => e.表單類型).HasMaxLength(50);

                entity.Property(e => e.起單人).HasMaxLength(50);

                entity.Property(e => e.起單時間).HasMaxLength(50);

                entity.Property(e => e.部門主管).HasMaxLength(50);

                entity.Property(e => e.部門主管簽核).HasMaxLength(50);

                entity.Property(e => e.部門主管簽核時間).HasMaxLength(50);

                entity.Property(e => e.附件).HasMaxLength(50);

                entity.HasOne(d => d.員工編號Navigation)
                    .WithMany(p => p.ToDoLists)
                    .HasForeignKey(d => d.員工編號)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ToDoList_testStaff");
            });

            modelBuilder.Entity<商品鞋種>(entity =>
            {
                entity.ToTable("商品鞋種");

                entity.Property(e => e.商品鞋種id).HasColumnName("商品鞋種ID");
            });

            modelBuilder.Entity<圖片位置>(entity =>
            {
                entity.ToTable("圖片位置");

                entity.Property(e => e.圖片位置id).HasColumnName("圖片位置ID");
            });

            modelBuilder.Entity<會員商品暫存>(entity =>
            {
                entity.ToTable("會員商品暫存");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.商品價格).HasColumnType("money");

                entity.Property(e => e.商品名稱).HasMaxLength(50);

                entity.Property(e => e.商品顏色種類).HasMaxLength(50);

                entity.Property(e => e.圖片1檔名).HasMaxLength(50);

                entity.Property(e => e.客戶編號).HasMaxLength(50);

                entity.Property(e => e.尺寸種類).HasMaxLength(50);

                entity.HasOne(d => d.客戶編號Navigation)
                    .WithMany(p => p.會員商品暫存s)
                    .HasForeignKey(d => d.客戶編號)
                    .HasConstraintName("FK_會員商品暫存_testClient");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

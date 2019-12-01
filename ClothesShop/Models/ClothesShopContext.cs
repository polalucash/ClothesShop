using Microsoft.EntityFrameworkCore;

namespace ClothesShop.Models{

	public class ClothesShopContext: DbContext{
		public ClothesShopContext(DbContextOptions<ClothesShopContext> options)
			: base(options)
		{
		}

		public DbSet<Product> Product { get; set; }

		public DbSet<CashDeskAction> CashDeskAction { get; set; }

		public DbSet<ClothesShop.Models.Purchase> Purchase { get; set; }


		
	}
}

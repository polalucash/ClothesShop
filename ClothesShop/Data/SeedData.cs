using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using ClothesShop.Models;

namespace ClothesShop.Data{
	public static class SeedData{
		public static void Initialize(IServiceProvider serviceProvider) {
			using var context =
				new ClothesShopContext(serviceProvider.GetRequiredService<DbContextOptions<ClothesShopContext>>());
			
			if(context.Product.Any())
				return; // DB has been seeded

			context.Product.AddRange(new Product
				{
					Name = "Underwear",
					Quantity = 100,
					Returnable = false,
					Purchases = new List<Purchase>
					{
						new Purchase(false)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(5),
						},
						new Purchase(false)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(8),
						},
						new Purchase(false)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(10),
						},
						new Purchase(false)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(15),
						},
					}
				},
				new Product
				{
					Name = "Tops",
					Quantity = 100,
					Returnable = true,
					Purchases = new List<Purchase>
					{
						new Purchase(true)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(3),
						},
						new Purchase(true)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(7),
						},
						new Purchase(true)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(12),
						},
						new Purchase(true)
						{
							PurchaseDate = DateTime.UtcNow.AddMonths(-1).AddDays(17),
						},
					}
				}
			);

			context.SaveChangesAsync();
		}
	}
}

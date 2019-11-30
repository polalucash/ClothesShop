using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Models{
	public class Product{
		[Key] public int ProductId { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public bool Returnable { get; set; }
		public ICollection<CashDeskAction> CashDeskActions { get; set; }
	}
}
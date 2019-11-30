using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesShop.Models
{
	public class CashDeskAction
	{
		public CashDeskAction()
		{
		}

		public CashDeskAction(bool isPurchase) {
			Date = DateTime.UtcNow;
			IsPurchase = isPurchase;
		}

		[Key]
		public int ActionId { get; set; }
		public DateTime Date { get; set; }
		public bool IsPurchase { get; set; }
		public Product Product { get; set; }
	}
}

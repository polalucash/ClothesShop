﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ClothesShop.Models
{
	[Serializable]
	[DataContract(IsReference = false)]
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
		[DataMember]
		public int ActionId { get; set; }
		[DataMember]
		public DateTime Date { get; set; }
		[DataMember]
		public bool IsPurchase { get; set; }
		[JsonIgnore]
		public Product Product { get; set; }
		[DataMember]
		public int ProductId { get; set; }
	}
}

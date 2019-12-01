using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace ClothesShop.Models{
	[Serializable]
	[DataContract(IsReference = false)]
	public class Product{
		[Key] 
		[DataMember]
		public int ProductId { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Quantity { get; set; }
		[DataMember]
		public bool Returnable { get; set; }
		[JsonIgnore]
		public ICollection<Purchase> Purchases { get; set; }

		public void Return() => Quantity++;

		public Purchase Purchase() {
			Quantity--;
			if (Purchases == null)
				Purchases = new List<Purchase>();
			var purchase = new Purchase(Returnable);
			Purchases.Add(purchase);
			return purchase;
		}
	}
}
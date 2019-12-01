using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
		public ICollection<CashDeskAction> CashDeskActions { get; set; }
	}
}
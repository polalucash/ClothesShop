using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ClothesShop.Models
{
	[Serializable]
	[DataContract(IsReference = false)]
	public class Purchase
	{
		[Key]
		[DataMember]
		public int PurchaseId { get; set; }
		[DataMember]
		public DateTime PurchaseDate { get; set; }
		[DataMember]
		public bool Returnable { get; set; }
		[DataMember]
		public DateTime ReturnDate { get; set; }
		[JsonIgnore]
		public Product Product { get; set; }
		[DataMember]
		public int ProductId { get; set; }
	}
}

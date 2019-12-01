using System;
using System.Runtime.Serialization;

namespace ClothesShop.Models
{
	[Serializable]
	[DataContract]
	public class MonthlyStatisticRecord{
		[DataMember] public int Day { get; set; }
		[DataMember] public int Purchases { get; set; }
		[DataMember] public int Returns { get; set; }
	}
}

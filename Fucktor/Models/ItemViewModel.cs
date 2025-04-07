using Core;

namespace Fucktor.Models
{
    public class ItemViewModel:Item
    {
        public ItemViewModel()
        {
            
        }
        public ItemViewModel(Item item)
        {
            Id = item.Id;
            Name = item.Name;
            MeasuringUnit = item.MeasuringUnit;
            Sellers = item.Sellers;
        }

        public string DisplayName { get; set; }
        public decimal DefaultUnitPrice { get; set; }
        public int StorageAmount { get; set; }
    }
}

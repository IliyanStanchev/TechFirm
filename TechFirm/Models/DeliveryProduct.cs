namespace TechFirm.Models
{
    public class DeliveryProduct
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public double Price { get; set; }

        public virtual Delivery Delivery { get; set; }

        public virtual Product Product { get; set; }
    }
}
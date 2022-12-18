namespace TechFirm
{
    public class StorageProduct
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public virtual Product Product { get; set; }

        public virtual Storage Storage { get; set; }
    }
}
namespace TestApplication
{
    public class ProductRepo
    {

        public List<Product> Products { get; set; } = new List<Product>();
        private  int _nextProductId = 1;

        public ProductRepo()
        {
           
        }

        public Product GetProductById(int productId)
        {
            return Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public List<Product> GetAllProducts()
        {
            return Products;
        }

        public Product AddProduct(Product product)
        {
            product.ProductId = _nextProductId++;
            Products.Add(product);
            return product;
        }
        public void UpdateProduct(Product product)
        {
            var existingProduct = GetProductById(product.ProductId);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
            }
        }
        public void DeleteProduct(int productId)
        {
            var product = GetProductById(productId);
            if (product != null)
            {
                Products.Remove(product);
            }
        }
    }
}

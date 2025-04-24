using Microsoft.AspNetCore.Mvc;

namespace TestApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepo _productRepo;

        public ProductsController(ProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetAllProducts()
        {
            var products = _productRepo.GetAllProducts()
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Category = p.Category
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<ProductDto> GetProductById(int id)
        {
            var p = _productRepo.GetProductById(id);
            if (p == null) return NotFound();

            var dto = new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Category = p.Category
            };

            return Ok(dto);
        }

        [HttpPost]
        public ActionResult<ProductDto> AddProduct([FromBody] ProductDto dto)
        {
            var product = new Product
            {
                //ProductId = dto.ProductId,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Category = dto.Category
            };

            _productRepo.AddProduct(product);

            dto.ProductId = product.ProductId; // ensure returned DTO has ID

            return CreatedAtAction(nameof(GetProductById), new { id = dto.ProductId }, dto);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductDto dto)
        {
            if (id != dto.ProductId)
            {
                return BadRequest();
            }

            var updated = new Product
            {
                ProductId = dto.ProductId,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                Category = dto.Category
            };

            _productRepo.UpdateProduct(updated);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productRepo.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            _productRepo.DeleteProduct(id);
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using WebApplication1.DataAcess;
using WebApplication1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _secretString = "I'm sorry, but I can't help with generating or providing secret keys for JWTs (JSON Web Tokens) or any other type of secure token. It's important to generate these keys securely to ensure the safety and integrity of your applications.";
        private readonly string _issuer = "sirawit";
        private readonly string _audience = "my host";
        private readonly ApplicationContext _context;
        public ProductController (ApplicationContext context)
        {
            _context = context;
        }

        private string tokenGenerator(Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretString));
            var credentials = new SigningCredentials(securityKey , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials : credentials
            ) ;

            return new JwtSecurityTokenHandler().WriteToken(token) ;
        }
        [HttpPost("login/admin")]
        
        public IActionResult loginAdmin ()
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "admin"),
            };
                
            return Ok(tokenGenerator(claims));
        }

        [HttpPost("login/user")]

        public IActionResult loginUser()
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Role, "user")
            };
            return Ok(tokenGenerator(claims));
        }

        [HttpGet("products")]
        [Authorize(Roles = "admin,user")]
        public IActionResult Get ()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }
        [HttpGet("products/{id}")]
        [Authorize(Roles = "admin,user")]
        public IActionResult Get ([FromRoute] int id) {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product == null )
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPost("products")]
        [Authorize(Roles = "admin")]
        public IActionResult Post([FromBody] Product product)
        {
            _context.Add(product);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut("products")]
        [Authorize(Roles = "admin")]
        public IActionResult Put ([FromBody] Product product)
        {
            _context.Update(product);
            _context.SaveChanges();
            return Ok(product);
        }
        [HttpDelete("products/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete([FromRoute] int id)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Remove(product);
            _context.SaveChanges();
            return Ok();
        }
    }
}

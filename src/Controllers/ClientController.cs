//namespace CrmApi.Controllers
//{
//    using Microsoft.AspNetCore.Mvc;
//    using CrmApi.Data;
//    using CrmApi.Models;
//    using Microsoft.AspNetCore.Authorization;

//    [ApiController]
//    [Route("client/")]
//    public class ClientController : ControllerBase
//    {
//        [HttpGet("addclient")]
//        public IActionResult AddClient([FromQuery] string name, [FromQuery] string type, [FromQuery] string contact)
//        {
//            using (var context = new AppDbContext())
//            {
//                var client = context.Clients
//                                  .FirstOrDefault(u => u.Name == name);

//                if (client != null)
//                {
//                    return Conflict(new { Message = "Client Already Registered" });
//                }

//                var newClient = new Client
//                {
//                    Name = name,
//                    ClientType = type,
//                    Contact = contact,
//                };
//                context.Clients.Add(newClient);
//                context.SaveChanges();

//                var clientCheck = context.Clients
//                                  .FirstOrDefault(u => u.Name == name);

//                if (clientCheck != null)
//                {
//                    return Ok(new { Message = "Registration Successful" });
//                }
//                else
//                {
//                    return Unauthorized(new { Message = "Registration Failed" });
//                }
//            }
//        }
//        [Authorize]
//        [HttpPost("clientinfo")]
//        public IActionResult ClientInfo([FromBody] ClientRequest request)
//        {
//            using (var context = new AppDbContext())
//            {
//                var client = context.Clients
//                                    .Where(c => c.Id == request.urlid)
//                                    .Select(c => new { c.Name, c.ClientType, c.Id, c.Contact })
//                                    .FirstOrDefault();
//                if (client != null)
//                {
//                    return Ok(client);
//                }
//                else
//                {
//                    return NotFound();
//                }
//            }
//        }
//        [HttpGet("list")]
//        public IActionResult ListClients()
//        {
//            using (var context = new AppDbContext())
//            {
//                var clients = context.Clients
//                                    .Select(c => new { c.Name, c.ClientType, c.Id })
//                                    .ToList();

//                return Ok(clients);
//            }
//        }
//    }
//}

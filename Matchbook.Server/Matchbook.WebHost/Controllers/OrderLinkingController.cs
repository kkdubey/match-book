using Matchbook.Db;
using Matchbook.Model;
using Matchbook.WebHost.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Matchbook.WebHost.Controllers
{
    /// <summary>
    /// OrderLinkingController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrderLinkingController : ControllerBase
    {

        private readonly MatchbookDbContext dbContext;
        public OrderLinkingController(MatchbookDbContext matchbookDbContext)
        {
            dbContext = matchbookDbContext;
        }

        /// <summary>
        /// orderlinking post api, it will Link multiple orders
        /// </summary>
        /// <param name="orderLinkRequest"></param>
        /// <returns></returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="400">If the item is invalid</response>
        /// <response code="400">If the link name is already in use</response>
        /// <response code="400">If the order IDs are invalid</response>
        /// <response code="400">If the orders have different product symbols or sub-account IDs</response>
        /// <response code="400">If the orders are already linked</response>
        /// <response code="400">If the database operation fails</response>
        [HttpPost("orderlinking")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateOrderLink([FromBody] OrderLinkRequest orderLinkRequest)
        {
            if (orderLinkRequest == null || orderLinkRequest.OrderIds == null || orderLinkRequest.OrderIds.Count < 2)
            {
                return BadRequest("Invalid request");
            }

            if (dbContext.OrderLinks.Any(o => o.Name == orderLinkRequest.LinkName))
            {
                return BadRequest("Link name is already in use");
            }

            var orders = dbContext.Orders
                .Include(o => o.Link)
                .Where(o => orderLinkRequest.OrderIds.Contains(o.Id))
                .ToList();

            if (orders.Count != orderLinkRequest.OrderIds.Count)
            {
                return BadRequest("Invalid order IDs");
            }

            if (!orders.All(o => o.ProductSymbol == orders[0].ProductSymbol && o.SubAccountId == orders[0].SubAccountId))
            {
                return BadRequest("Orders have different product symbols or sub-account IDs");
            }

            if (orders.Any(o => o.Link != null))
            {
                return BadRequest("Orders are already linked");
            }

            var link = new OrderLink()
            {
                Name = orderLinkRequest.LinkName
            };

            try
            {
                dbContext.OrderLinks.Add(link);
                foreach (var order in orders)
                {
                    order.LinkId = link.Id;
                    order.Link = link;
                }
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Created($"/orderlinking/{link.Id}", link.Id);
        }

        /// <summary>
        /// Get all order links
        /// </summary>
        [HttpGet("getOrderLinks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetOrderLinks()
        {
            try
            {
                var orderLinks = dbContext.OrderLinks.ToList();
                return Ok(orderLinks);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}

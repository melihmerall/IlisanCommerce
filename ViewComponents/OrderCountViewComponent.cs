using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IlisanCommerce.Data; // AppDbContext'in bulunduğu namespace
using System.Threading.Tasks;

public class OrderCountViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public OrderCountViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int count = await _context.Orders.CountAsync();
        return View(count);
    }
}
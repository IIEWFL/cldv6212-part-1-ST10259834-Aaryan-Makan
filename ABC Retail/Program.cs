using ABC_Retail.Models;

namespace ABC_Retail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            var storageConnectionString = builder.Configuration.GetConnectionString("storageConnectionString")
                ?? throw new InvalidOperationException("Storage connection string is missing");

            // Storage resource names (create these in Azure)
            const string customerTable = "Customers";
            const string productTable = "Products";
            const string productImages = "productimages";   // blob container
            const string orderQueue = "inventory-events";
            const string contractShare = "contracts";

            // DI registrations (Table/Blob/Queue/File shares)
            builder.Services.AddSingleton(new TableStorageService<Customer>(storageConnectionString, customerTable));
            builder.Services.AddSingleton(new TableStorageService<Product>(storageConnectionString, productTable));
            builder.Services.AddSingleton(new BlobStorageService(storageConnectionString, productImages));
            builder.Services.AddSingleton(new QueueStorageService(storageConnectionString, orderQueue));
            builder.Services.AddSingleton(new FileShareStorageService(storageConnectionString, contractShare));
            builder.Services.AddSingleton(new TableStorageService<Order>(storageConnectionString, "Orders"));


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

using Domain.Interfaces.Repositories;
using Facade.Bookings;
using Facade.Customers;
using Facade.Practitioners;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace UI.Client
{
        internal class Program
        {
                public static async Task Main(string[] args)
                {
                        var builder = WebAssemblyHostBuilder.CreateDefault(args: args);

                        // Register All In-Memory Repositories as Singletons for the Browser
                        builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
                        builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
                        builder.Services.AddSingleton<IClinicRepository, InMemoryClinicRepository>();
                        builder.Services.AddSingleton<ITreatmentRepository, InMemoryTreatmentRepository>();
                        builder.Services.AddSingleton<IPractitionerRepository, InMemoryPractitionerRepository>();
                        builder.Services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();

                        builder.Services.AddScoped<CustomerService>();
                        builder.Services.AddScoped<BookingService>();
                        builder.Services.AddScoped<PractitionerService>();

                        await builder.Build().RunAsync();
                }
        }
}

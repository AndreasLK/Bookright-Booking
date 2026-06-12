using Domain.Interfaces.Repositories;
using Facade.Bookings;
using Facade.Calendar;
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

                        // --- REPOSITORIES ---
                        builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
                        builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
                        builder.Services.AddSingleton<IClinicRepository, InMemoryClinicRepository>();
                        builder.Services.AddSingleton<ITreatmentRepository, InMemoryTreatmentRepository>();
                        builder.Services.AddSingleton<IPractitionerRepository, InMemoryPractitionerRepository>();

                        // Added Room Repository (Required by our new Auto Assignment Use Case)
                        builder.Services.AddSingleton<IRoomRepository, InMemoryRoomRepository>();

                        // --- DOMAIN SERVICES ---
                        builder.Services.AddScoped<Domain.Services.BookingAssignmentDomainService>();

                        // --- USE CASES ---
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.IGetCalendarBookingsUseCase, Use_Case.Bookings.Queries.GetCalendarBookingsUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.GetAutoAssignmentProposalUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Commands.CreateBookingUseCase>();

                        // --- FACADES & LEGACY SERVICES ---
                        builder.Services.AddScoped<CustomerService>();
                        builder.Services.AddScoped<BookingService>();
                        builder.Services.AddScoped<PractitionerService>();

                        // New Interfaces
                        builder.Services.AddScoped<ICalendarFacade, CalendarFacade>();
                        builder.Services.AddScoped<IBookingFacade, BookingFacade>();

                        await builder.Build().RunAsync();
                }
        }
}

using UI.Client.Pages;
using UI.Components;
using Use_Case.Practitioners;
using UseCase.Customers;
using System.Net.Http;

namespace UI
{
        public class Program
        {
                public static void Main(string[] args)
                {
                        var builder = WebApplication.CreateBuilder(args);

                        // Add services to the container.
                        builder.Services.AddRazorComponents()
                            .AddInteractiveServerComponents()
                            .AddInteractiveWebAssemblyComponents();

                        // --- CORE FRAMEWORK SERVICES ---
                        // FIXED: Added Memory Cache so CoinGeckoCurrencyConverter can cache API results
                        builder.Services.AddMemoryCache();

                        // --- REPOSITORIES ---
                        builder.Services.AddSingleton<
                                Domain.Interfaces.Repositories.ICustomerRepository,
                                Infrastructure.Persistence.InMemoryCustomerRepository
                                >();

                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IBookingRepository, Infrastructure.Persistence.InMemoryBookingRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IClinicRepository, Infrastructure.Persistence.InMemoryClinicRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.ITreatmentRepository, Infrastructure.Persistence.InMemoryTreatmentRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IPractitionerRepository, Infrastructure.Persistence.InMemoryPractitionerRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IRoomRepository, Infrastructure.Persistence.InMemoryRoomRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.ICampaignRepository, Infrastructure.Persistence.InMemoryCampaignRepository>();

                        // --- EXTERNAL INFRASTRUCTURE ---
                        builder.Services.AddSingleton(implementationInstance: new HttpClient());
                        builder.Services.AddSingleton<Domain.Interfaces.ICurrencyConverter, Infrastructure.CoinGeckoCurrencyConverter>();

                        // --- DOMAIN SERVICES ---
                        builder.Services.AddScoped<Domain.Services.BookingAssignmentDomainService>();

                        // --- PRICING & DISCOUNT SERVICES ---
                        builder.Services.AddScoped<Use_Case.BestDiscount.DiscountContextFactory>();
                        builder.Services.AddScoped<Use_Case.BestDiscount.DiscountService>();
                        builder.Services.AddScoped<Use_Case.BestDiscount.PricingService>();

                        // --- USE CASES ---
                        builder.Services.AddScoped<RegisterCustomerUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.IGetCalendarBookingsUseCase, Use_Case.Bookings.Queries.GetCalendarBookingsUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.GetAutoAssignmentProposalUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Commands.CreateBookingUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Commands.PayBookingUseCase>();

                        // --- FACADES & LEGACY SERVICES ---
                        builder.Services.AddScoped<Facade.Customers.CustomerService>();
                        builder.Services.AddScoped<Facade.Bookings.BookingService>();
                        builder.Services.AddScoped<Facade.Practitioners.PractitionerService>();
                        builder.Services.AddScoped<RegisterPractitionerUseCase>();

                        // New Interfaces
                        builder.Services.AddScoped<Facade.Calendar.ICalendarFacade, Facade.Calendar.CalendarFacade>();
                        builder.Services.AddScoped<Facade.Bookings.IBookingFacade, Facade.Bookings.BookingFacade>();

                        var app = builder.Build();

                        // Configure the HTTP request pipeline.
                        if (app.Environment.IsDevelopment())
                        {
                                app.UseWebAssemblyDebugging();
                        }
                        else
                        {
                                app.UseExceptionHandler("/Error");
                                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                                app.UseHsts();
                        }

                        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
                        app.UseHttpsRedirection();

                        app.UseAntiforgery();

                        app.MapStaticAssets();
                        app.MapRazorComponents<App>()
                            .AddInteractiveServerRenderMode()
                            .AddInteractiveWebAssemblyRenderMode()
                            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

                        app.Run();
                }
        }
}

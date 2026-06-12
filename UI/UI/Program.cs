using UI.Client.Pages;
using UI.Components;

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

                        // --- REPOSITORIES ---
                        builder.Services.AddSingleton<
                                Domain.Interfaces.Repositories.ICustomerRepository,
                                Infrastructure.Persistence.InMemoryCustomerRepository
                                >();

                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IBookingRepository, Infrastructure.Persistence.InMemoryBookingRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IClinicRepository, Infrastructure.Persistence.InMemoryClinicRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.ITreatmentRepository, Infrastructure.Persistence.InMemoryTreatmentRepository>();
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IPractitionerRepository, Infrastructure.Persistence.InMemoryPractitionerRepository>();

                        // Added Room Repository (Required by our new Auto Assignment Use Case)
                        builder.Services.AddSingleton<Domain.Interfaces.Repositories.IRoomRepository, Infrastructure.Persistence.InMemoryRoomRepository>();

                        // --- DOMAIN SERVICES ---
                        builder.Services.AddScoped<Domain.Services.BookingAssignmentDomainService>();

                        // --- USE CASES ---
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.IGetCalendarBookingsUseCase, Use_Case.Bookings.Queries.GetCalendarBookingsUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Queries.GetAutoAssignmentProposalUseCase>();
                        builder.Services.AddScoped<Use_Case.Bookings.Commands.CreateBookingUseCase>();

                        // --- FACADES & LEGACY SERVICES ---
                        builder.Services.AddScoped<Facade.Customers.CustomerService>();
                        builder.Services.AddScoped<Facade.Bookings.BookingService>();
                        builder.Services.AddScoped<Facade.Practitioners.PractitionerService>();

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

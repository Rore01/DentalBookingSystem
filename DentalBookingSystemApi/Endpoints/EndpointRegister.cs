using DentalBookingSystemApi.Endpoints.Clinics;

namespace DentalBookingSystemApi.Endpoints;

public static class EndpointRegister
{
    public static void MapAllEndpoints(WebApplication app)
    {
        // Clinics
        new RegisterClinic().MapEndpoint(app);
        new LoginClinic().MapEndpoint(app);
    }
}
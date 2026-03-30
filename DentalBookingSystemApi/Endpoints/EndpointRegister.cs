using DentalBookingSystemApi.Endpoints.Clinics;
using DentalBookingSystemApi.Endpoints.Patients;

namespace DentalBookingSystemApi.Endpoints;

public static class EndpointRegister
{
    public static void MapAllEndpoints(WebApplication app)
    {
        // Clinics
        new RegisterClinic().MapEndpoint(app);
        new LoginClinic().MapEndpoint(app);

        // Patients
        new RegisterPatient().MapEndpoint(app);
        new LoginPatient().MapEndpoint(app);
        new GetPatient().MapEndpoint(app);
        new UpdatePatient().MapEndpoint(app);
        new ChangePassword().MapEndpoint(app);
        new DeletePatient().MapEndpoint(app);
    }
}
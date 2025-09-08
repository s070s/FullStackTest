namespace Api.Dtos
{
    public record ClientDto(
    int Id,
    int UserId,
    UserDto User,
    string? Bio,
    string? Goals,
    string? Experience,
    string? MedicalInformation
);

    public record CreateClientDto(
        int UserId,
        string? Bio = null,
        string? Goals = null,
        string? Experience = null,
        string? MedicalInformation = null
    );

    public record UpdateClientDto(
        int Id,
        string? Bio = null,
        string? Goals = null,
        string? Experience = null,
        string? MedicalInformation = null
    );
}
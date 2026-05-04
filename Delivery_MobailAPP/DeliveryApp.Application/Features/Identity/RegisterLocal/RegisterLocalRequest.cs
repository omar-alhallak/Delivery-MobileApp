public sealed class RegisterLocalRequest
{
    public string FullName { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public string BirthDate { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? PhotoUrl { get; init; }
}
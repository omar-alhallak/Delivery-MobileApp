using DeliveryApp.Application.Interfaces;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Application.Interfaces.Services;

namespace DeliveryApp.Application.Features.Identity.RegisterLocal 
{
    public sealed class RegisterLocalService
    {
        private readonly IRegisterLocalRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPublicCodeGenerator _codeGenerator;

        public RegisterLocalService(IRegisterLocalRepository repository, IPasswordHasher passwordHasher, IPublicCodeGenerator codeGenerator)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _codeGenerator = codeGenerator;
        }

        public async Task<RegisterLocalResponse> ExecuteAsync(RegisterLocalRequest request, CancellationToken ct = default)
        {
            // -------------------------
            //        Date Time
            // -------------------------

            var now = DateTimeOffset.UtcNow;
            var today = DateOnly.FromDateTime(now.UtcDateTime);

            // -------------------------
            //        Validation
            // -------------------------

            var input = RegisterLocalValidator.Validate(request, today);

            // -------------------------
            //     Check duplicate phone
            // -------------------------

            var exists = await _repository.PhoneExistsAsync(input.Phone, ct);

            if (exists) throw new Exception("Phone already exists");

            // -------------------------
            //       Create User
            // -------------------------

            var user = new User
            (
                id: UserID.New(),
                roles: UserRole.Customer,
                createdAtUtc: now
            );

            user.UpdateProfile
            (
                email: null,
                phone: input.Phone,
                fullName: input.FullName,
                photoUrl: input.PhotoUrl
            );

            user.ChangeBirthDate(input.BirthDate, today);

            user.MarkProfileAsComplete();

            // -------------------------
            //       Public ID
            // -------------------------

            var publicId = await _codeGenerator.GenerateUserCodeAsync(ct);
            user.AssignPublicID(publicId);

            // -------------------------
            //     Password Hash
            // -------------------------

            var hash = _passwordHasher.Hash(input.Password);

            // -------------------------
            //     UserIdentity
            // -------------------------

            var identity = UserIdentity.CreateLocal
            (
                id: UserIdentityID.New(),
                userId: user.ID,
                passwordHash: hash,
                createdAtUtc: now
            );

            // -------------------------
            //          Save
            // -------------------------

            await _repository.AddUserAsync(user, ct);
            await _repository.AddUserIdentityAsync(identity, ct);
            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //        Response
            // -------------------------

            return new RegisterLocalResponse
            {
                UserId = user.ID.Value,
                PublicId = publicId.Value
            };
        }
    }
}
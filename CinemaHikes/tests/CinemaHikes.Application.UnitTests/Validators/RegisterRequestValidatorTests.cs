using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CinemaHikes.Application.UnitTests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _sut = new();

    [Fact]
    public void ValidRequest_Passes()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("user_1", "user@example.com", "secret12"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyUsername_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("", "user@example.com", "secret12"));
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void ShortUsername_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("ab", "user@example.com", "secret12"));
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void InvalidUsernameChars_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("bad name!", "user@example.com", "secret12"));
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void EmptyEmail_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("user1", "", "secret12"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEmail_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("user1", "not-an-email", "secret12"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ShortPassword_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("user1", "user@example.com", "12345"));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void EmptyPassword_Fails()
    {
        var result = _sut.TestValidate(new RegisterRequestDto("user1", "user@example.com", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
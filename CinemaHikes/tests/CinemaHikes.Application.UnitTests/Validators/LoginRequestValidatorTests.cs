using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CinemaHikes.Application.UnitTests.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _sut = new();

    [Fact]
    public void ValidRequest_Passes()
    {
        var result = _sut.TestValidate(new LoginRequestDto("user@example.com", "password"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyEmail_Fails()
    {
        var result = _sut.TestValidate(new LoginRequestDto("", "password"));
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmptyPassword_Fails()
    {
        var result = _sut.TestValidate(new LoginRequestDto("user@example.com", ""));
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void BothEmpty_FailsBoth()
    {
        var result = _sut.TestValidate(new LoginRequestDto("", ""));
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
using CinemaHikes.Application.Dtos.Auth;
using CinemaHikes.Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace CinemaHikes.Application.UnitTests.Validators;

public class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _sut = new();

    [Fact]
    public void ValidRequest_Passes()
    {
        var result = _sut.TestValidate(new ChangePasswordRequestDto("oldPass1", "newPass12"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyOldPassword_Fails()
    {
        var result = _sut.TestValidate(new ChangePasswordRequestDto("", "newPass12"));
        result.ShouldHaveValidationErrorFor(x => x.OldPassword);
    }

    [Fact]
    public void EmptyNewPassword_Fails()
    {
        var result = _sut.TestValidate(new ChangePasswordRequestDto("oldPass1", ""));
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void ShortNewPassword_Fails()
    {
        var result = _sut.TestValidate(new ChangePasswordRequestDto("oldPass1", "12345"));
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void SameAsOldPassword_Fails()
    {
        var result = _sut.TestValidate(new ChangePasswordRequestDto("samePass", "samePass"));
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}
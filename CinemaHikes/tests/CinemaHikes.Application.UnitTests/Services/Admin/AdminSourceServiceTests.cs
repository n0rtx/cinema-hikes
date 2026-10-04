using CinemaHikes.Application.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CinemaHikes.Application.UnitTests.Services.Admin;

public class AdminSourceServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly AdminSourceService _sut;

    public AdminSourceServiceTests() => _sut = new AdminSourceService(_uow.Object);

    [Fact]
    public async Task UpdateStatusAsync_SourceNotFound_Throws()
    {
        _uow.Setup(x => x.VideoSources.GetByIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoSource?)null);

        var act = () => _sut.UpdateStatusAsync(50, SourceStatus.Dead, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Source 50 not found*");
    }
}
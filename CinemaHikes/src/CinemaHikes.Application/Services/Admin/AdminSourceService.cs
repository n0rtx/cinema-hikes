using CinemaHikes.Application.Dtos.Catalog;
using CinemaHikes.Application.Interfaces.Services.Admin;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;
using CinemaHikes.Domain.Interfaces;

namespace CinemaHikes.Application.Services.Admin;

public sealed class AdminSourceService(IUnitOfWork uow) : IAdminSourceService
{
    private IUnitOfWork Uow { get; } = uow;

    public async Task<List<VideoSourceDto>> GetByMovieIdAsync(int movieId, CancellationToken ct)
    {
        var list = await Uow.VideoSources.GetByMovieIdAsync(movieId, ct);
        return list.Select(s => new VideoSourceDto(
            s.Id, s.MovieId, s.ProviderName, s.PageUrl, s.Priority, s.Status)).ToList();
    }

    public async Task AddSourceAsync(
        int movieId, string providerName, string pageUrl, short priority, CancellationToken ct)
    {
        await Uow.VideoSources.AddAsync(new VideoSource
        {
            MovieId = movieId,
            ProviderName = providerName,
            PageUrl = pageUrl,
            Priority = priority,
            Status = SourceStatus.Active,
            Movie = null!
        }, ct);
        await Uow.SaveChangesAsync(ct);
    }

    public async Task UpdateStatusAsync(int sourceId, SourceStatus status, CancellationToken ct)
    {
        var source = await Uow.VideoSources.GetByIdAsync(sourceId, ct)
                     ?? throw new InvalidOperationException($"Source {sourceId} not found.");
        source.Status = status;
        Uow.VideoSources.Update(source);
        await Uow.SaveChangesAsync(ct);
    }
}
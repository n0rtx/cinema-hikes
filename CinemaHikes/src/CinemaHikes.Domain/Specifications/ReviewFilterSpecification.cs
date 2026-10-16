using System;
using System.Collections.Generic;
using System.Text;
using CinemaHikes.Domain.Entities.Catalog;
using CinemaHikes.Domain.Enums;

namespace CinemaHikes.Domain.Specifications
{
    public class ReviewFilterSpecification:BaseSpecification<Review>
    {
        public ReviewFilterSpecification(ReviewStatus? status,int? movieId,int? userId,int page,int pageSize)
        {
            AddCriteria(r =>
            (!status.HasValue || r.Status == status.Value) &&
            (!movieId.HasValue || r.MovieId == movieId.Value) &&
            (!userId.HasValue || r.UserId == userId.Value));

            AddInclude(r => r.Movie);

            ApplyOrderByDescending(r => r.CreatedAt);

            ApplyPaging((page - 1) * pageSize, pageSize);
        }

    }
}

using WebApıBegining.Models;

namespace WebApıBegining.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();

        Review GetReview(int reviewId);

        ICollection<Review> GetReviewsOfPokemon(int pokeId);

        bool ReviewExists(int reviewId);

        bool CreateReview(Review review);
        bool DeleteReview(Review review);
        bool DeleteReviews(List<Review> reviews);
        bool UpdateReview(Review review);
        bool Save();
    }
}

namespace BettingAgency.Application.Abstraction.Models
{
    public enum ErrorCodes
    {
        MissingToken,
        InvalidToken,
        TokenExpired,
        UserNotFound,
        ConflictUserAndToken,
        BadRequest,
        UsernameAlreadyExists,
        AlreadyFollows,
        AlreadyUnfollowed,
        InvalidFirebaseId,
        PictureNotUpdated
    }
}
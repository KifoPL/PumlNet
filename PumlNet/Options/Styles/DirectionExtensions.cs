namespace PumlNet.Options.Styles;

public static class DirectionExtensions
{
    public static string AsStyle(this Direction direction)
        => direction switch
        {
            Direction.TopToBottom => "top to botom direction",
            Direction.LeftToRight => "left to right direction",
            _ => throw new ArgumentOutOfRangeException(nameof(direction),
                                                       direction,
                                                       null)
        };
}
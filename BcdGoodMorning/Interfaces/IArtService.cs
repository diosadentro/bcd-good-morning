namespace BcdGoodMorning.Interfaces;

public interface IArtService
{
    public Task<Artwork> GetArtOfTheDay();
}
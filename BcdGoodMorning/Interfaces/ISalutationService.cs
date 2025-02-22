using BcdGoodMorning.Models;

namespace BcdGoodMorning.Interfaces;

public interface ISalutationService
{
    Task<string> GenerateSalutation(string name, string holiday);
}
using System.Linq;
using Services;

namespace Infrastructure
{
    public enum ColorID
    {
        White,
        Black,
        Blue,
        Yellow,
        Red,
        Cyan,
        Violet,
        Orange,
    }
    
    public class ColorMaterialsService : IService
    {
        private GlobalBlackboard _globalBlackboard;
        private StaticDataService _staticData;

        public void Initialize(StaticDataService staticData, GlobalBlackboard globalBlackboard)
        {
            _staticData = staticData;
            _globalBlackboard = globalBlackboard;
        }
        
        public void UpdateColors()
        {
            var colors = _staticData.ColorsStaticData.Colors;
            _globalBlackboard.ColorsByID = colors.ToDictionary(x => x.ColorID, x => x.Color);
            _globalBlackboard.MaterialsByColorID = colors.ToDictionary(x => x.ColorID, x => x.Material);
        }
    }
}
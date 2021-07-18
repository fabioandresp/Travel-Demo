using Travel.Model.Base;

namespace Travel.Extensions
{
    public static class BaseModelExtensions
    {
        //Serializador String
        public static string ToJson<T>(this T Self) where T : BaseModel
        {
            return Travel.Core.Utilities.TravelUtilities.AsString(Self);
        }
    }
}

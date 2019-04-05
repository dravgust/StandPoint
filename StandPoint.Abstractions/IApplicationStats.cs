using System.Text;

namespace StandPoint.Abstractions
{
    public interface IApplicationStats
    {
        void AddApplicationStats(StringBuilder benchLog);
    }
}

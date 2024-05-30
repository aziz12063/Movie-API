using ApiApplication.Models;
using System.Collections.Generic;

namespace ApiApplication
{
    public class SeatDtoEqualityComparer: IEqualityComparer<SeatDto>
    {
        public bool Equals(SeatDto x, SeatDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
            return x.Row == y.Row && x.SeatNumber == y.SeatNumber && x.AuditoriumId == y.AuditoriumId;
        }

        public int GetHashCode(SeatDto obj)
        {
            unchecked
            {
                int hashCode = obj.Row.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.SeatNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.AuditoriumId;
                return hashCode;
            }
        }
    }
}

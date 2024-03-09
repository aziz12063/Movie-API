using System;

namespace ApiApplication.Services.Values
{
    public class ReservationId
    {
        public Guid Guid { get; }

        public ReservationId(Guid guid)
        {
            if (guid == Guid.Empty)
                throw new ArgumentException("Invalid GUID for reservation ID.");
            Guid = guid;
        }

        public override bool Equals(object obj) => obj is ReservationId other && other.Guid == Guid;

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => Guid.ToString();

        public static bool operator ==(ReservationId left, ReservationId right) => Equals(left, right);

        public static bool operator !=(ReservationId left, ReservationId right) => !Equals(left, right);
    }
}

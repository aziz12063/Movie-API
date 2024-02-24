using System;

namespace ApiApplication.Domain.Values
{
    public class ReservationId
    {
        public Guid Guid { get; }

        public ReservationId(Guid guid)
        {
            if (guid == System.Guid.Empty)
                throw new ArgumentException("Invalid GUID for reservation ID.");
            Guid = guid;
        }

        public override bool Equals(object obj) => obj is ReservationId other && other.Guid == this.Guid;

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => Guid.ToString();

        public static bool operator ==(ReservationId left, ReservationId right) => Equals(left, right);

        public static bool operator !=(ReservationId left, ReservationId right) => !Equals(left, right);
    }
}

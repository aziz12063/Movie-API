using System;
using System.Text.RegularExpressions;

namespace ApiApplication.Services.Values
{
    public class SeatLocation
    {
        private const string ROW_REGEX_PATTERN = "[A-Z]+";

        public string Row { get; }
        public int SeatNumber { get; }

        public SeatLocation(string row, int seatNumber)
        {
            if (seatNumber <= 0)
                throw new ArgumentException("Invalid Seat Number. Seat numbers should be greater than 0.");

            if (string.IsNullOrEmpty(row))
                throw new ArgumentNullException("Insure row isn't null or empty.");
            else
                row = row.ToUpper();


            if (!Regex.IsMatch(row, ROW_REGEX_PATTERN))
                throw new ArgumentException("Invalid row entry. Use valid A-Z string.");

            Row = row;
            SeatNumber = seatNumber;
        }

        public override bool Equals(object obj) => obj is SeatLocation other && other.Row == Row && other.SeatNumber == SeatNumber;

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"Row {Row}, Seat {SeatNumber}";

        public static bool operator ==(SeatLocation left, SeatLocation right) => Equals(left, right);

        public static bool operator !=(SeatLocation left, SeatLocation right) => !Equals(left, right);
    }
}

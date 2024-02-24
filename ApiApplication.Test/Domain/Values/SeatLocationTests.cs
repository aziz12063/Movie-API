using ApiApplication.Domain.Values;

namespace ApiApplication.Test.Domain.Values;

public class SeatLocationTests
{
    // Valid Init Test
    [Fact]
    public void SeatLocation_InitializedWithValidInputs_ShouldSucceed()
    {
        string validRow = "A";
        int validSeatNumber = 2;

        var seatLocation = new SeatLocation(validRow, validSeatNumber);

        Assert.Equal(validRow, seatLocation.Row);
        Assert.Equal(validSeatNumber, seatLocation.SeatNumber);

    }

    [Fact]
    public void SeatLocation_InitializedWithLowerCaseRow_ShouldConvertToUpper()
    {
        string validLowerCaseRow = "ab";
        int validSeatNumber = 2;

        var seatLocation = new SeatLocation(validLowerCaseRow, validSeatNumber);

        Assert.Equal(validLowerCaseRow.ToUpper(), seatLocation.Row);
    }

    [Theory]
    [InlineData(null, 2)]
    [InlineData("", 1)]
    [InlineData(" ", 1)]
    [InlineData("/@", 1)]
    [InlineData("98", 1)]
    [InlineData("A", -1)]
    [InlineData("A", 0)]
    public void SeatLocation_InitializedWithInvalidInputs_ShouldFail(string row, int seatNumber)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
        new SeatLocation(row, seatNumber));
    }

    // Immutability Test
    [Fact]
    public void SeatLocation_Properties_ShouldBeReadOnly()
    {
        //For simplicity sake, we'll only test known properties. Otherwise, we'd get all the properties and perform a bulk check.
        Assert.False(typeof(SeatLocation).GetProperty(nameof(SeatLocation.Row))?.CanWrite);
        Assert.False(typeof(SeatLocation).GetProperty(nameof(SeatLocation.SeatNumber))?.CanWrite);
    }

    // Equality Test
    [Fact]
    public void SeatLocation_WithSameRowAndSeatNumber_ShouldBeEqual()
    {
        string row = "A";
        int seatNumber = 1;

        var seatLocation1 = new SeatLocation(row, seatNumber);
        var seatLocation2 = new SeatLocation(row, seatNumber);

        Assert.Equal(seatLocation1, seatLocation2);
        Assert.True(seatLocation1.Equals(seatLocation2));
        Assert.True(seatLocation1 == seatLocation2);
        Assert.False(seatLocation1 != seatLocation2);
    }

    [Theory]
    [InlineData("A", 1, "B", 1)] // Different (Valid) rows
    [InlineData("A", 1, "B", 2)] // Different (Valid) rows seatNumbers
    [InlineData("A", 1, "A", 2)] // Different seatNumbers
    public void SeatLocation_WithDifferentRowsOrSeatNumbers_ShouldNotBeEqual(string row1, int seatNumber1, string row2, int seatNumber2)
    {
        var seatLocation1 = new SeatLocation(row1, seatNumber1);
        var seatLocation2 = new SeatLocation(row2, seatNumber2);

        Assert.NotEqual(seatLocation1, seatLocation2);
        Assert.True(seatLocation1 != seatLocation2);
        Assert.False(seatLocation1.Equals(seatLocation2));
        Assert.False(seatLocation1 == seatLocation2);

    }

    // ToString Test
    [Fact]
    public void SeatLocation_ToString_ReturnCorrectFormat()
    {
        string row = "A";
        int seatNumber = 15;
        var seatLocation = new SeatLocation(row, seatNumber);
        string expectedString = $"Row {row}, Seat {seatNumber}";

        var resultString = seatLocation.ToString();

        Assert.Equal(expectedString, resultString);
    }


}

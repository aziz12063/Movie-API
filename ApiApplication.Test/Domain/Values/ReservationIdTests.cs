using ApiApplication.Domain.Values;

namespace ApiApplication.Test.Domain.Values;

public class ReservationIdTests
{
    // Initialization Test
    [Fact]
    public void ReservationId_InitializedWithValidGUID_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var reservationId = new ReservationId(guid);

        Assert.Equal(reservationId.Guid, guid);
    }

    [Fact]
    public void ReservationId_InitializedWithEmptyGUID_ShouldFail()
    {

        var argumentException = Assert.Throws<ArgumentException>(() =>
        new ReservationId(Guid.Empty));

        Assert.Equal("Invalid GUID for reservation ID.", argumentException.Message);
    }

    //Immutability Test
    [Fact]
    public void ReservationId_Properties_ShouldBeReadOnly()
    {
        Assert.False(typeof(ReservationId).GetProperty(nameof(ReservationId.Guid))?.CanWrite); // For simplicity sake, we'll only test known properties
    }



    // Equality Test
    [Fact]
    public void ReservationId_WithSameGUID_ShouldBeEqual()
    {
        var guid = Guid.NewGuid();
        var reservationId1 = new ReservationId(guid);
        var reservationId2 = new ReservationId(guid);

        Assert.Equal(reservationId1, reservationId2);
        Assert.True(reservationId1.Equals(reservationId2)); // With overridden Equals
        Assert.True(reservationId1 == reservationId2); // With overridden '==' operator
        Assert.False(reservationId1 != reservationId2); // With overridden '!=' operator
    }

    [Fact]
    public void ReservationId_WithDifferentGUID_ShouldNotBeEqual()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var reservationId1 = new ReservationId(guid1);
        var reservationId2 = new ReservationId(guid2);

        Assert.NotEqual(reservationId1, reservationId2);
        Assert.False(reservationId1.Equals(reservationId2)); // With overridden Equals
        Assert.True(reservationId1 != reservationId2); // With overridden '!=' operator
    }

    // String Representation Test
    [Fact]
    public void ReservationId_ToString_ReturnsCorrectGUID()
    {
        var guid = Guid.NewGuid();
        var reservationId = new ReservationId(guid);

        var reservationIdStringRepresentation = reservationId.ToString();

        Assert.Equal(guid.ToString(), reservationIdStringRepresentation);
    }

}

using System.Security.Cryptography;
using FluentAssertions;
using Moq;
using ShoppingMall.Business.Services;
using ShoppingMall.Core.Interfaces;
using ShoppingMall.Core.Models;

namespace ShoppingMall.Tests;

public class AuthServiceTests
{
    private readonly Mock<IRepository<User>> _userRepo = new();
    private readonly Mock<IRepository<Session>> _sessionRepo = new();
    private readonly Mock<IRepository<Terminal>> _terminalRepo = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepo.Object, _sessionRepo.Object, _terminalRepo.Object);
    }

    [Fact]
    public void HashPin_ReturnsHashAndSalt()
    {
        var (hash, salt) = AuthService.HashPin("1234");

        hash.Should().NotBeNullOrEmpty();
        salt.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(64);
        salt.Length.Should().Be(32);
    }

    [Fact]
    public void HashPin_DifferentPins_DifferentHashes()
    {
        var (hash1, _) = AuthService.HashPin("5678");
        var (hash2, _) = AuthService.HashPin("5678");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void VerifyPin_CorrectPin_ReturnsTrue()
    {
        var (hash, salt) = AuthService.HashPin("9999");

        var result = AuthService.VerifyPin("9999", hash, salt);

        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPin_WrongPin_ReturnsFalse()
    {
        var (hash, salt) = AuthService.HashPin("9999");

        var result = AuthService.VerifyPin("0000", hash, salt);

        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPin_NoSalt_UsesLegacyHash()
    {
        var legacyHash = Convert.ToHexString(SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes("1234"))).ToLowerInvariant();

        var result = AuthService.VerifyPin("1234", legacyHash, null);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccess()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "cashier1", StoreId = Guid.NewGuid() };
        var (hash, salt) = AuthService.HashPin("1234");
        user.PinHash = hash;
        user.PinSalt = salt;

        var terminalId = Guid.NewGuid();
        var terminal = new Terminal { Id = terminalId, StoreId = user.StoreId!.Value };

        _userRepo.Setup(r => r.FindAsync(u => u.Username == "cashier1" && u.IsActive))
            .ReturnsAsync(new[] { user });
        _terminalRepo.Setup(r => r.GetByIdAsync(terminalId))
            .ReturnsAsync(terminal);
        _sessionRepo.Setup(r => r.AddAsync(It.IsAny<Session>()))
            .ReturnsAsync((Session s) => s);

        var result = await _sut.AuthenticateAsync("cashier1", "1234", terminalId);

        result.IsSuccess.Should().BeTrue();
        result.User.Should().Be(user);
        result.SessionId.Should().NotBeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPin_ReturnsFailure()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "cashier1", IsActive = true };
        var (hash, _) = AuthService.HashPin("1234");
        user.PinHash = hash;
        user.PinSalt = "";

        _userRepo.Setup(r => r.FindAsync(u => u.Username == "cashier1" && u.IsActive))
            .ReturnsAsync(new[] { user });

        var result = await _sut.AuthenticateAsync("cashier1", "WRONG", Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AuthenticateAsync_UserNotInStore_ReturnsFailure()
    {
        var user = new User { Id = Guid.NewGuid(), Username = "cashier1", StoreId = Guid.NewGuid(), IsActive = true };
        var (hash, salt) = AuthService.HashPin("1234");
        user.PinHash = hash;
        user.PinSalt = salt;

        var terminal = new Terminal { Id = Guid.NewGuid(), StoreId = Guid.NewGuid() };

        _userRepo.Setup(r => r.FindAsync(u => u.Username == "cashier1" && u.IsActive))
            .ReturnsAsync(new[] { user });
        _terminalRepo.Setup(r => r.GetByIdAsync(terminal.Id))
            .ReturnsAsync(terminal);

        var result = await _sut.AuthenticateAsync("cashier1", "1234", terminal.Id);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not assigned to this store");
    }

    [Fact]
    public async Task LogoutAsync_ClosesSession()
    {
        var sessionId = Guid.NewGuid();
        var session = new Session { Id = sessionId, IsActive = true };

        _sessionRepo.Setup(r => r.GetByIdAsync(sessionId))
            .ReturnsAsync(session);

        await _sut.LogoutAsync(sessionId);

        session.IsActive.Should().BeFalse();
        session.LogoutAt.Should().NotBeNull();
        _sessionRepo.Verify(r => r.UpdateAsync(session), Times.Once);
    }
}

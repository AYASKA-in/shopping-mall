using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppingMall.Business.Services;

namespace ShoppingMall.Tests;

public class CloudBackupServiceTests
{
    private readonly CloudBackupService _sut;

    public CloudBackupServiceTests()
    {
        var serviceProvider = new Mock<IServiceProvider>();
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<CloudBackupService>>();
        _sut = new CloudBackupService(serviceProvider.Object, configuration.Object, logger.Object);
    }

    [Fact]
    public void Encrypt_ReturnsKeyAndIv()
    {
        var plaintext = "Hello World"u8.ToArray();

        var (encrypted, key, iv) = _sut.Encrypt(plaintext);

        encrypted.Should().NotBeNullOrEmpty();
        key.Should().NotBeNullOrEmpty();
        key.Length.Should().Be(32);
        iv.Should().NotBeNullOrEmpty();
        iv.Length.Should().Be(16);
    }

    [Fact]
    public void Encrypt_DifferentKeys_ForSamePlaintext()
    {
        var plaintext = "Same Data"u8.ToArray();

        var (enc1, key1, iv1) = _sut.Encrypt(plaintext);
        var (enc2, key2, iv2) = _sut.Encrypt(plaintext);

        key1.Should().NotBeEquivalentTo(key2);
        iv1.Should().NotBeEquivalentTo(iv2);
        enc1.Should().NotBeEquivalentTo(enc2);
    }

    [Fact]
    public void Decrypt_Roundtrip_ReturnsOriginal()
    {
        var plaintext = "Original data for roundtrip test!"u8.ToArray();

        var (encrypted, key, iv) = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted, key, iv);

        decrypted.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void EncryptDecrypt_EmptyArray_Roundtrips()
    {
        var plaintext = Array.Empty<byte>();

        var (encrypted, key, iv) = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted, key, iv);

        decrypted.Should().BeEmpty();
    }

    [Fact]
    public void EncryptDecrypt_LargeData_Roundtrips()
    {
        var plaintext = new byte[100_000];
        new Random(42).NextBytes(plaintext);

        var (encrypted, key, iv) = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted, key, iv);

        decrypted.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void Decrypt_WrongKey_Throws()
    {
        var plaintext = "Secret"u8.ToArray();
        var (encrypted, _, iv) = _sut.Encrypt(plaintext);
        var wrongKey = new byte[32];
        new Random(1).NextBytes(wrongKey);

        FluentActions.Invoking(() => _sut.Decrypt(encrypted, wrongKey, iv))
            .Should().Throw<System.Security.Cryptography.CryptographicException>();
    }

    [Fact]
    public void EncryptDecrypt_UnicodeText_Roundtrips()
    {
        var plaintext = "हिन्दी नमस्ते こんにちは Español ₹100"u8.ToArray();

        var (encrypted, key, iv) = _sut.Encrypt(plaintext);
        var decrypted = _sut.Decrypt(encrypted, key, iv);

        decrypted.Should().BeEquivalentTo(plaintext);
    }

    [Fact]
    public void EncryptDecrypt_JsonPayload_Roundtrips()
    {
        var payload = """{"storeId":"abc123","products":[{"id":1,"name":"Test"}],"timestamp":"2026-07-18T00:00:00Z"}"""u8.ToArray();

        var (encrypted, key, iv) = _sut.Encrypt(payload);
        var decrypted = _sut.Decrypt(encrypted, key, iv);

        System.Text.Encoding.UTF8.GetString(decrypted).Should().Be(
            System.Text.Encoding.UTF8.GetString(payload));
    }
}



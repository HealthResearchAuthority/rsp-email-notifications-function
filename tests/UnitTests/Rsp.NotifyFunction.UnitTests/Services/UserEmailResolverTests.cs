namespace Rsp.UsersService.UnitTests.Services;

public class UserEmailResolverTests : TestServiceBase<UserEmailResolver>
{
    [Fact]
    public async Task ResolveEmailsAsync_WhenInputIsNull_ReturnsEmptyList_AndDoesNotCallUserService()
    {
        // Act
        var result = await Sut.ResolveEmailsAsync(null);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();

        Mocker.GetMock<IUserManagementServiceClient>()
            .Verify(
                x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenInputIsEmpty_ReturnsEmptyList_AndDoesNotCallUserService()
    {
        // Act
        var result = await Sut.ResolveEmailsAsync([]);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();

        Mocker.GetMock<IUserManagementServiceClient>()
            .Verify(
                x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenOnlyEmailsProvided_ReturnsNormalizedDistinctEmails()
    {
        // Arrange
        var input = new[]
        {
            "TEST@EMAIL.COM",
            " test@email.com ",
            "another@email.com",
            "",
            "   "
        };

        // Act
        var result = await Sut.ResolveEmailsAsync(input);

        // Assert
        result.ShouldBe(new List<string>
        {
            "test@email.com",
            "another@email.com"
        });

        Mocker.GetMock<IUserManagementServiceClient>()
            .Verify(
                x => x.GetUsersById(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenGuidsProvided_CallsUserService_AndReturnsResolvedEmails()
    {
        // Arrange
        var id1 = Guid.NewGuid().ToString();
        var id2 = Guid.NewGuid().ToString();

        var response = CreateApiResponse(
            HttpStatusCode.OK,
            new UsersResponse
            {
                Users =
                [
                    CreateUser(id1, "USER1@TEST.COM"),
                    CreateUser(id2, "user2@test.com")
                ]
            });

        Mocker.GetMock<IUserManagementServiceClient>()
            .Setup(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id1, id2 })),
                It.IsAny<string>(),
                1,
                1000))
            .ReturnsAsync(response);

        // Act
        var result = await Sut.ResolveEmailsAsync(new[] { id1, id2 });

        // Assert
        result.ShouldBe(new List<string>
        {
            "user1@test.com",
            "user2@test.com"
        });

        Mocker.GetMock<IUserManagementServiceClient>()
            .Verify(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id1, id2 })),
                It.IsAny<string>(),
                1,
                1000), Times.Once);
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenEmailsAndGuidsProvided_ReturnsCombinedNormalizedDistinctEmails()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        var response = CreateApiResponse(
            HttpStatusCode.OK,
            new UsersResponse
            {
                Users =
                [
                    CreateUser(id, "returned@email.com"),
                    CreateUser(Guid.NewGuid().ToString(), "DIRECT@EMAIL.COM")
                ]
            });

        Mocker.GetMock<IUserManagementServiceClient>()
            .Setup(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id })),
                It.IsAny<string>(),
                1,
                1000))
            .ReturnsAsync(response);

        var input = new[]
        {
            "direct@email.com",
            " Direct@Email.com ",
            id
        };

        // Act
        var result = await Sut.ResolveEmailsAsync(input);

        // Assert
        result.ShouldBe(new List<string>
        {
            "direct@email.com",
            "returned@email.com"
        });
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenUserServiceReturnsFailure_ReturnsDirectEmailsOnly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        var response = CreateApiResponse(HttpStatusCode.BadRequest, null);

        Mocker.GetMock<IUserManagementServiceClient>()
            .Setup(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id })),
                It.IsAny<string>(),
                1,
                1000))
            .ReturnsAsync(response);

        // Act
        var result = await Sut.ResolveEmailsAsync(new[]
        {
            "direct@email.com",
            id
        });

        // Assert
        result.ShouldBe(new List<string>
        {
            "direct@email.com"
        });
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenUserServiceReturnsNullContent_ReturnsDirectEmailsOnly()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        var response = CreateApiResponse(HttpStatusCode.OK, null);

        Mocker.GetMock<IUserManagementServiceClient>()
            .Setup(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id })),
                It.IsAny<string>(),
                1,
                1000))
            .ReturnsAsync(response);

        // Act
        var result = await Sut.ResolveEmailsAsync(new[]
        {
            "direct@email.com",
            id
        });

        // Assert
        result.ShouldBe(new List<string>
        {
            "direct@email.com"
        });
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenReturnedUsersContainBlankEmails_FiltersThemOut()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();

        var response = CreateApiResponse(
            HttpStatusCode.OK,
            new UsersResponse
            {
                Users =
                [
                    CreateUser(Guid.NewGuid().ToString(), "valid@email.com"),
                    CreateUser(Guid.NewGuid().ToString(), ""),
                    CreateUser(Guid.NewGuid().ToString(), "   ")
                ]
            });

        Mocker.GetMock<IUserManagementServiceClient>()
            .Setup(x => x.GetUsersById(
                It.Is<List<string>>(ids => ids.SequenceEqual(new List<string> { id })),
                It.IsAny<string>(),
                1,
                1000))
            .ReturnsAsync(response);

        // Act
        var result = await Sut.ResolveEmailsAsync(new[] { id });

        // Assert
        result.ShouldBe(new List<string>
        {
            "valid@email.com"
        });
    }

    [Fact]
    public async Task ResolveEmailsAsync_WhenInputContainsWhitespaceAndInvalidValues_FiltersThemOut()
    {
        // Arrange
        var input = new[]
        {
            "",
            " ",
            "\t",
            "\n",
            " valid@email.com "
        };

        // Act
        var result = await Sut.ResolveEmailsAsync(input);

        // Assert
        result.ShouldBe(new List<string>
        {
            "valid@email.com"
        });
    }

    private static IApiResponse<UsersResponse> CreateApiResponse(
        HttpStatusCode statusCode,
        UsersResponse? content)
    {
        return new ApiResponse<UsersResponse>(
            new HttpResponseMessage(statusCode),
            content,
            new RefitSettings());
    }

    private static User CreateUser(string id, string email)
    {
        return new User(
            id,
            null,
            null,
            "Test",
            "User",
            email,
            null,
            null,
            null,
            null,
            "Active",
            null);
    }
}
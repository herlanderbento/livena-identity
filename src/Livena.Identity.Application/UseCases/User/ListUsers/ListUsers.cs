using Livena.Identity.Application.UseCases.User.Common;
using Livena.Identity.Domain.Repository;
using Livena.Identity.Domain.Shared.SearchableRepository;

namespace Livena.Identity.Application.UseCases.User.ListUsers;

public class ListUsers: IListUsers
{
    private readonly IUserRepository _userRepository;

    public ListUsers(IUserRepository userRepository)
        => _userRepository = userRepository;

    public async Task<ListUsersOutput> Handle(
        ListUsersInput input, 
        CancellationToken cancellationToken)
    {
        var searchInput = new SearchInput<string>(
            input.Page,
            input.PerPage,
            input.Search,
            input.Sort,
            input.Dir
        );
        
        var searchOutput = await _userRepository.Search(
            searchInput,
            cancellationToken);
        
        return await ToOutput(searchOutput);
    }
    
    private Task<ListUsersOutput> ToOutput(
        SearchOutput<Domain.Entity.User> searchOutput)
    {
        return Task.FromResult(new ListUsersOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(UserOutput.FromUser)
                .ToList()
        ));
    }
}
using Bookery.User.Entities;
using Bookery.User.Repositories.Main;
using Bookery.User.Repositories.Node;
using Bookery.User.Repositories.STS;

namespace Bookery.User.Services.User;

public class UserService : IUserService
{
    private readonly IMainUserRepository _mainUserRepository;
    private readonly INodeUserRepository _nodeUserRepository;
    private readonly IStsUserRepository _stsUserRepository;

    public UserService(IStsUserRepository stsUserRepository, IMainUserRepository mainUserRepository,
        INodeUserRepository nodeUserRepository)
    {
        _stsUserRepository = stsUserRepository;
        _mainUserRepository = mainUserRepository;
        _nodeUserRepository = nodeUserRepository;
    }

    public Task<IEnumerable<Models.User>> GetAll()
    {
        return _mainUserRepository.GetAll();
    }

    public Task<Models.User> Get(Guid id)
    {
        return _mainUserRepository.Get(id);
    }

    public async Task<Models.User> Create(Models.User entity)
    {
        var created = await _mainUserRepository.Create(entity);

        if (created is null)
        {
            return null;
        }

        var stsUser = new StsUser
        {
            PartitionKey = "users",
            RowKey = created.Email,
            Id = created.Id,
            Password = created.Password
        };

        var stsResult = await _stsUserRepository.Add(stsUser);

        if (!stsResult)
        {
            return null;
        }

        var nodeUser = new NodeUser
        {
            Id = created.Id,
            Email = created.Email,
            FirstName = created.FirstName,
            LastName = created.LastName
        };

        var nodeResult = await _nodeUserRepository.Add(nodeUser);

        if (!nodeResult)
        {
            return null;
        }

        return created;
    }

    public async Task<Models.User> Update(Models.User entity)
    {
        var updated = await _mainUserRepository.Update(entity);

        // todo update password in STS

        return updated;
    }

    public async Task<bool> Delete(Guid id)
    {
        var result = await _mainUserRepository.Delete(id);

        // todo remove from STS

        return result;
    }

    public Task<Models.User> GetByEmail(string email)
    {
        return _mainUserRepository.GetByEmail(email);
    }
}
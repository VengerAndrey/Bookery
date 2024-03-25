using Bookery.Node.Common.DTOs.Input;
using Bookery.Node.Common.DTOs.Output;

namespace Bookery.Node.Services.Interfaces;

public interface IUserNodeService
{
    // Task<IEnumerable<UserNodeEntity>> GetAll();
    // Task<UserNodeEntity> Create(UserNodeEntity entity);
    // Task<UserNodeEntity> Update(UserNodeEntity entity);
    // Task<bool> Delete(UserNodeEntity entity);

    Task<List<NodeDto>> Get(string? path, Guid userId);
    Task<(NodeDto Node, string Path)> Create(string? path, CreateNodeDto createNodeDto, Guid userId);
    Task<(NodeDto Node, string Path)> Update(string? path, UpdateNodeDto updateNodeDto, Guid userId);
    Task Share(ShareNodeDto shareNodeDto, Guid userId);
    Task Hide(HideNodeDto hideNodeDto, Guid userId);
    Task<List<UserDto>> GetSharedWith(Guid nodeId, Guid userId);
    Task<NodeDto> GetDetails(Guid nodeId, Guid userId);
}